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
            this.btnClose = new System.Windows.Forms.Button();
            this.lblStreamTitle = new System.Windows.Forms.Label();
            this.lblOutputFilename = new System.Windows.Forms.Label();
            this.lblProgressOverall = new System.Windows.Forms.Label();
            this.btnStartDownload = new System.Windows.Forms.Button();
            this.btnStopDownload = new System.Windows.Forms.Button();
            this.grpDownloadRange = new System.Windows.Forms.GroupBox();
            this.btnSetMaxChunkTo = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.editTo = new System.Windows.Forms.TextBox();
            this.editFrom = new System.Windows.Forms.TextBox();
            this.contextMenuStreamTitle = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyStreamTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbFileList = new System.Windows.Forms.ListBox();
            this.lblFilelist = new System.Windows.Forms.Label();
            this.lblGroupProgress = new System.Windows.Forms.Label();
            this.grpDownloadOptions = new System.Windows.Forms.GroupBox();
            this.rbDownloadChunksSeparately = new System.Windows.Forms.RadioButton();
            this.rbDownloadOneBigFile = new System.Windows.Forms.RadioButton();
            this.timerElapsed = new System.Windows.Forms.Timer(this.components);
            this.lblElapsedTime = new System.Windows.Forms.Label();
            this.timerFcst = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.imgFcst = new System.Windows.Forms.PictureBox();
            this.imgScrollBar = new System.Windows.Forms.PictureBox();
            this.pictureBoxStreamImage = new System.Windows.Forms.PictureBox();
            this.btnCopyUrlList = new System.Windows.Forms.Button();
            this.multipleProgressBarOverall = new Twitch_prime_downloader.MultipleProgressBar();
            this.multipleProgressBarGroup = new Twitch_prime_downloader.MultipleProgressBar();
            this.contextMenuProgressBarGroup = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miIncreaseGroupSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miDecreaseGroupSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpDownloadRange.SuspendLayout();
            this.contextMenuStreamTitle.SuspendLayout();
            this.grpDownloadOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgFcst)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgScrollBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStreamImage)).BeginInit();
            this.contextMenuProgressBarGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(592, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // lblStreamTitle
            // 
            this.lblStreamTitle.Location = new System.Drawing.Point(3, 3);
            this.lblStreamTitle.Name = "lblStreamTitle";
            this.lblStreamTitle.Size = new System.Drawing.Size(516, 50);
            this.lblStreamTitle.TabIndex = 1;
            this.lblStreamTitle.Text = "lblStreamTitle";
            this.lblStreamTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LblStreamTitle_MouseUp);
            // 
            // lblOutputFilename
            // 
            this.lblOutputFilename.Location = new System.Drawing.Point(3, 57);
            this.lblOutputFilename.Name = "lblOutputFilename";
            this.lblOutputFilename.Size = new System.Drawing.Size(516, 57);
            this.lblOutputFilename.TabIndex = 3;
            this.lblOutputFilename.Text = "lblOutputFilename";
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
            this.btnStartDownload.Click += new System.EventHandler(this.BtnStartDownload_Click);
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
            this.btnStopDownload.Click += new System.EventHandler(this.BtnStopDownload_Click);
            // 
            // grpDownloadRange
            // 
            this.grpDownloadRange.Controls.Add(this.btnSetMaxChunkTo);
            this.grpDownloadRange.Controls.Add(this.label2);
            this.grpDownloadRange.Controls.Add(this.label1);
            this.grpDownloadRange.Controls.Add(this.editTo);
            this.grpDownloadRange.Controls.Add(this.editFrom);
            this.grpDownloadRange.Location = new System.Drawing.Point(673, 76);
            this.grpDownloadRange.Name = "grpDownloadRange";
            this.grpDownloadRange.Size = new System.Drawing.Size(219, 83);
            this.grpDownloadRange.TabIndex = 11;
            this.grpDownloadRange.TabStop = false;
            this.grpDownloadRange.Text = "Диапазон скачивания";
            // 
            // btnSetMaxChunkTo
            // 
            this.btnSetMaxChunkTo.Location = new System.Drawing.Point(162, 55);
            this.btnSetMaxChunkTo.Name = "btnSetMaxChunkTo";
            this.btnSetMaxChunkTo.Size = new System.Drawing.Size(37, 20);
            this.btnSetMaxChunkTo.TabIndex = 4;
            this.btnSetMaxChunkTo.Text = "max";
            this.btnSetMaxChunkTo.UseVisualStyleBackColor = true;
            this.btnSetMaxChunkTo.Click += new System.EventHandler(this.BtnSetMaxChunkTo_Click);
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
            // editTo
            // 
            this.editTo.Location = new System.Drawing.Point(112, 55);
            this.editTo.Name = "editTo";
            this.editTo.Size = new System.Drawing.Size(44, 20);
            this.editTo.TabIndex = 1;
            this.editTo.Text = "10";
            this.editTo.Leave += new System.EventHandler(this.EditTo_Leave);
            // 
            // editFrom
            // 
            this.editFrom.Location = new System.Drawing.Point(112, 23);
            this.editFrom.Name = "editFrom";
            this.editFrom.Size = new System.Drawing.Size(44, 20);
            this.editFrom.TabIndex = 0;
            this.editFrom.Text = "1";
            this.editFrom.Leave += new System.EventHandler(this.EditFrom_Leave);
            // 
            // contextMenuStreamTitle
            // 
            this.contextMenuStreamTitle.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyStreamTitleToolStripMenuItem});
            this.contextMenuStreamTitle.Name = "contextMenuStreamTitle";
            this.contextMenuStreamTitle.Size = new System.Drawing.Size(200, 26);
            // 
            // copyStreamTitleToolStripMenuItem
            // 
            this.copyStreamTitleToolStripMenuItem.Name = "copyStreamTitleToolStripMenuItem";
            this.copyStreamTitleToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.copyStreamTitleToolStripMenuItem.Text = "Скопировать название";
            this.copyStreamTitleToolStripMenuItem.Click += new System.EventHandler(this.CopyStreamTitleToolStripMenuItem_Click);
            // 
            // lbFileList
            // 
            this.lbFileList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbFileList.FormattingEnabled = true;
            this.lbFileList.Location = new System.Drawing.Point(898, 19);
            this.lbFileList.Name = "lbFileList";
            this.lbFileList.Size = new System.Drawing.Size(174, 212);
            this.lbFileList.TabIndex = 13;
            this.lbFileList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbFileList_DrawItem);
            // 
            // lblFilelist
            // 
            this.lblFilelist.AutoSize = true;
            this.lblFilelist.Location = new System.Drawing.Point(895, 3);
            this.lblFilelist.Name = "lblFilelist";
            this.lblFilelist.Size = new System.Drawing.Size(130, 13);
            this.lblFilelist.TabIndex = 14;
            this.lblFilelist.Text = "Файлы для скачивания:";
            // 
            // lblGroupProgress
            // 
            this.lblGroupProgress.AutoSize = true;
            this.lblGroupProgress.Location = new System.Drawing.Point(3, 114);
            this.lblGroupProgress.Name = "lblGroupProgress";
            this.lblGroupProgress.Size = new System.Drawing.Size(110, 13);
            this.lblGroupProgress.TabIndex = 15;
            this.lblGroupProgress.Text = "lblCurrentChunkName";
            // 
            // grpDownloadOptions
            // 
            this.grpDownloadOptions.Controls.Add(this.rbDownloadChunksSeparately);
            this.grpDownloadOptions.Controls.Add(this.rbDownloadOneBigFile);
            this.grpDownloadOptions.Location = new System.Drawing.Point(673, 3);
            this.grpDownloadOptions.Name = "grpDownloadOptions";
            this.grpDownloadOptions.Size = new System.Drawing.Size(218, 63);
            this.grpDownloadOptions.TabIndex = 16;
            this.grpDownloadOptions.TabStop = false;
            this.grpDownloadOptions.Text = "Параметры скачивания";
            // 
            // rbDownloadChunksSeparately
            // 
            this.rbDownloadChunksSeparately.AutoSize = true;
            this.rbDownloadChunksSeparately.Location = new System.Drawing.Point(17, 33);
            this.rbDownloadChunksSeparately.Name = "rbDownloadChunksSeparately";
            this.rbDownloadChunksSeparately.Size = new System.Drawing.Size(188, 17);
            this.rbDownloadChunksSeparately.TabIndex = 1;
            this.rbDownloadChunksSeparately.TabStop = true;
            this.rbDownloadChunksSeparately.Text = "Каждый чанк в отдельный файл";
            this.rbDownloadChunksSeparately.UseVisualStyleBackColor = true;
            this.rbDownloadChunksSeparately.CheckedChanged += new System.EventHandler(this.rbDownloadChunksSeparately_CheckedChanged);
            // 
            // rbDownloadOneBigFile
            // 
            this.rbDownloadOneBigFile.AutoSize = true;
            this.rbDownloadOneBigFile.Checked = true;
            this.rbDownloadOneBigFile.Location = new System.Drawing.Point(17, 16);
            this.rbDownloadOneBigFile.Name = "rbDownloadOneBigFile";
            this.rbDownloadOneBigFile.Size = new System.Drawing.Size(127, 17);
            this.rbDownloadOneBigFile.TabIndex = 0;
            this.rbDownloadOneBigFile.TabStop = true;
            this.rbDownloadOneBigFile.Text = "Один большой файл";
            this.rbDownloadOneBigFile.UseVisualStyleBackColor = true;
            this.rbDownloadOneBigFile.CheckedChanged += new System.EventHandler(this.rbDownloadOneBigFile_CheckedChanged);
            // 
            // timerElapsed
            // 
            this.timerElapsed.Interval = 1000;
            this.timerElapsed.Tick += new System.EventHandler(this.timerElapsed_Tick);
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
            // timerFcst
            // 
            this.timerFcst.Tick += new System.EventHandler(this.timerFcst_Tick);
            // 
            // imgFcst
            // 
            this.imgFcst.Image = global::Twitch_prime_downloader.Properties.Resources.fcst_istra_01;
            this.imgFcst.Location = new System.Drawing.Point(6, 198);
            this.imgFcst.Name = "imgFcst";
            this.imgFcst.Size = new System.Drawing.Size(70, 70);
            this.imgFcst.TabIndex = 18;
            this.imgFcst.TabStop = false;
            this.toolTip1.SetToolTip(this.imgFcst, "Фцыст идёт по Истре за Ягермейстером");
            this.imgFcst.Visible = false;
            // 
            // imgScrollBar
            // 
            this.imgScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.imgScrollBar.Location = new System.Drawing.Point(0, 302);
            this.imgScrollBar.Name = "imgScrollBar";
            this.imgScrollBar.Size = new System.Drawing.Size(667, 15);
            this.imgScrollBar.TabIndex = 19;
            this.imgScrollBar.TabStop = false;
            this.imgScrollBar.Paint += new System.Windows.Forms.PaintEventHandler(this.imgScrollBar_Paint);
            // 
            // pictureBoxStreamImage
            // 
            this.pictureBoxStreamImage.Location = new System.Drawing.Point(525, 32);
            this.pictureBoxStreamImage.Name = "pictureBoxStreamImage";
            this.pictureBoxStreamImage.Size = new System.Drawing.Size(142, 86);
            this.pictureBoxStreamImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxStreamImage.TabIndex = 2;
            this.pictureBoxStreamImage.TabStop = false;
            this.pictureBoxStreamImage.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBoxStreamImage_Paint);
            // 
            // btnCopyUrlList
            // 
            this.btnCopyUrlList.Location = new System.Drawing.Point(898, 237);
            this.btnCopyUrlList.Name = "btnCopyUrlList";
            this.btnCopyUrlList.Size = new System.Drawing.Size(174, 23);
            this.btnCopyUrlList.TabIndex = 20;
            this.btnCopyUrlList.Text = "Скопировать ссылки";
            this.btnCopyUrlList.UseVisualStyleBackColor = true;
            this.btnCopyUrlList.Click += new System.EventHandler(this.btnCopyUrlList_Click);
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
            // multipleProgressBarGroup
            // 
            this.multipleProgressBarGroup.BackColor = System.Drawing.SystemColors.Control;
            this.multipleProgressBarGroup.ForeColor = System.Drawing.Color.Black;
            this.multipleProgressBarGroup.Location = new System.Drawing.Point(6, 130);
            this.multipleProgressBarGroup.Name = "multipleProgressBarGroup";
            this.multipleProgressBarGroup.Size = new System.Drawing.Size(661, 23);
            this.multipleProgressBarGroup.TabIndex = 21;
            this.multipleProgressBarGroup.Text = "multipleProgressBar1";
            this.multipleProgressBarGroup.MouseDown += new System.Windows.Forms.MouseEventHandler(this.multipleProgressBarGroup_MouseDown);
            // 
            // contextMenuProgressBarGroup
            // 
            this.contextMenuProgressBarGroup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miIncreaseGroupSizeToolStripMenuItem,
            this.miDecreaseGroupSizeToolStripMenuItem});
            this.contextMenuProgressBarGroup.Name = "contextMenuProgressBarGroup";
            this.contextMenuProgressBarGroup.Size = new System.Drawing.Size(226, 48);
            // 
            // miIncreaseGroupSizeToolStripMenuItem
            // 
            this.miIncreaseGroupSizeToolStripMenuItem.Name = "miIncreaseGroupSizeToolStripMenuItem";
            this.miIncreaseGroupSizeToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.miIncreaseGroupSizeToolStripMenuItem.Text = "Увеличить размер группы";
            this.miIncreaseGroupSizeToolStripMenuItem.Click += new System.EventHandler(this.miIncreaseGroupSizeToolStripMenuItem_Click);
            // 
            // miDecreaseGroupSizeToolStripMenuItem
            // 
            this.miDecreaseGroupSizeToolStripMenuItem.Name = "miDecreaseGroupSizeToolStripMenuItem";
            this.miDecreaseGroupSizeToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.miDecreaseGroupSizeToolStripMenuItem.Text = "Уменьшить размер группы";
            this.miDecreaseGroupSizeToolStripMenuItem.Click += new System.EventHandler(this.miDecreaseGroupSizeToolStripMenuItem_Click);
            // 
            // DownloadFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.multipleProgressBarOverall);
            this.Controls.Add(this.multipleProgressBarGroup);
            this.Controls.Add(this.btnCopyUrlList);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.imgScrollBar);
            this.Controls.Add(this.imgFcst);
            this.Controls.Add(this.lblElapsedTime);
            this.Controls.Add(this.grpDownloadOptions);
            this.Controls.Add(this.lblGroupProgress);
            this.Controls.Add(this.lblFilelist);
            this.Controls.Add(this.lbFileList);
            this.Controls.Add(this.grpDownloadRange);
            this.Controls.Add(this.btnStopDownload);
            this.Controls.Add(this.btnStartDownload);
            this.Controls.Add(this.lblProgressOverall);
            this.Controls.Add(this.lblOutputFilename);
            this.Controls.Add(this.pictureBoxStreamImage);
            this.Controls.Add(this.lblStreamTitle);
            this.Name = "DownloadFrame";
            this.Size = new System.Drawing.Size(1141, 320);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DownloadFrame_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DownloadFrame_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DownloadFrame_MouseMove);
            this.Resize += new System.EventHandler(this.DownloadFrame_Resize);
            this.grpDownloadRange.ResumeLayout(false);
            this.grpDownloadRange.PerformLayout();
            this.contextMenuStreamTitle.ResumeLayout(false);
            this.grpDownloadOptions.ResumeLayout(false);
            this.grpDownloadOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgFcst)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgScrollBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStreamImage)).EndInit();
            this.contextMenuProgressBarGroup.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnClose;
		public System.Windows.Forms.Label lblStreamTitle;
		public System.Windows.Forms.PictureBox pictureBoxStreamImage;
		public System.Windows.Forms.Label lblOutputFilename;
		public System.Windows.Forms.Label lblProgressOverall;
		public System.Windows.Forms.Button btnStartDownload;
		public System.Windows.Forms.Button btnStopDownload;
		private System.Windows.Forms.GroupBox grpDownloadRange;
		private System.Windows.Forms.TextBox editTo;
		private System.Windows.Forms.TextBox editFrom;
		private System.Windows.Forms.Button btnSetMaxChunkTo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStreamTitle;
		private System.Windows.Forms.ToolStripMenuItem copyStreamTitleToolStripMenuItem;
		public System.Windows.Forms.ListBox lbFileList;
		private System.Windows.Forms.Label lblFilelist;
		private System.Windows.Forms.Label lblGroupProgress;
		private System.Windows.Forms.GroupBox grpDownloadOptions;
		private System.Windows.Forms.RadioButton rbDownloadChunksSeparately;
		private System.Windows.Forms.RadioButton rbDownloadOneBigFile;
		private System.Windows.Forms.Timer timerElapsed;
		private System.Windows.Forms.Label lblElapsedTime;
		private System.Windows.Forms.PictureBox imgFcst;
		private System.Windows.Forms.Timer timerFcst;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.PictureBox imgScrollBar;
		private System.Windows.Forms.Button btnCopyUrlList;
		private MultipleProgressBar multipleProgressBarGroup;
		private MultipleProgressBar multipleProgressBarOverall;
		private System.Windows.Forms.ContextMenuStrip contextMenuProgressBarGroup;
		private System.Windows.Forms.ToolStripMenuItem miIncreaseGroupSizeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem miDecreaseGroupSizeToolStripMenuItem;
	}
}
