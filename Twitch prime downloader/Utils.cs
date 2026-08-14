using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MultiThreadedDownloaderLib;
using TwitchApiLib;

namespace Twitch_prime_downloader
{
	public static class Utils
	{
		public static List<VodFrame> vodFrames = new List<VodFrame>();
		public static List<DownloadFrame> downloadFrames = new List<DownloadFrame>();

		public static readonly Configurator config = new Configurator();

		public const string FILENAME_FORMAT_DEFAULT =
			"<channel_name> [<year>-<month>-<day> <hour>-<minute>-<second><GMT>] <video_title>";
		internal static readonly TwitchApplication defaultApplication = new TwitchApplication(
			"Test application", "No description",
			"gs7pui3law5lsi69yzi9qzyaqvlcsy", // Client ID
			"srr2yi260t15ir6w0wq5blir22i9pq"  // Client secret key
		);

		public enum DownloadMode { SingleFile, Chunked };

		public static string ExtractVodIdFromUrl(string url)
		{
			try
			{
				Uri uri = new Uri(url);
				string host = !string.IsNullOrWhiteSpace(uri.Host) ? uri.Host.ToLower() : null;
				if (string.IsNullOrWhiteSpace(host) || !host.Contains("twitch.tv"))
				{
					return null;
				}

				if (!string.IsNullOrWhiteSpace(uri.LocalPath) && uri.LocalPath.ToLower().StartsWith("/videos/"))
				{
					string[] strings = uri.LocalPath.Split('/');
					string t = strings[strings.Length - 1];
					int n = t.IndexOf("&");
					return n < 0 ? t : t.Substring(0, n);
				}
			}
			catch { }

			return null;
		}

		public static string GetNumberedDirectoryName(string dirPathOrig, out string errorMessage)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(dirPathOrig))
				{
					errorMessage = "An empty string passed";
					return null;
				}

				errorMessage = null;
				if (dirPathOrig.EndsWith("\\"))
				{
					if (dirPathOrig.Length == 3)
					{
						errorMessage = "Root directories is not supported";
						return null;
					}

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

					return dirPathNew;
				}

				return dirPathOrig;
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
#if DEBUG
				System.Diagnostics.Debug.WriteLine(errorMessage);
#endif
				return null;
			}
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

		public static Color GetColorFromRGB(int rgbColor)
		{
			byte r = (byte)(rgbColor >> 16 & 0xFF);
			byte g = (byte)(rgbColor >>  8 & 0xFF);
			byte b = (byte)(rgbColor       & 0xFF);
			return Color.FromArgb(r, b, g);
		}

		public static Bitmap GenerateErrorImage()
		{
			try
			{
				Bitmap bmp = new Bitmap(320, 180);
				using (Graphics g = Graphics.FromImage(bmp))
				{
					g.FillRectangle(Brushes.Black, new RectangleF(0, 0, bmp.Width, bmp.Height));
					using (Font font = new Font("Arial", 12))
					{
						Point center = new Point(bmp.Width / 2, bmp.Height / 2);
						Random random = new Random();
						int n = random.Next(10);
						if (n < 5)
						{
							string t = "matrix has you";
							SizeF sz = g.MeasureString(t, font);
							float yDraw = center.Y - sz.Height / 2.0f;
							g.DrawString(t, font, Brushes.Lime, center.X - sz.Width / 2.0f, yDraw);
							t = "fuck";
							sz = g.MeasureString(t, font);
							yDraw -= sz.Height;
							g.DrawString(t, font, Brushes.Lime, center.X - sz.Width / 2.0f, yDraw);
							t = "there is no image";
							sz = g.MeasureString(t, font);
							yDraw -= sz.Height;
							g.DrawString(t, font, Brushes.Lime, center.X - sz.Width / 2.0f, yDraw);
							t = "sorry :'(";
							sz = g.MeasureString(t, font);
							g.DrawString(t, font, Brushes.Lime, center.X - sz.Width / 2.0f, center.Y + sz.Height);
						}
						else
						{
							string t = "картинки нет, но вы там держитесь";
							SizeF sz = g.MeasureString(t, font);
							float x = center.X - sz.Width / 2.0f;
							g.DrawString(t, font, Brushes.Lime, x, center.Y - sz.Height);

							t = "хорошего настроения и здоровья";
							sz = g.MeasureString(t, font);
							x = bmp.Width / 2.0f - sz.Width / 2.0f;
							g.DrawString(t, font, Brushes.Lime, x, center.Y);
						}
					}
				}

				return bmp;
			}
#if DEBUG
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
#else
			catch
			{
#endif
				return null;
			}
		}

		internal static IEnumerable<MultipleProgressBarItem> GetMultipleProgressBarItems(IEnumerable<DownloadProgressItem> items)
		{
			foreach (DownloadProgressItem item in items)
			{
				double percent = 100.0 / item.ChunkSize * item.DownloadedSize;
				string percentFormatted = string.Format("{0:F2}", percent);

				string itemText;
				switch (item.State)
				{
					case DownloadItemState.Preparing:
						itemText = $"{item.ChunkDownloader.ChunkWrapper.FileName}: Preparing...";
						break;

					case DownloadItemState.Connecting:
						itemText = $"{item.ChunkDownloader.ChunkWrapper.FileName}: Connecting...";
						break;

					case DownloadItemState.Downloading:
					case DownloadItemState.Finished:
					case DownloadItemState.Errored:
						itemText = $"{item.ChunkDownloader.ChunkWrapper.FileName}: " +
							$"{FormatSize(item.DownloadedSize)} / {FormatSize(item.ChunkSize)} ({percentFormatted}%)";
						break;

					default:
						itemText = null;
						break;
				}

				int percentRounded = (int)Math.Round(percent, 3);
				MultipleProgressBarItem mpi = new MultipleProgressBarItem(
					0, 100, percentRounded, itemText, Color.Lime);
				yield return mpi;
			}
		}

		public static bool SetClipboardText(string text)
		{
			if (string.IsNullOrEmpty(text)) { return false; }

			while (true)
			{
				try
				{
					Clipboard.SetText(text);
					return true;
				}
#if DEBUG
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex.Message);
				}
#else
				catch { }
#endif
			}
		}

		public static string FormatFileName(string fmt, TwitchVod twitchVod)
		{
			DateTime creationDate = config.UseGmtVodDates ?
				twitchVod.CreationDate : twitchVod.CreationDate.ToLocalTime();
			return fmt.Replace("<year>", creationDate.Year.ToString())
				.Replace("<month>", creationDate.Month.ToString().PadLeft(2, '0'))
				.Replace("<day>", creationDate.Day.ToString().PadLeft(2, '0'))
				.Replace("<hour>", creationDate.Hour.ToString().PadLeft(2, '0'))
				.Replace("<minute>", creationDate.Minute.ToString().PadLeft(2, '0'))
				.Replace("<second>", creationDate.Second.ToString().PadLeft(2, '0'))
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

		public static Image TryLoadImageFromStream(Stream stream, out string errorMessage)
		{
			if (stream == null)
			{
				errorMessage = "Stream is null";
				return null;
			}
			else if (stream.Length == 0L)
			{
				errorMessage = "Stream is empty";
				return null;
			}

			try
			{
				errorMessage = null;
				return Image.FromStream(stream);
			}
			catch (Exception ex)
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
				errorMessage = ex.Message;
				return null;
			}
		}

		internal static bool IsContinuousSequence(List<DownloadProgressItem> items)
		{
			for (int i = 0; i < items.Count; ++i)
			{
				if (items[i].TaskId != i) { return false; }
			}

			return true;
		}

		public static bool SaveStreamToFile(Stream stream, string filePath)
		{
			try
			{
				if (File.Exists(filePath)) { File.Delete(filePath); }
				return stream.SaveToFile(filePath);
			}
#if DEBUG
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
#else
			catch { }
#endif
			return false;
		}

		public static Image TryLoadImageFromStream(Stream stream)
		{
			return TryLoadImageFromStream(stream, out _);
		}

		public static int DownloadData(string url, out Stream stream, FileDownloader downloader = null)
		{
			try
			{
				FileDownloader d = downloader ?? new FileDownloader();
				d.Url = url;
				stream = new MemoryStream();
				return d.Download(stream);
			}
			catch (Exception ex)
			{
#if DEBUG
				System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
				stream = null;
				return ex.HResult;
			}
		}

		public static TwitchApplication MakeTwitchApplication()
		{
			return new TwitchApplication(
				config.ApiApplicationTitle,
				config.ApiApplicationDescription,
				config.ApiApplicationClientId,
				config.ApiApplicationClientSecretKey);
		}

		public static bool IsTwitchApplicationValid(out string errorMessage)
		{
			if (string.IsNullOrWhiteSpace(config.ApiApplicationClientId))
			{
				errorMessage = "Не указан ID приложения Twitch!";
				return false;
			}
			else if (string.IsNullOrWhiteSpace(config.ApiApplicationClientSecretKey))
			{
				errorMessage = "Не указан секретный ключ приложения Twitch!";
				return false;
			}

			errorMessage = null;
			return true;
		}

		public static DateTime UnixMillisecondsToDateTime(long unixMilliseconds)
		{
			DateTime minUnixDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return minUnixDate.AddMilliseconds(unixMilliseconds);
		}
	}
}
