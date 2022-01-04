using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Twitch_prime_downloader.TwitchApi;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
    public class ThreadGetVodPlaylist
    {
        public string PlaylistUrl { get; private set; }
        public string PlaylistString { get; private set; }
        public int ErrorCode { get; private set; }
        public List<TwitchVodChunk> chunks = new List<TwitchVodChunk>();
        public TwitchStreamInfo StreamInfo { get; private set; }
        public List<string> resPlaylist;
        public List<object> controls = new List<object>();
        
        public delegate void ThreadCompletedDelegate(object sender, int errorCode);
        public ThreadCompletedDelegate ThreadCompleted;

        public ThreadGetVodPlaylist(TwitchStreamInfo streamInfo)
        {
            StreamInfo = streamInfo;
        }

        public void Work(object aContext)
        {
            ErrorCode = GetStreamPlaylistUrl(StreamInfo, out string playlistUrl);
            if (ErrorCode == 200)
            {
                PlaylistUrl = playlistUrl;
                ErrorCode = DownloadString(PlaylistUrl, out string playlistString);
                if (ErrorCode == 200)
                {
                    PlaylistString = playlistString;
                    resPlaylist = PlaylistString.Split(new string[] { "\n" },
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

            if (ThreadCompleted != null && aContext != null)
            {
                (aContext as SynchronizationContext).Send(OnComplete_Context, this);
            }
        }
        
        private void OnComplete_Context(object obj)
        {
            ThreadCompleted.Invoke(obj, ErrorCode);
        }
    }
}
