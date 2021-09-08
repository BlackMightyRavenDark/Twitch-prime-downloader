using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Twitch_prime_downloader
{
    public class DoubleGauge : Control
    {
        private int _minValue1;
        private int _maxValue1 = 100;
        private int _value1 = 50;

        private int _minValue2;
        private int _maxValue2 = 100;
        private int _value2 = 76;

        private Color _colorBkg1 = Color.FromArgb(230, 230, 230);
        private Color _colorFore1 = Color.Blue;
        private Color _colorBkg2 = Color.FromArgb(230, 230, 230);
        private Color _colorFore2 = Color.Red;


        [DefaultValue(0)]
        public int MinValue1
        {
            get
            {
                return _minValue1;
            }
            set
            {
                SetMinValue1(value);
            }
        }

        [DefaultValue(100)]
        public int MaxValue1
        {
            get
            {
                return _maxValue1;
            }
            set
            {
                SetMaxValue1(value);
            }
        }

        [DefaultValue(50)]
        public int Value1
        {
            get
            {
                return _value1;
            }
            set
            {
                SetValue1(value);
            }
        }

        [DefaultValue(0)]
        public int MinValue2
        {
            get
            {
                return _minValue2;
            }
            set
            {
                SetMinValue2(value);
            }
        }

        [DefaultValue(100)]
        public int MaxValue2
        {
            get
            {
                return _maxValue2;
            }
            set
            {
                SetMaxValue2(value);
            }
        }

        [DefaultValue(50)]
        public int Value2
        {
            get
            {
                return _value2;
            }
            set
            {
                SetValue2(value);
            }
        }

        [DefaultValue(typeof(Color), "0x0000FF")]
        public Color ColorForeground1
        {
            get
            {
                return _colorFore1;
            }
            set
            {
                SetForegroundColor1(value);
            }
        }

        [DefaultValue(typeof(Color), "0xFF0000")]
        public Color ColorForeground2
        {
            get
            {
                return _colorFore2;
            }
            set
            {
                SetForegroundColor2(value);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DrawAsBars(e);
        }

        private void DrawAsBars(PaintEventArgs e)
        {
            int halfHeight = Height / 2;
            Brush brush1 = new SolidBrush(_colorBkg1);
            e.Graphics.FillRectangle(brush1, ClientRectangle);
            brush1.Dispose();
            if (MaxValue1 > 0)
            {
                brush1 = new SolidBrush(ColorForeground1);
                int x = Value1 * Width / MaxValue1;
                e.Graphics.FillRectangle(brush1, new RectangleF(0, 0, x, halfHeight));
                brush1.Dispose();
            }

            /*brush1 = new SolidBrush(_colorBkg2);
            //e.Graphics.FillRectangle(brush1, ClientRectangle);
            brush1.Dispose();-**/
            if (MaxValue2 > 0)
            {
                brush1 = new SolidBrush(ColorForeground2);
                int x = Value2 * Width / MaxValue2;
                e.Graphics.FillRectangle(brush1, new RectangleF(0, halfHeight, x, Height));
                brush1.Dispose();
            }
        }

        private void SetMinValue1(int newValue)
        {
            _minValue1 = newValue;
            Refresh();
        }

        private void SetMaxValue1(int newValue)
        {
            if (_maxValue1 != newValue)
            {
                _maxValue1 = newValue;
                Refresh();
            }
        }

        private void SetValue1(int newValue)
        {
            if (_value1 != newValue)
            {
                _value1 = newValue;
                Refresh();
            }
        }

        private void SetMinValue2(int newValue)
        {
            _minValue2 = newValue;
            Refresh();
        }

        private void SetMaxValue2(int newValue)
        {
            if (_maxValue2 != newValue)
            {
                _maxValue2 = newValue;
                Refresh();
            }
        }

        private void SetValue2(int newValue)
        {
            if (_value2 != newValue)
            {
                _value2 = newValue;
                Refresh();
            }
        }
        private void SetForegroundColor1(Color newColor)
        {
            if (_colorFore1 != newColor)
            {
                _colorFore1 = newColor;
                Refresh();
            }
        }

        private void SetForegroundColor2(Color newColor)
        {
            if (_colorFore2 != newColor)
            {
                _colorFore2 = newColor;
                Refresh();
            }
        }
    }
}
