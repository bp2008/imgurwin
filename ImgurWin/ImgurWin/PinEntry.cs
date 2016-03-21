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
	public partial class PinEntry : Form
	{
		Options frmOptions;
		public PinEntry(Options frmOptions)
		{
			this.frmOptions = frmOptions;
			InitializeComponent();
		}

		private void btnDone_Click(object sender, EventArgs e)
		{
			frmOptions.AcceptPIN(txtPIN.Text);
		}
	}
}
