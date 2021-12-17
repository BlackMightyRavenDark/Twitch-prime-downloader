using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Twitch_prime_downloader.FileDownloader;

namespace Twitch_prime_downloader
{
    public sealed class MultiThreadedDownloader
    {
        private sealed class ProgressItem
        {
            public string FileName { get; set; }
            public int Id { get; }
            public long Processed { get; }
            public long Total { get; }

            public ProgressItem(string fileName, int id, long processed, long total)
            {
                FileName = fileName;
                Id = id;
                Processed = processed;
                Total = total;
            }
        }

        public const int MEGABYTE = 1048576; //1024 * 1024;

        public const int DOWNLOAD_ERROR_MERGING_CHUNKS = -200;
        public const int DOWNLOAD_ERROR_CREATE_FILE = -201;
        public const int DOWNLOAD_ERROR_CANCELED = -202;
        public const int DOWNLOAD_ERROR_NO_URL_SPECIFIED = -203;
        public const int DOWNLOAD_ERROR_NO_FILE_NAME_SPECIFIED = -204;
        public const int DOWNLOAD_ERROR_TEMPORARY_DIR_NOT_EXISTS = -205;
        public const int DOWNLOAD_ERROR_MERGING_DIR_NOT_EXISTS = -206;

        public delegate void DownloadStartedDelegate(object sender, long contentLenth);
        public delegate void DownloadProgressDelegate(object sender, long bytesTransfered);
        public delegate void DownloadFinishedDelegate(object sender, long bytesTransfered, int errorCode, string fileName);
        public delegate void CancelTestDelegate(object sender, ref bool cancel);
        public delegate void MergingStartedDelegate(object sender, int chunkCount);
        public delegate void MergingProgressDelegate(object sender, int chunkId);
        public delegate void MergingFinishedDelegate(object sender, int errorCode);

        public DownloadStartedDelegate DownloadStarted;
        public DownloadProgressDelegate DownloadProgress;
        public DownloadFinishedDelegate DownloadFinished;
        public CancelTestDelegate CancelTest;
        public MergingStartedDelegate MergingStarted;
        public MergingProgressDelegate MergingProgress;
        public MergingFinishedDelegate MergingFinished;


        public string Url { get; set; } = null;
        /// <summary>
        /// Warning! The file name will be automatically changed after downloading if a file with that name already exists!
        /// Therefore, you need to double-check this value after the download is complete.
        /// </summary>
        public string OutputFileName { get; set; } = null;
        public string TempDirectory { get; set; } = null;
        public string MergingDirectory { get; set; } = null;
        public long ContentLength { get; private set; } = -1L;
        public long DownloadedBytes { get; private set; } = 0L;
        public int UpdateInterval { get; set; } = 10;
        public int ThreadCount { get; set; } = 2;
        private bool aborted = false;
        public List<string> Chunks { get; private set; } = new List<string>();

        public static string GetNumberedFileName(string fn)
        {
            if (File.Exists(fn))
            {
                int n = fn.LastIndexOf(".");
                string part1 = fn.Substring(0, n);
                string ext = fn.Substring(n, fn.Length - n);
                string newFileName;
                int i = 2;
                do
                {
                    newFileName = $"{part1}_{i++}{ext}";
                } while (File.Exists(newFileName));
                return newFileName;
            }
            else
            {
                return fn;
            }
        }

        public static bool AppendStream(Stream streamFrom, Stream streamTo)
        {
            long size = streamTo.Length;
            byte[] buf = new byte[4096];
            do
            {
                int bytesRead = streamFrom.Read(buf, 0, buf.Length);
                if (bytesRead <= 0)
                {
                    break;
                }
                streamTo.Write(buf, 0, bytesRead);
            } while (true);

            return streamTo.Length == size + streamFrom.Length;
        }

        public static int GetUrlContentLength(string url, out long contentLength)
        {
            WebContent webContent = new WebContent();
            int errorCode = webContent.GetResponseStream(url);
            contentLength = errorCode == 200 ? webContent.Length : -1L;
            webContent.Dispose();
            return errorCode;
        }

        private IEnumerable<Tuple<long, long>> Split(long contentLength, int chunkCount)
        {
            if (chunkCount <= 1 || contentLength <= MEGABYTE)
            {
                yield return new Tuple<long, long>(0, contentLength - 1);
                yield break;
            }
            long chunkSize = contentLength / chunkCount;
            for (int i = 0; i < chunkCount; i++)
            {
                long startPos = chunkSize * i;
                bool lastChunk = i == chunkCount - 1;
                long endPos = lastChunk ? (contentLength - 1) : (startPos + chunkSize - 1);
                yield return new Tuple<long, long>(startPos, endPos);
            }
        }

        public async Task<int> Download()
        {
            aborted = false;
            DownloadedBytes = 0;
            if (string.IsNullOrEmpty(Url) || string.IsNullOrWhiteSpace(Url))
            {
                return DOWNLOAD_ERROR_NO_URL_SPECIFIED;
            }
            if (string.IsNullOrEmpty(OutputFileName) || string.IsNullOrWhiteSpace(OutputFileName))
            {
                return DOWNLOAD_ERROR_NO_FILE_NAME_SPECIFIED;
            }
            if (!string.IsNullOrEmpty(TempDirectory) && !string.IsNullOrWhiteSpace(TempDirectory) && !Directory.Exists(TempDirectory))
            {
                return DOWNLOAD_ERROR_TEMPORARY_DIR_NOT_EXISTS;
            }
            if (!string.IsNullOrEmpty(MergingDirectory) && !string.IsNullOrWhiteSpace(MergingDirectory) && !Directory.Exists(MergingDirectory))
            {
                return DOWNLOAD_ERROR_MERGING_DIR_NOT_EXISTS;
            }
            int errorCode = GetUrlContentLength(Url, out long contentLength);
            if (errorCode != 200)
            {
                return errorCode;
            }
            if (contentLength == 0)
            {
                return DOWNLOAD_ERROR_ZERO_LENGTH_CONTENT;
            }

            ContentLength = contentLength;
            DownloadStarted?.Invoke(this, contentLength);

            Dictionary<int, ProgressItem> threadProgressDict = new Dictionary<int, ProgressItem>();
            Progress<ProgressItem> progress = new Progress<ProgressItem>();
            progress.ProgressChanged += (s, progressItem) =>
            {
                threadProgressDict[progressItem.Id] = progressItem;

                DownloadedBytes = threadProgressDict.Values.Select(it => it.Processed).Sum();

                DownloadProgress?.Invoke(this, DownloadedBytes);
                CancelTest?.Invoke(this, ref aborted);
            };

            if (string.IsNullOrEmpty(TempDirectory) || string.IsNullOrWhiteSpace(TempDirectory))
            {
                TempDirectory = Path.GetDirectoryName(OutputFileName);
            }

            if (ThreadCount <= 0)
            {
                ThreadCount = 2;
            }
            int chunkCount = contentLength > MEGABYTE ? ThreadCount : 1;
            var tasks = Split(contentLength, chunkCount).Select((range, id) => Task.Run(() =>
            {
                long chunkFirstByte = range.Item1;
                long chunkLastByte = range.Item2;

                IProgress<ProgressItem> reporter = progress;

                string chunkFileName;
                if (chunkCount > 1)
                {
                    string path = Path.GetFileName(OutputFileName);
                    chunkFileName = $"{path}.chunk_{id}.tmp";
                    if (!string.IsNullOrEmpty(TempDirectory) && !string.IsNullOrWhiteSpace(TempDirectory))
                    {
                        chunkFileName = TempDirectory.EndsWith("\\") ?
                            TempDirectory + chunkFileName : $"{TempDirectory}\\{chunkFileName}";
                    }
                }
                else
                {
                    chunkFileName = OutputFileName + ".tmp";
                }

                chunkFileName = GetNumberedFileName(chunkFileName);

                FileDownloader downloader = new FileDownloader();
                downloader.ProgressUpdateInterval = UpdateInterval;
                downloader.Url = Url;
                downloader.SetRange(chunkFirstByte, chunkLastByte);
                downloader.WorkProgress += (object sender, long transfered, long contentLen) =>
                {
                    reporter.Report(new ProgressItem(chunkFileName, id, transfered, chunkLastByte));
                };
                downloader.WorkFinished += (object sender, long transfered, long contentLen, int errCode) =>
                {
                    reporter.Report(new ProgressItem(chunkFileName, id, transfered, chunkLastByte));
                };
                downloader.CancelTest += (object s, ref bool stop) =>
                {
                    stop = aborted;
                };

                Stream stream = File.OpenWrite(chunkFileName);
                errorCode = downloader.Download(stream);
                stream.Dispose();

                if (errorCode != 200 && errorCode != 206)
                {
                    if (aborted)
                    {
                        throw new OperationCanceledException();
                    }
                    throw new Exception($"Error code = {errorCode}");
                }
            }
            ));

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return DOWNLOAD_ERROR_CANCELED;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return ex.HResult;
            }

            Chunks.Clear();
            for (int i = 0; i < threadProgressDict.Count; i++)
            {
                Chunks.Add(threadProgressDict[i].FileName);
            }
            if (Chunks.Count > 1)
            {
                MergingStarted?.Invoke(this, Chunks.Count);
                errorCode = await MergeChunks();
                MergingFinished?.Invoke(this, errorCode);
            }
            else
            {
                string chunkFileName = Chunks[0];
                if (File.Exists(chunkFileName))
                {
                    OutputFileName = GetNumberedFileName(OutputFileName);
                    File.Move(chunkFileName, OutputFileName);
                }
                errorCode = 200;
            }
            Chunks.Clear();

            DownloadFinished?.Invoke(this, DownloadedBytes, errorCode, OutputFileName);

            return errorCode;
        }

        private async Task<int> MergeChunks()
        {
            Progress<int> progressMerging = new Progress<int>();
            progressMerging.ProgressChanged += (s, n) =>
            {
                MergingProgress?.Invoke(this, n);
            };

            int res = await Task.Run(() =>
            {
                string tmpFileName;
                if (!string.IsNullOrEmpty(MergingDirectory) &&
                    !string.IsNullOrWhiteSpace(MergingDirectory) &&
                    Directory.Exists(MergingDirectory))
                {
                    string fn = Path.GetFileName(OutputFileName);
                    tmpFileName = MergingDirectory.EndsWith("\\") ?
                        $"{MergingDirectory}{fn}.tmp" : $"{MergingDirectory}\\{fn}.tmp";
                }
                else
                {
                    tmpFileName = $"{OutputFileName}.tmp";
                }
                tmpFileName = GetNumberedFileName(tmpFileName);

                Stream outputStream = null;
                try
                {
                    outputStream = File.OpenWrite(tmpFileName);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    if (outputStream != null)
                    {
                        outputStream.Dispose();
                    }
                    return DOWNLOAD_ERROR_CREATE_FILE;
                }

                IProgress<int> reporter = progressMerging;
                try
                {
                    for (int i = 0; i < Chunks.Count; i++)
                    {
                        string chunkFileName = Chunks[i];
                        if (!File.Exists(chunkFileName))
                        {
                            return DOWNLOAD_ERROR_MERGING_CHUNKS;
                        }
                        Stream streamChunk = File.OpenRead(chunkFileName);
                        bool appended = AppendStream(streamChunk, outputStream);
                        streamChunk.Dispose();
                        if (!appended)
                        {
                            outputStream.Dispose();
                            return DOWNLOAD_ERROR_MERGING_CHUNKS;
                        }

                        File.Delete(chunkFileName);
                        reporter.Report(i);

                        if (CancelTest != null)
                        {
                            CancelTest.Invoke(this, ref aborted);
                            if (aborted)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    outputStream.Dispose();
                    return DOWNLOAD_ERROR_MERGING_CHUNKS;
                }
                outputStream.Dispose();

                if (aborted)
                {
                    return DOWNLOAD_ERROR_CANCELED;
                }

                OutputFileName = GetNumberedFileName(OutputFileName);
                File.Move(tmpFileName, OutputFileName);

                return 200;
            });

            return res;
        }
    }
}
