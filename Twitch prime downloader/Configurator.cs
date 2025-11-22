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
		public string FileName { get; }
		public string SelfExePath { get; }
		public string SelfDirPath { get; }
		public string DownloadingDirPath { get; set; }
		public string FileNameFormat { get; set; }
		public string TempDirPath { get; set; }
		public string LastUsedDirPath { get; set; }
		public string ChannelListFilePath { get; set; }
		public string BrowserExeFilePath { get; set; }
		public string UrlListFilePath { get; set; }
		public int VodInfoGuiFontSize { get; set; }
		public bool UseGmtVodDates { get; set; }
		public bool SaveVodInfo { get; set; }
		public bool SaveVodChunksInfo { get; set; }
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
			SelfExePath = Application.ExecutablePath;
			SelfDirPath = Path.GetDirectoryName(SelfExePath);
			string fn = Path.GetFileNameWithoutExtension(SelfExePath);
			FileName = Path.Combine(SelfDirPath, fn + "_config.json");
			DebugMode = false;
		}

		public void LoadDefaults()
		{
			string fn = Path.GetFileNameWithoutExtension(SelfExePath);
			ChannelListFilePath = Path.Combine(SelfDirPath, fn + "_channelList.txt");
			DownloadingDirPath = SelfDirPath;
			FileNameFormat = Utils.FILENAME_FORMAT_DEFAULT;
			LastUsedDirPath = SelfDirPath;
			UrlListFilePath = Path.Combine(SelfDirPath, fn + "_urls.txt");
			BrowserExeFilePath = "firefox.exe";
			UseGmtVodDates = true;
			SaveVodInfo = true;
			SaveVodChunksInfo = false;
		}

		public void Load()
		{
			LoadDefaults();
			try
			{
				if (File.Exists(FileName))
				{
					JObject json = JObject.Parse(File.ReadAllText(FileName));
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
				if (File.Exists(FileName))
				{
					File.Delete(FileName);
				}
				File.WriteAllText(FileName, json.ToString());
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
