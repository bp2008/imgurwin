using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ImgurWin
{
	public static class ScreenCapture
	{
		public static Bitmap CaptureScreenshotRect(Rectangle rect)
		{
			IntPtr hDesk = GetDesktopWindow();
			IntPtr hSrce = GetWindowDC(hDesk);
			IntPtr hDest = CreateCompatibleDC(hSrce);
			IntPtr hBmp = CreateCompatibleBitmap(hSrce, rect.Width, rect.Height);
			IntPtr hOldBmp = SelectObject(hDest, hBmp);
			bool b = BitBlt(hDest, 0, 0, rect.Width, rect.Height, hSrce, rect.X, rect.Y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
			Bitmap bmp = Bitmap.FromHbitmap(hBmp);
			SelectObject(hDest, hOldBmp);
			DeleteObject(hBmp);
			DeleteDC(hDest);
			ReleaseDC(hDesk, hSrce);
			return bmp;
		}
		// P/Invoke declarations
		[DllImport("gdi32.dll")]
		static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
		wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
		[DllImport("user32.dll")]
		static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
		[DllImport("gdi32.dll")]
		static extern IntPtr DeleteDC(IntPtr hDc);
		[DllImport("gdi32.dll")]
		static extern IntPtr DeleteObject(IntPtr hDc);
		[DllImport("gdi32.dll")]
		static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
		[DllImport("gdi32.dll")]
		static extern IntPtr CreateCompatibleDC(IntPtr hdc);
		[DllImport("gdi32.dll")]
		static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr ptr);
	}
}
