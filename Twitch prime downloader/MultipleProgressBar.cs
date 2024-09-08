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
					int itemWidth = itemCount > 1 ? Width / itemCount : Width - 1;
					int iter = 0;
					foreach (MultipleProgressBarItem item in Items)
					{
						Rectangle rect;
						e.Graphics.SetClip(e.ClipRectangle);
						int itemPositionX = itemWidth * iter;
						int n = item.Value * itemWidth / item.MaxValue;
						if (n > 0)
						{
							rect = new Rectangle(itemPositionX, 0, n, rectangle.Height);
							Brush brush = new SolidBrush(item.BackgroundColor);
							e.Graphics.FillRectangle(brush, rect);
							brush.Dispose();
						}
						rect = new Rectangle(itemPositionX, 0, itemWidth, rectangle.Height);
						e.Graphics.DrawRectangle(Pens.Black, rect);
						e.Graphics.SetClip(rect);

						Brush brushText = new SolidBrush(ForeColor);
						string title = !string.IsNullOrEmpty(item.Title) && !string.IsNullOrWhiteSpace(item.Title) ?
							item.Title : $"Item №{iter}";
						SizeF size = e.Graphics.MeasureString(title, Font);
						float y = Height / 2.0f - size.Height / 2.0f;
						e.Graphics.DrawString(item.Title, Font, brushText, itemPositionX + 4.0f, y);
						brushText.Dispose();

						iter++;
					}
				}
			}
			else
			{
				e.Graphics.DrawRectangle(Pens.Black, rectangle);
			}

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
