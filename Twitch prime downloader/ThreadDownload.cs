using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static Twitch_prime_downloader.Utils;

namespace Twitch_prime_downloader
{
    public class ThreadDownload
    {
        private bool fCanceled = false;
        private bool fTerminated = false;
        public int fChunkFrom = 0;
        public int fChunkTo = 10;
        public long fCurrentChunkSize = 0L;
        private long fCurrentChunkBytesTransfered = 0L;
        public DownloadingMode _downloadingMode;
        public int iDownload;
        public string fDownloadFilename;
        public long fDownloadedFileSize = 0L;
        public FileDownloader fileDownloader = null;
        public string _streamRoot;
        public List<TwitchVodChunk> _chunks;
        public int lastErrorCode;
        public const int ERROR_APPENDING_STREAM = -100;

        private SynchronizationContext context;
        public delegate void ConnectingDelegate(object sender, TwitchVodChunk chunk);
        public delegate void WorkStartDelegate(object sender, long fileSize, int chunkId);
        public delegate void WorkProgressDelegate(object sender, long transferedSize);
        public delegate void WorkEndDelegate(object sender, long transferedSize, int errorCode);
        public delegate void ChunkChangedDelegate(object sender, int chunkId);
        public ConnectingDelegate Connecting;
        public WorkStartDelegate WorkStart;
        public WorkProgressDelegate WorkProgress;
        public event Action<object> OnComplete;
        public WorkEndDelegate WorkEnd;
        public ChunkChangedDelegate ChunkChanged;

        public void Work(object par)
        {
            context = (SynchronizationContext)par;
            
            Stream fDownloadingStream = _downloadingMode == DownloadingMode.WholeFile ? File.OpenWrite(fDownloadFilename) : null;

            fileDownloader = new FileDownloader();
            fileDownloader.Connecting += (s, url) =>
            {
                if (context != null)
                    context.Send(OnConnecting_Context, this);
            };
            fileDownloader.WorkStarted += (s, max) =>
            {
                fCurrentChunkSize = max;
                if (context != null)
                    context.Send(WorkStart_Context, this);
            };
            fileDownloader.WorkProgress += (s, bytes, max) =>
            {
                fCurrentChunkBytesTransfered += bytes;
                if (context != null)
                    context.Send(OnWorkProgress_Context, this);
            };
            fileDownloader.CancelTest += (object s, ref bool stop) =>
            {
                stop = fCanceled;
            };

            if (_downloadingMode == DownloadingMode.Chunked && !Directory.Exists(fDownloadFilename))
            {
                Directory.CreateDirectory(fDownloadFilename);
            }

            fCanceled = false;
            fTerminated = false;
            fDownloadedFileSize = 0L;
            iDownload = fChunkFrom;
            lastErrorCode = 400;
            do
            {
                MemoryStream mem = new MemoryStream();
                fileDownloader.Url = _streamRoot + _chunks[iDownload].fileName;

                #region Download chunk
                lastErrorCode = fileDownloader.Download(mem);
                if (fCanceled || fTerminated)
                {
                    mem.Dispose();
                    break;
                }
                if (lastErrorCode != 200)
                {
                    mem.Dispose();

                    int errors = 0;
                    do
                    {
                        _chunks[iDownload].NextState();

                        if (context != null)
                            context.Send(OnChunkChanged_Context, this);

                        fileDownloader.Url = _streamRoot + _chunks[iDownload].fileName;
                        mem = new MemoryStream();
                        lastErrorCode = fileDownloader.Download(mem);
                        if (lastErrorCode != 200 || fCanceled || fTerminated)
                        {
                            mem.Dispose();
                            errors++;
                        }
                    } while (errors < 8 && lastErrorCode != 200 && !fCanceled && !fTerminated);
                    if (lastErrorCode != 200 || fCanceled || fTerminated)
                    {
                        mem.Dispose();
                        break;
                    }
                }
                #endregion

                if (mem.Length != fCurrentChunkSize)
                {
                    lastErrorCode = FileDownloader.DOWNLOAD_ERROR_INCOMPLETE_DATA_READ;
                    mem.Dispose();
                    break;
                }

                mem.Position = 0L;
                if (_downloadingMode == DownloadingMode.WholeFile)
                {
                    bool appended = MultiThreadedDownloader.AppendStream(mem, fDownloadingStream);
                    mem.Dispose();
                    if (!appended)
                    {
                        lastErrorCode = ERROR_APPENDING_STREAM;
                        break;
                    }

                    fDownloadedFileSize = fDownloadingStream.Length;
                }
                else
                {
                    string chunkFileExtension = Path.GetExtension(_chunks[iDownload].fileName);
                    string chunkFileName = $"{fDownloadFilename}chunk{iDownload + 1}_{_chunks[iDownload].fileName}{chunkFileExtension}";
                    Stream stream = File.OpenWrite(chunkFileName);
                    bool appended = MultiThreadedDownloader.AppendStream(mem, stream);
                    stream.Dispose();
                    mem.Dispose();

                    if (!appended)
                    {
                        lastErrorCode = ERROR_APPENDING_STREAM;
                    }
                }

                if (context != null)
                    context.Send(WorkEnd_Context, this);

                iDownload++;
            } while (iDownload <= fChunkTo && !fCanceled && !fTerminated && lastErrorCode == 200);
            if (fDownloadingStream != null)
            {
                fDownloadingStream.Dispose();
            }

            if (!fTerminated)
            {
                if (fCanceled)
                    lastErrorCode = FileDownloader.DOWNLOAD_ERROR_ABORTED_BY_USER;
                if (context != null)
                {
                    context.Send(OnWorkProgress_Context, this);
                    context.Send(OnCompleted, this);
                }
            }
            else
            {
                lastErrorCode = FileDownloader.DOWNLOAD_ERROR_ABORTED_BY_USER;
            }
        }


        public void Cancel()
        {
            fTerminated = false;
            fCanceled = true;
        }

        public void Abort()
        {
            fTerminated = true;
            fCanceled = true;
        }

        private void OnConnecting_Context(object sender)
        {
            Connecting?.Invoke(sender, _chunks[iDownload]);
        }

        private void OnWorkProgress_Context(object sender)
        {
            WorkProgress?.Invoke(sender, fileDownloader.BytesTransfered);
        }

        private void OnChunkChanged_Context(object sender)
        {
            ChunkChanged?.Invoke(sender, iDownload);
        }

        private void OnCompleted(object sender)
        {
            OnComplete?.Invoke(sender);
        }

        private void WorkStart_Context(object sender)
        {
            WorkStart?.Invoke(sender, fCurrentChunkSize, iDownload);
        }

        private void WorkEnd_Context(object sender)
        {
            WorkEnd?.Invoke(sender, fCurrentChunkBytesTransfered, lastErrorCode);
        }            
            
        public long GetDownloadedStreamSize()
        {
            return fDownloadedFileSize;
        }
    }
}
