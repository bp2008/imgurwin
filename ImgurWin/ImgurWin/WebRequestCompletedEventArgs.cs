using System;
using System.Collections.Generic;

namespace ImgurWin
{
	public class WebRequestCompletedEventArgs
	{
		public bool Canceled;
		public Exception Error;
		public Resource Resource;
		public object[] Args;
		public string Response;
		public dynamic Result;
		public SortedList<string, string> Headers;

		public WebRequestCompletedEventArgs(bool Canceled, Exception Error, Resource Resource, object[] Args, string Response, SortedList<string,string> Headers)
		{
			this.Canceled = Canceled;
			this.Error = Error;
			this.Resource = Resource;
			this.Args = Args;
			this.Response = Response;
			this.Result = Response == null ? null : Newtonsoft.Json.JsonConvert.DeserializeObject(Response);
			this.Headers = Headers;
		}
	}
}