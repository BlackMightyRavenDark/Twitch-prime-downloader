using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MultiThreadedDownloaderLib;
using TwitchApiLib;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
	internal class DownloadAbstractor
	{
		public TwitchVodPlaylist VodPlaylist { get; }
		public int MaxGroupSize { get; set; }

		public delegate void GroupDownloadStartedDelegate(object sender, int groupSize);
		public delegate void GroupDownloadProgressedDelegate(object sender, IEnumerable<DownloadItem> groupItems);
		public delegate void GroupDownloadFinishedDelegate(object sender, IEnumerable<DownloadItem> groupItems, int errorCode);
		public delegate void ChunkMergingProgressedDelegate(object sender,
			long processedBytes, long totalSize,int chunkId, int chunkCount, DownloadMode downloadMode);
		public delegate void GroupMergingFinishedDelegate(object sender, IEnumerable<DownloadItem> groupItems, int errorCode);
		public delegate void ChunkChangedDelegate(object sender, TwitchVodChunk chunk, int chunkId);
		public delegate void DownloadCompletedDelegate(object sender, int errorCode);

		public const int DOWNLOAD_ERROR_CHUNK_RANGE = int.MaxValue;
		public const int DOWNLOAD_ERROR_GROUP_EMPTY = int.MaxValue - 1;

		private CancellationTokenSource _cancellationTokenSource;
		private CancellationToken _cancellationToken;

		public DownloadAbstractor(TwitchVodPlaylist vodPlaylist, int maxGroupSize)
		{
			VodPlaylist = vodPlaylist;
			MaxGroupSize = maxGroupSize;

			_cancellationTokenSource = new CancellationTokenSource();
			_cancellationToken = _cancellationTokenSource.Token;
		}

		public async Task<int> Download(
			string outputFilePath,
			int firstChunkId,
			int lastChunkId,
			DownloadMode downloadMode,
			GroupDownloadStartedDelegate groupDownloadStarted,
			GroupDownloadProgressedDelegate groupDownloadProgressed,
			GroupDownloadFinishedDelegate groupDownloadFinished,
			ChunkMergingProgressedDelegate chunkMergingProgressed,
			GroupMergingFinishedDelegate groupMergingFinished,
			ChunkChangedDelegate chunkChanged,
			DownloadCompletedDelegate downloadCompleted)
		{
			if (lastChunkId >= VodPlaylist.Count)
			{
				return DOWNLOAD_ERROR_CHUNK_RANGE;
			}

			bool saveChunksInfo = config.SaveVodChunksInfo;
			string streamRootUrl = VodPlaylist.StreamRoot.EndsWith("/") ?
				VodPlaylist.StreamRoot : $"{VodPlaylist.StreamRoot}/";

			return await Task.Run(async () =>
			{
				if (downloadMode == DownloadMode.Chunked && !Directory.Exists(outputFilePath))
				{
					Directory.CreateDirectory(outputFilePath);

					if (!Directory.Exists(outputFilePath))
					{
						return MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_DIR_NOT_EXISTS;
					}
				}

				Stream outputStream = null;
				if (downloadMode == DownloadMode.WholeFile)
				{
					if (File.Exists(outputFilePath)) { File.Delete(outputFilePath); }
					outputStream = File.OpenWrite(outputFilePath);
				}

				JArray jaChunks = downloadMode == DownloadMode.WholeFile ? new JArray() : null;

				int lastErrorCode = 400;
				int currentChunkId = firstChunkId;
				while (currentChunkId <= lastChunkId)
				{
					LinkedList<TwitchVodChunk> chunkGroup = new LinkedList<TwitchVodChunk>();
					for (int i = 0; i < MaxGroupSize; ++i)
					{
						int id = currentChunkId + i;
						if (id > lastChunkId) { break; }

						chunkGroup.AddLast(VodPlaylist.GetChunk(id));
					}

					if (chunkGroup.Count > 0)
					{
						groupDownloadStarted?.Invoke(this, chunkGroup.Count);

						Dictionary<int, DownloadItem> dict = new Dictionary<int, DownloadItem>();

						Progress<DownloadItem> progress = new Progress<DownloadItem>();
						progress.ProgressChanged += (s, progressItem) =>
						{
							lock (dict)
							{
								dict[progressItem.TaskId] = progressItem;

								List<DownloadItem> items = dict.Values.ToList();
								items.Sort((x, y) => x.TaskId < y.TaskId ? -1 : 1);
								groupDownloadProgressed?.Invoke(this, items);
							}
						};

						var tasks = chunkGroup.Select((chunk, taskId) => Task.Run(() =>
						{
							IProgress<DownloadItem> reporter = progress;

							Stream chunkStream = null;
							FileDownloader d = new FileDownloader() { Url = streamRootUrl + chunk.FileName };
							d.Connecting += (sender, url) =>
							{
								DownloadItem downloadItem = new DownloadItem(
									taskId, chunk, 0L, 0L, chunkStream, 0, DownloadItemState.Connecting);
								reporter.Report(downloadItem);
							};

							d.WorkProgress += (sender, downloadedBytes, contentLength) =>
							{
								DownloadItem downloadItem = new DownloadItem(
									taskId, chunk, contentLength, downloadedBytes, chunkStream,
									(sender as FileDownloader).LastErrorCode, DownloadItemState.Downloading);
								reporter.Report(downloadItem);
							};

							d.WorkFinished += (sender, downloadedBytes, contentLength, errCode) =>
							{
								lastErrorCode = errCode;

								DownloadItem downloadItem = new DownloadItem(
									taskId, chunk, contentLength, downloadedBytes, chunkStream,
									lastErrorCode, DownloadItemState.Finished);
								reporter.Report(downloadItem);

								if (lastErrorCode != 200 && !_cancellationToken.IsCancellationRequested)
								{
									throw new DownloadFailedException($"Download was finished with error code {lastErrorCode}");
								}
							};

							int errorCode = DownloadChunk(d, chunk, streamRootUrl, ref chunkStream,
								() => chunkChanged?.Invoke(this, chunk, currentChunkId + taskId));
							return errorCode;
						}));

						try
						{
							await Task.WhenAll(tasks);

							var items = dict.Values.ToList();
							items.Sort((x, y) => x.TaskId < y.TaskId ? -1 : 1);

							groupDownloadFinished?.Invoke(this, items, lastErrorCode);

							if (lastErrorCode == 200)
							{
								if (outputStream != null)
								{
									if (!AppendGroup(items, outputStream, jaChunks,
										chunkMergingProgressed, this))
									{
										lastErrorCode = MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS;
										groupMergingFinished?.Invoke(this, items, lastErrorCode);
										break;
									}

									groupMergingFinished?.Invoke(this, items, 200);
								}
								else if (downloadMode == DownloadMode.Chunked)
								{
									bool hasError = false;
									foreach (DownloadItem downloadItem in items)
									{
										if (!hasError)
										{
											string fn = Path.Combine(outputFilePath, downloadItem.VodChunk.FileName);
											hasError = !SaveStreamToFile(downloadItem.OutputStream, fn);
										}

										downloadItem.OutputStream.Close();
									}

									if (hasError)
									{
										lastErrorCode = MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS;
										break;
									}
								}
							}
						} catch (Exception ex)
						{
							System.Diagnostics.Debug.WriteLine(ex.Message);
							lastErrorCode = ex.HResult;
							break;
						}

						currentChunkId += chunkGroup.Count;
					}
					else
					{
						lastErrorCode = DOWNLOAD_ERROR_GROUP_EMPTY;
						break;
					}

					if (_cancellationToken.IsCancellationRequested)
					{
						lastErrorCode = FileDownloader.DOWNLOAD_ERROR_CANCELED_BY_USER;
						break;
					}
				}

				outputStream?.Close();

				if (saveChunksInfo && downloadMode == DownloadMode.WholeFile)
				{
					string path = Path.GetDirectoryName(outputFilePath);
					string fn = Path.GetFileNameWithoutExtension(outputFilePath);
					string chunksFilePath = Path.Combine(path, $"{fn}_chunks.json");
					if (File.Exists(chunksFilePath)) { File.Delete(chunksFilePath); }
					File.WriteAllText(chunksFilePath, jaChunks.ToString());
				}

				downloadCompleted?.Invoke(this, lastErrorCode);

				return lastErrorCode;
			});
		}

		private int DownloadChunk(FileDownloader fileDownloader,
			TwitchVodChunk chunk, string streamRootUrl, ref Stream stream, Action stateModified)
		{
			try
			{
				const int BUFFER_SIZE = 4096;
				fileDownloader.Url = streamRootUrl + chunk.FileName;
				stream = new MemoryStream();
				int errorCode = fileDownloader.Download(stream, BUFFER_SIZE, _cancellationTokenSource);
				if (errorCode != 200 && !_cancellationTokenSource.IsCancellationRequested)
				{
					stream.Close();
					stream = new MemoryStream();
					chunk.NextState();
					stateModified?.Invoke();
					fileDownloader.Url = streamRootUrl + chunk.FileName;
					errorCode = fileDownloader.Download(stream, BUFFER_SIZE, _cancellationTokenSource);
					if (errorCode != 200 && !_cancellationTokenSource.IsCancellationRequested)
					{
						stream.Close();
						stream = new MemoryStream();
						chunk.NextState();
						stateModified?.Invoke();
						fileDownloader.Url = streamRootUrl + chunk.FileName;
						errorCode = fileDownloader.Download(stream, BUFFER_SIZE, _cancellationTokenSource);
					}
				}
				return errorCode;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				return ex.HResult;
			}
		}

		private static bool AppendGroup(IEnumerable<DownloadItem> items, Stream stream,
			JArray chunkList,
			ChunkMergingProgressedDelegate chunkMergingProgressed, object caller)
		{
			int itemCount = items.Count();
			if (itemCount == 0) { return false; }

			long totalSize = items.Select(item => item.ChunkSize).Sum();
			long totalProcessed = 0L;

			bool hasError = false;
			int iter = 0;
			foreach (DownloadItem item in items)
			{
				if (!hasError)
				{
					void func(long sourcePosition, long sourceLength,
						long destinationPosition, long destinationLength)
					{
						totalProcessed += sourcePosition;
						chunkMergingProgressed?.Invoke(caller, totalProcessed, totalSize,
							iter, itemCount, DownloadMode.WholeFile);
					}

					item.OutputStream.Position = 0L;
					hasError = !StreamAppender.Append(item.OutputStream, stream, func, func, func);
					if (!hasError)
					{
						JObject jChunk = new JObject();
						jChunk["position"] = stream.Position - item.OutputStream.Length;
						jChunk["size"] = item.OutputStream.Length;
						jChunk["fileName"] = item.VodChunk.FileName;

						chunkList.Add(jChunk);
					}

					iter++;
				}

				item.OutputStream.Close();
			}

			return !hasError;
		}

		private static bool SaveStreamToFile(Stream stream, string filePath)
		{
			try
			{
				if (File.Exists(filePath)) { File.Delete(filePath); }
				return stream.SaveToFile(filePath);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				return false;
			}
		}

		public void Stop()
		{
			if (_cancellationToken != null && _cancellationTokenSource != null && !_cancellationToken.IsCancellationRequested)
			{
				_cancellationTokenSource.Cancel();
			}
		}
	}

	internal enum DownloadItemState { Connecting, Downloading, Finished, Errored }

	internal class DownloadItem
	{
		public int TaskId { get; }
		public TwitchVodChunk VodChunk { get; }
		public long ChunkSize { get; }
		public long DownloadedSize { get; }
		public Stream OutputStream { get; }
		public int ErrorCode { get; }
		public DownloadItemState State { get; }

		public DownloadItem(int taskId, TwitchVodChunk vodChunk,
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

	public class DownloadFailedException : Exception
	{
		public DownloadFailedException(string message) : base(message) { }
	}
}
