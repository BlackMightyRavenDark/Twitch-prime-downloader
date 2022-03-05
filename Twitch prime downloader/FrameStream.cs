using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
    public partial class FrameStream : UserControl
    {
        public TwitchVod StreamInfo { get; private set; }
        private int _hudInfoFontSize = 10;
        private bool _showStreamInfoHud = true;
        private bool _useLocalTime;
        public int HudInfoFontSize { get { return _hudInfoFontSize; } set { SetInfoGuiFontSize(value); } }
        public bool ShowStreaInfoHud { get { return _showStreamInfoHud; } set { SetShowInfoHud(value); } }
        public bool UseLocalTime { get { return _useLocalTime; } set { SetUseLocalTime(value); } }

        public static readonly Color colorActive = IntToColor(0x909090);
        public static readonly Color colorInactive = IntToColor(0x303030);

        public delegate void ImageMouseDownDelegate(object sender, MouseEventArgs e);
        public delegate void FrameActivatedDelegate(object sender);
        public delegate void DownloadButtonPressedDelegate(object sender);
        public ImageMouseDownDelegate ImageMouseDown;
        public FrameActivatedDelegate Activated;
        public DownloadButtonPressedDelegate DownloadButtonPressed;
        
        public FrameStream()
        {
            InitializeComponent();

            _useLocalTime = config.showLocalVodTime;

            try
            {
                using (GraphicsPath graphicsPath = new GraphicsPath())
                {
                    graphicsPath.AddEllipse(0, 0, btnDownload.Width, btnDownload.Height);
                    Region region = new Region(graphicsPath);
                    btnDownload.Region = region;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        public void SetStreamInfo(TwitchVod vod)
        {
            StreamInfo = vod;
            lblStreamTitle.Text = StreamInfo.Title;
            lblChannelName.Text = StreamInfo.UserInfo.DisplayName;
            lblGameName.Text = StreamInfo.GameInfo.Title;
            lblBroadcastType.Text = StreamInfo.Type;
            if (StreamInfo.MutedSegments.Segments.Count > 0)
            {
                lblMutedChunks.Text = $"Muted segments: {StreamInfo.MutedSegments.Segments.Count}";
                lblMutedChunks.Left = Width - lblMutedChunks.Width;
                lblMutedChunks.Visible = true;
            }
            else
            {
                lblMutedChunks.Visible = false;
            }
            lblPrime.Visible = StreamInfo.IsPrime;
        }

        private void imageStream_Paint(object sender, PaintEventArgs e)
        {
            if (ShowStreaInfoHud)
            {
                try
                {
                    using (Font fnt = new Font("Lucida Console", HudInfoFontSize))
                    {
                        string t = StreamInfo.Length.ToString("h':'mm':'ss");
                        SizeF size = e.Graphics.MeasureString(t, fnt);
                        RectangleF r = new RectangleF(0, 0, size.Width, size.Height);
                        e.Graphics.FillRectangle(Brushes.Black, r);
                        e.Graphics.DrawString(t, fnt, Brushes.White, r.X, r.Y);

                        t = StreamInfo.DateCreation.ToString("yyyy.MM.dd, HH:mm:ss");
                        size = e.Graphics.MeasureString(t, fnt);
                        r = new RectangleF(
                            (sender as PictureBox).Width - size.Width - 2,
                            (sender as PictureBox).Height - size.Height - 2,
                            size.Width, size.Height);
                        e.Graphics.FillRectangle(Brushes.Black, r);
                        e.Graphics.DrawString(t, fnt, Brushes.White, new Point((int)r.X, (int)r.Y));
                        if (StreamInfo.DateDeletion > DateTime.MinValue)
                        {
                            int y = (int)((sender as PictureBox).Height - (size.Height * 2) - 2);
                            t = "Будет удалён: " + StreamInfo.DateDeletion.ToString("yyyy.MM.dd, HH:mm:ss");
                            size = e.Graphics.MeasureString(t, fnt);
                            r = new RectangleF((sender as PictureBox).Width - size.Width - 2, y, size.Width, size.Height);
                            e.Graphics.FillRectangle(Brushes.Black, r);
                            e.Graphics.DrawString(t, fnt, Brushes.Yellow, new Point((int)r.X, (int)r.Y));
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        private void imageStream_MouseDown(object sender, MouseEventArgs e)
        {
            Activated?.Invoke(this);
            ImageMouseDown?.Invoke(this, e);
        }

        private void FrameStream_MouseDown(object sender, MouseEventArgs e)
        {
            Activated?.Invoke(this);
        }

        private void lblStreamTitle_MouseDown(object sender, MouseEventArgs e)
        {
            Activated?.Invoke(this);
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DownloadButtonPressed?.Invoke(this);
        }

        private void btnDownload_Paint(object sender, PaintEventArgs e)
        {
            Button button = sender as Button;
            Color color = button.Enabled ? button.BackColor : Color.FromArgb(192, 192, 192);
            Brush brush = new SolidBrush(color);
            e.Graphics.FillRectangle(brush, e.ClipRectangle);
            brush.Dispose();
            string t = button.Enabled ? button.Text : "Ждите...";
            if (!string.IsNullOrEmpty(t) && !string.IsNullOrWhiteSpace(t))
            {
                SizeF sz = e.Graphics.MeasureString(t, button.Font);
                int x = (int)(button.Width / 2 - sz.Width / 2);
                int y = (int)(button.Height / 2 - sz.Height / 2);
                brush = new SolidBrush(button.ForeColor);
                e.Graphics.DrawString(t, button.Font, brush, new Point(x, y));
                brush.Dispose();
            }
        }

        private void SetInfoGuiFontSize(int newFontSize)
        {
            if (_hudInfoFontSize != newFontSize)
            {
                _hudInfoFontSize = newFontSize;
                if (ShowStreaInfoHud)
                {
                    imageStream.Refresh();
                }
            }
        }

        private void SetShowInfoHud(bool flag)
        {
            if (_showStreamInfoHud != flag)
            {
                _showStreamInfoHud = flag;
                imageStream.Refresh();
            }
        }

        private void SetUseLocalTime(bool flag)
        {
            if (_useLocalTime != flag)
            {
                _useLocalTime = flag;
                StreamInfo.DateCreation = TwitchTimeToDateTime(StreamInfo.DateCreationString, flag);
                TimeSpan vodLifeTime = TimeSpan.FromDays(StreamInfo.UserInfo.BroadcasterType == TwitchBroadcasterType.Partner ? 60 : 14);
                StreamInfo.DateDeletion = new DateTime((TimeSpan.FromTicks(StreamInfo.DateCreation.Ticks) + vodLifeTime).Ticks);
                imageStream.Refresh();
            }
        }

        private void copyStreamTitleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetClipboardText(StreamInfo.Title);
        }

        private void copyStreamDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetClipboardText(StreamInfo.DateCreation.ToString("[yyyy-MM-dd]"));
        }

        private void copyStreamTitlePlusDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetClipboardText($"[{StreamInfo.DateCreation:yyyy-MM-dd}] {StreamInfo.Title}");
        }

        private void lblMutedChunks_DoubleClick(object sender, EventArgs e)
        {
            string t = $"Стрим: {StreamInfo.Title}{Environment.NewLine}Выпилен звук:{Environment.NewLine}";
            for (int i = 0; i < StreamInfo.MutedSegments.SegmentList.Count; i++)
            {
                t += StreamInfo.MutedSegments.SegmentList[i] + Environment.NewLine;
            }

            double percent = 100.0 / StreamInfo.Length.Ticks * StreamInfo.MutedSegments.TotalLength.Ticks;
            t += $"\nВсего выпилено: {StreamInfo.MutedSegments.TotalLength:h':'mm':'ss} ({string.Format("{0:F2}", percent)}%)";
            if (MessageBox.Show($"{t}\n\nСкопировать это прямо в буфер?", "Определятор выпиленного звука",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SetClipboardText(t);
            }
        }
    }
}
