namespace ImgurWin
{
	partial class Options
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.lblAuthenticationStatus = new System.Windows.Forms.Label();
			this.btnLogin = new System.Windows.Forms.Button();
			this.cbFreezeScreenWhileSnipping = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(140, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Imgur Authentication Status:";
			// 
			// lblAuthenticationStatus
			// 
			this.lblAuthenticationStatus.AutoSize = true;
			this.lblAuthenticationStatus.Location = new System.Drawing.Point(159, 13);
			this.lblAuthenticationStatus.Name = "lblAuthenticationStatus";
			this.lblAuthenticationStatus.Size = new System.Drawing.Size(16, 13);
			this.lblAuthenticationStatus.TabIndex = 1;
			this.lblAuthenticationStatus.Text = "...";
			// 
			// btnLogin
			// 
			this.btnLogin.Location = new System.Drawing.Point(311, 8);
			this.btnLogin.Name = "btnLogin";
			this.btnLogin.Size = new System.Drawing.Size(75, 23);
			this.btnLogin.TabIndex = 2;
			this.btnLogin.Text = "Log in";
			this.btnLogin.UseVisualStyleBackColor = true;
			this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
			// 
			// cbFreezeScreenWhileSnipping
			// 
			this.cbFreezeScreenWhileSnipping.AutoSize = true;
			this.cbFreezeScreenWhileSnipping.Location = new System.Drawing.Point(16, 51);
			this.cbFreezeScreenWhileSnipping.Name = "cbFreezeScreenWhileSnipping";
			this.cbFreezeScreenWhileSnipping.Size = new System.Drawing.Size(162, 17);
			this.cbFreezeScreenWhileSnipping.TabIndex = 3;
			this.cbFreezeScreenWhileSnipping.Text = "Freeze screen while snipping";
			this.cbFreezeScreenWhileSnipping.UseVisualStyleBackColor = true;
			this.cbFreezeScreenWhileSnipping.CheckedChanged += new System.EventHandler(this.cbFreezeScreenWhileSnipping_CheckedChanged);
			// 
			// Options
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(398, 415);
			this.Controls.Add(this.cbFreezeScreenWhileSnipping);
			this.Controls.Add(this.btnLogin);
			this.Controls.Add(this.lblAuthenticationStatus);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "Options";
			this.Text = "ImgurWin Options";
			this.Load += new System.EventHandler(this.Options_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblAuthenticationStatus;
		private System.Windows.Forms.Button btnLogin;
		private System.Windows.Forms.CheckBox cbFreezeScreenWhileSnipping;
	}
}