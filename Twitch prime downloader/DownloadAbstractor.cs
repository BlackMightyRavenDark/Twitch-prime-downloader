using System;
using System.Collections.Concurrent;
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
	internal class DownloadAbstractor : IDisposable
	{
		internal TwitchPlaylist Playlist { get; }
		internal int SimultaneousDownloadChunkChunkCount { get; set; }
		internal JArray SerializedChunkList { get; private set; }
		internal DownloadMode DownloadMode { get; }

		internal delegate void ChunkGroupDownloadStartedDelegate(object sender, IEnumerable<DownloadProgressItem> groupItems);
		internal delegate void ChunkGroupDownloadProgressedDelegate(object sender, IEnumerable<DownloadProgressItem> groupItems);
		internal delegate void ChunkGroupDownloadFinishedDelegate(object sender, IEnumerable<DownloadProgressItem> groupItems, int errorCode);
		internal delegate void ChunkMergerProgressedDelegate(object sender,
			long processedBytes, long totalSize, int chunkId, int chunkCount, DownloadMode downloadMode);
		internal delegate void ChunkAppendedDelegate(object sender, long totalSize);
		internal delegate void ChunkGroupMergerFinishedDelegate(object sender, IEnumerable<DownloadProgressItem> groupItems, int errorCode);
		internal delegate void DownloadCompletedDelegate(object sender, int errorCode);

		internal const int DOWNLOAD_ERROR_CHUNK_RANGE = int.MaxValue;
		internal const int DOWNLOAD_ERROR_GROUP_EMPTY = int.MaxValue - 1;
		internal const int DOWNLOAD_ERROR_GROUP_SEQUENCE = int.MaxValue - 2;
		internal const int DOWNLOAD_ERROR_OUTPUT_DIR_NOT_EXISTS = int.MaxValue - 3;
		internal const int DOWNLOAD_ERROR_CHUNK_BAD_STATUS_CODE = int.MaxValue - 4;
		internal const int DOWNLOAD_ERROR_EMPTY_CHUNK = int.MaxValue - 5;
		internal const int DOWNLOAD_ERROR_CHUNK_SIZE_MISMATCH = int.MaxValue - 6;
		internal const int DOWNLOAD_ERROR_UNDEFINED = int.MaxValue - 7;

		private CancellationTokenSource _cancellationTokenSource;

		internal DownloadAbstractor(TwitchPlaylist playlist, DownloadMode downloadMode, int simultaneousDownloadChunkChunkCount)
		{
			Playlist = playlist;
			DownloadMode = downloadMode;
			SimultaneousDownloadChunkChunkCount = simultaneousDownloadChunkChunkCount;
		}

		public void Dispose()
		{
			Stop();
		}

		internal int Download(
			string outputFilePath,
			int firstChunkId,
			int lastChunkId,
			bool saveChunkInfo,
			bool storeSubChunksInfo,
			string rawVodInfo,
			ChunkGroupDownloadStartedDelegate chunkGroupDownloadStarted,
			ChunkGroupDownloadProgressedDelegate chunkGroupDownloadProgressed,
			ChunkGroupDownloadFinishedDelegate chunkGroupDownloadFinished,
			ChunkMergerProgressedDelegate chunkMergerProgressed,
			ChunkGroupMergerFinishedDelegate chunkGroupMergerFinished,
			TwitchVodChunkDownloader.ChunkStateChangedDelegate chunkStateChanged,
			ChunkAppendedDelegate chunkAppended,
			DownloadCompletedDelegate downloadCompleted)
		{
			int errorCode = DOWNLOAD_ERROR_UNDEFINED;

			try
			{
				if (lastChunkId >= Playlist.Count)
				{
					downloadCompleted?.Invoke(this, DOWNLOAD_ERROR_CHUNK_RANGE);
					return DOWNLOAD_ERROR_CHUNK_RANGE;
				}

				if (DownloadMode == DownloadMode.Chunked && !Directory.Exists(outputFilePath))
				{
					Directory.CreateDirectory(outputFilePath);

					if (!Directory.Exists(outputFilePath))
					{
						downloadCompleted?.Invoke(this, DOWNLOAD_ERROR_OUTPUT_DIR_NOT_EXISTS);
						return DOWNLOAD_ERROR_OUTPUT_DIR_NOT_EXISTS;
					}
				}

				_cancellationTokenSource = new CancellationTokenSource();

				Stream outputStream = null;
				if (DownloadMode == DownloadMode.SingleFile)
				{
					if (File.Exists(outputFilePath)) { File.Delete(outputFilePath); }
					outputStream = File.OpenWrite(outputFilePath);
				}

				List<TwitchVodChunk> chunkList = Playlist.GetFilteredChunkList(item => true).ToList();
				if (Playlist.StreamHeaderChunk != null)
				{
					chunkList.Insert(0, Playlist.StreamHeaderChunk);
				}

				SerializedChunkList = DownloadMode == DownloadMode.SingleFile ? new JArray() : null;
				int currentChunkId = firstChunkId;
				while (currentChunkId <= lastChunkId && !_cancellationTokenSource.IsCancellationRequested)
				{
					List<TwitchVodChunkDownloader> chunkDownloaders = GetChunkGroup(chunkList, currentChunkId, lastChunkId, SimultaneousDownloadChunkChunkCount)?
						.Select(item => new TwitchVodChunkDownloader(item, _cancellationTokenSource)).ToList();
					if (chunkDownloaders == null || chunkDownloaders.Count <= 0)
					{
						downloadCompleted?.Invoke(this, DOWNLOAD_ERROR_GROUP_EMPTY);
						break;
					}

					ConcurrentDictionary<int, DownloadProgressItem> dictProgress = new ConcurrentDictionary<int, DownloadProgressItem>();
					for (int i = 0; i < chunkDownloaders.Count; ++i)
					{
						dictProgress[i] = new DownloadProgressItem(i, chunkDownloaders[i],
							0L, 0L, DOWNLOAD_ERROR_UNDEFINED, DownloadItemState.Preparing);
					}

					chunkGroupDownloadStarted?.Invoke(this, dictProgress.Values);

					void OnProgressChanged(DownloadProgressItem progressItem)
					{
						dictProgress[progressItem.TaskId] = progressItem;
						if (chunkGroupDownloadProgressed != null)
						{
							List<DownloadProgressItem> itemList = dictProgress.Values.ToList();
							itemList.Sort((x, y) => x.TaskId < y.TaskId ? -1 : 1);
							chunkGroupDownloadProgressed.Invoke(this, itemList);
						}
					}

					var tasks = chunkDownloaders.Select((chunkDownloader, taskId) => Task.Run(() =>
					{
						FileDownloader d = new FileDownloader()
						{
							ConnectionTimeout = 5000,
							SkipHeaderRequest = true,
							TryCountLimit = 2,
							RetryIntervalMilliseconds = 3000
						};
						d.Connecting += (sender, url, tryNumber, maxTryCount) =>
						{
							DownloadProgressItem progressItem = new DownloadProgressItem(
								taskId, chunkDownloader, 0L, 0L, 0, DownloadItemState.Connecting);
							OnProgressChanged(progressItem);
						};

						d.WorkProgress += (sender, downloadedBytes, contentLength, tryNumber, maxTryCount) =>
						{
							DownloadProgressItem progressItem = new DownloadProgressItem(
								taskId, chunkDownloader, contentLength, downloadedBytes,
								(sender as FileDownloader).LastErrorCode, DownloadItemState.Downloading);
							OnProgressChanged(progressItem);
						};

						d.WorkFinished += (sender, downloadedBytes, contentLength, tryNumber, maxTryCount, errCode) =>
						{
							DownloadProgressItem progressItem = new DownloadProgressItem(
								taskId, chunkDownloader, contentLength, downloadedBytes,
								errCode, DownloadItemState.Finished);
							OnProgressChanged(progressItem);
						};

						chunkDownloader.DownloadChunk(d, chunkStateChanged);
					}));

					Task.WhenAll(tasks).Wait();
					List<DownloadProgressItem> groupProgressItems = dictProgress.Values.ToList();
					chunkGroupDownloadFinished?.Invoke(this, groupProgressItems, errorCode);

					if (_cancellationTokenSource.IsCancellationRequested)
					{
						ClearGarbage(groupProgressItems);
						break;
					}

					if (DownloadMode == DownloadMode.SingleFile && chunkDownloaders.Count > 1 &&
						!IsContinuousSequence(groupProgressItems))
					{
						ClearGarbage(groupProgressItems);
						errorCode = DOWNLOAD_ERROR_GROUP_SEQUENCE;
						break;
					}

					bool allChunkStatusesOk = groupProgressItems.All(item => item.ErrorCode == 200);
					if (!allChunkStatusesOk)
					{
						ClearGarbage(groupProgressItems);
						errorCode = DOWNLOAD_ERROR_CHUNK_BAD_STATUS_CODE;
						break;
					}

					bool hasEmptyChunk = groupProgressItems.Any(item => item.DownloadedSize <= 0L || item.ChunkSize <= 0L ||
						item.ChunkDownloader.OutputStream == null || item.ChunkDownloader.OutputStream.Length == 0L);
					if (hasEmptyChunk)
					{
						ClearGarbage(groupProgressItems);
						errorCode = DOWNLOAD_ERROR_EMPTY_CHUNK;
						break;
					}

					bool allChunkSizesOk = groupProgressItems.All(item => item.DownloadedSize > 0L && item.DownloadedSize == item.ChunkSize);
					if (!allChunkSizesOk)
					{
						ClearGarbage(groupProgressItems);
						errorCode = DOWNLOAD_ERROR_CHUNK_SIZE_MISMATCH;
						break;
					}

					errorCode = 200;
					groupProgressItems.Sort((x, y) => x.TaskId < y.TaskId ? -1 : 1);

					if (outputStream != null)
					{
						if (!AppendChunkGroup(groupProgressItems, outputStream, saveChunkInfo, storeSubChunksInfo,
							chunkMergerProgressed, chunkAppended))
						{
							errorCode = MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS;
							chunkGroupMergerFinished?.Invoke(this, groupProgressItems, errorCode);
							break;
						}

						chunkGroupMergerFinished?.Invoke(this, groupProgressItems, 200);
					}
					else if (DownloadMode == DownloadMode.Chunked)
					{
						bool success = true;
						foreach (DownloadProgressItem progressItem in groupProgressItems)
						{
							if (success)
							{
								string fn = Path.Combine(outputFilePath, progressItem.ChunkDownloader.ChunkWrapper.FileName);
								success = SaveStreamToFile(progressItem.ChunkDownloader.OutputStream, fn, out
#if DEBUG
									string errorMessage
#else
									_
#endif
									);
								if (success)
								{
									if (saveChunkInfo)
									{
										JObject jChunk = progressItem.ChunkDownloader.ChunkWrapper.Serialize(-1L, progressItem.DownloadedSize);
										if (storeSubChunksInfo)
										{
											JArray jaSubChunks = progressItem.ChunkDownloader.ChunkWrapper.ExtractSubChunks(progressItem.ChunkDownloader.OutputStream, 0L);
											if (jaSubChunks != null)
											{
												jChunk["subChunks"] = jaSubChunks;
											}
										}
										File.WriteAllText(fn + "_chunk.json", jChunk.ToString());
									}
								}
#if DEBUG
								else if (!string.IsNullOrWhiteSpace(errorMessage))
								{
									System.Diagnostics.Debug.WriteLine(errorMessage);
								}
#endif
							}

							progressItem.ChunkDownloader.OutputStream.Dispose();
						}

						if (!success)
						{
							ClearGarbage(groupProgressItems);
							errorCode = MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS;
							break;
						}

						chunkGroupMergerFinished?.Invoke(this, groupProgressItems, 200);
					}

					currentChunkId += chunkDownloaders.Count;
				}

				if (_cancellationTokenSource.IsCancellationRequested) { errorCode = FileDownloader.DOWNLOAD_ERROR_CANCELED; }

				outputStream?.Dispose();

				try
				{
					if (config.SaveVodInfo && !string.IsNullOrEmpty(rawVodInfo))
					{
						string infoFilePath = DownloadMode == DownloadMode.SingleFile ?
							outputFilePath + "_info.json" :
							Path.Combine(outputFilePath, "_info.json");
						File.WriteAllText(infoFilePath, rawVodInfo);
					}
				}
#if DEBUG
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
#else
				catch
				{
#endif
				}

				try
				{
					if (saveChunkInfo && SerializedChunkList != null && SerializedChunkList.Count > 0)
					{
						string chunksFilePath = outputFilePath + "_chunks.json";
						if (File.Exists(chunksFilePath)) { File.Delete(chunksFilePath); }
						File.WriteAllText(chunksFilePath, SerializedChunkList.ToString());
					}
				}
#if DEBUG
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
#else
				catch
				{
#endif
				}
			}
			catch (Exception ex)
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
				errorCode = ex.HResult;
			}

			if (_cancellationTokenSource != null)
			{
				_cancellationTokenSource.Dispose();
				_cancellationTokenSource = null;
			}

			downloadCompleted?.Invoke(this, errorCode);
			return errorCode;
		}

		private static List<TwitchVodChunkWrapper> GetChunkGroup(List<TwitchVodChunk> chunks,
			int currentChunkId, int lastChunkId, int maxGroupSize)
		{
			List<TwitchVodChunkWrapper> list = new List<TwitchVodChunkWrapper>() { Capacity = maxGroupSize };
			for (int i = 0; i < maxGroupSize; ++i)
			{
				int id = currentChunkId + i;
				if (id > lastChunkId) { break; }

				list.Add(new TwitchVodChunkWrapper(chunks[id]));
			}
			return list;
		}

		private bool AppendChunkGroup(IEnumerable<DownloadProgressItem> items,
			Stream outputStream, bool saveChunkInfo, bool storeSubChunksInfo,
			ChunkMergerProgressedDelegate chunkMergerProgressed, ChunkAppendedDelegate chunkAppended)
		{
			int itemCount = items.Count();
			if (itemCount == 0) { return false; }

			long totalSize = items.Sum(item => item.DownloadedSize);
			long totalProcessed = 0L;
			long outputStreamInitialPosition = outputStream.Position;

			bool success = true;
			int iter = 0;
			foreach (DownloadProgressItem item in items)
			{
				if (success)
				{
					void progressFunc(long sourcePosition, long sourceLength,
						long destinationPosition, long destinationLength, long bytesTransferred)
					{
						totalProcessed = destinationPosition - outputStreamInitialPosition;
						chunkMergerProgressed?.Invoke(this, totalProcessed, totalSize,
							iter, itemCount, DownloadMode.SingleFile);
					}

					long chunkPosition = outputStream.Position;
					item.ChunkDownloader.OutputStream.Position = 0L;
					success = StreamAppender.Append(item.ChunkDownloader.OutputStream, outputStream,
						(sourcePosition, sourceLength, destinationPosition, destinationLength) =>
						{
							totalProcessed = 0L;
							chunkMergerProgressed?.Invoke(this, totalProcessed, totalSize,
								iter, itemCount, DownloadMode.SingleFile);
						},
						progressFunc, progressFunc);
					if (success)
					{
						if (saveChunkInfo && SerializedChunkList != null)
						{
							JObject jChunk = item.ChunkDownloader.ChunkWrapper.Serialize(chunkPosition, item.DownloadedSize);
							if (storeSubChunksInfo)
							{
								JArray jaSubChunks = item.ChunkDownloader.ChunkWrapper.ExtractSubChunks(item.ChunkDownloader.OutputStream, chunkPosition);
								if (jaSubChunks != null)
								{
									jChunk["subChunks"] = jaSubChunks;
								}
							}
							SerializedChunkList.Add(jChunk);
						}

						chunkAppended?.Invoke(this, outputStream.Length);
					}

					iter++;
				}

				item.ChunkDownloader.OutputStream.Dispose();
			}

			return success;
		}

		internal void Stop()
		{
			if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
			{
				_cancellationTokenSource.Cancel();
			}
		}

		private static void ClearGarbage(IEnumerable<DownloadProgressItem> items)
		{
			foreach (DownloadProgressItem item in items)
			{
				item.ChunkDownloader.OutputStream?.Dispose();
			}
		}
	}
}
