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
        public DOWNLOADING_MODE _downloadingMode;
        public int iDownload;
        public string fDownloadFilename;
        public long fDownloadedFileSize = 0L;
        public FileDownloader fileDownloader = null;
        public string _streamRoot;
        public List<TwitchVodChunk> _chunks;
        public int lastErrorCode;
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
            
            Stream fDownloadingStream = _downloadingMode == DOWNLOADING_MODE.DM_FILE ? File.OpenWrite(fDownloadFilename) : null;
            fileDownloader = new FileDownloader();
            fileDownloader.Connecting += (s) =>
            {
                if (context != null)
                    context.Send(OnConnecting_Context, this);
            };
            fileDownloader.WorkStart += (s, n) =>
            {
                fCurrentChunkSize = n;
                if (context != null)
                    context.Send(WorkStart_Context, this);
            };
            fileDownloader.WorkProgress += (object s, long n, ref bool c) =>
            {
                fCurrentChunkBytesTransfered += n;
                c = fCanceled;
                if (context != null)
                    context.Send(OnWorkProgress_Context, this);
            };

            fCanceled = false;
            fTerminated = false;
            fDownloadedFileSize = 0L;
            iDownload = fChunkFrom;
            lastErrorCode = 400;
            do
            {
                MemoryStream mem = new MemoryStream();
                fileDownloader.url = _streamRoot + _chunks[iDownload].fileName;

                #region Download chunk
                lastErrorCode = fileDownloader.Download(mem);
                if (fCanceled || fTerminated)
                {
                    mem.Close();
                    mem.Dispose();
                    break;
                }
                if (lastErrorCode != 200)
                {
                    mem.Close();
                    mem.Dispose();

                    int errors = 0;
                    do
                    {
                        _chunks[iDownload].NextState();

                        if (context != null)
                            context.Send(OnChunkChanged_Context, this);

                        fileDownloader.url = _streamRoot + _chunks[iDownload].fileName;
                        mem = new MemoryStream();
                        lastErrorCode = fileDownloader.Download(mem);
                        if (lastErrorCode != 200 || fCanceled || fTerminated)
                        {
                            mem.Close();
                            mem.Dispose();
                            errors++;
                        }
                    } while (errors < 8 && lastErrorCode != 200 && !fCanceled && !fTerminated);
                    if (lastErrorCode != 200 || fCanceled || fTerminated)
                    {
                        mem.Close();
                        mem.Dispose();
                        break;
                    }
                }
                #endregion

                if (mem.Length != fCurrentChunkSize)
                {
                    lastErrorCode = FileDownloader.DOWNLOAD_ERROR_INCOMPLETE_DATA_READ;
                    mem.Close();
                    mem.Dispose();
                    break;
                }

                mem.Position = 0;
                if (_downloadingMode == DOWNLOADING_MODE.DM_FILE)
                {
                    if (!FileDownloader.AppendStream(mem, fDownloadingStream))
                    {
                        mem.Close();
                        mem.Dispose();
                        lastErrorCode = FileDownloader.ERROR_MERGING_CHUNKS;
                        break;
                    }
                    mem.Close();
                    mem.Dispose();

                    fDownloadedFileSize = fDownloadingStream.Length;
                }
                else
                {
                    //TODO: Implement this
                }

                if (context != null)
                    context.Send(WorkEnd_Context, this);

                iDownload++;
            } while (iDownload <= fChunkTo && !fCanceled && !fTerminated);
            if (fDownloadingStream != null)
            {
                fDownloadingStream.Close();
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
                lastErrorCode = FileDownloader.DOWNLOAD_ERROR_TERMINATED;
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
            WorkProgress?.Invoke(sender, fileDownloader.GetBytesTransfered());
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
