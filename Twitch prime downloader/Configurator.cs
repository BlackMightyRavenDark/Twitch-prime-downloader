#if DEBUG
using System;
#endif
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Twitch_prime_downloader
{
	public sealed class Configurator
	{
		public string ConfigurationFilePath { get; }
		public string SelfExeFilePath { get; }
		public string SelfDirectory { get; }
		public string DownloadDirectory { get; set; }
		public string OutputFileNameFormat { get; set; }
		public string LastUsedDirectory { get; set; }
		public string ChannelListFilePath { get; set; }
		public string BrowserExeFilePath { get; set; }
		public string UrlListFilePath { get; set; }
		public int VodInfoHudFontSize { get; set; }
		public bool UseGmtVodDates { get; set; }
		public bool SaveVodInfo { get; set; }
		public bool SaveVodChunkInfo { get; set; }
		public bool DebugMode { get; set; }
		public string ApiApplicationTitle { get; set; }
		public string ApiApplicationDescription { get; set; }
		public string ApiApplicationClientId { get; set; }
		public string ApiApplicationClientSecretKey { get; set; }

		public delegate void SavingDelegate(object sender, JObject json);
		public delegate void LoadingDelegate(object sender, JObject json);
		public delegate void LoadedDelegate(object sender);
		public SavingDelegate Saving;
		public LoadingDelegate Loading;
		public LoadedDelegate Loaded;

		public Configurator()
		{
			SelfExeFilePath = Application.ExecutablePath;
			SelfDirectory = Path.GetDirectoryName(SelfExeFilePath);
			string fn = Path.GetFileNameWithoutExtension(SelfExeFilePath);
			ConfigurationFilePath = Path.Combine(SelfDirectory, fn + "_config.json");
			DebugMode = false;
		}

		public void LoadDefaults()
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
		}

		public void Load()
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

		public void Save()
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
