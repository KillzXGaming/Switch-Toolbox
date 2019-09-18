using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Toolbox.Library.Forms;
using System.Drawing.Drawing2D;

namespace Toolbox.Library.IO
{
    public class ColorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        IWindowsFormsEditorService _service;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value.GetType() != typeof(STColor8) &&
                value.GetType() != typeof(STColor16) &&
                value.GetType() != typeof(STColor))
            {
                return value;
            }

            _service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (_service == null)
                return null;

            var control = new ColorSelectorDropdown();
            control.ColorApplied += ColorSelectorClose;

            Color color = Color.White;
            if (value is STColor8)
                color = ((STColor8)value).Color;
            if (value is STColor16)
                color = ((STColor16)value).Color;
            if (value is STColor)
                color = ((STColor)value).Color;

            control.Color = color;
            control.Alpha = color.A;

            _service.DropDownControl(control);

            if (value is STColor8)
                return control.Color8;
            if (value is STColor16)
                return control.Color16;
            if (value is STColor)
                return control.Color32;

            return value;
        }

        private void ColorSelectorClose(object sender, EventArgs e)
        {
            if (_service != null)
                _service.CloseDropDown();
        }

        private void OnColorChanged(object sender, EventArgs e)
        {

        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            Color color = Color.White;
            if (e.Value is STColor8)
                color = ((STColor8)e.Value).Color;
            if (e.Value is STColor16)
                color = ((STColor16)e.Value).Color;
            if (e.Value is STColor)
                color = ((STColor)e.Value).Color;


            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

            Brush RGBColor = new SolidBrush(Color.FromArgb(255, color.R, color.G, color.B));
            Brush AlphaColor = new SolidBrush(Color.FromArgb(color.A, color.R, color.G, color.B));

            Point rgbPos = new Point(e.Bounds.X, e.Bounds.Y);
            Point alphaPos = new Point(e.Bounds.X + e.Bounds.Width / 2, e.Bounds.Y);

            e.Graphics.FillRectangle(RGBColor, new RectangleF(rgbPos.X, rgbPos.Y, e.Bounds.Width / 2, e.Bounds.Height));
            e.Graphics.FillRectangle(AlphaColor, new RectangleF(alphaPos.X, alphaPos.Y, e.Bounds.Width / 2, e.Bounds.Height));

            e.Graphics.DrawRectangle(Pens.Black, e.Bounds);
        }
    }
}
