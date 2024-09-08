using TwitchApiLib;

namespace Twitch_prime_downloader
{
	internal class TwitchVodChunkItem
	{
		public TwitchVodChunk Chunk { get; }

		public TwitchVodChunkItem(TwitchVodChunk chunk)
		{
			Chunk = chunk;
		}

		public override string ToString()
		{
			return Chunk.FileName;
		}
	}
}
