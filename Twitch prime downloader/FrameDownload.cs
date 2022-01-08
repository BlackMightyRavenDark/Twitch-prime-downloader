using System;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Twitch_prime_downloader.Utils;
using Twitch_prime_downloader.Properties;

namespace Twitch_prime_downloader
{
    public partial class FrameDownload : UserControl
    { 
        private SynchronizationContext fContext = null;
        private ThreadDownload threadDownload = null;
        public TwitchVod StreamInfo { get; private set; }
        public string OutputFilenameOrig { get; set; }
        public string OutputFileName { get; private set; }
        private int _chunkFrom = 0;
        private int _chunkTo = 10;
        private int _currentChunkId = 0;
        public int ChunkFrom { get { return _chunkFrom; } set { SetChunkFrom(value); } }
        public int ChunkTo { get { return _chunkTo; } set { SetChunkTo(value); } }
        public long CurrentChunkFileSize { get; private set; }
        public long DownloadedSize { get; private set; }
        public string StreamRootUrl { get; private set; }
        public DownloadingMode DownloadingMode { get; private set; } = DownloadingMode.WholeFile;
        public DateTime DownloadStarted { get; private set; }
        private List<TwitchVodChunk> _chunks;
        public TwitchVodChunk[] Chunks => _chunks.ToArray();

        public const int EXTRA_WIDTH = 450;
        private int fcstId = 0;
        private int oldX;
        private bool needToStop;

        public int TotalChunksCount => _chunks.Count;

        public delegate void ClosedDelegate(object sender);
        public ClosedDelegate Closed;

        public FrameDownload(string streamRootUrl)
        {
            InitializeComponent();

            StreamRootUrl = streamRootUrl;

            OnFrameCreate();
        }

        public void OnFrameCreate()
        {
            fContext = SynchronizationContext.Current;

            _chunks = new List<TwitchVodChunk>();
            progressBar1.MaxValue2 = ChunkTo;
            lblCurrentChunkName.Text = null;
            lblProgressCurrentChunk.Text = null;
            lblElapsedTime.Text = null;
            imgScrollBar.Top = Height - imgScrollBar.Height;
        }

        public void FrameDispose()
        {
            threadDownload?.Abort();
        }

        private void FrameDownload_Resize(object sender, EventArgs e)
        {
            int panelWidth = Parent.Parent.Parent.Width - 20;
            btnClose.Location = new Point(panelWidth - btnClose.Width - 6, 2);
            pictureBoxStreamImage.Left = panelWidth - pictureBoxStreamImage.Width - 6;
            lblStreamTitle.Width = pictureBoxStreamImage.Left - lblStreamTitle.Left - 4;
            lblOutputFilename.Width = lblStreamTitle.Width;
            progressBar1.Left = lblStreamTitle.Left;
            progressBar1.Width = pictureBoxStreamImage.Left + pictureBoxStreamImage.Width;
            btnStopDownload.Left = panelWidth - btnStopDownload.Width;
            btnStartDownload.Left = btnStopDownload.Left - btnStartDownload.Width - 6;

            imgScrollBar.Left = -Left;
            imgScrollBar.Width = panelWidth;

            grpDownloadOptions.Left = panelWidth + 6;
            grpDownloadRange.Left = grpDownloadOptions.Left;

            lblFilelist.Left = grpDownloadRange.Left + grpDownloadRange.Width + 10;
            lbFileList.Left = lblFilelist.Left;
        }

        private void FrameDownload_Paint(object sender, PaintEventArgs e)
        {
            progressBar1.Refresh();
            pictureBoxStreamImage.Refresh();
            imgScrollBar.Refresh();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Закрыть фрейм?", "Быть или не быть?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                FrameDispose();
                Closed?.Invoke(this);
                Dispose();               
            }
        }

        private void PictureBoxStreamImage_Paint(object sender, PaintEventArgs e)
        {
            Font fnt = new Font("arial", 12);
            if (StreamInfo.Length > TimeSpan.MinValue)
            {
                string t = StreamInfo.Length.ToString("h':'mm':'ss");
                SizeF sz = e.Graphics.MeasureString(t, fnt);
                e.Graphics.FillRectangle(Brushes.Black, new RectangleF(0.0f, 0.0f, sz.Width, sz.Height));
                e.Graphics.DrawString(t, fnt, Brushes.Lime, 0.0f, 0.0f);
            }
            if (StreamInfo.IsPrime)
            {
                SizeF sz = e.Graphics.MeasureString("$", fnt);
                float x = (sender as PictureBox).Width - sz.Width;
                e.Graphics.FillRectangle(Brushes.Black, new RectangleF(x, 0.0f, sz.Width, sz.Height));
                e.Graphics.DrawString("$", fnt, Brushes.Lime, x, 0.0f);
            }
            fnt.Dispose();
        }

        private void ThreadDownload_WorkStarted(object sender, long fileSize, int chunkId)
        {
            CurrentChunkFileSize = fileSize;
            _currentChunkId = chunkId;
            progressBar1.Value1 = 0;
            progressBar1.MaxValue1 = 100;
            lblProgressCurrentChunk.Text = $": 0 / {FormatSize(fileSize)}";
        }

        private void ThreadDownload_WorkFinished(object sender, long bytesTransfered, int errorCode)
        {
            if (errorCode == 200)
            {
                double percent = 100.0 / CurrentChunkFileSize * bytesTransfered;
                progressBar1.Value1 = (int)percent;
                progressBar1.Value2 = _currentChunkId - ChunkFrom + 1;
                int max = ChunkTo - ChunkFrom + 1;
                percent = 100.0 / max * progressBar1.Value2;
                string t;
                if (threadDownload.DownloadingMode == DownloadingMode.WholeFile)
                {
                    t = $"Скачано чанков: {progressBar1.Value2} / {max} ({string.Format("{0:F2}", percent)}%)" +
                        ", Размер файла: " + FormatSize(threadDownload.DownloadedFileSize);
                }
                else
                {
                    DownloadedSize += bytesTransfered;
                    t = $"Скачано чанков: {progressBar1.Value2} / {max} ({string.Format("{0:F2}", percent)}%)" +
                        ", Размер скачанного: " + FormatSize(DownloadedSize);
                }

                lblProgressOverall.Text = t;

                int x = progressBar1.Value2 * (progressBar1.Width - imgFcst.Width) / max;
                imgFcst.Left = x;
            }
        }

        private void ThreadDownload_Progress(object sender, long bytesTransfered)
        {
            double percent = 100.0 / CurrentChunkFileSize * bytesTransfered;
            progressBar1.Value1 = (int)percent;
            lblProgressCurrentChunk.Text = $": {FormatSize(bytesTransfered)} / {FormatSize(CurrentChunkFileSize)}" +
                $" ({string.Format("{0:F2}", percent)}%)";
        }

        private void ThreadDownloading_Completed(object sender, int errorCode)
        {
            timerElapsed.Enabled = false;
            timerFcst.Enabled = false;

            string msgCaption = StreamInfo.IsPrime ? "Скачиватор платного бесплатно" : "Скачивание";
            if (errorCode != ThreadDownload.ERROR_DOWNLOAD_TERMINATED)
            {
                switch (errorCode)
                {
                    case 200:
                        MessageBox.Show($"{StreamInfo.Title}\nСкачано успешно!", msgCaption,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;

                    case FileDownloader.DOWNLOAD_ERROR_ABORTED_BY_USER:
                        MessageBox.Show($"{StreamInfo.Title}\nСкачивание успешно отменено!", msgCaption,
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;

                    case FileDownloader.DOWNLOAD_ERROR_INCOMPLETE_DATA_READ:
                        MessageBox.Show($"{StreamInfo.Title}\nОшибка INCOMPLETE_DATA_READ!\nСкачивание прервано!",
                            msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS:
                        MessageBox.Show($"{StreamInfo.Title}\nОшибка объединения чанков!\nСкачивание прервано!",
                            msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case FileDownloader.DOWNLOAD_ERROR_ZERO_LENGTH_CONTENT:
                        MessageBox.Show($"{StreamInfo.Title}\nФайл на сервере пуст!",
                            msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    case FileDownloader.DOWNLOAD_ERROR_UNKNOWN:
                        MessageBox.Show($"{StreamInfo.Title}\nНеизвестная ошибка!\nСкачивание прервано!", msgCaption,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;

                    default:
                        MessageBox.Show($"{StreamInfo.Title}\nНеизвестная ошибка!" +
                            $"\nСкачивание прервано!\nКод ошибки: {errorCode}", msgCaption,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }

            editFrom.Enabled = true;
            editTo.Enabled = true;
            btnSetMaxChunkTo.Enabled = true;
            rbDownloadOneBigFile.Enabled = true;
            rbDownloadChunksSeparatelly.Enabled = true;
            btnStartDownload.Enabled = true;
        }

        private void StartDownload()
        {
            btnStartDownload.Enabled = false;
            needToStop = false;
            DownloadStarted = DateTime.Now;
            lblElapsedTime.Text = "Прошло времени: 0:00:00";
            timerElapsed.Enabled = true;
            OutputFileName = GetNumberedFileName(OutputFilenameOrig);
            lblOutputFilename.Text = "Имя файла: " + OutputFileName;
            lblProgressOverall.Text = $"Скачано чанков: 0 / {ChunkTo - ChunkFrom + 1} (0.00%), Размер файла: 0 bytes";
            progressBar1.MinValue1 = 0;
            progressBar1.Value1 = 0;
            progressBar1.Value2 = 0;
            DownloadedSize = 0L;
            editFrom.Enabled = false;
            editTo.Enabled = false;
            btnSetMaxChunkTo.Enabled = false;
            rbDownloadOneBigFile.Enabled = false;
            rbDownloadChunksSeparatelly.Enabled = false;

            if (DownloadingMode == DownloadingMode.Chunked)
            {
                OutputFileName = GetNumberedDirectoryName(OutputFilenameOrig);
                lblOutputFilename.Text = $"Папка для скачивания: {OutputFileName}";
            }

            threadDownload = new ThreadDownload(OutputFileName, DownloadingMode);
            threadDownload.WorkProgress += ThreadDownload_Progress;
            threadDownload.WorkStarted += ThreadDownload_WorkStarted;
            threadDownload.WorkFinished += ThreadDownload_WorkFinished;
            threadDownload.Completed += ThreadDownloading_Completed;
            threadDownload.Connecting += (object sender, TwitchVodChunk chunk) =>
            {
                progressBar1.Value1 = 0;
                lblCurrentChunkName.Text = chunk.FileName;
                lblCurrentChunkName.ForeColor = chunk.GetState() == TwitchVodChunkState.NotMuted ? Color.Black : Color.Red;
                lblProgressCurrentChunk.Text = ": Connecting...";
                lblProgressCurrentChunk.Left = lblCurrentChunkName.Left + lblCurrentChunkName.Width;
            };
            threadDownload.CancelTest += (object sender, ref bool stop) =>
            {
                stop = needToStop;
            };
            threadDownload.ChunkChanged += (s, id) =>
            {
                _chunks.RemoveAt(id);
                _chunks.Insert(id, new TwitchVodChunk((s as ThreadDownload)._chunks[id].FileName));
                lbFileList.Items.RemoveAt(id);
                lbFileList.Items.Insert(id, _chunks[id].FileName);
            };

            threadDownload._streamRoot = StreamRootUrl;
            threadDownload._chunks = _chunks;
            threadDownload.ChunkFrom = _chunkFrom;
            threadDownload.ChunkTo = ChunkTo;

            imgFcst.Visible = true;
            timerFcst.Enabled = true;
            Thread thr = new Thread(threadDownload.Work);
            thr.Start(fContext);
        }

        public void StopDownload()
        {
            needToStop = true;
        }

        public void AbortDownload()
        {
            if (threadDownload != null)
                threadDownload.Abort();
        }

        private void BtnStartDownload_Click(object sender, EventArgs e)
        {
            StartDownload();
        }

        private void FrameDownload_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                oldX = e.X;
            }
        }

        private void FrameDownload_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int newX = Left + e.X - oldX;
                if (newX > 0)
                {
                    newX = 0;
                }
                else if (newX < -EXTRA_WIDTH)
                {
                    newX = -EXTRA_WIDTH;
                }
                Left = newX;
                imgScrollBar.Left = -Left;

                Refresh();
            }
        }

        public void SetStreamInfo(TwitchVod vod)
        {
            StreamInfo = vod;
            lblStreamTitle.Text = $"Стрим: {StreamInfo.Title}";
            OutputFilenameOrig = config.downloadingPath +
                FixFileName(FormatFileName(config.fileNameFormat, StreamInfo)) +
                (vod.IsHighlight() ? " [highlight].ts" : ".ts");
            lblOutputFilename.Text = $"Имя файла: {OutputFilenameOrig}";
            if (vod.ImageData != null)
            {
                pictureBoxStreamImage.Image = Image.FromStream(vod.ImageData);
            }
        }

        public void SetChunks(IEnumerable<TwitchVodChunk> chunks)
        {
            foreach (TwitchVodChunk chunk in chunks)
            {
                _chunks.Add(chunk);
                lbFileList.Items.Add(chunk.FileName);
            }
        }

        private void SetChunkIndicators()
        {
            lblProgressCurrentChunk.Text = null;
            progressBar1.Value1 = 0;
            lblProgressOverall.Text = $"Всего чанков: {TotalChunksCount}, Скачивать: {ChunkTo - ChunkFrom + 1}";
            progressBar1.MinValue2 = 0;
            progressBar1.Value2 = 0;
            progressBar1.MaxValue2 = ChunkTo - ChunkFrom;
        }

        private void SetChunkFrom(int chunkId)
        {
            if (chunkId != _chunkFrom)
            {
                if (chunkId < 0)
                {
                    chunkId = 0;
                }
                else if (chunkId >= TotalChunksCount)
                {
                    chunkId = TotalChunksCount - 1;
                }
                _chunkFrom = chunkId;
                if (_chunkTo < _chunkFrom)
                {
                    _chunkTo = _chunkFrom;
                    editTo.Text = (_chunkTo + 1).ToString();
                }
                editFrom.Text = (_chunkFrom + 1).ToString();
                SetChunkIndicators();
            }
        }

        private void SetChunkTo(int chunkId)
        {
            if (chunkId != _chunkTo)
            {
                _chunkTo = chunkId;
                editTo.Text = (_chunkTo + 1).ToString();
                if (_chunkTo < _chunkFrom)
                {
                    _chunkFrom = _chunkTo;
                    editFrom.Text = (_chunkFrom + 1).ToString();
                }
                SetChunkIndicators();
            }
        }

        private void EditFrom_Leave(object sender, EventArgs e)
        {
            _chunkFrom = int.Parse(editFrom.Text) - 1;
            if (_chunkFrom < 0)
            {
                _chunkFrom = 0;
                editFrom.Text = "1";
            }
            else if (_chunkFrom >= TotalChunksCount)
            {
                _chunkFrom = TotalChunksCount - 1;
                editFrom.Text = (_chunkFrom + 1).ToString();
            }
            if (_chunkFrom > _chunkTo)
            {
                _chunkTo = _chunkFrom;
                editTo.Text = (_chunkTo + 1).ToString();
            }

            SetChunkIndicators();
        }

        private void EditTo_Leave(object sender, EventArgs e)
        {
            _chunkTo = int.Parse(editTo.Text) - 1;
            if (_chunkTo < 0)
            {
                _chunkTo = 0;
                editTo.Text = "1";
            }
            if (_chunkTo >= TotalChunksCount)
            {
                _chunkTo = TotalChunksCount - 1;
                editTo.Text = (_chunkTo + 1).ToString();
            }
            else if (_chunkTo < _chunkFrom)
            { 
                _chunkFrom = _chunkTo;
                editFrom.Text = (_chunkFrom + 1).ToString();
            }

            SetChunkIndicators();
        }

        private void BtnSetMaxChunkTo_Click(object sender, EventArgs e)
        {
            ChunkTo = TotalChunksCount - 1;
        }

        private void CopyStreamTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetClipboardText(StreamInfo.Title);
        }

        private void LblStreamTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStreamTitle.Show(Cursor.Position);
            }
        }

        private void BtnStopDownload_Click(object sender, EventArgs e)
        {
            StopDownload();
        }

        private void timerElapsed_Tick(object sender, EventArgs e)
        {
            DateTime elapsedTime = new DateTime((DateTime.Now - DownloadStarted).Ticks);
            lblElapsedTime.Text = $"Прошло времени: {elapsedTime:H:mm:ss}";
        }

        private void timerFcst_Tick(object sender, EventArgs e)
        {
            fcstId++;
            if (fcstId > 7)
            {
                fcstId = 0;
            }
            var res = Resources.ResourceManager.GetObject($"fcst_istra_0{fcstId + 1}");
            imgFcst.Image = (Bitmap)res;
        }

        private void rbDownloadOneBigFile_CheckedChanged(object sender, EventArgs e)
        {
            DownloadingMode = DownloadingMode.WholeFile;
            string fn = config.downloadingPath + FixFileName(FormatFileName(config.fileNameFormat, StreamInfo)) + ".ts";
            OutputFilenameOrig = fn;
            lblOutputFilename.Text = $"Имя файла: {OutputFilenameOrig}";
        }

        private void rbDownloadChunksSeparatelly_CheckedChanged(object sender, EventArgs e)
        {
            DownloadingMode = DownloadingMode.Chunked;
            OutputFilenameOrig = $"{config.downloadingPath}{FixFileName(FormatFileName(config.fileNameFormat, StreamInfo))}\\";
            lblOutputFilename.Text = $"Папка для скачивания: {OutputFilenameOrig}";
        }

        private void imgScrollBar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
            int xLeft = (int)Math.Round(imgScrollBar.Width / (double)Width * -Left);
            int xRight = (int)Math.Round(imgScrollBar.Width / (double)Width * (-Left + Parent.Width));
            Rectangle r = new Rectangle(xLeft, 0, xRight - xLeft, imgScrollBar.Height);
            e.Graphics.FillRectangle(Brushes.Black, r);
        }

    }
}
