using TwitchApiLib;

namespace Twitch_prime_downloader
{
	internal class TwitchVodChunkItem
	{
		internal TwitchVodChunk Chunk { get; }

		internal TwitchVodChunkItem(TwitchVodChunk chunk)
		{
			Chunk = chunk;
		}

		public override string ToString()
		{
			return Chunk.FileName;
		}
	}
}
