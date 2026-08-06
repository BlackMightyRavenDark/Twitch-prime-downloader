using System;
using System.IO;
using System.Threading;
using MultiThreadedDownloaderLib;
using TwitchApiLib;

namespace Twitch_prime_downloader
{
	internal class TwitchVodChunkDownloader : IDisposable
	{
		internal TwitchVodChunkWrapper ChunkWrapper { get; }
		internal Stream OutputStream { get; private set; }
		internal int ErrorCode { get; private set; }

		private readonly CancellationTokenSource _cancellationTokenSource;

		internal delegate void ChunkStateChangedDelegate(object sender, TwitchVodChunk chunk);

		internal TwitchVodChunkDownloader(TwitchVodChunkWrapper chunkWrapper, CancellationTokenSource cancellationTokenSource)
		{
			ChunkWrapper = chunkWrapper;
			_cancellationTokenSource = cancellationTokenSource;
			ErrorCode = DownloadAbstractor.DOWNLOAD_ERROR_UNDEFINED;
		}

		public void Dispose()
		{
			if (OutputStream != null)
			{
				OutputStream.Dispose();
				OutputStream = null;
			}
		}

		internal void DownloadChunk(FileDownloader fileDownloader, ChunkStateChangedDelegate chunkStateChanged)
		{
			try
			{
				ErrorCode = DownloadChunk(fileDownloader, ChunkWrapper.FileUrl);
				if (ErrorCode != 200 && !_cancellationTokenSource.IsCancellationRequested)
				{
					ChunkWrapper.SetNextState();
					chunkStateChanged?.Invoke(this, ChunkWrapper);
					ErrorCode = DownloadChunk(fileDownloader, ChunkWrapper.FileUrl);
					if (ErrorCode != 200 && !_cancellationTokenSource.IsCancellationRequested)
					{
						ChunkWrapper.SetNextState();
						chunkStateChanged?.Invoke(this, ChunkWrapper);
						ErrorCode = DownloadChunk(fileDownloader, ChunkWrapper.FileUrl);
					}
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
				Dispose();
			}
		}

		private int DownloadChunk(FileDownloader fileDownloader, string chunkUrl)
		{
			try
			{
				OutputStream = new MemoryStream();
				fileDownloader.Url = chunkUrl;
				return fileDownloader.Download(OutputStream, _cancellationTokenSource);
			}
			catch (Exception ex)
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
				return ex.HResult;
			}
		}
	}
}
