using System.Collections.Generic;
using System.Globalization;
using MultiThreadedDownloaderLib;

namespace Twitch_prime_downloader
{
    public class VodPlaylistDownloader
    {
        public List<TwitchVodChunk> ParsedPlaylist { get; private set; }
        public string PlaylistUrl { get; private set; }
        public string PlaylistRaw { get; private set; }
        public TwitchVod TwitchVod { get; private set; }

        public delegate void WorkFinishedDelegate(object sender, int errorCode);

        public int DownloadAndParse(string vodId, string streamId,
            string imagePreviewUrl, bool isHighlight, WorkFinishedDelegate workFinished)
        {
            int errorCode = TwitchApi.GetVodPlaylistUrl(
                vodId, streamId, imagePreviewUrl, isHighlight, out string playlistUrl);
            if (errorCode == 200)
            {
                PlaylistUrl = playlistUrl;
                FileDownloader d = new FileDownloader() { Url = playlistUrl };
                errorCode = d.DownloadString(out string playlist);
                if (errorCode == 200)
                {
                    PlaylistRaw = playlist;
                    ParsedPlaylist = ParsePlaylist(playlist);
                    for (int i = 0; i < ParsedPlaylist.Count; ++i)
                    {
                        if (ParsedPlaylist[i].GetState() == TwitchVodChunkState.Unmuted)
                        {
                            ParsedPlaylist[i].SetState(TwitchVodChunkState.Muted);
                        }
                    }
                }
            }

            workFinished?.Invoke(this, errorCode);
            return errorCode;
        }

        public int DownloadAndParse(TwitchVod vod, WorkFinishedDelegate workFinished)
        {
            TwitchVod = vod;
            return DownloadAndParse(vod.VideoId, vod.StreamId,
                vod.ImagePreviewTemplateUrl, vod.IsHighlight(), workFinished);
        }

        private List<TwitchVodChunk> ParsePlaylist(string playlist)
        {
            List<TwitchVodChunk> chunks = new List<TwitchVodChunk>();

            string[] strings = playlist.Split('\n');
            int stringId = FindFirstChunkStringId(strings);
            double offset = 0.0;
            if (stringId >= 0)
            {
                for (; stringId < strings.Length; stringId += 2)
                {
                    string[] splitted = strings[stringId].Split(':');
                    if (splitted[0] != "#EXTINF") { continue; }
                    string[] lengthSplitted = splitted[1].Split(',');

                    NumberFormatInfo numberFormatInfo = new NumberFormatInfo() { NumberDecimalSeparator = "." };
                    double chunkLength = double.TryParse(lengthSplitted[0], NumberStyles.Any,
                        numberFormatInfo, out double d) ? d : 0.0;

                    TwitchVodChunk chunk = new TwitchVodChunk(strings[stringId + 1], offset, chunkLength);
                    chunks.Add(chunk);

                    offset += chunkLength;
                }
            }

            return chunks;
        }

        private int FindFirstChunkStringId(string[] strings)
        {
            for (int i = 0; i < 20 || i < strings.Length; ++i)
            {
                if (strings[i].StartsWith("#EXTINF:")) { return i; }
            }

            return -1;
        }
    }
}
