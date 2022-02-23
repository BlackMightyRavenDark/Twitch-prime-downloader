using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using System.Globalization;
using Newtonsoft.Json.Linq;
using static Twitch_prime_downloader.TwitchApi;

namespace Twitch_prime_downloader
{
    public static class Utils
    {
        public static List<FrameStream> framesStream = new List<FrameStream>();
        public static List<FrameDownloading> framesDownloading = new List<FrameDownloading>();
        public static List<string> twitchArchiveUrls = new List<string>();

        public static readonly MainConfiguration config = new MainConfiguration();

        public static readonly Random random = new Random((int)DateTime.Now.Ticks);

        public const string FILENAME_FORMAT_DEFAULT = "<channel_name> [<year>-<month>-<day>] <video_title>";

        public static int DownloadString(string url, out string resString)
        {
            FileDownloader d = new FileDownloader();
            d.Url = url;
            return d.DownloadString(out resString);
        }

        public static int HttpsPost(string url, out string responseString)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseString = streamReader.ReadToEnd();
                    return (int)httpResponse.StatusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
                    responseString = ex.Message;
                    return (int)httpWebResponse.StatusCode;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            responseString = "Client error";
            return 400;
        }

        public static int HttpsPost(string url, string body, out string responseString)
        {
            responseString = "Client error";
            int res = 400;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Client-ID", TWITCH_CLIENT_ID_PRIVATE);
                httpWebRequest.Method = "POST";
                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(body);
                }
                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                try
                {
                    using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        responseString = streamReader.ReadToEnd();
                        res = (int)httpResponse.StatusCode;
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
                        responseString = ex.Message;
                        res = (int)httpWebResponse.StatusCode;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                res = ex.HResult;
            }
            return res;
        }

        public static string ExtractStreamIDFromImageURL(string url)
        {
            string res = string.Empty;
            int n = url.IndexOf(".tv/");
            if (n > 0)
            {
                res = url.Remove(0, n + 5);
            }
            else
            {
                n = url.IndexOf("vods/");
                if (n > 0)
                {
                    res = url.Remove(0, n + 5);
                    n = res.IndexOf("/");
                    string t = res.Substring(n + 1);
                    res = t.Substring(0, t.IndexOf('/'));
                }
                else
                {
                    n = url.IndexOf(".net/");
                    if (n > 0)
                    {
                        res = url.Remove(0, n + 5);
                        res = res.Substring(0, res.IndexOf("/"));
                    }
                }
            }

            return res;
        }

        public static string ExtractVodIdFromUrl(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                string host = !string.IsNullOrEmpty(uri.Host) ? uri.Host.ToLower() : null;
                if (string.IsNullOrEmpty(host) || !host.Contains("twitch.tv"))
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(uri.LocalPath) && uri.LocalPath.ToLower().StartsWith("/videos/"))
                {
                    string[] strings = uri.LocalPath.Split('/');
                    string t = strings[strings.Length - 1];
                    int n = t.IndexOf("&");
                    return n < 0 ? t : t.Substring(0, n);
                }
                return null;            
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string FormatSize(long n)
        {
            const int KB = 1000;
            const int MB = 1000000;
            const int GB = 1000000000;
            const long TB = 1000000000000;
            long b = n % KB;
            long kb = (n % MB) / KB;
            long mb = (n % GB) / MB;
            long gb = (n % TB) / GB;

            if (n >= 0 && n < KB)
                return string.Format("{0} b", b);
            if (n >= KB && n < MB)
                return string.Format("{0},{1:D3} KB", kb, b);
            if (n >= MB && n < GB)
                return string.Format("{0},{1:D3},{2:D3} MB", mb, kb, b);
            if (n >= GB && n < TB)
                return string.Format("{0},{1:D3},{2:D3},{3:D3} GB", gb, mb, kb, b);

            return string.Format("{0} {1:D3} {2:D3} {3:D3} bytes", gb, mb, kb, b);
        }

        public static string ExtractUrlFilePath(string url)
        {
            return url.Substring(0, url.LastIndexOf("/") + 1);
        }

        public static DateTime TwitchTimeToDateTime(string t, bool localTime)
        {
            return DateTime.ParseExact(t, "MM/dd/yyyy HH:mm:ss",
                CultureInfo.InvariantCulture,
                localTime ? DateTimeStyles.AssumeUniversal : DateTimeStyles.AssumeLocal);
        }

        public static string GetNumberedFileName(string fnOrig)
        {
            if (File.Exists(fnOrig))
            {
                string dir = Path.GetDirectoryName(fnOrig) + "\\";
                string fn = Path.GetFileNameWithoutExtension(fnOrig);
                string ext = Path.GetExtension(fnOrig);
                string fnNew;
                int n = 2;
                do
                {
                    fnNew = $"{dir}{fn}_{n++}{ext}";
                } while (File.Exists(fnNew));
                return fnNew;
            }

            return fnOrig;
        }

        public static string GetNumberedDirectoryName(string dirPathOrig)
        {
            if (dirPathOrig.EndsWith("\\"))
            {
                dirPathOrig = dirPathOrig.Remove(dirPathOrig.Length - 1, 1);
            }
            if (Directory.Exists(dirPathOrig))
            {
                int n = 2;
                string dirPathNew;
                do
                {
                    dirPathNew = $"{dirPathOrig}_{n++}";
                }
                while (Directory.Exists(dirPathNew));
                return dirPathNew + "\\";
            }
            return dirPathOrig.EndsWith("\\") ? dirPathOrig : dirPathOrig + "\\";
        }

        public static Color IntToColor(int color)
        {
            //TODO: Rewrite this shit
            byte[] values = BitConverter.GetBytes(color);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(values);
            }
            return Color.FromArgb(values[0], values[1], values[2]);
        }

        public static Bitmap GenerateErrorImage()
        {
            Bitmap bmp = new Bitmap(320, 180);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.Black, new RectangleF(0, 0, bmp.Width, bmp.Height));
            Font fnt = new Font("Arial", 12);
            Point center = new Point(bmp.Width / 2, bmp.Height / 2);
            int n = random.Next(10);
            if (n < 5)
            {
                string t = "matrix has you";
                SizeF sz = g.MeasureString(t, fnt);
                float yDraw = center.Y - sz.Height / 2.0f;
                g.DrawString(t, fnt, Brushes.Lime, center.X - sz.Width / 2.0f, yDraw);
                t = "fuck";
                sz = g.MeasureString(t, fnt);
                yDraw -= sz.Height;
                g.DrawString(t, fnt, Brushes.Lime, center.X - sz.Width / 2.0f, yDraw);
                t = "there is no image";
                sz = g.MeasureString(t, fnt);
                yDraw -= sz.Height;
                g.DrawString(t, fnt, Brushes.Lime, center.X - sz.Width / 2.0f, yDraw);
                t = "sorry :'(";
                sz = g.MeasureString(t, fnt);
                g.DrawString(t, fnt, Brushes.Lime, center.X - sz.Width / 2.0f, center.Y + sz.Height);
            }
            else
            {
                string t = "картинки нет, но вы там держитесь";
                SizeF sz = g.MeasureString(t, fnt);
                float x = center.X - sz.Width / 2.0f;
                g.DrawString(t, fnt, Brushes.Lime, x, center.Y - sz.Height);

                t = "хорошего настроения и здоровья";
                sz = g.MeasureString(t, fnt);
                x = bmp.Width / 2.0f - sz.Width / 2.0f;
                g.DrawString(t, fnt, Brushes.Lime, x, center.Y);
            }
            fnt.Dispose();

            return bmp;
        }

        public static void SetClipboardText(string text)
        {
            bool suc;
            do
            {
                try
                {
                    Clipboard.SetText(text);
                    suc = true;
                }
                catch
                {
                    suc = false;
                }
            } while (!suc); 
        }

        public static string LeadZero(int n)
        {
            return n < 10 ? $"0{n}" : n.ToString();
        }

        public static string FormatFileName(string fmt, TwitchVod twitchVod)
        {
            return fmt.Replace("<year>", LeadZero(twitchVod.DateCreation.Year))
                .Replace("<month>", LeadZero(twitchVod.DateCreation.Month))
                .Replace("<day>", LeadZero(twitchVod.DateCreation.Day))
                .Replace("<video_title>", twitchVod.Title)
                .Replace("<channel_name>", twitchVod.UserInfo.DisplayName);
        }

        public static string FixFileName(string fn)
        {
            return fn.Replace("\\", "\u29F9").Replace("|", "\u2758").Replace("/", "\u2044")
                .Replace("?", "\u2753").Replace(":", "\uFE55").Replace("<", "\u227A").Replace(">", "\u227B")
                .Replace("\"", "\u201C").Replace("*", "\uFE61").Replace("^", "\u2303").Replace("\n", string.Empty);
        }
    }

    public sealed class MainConfiguration
    {
        public string selfPath;
        public string fileName;
        public string downloadingPath;
        public string fileNameFormat;
        public string tempPath;
        public string lastUsedPath;
        public string channelsListFileName;
        public string browserExe;
        public string urlsFileName;
        public int streamInfoGUIFontSize;
        public bool debugMode;

        public MainConfiguration()
        {
            selfPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            fileName = selfPath + "tpd_config.json";
            debugMode = false;
        }

        public void LoadDefaults()
        {
            channelsListFileName = selfPath + "tpd_channelList.txt";
            downloadingPath = selfPath;
            fileNameFormat = Utils.FILENAME_FORMAT_DEFAULT;
            lastUsedPath = selfPath;
            urlsFileName = selfPath + "tpd_urls.txt";
            browserExe = "firefox.exe";
        }

        public void Load()
        {
            LoadDefaults();
            if (File.Exists(fileName))
            {
                JObject json = JObject.Parse(File.ReadAllText(fileName));
                JToken jt = json.Value<JToken>("downloadingPath");
                if (jt != null)
                {
                    downloadingPath = jt.Value<string>();
                }
                jt = json.Value<JToken>("tempPath");
                if (jt != null)
                {
                    tempPath = jt.Value<string>();
                }
                jt = json.Value<JToken>("lastUsedPath");
                if (jt != null)
                {
                    lastUsedPath = jt.Value<string>();
                }
                jt = json.Value<JToken>("fileNameFormat");
                if (jt != null)
                {
                    fileNameFormat = jt.Value<string>();
                    if (string.IsNullOrEmpty(fileNameFormat))
                    {
                        fileNameFormat = Utils.FILENAME_FORMAT_DEFAULT;
                    }
                }
                jt = json.Value<JToken>("browserExe");
                if (jt != null)
                {
                    browserExe = jt.Value<string>();
                }
            }
        }

        public void Save()
        {
            JObject json = new JObject();
            json["downloadingPath"] = downloadingPath;
            json["tempPath"] = tempPath;
            json["fileNameFormat"] = fileNameFormat;
            json["lastUsedPath"] = lastUsedPath;
            json["browserExe"] = browserExe;
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.WriteAllText(fileName, json.ToString());
        }
    }

    public enum DownloadingMode { WholeFile, Chunked };
}
