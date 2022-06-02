using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorPickerV2.UI
{
    public partial class RoundedToggleButton : Panel
    {
        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                Invalidate();

                OnCheckedChanged?.Invoke(this, new EventArgs());
            }
        }

        private bool IsHovered
        {
            get => _isHovered;
            set
            {
                _isHovered = value;
                Invalidate();
            }
        }

        private bool IsDown
        {
            get => _isDown;
            set
            {
                _isDown = value;
                Invalidate();
            }
        }

        public event EventHandler OnCheckedChanged = null;

        private bool _checked, _isHovered, _isDown;
        private Color _fillColor = Color.FromArgb(51, 54, 60), _hoverColor = Color.FromArgb(55, 57, 63), _downColor = Color.FromArgb(57, 60, 66), _outlineColor = Color.FromArgb(55, 57, 63);
        private Color _buttonFillColor = Color.FromArgb(47, 51, 54), _buttonOutlineColor = Color.FromArgb(36, 40, 43);

        public RoundedToggleButton()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Checked = !Checked;
            base.OnMouseClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                IsDown = true;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                IsDown = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            IsHovered = true;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            IsHovered = false;
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            using (GraphicsPath path = UITools.RoundedRect(new Rectangle(0, 0, this.Width - 1, this.Height - 1), this.Height / 2))
            {
                using (Brush b = new SolidBrush(_isHovered ? (_isDown ? _downColor : _hoverColor) : _fillColor))
                    e.Graphics.FillPath(b, path);
                using (Pen p = new Pen(_outlineColor))
                    e.Graphics.DrawPath(p, path);
            }

            int holder = this.Height - 5;
            using (GraphicsPath path = UITools.RoundedRect(new Rectangle(_checked ? this.Width - holder - 2 : 2, 2, holder, holder), holder / 2))
            {
                using (Brush b = new SolidBrush(_buttonFillColor))
                    e.Graphics.FillPath(b, path);
                using (Pen p = new Pen(_buttonOutlineColor))
                    e.Graphics.DrawPath(p, path);
            }
        }
    }
}
