
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
			this.richTextBoxDebugLog = new System.Windows.Forms.RichTextBox();
			this.tabPageSettings = new System.Windows.Forms.TabPage();
			this.groupBoxApiSettings = new System.Windows.Forms.GroupBox();
			this.groupBoxTwitchApplicationSettings = new System.Windows.Forms.GroupBox();
			this.btnApplyApiApplication = new System.Windows.Forms.Button();
			this.btnRestoreDefaultApiApplication = new System.Windows.Forms.Button();
			this.textBoxHelixApiClientSecretKey = new System.Windows.Forms.TextBox();
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
			this.checkBoxSaveVodChunkInfo = new System.Windows.Forms.CheckBox();
			this.checkBoxUseGmtTime = new System.Windows.Forms.CheckBox();
			this.checkBoxSaveVodInfo = new System.Windows.Forms.CheckBox();
			this.groupBoxFilesAndFoldersSettings = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textBoxBrowserExePath = new System.Windows.Forms.TextBox();
			this.textBoxDownloadDirectory = new System.Windows.Forms.TextBox();
			this.btnRestoreDefaultOutputFileNameFormat = new System.Windows.Forms.Button();
			this.btnSelectDownloadDirectory = new System.Windows.Forms.Button();
			this.textBoxOutputFileNameFormat = new System.Windows.Forms.TextBox();
			this.btnSelectBrowser = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.tabPageSearch = new System.Windows.Forms.TabPage();
			this.groupBoxSearchByUrls = new System.Windows.Forms.GroupBox();
			this.btnSearchByUrls = new System.Windows.Forms.Button();
			this.textBoxVideoUrls = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBoxSearchByChannelName = new System.Windows.Forms.GroupBox();
			this.btnEditChannelList = new System.Windows.Forms.Button();
			this.btnAddChannelToList = new System.Windows.Forms.Button();
			this.textBoxChannelName = new System.Windows.Forms.TextBox();
			this.listBoxChannelList = new System.Windows.Forms.ListBox();
			this.groupBoxSearchResultCount = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.numericUpDownSearchLimit = new System.Windows.Forms.NumericUpDown();
			this.radioButtonSearchLimited = new System.Windows.Forms.RadioButton();
			this.radioButtonSearchAll = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSearchByChannelName = new System.Windows.Forms.Button();
			this.tabPageEventLog = new System.Windows.Forms.TabPage();
			this.listBoxEventLog = new System.Windows.Forms.ListBox();
			this.tabPageStreams = new System.Windows.Forms.TabPage();
			this.panelStreams = new System.Windows.Forms.Panel();
			this.scrollBarStreams = new System.Windows.Forms.VScrollBar();
			this.tabPageDownloads = new System.Windows.Forms.TabPage();
			this.scrollBarDownloads = new System.Windows.Forms.VScrollBar();
			this.panelDownloads = new System.Windows.Forms.Panel();
			this.contextMenuVodThumbnailImage = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miCopyVideoUrl = new System.Windows.Forms.ToolStripMenuItem();
			this.miCopyVodThumbnailImageUrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miCopyVodInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miSaveVodPlaylistAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miSaveVodThumbnailImageAssToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.miOpenVideoInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tabControlMain.SuspendLayout();
			this.tabPageDebug.SuspendLayout();
			this.tabPageSettings.SuspendLayout();
			this.groupBoxApiSettings.SuspendLayout();
			this.groupBoxTwitchApplicationSettings.SuspendLayout();
			this.groupBoxHelixApiToken.SuspendLayout();
			this.groupBoxVideoInformationSettings.SuspendLayout();
			this.groupBoxFilesAndFoldersSettings.SuspendLayout();
			this.tabPageSearch.SuspendLayout();
			this.groupBoxSearchByUrls.SuspendLayout();
			this.groupBoxSearchByChannelName.SuspendLayout();
			this.groupBoxSearchResultCount.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSearchLimit)).BeginInit();
			this.tabPageEventLog.SuspendLayout();
			this.tabPageStreams.SuspendLayout();
			this.tabPageDownloads.SuspendLayout();
			this.contextMenuVodThumbnailImage.SuspendLayout();
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
			this.tabControlMain.Controls.Add(this.tabPageEventLog);
			this.tabControlMain.Controls.Add(this.tabPageStreams);
			this.tabControlMain.Controls.Add(this.tabPageDownloads);
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
			this.tabPageDebug.Controls.Add(this.richTextBoxDebugLog);
			this.tabPageDebug.Location = new System.Drawing.Point(4, 22);
			this.tabPageDebug.Name = "tabPageDebug";
			this.tabPageDebug.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDebug.Size = new System.Drawing.Size(818, 472);
			this.tabPageDebug.TabIndex = 2;
			this.tabPageDebug.Text = "Debug";
			// 
			// richTextBoxDebugLog
			// 
			this.richTextBoxDebugLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBoxDebugLog.BackColor = System.Drawing.Color.Black;
			this.richTextBoxDebugLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.richTextBoxDebugLog.ForeColor = System.Drawing.Color.Lime;
			this.richTextBoxDebugLog.Location = new System.Drawing.Point(6, 3);
			this.richTextBoxDebugLog.Name = "richTextBoxDebugLog";
			this.richTextBoxDebugLog.Size = new System.Drawing.Size(806, 466);
			this.richTextBoxDebugLog.TabIndex = 0;
			this.richTextBoxDebugLog.Text = "";
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
			this.groupBoxApiSettings.Controls.Add(this.groupBoxTwitchApplicationSettings);
			this.groupBoxApiSettings.Controls.Add(this.groupBoxHelixApiToken);
			this.groupBoxApiSettings.Location = new System.Drawing.Point(8, 205);
			this.groupBoxApiSettings.Name = "groupBoxApiSettings";
			this.groupBoxApiSettings.Size = new System.Drawing.Size(798, 260);
			this.groupBoxApiSettings.TabIndex = 21;
			this.groupBoxApiSettings.TabStop = false;
			this.groupBoxApiSettings.Text = "API";
			// 
			// groupBoxTwitchApplicationSettings
			// 
			this.groupBoxTwitchApplicationSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxTwitchApplicationSettings.Controls.Add(this.btnApplyApiApplication);
			this.groupBoxTwitchApplicationSettings.Controls.Add(this.btnRestoreDefaultApiApplication);
			this.groupBoxTwitchApplicationSettings.Controls.Add(this.textBoxHelixApiClientSecretKey);
			this.groupBoxTwitchApplicationSettings.Controls.Add(this.textBoxHelixApiClientId);
			this.groupBoxTwitchApplicationSettings.Controls.Add(this.textBoxApiApplicationDescription);
			this.groupBoxTwitchApplicationSettings.Controls.Add(this.textBoxApiApplicationTitle);
			this.groupBoxTwitchApplicationSettings.Controls.Add(this.label12);
			this.groupBoxTwitchApplicationSettings.Controls.Add(this.label11);
			this.groupBoxTwitchApplicationSettings.Controls.Add(this.label10);
			this.groupBoxTwitchApplicationSettings.Controls.Add(this.label9);
			this.groupBoxTwitchApplicationSettings.Location = new System.Drawing.Point(7, 19);
			this.groupBoxTwitchApplicationSettings.Name = "groupBoxTwitchApplicationSettings";
			this.groupBoxTwitchApplicationSettings.Size = new System.Drawing.Size(785, 154);
			this.groupBoxTwitchApplicationSettings.TabIndex = 6;
			this.groupBoxTwitchApplicationSettings.TabStop = false;
			this.groupBoxTwitchApplicationSettings.Text = "Twitch application";
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
			// btnRestoreDefaultApiApplication
			// 
			this.btnRestoreDefaultApiApplication.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRestoreDefaultApiApplication.Location = new System.Drawing.Point(682, 126);
			this.btnRestoreDefaultApiApplication.Name = "btnRestoreDefaultApiApplication";
			this.btnRestoreDefaultApiApplication.Size = new System.Drawing.Size(97, 23);
			this.btnRestoreDefaultApiApplication.TabIndex = 8;
			this.btnRestoreDefaultApiApplication.Text = "По-умолчанию";
			this.btnRestoreDefaultApiApplication.UseVisualStyleBackColor = true;
			this.btnRestoreDefaultApiApplication.Click += new System.EventHandler(this.btnRestoreDefaultApiApplication_Click);
			// 
			// textBoxHelixApiClientSecretKey
			// 
			this.textBoxHelixApiClientSecretKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxHelixApiClientSecretKey.Location = new System.Drawing.Point(100, 97);
			this.textBoxHelixApiClientSecretKey.Name = "textBoxHelixApiClientSecretKey";
			this.textBoxHelixApiClientSecretKey.Size = new System.Drawing.Size(685, 20);
			this.textBoxHelixApiClientSecretKey.TabIndex = 7;
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
			this.groupBoxVideoInformationSettings.Controls.Add(this.checkBoxSaveVodChunkInfo);
			this.groupBoxVideoInformationSettings.Controls.Add(this.checkBoxUseGmtTime);
			this.groupBoxVideoInformationSettings.Controls.Add(this.checkBoxSaveVodInfo);
			this.groupBoxVideoInformationSettings.Location = new System.Drawing.Point(8, 152);
			this.groupBoxVideoInformationSettings.Name = "groupBoxVideoInformationSettings";
			this.groupBoxVideoInformationSettings.Size = new System.Drawing.Size(804, 47);
			this.groupBoxVideoInformationSettings.TabIndex = 20;
			this.groupBoxVideoInformationSettings.TabStop = false;
			this.groupBoxVideoInformationSettings.Text = "Информация о видео";
			// 
			// checkBoxSaveVodChunkInfo
			// 
			this.checkBoxSaveVodChunkInfo.AutoSize = true;
			this.checkBoxSaveVodChunkInfo.Location = new System.Drawing.Point(202, 19);
			this.checkBoxSaveVodChunkInfo.Name = "checkBoxSaveVodChunkInfo";
			this.checkBoxSaveVodChunkInfo.Size = new System.Drawing.Size(194, 17);
			this.checkBoxSaveVodChunkInfo.TabIndex = 18;
			this.checkBoxSaveVodChunkInfo.Text = "Сохранять информацию о чанках";
			this.checkBoxSaveVodChunkInfo.UseVisualStyleBackColor = true;
			this.checkBoxSaveVodChunkInfo.CheckedChanged += new System.EventHandler(this.checkBoxSaveVodChunkInfo_CheckedChanged);
			// 
			// checkBoxUseGmtTime
			// 
			this.checkBoxUseGmtTime.AutoSize = true;
			this.checkBoxUseGmtTime.Location = new System.Drawing.Point(402, 19);
			this.checkBoxUseGmtTime.Name = "checkBoxUseGmtTime";
			this.checkBoxUseGmtTime.Size = new System.Drawing.Size(176, 17);
			this.checkBoxUseGmtTime.TabIndex = 16;
			this.checkBoxUseGmtTime.Text = "Использовать время по GMT";
			this.checkBoxUseGmtTime.UseVisualStyleBackColor = true;
			this.checkBoxUseGmtTime.CheckedChanged += new System.EventHandler(this.checkBoxUseGmtTime_CheckedChanged);
			// 
			// checkBoxSaveVodInfo
			// 
			this.checkBoxSaveVodInfo.AutoSize = true;
			this.checkBoxSaveVodInfo.Location = new System.Drawing.Point(6, 19);
			this.checkBoxSaveVodInfo.Name = "checkBoxSaveVodInfo";
			this.checkBoxSaveVodInfo.Size = new System.Drawing.Size(190, 17);
			this.checkBoxSaveVodInfo.TabIndex = 17;
			this.checkBoxSaveVodInfo.Text = "Сохранять информацию о видео";
			this.checkBoxSaveVodInfo.UseVisualStyleBackColor = true;
			this.checkBoxSaveVodInfo.CheckedChanged += new System.EventHandler(this.checkBoxSaveVodInfo_CheckedChanged);
			// 
			// groupBoxFilesAndFoldersSettings
			// 
			this.groupBoxFilesAndFoldersSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.label4);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.label5);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.textBoxBrowserExePath);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.textBoxDownloadDirectory);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.btnRestoreDefaultOutputFileNameFormat);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.btnSelectDownloadDirectory);
			this.groupBoxFilesAndFoldersSettings.Controls.Add(this.textBoxOutputFileNameFormat);
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
			// textBoxBrowserExePath
			// 
			this.textBoxBrowserExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxBrowserExePath.Location = new System.Drawing.Point(7, 110);
			this.textBoxBrowserExePath.Name = "textBoxBrowserExePath";
			this.textBoxBrowserExePath.Size = new System.Drawing.Size(743, 20);
			this.textBoxBrowserExePath.TabIndex = 7;
			// 
			// textBoxDownloadDirectory
			// 
			this.textBoxDownloadDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxDownloadDirectory.Location = new System.Drawing.Point(7, 32);
			this.textBoxDownloadDirectory.Name = "textBoxDownloadDirectory";
			this.textBoxDownloadDirectory.Size = new System.Drawing.Size(743, 20);
			this.textBoxDownloadDirectory.TabIndex = 8;
			this.textBoxDownloadDirectory.Leave += new System.EventHandler(this.textBoxDownloadDirectory_Leave);
			// 
			// btnRestoreDefaultOutputFileNameFormat
			// 
			this.btnRestoreDefaultOutputFileNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRestoreDefaultOutputFileNameFormat.Location = new System.Drawing.Point(689, 71);
			this.btnRestoreDefaultOutputFileNameFormat.Name = "btnRestoreDefaultOutputFileNameFormat";
			this.btnRestoreDefaultOutputFileNameFormat.Size = new System.Drawing.Size(109, 23);
			this.btnRestoreDefaultOutputFileNameFormat.TabIndex = 15;
			this.btnRestoreDefaultOutputFileNameFormat.Text = "Вернуть как было";
			this.btnRestoreDefaultOutputFileNameFormat.UseVisualStyleBackColor = true;
			this.btnRestoreDefaultOutputFileNameFormat.Click += new System.EventHandler(this.btnRestoreDefaultOutputFileNameFormat_Click);
			// 
			// btnSelectDownloadDirectory
			// 
			this.btnSelectDownloadDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSelectDownloadDirectory.Location = new System.Drawing.Point(756, 32);
			this.btnSelectDownloadDirectory.Name = "btnSelectDownloadDirectory";
			this.btnSelectDownloadDirectory.Size = new System.Drawing.Size(42, 22);
			this.btnSelectDownloadDirectory.TabIndex = 9;
			this.btnSelectDownloadDirectory.Text = "...";
			this.btnSelectDownloadDirectory.UseVisualStyleBackColor = true;
			this.btnSelectDownloadDirectory.Click += new System.EventHandler(this.btnSelectDownloadDirectory_Click);
			// 
			// textBoxOutputFileNameFormat
			// 
			this.textBoxOutputFileNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxOutputFileNameFormat.Location = new System.Drawing.Point(9, 71);
			this.textBoxOutputFileNameFormat.Name = "textBoxOutputFileNameFormat";
			this.textBoxOutputFileNameFormat.Size = new System.Drawing.Size(674, 20);
			this.textBoxOutputFileNameFormat.TabIndex = 14;
			this.textBoxOutputFileNameFormat.Leave += new System.EventHandler(this.textBoxOutputFileNameFormat_Leave);
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
			this.tabPageSearch.Controls.Add(this.groupBoxSearchByUrls);
			this.tabPageSearch.Controls.Add(this.groupBoxSearchByChannelName);
			this.tabPageSearch.Location = new System.Drawing.Point(4, 22);
			this.tabPageSearch.Name = "tabPageSearch";
			this.tabPageSearch.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageSearch.Size = new System.Drawing.Size(818, 472);
			this.tabPageSearch.TabIndex = 5;
			this.tabPageSearch.Text = "Поиск видео";
			// 
			// groupBoxSearchByUrls
			// 
			this.groupBoxSearchByUrls.Controls.Add(this.btnSearchByUrls);
			this.groupBoxSearchByUrls.Controls.Add(this.textBoxVideoUrls);
			this.groupBoxSearchByUrls.Controls.Add(this.label6);
			this.groupBoxSearchByUrls.Location = new System.Drawing.Point(323, 16);
			this.groupBoxSearchByUrls.Name = "groupBoxSearchByUrls";
			this.groupBoxSearchByUrls.Size = new System.Drawing.Size(352, 162);
			this.groupBoxSearchByUrls.TabIndex = 1;
			this.groupBoxSearchByUrls.TabStop = false;
			this.groupBoxSearchByUrls.Text = "Поиск видео по ссылкам";
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
			// textBoxVideoUrls
			// 
			this.textBoxVideoUrls.Location = new System.Drawing.Point(18, 38);
			this.textBoxVideoUrls.Multiline = true;
			this.textBoxVideoUrls.Name = "textBoxVideoUrls";
			this.textBoxVideoUrls.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxVideoUrls.Size = new System.Drawing.Size(328, 89);
			this.textBoxVideoUrls.TabIndex = 1;
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
			// groupBoxSearchByChannelName
			// 
			this.groupBoxSearchByChannelName.Controls.Add(this.btnEditChannelList);
			this.groupBoxSearchByChannelName.Controls.Add(this.btnAddChannelToList);
			this.groupBoxSearchByChannelName.Controls.Add(this.textBoxChannelName);
			this.groupBoxSearchByChannelName.Controls.Add(this.listBoxChannelList);
			this.groupBoxSearchByChannelName.Controls.Add(this.groupBoxSearchResultCount);
			this.groupBoxSearchByChannelName.Controls.Add(this.label1);
			this.groupBoxSearchByChannelName.Controls.Add(this.btnSearchByChannelName);
			this.groupBoxSearchByChannelName.Location = new System.Drawing.Point(6, 16);
			this.groupBoxSearchByChannelName.Name = "groupBoxSearchByChannelName";
			this.groupBoxSearchByChannelName.Size = new System.Drawing.Size(311, 308);
			this.groupBoxSearchByChannelName.TabIndex = 0;
			this.groupBoxSearchByChannelName.TabStop = false;
			this.groupBoxSearchByChannelName.Text = "Поиск по названию канала";
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
			// groupBoxSearchResultCount
			// 
			this.groupBoxSearchResultCount.Controls.Add(this.label3);
			this.groupBoxSearchResultCount.Controls.Add(this.numericUpDownSearchLimit);
			this.groupBoxSearchResultCount.Controls.Add(this.radioButtonSearchLimited);
			this.groupBoxSearchResultCount.Controls.Add(this.radioButtonSearchAll);
			this.groupBoxSearchResultCount.Location = new System.Drawing.Point(6, 227);
			this.groupBoxSearchResultCount.Name = "groupBoxSearchResultCount";
			this.groupBoxSearchResultCount.Size = new System.Drawing.Size(299, 75);
			this.groupBoxSearchResultCount.TabIndex = 5;
			this.groupBoxSearchResultCount.TabStop = false;
			this.groupBoxSearchResultCount.Text = "Диапазон поиска";
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
			// radioButtonSearchLimited
			// 
			this.radioButtonSearchLimited.AutoSize = true;
			this.radioButtonSearchLimited.Checked = true;
			this.radioButtonSearchLimited.Location = new System.Drawing.Point(8, 45);
			this.radioButtonSearchLimited.Name = "radioButtonSearchLimited";
			this.radioButtonSearchLimited.Size = new System.Drawing.Size(62, 17);
			this.radioButtonSearchLimited.TabIndex = 1;
			this.radioButtonSearchLimited.TabStop = true;
			this.radioButtonSearchLimited.Text = "Только";
			this.radioButtonSearchLimited.UseVisualStyleBackColor = true;
			// 
			// radioButtonSearchAll
			// 
			this.radioButtonSearchAll.AutoSize = true;
			this.radioButtonSearchAll.Location = new System.Drawing.Point(8, 18);
			this.radioButtonSearchAll.Name = "radioButtonSearchAll";
			this.radioButtonSearchAll.Size = new System.Drawing.Size(196, 17);
			this.radioButtonSearchAll.TabIndex = 0;
			this.radioButtonSearchAll.Text = "Все видео канала (кроме клипов)";
			this.toolTip1.SetToolTip(this.radioButtonSearchAll, "Может быть очень долго!");
			this.radioButtonSearchAll.UseVisualStyleBackColor = true;
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
			// btnSearchByChannelName
			// 
			this.btnSearchByChannelName.Location = new System.Drawing.Point(230, 198);
			this.btnSearchByChannelName.Name = "btnSearchByChannelName";
			this.btnSearchByChannelName.Size = new System.Drawing.Size(75, 23);
			this.btnSearchByChannelName.TabIndex = 1;
			this.btnSearchByChannelName.Text = "Искать";
			this.btnSearchByChannelName.UseVisualStyleBackColor = true;
			this.btnSearchByChannelName.Click += new System.EventHandler(this.btnSearchByChannelName_Click);
			// 
			// tabPageEventLog
			// 
			this.tabPageEventLog.BackColor = System.Drawing.Color.Black;
			this.tabPageEventLog.Controls.Add(this.listBoxEventLog);
			this.tabPageEventLog.Location = new System.Drawing.Point(4, 22);
			this.tabPageEventLog.Name = "tabPageEventLog";
			this.tabPageEventLog.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageEventLog.Size = new System.Drawing.Size(818, 472);
			this.tabPageEventLog.TabIndex = 3;
			this.tabPageEventLog.Text = "Лог событий";
			// 
			// listBoxEventLog
			// 
			this.listBoxEventLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxEventLog.BackColor = System.Drawing.Color.Black;
			this.listBoxEventLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listBoxEventLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.listBoxEventLog.ForeColor = System.Drawing.Color.Lime;
			this.listBoxEventLog.FormattingEnabled = true;
			this.listBoxEventLog.ItemHeight = 24;
			this.listBoxEventLog.Location = new System.Drawing.Point(0, 0);
			this.listBoxEventLog.Name = "listBoxEventLog";
			this.listBoxEventLog.Size = new System.Drawing.Size(812, 456);
			this.listBoxEventLog.TabIndex = 0;
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
			// tabPageDownloads
			// 
			this.tabPageDownloads.BackColor = System.Drawing.Color.DimGray;
			this.tabPageDownloads.Controls.Add(this.scrollBarDownloads);
			this.tabPageDownloads.Controls.Add(this.panelDownloads);
			this.tabPageDownloads.Location = new System.Drawing.Point(4, 22);
			this.tabPageDownloads.Name = "tabPageDownloads";
			this.tabPageDownloads.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageDownloads.Size = new System.Drawing.Size(818, 472);
			this.tabPageDownloads.TabIndex = 6;
			this.tabPageDownloads.Text = "Скачивание";
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
			// contextMenuVodThumbnailImage
			// 
			this.contextMenuVodThumbnailImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.miCopyVideoUrl,
			this.miCopyVodThumbnailImageUrlToolStripMenuItem,
			this.miCopyVodInfoToolStripMenuItem,
			this.miSaveVodPlaylistAsToolStripMenuItem,
			this.miSaveVodThumbnailImageAssToolStripMenuItem,
			this.toolStripMenuItem1,
			this.miOpenVideoInBrowserToolStripMenuItem});
			this.contextMenuVodThumbnailImage.Name = "contextMenuStreamImage";
			this.contextMenuVodThumbnailImage.Size = new System.Drawing.Size(283, 142);
			// 
			// miCopyVideoUrl
			// 
			this.miCopyVideoUrl.Name = "miCopyVideoUrl";
			this.miCopyVideoUrl.Size = new System.Drawing.Size(282, 22);
			this.miCopyVideoUrl.Text = "Скопировать ссылку на видео";
			this.miCopyVideoUrl.Click += new System.EventHandler(this.miCopyVideoUrl_Click);
			// 
			// miCopyVodThumbnailImageUrlToolStripMenuItem
			// 
			this.miCopyVodThumbnailImageUrlToolStripMenuItem.Name = "miCopyVodThumbnailImageUrlToolStripMenuItem";
			this.miCopyVodThumbnailImageUrlToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
			this.miCopyVodThumbnailImageUrlToolStripMenuItem.Text = "Скопировать ссылку на изображение";
			this.miCopyVodThumbnailImageUrlToolStripMenuItem.Click += new System.EventHandler(this.miCopyVodThumbnailImageUrlToolStripMenuItem_Click);
			// 
			// miCopyVodInfoToolStripMenuItem
			// 
			this.miCopyVodInfoToolStripMenuItem.Name = "miCopyVodInfoToolStripMenuItem";
			this.miCopyVodInfoToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
			this.miCopyVodInfoToolStripMenuItem.Text = "Скопировать информацию о стриме";
			this.miCopyVodInfoToolStripMenuItem.Click += new System.EventHandler(this.miCopyVodInfoToolStripMenuItem_Click);
			// 
			// miSaveVodPlaylistAsToolStripMenuItem
			// 
			this.miSaveVodPlaylistAsToolStripMenuItem.Name = "miSaveVodPlaylistAsToolStripMenuItem";
			this.miSaveVodPlaylistAsToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
			this.miSaveVodPlaylistAsToolStripMenuItem.Text = "Сохранить плейлист как...";
			this.miSaveVodPlaylistAsToolStripMenuItem.Click += new System.EventHandler(this.miSaveVodPlaylistAsToolStripMenuItem_Click);
			// 
			// miSaveVodThumbnailImageAssToolStripMenuItem
			// 
			this.miSaveVodThumbnailImageAssToolStripMenuItem.Name = "miSaveVodThumbnailImageAssToolStripMenuItem";
			this.miSaveVodThumbnailImageAssToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
			this.miSaveVodThumbnailImageAssToolStripMenuItem.Text = "Сохранить изображение как...";
			this.miSaveVodThumbnailImageAssToolStripMenuItem.Click += new System.EventHandler(this.miSaveVodThumbnailImageAssToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(279, 6);
			// 
			// miOpenVideoInBrowserToolStripMenuItem
			// 
			this.miOpenVideoInBrowserToolStripMenuItem.Name = "miOpenVideoInBrowserToolStripMenuItem";
			this.miOpenVideoInBrowserToolStripMenuItem.Size = new System.Drawing.Size(282, 22);
			this.miOpenVideoInBrowserToolStripMenuItem.Text = "Открыть видео в браузере";
			this.miOpenVideoInBrowserToolStripMenuItem.Click += new System.EventHandler(this.miOpenVideoInBrowserToolStripMenuItem_Click);
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
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.form1_FormClosed);
			this.Load += new System.EventHandler(this.form1_Load);
			this.Resize += new System.EventHandler(this.form1_Resize);
			this.tabControlMain.ResumeLayout(false);
			this.tabPageDebug.ResumeLayout(false);
			this.tabPageSettings.ResumeLayout(false);
			this.groupBoxApiSettings.ResumeLayout(false);
			this.groupBoxTwitchApplicationSettings.ResumeLayout(false);
			this.groupBoxTwitchApplicationSettings.PerformLayout();
			this.groupBoxHelixApiToken.ResumeLayout(false);
			this.groupBoxHelixApiToken.PerformLayout();
			this.groupBoxVideoInformationSettings.ResumeLayout(false);
			this.groupBoxVideoInformationSettings.PerformLayout();
			this.groupBoxFilesAndFoldersSettings.ResumeLayout(false);
			this.groupBoxFilesAndFoldersSettings.PerformLayout();
			this.tabPageSearch.ResumeLayout(false);
			this.groupBoxSearchByUrls.ResumeLayout(false);
			this.groupBoxSearchByUrls.PerformLayout();
			this.groupBoxSearchByChannelName.ResumeLayout(false);
			this.groupBoxSearchByChannelName.PerformLayout();
			this.groupBoxSearchResultCount.ResumeLayout(false);
			this.groupBoxSearchResultCount.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownSearchLimit)).EndInit();
			this.tabPageEventLog.ResumeLayout(false);
			this.tabPageStreams.ResumeLayout(false);
			this.tabPageDownloads.ResumeLayout(false);
			this.contextMenuVodThumbnailImage.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControlMain;
		private System.Windows.Forms.TabPage tabPageSettings;
		private System.Windows.Forms.Button btnSelectBrowser;
		private System.Windows.Forms.Button btnSelectDownloadDirectory;
		private System.Windows.Forms.TextBox textBoxDownloadDirectory;
		private System.Windows.Forms.TextBox textBoxBrowserExePath;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ContextMenuStrip contextMenuVodThumbnailImage;
		private System.Windows.Forms.ToolStripMenuItem miCopyVodThumbnailImageUrlToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem miCopyVodInfoToolStripMenuItem;
		private System.Windows.Forms.TabPage tabPageDebug;
		private System.Windows.Forms.RichTextBox richTextBoxDebugLog;
		private System.Windows.Forms.TabPage tabPageEventLog;
		private System.Windows.Forms.ListBox listBoxEventLog;
		private System.Windows.Forms.TabPage tabPageStreams;
		private System.Windows.Forms.TabPage tabPageSearch;
		private System.Windows.Forms.GroupBox groupBoxSearchByChannelName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSearchByChannelName;
		private System.Windows.Forms.TabPage tabPageDownloads;
		private System.Windows.Forms.Panel panelDownloads;
		private System.Windows.Forms.VScrollBar scrollBarDownloads;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupBoxSearchResultCount;
		private System.Windows.Forms.NumericUpDown numericUpDownSearchLimit;
		private System.Windows.Forms.RadioButton radioButtonSearchLimited;
		private System.Windows.Forms.RadioButton radioButtonSearchAll;
		private System.Windows.Forms.ToolStripMenuItem miSaveVodThumbnailImageAssToolStripMenuItem;
		private System.Windows.Forms.VScrollBar scrollBarStreams;
		private System.Windows.Forms.Panel panelStreams;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ToolStripMenuItem miOpenVideoInBrowserToolStripMenuItem;
		private System.Windows.Forms.TextBox textBoxOutputFileNameFormat;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem miCopyVideoUrl;
		private System.Windows.Forms.Button btnRestoreDefaultOutputFileNameFormat;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.GroupBox groupBoxSearchByUrls;
		private System.Windows.Forms.Button btnSearchByUrls;
		private System.Windows.Forms.TextBox textBoxVideoUrls;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkBoxUseGmtTime;
		private System.Windows.Forms.CheckBox checkBoxSaveVodInfo;
		private System.Windows.Forms.CheckBox checkBoxSaveVodChunkInfo;
		private System.Windows.Forms.ToolStripMenuItem miSaveVodPlaylistAsToolStripMenuItem;
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
		private System.Windows.Forms.GroupBox groupBoxTwitchApplicationSettings;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textBoxHelixApiClientSecretKey;
		private System.Windows.Forms.TextBox textBoxHelixApiClientId;
		private System.Windows.Forms.TextBox textBoxApiApplicationDescription;
		private System.Windows.Forms.TextBox textBoxApiApplicationTitle;
		private System.Windows.Forms.Button btnRestoreDefaultApiApplication;
		private System.Windows.Forms.Button btnApplyApiApplication;
		private System.Windows.Forms.Button btnResetHelixApiToken;
	}
}
