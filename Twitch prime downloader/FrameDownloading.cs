using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MultiThreadedDownloaderLib;
using TwitchApiLib;
using static TwitchApiLib.TwitchVodChunk;
using static Twitch_prime_downloader.Utils;
using Twitch_prime_downloader.Properties;

namespace Twitch_prime_downloader
{
	public partial class FrameDownloading : UserControl
	{ 
		private DownloadAbstractor downloadAbstractor = null;
		public TwitchVod StreamInfo { get; private set; }
		public string OutputDirPath { get; private set; }
		public string OutputFilePathOrig { get; private set; }
		public string OutputFilePath { get; private set; }
		public int ChunkFrom { get { return _chunkFrom; } set { SetChunkFrom(value); } }
		public int ChunkTo { get { return _chunkTo; } set { SetChunkTo(value); } }
		public int ChunkGroupSize { get; private set; } = 3;
		public DownloadMode DownloadMode { get; private set; }
		public DateTime DownloadStarted { get; private set; }
		public TwitchPlaylist Playlist { get; }
		public int TotalChunkDownloadedCount { get; private set; }
		public long TotalByteDownloadedCount { get; private set; }
		public bool IsDownloading { get; private set; }
		public int TotalChunksCount => Playlist != null ? Playlist.Count : 0;

		private int _chunkFrom = 0;
		private int _chunkTo = 10;
		private string _fixedFileName;

		public const int EXTRA_WIDTH = 450;
		private int fcstId = 0;
		private int oldX;

		public delegate void ClosedDelegate(object sender);
		public ClosedDelegate Closed;

		public FrameDownloading(TwitchVod streamInfo, TwitchPlaylist vodPlaylist)
		{
			InitializeComponent();

			Playlist = vodPlaylist;
			OutputDirPath = config.DownloadingDirPath;
			SetStreamInfo(streamInfo);

			OnFrameCreate();
		}

		public void OnFrameCreate()
		{
			lblGroupProgress.Text = null;
			lblElapsedTime.Text = null;
			imgScrollBar.Top = Height - imgScrollBar.Height;

			lblOutputFilename.Text = DownloadMode == DownloadMode.WholeFile ?
				$"Имя файла: {OutputFilePathOrig}" : $"Папка для скачивания: {OutputFilePathOrig}";
		}

		public void FrameDispose()
		{
			StopDownload();
		}

		private void FrameDownload_Resize(object sender, EventArgs e)
		{
			int panelWidth = Parent.Parent.Parent.Width - 24;
			btnClose.Location = new Point(panelWidth - btnClose.Width - 6, 2);
			pictureBoxStreamImage.Left = panelWidth - pictureBoxStreamImage.Width - 6;
			lblStreamTitle.Width = pictureBoxStreamImage.Left - lblStreamTitle.Left - 6;
			lblOutputFilename.Width = lblStreamTitle.Width;
			multipleProgressBarGroup.Left = lblStreamTitle.Left;
			multipleProgressBarGroup.Width = pictureBoxStreamImage.Left + pictureBoxStreamImage.Width - multipleProgressBarGroup.Left;
			multipleProgressBarOverall.Left = multipleProgressBarGroup.Left;
			multipleProgressBarOverall.Width = multipleProgressBarGroup.Width;
			btnStopDownload.Left = panelWidth - btnStopDownload.Width - 6;
			btnStartDownload.Left = btnStopDownload.Left - btnStartDownload.Width - 6;

			imgScrollBar.Left = -Left;
			imgScrollBar.Width = panelWidth;

			grpDownloadOptions.Left = panelWidth + 10;
			grpDownloadRange.Left = grpDownloadOptions.Left;

			lblFilelist.Left = grpDownloadRange.Left + grpDownloadRange.Width + 10;
			lbFileList.Left = btnCopyUrlList.Left = lblFilelist.Left;

			int max = ChunkTo - ChunkFrom + 1;
			int animationPositionX = max > 0 ? TotalChunkDownloadedCount * (multipleProgressBarGroup.Width - imgFcst.Width) / max : 0;
			imgFcst.Left = animationPositionX;
		}

		private void FrameDownload_Paint(object sender, PaintEventArgs e)
		{
			multipleProgressBarGroup.Refresh();
			pictureBoxStreamImage.Refresh();
			imgScrollBar.Refresh();
		}

		private void PictureBoxStreamImage_Paint(object sender, PaintEventArgs e)
		{
			try
			{
				using (Font fnt = new Font("Arial", 12.0f))
				{
					if (StreamInfo.Duration > TimeSpan.MinValue)
					{
						string t = StreamInfo.Duration.ToString("h':'mm':'ss");
						SizeF sz = e.Graphics.MeasureString(t, fnt);
						e.Graphics.FillRectangle(Brushes.Black, new RectangleF(0.0f, 0.0f, sz.Width, sz.Height));
						e.Graphics.DrawString(t, fnt, Brushes.Lime, 0.0f, 0.0f);
					}
					if (StreamInfo.IsSubscribersOnly)
					{
						SizeF sz = e.Graphics.MeasureString("$", fnt);
						float x = (sender as PictureBox).Width - sz.Width;
						e.Graphics.FillRectangle(Brushes.Black, new RectangleF(x, 0.0f, sz.Width, sz.Height));
						e.Graphics.DrawString("$", fnt, Brushes.Lime, x, 0.0f);
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
			}
		}

		private void imgScrollBar_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
			int xLeft = (int)Math.Round(imgScrollBar.Width / (double)Width * -Left);
			int xRight = (int)Math.Round(imgScrollBar.Width / (double)Width * (-Left + Parent.Width));
			Rectangle r = new Rectangle(xLeft, 0, xRight - xLeft, imgScrollBar.Height);
			e.Graphics.FillRectangle(Brushes.Black, r);
		}

		private void lbFileList_DrawItem(object sender, DrawItemEventArgs e)
		{
			try
			{
				using (Bitmap bitmap = new Bitmap(e.Bounds.Width, e.Bounds.Height))
				{
					using (Graphics g = Graphics.FromImage(bitmap))
					{
						TwitchVodChunkItem chunkItem = lbFileList.Items[e.Index] as TwitchVodChunkItem;
						TwitchVodChunkState chunkState = chunkItem.Chunk.GetState();
						Brush brush = chunkState == TwitchVodChunkState.NotMuted ? Brushes.Black : Brushes.Red;
						bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
						if (selected) { brush = chunkState == TwitchVodChunkState.NotMuted ? Brushes.White : Brushes.Gold; }
						g.FillRectangle(selected ? SystemBrushes.Highlight : Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
						g.DrawString(chunkItem.Chunk.FileName, lbFileList.Font, brush, 0f, 0f);
						e.Graphics.DrawImage(bitmap, e.Bounds.X, e.Bounds.Y);
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
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

		private void btnCopyUrlList_Click(object sender, EventArgs e)
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

		private void multipleProgressBarGroup_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				contextMenuProgressBarGroup.Show(Cursor.Position);
			}
		}

		private void OnChunkChanged(object sender, TwitchVodChunk chunk, int chunkId)
		{
			Invoke(new MethodInvoker(() =>
			{
				lbFileList.Items[chunkId] = new TwitchVodChunkItem(chunk);
			}));
		}

		private void OnGroupDownloadProgressed(object sender, IEnumerable<DownloadItem> items)
		{
			Invoke(new MethodInvoker(() =>
			{
				long chunksSummarySize = items.Select(item => item.ChunkSize).Sum();
				long downloaded = items.Select(item => item.DownloadedSize).Sum();

				if (chunksSummarySize > 0L)
				{
					double percent = 100.0 / chunksSummarySize * downloaded;
					string percentFormatted = string.Format("{0:F2}", percent);
					lblGroupProgress.Text = $"Скачано: {FormatSize(downloaded)} / {FormatSize(chunksSummarySize)} ({percentFormatted}%)";
				}
				else
				{
					lblGroupProgress.Text = "Подключение...";
				}

				LinkedList<MultipleProgressBarItem> list = new LinkedList<MultipleProgressBarItem>();
				foreach (DownloadItem item in items)
				{
					double percent = 100.0 / item.ChunkSize * item.DownloadedSize;
					string percentFormatted = string.Format("{0:F2}", percent);

					string itemText;
					switch (item.State)
					{
						case DownloadItemState.Connecting:
							itemText = $"{item.VodChunk.FileName}: Connecting...";
							break;

						case DownloadItemState.Downloading:
						case DownloadItemState.Finished:
						case DownloadItemState.Errored:
							itemText = $"{item.VodChunk.FileName}: " +
								$"{FormatSize(item.DownloadedSize)} / {FormatSize(item.ChunkSize)} ({percentFormatted}%)";
							break;

						default:
							itemText = null;
							break;
					}

					int percentRounded = (int)Math.Round(percent, 3);
					MultipleProgressBarItem mpi = new MultipleProgressBarItem(
						0, 100, percentRounded, itemText, Color.Lime);
					list.AddLast(mpi);
				}
				multipleProgressBarGroup.SetItems(list);
			}));
		}

		private void OnGroupDownloadFinished(object sender, IEnumerable<DownloadItem> items, int errorCode)
		{
			Invoke(new MethodInvoker(() =>
			{
				TotalChunkDownloadedCount += items.Count();

				long chunksSummarySize = items.Select(item => item.ChunkSize).Sum();
				long downloaded = items.Select(item => item.DownloadedSize).Sum();
				TotalByteDownloadedCount += downloaded;
				lblGroupProgress.Text = $"Скачано: {FormatSize(downloaded)} / {FormatSize(chunksSummarySize)}";
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

					lblGroupProgress.Text = progressText;

					int percentRounded = (int)Math.Round(percent, 3);
					multipleProgressBarGroup.SetItem(0, 100, percentRounded, progressText, Color.Lime);
				}
				else
				{
					string progressText = $"Объединение чанков: {chunkId + 1} / {chunkCount} | " +
						$"{FormatSize(processedBytes)} / <unknown>";
					lblGroupProgress.Text = progressText;
					multipleProgressBarGroup.SetItem(0, 100, 0, progressText, Color.Lime);
				}
			}));
		}

		private void OnGroupMergingFinished(object sender, IEnumerable<DownloadItem> groupItems, int errorCode)
		{
			Invoke(new MethodInvoker(() =>
			{
				int chunkCountMax = ChunkTo - ChunkFrom + 1;
				double percent = 100.0 / chunkCountMax * TotalChunkDownloadedCount;
				string percentFormatted = string.Format("{0:F2}", percent);
				string progressText = $"Скачано чанков: {TotalChunkDownloadedCount} / {chunkCountMax}" +
					$" ({percentFormatted}%), Размер файла: {FormatSize(TotalByteDownloadedCount)}";

				multipleProgressBarOverall.SetItem(0, chunkCountMax, TotalChunkDownloadedCount, progressText, Color.Lime);

				int animationPositionX = chunkCountMax > 0 ? TotalChunkDownloadedCount * (multipleProgressBarGroup.Width - imgFcst.Width) / chunkCountMax : 0;
				imgFcst.Left = animationPositionX;
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
			lblElapsedTime.Text = "Прошло времени: 0:00:00";
			timerElapsed.Enabled = true;
			if (DownloadMode == DownloadMode.WholeFile)
			{
				OutputFilePath = MultiThreadedDownloader.GetNumberedFileName(OutputFilePathOrig);
				lblOutputFilename.Text = "Имя файла: " + OutputFilePath;
			}
			else
			{
				OutputFilePath = GetNumberedDirectoryName(OutputFilePathOrig);
				lblOutputFilename.Text = $"Папка для скачивания: {OutputFilePath}";
			}
			TotalChunkDownloadedCount = 0;
			TotalByteDownloadedCount = 0L;

			multipleProgressBarGroup.ClearItems();
			int chunkCountMax = ChunkTo - ChunkFrom + 1;
			string progressText = $"Скачано чанков: 0 / {chunkCountMax} (0.00%), Размер файла: 0 bytes";
			multipleProgressBarOverall.SetItem(0, chunkCountMax, 0, progressText, Color.Lime);

			editFrom.Enabled = false;
			editTo.Enabled = false;
			btnSetMaxChunkTo.Enabled = false;
			rbDownloadOneBigFile.Enabled = false;
			rbDownloadChunksSeparately.Enabled = false;

			imgFcst.Left = multipleProgressBarGroup.Left;
			imgFcst.Visible = true;
			timerFcst.Enabled = true;

			downloadAbstractor = new DownloadAbstractor(Playlist, ChunkGroupSize);
			int errorCode = await downloadAbstractor.Download(OutputFilePath,
				_chunkFrom, ChunkTo, DownloadMode,
				null, OnGroupDownloadProgressed, OnGroupDownloadFinished,
				OnChunkMergingProgressed, OnGroupMergingFinished, OnChunkChanged, null);
			downloadAbstractor = null;

			timerElapsed.Enabled = false;
			timerFcst.Enabled = false;

			string msgCaption = StreamInfo.IsSubscribersOnly ? "Скачиватор платного бесплатно" : "Скачивание";
			switch (errorCode)
			{
				case 200:
					{
						if (config.SaveVodInfo && !string.IsNullOrEmpty(StreamInfo.RawData))
						{
							string infoFp;
							if (DownloadMode == DownloadMode.WholeFile)
							{
								string fn = Path.GetFileNameWithoutExtension(OutputFilePath);
								infoFp = Path.Combine(OutputDirPath, $"{fn}_info.json");
							}
							else
							{
								infoFp = Path.Combine(OutputFilePath, "_info.json");
							}
							File.WriteAllText(infoFp, StreamInfo.RawData);
						}

						string msg = $"{StreamInfo.Title}\nСкачано успешно!";
						MessageBox.Show(msg, msgCaption,
							MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					break;

				case FileDownloader.DOWNLOAD_ERROR_CANCELED_BY_USER:
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

				case MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_DIR_NOT_EXISTS:
					MessageBox.Show($"{StreamInfo.Title}\nПапка для скачивания не найдена!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				case FileDownloader.DOWNLOAD_ERROR_ZERO_LENGTH_CONTENT:
					MessageBox.Show($"{StreamInfo.Title}\nФайл на сервере пуст!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				case DownloadAbstractor.DOWNLOAD_ERROR_GROUP_EMPTY:
					MessageBox.Show($"{StreamInfo.Title}\nГруппа чанков пуста!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				case DownloadAbstractor.DOWNLOAD_ERROR_GROUP_SEQUENCE:
					MessageBox.Show($"{StreamInfo.Title}\nНеправильная последовательность чанков!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				case DownloadAbstractor.DOWNLOAD_ERROR_CHUNK_RANGE:
					MessageBox.Show($"{StreamInfo.Title}\nУказан неверный диапазон чанков!",
						msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;

				default:
					MessageBox.Show($"{StreamInfo.Title}\nНеизвестная ошибка!" +
						$"\nСкачивание прервано!\nКод ошибки: {errorCode}", msgCaption,
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					break;
			}

			editFrom.Enabled = true;
			editTo.Enabled = true;
			btnSetMaxChunkTo.Enabled = true;
			rbDownloadOneBigFile.Enabled = true;
			rbDownloadChunksSeparately.Enabled = true;
			btnStartDownload.Enabled = true;

			IsDownloading = false;
		}

		public void StopDownload()
		{
			downloadAbstractor?.Stop();
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
			_fixedFileName = FixFileName(FormatFileName(config.FileNameFormat, StreamInfo));
			if (rbDownloadOneBigFile.Checked)
			{
				DownloadMode = DownloadMode.WholeFile;
				OutputFilePathOrig = Path.Combine(OutputDirPath,
					 StreamInfo.IsHighlight ? $"{_fixedFileName} [highlight].ts" : $"{_fixedFileName}.ts");
				lblOutputFilename.Text = $"Имя файла: {OutputFilePathOrig}";
			}
			else
			{
				DownloadMode = DownloadMode.Chunked;
				OutputFilePathOrig = Path.Combine(OutputDirPath, _fixedFileName);
				lblOutputFilename.Text = $"Папка для скачивания: {OutputFilePathOrig}";
			}

			Image image = TryLoadImageFromStream(vod.PreviewImageData);
			if (image == null) { image = GenerateErrorImage(); }
			pictureBoxStreamImage.Image = image;

			lbFileList.Items.Clear();
			if (Playlist != null && Playlist.Count > 0)
			{
				for (int i = 0; i < Playlist.Count; ++i)
				{
					TwitchVodChunkItem item = new TwitchVodChunkItem(Playlist.GetChunk(i));
					lbFileList.Items.Add(item);
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
					editTo.Text = (_chunkTo + 1).ToString();
				}
				editFrom.Text = (_chunkFrom + 1).ToString();

				SetChunkCountIndicators();
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

				SetChunkCountIndicators();
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

			SetChunkCountIndicators();
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

			SetChunkCountIndicators();
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

			imgFcst.Image = (Bitmap)Resources.ResourceManager.GetObject($"fcst_istra_0{fcstId + 1}");
		}

		private void rbDownloadOneBigFile_CheckedChanged(object sender, EventArgs e)
		{
			DownloadMode = DownloadMode.WholeFile;
			string fn = StreamInfo.IsHighlight ? $"{_fixedFileName} [highlight].ts" : $"{_fixedFileName}.ts";
			OutputFilePathOrig = Path.Combine(OutputDirPath, fn);
			lblOutputFilename.Text = $"Имя файла: {OutputFilePathOrig}";
		}

		private void rbDownloadChunksSeparately_CheckedChanged(object sender, EventArgs e)
		{
			DownloadMode = DownloadMode.Chunked;
			string fn = StreamInfo.IsHighlight ? $"{_fixedFileName} [highlight]" : _fixedFileName;
			OutputFilePathOrig = Path.Combine(OutputDirPath, fn + "\\");
			lblOutputFilename.Text = $"Папка для скачивания: {OutputFilePathOrig}";
		}

		private void miIncreaseGroupSizeToolStripMenuItem_Click(object sender, EventArgs e)
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

		private void miDecreaseGroupSizeToolStripMenuItem_Click(object sender, EventArgs e)
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
	}
}
