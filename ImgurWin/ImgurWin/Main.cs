using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ImgurWin
{
	public partial class Main : Form
	{
		public ImgurRequestManager irm;
		Options frmOptions;
		SnippingTool snippingTool = new SnippingTool();
		LinkGenerator linkGenerator = new LinkGenerator();
		GlobalMouseHook gmh = new GlobalMouseHook();
		ButtonTable buttonTable;
		public ImgurWinConfig cfg;

		public readonly string applicationDirectoryBase;

		public static Main frmMain;

		internal bool _isLoggedIn = false;
		public LoginStatus loginStatus
		{
			get
			{
				if (string.IsNullOrWhiteSpace(cfg.access_token))
					return LoginStatus.Not_logged_in;
				if (_isLoggedIn)
					return LoginStatus.Logged_in;
				return LoginStatus.Maybe_logged_in;
			}
		}

		#region Keep track of uploaded image IDs from the last user input operation
		long queuedImages = 0;
		long queuedDeletes = 0;
		/// <summary>
		/// The Ids list is always the one to get locked. Not the Urls list.
		/// </summary>
		private List<string> imgurIdsFromLastOperation = new List<string>();
		private List<string> imgurUrlsFromLastOperation = new List<string>();
		private void StartNewInputOperation()
		{
			if (IsOperationInProgress())
			{
				MessageBoxShow("A previous operation is still active. If you believe this is incorrect, please restart the program.");
				throw new OperationStartCanceledException();
			}
			lock (imgurIdsFromLastOperation)
				imgurIdsFromLastOperation.Clear();
		}
		private string[] GetImgurIdsFromLastOperation()
		{
			lock (imgurIdsFromLastOperation)
				return imgurIdsFromLastOperation.ToArray();
		}
		private string[] GetImgurUrlsFromLastOperation()
		{
			lock (imgurIdsFromLastOperation)
				return imgurUrlsFromLastOperation.ToArray();
		}
		private void ImageIdLoaded(string imageId, string imageUrl)
		{
			lock (imgurIdsFromLastOperation)
			{
				imgurIdsFromLastOperation.Add(imageId);
				imgurUrlsFromLastOperation.Add(imageUrl);
			}
		}
		private void ImageIdDeleted(string imageId)
		{
			lock (imgurIdsFromLastOperation)
			{
				int index = imgurIdsFromLastOperation.IndexOf(imageId);
				if (index > -1)
				{
					imgurIdsFromLastOperation.RemoveAt(index);
					imgurUrlsFromLastOperation.RemoveAt(index);
				}
			}
		}
		private bool IsOperationInProgress()
		{
			long qImgs = Interlocked.Read(ref queuedImages);
			long qDels = Interlocked.Read(ref queuedDeletes);
			return qImgs > 0 || qDels > 0;
		}
		#endregion

		public Main()
		{
			frmMain = this;
			Application.ThreadException += Application_ThreadException;

			string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			applicationDirectoryBase = new FileInfo(exePath).Directory.FullName.TrimEnd('\\', '/').Replace('\\', '/') + '/';

			cfg = new ImgurWinConfig();
			cfg.Load();

			irm = new ImgurRequestManager(this);
			irm.ResponseReceived += ResponseReceived;
			irm.UploadProgressChanged += Irm_UploadProgressChanged;

			InitializeComponent();

			buttonTable = new LinkGeneratorButtonTable(this);
			buttonTable.ButtonClick += ButtonTable_ButtonClick;

			gmh.MouseUp += Gmh_MouseUp;
		}

		#region Misc UI Events
		private void Irm_UploadProgressChanged(int percent)
		{
			status_progressBar.Value = percent;
		}

		private void Main_Load(object sender, EventArgs e)
		{
			this.Text = "ImgurWin " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
			LoginStatusChanged();
			irm.Start();
			if (loginStatus == LoginStatus.Maybe_logged_in || loginStatus == LoginStatus.Logged_in)
				irm.EnqueueCommand(Resource.AccountStatus);
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			irm.Stop();
		}

		private void btnOptions_Click(object sender, EventArgs e)
		{
			Options oldOptions = frmOptions;
			if (oldOptions != null && !oldOptions.IsDisposed)
				try
				{
					oldOptions.Close();
				}
				catch (Exception ex) { MessageBoxShow(ex.ToString()); }
			Options newOptions = frmOptions = new Options(this);
			newOptions.Show();
		}
		#endregion
		#region ResponseRecieved event handler
		private void ResponseReceived(WebRequestCompletedEventArgs e)
		{
			try
			{
				if (e.Resource == Resource.UploadImageData)
					Interlocked.Decrement(ref queuedImages);
				if (e.Resource == Resource.DeleteImage)
					Interlocked.Decrement(ref queuedDeletes);
				// Handle errors
				if (e.Error != null)
				{
					if (e.Resource == Resource.UploadImageData)
					{
						SetPictureBoxImage(ImgurWin.Properties.Resources.UploadFailed);
						MessageBoxShow(e.Error, "Image failed to upload: " + e.Args[4].ToString());
					}
					else
						MessageBoxShow(e.Error);
				}
				// Handle cancellation
				if (e.Canceled)
					return;
				// Handle response headers
				HandleResponseHeaders(e.Headers);
				// Handle response
				switch (e.Resource)
				{
					case Resource.AuthTokenExchange:
						break;
					case Resource.AuthTokenRefresh:
						break;
					case Resource.UploadImageData:
						UpdateImageCount(1);
						ImageIdLoaded((string)e.Result.data.id, (string)e.Result.data.link);
						SetPictureBoxImage(ImageHelper.BitmapFromCompressedImageData((byte[])e.Args[0]), false);
						break;
					case Resource.AccountStatus:
						if (e.Result.success.Value)
						{
							_isLoggedIn = true;
							lblAccountName.Text = cfg.account_username = e.Result.data.url;
							cfg.account_id = e.Result.data.id.ToString();
							LoginStatusChanged();

							if (loginStatus == LoginStatus.Logged_in)
								irm.EnqueueCommand(Resource.ImageCount);
						}
						break;
					case Resource.ImageCount:
						if (e.Result.success.Value)
						{
							lblImages.Text = e.Result.data.ToString();
						}
						break;
					case Resource.DeleteImage:
						if (e.Result.success.Value)
						{
							ImageIdDeleted((string)e.Args[0]);
							UpdateImageCount(-1);
							SetPictureBoxImage(ImgurWin.Properties.Resources.DeletedImage);
						}
						break;
					default:
						Main.MessageBoxShow("Unhandled response type");
						Logger.Debug("Unhandled response type: " + e.Resource);
						break;
				}
			}
			finally
			{
				UpdateStatusLabel();
			}
		}

		private void UpdateImageCount(int diff)
		{
			int imageCount;
			if (int.TryParse(lblImages.Text, out imageCount))
			{
				imageCount += diff;
				lblImages.Text = imageCount.ToString();
			}
		}

		private string PostLimitStr = Environment.NewLine + "Your upload limit: unknown until you upload an image";
		private void HandleResponseHeaders(SortedList<string, string> headers)
		{
			long UserLimit = GetHeaderLong(headers, "X-RateLimit-UserLimit"); // Total credits that can be allocated.
			long UserRemaining = GetHeaderLong(headers, "X-RateLimit-UserRemaining"); // Total credits available.
			long UserReset = GetHeaderLong(headers, "X-RateLimit-UserReset"); // Timestamp (unix epoch) for when the credits will be reset.
			long ClientLimit = GetHeaderLong(headers, "X-RateLimit-ClientLimit"); // Total credits that can be allocated for the application in a day.
			long ClientRemaining = GetHeaderLong(headers, "X-RateLimit-ClientRemaining"); // Total credits remaining for the application in a day.
			long PostLimit = GetHeaderLong(headers, "X-Post-Rate-Limit-Limit"); // Total POST credits that are allocated.
			long PostRemaining = GetHeaderLong(headers, "X-Post-Rate-Limit-Remaining"); // Total POST credits available.
			long PostReset = GetHeaderLong(headers, "X-Post-Rate-Limit-Reset"); // Time in seconds until your POST ratelimit is reset

			long remaining;
			string resetTime;
			bool appSeeingHeavyUsage = false;
			if (UserRemaining < ClientRemaining)
			{
				remaining = UserRemaining;
				resetTime = DateTimeHelpers.FromUnixTime(UserReset).ToString();
			}
			else
			{
				remaining = ClientRemaining;
				appSeeingHeavyUsage = true;
			}

			if (PostLimit != -1)
			{
				if (PostRemaining < remaining)
				{
					remaining = PostRemaining;
					resetTime = DateTime.Now.AddSeconds(PostReset).ToString();
				}
				PostLimitStr = Environment.NewLine + "Your upload limit: " + PostRemaining + " POSTs remaining until " + DateTime.Now.AddSeconds(PostReset).ToString();
			}

			toolTip1.SetToolTip(lblTokens, "Imgur enforces several usage limits:"
				+ Environment.NewLine + "Your account limit: " + UserRemaining + " API credits remaining until " + DateTimeHelpers.FromUnixTime(UserReset).ToString()
				+ PostLimitStr
				+ Environment.NewLine + "ImgurWin global limit: " + ClientRemaining + " API credits remaining. Resets daily."
				);
			lblTokens.Text = remaining.ToString();
			if (appSeeingHeavyUsage)
				lblTokens.ForeColor = Color.Red;
			else
				lblTokens.ForeColor = Color.Black;

			Console.WriteLine("UserRemaining: " + UserRemaining + ", ClientRemaining: " + ClientRemaining + ", UserReset: " + (DateTimeHelpers.FromUnixTime(UserReset) - DateTime.Now) + ", PostRemaining: " + PostRemaining + ", PostReset: " + TimeSpan.FromSeconds(PostReset));
		}

		private long GetHeaderLong(SortedList<string, string> headers, string key)
		{
			if (headers.ContainsKey(key))
			{
				string value = headers[key];
				int i;
				if (int.TryParse(value, out i))
					return i;
			}
			return -1;
		}

		#endregion
		#region User Account Authentication
		internal void AcceptPIN(string text)
		{
			irm.EnqueueCommand(Resource.AuthTokenExchange, text);
		}

		internal void DoLogin()
		{
			Process.Start("https://api.imgur.com/oauth2/authorize?client_id=" + ImgurRequestManager.clientId + "&response_type=pin");
		}
		internal void LoginStatusChanged()
		{
			if (btnOpenFiles.InvokeRequired)
				btnOpenFiles.Invoke((Action)LoginStatusChanged);
			else
				btnOpenFiles.Enabled = btnSnip.Enabled = /*btnWindowSelect.Enabled = */btnPaste.Enabled = loginStatus == LoginStatus.Logged_in;
		}
		#endregion
		#region Link Generation
		private void pbImage_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				gmh.EnableHook(Invoke);
				Point startLoc = pbImage.PointToScreen(e.Location);
				startLoc.X -= buttonTable.Width / 2;
				startLoc.Y -= buttonTable.Height / 2;
				buttonTable.Location = startLoc;
				buttonTable.StartPosition = FormStartPosition.Manual;
				buttonTable.ShowDialog();
			}
		}

		private void Gmh_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				Invoke((Action)HideButtonTable);
		}
		private void HideButtonTable()
		{
			if (buttonTable != null)
			{
				buttonTable.Hide();
				SetTimeout.OnGui(() =>
				{
					gmh.DisableHook();
				}, 0, this);
			}
		}
		private void ButtonTable_ButtonClick(object sender, string tag)
		{
			if (tag == null)
				return;
			if (tag == "Open Imgur Page")
			{
				string[] imageIds = GetImgurIdsFromLastOperation();
				foreach (string id in imageIds)
					Process.Start("http://imgur.com/" + id);
				UpdateStatusLabel("Opened " + imageIds.Length + " imgur page" + (imageIds.Length == 1 ? "" : "s"));
			}
			else
			{
				string[] parts = tag.Split('|');
				if (parts.Length != 2)
				{
					MessageBoxShow("Application error; button \"" + tag + "\" was not set up correctly");
					return;
				}
				string type = parts[0]; // "URL", "HTML", "BBCode", "Linked HTML", "Linked BBCode", "Open"
				string suffix = parts[1];
				string[] imageUrls = GetImgurUrlsFromLastOperation();
				string[] imageUrlsWithSuffix = new string[imageUrls.Length];
				for (int i = 0; i < imageUrls.Length; i++)
					imageUrlsWithSuffix[i] = InsertImageSuffix(imageUrls[i], suffix);

				if (type == "Open")
				{
					foreach (string url in imageUrlsWithSuffix)
						Process.Start(url);
					UpdateStatusLabel("Opened " + imageUrls.Length + " image" + (imageUrls.Length == 1 ? "" : "s"));
				}
				else
				{
					if (type == "URL")
					{
						Clipboard.SetText(string.Join(Environment.NewLine, imageUrlsWithSuffix));
					}
					else if (type == "HTML")
					{
						List<string> strs = new List<string>();
						foreach (string url in imageUrlsWithSuffix)
							strs.Add("<img src=\"" + url + "\" />");
						Clipboard.SetText(string.Join(Environment.NewLine, strs));
					}
					else if (type == "BBCode")
					{
						List<string> strs = new List<string>();
						foreach (string url in imageUrlsWithSuffix)
							strs.Add("[img]" + url + "[/img]");
						Clipboard.SetText(string.Join(Environment.NewLine, strs));
					}
					else if (type == "Linked HTML")
					{
						List<string> strs = new List<string>();
						for (int i = 0; i < imageUrls.Length; i++)
							strs.Add("<a href=\"" + imageUrls[i] + "\"><img src=\"" + imageUrlsWithSuffix[i] + "\" /></a>");
						Clipboard.SetText(string.Join(Environment.NewLine, strs));
					}
					else if (type == "Linked BBCode")
					{
						List<string> strs = new List<string>();
						for (int i = 0; i < imageUrls.Length; i++)
							strs.Add("[url=" + imageUrls[i] + "][img]" + imageUrlsWithSuffix[i] + "[/img][/url]");
						Clipboard.SetText(string.Join(Environment.NewLine, strs));
					}
					UpdateStatusLabel("Copied " + imageUrls.Length + " " + type + (imageUrls.Length == 1 ? "" : "s") + " to clipboard");
				}
			}
		}

		private string InsertImageSuffix(string url, string suffix)
		{
			if (!string.IsNullOrWhiteSpace(suffix))
			{
				int idxLastDot = url.LastIndexOf('.');
				if (idxLastDot > -1)
					return url.Insert(idxLastDot, suffix);
			}
			return url;
		}
		#endregion
		#region Misc
		public static void MessageBoxShow(Exception ex)
		{
			MessageBoxShow(ex.ToString());
		}
		public static void MessageBoxShow(Exception ex, string additionalInfo)
		{
			MessageBoxShow(additionalInfo + ":" + Environment.NewLine + ex.ToString());
		}
		public static void MessageBoxShow(string message)
		{
			MessageBox.Show(message, "ImgurWin");
		}
		private void CreateDirectoryIfNoExist(string dir)
		{
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
		}
		/// <summary>
		/// Assigns the picture box a copy of the passed-in Bitmap and disposes of the previously assigned image.
		/// </summary>
		/// <param name="bmp">The Bitmap to copy and show in the picture box.</param>
		/// <param name="cloneInputImage">If true, the picture box will be assigned a clone/copy of the passed-in image.</param>
		private void SetPictureBoxImage(Bitmap bmp, bool cloneInputImage = true)
		{
			if (pbImage.InvokeRequired)
			{
				pbImage.Invoke((Action<Bitmap, bool>)SetPictureBoxImage, bmp, cloneInputImage);
				return;
			}
			Image original = pbImage.Image;
			if (cloneInputImage && bmp != null)
				pbImage.Image = (Bitmap)bmp.Clone();
			else
				pbImage.Image = bmp;
			if (original != null)
				original.Dispose();
		}
		private void NewImageQueued()
		{
			Interlocked.Increment(ref queuedImages);
			UpdateStatusLabel();
		}
		private void NewDeleteOperation()
		{
			Interlocked.Increment(ref queuedDeletes);
			UpdateStatusLabel();
		}
		public void UpdateStatusLabel(string str = null)
		{
			if (this.InvokeRequired)
			{
				this.Invoke((Action<string>)UpdateStatusLabel, str);
				return;
			}
			if (str != null)
				status_label.Text = str;
			else
			{
				long qImgs = Interlocked.Read(ref queuedImages);
				if (qImgs < 0)
				{
					// This shouldn't happen and isn't terribly important, so I don't really care if it isn't thread-safe.
					qImgs = 0;
					Interlocked.Exchange(ref queuedImages, 0);
				}
				long qDels = Interlocked.Read(ref queuedDeletes);
				if (qDels < 0)
				{
					// This shouldn't happen and isn't terribly important, so I don't really care if it isn't thread-safe.
					qDels = 0;
					Interlocked.Exchange(ref queuedDeletes, 0);
				}
				if (qImgs > 0)
					status_label.Text = "Uploading " + qImgs + " images";
				else if (qDels > 0)
					status_label.Text = "Deleting " + qDels + " images";
				else
					status_label.Text = "Ready";
			}
		}
		private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			if (e.Exception is WebException)
			{
				WebException ex = (WebException)e.Exception;
				using (StreamReader sr = new StreamReader(ex.Response.GetResponseStream()))
				{
					string result = sr.ReadToEnd();
					Logger.Debug(e.Exception);
					Logger.Debug("WebException response: " + result);
					MessageBoxShow("Thread Exception thrown: " + e.Exception.ToString());
					MessageBoxShow("WebException response: " + result);
				}
			}
			else {
				Logger.Debug(e.Exception);
				MessageBoxShow("Thread Exception thrown: " + e.Exception.ToString());
			}
		}
		#endregion
		#region Snipping Tool
		private void btnSnip_Click(object sender, EventArgs e)
		{
			snippingTool.DrawOverlayAcrossAllScreens(this);
		}

		internal void AcceptSnip(Bitmap imgSrc)
		{
			try
			{
				StartNewInputOperation();
				SetTimeout.AfterGuiResumesThenOnBackground(() =>
				{
					string contentType;
					byte[] imgData = ImageHelper.GetEfficientCompressedImageData(imgSrc, out contentType);

					imgSrc.Dispose();

					if (imgData.Length >= ImageHelper.tenMegabytes)
					{
						MessageBoxShow("Image is too large.");
					}
					else
					{
						string title = "Screen Capture";
						string caption = "Captured by " + ImgurRequestManager.AppName + " " + typeof(Main).Assembly.GetName().Version + " at " + DateTime.Now.ToLongTimeString() + " on " + DateTime.Now.ToString("yyyy/MM/dd");

						SaveCapturedImage(imgData, contentType == "image/png");
						SetPictureBoxImage(ImgurWin.Properties.Resources.UploadingImage);
						NewImageQueued();
						irm.EnqueueCommand(Resource.UploadImageData, imgData, contentType, title, caption, title);
					}
				}, 1, this);
			}
			catch (OperationStartCanceledException)
			{
			}
			catch (Exception ex)
			{
				MessageBoxShow(ex);
				Logger.Debug(ex);
			}
		}

		#endregion
		#region Captures Folder
		private void SaveCapturedImage(byte[] data, bool isPng)
		{
			string fileName = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + (isPng ? ".png" : ".jpg");
			CreateDirectoryIfNoExist(applicationDirectoryBase + "Captures");
			File.WriteAllBytes(applicationDirectoryBase + "Captures/" + fileName, data);
		}
		private void btnCaptures_Click(object sender, EventArgs e)
		{
			CreateDirectoryIfNoExist(applicationDirectoryBase + "Captures");
			Process.Start(applicationDirectoryBase + "Captures");
		}
		#endregion
		#region Open Files From Disk
		private void btnOpenFiles_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = openFileDialog1.ShowDialog();
			if (dialogResult == DialogResult.OK)
			{
				try
				{
					StartNewInputOperation();
					foreach (string fileName in openFileDialog1.FileNames)
					{
						FileInfo fi = new FileInfo(fileName);
						if (fi.Exists)
							try
							{
								UploadImageFromDisk(fi.FullName);
							}
							catch (Exception ex)
							{
								Logger.Debug(ex, "Failed to process image \"" + fileName + "\"");
							}
					}
				}
				catch (OperationStartCanceledException)
				{
				}
				catch (Exception ex)
				{
					MessageBoxShow(ex);
					Logger.Debug(ex);
				}
			}
		}
		private void UploadImageFromDisk(string fullName)
		{
			FileInfo fi = new FileInfo(fullName);
			byte[] data = File.ReadAllBytes(fi.FullName);
			using (Image img = ImageHelper.BitmapFromCompressedImageData(data))
			{
				string mimeType = ImageHelper.GetMimeType(img.RawFormat);
				string nameWithoutExtension = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
				string caption = "Uploaded by " + ImgurRequestManager.AppName + " " + typeof(Main).Assembly.GetName().Version + " at " + DateTime.Now.ToLongTimeString() + " on " + DateTime.Now.ToString("yyyy/MM/dd");
				SetPictureBoxImage(ImgurWin.Properties.Resources.UploadingImage);
				NewImageQueued();
				irm.EnqueueCommand(Resource.UploadImageData, data, mimeType, nameWithoutExtension, caption, fi.FullName);
			}
		}
		#endregion
		#region Handle Clipboard / Paste
		private void btnPaste_Click(object sender, EventArgs e)
		{
			try
			{
				string caption = "Uploaded by " + ImgurRequestManager.AppName + " " + typeof(Main).Assembly.GetName().Version + " at " + DateTime.Now.ToLongTimeString() + " on " + DateTime.Now.ToString("yyyy/MM/dd");
				if (Clipboard.ContainsImage())
				{
					StartNewInputOperation();
					Image img = Clipboard.GetImage();
					SetTimeout.OnBackground(() =>
					{
						try
						{
							string mimeType;
							byte[] imgData = ImageHelper.GetEfficientCompressedImageData(img, out mimeType);

							if (imgData.Length >= ImageHelper.tenMegabytes)
							{
								MessageBoxShow("Image is too large.");
							}
							else
							{
								SetPictureBoxImage(ImgurWin.Properties.Resources.UploadingImage);
								NewImageQueued();
								irm.EnqueueCommand(Resource.UploadImageData, imgData, mimeType, "", caption, "Clipboard Image");
							}
						}
						finally
						{
							img.Dispose();
						}
					}, 0);
				}
				else if (Clipboard.ContainsFileDropList())
				{
					System.Collections.Specialized.StringCollection strs = Clipboard.GetFileDropList();
					StartNewInputOperation();
					SetTimeout.OnBackground(() =>
					{
						foreach (string str in strs)
						{
							try
							{
								UploadImageFromDisk(str);
							}
							catch (Exception ex)
							{
								Logger.Debug(ex, "Failed to process file from clipboard \"" + str + "\"");
							}
						}
					}, 0);
				}
				else if (Clipboard.ContainsText(TextDataFormat.Text))
				{
					string text = Clipboard.GetText(TextDataFormat.Text);
					string[] lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
					if (lines.Length > 0)
					{
						StartNewInputOperation();
						SetTimeout.OnBackground(() =>
						{
							HandleTextLinesAsInput(lines, "clipboard");
						}, 0);
					}
				}
				else
				{
					MessageBoxShow("Unsupported clipboard content");
				}
			}
			catch (OperationStartCanceledException)
			{
			}
			catch (Exception ex)
			{
				MessageBoxShow(ex);
				Logger.Debug(ex);
			}
		}
		private void UploadImageFromUri(Uri uri)
		{
			WebClient wc = new WebClient();
			byte[] data = wc.DownloadData(uri);
			using (Image img = ImageHelper.BitmapFromCompressedImageData(data))
			{
				string mimeType = ImageHelper.GetMimeType(img.RawFormat);
				string caption = "Uploaded by " + ImgurRequestManager.AppName + " " + typeof(Main).Assembly.GetName().Version + " at " + DateTime.Now.ToLongTimeString() + " on " + DateTime.Now.ToString("yyyy/MM/dd");
				SetPictureBoxImage(ImgurWin.Properties.Resources.UploadingImage);
				NewImageQueued();
				irm.EnqueueCommand(Resource.UploadImageData, data, mimeType, "", caption, uri.OriginalString);
			}
		}
		#endregion
		#region Text lines as input
		private void HandleTextLinesAsInput(string[] lines, string inputMethod)
		{
			if (lines.Length > 25)
				MessageBoxShow("Too many text lines from " + inputMethod + ". Please limit the list of items to 25 or fewer.");
			int errorMessagesShown = 0;
			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i].Trim();
				// This could be a file path
				try
				{
					FileInfo fi = new FileInfo(line);
					if (fi.Exists)
					{
						try
						{
							UploadImageFromDisk(fi.FullName);
						}
						catch (Exception ex)
						{
							Logger.Debug(ex, "Failed to process file path from " + inputMethod + " \"" + line + "\"");
							if (errorMessagesShown++ < 3)
								MessageBoxShow("Failed to process file path from " + inputMethod + " \"" + line + "\"");
						}
						continue;
					}
				}
				catch (Exception) { }
				// This could be a web address
				try
				{
					Uri uri = new Uri(line);
					if (uri.Scheme == "http" || uri.Scheme == "https")
					{
						try
						{
							UploadImageFromUri(uri);
						}
						catch (Exception ex)
						{
							Logger.Debug(ex, "Failed to process Uri from " + inputMethod + " \"" + line + "\"");
							if (errorMessagesShown++ < 3)
								MessageBoxShow("Failed to process Uri from " + inputMethod + " \"" + line + "\"");
						}
					}
					else
					{
						if (errorMessagesShown++ < 3)
							MessageBoxShow("Unsupported Uri: " + line);
					}
					continue;
				}
				catch (Exception) { }
				if (errorMessagesShown++ < 3)
					MessageBoxShow("Could not identify resource type: " + line);
			}
		}
		#endregion
		#region Drag and Drop
		private void Main_DragEnter(object sender, DragEventArgs e)
		{
			if ((e.AllowedEffect & DragDropEffects.Copy) > 0)
			{
				string[] formats = e.Data.GetFormats(false);
				if (formats.Contains("FileDrop") || formats.Contains("Text"))
					e.Effect = DragDropEffects.Copy;
			}
			else
				e.Effect = DragDropEffects.None;
		}

		private void Main_DragDrop(object sender, DragEventArgs e)
		{
			try
			{
				if ((e.AllowedEffect & DragDropEffects.Copy) > 0)
				{
					string[] formats = e.Data.GetFormats(false);
					string[] lines = new string[0];
					if (formats.Contains("FileDrop"))
						lines = (string[])e.Data.GetData("FileDrop");
					else if (formats.Contains("Text"))
						lines = ((string)e.Data.GetData("Text")).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
					if (lines.Length > 0)
					{
						StartNewInputOperation();
						SetTimeout.OnBackground(() =>
						{
							HandleTextLinesAsInput(lines, "drag and drop");
						}, 0);
					}
				}
			}
			catch (OperationStartCanceledException)
			{
			}
			catch (Exception ex)
			{
				MessageBoxShow(ex);
				Logger.Debug(ex);
			}
		}
		#endregion

		#region Delete Images
		private void contextMenuStrip_pictureBox_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (IsOperationInProgress())
			{
				MessageBoxShow("The last operation has not yet finished. Unable to delete.");
				return;
			}
			string[] imgs = GetImgurIdsFromLastOperation();
			if (imgs.Length == 0)
			{
				UpdateStatusLabel("No images to delete");
				return;
			}
			if (e.ClickedItem == toolStripMenuItem_deleteAllLastOperationImages)
			{
				foreach (string img in imgs)
				{
					NewDeleteOperation();
					irm.EnqueueCommand(Resource.DeleteImage, img);
				}
			}
			else if (e.ClickedItem == toolStripMenuItem_deleteLastImage)
			{
				NewDeleteOperation();
				irm.EnqueueCommand(Resource.DeleteImage, imgs[imgs.Length - 1]);
			}
		}

		private void EnqueueDeleteImageCommand(string imgId)
		{
			irm.EnqueueCommand(Resource.DeleteImage, imgId);
		}

		private void contextMenuStrip_pictureBox_Opening(object sender, CancelEventArgs e)
		{
			toolStripMenuItem_deleteAllLastOperationImages.Text = "Delete all " + GetImgurIdsFromLastOperation().Length + " images";
		}
		#endregion
	}
}
