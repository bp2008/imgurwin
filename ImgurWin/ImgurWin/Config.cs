using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImgurWin
{
	public class ImgurWinConfig : SerializableObjectBase
	{
		public string access_token = "";
		public string refresh_token = "";
		public string account_id = "";
		public string account_username = "";
		public string token_type = "";
		public bool disableWebProxy = true;
	}
}
