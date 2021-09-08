using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Twitch_prime_downloader
{
    public class MultyThreadDownloader
    {
        public class ProgressItem
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

        public const int ERROR_BAD_FILE_NAME = -200;
        public const int ERROR_DOWNLOAD_CANCELED = -201;

        public delegate void OnDownloadStartDelegate(object sender, long fileSize);
        public delegate void OnProgressDelegate(object sender, long bytesTransfered);
        public delegate void OnDownloadFinishDelegate(object sender, long bytesTransfered);
        public delegate void CancelTestDelegate(object sender, ref bool cancel);
        public delegate void MergingStartDelegate(object sender, int chunkCount);
        public delegate void MergingProgressDelegate(object sender, int chunkId);

        public OnDownloadStartDelegate DownloadStart;
        public OnProgressDelegate DownloadProgress;
        public OnDownloadFinishDelegate DownloadFinish;
        public CancelTestDelegate CancelTest;
        public MergingStartDelegate MergingStart;
        public MergingProgressDelegate MergingProgress;


        public string Url { get; set; }
        public string tempDirectory = string.Empty;
        public string outputFileName = string.Empty;
        private long fileSize = 0;
        public long ContentLength { get { return fileSize; } }
        private long downloadedBytes = 0;
        public long DownloadedBytes { get { return downloadedBytes; } }
        public int UpdateInterval { get; set; } = 10;
        public int threadCount = 2;
        private bool aborted = false;

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
                    newFileName = part1 + "_" + i++.ToString() + ext;
                } while (File.Exists(newFileName));
                return newFileName;
            }
            return fn;
        }

        public static int GetUrlFileSize(string url, out long contentLength)
        {
            return FileDownloader.GetContentLength(url, out contentLength);
        }

        private IEnumerable<Tuple<long, long>> Split(long length, int chunkCount)
        {
            if (chunkCount <= 1)
            {
                yield return new Tuple<long, long>(0, length - 1);
                yield break;
            }
            long chunkSize = length / chunkCount;
            for (int i = 0; i < chunkCount; i++)
            {
                long startPos = chunkSize * i;
                long endPos = i == chunkCount - 1 ? (length - 1) : (startPos + chunkSize - 1);
                yield return new Tuple<long, long>(startPos, endPos);
            }
        }

        public async Task<int> Download()
        {
            aborted = false;
            downloadedBytes = 0;
            if (string.IsNullOrEmpty(outputFileName) || string.IsNullOrWhiteSpace(outputFileName))
                return 400;
            if (string.IsNullOrEmpty(Url) || string.IsNullOrWhiteSpace(Url))
                return 400;
            if (GetUrlFileSize(Url, out long contentLength) != 200)
                return 404;
            fileSize = contentLength;
            DownloadStart?.Invoke(this, contentLength);
            int chunkCount = contentLength > 0 ? threadCount : 1;

            Dictionary<int, ProgressItem> threadProgressDict = new Dictionary<int, ProgressItem>();
            Progress<ProgressItem> progress = new Progress<ProgressItem>();
            progress.ProgressChanged += (s, p) =>
            {
                threadProgressDict[p.Id] = p;

                downloadedBytes = threadProgressDict.Values.Select(it => it.Processed).Sum();
                double percent = 100 / (double)contentLength * downloadedBytes;

                DownloadProgress?.Invoke(this, downloadedBytes);
                CancelTest?.Invoke(this, ref aborted);
            };

            if (string.IsNullOrEmpty(tempDirectory) || string.IsNullOrWhiteSpace(tempDirectory))
                tempDirectory = Path.GetDirectoryName(outputFileName);

            var tasks = Split(contentLength, chunkCount)
                .Select((range, id) => Task.Run(() =>
                {
                    IProgress<ProgressItem> reporter = progress;

                    FileDownloader downloader = new FileDownloader();
                    downloader.progressUpdateInterval = UpdateInterval;

                    string chunkFileName;
                    if (chunkCount > 1)
                    {
                        chunkFileName = Path.GetFileName(outputFileName) +
                            ".chunk_" + id.ToString() + ".tmp";
                        if (!string.IsNullOrEmpty(tempDirectory))
                            chunkFileName = tempDirectory + chunkFileName;
                    }
                    else
                        chunkFileName = outputFileName + ".tmp";

                    chunkFileName = GetNumberedFileName(chunkFileName);
                    downloader.url = Url;
                    downloader.rangeFrom = range.Item1;
                    downloader.rangeTo = range.Item2;
                    downloader.WorkProgress += (object sender1, long n, ref bool stop) =>
                    {
                        reporter.Report(new ProgressItem(chunkFileName, id, (int)n, range.Item2));
                        stop = aborted;
                    };
                    downloader.WorkEnd += (s, n, errCode) =>
                    {
                        reporter.Report(new ProgressItem(chunkFileName, id, (int)n, range.Item2));
                    };

                    Stream stream = File.OpenWrite(chunkFileName);
                    int errorCode = downloader.Download(stream);

                    stream.Close();
                    stream.Dispose();

                    if (errorCode != 200 && errorCode != 206)
                    {
                        if (aborted)
                            throw new OperationCanceledException();
                        throw new Exception("Errorcode = " + errorCode.ToString());
                    }
                }
                ));

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                return ERROR_DOWNLOAD_CANCELED;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return 400;
            }

            if (threadCount > 1)
            {
                MergingStart?.Invoke(this, threadProgressDict.Count);
                Progress<int> progressMerging = new Progress<int>();
                progressMerging.ProgressChanged += (s, n) =>
                {
                    MergingProgress?.Invoke(this, n);
                };

                int res = await Task.Run(() =>
                {
                    string tmpFileName = GetNumberedFileName(outputFileName + ".tmp");


                    Stream outputStream = null;
                    try
                    {
                        outputStream = File.OpenWrite(tmpFileName);
                    }
                    catch
                    {
                        return ERROR_BAD_FILE_NAME;
                    }
                    IProgress<int> rep = progressMerging;

                    for (int i = 0; i < threadProgressDict.Count; i++)
                    {
                        ProgressItem item = threadProgressDict[i];
                        try
                        {
                            Stream s = File.OpenRead(item.FileName);
                            if (!FileDownloader.AppendStream(s, outputStream))
                            {
                                outputStream.Close();
                                outputStream.Dispose();
                                s.Close();
                                s.Dispose();
                                return FileDownloader.ERROR_MERGING_CHUNKS;
                            }
                            s.Close();
                            s.Dispose();
                        }
                        catch
                        {
                            return FileDownloader.ERROR_MERGING_CHUNKS;
                        }
                        File.Delete(item.FileName);
                        rep.Report(i);
                    }
                    outputStream.Close();
                    outputStream.Dispose();

                    outputFileName = GetNumberedFileName(outputFileName);
                    File.Move(tmpFileName, outputFileName);

                    return 200;
                });
                DownloadFinish?.Invoke(this, downloadedBytes);
                return res;
            }
            else
            {
                string t = threadProgressDict[0].FileName;
                if (File.Exists(t))
                {
                    outputFileName = GetNumberedFileName(outputFileName);
                    File.Move(t, outputFileName);
                }
                DownloadFinish?.Invoke(this, downloadedBytes);
            }

            return 200;
        }
    }

}
