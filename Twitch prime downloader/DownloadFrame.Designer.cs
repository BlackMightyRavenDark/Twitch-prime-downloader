namespace Twitch_prime_downloader
{
	partial class DownloadFrame
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
			this.btnCloseFrame = new System.Windows.Forms.Button();
			this.lblVodTitle = new System.Windows.Forms.Label();
			this.lblOutputFileName = new System.Windows.Forms.Label();
			this.lblProgressOverall = new System.Windows.Forms.Label();
			this.btnStartDownload = new System.Windows.Forms.Button();
			this.btnStopDownload = new System.Windows.Forms.Button();
			this.groupBoxDownloadVodChunkRange = new System.Windows.Forms.GroupBox();
			this.btnSetMaxChunkTo = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxChunkTo = new System.Windows.Forms.TextBox();
			this.textBoxChunkFrom = new System.Windows.Forms.TextBox();
			this.contextMenuVodTitle = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miCopyVodTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.listBoxChunkFileList = new System.Windows.Forms.ListBox();
			this.lblChunkFileList = new System.Windows.Forms.Label();
			this.lblProgressChunkGroup = new System.Windows.Forms.Label();
			this.groupBoxDownloadMode = new System.Windows.Forms.GroupBox();
			this.radioButtonDownloadChunksSeparately = new System.Windows.Forms.RadioButton();
			this.radioButtonDownloadSingleBigVideoFile = new System.Windows.Forms.RadioButton();
			this.timerElapsedTime = new System.Windows.Forms.Timer(this.components);
			this.lblElapsedTime = new System.Windows.Forms.Label();
			this.timerAnimation = new System.Windows.Forms.Timer(this.components);
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.pictureBoxAnimation = new System.Windows.Forms.PictureBox();
			this.pictureBoxScrollBar = new System.Windows.Forms.PictureBox();
			this.pictureBoxVodThumbnailImage = new System.Windows.Forms.PictureBox();
			this.btnCopyVodChunkUrlList = new System.Windows.Forms.Button();
			this.multipleProgressBarOverall = new Twitch_prime_downloader.MultipleProgressBar();
			this.multipleProgressBarChunkGroup = new Twitch_prime_downloader.MultipleProgressBar();
			this.contextMenuProgressBarChunkGroup = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miIncreaseChunkGroupSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miDecreaseChunkGroupSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBoxDownloadVodChunkRange.SuspendLayout();
			this.contextMenuVodTitle.SuspendLayout();
			this.groupBoxDownloadMode.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxScrollBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxVodThumbnailImage)).BeginInit();
			this.contextMenuProgressBarChunkGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnCloseFrame
			// 
			this.btnCloseFrame.Location = new System.Drawing.Point(592, 3);
			this.btnCloseFrame.Name = "btnCloseFrame";
			this.btnCloseFrame.Size = new System.Drawing.Size(75, 23);
			this.btnCloseFrame.TabIndex = 0;
			this.btnCloseFrame.Text = "Закрыть";
			this.btnCloseFrame.UseVisualStyleBackColor = true;
			this.btnCloseFrame.Click += new System.EventHandler(this.btnCloseFrame_Click);
			// 
			// lblVodTitle
			// 
			this.lblVodTitle.Location = new System.Drawing.Point(3, 3);
			this.lblVodTitle.Name = "lblVodTitle";
			this.lblVodTitle.Size = new System.Drawing.Size(516, 50);
			this.lblVodTitle.TabIndex = 1;
			this.lblVodTitle.Text = "lblVodTitle";
			this.lblVodTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblVodTitle_MouseUp);
			// 
			// lblOutputFileName
			// 
			this.lblOutputFileName.Location = new System.Drawing.Point(3, 57);
			this.lblOutputFileName.Name = "lblOutputFileName";
			this.lblOutputFileName.Size = new System.Drawing.Size(516, 57);
			this.lblOutputFileName.TabIndex = 3;
			this.lblOutputFileName.Text = "lblOutputFileName";
			// 
			// lblProgressOverall
			// 
			this.lblProgressOverall.AutoSize = true;
			this.lblProgressOverall.Location = new System.Drawing.Point(3, 182);
			this.lblProgressOverall.Name = "lblProgressOverall";
			this.lblProgressOverall.Size = new System.Drawing.Size(91, 13);
			this.lblProgressOverall.TabIndex = 8;
			this.lblProgressOverall.Text = "lblProgressOverall";
			// 
			// btnStartDownload
			// 
			this.btnStartDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnStartDownload.Location = new System.Drawing.Point(472, 273);
			this.btnStartDownload.Name = "btnStartDownload";
			this.btnStartDownload.Size = new System.Drawing.Size(90, 23);
			this.btnStartDownload.TabIndex = 9;
			this.btnStartDownload.Text = "Запустиська";
			this.btnStartDownload.UseVisualStyleBackColor = true;
			this.btnStartDownload.Click += new System.EventHandler(this.btnStartDownload_Click);
			// 
			// btnStopDownload
			// 
			this.btnStopDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnStopDownload.Location = new System.Drawing.Point(568, 273);
			this.btnStopDownload.Name = "btnStopDownload";
			this.btnStopDownload.Size = new System.Drawing.Size(99, 23);
			this.btnStopDownload.TabIndex = 10;
			this.btnStopDownload.Text = "Остановиська";
			this.btnStopDownload.UseVisualStyleBackColor = true;
			this.btnStopDownload.Click += new System.EventHandler(this.btnStopDownload_Click);
			// 
			// groupBoxDownloadVodChunkRange
			// 
			this.groupBoxDownloadVodChunkRange.Controls.Add(this.btnSetMaxChunkTo);
			this.groupBoxDownloadVodChunkRange.Controls.Add(this.label2);
			this.groupBoxDownloadVodChunkRange.Controls.Add(this.label1);
			this.groupBoxDownloadVodChunkRange.Controls.Add(this.textBoxChunkTo);
			this.groupBoxDownloadVodChunkRange.Controls.Add(this.textBoxChunkFrom);
			this.groupBoxDownloadVodChunkRange.Location = new System.Drawing.Point(673, 65);
			this.groupBoxDownloadVodChunkRange.Name = "groupBoxDownloadVodChunkRange";
			this.groupBoxDownloadVodChunkRange.Size = new System.Drawing.Size(219, 83);
			this.groupBoxDownloadVodChunkRange.TabIndex = 11;
			this.groupBoxDownloadVodChunkRange.TabStop = false;
			this.groupBoxDownloadVodChunkRange.Text = "Диапазон скачивания";
			// 
			// btnSetMaxChunkTo
			// 
			this.btnSetMaxChunkTo.Location = new System.Drawing.Point(162, 55);
			this.btnSetMaxChunkTo.Name = "btnSetMaxChunkTo";
			this.btnSetMaxChunkTo.Size = new System.Drawing.Size(37, 20);
			this.btnSetMaxChunkTo.TabIndex = 4;
			this.btnSetMaxChunkTo.Text = "max";
			this.btnSetMaxChunkTo.UseVisualStyleBackColor = true;
			this.btnSetMaxChunkTo.Click += new System.EventHandler(this.btnSetMaxChunkTo_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 57);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(92, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Последний чанк:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Первый чанк:";
			// 
			// textBoxChunkTo
			// 
			this.textBoxChunkTo.Location = new System.Drawing.Point(112, 55);
			this.textBoxChunkTo.Name = "textBoxChunkTo";
			this.textBoxChunkTo.Size = new System.Drawing.Size(44, 20);
			this.textBoxChunkTo.TabIndex = 1;
			this.textBoxChunkTo.Text = "10";
			this.textBoxChunkTo.Leave += new System.EventHandler(this.textBoxChunkTo_Leave);
			// 
			// textBoxChunkFrom
			// 
			this.textBoxChunkFrom.Location = new System.Drawing.Point(112, 23);
			this.textBoxChunkFrom.Name = "textBoxChunkFrom";
			this.textBoxChunkFrom.Size = new System.Drawing.Size(44, 20);
			this.textBoxChunkFrom.TabIndex = 0;
			this.textBoxChunkFrom.Text = "1";
			this.textBoxChunkFrom.Leave += new System.EventHandler(this.textBoxChunkFrom_Leave);
			// 
			// contextMenuVodTitle
			// 
			this.contextMenuVodTitle.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.miCopyVodTitleToolStripMenuItem});
			this.contextMenuVodTitle.Name = "contextMenuStreamTitle";
			this.contextMenuVodTitle.Size = new System.Drawing.Size(200, 26);
			// 
			// miCopyVodTitleToolStripMenuItem
			// 
			this.miCopyVodTitleToolStripMenuItem.Name = "miCopyVodTitleToolStripMenuItem";
			this.miCopyVodTitleToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
			this.miCopyVodTitleToolStripMenuItem.Text = "Скопировать название";
			this.miCopyVodTitleToolStripMenuItem.Click += new System.EventHandler(this.miCopyVodTitleToolStripMenuItem_Click);
			// 
			// listBoxChunkFileList
			// 
			this.listBoxChunkFileList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.listBoxChunkFileList.FormattingEnabled = true;
			this.listBoxChunkFileList.Location = new System.Drawing.Point(898, 19);
			this.listBoxChunkFileList.Name = "listBoxChunkFileList";
			this.listBoxChunkFileList.Size = new System.Drawing.Size(174, 212);
			this.listBoxChunkFileList.TabIndex = 13;
			this.listBoxChunkFileList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBoxChunkFileList_DrawItem);
			// 
			// lblChunkFileList
			// 
			this.lblChunkFileList.AutoSize = true;
			this.lblChunkFileList.Location = new System.Drawing.Point(895, 3);
			this.lblChunkFileList.Name = "lblChunkFileList";
			this.lblChunkFileList.Size = new System.Drawing.Size(130, 13);
			this.lblChunkFileList.TabIndex = 14;
			this.lblChunkFileList.Text = "Файлы для скачивания:";
			// 
			// lblProgressChunkGroup
			// 
			this.lblProgressChunkGroup.AutoSize = true;
			this.lblProgressChunkGroup.Location = new System.Drawing.Point(3, 114);
			this.lblProgressChunkGroup.Name = "lblProgressChunkGroup";
			this.lblProgressChunkGroup.Size = new System.Drawing.Size(118, 13);
			this.lblProgressChunkGroup.TabIndex = 15;
			this.lblProgressChunkGroup.Text = "lblProgressChunkGroup";
			// 
			// groupBoxDownloadMode
			// 
			this.groupBoxDownloadMode.Controls.Add(this.radioButtonDownloadChunksSeparately);
			this.groupBoxDownloadMode.Controls.Add(this.radioButtonDownloadSingleBigVideoFile);
			this.groupBoxDownloadMode.Location = new System.Drawing.Point(673, 3);
			this.groupBoxDownloadMode.Name = "groupBoxDownloadMode";
			this.groupBoxDownloadMode.Size = new System.Drawing.Size(218, 56);
			this.groupBoxDownloadMode.TabIndex = 16;
			this.groupBoxDownloadMode.TabStop = false;
			this.groupBoxDownloadMode.Text = "Режим скачивания";
			// 
			// radioButtonDownloadChunksSeparately
			// 
			this.radioButtonDownloadChunksSeparately.AutoSize = true;
			this.radioButtonDownloadChunksSeparately.Location = new System.Drawing.Point(17, 33);
			this.radioButtonDownloadChunksSeparately.Name = "radioButtonDownloadChunksSeparately";
			this.radioButtonDownloadChunksSeparately.Size = new System.Drawing.Size(188, 17);
			this.radioButtonDownloadChunksSeparately.TabIndex = 1;
			this.radioButtonDownloadChunksSeparately.TabStop = true;
			this.radioButtonDownloadChunksSeparately.Text = "Каждый чанк в отдельный файл";
			this.radioButtonDownloadChunksSeparately.UseVisualStyleBackColor = true;
			this.radioButtonDownloadChunksSeparately.CheckedChanged += new System.EventHandler(this.radioButtonDownloadChunksSeparately_CheckedChanged);
			// 
			// radioButtonDownloadSingleBigVideoFile
			// 
			this.radioButtonDownloadSingleBigVideoFile.AutoSize = true;
			this.radioButtonDownloadSingleBigVideoFile.Checked = true;
			this.radioButtonDownloadSingleBigVideoFile.Location = new System.Drawing.Point(17, 16);
			this.radioButtonDownloadSingleBigVideoFile.Name = "radioButtonDownloadSingleBigVideoFile";
			this.radioButtonDownloadSingleBigVideoFile.Size = new System.Drawing.Size(127, 17);
			this.radioButtonDownloadSingleBigVideoFile.TabIndex = 0;
			this.radioButtonDownloadSingleBigVideoFile.TabStop = true;
			this.radioButtonDownloadSingleBigVideoFile.Text = "Один большой файл";
			this.radioButtonDownloadSingleBigVideoFile.UseVisualStyleBackColor = true;
			this.radioButtonDownloadSingleBigVideoFile.CheckedChanged += new System.EventHandler(this.radioButtonDownloadSingleBigVideoFile_CheckedChanged);
			// 
			// timerElapsedTime
			// 
			this.timerElapsedTime.Interval = 1000;
			this.timerElapsedTime.Tick += new System.EventHandler(this.timerElapsedTime_Tick);
			// 
			// lblElapsedTime
			// 
			this.lblElapsedTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblElapsedTime.AutoSize = true;
			this.lblElapsedTime.Location = new System.Drawing.Point(3, 280);
			this.lblElapsedTime.Name = "lblElapsedTime";
			this.lblElapsedTime.Size = new System.Drawing.Size(78, 13);
			this.lblElapsedTime.TabIndex = 17;
			this.lblElapsedTime.Text = "lblElapsedTime";
			// 
			// timerAnimation
			// 
			this.timerAnimation.Tick += new System.EventHandler(this.timerAnimation_Tick);
			// 
			// pictureBoxAnimation
			// 
			this.pictureBoxAnimation.Image = global::Twitch_prime_downloader.Properties.Resources.fcst_istra_01;
			this.pictureBoxAnimation.Location = new System.Drawing.Point(6, 198);
			this.pictureBoxAnimation.Name = "pictureBoxAnimation";
			this.pictureBoxAnimation.Size = new System.Drawing.Size(70, 70);
			this.pictureBoxAnimation.TabIndex = 18;
			this.pictureBoxAnimation.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureBoxAnimation, "Фцыст идёт по Истре за Ягермейстером");
			this.pictureBoxAnimation.Visible = false;
			// 
			// pictureBoxScrollBar
			// 
			this.pictureBoxScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.pictureBoxScrollBar.Location = new System.Drawing.Point(0, 302);
			this.pictureBoxScrollBar.Name = "pictureBoxScrollBar";
			this.pictureBoxScrollBar.Size = new System.Drawing.Size(667, 15);
			this.pictureBoxScrollBar.TabIndex = 19;
			this.pictureBoxScrollBar.TabStop = false;
			this.pictureBoxScrollBar.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxScrollBar_Paint);
			// 
			// pictureBoxVodThumbnailImage
			// 
			this.pictureBoxVodThumbnailImage.Location = new System.Drawing.Point(525, 32);
			this.pictureBoxVodThumbnailImage.Name = "pictureBoxVodThumbnailImage";
			this.pictureBoxVodThumbnailImage.Size = new System.Drawing.Size(142, 86);
			this.pictureBoxVodThumbnailImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxVodThumbnailImage.TabIndex = 2;
			this.pictureBoxVodThumbnailImage.TabStop = false;
			this.pictureBoxVodThumbnailImage.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxVodThumbnailImage_Paint);
			// 
			// btnCopyVodChunkUrlList
			// 
			this.btnCopyVodChunkUrlList.Location = new System.Drawing.Point(898, 237);
			this.btnCopyVodChunkUrlList.Name = "btnCopyVodChunkUrlList";
			this.btnCopyVodChunkUrlList.Size = new System.Drawing.Size(174, 23);
			this.btnCopyVodChunkUrlList.TabIndex = 20;
			this.btnCopyVodChunkUrlList.Text = "Скопировать ссылки";
			this.btnCopyVodChunkUrlList.UseVisualStyleBackColor = true;
			this.btnCopyVodChunkUrlList.Click += new System.EventHandler(this.btnCopyVodChunkUrlList_Click);
			// 
			// multipleProgressBarOverall
			// 
			this.multipleProgressBarOverall.Location = new System.Drawing.Point(6, 156);
			this.multipleProgressBarOverall.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.multipleProgressBarOverall.Name = "multipleProgressBarOverall";
			this.multipleProgressBarOverall.Size = new System.Drawing.Size(661, 23);
			this.multipleProgressBarOverall.TabIndex = 22;
			this.multipleProgressBarOverall.Text = "multipleProgressBar2";
			// 
			// multipleProgressBarChunkGroup
			// 
			this.multipleProgressBarChunkGroup.BackColor = System.Drawing.SystemColors.Control;
			this.multipleProgressBarChunkGroup.ForeColor = System.Drawing.Color.Black;
			this.multipleProgressBarChunkGroup.Location = new System.Drawing.Point(6, 130);
			this.multipleProgressBarChunkGroup.Name = "multipleProgressBarChunkGroup";
			this.multipleProgressBarChunkGroup.Size = new System.Drawing.Size(661, 23);
			this.multipleProgressBarChunkGroup.TabIndex = 21;
			this.multipleProgressBarChunkGroup.Text = "multipleProgressBar1";
			this.multipleProgressBarChunkGroup.MouseDown += new System.Windows.Forms.MouseEventHandler(this.multipleProgressBarChunkGroup_MouseDown);
			// 
			// contextMenuProgressBarChunkGroup
			// 
			this.contextMenuProgressBarChunkGroup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.miIncreaseChunkGroupSizeToolStripMenuItem,
			this.miDecreaseChunkGroupSizeToolStripMenuItem});
			this.contextMenuProgressBarChunkGroup.Name = "contextMenuProgressBarGroup";
			this.contextMenuProgressBarChunkGroup.Size = new System.Drawing.Size(226, 48);
			// 
			// miIncreaseChunkGroupSizeToolStripMenuItem
			// 
			this.miIncreaseChunkGroupSizeToolStripMenuItem.Name = "miIncreaseChunkGroupSizeToolStripMenuItem";
			this.miIncreaseChunkGroupSizeToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			this.miIncreaseChunkGroupSizeToolStripMenuItem.Text = "Увеличить размер группы";
			this.miIncreaseChunkGroupSizeToolStripMenuItem.Click += new System.EventHandler(this.miIncreaseChunkGroupSizeToolStripMenuItem_Click);
			// 
			// miDecreaseChunkGroupSizeToolStripMenuItem
			// 
			this.miDecreaseChunkGroupSizeToolStripMenuItem.Name = "miDecreaseChunkGroupSizeToolStripMenuItem";
			this.miDecreaseChunkGroupSizeToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
			this.miDecreaseChunkGroupSizeToolStripMenuItem.Text = "Уменьшить размер группы";
			this.miDecreaseChunkGroupSizeToolStripMenuItem.Click += new System.EventHandler(this.miDecreaseChunkGroupSizeToolStripMenuItem_Click);
			// 
			// DownloadFrame
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.multipleProgressBarOverall);
			this.Controls.Add(this.multipleProgressBarChunkGroup);
			this.Controls.Add(this.btnCopyVodChunkUrlList);
			this.Controls.Add(this.btnCloseFrame);
			this.Controls.Add(this.pictureBoxScrollBar);
			this.Controls.Add(this.pictureBoxAnimation);
			this.Controls.Add(this.lblElapsedTime);
			this.Controls.Add(this.groupBoxDownloadMode);
			this.Controls.Add(this.lblProgressChunkGroup);
			this.Controls.Add(this.lblChunkFileList);
			this.Controls.Add(this.listBoxChunkFileList);
			this.Controls.Add(this.groupBoxDownloadVodChunkRange);
			this.Controls.Add(this.btnStopDownload);
			this.Controls.Add(this.btnStartDownload);
			this.Controls.Add(this.lblProgressOverall);
			this.Controls.Add(this.lblOutputFileName);
			this.Controls.Add(this.pictureBoxVodThumbnailImage);
			this.Controls.Add(this.lblVodTitle);
			this.Name = "DownloadFrame";
			this.Size = new System.Drawing.Size(1141, 320);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.downloadFrame_Paint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.downloadFrame_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.downloadFrame_MouseMove);
			this.Resize += new System.EventHandler(this.downloadFrame_Resize);
			this.groupBoxDownloadVodChunkRange.ResumeLayout(false);
			this.groupBoxDownloadVodChunkRange.PerformLayout();
			this.contextMenuVodTitle.ResumeLayout(false);
			this.groupBoxDownloadMode.ResumeLayout(false);
			this.groupBoxDownloadMode.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxAnimation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxScrollBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxVodThumbnailImage)).EndInit();
			this.contextMenuProgressBarChunkGroup.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCloseFrame;
		public System.Windows.Forms.Label lblVodTitle;
		public System.Windows.Forms.PictureBox pictureBoxVodThumbnailImage;
		public System.Windows.Forms.Label lblOutputFileName;
		public System.Windows.Forms.Label lblProgressOverall;
		public System.Windows.Forms.Button btnStartDownload;
		public System.Windows.Forms.Button btnStopDownload;
		private System.Windows.Forms.GroupBox groupBoxDownloadVodChunkRange;
		private System.Windows.Forms.TextBox textBoxChunkTo;
		private System.Windows.Forms.TextBox textBoxChunkFrom;
		private System.Windows.Forms.Button btnSetMaxChunkTo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ContextMenuStrip contextMenuVodTitle;
		private System.Windows.Forms.ToolStripMenuItem miCopyVodTitleToolStripMenuItem;
		public System.Windows.Forms.ListBox listBoxChunkFileList;
		private System.Windows.Forms.Label lblChunkFileList;
		private System.Windows.Forms.Label lblProgressChunkGroup;
		private System.Windows.Forms.GroupBox groupBoxDownloadMode;
		private System.Windows.Forms.RadioButton radioButtonDownloadChunksSeparately;
		private System.Windows.Forms.RadioButton radioButtonDownloadSingleBigVideoFile;
		private System.Windows.Forms.Timer timerElapsedTime;
		private System.Windows.Forms.Label lblElapsedTime;
		private System.Windows.Forms.PictureBox pictureBoxAnimation;
		private System.Windows.Forms.Timer timerAnimation;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.PictureBox pictureBoxScrollBar;
		private System.Windows.Forms.Button btnCopyVodChunkUrlList;
		private MultipleProgressBar multipleProgressBarChunkGroup;
		private MultipleProgressBar multipleProgressBarOverall;
		private System.Windows.Forms.ContextMenuStrip contextMenuProgressBarChunkGroup;
		private System.Windows.Forms.ToolStripMenuItem miIncreaseChunkGroupSizeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem miDecreaseChunkGroupSizeToolStripMenuItem;
	}
}
