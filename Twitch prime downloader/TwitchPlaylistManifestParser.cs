using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace Twitch_prime_downloader
{
    public class TwitchPlaylistManifestParser
    {
        private string Manifest;
        private List<ManifestItem> manifestItems;

        public TwitchPlaylistManifestParser(string manifest)
        {
            Manifest = manifest;
            manifestItems = Parse();
        }

        private List<ManifestItem> Parse()
        {
            List<string> fixedList = GetFixedList();
            int max = fixedList.Count - 1;
            List<ManifestItem> resultList = new List<ManifestItem>();
            for (int i = 0; i < max; i += 2)
            {
                string str = fixedList[i].Substring(fixedList[i].IndexOf(':') + 1);
                Dictionary<string, string> dict = SplitStringToKeyValues(str, ",", '=');
                int bandwidth = dict.ContainsKey("BANDWIDTH") ? int.Parse(dict["BANDWIDTH"]) : 0;
                int videoWidth = 0;
                int videoHeight = 0;
                if (dict.ContainsKey("RESOLUTION"))
                {
                    string res = dict["RESOLUTION"];
                    string[] resSplitted = res.Split('x');
                    videoWidth = int.Parse(resSplitted[0]);
                    videoHeight = int.Parse(resSplitted[1]);
                }
                string codecs = dict.ContainsKey("CODECS") ? HttpUtility.UrlDecode(dict["CODECS"]) : null;
                string formatId = dict.ContainsKey("VIDEO") ? dict["VIDEO"] : null;
                string rate = dict.ContainsKey("FRAME-RATE") ? dict["FRAME-RATE"] : null;
                int frameRate = !string.IsNullOrEmpty(rate) && !string.IsNullOrWhiteSpace(rate) ?
                    (int)float.Parse(rate.Replace('.', ',')) : 0;
                string url = fixedList[i + 1];

                ManifestItem manifestItem = new ManifestItem(videoWidth, videoHeight, bandwidth,
                    frameRate, codecs, formatId, url);
                resultList.Add(manifestItem);
            }

            return resultList;
        }

        private List<string> GetFixedList()
        {
            string manifest = Manifest;
            Regex regex = new Regex("CODECS=\"(.+?)\"");
            MatchCollection matches = regex.Matches(manifest);
            for (int i = matches.Count - 1; i >= 0; --i)
            {
                Match match = matches[i];

                int len = 7;
                string tmp = match.Value.Substring(len);
                int actualPos = match.Index + len;
                int actualLength = match.Length - len;
                string encoded = HttpUtility.UrlEncode(tmp);
                manifest = manifest.Remove(actualPos, actualLength).Insert(actualPos, encoded);
            }

            List<string> resultList = new List<string>();
            string[] strings = manifest.Split('\n');
            foreach (string str in strings)
            {
                if (str.StartsWith("#EXT-X-STREAM-INF:") || str.StartsWith("http"))
                {
                    resultList.Add(str);
                }
            }
            return resultList;
        }

        public string FindPlaylistUrl(string formatId)
        {
            foreach (ManifestItem item in manifestItems)
            {
                if (item.FormatId == $"\"{formatId}\"")
                {
                    return item.PlaylistUrl;
                }
            }
            return null;
        }

        private static Dictionary<string, string> SplitStringToKeyValues(
            string inputString, string keySeparaor, char valueSeparator)
        {
            if (string.IsNullOrEmpty(inputString) || string.IsNullOrWhiteSpace(inputString))
            {
                return null;
            }
            string[] keyValues = inputString.Split(new string[] { keySeparaor }, StringSplitOptions.None);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < keyValues.Length; i++)
            {
                string[] t = keyValues[i].Split(valueSeparator);
                dict.Add(t[0], t[1]);
            }
            return dict;
        }
    }

    public sealed class ManifestItem
    {
        public int ResolutionWidth { get; private set; }
        public int ResolutionHeight { get; private set; }
        public int Bandwidth { get; private set; }
        public int FrameRate { get; private set; }
        public string Codecs { get; private set; }
        public string FormatId { get; private set; }
        public string PlaylistUrl { get; private set; }

        public ManifestItem(int resolutionWidth, int resolutionHeight, int bandwidth,
            int frameRate, string codecs, string formatId, string playlistUrl)
        {
            ResolutionWidth = resolutionWidth;
            ResolutionHeight = resolutionHeight;
            Bandwidth = bandwidth;
            FrameRate = frameRate;
            Codecs = codecs;
            FormatId = formatId;
            PlaylistUrl = playlistUrl;
        }
    }
}
