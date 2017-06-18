namespace ImgurWin
{
	partial class Main
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
			this.btnOpenFiles = new System.Windows.Forms.Button();
			this.btnSnip = new System.Windows.Forms.Button();
			this.btnWindowSelect = new System.Windows.Forms.Button();
			this.btnPaste = new System.Windows.Forms.Button();
			this.btnOptions = new System.Windows.Forms.Button();
			this.btnCaptures = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.pbImage = new System.Windows.Forms.PictureBox();
			this.contextMenuStrip_pictureBox = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_deleteLastImage = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_deleteAllLastOperationImages = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.status_progressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.status_label = new System.Windows.Forms.ToolStripStatusLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.lblAccountName = new System.Windows.Forms.Label();
			this.lblImages = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lblTokens = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.btnFavoriteLink1 = new System.Windows.Forms.Button();
			this.btnFavoriteLink2 = new System.Windows.Forms.Button();
			this.btnFavoriteLink3 = new System.Windows.Forms.Button();
			this.btnFavoriteLink4 = new System.Windows.Forms.Button();
			this.cbHighQuality = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
			this.contextMenuStrip_pictureBox.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOpenFiles
			// 
			this.btnOpenFiles.Enabled = false;
			this.btnOpenFiles.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenFiles.Image")));
			this.btnOpenFiles.Location = new System.Drawing.Point(0, 0);
			this.btnOpenFiles.Name = "btnOpenFiles";
			this.btnOpenFiles.Size = new System.Drawing.Size(64, 64);
			this.btnOpenFiles.TabIndex = 0;
			this.btnOpenFiles.Tag = "";
			this.toolTip1.SetToolTip(this.btnOpenFiles, "Open files from the disk");
			this.btnOpenFiles.UseVisualStyleBackColor = true;
			this.btnOpenFiles.Click += new System.EventHandler(this.btnOpenFiles_Click);
			// 
			// btnSnip
			// 
			this.btnSnip.Enabled = false;
			this.btnSnip.Image = ((System.Drawing.Image)(resources.GetObject("btnSnip.Image")));
			this.btnSnip.Location = new System.Drawing.Point(64, 0);
			this.btnSnip.Name = "btnSnip";
			this.btnSnip.Size = new System.Drawing.Size(64, 64);
			this.btnSnip.TabIndex = 1;
			this.btnSnip.Tag = "";
			this.toolTip1.SetToolTip(this.btnSnip, "Capture a screen region by drawing a rectangle");
			this.btnSnip.UseVisualStyleBackColor = true;
			this.btnSnip.Click += new System.EventHandler(this.btnSnip_Click);
			// 
			// btnWindowSelect
			// 
			this.btnWindowSelect.Enabled = false;
			this.btnWindowSelect.Image = ((System.Drawing.Image)(resources.GetObject("btnWindowSelect.Image")));
			this.btnWindowSelect.Location = new System.Drawing.Point(128, 0);
			this.btnWindowSelect.Name = "btnWindowSelect";
			this.btnWindowSelect.Size = new System.Drawing.Size(64, 64);
			this.btnWindowSelect.TabIndex = 2;
			this.btnWindowSelect.Tag = "";
			this.toolTip1.SetToolTip(this.btnWindowSelect, "Screenshot the content of a window");
			this.btnWindowSelect.UseVisualStyleBackColor = true;
			// 
			// btnPaste
			// 
			this.btnPaste.Enabled = false;
			this.btnPaste.Location = new System.Drawing.Point(192, 0);
			this.btnPaste.Name = "btnPaste";
			this.btnPaste.Size = new System.Drawing.Size(64, 64);
			this.btnPaste.TabIndex = 3;
			this.btnPaste.Tag = "";
			this.btnPaste.Text = "Paste";
			this.toolTip1.SetToolTip(this.btnPaste, "Paste an image from the clipboard");
			this.btnPaste.UseVisualStyleBackColor = true;
			this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
			// 
			// btnOptions
			// 
			this.btnOptions.Location = new System.Drawing.Point(296, 0);
			this.btnOptions.Name = "btnOptions";
			this.btnOptions.Size = new System.Drawing.Size(64, 64);
			this.btnOptions.TabIndex = 4;
			this.btnOptions.Text = "Options";
			this.btnOptions.UseVisualStyleBackColor = true;
			this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
			// 
			// btnCaptures
			// 
			this.btnCaptures.Location = new System.Drawing.Point(360, 0);
			this.btnCaptures.Name = "btnCaptures";
			this.btnCaptures.Size = new System.Drawing.Size(64, 64);
			this.btnCaptures.TabIndex = 5;
			this.btnCaptures.Text = "Captures";
			this.toolTip1.SetToolTip(this.btnCaptures, "Open the Captures directory where \r\nevery screenshot you capture with this \r\nprog" +
        "ram is automatically stored");
			this.btnCaptures.UseVisualStyleBackColor = true;
			this.btnCaptures.Click += new System.EventHandler(this.btnCaptures_Click);
			// 
			// pbImage
			// 
			this.pbImage.ContextMenuStrip = this.contextMenuStrip_pictureBox;
			this.pbImage.Location = new System.Drawing.Point(0, 116);
			this.pbImage.Name = "pbImage";
			this.pbImage.Size = new System.Drawing.Size(424, 303);
			this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pbImage.TabIndex = 6;
			this.pbImage.TabStop = false;
			this.pbImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbImage_MouseDown);
			// 
			// contextMenuStrip_pictureBox
			// 
			this.contextMenuStrip_pictureBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_deleteLastImage,
            this.toolStripMenuItem_deleteAllLastOperationImages});
			this.contextMenuStrip_pictureBox.Name = "contextMenuStrip_pictureBox";
			this.contextMenuStrip_pictureBox.Size = new System.Drawing.Size(178, 48);
			this.contextMenuStrip_pictureBox.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_pictureBox_Opening);
			this.contextMenuStrip_pictureBox.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip_pictureBox_ItemClicked);
			// 
			// toolStripMenuItem_deleteLastImage
			// 
			this.toolStripMenuItem_deleteLastImage.Name = "toolStripMenuItem_deleteLastImage";
			this.toolStripMenuItem_deleteLastImage.Size = new System.Drawing.Size(177, 22);
			this.toolStripMenuItem_deleteLastImage.Text = "Delete Last Image";
			// 
			// toolStripMenuItem_deleteAllLastOperationImages
			// 
			this.toolStripMenuItem_deleteAllLastOperationImages.Name = "toolStripMenuItem_deleteAllLastOperationImages";
			this.toolStripMenuItem_deleteAllLastOperationImages.Size = new System.Drawing.Size(177, 22);
			this.toolStripMenuItem_deleteAllLastOperationImages.Text = "Delete All N Images";
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.Filter = "Pictures (jpg,png,gif,bmp)|*.jpg;*.png;*.gif;*.bmp;*.jpeg";
			this.openFileDialog1.Multiselect = true;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status_progressBar,
            this.status_label});
			this.statusStrip1.Location = new System.Drawing.Point(0, 419);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(424, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 20;
			// 
			// status_progressBar
			// 
			this.status_progressBar.Name = "status_progressBar";
			this.status_progressBar.Size = new System.Drawing.Size(100, 16);
			// 
			// status_label
			// 
			this.status_label.AutoSize = false;
			this.status_label.Name = "status_label";
			this.status_label.Size = new System.Drawing.Size(320, 17);
			this.status_label.Text = "Ready";
			this.status_label.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 67);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Account:";
			// 
			// lblAccountName
			// 
			this.lblAccountName.AutoEllipsis = true;
			this.lblAccountName.Location = new System.Drawing.Point(62, 67);
			this.lblAccountName.Name = "lblAccountName";
			this.lblAccountName.Size = new System.Drawing.Size(100, 13);
			this.lblAccountName.TabIndex = 9;
			this.lblAccountName.Text = "...";
			// 
			// lblImages
			// 
			this.lblImages.AutoEllipsis = true;
			this.lblImages.Location = new System.Drawing.Point(62, 80);
			this.lblImages.Name = "lblImages";
			this.lblImages.Size = new System.Drawing.Size(100, 13);
			this.lblImages.TabIndex = 11;
			this.lblImages.Text = "...";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(18, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(44, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Images:";
			// 
			// lblTokens
			// 
			this.lblTokens.AutoEllipsis = true;
			this.lblTokens.Location = new System.Drawing.Point(62, 93);
			this.lblTokens.Name = "lblTokens";
			this.lblTokens.Size = new System.Drawing.Size(100, 13);
			this.lblTokens.TabIndex = 13;
			this.lblTokens.Text = "...";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(0, 93);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(62, 13);
			this.label5.TabIndex = 12;
			this.label5.Text = "API Credits:";
			// 
			// btnFavoriteLink1
			// 
			this.btnFavoriteLink1.Enabled = false;
			this.btnFavoriteLink1.Location = new System.Drawing.Point(168, 67);
			this.btnFavoriteLink1.Name = "btnFavoriteLink1";
			this.btnFavoriteLink1.Size = new System.Drawing.Size(64, 44);
			this.btnFavoriteLink1.TabIndex = 14;
			this.btnFavoriteLink1.Tag = "";
			this.btnFavoriteLink1.Text = "Favorite Link 1";
			this.btnFavoriteLink1.UseVisualStyleBackColor = true;
			// 
			// btnFavoriteLink2
			// 
			this.btnFavoriteLink2.Enabled = false;
			this.btnFavoriteLink2.Location = new System.Drawing.Point(232, 67);
			this.btnFavoriteLink2.Name = "btnFavoriteLink2";
			this.btnFavoriteLink2.Size = new System.Drawing.Size(64, 44);
			this.btnFavoriteLink2.TabIndex = 16;
			this.btnFavoriteLink2.Tag = "";
			this.btnFavoriteLink2.Text = "Favorite Link 2";
			this.btnFavoriteLink2.UseVisualStyleBackColor = true;
			// 
			// btnFavoriteLink3
			// 
			this.btnFavoriteLink3.Enabled = false;
			this.btnFavoriteLink3.Location = new System.Drawing.Point(296, 67);
			this.btnFavoriteLink3.Name = "btnFavoriteLink3";
			this.btnFavoriteLink3.Size = new System.Drawing.Size(64, 44);
			this.btnFavoriteLink3.TabIndex = 17;
			this.btnFavoriteLink3.Tag = "";
			this.btnFavoriteLink3.Text = "Favorite Link 3";
			this.btnFavoriteLink3.UseVisualStyleBackColor = true;
			// 
			// btnFavoriteLink4
			// 
			this.btnFavoriteLink4.Enabled = false;
			this.btnFavoriteLink4.Location = new System.Drawing.Point(360, 67);
			this.btnFavoriteLink4.Name = "btnFavoriteLink4";
			this.btnFavoriteLink4.Size = new System.Drawing.Size(64, 44);
			this.btnFavoriteLink4.TabIndex = 18;
			this.btnFavoriteLink4.Tag = "";
			this.btnFavoriteLink4.Text = "Favorite Link 4";
			this.btnFavoriteLink4.UseVisualStyleBackColor = true;
			// 
			// cbHighQuality
			// 
			this.cbHighQuality.Location = new System.Drawing.Point(259, 0);
			this.cbHighQuality.Name = "cbHighQuality";
			this.cbHighQuality.Size = new System.Drawing.Size(35, 64);
			this.cbHighQuality.TabIndex = 21;
			this.cbHighQuality.Tag = "";
			this.cbHighQuality.Text = "HQ";
			this.toolTip1.SetToolTip(this.cbHighQuality, "In High Quality mode, image size is sacrificed in exchange for higher quality.");
			this.cbHighQuality.UseVisualStyleBackColor = true;
			this.cbHighQuality.CheckedChanged += new System.EventHandler(this.cbHighQuality_CheckedChanged);
			// 
			// Main
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(424, 441);
			this.Controls.Add(this.cbHighQuality);
			this.Controls.Add(this.btnFavoriteLink4);
			this.Controls.Add(this.btnFavoriteLink3);
			this.Controls.Add(this.btnFavoriteLink2);
			this.Controls.Add(this.btnFavoriteLink1);
			this.Controls.Add(this.lblTokens);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.lblImages);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.lblAccountName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.pbImage);
			this.Controls.Add(this.btnCaptures);
			this.Controls.Add(this.btnOptions);
			this.Controls.Add(this.btnPaste);
			this.Controls.Add(this.btnWindowSelect);
			this.Controls.Add(this.btnSnip);
			this.Controls.Add(this.btnOpenFiles);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(440, 480);
			this.MinimumSize = new System.Drawing.Size(440, 480);
			this.Name = "Main";
			this.Text = "ImgurWin";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
			this.Load += new System.EventHandler(this.Main_Load);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Main_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Main_DragEnter);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
			this.contextMenuStrip_pictureBox.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOpenFiles;
		private System.Windows.Forms.Button btnSnip;
		private System.Windows.Forms.Button btnWindowSelect;
		private System.Windows.Forms.Button btnPaste;
		private System.Windows.Forms.Button btnOptions;
		private System.Windows.Forms.Button btnCaptures;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.PictureBox pbImage;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblAccountName;
		private System.Windows.Forms.Label lblImages;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label lblTokens;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnFavoriteLink1;
		private System.Windows.Forms.Button btnFavoriteLink2;
		private System.Windows.Forms.Button btnFavoriteLink3;
		private System.Windows.Forms.Button btnFavoriteLink4;
		public System.Windows.Forms.ToolStripStatusLabel status_label;
		public System.Windows.Forms.ToolStripProgressBar status_progressBar;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_pictureBox;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_deleteLastImage;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_deleteAllLastOperationImages;
		private System.Windows.Forms.CheckBox cbHighQuality;
	}
}

