using System;
using System.Collections.Generic;
using System.Linq;
using TwitchApiLib;
using static TwitchApiLib.TwitchVodChunk;

namespace Twitch_prime_downloader
{
	public class TwitchVodMutedSegments
	{
		public List<string> SegmentList { get; private set; } = new List<string>();
		public TimeSpan TotalDuration { get; private set; } = TimeSpan.Zero;

		public List<List<TwitchVodChunk>> Segments = new List<List<TwitchVodChunk>>();

		public void CalculateTotalDuration()
		{
			TotalDuration = TimeSpan.Zero;

			foreach (List<TwitchVodChunk> list in Segments)
			{
				foreach (TwitchVodChunk chunk in list)
				{
					TotalDuration = TotalDuration.Add(TimeSpan.FromSeconds(chunk.Duration));
				}
			}
		}

		public void BuildSegmentList()
		{
			SegmentList.Clear();
			foreach (List<TwitchVodChunk> list in Segments)
			{
				string t = SegmentToString(list);
				SegmentList.Add(t);
			}
		}

		private string SegmentToString(List<TwitchVodChunk> segment)
		{
			double dur = segment.Select(x => x.Duration).Sum();
			TimeSpan start = TimeSpan.FromSeconds(segment[0].Offset);
			TimeSpan end = TimeSpan.FromSeconds(segment[0].Offset + dur);
			return $"{start:hh':'mm':'ss} - {end:hh':'mm':'ss}";
		}

		public static TwitchVodMutedSegments ParseMutedSegments(List<TwitchVodChunk> chunkList)
		{
			TwitchVodMutedSegments result = new TwitchVodMutedSegments();
			List<TwitchVodChunk> segmentList = null;
			for (int i = 0; i < chunkList.Count; ++i)
			{
				if (chunkList[i].GetState() != TwitchVodChunkState.NotMuted)
				{
					if (segmentList == null)
					{
						segmentList = new List<TwitchVodChunk>();
					}

					TwitchVodChunk chunk = new TwitchVodChunk(
						chunkList[i].FileName, chunkList[i].Offset, chunkList[i].Duration);
					segmentList.Add(chunk);
				}
				else if (segmentList != null)
				{
					result.Segments.Add(segmentList);
					segmentList = null;
				}
			}

			if (segmentList != null)
			{
				result.Segments.Add(segmentList);
			}

			return result;
		}

		public void Clear()
		{
			Segments.Clear();
			SegmentList.Clear();
			TotalDuration = TimeSpan.Zero;
		}

		public override string ToString()
		{
			string t = string.Empty;
			if (SegmentList.Count > 0)
			{
				foreach (string segment in SegmentList)
				{
					t += segment + Environment.NewLine;
				}
			}
			return t;
		}
	}
}
