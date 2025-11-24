namespace Twitch_prime_downloader
{
	partial class VodFrame
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.lblVodTitle = new System.Windows.Forms.Label();
			this.lblChannelName = new System.Windows.Forms.Label();
			this.lblGameName = new System.Windows.Forms.Label();
			this.btnDownload = new System.Windows.Forms.Button();
			this.contextMenuVodThumbnail = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miCopyVodTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miCopyVodCreationDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miCopyVodTitlePlusCreationDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lblMutedChunks = new System.Windows.Forms.Label();
			this.lblIsPrime = new System.Windows.Forms.Label();
			this.pictureBoxThumbnailImageGame = new System.Windows.Forms.PictureBox();
			this.pictureBoxThumbnailImageVod = new System.Windows.Forms.PictureBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.lblBroadcastType = new System.Windows.Forms.Label();
			this.contextMenuVodThumbnail.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnailImageGame)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnailImageVod)).BeginInit();
			this.SuspendLayout();
			// 
			// lblVodTitle
			// 
			this.lblVodTitle.BackColor = System.Drawing.Color.Black;
			this.lblVodTitle.ForeColor = System.Drawing.Color.White;
			this.lblVodTitle.Location = new System.Drawing.Point(72, 196);
			this.lblVodTitle.Name = "lblVodTitle";
			this.lblVodTitle.Size = new System.Drawing.Size(295, 40);
			this.lblVodTitle.TabIndex = 0;
			this.lblVodTitle.Text = "lblStreamTitle";
			this.lblVodTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblVodTitle_MouseDown);
			// 
			// lblChannelName
			// 
			this.lblChannelName.AutoSize = true;
			this.lblChannelName.ForeColor = System.Drawing.Color.White;
			this.lblChannelName.Location = new System.Drawing.Point(72, 255);
			this.lblChannelName.Name = "lblChannelName";
			this.lblChannelName.Size = new System.Drawing.Size(84, 13);
			this.lblChannelName.TabIndex = 3;
			this.lblChannelName.Text = "lblChannelName";
			this.lblChannelName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.vodFrame_MouseDown);
			// 
			// lblGameName
			// 
			this.lblGameName.AutoSize = true;
			this.lblGameName.ForeColor = System.Drawing.Color.Yellow;
			this.lblGameName.Location = new System.Drawing.Point(72, 238);
			this.lblGameName.Name = "lblGameName";
			this.lblGameName.Size = new System.Drawing.Size(73, 13);
			this.lblGameName.TabIndex = 4;
			this.lblGameName.Text = "lblGameName";
			this.lblGameName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.vodFrame_MouseDown);
			// 
			// btnDownload
			// 
			this.btnDownload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.btnDownload.ForeColor = System.Drawing.Color.White;
			this.btnDownload.Location = new System.Drawing.Point(290, 255);
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.Size = new System.Drawing.Size(77, 22);
			this.btnDownload.TabIndex = 5;
			this.btnDownload.Text = "Скачать";
			this.btnDownload.UseVisualStyleBackColor = false;
			this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
			this.btnDownload.Paint += new System.Windows.Forms.PaintEventHandler(this.btnDownload_Paint);
			this.btnDownload.MouseDown += new System.Windows.Forms.MouseEventHandler(this.vodFrame_MouseDown);
			// 
			// contextMenuVodThumbnail
			// 
			this.contextMenuVodThumbnail.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.miCopyVodTitleToolStripMenuItem,
			this.miCopyVodCreationDateToolStripMenuItem,
			this.miCopyVodTitlePlusCreationDateToolStripMenuItem});
			this.contextMenuVodThumbnail.Name = "contextMenuStrip1";
			this.contextMenuVodThumbnail.Size = new System.Drawing.Size(236, 70);
			// 
			// miCopyVodTitleToolStripMenuItem
			// 
			this.miCopyVodTitleToolStripMenuItem.Name = "miCopyVodTitleToolStripMenuItem";
			this.miCopyVodTitleToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
			this.miCopyVodTitleToolStripMenuItem.Text = "Скопировать название";
			this.miCopyVodTitleToolStripMenuItem.Click += new System.EventHandler(this.miCopyVodTitleToolStripMenuItem_Click);
			// 
			// miCopyVodCreationDateToolStripMenuItem
			// 
			this.miCopyVodCreationDateToolStripMenuItem.Name = "miCopyVodCreationDateToolStripMenuItem";
			this.miCopyVodCreationDateToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
			this.miCopyVodCreationDateToolStripMenuItem.Text = "Скопировать дату";
			this.miCopyVodCreationDateToolStripMenuItem.Click += new System.EventHandler(this.miCopyVodCreationDateToolStripMenuItem_Click);
			// 
			// miCopyVodTitlePlusCreationDateToolStripMenuItem
			// 
			this.miCopyVodTitlePlusCreationDateToolStripMenuItem.Name = "miCopyVodTitlePlusCreationDateToolStripMenuItem";
			this.miCopyVodTitlePlusCreationDateToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
			this.miCopyVodTitlePlusCreationDateToolStripMenuItem.Text = "Скопировать дату и название";
			this.miCopyVodTitlePlusCreationDateToolStripMenuItem.Click += new System.EventHandler(this.miCopyVodTitlePlusCreationDateToolStripMenuItem_Click);
			// 
			// lblMutedChunks
			// 
			this.lblMutedChunks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lblMutedChunks.AutoSize = true;
			this.lblMutedChunks.ForeColor = System.Drawing.Color.White;
			this.lblMutedChunks.Location = new System.Drawing.Point(287, 280);
			this.lblMutedChunks.Name = "lblMutedChunks";
			this.lblMutedChunks.Size = new System.Drawing.Size(83, 13);
			this.lblMutedChunks.TabIndex = 6;
			this.lblMutedChunks.Text = "lblMutedChunks";
			this.lblMutedChunks.Visible = false;
			this.lblMutedChunks.DoubleClick += new System.EventHandler(this.lblMutedChunks_DoubleClick);
			this.lblMutedChunks.MouseDown += new System.Windows.Forms.MouseEventHandler(this.vodFrame_MouseDown);
			// 
			// lblIsPrime
			// 
			this.lblIsPrime.AutoSize = true;
			this.lblIsPrime.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblIsPrime.ForeColor = System.Drawing.Color.Lime;
			this.lblIsPrime.Location = new System.Drawing.Point(352, 6);
			this.lblIsPrime.Name = "lblIsPrime";
			this.lblIsPrime.Size = new System.Drawing.Size(24, 26);
			this.lblIsPrime.TabIndex = 7;
			this.lblIsPrime.Text = "$";
			this.toolTip1.SetToolTip(this.lblIsPrime, "Очень платное видео");
			this.lblIsPrime.Visible = false;
			this.lblIsPrime.MouseDown += new System.Windows.Forms.MouseEventHandler(this.vodFrame_MouseDown);
			// 
			// pictureBoxThumbnailImageGame
			// 
			this.pictureBoxThumbnailImageGame.Location = new System.Drawing.Point(10, 196);
			this.pictureBoxThumbnailImageGame.Name = "pictureBoxThumbnailImageGame";
			this.pictureBoxThumbnailImageGame.Size = new System.Drawing.Size(52, 72);
			this.pictureBoxThumbnailImageGame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxThumbnailImageGame.TabIndex = 2;
			this.pictureBoxThumbnailImageGame.TabStop = false;
			this.pictureBoxThumbnailImageGame.MouseDown += new System.Windows.Forms.MouseEventHandler(this.vodFrame_MouseDown);
			// 
			// pictureBoxThumbnailImageVod
			// 
			this.pictureBoxThumbnailImageVod.Location = new System.Drawing.Point(27, 10);
			this.pictureBoxThumbnailImageVod.Name = "pictureBoxThumbnailImageVod";
			this.pictureBoxThumbnailImageVod.Size = new System.Drawing.Size(320, 180);
			this.pictureBoxThumbnailImageVod.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxThumbnailImageVod.TabIndex = 1;
			this.pictureBoxThumbnailImageVod.TabStop = false;
			this.pictureBoxThumbnailImageVod.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxThumbnailImageVod_Paint);
			this.pictureBoxThumbnailImageVod.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxThumbnailImageVod_MouseDown);
			// 
			// lblBroadcastType
			// 
			this.lblBroadcastType.AutoSize = true;
			this.lblBroadcastType.ForeColor = System.Drawing.Color.White;
			this.lblBroadcastType.Location = new System.Drawing.Point(7, 280);
			this.lblBroadcastType.Name = "lblBroadcastType";
			this.lblBroadcastType.Size = new System.Drawing.Size(89, 13);
			this.lblBroadcastType.TabIndex = 8;
			this.lblBroadcastType.Text = "lblBroadcastType";
			this.lblBroadcastType.MouseDown += new System.Windows.Forms.MouseEventHandler(this.vodFrame_MouseDown);
			// 
			// VodFrame
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
			this.Controls.Add(this.lblBroadcastType);
			this.Controls.Add(this.lblIsPrime);
			this.Controls.Add(this.lblMutedChunks);
			this.Controls.Add(this.btnDownload);
			this.Controls.Add(this.lblGameName);
			this.Controls.Add(this.lblChannelName);
			this.Controls.Add(this.pictureBoxThumbnailImageGame);
			this.Controls.Add(this.pictureBoxThumbnailImageVod);
			this.Controls.Add(this.lblVodTitle);
			this.Name = "VodFrame";
			this.Size = new System.Drawing.Size(380, 296);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.vodFrame_MouseDown);
			this.contextMenuVodThumbnail.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnailImageGame)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxThumbnailImageVod)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.Label lblVodTitle;
		public System.Windows.Forms.PictureBox pictureBoxThumbnailImageVod;
		public System.Windows.Forms.PictureBox pictureBoxThumbnailImageGame;
		public System.Windows.Forms.Label lblChannelName;
		public System.Windows.Forms.Label lblGameName;
		public System.Windows.Forms.Button btnDownload;
		private System.Windows.Forms.ContextMenuStrip contextMenuVodThumbnail;
		private System.Windows.Forms.ToolStripMenuItem miCopyVodTitleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem miCopyVodCreationDateToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem miCopyVodTitlePlusCreationDateToolStripMenuItem;
		private System.Windows.Forms.Label lblMutedChunks;
		private System.Windows.Forms.Label lblIsPrime;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label lblBroadcastType;
	}
}
