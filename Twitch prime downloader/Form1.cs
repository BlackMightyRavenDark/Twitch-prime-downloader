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

		private void form1_Load(object sender, EventArgs e)
		{
			TwitchApiLib.Utils.TwitchHelixOauthToken.TokenUpdating += (s) =>
				Invoke(new MethodInvoker(() => textBoxHelixApiToken.Text = lblHelixApiTokenExpirationDate.Text = "Обновляется..."));
			TwitchApiLib.Utils.TwitchHelixOauthToken.TokenUpdated += (s, errorCode, errorMessage) =>
			{
				Invoke(new MethodInvoker(() =>
				{
					if (errorCode != 200)
					{
						textBoxHelixApiToken.Text = "<NULL>";
						lblHelixApiTokenExpirationDate.Text = "<Неизвестно>";

						string msg = "Не удалось обновить Helix API token!";
						if (!string.IsNullOrEmpty(errorMessage)) { msg += Environment.NewLine + errorMessage; }
						MessageBox.Show(msg, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					textBoxHelixApiToken.Text = TwitchApiLib.Utils.TwitchHelixOauthToken.AccessToken;
					lblHelixApiTokenExpirationDate.Text = TwitchApiLib.Utils.FormatDateTime(TwitchApiLib.Utils.TwitchHelixOauthToken.ExpirationDate);
					listBoxEventLog.Items.Add("Twitch Helix API token успешно обновлён!");
				}));
			};

			MultiThreadedDownloaderLib.Utils.ConnectionLimit = 100;

			config.Saving += (s, json) =>
			{
				json["downloadDirectory"] = config.DownloadDirectory;
				json["outputFileNameFormat"] = config.OutputFileNameFormat;
				json["lastUsedDirectory"] = config.LastUsedDirectory;
				json["webBrowserExeFilePath"] = config.BrowserExeFilePath;
				json["useGmtTime"] = config.UseGmtVodDates;
				json["saveVodInfo"] = config.SaveVodInfo;
				json["saveVodChunkInfo"] = config.SaveVodChunksInfo;
				json["apiApplicationTitle"] = config.ApiApplicationTitle;
				json["apiApplicationDescription"] = config.ApiApplicationDescription;
				json["apiApplicationClientId"] = config.ApiApplicationClientId;
				json["apiApplicationClientSecretKey"] = config.ApiApplicationClientSecretKey;
			};
			config.Loading += (s, json) =>
			{
				JToken jt = json.Value<JToken>("downloadDirectory");
				if (jt != null)
				{
					config.DownloadDirectory = jt.Value<string>();
				}

				jt = json.Value<JToken>("lastUsedDirectory");
				if (jt != null)
				{
					config.LastUsedDirectory = jt.Value<string>();
				}

				jt = json.Value<JToken>("outputFileNameFormat");
				if (jt != null)
				{
					config.OutputFileNameFormat = jt.Value<string>();
					if (string.IsNullOrEmpty(config.OutputFileNameFormat))
					{
						config.OutputFileNameFormat = FILENAME_FORMAT_DEFAULT;
					}
				}

				jt = json.Value<JToken>("webBrowserExeFilePath");
				if (jt != null)
				{
					config.BrowserExeFilePath = jt.Value<string>();
				}

				jt = json.Value<JToken>("useGmtTime");
				config.UseGmtVodDates = jt != null ? jt.Value<bool>() : true;

				jt = json.Value<JToken>("saveVodInfo");
				if (jt != null)
				{
					config.SaveVodInfo = jt.Value<bool>();
				}

				jt = json.Value<JToken>("saveVodChunkInfo");
				if (jt != null)
				{
					config.SaveVodChunksInfo = jt.Value<bool>();
				}

				config.ApiApplicationTitle = json.Value<string>("apiApplicationTitle");
				config.ApiApplicationDescription = json.Value<string>("apiApplicationDescription");
				config.ApiApplicationClientId = json.Value<string>("apiApplicationClientId");
				config.ApiApplicationClientSecretKey = json.Value<string>("apiApplicationClientSecretKey");

				if (string.IsNullOrEmpty(config.ApiApplicationClientId) && string.IsNullOrEmpty(config.ApiApplicationClientSecretKey))
				{
					SetDefaultTwitchApplication();
				}
			};
			config.Loaded += (s) =>
			{
				checkBoxUseGmtTime.Checked = config.UseGmtVodDates;
				checkBoxSaveVodInfo.Checked = config.SaveVodInfo;
				checkBoxSaveVodChunkInfo.Checked = config.SaveVodChunksInfo;
				textBoxDownloadDirectory.Text = config.DownloadDirectory;
				textBoxOutputFileNameFormat.Text = config.OutputFileNameFormat;
				textBoxBrowserExePath.Text = config.BrowserExeFilePath;
				textBoxApiApplicationTitle.Text = config.ApiApplicationTitle;
				textBoxApiApplicationDescription.Text = config.ApiApplicationDescription;
				textBoxHelixApiClientId.Text = config.ApiApplicationClientId;
				textBoxHelixApiClientSecretKey.Text = config.ApiApplicationClientSecretKey;

				TwitchApplication application = MakeTwitchApplication();
				TwitchApi.SetApplication(application);

				try
				{
					if (File.Exists(config.ChannelListFilePath))
					{
						string t = File.ReadAllText(config.ChannelListFilePath);
						string[] strings = t.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
						if (strings.Length > 0)
						{
							listBoxChannelList.Items.AddRange(strings);
							if (listBoxChannelList.Items.Count > 0)
							{
								listBoxChannelList.SelectedIndex = 0;
								textBoxChannelName.Text = listBoxChannelList.Items[0].ToString();
							}
						}
					}
				} catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Ошибка!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				if (File.Exists(config.UrlListFilePath))
				{
					string[] strings = File.ReadAllLines(config.UrlListFilePath);
					textBoxVideoUrls.Lines = strings;
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

		private void form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			ClearVodFrames();
			foreach (DownloadFrame frame in downloadFrames)
			{
				frame.FrameDispose();
				frame.Dispose();
			}

			if (File.Exists(config.UrlListFilePath))
			{
				File.Delete(config.UrlListFilePath);
			}
			string[] urls = textBoxVideoUrls.Lines;
			if (urls.Length > 0)
			{
				urls.SaveToFile(config.UrlListFilePath);
			}
			config.Save();
		}

		private void form1_Resize(object sender, EventArgs e)
		{
			if (tabControlMain.SelectedTab == tabPageStreams)
			{
				StackVodFrames();
			}
			else if (tabControlMain.SelectedTab == tabPageDownloads)
			{
				StackDownloadFrames();
			}
		}

		private void panelStreams_MouseDown(object sender, MouseEventArgs e)
		{
			foreach (VodFrame frame in vodFrames)
			{
				frame.BackColor = VodFrame.ColorInactive;
			}

			activeFrameStream = null;
		}

		private async void btnSearchByChannelName_Click(object sender, EventArgs e)
		{
			btnSearchByChannelName.Enabled = false;
			if (!IsTwitchApplicationValid())
			{
				btnSearchByChannelName.Enabled = true;
				return;
			}

			string channelName = textBoxChannelName.Text?.Trim();
			if (string.IsNullOrEmpty(channelName) || string.IsNullOrWhiteSpace(channelName))
			{
				MessageBox.Show("Не введено название канала!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				btnSearchByChannelName.Enabled = true;
				return;
			}
			if (channelName.Contains(" "))
			{
				MessageBox.Show("Название канала не может содержать пробелов!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				btnSearchByChannelName.Enabled = true;
				return;
			}

			listBoxEventLog.Items.Clear();
			listBoxEventLog.Items.Add($"Скачивание списка видео канала {channelName}...");
			tabControlMain.SelectedTab = tabPageEventLog;
			ClearVodFrames();
			tabPageStreams.Text = "Стримы";

			uint limit = radioButtonSearchLimited.Checked ? (uint)numericUpDownSearchLimit.Value : uint.MaxValue;
			TwitchUserResult userResult = await Task.Run(() => TwitchUser.Get(channelName.ToLower()));
			if (userResult.ErrorCode != 200)
			{
				listBoxEventLog.Items.Add($"Канал \"{channelName}\" не найден!");
				btnSearchByChannelName.Enabled = true;
				return;
			}

			List<TwitchVodResult> vodResults = await Task.Run(() => userResult.User.GetVideosMultiThreaded(limit).ToList());
			if (vodResults.Count == 0)
			{
				listBoxEventLog.Items.Add("Видео не найдены!");
				btnSearchByChannelName.Enabled = true;
				return;
			}

			vodResults.Sort((x, y) =>
			{
				if (x.ErrorCode != 200 || y.ErrorCode != 200) { return 0; }
				return x.Vod.CreationDate > y.Vod.CreationDate ? -1 : 1;
			});

			listBoxEventLog.Items.Add($"Найдено {vodResults.Count} видео");
			int errorCount = 0;
			foreach (TwitchVodResult vodResult in vodResults)
			{
				if (vodResult.ErrorCode == 200)
				{
					listBoxEventLog.Items.Add($"Создание фрейма для видео {vodResult.Vod.Id} \"{vodResult.Vod.Title}\"...");
					AddStreamItem(vodResult.Vod);
				}
				else
				{
					errorCount++;
				}
			}

			if (errorCount > 0)
			{
				listBoxEventLog.Items.Add($"Количество ошибок: {errorCount}");
			}

			int actualStreamCount = vodResults.Count - errorCount;
			if (actualStreamCount > 0)
			{
				tabPageStreams.Text = $"Стримы ({actualStreamCount})";

				StackVodFrames();

				tabControlMain.SelectedTab = tabPageStreams;
			}

			btnSearchByChannelName.Enabled = true;
		}

		private async void btnSearchByUrls_Click(object sender, EventArgs e)
		{
			btnSearchByUrls.Enabled = false;
			if (!IsTwitchApplicationValid())
			{
				btnSearchByUrls.Enabled = true;
				return;
			}

			string[] urls = textBoxVideoUrls.Lines;
			if (urls.Length == 0)
			{
				MessageBox.Show("Введите ссылки!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				btnSearchByUrls.Enabled = true;
				return;
			}

			tabControlMain.SelectedTab = tabPageEventLog;
			listBoxEventLog.Items.Clear();
			listBoxEventLog.Items.Add("Поиск видео по ссылкам...");
			tabPageStreams.Text = "Стримы";

			ClearVodFrames();

			for (int i = 0; i < urls.Length; ++i)
			{
				if (string.IsNullOrEmpty(urls[i]) || string.IsNullOrWhiteSpace(urls[i]))
				{
					listBoxEventLog.Items.Add($"{i + 1} / {urls.Length}: Empty URL!");
					continue;
				}
				string vodId = ExtractVodIdFromUrl(urls[i]);
				if (string.IsNullOrEmpty(vodId) || string.IsNullOrWhiteSpace(vodId))
				{
					listBoxEventLog.Items.Add($"{i + 1} / {urls.Length}: {urls[i]}...FAILED!");
					continue;
				}

				if (uint.TryParse(vodId, out uint id))
				{
					TwitchVodResult vodResult = await Task.Run(() => TwitchVod.Get(id));
					if (vodResult.ErrorCode == 200)
					{
						AddStreamItem(vodResult.Vod);

						listBoxEventLog.Items.Add($"{i + 1} / {urls.Length}: {urls[i]}...OK");
					}
					else
					{
						listBoxEventLog.Items.Add($"{i + 1} / {urls.Length}: {urls[i]}...FAILED! Error code {vodResult.ErrorCode}");
					}
				}
				else
				{
					listBoxEventLog.Items.Add($"{i + 1} / {urls.Length}: {urls[i]}...FAILED! Can't parse video ID!");
				}
			}

			if (vodFrames.Count > 0)
			{
				tabPageStreams.Text = $"Стримы ({vodFrames.Count})";
				tabControlMain.SelectedTab = tabPageStreams;
			}

			btnSearchByUrls.Enabled = true;
		}

		private void btnSelectDownloadDirectory_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			folderBrowserDialog.Description = "Выберите папку для скачивания";
			folderBrowserDialog.SelectedPath =
				(!string.IsNullOrEmpty(config.DownloadDirectory) && Directory.Exists(config.DownloadDirectory)) ?
				config.DownloadDirectory : config.SelfDirectory;
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				config.LastUsedDirectory =
				config.DownloadDirectory = folderBrowserDialog.SelectedPath;

				textBoxDownloadDirectory.Text = config.DownloadDirectory;
			}
		}

		private void btnRestoreDefaultOutputFileNameFormat_Click(object sender, EventArgs e)
		{
			textBoxOutputFileNameFormat.Text = FILENAME_FORMAT_DEFAULT;
			config.OutputFileNameFormat = FILENAME_FORMAT_DEFAULT;
		}

		private void btnSelectBrowser_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Выберите браузер";
			ofd.Filter = "exe|*.exe";
			string dir = string.IsNullOrEmpty(config.BrowserExeFilePath) ?
				config.SelfDirectory : Path.GetFullPath(config.BrowserExeFilePath);
			ofd.InitialDirectory = dir;
			if (ofd.ShowDialog() != DialogResult.Cancel)
			{
				config.BrowserExeFilePath = ofd.FileName;
				textBoxBrowserExePath.Text = ofd.FileName;
				config.LastUsedDirectory = Path.GetDirectoryName(ofd.FileName);
			}
			ofd.Dispose();
		}

		private void btnAddChannelToList_Click(object sender, EventArgs e)
		{
			try
			{
				string channelName = textBoxChannelName.Text.Trim();
				if (string.IsNullOrEmpty(channelName) || string.IsNullOrWhiteSpace(channelName))
				{
					MessageBox.Show("Введите название канал", "Ошибка!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (channelName.Contains(" "))
				{
					MessageBox.Show("Имя канала не должно содержать пробелов!", "Ошибка!",
						MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}

				IEnumerable<string> names = listBoxChannelList.GetStrings();
				if (names.Any(item => string.Equals(item, channelName, StringComparison.OrdinalIgnoreCase)))
				{
					MessageBox.Show($"Канал \"{channelName}\" уже есть в списке!", "Ошибка!",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				listBoxChannelList.Items.Add(channelName);
				listBoxChannelList.SelectedIndex = listBoxChannelList.Items.Count - 1;
				if (File.Exists(config.ChannelListFilePath)) { File.Delete(config.ChannelListFilePath); }
				File.WriteAllText(config.ChannelListFilePath, names.ToText());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnRestoreDefaultApiApplication_Click(object sender, EventArgs e)
		{
			const string msg = "Внимание! Значения по-умолчанию в данный момент могут быть устаревшими и больше не работать!\n" +
				"Восстановить значения по-умолчанию?";
			if (MessageBox.Show(msg, Text,
				MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				SetDefaultTwitchApplication();
			}
		}

		private async void btnApplyApiApplication_Click(object sender, EventArgs e)
		{
			btnUpdateHelixApiToken.Enabled =
			btnResetHelixApiToken.Enabled =
			btnApplyApiApplication.Enabled =
			btnRestoreDefaultApiApplication.Enabled = false;

			config.ApiApplicationTitle = textBoxApiApplicationTitle.Text;
			config.ApiApplicationDescription = textBoxApiApplicationDescription.Text;
			config.ApiApplicationClientId = textBoxHelixApiClientId.Text;
			config.ApiApplicationClientSecretKey = textBoxHelixApiClientSecretKey.Text;

			TwitchApplication application = MakeTwitchApplication();
			TwitchApi.SetApplication(application);

			await Task.Delay(500);

			btnRestoreDefaultApiApplication.Enabled =
			btnApplyApiApplication.Enabled =
			btnResetHelixApiToken.Enabled =
			btnUpdateHelixApiToken.Enabled = true;
		}

		private async void btnUpdateHelixApiToken_Click(object sender, EventArgs e)
		{
			btnUpdateHelixApiToken.Enabled =
			btnResetHelixApiToken.Enabled =
			btnApplyApiApplication.Enabled =
			btnRestoreDefaultApiApplication.Enabled = false;

			if (IsTwitchApplicationValid())
			{
				await Task.Run(() =>
				{
					TwitchApplication application = TwitchApi.GetApplication();
					TwitchApiLib.Utils.TwitchHelixOauthToken.Update(application, out _);
				});
			}

			btnRestoreDefaultApiApplication.Enabled =
			btnApplyApiApplication.Enabled =
			btnResetHelixApiToken.Enabled =
			btnUpdateHelixApiToken.Enabled = true;
		}

		private void btnResetHelixApiToken_Click(object sender, EventArgs e)
		{
			lock (TwitchApiLib.Utils.TwitchHelixOauthToken)
			{
				TwitchApiLib.Utils.TwitchHelixOauthToken.Reset();
				textBoxHelixApiToken.Text = "<NULL>";
				lblHelixApiTokenExpirationDate.Text = "<Неизвестно>";
			}
		}

		private void btnEditChannelList_Click(object sender, EventArgs e)
		{
			try
			{
				FormChannelListEditor editor = new FormChannelListEditor(listBoxChannelList.GetStrings());
				if (editor.ShowDialog() == DialogResult.OK)
				{
					listBoxChannelList.Items.Clear();
					if (editor.Channels.Count > 0)
					{
						foreach (string t in editor.Channels)
						{
							listBoxChannelList.Items.Add(t);
						}
						textBoxChannelName.Text = listBoxChannelList.Items[0].ToString();
						listBoxChannelList.SelectedIndex = 0;
					}
					else
					{
						textBoxChannelName.Text = null;
					}

					if (listBoxChannelList.Items.Count > 0)
					{
						string list = listBoxChannelList.GetStrings().ToText();
						if (File.Exists(config.ChannelListFilePath)) { File.Delete(config.ChannelListFilePath); }
						File.WriteAllText(config.ChannelListFilePath, list);
					}
					else if (File.Exists(config.ChannelListFilePath))
					{
						File.Delete(config.ChannelListFilePath);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void checkBoxUseGmtTime_CheckedChanged(object sender, EventArgs e)
		{
			config.UseGmtVodDates = checkBoxUseGmtTime.Checked;
			foreach (VodFrame frame in vodFrames)
			{
				frame.UseGmtTime = config.UseGmtVodDates;
			}
		}

		private void checkBoxSaveVodInfo_CheckedChanged(object sender, EventArgs e)
		{
			config.SaveVodInfo = checkBoxSaveVodInfo.Checked;
		}

		private void checkBoxSaveVodChunkInfo_CheckedChanged(object sender, EventArgs e)
		{
			config.SaveVodChunksInfo = checkBoxSaveVodChunkInfo.Checked;
		}

		private void listBoxChannelList_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBoxChannelName.Text = listBoxChannelList.Items[listBoxChannelList.SelectedIndex].ToString();
		}

		private void listBoxChannelList_DoubleClick(object sender, EventArgs e)
		{
			if (listBoxChannelList.SelectedItem != null &&
				MessageBox.Show($"Найти видео канала \"{listBoxChannelList.SelectedItem}\"?", "Поиск",
					MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
			{
				btnSearchByChannelName.PerformClick();
			}
		}

		private void miSaveVodThumbnailImageAssToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "jpg|*.jpg";
			sfd.DefaultExt = ".jpg";
			sfd.InitialDirectory = config.LastUsedDirectory;
			string fn = FixFileName(FormatFileName(config.OutputFileNameFormat, activeFrameStream.StreamInfo));
			sfd.FileName = fn + "_thumbnail";
			if (sfd.ShowDialog() != DialogResult.Cancel)
			{
				config.LastUsedDirectory = Path.GetDirectoryName(sfd.FileName);
				activeFrameStream.StreamInfo.ThumbnailImageData.SaveToFile(sfd.FileName);
			}
			sfd.Dispose();
		}

		private void miOpenVideoInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(config.BrowserExeFilePath) || string.IsNullOrWhiteSpace(config.BrowserExeFilePath))
			{
				MessageBox.Show("Браузер не указан!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (!File.Exists(config.BrowserExeFilePath))
			{
				MessageBox.Show("Браузер не найден!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Process process = new Process();
			process.StartInfo.FileName = Path.GetFileName(config.BrowserExeFilePath);
			process.StartInfo.WorkingDirectory = Path.GetFullPath(config.BrowserExeFilePath);
			process.StartInfo.Arguments = activeFrameStream.StreamInfo.Url;
			process.Start();
		}

		private void miCopyVodThumbnailImageUrlToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string url = activeFrameStream.StreamInfo?.FormatThumbnailTemplateUrl(1920, 1080);
			if (!string.IsNullOrEmpty(url) && !string.IsNullOrWhiteSpace(url))
			{
				SetClipboardText(url);
			}
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

		private void miCopyVodInfoToolStripMenuItem_Click(object sender, EventArgs e)
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

		private void miSaveVodPlaylistAsToolStripMenuItem_Click(object sender, EventArgs e)
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
						sfd.InitialDirectory = config.DownloadDirectory;
						sfd.FileName = FixFileName(FormatFileName(config.OutputFileNameFormat, activeFrameStream.StreamInfo)) + "_playlist.m3u8";
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

		private void scrollBarStreams_Scroll(object sender, ScrollEventArgs e)
		{
			StackVodFrames();
		}

		private void scrollBarDownloads_Scroll(object sender, ScrollEventArgs e)
		{
			StackDownloadFrames();
		}

		private void tabControlMain_Selected(object sender, TabControlEventArgs e)
		{
			if (e.TabPage == tabPageStreams)
			{
				StackVodFrames();
			}
			else if (e.TabPage == tabPageDownloads)
			{
				StackDownloadFrames();
			}
		}

		private void textBoxDownloadDirectory_Leave(object sender, EventArgs e)
		{
			config.DownloadDirectory = textBoxDownloadDirectory.Text;
		}

		private void textBoxOutputFileNameFormat_Leave(object sender, EventArgs e)
		{
			config.OutputFileNameFormat = (sender as TextBox).Text;
		}

		private void AddStreamItem(TwitchVod vod)
		{
			VodFrame frame = new VodFrame(vod);
			frame.Parent = panelStreams;
			frame.Activated += OnVodFrame_Activated;
			frame.ImageMouseDown += OnVodFrame_ThumbnailImageMouseDown;
			frame.DownloadButtonClicked += OnVodFrame_DownloadButtonClick;
			vodFrames.Add(frame);
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
				tabPageDownloads.Text = $"Скачивание ({downloadFrames.Count})";
				StackDownloadFrames();
			}
			else
			{
				tabPageDownloads.Text = "Скачивание";
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

		private void OnVodFrame_ThumbnailImageMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				contextMenuVodThumbnailImage.Show(Cursor.Position.X, Cursor.Position.Y);
			}
		}

		private async void OnVodFrame_DownloadButtonClick(object sender)
		{
			if (string.IsNullOrEmpty(config.DownloadDirectory))
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
					richTextBoxDebugLog.Text = playlistResult.Playlist.PlaylistRaw;
				}

				playlistResult.Playlist.Parse();

				DownloadFrame frame = new DownloadFrame(frameStream.StreamInfo, playlistResult.Playlist);
				frame.Parent = panelDownloads;
				frame.Location = new Point(0, 0);
				frame.Closed += OnFrameDownload_Closed;

				frame.ChunkFrom = 0;
				frame.ChunkTo = frame.Playlist.Count - 1;

				downloadFrames.Add(frame);

				tabPageDownloads.Text = $"Скачивание ({downloadFrames.Count})";
				if (tabControlMain.SelectedTab == tabPageDownloads)
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

		private static TwitchApplication MakeTwitchApplication()
		{
			return new TwitchApplication(
				config.ApiApplicationTitle,
				config.ApiApplicationDescription,
				config.ApiApplicationClientId,
				config.ApiApplicationClientSecretKey);
		}

		private void SetDefaultTwitchApplication()
		{
			textBoxApiApplicationTitle.Text = config.ApiApplicationTitle = defaultApplication.Name;
			textBoxApiApplicationDescription.Text = config.ApiApplicationDescription = defaultApplication.Description;
			textBoxHelixApiClientId.Text = config.ApiApplicationClientId = defaultApplication.ClientId;
			textBoxHelixApiClientSecretKey.Text = config.ApiApplicationClientSecretKey = defaultApplication.ClientSecretKey;
			TwitchApi.SetApplication(defaultApplication);
		}

		private static bool IsTwitchApplicationValid()
		{
			if (string.IsNullOrEmpty(config.ApiApplicationClientId) || string.IsNullOrWhiteSpace(config.ApiApplicationClientId))
			{
				MessageBox.Show("Не указан ID приложения Twitch!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			if (string.IsNullOrEmpty(config.ApiApplicationClientSecretKey) || string.IsNullOrWhiteSpace(config.ApiApplicationClientSecretKey))
			{
				MessageBox.Show("Не указан секретный ключ приложения Twitch!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			return true;
		}
	}
}
