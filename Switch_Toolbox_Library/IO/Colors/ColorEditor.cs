using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Toolbox.Library.Forms;

namespace Toolbox.Library.IO
{
    public class ColorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value.GetType() != typeof(STColor8) && 
                value.GetType() != typeof(STColor16) &&
                value.GetType() != typeof(STColor))
            {
                return value;
            }

            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (svc != null)
            {
                Color color = Color.White;
                if (value is STColor8)
                    color = ((STColor8)value).Color;
                if (value is STColor16)
                    color = ((STColor16)value).Color;
                if (value is STColor)
                    color = ((STColor)value).Color;

                using (ColorDialog form = new ColorDialog())
                {
                    form.Color = color;
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        if (value is STColor8)
                            ((STColor8)value).Color = form.Color;
                        if (value is STColor16)
                            ((STColor16)value).Color = form.Color;
                        if (value is STColor)
                            ((STColor)value).Color = form.Color;

                        return value;
                    }
                }

                //Todo custom dialog
                /*    using (STColorDialog form = new STColorDialog(color))
                     {
                         if (svc.ShowDialog(form) == DialogResult.OK)
                         {
                             ((STColor)value).Color = form.NewColor;
                             return value;
                         }
                     }*/
            }

            return value;
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

            using (SolidBrush brush = new SolidBrush(color))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            e.Graphics.DrawRectangle(Pens.Black, e.Bounds);
        }
    }
}
