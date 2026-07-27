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
			List<int> ids = new List<int>();

			if (_fileExtension == ".mp4")
			{
				byte[] sample = new byte[] { (byte)'e', (byte)'m', (byte)'s', (byte)'g' };
				for (long i = 0L; i < chunkData.LongLength - sample.LongLength; ++i)
				{
					if (CompareSample(sample, chunkData, i))
					{
						positions.Add(i - 4);
						string fn = ExtractChunkFileName(chunkData, i);
						fileNames.Add(fn);
						creationDates.Add(ExtractChunkCreationDate(chunkData, i));
						ids.Add(ExtractChunkIdFromFileName(fn));
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

						string fn = ExtractChunkFileName(chunkData, i);
						fileNames.Add(fn);
						creationDates.Add(ExtractChunkCreationDate(chunkData, i));
						ids.Add(ExtractChunkIdFromFileName(fn));
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
							bool isDateOk = DateTime.TryParse(dateString, null,
								System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime dateTime);

							// Searching for the 'ingest_r' value.
							byte[] ingestSample = new byte[] { (byte)'i', (byte)'n', (byte)'g', (byte)'e', (byte)'s', (byte)'t', (byte)'_', (byte)'r' };
							for (long n = 0L; n < 300L; ++n)
							{
								long pos2 = pos + n + 21L;
								if (pos2 >= chunkData.LongLength)
								{
									return isDateOk ? dateTime : DateTime.MaxValue;
								}

								if (CompareSample(ingestSample, chunkData, pos2))
								{
									// The 'ingest_r' key is found.
									byte[] ingestValueBytes = new byte[20];
									for (long n2 = 0L; n2 < 20L; ++n2)
									{
										long pos3 = pos2 + n2 + ingestSample.LongLength + 2L;
										if (pos3 >= chunkData.LongLength)
										{
											return isDateOk ? dateTime : DateTime.MinValue;
										}

										if (chunkData[pos3] == (byte)',') { break; }
										ingestValueBytes[n2] = chunkData[pos3];
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
