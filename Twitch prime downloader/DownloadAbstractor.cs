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

		public delegate void GroupDownloadStartedDelegate(object sender, int groupSize);
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
			GroupDownloadStartedDelegate groupDownloadStarted,
			GroupDownloadProgressedDelegate groupDownloadProgressed,
			GroupDownloadFinishedDelegate groupDownloadFinished,
			ChunkMergingProgressedDelegate chunkMergingProgressed,
			GroupMergingFinishedDelegate groupMergingFinished,
			ChunkChangedDelegate chunkChanged,
			DownloadCompletedDelegate downloadCompleted)
		{
			try
			{
				if (lastChunkId >= VodPlaylist.Count)
				{
					return DOWNLOAD_ERROR_CHUNK_RANGE;
				}

				if (downloadMode == DownloadMode.Chunked && !Directory.Exists(outputFilePath))
				{
					Directory.CreateDirectory(outputFilePath);

					if (!Directory.Exists(outputFilePath))
					{
						return DOWNLOAD_ERROR_OUTPUT_DIR_NOT_EXISTS;
					}
				}

				_cancellationTokenSource = new CancellationTokenSource();

				Stream outputStream = null;
				if (downloadMode == DownloadMode.WholeFile)
				{
					if (File.Exists(outputFilePath)) { File.Delete(outputFilePath); }
					outputStream = File.OpenWrite(outputFilePath);
				}

				bool saveChunksInfo = config.SaveVodChunksInfo;
				JArray jaChunks = saveChunksInfo && downloadMode == DownloadMode.WholeFile ? new JArray() : null;
				string streamRootUrl = VodPlaylist.StreamRootUrl.EndsWith("/") ?
					VodPlaylist.StreamRootUrl : $"{VodPlaylist.StreamRootUrl}/";

				int lastErrorCode = 400;
				int currentChunkId = firstChunkId;
				while (currentChunkId <= lastChunkId)
				{
					TwitchVodChunk[] chunkGroup = GetChunkGroup(VodPlaylist, currentChunkId, lastChunkId, MaxGroupSize).ToArray();

					if (chunkGroup.Length > 0)
					{
						groupDownloadStarted?.Invoke(this, chunkGroup.Length);

						ConcurrentDictionary<int, DownloadProgressItem> dictProgress = new ConcurrentDictionary<int, DownloadProgressItem>();

						void OnProgressChanged(DownloadProgressItem progressItem)
						{
							dictProgress[progressItem.TaskId] = progressItem;

							List<DownloadProgressItem> items = dictProgress.Values.ToList();
							items.Sort((x, y) => x.TaskId < y.TaskId ? -1 : 1);
							groupDownloadProgressed?.Invoke(this, items);
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
								lastErrorCode = errCode;

								DownloadProgressItem progressItem = new DownloadProgressItem(
									taskId, chunk, contentLength, downloadedBytes, chunkStream,
									lastErrorCode, DownloadItemState.Finished);
								OnProgressChanged(progressItem);
							};

							int errorCode = DownloadChunk(d, chunk, streamRootUrl, ref chunkStream,
								() => chunkChanged?.Invoke(this, chunk, currentChunkId + taskId));
							return errorCode;
						}));

						try
						{
							Task.WhenAll(tasks).Wait();

							if (downloadMode == DownloadMode.WholeFile && chunkGroup.Length > 1 && 
								!IsContinuousSequence(dictProgress))
							{
								ClearGarbage(dictProgress.Values);
								downloadCompleted?.Invoke(this, DOWNLOAD_ERROR_GROUP_SEQUENCE);
								return DOWNLOAD_ERROR_GROUP_SEQUENCE;
							}

							List<DownloadProgressItem> items = dictProgress.Values.ToList();

							bool statusesOk = items.All(item => item.ErrorCode == 200);
							if (!statusesOk)
							{
								ClearGarbage(items);
								downloadCompleted?.Invoke(this, DOWNLOAD_ERROR_CHUNK_BAD_STATUS_CODE);
								return DOWNLOAD_ERROR_CHUNK_BAD_STATUS_CODE;
							}

							bool hasEmptyChunk = items.Any(item => item.ChunkSize <= 0L || item.OutputStream == null || item.OutputStream.Length == 0L);
							if (hasEmptyChunk)
							{
								ClearGarbage(items);
								downloadCompleted?.Invoke(this, DOWNLOAD_ERROR_EMPTY_CHUNK);
								return DOWNLOAD_ERROR_EMPTY_CHUNK;
							}

							items.Sort((x, y) => x.TaskId < y.TaskId ? -1 : 1);

							groupDownloadFinished?.Invoke(this, items, lastErrorCode);

							if (lastErrorCode == 200)
							{
								if (outputStream != null)
								{
									if (!AppendGroup(items, outputStream, jaChunks, chunkMergingProgressed))
									{
										lastErrorCode = MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS;
										groupMergingFinished?.Invoke(this, items, lastErrorCode);
										break;
									}

									groupMergingFinished?.Invoke(this, items, 200);
								}
								else if (downloadMode == DownloadMode.Chunked)
								{
									bool success = true;
									foreach (DownloadProgressItem progressItem in items)
									{
										if (success)
										{
											string fn = Path.Combine(outputFilePath, progressItem.VodChunk.FileName);
											success = SaveStreamToFile(progressItem.OutputStream, fn);
										}

										progressItem.OutputStream.Dispose();
									}

									if (!success)
									{
										lastErrorCode = MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS;
										break;
									}

									groupMergingFinished?.Invoke(this, items, 200);
								}
							}
						}
						catch (Exception ex)
						{
#if DEBUG
							System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
							lastErrorCode = ex.HResult;
							break;
						}

						currentChunkId += chunkGroup.Length;
					}
					else
					{
						lastErrorCode = DOWNLOAD_ERROR_GROUP_EMPTY;
						break;
					}

					if (_cancellationTokenSource.IsCancellationRequested)
					{
						lastErrorCode = FileDownloader.DOWNLOAD_ERROR_CANCELED;
						break;
					}
				}

				outputStream?.Dispose();

				_cancellationTokenSource.Dispose();
				_cancellationTokenSource = null;

				if (jaChunks != null)
				{
					string chunksFilePath = outputFilePath + "_chunks.json";
					if (File.Exists(chunksFilePath)) { File.Delete(chunksFilePath); }
					File.WriteAllText(chunksFilePath, jaChunks.ToString());
				}

				downloadCompleted?.Invoke(this, lastErrorCode);

				return lastErrorCode;
			} catch (Exception ex)
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
				if (_cancellationTokenSource != null)
				{
					_cancellationTokenSource.Dispose();
					_cancellationTokenSource = null;
				}
				return ex.HResult;
			}
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

			long totalSize = items.Where(item => item.ChunkSize > 0L).Sum(item => item.ChunkSize);
			long totalProcessed = 0L;
			long outputStreamInitialPosition = outpuStream.Position;

			bool success = true;
			int iter = 0;
			foreach (DownloadProgressItem item in items)
			{
				if (success)
				{
					void func(long sourcePosition, long sourceLength,
						long destinationPosition, long destinationLength, long bytesTransferred)
					{
						totalProcessed = destinationPosition - outputStreamInitialPosition;
						chunkMergingProgressed?.Invoke(this, totalProcessed, totalSize,
							iter, itemCount, DownloadMode.WholeFile);
					}

					long chunkPosition = outpuStream.Position;
					item.OutputStream.Position = 0L;
					success = StreamAppender.Append(item.OutputStream, outpuStream,
						(sourcePosition, sourceLength, destinationPosition, destinationLength) =>
						{
							totalProcessed = 0L;
							chunkMergingProgressed?.Invoke(this, totalProcessed, totalSize,
								iter, itemCount, DownloadMode.WholeFile);
						}, func, func);
					if (success && chunkList != null)
					{
						JObject jChunk = new JObject()
						{
							["position"] = chunkPosition,
							["size"] = item.OutputStream.Length,
							["fileName"] = item.VodChunk.FileName
						};

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
