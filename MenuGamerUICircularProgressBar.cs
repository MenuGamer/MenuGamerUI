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

namespace ProcessLister.MenuGamerUI
{
    public partial class MenuGamerUICircularProgressBar : Control
    {
        #region Enums

        public enum _ProgressShape
        {
            Round,
            Flat
        }

        public enum _TextMode
        {
            None,
            Value,
            Percentage,
            Custom
        }

        #endregion

        #region Private Variables

        private double _Value;
        private double _Maximum = 100;
        private int _LineWitdh = 1;
        private int _Decimals = 1;
        private int _DecimalsShow = 10;
        private float _BarWidth = 14f;

        private Color _ProgressColor1 = Color.Orange;
        private Color _ProgressColor2 = Color.Orange;
        private Color _LineColor = Color.Silver;
        private LinearGradientMode _GradientMode = LinearGradientMode.ForwardDiagonal;
        private _ProgressShape ProgressShapeVal;
        private _TextMode ProgressTextMode;

        #endregion

        #region Contructor

        public MenuGamerUICircularProgressBar()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            this.BackColor = SystemColors.Control;
            this.ForeColor = Color.DimGray;

            this.Size = new Size(130, 130);
            this.Font = new Font("Segoe UI", 15);
            this.MinimumSize = new Size(100, 100);
            this.DoubleBuffered = true;

            this.LineWidth = 1;
            this.LineColor = Color.DimGray;

            Value = 57;
            ProgressShape = _ProgressShape.Flat;
            TextMode = _TextMode.Percentage;
        }

        #endregion

        #region Public Custom Properties

        /// <summary>Determina el Valor del Progreso</summary>
        [Description("Value of progress"), Category("Behavior")]
        public double Value
        {
            get { return _Value; }
            set
            {
                if (value > _Maximum)
                    value = _Maximum;
                _Value = value;
                Invalidate();
            }
        }

        [Description("Maximum progress value"), Category("Behavior")]
        public double Maximum
        {
            get { return _Maximum; }
            set
            {
                if (value < 1)
                    value = 1;
                _Maximum = value;
                Invalidate();
            }
        }

        [Description("Color of layer 1"), Category("Appearance")]
        public Color BarColor1
        {
            get { return _ProgressColor1; }
            set
            {
                _ProgressColor1 = value;
                Invalidate();
            }
        }

        [Description("Color of layer 1"), Category("Appearance")]
        public Color BarColor2
        {
            get { return _ProgressColor2; }
            set
            {
                _ProgressColor2 = value;
                Invalidate();
            }
        }

        [Description("Circular progress bar width"), Category("Appearance")]
        public float BarWidth
        {
            get { return _BarWidth; }
            set
            {
                _BarWidth = value;
                Invalidate();
            }
        }

        [Description("Determines how color1 and color2 are mixed."), Category("Appearance")]
        public LinearGradientMode GradientMode
        {
            get { return _GradientMode; }
            set
            {
                _GradientMode = value;
                Invalidate();
            }
        }

        [Description("Color of the outline."), Category("Appearance")]
        public Color LineColor
        {
            get { return _LineColor; }
            set
            {
                _LineColor = value;
                Invalidate();
            }
        }

        [Description("Progresbar outline line width."), Category("Appearance")]
        public int LineWidth
        {
            get { return _LineWitdh; }
            set
            {
                _LineWitdh = value;
                Invalidate();
            }
        }

        [Description("Gets or sets the ammount of decimals is shown."), Category("Appearance")]
        public int Decimals
        {
            get { return _Decimals; }
            set
            {
                _Decimals = value;
                Invalidate();
            }
        }

        [Description("Gets or sets the value which deactivates decimals."), Category("Appearance")]
        public int DecimalDisplayValue
        {
            get { return _DecimalsShow; }
            set
            {
                _DecimalsShow = value;
                Invalidate();
            }
        }

        [Description("Gets or Sets the Shape of the progress bar terminals."), Category("Appearance")]
        public _ProgressShape ProgressShape
        {
            get { return ProgressShapeVal; }
            set
            {
                ProgressShapeVal = value;
                Invalidate();
            }
        }

        [Description("Gets or Sets the Mode as the Text is displayed inside the Progress bar."), Category("Behavior")]
        public _TextMode TextMode
        {
            get { return ProgressTextMode; }
            set
            {
                ProgressTextMode = value;
                Invalidate();
            }
        }

        [Description("Get the Text that is displayed inside the Control"), Category("Behavior")]
        public override string Text { get; set; }

        #endregion

        #region EventArgs

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetStandardSize();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetStandardSize();
        }

        protected override void OnPaintBackground(PaintEventArgs p)
        {
            base.OnPaintBackground(p);
        }

        #endregion

        #region Methods

        private void SetStandardSize()
        {
            int _Size = Math.Max(Width, Height);
            Size = new Size(_Size, _Size);
        }

        public void Increment(int Val)
        {
            this._Value += Val;
            Invalidate();
        }

        public void Decrement(int Val)
        {
            this._Value -= Val;
            Invalidate();
        }
        #endregion

        #region Events

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Bitmap bitmap = new Bitmap(this.Width, this.Height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    //graphics.Clear(Color.Transparent); //<-- this.BackColor, SystemColors.Control, Color.Transparent

                    PaintTransparentBackground(this, e);

                    using (Brush mBackColor = new SolidBrush(this.BackColor))
                    {
                        graphics.FillEllipse(mBackColor,
                                18, 18,
                                (this.Width - 0x30) + 12,
                                (this.Height - 0x30) + 12);
                    }

                    using (Pen pen2 = new Pen(LineColor, this.LineWidth))
                    {
                        graphics.DrawEllipse(pen2,
                            18, 18,
                          (this.Width - 0x30) + 12,
                          (this.Height - 0x30) + 12);
                    }

                    using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                        this._ProgressColor1, this._ProgressColor2, this.GradientMode))
                    {
                        using (Pen pen = new Pen(brush, this.BarWidth))
                        {
                            switch (this.ProgressShapeVal)
                            {
                                case _ProgressShape.Round:
                                    pen.StartCap = LineCap.Round;
                                    pen.EndCap = LineCap.Round;
                                    break;

                                case _ProgressShape.Flat:
                                    pen.StartCap = LineCap.Flat;
                                    pen.EndCap = LineCap.Flat;
                                    break;
                            }

                            graphics.DrawArc(pen,
                                0x12, 0x12,
                                (this.Width - 0x23) - 2,
                                (this.Height - 0x23) - 2,
                                -90,
                                (int)Math.Round(((360.0 / (this._Maximum)) * this._Value), _Decimals));
                        }
                    }

                    #region Dibuja el Texto de Progreso

                    switch (this.TextMode)
                    {
                        case _TextMode.None:
                            this.Text = string.Empty;
                            break;

                        case _TextMode.Value:
                            this.Text = _Value.ToString();
                            break;

                        case _TextMode.Percentage:
                            this.Text = Convert.ToString(Math.Round((100 / _Maximum) * _Value, _Value >= (double)_DecimalsShow ? 0 : _Decimals)) + "%";
                            break;

                        default:
                            break;
                    }

                    if (this.Text != string.Empty)
                    {
                        using (Brush FontColor = new SolidBrush(this.ForeColor))
                        {
                            int ShadowOffset = 2;
                            SizeF MS = graphics.MeasureString(this.Text, this.Font);
                            SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(100, this.ForeColor));

                            graphics.DrawString(this.Text, this.Font, shadowBrush,
                                Convert.ToInt32(Width / 2 - MS.Width / 2) + ShadowOffset,
                                Convert.ToInt32(Height / 2 - MS.Height / 2) + ShadowOffset
                            );

                            graphics.DrawString(this.Text, this.Font, FontColor,
                                Convert.ToInt32(Width / 2 - MS.Width / 2),
                                Convert.ToInt32(Height / 2 - MS.Height / 2));
                        }
                    }

                    #endregion

                    e.Graphics.DrawImage(bitmap, 0, 0);
                    graphics.Dispose();
                    bitmap.Dispose();
                }
            }
        }

        private static void PaintTransparentBackground(Control c, PaintEventArgs e)
        {
            if (c.Parent == null || !Application.RenderWithVisualStyles)
                return;

            ButtonRenderer.DrawParentBackground(e.Graphics, c.ClientRectangle, c);
        }

        /// <summary>Dibuja un Circulo Relleno de Color con los Bordes perfectos.</summary>
        /// <param name="g">'Canvas' del Objeto donde se va a dibujar</param>
        /// <param name="brush">Color y estilo del relleno</param>
        /// <param name="centerX">Centro del Circulo, en el eje X</param>
        /// <param name="centerY">Centro del Circulo, en el eje Y</param>
        /// <param name="radius">Radio del Circulo</param>
        private void FillCircle(Graphics g, Brush brush, float centerX, float centerY, float radius)
        {
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using (System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath())
            {
                g.FillEllipse(brush, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
            }
        }

        #endregion
    }
}
