using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImgurWin
{
	public static class DateTimeHelpers
	{
		public static DateTime FromUnixTime(long unixTime)
		{
			DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			DateTime time = epoch.AddSeconds(unixTime);
			return TimeZoneInfo.ConvertTimeFromUtc(time, TimeZoneInfo.Local);
		}

		public static long ToUnixTime(DateTime date)
		{
			DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return Convert.ToInt64((date - epoch).TotalSeconds);
		}
	}
}
