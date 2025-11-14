using System;
using System.Collections.Generic;
using System.Drawing;
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
			if (fromOrigin) { stream.Position = 0L; }
			bool res = MultiThreadedDownloaderLib.StreamAppender.Append(stream, fileStream);
			fileStream.Dispose();
			return res;
		}

		public static Rectangle Deflate(this Rectangle rectangle, int width, int height)
		{
			return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width - width, rectangle.Height - height);
		}

		public static DateTime ToLocal(this DateTime dateTime)
		{
			return dateTime.IsGmt() ? dateTime.ToLocalTime() : dateTime;
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
