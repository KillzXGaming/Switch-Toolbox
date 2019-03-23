#region License

/* Copyright (c) 2017 Fabrice Lacharme
 * This code is inspired from Michal Brylka 
 * https://www.codeproject.com/Articles/17395/Owner-drawn-trackbar-slider
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;

namespace BarSlider
{
    /* Original code from Michal Brylka on Code Project
    * see https://www.codeproject.com/Articles/17395/Owner-drawn-trackbar-slider
    * BarSlider is a trackbar control written in C# as a replacement of the trackbar 
    * 
    * CodeProject: https://www.codeproject.com/Tips/1193311/Csharp-Slider-Trackbar-Control-using-Windows-Forms
    * Github: https://github.com/fabricelacharme/BarSlider
    * 
    * 20/11/17 - version 1.0.O.1
    * 
    * Fixed: erroneous vertical display in case of minimum <> 0 (negative or positive)
    * Modified: DrawBarSlider, OnMouseMove
    * 
    * Added: Ticks display transformations
    * - TickAdd: allow to add a fixed value to the graduations: 
    *       usage: transform K = °C + 273,15, or °F = 1,8°C + 32   K = (°F + 459,67) / 1,8
    * - TickDivide: allow to diveide by a fixed value the graduations 
    *       usage: divide by 1000 => display graduations in kilograms when in gram
    *       
    *       
    * 10/12/17 - version 1.0.0.2
    * 
    * Added ForeColor property to graduations text color
    * 
    */

    public class KeyFrameThumb
    {
        public Color Color { get; set; }
        public Rectangle Rectangle { get; set; }
        public int Frame { get; set; }
        public Bitmap bitmap { get; set; }

    }

    public class BarTextBox : TextBox
    {
        public BarTextBox()
        {
        }
    }

    /// <summary>
    /// Encapsulates control that visualy displays certain integer value and allows user to change it within desired range. It imitates <see cref="System.Windows.Forms.TrackBar"/> as far as mouse usage is concerned.
    /// </summary>
    [ToolboxBitmap(typeof(TrackBar))]
    [DefaultEvent("Scroll"), DefaultProperty("BarInnerColor")]
    public partial class BarSlider : Control
    {
        BarTextBox textBox;

        Point prevPos;

        float previousValue;

        public float IncrementAmount { get; set; } = 0.01f;

        private Type dataType;

        public Type DataType
        {
            get
            {
                return dataType;
            }
            set
            {
                dataType = value;

                _minimum = -99999999;
                _maximum = 999999999;

                /*     if (dataType == typeof(uint)) {
                         Precision = 1;
                      //   Minimum = uint.MinValue;
                     //    Maximum = uint.MaxValue;

                         Minimum = 0;
                         Maximum = 999999999999999;

                     }

                     if (dataType == typeof(int)) {
                         Precision = 1;
                     //    Minimum = int.MinValue;
                       //  Maximum = int.MaxValue;
                     }

                     if (dataType == typeof(float)) {
                         Precision = 0.01f;
                         Minimum = -999999999999999;
                         Maximum = 999999999999999;
                     }*/
            }
        }



        private bool TextEditorActive { get; set; }

        private bool DisplayInterlapsedBar { get; set; }

        public Color ActiveEditColor { get; set; } = Color.FromArgb(40,40,40);

        public void SetTheme()
        {
            Color c1 = _barPenColorTop;
            Color c2 = _barPenColorMiddle;
            Color c3 = _barPenColorBottom;

            c1 = _elapsedPenColorTop;
            c2 = _elapsedPenColorMiddle;
            c3 = _elapsedPenColorBottom;

            ActiveEditColor = FormThemes.BaseTheme.TextEditorBackColor;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            TextEditorActive = true;

            prevPos = Cursor.Position;

            Cursor.Show();

            textBox.Location = new Point(30, ClientSize.Height / 2 - 5);
            textBox.Text = Value.ToString();
            textBox.Font = this.Font;
            textBox.BorderStyle = BorderStyle.None;
            textBox.Visible = true;
            textBox.Width = ClientSize.Width - 50;
            textBox.Height = ClientSize.Height;
            textBox.TextChanged += new EventHandler(OnTextChanged);
            textBox.KeyDown += new KeyEventHandler(OnKeyPressed);
            textBox.ReadOnly = false;
            textBox.ForeColor = this.ForeColor;
            textBox.BackColor = ActiveEditColor;

            DisplayInterlapsedBar = false;

            Invalidate();
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DisableTextEditor();
            }
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            float value;

            bool IsSuccess = false;

            IsSuccess = float.TryParse(textBox.Text, out value);

            if (IsSuccess && value < Maximum && value > Minimum)
            {
                Value = value;

                Invalidate();
            }
        }


        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (TextEditorActive)
                return;

            Cursor.Position = prevPos;
            Cursor.Show();
        }




        #region Events

        /// <summary>
        /// Fires when Slider position has changed
        /// </summary>
        [Description("Event fires when the Value property changes")]
        [Category("Action")]
        public event EventHandler ValueChanged;

        /// <summary>
        /// Fires when user scrolls the Slider
        /// </summary>
        [Description("Event fires when the Slider position is changed")]
        [Category("Behavior")]
        public event ScrollEventHandler Scroll;

        #endregion

        public SliderLayout sliderLayout = SliderLayout.BlenderSlider;

        public enum SliderLayout
        {
            CenterNumber,
            BlenderSlider,
        }



        public bool UseInterlapsedBar { get; set; } = true;

        public string InputName { get; set; }

        private float precision = 0.01f;
        public float Precision
        {
            get { return precision; }
            set
            {
                precision = value;
                // todo: update the 5 properties below
            }
        }


        #region Properties

        private Rectangle barRect; //bounding rectangle of bar area
        private Rectangle barHalfRect;
        private Rectangle thumbHalfRect;
        private Rectangle elapsedRect; //bounding rectangle of elapsed area

        public List<KeyFrameThumb> keyFrameThumbs = new List<KeyFrameThumb>();
        public void AddKeyFrameThumbSlider(int frame, int translateY, int scale, Color color)
        {
            keyFrameThumbs.Add(new KeyFrameThumb()
            {
                Rectangle = new Rectangle(new Point(frame, translateY), new Size(scale / 2, scale / 2)),
                Color = color,
                Frame = frame,
            });
        }
        public void AddKeyFrameThumbSlider(int frame, int translateY, int scale, Image Image)
        {
            Bitmap bitmap = ResizeImage(Image, scale / 2, scale / 2);

            keyFrameThumbs.Add(new KeyFrameThumb()
            {
                Rectangle = new Rectangle(new Point(frame, translateY), new Size(scale, scale)),
                Color = Color.Black,
                Frame = frame,
                bitmap = bitmap,
            });
        }
        #region thumb

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private Rectangle thumbRect; //bounding rectangle of thumb area
        /// <summary>
        /// Gets the thumb rect. Usefull to determine bounding rectangle when creating custom thumb shape.
        /// </summary>
        /// <value>The thumb rect.</value>
        [Browsable(false)]
        public Rectangle ThumbRect
        {
            get { return thumbRect; }
        }

        private Size _thumbSize = new Size(16, 16);

        /// <summary>
        /// Gets or sets the size of the thumb.
        /// </summary>
        /// <value>The size of the thumb.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">exception thrown when value is lower than zero or grather than half of appropiate dimension</exception>
        [Description("Set Slider thumb size")]
        [Category("BarSlider")]
        [DefaultValue(16)]
        public Size ThumbSize
        {
            get { return _thumbSize; }
            set
            {
                int h = value.Height;
                int w = value.Width;
                if (h > 0 && w > 0)
                {
                    _thumbSize = new Size(w, h);
                }
                else
                    throw new ArgumentOutOfRangeException(
                        "TrackSize has to be greather than zero and lower than half of Slider width");

                Invalidate();
            }
        }

        private GraphicsPath _thumbCustomShape = null;
        /// <summary>
        /// Gets or sets the thumb custom shape. Use ThumbRect property to determine bounding rectangle.
        /// </summary>
        /// <value>The thumb custom shape. null means default shape</value>
        [Description("Set Slider's thumb's custom shape")]
        [Category("BarSlider")]
        [Browsable(false)]
        [DefaultValue(typeof(GraphicsPath), "null")]
        public GraphicsPath ThumbCustomShape
        {
            get { return _thumbCustomShape; }
            set
            {
                _thumbCustomShape = value;
                //_thumbSize = (int) (_barOrientation == Orientation.Horizontal ? value.GetBounds().Width : value.GetBounds().Height) + 1;
                _thumbSize = new Size((int)value.GetBounds().Width, (int)value.GetBounds().Height);

                Invalidate();
            }
        }

        private Size _thumbRoundRectSize = new Size(16, 16);
        /// <summary>
        /// Gets or sets the size of the thumb round rectangle edges.
        /// </summary>
        /// <value>The size of the thumb round rectangle edges.</value>
        [Description("Set Slider's thumb round rect size")]
        [Category("BarSlider")]
        [DefaultValue(typeof(Size), "16; 16")]
        public Size ThumbRoundRectSize
        {
            get { return _thumbRoundRectSize; }
            set
            {
                int h = value.Height, w = value.Width;
                if (h <= 0) h = 1;
                if (w <= 0) w = 1;
                _thumbRoundRectSize = new Size(w, h);
                Invalidate();
            }
        }

        private Size _borderRoundRectSize = new Size(8, 8);
        /// <summary>
        /// Gets or sets the size of the border round rect.
        /// </summary>
        /// <value>The size of the border round rect.</value>
        [Description("Set Slider's border round rect size")]
        [Category("BarSlider")]
        [DefaultValue(typeof(Size), "8; 8")]
        public Size BorderRoundRectSize
        {
            get { return _borderRoundRectSize; }
            set
            {
                int h = value.Height, w = value.Width;
                if (h <= 0) h = 1;
                if (w <= 0) w = 1;
                _borderRoundRectSize = new Size(w, h);
                Invalidate();
            }
        }

        private bool _drawSemitransparentThumb = true;
        /// <summary>
        /// Gets or sets a value indicating whether to draw semitransparent thumb.
        /// </summary>
        /// <value><c>true</c> if semitransparent thumb should be drawn; otherwise, <c>false</c>.</value>
        [Description("Set whether to draw semitransparent thumb")]
        [Category("BarSlider")]
        [DefaultValue(true)]
        public bool DrawSemitransparentThumb
        {
            get { return _drawSemitransparentThumb; }
            set
            {
                _drawSemitransparentThumb = value;
                Invalidate();
            }
        }

        private Image _thumbImage = null;
        //private Image _thumbImage = Properties.Resources.BTN_Thumb_Blue;
        /// <summary>
        /// Gets or sets the Image used to render the thumb.
        /// </summary>
        /// <value>the thumb Image</value> 
        [Description("Set to use a specific Image for the thumb")]
        [Category("BarSlider")]
        [DefaultValue(null)]
        public Image ThumbImage
        {
            get { return _thumbImage; }
            set
            {
                if (value != null)
                    _thumbImage = value;
                else
                    _thumbImage = null;
                Invalidate();
            }
        }

        #endregion


        #region Appearance

        private Orientation _barOrientation = Orientation.Horizontal;
        /// <summary>
        /// Gets or sets the orientation of Slider.
        /// </summary>
        /// <value>The orientation.</value>
        [Description("Set Slider orientation")]
        [Category("BarSlider")]
        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation
        {
            get { return _barOrientation; }
            set
            {
                if (_barOrientation != value)
                {
                    _barOrientation = value;
                    // Switch from horizontal to vertical (design mode)
                    // Comment these lines if problems in Run mode
                    if (this.DesignMode)
                    {
                        int temp = Width;
                        Width = Height;
                        Height = temp;
                    }

                    Invalidate();
                }
            }
        }

        private bool _drawFocusRectangle = false;
        /// <summary>
        /// Gets or sets a value indicating whether to draw focus rectangle.
        /// </summary>
        /// <value><c>true</c> if focus rectangle should be drawn; otherwise, <c>false</c>.</value>
        [Description("Set whether to draw focus rectangle")]
        [Category("BarSlider")]
        [DefaultValue(false)]
        public bool DrawFocusRectangle
        {
            get { return _drawFocusRectangle; }
            set
            {
                _drawFocusRectangle = value;
                Invalidate();
            }
        }

        private bool _mouseEffects = true;
        /// <summary>
        /// Gets or sets whether mouse entry and exit actions have impact on how control look.
        /// </summary>
        /// <value><c>true</c> if mouse entry and exit actions have impact on how control look; otherwise, <c>false</c>.</value>
        [Description("Set whether mouse entry and exit actions have impact on how control look")]
        [Category("BarSlider")]
        [DefaultValue(true)]
        public bool MouseEffects
        {
            get { return _mouseEffects; }
            set
            {
                _mouseEffects = value;
                Invalidate();
            }
        }

        #endregion


        #region values

        private int _trackerValue = 30;
        /// <summary>
        /// Gets or sets the value of Slider.
        /// </summary>
        /// <value>The value.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">exception thrown when value is outside appropriate range (min, max)</exception>
        [Description("Set Slider value")]
        [Category("BarSlider")]
        [DefaultValue(30)]
        public float Value
        {
            get { return _trackerValue * precision; }
            set
            {
                if (value >= _minimum & value <= _maximum)
                {
                    _trackerValue = (int)(value / precision);
                    if (ValueChanged != null) ValueChanged(this, new EventArgs());
                    Invalidate();
                }
                else throw new ArgumentOutOfRangeException($"Value is outside appropriate range (min, max) {value} {_minimum} {_maximum}");
            }
        }

        private int _minimum = 0;
        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">exception thrown when minimal value is greather than maximal one</exception>
        [Description("Set Slider minimal point")]
        [Category("BarSlider")]
        [DefaultValue(0)]
        public float Minimum
        {
            get { return _minimum * precision; }
            set
            {
                if (value < _maximum)
                {
                    _minimum = (int)(value / precision);
                    if (_trackerValue < _minimum)
                    {
                        _trackerValue = _minimum;
                        if (ValueChanged != null) ValueChanged(this, new EventArgs());
                    }
                    Invalidate();
                }
                else throw new ArgumentOutOfRangeException("Minimal value is greather than maximal one");
            }
        }

        private int _maximum = 100;
        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">exception thrown when maximal value is lower than minimal one</exception>
        [Description("Set Slider maximal point")]
        [Category("BarSlider")]
        [DefaultValue(100)]

        public float Maximum
        {
            get { return _maximum * precision; }
            set
            {
                if (value > _minimum)
                {
                    _maximum = (int)(value / precision);
                    if (_trackerValue > _maximum)
                    {
                        _trackerValue = _maximum;
                        if (ValueChanged != null) ValueChanged(this, new EventArgs());
                    }
                    Invalidate();
                }
                //else throw new ArgumentOutOfRangeException("Maximal value is lower than minimal one");
            }
        }

        private uint _smallChange = 1;
        /// <summary>
        /// Gets or sets trackbar's small change. It affects how to behave when directional keys are pressed
        /// </summary>
        /// <value>The small change value.</value>
        [Description("Set trackbar's small change")]
        [Category("BarSlider")]
        [DefaultValue(1)]
        public float SmallChange
        {
            get { return _smallChange * precision; }
            set { _smallChange = (uint)(value / precision); }
        }

        private uint _largeChange = 5;
        /// <summary>
        /// Gets or sets trackbar's large change. It affects how to behave when PageUp/PageDown keys are pressed
        /// </summary>
        /// <value>The large change value.</value>
        [Description("Set trackbar's large change")]
        [Category("BarSlider")]
        [DefaultValue(5)]
        public new float LargeChange
        { get { return _largeChange * precision; } set { _largeChange = (uint)(value / precision); } }

        private int _mouseWheelBarPartitions = 10;
        /// <summary>
        /// Gets or sets the mouse wheel bar partitions.
        /// </summary>
        /// <value>The mouse wheel bar partitions.</value>
        /// <exception cref="T:System.ArgumentOutOfRangeException">exception thrown when value isn't greather than zero</exception>
        [Description("Set to how many parts is bar divided when using mouse wheel")]
        [Category("BarSlider")]
        [DefaultValue(10)]
        public int MouseWheelBarPartitions
        {
            get { return _mouseWheelBarPartitions; }
            set
            {
                if (value > 0)
                    _mouseWheelBarPartitions = value;
                else throw new ArgumentOutOfRangeException("MouseWheelBarPartitions has to be greather than zero");
            }
        }

        #endregion


        #region colors

        private Color _thumbOuterColor = Color.White;
        /// <summary>
        /// Gets or sets the thumb outer color.
        /// </summary>
        /// <value>The thumb outer color.</value>
        [Description("Sets Slider thumb outer color")]
        [Category("BarSlider")]
        [DefaultValue(typeof(Color), "White")]
        public Color ThumbOuterColor
        {
            get { return _thumbOuterColor; }
            set
            {
                _thumbOuterColor = value;
                Invalidate();
            }
        }

        private Color _thumbInnerColor = Color.FromArgb(21, 56, 152);
        /// <summary>
        /// Gets or sets the inner color of the thumb.
        /// </summary>
        /// <value>The inner color of the thumb.</value>
        [Description("Set Slider thumb inner color")]
        [Category("BarSlider")]
        public Color ThumbInnerColor
        {
            get { return _thumbInnerColor; }
            set
            {
                _thumbInnerColor = value;
                Invalidate();
            }
        }

        private Color _thumbPenColor = Color.FromArgb(21, 56, 152);
        /// <summary>
        /// Gets or sets the color of the thumb pen.
        /// </summary>
        /// <value>The color of the thumb pen.</value>
        [Description("Set Slider thumb pen color")]
        [Category("BarSlider")]
        public Color ThumbPenColor
        {
            get { return _thumbPenColor; }
            set
            {
                _thumbPenColor = value;
                Invalidate();
            }
        }

        private Color _barInnerColor = Color.Black;
        /// <summary>
        /// Gets or sets the inner color of the bar.
        /// </summary>
        /// <value>The inner color of the bar.</value>
        [Description("Set Slider bar inner color")]
        [Category("BarSlider")]
        [DefaultValue(typeof(Color), "Black")]
        public Color BarInnerColor
        {
            get { return _barInnerColor; }
            set
            {
                _barInnerColor = value;
                Invalidate();
            }
        }

        private Color _elapsedPenColorMiddle = Color.FromArgb(65, 65, 65);
        /// <summary>
        /// Gets or sets the top color of the Elapsed
        /// </summary>
        [Description("Gets or sets the top color of the elapsed")]
        [Category("BarSlider")]
        public Color ElapsedPenColorMiddle
        {
            get { return _elapsedPenColorMiddle; }
            set
            {
                _elapsedPenColorMiddle = value;
                Invalidate();
            }
        }

        private Color _elapsedPenColorTop = Color.FromArgb(60, 60, 60);
        /// <summary>
        /// Gets or sets the top color of the Elapsed
        /// </summary>
        [Description("Gets or sets the top color of the elapsed")]
        [Category("BarSlider")]
        public Color ElapsedPenColorTop
        {
            get { return _elapsedPenColorTop; }
            set
            {
                _elapsedPenColorTop = value;
                Invalidate();
            }
        }

        private Color _elapsedPenColorBottom = Color.FromArgb(60, 60, 60);
        /// <summary>
        /// Gets or sets the bottom color of the elapsed
        /// </summary>
        [Description("Gets or sets the bottom color of the elapsed")]
        [Category("BarSlider")]
        public Color ElapsedPenColorBottom
        {
            get { return _elapsedPenColorBottom; }
            set
            {
                _elapsedPenColorBottom = value;
                Invalidate();
            }
        }

        private Color _barPenColorTop = Color.FromArgb(40, 40, 40);
        /// <summary>
        /// Gets or sets the top color of the bar
        /// </summary>
        [Description("Gets or sets the top color of the bar")]
        [Category("BarSlider")]
        public Color BarPenColorTop
        {
            get { return _barPenColorTop; }
            set
            {
                _barPenColorTop = value;
                Invalidate();
            }
        }



        private Color _barPenColorMiddle = Color.FromArgb(45, 45, 45);
        /// <summary>
        /// Gets or sets the middle color of bar
        /// </summary>
        [Description("Gets or sets the middle color of the bar")]
        [Category("BarSlider")]
        public Color BarPenColorMiddle
        {
            get { return _barPenColorMiddle; }
            set
            {
                _barPenColorMiddle = value;
                Invalidate();
            }
        }

        private Color _barPenColorBottom = Color.FromArgb(50, 50, 50);
        /// <summary>
        /// Gets or sets the bottom color of bar
        /// </summary>
        [Description("Gets or sets the bottom color of the bar")]
        [Category("BarSlider")]
        public Color BarPenColorBottom
        {
            get { return _barPenColorBottom; }
            set
            {
                _barPenColorBottom = value;
                Invalidate();
            }
        }

        private Color _elapsedInnerColor = Color.FromArgb(21, 56, 152);
        /// <summary>
        /// Gets or sets the inner color of the elapsed.
        /// </summary>
        /// <value>The inner color of the elapsed.</value>
        [Description("Set Slider's elapsed part inner color")]
        [Category("BarSlider")]
        public Color ElapsedInnerColor
        {
            get { return _elapsedInnerColor; }
            set
            {
                _elapsedInnerColor = value;
                Invalidate();
            }
        }

        private Color _tickColor = Color.White;
        /// <summary>
        /// Gets or sets the color of the graduations
        /// </summary>
        [Description("Color of graduations")]
        [Category("BarSlider")]
        public Color TickColor
        {
            get { return _tickColor; }
            set
            {
                if (value != _tickColor)
                {
                    _tickColor = value;
                    Invalidate();
                }
            }
        }

        #endregion


        #region divisions

        // For ex: if values are multiples of 50, 
        // values = 0, 50, 100, 150 etc...
        //set TickDivide to 50
        // And ticks will be displayed as 
        // values = 0, 1, 2, 3 etc...
        private float _tickDivide = 0;

        [Description("Gets or sets a value used to divide the graduation")]
        [Category("BarSlider")]
        public float TickDivide
        {
            get { return _tickDivide; }
            set
            {
                _tickDivide = value;
                Invalidate();
            }
        }

        private float _tickAdd = 0;
        [Description("Gets or sets a value added to the graduation")]
        [Category("BarSlider")]
        public float TickAdd
        {
            get { return _tickAdd; }
            set
            {
                _tickAdd = value;
                Invalidate();
            }
        }

        private TickStyle _tickStyle = TickStyle.TopLeft;
        /// <summary>
        /// Gets or sets where to display the ticks (None, both top-left, bottom-right)
        /// </summary>
        [Description("Gets or sets where to display the ticks")]
        [Category("BarSlider")]
        [DefaultValue(TickStyle.TopLeft)]
        public TickStyle TickStyle
        {
            get { return _tickStyle; }
            set
            {
                _tickStyle = value;
                Invalidate();
            }
        }

        private int _scaleDivisions = 10;
        /// <summary>
        /// How many divisions of maximum?
        /// </summary>
        [Description("Set the number of intervals between minimum and maximum")]
        [Category("BarSlider")]
        public int ScaleDivisions
        {
            get { return _scaleDivisions; }
            set
            {
                if (value > 0)
                {
                    _scaleDivisions = value;
                }
                //else throw new ArgumentOutOfRangeException("TickFreqency must be > 0 and < Maximum");

                Invalidate();
            }
        }

        private int _scaleSubDivisions = 5;
        /// <summary>
        /// How many subdivisions for each division
        /// </summary>
        [Description("Set the number of subdivisions between main divisions of graduation.")]
        [Category("BarSlider")]
        public int ScaleSubDivisions
        {
            get { return _scaleSubDivisions; }
            set
            {
                if (value > 0 && _scaleDivisions > 0 && (_maximum - _minimum) / ((value + 1) * _scaleDivisions) > 0)
                {
                    _scaleSubDivisions = value;

                }
                //else throw new ArgumentOutOfRangeException("TickSubFreqency must be > 0 and < TickFrequency");

                Invalidate();
            }
        }

        private bool _showSmallScale = false;
        /// <summary>
        /// Shows Small Scale marking.
        /// </summary>
        [Description("Show or hide subdivisions of graduations")]
        [Category("BarSlider")]
        public bool ShowSmallScale
        {
            get { return _showSmallScale; }
            set
            {

                if (value == true)
                {
                    if (_scaleDivisions > 0 && _scaleSubDivisions > 0 && (_maximum - _minimum) / ((_scaleSubDivisions + 1) * _scaleDivisions) > 0)
                    {
                        _showSmallScale = value;
                        Invalidate();
                    }
                    else
                    {
                        _showSmallScale = false;
                    }
                }
                else
                {
                    _showSmallScale = value;
                    // need to redraw 
                    Invalidate();
                }
            }
        }

        private bool _showDivisionsText = true;
        /// <summary>
        /// Shows Small Scale marking.
        /// </summary>
        [Description("Show or hide text value of graduations")]
        [Category("BarSlider")]
        public bool ShowDivisionsText
        {
            get { return _showDivisionsText; }
            set
            {
                _showDivisionsText = value;
                Invalidate();
            }
        }

        #endregion


        #region Font

        /// <summary>
        /// Get or Sets the Font of the Text being displayed.
        /// </summary>
        [Bindable(true),
        Browsable(true),
        Category("BarSlider"),
        Description("Get or Sets the Font of the Text being displayed."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        EditorBrowsable(EditorBrowsableState.Always)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                Invalidate();
                OnFontChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Get or Sets the Font of the Text being displayed.
        /// </summary>
        [Bindable(true),
        Browsable(true),
        Category("BarSlider"),
        Description("Get or Sets the Color of the Text being displayed."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        EditorBrowsable(EditorBrowsableState.Always)]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                Invalidate();
                OnForeColorChanged(EventArgs.Empty);
            }
        }

        #endregion

        #endregion


        #region Color schemas

        //define own color schemas
        private Color[,] aColorSchema = new Color[,]
        {
                {
                    Color.White,                    // thumb outer
                    Color.FromArgb(21, 56, 152),    // thumb inner
                    Color.FromArgb(21, 56, 152),    // thumb pen color
                    
                    Color.Black,                    // bar inner    

                    Color.FromArgb(95, 140, 180),     // slider elapsed top                   
                    Color.FromArgb(99, 130, 208),     // slider elapsed bottom                    

                    Color.FromArgb(55, 60, 74),     // slider remain top                    
                    Color.FromArgb(87, 94, 110),     // slider remain bottom
                                         
                    Color.FromArgb(21, 56, 152)     // elapsed interieur centre
                },
                {
                    Color.White,                    // thumb outer
                    Color.Red,    // thumb inner
                    Color.Red,    // thumb pen color
                    
                    Color.Black,                    // bar inner    

                    Color.LightCoral,     // slider elapsed top                   
                    Color.Salmon,     // slider elapsed bottom
                    

                    Color.FromArgb(55, 60, 74),     // slider remain top                    
                    Color.FromArgb(87, 94, 110),     // slider remain bottom
                                         
                    Color.Red     // gauche interieur centre
                },
                {
                    Color.White,                    // thumb outer
                    Color.Green,    // thumb inner
                    Color.Green,    // thumb pen color
                    
                    Color.Black,                    // bar inner    

                    Color.SpringGreen,     // slider elapsed top                   
                    Color.LightGreen,     // slider elapsed bottom
                    

                    Color.FromArgb(55, 60, 74),     // slider remain top                    
                    Color.FromArgb(87, 94, 110),     // slider remain bottom
                                         
                    Color.Green     // gauche interieur centre
                },
        };

        public enum ColorSchemas
        {
            BlueColors,
            RedColors,
            GreenColors
        }

        private ColorSchemas colorSchema = ColorSchemas.BlueColors;

        /// <summary>
        /// Sets color schema. Color generalization / fast color changing. Has no effect when slider colors are changed manually after schema was applied. 
        /// </summary>
        /// <value>New color schema value</value>
        [Description("Set Slider color schema. Has no effect when slider colors are changed manually after schema was applied.")]
        [Category("BarSlider")]
        [DefaultValue(typeof(ColorSchemas), "BlueColors")]
        public ColorSchemas ColorSchema
        {
            get { return colorSchema; }
            set
            {
                colorSchema = value;
                byte sn = (byte)value;
                _thumbOuterColor = aColorSchema[sn, 0];
                _thumbInnerColor = aColorSchema[sn, 1];
                _thumbPenColor = aColorSchema[sn, 2];

                _barInnerColor = aColorSchema[sn, 3];

                _elapsedPenColorTop = aColorSchema[sn, 4];
                _elapsedPenColorBottom = aColorSchema[sn, 5];

                _barPenColorTop = aColorSchema[sn, 6];
                _barPenColorBottom = aColorSchema[sn, 7];

                _elapsedInnerColor = aColorSchema[sn, 8];

                Invalidate();
            }
        }

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BarSlider"/> class.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="value">The current value.</param>
        public BarSlider(int min, int max, int value)
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw | ControlStyles.Selectable |
                     ControlStyles.SupportsTransparentBackColor | ControlStyles.UserMouse |
                     ControlStyles.UserPaint, true);

            // Default backcolor
            BackColor = Color.FromArgb(70, 77, 95);
            ForeColor = Color.White;

            // Font
            //this.Font = new Font("Tahoma", 6.75f);
            this.Font = new Font("Microsoft Sans Serif", 8f);

            Minimum = min;
            Maximum = max;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BarSlider"/> class.
        /// </summary>
        public BarSlider() : this(0, 100, 30)
        {
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);

            DisplayInterlapsedBar = true;

            BackColor = FormThemes.BaseTheme.ValueBarSliderElapseBottmColor;

            ThumbPenColor = FormThemes.BaseTheme.ValueBarSliderElapseBottmColor;
            ThumbPenColor = FormThemes.BaseTheme.ValueBarSliderElapseBottmColor;
            ThumbInnerColor = FormThemes.BaseTheme.ValueBarSliderElapseBottmColor;

            ElapsedPenColorTop = FormThemes.BaseTheme.ValueBarSliderElapseTopColor;
            ElapsedPenColorMiddle = FormThemes.BaseTheme.ValueBarSliderElapseMiddleColor;
            ElapsedPenColorBottom = FormThemes.BaseTheme.ValueBarSliderElapseBottmColor;

            BarPenColorTop = FormThemes.BaseTheme.ValueBarSliderTopColor;
            BarPenColorMiddle = FormThemes.BaseTheme.ValueBarSliderMiddleColor;
            BarPenColorBottom = FormThemes.BaseTheme.ValueBarSliderBottmColor;


            textBox = new BarTextBox();
            textBox.Visible = false;
            textBox.Enabled = true;
            textBox.KeyUp += new KeyEventHandler(textBox1_KeyUp);
            Controls.Add(textBox);
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void DisableTextEditor()
        {
            prevPos = Cursor.Position;

            textBox.Visible = false;

            Invalidate();

            TextEditorActive = false;
        }

        #endregion

        #region Paint

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!Enabled)
            {
                Color[] desaturatedColors = DesaturateColors(_thumbOuterColor, _thumbInnerColor, _thumbPenColor,
                                                             _barInnerColor,
                                                             _elapsedPenColorTop, _elapsedPenColorBottom,
                                                             _barPenColorTop, _barPenColorBottom,
                                                             _elapsedInnerColor);
                DrawBarSlider(e,
                                    desaturatedColors[0], desaturatedColors[1], desaturatedColors[2],
                                    desaturatedColors[3],
                                    desaturatedColors[4], desaturatedColors[5],
                                    desaturatedColors[6], desaturatedColors[7],
                                    desaturatedColors[8]);
            }
            else
            {
                if (_mouseEffects && mouseInRegion)
                {
                    Color[] lightenedColors = LightenColors(_thumbOuterColor, _thumbInnerColor, _thumbPenColor,
                                                            _barInnerColor,
                                                            _elapsedPenColorTop, _elapsedPenColorBottom,
                                                            _barPenColorTop, _barPenColorBottom,
                                                            _elapsedInnerColor);
                    DrawBarSlider(e,
                        lightenedColors[0], lightenedColors[1], lightenedColors[2],
                        lightenedColors[3],
                        lightenedColors[4], lightenedColors[5],
                        lightenedColors[6], lightenedColors[7],
                        lightenedColors[8]);
                }
                else
                {
                    DrawBarSlider(e,
                                    _thumbOuterColor, _thumbInnerColor, _thumbPenColor,
                                    _barInnerColor,
                                    _elapsedPenColorTop, _elapsedPenColorBottom,
                                    _barPenColorTop, _barPenColorBottom,
                                    _elapsedInnerColor);
                }
            }
        }

        /// <summary>
        /// Draws the BarSlider control using passed colors.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        /// <param name="thumbOuterColorPaint">The thumb outer color paint.</param>
        /// <param name="thumbInnerColorPaint">The thumb inner color paint.</param>
        /// <param name="thumbPenColorPaint">The thumb pen color paint.</param>
        /// <param name="barInnerColorPaint">The bar inner color paint.</param>
        /// <param name="barPenColorPaint">The bar pen color paint.</param>
        /// <param name="elapsedInnerColorPaint">The elapsed inner color paint.</param>
        private void DrawBarSlider(PaintEventArgs e,
            Color thumbOuterColorPaint, Color thumbInnerColorPaint, Color thumbPenColorPaint,
            Color barInnerColorPaint,
            Color ElapsedTopPenColorPaint, Color ElapsedBottomPenColorPaint,
            Color barTopPenColorPaint, Color barBottomPenColorPaint,
            Color elapsedInnerColorPaint)
        {
            try
            {
                //set up thumbRect approprietly
                if (_barOrientation == Orientation.Horizontal)
                {
                    #region horizontal
                    int TrackX = (((_trackerValue - _minimum) * (ClientRectangle.Width - _thumbSize.Width)) / (_maximum - _minimum));
                    thumbRect = new Rectangle(TrackX, 0, _thumbSize.Width, ClientRectangle.Height);
                    #endregion
                }
                else
                {
                    #region vertical
                    int TrackY = (((_maximum - (_trackerValue)) * (ClientRectangle.Height - _thumbSize.Height)) / (_maximum - _minimum));
                    thumbRect = new Rectangle(ClientRectangle.X + ClientRectangle.Width / 2 - _thumbSize.Width / 2, TrackY, _thumbSize.Width, _thumbSize.Height);
                    #endregion
                }


                //adjust drawing rects
                barRect = ClientRectangle;
                // TODO : make barRect rectangle smaller than Control rectangle  
                // barRect = new Rectangle(ClientRectangle.X + 5, ClientRectangle.Y + 5, ClientRectangle.Width - 10, ClientRectangle.Height - 10);
                thumbHalfRect = thumbRect;
                LinearGradientMode gradientOrientation;


                if (_barOrientation == Orientation.Horizontal)
                {
                    #region horizontal
                    barRect.Inflate(-1, -barRect.Height / 3);
                    barHalfRect = barRect;
                    barHalfRect.Height /= 2;

                    gradientOrientation = LinearGradientMode.Vertical;


                    thumbHalfRect.Height /= 2;
                    elapsedRect = barRect;
                    elapsedRect.Width = thumbRect.Left + _thumbSize.Width / 2;
                    #endregion
                }
                else
                {
                    #region vertical
                    barRect.Inflate(-barRect.Width / 3, -1);
                    barHalfRect = barRect;
                    barHalfRect.Width /= 2;

                    gradientOrientation = LinearGradientMode.Vertical;

                    thumbHalfRect.Width /= 2;
                    elapsedRect = barRect;
                    elapsedRect.Height = barRect.Height - (thumbRect.Top + ThumbSize.Height / 2);
                    elapsedRect.Y = 1 + thumbRect.Top + ThumbSize.Height / 2;

                    #endregion
                }

           

                #region draw inner bar

                // inner bar is a single line 
                // draw the line on the whole lenght of the control
                if (_barOrientation == Orientation.Horizontal)
                {
                    RectangleF r = new RectangleF(0, 0, e.ClipRectangle.Width, e.ClipRectangle.Height);

                    Color c1 = _barPenColorTop;
                    Color c2 = _barPenColorMiddle;
                    Color c3 = _barPenColorBottom;

                    if (!UseInterlapsedBar)
                    {
                        c1 = _elapsedPenColorTop;
                        c2 = _elapsedPenColorMiddle;
                        c3 = _elapsedPenColorBottom;
                    }

                    if (TextEditorActive)
                    {
                        c1 = ActiveEditColor;
                        c2 = ActiveEditColor;
                        c3 = ActiveEditColor;
                    }


                    LinearGradientBrush br = new LinearGradientBrush(r, c1, c3, 90, true);

                    ColorBlend cb = new ColorBlend();
                    cb.Positions = new[] { 0, (float)0.5, 1 };
                    cb.Colors = new[] { c1, c2, c3 };
                    br.InterpolationColors = cb;

                    // paint
                    e.Graphics.FillRectangle(br, r);

                    //   e.Graphics.DrawLine(new Pen(barInnerColorPaint, ClientSize.Height /  2), barRect.X, barRect.Y + barRect.Height/2, barRect.X + barRect.Width, barRect.Y + barRect.Height / 2);
                    //  e.Graphics.DrawRectangle(new Pen(barInnerColorPaint, ClientSize.Height / 2), barRect.X, barRect.Y + barRect.Height / 2, barRect.X + barRect.Width, barRect.Y + barRect.Height / 2);
                }
                else
                {
                    e.Graphics.DrawLine(new Pen(barInnerColorPaint, ClientSize.Width), barRect.X + barRect.Width / 2, barRect.Y, barRect.X + barRect.Width / 2, barRect.Y + barRect.Height);
                }
                #endregion


                #region draw elapsed bar

                if (UseInterlapsedBar && !TextEditorActive)
                {
                    //draw elapsed inner bar (single line too)                               
                    if (_barOrientation == Orientation.Horizontal)
                    {
                        RectangleF r = new RectangleF(barRect.X, 0, barRect.X + elapsedRect.Width, e.ClipRectangle.Height);

                        Color c1 = _elapsedPenColorTop;
                        Color c2 = _elapsedPenColorMiddle;
                        Color c3 = _elapsedPenColorBottom;

                        LinearGradientBrush br = new LinearGradientBrush(r, c1, c3, 90, true);

                        ColorBlend cb = new ColorBlend();
                        cb.Positions = new[] { 0, (float)0.5, 1 };
                        cb.Colors = new[] { c1, c2, c3 };
                        br.InterpolationColors = cb;

                        // paint
                        e.Graphics.FillRectangle(br, r);

                    }
                    else
                    {
                    }
                }



                #endregion draw elapsed bar

                #region draw focusing rectangle
                //draw focusing rectangle
                if (Focused & _drawFocusRectangle)
                    using (Pen p = new Pen(Color.FromArgb(200, ElapsedTopPenColorPaint)))
                    {
                        p.DashStyle = DashStyle.Dot;
                        Rectangle r = ClientRectangle;
                        r.Width -= 2;
                        r.Height--;
                        r.X++;

                        using (GraphicsPath gpBorder = CreateRoundRectPath(r, _borderRoundRectSize))
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            e.Graphics.DrawPath(p, gpBorder);
                        }
                    }
                #endregion draw focusing rectangle



                SolidBrush br2 = new SolidBrush(ForeColor);

                var str2 = String.Format("{0}", Value);
                SizeF sizeF = e.Graphics.MeasureString(str2, this.Font);


                int tx2 = ClientSize.Width / 2;
                int ty2 = ClientSize.Height / 2 - 6;

                if (sliderLayout == SliderLayout.BlenderSlider)
                {
                    tx2 = 10;

                    if (!TextEditorActive)
                        e.Graphics.DrawString(InputName, this.Font, br2, tx2, ty2);

                    tx2 = ClientSize.Width - 60;
                }

                if (!TextEditorActive)
                {
                    e.Graphics.DrawString(str2, this.Font, br2, tx2, ty2);
                }
            }
            catch (Exception Err)
            {
                Console.WriteLine("DrawBackGround Error in " + Name + ":" + Err.Message);
            }
            finally
            {
            }
        }

        #endregion

        #region Overided events

        private bool mouseInRegion = false;
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.EnabledChanged"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (TextEditorActive)
                return;

            base.OnMouseEnter(e);
            mouseInRegion = true;
            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            if (TextEditorActive)
                return;

            base.OnMouseLeave(e);
            mouseInRegion = false;
            mouseInThumbRegion = false;
            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (TextEditorActive)
                return;

            prevPos = Cursor.Position;
            previousValue = Value;
            Cursor.Hide();

            base.OnMouseDown(e);
           if (e.Button == MouseButtons.Left)
            {
             /*   Capture = true;
                if (Scroll != null) Scroll(this, new ScrollEventArgs(ScrollEventType.ThumbTrack, _trackerValue));
                if (ValueChanged != null) ValueChanged(this, new EventArgs());
                OnMouseMove(e);*/
            }
        }

        private bool mouseInThumbRegion = false;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (TextEditorActive)
                return;

            int currentPositionX = 0;
            int direction = 0;

            base.OnMouseMove(e);

            mouseInThumbRegion = IsPointInRect(e.Location, thumbRect);
            if (Capture & e.Button == MouseButtons.Left)
            {
                double deltaDirection = currentPositionX - e.Location.X;
                direction = deltaDirection > 0 ? 1 : -1;
                currentPositionX = e.Location.X;

                //Increase or decrease based on mouse direction
                if (direction == 1) //Left
                {
                    _trackerValue -= (int)(IncrementAmount / precision);
                }
                else if (direction == -1) //Right
                {
                    _trackerValue += (int)(IncrementAmount / precision);
                }


                if (_trackerValue <= _minimum)
                {
                    _trackerValue = _minimum;
                }
                else if (_trackerValue >= _maximum)
                {
                    _trackerValue = _maximum;
                }

                if (ValueChanged != null) ValueChanged(this, new EventArgs());
            }
            else
            {
                currentPositionX = e.Location.X;
            }

            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (TextEditorActive)
                return;

            base.OnMouseUp(e);
            Capture = false;
            mouseInThumbRegion = IsPointInRect(e.Location, thumbRect);
            if (Scroll != null) Scroll(this, new ScrollEventArgs(ScrollEventType.EndScroll, _trackerValue));
            if (ValueChanged != null) ValueChanged(this, new EventArgs());
            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseWheel"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            return;

            if (TextEditorActive)
                return;

            base.OnMouseWheel(e);

            if (mouseInRegion)
            {
                int v = e.Delta / 120 * (_maximum - _minimum) / _mouseWheelBarPartitions;
                SetProperValue((int)Value + v);

                // Avoid to send MouseWheel event to the parent container
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.GotFocus"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.LostFocus"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyUp"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"></see> that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (TextEditorActive)
                return;

            Cursor.Position = prevPos;
            Cursor.Show();

            base.OnKeyUp(e);
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Left:
                    SetProperValue((int)Value - (int)_smallChange);
                    if (Scroll != null) Scroll(this, new ScrollEventArgs(ScrollEventType.SmallDecrement, (int)Value));
                    break;
                case Keys.Up:
                case Keys.Right:
                    SetProperValue((int)Value + (int)_smallChange);
                    if (Scroll != null) Scroll(this, new ScrollEventArgs(ScrollEventType.SmallIncrement, (int)Value));
                    break;
                case Keys.Home:
                    Value = _minimum;
                    break;
                case Keys.End:
                    Value = _maximum;
                    break;
                case Keys.PageDown:
                    SetProperValue((int)Value - (int)_largeChange);
                    if (Scroll != null) Scroll(this, new ScrollEventArgs(ScrollEventType.LargeDecrement, (int)Value));
                    break;
                case Keys.PageUp:
                    SetProperValue((int)Value + (int)_largeChange);
                    if (Scroll != null) Scroll(this, new ScrollEventArgs(ScrollEventType.LargeIncrement, (int)Value));
                    break;
            }
            if (Scroll != null && Value == _minimum) Scroll(this, new ScrollEventArgs(ScrollEventType.First, (int)Value));
            if (Scroll != null && Value == _maximum) Scroll(this, new ScrollEventArgs(ScrollEventType.Last, (int)Value));
            Point pt = PointToClient(Cursor.Position);
            OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, 0));
        }

        /// <summary>
        /// Processes a dialog key.
        /// </summary>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"></see> values that represents the key to process.</param>
        /// <returns>
        /// true if the key was processed by the control; otherwise, false.
        /// </returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (TextEditorActive)
                return false;

            if (keyData == Keys.Tab | ModifierKeys == Keys.Shift)
                return base.ProcessDialogKey(keyData);
            else
            {
                OnKeyDown(new KeyEventArgs(keyData));
                return true;
            }
        }

        #endregion

        #region Help routines

        /// <summary>
        /// Creates the round rect path.
        /// </summary>
        /// <param name="rect">The rectangle on which graphics path will be spanned.</param>
        /// <param name="size">The size of rounded rectangle edges.</param>
        /// <returns></returns>
        public static GraphicsPath CreateRoundRectPath(Rectangle rect, Size size)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.AddLine(rect.Left + size.Width / 2, rect.Top, rect.Right - size.Width / 2, rect.Top);
            gp.AddArc(rect.Right - size.Width, rect.Top, size.Width, size.Height, 270, 90);

            gp.AddLine(rect.Right, rect.Top + size.Height / 2, rect.Right, rect.Bottom - size.Width / 2);
            gp.AddArc(rect.Right - size.Width, rect.Bottom - size.Height, size.Width, size.Height, 0, 90);

            gp.AddLine(rect.Right - size.Width / 2, rect.Bottom, rect.Left + size.Width / 2, rect.Bottom);
            gp.AddArc(rect.Left, rect.Bottom - size.Height, size.Width, size.Height, 90, 90);

            gp.AddLine(rect.Left, rect.Bottom - size.Height / 2, rect.Left, rect.Top + size.Height / 2);
            gp.AddArc(rect.Left, rect.Top, size.Width, size.Height, 180, 90);
            return gp;
        }

        /// <summary>
        /// Desaturates colors from given array.
        /// </summary>
        /// <param name="colorsToDesaturate">The colors to be desaturated.</param>
        /// <returns></returns>
        public static Color[] DesaturateColors(params Color[] colorsToDesaturate)
        {
            Color[] colorsToReturn = new Color[colorsToDesaturate.Length];
            for (int i = 0; i < colorsToDesaturate.Length; i++)
            {
                //use NTSC weighted avarage
                int gray =
                    (int)(colorsToDesaturate[i].R * 0.3 + colorsToDesaturate[i].G * 0.6 + colorsToDesaturate[i].B * 0.1);
                colorsToReturn[i] = Color.FromArgb(-0x010101 * (255 - gray) - 1);
            }
            return colorsToReturn;
        }

        /// <summary>
        /// Lightens colors from given array.
        /// </summary>
        /// <param name="colorsToLighten">The colors to lighten.</param>
        /// <returns></returns>
        public static Color[] LightenColors(params Color[] colorsToLighten)
        {
            Color[] colorsToReturn = new Color[colorsToLighten.Length];
            for (int i = 0; i < colorsToLighten.Length; i++)
            {
                colorsToReturn[i] = ControlPaint.Light(colorsToLighten[i], 0.1f);
            }
            return colorsToReturn;
        }

        /// <summary>
        /// Sets the trackbar value so that it wont exceed allowed range.
        /// </summary>
        /// <param name="val">The value.</param>
        private void SetProperValue(int val)
        {
            if (val < _minimum) Value = _minimum;
            else if (val > _maximum) Value = _maximum;
            else Value = val;
        }

        /// <summary>
        /// Determines whether rectangle contains given point.
        /// </summary>
        /// <param name="pt">The point to test.</param>
        /// <param name="rect">The base rectangle.</param>
        /// <returns>
        /// 	<c>true</c> if rectangle contains given point; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsPointInRect(Point pt, Rectangle rect)
        {
            if (pt.X > rect.Left & pt.X < rect.Right & pt.Y > rect.Top & pt.Y < rect.Bottom)
                return true;
            else return false;
        }

        #endregion
    }
}
