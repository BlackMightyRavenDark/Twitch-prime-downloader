using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using TwitchApiLib;

namespace Twitch_prime_downloader
{
	internal class TwitchVodChunkWrapper : TwitchVodChunk
	{
		private string _fileExtension;

		public TwitchVodChunkWrapper(TwitchVodChunk chunk) : base(chunk)
		{
			_fileExtension = Path.GetExtension(chunk.FileName);
		}

		public JArray ExtractSubChunks(byte[] chunkData, long chunkPosition)
		{
			List<long> positions = new List<long>();
			List<string> fileNames = new List<string>();
			List<DateTime> creationDates = new List<DateTime>();

			if (_fileExtension == ".mp4")
			{
				byte[] sample = new byte[] { (byte)'e', (byte)'m', (byte)'s', (byte)'g' };
				for (long i = 0L; i < chunkData.LongLength - sample.LongLength; ++i)
				{
					if (CompareSample(sample, chunkData, i))
					{
						positions.Add(i - 4);
						fileNames.Add(ExtractChunkFileName(chunkData, i));
						creationDates.Add(ExtractChunkCreationDate(chunkData, i));
					}
				}
			}
			else
			{
				byte[] sample = new byte[] { (byte)'T', (byte)'R', (byte)'C', (byte)'K' };
				byte[] sample2 = new byte[] { (byte)'G', (byte)'@' };
				const int maxBackwardRange = 1024;
				for (long i = 0L; i < chunkData.LongLength - sample.LongLength; ++i)
				{
					if (CompareSample(sample, chunkData, i))
					{
						for (int j = sample2.Length; j < maxBackwardRange; ++j)
						{
							long pos = i - j;
							if ((positions.Count > 0 && pos <= positions[positions.Count - 1]) || pos < 0L) { break; }
							if (CompareSample(sample2, chunkData, pos))
							{
								positions.Add(pos);
								break;
							}
						}

						fileNames.Add(ExtractChunkFileName(chunkData, i));
						creationDates.Add(ExtractChunkCreationDate(chunkData, i));
					}
				}
			}

			if (positions.Count > 0)
			{
				JArray ja = new JArray();
				int subChunkCount = positions.Count;
				for (int i = 0; i < subChunkCount; ++i)
				{
					JObject j = new JObject()
					{
						["position"] = chunkPosition + positions[i],
						["size"] = (i < subChunkCount - 1 ? positions[i + 1] : chunkData.Length) - positions[i]
					};
					if (!string.IsNullOrEmpty(fileNames[i]))
					{
						j["fileName"] = fileNames[i];
					}
					if (creationDates[i] < DateTime.MaxValue)
					{
						j["creationDate"] = creationDates[i];
					}
					ja.Add(j);
				}
				return ja;
			}

			return null;
		}

		public JArray ExtractSubChunks(Stream chunkData, long chunkPosition)
		{
			byte[] bytes = new byte[chunkData.Length];
			chunkData.Position = 0L;
			return chunkData.Read(bytes, 0, bytes.Length) != chunkData.Length ? null : ExtractSubChunks(bytes, chunkPosition);
		}

		private static bool CompareSample(byte[] sample, byte[] buffer, long bufferPosition)
		{
			if (buffer.LongLength < sample.LongLength) { return false; }
			bool matched = true;
			for (long i = 0L; i < sample.LongLength && i + bufferPosition < buffer.LongLength; ++i)
			{
				matched &= buffer[i + bufferPosition] == sample[i];
				if (!matched) { break; }
			}

			return matched;
		}

		private static string ExtractChunkFileName(byte[] chunkData, long chunkStartPosition)
		{
			byte[] sample = new byte[] { (byte)'T', (byte)'O', (byte)'F', (byte)'N' };
			for (int i = 0; i < 1024; ++i)
			{
				if (i + chunkStartPosition >= chunkData.LongLength) { return null; }
				if (CompareSample(sample, chunkData, i + chunkStartPosition))
				{
					for (long j = 12; j < 60; ++j)
					{
						long pos = i + j + chunkStartPosition;
						if (pos >= chunkData.LongLength) { return null; }
						if (chunkData[pos] == '\0')
						{
							long fileNamePosition = chunkStartPosition + i + 11L;
							byte[] fileNameBytes = new byte[pos - fileNamePosition];
							Array.Copy(chunkData, fileNamePosition, fileNameBytes, 0, fileNameBytes.LongLength);
							return Encoding.ASCII.GetString(fileNameBytes);
						}
					}
				}
			}

			return null;
		}

		private static DateTime ExtractChunkCreationDate(byte[] chunkData, long chunkStartPosition)
		{
			byte[] sample = new byte[] { (byte)'T', (byte)'D', (byte)'E', (byte)'N' };
			for (long i = 0L; i < 1024L && i + chunkStartPosition < chunkData.LongLength; ++i)
			{
				if (CompareSample(sample, chunkData, i + chunkStartPosition))
				{
					byte byteTwo = (byte)'2';
					for (long j = 0L; j < 100L; ++j)
					{
						long pos = i + j + chunkStartPosition;
						if (pos >= chunkData.LongLength) { return DateTime.MaxValue; }
						if (chunkData[pos] == byteTwo)
						{
							if (pos + 20L >= chunkData.LongLength) { return DateTime.MaxValue; }
							byte[] dateBytes = new byte[20];
							Array.Copy(chunkData, pos, dateBytes, 0, dateBytes.LongLength - 1L);
							dateBytes[19] = (byte)'Z';
							string dateString = Encoding.ASCII.GetString(dateBytes);
							return DateTime.TryParse(dateString, null, System.Globalization.DateTimeStyles.AdjustToUniversal,
								out DateTime dateTime) ? dateTime : DateTime.MaxValue;
						}
					}
				}
			}

			return DateTime.MaxValue;
		}
	}
}
