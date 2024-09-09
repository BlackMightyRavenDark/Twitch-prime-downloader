using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Twitch_prime_downloader
{
	public class MultipleProgressBar : Control
	{
		[DefaultValue(null)]
		public IEnumerable<MultipleProgressBarItem> Items { get; private set; }

		public MultipleProgressBar()
		{
			DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Rectangle rectangle = e.ClipRectangle.Deflate(1, 1);
			Brush brushBkg = new SolidBrush(BackColor);
			e.Graphics.FillRectangle(brushBkg, rectangle);
			if (Items != null)
			{
				int itemCount = Items.Count();
				if (itemCount > 0)
				{
					float itemWidth = (float)Width / itemCount;
					int iter = 0;
					foreach (MultipleProgressBarItem item in Items)
					{
						Rectangle clipRect = e.ClipRectangle.Deflate(1, 1);
						e.Graphics.SetClip(e.ClipRectangle);
						Rectangle rect;
						float itemPositionX = (float)Math.Floor(itemWidth * iter);
						float n = (float)Math.Floor(item.Value * itemWidth / item.MaxValue);
						if (n > 0.0f)
						{
							rect = new Rectangle((int)itemPositionX, 0, (int)n, rectangle.Height);
							Brush brush = new SolidBrush(item.BackgroundColor);
							e.Graphics.FillRectangle(brush, rect);
							brush.Dispose();
						}
						rect = new Rectangle((int)itemPositionX, 0, (int)itemWidth, rectangle.Height);
						e.Graphics.SetClip(rect);
						e.Graphics.DrawLine(Pens.Black, rect.Left, rect.Top, rect.Left, rect.Bottom);

						if (!string.IsNullOrEmpty(item.Title) && !string.IsNullOrWhiteSpace(item.Title))
						{
							SizeF size = e.Graphics.MeasureString(item.Title, Font);
							float y = Height / 2.0f - size.Height / 2.0f;
							Brush brushText = new SolidBrush(ForeColor);
							e.Graphics.DrawString(item.Title, Font, brushText, itemPositionX + 4.0f, y);
							brushText.Dispose();
						}

						iter++;
					}
				}
			}

			e.Graphics.SetClip(e.ClipRectangle);
			e.Graphics.DrawRectangle(Pens.Black, rectangle);
			brushBkg.Dispose();
		}

		public void SetItems(IEnumerable<MultipleProgressBarItem> items)
		{
			Items = items;
			Refresh();
		}

		public void SetItem(int min, int max, int value, string title, Color backgroundColor)
		{
			MultipleProgressBarItem item = new MultipleProgressBarItem(min, max, value, title, backgroundColor);
			SetItems(new[] { item });
		}

		public void ClearItems()
		{
			SetItems(null);
		}
	}

	public class MultipleProgressBarItem
	{
		public int MinValue { get; }
		public int MaxValue { get; }
		public int Value { get; }
		public string Title { get; }
		public Color BackgroundColor { get; }

		public MultipleProgressBarItem(int minValue, int maxValue, int value,
			string title, Color backgroundColor)
		{
			MinValue = minValue;
			MaxValue = maxValue;
			Value = value;
			Title = title;
			BackgroundColor = backgroundColor;
		}
	}
}
