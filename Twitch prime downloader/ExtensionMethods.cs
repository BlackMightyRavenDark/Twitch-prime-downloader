using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using TwitchApiLib;

namespace Twitch_prime_downloader
{
	internal static class ExtensionMethods
	{
		internal static string ToText(this IEnumerable<string> collection)
		{
			return string.Join(Environment.NewLine, collection);
		}

		internal static IEnumerable<string> GetStrings(this ListBox listBox)
		{
			foreach (var item in listBox.Items)
			{
				yield return item.ToString();
			}
		}

		internal static void SaveToFile(this IEnumerable<string> collection, string fileName)
		{
			string t = collection.ToText();
			File.WriteAllText(fileName, t);
		}

		internal static bool SaveToFile(this Stream stream, string fileName, bool fromOrigin = true)
		{
			using (Stream fileStream = File.OpenWrite(fileName))
			{
				if (fromOrigin) { stream.Position = 0L; }
				return MultiThreadedDownloaderLib.StreamAppender.Append(stream, fileStream);
			}
		}

		internal static Rectangle Deflate(this Rectangle rectangle, int width, int height)
		{
			return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width - width, rectangle.Height - height);
		}

		internal static void FixMutedChunkUrls(this TwitchPlaylist playlist)
		{
			for (int i = 0; i < playlist.Count; ++i)
			{
				if (playlist[i].GetState() == TwitchVodChunk.TwitchVodChunkState.Unmuted)
				{
					playlist[i].SetState(TwitchVodChunk.TwitchVodChunkState.Muted);
				}
			}
		}

		internal static JObject Serialize(this TwitchVodChunk chunk, long position, long fileSize)
		{
			JObject j = new JObject();
			if (chunk.Id >= 0) { j["id"] = chunk.Id; }
			if (position >= 0L) { j["position"] = position; }
			j["size"] = fileSize;
			if (chunk.Duration >= 0.0)
			{
				j["length"] = chunk.Duration;
			}
			if (chunk.Offset >= 0.0)
			{
				j["offset"] = chunk.Offset;
			}
			if (chunk.AbsoluteOffset >= 0.0 && chunk.AbsoluteOffset != chunk.Offset)
			{
				j["absoluteOffset"] = chunk.AbsoluteOffset;
			}
			if (chunk.CreationDate.Year > 2000 && chunk.CreationDate < DateTime.MaxValue)
			{
				j["creationDate"] = chunk.CreationDate;
			}
			if (!string.IsNullOrEmpty(chunk.FileName) && !string.IsNullOrWhiteSpace(chunk.FileName))
			{
				j["fileName"] = chunk.FileName;
			}
			return j;
		}
	}
}
