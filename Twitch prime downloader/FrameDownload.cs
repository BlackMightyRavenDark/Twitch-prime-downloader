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
        private TwitchStreamInfo fStreamInfo;
        public string fOutputFilenameOrig;
        public string fOutputFileName;
        private int fChunkFrom = 0;
        private int fChunkTo = 10;
        private int fCurrentChunkID = 0;
        private long fCurrentChunkFileSize;
        public string streamRoot;
        private DOWNLOADING_MODE downloadingMode = DOWNLOADING_MODE.DM_FILE;
        private DateTime downloadStarted;
        public List<TwitchVodChunk> fChunks;
        public const int EXTRA_WIDTH = 450;
        private int fcstId = 0;
        private int oldX;
        private bool drag = false;

        public int TotalChunksCount
        {
            get
            {
                return fChunks.Count;
            }
        }

        public int ChunkFrom
        {
            set
            {
                SetChunkFrom(value);
            }
            get
            {
                return fChunkFrom;
            }
        }
        public int ChunkTo
        {
            set
            {
                SetChunkTo(value);
            }
            get
            {
                return fChunkTo;
            }
        }

        public TwitchStreamInfo StreamInfo
        {
            get
            {
                return fStreamInfo;
            }
        }

        public event Action<object> Close;

        public FrameDownload()
        {
            InitializeComponent();
            OnFrameCreate();
        }

        public void OnFrameCreate()
        {
            fContext = SynchronizationContext.Current;

            fChunks = new List<TwitchVodChunk>();
            progressBar1.MaxValue2 = fChunkTo;
            lblCurrentChunkName.Text = string.Empty;
            lblProgressCurrentChunk.Text = string.Empty;
            lblElapsedTime.Text = string.Empty;
            imgScrollBar.Top = Height - imgScrollBar.Height;
        }

        public void FrameDispose()
        {
            if (threadDownload != null)
            {
                threadDownload.Abort();
            }
        }

        private void FrameDownload_Resize(object sender, EventArgs e)
        {
            btnClose.Location = new Point(Parent.Width - btnClose.Width - 6, 2);
            pictureBoxStreamImage.Left = Parent.Width - pictureBoxStreamImage.Width - 6;
            lblStreamTitle.Width = pictureBoxStreamImage.Left - lblStreamTitle.Left - 4;
            lblOutputFilename.Width = lblStreamTitle.Width;
            progressBar1.Left = lblStreamTitle.Left;
            progressBar1.Width = pictureBoxStreamImage.Left + pictureBoxStreamImage.Width;
            btnStopDownload.Left = Parent.Width - btnStopDownload.Width;
            btnStartDownload.Left = btnStopDownload.Left - btnStartDownload.Width - 6;

            imgScrollBar.Left = -Left;
            imgScrollBar.Width = Parent.Width;

            grpDownloadOptions.Left = Parent.Width + 6;
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
                Close?.Invoke(this);
                Dispose();               
            }
        }

        private void PictureBoxStreamImage_Paint(object sender, PaintEventArgs e)
        {
            Font fnt = new Font("arial", 12);
            if (StreamInfo.length > DateTime.MinValue)
            {
                string t = StreamInfo.length.ToString("H:mm:ss");
                SizeF sz = e.Graphics.MeasureString(t, fnt);
                e.Graphics.FillRectangle(Brushes.Black, new RectangleF(0.0f, 0.0f, sz.Width, sz.Height));
                e.Graphics.DrawString(t, fnt, Brushes.Lime, 0.0f, 0.0f);
            }
            if (StreamInfo.isPrime)
            {
                SizeF sz = e.Graphics.MeasureString("$", fnt);
                float x = (sender as PictureBox).Width - sz.Width;
                e.Graphics.FillRectangle(Brushes.Black, new RectangleF(x, 0.0f, sz.Width, sz.Height));
                e.Graphics.DrawString("$", fnt, Brushes.Lime, x, 0.0f);
            }
            fnt.Dispose();
        }

        private void ThreadDownload_WorkStart(object sender, long iFileSize, int chunkId)
        {
            fCurrentChunkFileSize = iFileSize;
            fCurrentChunkID = chunkId;
            progressBar1.Value1 = 0;
            progressBar1.MaxValue1 = 100;
            lblProgressCurrentChunk.Text = $": 0 / {FormatSize(iFileSize)}";
        }

        private void ThreadDownload_WorkEnd(object sender, long bytesTransfered, int errorCode)
        {
            if (errorCode == 200)
            {
                double percent = 100.0 / fCurrentChunkFileSize * bytesTransfered;
                progressBar1.Value1 = (int)percent;
                progressBar1.Value2 = fCurrentChunkID - ChunkFrom + 1;
                int max = ChunkTo - ChunkFrom + 1;
                percent = 100.0 / max * progressBar1.Value2;
                string t = $"Скачано чанков: {progressBar1.Value2} / {max} ({string.Format("{0:F2}", percent)}%)" +
                    ", Размер файла: " + FormatSize((sender as ThreadDownload).GetDownloadedStreamSize());
                lblProgressOverall.Text = t;

                int x = progressBar1.Value2 * (progressBar1.Width - imgFcst.Width) / max;
                imgFcst.Left = x;
            }
        }

        private void OnThreadDownload_Progress(object sender, long bytesTransfered)
        {
            double percent = 100.0 / fCurrentChunkFileSize * bytesTransfered;
            progressBar1.Value1 = (int)percent;
            lblProgressCurrentChunk.Text = $": {FormatSize(bytesTransfered)} / {FormatSize(fCurrentChunkFileSize)}" +
                $" ({string.Format("{0:F2}", percent)}%)";
        }

        private void OnThreadCompleted(object sender)
        {
            timerElapsed.Enabled = false;
            timerFcst.Enabled = false;

            string msgCaption = StreamInfo.isPrime ? "Скачиватор платного бесплатно" : "Скачивание";
            int errorCode = (sender as ThreadDownload).lastErrorCode;
            switch (errorCode)
            {
                case 200:
                    MessageBox.Show($"{StreamInfo.title}\nСкачано успешно!", msgCaption,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            
                case FileDownloader.DOWNLOAD_ERROR_ABORTED_BY_USER:
                    MessageBox.Show($"{StreamInfo.title}\nСкачивание успешно отменено", msgCaption,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                
                case FileDownloader.DOWNLOAD_ERROR_INCOMPLETE_DATA_READ:
                    MessageBox.Show($"{StreamInfo.title}\nОшибка INCOMPLETE_DATA_READ!\nСкачивание прервано!", 
                        msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                
                default:
                    MessageBox.Show($"{StreamInfo.title}\nНеизвестная ошибка!" +
                        $"\nСкачивание прервано!\nКод ошибки: {errorCode}", msgCaption,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            editFrom.Enabled = true;
            editTo.Enabled = true;
            btnSetMaxChunkTo.Enabled = true;
            rbDownloadOneBigFile.Enabled = true;
            btnStartDownload.Enabled = true;
        }

        private void StartDownload()
        {
            btnStartDownload.Enabled = false;
            downloadStarted = DateTime.Now;
            lblElapsedTime.Text = "Прошло времени: 0:00:00";
            timerElapsed.Enabled = true;
            fOutputFileName = GetNumberedFileName(fOutputFilenameOrig);
            lblOutputFilename.Text = "Имя файла: " + fOutputFileName;
            lblProgressOverall.Text = $"Скачано чанков: 0 / {ChunkTo - ChunkFrom + 1} (0.00%), Размер файла: 0 bytes";
            progressBar1.MinValue1 = 0;
            progressBar1.Value1 = 0;
            progressBar1.Value2 = 0;
            editFrom.Enabled = false;
            editTo.Enabled = false;
            btnSetMaxChunkTo.Enabled = false;
            rbDownloadOneBigFile.Enabled = false;
            rbDownloadChunksSeparatelly.Enabled = false;

            threadDownload = new ThreadDownload();
            threadDownload.WorkProgress += OnThreadDownload_Progress;
            threadDownload.WorkStart += ThreadDownload_WorkStart;
            threadDownload.WorkEnd += ThreadDownload_WorkEnd;
            threadDownload.OnComplete += OnThreadCompleted;
            threadDownload.Connecting += (object sender, TwitchVodChunk chunk) =>
            {
                progressBar1.Value1 = 0;
                lblCurrentChunkName.Text = chunk.fileName;
                lblCurrentChunkName.ForeColor = chunk.GetState() == TwitchVodChunkState.CS_NONMUTED ? Color.Black : Color.Red;
                lblProgressCurrentChunk.Text = ": Connecting...";
                lblProgressCurrentChunk.Left = lblCurrentChunkName.Left + lblCurrentChunkName.Width;
            };
            threadDownload.ChunkChanged += (s, id) =>
            {
                fChunks[id].fileName = (s as ThreadDownload)._chunks[id].fileName;
                lbFileList.Items.RemoveAt(id);
                lbFileList.Items.Insert(id, fChunks[id].fileName);
            };

            threadDownload._streamRoot = streamRoot;
            threadDownload._chunks = fChunks;
            threadDownload.fChunkFrom = fChunkFrom;
            threadDownload.fChunkTo = ChunkTo;
            threadDownload.fDownloadFilename = fOutputFileName;
            threadDownload._downloadingMode = downloadingMode;

            imgFcst.Visible = true;
            timerFcst.Enabled = true;
            Thread thr = new Thread(threadDownload.Work);
            thr.Start(fContext);
        }

        public void StopDownload()
        {
            if (threadDownload != null)
                threadDownload.Cancel();
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
                drag = true;
            }
        }

        private void FrameDownload_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drag = false;
            }
        }

        private void FrameDownload_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
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

        public void SetStreamInfo(TwitchStreamInfo aStreamInfo)
        {
            fStreamInfo = aStreamInfo;
            lblStreamTitle.Text = $"Стрим: {fStreamInfo.title}";
            fOutputFilenameOrig = config.downloadingPath +
                FixFileName(FormatFileName(config.fileNameFormat, fStreamInfo)) + ".ts";
            lblOutputFilename.Text = $"Имя файла: {fOutputFilenameOrig}";
            if (aStreamInfo.imageData != null)
                pictureBoxStreamImage.Image = Image.FromStream(aStreamInfo.imageData);
        }

        public void SetChunks(List<TwitchVodChunk> chunks)
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                TwitchVodChunk chunk = new TwitchVodChunk(chunks[i].fileName);

                fChunks.Add(chunk);
                lbFileList.Items.Add(chunk.fileName);
            }

        }

        private void SetChunkIndicators()
        {
            lblProgressCurrentChunk.Text = string.Empty;
            progressBar1.Value1 = 0;
            lblProgressOverall.Text = "Всего чанков: " + TotalChunksCount.ToString() +
                ", Скачивать: " + (fChunkTo - fChunkFrom + 1).ToString();
            progressBar1.MinValue2 = 0;
            progressBar1.Value2 = 0;
            progressBar1.MaxValue2 = fChunkTo - fChunkFrom;

        }

        private void SetChunkFrom(int chunkId)
        {
            if (chunkId != fChunkFrom)
            {
                if (chunkId < 0)
                    chunkId = 0;
                else if (chunkId >= TotalChunksCount)
                    chunkId = TotalChunksCount - 1;
                fChunkFrom = chunkId;
                if (fChunkTo < fChunkFrom)
                {
                    fChunkTo = fChunkFrom;
                    editTo.Text = (fChunkTo + 1).ToString();
                }
                editFrom.Text = (fChunkFrom + 1).ToString();
                SetChunkIndicators();
            }
        }

        private void SetChunkTo(int chunkId)
        {
            if (chunkId != fChunkTo)
            {
                fChunkTo = chunkId;
                editTo.Text = (fChunkTo + 1).ToString();
                if (fChunkTo < fChunkFrom)
                {
                    fChunkFrom = fChunkTo;
                    editFrom.Text = (fChunkFrom + 1).ToString();
                }
                SetChunkIndicators();
            }
        }

        private void EditFrom_Leave(object sender, EventArgs e)
        {
            fChunkFrom = int.Parse(editFrom.Text) - 1;
            if (fChunkFrom < 0)
            {
                fChunkFrom = 0;
                editFrom.Text = "1";
            }
            else if (fChunkFrom >= TotalChunksCount)
            {
                fChunkFrom = TotalChunksCount - 1;
                editFrom.Text = (fChunkFrom + 1).ToString();
            }
            if (fChunkFrom > fChunkTo)
            {
                fChunkTo = fChunkFrom;
                editTo.Text = (fChunkTo + 1).ToString();
            }

            SetChunkIndicators();
        }

        private void EditTo_Leave(object sender, EventArgs e)
        {
            fChunkTo = int.Parse(editTo.Text) - 1;
            if (fChunkTo < 0)
            {
                fChunkTo = 0;
                editTo.Text = "1";
            }
            if (fChunkTo >= TotalChunksCount)
            {
                fChunkTo = TotalChunksCount - 1;
                editTo.Text = (fChunkTo + 1).ToString();
            }
            else if (fChunkTo < fChunkFrom)
            { 
                fChunkFrom = fChunkTo;
                editFrom.Text = (fChunkFrom + 1).ToString();
            }

            SetChunkIndicators();
        }

        private void BtnSetMaxChunkTo_Click(object sender, EventArgs e)
        {
            ChunkTo = TotalChunksCount - 1;
        }

        private void CopyStreamTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetClipboardText(StreamInfo.title);
        }

        private void LblStreamTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuStreamTitle.Show(Cursor.Position.X, Cursor.Position.Y);
        }

        private void BtnStopDownload_Click(object sender, EventArgs e)
        {
            StopDownload();
        }

        private void timerElapsed_Tick(object sender, EventArgs e)
        {
            DateTime elapsed = new DateTime((DateTime.Now - downloadStarted).Ticks);
            lblElapsedTime.Text = "Прошло времени: " + elapsed.ToString("H:mm:ss");
        }

        private void timerFcst_Tick(object sender, EventArgs e)
        {
            fcstId++;
            if (fcstId > 7)
                fcstId = 0;
            var res = Resources.ResourceManager.GetObject($"fcst_istra_0{fcstId + 1}");
            imgFcst.Image = (Bitmap)res;
        }

        private void rbDownloadOneBigFile_CheckedChanged(object sender, EventArgs e)
        {
            downloadingMode = DOWNLOADING_MODE.DM_FILE;
            string fn = config.downloadingPath + FixFileName(FormatFileName(config.fileNameFormat, StreamInfo)) + ".ts";
            fOutputFilenameOrig = fn;
            lblOutputFilename.Text = $"Имя файла: {fOutputFilenameOrig}";
        }

        private void rbDownloadChunksSeparatelly_CheckedChanged(object sender, EventArgs e)
        {
            downloadingMode = DOWNLOADING_MODE.DM_CHUNKED;
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
