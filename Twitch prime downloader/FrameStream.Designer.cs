namespace Twitch_prime_downloader
{
    partial class FrameStream
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
            this.lblStreamTitle = new System.Windows.Forms.Label();
            this.lblChannelName = new System.Windows.Forms.Label();
            this.lblGameName = new System.Windows.Forms.Label();
            this.btnDownload = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyStreamTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyStreamDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyStreamTitlePlusDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblMutedChunks = new System.Windows.Forms.Label();
            this.lblPrime = new System.Windows.Forms.Label();
            this.imageGame = new System.Windows.Forms.PictureBox();
            this.imageStream = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblBroadcastType = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageGame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageStream)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStreamTitle
            // 
            this.lblStreamTitle.BackColor = System.Drawing.Color.Black;
            this.lblStreamTitle.ForeColor = System.Drawing.Color.White;
            this.lblStreamTitle.Location = new System.Drawing.Point(72, 196);
            this.lblStreamTitle.Name = "lblStreamTitle";
            this.lblStreamTitle.Size = new System.Drawing.Size(295, 40);
            this.lblStreamTitle.TabIndex = 0;
            this.lblStreamTitle.Text = "lblStreamTitle";
            this.lblStreamTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblStreamTitle_MouseDown);
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
            this.lblChannelName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblChannelName_MouseDown);
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
            this.btnDownload.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnDownload_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyStreamTitleToolStripMenuItem,
            this.copyStreamDateToolStripMenuItem,
            this.copyStreamTitlePlusDateToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(236, 92);
            // 
            // copyStreamTitleToolStripMenuItem
            // 
            this.copyStreamTitleToolStripMenuItem.Name = "copyStreamTitleToolStripMenuItem";
            this.copyStreamTitleToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.copyStreamTitleToolStripMenuItem.Text = "Скопировать название";
            this.copyStreamTitleToolStripMenuItem.Click += new System.EventHandler(this.copyStreamTitleToolStripMenuItem_Click);
            // 
            // copyStreamDateToolStripMenuItem
            // 
            this.copyStreamDateToolStripMenuItem.Name = "copyStreamDateToolStripMenuItem";
            this.copyStreamDateToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.copyStreamDateToolStripMenuItem.Text = "Скопировать дату";
            this.copyStreamDateToolStripMenuItem.Click += new System.EventHandler(this.copyStreamDateToolStripMenuItem_Click);
            // 
            // copyStreamTitlePlusDateToolStripMenuItem
            // 
            this.copyStreamTitlePlusDateToolStripMenuItem.Name = "copyStreamTitlePlusDateToolStripMenuItem";
            this.copyStreamTitlePlusDateToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.copyStreamTitlePlusDateToolStripMenuItem.Text = "Скопировать дату и название";
            this.copyStreamTitlePlusDateToolStripMenuItem.Click += new System.EventHandler(this.copyStreamTitlePlusDateToolStripMenuItem_Click);
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
            this.lblMutedChunks.DoubleClick += new System.EventHandler(this.lblMutedChunks_DoubleClick);
            this.lblMutedChunks.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FrameStream_MouseDown);
            // 
            // lblPrime
            // 
            this.lblPrime.AutoSize = true;
            this.lblPrime.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblPrime.ForeColor = System.Drawing.Color.Lime;
            this.lblPrime.Location = new System.Drawing.Point(352, 6);
            this.lblPrime.Name = "lblPrime";
            this.lblPrime.Size = new System.Drawing.Size(24, 26);
            this.lblPrime.TabIndex = 7;
            this.lblPrime.Text = "$";
            this.toolTip1.SetToolTip(this.lblPrime, "Очень платное видео");
            // 
            // imageGame
            // 
            this.imageGame.Location = new System.Drawing.Point(10, 196);
            this.imageGame.Name = "imageGame";
            this.imageGame.Size = new System.Drawing.Size(52, 72);
            this.imageGame.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imageGame.TabIndex = 2;
            this.imageGame.TabStop = false;
            this.imageGame.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageGame_MouseDown);
            // 
            // imageStream
            // 
            this.imageStream.Location = new System.Drawing.Point(27, 10);
            this.imageStream.Name = "imageStream";
            this.imageStream.Size = new System.Drawing.Size(320, 180);
            this.imageStream.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imageStream.TabIndex = 1;
            this.imageStream.TabStop = false;
            this.imageStream.Paint += new System.Windows.Forms.PaintEventHandler(this.imageStream_Paint);
            this.imageStream.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageStream_MouseDown);
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
            // 
            // FrameStream
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.Controls.Add(this.lblBroadcastType);
            this.Controls.Add(this.lblPrime);
            this.Controls.Add(this.lblMutedChunks);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.lblGameName);
            this.Controls.Add(this.lblChannelName);
            this.Controls.Add(this.imageGame);
            this.Controls.Add(this.imageStream);
            this.Controls.Add(this.lblStreamTitle);
            this.Name = "FrameStream";
            this.Size = new System.Drawing.Size(380, 296);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FrameStream_MouseDown);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageGame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageStream)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblStreamTitle;
        public System.Windows.Forms.PictureBox imageStream;
        public System.Windows.Forms.PictureBox imageGame;
        public System.Windows.Forms.Label lblChannelName;
        public System.Windows.Forms.Label lblGameName;
        public System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyStreamTitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyStreamDateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyStreamTitlePlusDateToolStripMenuItem;
        private System.Windows.Forms.Label lblMutedChunks;
        private System.Windows.Forms.Label lblPrime;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblBroadcastType;
    }
}
