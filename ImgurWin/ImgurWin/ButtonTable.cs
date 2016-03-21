using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImgurWin
{
	public abstract class ButtonTable : Form
	{
		public event Action<object, string> ButtonClick = delegate { };
		protected void InvokeButtonClick(object sender, string tag)
		{
			ButtonClick(sender, tag);
		}
		Main frmMain;
		public ButtonTable()
		{
			ShowInTaskbar = false;
			Text = "Menu";
			BackColor = Color.FromArgb(215, 215, 215);
			TransparencyKey = Color.CornflowerBlue;
			FormBorderStyle = FormBorderStyle.None;
			Padding = Padding.Empty;
			TopMost = true;
			frmMain = (Main)Parent;
		}
	}
	public class LinkGeneratorButtonTable : ButtonTable
	{
		Main frmMain;
		public LinkGeneratorButtonTable(Main frmMain) : base()
		{
			this.frmMain = frmMain;

			string[] Columns = new string[]
				{
					"URL", "HTML", "BBCode", "Linked HTML", "Linked BBCode", "Open"
				};
			string[][] Rows = new string[][]
				{
					new string[] { "Full",       "Full Image",   ""  },
					new string[] { "160",        "Small",        "t" },
					new string[] { "320",        "Medium",       "m" },
					new string[] { "640",        "Large",        "l" },
					new string[] { "1024",       "Huge",         "h" },
					new string[] { "Square 90",  "Tiny Square",  "s" },
					new string[] { "Square 160", "Small Square", "b" }
				};
			Size = new Size(64 * (Columns.Length + 1), 64 * (Rows.Length + 1));
			AddButton("Open Imgur Page", 0, 0, 64, 64, Color.FromArgb(235, 235, 235), "Open Imgur Page");
			for (int i = 0; i < Columns.Length; i++)
				AddLabel(Columns[i], (i + 1) * 64, 0, 64, 64);
			for (int r = 0; r < Rows.Length; r++)
			{
				int yPos = (r + 1) * 64;
				AddLabel(Rows[r][1], 0, yPos, 64, 64);
				for (int i = 0; i < Columns.Length; i++)
				{
					Color backColor = Color.White;
					if (r > 4)
						backColor = Color.FromArgb(235, 235, 235);
					else if (i == 2)
						backColor = Color.FromArgb(210, 255, 210);
					else if (i == 4)
						backColor = Color.FromArgb(0, 255, 0);
					AddButton(Rows[r][0], (i + 1) * 64, yPos, 64, 64, backColor, Columns[i] + "|" + Rows[r][2]);
				}
			}
		}

		private void AddButton(string text, int x, int y, int w, int h, Color backColor, string tag)
		{
			Button btn = new Button();
			btn.Text = text;
			btn.AutoSize = false;
			btn.SetBounds(x, y, w, h);
			btn.MouseUp += Btn_MouseUp;
			btn.BackColor = backColor;
			btn.Tag = tag;
			Controls.Add(btn);
		}

		private void Btn_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				InvokeButtonClick(sender, (string)((Button)sender).Tag);
		}

		private void AddLabel(string text, int x, int y, int w, int h)
		{
			Label lbl = new Label();
			lbl.Text = text;
			lbl.AutoSize = false;
			lbl.TextAlign = ContentAlignment.MiddleCenter;
			lbl.SetBounds(x, y, w, h);
			Controls.Add(lbl);
		}
	}
}
