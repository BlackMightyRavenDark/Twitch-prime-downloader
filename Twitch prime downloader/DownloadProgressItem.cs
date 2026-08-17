
namespace Twitch_prime_downloader
{
	internal class DownloadProgressItem
	{
		internal int TaskId { get; }
		internal TwitchVodChunkDownloader ChunkDownloader { get; }
		internal long ChunkSize { get; }
		internal long DownloadedSize { get; }
		internal int ErrorCode { get; }
		internal DownloadItemState State { get; }

		internal DownloadProgressItem(int taskId, TwitchVodChunkDownloader chunkDownloader,
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
