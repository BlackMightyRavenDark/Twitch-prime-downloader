using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TwitchApiLib;

namespace Twitch_prime_downloader
{
    public static class Helper
    {
        public static string ToText(this IEnumerable<string> collection)
        {
            string res = string.Empty;
            foreach (string str in collection)
            {
                res += str + Environment.NewLine;
            }
            return res;
        }

        public static void LoadFromFile(this ComboBox comboBox, string fileName)
        {
            try
            {
                using (StreamReader file = new StreamReader(fileName))
                {
                    string s;
                    while ((s = file.ReadLine()) != null)
                    {
                        comboBox.Items.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }

        public static void SaveToFile(this ComboBox comboBox, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                try
                {
                    using (StreamWriter sw = new StreamWriter(fileName))
                    {
                        for (int i = 0; i < comboBox.Items.Count; ++i)
                        {
                            sw.WriteLine(comboBox.Items[i]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        public static void SaveToFile(this IEnumerable<string> collection, string fileName)
        {
            string t = collection.ToText();
            File.WriteAllText(fileName, t);
        }

        public static bool SaveToFile(this Stream stream, string fileName, bool fromOrigin = true)
        {
            Stream fileStream = File.OpenWrite(fileName);
            if (fromOrigin)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            bool res = MultiThreadedDownloaderLib.StreamAppender.Append(stream, fileStream);
            fileStream.Dispose();
            return res;
        }

        public static DateTime ToLocal(this DateTime dateTime)
        {
            return dateTime.IsGmt() ? dateTime.ToLocalTime() : dateTime;
        }

        public static string FormatThumbnailUrl(this TwitchVod vod, ushort imageWidth, ushort imageHeight)
        {
            return vod.ThumbnailUrl?
                .Replace("%{width}", imageWidth.ToString())
                .Replace("%{height}", imageHeight.ToString());
        }

        public static TwitchVodMutedSegments GetMutedSegments(this TwitchVod vod)
        {
            TwitchVodPlaylist playlist = vod.GetPlaylist();
            if (playlist != null)
            {
                playlist.Parse();
                if (playlist.Count > 0)
                {
                    List<TwitchVodChunk> chunks = playlist.GetChunkList();
                    TwitchVodMutedSegments mutedSegments = TwitchVodMutedSegments.ParseMutedSegments(chunks);
                    mutedSegments.BuildSegmentList();
                    mutedSegments.CalculateTotalDuration();
                    return mutedSegments;
                }
            }

            return null;
        }

        public static List<TwitchVodChunk> GetChunkList(this TwitchVodPlaylist playlist)
        {
            List<TwitchVodChunk> chunks = new List<TwitchVodChunk>();
            for (int i = 0; i < playlist.Count; ++i)
            {
                chunks.Add(playlist.GetChunk(i));
            }
            return chunks;
        }
    }
}
