using System;
using System.IO;
using System.Net;

namespace Twitch_prime_downloader
{
    public class FileDownloader
    {
        public string url;
        public long streamSize = 0;
        public long bytesTranfered = 0;
        public long rangeFrom;
        public long rangeTo;
        public int progressUpdateInterval = 10;
        private bool stopped = false;

        public int lastErrorCode;
        public const int DOWNLOAD_ERROR_TERMINATED = -3;
        public const int DOWNLOAD_ERROR_ABORTED_BY_USER = -2;
        public const int DOWNLOAD_ERROR_UNKNOWN = -1;
        public const int DOWNLOAD_ERROR_INCOMPLETE_DATA_READ = -4;
        public const int ERROR_MERGING_CHUNKS = -5;

        public delegate void WorkStartDelegate(object sender, long fileSize);
        public delegate void WorkProgressDelegate(object sender, long bytesTransfered, ref bool stop);
        public delegate void WorkEndDelegate(object sender, long bytesTransfered, int errorCode);
        public delegate void ConnectingDelegate(object sender);
        public WorkStartDelegate WorkStart;
        public WorkProgressDelegate WorkProgress;
        public WorkEndDelegate WorkEnd;
        public ConnectingDelegate Connecting;

        public FileDownloader()
        {
            rangeFrom = 0;
            rangeTo = 0;
        }

        public int Download(Stream stream)
        {
            stopped = false;
            lastErrorCode = DOWNLOAD_ERROR_UNKNOWN;
            streamSize = stream.Length;
            bytesTranfered = 0;
            Connecting?.Invoke(this);
            HttpWebResponse response;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (rangeTo > 0)
                    request.AddRange(rangeFrom, rangeTo);
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)e.Response;
                    lastErrorCode = (int)httpWebResponse.StatusCode;
                    return lastErrorCode;
                }
                else
                {
                    lastErrorCode = 400;
                    return 400;
                }
            }
            catch (Exception)
            {
                lastErrorCode = 400;
                return 400;
            }

            Stream responseStream = response.GetResponseStream();
            long size = response.ContentLength;

            WorkStart?.Invoke(this, size);

            byte[] buf = new byte[4096];
            int bytesRead;
            long bytesAvaliable;
            lastErrorCode = 200;
            int iter = 0;
            try
            {
                do
                {
                    bytesAvaliable = size - bytesTranfered;
                    if (bytesAvaliable > 0)
                    {
                        bytesRead = responseStream.Read(buf, 0, buf.Length);
                        if (bytesRead > 0)
                        {
                            stream.Write(buf, 0, bytesRead);
                            bytesTranfered += bytesRead;
                            streamSize = stream.Length;
                            if (WorkProgress != null && (progressUpdateInterval == 0 || iter++ >= progressUpdateInterval) && !stopped)
                            {
                                WorkProgress.Invoke(this, bytesTranfered, ref stopped);
                                iter = 0;
                            }
                        }
                    }
                }
                while (bytesAvaliable > 0 && !stopped);
            }
            catch (Exception)
            {
                stopped = false;

                lastErrorCode = DOWNLOAD_ERROR_UNKNOWN;
            }
            response.Close();
            response.Dispose();
            if (stopped)
            {
                lastErrorCode = DOWNLOAD_ERROR_ABORTED_BY_USER;
            }
            else if (lastErrorCode != DOWNLOAD_ERROR_UNKNOWN)
            {
                if (size != 0 && bytesTranfered != size)
                    lastErrorCode = DOWNLOAD_ERROR_INCOMPLETE_DATA_READ;
            }

            WorkEnd?.Invoke(this, bytesTranfered, lastErrorCode);
            return lastErrorCode;
        }

        public static int GetContentLength(string url, out long contentLength)
        {
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();
                contentLength = response.ContentLength;
                response.Close();
                response.Dispose();
                return 200;
            }
            catch (WebException e)
            {
                contentLength = 0;
                if (response != null)
                {
                    response.Close();
                    response.Dispose();
                }
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpWebResponse = (HttpWebResponse)e.Response;
                    url = e.Message;
                    return (int)httpWebResponse.StatusCode;
                }
                else
                    return 400;
            }
            catch (Exception)
            {
                if (response != null)
                {
                    response.Close();
                    response.Dispose();
                }
                contentLength = 0;
                return 400;
            }
        }

        public void Stop()
        {
            stopped = true;
        }

        public long GetBytesTransfered()
        {
            return bytesTranfered;
        }

        public long GetStreamSize()
        {
            return streamSize;
        }

        public static bool AppendStream(Stream streamFrom, Stream streamTo)
        {
            long size = streamTo.Length;
            byte[] buf = new byte[4096];
            int bytesRead;
            do
            {
                bytesRead = streamFrom.Read(buf, 0, buf.Length);
                if (bytesRead > 0)
                    streamTo.Write(buf, 0, bytesRead);
            } while (bytesRead > 0);
            return streamTo.Length == size + streamFrom.Length;
        }

    }
}
