using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
    public class ThreadGetVodPlaylist
    {
        public string playlistUrl;
        public string playlistString;
        public int errorCode;
        public List<TwitchVodChunk> chunks = new List<TwitchVodChunk>();
        public TwitchStreamInfo _streamInfo;
        public List<string> resPlaylist;
        public List<object> controls = new List<object>();
        public event Action<object, int> Completed;

        public void Work(object aContext)
        {
            errorCode = GetStreamPlaylistUrl(_streamInfo, out playlistUrl);
            if (errorCode == 200)
            {
                errorCode = DownloadString(playlistUrl, out playlistString);
                if (errorCode == 200)
                {
                    resPlaylist = playlistString.Split(new string[] { "\n" },
                        StringSplitOptions.RemoveEmptyEntries).Where(s => s.EndsWith(".ts")).ToList();
                    for (int i = 0; i < resPlaylist.Count; i++)
                    {
                        TwitchVodChunk chunk = new TwitchVodChunk(resPlaylist[i]);
                        if (chunk.GetState() == TwitchVodChunkState.CS_UNMUTED)
                        {
                            chunk.SetState(TwitchVodChunkState.CS_MUTED);
                        }
                        chunks.Add(chunk);
                    }
                }
            }
            (aContext as SynchronizationContext).Send(OnComplete_Context, this);
        }
        
        private void OnComplete_Context(object obj)
        {
            Completed?.Invoke(obj, errorCode);
        }
    }
}
