using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
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
		public TwitchVod Vod { get; private set; }
		public TwitchPlaylist Playlist { get; }
		public int ChunkCountInPlaylist => Playlist != null ? Playlist.Count : 0;
		public int ChunkRangeFirstId { get => _chunkRangeFirstId; set { SetFirstDownloadableChunkId(value); } }
		public int ChunkRangeLastId { get => _chunkRangeLastId; set { SetLastDownloadableChunkId(value); } }
		public int ChunkGroupSize { get; private set; } = 3;
		public DownloadMode DownloadMode { get; private set; }
		public string OutputDirectory { get; private set; }
		public string OutputFilePathOriginal { get; private set; }
		public string OutputFilePath { get; private set; }
		public long OutputFileSize { get; private set; }
		public DateTime DownloadStarted { get; private set; }
		public int DownloadedChunkCount { get; private set; }
		public bool IsDownloading { get; private set; }

		private DownloadAbstractor _downloadAbstractor;
		private int _chunkRangeFirstId = 0;
		private int _chunkRangeLastId = 10;
		private string _fixedFileNameWithoutExt;
		private bool _isAborted = false;

		public const int EXTRA_WIDTH = 450;
		private int _fcstId = 0;

		public delegate void ClosedDelegate(object sender);
		public ClosedDelegate Closed;

		public DownloadFrame(TwitchVod vod, TwitchPlaylist playlist)
		{
			InitializeComponent();

			Playlist = playlist;
			DownloadMode = radioButtonDownloadChunksSeparately.Checked ? DownloadMode.Chunked : DownloadMode.SingleFile;
			string t = DownloadMode == DownloadMode.SingleFile ? "файл" : "папка";
			toolTip1.SetToolTip(lblOutputFileName,
				$"Если {t} уже существует, будет использовано пронумерованное имя");
			OutputDirectory = config.DownloadDirectory;
			SetStreamInfo(vod);

			lblProgressChunkGroup.Text = null;
			lblElapsedTime.Text = null;
			pictureBoxScrollBar.Top = Height - pictureBoxScrollBar.Height;
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

			int max = ChunkRangeLastId - ChunkRangeFirstId + 1;
			int animationPositionX = max > 0 ? DownloadedChunkCount * (multipleProgressBarChunkGroup.Width - pictureBoxAnimation.Width) / max : 0;
			pictureBoxAnimation.Left = animationPositionX;
		}

		private void downloadFrame_Paint(object sender, PaintEventArgs e)
		{
			multipleProgressBarChunkGroup.Refresh();
			pictureBoxVodThumbnailImage.Refresh();
			pictureBoxScrollBar.Refresh();
		}

		#region Dragging this frame
		private bool _canDrag = false;
		private int _oldX;

		private void downloadFrame_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				_oldX = e.X;
				_canDrag = true;
			}
		}

		private void DownloadFrame_MouseUp(object sender, MouseEventArgs e)
		{
			_canDrag = false;
		}

		private void downloadFrame_MouseMove(object sender, MouseEventArgs e)
		{
			if (_canDrag)
			{
				int newX = Left + e.X - _oldX;
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
		#endregion

		private void pictureBoxVodThumbnailImage_Paint(object sender, PaintEventArgs e)
		{
			try
			{
				using (Font font = new Font("Arial", 12.0f))
				{
					if (Vod.Duration > TimeSpan.MinValue)
					{
						string t = Vod.Duration.ToString("h':'mm':'ss");
						SizeF sz = e.Graphics.MeasureString(t, font);
						e.Graphics.FillRectangle(Brushes.Black, new RectangleF(0.0f, 0.0f, sz.Width, sz.Height));
						e.Graphics.DrawString(t, font, Brushes.Lime, 0.0f, 0.0f);
					}

					if (Vod.IsSubscribersOnly)
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

		private void pictureBoxVodThumbnailImage_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				contextMenuThumbnail.Show(Cursor.Position);
			}
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
						Brush brush = chunkState == TwitchVodChunkState.Normal ? Brushes.Black : Brushes.Red;
						bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
						if (selected) { brush = chunkState == TwitchVodChunkState.Normal ? Brushes.White : Brushes.Gold; }
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

		private async void btnCloseFrame_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Закрыть фрейм?", "Быть или не быть?",
				MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				_isAborted = true;
				EnableControls(false);

				if (IsDownloading)
				{
					StopDownload();
					await Task.Run(() =>
					{
						do { Thread.Sleep(200); } while (IsDownloading);
					});
				}

				Closed?.Invoke(this);
				Dispose();
			}
		}

		private void btnCopyVodChunkUrlList_Click(object sender, EventArgs e)
		{
			if (Playlist.Count > 0)
			{
				string urls = Playlist.GetChunkUrlList();
				SetClipboardText(urls);
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
			btnStopDownload.Enabled = false;
			if (IsDownloading && !_isAborted &&
				MessageBox.Show("Остановить скачивание?", "Отменятор отменения отмены",
					MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				StopDownload();
			}
			if (!_isAborted) { btnStopDownload.Enabled = true; }
		}

		private void btnSetMaxChunkTo_Click(object sender, EventArgs e)
		{
			ChunkRangeLastId = ChunkCountInPlaylist - 1;
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
			SetClipboardText(Vod.Title);
		}

		private void miVodSaveChunkListToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if (IsDownloading)
				{
					MessageBox.Show("Невозможно сохранить список чанков во время скачивания!", "Ошибка!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (_downloadAbstractor != null && _downloadAbstractor.DownloadMode == DownloadMode.Chunked)
				{
					MessageBox.Show("Сохранить список чанков можно только в режиме скачивания в целый файл!", "Внимание!",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}

				if (_downloadAbstractor?.SerializedChunkList == null)
				{
					MessageBox.Show("Список чанков не существует!", "Ошибка!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (_downloadAbstractor.SerializedChunkList.Count == 0)
				{
					MessageBox.Show("Список чанков пуст!", "Ошибка!",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				using (SaveFileDialog sfd = new SaveFileDialog()
				{
					Title = "Куда будем сохранять список чанков?",
					Filter = "JSON-files|*.json",
					DefaultExt = ".json",
					FileName = Path.GetFileName(OutputFilePath) + "_chunks.json"
				})
				{
					if (!string.IsNullOrWhiteSpace(config.LastUsedDirectory) &&
						Directory.Exists(config.LastUsedDirectory))
					{
						sfd.InitialDirectory = config.LastUsedDirectory;
					}

					if (sfd.ShowDialog() == DialogResult.OK)
					{
						config.LastUsedDirectory = Path.GetDirectoryName(sfd.FileName);
						File.WriteAllText(sfd.FileName, _downloadAbstractor.SerializedChunkList.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void miDecreaseChunkGroupSizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ChunkGroupSize > 1)
			{
				ChunkGroupSize--;
				if (_downloadAbstractor != null)
				{
					_downloadAbstractor.SimultaneousDownloadChunkChunkCount = ChunkGroupSize;
				}
			}
		}

		private void miIncreaseChunkGroupSizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ChunkGroupSize < 10)
			{
				ChunkGroupSize++;
				if (_downloadAbstractor != null)
				{
					_downloadAbstractor.SimultaneousDownloadChunkChunkCount = ChunkGroupSize;
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

		private void textBoxChunkRangeFirstId_Leave(object sender, EventArgs e)
		{
			try
			{
				_chunkRangeFirstId = int.Parse(textBoxChunkRangeFirstId.Text) - 1;
				if (_chunkRangeFirstId < 0)
				{
					_chunkRangeFirstId = 0;
					textBoxChunkRangeFirstId.Text = "1";
				}
				else if (_chunkRangeFirstId >= ChunkCountInPlaylist)
				{
					_chunkRangeFirstId = ChunkCountInPlaylist - 1;
					textBoxChunkRangeFirstId.Text = (_chunkRangeFirstId + 1).ToString();
				}
				if (_chunkRangeFirstId > _chunkRangeLastId)
				{
					_chunkRangeLastId = _chunkRangeFirstId;
					textBoxChunkRangeLastId.Text = (_chunkRangeLastId + 1).ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				_chunkRangeFirstId = 0;
				textBoxChunkRangeFirstId.Text = "1";
				_chunkRangeLastId = ChunkCountInPlaylist - 1;
				textBoxChunkRangeLastId.Text = ChunkCountInPlaylist.ToString();
			}

			SetChunkCountIndicators();
		}

		private void textBoxChunkRangeLastId_Leave(object sender, EventArgs e)
		{
			try
			{
				_chunkRangeLastId = int.Parse(textBoxChunkRangeLastId.Text) - 1;
				if (_chunkRangeLastId < 0)
				{
					_chunkRangeLastId = 0;
					textBoxChunkRangeLastId.Text = "1";
				}
				if (_chunkRangeLastId >= ChunkCountInPlaylist)
				{
					_chunkRangeLastId = ChunkCountInPlaylist - 1;
					textBoxChunkRangeLastId.Text = (_chunkRangeLastId + 1).ToString();
				}
				else if (_chunkRangeLastId < _chunkRangeFirstId)
				{
					_chunkRangeFirstId = _chunkRangeLastId;
					textBoxChunkRangeFirstId.Text = (_chunkRangeFirstId + 1).ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				_chunkRangeFirstId = 0;
				textBoxChunkRangeFirstId.Text = "1";
				_chunkRangeLastId = ChunkCountInPlaylist - 1;
				textBoxChunkRangeLastId.Text = ChunkCountInPlaylist.ToString();
			}

			SetChunkCountIndicators();
		}

		private void timerElapsedTime_Tick(object sender, EventArgs e)
		{
			DateTime elapsedTime = new DateTime((DateTime.UtcNow - DownloadStarted).Ticks);
			lblElapsedTime.Text = $"Прошло времени: {elapsedTime:H:mm:ss}";
		}

		private void timerAnimation_Tick(object sender, EventArgs e)
		{
			_fcstId++;
			if (_fcstId > 7) { _fcstId = 0; }

			pictureBoxAnimation.Image = (Bitmap)Resources.ResourceManager.GetObject($"fcst_istra_0{_fcstId + 1}");
		}

		private void OnChunkStateChanged(object sender, TwitchVodChunk chunk)
		{
			Invoke(new MethodInvoker(() =>
			{
				listBoxChunkFileList.Items[chunk.Id] = new TwitchVodChunkItem(chunk);
			}));
		}

		private void OnChunkAppended(object sender, long outputFileSize)
		{
			Invoke(new MethodInvoker(() =>
			{
				DownloadedChunkCount++;
				OutputFileSize = outputFileSize;
				UpdateOverallProgressBarAndAnimationPosition();
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

				if (chunksSummarySize > 0L)
				{
					long downloaded = items.Select(item => item.DownloadedSize).Sum();
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

		private void UpdateOverallProgressBarAndAnimationPosition()
		{
			int chunkCount = ChunkRangeLastId - ChunkRangeFirstId + 1;
			double percent = 100.0 / chunkCount * DownloadedChunkCount;
			string percentFormatted = string.Format("{0:F2}", percent);
			string progressText = $"Скачано чанков: {DownloadedChunkCount} / {chunkCount}" +
				$" ({percentFormatted}%), Размер файла: {FormatSize(OutputFileSize)}";

			multipleProgressBarOverall.SetItem(0, chunkCount, DownloadedChunkCount, progressText, Color.Lime);

			int animationPositionX = chunkCount > 0 ? (DownloadedChunkCount * (multipleProgressBarChunkGroup.Width - pictureBoxAnimation.Width) / chunkCount) : 0;
			pictureBoxAnimation.Left = animationPositionX;
		}

		private async void StartDownload()
		{
			if (_isAborted) { return; }
			if (IsDownloading)
			{
				btnStartDownload.Enabled = false;
				btnStopDownload.Enabled = true;
				return;
			}

			IsDownloading = true;
			DownloadStarted = DateTime.UtcNow;
			lblElapsedTime.Text = "Прошло времени: 0:00:00";
			if (DownloadMode == DownloadMode.SingleFile)
			{
				OutputFilePath = MultiThreadedDownloaderLib.Utils.GetNumberedFileName(OutputFilePathOriginal + Playlist.StreamFileExtension);
				if (string.IsNullOrWhiteSpace(OutputFilePath))
				{
					const string msg = "Ошибка нумерования файла!";
					lblProgressChunkGroup.Text = msg;
					MessageBox.Show(msg, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					btnStartDownload.Enabled = true;
					IsDownloading = false;
					return;
				}
				lblOutputFileName.Text = $"Имя файла: {OutputFilePath}";
			}
			else
			{
				OutputFilePath = GetNumberedDirectoryName(OutputFilePathOriginal, out string errorMessage);
				if (!string.IsNullOrWhiteSpace(errorMessage))
				{
					lblProgressChunkGroup.Text = errorMessage;
					MessageBox.Show(errorMessage, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					btnStartDownload.Enabled = true;
					IsDownloading = false;
					return;
				}
				lblOutputFileName.Text = $"Папка для скачивания: {OutputFilePath}";
			}

			lblProgressChunkGroup.Text = "Подготовка к скачиванию...";
			btnStartDownload.Enabled = false;
			btnStopDownload.Enabled = true;
			DownloadedChunkCount = 0;
			OutputFileSize = 0L;
			timerElapsedTime.Enabled = true;

			multipleProgressBarChunkGroup.ClearItems();
			int chunkCountMax = ChunkRangeLastId - ChunkRangeFirstId + 1;
			string progressText = $"Скачано чанков: 0 / {chunkCountMax} (0.00%), Размер файла: 0 bytes";
			multipleProgressBarOverall.SetItem(0, chunkCountMax, 0, progressText, Color.Lime);

			textBoxChunkRangeFirstId.Enabled = false;
			textBoxChunkRangeLastId.Enabled = false;
			btnSetMaxChunkTo.Enabled = false;
			radioButtonDownloadSingleBigVideoFile.Enabled = false;
			radioButtonDownloadChunksSeparately.Enabled = false;

			pictureBoxAnimation.Left = multipleProgressBarChunkGroup.Left;
			pictureBoxAnimation.Visible = true;
			timerAnimation.Enabled = true;

			int errorCode = await Task.Run(() =>
			{
				_downloadAbstractor = new DownloadAbstractor(Playlist, DownloadMode, ChunkGroupSize);
				return _downloadAbstractor.Download(OutputFilePath,
					_chunkRangeFirstId, ChunkRangeLastId, config.SaveVodChunkInfo, config.StoreVodSubChunksInfo, Vod.RawData,
					OnGroupDownloadStarted, OnGroupDownloadProgressed, null,
					OnChunkMergingProgressed, null, OnChunkStateChanged, OnChunkAppended, null);
			});

			timerElapsedTime.Enabled =
			timerAnimation.Enabled = false;
			if (!_isAborted)
			{
				string msgCaption = Vod.IsSubscribersOnly ? "Скачиватор платного бесплатно" : "Скачивание";
				switch (errorCode)
				{
					case 200:
						MessageBox.Show($"{Vod.Title}\nСкачано успешно!", msgCaption,
							MessageBoxButtons.OK, MessageBoxIcon.Information);
						break;

					case FileDownloader.DOWNLOAD_ERROR_CANCELED:
						MessageBox.Show($"{Vod.Title}\nСкачивание успешно отменено!", msgCaption,
							MessageBoxButtons.OK, MessageBoxIcon.Warning);
						break;

					case FileDownloader.DOWNLOAD_ERROR_DATA_SIZE_MISMATCH:
						MessageBox.Show($"{Vod.Title}\nОшибка DATA_SIZE_MISMATCH!\nСкачивание прервано!",
							msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;

					case MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS:
						MessageBox.Show($"{Vod.Title}\nОшибка объединения чанков!\nСкачивание прервано!",
							msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;

					case DownloadAbstractor.DOWNLOAD_ERROR_OUTPUT_DIR_NOT_EXISTS:
						MessageBox.Show($"{Vod.Title}\nПапка для скачивания не найдена!",
							msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;

					case FileDownloader.DOWNLOAD_ERROR_ZERO_LENGTH_CONTENT:
						MessageBox.Show($"{Vod.Title}\nФайл на сервере пуст!",
							msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;

					case DownloadAbstractor.DOWNLOAD_ERROR_GROUP_EMPTY:
						MessageBox.Show($"{Vod.Title}\nГруппа чанков пуста!",
							msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;

					case DownloadAbstractor.DOWNLOAD_ERROR_GROUP_SEQUENCE:
						MessageBox.Show($"{Vod.Title}\nНеправильная последовательность чанков!",
							msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;

					case DownloadAbstractor.DOWNLOAD_ERROR_CHUNK_RANGE:
						MessageBox.Show($"{Vod.Title}\nУказан неверный диапазон чанков!",
							msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;

					case DownloadAbstractor.DOWNLOAD_ERROR_CHUNK_BAD_STATUS_CODE:
						MessageBox.Show($"{Vod.Title}\nОдин из чанков скачался неудачно!\nСкачивание прервано!",
							msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;

					case DownloadAbstractor.DOWNLOAD_ERROR_EMPTY_CHUNK:
						MessageBox.Show($"{Vod.Title}\nОдин из скачанных чанков оказался пуст!\nСкачивание прервано!",
							msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;

					case DownloadAbstractor.DOWNLOAD_ERROR_CHUNK_SIZE_MISMATCH:
						MessageBox.Show($"{Vod.Title}\nОдин из скачанных чанков не соответствует размеру, который указан на сервере! " +
							"Вероятно, во время скачивания произошла ошибка!\nСкачивание прервано!",
							msgCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;

					default:
						MessageBox.Show($"{Vod.Title}\nНеизвестная ошибка!" +
							$"\nСкачивание прервано!\nКод ошибки: {errorCode}", msgCaption,
							MessageBoxButtons.OK, MessageBoxIcon.Error);
						break;
				}

				textBoxChunkRangeFirstId.Enabled = true;
				textBoxChunkRangeLastId.Enabled = true;
				btnSetMaxChunkTo.Enabled = true;
				radioButtonDownloadSingleBigVideoFile.Enabled = true;
				radioButtonDownloadChunksSeparately.Enabled = true;
				btnStartDownload.Enabled = true;
			}

			IsDownloading = false;
		}

		public void StopDownload()
		{
			if (IsDownloading && _downloadAbstractor != null)
			{
				_downloadAbstractor.Stop();
			}
		}

		public void SetStreamInfo(TwitchVod vod)
		{
			Vod = vod;
			lblVodTitle.Text = $"Стрим: {Vod.Title}";
			_fixedFileNameWithoutExt = FixFileName(FormatFileName(config.OutputFileNameFormat, Vod));
			DisplayOutputFilePathOrDirectory();

			pictureBoxVodThumbnailImage.Image =
				TryLoadImageFromStream(vod.ThumbnailImageData) ?? GenerateErrorImage();

			listBoxChunkFileList.Items.Clear();
			int count = Playlist.Count;
			if (count > 0)
			{
				if (Playlist.StreamHeaderChunk != null)
				{
					listBoxChunkFileList.Items.Add(new TwitchVodChunkItem(Playlist.StreamHeaderChunk));
				}

				for (int i = 0; i < count; ++i)
				{
					TwitchVodChunkItem item = new TwitchVodChunkItem(Playlist[i]);
					listBoxChunkFileList.Items.Add(item);
				}
			}
		}

		private void SetChunkCountIndicators()
		{
			lblProgressOverall.Text = $"Всего чанков: {ChunkCountInPlaylist}, Скачивать: {ChunkRangeLastId - ChunkRangeFirstId + 1}";
		}

		private void SetFirstDownloadableChunkId(int chunkId)
		{
			if (chunkId < 0)
			{
				chunkId = 0;
			}
			else if (chunkId >= ChunkCountInPlaylist)
			{
				chunkId = ChunkCountInPlaylist - 1;
			}

			_chunkRangeFirstId = chunkId;
			if (_chunkRangeLastId < _chunkRangeFirstId)
			{
				_chunkRangeLastId = _chunkRangeFirstId;
				textBoxChunkRangeLastId.Text = (_chunkRangeLastId + 1).ToString();
			}
			textBoxChunkRangeFirstId.Text = (_chunkRangeFirstId + 1).ToString();

			SetChunkCountIndicators();
		}

		private void SetLastDownloadableChunkId(int chunkId)
		{
			_chunkRangeLastId = chunkId;
			textBoxChunkRangeLastId.Text = (_chunkRangeLastId + 1).ToString();
			if (_chunkRangeLastId < _chunkRangeFirstId)
			{
				_chunkRangeFirstId = _chunkRangeLastId;
				textBoxChunkRangeFirstId.Text = (_chunkRangeFirstId + 1).ToString();
			}

			SetChunkCountIndicators();
		}

		private void DisplayOutputFilePathOrDirectory()
		{
			string fn = Vod.IsHighlight ? $"{_fixedFileNameWithoutExt} [highlight]" : _fixedFileNameWithoutExt;
			OutputFilePathOriginal = Path.Combine(OutputDirectory, fn);
			lblOutputFileName.Text = DownloadMode == DownloadMode.SingleFile ?
				$"Имя файла: {OutputFilePathOriginal}{Playlist.StreamFileExtension}" :
				$"Папка для скачивания: {OutputFilePathOriginal}";
		}

		private void EnableControls(bool enabled)
		{
			btnStartDownload.Enabled =
			btnStopDownload.Enabled =
			btnCopyVodChunkUrlList.Enabled =
			btnCloseFrame.Enabled = enabled;
		}

		public void AbortDownload()
		{
			if (!_isAborted)
			{
				StopDownload();
				_isAborted = true;
				EnableControls(false);
			}
		}
	}
}
