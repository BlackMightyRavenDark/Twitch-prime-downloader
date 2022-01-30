using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
    public partial class Form1 : Form
    {
        private SynchronizationContext synchronizationContext = null;
        private FrameStream activeFrameStream = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            synchronizationContext = SynchronizationContext.Current;

            ServicePointManager.DefaultConnectionLimit = 1000;

            config.Load();

            if (File.Exists(config.channelsListFileName))
            {
                cboxChannelName.LoadFromFile(config.channelsListFileName);

                if (cboxChannelName.Items.Count > 0)
                {
                    cboxChannelName.SelectedIndex = 0;
                }
            }

            if (File.Exists(config.urlsFileName))
            {
                string[] strings = File.ReadAllLines(config.urlsFileName);
                textBoxUrls.Lines = strings;
            }

            foreach (string s in Environment.GetCommandLineArgs())
            {
                if (s.ToLower().Equals("/debug"))
                {
                    config.debugMode = true;
                    break;
                }
            }

            if (!config.debugMode)
            {
                tabControlMain.TabPages.Remove(tabPageDebug);
            }
            textBox_DownloadingPath.Text = config.downloadingPath;
            textBox_FileNameFormat.Text = config.fileNameFormat;
            textBox_Browser.Text = config.browserExe;

            tabControlMain.SelectedTab = tabPageSearch;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClearFramesStream();
            foreach (FrameDownload frd in framesDownload)
            {
                frd.FrameDispose();
                frd.Dispose();
            }

            cboxChannelName.SaveToFile(config.channelsListFileName);

            if (File.Exists(config.urlsFileName))
            {
                File.Delete(config.urlsFileName);
            }
            string[] urls = textBoxUrls.Lines;
            if (urls.Length > 0)
            {
                urls.SaveToFile(config.urlsFileName);
            }
            config.Save();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageStreams)
            {
                StackFramesStream();
            }
            else if (tabControlMain.SelectedTab == tabPageDownload)
            {
                StackFramesDownload();
            }
        }

        private void ClearFramesStream()
        {
            foreach (FrameStream frameStream in framesStream)
            {
                frameStream.Dispose();
            }
            framesStream.Clear();
        }

        private int StackFramesStream()
        {
            if (framesStream.Count > 0)
            {
                int w = framesStream[0].Width;
                int h = framesStream[0].Height;
                int gap = 4;
                int panelWidth = tabControlMain.Width - scrollBarStreams.Width - 10;
                int perRow = panelWidth / (w + gap);
                if (perRow == 0)
                {
                    perRow = 1;
                }
                int rowsCount = framesStream.Count / perRow;
                if (framesStream.Count % perRow != 0)
                {
                    rowsCount++;
                }
                int xStart = (panelWidth / 2) - ((w + gap) * perRow / 2);
                int x = xStart;
                int y = -h - gap;
                for (int i = 0; i < framesStream.Count; i++)
                {
                    if (i % perRow == 0)
                    {
                        y += h + gap;
                        x = xStart;
                    }
                    framesStream[i].Location = new Point(x, y - scrollBarStreams.Value);
                    x += w + gap;
                }

                int j = (h + gap) * rowsCount;
                if (j > panelStreams.Height)
                {
                    scrollBarStreams.Maximum = j;
                    scrollBarStreams.LargeChange = panelStreams.Height;
                    scrollBarStreams.SmallChange = 10;
                    scrollBarStreams.Enabled = true;
                }
                else
                {
                    scrollBarStreams.Enabled = false;
                }
                return rowsCount;
            }
            else
            {
                scrollBarStreams.Enabled = false;
            }
            return 0;
        }

        private void StackFramesDownload()
        {
            if (framesDownload.Count > 0)
            {
                for (int i = 0; i < framesDownload.Count; i++)
                {
                    int locY = i * framesDownload[i].Height - scrollBarDownloads.Value;
                    framesDownload[i].Location = new Point(0, locY);
                    framesDownload[i].Width = Width - 40 + FrameDownload.EXTRA_WIDTH;
                }

                int h = framesDownload.Count * framesDownload[0].Height;
                if (h > panelDownloads.Height)
                {
                    scrollBarDownloads.Maximum = h;
                    scrollBarDownloads.LargeChange = panelDownloads.Height;
                    scrollBarDownloads.SmallChange = 10;
                    scrollBarDownloads.Enabled = true;

                    return;
                }
            }
            scrollBarDownloads.Enabled = false;
        }

        private void OnFrameDownload_Closed(object sender)
        {
            int i;
            for (i = 0; i < framesDownload.Count; i++)
            {
                if (framesDownload[i] == sender)
                {
                    break;
                }
            }

            framesDownload.RemoveAt(i);
            if (framesDownload.Count > 0)
            {
                tabPageDownload.Text = $"Скачивание ({framesDownload.Count})";
                StackFramesDownload();
            }
            else
            {
                tabPageDownload.Text = "Скачивание";
            }
        }

        private void OnFrameStream_Activated(object sender)
        {
            activeFrameStream = sender as FrameStream;
            for (int i = 0; i < framesStream.Count; i++)
            {
                framesStream[i].BackColor =
                    activeFrameStream == framesStream[i] ? FrameStream.colorActive : FrameStream.colorInactive;
            }
        }

        private void OnThreadGetVodPlaylist_Completed(object sender, int errorCode)
        {
            ThreadGetVodPlaylist thrObj = sender as ThreadGetVodPlaylist;
            if (errorCode == 200)
            {
                if (config.debugMode)
                {
                    memoDebug.Text = thrObj.PlaylistString;
                }

                string streamRoot = ExtractUrlFilePath(thrObj.PlaylistUrl);
                FrameDownload frd = new FrameDownload(streamRoot);
                frd.Parent = panelDownloads;
                frd.Location = new Point(0, 0);
                frd.Closed += OnFrameDownload_Closed;
                frd.SetStreamInfo(thrObj.StreamInfo);

                frd.SetChunks(thrObj.Chunks);

                frd.ChunkFrom = 0;
                frd.ChunkTo = frd.Chunks.Length - 1;

                framesDownload.Add(frd);

                tabPageDownload.Text = $"Скачивание ({framesDownload.Count})";
                if (tabControlMain.SelectedTab == tabPageDownload)
                {
                    StackFramesDownload();
                }
            }
            else
            {
                MessageBox.Show($"Error {errorCode}", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
         
            foreach (object ctl in thrObj.controls)
            {
                if (ctl is ToolStripMenuItem)
                {
                    (ctl as ToolStripMenuItem).Enabled = true;
                }
                else
                {
                    (ctl as Control).Enabled = true;
                }
            }
        }

        private void OnStreamImageMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStreamImage.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private int ParseVideosListJSON(string aJsonString)
        {
            JObject json = JObject.Parse(aJsonString);
            JToken jToken = json.Value<JToken>("videos");
            if (jToken == null)
            {
                TwitchVod stream = ParseVodInfo_Kraken(jToken.Value<string>());
            }
            else
            {
                JArray jsonArr = jToken.Value<JArray>();
                for (int i = 0; i < jsonArr.Count; i++)
                {
                    lbLog.Items.RemoveAt(lbLog.Items.Count - 1);
                    lbLog.Items.Add($"Обработка данных... {i + 1} / {jsonArr.Count}");

                    TwitchVod streamInfo = ParseVodInfo_Kraken(jsonArr[i].ToString());

                    FrameStream frameStream = new FrameStream();
                    frameStream.Parent = panelStreams;
                    frameStream.Location = new Point(0, 0);
                    frameStream.Activated += OnFrameStream_Activated;
                    frameStream.ImageMouseDown += OnStreamImageMouseDown;
                    frameStream.DownloadButtonPressed += OnDownloadButtonClick;
                    frameStream.BackColor = FrameStream.colorInactive;
                    frameStream.SetStreamInfo(streamInfo);
                    framesStream.Add(frameStream);

                    Application.DoEvents();
                }
                return jsonArr.Count;
            }
            return 0;
        }

        private int GetChannelVideosListJson_Kraken(string channelName, int maxVids, out string resJson)
        {
            resJson = null;
            if (string.IsNullOrEmpty(channelName) || string.IsNullOrWhiteSpace(channelName) || channelName.Contains(" "))
            {
                return 0;
            }
            int sum = 0;
            TwitchApi twitchApi = new TwitchApi();
            if (twitchApi.GetUserInfo_Helix(channelName, out TwitchUserInfo userInfo, out _) == 200)
            {
                int total = 0;
                int offset = 0;
                int max = 0;
                int errorCode;
                JArray jsonArr = new JArray();
                do
                {
                    string url = twitchApi.GetChannelVideosUrl_Kraken(userInfo.Id, 100, offset);
                    errorCode = twitchApi.HttpsGet_Kraken(url, out string buf);
                    if (errorCode == 200)
                    {
                        JObject json = JObject.Parse(buf);
                        if (total == 0)
                        {
                            total = json.Value<int>("_total");
                            if (maxVids > 0)
                            {
                                max = maxVids >= total ? total : maxVids;
                            }
                            else
                            {
                                max = total;
                            }
                        }
                        if (total > 0)
                        {
                            JArray jsonArr2 = json.Value<JArray>("videos");
                            foreach (JObject jObject in jsonArr2)
                            {
                                lbLog.Items.RemoveAt(lbLog.Items.Count - 1);
                                lbLog.Items.Add($"Скачивание списка стримов канала {channelName}... {sum + 1} / {max}");
                                string id = jObject.Value<string>("_id");
                                if (id.ToLower().StartsWith("v"))
                                {
                                    id = id.Remove(0, 1);
                                }

                                int err = twitchApi.GetVodInfo_Kraken(id, out string vodInfo);
                                if (err == 200 && !string.IsNullOrEmpty(vodInfo) && !string.IsNullOrWhiteSpace(vodInfo))
                                {
                                    JObject j = JObject.Parse(vodInfo);
                                    if (j != null)
                                    {
                                        jsonArr.Add(j);
                                        sum++;
                                        if (sum >= max)
                                        {
                                            break;
                                        }
                                    }
                                }
                                Application.DoEvents();
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    Application.DoEvents();
                }
                while (sum < max && errorCode == 200);

                JObject jsonToSave = new JObject();
                jsonToSave.Add("_total", jsonArr.Count.ToString());
                jsonToSave.Add(new JProperty("videos", jsonArr));
                resJson = jsonToSave.ToString();
            }
            return sum;
        }

        private void DownloadImages()
        {
            if (framesStream.Count > 0)
            {
                for (int i = 0; i < framesStream.Count; i++)
                {
                    lbLog.Items.RemoveAt(lbLog.Items.Count - 1);
                    lbLog.Items.Add($"Скачивание изображений... {i + 1} / {framesStream.Count}");

                    TwitchVod vod = framesStream[i].StreamInfo;

                    string imgUrl = vod.ImagePreviewTemplateUrl.Replace("{width}", "1920").Replace("{height}", "1080");
                    FileDownloader downloader = new FileDownloader();
                    downloader.Url = imgUrl;
                    
                    if (downloader.Download(vod.ImageData) == 200)
                    {
                        framesStream[i].imageStream.Image = Image.FromStream(vod.ImageData);
                    }
                    else
                    {
                        Bitmap bmp = GenerateErrorImage();
                        bmp.Save(vod.ImageData, ImageFormat.Bmp);
                        framesStream[i].imageStream.Image = bmp;
                    }

                    downloader.Url = vod.GameInfo.ImagePreviewSmallUrl;
                    if (downloader.Download(vod.GameInfo.ImageData) == 200)
                    {
                        framesStream[i].imageGame.Image = Image.FromStream(vod.GameInfo.ImageData);
                    }

                    Application.DoEvents();
                }
            }
        }

        private void CopyImageUrlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TwitchVod vod = activeFrameStream.StreamInfo;
            if (!string.IsNullOrEmpty(vod.ImagePreviewTemplateUrl) && !string.IsNullOrWhiteSpace(vod.ImagePreviewTemplateUrl))
            {
                SetClipboardText(vod.ImagePreviewTemplateUrl);
            }
        }

        private void TextBox_DownloadingPath_Leave(object sender, EventArgs e)
        {
            config.downloadingPath = textBox_DownloadingPath.Text;
        }

        private void CopyStreamInfoJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TwitchVod vod = activeFrameStream.StreamInfo;
            if (!string.IsNullOrEmpty(vod.InfoStringJson) && !string.IsNullOrWhiteSpace(vod.InfoStringJson))
            {
                SetClipboardText(vod.InfoStringJson);
            }
            else
            {
                MessageBox.Show("Информация о стриме пуста", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAddChannel_Click(object sender, EventArgs e)
        {
            string channelName = cboxChannelName.Text;
            if (string.IsNullOrEmpty(channelName) || string.IsNullOrWhiteSpace(channelName))
            {
                MessageBox.Show("Не введено название канала!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (channelName.Contains(" "))
            {
                MessageBox.Show("Название канала не может содержать пробелов!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int n = cboxChannelName.Items.IndexOf(channelName);
            if (n >= 0)
            {
                MessageBox.Show("Этот канал уже есть в списке!", "Добавлятор каналов в список",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            cboxChannelName.Items.Add(channelName);
            cboxChannelName.SelectedIndex = cboxChannelName.Items.Count - 1;
        }

        private void btnRemoveChannel_Click(object sender, EventArgs e)
        {
            int n = cboxChannelName.SelectedIndex;
            if (n == -1)
            {
                MessageBox.Show("Список и так пуст!", "Удалятор каналов из списка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            cboxChannelName.Items.RemoveAt(n);
            cboxChannelName.SelectedIndex = n > cboxChannelName.Items.Count - 1 ? n - 1 : cboxChannelName.Items.Count - 1;
            if (cboxChannelName.Items.Count == 0)
            {
                cboxChannelName.Text = null;
            }
        }
        
        private void btnSearchChannelName_Click(object sender, EventArgs e)
        {
            btnSearchChannelName.Enabled = false;
            string channelName = cboxChannelName.Text;
            if (string.IsNullOrEmpty(channelName) || string.IsNullOrWhiteSpace(channelName))
            {
                MessageBox.Show("Не введено название канала!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSearchChannelName.Enabled = true;
                return;
            }
            if (channelName.Contains(" "))
            {
                MessageBox.Show("Название канала не может содержать пробелов!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSearchChannelName.Enabled = true;
                return;
            }

            lbLog.Items.Clear();
            lbLog.Items.Add($"Скачивание списка стримов канала {channelName}...");
            tabControlMain.SelectedTab = tabPageLog;
            lbLog.Update();
            ClearFramesStream();
            tabPageStreams.Text = "Стримы";

            int limit = 0;
            if (rbSearchLimit.Checked)
            {
                limit = (int)numericUpDownSearchLimit.Value;
            }
            int n = GetChannelVideosListJson_Kraken(channelName, limit, out string resList);
            if (n > 0)
            {
                tabPageStreams.Text = $"Стримы ({n})";
                lbLog.Items.Add("Обработка данных...");
                ParseVideosListJSON(resList);
                if (tabControlMain.SelectedTab == tabPageStreams)
                {
                    StackFramesStream();
                }
                lbLog.Items.Add("Скачивание изображений...");
                DownloadImages();
                tabControlMain.SelectedTab = tabPageStreams;
                lbLog.Items.Add("Готово!");
            }
            else
            {
                lbLog.Items.Add("Стримы не найдены!");
            }

            btnSearchChannelName.Enabled = true;
        }

        private void btnSearchByUrls_Click(object sender, EventArgs e)
        {
            btnSearchByUrls.Enabled = false;

            string[] urls = textBoxUrls.Lines;
            if (urls.Length == 0)
            {
                MessageBox.Show("Введите ссылки!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSearchByUrls.Enabled = true;
                return;
            }

            tabControlMain.SelectedTab = tabPageLog;
            lbLog.Items.Clear();
            lbLog.Items.Add("Поиск видео по ссылкам...");
            tabPageStreams.Text = "Стримы";

            ClearFramesStream();

            JArray jsonArray = new JArray();
            for (int i = 0; i < urls.Length; i++)
            {
                if (string.IsNullOrEmpty(urls[i]) || string.IsNullOrWhiteSpace(urls[i]))
                {
                    lbLog.Items.Add($"{i + 1} / {urls.Length}: Empty URL!");
                    continue;
                }
                string vodId = ExtractVodIdFromUrl(urls[i]);
                if (string.IsNullOrEmpty(vodId) || string.IsNullOrWhiteSpace(vodId))
                {
                    lbLog.Items.Add($"{i + 1} / {urls.Length}: {urls[i]}...FAILED!");
                    continue;
                }

                TwitchApi twitchApi = new TwitchApi();
                int errorCode = twitchApi.GetVodInfo_Kraken(vodId, out string infoStringJson);
                if (errorCode == 200)
                {
                    JObject jObject = JObject.Parse(infoStringJson);

                    jsonArray.Add(jObject);
                    lbLog.Items.Add($"{i + 1} / {urls.Length}: {urls[i]}...OK");
                }
                else
                {
                    lbLog.Items.Add($"{i + 1} / {urls.Length}: {urls[i]}...FAILED! Error code {errorCode}");
                }
                Application.DoEvents();
            }

            if (jsonArray.Count > 0)
            {
                lbLog.Items.Add("Обработка данных...");
                JObject json = new JObject();
                json.Add(new JProperty("videos", jsonArray));
                int count = ParseVideosListJSON(json.ToString());
                if (count > 0)
                {
                    tabPageStreams.Text = $"Стримы ({count})";
                    lbLog.Items.Add("Скачивание изображений...");
                    DownloadImages();
                    StackFramesStream();
                    tabControlMain.SelectedTab = tabPageStreams;
                    lbLog.Items.Add("Готово!");
                }
                else
                {
                    lbLog.Items.Add("Стримы не найдены!");
                }
            }
            else
            {
                lbLog.Items.Add("Стримы не найдены!");
            }

            btnSearchByUrls.Enabled = true;
        }

        private void OnDownloadButtonClick(object sender)
        {
            if (string.IsNullOrEmpty(config.downloadingPath))
            {
                MessageBox.Show("Не указана папка для скачивания!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FrameStream frameStream = sender as FrameStream;
            frameStream.btnDownload.Enabled = false;
            ThreadGetVodPlaylist threadGetVodPlaylist = new ThreadGetVodPlaylist(frameStream.StreamInfo);
            threadGetVodPlaylist.controls.Add(frameStream.btnDownload);
            threadGetVodPlaylist.Completed += OnThreadGetVodPlaylist_Completed;

            Thread thr = new Thread(threadGetVodPlaylist.Work);
            thr.Start(synchronizationContext);
        }

        private void btnSelectDownloadingPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Выберите папку для скачивания";
            folderBrowserDialog.SelectedPath =
                (!string.IsNullOrEmpty(config.downloadingPath) && Directory.Exists(config.downloadingPath)) ?
                config.downloadingPath : config.selfPath;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                config.downloadingPath =
                    folderBrowserDialog.SelectedPath.EndsWith("\\")
                    ? folderBrowserDialog.SelectedPath : folderBrowserDialog.SelectedPath + "\\";

                textBox_DownloadingPath.Text = config.downloadingPath;
            }
        }

        private void scrollBarStreams_Scroll(object sender, ScrollEventArgs e)
        {
            StackFramesStream();
        }

        private void saveImageAssToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "jpg|*.jpg";
            sfd.DefaultExt = ".jpg";
            sfd.InitialDirectory = config.lastUsedPath;
            string fn = FixFileName(FormatFileName(config.fileNameFormat, activeFrameStream.StreamInfo));
            sfd.FileName = fn + "_preview";
            if (sfd.ShowDialog() != DialogResult.Cancel)
            {
                config.lastUsedPath = sfd.FileName;
                activeFrameStream.StreamInfo.ImageData.SaveToFile(sfd.FileName);
            }
            sfd.Dispose();
        }

        private void btnSelectBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Выберите браузер";
            ofd.Filter = "exe|*.exe";
            string dir = string.IsNullOrEmpty(config.browserExe) ? config.selfPath : Path.GetFullPath(config.browserExe);
            ofd.InitialDirectory = dir;
            if (ofd.ShowDialog() != DialogResult.Cancel)
            {
                config.browserExe = ofd.FileName;
                textBox_Browser.Text = ofd.FileName;
            }
            ofd.Dispose();
        }

        private void openVideoInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(config.browserExe) || string.IsNullOrWhiteSpace(config.browserExe))
            {
                MessageBox.Show("Браузер не указан!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!File.Exists(config.browserExe))
            {
                MessageBox.Show("Браузер не найден!", "Ошибка!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Process process = new Process();
            process.StartInfo.FileName = Path.GetFileName(config.browserExe);
            process.StartInfo.WorkingDirectory = Path.GetFullPath(config.browserExe);
            process.StartInfo.Arguments = activeFrameStream.StreamInfo.VideoUrl;
            process.Start();
        }

        private void textBox_FileNameFormat_Leave(object sender, EventArgs e)
        {
            config.fileNameFormat = (sender as TextBox).Text;
        }

        private void scrollBarDownloads_Scroll(object sender, ScrollEventArgs e)
        {
            StackFramesDownload();
        }

        private void miCopyVideoUrl_Click(object sender, EventArgs e)
        {
            SetClipboardText(activeFrameStream.StreamInfo.VideoUrl);
        }

        private void btnRestoreDefaultFilenameFormat_Click(object sender, EventArgs e)
        {
            textBox_FileNameFormat.Text = FILENAME_FORMAT_DEFAULT;
            config.fileNameFormat = FILENAME_FORMAT_DEFAULT;
        }

        private void tabControlMain_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage == tabPageStreams)
            {
                StackFramesStream();
            }
            else if (e.TabPage == tabPageDownload)
            {
                StackFramesDownload();
            }
        }
    }
}
