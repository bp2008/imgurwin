using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImgurWin
{
	public class SnippingTool
	{
		Font font = new Font(FontFamily.GenericMonospace, 12);
		public void DrawOverlayAcrossAllScreens(Main frmMain)
		{
			int top = int.MaxValue, bottom = int.MinValue, left = int.MaxValue, right = int.MinValue;
			foreach (Screen s in Screen.AllScreens)
			{
				top = Math.Min(top, s.Bounds.Top);
				bottom = Math.Max(bottom, s.Bounds.Bottom);
				left = Math.Min(left, s.Bounds.Left);
				right = Math.Max(right, s.Bounds.Right);
			}

			frmMain.Hide();
			frmMain.Opacity = 0;

			Form hud = new DoubleBufferedForm();
			Color defaultTransparencyKey = hud.TransparencyKey;
			hud.ShowInTaskbar = false;
			hud.Text = "Screen Capture";
			hud.BackColor = Color.CornflowerBlue;
			hud.TransparencyKey = Color.CornflowerBlue;
			hud.FormBorderStyle = FormBorderStyle.None;
			hud.Padding = Padding.Empty;
			hud.TopMost = true;
			hud.Cursor = Cursors.Cross;

			Form bg;
			if (frmMain.cfg.freezeWhileSnipping)
				bg = hud;
			else
			{
				bg = new Form();
				bg.ShowInTaskbar = false;
				bg.Text = "Screen Capture BG";
				bg.BackColor = Color.White;
				bg.Opacity = 0.2;
				bg.FormBorderStyle = FormBorderStyle.None;
				bg.Padding = Padding.Empty;
				bg.TopMost = true;
				bg.Cursor = Cursors.Cross;
			}
			Bitmap baseScreenShot = null;
			hud.FormClosing += (object sender, FormClosingEventArgs e) =>
			{
				if (bg != hud)
					bg.Close();
				frmMain.Opacity = 1;
				frmMain.Show();
				baseScreenShot?.Dispose();
			};


			hud.Show();
			if (bg != hud)
				bg.Show();

			Rectangle screenBounds = new Rectangle(left, top, right - left, bottom - top);

			hud.Bounds = screenBounds;
			if (bg != hud)
				bg.Bounds = screenBounds;

			Rectangle clientBounds = hud.RectangleToClient(screenBounds);

			bool isMouseDown = false;
			int mouseStartX = 0, mouseStartY = 0, mouseEndX = 0, mouseEndY = 0;
			Rectangle labelRect = Rectangle.Empty;
			bool ifNextClickIsRightThenClose = false;

			Rectangle finalSnippingSelection = Rectangle.Empty;
			bool readyForCapture = false;

			hud.Paint += (object sender, PaintEventArgs e) =>
			{
				if (frmMain.cfg.freezeWhileSnipping)
				{
					if (baseScreenShot == null)
						baseScreenShot = ScreenCapture.CaptureScreenshotRect(screenBounds);
					if (hud.TransparencyKey != defaultTransparencyKey)
						hud.TransparencyKey = defaultTransparencyKey;
					e.Graphics.DrawImage(baseScreenShot, 0, 0, hud.Width, hud.Height);
				}
				string label;
				Size labelOffset = Size.Empty;
				SizeF labelSize = Size.Empty;
				if (isMouseDown)
				{
					int x = Math.Min(mouseStartX, mouseEndX);
					int y = Math.Min(mouseStartY, mouseEndY);
					int w = Math.Abs(mouseEndX - mouseStartX);
					int h = Math.Abs(mouseEndY - mouseStartY);
					if (w < 10 || h < 10)
						e.Graphics.FillRectangle(Brushes.Red, new Rectangle(x, y, w + 1, h + 1));
					else
						e.Graphics.DrawRectangle(Pens.Red, new Rectangle(x, y, w, h));
					e.Graphics.DrawRectangle(Pens.Blue, new Rectangle(x - 1, y - 1, w + 2, h + 2));

					label = w + " x " + h;
					if (w < 10 || h < 10)
						label += Environment.NewLine + "(too small; will not capture)";
					labelSize = e.Graphics.MeasureString(label, font);
					labelOffset.Width = mouseStartX < mouseEndX ? 10 : (int)(-1 * labelSize.Width) - 10;
					labelOffset.Height = mouseStartY < mouseEndY ? 10 : (int)(-1 * labelSize.Height) - 10;
				}
				else
				{
					label = "Click and drag to capture region." + Environment.NewLine + "Right click to cancel selection.";
					labelOffset.Width = labelOffset.Height = 10;
					labelSize = e.Graphics.MeasureString(label, font);
				}
				labelRect = new Rectangle(mouseEndX + labelOffset.Width, mouseEndY + labelOffset.Height, (int)labelSize.Width + 5, (int)labelSize.Height + 5);

				// Make sure the label stays within major screen bounds (it may still disappear off interior corners, however)
				if (labelRect.Right > clientBounds.Right)
					labelRect.X -= labelRect.Right - clientBounds.Right;
				if (labelRect.Bottom > clientBounds.Bottom)
					labelRect.Y -= labelRect.Bottom - clientBounds.Bottom;
				if (labelRect.Left < clientBounds.Left)
					labelRect.X += clientBounds.Left - labelRect.Left;
				if (labelRect.Top < clientBounds.Top)
					labelRect.Y += clientBounds.Top - labelRect.Top;

				// Draw label over filled background.
				e.Graphics.FillRectangle(Brushes.White, labelRect);
				e.Graphics.DrawRectangle(Pens.Black, labelRect);
				e.Graphics.DrawString(label, font, Brushes.Black, labelRect.X + 3, labelRect.Y + 3);
			};
			bg.MouseUp += (object sender, MouseEventArgs e) =>
			{
				if (isMouseDown && e.Button == MouseButtons.Left)
				{
					isMouseDown = false;
					finalSnippingSelection = new Rectangle();
					mouseEndX = e.X;
					mouseEndY = e.Y;
					int x = Math.Min(mouseStartX, mouseEndX);
					int y = Math.Min(mouseStartY, mouseEndY);
					int w = Math.Abs(mouseEndX - mouseStartX);
					int h = Math.Abs(mouseEndY - mouseStartY);
					finalSnippingSelection = new Rectangle(x, y, w, h);
					if (!frmMain.cfg.freezeWhileSnipping || baseScreenShot == null)
						finalSnippingSelection = hud.RectangleToScreen(finalSnippingSelection);
					if (finalSnippingSelection.Width >= 10 && finalSnippingSelection.Height >= 10)
						try
						{
							hud.Hide();
							hud.Opacity = 0;
							bg.Hide();
							bg.Opacity = 0;
							if (frmMain.cfg.freezeWhileSnipping && baseScreenShot != null)
								frmMain.AcceptSnip(baseScreenShot.Clone(finalSnippingSelection, baseScreenShot.PixelFormat));
							else
								frmMain.AcceptSnip(ScreenCapture.CaptureScreenshotRect(finalSnippingSelection));
						}
						catch (Exception ex)
						{
							Main.MessageBoxShow(ex);
						}
					hud.Close();
				}
			};
			bg.MouseDown += (object sender, MouseEventArgs e) =>
			{
				ifNextClickIsRightThenClose = false;
				if (e.Button == MouseButtons.Left)
				{
					isMouseDown = true;
					mouseStartX = mouseEndX = e.X;
					mouseStartY = mouseEndY = e.Y;
				}
				else
				{
					if (isMouseDown)
					{
						hud.Invalidate();
						isMouseDown = false;
					}
					else
						ifNextClickIsRightThenClose = true;
				}
			};
			bg.Click += (object sender, EventArgs e) =>
			{
				if (ifNextClickIsRightThenClose && ((MouseEventArgs)e).Button == MouseButtons.Right)
					hud.Close();
				else
					ifNextClickIsRightThenClose = false;
			};
			bg.KeyDown += (object sender, KeyEventArgs e) =>
			{
				if (e.KeyCode == Keys.Escape)
					hud.Close();
			};
			bg.MouseMove += (object sender, MouseEventArgs e) =>
			{
				if (isMouseDown)
				{
					int x = Math.Min(mouseStartX, mouseEndX);
					int y = Math.Min(mouseStartY, mouseEndY);
					int w = Math.Abs(mouseEndX - mouseStartX);
					int h = Math.Abs(mouseEndY - mouseStartY);
					hud.Invalidate(new Rectangle(x - 1, y - 1, w + 3, h + 3));
					mouseEndX = e.X;
					mouseEndY = e.Y;
					x = Math.Min(mouseStartX, mouseEndX);
					y = Math.Min(mouseStartY, mouseEndY);
					w = Math.Abs(mouseEndX - mouseStartX);
					h = Math.Abs(mouseEndY - mouseStartY);
					hud.Invalidate(new Rectangle(x - 1, y - 1, w + 3, h + 3));
				}
				else
				{
					mouseEndX = e.X;
					mouseEndY = e.Y;
				}
				if (labelRect != Rectangle.Empty)
				{
					Rectangle invalidateRect = labelRect;
					invalidateRect.Inflate(2, 2);
					hud.Invalidate(invalidateRect);
				}
				else
					hud.Invalidate();
			};
		}

		private class DoubleBufferedForm : Form
		{
			public DoubleBufferedForm()
			{
				this.DoubleBuffered = true;
			}
		}
	}
}
