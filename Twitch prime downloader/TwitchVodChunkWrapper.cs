using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using TwitchApiLib;

namespace Twitch_prime_downloader
{
	internal class TwitchVodChunkWrapper : TwitchVodChunk
	{
		private readonly string _fileExtension;

		public TwitchVodChunkWrapper(TwitchVodChunk chunk) : base(chunk)
		{
			_fileExtension = Path.GetExtension(chunk.FileName);
		}

		public JArray ExtractSubChunks(byte[] chunkData, long chunkPosition)
		{
			List<long> positions = new List<long>();
			List<string> fileNames = new List<string>();
			List<DateTime> creationDates = new List<DateTime>();
			List<int> ids = new List<int>();

			if (_fileExtension == ".mp4")
			{
				byte[] sample = new byte[] { (byte)'e', (byte)'m', (byte)'s', (byte)'g' };
				for (long i = 0L; i < chunkData.LongLength; ++i)
				{
					if (i >= chunkData.LongLength - sample.LongLength) { break; }
					if (CompareSample(sample, chunkData, i))
					{
						long maxPosition = i + 1020L;
						positions.Add(i - 4);
						string fn = ExtractChunkFileName(chunkData, i, maxPosition);
						fileNames.Add(fn);
						creationDates.Add(ExtractChunkCreationDate(chunkData, i, maxPosition));
						ids.Add(ExtractChunkIdFromFileName(fn));
					}
				}
			}
			else
			{
				byte[] sample = new byte[] { (byte)'T', (byte)'R', (byte)'C', (byte)'K' };
				byte[] sample2 = new byte[] { (byte)'G', (byte)'@' };
				const int maxRangeBetweenSamples = 1024;
				for (long i = 0; i < chunkData.LongLength; ++i)
				{
					if (i >= chunkData.LongLength - sample.LongLength) { break; }
					if (CompareSample(sample, chunkData, i))
					{
						long sample2Position = -1L;
						for (int j = sample2.Length; j < maxRangeBetweenSamples; ++j)
						{
							long pos = i - j;
							if ((positions.Count > 0 && pos <= positions[positions.Count - 1]) || pos < 0L) { break; }
							if (CompareSample(sample2, chunkData, pos))
							{
								sample2Position = pos;
								positions.Add(pos);
								break;
							}
						}

						if (sample2Position >= 0L)
						{
							long maxPosition = sample2Position + 1024L;
							if (maxPosition > chunkData.LongLength) { maxPosition = chunkData.LongLength; }
							string fn = ExtractChunkFileName(chunkData, i, maxPosition);
							fileNames.Add(fn);
							creationDates.Add(ExtractChunkCreationDate(chunkData, i, maxPosition));
							ids.Add(ExtractChunkIdFromFileName(fn));
						}
					}
				}
			}

			if (positions.Count > 0)
			{
				JArray ja = new JArray();
				int subChunkCount = positions.Count;
				for (int i = 0; i < subChunkCount; ++i)
				{
					JObject j = new JObject();
					if (ids[i] >= 0) { j["id"] = ids[i]; }
					j["position"] = chunkPosition + positions[i];
					j["size"] = (i < subChunkCount - 1 ? positions[i + 1] : chunkData.Length) - positions[i];
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
			for (int i = 0; i < sample.Length; ++i)
			{
				long pos = bufferPosition + i;
				if (pos >= buffer.LongLength) { return false; }
				matched &= buffer[pos] == sample[i];
				if (!matched) { break; }
			}

			return matched;
		}

		private static string ExtractChunkFileName(byte[] chunkData, long arrayIndex, long maxArrayIndex)
		{
			byte[] sample = new byte[] { (byte)'T', (byte)'O', (byte)'F', (byte)'N' };
			for (int i = 0; i < 1024; ++i)
			{
				long pos = arrayIndex + i;
				if (pos >= maxArrayIndex || pos >= chunkData.LongLength - sample.LongLength) { return null; }
				if (CompareSample(sample, chunkData, pos))
				{
					for (long j = 12; j < 60; ++j)
					{
						long pos2 = pos + j;
						if (pos2 >= maxArrayIndex || pos2 >= chunkData.LongLength - sample.LongLength) { return null; }
						if (chunkData[pos2] == '\0')
						{
							long fileNamePosition = pos + 11L;
							byte[] fileNameBytes = new byte[pos2 - fileNamePosition];
							Array.Copy(chunkData, fileNamePosition, fileNameBytes, 0, fileNameBytes.LongLength);
							return Encoding.ASCII.GetString(fileNameBytes);
						}
					}
				}
			}

			return null;
		}

		private static DateTime ExtractChunkCreationDate(byte[] chunkData, long arrayIndex, long maxArrayIndex)
		{
			byte[] sample = new byte[] { (byte)'T', (byte)'D', (byte)'E', (byte)'N' };
			for (int i = 0; i < 1024; ++i)
			{
				long pos = arrayIndex + i;
				if (pos >= maxArrayIndex || pos >= chunkData.LongLength - sample.LongLength) { break; }
				if (CompareSample(sample, chunkData, pos))
				{
					byte byteTwo = (byte)'2';
					for (int j = 0; j < 100; ++j)
					{
						long pos2 = pos + j;
						if (pos2 >= maxArrayIndex || pos2 >= chunkData.LongLength - sample.LongLength) { return DateTime.MaxValue; }
						if (chunkData[pos2] == byteTwo)
						{
							if (pos2 + 20L >= chunkData.LongLength) { return DateTime.MaxValue; }
							byte[] dateBytes = new byte[20];
							Array.Copy(chunkData, pos2, dateBytes, 0, dateBytes.LongLength - 1L);
							dateBytes[19] = (byte)'Z';
							string dateString = Encoding.ASCII.GetString(dateBytes);
							bool isDateOk = DateTime.TryParse(dateString, null,
								System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime dateTime);

							// Searching for the 'ingest_r' value.
							byte[] ingestSample = new byte[] { (byte)'i', (byte)'n', (byte)'g', (byte)'e', (byte)'s', (byte)'t', (byte)'_', (byte)'r' };
							for (int n = 0; n < 1024; ++n)
							{
								long pos3 = pos2 + n + 21L;
								if (pos3 >= maxArrayIndex || pos3 >= chunkData.LongLength - ingestSample.LongLength)
								{
									return isDateOk ? dateTime : DateTime.MaxValue;
								}

								if (CompareSample(ingestSample, chunkData, pos3))
								{
									// The 'ingest_r' key is found.
									byte[] ingestValueBytes = new byte[20];
									for (int n2 = 0; n2 < 20; ++n2)
									{
										long pos4 = pos3 + n2 + ingestSample.LongLength + 2L;
										if (pos4 >= chunkData.LongLength)
										{
											return isDateOk ? dateTime : DateTime.MinValue;
										}

										if (chunkData[pos4] == (byte)',') { break; }
										ingestValueBytes[n2] = chunkData[pos4];
									}

									string ingestValueString = Encoding.ASCII.GetString(ingestValueBytes);
									return long.TryParse(ingestValueString, out long ingestValue) ?
										Utils.UnixMillisecondsToDateTime(ingestValue) :
										(isDateOk ? dateTime : DateTime.MaxValue);
								}
							}

							return isDateOk ? dateTime : DateTime.MaxValue;
						}
					}
				}
			}

			return DateTime.MaxValue;
		}

		private static int ExtractChunkIdFromFileName(string fileName)
		{
			try
			{
				if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrWhiteSpace(fileName))
				{
					Regex regex = new Regex(@"(\d+)(?:-.*)?.\w*$");
					Match match = regex.Match(fileName);
					if (match.Success && match.Groups.Count > 1 &&
						!string.IsNullOrEmpty(match.Groups[1].Value) &&
						!string.IsNullOrWhiteSpace(match.Groups[1].Value) &&
						int.TryParse(match.Groups[1].Value, out int id))
					{
						return id;
					}
				}
			}
#if DEBUG
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
#else
			catch { }
#endif
			return -1;
		}
	}
}
