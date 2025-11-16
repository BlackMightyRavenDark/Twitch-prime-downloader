using System.IO;
using TwitchApiLib;

namespace Twitch_prime_downloader
{
	internal class DownloadProgressItem
	{
		public int TaskId { get; }
		public TwitchVodChunk VodChunk { get; }
		public long ChunkSize { get; }
		public long DownloadedSize { get; }
		public Stream OutputStream { get; }
		public int ErrorCode { get; }
		public DownloadItemState State { get; }

		public DownloadProgressItem(int taskId, TwitchVodChunk vodChunk,
			long chunkSize, long downloadedSize, Stream outputStream,
			int errorCode, DownloadItemState state)
		{
			TaskId = taskId;
			VodChunk = vodChunk;
			ChunkSize = chunkSize;
			DownloadedSize = downloadedSize;
			OutputStream = outputStream;
			ErrorCode = errorCode;
			State = state;
		}
	}

	internal enum DownloadItemState { Connecting, Downloading, Finished, Errored }
}
