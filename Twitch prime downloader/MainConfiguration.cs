using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Twitch_prime_downloader
{
    public sealed class MainConfiguration
    {
        public string SelfDirPath { get; set; }
        public string FileName { get; set; }
        public string DownloadingDirPath { get; set; }
        public string FileNameFormat { get; set; }
        public string TempDirPath { get; set; }
        public string LastUsedDirPath { get; set; }
        public string ChannelListFilePath { get; set; }
        public string BrowserExeFiLePath { get; set; }
        public string UrlListFilePath { get; set; }
        public int VodInfoGuiFontSize { get; set; }
        public bool UseLocalVodDate { get; set; }
        public bool SaveVodInfo { get; set; }
        public bool SaveChunksInfo { get; set; }
        public bool DebugMode { get; set; }

        public MainConfiguration()
        {
            SelfDirPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            FileName = SelfDirPath + "tpd_config.json";
            DebugMode = false;
        }

        public void LoadDefaults()
        {
            ChannelListFilePath = SelfDirPath + "tpd_channelList.txt";
            DownloadingDirPath = SelfDirPath;
            FileNameFormat = Utils.FILENAME_FORMAT_DEFAULT;
            LastUsedDirPath = SelfDirPath;
            UrlListFilePath = SelfDirPath + "tpd_urls.txt";
            BrowserExeFiLePath = "firefox.exe";
            UseLocalVodDate = false;
            SaveVodInfo = true;
            SaveChunksInfo = false;
        }

        public void Load()
        {
            LoadDefaults();
            if (File.Exists(FileName))
            {
                JObject json = JObject.Parse(File.ReadAllText(FileName));
                JToken jt = json.Value<JToken>("downloadingPath");
                if (jt != null)
                {
                    DownloadingDirPath = jt.Value<string>();
                }
                jt = json.Value<JToken>("tempPath");
                if (jt != null)
                {
                    TempDirPath = jt.Value<string>();
                }
                jt = json.Value<JToken>("lastUsedPath");
                if (jt != null)
                {
                    LastUsedDirPath = jt.Value<string>();
                }
                jt = json.Value<JToken>("fileNameFormat");
                if (jt != null)
                {
                    FileNameFormat = jt.Value<string>();
                    if (string.IsNullOrEmpty(FileNameFormat))
                    {
                        FileNameFormat = Utils.FILENAME_FORMAT_DEFAULT;
                    }
                }
                jt = json.Value<JToken>("browserExe");
                if (jt != null)
                {
                    BrowserExeFiLePath = jt.Value<string>();
                }

                jt = json.Value<JToken>("useLocalVodDate");
                UseLocalVodDate = jt != null ? jt.Value<bool>() : false;

                jt = json.Value<JToken>("saveVideoInfo");
                if (jt != null)
                {
                    SaveVodInfo = jt.Value<bool>();
                }
                jt = json.Value<JToken>("saveChunksInfo");
                if (jt != null)
                {
                    SaveChunksInfo = jt.Value<bool>();
                }
            }
        }

        public void Save()
        {
            JObject json = new JObject();
            json["downloadingPath"] = DownloadingDirPath;
            json["tempPath"] = TempDirPath;
            json["fileNameFormat"] = FileNameFormat;
            json["lastUsedPath"] = LastUsedDirPath;
            json["browserExe"] = BrowserExeFiLePath;
            json["useLocalVodDate"] = UseLocalVodDate;
            json["saveVideoInfo"] = SaveVodInfo;
            json["saveChunksInfo"] = SaveChunksInfo;
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
            File.WriteAllText(FileName, json.ToString());
        }
    }
}
