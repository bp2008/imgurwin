using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImgurWin
{
	public partial class Options : Form
	{
		Main frmMain;
		PinEntry frmPin;
		public Options(Main frmMain)
		{
			this.frmMain = frmMain;
			InitializeComponent();
			SetLoginStatus(frmMain.loginStatus);
			cbFreezeScreenWhileSnipping.Checked = frmMain.cfg.freezeWhileSnipping;
		}

		private void SetLoginStatus(LoginStatus status)
		{
			switch (status)
			{
				case LoginStatus.Not_logged_in:
					lblAuthenticationStatus.ForeColor = Color.Black;
					lblAuthenticationStatus.Text = "Not logged in";
					break;
				case LoginStatus.Logged_in:
					lblAuthenticationStatus.ForeColor = Color.Green;
					lblAuthenticationStatus.Text = "Logged in";
					break;
				case LoginStatus.Error:
					lblAuthenticationStatus.ForeColor = Color.Red;
					lblAuthenticationStatus.Text = "Error - Check Log";
					break;
			}
		}

		public void AcceptPIN(string text)
		{
			frmMain.AcceptPIN(text);
			frmPin.Close();
		}

		private void Options_Load(object sender, EventArgs e)
		{

		}

		private void btnLogin_Click(object sender, EventArgs e)
		{
			if (frmPin != null)
				try
				{
					frmPin.Close();
				}
				catch (Exception ex) { MessageBox.Show(ex.ToString()); }

			frmPin = new PinEntry(this);
			frmPin.Show();

			frmMain.DoLogin();
		}

		private void cbFreezeScreenWhileSnipping_CheckedChanged(object sender, EventArgs e)
		{
			frmMain.cfg.freezeWhileSnipping = cbFreezeScreenWhileSnipping.Checked;
			frmMain.cfg.Save();
		}
	}
}
