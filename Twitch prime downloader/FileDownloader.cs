using System;
using System.IO;
using System.Net;
using System.Text;

namespace Twitch_prime_downloader
{
    public sealed class FileDownloader
    {
        public string Url { get; set; }
        public long StreamSize { get; private set; } = 0L;
        public long BytesTransfered { get; private set; } = 0L;
        private long _rangeFrom = 0L;
        private long _rangeTo = 0L;
        public string Accept { get; set; }
        public WebHeaderCollection Headers { get; private set; } = new WebHeaderCollection();
        public int ProgressUpdateInterval { get; set; } = 10;
        public bool Stopped { get; private set; } = false;
        public int LastErrorCode { get; private set; } = DOWNLOAD_ERROR_UNKNOWN;

        public const int DOWNLOAD_ERROR_UNKNOWN = -1;
        public const int DOWNLOAD_ERROR_ABORTED_BY_USER = -2;
        public const int DOWNLOAD_ERROR_INCOMPLETE_DATA_READ = -3;
        public const int DOWNLOAD_ERROR_RANGE = -4;
        public const int DOWNLOAD_ERROR_ZERO_LENGTH_CONTENT = -5;

        public delegate void WorkStartedDelegate(object sender, long contentLength);
        public delegate void WorkProgressDelegate(object sender, long bytesTransfered, long contentLength);
        public delegate void WorkFinishedDelegate(object sender, long bytesTransfered, long contentLength, int errorCode);
        public delegate void ConnectingDelegate(object sender, string url);
        public delegate void CancelTestDelegate(object sender, ref bool stop);
        public WorkStartedDelegate WorkStarted;
        public WorkProgressDelegate WorkProgress;
        public WorkFinishedDelegate WorkFinished;
        public ConnectingDelegate Connecting;
        public CancelTestDelegate CancelTest;

        public int Download(Stream stream)
        {
            Stopped = false;
            LastErrorCode = DOWNLOAD_ERROR_UNKNOWN;
            BytesTransfered = 0L;
            StreamSize = stream.Length;

            Connecting?.Invoke(this, Url);
            WebContent content = new WebContent();
            content.Accept = Accept;
            content.Headers = Headers;
            LastErrorCode = content.GetResponseStream(Url, _rangeFrom, _rangeTo);
            if (LastErrorCode != 200 && LastErrorCode != 206)
            {
                content.Dispose();
                return LastErrorCode;
            }

            if (content.Length == 0L)
            {
                content.Dispose();
                return DOWNLOAD_ERROR_ZERO_LENGTH_CONTENT;
            }

            WorkStarted?.Invoke(this, content.Length);

            LastErrorCode = ContentToStream(content, stream);
            long size = content.Length;
            content.Dispose();

            WorkFinished?.Invoke(this, BytesTransfered, size, LastErrorCode);

            return LastErrorCode;
        }

        public int DownloadString(out string resString)
        {
            resString = null;

            Stopped = false;
            LastErrorCode = DOWNLOAD_ERROR_UNKNOWN;
            BytesTransfered = 0L;
            StreamSize = 0L;

            Connecting?.Invoke(this, Url);
            WebContent content = new WebContent();
            content.Accept = Accept;
            content.Headers = Headers;
            LastErrorCode = content.GetResponseStream(Url, _rangeFrom, _rangeTo);
            if (LastErrorCode != 200 && LastErrorCode != 206)
            {
                content.Dispose();
                return LastErrorCode;
            }

            if (content.Length == 0L)
            {
                content.Dispose();
                return DOWNLOAD_ERROR_ZERO_LENGTH_CONTENT;
            }

            WorkStarted?.Invoke(this, content.Length);

            MemoryStream memoryStream = new MemoryStream();
            LastErrorCode = ContentToStream(content, memoryStream);
            if (LastErrorCode == 200)
            {
                resString = Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
            long size = content.Length;
            content.Dispose();
            memoryStream.Dispose();

            WorkFinished?.Invoke(this, BytesTransfered, size, LastErrorCode);

            return LastErrorCode;
        }

        private int ContentToStream(WebContent content, Stream stream)
        {
            byte[] buf = new byte[4096];
            int bytesRead;
            int errorCode = 200;
            int iter = 0;
            try
            {
                do
                {
                    bytesRead = content.ContentData.Read(buf, 0, buf.Length);
                    if (bytesRead <= 0)
                    {
                        break;
                    }
                    stream.Write(buf, 0, bytesRead);
                    BytesTransfered += bytesRead;
                    StreamSize = stream.Length;
                    if (WorkProgress != null && (ProgressUpdateInterval == 0 || iter++ >= ProgressUpdateInterval))
                    {
                        WorkProgress.Invoke(this, BytesTransfered, content.Length);
                        iter = 0;
                    }
                    if (CancelTest != null)
                    {
                        bool stop = false;
                        CancelTest.Invoke(this, ref stop);
                        Stopped = stop;
                        if (Stopped)
                        {
                            break;
                        }
                    }
                }
                while (bytesRead > 0);
            }
            catch (Exception)
            {
                errorCode = DOWNLOAD_ERROR_UNKNOWN;
            }
            if (Stopped)
            {
                errorCode = DOWNLOAD_ERROR_ABORTED_BY_USER;
            }
            else if (errorCode == 200)
            {
                if (content.Length >= 0L && BytesTransfered != content.Length)
                {
                    LastErrorCode = DOWNLOAD_ERROR_INCOMPLETE_DATA_READ;
                }
            }
            return errorCode;
        }

        public void SetRange(long from, long to)
        {
            _rangeFrom = from;
            _rangeTo = to;
        }
    }

    public sealed class WebContent : IDisposable
    {
        private HttpWebResponse webResponse = null;

        public WebHeaderCollection Headers { get; set; }
        public string Accept { get; set; }
        public long Length { get; private set; } = -1L;
        public Stream ContentData { get; private set; } = null;

        public void Dispose()
        {
            if (webResponse != null)
            {
                webResponse.Dispose();
                webResponse = null;
            }
            if (ContentData != null)
            {
                ContentData.Dispose();
                ContentData = null;
                Length = -1L;
            }
        }

        public int GetResponseStream(string url)
        {
            int errorCode = GetResponseStream(url, 0L, 0L);
            return errorCode;
        }

        public int GetResponseStream(string url, long rangeFrom, long rangeTo)
        {
            int errorCode = GetResponseStream(url, rangeFrom, rangeTo, out Stream stream);
            if (errorCode == 200 || errorCode == 206)
            {
                ContentData = stream;
                Length = webResponse.ContentLength;
            }
            else
            {
                ContentData = null;
                Length = -1L;
            }
            return errorCode;
        }

        public int GetResponseStream(string url, long rangeFrom, long rangeTo, out Stream stream)
        {
            stream = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (rangeTo > 0L)
                {
                    if (rangeFrom > rangeTo)
                    {
                        return FileDownloader.DOWNLOAD_ERROR_RANGE;
                    }
                    request.AddRange(rangeFrom, rangeTo);
                }

                request.Accept = Accept;
                if (Headers != null)
                {
                    request.Headers = Headers;
                }

                webResponse = (HttpWebResponse)request.GetResponse();
                stream = webResponse.GetResponseStream();
                return 200;
            }
            catch (WebException ex)
            {
                if (webResponse != null)
                {
                    webResponse.Dispose();
                    webResponse = null;
                }
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
                    int statusCode = (int)httpWebResponse.StatusCode;
                    return statusCode;
                }
                else
                {
                    return ex.HResult;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (webResponse != null)
                {
                    webResponse.Dispose();
                    webResponse = null;
                }
                return ex.HResult;
            }
        }
    }
}
