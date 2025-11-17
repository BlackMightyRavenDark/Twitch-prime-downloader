using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
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
				string t = File.ReadAllText(fileName);
				comboBox.Items.Clear();
				string[] splitted = t.Split(new string[] { "\n", "\r\n" }, 2, StringSplitOptions.None);
				foreach (string str in splitted)
				{
					if (!string.IsNullOrEmpty(str)) { comboBox.Items.Add(str);}
				}
			}
#if DEBUG
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
			}
#else
			catch { }
#endif
		}

		public static void SaveToFile(this ComboBox comboBox, string fileName)
		{
			try
			{
				if (!string.IsNullOrEmpty(fileName))
				{
					if (File.Exists(fileName)) { File.Delete(fileName); }

					using (StreamWriter sw = new StreamWriter(fileName))
					{
						for (int i = 0; i < comboBox.Items.Count; ++i)
						{
							sw.WriteLine(comboBox.Items[i]);
						}
					}
				}
			}
#if DEBUG
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.StackTrace);
			}
#else
			catch { }
#endif
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

		internal static JObject Serialize(this TwitchVodChunk chunk, long position, long fileSize)
		{
			return new JObject()
			{
				["position"] = position,
				["size"] = fileSize,
				["fileName"] = chunk.FileName
			};
		}
	}
}
