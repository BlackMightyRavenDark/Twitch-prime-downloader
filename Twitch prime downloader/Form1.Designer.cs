
namespace Twitch_prime_downloader
{
	partial class Form1
	{
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.tabControlMain = new System.Windows.Forms.TabControl();
			this.tabPageDebug = new System.Windows.Forms.TabPage();
			this.memoDebug = new System.Windows.Forms.RichTextBox();
			this.tabPageSettings = new System.Windows.Forms.TabPage();
			this.groupBoxApiSettings = new System.Windows.Forms.GroupBox();
			this.groupBoxTwitchApplication = new System.Windows.Forms.GroupBox();
			this.btnApplyApiApplication = new System.Windows.Forms.Button();
			this.btnSetDefaultApiApplication = new System.Windows.Forms.Button();
			this.textBoxHelixApiClentSecretKey = new System.Windows.Forms.TextBox();
			this.textBoxHelixApiClientId = new System.Windows.Forms.TextBox();
			this.textBoxApiApplicationDescription = new System.Windows.Forms.TextBox();
			this.textBoxApiApplicationTitle = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.groupBoxHelixApiToken = new System.Windows.Forms.GroupBox();
			this.btnResetHelixApiToken = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.btnUpdateHelixApiToken = new System.Windows.Forms.Button();
			this.textBoxHelixApiToken = new System.Windows.Forms.TextBox();
			this.lblHelixApiTokenExpirationDate = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.groupBoxVideoInformationSettings = new System.Windows.Forms.GroupBox();
			this.chkSaveVodChunksInfo = new System.Windows.Forms.CheckBox();
			this.chkUseGmtTime = new System.Windows.Forms.CheckBox();
			this.chkSaveVodInfo = new System.Windows.Forms.CheckBox();
			this.groupBoxFilesAndFoldersSettings = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textBox_Browser = new System.Windows.Forms.TextBox();
			this.textBox_DownloadingPath = new System.Windows.Forms.TextBox();
			this.btnRestoreDefaultFilenameFormat = new System.Windows.Forms.Button();
			this.btnSelectDownloadingPath = new System.Windows.Forms.Button();
			this.textBox_FileNameFormat = new System.Windows.Forms.TextBox();
			this.btnSelectBrowser = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.tabPageSearch = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnSearchByUrls = new System.Windows.Forms.Button();
			this.textBoxUrls = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnEditChannelList = new System.Windows.Forms.Button();
			this.btnAddChannelToList = new System.Windows.Forms.Button();
			this.textBoxChannelName = new System.Windows.Forms.TextBox();
			this.listBoxChannelList = new System.Windows.Forms.ListBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.numericUpDownSearchLimit = new System.Windows.Forms.NumericUpDown();
			this.rbSearchLimit = new System.Windows.Forms.RadioButton();
			this.rbSearchAll = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSearchChannelName = new System.Windows.Forms.Button();
			this.tabPageLog = new System.Windows.Forms.TabPage();
			this.lbLog = new System.Windows.Forms.ListBox();
			this.tabPageStreams = new System.Windows.Forms.TabPage();
			this.panelStreams = new System.Windows.Forms.Panel();
			this.scrollBarStreams = new System.Windows.Forms.VScrollBar();
			this.tabPageDownloading = new System.Windows.Forms.TabPage();
			this.scrollBarDownloads = new System.Windows.Forms.VScrollBar();
			this.panelDownloads = new System.Windows.Forms.Panel();
			this.contextMenuStreamImage = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miCopyVideoUrl = new System.Windows.Forms.ToolStripMenuItem();
			this.copyImageUrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyStreamInfoJsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miSavePlaylistAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveImageAssToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.openVideoInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tabControlMain.SuspendLayout();
			this.tabPageDebug.SuspendLayout();
			this.tabPageSettings.SuspendLayout();
			this.groupBoxApiSettings.SuspendLayout();
			this.groupBoxTwitchApplication.SuspendLayout();
			this.groupBoxHelixApiToken.SuspendLayout();
			this.groupBoxVideoInformationSettings.SuspendLayout();
			this.groupBoxFilesAndFoldersSettings.SuspendLayout();
			this.tabPageSearch.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSearchLimit)).BeginInit();
			this.tabPageLog.SuspendLayout();
			this.tabPageStreams.SuspendLayout();
			this.tabPageDownloading.SuspendLayout();
			this.contextMenuStreamImage.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControlMain
			// 
			this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlMain.Controls.Add(this.tabPageDebug);
			this.tabControlMain.Controls.Add(this.tabPageSettings);
			this.tabControlMain.Controls.Add(this.tabPageSearch);
			this.tabControlMain.Controls.Add(this.tabPageLog);
			this.tabControlMain.Controls.Add(this.tabPageStreams);
			this.tabControlMain.Controls.Add(this.tabPageDownloading);
			this.tabControlMain.Location = new System.Drawing.Point(0, 0);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.SelectedIndex = 0;
			this.tabControlMain.Size = new System.Drawing.Size(826, 498);
			this.tabControlMain.TabIndex = 0;
			this.tabControlMain.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControlMain_Selected);
			// 
			// tabPageDebug
			// 
			this.tabPageDebug.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageDebug.Controls.Add(this.memoDebug);
			this.tabPageDebug.Location = new System.Drawing.Point(4, 22);
			this.tabPageDebug.Name = "tabPageDebug";
			this.tabPageDebug.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDebug.Size = new System.Drawing.Size(818, 472);
			this.tabPageDebug.TabIndex = 2;
			this.tabPageDebug.Text = "Debug";
			// 
			// memoDebug
			// 
			this.memoDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.memoDebug.BackColor = System.Drawing.Color.Black;
			this.memoDebug.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.memoDebug.ForeColor = System.Drawing.Color.Lime;
			this.memoDebug.Location = new System.Drawing.Point(6, 3);
			this.memoDebug.Name = "memoDebug";
			this.memoDebug.Size = new System.Drawing.Size(806, 466);
			this.memoDebug.TabIndex = 0;
			this.memoDebug.Text = "";
			// 
			// tabPageSettings
			// 
			this.tabPageSettings.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageSettings.Controls.Add(this.groupBoxApiSettings);
			this.tabPageSettings.Controls.Add(this.groupBoxVideoInformationSettings);
			this.tabPageSettings.Controls.Add(this.groupBoxFilesAndFoldersSettings);
			this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
			this.tabPageSettings.Name = "tabPageSettings";
			this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageSettings.Size = new System.Drawing.Size(818, 472);
			this.tabPageSettings.TabIndex = 1;
			this.tabPageSettings.Text = "Настройки";
			// 
			// groupBoxApiSettings
			// 
			this.groupBoxApiSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxApiSettings.Controls.Add(this.groupBoxTwitchApplication);
			this.groupBoxApiSettings.Controls.Add(this.groupBoxHelixApiToken);
			this.groupBoxApiSettings.Location = new System.Drawing.Point(8, 205);
			this.groupBoxApiSettings.Name = "groupBoxApiSettings";
			this.groupBoxApiSettings.Size = new System.Drawing.Size(798, 260);
			this.groupBoxApiSettings.TabIndex = 21;
			this.groupBoxApiSettings.TabStop = false;
			this.groupBoxApiSettings.Text = "API";
			// 
			// groupBoxTwitchApplication
			// 
			this.groupBoxTwitchApplication.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxTwitchApplication.Controls.Add(this.btnApplyApiApplication);
			this.groupBoxTwitchApplication.Controls.Add(this.btnSetDefaultApiApplication);
			this.groupBoxTwitchApplication.Controls.Add(this.textBoxHelixApiClentSecretKey);
			this.groupBoxTwitchApplication.Controls.Add(this.textBoxHelixApiClientId);
			this.groupBoxTwitchApplication.Controls.Add(this.textBoxApiApplicationDescription);
			this.groupBoxTwitchApplication.Controls.Add(this.textBoxApiApplicationTitle);
			this.groupBoxTwitchApplication.Controls.Add(this.label12);
			this.groupBoxTwitchApplication.Controls.Add(this.label11);
			this.groupBoxTwitchApplication.Controls.Add(this.label10);
			this.groupBoxTwitchApplication.Controls.Add(this.label9);
			this.groupBoxTwitchApplication.Location = new System.Drawing.Point(7, 19);
			this.groupBoxTwitchApplication.Name = "groupBoxTwitchApplication";
			this.groupBoxTwitchApplication.Size = new System.Drawing.Size(785, 154);
			this.groupBoxTwitchApplication.TabIndex = 6;
			this.groupBoxTwitchApplication.TabStop = false;
			this.groupBoxTwitchApplication.Text = "Twitch application";
			// 
			// btnApplyApiApplication
			// 
			this.btnApplyApiApplication.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnApplyApiApplication.Location = new System.Drawing.Point(3, 126);
			this.btnApplyApiApplication.Name = "btnApplyApiApplication";
			this.btnApplyApiApplication.Size = new System.Drawing.Size(75, 23);
			this.btnApplyApiApplication.TabIndex = 9;
			this.btnApplyApiApplication.Text = "Применить";
			this.btnApplyApiApplication.UseVisualStyleBackColor = true;
			this.btnApplyApiApplication.Click += new System.EventHandler(this.btnApplyApiApplication_Click);
			// 
			// btnSetDefaultApiApplication
			// 
			this.btnSetDefaultApiApplication.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSetDefaultApiApplication.Location = new System.Drawing.Point(682, 126);
			this.btnSetDefaultApiApplication.Name = "btnSetDefaultApiApplication";
			this.btnSetDefaultApiApplication.Size = new System.Drawing.Size(97, 23);
			this.btnSetDefaultApiApplication.TabIndex = 8;
			this.btnSetDefaultApiApplication.Text = "По-умолчанию";
			this.btnSetDefaultApiApplication.UseVisualStyleBackColor = true;
			this.btnSetDefaultApiApplication.Click += new System.EventHandler(this.btnSetDefaultApiApplication_Click);
			// 
			// textBoxHelixApiClentSecretKey
			// 
			this.textBoxHelixApiClentSecretKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxHelixApiClentSecretKey.Location = new System.Drawing.Point(100, 97);
			this.textBoxHelixApiClentSecretKey.Name = "textBoxHelixApiClentSecretKey";
			this.textBoxHelixApiClentSecretKey.Size = new System.Drawing.Size(685, 20);
			this.textBoxHelixApiClentSecretKey.TabIndex = 7;
			// 
			// textBoxHelixApiClientId
			// 
			this.textBoxHelixApiClientId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxHelixApiClientId.Location = new System.Drawing.Point(100, 71);
			this.textBoxHelixApiClientId.Name = "textBoxHelixApiClientId";
			this.textBoxHelixApiClientId.Size = new System.Drawing.Size(691, 20);
			this.textBoxHelixApiClientId.TabIndex = 6;
			// 
			// textBoxApiApplicationDescription
			// 
			this.textBoxApiApplicationDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxApiApplicationDescription.Location = new System.Drawing.Point(100, 45);
			this.textBoxApiApplicationDescription.Name = "textBoxApiApplicationDescription";
			this.textBoxApiApplicationDescription.Size = new System.Drawing.Size(691, 20);
			this.textBoxApiApplicationDescription.TabIndex = 5;
			// 
			// textBoxApiApplicationTitle
			// 
			this.textBoxApiApplicationTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxApiApplicationTitle.Location = new System.Drawing.Point(100, 19);
			this.textBoxApiApplicationTitle.Name = "textBoxApiApplicationTitle";
			this.textBoxApiApplicationTitle.Size = new System.Drawing.Size(691, 20);
			this.textBoxApiApplicationTitle.TabIndex = 4;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(6, 100);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(88, 13);
			this.label12.TabIndex = 3;
			this.label12.Text = "Client secret key:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(6, 75);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(50, 13);
			this.label11.TabIndex = 2;
			this.label11.Text = "Client ID:";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(6, 48);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(60, 13);
			this.label10.TabIndex = 1;
			this.label10.Text = "Описание:";
			this.toolTip1.SetToolTip(this.label10, "Необязательно");
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(6, 22);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(60, 13);
			this.label9.TabIndex = 0;
			this.label9.Text = "Название:";
			this.toolTip1.SetToolTip(this.label9, "Необязательно");
			// 
			// groupBoxHelixApiToken
			// 
			this.groupBoxHelixApiToken.Controls.Add(this.btnResetHelixApiToken);
			this.groupBoxHelixApiToken.Controls.Add(this.label7);
			this.groupBoxHelixApiToken.Controls.Add(this.btnUpdateHelixApiToken);
			this.groupBoxHelixApiToken.Controls.Add(this.textBoxHelixApiToken);
			this.groupBoxHelixApiToken.Controls.Add(this.lblHelixApiTokenExpirationDate);
			this.groupBoxHelixApiToken.Controls.Add(this.label8);
			this.groupBoxHelixApiToken.Location = new System.Drawing.Point(7, 179);
			this.groupBoxHelixApiToken.Name = "groupBoxHelixApiToken";
			this.groupBoxHelixApiToken.Size = new System.Drawing.Size(785, 73);
			this.groupBoxHelixApiToken.TabIndex = 5;
			this.groupBoxHelixApiToken.TabStop = false;
			this.groupBoxHelixApiToken.Text = "Helix API token";
			// 
			// btnResetHelixApiToken
			// 
			this.btnResetHelixApiToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnResetHelixApiToken.Location = new System.Drawing.Point(621, 45);
			this.btnResetHelixApiToken.Name = "btnResetHelixApiToken";
			this.btnResetHelixApiToken.Size = new System.Drawing.Size(75, 23);
			this.btnResetHelixApiToken.TabIndex = 5;
			this.btnResetHelixApiToken.Text = "Сбросить";
			this.btnResetHelixApiToken.UseVisualStyleBackColor = true;
			this.btnResetHelixApiToken.Click += new System.EventHandler(this.btnResetHelixApiToken_Click);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(8, 22);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(70, 13);
			this.label7.TabIndex = 0;
			this.label7.Text = "OAuth token:";
			// 
			// btnUpdateHelixApiToken
			// 
			this.btnUpdateHelixApiToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUpdateHelixApiToken.Location = new System.Drawing.Point(702, 45);
			this.btnUpdateHelixApiToken.Name = "btnUpdateHelixApiToken";
			this.btnUpdateHelixApiToken.Size = new System.Drawing.Size(75, 23);
			this.btnUpdateHelixApiToken.TabIndex = 4;
			this.btnUpdateHelixApiToken.Text = "Обновить";
			this.btnUpdateHelixApiToken.UseVisualStyleBackColor = true;
			this.btnUpdateHelixApiToken.Click += new System.EventHandler(this.btnUpdateHelixApiToken_Click);
			// 
			// textBoxHelixApiToken
			// 
			this.textBoxHelixApiToken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxHelixApiToken.Location = new System.Drawing.Point(115, 19);
			this.textBoxHelixApiToken.Name = "textBoxHelixApiToken";
			this.textBoxHelixApiToken.ReadOnly = true;
			this.textBoxHelixApiToken.Size = new System.Drawing.Size(662, 20);
			this.textBoxHelixApiToken.TabIndex = 1;
			this.textBoxHelixApiToken.Text = "<NULL>";
			// 
			// lblHelixApiTokenExpirationDate
			// 
			this.lblHelixApiTokenExpirationDate.AutoSize = true;
			this.lblHelixApiTokenExpirationDate.Location = new System.Drawing.Point(112, 50);
			this.lblHelixApiTokenExpirationDate.Name = "lblHelixApiTokenExpirationDate";
			this.lblHelixApiTokenExpirationDate.Size = new System.Drawing.Size(80, 13);
			this.lblHelixApiTokenExpirationDate.TabIndex = 3;
			this.lblHelixApiTokenExpirationDate.Text = "<Неизвестно>";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(8, 50);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(98, 13);
			this.label8.TabIndex = 2;
			this.label8.Text = "Действителен до:";
			// 
			// groupBoxVideoInformationSettings
			// 
			this.groupBoxVideoInformationSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxVideoInformationSettings.Controls.Add(this.chkSaveVodChunksInfo);
			this.groupBoxVideoInformationSettings.Controls.Add(this.chkUseGmtTime);
			this.groupBoxVideoInformationSettings.Controls.Add(this.chkSaveVodInfo);
			this.groupBoxVideoInformationSettings.Location = new System.Drawing.Point(8, 152);
			this.groupBoxVideoInformationSettings.Name = "groupBoxVideoInformationSettings";
			this.groupBoxVideoInformationSettings.Size = new System.Drawing.Size(804, 47);
			this.groupBoxVideoInformationSettings.TabIndex = 20;
			this.groupBoxVideoInformationSettings.TabStop = false;
			this.groupBoxVideoInformationSettings.Text = "Информация о видео";
			// 
			// chkSaveVodChunksInfo
			// 
			this.chkSaveVodChunksInfo.AutoSize = true;
			this.chkSaveVodChunksInfo.Location = new System.Drawing.Point(202, 19);
			this.chkSaveVodChunksInfo.Name = "chkSaveVodChunksInfo";
			this.chkSaveVodChunksInfo.Size = new System.Drawing.Size(194, 17);
			this.chkSaveVodChunksInfo.TabIndex = 18;
			this.chkSaveVodChunksInfo.Text = "Сохранять информацию о чанках";
			this.chkSaveVodChunksInfo.UseVisualStyleBackColor = true;
			this.chkSaveVodChunksInfo.CheckedChanged += new System.EventHandler(this.chkSaveVodChunksInfo_CheckedChanged);
			// 
			// chkUseGmtTime
			// 
			this.chkUseGmtTime.AutoSize = true;
			this.chkUseGmtTime.Location = new System.Drawing.Point(402, 19);
			this.chkUseGmtTime.Name = "chkUseGmtTime";
			this.chkUseGmtTime.Size = new System.Drawing.Size(176, 17);
			this.chkUseGmtTime.TabIndex = 16;
			this.chkUseGmtTime.Text = "Использовать время по GMT";
			this.chkUseGmtTime.UseVisualStyleBackColor = true;
			this.chkUseGmtTime.CheckedChanged += new System.EventHandler(this.chkUseGmtTime_CheckedChanged);
			// 
			// chkSaveVodInfo
			// 
			this.chkSaveVodInfo.AutoSize = true;
			this.chkSaveVodInfo.Location = new System.Drawing.Point(6, 19);
			this.chkSaveVodInfo.Name = "chkSaveVodInfo";
			this.chkSaveVodInfo.Size = new System.Drawing.Size(190, 17);
			this.chkSaveVodInfo.TabIndex = 17;
			this.chkSaveVodInfo.Text = "Сохранять информацию о видео";
			this.chkSaveVodInfo.UseVisualStyleBackColor = true;
			this.chkSaveVodInfo.CheckedChanged += new System.EventHandler(this.chkSaveVodInfo_CheckedChanged);
			// 
			// groupBoxFilesAndFoldersSettings
			// 
			this.groupBoxFilesAndFoldersSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.label4);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.label5);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.textBox_Browser);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.textBox_DownloadingPath);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.btnRestoreDefaultFilenameFormat);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.btnSelectDownloadingPath);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.textBox_FileNameFormat);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.btnSelectBrowser);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.label2);
			this.groupBoxFilesAndFoldersSettings.Location = new System.Drawing.Point(8, 6);
			this.groupBoxFilesAndFoldersSettings.Name = "groupBoxFilesAndFoldersSettings";
			this.groupBoxFilesAndFoldersSettings.Size = new System.Drawing.Size(804, 140);
			this.groupBoxFilesAndFoldersSettings.TabIndex = 19;
			this.groupBoxFilesAndFoldersSettings.TabStop = false;
			this.groupBoxFilesAndFoldersSettings.Text = "Файлы и папки";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(125, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Папка для скачивания:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 94);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(73, 13);
			this.label5.TabIndex = 6;
			this.label5.Text = "Веб-браузер:";
			// 
			// textBox_Browser
			// 
			this.textBox_Browser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_Browser.Location = new System.Drawing.Point(7, 110);
			this.textBox_Browser.Name = "textBox_Browser";
			this.textBox_Browser.Size = new System.Drawing.Size(743, 20);
			this.textBox_Browser.TabIndex = 7;
			// 
			// textBox_DownloadingPath
			// 
			this.textBox_DownloadingPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_DownloadingPath.Location = new System.Drawing.Point(7, 32);
			this.textBox_DownloadingPath.Name = "textBox_DownloadingPath";
			this.textBox_DownloadingPath.Size = new System.Drawing.Size(743, 20);
			this.textBox_DownloadingPath.TabIndex = 8;
			this.textBox_DownloadingPath.Leave += new System.EventHandler(this.TextBox_DownloadingPath_Leave);
			// 
			// btnRestoreDefaultFilenameFormat
			// 
			this.btnRestoreDefaultFilenameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRestoreDefaultFilenameFormat.Location = new System.Drawing.Point(689, 71);
			this.btnRestoreDefaultFilenameFormat.Name = "btnRestoreDefaultFilenameFormat";
			this.btnRestoreDefaultFilenameFormat.Size = new System.Drawing.Size(109, 23);
			this.btnRestoreDefaultFilenameFormat.TabIndex = 15;
			this.btnRestoreDefaultFilenameFormat.Text = "Вернуть как было";
			this.btnRestoreDefaultFilenameFormat.UseVisualStyleBackColor = true;
			this.btnRestoreDefaultFilenameFormat.Click += new System.EventHandler(this.btnRestoreDefaultFilenameFormat_Click);
			// 
			// btnSelectDownloadingPath
			// 
			this.btnSelectDownloadingPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelectDownloadingPath.Location = new System.Drawing.Point(756, 32);
			this.btnSelectDownloadingPath.Name = "btnSelectDownloadingPath";
			this.btnSelectDownloadingPath.Size = new System.Drawing.Size(42, 22);
			this.btnSelectDownloadingPath.TabIndex = 9;
			this.btnSelectDownloadingPath.Text = "...";
			this.btnSelectDownloadingPath.UseVisualStyleBackColor = true;
			this.btnSelectDownloadingPath.Click += new System.EventHandler(this.btnSelectDownloadingPath_Click);
			// 
			// textBox_FileNameFormat
			// 
			this.textBox_FileNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_FileNameFormat.Location = new System.Drawing.Point(9, 71);
			this.textBox_FileNameFormat.Name = "textBox_FileNameFormat";
			this.textBox_FileNameFormat.Size = new System.Drawing.Size(674, 20);
			this.textBox_FileNameFormat.TabIndex = 14;
			this.textBox_FileNameFormat.Leave += new System.EventHandler(this.textBox_FileNameFormat_Leave);
			// 
			// btnSelectBrowser
			// 
			this.btnSelectBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelectBrowser.Location = new System.Drawing.Point(756, 110);
			this.btnSelectBrowser.Name = "btnSelectBrowser";
			this.btnSelectBrowser.Size = new System.Drawing.Size(42, 20);
			this.btnSelectBrowser.TabIndex = 10;
			this.btnSelectBrowser.Text = "...";
			this.btnSelectBrowser.UseVisualStyleBackColor = true;
			this.btnSelectBrowser.Click += new System.EventHandler(this.btnSelectBrowser_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(122, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "Формат имени файла:";
			// 
			// tabPageSearch
			// 
			this.tabPageSearch.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageSearch.Controls.Add(this.groupBox2);
			this.tabPageSearch.Controls.Add(this.groupBox1);
			this.tabPageSearch.Location = new System.Drawing.Point(4, 22);
			this.tabPageSearch.Name = "tabPageSearch";
			this.tabPageSearch.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageSearch.Size = new System.Drawing.Size(818, 472);
			this.tabPageSearch.TabIndex = 5;
			this.tabPageSearch.Text = "Поиск видео";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnSearchByUrls);
			this.groupBox2.Controls.Add(this.textBoxUrls);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Location = new System.Drawing.Point(323, 16);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(352, 162);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Поиск видео по ссылкам";
			// 
			// btnSearchByUrls
			// 
			this.btnSearchByUrls.Location = new System.Drawing.Point(271, 133);
			this.btnSearchByUrls.Name = "btnSearchByUrls";
			this.btnSearchByUrls.Size = new System.Drawing.Size(75, 23);
			this.btnSearchByUrls.TabIndex = 2;
			this.btnSearchByUrls.Text = "Искать";
			this.btnSearchByUrls.UseVisualStyleBackColor = true;
			this.btnSearchByUrls.Click += new System.EventHandler(this.btnSearchByUrls_Click);
			// 
			// textBoxUrls
			// 
			this.textBoxUrls.Location = new System.Drawing.Point(18, 38);
			this.textBoxUrls.Multiline = true;
			this.textBoxUrls.Name = "textBoxUrls";
			this.textBoxUrls.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxUrls.Size = new System.Drawing.Size(328, 89);
			this.textBoxUrls.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(15, 17);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(181, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "Введите ссылки на видео с твича:";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnEditChannelList);
			this.groupBox1.Controls.Add(this.btnAddChannelToList);
			this.groupBox1.Controls.Add(this.textBoxChannelName);
			this.groupBox1.Controls.Add(this.listBoxChannelList);
			this.groupBox1.Controls.Add(this.groupBox3);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.btnSearchChannelName);
			this.groupBox1.Location = new System.Drawing.Point(6, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(311, 308);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Поиск по названию канала";
			// 
			// btnEditChannelList
			// 
			this.btnEditChannelList.Location = new System.Drawing.Point(0, 198);
			this.btnEditChannelList.Name = "btnEditChannelList";
			this.btnEditChannelList.Size = new System.Drawing.Size(105, 23);
			this.btnEditChannelList.TabIndex = 9;
			this.btnEditChannelList.Text = "Редактировать";
			this.btnEditChannelList.UseVisualStyleBackColor = true;
			this.btnEditChannelList.Click += new System.EventHandler(this.btnEditChannelList_Click);
			// 
			// btnAddChannelToList
			// 
			this.btnAddChannelToList.Location = new System.Drawing.Point(216, 17);
			this.btnAddChannelToList.Name = "btnAddChannelToList";
			this.btnAddChannelToList.Size = new System.Drawing.Size(75, 23);
			this.btnAddChannelToList.TabIndex = 8;
			this.btnAddChannelToList.Text = "< Добавить";
			this.toolTip1.SetToolTip(this.btnAddChannelToList, "Добавить канал в список");
			this.btnAddChannelToList.UseVisualStyleBackColor = true;
			this.btnAddChannelToList.Click += new System.EventHandler(this.btnAddChannelToList_Click);
			// 
			// textBoxChannelName
			// 
			this.textBoxChannelName.Location = new System.Drawing.Point(110, 19);
			this.textBoxChannelName.Name = "textBoxChannelName";
			this.textBoxChannelName.Size = new System.Drawing.Size(100, 20);
			this.textBoxChannelName.TabIndex = 7;
			// 
			// listBoxChannelList
			// 
			this.listBoxChannelList.FormattingEnabled = true;
			this.listBoxChannelList.Location = new System.Drawing.Point(2, 45);
			this.listBoxChannelList.Name = "listBoxChannelList";
			this.listBoxChannelList.Size = new System.Drawing.Size(303, 147);
			this.listBoxChannelList.TabIndex = 6;
			this.listBoxChannelList.SelectedIndexChanged += new System.EventHandler(this.listBoxChannelList_SelectedIndexChanged);
			this.listBoxChannelList.DoubleClick += new System.EventHandler(this.listBoxChannelList_DoubleClick);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.numericUpDownSearchLimit);
			this.groupBox3.Controls.Add(this.rbSearchLimit);
			this.groupBox3.Controls.Add(this.rbSearchAll);
			this.groupBox3.Location = new System.Drawing.Point(6, 227);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(299, 75);
			this.groupBox3.TabIndex = 5;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Диапазон поиска";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(127, 49);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(93, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "последних видео";
			// 
			// numericUpDownSearchLimit
			// 
			this.numericUpDownSearchLimit.Location = new System.Drawing.Point(76, 45);
			this.numericUpDownSearchLimit.Maximum = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.numericUpDownSearchLimit.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.numericUpDownSearchLimit.Name = "numericUpDownSearchLimit";
			this.numericUpDownSearchLimit.Size = new System.Drawing.Size(45, 20);
			this.numericUpDownSearchLimit.TabIndex = 2;
			this.numericUpDownSearchLimit.Value = new decimal(new int[] {
			5,
			0,
			0,
			0});
			// 
			// rbSearchLimit
			// 
			this.rbSearchLimit.AutoSize = true;
			this.rbSearchLimit.Checked = true;
			this.rbSearchLimit.Location = new System.Drawing.Point(8, 45);
			this.rbSearchLimit.Name = "rbSearchLimit";
			this.rbSearchLimit.Size = new System.Drawing.Size(62, 17);
			this.rbSearchLimit.TabIndex = 1;
			this.rbSearchLimit.TabStop = true;
			this.rbSearchLimit.Text = "Только";
			this.rbSearchLimit.UseVisualStyleBackColor = true;
			// 
			// rbSearchAll
			// 
			this.rbSearchAll.AutoSize = true;
			this.rbSearchAll.Location = new System.Drawing.Point(8, 18);
			this.rbSearchAll.Name = "rbSearchAll";
			this.rbSearchAll.Size = new System.Drawing.Size(196, 17);
			this.rbSearchAll.TabIndex = 0;
			this.rbSearchAll.Text = "Все видео канала (кроме клипов)";
			this.toolTip1.SetToolTip(this.rbSearchAll, "Может быть очень долго!");
			this.rbSearchAll.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Название канала:";
			// 
			// btnSearchChannelName
			// 
			this.btnSearchChannelName.Location = new System.Drawing.Point(230, 198);
			this.btnSearchChannelName.Name = "btnSearchChannelName";
			this.btnSearchChannelName.Size = new System.Drawing.Size(75, 23);
			this.btnSearchChannelName.TabIndex = 1;
			this.btnSearchChannelName.Text = "Искать";
			this.btnSearchChannelName.UseVisualStyleBackColor = true;
			this.btnSearchChannelName.Click += new System.EventHandler(this.btnSearchChannelName_Click);
			// 
			// tabPageLog
			// 
			this.tabPageLog.BackColor = System.Drawing.Color.Black;
			this.tabPageLog.Controls.Add(this.lbLog);
			this.tabPageLog.Location = new System.Drawing.Point(4, 22);
			this.tabPageLog.Name = "tabPageLog";
			this.tabPageLog.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageLog.Size = new System.Drawing.Size(818, 472);
			this.tabPageLog.TabIndex = 3;
			this.tabPageLog.Text = "Лог событий";
			// 
			// lbLog
			// 
			this.lbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lbLog.BackColor = System.Drawing.Color.Black;
			this.lbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lbLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lbLog.ForeColor = System.Drawing.Color.Lime;
			this.lbLog.FormattingEnabled = true;
			this.lbLog.ItemHeight = 24;
			this.lbLog.Location = new System.Drawing.Point(0, 0);
			this.lbLog.Name = "lbLog";
			this.lbLog.Size = new System.Drawing.Size(812, 432);
			this.lbLog.TabIndex = 0;
			// 
			// tabPageStreams
			// 
			this.tabPageStreams.Controls.Add(this.panelStreams);
			this.tabPageStreams.Controls.Add(this.scrollBarStreams);
			this.tabPageStreams.Location = new System.Drawing.Point(4, 22);
			this.tabPageStreams.Name = "tabPageStreams";
			this.tabPageStreams.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageStreams.Size = new System.Drawing.Size(818, 472);
			this.tabPageStreams.TabIndex = 4;
			this.tabPageStreams.Text = "Стримы";
			this.tabPageStreams.UseVisualStyleBackColor = true;
			// 
			// panelStreams
			// 
			this.panelStreams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.panelStreams.BackColor = System.Drawing.Color.Black;
			this.panelStreams.Location = new System.Drawing.Point(0, 0);
			this.panelStreams.Name = "panelStreams";
			this.panelStreams.Size = new System.Drawing.Size(795, 472);
			this.panelStreams.TabIndex = 2;
			this.panelStreams.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelStreams_MouseDown);
			// 
			// scrollBarStreams
			// 
			this.scrollBarStreams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.scrollBarStreams.Location = new System.Drawing.Point(798, 0);
			this.scrollBarStreams.Name = "scrollBarStreams";
			this.scrollBarStreams.Size = new System.Drawing.Size(17, 472);
			this.scrollBarStreams.TabIndex = 1;
			this.scrollBarStreams.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollBarStreams_Scroll);
			// 
			// tabPageDownloading
			// 
			this.tabPageDownloading.BackColor = System.Drawing.Color.DimGray;
			this.tabPageDownloading.Controls.Add(this.scrollBarDownloads);
			this.tabPageDownloading.Controls.Add(this.panelDownloads);
			this.tabPageDownloading.Location = new System.Drawing.Point(4, 22);
			this.tabPageDownloading.Name = "tabPageDownloading";
			this.tabPageDownloading.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDownloading.Size = new System.Drawing.Size(818, 472);
			this.tabPageDownloading.TabIndex = 6;
			this.tabPageDownloading.Text = "Скачивание";
			// 
			// scrollBarDownloads
			// 
			this.scrollBarDownloads.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.scrollBarDownloads.Enabled = false;
			this.scrollBarDownloads.Location = new System.Drawing.Point(801, 0);
			this.scrollBarDownloads.Name = "scrollBarDownloads";
			this.scrollBarDownloads.Size = new System.Drawing.Size(17, 472);
			this.scrollBarDownloads.TabIndex = 1;
			this.scrollBarDownloads.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollBarDownloads_Scroll);
			// 
			// panelDownloads
			// 
			this.panelDownloads.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.panelDownloads.BackColor = System.Drawing.Color.DarkGray;
			this.panelDownloads.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.panelDownloads.Location = new System.Drawing.Point(0, 0);
			this.panelDownloads.Name = "panelDownloads";
			this.panelDownloads.Size = new System.Drawing.Size(801, 472);
			this.panelDownloads.TabIndex = 0;
			// 
			// contextMenuStreamImage
			// 
			this.contextMenuStreamImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.miCopyVideoUrl,
			this.copyImageUrlToolStripMenuItem,
			this.copyStreamInfoJsonToolStripMenuItem,
			this.miSavePlaylistAsToolStripMenuItem,
			this.saveImageAssToolStripMenuItem,
			this.toolStripMenuItem1,
			this.openVideoInBrowserToolStripMenuItem});
			this.contextMenuStreamImage.Name = "contextMenuStreamImage";
			this.contextMenuStreamImage.Size = new System.Drawing.Size(283, 142);
			// 
			// miCopyVideoUrl
			// 
			this.miCopyVideoUrl.Name = "miCopyVideoUrl";
			this.miCopyVideoUrl.Size = new System.Drawing.Size(282, 22);
			this.miCopyVideoUrl.Text = "Скопировать ссылку на видео";
			this.miCopyVideoUrl.Click += new System.EventHandler(this.miCopyVideoUrl_Click);
			// 
			// copyImageUrlToolStripMenuItem
			// 
			this.copyImageUrlToolStripMenuItem.Name = "copyImageUrlToolStripMenuItem";
			this.copyImageUrlToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
			this.copyImageUrlToolStripMenuItem.Text = "Скопировать ссылку на изображение";
			this.copyImageUrlToolStripMenuItem.Click += new System.EventHandler(this.CopyImageUrlToolStripMenuItem_Click);
			// 
			// copyStreamInfoJsonToolStripMenuItem
			// 
			this.copyStreamInfoJsonToolStripMenuItem.Name = "copyStreamInfoJsonToolStripMenuItem";
			this.copyStreamInfoJsonToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
			this.copyStreamInfoJsonToolStripMenuItem.Text = "Скопировать информацию о стриме";
			this.copyStreamInfoJsonToolStripMenuItem.Click += new System.EventHandler(this.CopyStreamInfoJsonToolStripMenuItem_Click);
			// 
			// miSavePlaylistAsToolStripMenuItem
			// 
			this.miSavePlaylistAsToolStripMenuItem.Name = "miSavePlaylistAsToolStripMenuItem";
			this.miSavePlaylistAsToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
			this.miSavePlaylistAsToolStripMenuItem.Text = "Сохранить плейлист как...";
			this.miSavePlaylistAsToolStripMenuItem.Click += new System.EventHandler(this.miSavePlaylistAsToolStripMenuItem_Click);
			// 
			// saveImageAssToolStripMenuItem
			// 
			this.saveImageAssToolStripMenuItem.Name = "saveImageAssToolStripMenuItem";
			this.saveImageAssToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
			this.saveImageAssToolStripMenuItem.Text = "Сохранить изображение как...";
			this.saveImageAssToolStripMenuItem.Click += new System.EventHandler(this.saveImageAssToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(279, 6);
			// 
			// openVideoInBrowserToolStripMenuItem
			// 
			this.openVideoInBrowserToolStripMenuItem.Name = "openVideoInBrowserToolStripMenuItem";
			this.openVideoInBrowserToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
			this.openVideoInBrowserToolStripMenuItem.Text = "Открыть видео в браузере";
			this.openVideoInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openVideoInBrowserToolStripMenuItem_Click);
			// 
			// Form1
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(828, 501);
			this.Controls.Add(this.tabControlMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(844, 540);
			this.Name = "Form1";
			this.Text = "Twitch prime downloader";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Resize += new System.EventHandler(this.Form1_Resize);
			this.tabControlMain.ResumeLayout(false);
			this.tabPageDebug.ResumeLayout(false);
			this.tabPageSettings.ResumeLayout(false);
			this.groupBoxApiSettings.ResumeLayout(false);
			this.groupBoxTwitchApplication.ResumeLayout(false);
			this.groupBoxTwitchApplication.PerformLayout();
			this.groupBoxHelixApiToken.ResumeLayout(false);
			this.groupBoxHelixApiToken.PerformLayout();
			this.groupBoxVideoInformationSettings.ResumeLayout(false);
			this.groupBoxVideoInformationSettings.PerformLayout();
			this.groupBoxFilesAndFoldersSettings.ResumeLayout(false);
			this.groupBoxFilesAndFoldersSettings.PerformLayout();
			this.tabPageSearch.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSearchLimit)).EndInit();
			this.tabPageLog.ResumeLayout(false);
			this.tabPageStreams.ResumeLayout(false);
			this.tabPageDownloading.ResumeLayout(false);
			this.contextMenuStreamImage.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControlMain;
		private System.Windows.Forms.TabPage tabPageSettings;
		private System.Windows.Forms.Button btnSelectBrowser;
		private System.Windows.Forms.Button btnSelectDownloadingPath;
		private System.Windows.Forms.TextBox textBox_DownloadingPath;
		private System.Windows.Forms.TextBox textBox_Browser;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ContextMenuStrip contextMenuStreamImage;
		private System.Windows.Forms.ToolStripMenuItem copyImageUrlToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyStreamInfoJsonToolStripMenuItem;
		private System.Windows.Forms.TabPage tabPageDebug;
		private System.Windows.Forms.RichTextBox memoDebug;
		private System.Windows.Forms.TabPage tabPageLog;
		private System.Windows.Forms.ListBox lbLog;
		private System.Windows.Forms.TabPage tabPageStreams;
		private System.Windows.Forms.TabPage tabPageSearch;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSearchChannelName;
		private System.Windows.Forms.TabPage tabPageDownloading;
		private System.Windows.Forms.Panel panelDownloads;
		private System.Windows.Forms.VScrollBar scrollBarDownloads;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.NumericUpDown numericUpDownSearchLimit;
		private System.Windows.Forms.RadioButton rbSearchLimit;
		private System.Windows.Forms.RadioButton rbSearchAll;
		private System.Windows.Forms.ToolStripMenuItem saveImageAssToolStripMenuItem;
		private System.Windows.Forms.VScrollBar scrollBarStreams;
		private System.Windows.Forms.Panel panelStreams;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ToolStripMenuItem openVideoInBrowserToolStripMenuItem;
		private System.Windows.Forms.TextBox textBox_FileNameFormat;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem miCopyVideoUrl;
		private System.Windows.Forms.Button btnRestoreDefaultFilenameFormat;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnSearchByUrls;
		private System.Windows.Forms.TextBox textBoxUrls;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox chkUseGmtTime;
		private System.Windows.Forms.CheckBox chkSaveVodInfo;
		private System.Windows.Forms.CheckBox chkSaveVodChunksInfo;
		private System.Windows.Forms.ToolStripMenuItem miSavePlaylistAsToolStripMenuItem;
		private System.Windows.Forms.Button btnAddChannelToList;
		private System.Windows.Forms.TextBox textBoxChannelName;
		private System.Windows.Forms.ListBox listBoxChannelList;
		private System.Windows.Forms.Button btnEditChannelList;
		private System.Windows.Forms.GroupBox groupBoxFilesAndFoldersSettings;
		private System.Windows.Forms.GroupBox groupBoxVideoInformationSettings;
		private System.Windows.Forms.GroupBox groupBoxApiSettings;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textBoxHelixApiToken;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label lblHelixApiTokenExpirationDate;
		private System.Windows.Forms.Button btnUpdateHelixApiToken;
		private System.Windows.Forms.GroupBox groupBoxHelixApiToken;
		private System.Windows.Forms.GroupBox groupBoxTwitchApplication;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textBoxHelixApiClentSecretKey;
		private System.Windows.Forms.TextBox textBoxHelixApiClientId;
		private System.Windows.Forms.TextBox textBoxApiApplicationDescription;
		private System.Windows.Forms.TextBox textBoxApiApplicationTitle;
		private System.Windows.Forms.Button btnSetDefaultApiApplication;
		private System.Windows.Forms.Button btnApplyApiApplication;
		private System.Windows.Forms.Button btnResetHelixApiToken;
	}
}
