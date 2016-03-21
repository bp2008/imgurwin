namespace ImgurWin
{
	partial class PinEntry
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
			this.txtPIN = new System.Windows.Forms.TextBox();
			this.btnDone = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(201, 26);
			this.label1.TabIndex = 0;
			this.label1.Text = "When you have your Imgur authorization \r\nPIN, enter it here:";
			// 
			// txtPIN
			// 
			this.txtPIN.Location = new System.Drawing.Point(12, 43);
			this.txtPIN.Name = "txtPIN";
			this.txtPIN.Size = new System.Drawing.Size(149, 20);
			this.txtPIN.TabIndex = 1;
			// 
			// btnDone
			// 
			this.btnDone.Location = new System.Drawing.Point(167, 41);
			this.btnDone.Name = "btnDone";
			this.btnDone.Size = new System.Drawing.Size(73, 23);
			this.btnDone.TabIndex = 2;
			this.btnDone.Text = "Done";
			this.btnDone.UseVisualStyleBackColor = true;
			this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
			// 
			// PinEntry
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(252, 75);
			this.Controls.Add(this.btnDone);
			this.Controls.Add(this.txtPIN);
			this.Controls.Add(this.label1);
			this.Name = "PinEntry";
			this.Text = "ImgurWin PIN Entry";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtPIN;
		private System.Windows.Forms.Button btnDone;
	}
}