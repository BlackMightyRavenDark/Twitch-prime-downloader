using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Globalization;
using Newtonsoft.Json.Linq;
using static Twitch_prime_downloader.TwitchApi;

namespace Twitch_prime_downloader
{
    public static class Utils
    {
        public static List<FrameStream> framesStream = new List<FrameStream>();
        public static List<FrameDownload> framesDownload = new List<FrameDownload>();
        public static List<string> twitchClientIDs = new List<string>();
        public static List<string> twitchArchiveUrls = new List<string>();
        public static TwitchHelixOauthToken twitchHelixOauthToken = new TwitchHelixOauthToken();
        
        public static MainConfiguration config = new MainConfiguration();

        public static Random random = new Random((int)DateTime.Now.Ticks);

        public const string FILENAME_FORMAT_DEFAULT = "<channel_name> [<year>-<month>-<day>] <video_title>";

        public enum DownloadingMode { WholeFile, Chunked };

        public static WebClient GetTwitchWebClient_Kraken(string clientId)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add("Client-ID", clientId);
            wc.Headers.Add("Accept", TWITCH_ACCEPT_V5_STRING);
            wc.Encoding = Encoding.UTF8;
            return wc;
        }

        public static WebClient GetTwitchWebClient_Helix(string clientId)
        {
            if (GetHelixOauthToken(out string token) == 200)
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Client-ID", clientId);
                wc.Headers.Add("Authorization", "Bearer " + token);
                wc.Encoding = Encoding.UTF8;
                return wc;
            }
            return null;
        }

        public static int DownloadString(string url, out string resString)
        {
            WebClient wc = new WebClient();
            if (wc == null)
            {
                resString = string.Empty;
                return 400;
            }
            int res = DownloadString(wc, url, out resString);
            wc.Dispose();
            return res;
        }

        public static int DownloadString(WebClient webClient, string url, out string resString)
        {
            if (webClient == null)
            {
                resString = "Client error";
                return 400;
            }

            int errorCode;
            string t;
            try
            {
                t = webClient.DownloadString(url);
                errorCode = 200;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)e.Response;
                    errorCode = (int)httpWebResponse.StatusCode;
                    t = httpWebResponse.StatusDescription;
                }
                else
                {
                    errorCode = 400;
                    t = "Client error";
                }
            }
            resString = t;
            return errorCode;
        }

        public static bool DownloadData(string url, Stream stream)
        {
            WebClient webClient = new WebClient();
            try
            {
                byte[] b = webClient.DownloadData(url);
                stream.Write(b, 0, b.Length);
                webClient.Dispose();
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            webClient.Dispose();
            return false;
        }

        public static int HttpsGet_Helix(string url, out string recvText)
        {
            recvText = string.Empty;
            int res = 400;
            for (int i = 0; i < twitchClientIDs.Count && res != 200; i++)
            {
                WebClient wc = GetTwitchWebClient_Helix(twitchClientIDs[i]);
                if (wc != null)
                {
                    res = DownloadString(wc, url, out recvText);
                    wc.Dispose();
                    if (res == 401)
                    {
                        twitchHelixOauthToken.Reset();
                    }
                }
            }

            return res;
        }

        public static int HttpsGet_Kraken(string url, out string recvText)
        {
            recvText = string.Empty;
            int res = 400;
            for (int i = 0; i < twitchClientIDs.Count && res != 200; i++)
            {
                WebClient wc = GetTwitchWebClient_Kraken(twitchClientIDs[i]);
                if (wc != null)
                {
                    res = DownloadString(wc, url, out recvText);
                    wc.Dispose();
                }
            }

            return res;
        }

        public static int HttpsPost(string url, out string recvText)
        {
            recvText = string.Empty;
            WebClient client = new WebClient();
            NameValueCollection values = new NameValueCollection();
            values.Add("grant_type", "client_credentials");
            int errorCode;
            try
            {
                byte[] response = client.UploadValues(url, values);
                recvText = Encoding.UTF8.GetString(response);
                errorCode = 200;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)e.Response;
                    errorCode = (int)httpWebResponse.StatusCode;
                }
                else
                {
                    errorCode = 400;
                }
            }
            client.Dispose();
            return errorCode;
        }

        public static int HttpsPost(string aUrl, string body, out string responseString)
        {
            responseString = "Client error";
            int res = 400;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(aUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("Client-ID", TWITCH_CLIENT_ID_PRIVATE);
            httpWebRequest.Method = "POST";
            StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
            try
            {
                streamWriter.Write(body);
                streamWriter.Close();
                streamWriter.Dispose();
            }
            catch
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
                return res;
            }
            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream());
                try
                {
                    responseString = streamReader.ReadToEnd();
                    streamReader.Close();
                    streamReader.Dispose();
                    res = (int)httpResponse.StatusCode;
                }
                catch
                {
                    if (streamReader != null)
                    {
                        streamReader.Close();
                        streamReader.Dispose();
                    }
                    return 400;
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
                    res = res.Substring(0, n);
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

        public static TwitchStreamInfo ParseStreamInfo(string jsonString)
        {
            TwitchStreamInfo stream = new TwitchStreamInfo();
            JObject json = JObject.Parse(jsonString);
            string title = json.Value<string>("title").Replace("\n", string.Empty);
            while (title.EndsWith(" "))
            {
                title = title.Remove(title.Length - 1);
            }
            stream.title = title;
            stream.videoId = json.Value<string>("_id");
            if (stream.videoId.ToLower().StartsWith("v"))
            {
                stream.videoId = stream.videoId.Remove(0, 1);
            }
            stream.imagePreviewTemplateUrl = json.Value<JObject>("preview").Value<string>("template");
            if (stream.imagePreviewTemplateUrl.Contains(".tv/_404/404_"))
            {
                stream.imagePreviewTemplateUrl = json.Value<JObject>("preview").Value<string>("large");
            }
            stream.imageAnimatedPreviewUrl = json.Value<string>("animated_preview_url");
            stream.streamId = ExtractStreamIDFromImageURL(stream.imageAnimatedPreviewUrl);
            stream.userInfo.displayName = json.Value<JObject>("channel").Value<string>("display_name");
            stream.videoUrl = json.Value<string>("url");
            stream.vodType = json.Value<string>("broadcast_type");
            stream.length = DateTime.MinValue + TimeSpan.FromSeconds(int.Parse(json.Value<string>("length")));
            string t = json.Value<string>("created_at");
            stream.dateCreation = TwitchTimeToDateTime(t, false);
            JToken jt = json.Value<JToken>("delete_at");
            if (jt != null)
            {
                t = jt.Value<string>();
                stream.dateDeletion = TwitchTimeToDateTime(t, false);
            }
            else
            {
                stream.dateDeletion = DateTime.MinValue;
            }
            jt = json.Value<JToken>("muted_segments");
            if (jt != null)
            {
                ParseMutedSegments(jt as JArray, stream.mutedChunks);
            }
            int errorCode = IsChannelPrime(stream.userInfo.displayName.ToLower(), out bool prime);
            stream.isPrime = errorCode == 200 && prime;

            stream.gameInfo.title = json.Value<string>("game");

            int n = GetTwitchGameInfo(Uri.EscapeDataString(stream.gameInfo.title), out string gameInfo);
            if (n == 200)
            {
                JObject jsonGame = JObject.Parse(gameInfo);
                JArray ja = jsonGame.Value<JArray>("games");
                if (ja != null && ja.Count > 0)
                {
                    stream.gameInfo.ImagePreviewSmallURL = ja[0].Value<JObject>("box").Value<string>("small");
                    stream.gameInfo.ImagePreviewMediumURL = ja[0].Value<JObject>("box").Value<string>("medium");
                    stream.gameInfo.ImagePreviewLargeURL = ja[0].Value<JObject>("box").Value<string>("large");
                }
                else
                {
                    stream.gameInfo.ImagePreviewSmallURL = UNKNOWN_GAME_URL;
                    stream.gameInfo.ImagePreviewMediumURL = UNKNOWN_GAME_URL;
                    stream.gameInfo.ImagePreviewLargeURL = UNKNOWN_GAME_URL;
                }
            }
            else
            {
                stream.gameInfo.ImagePreviewSmallURL = UNKNOWN_GAME_URL;
                stream.gameInfo.ImagePreviewMediumURL = UNKNOWN_GAME_URL;
                stream.gameInfo.ImagePreviewLargeURL = UNKNOWN_GAME_URL;
            }
            stream.infoStringJson = jsonString;

            return stream;
        }

        public static void ParseMutedSegments(JArray jArray, TwitchVodMutedChunks mutedChunks)
        {
            mutedChunks.Clear();
            if (jArray != null && jArray.Count > 0)
            {
                for (int i = 0; i < jArray.Count; i++)
                {
                    int offset = jArray[i].Value<int>("offset");
                    int dur = jArray[i].Value<int>("duration");
                    mutedChunks.segments.Add(new TwitchVodMutedSegment() { offset = offset, duration = dur });
                    mutedChunks.totalLength = new DateTime(mutedChunks.totalLength.Ticks + TimeSpan.FromSeconds(dur).Ticks);
                    DateTime start = DateTime.MinValue + TimeSpan.FromSeconds(offset);
                    DateTime end = DateTime.MinValue + TimeSpan.FromSeconds(offset + dur);
                    mutedChunks.segmentList.Add($"{start:HH:mm:ss} - {end:HH:mm:ss}");
                }
            }
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
                int n = 1;
                do
                {
                    n++;
                    fnNew = dir + fn + "_" + n.ToString() + ext;
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

        public static string FormatFileName(string fmt, TwitchStreamInfo streamInfo)
        {
            return fmt.Replace("<year>", LeadZero(streamInfo.dateCreation.Year))
                .Replace("<month>", LeadZero(streamInfo.dateCreation.Month))
                .Replace("<day>", LeadZero(streamInfo.dateCreation.Day))
                .Replace("<video_title>", streamInfo.title)
                .Replace("<channel_name>", streamInfo.userInfo.displayName);
        }

        public static string FixFileName(string fn)
        {
            return fn.Replace("\\", "\u29F9").Replace("|", "\u2758").Replace("/", "\u2044")
                .Replace("?", "\u2753").Replace(":", "\uFE55").Replace("<", "\u227A").Replace(">", "\u227B")
                .Replace("\"", "\u201C").Replace("*", "\uFE61").Replace("^", "\u2303").Replace("\n", string.Empty);
        }

    }

    public class MainConfiguration
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
                    downloadingPath = jt.Value<string>();
                jt = json.Value<JToken>("tempPath");
                if (jt != null)
                    tempPath = jt.Value<string>();
                jt = json.Value<JToken>("lastUsedPath");
                if (jt != null)
                    lastUsedPath = jt.Value<string>();
                jt = json.Value<JToken>("fileNameFormat");
                if (jt != null)
                {
                    fileNameFormat = jt.Value<string>();
                    if (string.IsNullOrEmpty(fileNameFormat))
                        fileNameFormat = Utils.FILENAME_FORMAT_DEFAULT;
                }
                jt = json.Value<JToken>("browserExe");
                if (jt != null)
                    browserExe = jt.Value<string>();
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
                File.Delete(fileName);
            File.WriteAllText(fileName, json.ToString());
        }
    }

    public struct TwitchVodMutedSegment
    {
        public int offset;
        public int duration;
    }

    public class TwitchVodMutedChunks
    {
        public List<int> chunkIds = new List<int>();
        public List<TwitchVodMutedSegment> segments = new List<TwitchVodMutedSegment>();
        public List<string> segmentList = new List<string>();
        public DateTime totalLength = DateTime.MinValue;

        public void Clear()
        {
            chunkIds.Clear();
            segments.Clear();
            segmentList.Clear();
            totalLength = DateTime.MinValue;
        }
    }

    public enum TwitchVodChunkState { NotMuted, Muted, Unmuted };

    public class TwitchVodChunk
    {
        public string fileName;

        public TwitchVodChunk(string fileName)
        {
            this.fileName = fileName;
        }

        public string GetName()
        {
            return fileName.Substring(0, fileName.IndexOf(
                GetState() == TwitchVodChunkState.NotMuted ? ".ts" : "-"));
        }

        public TwitchVodChunkState GetState()
        {
            if (fileName.EndsWith("-muted.ts"))
            {
                return TwitchVodChunkState.Muted;
            }
            else if (fileName.EndsWith("-unmuted.ts"))
            {
                return TwitchVodChunkState.Unmuted;
            }
            return TwitchVodChunkState.NotMuted;
        }

        public void SetState(TwitchVodChunkState state)
        {
            switch (state)
            {
                case TwitchVodChunkState.Muted:
                    fileName = GetName() + "-muted.ts";
                    break;

                case TwitchVodChunkState.Unmuted:
                    fileName = GetName() + "-unmuted.ts";
                    break;

                case TwitchVodChunkState.NotMuted:
                    fileName = GetName() + ".ts";
                    break;
            }
        }

        public TwitchVodChunkState NextState()
        {
            switch (GetState())
            {
                case TwitchVodChunkState.Muted:
                    SetState(TwitchVodChunkState.NotMuted);
                    return TwitchVodChunkState.NotMuted;

                case TwitchVodChunkState.NotMuted:
                    SetState(TwitchVodChunkState.Unmuted);
                    return TwitchVodChunkState.Unmuted;

                case TwitchVodChunkState.Unmuted:
                    SetState(TwitchVodChunkState.Muted);
                    return TwitchVodChunkState.Muted;
            }
            return TwitchVodChunkState.NotMuted;
        }
    }
}
