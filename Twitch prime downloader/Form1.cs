using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using TwitchApiLib;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
	public partial class Form1 : Form
	{
		private VodFrame _activeFrameStream = null;
		private bool _isClosing = false;

		public Form1()
		{
			InitializeComponent();
		}

		private void form1_Load(object sender, EventArgs e)
		{
			TwitchApiLib.Utils.TwitchHelixOauthToken.TokenUpdating += s =>
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
						if (!string.IsNullOrWhiteSpace(errorMessage)) { msg += Environment.NewLine + errorMessage; }
						MessageBox.Show(msg, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					textBoxHelixApiToken.Text = TwitchApiLib.Utils.TwitchHelixOauthToken.AccessToken;
					lblHelixApiTokenExpirationDate.Text = TwitchApiLib.Utils.FormatDateTime(TwitchApiLib.Utils.TwitchHelixOauthToken.ExpirationDate);
					listBoxEventLog.Items.Add("Twitch Helix API token успешно обновлён!");
				}));
			};

			MultiThreadedDownloaderLib.Utils.ConnectionLimit = 100;
			foreach (string s in Environment.GetCommandLineArgs())
			{
				if (string.Equals(s, "/debug"))
				{
					config.DebugMode = true;
					break;
				}
			}

			config.Saving += (s, json) =>
			{
				json["downloadDirectory"] = config.DownloadDirectory;
				json["outputFileNameFormat"] = config.OutputFileNameFormat;
				json["lastUsedDirectory"] = config.LastUsedDirectory;
				json["webBrowserExeFilePath"] = config.BrowserExeFilePath;
				json["useGmtTime"] = config.UseGmtVodDates;
				json["saveVodInfo"] = config.SaveVodInfo;
				json["saveVodChunkInfo"] = config.SaveVodChunkInfo;
				json["storeVodSubChunksInfo"] = config.StoreVodSubChunksInfo;
				json["vodInfoHudFontSize"] = config.VodInfoHudFontSize;
				json["askWhenClosingWithActiveTasks"] = config.AskWhenClosingWithActiveTasks;
				json["apiApplicationTitle"] = config.ApiApplicationTitle;
				json["apiApplicationDescription"] = config.ApiApplicationDescription;
				json["apiApplicationClientId"] = config.ApiApplicationClientId;
				json["apiApplicationClientSecretKey"] = config.ApiApplicationClientSecretKey;
			};
			config.Loading += (s, json) =>
			{
				config.DownloadDirectory = json.Value<string>("downloadDirectory");
				config.LastUsedDirectory = json.Value<string>("lastUsedDirectory");
				config.BrowserExeFilePath = json.Value<string>("webBrowserExeFilePath");

				{
					JToken jt = json.Value<JToken>("outputFileNameFormat");
					if (jt != null)
					{
						config.OutputFileNameFormat = jt.Value<string>();
						if (string.IsNullOrWhiteSpace(config.OutputFileNameFormat))
						{
							config.OutputFileNameFormat = FILENAME_FORMAT_DEFAULT;
						}
					}
					else
					{
						config.OutputFileNameFormat = FILENAME_FORMAT_DEFAULT;
					}
				}
				{
					JToken jt = json.Value<JToken>("useGmtTime");
					config.UseGmtVodDates = jt == null || jt.Value<bool>();
				}
				{
					JToken jt = json.Value<JToken>("saveVodInfo");
					config.SaveVodInfo = jt == null || jt.Value<bool>();
				}
				{
					JToken jt = json.Value<JToken>("saveVodChunkInfo");
					config.SaveVodChunkInfo = jt == null || jt.Value<bool>();
				}
				{
					JToken jt = json.Value<JToken>("storeVodSubChunksInfo");
					config.StoreVodSubChunksInfo = jt == null || jt.Value<bool>();
				}
				{
					JToken jt = json.Value<JToken>("vodInfoHudFontSize");
					if (jt != null)
					{
						int min = (int)numericUpDownVodInfoHudFontSize.Minimum;
						int max = (int)numericUpDownVodInfoHudFontSize.Maximum;
						config.VodInfoHudFontSize = Clamp(jt.Value<int>(), min, max);
					}
				}
				{
					JToken jt = json.Value<JToken>("askWhenClosingWithActiveTasks");
					config.AskWhenClosingWithActiveTasks = jt == null || jt.Value<bool>();
				}

				config.ApiApplicationTitle = json.Value<string>("apiApplicationTitle");
				config.ApiApplicationDescription = json.Value<string>("apiApplicationDescription");
				config.ApiApplicationClientId = json.Value<string>("apiApplicationClientId");
				config.ApiApplicationClientSecretKey = json.Value<string>("apiApplicationClientSecretKey");

				if (string.IsNullOrWhiteSpace(config.ApiApplicationClientId) && string.IsNullOrWhiteSpace(config.ApiApplicationClientSecretKey))
				{
					SetDefaultTwitchApplication();
				}
			};
			config.Loaded += s =>
			{
				checkBoxUseGmtTime.Checked = config.UseGmtVodDates;
				checkBoxSaveVodInfo.Checked = config.SaveVodInfo;
				checkBoxAutomaticallySaveVodChunkInfo.Checked = checkBoxStoreSubChunksInfo.Enabled = config.SaveVodChunkInfo;
				checkBoxStoreSubChunksInfo.Checked = config.StoreVodSubChunksInfo;
				textBoxDownloadDirectory.Text = config.DownloadDirectory;
				textBoxOutputFileNameFormat.Text = config.OutputFileNameFormat;
				textBoxBrowserExePath.Text = config.BrowserExeFilePath;
				numericUpDownVodInfoHudFontSize.Value = config.VodInfoHudFontSize;
				checkBoxAskWhenClosingWithActiveTasks.Checked = config.AskWhenClosingWithActiveTasks;
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
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Ошибка!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				try
				{
					if (File.Exists(config.UrlListFilePath))
					{
						string[] strings = File.ReadAllLines(config.UrlListFilePath);
						textBoxVideoUrls.Lines = strings;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Ошибка!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				if (!config.DebugMode)
				{
					tabControlMain.TabPages.Remove(tabPageDebug);
				}
			};
			config.Load();

			tabControlMain.SelectedTab = tabPageSearch;
		}

		private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_isClosing) { e.Cancel = true; return; }
			if (IsUnfinishedTaskPresent())
			{
				e.Cancel = true;
				bool canClose = true;
				if (config.AskWhenClosingWithActiveTasks && e.CloseReason == CloseReason.UserClosing)
				{
					string msg = $"Скачивание не завершено!{Environment.NewLine}Остановить скачивание и закрыть программу?";
					if (MessageBox.Show(msg, "Вопрошающий вопрос",
						MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
					{
						canClose = false;
					}
				}

				if (canClose)
				{
					StopAllTasks();
					await Task.Run(() =>
					{
						bool unfinished = true;
						do
						{
							Thread.Sleep(200);
							Invoke(new MethodInvoker(() => { unfinished = IsUnfinishedTaskPresent(); }));
						}
						while (unfinished);
					});

					_isClosing = false;
					Close();
				}
			}
		}

		private void form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			ClearVodFrames();
			ClearDownloadFrames();
			try
			{
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
#if DEBUG
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
#else
			catch { }
#endif
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

			_activeFrameStream = null;
		}

		private async void btnSearchByChannelName_Click(object sender, EventArgs e)
		{
			btnSearchByChannelName.Enabled = false;
			if (!IsTwitchApplicationValid(out string errorMessage))
			{
				MessageBox.Show(errorMessage, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				btnSearchByChannelName.Enabled = true;
				return;
			}

			string channelName = textBoxChannelName.Text?.Trim();
			if (string.IsNullOrWhiteSpace(channelName))
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

			List<TwitchVodResult> vodResults = await Task.Run(() =>
			{
				List<TwitchVodResult> results = userResult.User.GetVideosMultiThreaded(limit).ToList();
				if (results.Count > 0)
				{
					results.Sort((x, y) => x.Vod.CreationDate > y.Vod.CreationDate ? -1 : 1);
				}
				return results;
			});

			if (vodResults.Count > 0)
			{
				listBoxEventLog.Items.Add($"Найдено {vodResults.Count} видео");

				var successfulVods = vodResults.Where(item => item.ErrorCode == 200).Select(item => item.Vod);
				foreach (TwitchVod vod in successfulVods)
				{
					listBoxEventLog.Items.Add($"Создание фрейма для видео {vod.Id} \"{vod.Title}\"...");
					AddStreamItem(vod);
				}

				int errorCount = vodResults.Where(item => item.ErrorCode != 200).Count();
				if (errorCount > 0)
				{
					listBoxEventLog.Items.Add($"Количество ошибок: {errorCount}");
				}

				int successfulVodCount = successfulVods.Count();
				if (successfulVodCount > 0)
				{
					tabPageStreams.Text = $"Стримы ({successfulVodCount})";
					StackVodFrames();
					tabControlMain.SelectedTab = tabPageStreams;
				}
			}
			else
			{
				listBoxEventLog.Items.Add("Видео не найдены!");
			}

			btnSearchByChannelName.Enabled = true;
		}

		private async void btnSearchByUrls_Click(object sender, EventArgs e)
		{
			btnSearchByUrls.Enabled = false;
			string[] urls = textBoxVideoUrls.Lines;
			if (urls.Length == 0)
			{
				MessageBox.Show("Введите ссылки!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				btnSearchByUrls.Enabled = true;
				return;
			}

			if (!IsTwitchApplicationValid(out string errorMessage))
			{
				MessageBox.Show(errorMessage, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				if (string.IsNullOrWhiteSpace(urls[i]))
				{
					listBoxEventLog.Items.Add($"{i + 1} / {urls.Length}: Empty URL!");
					continue;
				}

				string vodId = ExtractVodIdFromUrl(urls[i]);
				if (string.IsNullOrWhiteSpace(vodId))
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
			try
			{
				using (FolderBrowserDialog fbd = new FolderBrowserDialog()
				{
					Description = "Выберите папку для скачивания",
					SelectedPath = (!string.IsNullOrWhiteSpace(config.DownloadDirectory) &&
						Directory.Exists(config.DownloadDirectory)) ?
						config.DownloadDirectory : config.SelfDirectory
				})
				{
					if (fbd.ShowDialog() == DialogResult.OK)
					{
						textBoxDownloadDirectory.Text =
						config.LastUsedDirectory =
						config.DownloadDirectory = fbd.SelectedPath;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnRestoreDefaultOutputFileNameFormat_Click(object sender, EventArgs e)
		{
			textBoxOutputFileNameFormat.Text =
			config.OutputFileNameFormat = FILENAME_FORMAT_DEFAULT;
		}

		private void btnSelectBrowser_Click(object sender, EventArgs e)
		{
			try
			{
				using (OpenFileDialog ofd = new OpenFileDialog()
				{
					Title = "Выберите браузер",
					Filter = "EXE-files|*.exe",
					InitialDirectory = string.IsNullOrWhiteSpace(config.BrowserExeFilePath) ?
						config.SelfDirectory : Path.GetFullPath(config.BrowserExeFilePath)
				})
				{
					if (ofd.ShowDialog() != DialogResult.Cancel)
					{
						textBoxBrowserExePath.Text =
						config.BrowserExeFilePath = ofd.FileName;
						config.LastUsedDirectory = Path.GetDirectoryName(ofd.FileName);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnAddChannelToList_Click(object sender, EventArgs e)
		{
			try
			{
				string channelName = textBoxChannelName.Text.Trim();
				if (string.IsNullOrWhiteSpace(channelName))
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

			if (IsTwitchApplicationValid(out string errorMessage))
			{
				await Task.Run(() =>
				{
					TwitchApplication application = TwitchApi.GetApplication();
					TwitchApiLib.Utils.TwitchHelixOauthToken.Update(application, out errorMessage);
				});
			}

			if (!string.IsNullOrWhiteSpace(errorMessage))
			{
				MessageBox.Show(errorMessage, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

		private void checkBoxAutomaticallySaveVodChunkInfo_CheckedChanged(object sender, EventArgs e)
		{
			config.SaveVodChunkInfo = checkBoxStoreSubChunksInfo.Enabled = checkBoxAutomaticallySaveVodChunkInfo.Checked;
		}

		private void checkBoxStoreSubChunksInfo_CheckedChanged(object sender, EventArgs e)
		{
			config.StoreVodSubChunksInfo = checkBoxStoreSubChunksInfo.Checked;
		}

		private void checkBoxAskWhenClosingWithActiveTasks_CheckedChanged(object sender, EventArgs e)
		{
			config.AskWhenClosingWithActiveTasks = checkBoxAskWhenClosingWithActiveTasks.Checked;
		}

        private void numericUpDownVodInfoHudFontSize_ValueChanged(object sender, EventArgs e)
        {

			config.VodInfoHudFontSize = (int)numericUpDownVodInfoHudFontSize.Value;
            foreach (VodFrame frame in vodFrames)
            {
				frame.HudFontSize = config.VodInfoHudFontSize;
            }
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
			try
			{
				string fixedFlleName = FixFileName(FormatFileName(config.OutputFileNameFormat, _activeFrameStream.Vod));
				using (SaveFileDialog sfd = new SaveFileDialog()
				{
					Title = "Куда будем сохранять картинку?",
					Filter = "JPG-files|*.jpg",
					DefaultExt = ".jpg",
					FileName = fixedFlleName + "_thumbnail"
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
						_activeFrameStream.Vod.ThumbnailImageData.SaveToFile(sfd.FileName);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void miOpenVideoInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(config.BrowserExeFilePath))
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
				process.StartInfo.Arguments = _activeFrameStream.Vod.Url;
				process.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void miCopyVodThumbnailImageUrlToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string url = _activeFrameStream.Vod?.FormatThumbnailTemplateUrl(1920, 1080);
			if (!string.IsNullOrWhiteSpace(url))
			{
				SetClipboardText(url);
			}
		}

		private void miCopyVideoUrl_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(_activeFrameStream?.Vod?.Url))
			{
				SetClipboardText(_activeFrameStream.Vod.Url);
			}
		}

		private void miCopyVodInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(_activeFrameStream?.Vod?.RawData))
			{
				SetClipboardText(_activeFrameStream.Vod.RawData);
			}
			else
			{
				MessageBox.Show("Информация о стриме пуста!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void miSaveVodPlaylistAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				string playlistRaw = _activeFrameStream.Vod?.Playlist?.PlaylistRaw;
				if (!string.IsNullOrWhiteSpace(playlistRaw))
				{
					string fixedFileName = FixFileName(FormatFileName(config.OutputFileNameFormat, _activeFrameStream.Vod));
					using (SaveFileDialog sfd = new SaveFileDialog()
					{
						Title = "Куда будем сохранять плейлист?",
						Filter = "M3U8-files|*.m3u8",
						DefaultExt = ".m3u8",
						FileName = fixedFileName + "_playlist"
					})
					{
						if (!string.IsNullOrWhiteSpace(config.DownloadDirectory) &&
							Directory.Exists(config.DownloadDirectory))
						{
							sfd.InitialDirectory = config.DownloadDirectory;
						}

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
			config.DownloadDirectory = (sender as TextBox).Text;
		}

		private void textBoxOutputFileNameFormat_Leave(object sender, EventArgs e)
		{
			config.OutputFileNameFormat = (sender as TextBox).Text;
		}

		private void AddStreamItem(TwitchVod vod)
		{
			VodFrame frame = new VodFrame(vod) { Parent = panelStreams, HudFontSize = config.VodInfoHudFontSize };
			frame.Activated += OnVodFrame_Activated;
			frame.ImageMouseDown += OnVodFrame_ThumbnailImageMouseDown;
			frame.DownloadButtonClicked += OnVodFrame_DownloadButtonClick;
			vodFrames.Add(frame);
		}

		private void OnFrameDownload_Closed(object sender)
		{
			int i = 0;
			for (; i < downloadFrames.Count; ++i)
			{
				if (downloadFrames[i] == sender) { break; }
			}

			if (i < downloadFrames.Count)
			{
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
		}

		private void OnVodFrame_Activated(object sender)
		{
			_activeFrameStream = sender as VodFrame;
			foreach (VodFrame frameStream in vodFrames)
			{
				frameStream.BackColor = frameStream == _activeFrameStream ?
					VodFrame.ColorActive : VodFrame.ColorInactive;
			}
		}

		private void OnVodFrame_ThumbnailImageMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				contextMenuVodThumbnailImage.Show(Cursor.Position);
			}
		}

		private async void OnVodFrame_DownloadButtonClick(object sender)
		{
			if (string.IsNullOrWhiteSpace(config.DownloadDirectory))
			{
				MessageBox.Show("Не указана папка для скачивания!", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			VodFrame frameStream = sender as VodFrame;
			frameStream.btnDownload.Enabled = false;

			TwitchPlaylistResult playlistResult = await Task.Run(() =>
			{
				if (frameStream.Vod.IsLive && frameStream.Vod.UpdatePlaylistManifest() == 200 &&
					frameStream.Vod.PlaylistManifest[0].UpdatePlaylist() == 200)
				{
					return new TwitchPlaylistResult(frameStream.Vod.PlaylistManifest[0].Playlist, 200, null);
				}

				TwitchPlaylistResult result = frameStream.Vod.GetPlaylist("chunked");
				return result.ErrorCode == 200 ? result :
					new TwitchPlaylistResult(frameStream.Vod.Playlist, frameStream.Vod.Playlist != null ? 200 : 404, null);
			});

			if (playlistResult.ErrorCode == 200)
			{
				if (config.DebugMode)
				{
					richTextBoxDebugLog.Text = playlistResult.Playlist.PlaylistRaw;
				}

				if (playlistResult.Playlist.Parse() > 0)
				{
					playlistResult.Playlist.FixMutedChunkUrls();

					DownloadFrame frame = new DownloadFrame(frameStream.Vod, playlistResult.Playlist)
					{
						Parent = panelDownloads,
						Location = new Point(0, 0),
						ChunkRangeFirstId = 0,
						ChunkRangeLastId = playlistResult.Playlist.Count - 1
					};
					frame.Closed += OnFrameDownload_Closed;
					downloadFrames.Add(frame);

					tabPageDownloads.Text = $"Скачивание ({downloadFrames.Count})";
					if (tabControlMain.SelectedTab == tabPageDownloads)
					{
						StackDownloadFrames();
					}
				}
				else
				{
					MessageBox.Show("Произошла ошибка обработки плейлиста или он оказался пуст!", "Ошибка!",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show($"Error {playlistResult.ErrorCode}", "Ошибка!",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			frameStream.btnDownload.Enabled = true;
		}

		private void ClearDownloadFrames()
		{
			foreach (DownloadFrame frame in downloadFrames)
			{
				frame.Dispose();
			}
			downloadFrames.Clear();
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
				const int gap = 4;
				int panelWidth = tabControlMain.Width - scrollBarStreams.Width - 10;
				int perRow = panelWidth / (w + gap);
				if (perRow <= 0) { perRow = 1; }
				int rowCount = vodFrames.Count / perRow;
				if (vodFrames.Count % perRow != 0) { rowCount++; }
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

				int j = (h + gap) * rowCount;
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

				return rowCount;
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
					int y = i * downloadFrames[i].Height - scrollBarDownloads.Value;
					downloadFrames[i].Location = new Point(0, y);
					downloadFrames[i].Width = Width + DownloadFrame.EXTRA_WIDTH - 40;
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

		private void SetDefaultTwitchApplication()
		{
			textBoxApiApplicationTitle.Text = config.ApiApplicationTitle = defaultApplication.Name;
			textBoxApiApplicationDescription.Text = config.ApiApplicationDescription = defaultApplication.Description;
			textBoxHelixApiClientId.Text = config.ApiApplicationClientId = defaultApplication.ClientId;
			textBoxHelixApiClientSecretKey.Text = config.ApiApplicationClientSecretKey = defaultApplication.ClientSecretKey;
			TwitchApi.SetApplication(defaultApplication);
		}

		private static bool IsUnfinishedTaskPresent()
		{
			return downloadFrames.Any(frame => frame.IsDownloading);
		}

		private static void StopAllTasks()
		{
			foreach (DownloadFrame frame in downloadFrames)
			{
				frame.AbortDownload();
			}
		}
    }
}
