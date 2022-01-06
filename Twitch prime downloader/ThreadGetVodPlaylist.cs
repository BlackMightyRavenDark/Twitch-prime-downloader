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
        private List<TwitchVodChunk> _chunks;
        public TwitchVodChunk[] Chunks => _chunks.ToArray();
        public TwitchVod StreamInfo { get; private set; }
        public string[] PlaylistParsed { get; private set; }
        public List<object> controls = new List<object>();
        
        public delegate void ThreadCompletedDelegate(object sender, int errorCode);
        public ThreadCompletedDelegate ThreadCompleted;

        public ThreadGetVodPlaylist(TwitchVod streamInfo)
        {
            StreamInfo = streamInfo;
            _chunks = new List<TwitchVodChunk>();
        }

        public void Work(object context)
        {
            ErrorCode = GetVodPlaylistUrl(StreamInfo, out string playlistUrl);
            if (ErrorCode == 200)
            {
                PlaylistUrl = playlistUrl;
                ErrorCode = DownloadString(PlaylistUrl, out string playlistString);
                if (ErrorCode == 200)
                {
                    PlaylistString = playlistString;
                    PlaylistParsed = PlaylistString.Split(new string[] { "\n" },
                        StringSplitOptions.RemoveEmptyEntries).Where(s => s.EndsWith(".ts")).ToArray();
                    for (int i = 0; i < PlaylistParsed.Length; i++)
                    {
                        TwitchVodChunk chunk = new TwitchVodChunk(PlaylistParsed[i]);
                        if (chunk.GetState() == TwitchVodChunkState.Unmuted)
                        {
                            chunk.SetState(TwitchVodChunkState.Muted);
                        }
                        _chunks.Add(chunk);
                    }
                }
            }

            if (ThreadCompleted != null && context != null)
            {
                (context as SynchronizationContext).Send(OnComplete_Context, this);
            }
        }
        
        private void OnComplete_Context(object obj)
        {
            ThreadCompleted.Invoke(obj, ErrorCode);
        }
    }
}
