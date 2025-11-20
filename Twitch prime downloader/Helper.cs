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

		public static IEnumerable<string> GetStrings(this ListBox listBox)
		{
			foreach (var item in listBox.Items)
			{
				yield return item.ToString();
			}
		}

		public static string GetWholeText(this ListBox listBox)
		{
			string t = string.Empty;
			foreach (var item in listBox.Items)
			{
				t += item.ToString() + Environment.NewLine;
			}
			return t;
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
			JObject j = new JObject();
			if (position >= 0L) { j["position"] = position; }
			j["size"] = fileSize;
			j["length"] = chunk.Duration;
			j["offset"] = chunk.Offset;
			j["fileName"] = chunk.FileName;
			return j;
		}
	}
}
