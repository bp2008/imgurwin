using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ImgurWin
{
	public static class SetTimeout
	{
		/// <summary>
		/// Invokes on the GUI thread the specified action after the specified timeout.
		/// </summary>
		/// <param name="TheAction"></param>
		/// <param name="Timeout"></param>
		/// <returns></returns>
		public static TimeoutHandle OnGui(Action TheAction, int Timeout, Form formForInvoking)
		{
			return _internal_setTimeout(TheAction, Timeout, true, formForInvoking);
		}
		/// <summary>
		/// Invokes on a background thread the specified action after the specified timeout.
		/// </summary>
		/// <param name="TheAction"></param>
		/// <param name="Timeout"></param>
		/// <returns></returns>
		public static TimeoutHandle OnBackground(Action TheAction, int Timeout)
		{
			return _internal_setTimeout(TheAction, Timeout, false, null);
		}
		/// <summary>
		/// Invokes a call to setTimeout_Background on the Gui Thread, so that other UI events have a chance to finish first. This timeout will not be cancelable.
		/// </summary>
		/// <param name="TheAction"></param>
		/// <param name="Timeout"></param>
		/// <returns></returns>
		public static void AfterGuiResumesThenOnBackground(Action TheAction, int Timeout, Form formForInvoking)
		{
			OnGui((Action)(() =>
			{
				OnBackground(TheAction, Timeout - 1);
			}), 1, formForInvoking);
		}
		private static TimeoutHandle _internal_setTimeout(Action TheAction, int Timeout, bool invokeOnGuiThread, Form formForInvoking)
		{
			TimeoutHandle cancelHandle = new TimeoutHandle();
			Thread t = new Thread(
				() =>
				{
					try
					{
						if (cancelHandle.Wait(Timeout))
							return;
						if (invokeOnGuiThread)
							formForInvoking.Invoke(TheAction);
						else
							TheAction();
					}
					catch (ThreadAbortException) { }
					catch (Exception ex)
					{
						Logger.Debug(ex);
						Main.MessageBoxShow(ex.ToString());
					}
				}
			);
			t.Name = "Timeout";
			t.IsBackground = true;
			t.Start();
			return cancelHandle;
		}
		public class TimeoutHandle
		{
			private EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
			/// <summary>
			/// Cancels this timeout, if it is still waiting.
			/// </summary>
			public void Cancel()
			{
				ewh.Set();
			}
			/// <summary>
			/// Waits up to the specified number of milliseconds and returns early with a value of true if the Cancel method was called during this time. Returns false at the end of the waiting period if not canceled.
			/// </summary>
			/// <param name="ms">The number of milliseconds to wait.</param>
			/// <returns></returns>
			internal bool Wait(int ms)
			{
				return ewh.WaitOne(ms);
			}
		}
	}
}
