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
		public TwitchVodPlaylist VodPlaylist { get; }
		public int MaxGroupSize { get; set; }

		public delegate void GroupDownloadStartedDelegate(object sender, IEnumerable<DownloadProgressItem> groupItems);
		public delegate void GroupDownloadProgressedDelegate(object sender, IEnumerable<DownloadProgressItem> groupItems);
		public delegate void GroupDownloadFinishedDelegate(object sender, IEnumerable<DownloadProgressItem> groupItems, int errorCode);
		public delegate void ChunkMergingProgressedDelegate(object sender,
			long processedBytes, long totalSize,int chunkId, int chunkCount, DownloadMode downloadMode);
		public delegate void GroupMergingFinishedDelegate(object sender, IEnumerable<DownloadProgressItem> groupItems, int errorCode);
		public delegate void ChunkChangedDelegate(object sender, TwitchVodChunk chunk, int chunkId);
		public delegate void DownloadCompletedDelegate(object sender, int errorCode);

		public const int DOWNLOAD_ERROR_CHUNK_RANGE = int.MaxValue;
		public const int DOWNLOAD_ERROR_GROUP_EMPTY = int.MaxValue - 1;
		public const int DOWNLOAD_ERROR_GROUP_SEQUENCE = int.MaxValue - 2;
		public const int DOWNLOAD_ERROR_OUTPUT_DIR_NOT_EXISTS = int.MaxValue - 3;
		public const int DOWNLOAD_ERROR_CHUNK_BAD_STATUS_CODE = int.MaxValue - 4;
		public const int DOWNLOAD_ERROR_EMPTY_CHUNK = int.MaxValue - 5;
		public const int DOWNLOAD_ERROR_CHUNK_SIZE_MISMATCH = int.MaxValue - 6;
		public const int DOWNLOAD_ERROR_UNDEFINED = int.MaxValue - 7;

		private CancellationTokenSource _cancellationTokenSource;

		public DownloadAbstractor(TwitchVodPlaylist vodPlaylist, int maxGroupSize)
		{
			VodPlaylist = vodPlaylist;
			MaxGroupSize = maxGroupSize;
		}

		public void Dispose()
		{
			Stop();
		}

		public int Download(
			string outputFilePath,
			int firstChunkId,
			int lastChunkId,
			DownloadMode downloadMode,
			bool saveChunkInfo,
			string rawVodInfo,
			GroupDownloadStartedDelegate groupDownloadStarted,
			GroupDownloadProgressedDelegate groupDownloadProgressed,
			GroupDownloadFinishedDelegate groupDownloadFinished,
			ChunkMergingProgressedDelegate chunkMergingProgressed,
			GroupMergingFinishedDelegate groupMergingFinished,
			ChunkChangedDelegate chunkChanged,
			DownloadCompletedDelegate downloadCompleted)
		{
			int lastErrorCode = DOWNLOAD_ERROR_UNDEFINED;

			try
			{
				if (lastChunkId >= VodPlaylist.Count)
				{
					downloadCompleted?.Invoke(this, DOWNLOAD_ERROR_CHUNK_RANGE);
					return DOWNLOAD_ERROR_CHUNK_RANGE;
				}

				if (downloadMode == DownloadMode.Chunked && !Directory.Exists(outputFilePath))
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
				if (downloadMode == DownloadMode.SingleFile)
				{
					if (File.Exists(outputFilePath)) { File.Delete(outputFilePath); }
					outputStream = File.OpenWrite(outputFilePath);
				}

				JArray jaChunks = saveChunkInfo && downloadMode == DownloadMode.SingleFile ? new JArray() : null;
				string streamRootUrl = VodPlaylist.StreamRootUrl.EndsWith("/") ?
					VodPlaylist.StreamRootUrl : $"{VodPlaylist.StreamRootUrl}/";

				int currentChunkId = firstChunkId;
				bool hasChunkError = false;
				object errorCodeLocker = new object();
				while (currentChunkId <= lastChunkId && !_cancellationTokenSource.IsCancellationRequested)
				{
					TwitchVodChunk[] chunkGroup = GetChunkGroup(VodPlaylist, currentChunkId, lastChunkId, MaxGroupSize).ToArray();

					if (chunkGroup.Length <= 0)
					{
						lastErrorCode = DOWNLOAD_ERROR_GROUP_EMPTY;
						downloadCompleted?.Invoke(this, DOWNLOAD_ERROR_GROUP_EMPTY);
						break;
					}

					ConcurrentDictionary<int, DownloadProgressItem> dictProgress = new ConcurrentDictionary<int, DownloadProgressItem>();
					for (int i = 0; i < chunkGroup.Length; ++i)
					{
						dictProgress[i] = new DownloadProgressItem(i, chunkGroup[i],
							0L, 0L, null, 0, DownloadItemState.Preparing);
					}

					groupDownloadStarted?.Invoke(this, dictProgress.Values);

					void OnProgressChanged(DownloadProgressItem progressItem)
					{
						dictProgress[progressItem.TaskId] = progressItem;
						if (groupDownloadProgressed != null)
						{
							List<DownloadProgressItem> itemList = dictProgress.Values.ToList();
							itemList.Sort((x, y) => x.TaskId < y.TaskId ? -1 : 1);
							groupDownloadProgressed.Invoke(this, itemList);
						}
					}

					var tasks = chunkGroup.Select((chunk, taskId) => Task.Run(() =>
					{
						Stream chunkStream = null;
						FileDownloader d = new FileDownloader()
						{
							Url = streamRootUrl + chunk.FileName,
							ConnectionTimeout = 5000,
							SkipHeaderRequest = true,
							TryCountLimit = 2,
							RetryIntervalMilliseconds = 3000
						};
						d.Connecting += (sender, url, tryNumber, maxTryCount) =>
						{
							DownloadProgressItem progressItem = new DownloadProgressItem(
								taskId, chunk, 0L, 0L, chunkStream, 0, DownloadItemState.Connecting);
							OnProgressChanged(progressItem);
						};

						d.WorkProgress += (sender, downloadedBytes, contentLength, tryNumber, maxTryCount) =>
						{
							DownloadProgressItem progressItem = new DownloadProgressItem(
								taskId, chunk, contentLength, downloadedBytes, chunkStream,
								(sender as FileDownloader).LastErrorCode, DownloadItemState.Downloading);
							OnProgressChanged(progressItem);
						};

						d.WorkFinished += (sender, downloadedBytes, contentLength, tryNumber, maxTryCount, errCode) =>
						{
							lock (errorCodeLocker)
							{
								if (!hasChunkError)
								{
									lastErrorCode = errCode;
									hasChunkError = errCode != 200 && errCode != FileDownloader.DOWNLOAD_ERROR_OUT_OF_TRIES_LEFT;
								}
							}

							DownloadProgressItem progressItem = new DownloadProgressItem(
								taskId, chunk, contentLength, downloadedBytes, chunkStream,
								errCode, DownloadItemState.Finished);
							OnProgressChanged(progressItem);
						};

						int errorCode = DownloadChunk(d, chunk, streamRootUrl, ref chunkStream,
							() => chunkChanged?.Invoke(this, chunk, currentChunkId + taskId));
						return errorCode;
					}));

					Task.WhenAll(tasks).Wait();

					if (_cancellationTokenSource.IsCancellationRequested)
					{
						ClearGarbage(dictProgress.Values);
						break;
					}

					if (downloadMode == DownloadMode.SingleFile && chunkGroup.Length > 1 &&
						!IsContinuousSequence(dictProgress))
					{
						ClearGarbage(dictProgress.Values);
						downloadCompleted?.Invoke(this, DOWNLOAD_ERROR_GROUP_SEQUENCE);
						return DOWNLOAD_ERROR_GROUP_SEQUENCE;
					}

					List<DownloadProgressItem> groupItems = dictProgress.Values.ToList();

					bool allChunkStatusesOk = groupItems.All(item => item.ErrorCode == 200);
					if (!allChunkStatusesOk)
					{
						ClearGarbage(groupItems);
						lastErrorCode = DOWNLOAD_ERROR_CHUNK_BAD_STATUS_CODE;
						break;
					}

					bool hasEmptyChunk = groupItems.Any(item => item.DownloadedSize <= 0L || item.ChunkSize <= 0L || item.OutputStream == null || item.OutputStream.Length == 0L);
					if (hasEmptyChunk)
					{
						ClearGarbage(groupItems);
						lastErrorCode = DOWNLOAD_ERROR_EMPTY_CHUNK;
						break;
					}

					bool allChunkSizesOk = groupItems.All(item => item.DownloadedSize > 0L && item.DownloadedSize == item.ChunkSize);
					if (!allChunkSizesOk)
					{
						ClearGarbage(groupItems);
						lastErrorCode = DOWNLOAD_ERROR_CHUNK_SIZE_MISMATCH;
						break;
					}

					groupItems.Sort((x, y) => x.TaskId < y.TaskId ? -1 : 1);

					groupDownloadFinished?.Invoke(this, groupItems, lastErrorCode);

					if (lastErrorCode == 200)
					{
						if (outputStream != null)
						{
							if (!AppendGroup(groupItems, outputStream, jaChunks, chunkMergingProgressed))
							{
								lastErrorCode = MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS;
								groupMergingFinished?.Invoke(this, groupItems, lastErrorCode);
								break;
							}

							groupMergingFinished?.Invoke(this, groupItems, 200);
						}
						else if (downloadMode == DownloadMode.Chunked)
						{
							bool success = true;
							foreach (DownloadProgressItem progressItem in groupItems)
							{
								if (success)
								{
									string fn = Path.Combine(outputFilePath, progressItem.VodChunk.FileName);
									success = SaveStreamToFile(progressItem.OutputStream, fn);
									if (success && saveChunkInfo)
									{
										JObject jChunk = progressItem.VodChunk.Serialize(-1L, progressItem.DownloadedSize);
										File.WriteAllText(fn + "_chunk.json", jChunk.ToString());
									}
								}

								progressItem.OutputStream.Dispose();
							}

							if (!success)
							{
								ClearGarbage(groupItems);
								lastErrorCode = MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS;
								break;
							}

							groupMergingFinished?.Invoke(this, groupItems, 200);
						}
					}

					currentChunkId += chunkGroup.Length;
				}

				if (_cancellationTokenSource.IsCancellationRequested) { lastErrorCode = FileDownloader.DOWNLOAD_ERROR_CANCELED; }

				outputStream?.Dispose();

				try
				{
					if (config.SaveVodInfo && !string.IsNullOrEmpty(rawVodInfo))
					{
						string infoFilePath = downloadMode == DownloadMode.SingleFile ?
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
					if (jaChunks != null && jaChunks.Count > 0)
					{
						string chunksFilePath = outputFilePath + "_chunks.json";
						if (File.Exists(chunksFilePath)) { File.Delete(chunksFilePath); }
						File.WriteAllText(chunksFilePath, jaChunks.ToString());
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
				lastErrorCode = ex.HResult;
			}

			if (_cancellationTokenSource != null)
			{
				_cancellationTokenSource.Dispose();
				_cancellationTokenSource = null;
			}

			downloadCompleted?.Invoke(this, lastErrorCode);
			return lastErrorCode;
		}

		private static IEnumerable<TwitchVodChunk> GetChunkGroup(TwitchVodPlaylist playlist,
			int currentChunkId, int lastChunkId, int maxGroupSize)
		{
			for (int i = 0; i < maxGroupSize; ++i)
			{
				int id = currentChunkId + i;
				if (id > lastChunkId) { break; }

				yield return playlist.GetChunk(id);
			}
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
					stream.Dispose();
					stream = new MemoryStream();
					chunk.NextState();
					stateModified?.Invoke();
					fileDownloader.Url = streamRootUrl + chunk.FileName;
					errorCode = fileDownloader.Download(stream, BUFFER_SIZE, _cancellationTokenSource);
					if (errorCode != 200 && !_cancellationTokenSource.IsCancellationRequested)
					{
						stream.Dispose();
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
#if DEBUG
				System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
				return ex.HResult;
			}
		}

		private bool AppendGroup(IEnumerable<DownloadProgressItem> items,
			Stream outpuStream, JArray chunkList,
			ChunkMergingProgressedDelegate chunkMergingProgressed)
		{
			int itemCount = items.Count();
			if (itemCount == 0) { return false; }

			long totalSize = items.Sum(item => item.DownloadedSize);
			long totalProcessed = 0L;
			long outputStreamInitialPosition = outpuStream.Position;

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
						chunkMergingProgressed?.Invoke(this, totalProcessed, totalSize,
							iter, itemCount, DownloadMode.SingleFile);
					}

					long chunkPosition = outpuStream.Position;
					item.OutputStream.Position = 0L;
					success = StreamAppender.Append(item.OutputStream, outpuStream,
						(sourcePosition, sourceLength, destinationPosition, destinationLength) =>
						{
							totalProcessed = 0L;
							chunkMergingProgressed?.Invoke(this, totalProcessed, totalSize,
								iter, itemCount, DownloadMode.SingleFile);
						},
						progressFunc, progressFunc);
					if (success && chunkList != null)
					{
						JObject jChunk = item.VodChunk.Serialize(chunkPosition, item.DownloadedSize);
						chunkList.Add(jChunk);
					}

					iter++;
				}

				item.OutputStream.Dispose();
			}

			return success;
		}

		public void Stop()
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
				item.OutputStream?.Dispose();
			}
		}
	}
}
