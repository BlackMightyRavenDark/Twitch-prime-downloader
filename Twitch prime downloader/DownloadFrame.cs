using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MultiThreadedDownloaderLib;
using TwitchApiLib;
using static TwitchApiLib.TwitchVodChunk;
using static Twitch_prime_downloader.Utils;
using Twitch_prime_downloader.Properties;

namespace Twitch_prime_downloader
{
	public partial class DownloadFrame : UserControl
	{ 
		private DownloadAbstractor downloadAbstractor = null;
		public TwitchVod VodInfo { get; private set; }
		public string OutputDirectory { get; private set; }
		public string OutputFilePathOriginal { get; private set; }
		public string OutputFilePath { get; private set; }
		public int ChunkFrom { get => _chunkFrom; set { SetChunkFrom(value); } }
		public int ChunkTo { get => _chunkTo; set { SetChunkTo(value); } }
		public int ChunkGroupSize { get; private set; } = 3;
		public DownloadMode DownloadMode { get; private set; }
		public DateTime DownloadStarted { get; private set; }
		public TwitchVodPlaylist Playlist { get; }
		public int TotalChunkDownloadedCount { get; private set; }
		public long TotalByteDownloadedCount { get; private set; }
		public bool IsDownloading { get; private set; }
		public int TotalChunksCount => Playlist != null ? Playlist.Count : 0;

		private int _chunkFrom = 0;
		private int _chunkTo = 10;
		private string _fixedFileNameWithoutExt;

		public const int EXTRA_WIDTH = 450;
		private int fcstId = 0;
		private int oldX;

		public delegate void ClosedDelegate(object sender);
		public ClosedDelegate Closed;

		public DownloadFrame(TwitchVod vodInfo, TwitchVodPlaylist vodPlaylist)
		{
			InitializeComponent();

			Playlist = vodPlaylist;
			DownloadMode = radioButtonDownloadChunksSeparately.Checked ? DownloadMode.Chunked : DownloadMode.SingleFile;
			string t = DownloadMode == DownloadMode.SingleFile ? "файл" : "папка";
			toolTip1.SetToolTip(lblOutputFileName,
				$"Если {t} уже существует, будет использовано пронумерованное имя");
			OutputDirectory = config.DownloadDirectory ?? config.SelfDirectory;
			SetStreamInfo(vodInfo);

			lblProgressChunkGroup.Text = null;
			lblElapsedTime.Text = null;
			pictureBoxScrollBar.Top = Height - pictureBoxScrollBar.Height;
		}

		public void FrameDispose()
		{
			StopDownload();
		}

		private void downloadFrame_Resize(object sender, EventArgs e)
		{
			int panelWidth = Parent.Parent.Parent.Width - 24;
			btnCloseFrame.Location = new Point(panelWidth - btnCloseFrame.Width - 6, 2);
			pictureBoxVodThumbnailImage.Left = panelWidth - pictureBoxVodThumbnailImage.Width - 6;
			lblVodTitle.Width = pictureBoxVodThumbnailImage.Left - lblVodTitle.Left - 6;
			lblOutputFileName.Width = lblVodTitle.Width;
			multipleProgressBarChunkGroup.Left = lblVodTitle.Left;
			multipleProgressBarChunkGroup.Width = pictureBoxVodThumbnailImage.Left + pictureBoxVodThumbnailImage.Width - multipleProgressBarChunkGroup.Left;
			multipleProgressBarOverall.Left = multipleProgressBarChunkGroup.Left;
			multipleProgressBarOverall.Width = multipleProgressBarChunkGroup.Width;
			btnStopDownload.Left = panelWidth - btnStopDownload.Width - 6;
			btnStartDownload.Left = btnStopDownload.Left - btnStartDownload.Width - 6;

			pictureBoxScrollBar.Left = -Left;
			pictureBoxScrollBar.Width = panelWidth;

			groupBoxDownloadMode.Left = panelWidth + 10;
			groupBoxDownloadVodChunkRange.Left = groupBoxDownloadMode.Left;

			lblChunkFileList.Left = groupBoxDownloadVodChunkRange.Left + groupBoxDownloadVodChunkRange.Width + 10;
			listBoxChunkFileList.Left = btnCopyVodChunkUrlList.Left = lblChunkFileList.Left;

			int max = ChunkTo - ChunkFrom + 1;
			int animationPositionX = max > 0 ? TotalChunkDownloadedCount * (multipleProgressBarChunkGroup.Width - pictureBoxAnimation.Width) / max : 0;
			pictureBoxAnimation.Left = animationPositionX;
		}

		private void downloadFrame_Paint(object sender, PaintEventArgs e)
		{
			multipleProgressBarChunkGroup.Refresh();
			pictureBoxVodThumbnailImage.Refresh();
			pictureBoxScrollBar.Refresh();
		}

		private void downloadFrame_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				oldX = e.X;
			}
		}

		private void downloadFrame_MouseMove(object sender, MouseEventArgs e)
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
				pictureBoxScrollBar.Left = -Left;

				Refresh();
			}
		}

		private void pictureBoxVodThumbnailImage_Paint(object sender, PaintEventArgs e)
		{
			try
			{
				using (Font font = new Font("Arial", 12.0f))
				{
					if (VodInfo.Duration > TimeSpan.MinValue)
					{
						string t = VodInfo.Duration.ToString("h':'mm':'ss");
						SizeF sz = e.Graphics.MeasureString(t, font);
						e.Graphics.FillRectangle(Brushes.Black, new RectangleF(0.0f, 0.0f, sz.Width, sz.Height));
						e.Graphics.DrawString(t, font, Brushes.Lime, 0.0f, 0.0f);
					}
					if (VodInfo.IsSubscribersOnly)
					{
						SizeF sz = e.Graphics.MeasureString("$", font);
						float x = (sender as PictureBox).Width - sz.Width;
						e.Graphics.FillRectangle(Brushes.Black, new RectangleF(x, 0.0f, sz.Width, sz.Height));
						e.Graphics.DrawString("$", font, Brushes.Lime, x, 0.0f);
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

		private void pictureBoxScrollBar_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
			int xLeft = (int)Math.Round(pictureBoxScrollBar.Width / (double)Width * -Left);
			int xRight = (int)Math.Round(pictureBoxScrollBar.Width / (double)Width * (-Left + Parent.Width));
			Rectangle r = new Rectangle(xLeft, 0, xRight - xLeft, pictureBoxScrollBar.Height);
			e.Graphics.FillRectangle(Brushes.Black, r);
		}

		private void listBoxChunkFileList_DrawItem(object sender, DrawItemEventArgs e)
		{
			try
			{
				using (Bitmap bitmap = new Bitmap(e.Bounds.Width, e.Bounds.Height))
				{
					using (Graphics g = Graphics.FromImage(bitmap))
					{
						TwitchVodChunkItem chunkItem = listBoxChunkFileList.Items[e.Index] as TwitchVodChunkItem;
						TwitchVodChunkState chunkState = chunkItem.Chunk.GetState();
						Brush brush = chunkState == TwitchVodChunkState.NotMuted ? Brushes.Black : Brushes.Red;
						bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
						if (selected) { brush = chunkState == TwitchVodChunkState.NotMuted ? Brushes.White : Brushes.Gold; }
						g.FillRectangle(selected ? SystemBrushes.Highlight : Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
						g.DrawString(chunkItem.Chunk.FileName, listBoxChunkFileList.Font, brush, 0f, 0f);
						e.Graphics.DrawImage(bitmap, e.Bounds.X, e.Bounds.Y);
					}
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

		private void btnCloseFrame_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Закрыть фрейм?", "Быть или не быть?",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				FrameDispose();
				Closed?.Invoke(this);
				Dispose();
			}
		}

		private void btnCopyVodChunkUrlList_Click(object sender, EventArgs e)
		{
			if (Playlist != null && Playlist.Count > 0)
			{
				string t = Playlist.ToString();
				SetClipboardText(t);
			}
			else
			{
				MessageBox.Show("Ошибка!", "Ошибатор ошибок",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnStartDownload_Click(object sender, EventArgs e)
		{
			StartDownload();
		}

		private void btnStopDownload_Click(object sender, EventArgs e)
		{
			StopDownload();
		}

		private void btnSetMaxChunkTo_Click(object sender, EventArgs e)
		{
			ChunkTo = TotalChunksCount - 1;
		}

		private void lblVodTitle_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				contextMenuVodTitle.Show(Cursor.Position);
			}
		}

		private void miCopyVodTitleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetClipboardText(VodInfo.Title);
		}

		private void miDecreaseChunkGroupSizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ChunkGroupSize > 1)
			{
				ChunkGroupSize--;
				if (downloadAbstractor != null)
				{
					downloadAbstractor.MaxGroupSize = ChunkGroupSize;
				}
			}
		}

		private void miIncreaseChunkGroupSizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ChunkGroupSize < 10)
			{
				ChunkGroupSize++;
				if (downloadAbstractor != null)
				{
					downloadAbstractor.MaxGroupSize = ChunkGroupSize;
				}
			}
		}

		private void multipleProgressBarChunkGroup_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				contextMenuProgressBarChunkGroup.Show(Cursor.Position);
			}
		}

		private void radioButtonDownloadSingleBigVideoFile_CheckedChanged(object sender, EventArgs e)
		{
			DownloadMode = DownloadMode.SingleFile;
			toolTip1.SetToolTip(lblOutputFileName,
				"Если файл уже существует, будет использовано пронумерованное имя");
			DisplayOutputFilePathOrDirectory();
		}

		private void radioButtonDownloadChunksSeparately_CheckedChanged(object sender, EventArgs e)
		{
			DownloadMode = DownloadMode.Chunked;
			toolTip1.SetToolTip(lblOutputFileName,
				"Если папка уже существует, будет использовано пронумерованное имя");
			DisplayOutputFilePathOrDirectory();
		}

		private void textBoxChunkFrom_Leave(object sender, EventArgs e)
		{
			_chunkFrom = int.Parse(textBoxChunkFrom.Text) - 1;
			if (_chunkFrom < 0)
			{
				_chunkFrom = 0;
				textBoxChunkFrom.Text = "1";
			}
			else if (_chunkFrom >= TotalChunksCount)
			{
				_chunkFrom = TotalChunksCount - 1;
				textBoxChunkFrom.Text = (_chunkFrom + 1).ToString();
			}
			if (_chunkFrom > _chunkTo)
			{
				_chunkTo = _chunkFrom;
				textBoxChunkTo.Text = (_chunkTo + 1).ToString();
			}

			SetChunkCountIndicators();
		}

		private void textBoxChunkTo_Leave(object sender, EventArgs e)
		{
			_chunkTo = int.Parse(textBoxChunkTo.Text) - 1;
			if (_chunkTo < 0)
			{
				_chunkTo = 0;
				textBoxChunkTo.Text = "1";
			}
			if (_chunkTo >= TotalChunksCount)
			{
				_chunkTo = TotalChunksCount - 1;
				textBoxChunkTo.Text = (_chunkTo + 1).ToString();
			}
			else if (_chunkTo < _chunkFrom)
			{
				_chunkFrom = _chunkTo;
				textBoxChunkFrom.Text = (_chunkFrom + 1).ToString();
			}

			SetChunkCountIndicators();
		}

		private void timerElapsedTime_Tick(object sender, EventArgs e)
		{
			DateTime elapsedTime = new DateTime((DateTime.Now - DownloadStarted).Ticks);
			lblElapsedTime.Text = $"Прошло времени: {elapsedTime:H:mm:ss}";
		}

		private void timerAnimation_Tick(object sender, EventArgs e)
		{
			fcstId++;
			if (fcstId > 7) { fcstId = 0; }

			pictureBoxAnimation.Image = (Bitmap)Resources.ResourceManager.GetObject($"fcst_istra_0{fcstId + 1}");
		}

		private void OnChunkChanged(object sender, TwitchVodChunk chunk, int chunkId)
		{
			Invoke(new MethodInvoker(() =>
			{
				listBoxChunkFileList.Items[chunkId] = new TwitchVodChunkItem(chunk);
			}));
		}

		private void OnGroupDownloadStarted(object sender, IEnumerable<DownloadProgressItem> items)
		{
			Invoke(new MethodInvoker(() =>
			{
				lblProgressChunkGroup.Text = "Подготовка...";
				IEnumerable<MultipleProgressBarItem> progressItems = GetMultipleProgressBarItems(items);
				multipleProgressBarChunkGroup.SetItems(progressItems);
			}));
		}

		private void OnGroupDownloadProgressed(object sender, IEnumerable<DownloadProgressItem> items)
		{
			Invoke(new MethodInvoker(() =>
			{
				long chunksSummarySize = items.Select(item => item.ChunkSize).Sum();
				long downloaded = items.Select(item => item.DownloadedSize).Sum();

				if (chunksSummarySize > 0L)
				{
					double percent = 100.0 / chunksSummarySize * downloaded;
					string percentFormatted = string.Format("{0:F2}", percent);
					lblProgressChunkGroup.Text = $"Скачано: {FormatSize(downloaded)} / {FormatSize(chunksSummarySize)} ({percentFormatted}%)";
				}
				else
				{
					lblProgressChunkGroup.Text = "Подготовка...";
				}

				IEnumerable<MultipleProgressBarItem> progressItems = GetMultipleProgressBarItems(items);
				multipleProgressBarChunkGroup.SetItems(progressItems);
			}));
		}

		private void OnGroupDownloadFinished(object sender, IEnumerable<DownloadProgressItem> items, int errorCode)
		{
			Invoke(new MethodInvoker(() =>
			{
				TotalChunkDownloadedCount += items.Count();

				long chunksSummarySize = items.Select(item => item.ChunkSize).Sum();
				long downloaded = items.Select(item => item.DownloadedSize).Sum();
				TotalByteDownloadedCount += downloaded;
				lblProgressChunkGroup.Text = $"Скачано: {FormatSize(downloaded)} / {FormatSize(chunksSummarySize)}";
			}));
		}

		private void OnChunkMergingProgressed(object sender,
			long processedBytes, long totalSize,
			int chunkId, int chunkCount, DownloadMode downloadMode)
		{
			Invoke(new MethodInvoker(() =>
			{
				if (totalSize > 0L)
				{
					double percent = 100.0 / totalSize * processedBytes;
					string percentFormatted = string.Format("{0:F2}", percent);
					string progressText = $"Объединение чанков: {chunkId + 1} / {chunkCount} | " +
						$"{FormatSize(processedBytes)} / {FormatSize(totalSize)} ({percentFormatted}%)";

					lblProgressChunkGroup.Text = progressText;

					int percentRounded = (int)Math.Round(percent, 3);
					multipleProgressBarChunkGroup.SetItem(0, 100, percentRounded, progressText, Color.Lime);
				}
				else
				{
					string progressText = $"Объединение чанков: {chunkId + 1} / {chunkCount} | " +
						$"{FormatSize(processedBytes)} / <unknown>";
					lblProgressChunkGroup.Text = progressText;
					multipleProgressBarChunkGroup.SetItem(0, 100, 0, progressText, Color.Lime);
				}
			}));
		}

		private void OnGroupMergingFinished(object sender, IEnumerable<DownloadProgressItem> groupItems, int errorCode)
		{
			Invoke(new MethodInvoker(() =>
			{
				int chunkCountMax = ChunkTo - ChunkFrom + 1;
				double percent = 100.0 / chunkCountMax * TotalChunkDownloadedCount;
				string percentFormatted = string.Format("{0:F2}", percent);
				string progressText = $"Скачано чанков: {TotalChunkDownloadedCount} / {chunkCountMax}" +
					$" ({percentFormatted}%), Размер файла: {FormatSize(TotalByteDownloadedCount)}";

				multipleProgressBarOverall.SetItem(0, chunkCountMax, TotalChunkDownloadedCount, progressText, Color.Lime);

				int animationPositionX = chunkCountMax > 0 ? TotalChunkDownloadedCount * (multipleProgressBarChunkGroup.Width - pictureBoxAnimation.Width) / chunkCountMax : 0;
				pictureBoxAnimation.Left = animationPositionX;
			}));
		}

		private async void StartDownload()
		{
			if (IsDownloading)
			{
				btnStartDownload.Enabled = false;
				btnStopDownload.Enabled = true;
				return;
			}

			IsDownloading = true;
			btnStartDownload.Enabled = false;
			DownloadStarted = DateTime.Now;
			lblProgressChunkGroup.Text = "Подготовка к скачиванию...";
			lblElapsedTime.Text = "Прошло времени: 0:00:00";
			timerElapsedTime.Enabled = true;
			if (DownloadMode == DownloadMode.SingleFile)
			{
				OutputFilePath = MultiThreadedDownloaderLib.Utils.GetNumberedFileName(OutputFilePathOriginal + ".ts");
				lblOutputFileName.Text = "Имя файла: " + OutputFilePath;
			}
			else
			{
				OutputFilePath = GetNumberedDirectoryName(OutputFilePathOriginal);
				lblOutputFileName.Text = $"Папка для скачивания: {OutputFilePath}";
			}
			TotalChunkDownloadedCount = 0;
			TotalByteDownloadedCount = 0L;

			multipleProgressBarChunkGroup.ClearItems();
			int chunkCountMax = ChunkTo - ChunkFrom + 1;
			string progressText = $"Скачано чанков: 0 / {chunkCountMax} (0.00%), Размер файла: 0 bytes";
			multipleProgressBarOverall.SetItem(0, chunkCountMax, 0, progressText, Color.Lime);

			textBoxChunkFrom.Enabled = false;
			textBoxChunkTo.Enabled = false;
			btnSetMaxChunkTo.Enabled = false;
			radioButtonDownloadSingleBigVideoFile.Enabled = false;
			radioButtonDownloadChunksSeparately.Enabled = false;

			pictureBoxAnimation.Left = multipleProgressBarChunkGroup.Left;
			pictureBoxAnimation.Visible = true;
			timerAnimation.Enabled = true;

			int errorCode = await Task.Run(() =>
			{
				downloadAbstractor = new DownloadAbstractor(Playlist, ChunkGroupSize);
				return downloadAbstractor.Download(OutputFilePath,
					_chunkFrom, ChunkTo, DownloadMode, VodInfo.RawData,
					OnGroupDownloadStarted, OnGroupDownloadProgressed, OnGroupDownloadFinished,
					OnChunkMergingProgressed, OnGroupMergingFinished, OnChunkChanged, null);
			});
			downloadAbstractor = null;

			timerElapsedTime.Enabled = false;
			timerAnimation.Enabled = false;

			string msgCaption = VodInfo.IsSubscribersOnly ? "Скачиватор платного бесплатно" : "Скачивание";
			switch (errorCode)
			{
				case 200:
					MessageBox.Show($"{VodInfo.Title}\nСкачано успешно!", msgCaption,
						MessageBoxButtons.OK, MessageBoxIcon.Information);
					break;

				case FileDownloader.DOWNLOAD_ERROR_CANCELED:
					MessageBox.Show($"{VodInfo.Title}\nСкачивание успешно отменено!", msgCaption,
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
					break;

				case FileDownloader.DOWNLOAD_ERROR_DATA_SIZE_MISMATCH:
					MessageBox.Show($"{VodInfo.Title}\nОшибка DATA_SIZE_MISMATCH!\nСкачивание прервано!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				case MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS:
					MessageBox.Show($"{VodInfo.Title}\nОшибка объединения чанков!\nСкачивание прервано!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				case DownloadAbstractor.DOWNLOAD_ERROR_OUTPUT_DIR_NOT_EXISTS:
					MessageBox.Show($"{VodInfo.Title}\nПапка для скачивания не найдена!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				case FileDownloader.DOWNLOAD_ERROR_ZERO_LENGTH_CONTENT:
					MessageBox.Show($"{VodInfo.Title}\nФайл на сервере пуст!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				case DownloadAbstractor.DOWNLOAD_ERROR_GROUP_EMPTY:
					MessageBox.Show($"{VodInfo.Title}\nГруппа чанков пуста!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				case DownloadAbstractor.DOWNLOAD_ERROR_GROUP_SEQUENCE:
					MessageBox.Show($"{VodInfo.Title}\nНеправильная последовательность чанков!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				case DownloadAbstractor.DOWNLOAD_ERROR_CHUNK_RANGE:
					MessageBox.Show($"{VodInfo.Title}\nУказан неверный диапазон чанков!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				case DownloadAbstractor.DOWNLOAD_ERROR_CHUNK_BAD_STATUS_CODE:
					MessageBox.Show($"{VodInfo.Title}\nОдин из чанков скачался неудачно!\nСкачивание прервано!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				case DownloadAbstractor.DOWNLOAD_ERROR_EMPTY_CHUNK:
					MessageBox.Show($"{VodInfo.Title}\nОдин из скачанных чанков оказался пуст!\nСкачивание прервано!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				default:
					MessageBox.Show($"{VodInfo.Title}\nНеизвестная ошибка!" +
						$"\nСкачивание прервано!\nКод ошибки: {errorCode}", msgCaption,
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;
			}

			textBoxChunkFrom.Enabled = true;
			textBoxChunkTo.Enabled = true;
			btnSetMaxChunkTo.Enabled = true;
			radioButtonDownloadSingleBigVideoFile.Enabled = true;
			radioButtonDownloadChunksSeparately.Enabled = true;
			btnStartDownload.Enabled = true;

			IsDownloading = false;
		}

		public void StopDownload()
		{
			downloadAbstractor?.Stop();
		}

		public void SetStreamInfo(TwitchVod vod)
		{
			VodInfo = vod;
			lblVodTitle.Text = $"Стрим: {VodInfo.Title}";
			_fixedFileNameWithoutExt = FixFileName(FormatFileName(config.OutputFileNameFormat, VodInfo));
			DisplayOutputFilePathOrDirectory();

			pictureBoxVodThumbnailImage.Image =
				TryLoadImageFromStream(vod.ThumbnailImageData) ?? GenerateErrorImage();

			listBoxChunkFileList.Items.Clear();
			if (Playlist != null && Playlist.Count > 0)
			{
				for (int i = 0; i < Playlist.Count; ++i)
				{
					TwitchVodChunkItem item = new TwitchVodChunkItem(Playlist.GetChunk(i));
					listBoxChunkFileList.Items.Add(item);
				}
			}
		}

		private void SetChunkCountIndicators()
		{
			lblProgressOverall.Text = $"Всего чанков: {TotalChunksCount}, Скачивать: {ChunkTo - ChunkFrom + 1}";
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
					textBoxChunkTo.Text = (_chunkTo + 1).ToString();
				}
				textBoxChunkFrom.Text = (_chunkFrom + 1).ToString();

				SetChunkCountIndicators();
			}
		}

		private void SetChunkTo(int chunkId)
		{
			if (chunkId != _chunkTo)
			{
				_chunkTo = chunkId;
				textBoxChunkTo.Text = (_chunkTo + 1).ToString();
				if (_chunkTo < _chunkFrom)
				{
					_chunkFrom = _chunkTo;
					textBoxChunkFrom.Text = (_chunkFrom + 1).ToString();
				}

				SetChunkCountIndicators();
			}
		}

		private void DisplayOutputFilePathOrDirectory()
		{
			string fn = VodInfo.IsHighlight ? $"{_fixedFileNameWithoutExt} [highlight]" : _fixedFileNameWithoutExt;
			OutputFilePathOriginal = Path.Combine(OutputDirectory, fn);
			lblOutputFileName.Text = DownloadMode == DownloadMode.SingleFile ?
				$"Имя файла: {OutputFilePathOriginal}.ts" :
				$"Папка для скачивания: {OutputFilePathOriginal + "\\"}";
		}
	}
}
