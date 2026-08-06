
namespace Twitch_prime_downloader
{
	internal class DownloadProgressItem
	{
		public int TaskId { get; }
		public TwitchVodChunkDownloader ChunkDownloader { get; }
		public long ChunkSize { get; }
		public long DownloadedSize { get; }
		public int ErrorCode { get; }
		public DownloadItemState State { get; }

		public DownloadProgressItem(int taskId, TwitchVodChunkDownloader chunkDownloader,
			long chunkSize, long downloadedSize, int errorCode, DownloadItemState state)
		{
			TaskId = taskId;
			ChunkDownloader = chunkDownloader;
			ChunkSize = chunkSize;
			DownloadedSize = downloadedSize;
			ErrorCode = errorCode;
			State = state;
		}
	}

	internal enum DownloadItemState { Preparing, Connecting, Downloading, Finished, Errored }
}
