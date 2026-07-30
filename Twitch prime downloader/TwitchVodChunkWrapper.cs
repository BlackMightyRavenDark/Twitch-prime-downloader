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
		private delegate void SubChunkFoundDelegate(long subChunkPosition, long samplePosition);

		internal TwitchVodChunkWrapper(TwitchVodChunk chunk) : base(chunk)
		{
			_fileExtension = Path.GetExtension(chunk.FileName);
		}

		internal JArray ExtractSubChunks(byte[] chunkData, long chunkPosition)
		{
			return ExtractSubChunks(chunkData, chunkPosition, _fileExtension);
		}

		internal JArray ExtractSubChunks(Stream chunkData, long chunkPosition)
		{
			return ExtractSubChunks(chunkData, chunkPosition, _fileExtension);
		}

		internal static JArray ExtractSubChunks(byte[] chunkData, long chunkPositionInStream, string fileExtension)
		{
			List<long> positions = new List<long>();
			List<string> fileNames = new List<string>();
			List<DateTime> creationDates = new List<DateTime>();
			List<int> ids = new List<int>();

			bool isMp4 = string.Equals(fileExtension, ".mp4", StringComparison.OrdinalIgnoreCase);
			FindSubChunkPositions(chunkData, positions.Count > 0 ? positions[positions.Count - 1] : 0L, isMp4,
				(subChunkPosition, samplePosition) =>
				{
					positions.Add(subChunkPosition);
					long maxPosition = subChunkPosition + 1024L;
					string fn = ExtractChunkFileName(chunkData, samplePosition, maxPosition);
					fileNames.Add(fn);
					creationDates.Add(ExtractChunkCreationDate(chunkData, samplePosition, maxPosition));
					ids.Add(ExtractChunkId(chunkData, samplePosition, fn));
				}
			);

			if (positions.Count > 0)
			{
				JArray ja = new JArray();
				int subChunkCount = positions.Count;
				for (int i = 0; i < subChunkCount; ++i)
				{
					JObject j = new JObject();
					if (ids[i] >= 0) { j["id"] = ids[i]; }
					j["position"] = chunkPositionInStream + positions[i];
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

		internal static JArray ExtractSubChunks(Stream chunkData, long chunkPosition, string fileExtension)
		{
			byte[] bytes = new byte[chunkData.Length];
			chunkData.Position = 0L;
			return chunkData.Read(bytes, 0, bytes.Length) != chunkData.Length ? null : ExtractSubChunks(bytes, chunkPosition, fileExtension);
		}

		private static void FindSubChunkPositions(byte[] chunkData, long lastSubChunkPosition,
			bool isMp4File, SubChunkFoundDelegate subChunkFound)
		{
			byte[] sample = isMp4File ? new byte[] { (byte)'e', (byte)'m', (byte)'s', (byte)'g' } :
				new byte[] { (byte)'T', (byte)'R', (byte)'C', (byte)'K' };
			for (long i = 0L; i < chunkData.LongLength - 4L; ++i)
			{
				if (CompareSample(sample, chunkData, i))
				{
					if (isMp4File) // probably MP4 chunk.
					{
						subChunkFound.Invoke(i - 4L, i);
					}
					else // probably TS chunk.
					{
						byte[] sample2 = new byte[] { (byte)'G', (byte)'@' };
						const int maxRangeBetweenSamples = 1024;
						for (int j = 0; j < maxRangeBetweenSamples; ++j)
						{
							long pos = i - j;
							if ((lastSubChunkPosition > 0L && pos <= lastSubChunkPosition) ||
								(lastSubChunkPosition == 0L && pos < lastSubChunkPosition))
							{
								// Impossible situation. What to do?
								break;
							}

							if (CompareSample(sample2, chunkData, pos))
							{
								subChunkFound.Invoke(pos, i);
								break;
							}
						}
					}
				}
			}
		}

		private static bool CompareSample(byte[] sample, byte[] buffer, long arrayIndex)
		{
			if (buffer.LongLength < sample.LongLength) { return false; }
			bool matched = true;
			for (int i = 0; i < sample.Length; ++i)
			{
				long pos = arrayIndex + i;
				if (pos >= buffer.LongLength) { return false; }
				matched &= buffer[pos] == sample[i];
				if (!matched) { break; }
			}

			return matched;
		}

		private static int ExtractChunkId(byte[] chunkData, long arrayIndex, string fileName = null)
		{
			byte[] sample = new byte[] { (byte)'T', (byte)'R', (byte)'C', (byte)'K' };
			for (int i = 0; i < 1024; ++i)
			{
				long pos = arrayIndex + i;
				if (CompareSample(sample, chunkData, pos))
				{
					if (chunkData.LongLength > pos + 11L)
					{
						const int idLength = 8;
						byte[] idBytes = new byte[idLength];
						for (int j = 0; j < idLength; ++j)
						{
							long pos2 = j + pos + 11L;
							if (pos2 >= chunkData.LongLength) { return ExtractChunkIdFromFileName(fileName); }
							if (chunkData[pos2] == '\0')
							{
								string idString = Encoding.ASCII.GetString(idBytes);
								if (!int.TryParse(idString, out int id)) { break; }
								return id;
							}
							else
							{
								idBytes[j] = chunkData[pos2];
							}
						}
					}

					break;
				}
			}

			if (string.IsNullOrEmpty(fileName) || string.IsNullOrWhiteSpace(fileName))
			{
				fileName = ExtractChunkFileName(chunkData, arrayIndex, arrayIndex + 1000L);
			}

			return ExtractChunkIdFromFileName(fileName);
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
									if (long.TryParse(ingestValueString, out long ingestValue) && ingestValue > 1000L)
									{
										return Utils.UnixMillisecondsToDateTime(ingestValue);
									}

									// Sometimes, the 'ingest_r' key contains an invalid value. So, continue searching the 'transc_r' key.
									byte[] transcodeSample = new byte[] { (byte)'t', (byte)'r', (byte)'a', (byte)'n', (byte)'s', (byte)'c', (byte)'_', (byte)'r' };
									for (int n3 = 0; n3 < 60; ++n3)
									{
										long pos4 = n3 + pos3 + ingestSample.LongLength + 3L;
										if (pos4 >= chunkData.LongLength)
										{
											return isDateOk ? dateTime : DateTime.MinValue;
										}

										if (CompareSample(transcodeSample, chunkData, pos4))
										{
											byte[] transcodeValueBytes = new byte[20];
											for (int n4 = 0; n4 < 20; ++n4)
											{
												long pos5 = n4 + pos4 + transcodeSample.LongLength + 2L;
												if (pos5 >= chunkData.LongLength)
												{
													return isDateOk ? dateTime : DateTime.MinValue;
												}

												if (chunkData[pos5] == (byte)',') { break; }
												transcodeValueBytes[n4] = chunkData[pos5];
											}

											string transcodeValueString = Encoding.ASCII.GetString(transcodeValueBytes);
											return long.TryParse(transcodeValueString, out long transcodeValue) ?
												Utils.UnixMillisecondsToDateTime(transcodeValue) :
												(isDateOk ? dateTime : DateTime.MinValue);
										}
									}

									break;
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
