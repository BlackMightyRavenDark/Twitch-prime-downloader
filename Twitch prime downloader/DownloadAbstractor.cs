using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MultiThreadedDownloaderLib;
using Newtonsoft.Json.Linq;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
    internal class DownloadAbstractor
    {
        public delegate void ConnectingDelegate(object sender, TwitchVodChunk chunk);
        public delegate void ChunkDownloadStartedDelegate(object sender, long contentLength, int chunkId);
        public delegate void ChunkDownloadProgressedDelegate(object sender, long downloadedBytes, long contentLength);
        public delegate void ChunkDownloadFinishedDelegate(object sender, long downloadedBytes, long contentLength, int errorCode);
        public delegate void ChunkMergingFinishedDelegate(object sender, long totalSize, DownloadMode downloadMode, int chunkId, int chunkCount);
        public delegate void ChunkChangedDelegate(object sender, TwitchVodChunk chunk, int chunkId);
        public delegate void CompletedDelegate(object sender, int errorCode);

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public DownloadAbstractor()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public async Task<int> Download(
            string outputFilePath,
            string streamRootUrl,
            List<TwitchVodChunk> chunkList,
            int firstChunkId,
            int lastChunkId,
            DownloadMode downloadMode,
            ConnectingDelegate connecting,
            ChunkDownloadStartedDelegate chunkDownloadStarted,
            ChunkDownloadProgressedDelegate chunkDownloadProgressed,
            ChunkDownloadFinishedDelegate chunkDownloadFinished,
            ChunkChangedDelegate chunkChanged,
            ChunkMergingFinishedDelegate chunkMergingFinished,
            bool saveChunkInfo)
        {
            return await Task.Run(() =>
            {
                if (firstChunkId > lastChunkId)
                {
                    return 0;
                }

                int errorCode = 200;
                try
                {
                    if (downloadMode == DownloadMode.Chunked && !Directory.Exists(outputFilePath))
                    {
                        Directory.CreateDirectory(outputFilePath);
                    }

                    Stream outputStream = null;
                    if (downloadMode == DownloadMode.WholeFile)
                    {
                        if (File.Exists(outputFilePath)) { File.Delete(outputFilePath); }
                        outputStream = File.OpenWrite(outputFilePath);
                    }

                    int currentChunkId = firstChunkId;
                    long totalBytesDownloaded = 0L;

                    FileDownloader fileDownloader = new FileDownloader();
                    fileDownloader.UpdateIntervalMilliseconds = 50;
                    fileDownloader.Connecting = (s, url) =>
                    {
                        connecting?.Invoke(this, chunkList[currentChunkId]);
                    };
                    fileDownloader.WorkStarted = (s, len) =>
                    {
                        chunkDownloadStarted?.Invoke(this, len, currentChunkId);
                    };
                    fileDownloader.WorkProgress = (sender, downloadedBytes, contentLength) =>
                    {
                        chunkDownloadProgressed?.Invoke(this, downloadedBytes, contentLength);
                    };
                    fileDownloader.WorkFinished = (sender, bytesTransfered, contentLength, errCode) =>
                    {
                        if (errCode == 200)
                        {
                            totalBytesDownloaded += bytesTransfered;
                        }
                        chunkDownloadFinished?.Invoke(this, bytesTransfered, contentLength, errCode);
                    };

                    if (!streamRootUrl.EndsWith("/")) { streamRootUrl += "/"; }

                    JArray jaChunks = new JArray();

                    while (currentChunkId <= lastChunkId && !_cancellationToken.IsCancellationRequested)
                    {
                        Stream mem = new MemoryStream();
                        errorCode = DownloadChunk(fileDownloader, chunkList[currentChunkId], streamRootUrl, ref mem,
                            () =>
                            {
                                chunkChanged?.Invoke(this, chunkList[currentChunkId], currentChunkId);
                            });
                        if (errorCode != 200)
                        {
                            mem.Close();
                            break;
                        }

                        totalBytesDownloaded += mem.Length;

                        if (downloadMode == DownloadMode.WholeFile)
                        {
                            long chunkPosition = outputStream.Position;

                            mem.Position = 0L;
                            if (!MultiThreadedDownloader.AppendStream(mem, outputStream))
                            {
                                errorCode = MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS;
                                mem.Close();
                                break;
                            }

                            chunkMergingFinished?.Invoke(this, outputStream.Length, downloadMode, currentChunkId, lastChunkId - firstChunkId + 1);

                            JObject jChunk = new JObject();
                            jChunk["position"] = chunkPosition;
                            jChunk["size"] = mem.Length;
                            jChunk["fileName"] = chunkList[currentChunkId].FileName;
                            jaChunks.Add(jChunk);
                        }
                        else
                        {
                            if (!SaveStreamToFile(mem, Path.Combine(outputFilePath, chunkList[currentChunkId].FileName)))
                            {
                                errorCode = MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS;
                                mem.Close();
                                break;
                            }

                            chunkMergingFinished?.Invoke(this, totalBytesDownloaded,
                                downloadMode, currentChunkId, lastChunkId - firstChunkId + 1);
                        }
                        mem.Close();

                        currentChunkId++;
                    }

                    if (saveChunkInfo && downloadMode == DownloadMode.WholeFile)
                    {
                        int n = outputFilePath.LastIndexOf(".");
                        string chunksFilePath = n > 0 ? $"{outputFilePath.Substring(0, n)}_chunks.json" : $"{outputFilePath}_chunks.json";
                        File.WriteAllText(chunksFilePath, jaChunks.ToString());
                    }

                    if (_cancellationToken.IsCancellationRequested)
                    {
                        errorCode = FileDownloader.DOWNLOAD_ERROR_CANCELED_BY_USER;
                    }

                    outputStream?.Close();
                    fileDownloader.Dispose();

                    return errorCode;
                } catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    return ex.HResult;
                }
            });
        }

        private int DownloadChunk(FileDownloader fileDownloader,
            TwitchVodChunk chunk, string streamRoot, ref Stream stream, Action stateModified)
        {
            try
            {
                fileDownloader.Url = streamRoot + chunk.FileName;
                stream = new MemoryStream();
                int errorCode = fileDownloader.Download(stream);
                if (errorCode != 200)
                {
                    stream.Close();
                    stream = new MemoryStream();
                    chunk.NextState();
                    stateModified?.Invoke();
                    fileDownloader.Url = streamRoot + chunk.FileName;
                    errorCode = fileDownloader.Download(stream);
                    if (errorCode != 200)
                    {
                        stream.Close();
                        stream = new MemoryStream();
                        chunk.NextState();
                        stateModified?.Invoke();
                        fileDownloader.Url = streamRoot + chunk.FileName;
                        errorCode = fileDownloader.Download(stream);
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

        private bool SaveStreamToFile(Stream stream, string filePath)
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
}
