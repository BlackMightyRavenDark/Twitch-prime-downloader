using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using TwitchApiLib;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
	public partial class Form1 : Form
	{
		private VodFrame activeFrameStream = null;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			TwitchApi.SetApplication(new TwitchApplication(
				"Test application", "No description",
				"gs7pui3law5lsi69yzi9qzyaqvlcsy",
				"srr2yi260t15ir6w0wq5blir22i9pq"
				)
			);

			MultiThreadedDownloaderLib.Utils.ConnectionLimit = 100;

			config.Saving += (s, json) =>
			{
				json["downloadingPath"] = config.DownloadingDirPath;
				json["tempPath"] = config.TempDirPath;
				json["fileNameFormat"] = config.FileNameFormat;
				json["lastUsedPath"] = config.LastUsedDirPath;
				json["browserExe"] = config.BrowserExeFiLePath;
				json["useGmtVodDates"] = config.UseGmtVodDates;
				json["saveVodInfo"] = config.SaveVodInfo;
				json["saveVodChunksInfo"] = config.SaveVodChunksInfo;
			};
			config.Loading += (s, json) =>
			{
				JToken jt = json.Value<JToken>("downloadingPath");
				if (jt != null)
				{
					config.DownloadingDirPath = jt.Value<string>();
				}

				jt = json.Value<JToken>("tempPath");
				if (jt != null)
				{
					config.TempDirPath = jt.Value<string>();
				}

				jt = json.Value<JToken>("lastUsedPath");
				if (jt != null)
				{
					config.LastUsedDirPath = jt.Value<string>();
				}

				jt = json.Value<JToken>("fileNameFormat");
				if (jt != null)
				{
					config.FileNameFormat = jt.Value<string>();
					if (string.IsNullOrEmpty(config.FileNameFormat))
					{
						config.FileNameFormat = FILENAME_FORMAT_DEFAULT;
					}
				}

				jt = json.Value<JToken>("browserExe");
				if (jt != null)
				{
					config.BrowserExeFiLePath = jt.Value<string>();
				}

				jt = json.Value<JToken>("useGmtVodDates");
				config.UseGmtVodDates = jt != null ? jt.Value<bool>() : true;

				jt = json.Value<JToken>("saveVodInfo");
				if (jt != null)
				{
					config.SaveVodInfo = jt.Value<bool>();
				}

				jt = json.Value<JToken>("saveVodChunksInfo");
				if (jt != null)
				{
					config.SaveVodChunksInfo = jt.Value<bool>();
				}
			};
			config.Loaded += (s) =>
			{
				chkUseGmtTime.Checked = config.UseGmtVodDates;
				chkSaveVodInfo.Checked = config.SaveVodInfo;
				chkSaveVodChunksInfo.Checked = config.SaveVodChunksInfo;
				textBox_DownloadingPath.Text = config.DownloadingDirPath;
				textBox_FileNameFormat.Text = config.FileNameFormat;
				textBox_Browser.Text = config.BrowserExeFiLePath;

				if (File.Exists(config.ChannelListFilePath))
				{
					cboxChannelName.LoadFromFile(config.ChannelListFilePath);

					if (cboxChannelName.Items.Count > 0)
					{
						cboxChannelName.SelectedIndex = 0;
					}
				}

				if (File.Exists(config.UrlListFilePath))
				{
					string[] strings = File.ReadAllLines(config.UrlListFilePath);
					textBoxUrls.Lines = strings;
				}

				if (!config.DebugMode)
				{
					tabControlMain.TabPages.Remove(tabPageDebug);
				}
			};
			config.Load();

			foreach (string s in Environment.GetCommandLineArgs())
			{
				if (s.ToLower().Equals("/debug"))
				{
					config.DebugMode = true;
					break;
				}
			}

			tabControlMain.SelectedTab = tabPageSearch;
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			ClearVodFrames();
			foreach (DownloadFrame frame in downloadFrames)
			{
				frame.FrameDispose();
				frame.Dispose();
			}

			cboxChannelName.SaveToFile(config.ChannelListFilePath);

			if (File.Exists(config.UrlListFilePath))
			{
				File.Delete(config.UrlListFilePath);
			}
			string[] urls = textBoxUrls.Lines;
			if (urls.Length > 0)
			{
				urls.SaveToFile(config.UrlListFilePath);
			}
			config.Save();
		}

		private void Form1_Resize(object sender, EventArgs e)
		{
			if (tabControlMain.SelectedTab == tabPageStreams)
			{
				StackVodFrames();
			}
			else if (tabControlMain.SelectedTab == tabPageDownloading)
			{
				StackDownloadFrames();
			}
		}

		private void ClearVodFrames()
		{
			foreach (VodFrame frame in vodFrames)
			{
				frame.Dispose();
			}
			vodFrames.Clear();
		}

		private int StackVodFrames()
		{
			if (vodFrames.Count > 0)
			{
				int w = vodFrames[0].Width;
				int h = vodFrames[0].Height;
				int gap = 4;
				int panelWidth = tabControlMain.Width - scrollBarStreams.Width - 10;
				int perRow = panelWidth / (w + gap);
				if (perRow == 0)
				{
					perRow = 1;
				}
				int rowsCount = vodFrames.Count / perRow;
				if (vodFrames.Count % perRow != 0)
				{
					rowsCount++;
				}
				int xStart = (panelWidth / 2) - ((w + gap) * perRow / 2);
				int x = xStart;
				int y = -h - gap;
				for (int i = 0; i < vodFrames.Count; ++i)
				{
					if (i % perRow == 0)
					{
						y += h + gap;
						x = xStart;
					}
					vodFrames[i].Location = new Point(x, y - scrollBarStreams.Value);
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

		private void StackDownloadFrames()
		{
			if (downloadFrames.Count > 0)
			{
				for (int i = 0; i < downloadFrames.Count; ++i)
				{
					int locY = i * downloadFrames[i].Height - scrollBarDownloads.Value;
					downloadFrames[i].Location = new Point(0, locY);
					downloadFrames[i].Width = Width - 40 + DownloadFrame.EXTRA_WIDTH;
				}

				int h = downloadFrames.Count * downloadFrames[0].Height;
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
			for (i = 0; i < downloadFrames.Count; ++i)
			{
				if (downloadFrames[i] == sender)
				{
					break;
				}
			}

			downloadFrames.RemoveAt(i);
			if (downloadFrames.Count > 0)
			{
				tabPageDownloading.Text = $"Скачивание ({downloadFrames.Count})";
				StackDownloadFrames();
			}
			else
			{
				tabPageDownloading.Text = "Скачивание";
			}
		}

		private void OnVodFrame_Activated(object sender)
		{
			activeFrameStream = sender as VodFrame;
			foreach (VodFrame frameStream in vodFrames)
			{
				frameStream.BackColor = frameStream == activeFrameStream ?
					VodFrame.ColorActive : VodFrame.ColorInactive;
			}
		}

		private void OnVodFrame_ImageMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				contextMenuStreamImage.Show(Cursor.Position.X, Cursor.Position.Y);
			}
		}

		private void CopyImageUrlToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string url = activeFrameStream.StreamInfo?.FormatThumbnailTemplateUrl(1920, 1080);
			if (!string.IsNullOrEmpty(url) && !string.IsNullOrWhiteSpace(url))
			{
				SetClipboardText(url);
			}
		}

		private void TextBox_DownloadingPath_Leave(object sender, EventArgs e)
		{
			config.DownloadingDirPath = textBox_DownloadingPath.Text;
		}

		private void CopyStreamInfoJsonToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TwitchVod vod = activeFrameStream.StreamInfo;
			if (!string.IsNullOrEmpty(vod.RawData) && !string.IsNullOrWhiteSpace(vod.RawData))
			{
				SetClipboardText(vod.RawData);
			}
			else
			{
				MessageBox.Show("Информация о стриме пуста", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void miSavePlaylistAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				string playlistRaw = activeFrameStream.StreamInfo?.Playlist?.PlaylistRaw;
				if (!string.IsNullOrEmpty(playlistRaw) && !string.IsNullOrWhiteSpace(playlistRaw))
				{
					using (SaveFileDialog sfd = new SaveFileDialog())
					{
						sfd.Title = "Куда будем сохранять?";
						sfd.Filter = "*.m3u8|*.M3U8-files";
						sfd.DefaultExt = ".m3u8";
						sfd.InitialDirectory = config.DownloadingDirPath;
						sfd.FileName = FixFileName(FormatFileName(config.FileNameFormat, activeFrameStream.StreamInfo)) + "_playlist.m3u8";
						if (sfd.ShowDialog() == DialogResult.OK)
						{
							if (File.Exists(sfd.FileName)) { File.Delete(sfd.FileName); }
							File.WriteAllText(sfd.FileName, playlistRaw);
						}
					}
				}
				else
				{
					MessageBox.Show("Плейлист не найден!", "Ошибка!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
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

		private async void btnSearchChannelName_Click(object sender, EventArgs e)
		{
			btnSearchChannelName.Enabled = false;
			string channelName = cboxChannelName.Text?.Trim();
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
			lbLog.Items.Add($"Скачивание списка видео канала {channelName}...");
			tabControlMain.SelectedTab = tabPageLog;
			ClearVodFrames();
			tabPageStreams.Text = "Стримы";

			uint limit = rbSearchLimit.Checked ? (uint)numericUpDownSearchLimit.Value : uint.MaxValue;
			TwitchUserResult userResult = await Task.Run(() => TwitchUser.Get(channelName.ToLower()));
			if (userResult.ErrorCode != 200)
			{
				lbLog.Items.Add($"Канал \"{channelName}\" не найден!");
				btnSearchChannelName.Enabled = true;
				return;
			}

			List<TwitchVodResult> vodResults = await Task.Run(() => userResult.User.GetVideosMultiThreaded(limit).ToList());
			if (vodResults.Count == 0)
			{
				lbLog.Items.Add("Видео не найдены!");
				btnSearchChannelName.Enabled = true;
				return;
			}

			vodResults.Sort((x, y) =>
			{
				if (x.ErrorCode != 200 || y.ErrorCode != 200) { return 0; }
				return x.Vod.CreationDate > y.Vod.CreationDate ? -1 : 1;
			});

			lbLog.Items.Add($"Найдено {vodResults.Count} видео");
			int errorCount = 0;
			foreach (TwitchVodResult vodResult in vodResults)
			{
				if (vodResult.ErrorCode == 200)
				{
					lbLog.Items.Add($"Создание фрейма для видео {vodResult.Vod.Id} \"{vodResult.Vod.Title}\"...");
					AddStreamItem(vodResult.Vod);
				}
				else
				{
					errorCount++;
				}
			}

			if (errorCount > 0)
			{
				lbLog.Items.Add($"Количество ошибок: {errorCount}");
			}

			int actualStreamCount = vodResults.Count - errorCount;
			if (actualStreamCount > 0)
			{
				tabPageStreams.Text = $"Стримы ({actualStreamCount})";

				StackVodFrames();

				tabControlMain.SelectedTab = tabPageStreams;
			}

			btnSearchChannelName.Enabled = true;
		}

		private async void btnSearchByUrls_Click(object sender, EventArgs e)
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

			ClearVodFrames();

			for (int i = 0; i < urls.Length; ++i)
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

				if (uint.TryParse(vodId, out uint id))
				{
					TwitchVodResult vodResult = await Task.Run(() => TwitchVod.Get(id));
					if (vodResult.ErrorCode == 200)
					{
						AddStreamItem(vodResult.Vod);

						lbLog.Items.Add($"{i + 1} / {urls.Length}: {urls[i]}...OK");
					}
					else
					{
						lbLog.Items.Add($"{i + 1} / {urls.Length}: {urls[i]}...FAILED! Error code {vodResult.ErrorCode}");
					}
				}
				else
				{
					lbLog.Items.Add($"{i + 1} / {urls.Length}: {urls[i]}...FAILED! Can't parse video ID!");
				}
			}

			if (vodFrames.Count > 0)
			{
				tabPageStreams.Text = $"Стримы ({vodFrames.Count})";
				tabControlMain.SelectedTab = tabPageStreams;
			}

			btnSearchByUrls.Enabled = true;
		}

		private void panelStreams_MouseDown(object sender, MouseEventArgs e)
		{
			foreach (VodFrame frame in vodFrames)
			{
				frame.BackColor = VodFrame.ColorInactive;
			}

			activeFrameStream = null;
		}

		private void AddStreamItem(TwitchVod vod)
		{
			VodFrame frame = new VodFrame(vod);
			frame.Parent = panelStreams;
			frame.Activated += OnVodFrame_Activated;
			frame.ImageMouseDown += OnVodFrame_ImageMouseDown;
			frame.DownloadButtonClicked += OnVodFrame_DownloadButtonClick;
			vodFrames.Add(frame);
		}

		private async void OnVodFrame_DownloadButtonClick(object sender)
		{
			if (string.IsNullOrEmpty(config.DownloadingDirPath))
			{
				MessageBox.Show("Не указана папка для скачивания!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			VodFrame frameStream = sender as VodFrame;
			frameStream.btnDownload.Enabled = false;

			TwitchVodPlaylistResult playlistResult = await Task.Run(() =>
			{
				if (frameStream.StreamInfo.IsLive && frameStream.StreamInfo.UpdatePlaylistManifest() == 200 &&
					frameStream.StreamInfo.PlaylistManifest[0].UpdatePlaylist() == 200)
				{
					return new TwitchVodPlaylistResult(frameStream.StreamInfo.PlaylistManifest[0].Playlist, 200, null);
				}

				return frameStream.StreamInfo.GetPlaylist("chunked");
			});
			if (playlistResult.ErrorCode == 200)
			{
				if (config.DebugMode)
				{
					memoDebug.Text = playlistResult.Playlist.PlaylistRaw;
				}

				playlistResult.Playlist.Parse();

				DownloadFrame frame = new DownloadFrame(frameStream.StreamInfo, playlistResult.Playlist);
				frame.Parent = panelDownloads;
				frame.Location = new Point(0, 0);
				frame.Closed += OnFrameDownload_Closed;

				frame.ChunkFrom = 0;
				frame.ChunkTo = frame.Playlist.Count - 1;

				downloadFrames.Add(frame);

				tabPageDownloading.Text = $"Скачивание ({downloadFrames.Count})";
				if (tabControlMain.SelectedTab == tabPageDownloading)
				{
					StackDownloadFrames();
				}
			}
			else
			{
				MessageBox.Show($"Error {playlistResult.ErrorCode}", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			frameStream.btnDownload.Enabled = true;
		}

		private void btnSelectDownloadingPath_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.Description = "Выберите папку для скачивания";
			folderBrowserDialog.SelectedPath =
				(!string.IsNullOrEmpty(config.DownloadingDirPath) && Directory.Exists(config.DownloadingDirPath)) ?
				config.DownloadingDirPath : config.SelfDirPath;
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				config.DownloadingDirPath = folderBrowserDialog.SelectedPath;

				textBox_DownloadingPath.Text = config.DownloadingDirPath;
			}
		}

		private void scrollBarStreams_Scroll(object sender, ScrollEventArgs e)
		{
			StackVodFrames();
		}

		private void saveImageAssToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "jpg|*.jpg";
			sfd.DefaultExt = ".jpg";
			sfd.InitialDirectory = config.LastUsedDirPath;
			string fn = FixFileName(FormatFileName(config.FileNameFormat, activeFrameStream.StreamInfo));
			sfd.FileName = fn + "_preview";
			if (sfd.ShowDialog() != DialogResult.Cancel)
			{
				config.LastUsedDirPath = sfd.FileName;
				activeFrameStream.StreamInfo.ThumbnailImageData.SaveToFile(sfd.FileName);
			}
			sfd.Dispose();
		}

		private void btnSelectBrowser_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Выберите браузер";
			ofd.Filter = "exe|*.exe";
			string dir = string.IsNullOrEmpty(config.BrowserExeFiLePath) ?
				config.SelfDirPath : Path.GetFullPath(config.BrowserExeFiLePath);
			ofd.InitialDirectory = dir;
			if (ofd.ShowDialog() != DialogResult.Cancel)
			{
				config.BrowserExeFiLePath = ofd.FileName;
				textBox_Browser.Text = ofd.FileName;
			}
			ofd.Dispose();
		}

		private void openVideoInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(config.BrowserExeFiLePath) || string.IsNullOrWhiteSpace(config.BrowserExeFiLePath))
			{
				MessageBox.Show("Браузер не указан!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (!File.Exists(config.BrowserExeFiLePath))
			{
				MessageBox.Show("Браузер не найден!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Process process = new Process();
			process.StartInfo.FileName = Path.GetFileName(config.BrowserExeFiLePath);
			process.StartInfo.WorkingDirectory = Path.GetFullPath(config.BrowserExeFiLePath);
			process.StartInfo.Arguments = activeFrameStream.StreamInfo.Url;
			process.Start();
		}

		private void textBox_FileNameFormat_Leave(object sender, EventArgs e)
		{
			config.FileNameFormat = (sender as TextBox).Text;
		}

		private void scrollBarDownloads_Scroll(object sender, ScrollEventArgs e)
		{
			StackDownloadFrames();
		}

		private void miCopyVideoUrl_Click(object sender, EventArgs e)
		{
			if (activeFrameStream != null && activeFrameStream.StreamInfo != null)
			{
				if (!string.IsNullOrEmpty(activeFrameStream.StreamInfo.Url) &&
					!string.IsNullOrWhiteSpace(activeFrameStream.StreamInfo.Url))
				{
					SetClipboardText(activeFrameStream.StreamInfo.Url);
				}
			}
		}

		private void btnRestoreDefaultFilenameFormat_Click(object sender, EventArgs e)
		{
			textBox_FileNameFormat.Text = FILENAME_FORMAT_DEFAULT;
			config.FileNameFormat = FILENAME_FORMAT_DEFAULT;
		}

		private void tabControlMain_Selected(object sender, TabControlEventArgs e)
		{
			if (e.TabPage == tabPageStreams)
			{
				StackVodFrames();
			}
			else if (e.TabPage == tabPageDownloading)
			{
				StackDownloadFrames();
			}
		}

		private void chkUseGmtTime_CheckedChanged(object sender, EventArgs e)
		{
			config.UseGmtVodDates = chkUseGmtTime.Checked;
			foreach (VodFrame frame in vodFrames)
			{
				frame.UseGmtTime = config.UseGmtVodDates;
			}
		}

		private void chkSaveVodInfo_CheckedChanged(object sender, EventArgs e)
		{
			config.SaveVodInfo = chkSaveVodInfo.Checked;
		}

		private void chkSaveVodChunksInfo_CheckedChanged(object sender, EventArgs e)
		{
			config.SaveVodChunksInfo = chkSaveVodChunksInfo.Checked;
		}
	}
}
