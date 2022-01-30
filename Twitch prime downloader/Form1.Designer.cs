
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
            this.btnRestoreDefaultFilenameFormat = new System.Windows.Forms.Button();
            this.textBox_FileNameFormat = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectBrowser = new System.Windows.Forms.Button();
            this.btnSelectDownloadingPath = new System.Windows.Forms.Button();
            this.textBox_DownloadingPath = new System.Windows.Forms.TextBox();
            this.textBox_Browser = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPageSearch = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSearchByUrls = new System.Windows.Forms.Button();
            this.textBoxUrls = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownSearchLimit = new System.Windows.Forms.NumericUpDown();
            this.rbSearchLimit = new System.Windows.Forms.RadioButton();
            this.rbSearchAll = new System.Windows.Forms.RadioButton();
            this.btnAddChannel = new System.Windows.Forms.Button();
            this.btnRemoveChannel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearchChannelName = new System.Windows.Forms.Button();
            this.cboxChannelName = new System.Windows.Forms.ComboBox();
            this.tabPageLog = new System.Windows.Forms.TabPage();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.tabPageStreams = new System.Windows.Forms.TabPage();
            this.panelStreams = new System.Windows.Forms.Panel();
            this.scrollBarStreams = new System.Windows.Forms.VScrollBar();
            this.tabPageDownload = new System.Windows.Forms.TabPage();
            this.scrollBarDownloads = new System.Windows.Forms.VScrollBar();
            this.panelDownloads = new System.Windows.Forms.Panel();
            this.contextMenuStreamImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miCopyVideoUrl = new System.Windows.Forms.ToolStripMenuItem();
            this.copyImageUrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyStreamInfoJsonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageAssToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openVideoInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControlMain.SuspendLayout();
            this.tabPageDebug.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.tabPageSearch.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSearchLimit)).BeginInit();
            this.tabPageLog.SuspendLayout();
            this.tabPageStreams.SuspendLayout();
            this.tabPageDownload.SuspendLayout();
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
            this.tabControlMain.Controls.Add(this.tabPageDownload);
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(826, 508);
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
            this.tabPageDebug.Size = new System.Drawing.Size(818, 482);
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
            this.memoDebug.Size = new System.Drawing.Size(806, 476);
            this.memoDebug.TabIndex = 0;
            this.memoDebug.Text = "";
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageSettings.Controls.Add(this.btnRestoreDefaultFilenameFormat);
            this.tabPageSettings.Controls.Add(this.textBox_FileNameFormat);
            this.tabPageSettings.Controls.Add(this.label2);
            this.tabPageSettings.Controls.Add(this.btnSelectBrowser);
            this.tabPageSettings.Controls.Add(this.btnSelectDownloadingPath);
            this.tabPageSettings.Controls.Add(this.textBox_DownloadingPath);
            this.tabPageSettings.Controls.Add(this.textBox_Browser);
            this.tabPageSettings.Controls.Add(this.label5);
            this.tabPageSettings.Controls.Add(this.label4);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(818, 482);
            this.tabPageSettings.TabIndex = 1;
            this.tabPageSettings.Text = "Настройки";
            // 
            // btnRestoreDefaultFilenameFormat
            // 
            this.btnRestoreDefaultFilenameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestoreDefaultFilenameFormat.Location = new System.Drawing.Point(702, 63);
            this.btnRestoreDefaultFilenameFormat.Name = "btnRestoreDefaultFilenameFormat";
            this.btnRestoreDefaultFilenameFormat.Size = new System.Drawing.Size(109, 23);
            this.btnRestoreDefaultFilenameFormat.TabIndex = 15;
            this.btnRestoreDefaultFilenameFormat.Text = "Вернуть как было";
            this.btnRestoreDefaultFilenameFormat.UseVisualStyleBackColor = true;
            this.btnRestoreDefaultFilenameFormat.Click += new System.EventHandler(this.btnRestoreDefaultFilenameFormat_Click);
            // 
            // textBox_FileNameFormat
            // 
            this.textBox_FileNameFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_FileNameFormat.Location = new System.Drawing.Point(10, 63);
            this.textBox_FileNameFormat.Name = "textBox_FileNameFormat";
            this.textBox_FileNameFormat.Size = new System.Drawing.Size(686, 20);
            this.textBox_FileNameFormat.TabIndex = 14;
            this.textBox_FileNameFormat.Leave += new System.EventHandler(this.textBox_FileNameFormat_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Формат имени файла:";
            // 
            // btnSelectBrowser
            // 
            this.btnSelectBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectBrowser.Location = new System.Drawing.Point(769, 102);
            this.btnSelectBrowser.Name = "btnSelectBrowser";
            this.btnSelectBrowser.Size = new System.Drawing.Size(42, 20);
            this.btnSelectBrowser.TabIndex = 10;
            this.btnSelectBrowser.Text = "...";
            this.btnSelectBrowser.UseVisualStyleBackColor = true;
            this.btnSelectBrowser.Click += new System.EventHandler(this.btnSelectBrowser_Click);
            // 
            // btnSelectDownloadingPath
            // 
            this.btnSelectDownloadingPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectDownloadingPath.Location = new System.Drawing.Point(769, 24);
            this.btnSelectDownloadingPath.Name = "btnSelectDownloadingPath";
            this.btnSelectDownloadingPath.Size = new System.Drawing.Size(42, 22);
            this.btnSelectDownloadingPath.TabIndex = 9;
            this.btnSelectDownloadingPath.Text = "...";
            this.btnSelectDownloadingPath.UseVisualStyleBackColor = true;
            this.btnSelectDownloadingPath.Click += new System.EventHandler(this.btnSelectDownloadingPath_Click);
            // 
            // textBox_DownloadingPath
            // 
            this.textBox_DownloadingPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_DownloadingPath.Location = new System.Drawing.Point(8, 24);
            this.textBox_DownloadingPath.Name = "textBox_DownloadingPath";
            this.textBox_DownloadingPath.Size = new System.Drawing.Size(755, 20);
            this.textBox_DownloadingPath.TabIndex = 8;
            this.textBox_DownloadingPath.Leave += new System.EventHandler(this.TextBox_DownloadingPath_Leave);
            // 
            // textBox_Browser
            // 
            this.textBox_Browser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Browser.Location = new System.Drawing.Point(8, 102);
            this.textBox_Browser.Name = "textBox_Browser";
            this.textBox_Browser.Size = new System.Drawing.Size(755, 20);
            this.textBox_Browser.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 86);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Веб-браузер:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Папка для скачивания:";
            // 
            // tabPageSearch
            // 
            this.tabPageSearch.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageSearch.Controls.Add(this.groupBox2);
            this.tabPageSearch.Controls.Add(this.groupBox1);
            this.tabPageSearch.Location = new System.Drawing.Point(4, 22);
            this.tabPageSearch.Name = "tabPageSearch";
            this.tabPageSearch.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSearch.Size = new System.Drawing.Size(818, 482);
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
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.btnAddChannel);
            this.groupBox1.Controls.Add(this.btnRemoveChannel);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnSearchChannelName);
            this.groupBox1.Controls.Add(this.cboxChannelName);
            this.groupBox1.Location = new System.Drawing.Point(6, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(311, 162);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Поиск по названию канала";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.numericUpDownSearchLimit);
            this.groupBox3.Controls.Add(this.rbSearchLimit);
            this.groupBox3.Controls.Add(this.rbSearchAll);
            this.groupBox3.Location = new System.Drawing.Point(12, 44);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(291, 83);
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
            this.rbSearchLimit.Location = new System.Drawing.Point(8, 45);
            this.rbSearchLimit.Name = "rbSearchLimit";
            this.rbSearchLimit.Size = new System.Drawing.Size(62, 17);
            this.rbSearchLimit.TabIndex = 1;
            this.rbSearchLimit.Text = "Только";
            this.rbSearchLimit.UseVisualStyleBackColor = true;
            // 
            // rbSearchAll
            // 
            this.rbSearchAll.AutoSize = true;
            this.rbSearchAll.Checked = true;
            this.rbSearchAll.Location = new System.Drawing.Point(8, 18);
            this.rbSearchAll.Name = "rbSearchAll";
            this.rbSearchAll.Size = new System.Drawing.Size(77, 17);
            this.rbSearchAll.TabIndex = 0;
            this.rbSearchAll.TabStop = true;
            this.rbSearchAll.Text = "Все видео";
            this.rbSearchAll.UseVisualStyleBackColor = true;
            // 
            // btnAddChannel
            // 
            this.btnAddChannel.Location = new System.Drawing.Point(236, 19);
            this.btnAddChannel.Name = "btnAddChannel";
            this.btnAddChannel.Size = new System.Drawing.Size(30, 21);
            this.btnAddChannel.TabIndex = 4;
            this.btnAddChannel.Text = "+";
            this.toolTip1.SetToolTip(this.btnAddChannel, "Добавить канал");
            this.btnAddChannel.UseVisualStyleBackColor = true;
            this.btnAddChannel.Click += new System.EventHandler(this.btnAddChannel_Click);
            // 
            // btnRemoveChannel
            // 
            this.btnRemoveChannel.Location = new System.Drawing.Point(273, 19);
            this.btnRemoveChannel.Name = "btnRemoveChannel";
            this.btnRemoveChannel.Size = new System.Drawing.Size(30, 21);
            this.btnRemoveChannel.TabIndex = 3;
            this.btnRemoveChannel.Text = "-";
            this.toolTip1.SetToolTip(this.btnRemoveChannel, "Удалить канал");
            this.btnRemoveChannel.UseVisualStyleBackColor = true;
            this.btnRemoveChannel.Click += new System.EventHandler(this.btnRemoveChannel_Click);
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
            this.btnSearchChannelName.Location = new System.Drawing.Point(228, 133);
            this.btnSearchChannelName.Name = "btnSearchChannelName";
            this.btnSearchChannelName.Size = new System.Drawing.Size(75, 23);
            this.btnSearchChannelName.TabIndex = 1;
            this.btnSearchChannelName.Text = "Искать";
            this.btnSearchChannelName.UseVisualStyleBackColor = true;
            this.btnSearchChannelName.Click += new System.EventHandler(this.btnSearchChannelName_Click);
            // 
            // cboxChannelName
            // 
            this.cboxChannelName.FormattingEnabled = true;
            this.cboxChannelName.Location = new System.Drawing.Point(111, 19);
            this.cboxChannelName.Name = "cboxChannelName";
            this.cboxChannelName.Size = new System.Drawing.Size(122, 21);
            this.cboxChannelName.TabIndex = 0;
            // 
            // tabPageLog
            // 
            this.tabPageLog.BackColor = System.Drawing.Color.Black;
            this.tabPageLog.Controls.Add(this.lbLog);
            this.tabPageLog.Location = new System.Drawing.Point(4, 22);
            this.tabPageLog.Name = "tabPageLog";
            this.tabPageLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLog.Size = new System.Drawing.Size(818, 482);
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
            this.lbLog.Size = new System.Drawing.Size(812, 456);
            this.lbLog.TabIndex = 0;
            // 
            // tabPageStreams
            // 
            this.tabPageStreams.Controls.Add(this.panelStreams);
            this.tabPageStreams.Controls.Add(this.scrollBarStreams);
            this.tabPageStreams.Location = new System.Drawing.Point(4, 22);
            this.tabPageStreams.Name = "tabPageStreams";
            this.tabPageStreams.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStreams.Size = new System.Drawing.Size(818, 482);
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
            this.panelStreams.Size = new System.Drawing.Size(795, 482);
            this.panelStreams.TabIndex = 2;
            // 
            // scrollBarStreams
            // 
            this.scrollBarStreams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollBarStreams.Location = new System.Drawing.Point(798, 0);
            this.scrollBarStreams.Name = "scrollBarStreams";
            this.scrollBarStreams.Size = new System.Drawing.Size(17, 482);
            this.scrollBarStreams.TabIndex = 1;
            this.scrollBarStreams.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollBarStreams_Scroll);
            // 
            // tabPageDownload
            // 
            this.tabPageDownload.BackColor = System.Drawing.Color.DimGray;
            this.tabPageDownload.Controls.Add(this.scrollBarDownloads);
            this.tabPageDownload.Controls.Add(this.panelDownloads);
            this.tabPageDownload.Location = new System.Drawing.Point(4, 22);
            this.tabPageDownload.Name = "tabPageDownload";
            this.tabPageDownload.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDownload.Size = new System.Drawing.Size(818, 482);
            this.tabPageDownload.TabIndex = 6;
            this.tabPageDownload.Text = "Скачивание";
            // 
            // scrollBarDownloads
            // 
            this.scrollBarDownloads.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollBarDownloads.Enabled = false;
            this.scrollBarDownloads.Location = new System.Drawing.Point(801, 0);
            this.scrollBarDownloads.Name = "scrollBarDownloads";
            this.scrollBarDownloads.Size = new System.Drawing.Size(17, 482);
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
            this.panelDownloads.Size = new System.Drawing.Size(801, 482);
            this.panelDownloads.TabIndex = 0;
            // 
            // contextMenuStreamImage
            // 
            this.contextMenuStreamImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCopyVideoUrl,
            this.copyImageUrlToolStripMenuItem,
            this.copyStreamInfoJsonToolStripMenuItem,
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
            this.ClientSize = new System.Drawing.Size(828, 511);
            this.Controls.Add(this.tabControlMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(844, 39);
            this.Name = "Form1";
            this.Text = "Twitch prime downloader";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.tabControlMain.ResumeLayout(false);
            this.tabPageDebug.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.tabPageSettings.PerformLayout();
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
            this.tabPageDownload.ResumeLayout(false);
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
        private System.Windows.Forms.ComboBox cboxChannelName;
        private System.Windows.Forms.TabPage tabPageDownload;
        private System.Windows.Forms.Panel panelDownloads;
        private System.Windows.Forms.Button btnAddChannel;
        private System.Windows.Forms.Button btnRemoveChannel;
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
    }
}
