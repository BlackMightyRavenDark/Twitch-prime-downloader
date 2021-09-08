using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
    public partial class FrameStream : UserControl
    {
        public TwitchStreamInfo streamInfo;
        private int fGuiInfoFontSize = 10;
        private bool fShowStreaInfoGUI = true;
        public static Color colorActive = IntToColor(0x909090);
        public static Color colorInactive = IntToColor(0x303030);

        public int InfoGuiFontSize
        {
            get
            {
                return fGuiInfoFontSize;
            }
            set
            {
                SetInfoGuiFontSize(value);
            }

        }
        public bool ShowStreamInfoGUI
        {
            get
            {
                return fShowStreaInfoGUI;
            }
            set
            {
                fShowStreaInfoGUI = value;
            }
        }
        public event Action<object, MouseEventArgs> OnImgMouseDown;
        public event Action<object> OnActivate;
        public delegate void DownloadButtonPressDelegate(object sender);
        public DownloadButtonPressDelegate DownloadButtonPress;
        
        public FrameStream()
        {
            InitializeComponent();
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddEllipse(0, 0, btnDownload.Width, btnDownload.Height);
            Region myRegion = new Region(myPath);
            btnDownload.Region = myRegion;
            myPath.Dispose();
        }

        public TwitchStreamInfo GetStreamInfo()
        {
            return streamInfo;
        }

        public void SetStreamInfo(TwitchStreamInfo aStream)
        {
            streamInfo = aStream;
            lblStreamTitle.Text = streamInfo.title;
            lblChannelName.Text = streamInfo.userInfo.displayName;
            lblGame.Text = streamInfo.gameInfo.title;
            if (aStream.mutedChunks.segments.Count > 0)
            {
                lblMutedChunks.Text = $"Muted segments: {aStream.mutedChunks.segments.Count}";
                lblMutedChunks.Left = Width - lblMutedChunks.Width;
                lblMutedChunks.Visible = true;
            }
            else
            {
                lblMutedChunks.Visible = false;
            }
            lblPrime.Visible = aStream.isPrime;
        }


        private void ImageStream_Paint(object sender, PaintEventArgs e)
        {
            if (ShowStreamInfoGUI)
            {
                //Random rnd = new Random();
                //int y = rnd.Next(0, (sender as PictureBox).Height / 2);
                Font fnt = new Font("Lucida Console", fGuiInfoFontSize);
                //e.Graphics.DrawString("pussy", fnt, Brushes.White, new Point(2, y));

                string t = streamInfo.length.ToString("HH:mm:ss");
                SizeF size = e.Graphics.MeasureString(t, fnt);
                RectangleF r = new RectangleF(0, 0, size.Width, size.Height); 
                e.Graphics.FillRectangle(Brushes.Black, r);
                e.Graphics.DrawString(t, fnt, Brushes.White, r.X, r.Y);
                
                t = streamInfo.dateCreation.ToString("yyyy.MM.dd, HH:mm:ss");
                size = e.Graphics.MeasureString(t, fnt);
                r = new RectangleF(
                    (sender as PictureBox).Width - size.Width - 2,
                    (sender as PictureBox).Height - size.Height - 2,
                    size.Width, size.Height);
                e.Graphics.FillRectangle(Brushes.Black, r);
                e.Graphics.DrawString(t, fnt, Brushes.White, new Point((int)r.X, (int)r.Y));
                if (streamInfo.dateDeletion != DateTime.MinValue)
                {
                    int y = (int)((sender as PictureBox).Height - (size.Height * 2) - 2);
                    t = "Будет удалён: " + streamInfo.dateDeletion.ToString("yyyy.MM.dd, HH:mm:ss");
                    size = e.Graphics.MeasureString(t, fnt);
                    r = new RectangleF((sender as PictureBox).Width - size.Width - 2, y, size.Width, size.Height);
                    e.Graphics.FillRectangle(Brushes.Black, r);
                    e.Graphics.DrawString(t, fnt, Brushes.Yellow, new Point((int)r.X, (int)r.Y));
                }
                fnt.Dispose();
            }
        }

        private void ImageStream_MouseDown(object sender, MouseEventArgs e)
        {
            if (OnActivate != null)
                OnActivate(this);
            if (OnImgMouseDown != null)
                OnImgMouseDown(this, e);
        }

        private void FrameStream_MouseDown(object sender, MouseEventArgs e)
        {
            if (OnActivate != null)
                OnActivate(this);
        }

        private void ImageGame_MouseDown(object sender, MouseEventArgs e)
        {
            if (OnActivate != null)
                OnActivate(this);
        }

        private void LabChannlName_MouseDown(object sender, MouseEventArgs e)
        {
            if (OnActivate != null)
                OnActivate(this);
        }

        private void LabStreamTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (OnActivate != null)
                OnActivate(this);
            if (e.Button == MouseButtons.Right)
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);

        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            DownloadButtonPress?.Invoke(this);
            
        }

        private void BtnDownload_MouseDown(object sender, MouseEventArgs e)
        {
            if (OnActivate != null)
                OnActivate(this);
        }

        private void BtnDownload_Paint(object sender, PaintEventArgs e)
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
            if (ShowStreamInfoGUI && fGuiInfoFontSize != newFontSize)
            {
                fGuiInfoFontSize = newFontSize;
                imageStream.Refresh();
            }
        }
        private void СкопироватьНаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetClipboardText(streamInfo.title);
        }

        private void CopyStreamDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetClipboardText(streamInfo.dateCreation.ToString("[yyyy-MM-dd] "));
        }

        private void CopyStreamTitlePlusDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetClipboardText(streamInfo.dateCreation.ToString("[yyyy-MM-dd] ") + streamInfo.title);
        }

        private void lblMutedChunks_DoubleClick(object sender, EventArgs e)
        {
            string t = $"Стрим: {streamInfo.title}{Environment.NewLine}Выпилен звук:{Environment.NewLine}";
            for (int i = 0; i < streamInfo.mutedChunks.segmentList.Count; i++)
            {
                t += streamInfo.mutedChunks.segmentList[i] + Environment.NewLine;
            }

            double percent = 100.0 / streamInfo.length.Ticks * streamInfo.mutedChunks.totalLength.Ticks;
            t += $"\nВсего выпилено: {streamInfo.mutedChunks.totalLength:HH:mm:ss} ({string.Format("{0:F2}", percent)}%)";
            if (MessageBox.Show($"{t}\n\nСкопировать это прямо в буфер?", "Определятор выпиленного звука",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SetClipboardText(t);
            }
        }
    }
}