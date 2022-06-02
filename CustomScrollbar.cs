using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Drawing2D;

namespace ColorPickerV2.UI
{
    internal class CustomScrollbarControlDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        public override SelectionRules SelectionRules
        {
            get
            {
                SelectionRules selectionRules = base.SelectionRules;
                PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(this.Component)["AutoSize"];
                if (propDescriptor != null)
                {
                    bool autoSize = (bool)propDescriptor.GetValue(this.Component);
                    if (autoSize)
                        selectionRules = SelectionRules.Visible | SelectionRules.Moveable | SelectionRules.BottomSizeable | SelectionRules.TopSizeable;
                    else
                        selectionRules = SelectionRules.Visible | SelectionRules.AllSizeable | SelectionRules.Moveable;
                }
                return selectionRules;
            }
        }
    }

    [Designer(typeof(CustomScrollbarControlDesigner))]
    public partial class CustomScrollbar : UserControl
    {
        protected Color moFillColor = Color.FromArgb(64, 64, 64);
        protected Color moScrollBarColor = Color.FromArgb(47, 51, 54);

        protected int moLargeChange = 10;
        protected int moSmallChange = 1;
        protected int moMinimum = 0;
        protected int moMaximum = 100;
        protected int moValue = 0;
        private int nClickPoint;

        protected int moThumbTop = 0;

        protected bool moAutoSize = false;

        private bool moThumbDown = false;
        private bool moThumbDragging = false;

        public new event EventHandler Scroll = null;
        public event EventHandler ValueChanged = null;

        private float GetThumbHeight()
        {
            int nTrackHeight = this.Height;
            float fThumbHeight = ((float)LargeChange / (float)Maximum) * nTrackHeight;

            if (fThumbHeight > nTrackHeight)
                fThumbHeight = nTrackHeight;
            if (fThumbHeight < 56)
                fThumbHeight = 56;

            return fThumbHeight;
        }

        public CustomScrollbar()
        {
            InitializeComponent(); Init();
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            base.MinimumSize = new Size(0, (int)GetThumbHeight());
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("LargeChange")]
        public int LargeChange
        {
            get => moLargeChange;
            set
            {
                moLargeChange = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("SmallChange")]
        public int SmallChange
        {
            get => moSmallChange;
            set
            {
                moSmallChange = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("Minimum")]
        public int Minimum
        {
            get => moMinimum; 
            set
            {
                moMinimum = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("Maximum")]
        public int Maximum
        {
            get => moMaximum;
            set
            {
                moMaximum = value;
                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Behavior"), Description("Value")]
        public int Value
        {
            get => moValue; 
            set
            {
                moValue = value;

                //figure out value
                int nPixelRange = this.Height - (int)GetThumbHeight();
                int nRealRange = (Maximum - Minimum) - LargeChange;
                float fPerc = 0.0f;
                if (nRealRange != 0)
                    fPerc = (float)moValue / (float)nRealRange;

                float fTop = fPerc * nPixelRange;
                moThumbTop = (int)fTop;


                Invalidate();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("FillColor")]
        public Color FillColor
        {
            get => moFillColor; 
            set => moFillColor = value;
        }

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true), DefaultValue(false), Category("Skin"), Description("ScrollBarColor")]
        public Color ScrollBarColor
        {
            get => moScrollBarColor; 
            set => moScrollBarColor = value; 
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            int radius = this.Width / 2;
            using (GraphicsPath path = UITools.RoundedRect(new Rectangle(0, 0, this.Width - 1, this.Height - 1), radius))
            {
                using (Brush b = new SolidBrush(FillColor))
                    e.Graphics.FillPath(b, path);
            }

            float fSpanHeight = (GetThumbHeight() - this.Width - 4) / 2.0f;
            int nSpanHeight = (int)fSpanHeight;

            int nTop = moThumbTop;
            using (Brush b = new SolidBrush(ScrollBarColor))
            {
                using (GraphicsPath topPath = UITools.RoundedRect(new Rectangle(0, nTop, this.Width - 1, this.Width), radius))
                    e.Graphics.FillPath(b, topPath);
                nTop += radius;

                e.Graphics.FillRectangle(b, new RectangleF(0f, (float)nTop, (float)this.Width, (float)fSpanHeight * 2));
                nTop += nSpanHeight;

                e.Graphics.FillRectangle(b, new Rectangle(0, nTop, this.Width, 30));
                //nTop += 30;

                Rectangle rect = new Rectangle(0, nTop, this.Width, nSpanHeight); // nSpanHeight * 2
                                                                                  //draw top span
                e.Graphics.FillRectangle(b, rect);
                nTop += nSpanHeight;

                using (GraphicsPath botPath = UITools.RoundedRect(new Rectangle(0, nTop - radius, this.Width - 1, this.Width + radius), radius))
                    e.Graphics.FillPath(b, botPath);
            }
        }

        private void Init()
        {
            this.SuspendLayout();
            // 
            // CustomScrollbar
            // 
            this.Name = "CustomScrollbar";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RoundedScrollbar_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RoundedScrollbar_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RoundedScrollbar_MouseUp);
            this.ResumeLayout(false);
        }

        private void RoundedScrollbar_MouseDown(object sender, MouseEventArgs e)
        {
            Point ptPoint = this.PointToClient(Cursor.Position);
            int nTop = moThumbTop;

            Rectangle thumbrect = new Rectangle(new Point(0, nTop), new Size(this.Width, this.Height));
            if (thumbrect.Contains(ptPoint))
            {
                nClickPoint = (ptPoint.Y - nTop);
                this.moThumbDown = true;
            }
        }

        private void RoundedScrollbar_MouseUp(object sender, MouseEventArgs e)
        {
            this.moThumbDown = false;
            this.moThumbDragging = false;
        }

        private void MoveThumb(int y)
        {
            int nRealRange = Maximum - Minimum;
            int nSpot = nClickPoint;

            int nPixelRange = (this.Height - (int)GetThumbHeight());
            if (moThumbDown && nRealRange > 0)
            {
                if (nPixelRange > 0)
                {
                    int nNewThumbTop = y - nSpot;

                    if (nNewThumbTop < 0)
                        moThumbTop = 0;
                    else if (nNewThumbTop > nPixelRange)
                        moThumbTop = nPixelRange;
                    else
                        moThumbTop = nNewThumbTop;

                    //figure out value
                    float fPerc = (float)moThumbTop / (float)nPixelRange;
                    float fValue = fPerc * (Maximum - LargeChange);
                    moValue = (int)fValue;

                    Application.DoEvents();

                    Invalidate();
                }
            }
        }

        private void RoundedScrollbar_MouseMove(object sender, MouseEventArgs e)
        {
            this.moThumbDragging = moThumbDown;
            if (this.moThumbDragging)
                MoveThumb(e.Y);

            if (ValueChanged != null)
                ValueChanged(this, new EventArgs());

            if (Scroll != null)
                Scroll(this, new EventArgs());
        }
    }
}
