using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitchApiLib;
using static TwitchApiLib.Utils;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
	public partial class VodFrame : UserControl
	{
		public TwitchVod Vod { get; private set; }
		public bool UseGmtTime
		{
			get => _useGmtTime;
			set
			{
				if (_useGmtTime != value)
				{
					_useGmtTime = value;
					Refresh();
				}
			}
		}
		public bool ShowVodInfoHud
		{
			get => _showVodInfoHud;
			set
			{
				if (_showVodInfoHud != value)
				{
					_showVodInfoHud = value;
					Refresh();
				}
			}
		}
		public int HudFontSize
		{
			get => _hudFontSize;
			set
			{
				if (_hudFontSize != value)
				{
					_hudFontSize = value;
					Refresh();
				}
			}
		}

		private bool _useGmtTime;
		private bool _showVodInfoHud = true;
		private int _hudFontSize = 10;

		public static readonly Color ColorActive = GetColorFromRGB(0x909090);
		public static readonly Color ColorInactive = GetColorFromRGB(0x303030);

		public delegate void ImageMouseDownDelegate(object sender, MouseEventArgs e);
		public delegate void FrameActivatedDelegate(object sender);
		public delegate void DownloadButtonClickedDelegate(object sender);
		public ImageMouseDownDelegate ImageMouseDown;
		public FrameActivatedDelegate Activated;
		public DownloadButtonClickedDelegate DownloadButtonClicked;
		
		public VodFrame(TwitchVod twitchVod)
		{
			InitializeComponent();

			lblGameName.Text = null;
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
#if DEBUG
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
			}
#else
			catch { }
#endif
			SetVod(twitchVod);
		}

		private async void SetVod(TwitchVod vod)
		{
			Vod = vod;
			lblVodTitle.Text = vod.Title;
			lblChannelName.Text = vod.User.DisplayName;
			lblBroadcastType.Text = vod.VodType.ToString();
			lblIsPrime.Visible = vod.IsSubscribersOnly;

			await Task.Run(() =>
			{
				Task[] tasks = new Task[]
				{
					Task.Run(() =>
					{
						if (vod.Playlist == null && vod.UpdatePlaylistManifest() == 200) { vod.Playlist.Parse(); }
					}),
					Task.Run(() => vod.ReceiveThumbnail(1920, 1080)),
					Task.Run(() =>
					{
						int errorCode = vod.UpdateGameInformation();
						if (errorCode == 200 || errorCode == 204)
						{
							vod.Game.ReceiveThumbnail(52, 72);
						}
					})
				};
				Task.WhenAll(tasks).Wait();
			});

			if (vod.Playlist != null &&
				vod.Playlist.UpdateMutedSegments(true) > 0 &&
				vod.Playlist.MutedSegments.Segments.Count > 0)
			{
				lblMutedChunks.Text = $"Muted segments: {vod.Playlist.MutedSegments.Segments.Count}";
				lblMutedChunks.Left = Width - lblMutedChunks.Width;
				lblMutedChunks.Visible = true;
			}
			else
			{
				lblMutedChunks.Visible = false;
			}

			Image imagePreview = TryLoadImageFromStream(vod.ThumbnailImageData) ?? GenerateErrorImage();
			pictureBoxThumbnailImageVod.Image = imagePreview;
			if (vod.Game != null)
			{
				if (vod.Game.IsKnown)
				{
					lblGameName.Text = vod.Game.Title;
					lblGameName.Visible = true;
				}
				else
				{
					lblGameName.Visible = false;
				}

				pictureBoxThumbnailImageGame.Image = TryLoadImageFromStream(vod.Game.ThumbnailImageData);
			}
			else
			{
				lblGameName.Text = null;
				Stream stream = null;
				if (await Task.Run(() => DownloadData(TwitchGame.UNKNOWN_GAME_BOXART_URL, out stream)) == 200 &&
					stream.Length > 0L)
				{
					pictureBoxThumbnailImageGame.Image = TryLoadImageFromStream(stream);
				}
			}
		}

		private void pictureBoxThumbnailImageVod_Paint(object sender, PaintEventArgs e)
		{
			if (ShowVodInfoHud)
			{
				try
				{
					using (Font fnt = new Font("Lucida Console", HudFontSize))
					{
						string durationFormatted = Vod.Duration.ToString("h':'mm':'ss");
						SizeF size = e.Graphics.MeasureString(durationFormatted, fnt);
						RectangleF r = new RectangleF(0, 0, size.Width, size.Height);
						e.Graphics.FillRectangle(Brushes.Black, r);
						e.Graphics.DrawString(durationFormatted, fnt, Brushes.White, r.X, r.Y);

						DateTime creationDate = UseGmtTime ? Vod.CreationDate : Vod.CreationDate.ToLocalTime();
						string creationDateFormatted = creationDate.FormatDateTime();
						size = e.Graphics.MeasureString(creationDateFormatted, fnt);
						r = new RectangleF(
							(sender as PictureBox).Width - size.Width - 2.0f,
							(sender as PictureBox).Height - size.Height - 2.0f,
							size.Width, size.Height);
						e.Graphics.FillRectangle(Brushes.Black, r);
						e.Graphics.DrawString(creationDateFormatted, fnt, Brushes.White, r.X, r.Y);
						if (Vod.VodType == TwitchApi.TwitchVodType.Archive &&
							Vod.DeletionDate < DateTime.MaxValue)
						{
							DateTime deletionDate = UseGmtTime ? Vod.DeletionDate : Vod.DeletionDate.ToLocalTime();
							string deletionDateString = $"Будет удалён: {deletionDate.FormatDateTime()}";
							int y = (int)((sender as PictureBox).Height - (size.Height * 2.0f) - 2.0f);
							size = e.Graphics.MeasureString(deletionDateString, fnt);
							r = new RectangleF((sender as PictureBox).Width - size.Width - 2.0f, y, size.Width, size.Height);
							e.Graphics.FillRectangle(Brushes.Black, r);
							e.Graphics.DrawString(deletionDateString, fnt, Brushes.Yellow, r.X, r.Y);
						}
					}
				}
#if DEBUG
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.StackTrace);
				}
#else
				catch { }
#endif
			}
		}

		private void pictureBoxThumbnailImageVod_MouseDown(object sender, MouseEventArgs e)
		{
			Activated?.Invoke(this);
			ImageMouseDown?.Invoke(this, e);
		}

		private void vodFrame_MouseDown(object sender, MouseEventArgs e)
		{
			Activated?.Invoke(this);
		}

		private void lblVodTitle_MouseDown(object sender, MouseEventArgs e)
		{
			Activated?.Invoke(this);
			if (e.Button == MouseButtons.Right)
			{
				contextMenuVodTitle.Show(Cursor.Position);
			}
		}

		private void btnDownload_Click(object sender, EventArgs e)
		{
			DownloadButtonClicked?.Invoke(this);
		}

		private void btnDownload_Paint(object sender, PaintEventArgs e)
		{
			try
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
					e.Graphics.DrawString(t, button.Font, brush, x, y);
					brush.Dispose();
				}
			}
#if DEBUG
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
#else
			catch { }
#endif
		}

		private void miCopyVodTitleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetClipboardText(Vod.Title);
		}

		private void miCopyVodCreationDateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetClipboardText(Vod.CreationDate.ToString("[yyyy-MM-dd]"));
		}

		private void miCopyVodTitlePlusCreationDateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetClipboardText($"[{Vod.CreationDate:yyyy-MM-dd}] {Vod.Title}");
		}

		private void lblMutedChunks_DoubleClick(object sender, EventArgs e)
		{
			string t = $"Стрим: {Vod.Title}{Environment.NewLine}Выпилен звук:{Environment.NewLine}{Vod.Playlist.MutedSegments}";
			string durationFormatted = Vod.Playlist.MutedSegments.TotalDuration.ToString("h':'mm':'ss");
			double percent = 100.0 / Vod.Duration.Ticks * Vod.Playlist.MutedSegments.TotalDuration.Ticks;
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
