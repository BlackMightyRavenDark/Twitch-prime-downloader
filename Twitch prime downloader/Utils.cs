using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TwitchApiLib;

namespace Twitch_prime_downloader
{
	public static class Utils
	{
		public static List<FrameStream> framesStream = new List<FrameStream>();
		public static List<FrameDownloading> framesDownloading = new List<FrameDownloading>();

		public static readonly Configurator config = new Configurator();

		public const string FILENAME_FORMAT_DEFAULT =
			"<channel_name> [<year>-<month>-<day> <hour>-<minute>-<second><GMT>] <video_title>";

		public enum DownloadMode { WholeFile, Chunked };

		public static string ExtractVodIdFromUrl(string url)
		{
			try
			{
				Uri uri = new Uri(url);
				string host = !string.IsNullOrEmpty(uri.Host) ? uri.Host.ToLower() : null;
				if (string.IsNullOrEmpty(host) || !host.Contains("twitch.tv"))
				{
					return null;
				}
				if (!string.IsNullOrEmpty(uri.LocalPath) && uri.LocalPath.ToLower().StartsWith("/videos/"))
				{
					string[] strings = uri.LocalPath.Split('/');
					string t = strings[strings.Length - 1];
					int n = t.IndexOf("&");
					return n < 0 ? t : t.Substring(0, n);
				}
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static string GetNumberedDirectoryName(string dirPathOrig)
		{
			if (dirPathOrig.EndsWith("\\"))
			{
				dirPathOrig = dirPathOrig.Remove(dirPathOrig.Length - 1, 1);
			}
			if (Directory.Exists(dirPathOrig))
			{
				int n = 2;
				string dirPathNew;
				do
				{
					dirPathNew = $"{dirPathOrig}_{n++}";
				}
				while (Directory.Exists(dirPathNew));
				return dirPathNew + "\\";
			}
			return dirPathOrig.EndsWith("\\") ? dirPathOrig : dirPathOrig + "\\";
		}

		public static string FormatSize(long n)
		{
			const int KB = 1000;
			const int MB = 1000000;
			const int GB = 1000000000;
			const long TB = 1000000000000;
			long b = n % KB;
			long kb = (n % MB) / KB;
			long mb = (n % GB) / MB;
			long gb = (n % TB) / GB;

			if (n >= 0 && n < KB)
				return string.Format("{0} b", b);
			if (n >= KB && n < MB)
				return string.Format("{0},{1:D3} KB", kb, b);
			if (n >= MB && n < GB)
				return string.Format("{0},{1:D3},{2:D3} MB", mb, kb, b);
			if (n >= GB && n < TB)
				return string.Format("{0},{1:D3},{2:D3},{3:D3} GB", gb, mb, kb, b);

			return string.Format("{0} {1:D3} {2:D3} {3:D3} bytes", gb, mb, kb, b);
		}

		public static Color IntToColor(int color)
		{
			//TODO: Rewrite this shit
			byte[] values = BitConverter.GetBytes(color);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(values);
			}
			return Color.FromArgb(values[0], values[1], values[2]);
		}

		public static Bitmap GenerateErrorImage()
		{
			Bitmap bmp = new Bitmap(320, 180);
			Graphics g = Graphics.FromImage(bmp);
			g.FillRectangle(Brushes.Black, new RectangleF(0, 0, bmp.Width, bmp.Height));
			Font fnt = new Font("Arial", 12);
			Point center = new Point(bmp.Width / 2, bmp.Height / 2);
			Random random = new Random(Environment.TickCount);
			int n = random.Next(10);
			if (n < 5)
			{
				string t = "matrix has you";
				SizeF sz = g.MeasureString(t, fnt);
				float yDraw = center.Y - sz.Height / 2.0f;
				g.DrawString(t, fnt, Brushes.Lime, center.X - sz.Width / 2.0f, yDraw);
				t = "fuck";
				sz = g.MeasureString(t, fnt);
				yDraw -= sz.Height;
				g.DrawString(t, fnt, Brushes.Lime, center.X - sz.Width / 2.0f, yDraw);
				t = "there is no image";
				sz = g.MeasureString(t, fnt);
				yDraw -= sz.Height;
				g.DrawString(t, fnt, Brushes.Lime, center.X - sz.Width / 2.0f, yDraw);
				t = "sorry :'(";
				sz = g.MeasureString(t, fnt);
				g.DrawString(t, fnt, Brushes.Lime, center.X - sz.Width / 2.0f, center.Y + sz.Height);
			}
			else
			{
				string t = "картинки нет, но вы там держитесь";
				SizeF sz = g.MeasureString(t, fnt);
				float x = center.X - sz.Width / 2.0f;
				g.DrawString(t, fnt, Brushes.Lime, x, center.Y - sz.Height);

				t = "хорошего настроения и здоровья";
				sz = g.MeasureString(t, fnt);
				x = bmp.Width / 2.0f - sz.Width / 2.0f;
				g.DrawString(t, fnt, Brushes.Lime, x, center.Y);
			}
			fnt.Dispose();

			return bmp;
		}

		public static bool SetClipboardText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			bool suc;
			do
			{
				try
				{
					Clipboard.SetText(text);
					suc = true;
				}
				catch
				{
					suc = false;
				}
			} while (!suc);
			return true;
		}

		public static string LeadZero(int n)
		{
			return n < 10 ? $"0{n}" : n.ToString();
		}

		public static string FormatFileName(string fmt, TwitchVod twitchVod)
		{
			DateTime creationDate = config.UseGmtVodDates ?
				twitchVod.CreationDate : twitchVod.CreationDate.ToLocal();
			return fmt.Replace("<year>", LeadZero(creationDate.Year))
				.Replace("<month>", LeadZero(creationDate.Month))
				.Replace("<day>", LeadZero(creationDate.Day))
				.Replace("<hour>", LeadZero(creationDate.Hour))
				.Replace("<minute>", LeadZero(creationDate.Minute))
				.Replace("<second>", LeadZero(creationDate.Second))
				.Replace("<GMT>", config.UseGmtVodDates ? " GMT" : string.Empty)
				.Replace("<video_title>", twitchVod.Title)
				.Replace("<channel_name>", twitchVod.User.DisplayName);
		}

		public static string FixFileName(string fn)
		{
			return fn.Replace("\\", "\u29F9").Replace("|", "\u2758").Replace("/", "\u2044")
				.Replace("?", "\u2753").Replace(":", "\uFE55").Replace("<", "\u227A").Replace(">", "\u227B")
				.Replace("\"", "\u201C").Replace("*", "\uFE61").Replace("^", "\u2303").Replace("\n", string.Empty);
		}

		public static Image TryLoadImageFromStream(Stream stream, out string errorText)
		{
			if (stream == null)
			{
				errorText = "Stream is null";
				return null;
			}
			else if (stream.Length == 0L)
			{
				errorText = "Stream is empty";
				return null;
			}

			try
			{
				errorText = null;
				return Image.FromStream(stream);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				errorText = ex.Message;
				return null;
			}
		}

		public static Image TryLoadImageFromStream(Stream stream)
		{
			return TryLoadImageFromStream(stream, out _);
		}
	}
}
