﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ImgurWin
{
	public enum LoggingMode
	{
		None = 0,
		Console = 1,
		File = 2,
		Email = 4
	}
	public class Logger
	{
		public static LoggingMode logType = LoggingMode.File | LoggingMode.Console;
		private static object lockObj = new object();
		public static string LogFilePath = "ImgurWin_Log.txt";
		public static void Debug(Exception ex, string additionalInformation = "")
		{
			if (additionalInformation == null)
				additionalInformation = "";
			lock (lockObj)
			{
				if ((logType & LoggingMode.Console) > 0)
				{
					if (ex != null)
						Console.Write("Exception thrown at ");
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine(DateTime.Now.ToString());
					if (ex != null)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(ex.ToString());
					}
					if (!string.IsNullOrEmpty(additionalInformation))
					{
						Console.ForegroundColor = ConsoleColor.DarkYellow;
						if (ex != null)
							Console.Write("Additional information: ");
						Console.WriteLine(additionalInformation);
					}
					Console.ResetColor();
				}
				if ((logType & LoggingMode.File) > 0 && (ex != null || !string.IsNullOrEmpty(additionalInformation)))
				{
					StringBuilder debugMessage = new StringBuilder();
					debugMessage.Append("-------------").Append(Environment.NewLine);
					if (ex != null)
						debugMessage.Append("Exception thrown at ");
					debugMessage.Append(DateTime.Now.ToString()).Append(Environment.NewLine);
					if (!string.IsNullOrEmpty(additionalInformation))
					{
						if (ex != null)
							debugMessage.Append("Additional information: ");
						debugMessage.Append(additionalInformation).Append(Environment.NewLine);
					}
					if (ex != null)
						debugMessage.Append(ex.ToString()).Append(Environment.NewLine);
					debugMessage.Append("-------------").Append(Environment.NewLine);
					int attempts = 0;
					while (attempts < 5)
					{
						try
						{
							File.AppendAllText(LogFilePath, debugMessage.ToString());
							attempts = 10;
						}
						catch (Exception e) { System.Windows.Forms.MessageBox.Show(e.ToString()); }
						{
							attempts++;
						}
					}
				}
			}
		}

		public static void Debug(string message)
		{
			Debug(null, message);
		}
		public static void Info(string message)
		{
			if (message == null)
				return;
			lock (lockObj)
			{
				if ((logType & LoggingMode.Console) > 0)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine(DateTime.Now.ToString());
					Console.ResetColor();
					Console.WriteLine(message);
				}
				if ((logType & LoggingMode.File) > 0)
				{
					int attempts = 0;
					while (attempts < 5)
					{
						try
						{
							File.AppendAllText(LogFilePath, DateTime.Now.ToString() + Environment.NewLine + message + Environment.NewLine);
							attempts = 10;
						}
						catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.ToString()); }
						{
							attempts++;
						}
					}
				}
			}
		}
	}
}
