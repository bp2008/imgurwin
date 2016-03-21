using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace ImgurWin
{
	public class ImgurRequestManager
	{
		internal const string AppName = "ImgurWin";
		internal const string clientId = "dfffd93680eb891";
		internal const string clientSecret = "6c7be8d990c0b0b3d6033a4d4da625a4274a3221";
		internal const string baseUrl = "https://api.imgur.com/3/";
		internal const string authTokenExchangeUrl = "https://api.imgur.com/oauth2/token";

		private Main frmMain;
		private Thread thrWebRequests;
		private bool[] threadEnabledFlag;
		private ConcurrentQueue<AsyncCommandArgs> cmdQueue;
		private SemaphoreSlim waitHandle;

		/// <summary>
		/// Raised on the UI thread after a web response is received.
		/// </summary>
		public event Action<WebRequestCompletedEventArgs> ResponseReceived = delegate { };
		/// <summary>
		/// Raised on the UI thread when upload progress is changed.
		/// </summary>
		public event Action<int> UploadProgressChanged = delegate { };

		public ImgurRequestManager(Main frmMain)
		{
			this.frmMain = frmMain;
		}
		/// <summary>
		/// Starts the background thread which does web requests.
		/// </summary>
		public void Start()
		{
			Stop();
			cmdQueue = new ConcurrentQueue<AsyncCommandArgs>();
			waitHandle = new SemaphoreSlim(0);
			threadEnabledFlag = new bool[] { true };
			thrWebRequests = new Thread(webRequestLoop);
			thrWebRequests.Name = "WebRequests";
			thrWebRequests.IsBackground = true;
			thrWebRequests.Start(threadEnabledFlag);
		}
		/// <summary>
		/// Stops the background thread which does web requests and clears all queued requests.
		/// </summary>
		public void Stop()
		{
			try
			{
				if (thrWebRequests != null)
				{
					threadEnabledFlag[0] = false;
					waitHandle.Release();
					if (!thrWebRequests.Join(1000))
						thrWebRequests.Abort();
					thrWebRequests = null;
				}
				cmdQueue = null;
				waitHandle = null;
			}
			catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString()); }
		}
		public void EnqueueCommand(Resource resource, params object[] args)
		{
			if (thrWebRequests == null)
				throw new Exception("BackgroundCommandManager has not been started.");
			cmdQueue.Enqueue(new AsyncCommandArgs(resource, args));
			waitHandle.Release();
		}

		private void webRequestLoop(object arg)
		{
			bool[] isEnabled = (bool[])arg;
			try
			{
				while (isEnabled[0])
				{
					AsyncCommandArgs cmd = null;
					try
					{
						waitHandle.Wait();

						while (isEnabled[0] && !cmdQueue.TryDequeue(out cmd))
							Thread.Sleep(1); // This only gets hit if something goes terribly, terribly wrong.

						if (cmd != null)
						{
							WebResponse response = RunCommandSynchronous(cmd);
							frmMain.Invoke(ResponseReceived, new WebRequestCompletedEventArgs(false, null, cmd.resource, cmd.args, response.Response, response.Headers));
						}
					}
					catch (ThreadAbortException) { Console.WriteLine("webRequestLoop aborted"); return; }
					catch (Exception ex)
					{
						Logger.Debug(ex);
						if (cmd != null)
							frmMain.Invoke(ResponseReceived, new WebRequestCompletedEventArgs(true, ex, cmd.resource, cmd.args, null, null));
					}
				}
				if (!isEnabled[0])
					Console.WriteLine("WebRequests thread stopped");
			}
			catch (ThreadAbortException) { Console.WriteLine("webRequestLoop aborted"); return; }
			catch (Exception ex)
			{
				Logger.Debug(ex);
			}
			finally
			{
				Console.WriteLine("WebRequests thread exiting");
			}
		}

		private WebResponse RunCommandSynchronous(AsyncCommandArgs cmd, int tryNum = 1)
		{
			try
			{
				WebResponse response = null;
				switch (cmd.resource)
				{
					case Resource.AuthTokenExchange:
						response = HTTP_POST(authTokenExchangeUrl
							, "client_id=" + clientId + "&client_secret=" + clientSecret + "&grant_type=pin&pin=" + Uri.EscapeDataString((string)cmd.args[0])
							, true);
						HandleNewAuthInfo(response, true);
						break;
					case Resource.AuthTokenRefresh:
						response = HTTP_POST(authTokenExchangeUrl
							, "client_id=" + clientId + "&client_secret=" + clientSecret + "&grant_type=refresh_token&refresh_token=" + Uri.EscapeDataString(frmMain.cfg.refresh_token)
							, true);
						HandleNewAuthInfo(response, false);
						break;
					case Resource.UploadImageData:
						//throw new Exception("FAIL TEST");
						response = HTTP_POST(baseUrl + "image?type=file&title=" + Uri.EscapeDataString((string)cmd.args[2]) + "&description=" + Uri.EscapeDataString((string)cmd.args[3])
							, (byte[])cmd.args[0]
							, (string)cmd.args[1]
							, false);
						break;
					case Resource.AccountStatus:
						response = HTTP_GET(baseUrl + "account/me", false);
						break;
					case Resource.ImageCount:
						response = HTTP_GET(baseUrl + "account/me/images/count", false);
						break;
					case Resource.DeleteImage:
						response = HTTP_DELETE(baseUrl + "image/" + (string)cmd.args[0], false);
						break;
					default:
						Logger.Debug("Unhandled request type: " + cmd.resource);
						throw new Exception("Unhandled request type");
				}
				Console.WriteLine(cmd.resource.ToString() + ": " + response.Response);
				return response;
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
					using (StreamReader sr = new StreamReader(ex.Response.GetResponseStream()))
					{
						string response = sr.ReadToEnd();
						Logger.Debug("WebException response: " + response);
						try
						{
							dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
							if (result.status == 403)
							{
								if (result.data.error.ToString().Contains("access token"))
								{
									if (string.IsNullOrWhiteSpace(frmMain.cfg.refresh_token) || tryNum > 1)
										throw new Exception("Your authentication is invalid. Please log in again.");
									else
									{
										RunCommandSynchronous(new AsyncCommandArgs(Resource.AuthTokenRefresh, new object[0]));
										return RunCommandSynchronous(cmd, tryNum + 1);
									}
								}
							}
						}
						catch (Exception e) { Main.MessageBoxShow(e.ToString()); }
					}
				throw ex;
			}
		}

		private void HandleNewAuthInfo(WebResponse response, bool showLoggedInDialog)
		{
			dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Response);
			if (result.account_id != null)
			{
				Main.frmMain.cfg.access_token = result.access_token;
				Main.frmMain.cfg.refresh_token = result.refresh_token;
				Main.frmMain.cfg.account_id = result.account_id.ToString();
				Main.frmMain.cfg.account_username = result.account_username;
				Main.frmMain.cfg.token_type = result.token_type;
				Main.frmMain.cfg.Save();
				Main.frmMain.LoginStatusChanged();

				if (showLoggedInDialog)
					Main.MessageBoxShow("Authentication complete. You are now logged in as " + frmMain.cfg.account_username);
			}
			else
			{
				Main.MessageBoxShow("Authentication failed.");
			}
		}

		private void PrepareWebRequest(bool authenticateAsApp, HttpWebRequest request)
		{
			if (frmMain.cfg.disableWebProxy)
				request.Proxy = null;

			string access_token = frmMain.cfg.access_token;
			string tokenType = frmMain.cfg.token_type;

			if (string.IsNullOrEmpty(access_token))
				authenticateAsApp = true;

			if (authenticateAsApp)
				request.Headers[HttpRequestHeader.Authorization] = "Client-ID " + clientId;
			else
			{
				if (tokenType == "bearer" || string.IsNullOrWhiteSpace(tokenType))
					tokenType = "Bearer";
				request.Headers[HttpRequestHeader.Authorization] = tokenType + " " + access_token;
			}
		}

		private void InvokeUploadProgressChanged(int percent)
		{
			try
			{
				frmMain.Invoke(UploadProgressChanged, percent);
			}
			catch (Exception ex)
			{
				Logger.Debug(ex);
			}
		}

		private WebResponse HTTP_POST(string url, byte[] data, string contentType, bool authenticateAsApp)
		{
			return HTTP_Request(url, data, contentType, "POST", authenticateAsApp);
		}

		private WebResponse HTTP_POST(string url, string parameters, bool authenticateAsApp)
		{
			return HTTP_Request(url, UTF8Encoding.UTF8.GetBytes(parameters), "application/x-www-form-urlencoded", "POST", authenticateAsApp);
		}
		private WebResponse HTTP_GET(string url, bool authenticateAsApp)
		{
			return HTTP_Request(url, null, null, "GET", authenticateAsApp);
		}
		private WebResponse HTTP_DELETE(string url, bool authenticateAsApp)
		{
			return HTTP_Request(url, null, null, "DELETE", authenticateAsApp);
		}
		private WebResponse HTTP_Request(string url, byte[] data, string contentType, string method, bool authenticateAsApp)
		{
			Logger.Info(method + " " + url);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			PrepareWebRequest(authenticateAsApp, request);
			if (contentType != null)
				request.ContentType = contentType;
			bool isUploadingData = data != null && method == "POST";
			request.Method = method;
			InvokeUploadProgressChanged(0);
			if (isUploadingData)
			{
				request.ContentLength = data.Length;
				using (Stream s = request.GetRequestStream())
				{
					int offset = 0;
					int amountPerChunk = 8196;
					int amountThisChunk = Math.Min(amountPerChunk, data.Length - offset);
					while (amountThisChunk > 0)
					{
						s.Write(data, offset, amountThisChunk);
						offset += amountThisChunk;
						amountThisChunk = Math.Min(amountPerChunk, data.Length - offset);
						InvokeUploadProgressChanged((int)Math.Round(((double)offset / data.Length) * 100));
					}
					s.Flush();
				}
			}
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
			{
				string str = sr.ReadToEnd();
				return new WebResponse(str, response.Headers);
			}
		}
		private class WebResponse
		{
			public string Response;
			public SortedList<string, string> Headers;
			public WebResponse(string response, WebHeaderCollection headers)
			{
				this.Response = response;
				Headers = new SortedList<string, string>(headers.Count);
				foreach (string key in headers.AllKeys)
					Headers[key] = headers.Get(key);
			}
		}
	}
}
