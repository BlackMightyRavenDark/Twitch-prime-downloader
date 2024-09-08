using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchApiLib;
using static TwitchApiLib.Utils;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
	public partial class FrameStream : UserControl
	{
		public TwitchVod StreamInfo { get; private set; }
		public TwitchVodMutedSegments MutedSegments { get; private set; }
		public bool UseGmtTime
		{
			get { return _useGmtTime; }
			set { _useGmtTime = value; Refresh(); }
		}
		private int _hudInfoFontSize = 10;
		private bool _showStreamInfoHud = true;
		private bool _useGmtTime;
		public int HudInfoFontSize { get { return _hudInfoFontSize; } set { SetInfoGuiFontSize(value); } }
		public bool ShowStreamInfoHud { get { return _showStreamInfoHud; } set { SetShowInfoHud(value); } }

		public static readonly Color ColorActive = IntToColor(0x909090);
		public static readonly Color ColorInactive = IntToColor(0x303030);

		public delegate void ImageMouseDownDelegate(object sender, MouseEventArgs e);
		public delegate void FrameActivatedDelegate(object sender);
		public delegate void DownloadButtonClickedDelegate(object sender);
		public ImageMouseDownDelegate ImageMouseDown;
		public FrameActivatedDelegate Activated;
		public DownloadButtonClickedDelegate DownloadButtonClicked;
		
		public FrameStream(TwitchVod twitchVod)
		{
			InitializeComponent();

			_useGmtTime = config.UseGmtVodDates;

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

			SetStreamInfo(twitchVod);
		}

		private async void SetStreamInfo(TwitchVod vod)
		{
			StreamInfo = vod;
			lblStreamTitle.Text = StreamInfo.Title;
			lblChannelName.Text = StreamInfo.User.DisplayName;
			lblGameName.Text = StreamInfo.Game.Title;
			lblBroadcastType.Text = StreamInfo.VodType.ToString();
			lblPrime.Visible = StreamInfo.IsSubscribersOnly;

			TwitchVodMutedSegments mutedSegments = await Task.Run(() => vod.GetMutedSegments());
			MutedSegments = mutedSegments;
			if (MutedSegments != null && MutedSegments.Segments.Count > 0)
			{
				lblMutedChunks.Text = $"Muted segments: {MutedSegments.Segments.Count}";
				lblMutedChunks.Left = Width - lblMutedChunks.Width;
				lblMutedChunks.Visible = true;
			}
			else
			{
				lblMutedChunks.Visible = false;
			}

			Task[] tasks = new Task[]
			{
				Task.Run(() => StreamInfo.RetrievePreviewImage(1920, 1080)),
				Task.Run(() => StreamInfo.Game?.RetrievePreviewImage(52, 72))
			};
			await Task.WhenAll(tasks);

			Image imagePreview = TryLoadImageFromStream(StreamInfo.PreviewImageData);
			if (imagePreview == null) { imagePreview = GenerateErrorImage(); }
			imageStream.Image = imagePreview;
			imageGame.Image = TryLoadImageFromStream(StreamInfo.Game?.PreviewImageData);
		}

		private void imageStream_Paint(object sender, PaintEventArgs e)
		{
			if (ShowStreamInfoHud)
			{
				try
				{
					using (Font fnt = new Font("Lucida Console", HudInfoFontSize))
					{
						string durationFormatted = StreamInfo.Duration.ToString("h':'mm':'ss");
						SizeF size = e.Graphics.MeasureString(durationFormatted, fnt);
						RectangleF r = new RectangleF(0, 0, size.Width, size.Height);
						e.Graphics.FillRectangle(Brushes.Black, r);
						e.Graphics.DrawString(durationFormatted, fnt, Brushes.White, r.X, r.Y);

						DateTime creationDate = UseGmtTime ? StreamInfo.CreationDate : StreamInfo.CreationDate.ToLocal();
						string creationDateFormatted = creationDate.FormatDateTime();
						size = e.Graphics.MeasureString(creationDateFormatted, fnt);
						r = new RectangleF(
							(sender as PictureBox).Width - size.Width - 2,
							(sender as PictureBox).Height - size.Height - 2,
							size.Width, size.Height);
						e.Graphics.FillRectangle(Brushes.Black, r);
						e.Graphics.DrawString(creationDateFormatted, fnt, Brushes.White, new Point((int)r.X, (int)r.Y));
						if (StreamInfo.VodType == TwitchApi.TwitchVodType.Archive &&
							StreamInfo.DeletionDate < DateTime.MaxValue)
						{
							DateTime deletionDate = UseGmtTime ? StreamInfo.DeletionDate : StreamInfo.DeletionDate.ToLocal();
							string deletionDateString = $"Будет удалён: {deletionDate.FormatDateTime()}";
							int y = (int)((sender as PictureBox).Height - (size.Height * 2) - 2);
							size = e.Graphics.MeasureString(deletionDateString, fnt);
							r = new RectangleF((sender as PictureBox).Width - size.Width - 2, y, size.Width, size.Height);
							e.Graphics.FillRectangle(Brushes.Black, r);
							e.Graphics.DrawString(deletionDateString, fnt, Brushes.Yellow, new Point((int)r.X, (int)r.Y));
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
			DownloadButtonClicked?.Invoke(this);
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
				if (ShowStreamInfoHud)
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

		private void copyStreamTitleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetClipboardText(StreamInfo.Title);
		}

		private void copyStreamDateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetClipboardText(StreamInfo.CreationDate.ToString("[yyyy-MM-dd]"));
		}

		private void copyStreamTitlePlusDateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetClipboardText($"[{StreamInfo.CreationDate:yyyy-MM-dd}] {StreamInfo.Title}");
		}

		private void lblMutedChunks_DoubleClick(object sender, EventArgs e)
		{
			string t = $"Стрим: {StreamInfo.Title}{Environment.NewLine}Выпилен звук:{Environment.NewLine}{MutedSegments}";

			string durationFormatted = MutedSegments.TotalDuration.ToString("h':'mm':'ss");
			double percent = 100.0 / StreamInfo.Duration.Ticks * MutedSegments.TotalDuration.Ticks;
			string percentFormatted = string.Format("{0:F2}", percent);
			t += $"{Environment.NewLine}Всего выпилено: {durationFormatted} ({percentFormatted}%){Environment.NewLine}";
			if (MessageBox.Show($"{t}{Environment.NewLine}Скопировать это прямо в буфер?", "Определятор выпиленного звука",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				SetClipboardText(t);
			}
		}
	}
}
