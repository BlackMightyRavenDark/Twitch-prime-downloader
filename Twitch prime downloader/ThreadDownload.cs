using System.Collections.Generic;
using System.IO;
using System.Threading;
using MultiThreadedDownloaderLib;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
    public sealed class ThreadDownload
    {
        private bool canceled = false;
        private bool terminated = false;
        public int ChunkFrom { get; set; } = 0;
        public int ChunkTo { get; set; } = 10;
        public long CurrentChunkSize { get; private set; } = 0L;
        public long CurrentChunkBytesTransfered { get; private set; } = 0L;
        public DownloadingMode DownloadingMode { get; private set; }
        private int iDownload;
        public string DownloadingFilePath { get; private set; }
        public long DownloadedFileSize { get; private set; } = 0L;
        public List<VideoChunk> VideoChunks { get; private set; } = null;
        public string _streamRoot;
        public bool SaveChunksInfo { get; private set; }
        public List<TwitchVodChunk> _chunks;
        public int LastErrorCode { get; private set; }

        public const int ERROR_DOWNLOAD_TERMINATED = -100;

        public delegate void ConnectingDelegate(object sender, TwitchVodChunk chunk);
        public delegate void WorkStartedDelegate(object sender, long fileSize, int chunkId);
        public delegate void WorkProgressDelegate(object sender, long transferedSize);
        public delegate void WorkFinishedDelegate(object sender, long transferedSize, int errorCode);
        public delegate void ChunkChangedDelegate(object sender, int chunkId);
        public delegate void CompletedDelegate(object sender, int errorCode);
        public ConnectingDelegate Connecting;
        public WorkStartedDelegate WorkStarted;
        public WorkProgressDelegate WorkProgress;
        public WorkFinishedDelegate WorkFinished;
        public ChunkChangedDelegate ChunkChanged;
        public CompletedDelegate Completed;

        public ThreadDownload(string downloadingFilePath,
            DownloadingMode downloadingMode, bool saveChunksInfo)
        {
            DownloadingFilePath = downloadingFilePath;
            DownloadingMode = downloadingMode;
            SaveChunksInfo = saveChunksInfo;
        }

        public void Work(object context)
        {
            SynchronizationContext synchronizationContext = context != null ? (SynchronizationContext)context : null;

            Stream downloadingStream = DownloadingMode == DownloadingMode.WholeFile ? File.OpenWrite(DownloadingFilePath) : null;

            FileDownloader fileDownloader = new FileDownloader();
            fileDownloader.Connecting += (s, url) =>
            {
                synchronizationContext?.Send(Connecting_Context, this);
            };
            fileDownloader.WorkStarted += (s, contentLen) =>
            {
                if (SaveChunksInfo)
                {
                    string chunkFn = _chunks[iDownload].FileName;
                    VideoChunks.Add(new VideoChunk(DownloadedFileSize, contentLen, chunkFn));
                }
                CurrentChunkSize = contentLen;
                CurrentChunkBytesTransfered = 0L;
                synchronizationContext?.Send(WorkStart_Context, this);
            };
            fileDownloader.WorkProgress += (s, bytes, contentLen) =>
            {
                CurrentChunkBytesTransfered = bytes;
                synchronizationContext?.Send(WorkProgress_Context, this);
            };
            fileDownloader.WorkFinished += (s, bytes, contentLen, errCode) =>
            {
                CurrentChunkBytesTransfered = bytes;
                synchronizationContext?.Send(WorkProgress_Context, this);
            };

            if (DownloadingMode == DownloadingMode.Chunked && !Directory.Exists(DownloadingFilePath))
            {
                Directory.CreateDirectory(DownloadingFilePath);
            }

            if (SaveChunksInfo)
            {
                VideoChunks = new List<VideoChunk>();
            }

            canceled = false;
            terminated = false;
            DownloadedFileSize = 0L;
            iDownload = ChunkFrom;
            LastErrorCode = 400;
            do
            {
                MemoryStream mem = new MemoryStream();
                fileDownloader.Url = _streamRoot + _chunks[iDownload].FileName;

                #region Download chunk
                LastErrorCode = fileDownloader.Download(mem);
                if (canceled || terminated)
                {
                    mem.Dispose();
                    break;
                }
                if (LastErrorCode != 200)
                {
                    mem.Dispose();

                    int errors = 0;
                    do
                    {
                        _chunks[iDownload].NextState();

                        synchronizationContext.Send(ChunkChanged_Context, this);

                        fileDownloader.Url = _streamRoot + _chunks[iDownload].FileName;
                        mem = new MemoryStream();
                        LastErrorCode = fileDownloader.Download(mem);
                        if (LastErrorCode != 200 || canceled || terminated)
                        {
                            mem.Dispose();
                            errors++;
                        }
                    } while (errors < 8 && LastErrorCode != 200 && !canceled && !terminated);
                    if (LastErrorCode != 200 || canceled || terminated)
                    {
                        mem.Dispose();
                        break;
                    }
                }
                #endregion

                if (mem.Length != CurrentChunkSize)
                {
                    LastErrorCode = FileDownloader.DOWNLOAD_ERROR_INCOMPLETE_DATA_READ;
                    mem.Dispose();
                    break;
                }

                mem.Position = 0L;
                if (DownloadingMode == DownloadingMode.WholeFile)
                {
                    bool appended = MultiThreadedDownloader.AppendStream(mem, downloadingStream);
                    mem.Dispose();
                    if (!appended)
                    {
                        LastErrorCode = MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS;
                        break;
                    }

                    DownloadedFileSize = downloadingStream.Length;
                }
                else
                {
                    string chunkFileExtension = Path.GetExtension(_chunks[iDownload].FileName);
                    string chunkFileName = $"{DownloadingFilePath}chunk{iDownload + 1}_{_chunks[iDownload].FileName}{chunkFileExtension}";
                    Stream stream = File.OpenWrite(chunkFileName);
                    bool appended = MultiThreadedDownloader.AppendStream(mem, stream);
                    stream.Dispose();
                    mem.Dispose();

                    if (!appended)
                    {
                        LastErrorCode = MultiThreadedDownloader.DOWNLOAD_ERROR_MERGING_CHUNKS;
                    }
                }
                synchronizationContext.Send(WorkFinished_Context, this);

                iDownload++;
            } while (iDownload <= ChunkTo && !canceled && !terminated && LastErrorCode == 200);
            downloadingStream?.Dispose();

            if (terminated)
            {
                LastErrorCode = ERROR_DOWNLOAD_TERMINATED;
                return;
            }

            if (canceled)
            {
                LastErrorCode = FileDownloader.DOWNLOAD_ERROR_CANCELED_BY_USER;
            }
            if (synchronizationContext != null)
            {
                synchronizationContext.Send(WorkProgress_Context, this);
                synchronizationContext.Send(Completed_Context, this);
            }
        }

        public void Cancel()
        {
            canceled = true;
        }

        public void Abort()
        {
            terminated = true;
            canceled = true;
        }

        private void Connecting_Context(object sender)
        {
            Connecting?.Invoke(sender, _chunks[iDownload]);
        }

        private void WorkStart_Context(object sender)
        {
            WorkStarted?.Invoke(sender, CurrentChunkSize, iDownload);
        }

        private void WorkProgress_Context(object sender)
        {
            WorkProgress?.Invoke(sender, CurrentChunkBytesTransfered);
        }

        private void WorkFinished_Context(object sender)
        {
            WorkFinished?.Invoke(sender, CurrentChunkBytesTransfered, LastErrorCode);
        }

        private void ChunkChanged_Context(object sender)
        {
            ChunkChanged?.Invoke(sender, iDownload);
        }

        private void Completed_Context(object sender)
        {
            Completed?.Invoke(sender, LastErrorCode);
        }
    }
}
