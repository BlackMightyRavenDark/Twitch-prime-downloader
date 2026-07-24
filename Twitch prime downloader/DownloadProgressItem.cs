using System.IO;

namespace Twitch_prime_downloader
{
	internal class DownloadProgressItem
	{
		public int TaskId { get; }
		public TwitchVodChunkWrapper Chunk { get; }
		public long ChunkSize { get; }
		public long DownloadedSize { get; }
		public Stream OutputStream { get; }
		public int ErrorCode { get; }
		public DownloadItemState State { get; }

		public DownloadProgressItem(int taskId, TwitchVodChunkWrapper chunk,
			long chunkSize, long downloadedSize, Stream outputStream,
			int errorCode, DownloadItemState state)
		{
			TaskId = taskId;
			Chunk = chunk;
			ChunkSize = chunkSize;
			DownloadedSize = downloadedSize;
			OutputStream = outputStream;
			ErrorCode = errorCode;
			State = state;
		}
	}

	internal enum DownloadItemState { Preparing, Connecting, Downloading, Finished, Errored }
}
