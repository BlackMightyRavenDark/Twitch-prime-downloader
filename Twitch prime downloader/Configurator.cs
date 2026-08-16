#if DEBUG
using System;
#endif
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Twitch_prime_downloader
{
	internal sealed class Configurator
	{
		internal string ConfigurationFilePath { get; }
		internal string SelfExeFilePath { get; }
		internal string SelfDirectory { get; }
		internal string DownloadDirectory { get; set; }
		internal string OutputFileNameFormat { get; set; }
		internal string LastUsedDirectory { get; set; }
		internal string ChannelListFilePath { get; set; }
		internal string BrowserExeFilePath { get; set; }
		internal string UrlListFilePath { get; set; }
		internal int VodInfoHudFontSize { get; set; }
		internal bool UseGmtVodDates { get; set; }
		internal bool SaveVodInfo { get; set; }
		internal bool SaveVodChunkInfo { get; set; }
		internal bool StoreVodSubChunksInfo { get; set; }
		internal bool AskWhenClosingWithActiveTasks { get; set; }
		internal bool DebugMode { get; set; }
		internal string ApiApplicationTitle { get; set; }
		internal string ApiApplicationDescription { get; set; }
		internal string ApiApplicationClientId { get; set; }
		internal string ApiApplicationClientSecretKey { get; set; }

		internal delegate void SavingDelegate(object sender, JObject json);
		internal delegate void LoadingDelegate(object sender, JObject json);
		internal delegate void LoadedDelegate(object sender);
		internal SavingDelegate Saving;
		internal LoadingDelegate Loading;
		internal LoadedDelegate Loaded;

		internal Configurator()
		{
			SelfExeFilePath = Application.ExecutablePath;
			SelfDirectory = Path.GetDirectoryName(SelfExeFilePath);
			string fn = Path.GetFileNameWithoutExtension(SelfExeFilePath);
			ConfigurationFilePath = Path.Combine(SelfDirectory, fn + "_config.json");
			DebugMode = false;
		}

		internal void LoadDefaults()
		{
			string fn = Path.GetFileNameWithoutExtension(SelfExeFilePath);
			ChannelListFilePath = Path.Combine(SelfDirectory, fn + "_channelList.txt");
			DownloadDirectory = SelfDirectory;
			OutputFileNameFormat = Utils.FILENAME_FORMAT_DEFAULT;
			LastUsedDirectory = SelfDirectory;
			UrlListFilePath = Path.Combine(SelfDirectory, fn + "_urls.txt");
			BrowserExeFilePath = "firefox.exe";
			UseGmtVodDates = true;
			SaveVodInfo = true;
			SaveVodChunkInfo = false;
			StoreVodSubChunksInfo = true;
			AskWhenClosingWithActiveTasks = true;
		}

		internal void Load()
		{
			LoadDefaults();
			try
			{
				if (File.Exists(ConfigurationFilePath))
				{
					JObject json = JObject.Parse(File.ReadAllText(ConfigurationFilePath));
					Loading?.Invoke(this, json);
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
			Loaded?.Invoke(this);
		}

		internal void Save()
		{
			try
			{
				JObject json = new JObject();
				Saving?.Invoke(this, json);
				if (File.Exists(ConfigurationFilePath))
				{
					File.Delete(ConfigurationFilePath);
				}
				File.WriteAllText(ConfigurationFilePath, json.ToString());
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
	}
}
