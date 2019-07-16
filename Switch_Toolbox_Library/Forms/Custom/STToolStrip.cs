using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Toolbox.Library.Forms
{
    public class STToolStrip : ToolStrip
    {
        private static Color titlebarBackColor = FormThemes.BaseTheme.FormContextMenuBackColor;
        private static Color titlebarForeColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        private static Color borderColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        private static Color borderItemColor = Color.Black;
        private static Color menuSelectItemColor = FormThemes.BaseTheme.FormContextMenuSelectColor;

        public STToolStrip()
        {
            this.BackColor = titlebarBackColor;

            if (FormThemes.ActivePreset != FormThemes.Preset.White)
            {
                this.BackColor = titlebarBackColor;
                this.ForeColor = titlebarForeColor;
                this.Renderer = new MenuRenderer();
            }
        }
        public class MenuRenderer : ToolStripProfessionalRenderer
        {
            public MenuRenderer() : base(new ColorTable())
            {

            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                //base.OnRenderToolStripBorder(e);
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                var tsMenuItem = e.Item as ToolStripMenuItem;
                if (tsMenuItem != null)
                    e.ArrowColor = Color.White;
                base.OnRenderArrow(e);
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = titlebarForeColor;
                base.OnRenderItemText(e);
            }
        }
        public class ColorTable : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground
            {
                get
                {
                    return titlebarBackColor;
                }
            }

            public override Color ImageMarginGradientBegin
            {
                get
                {
                    return titlebarBackColor;
                }
            }

            public override Color ImageMarginGradientMiddle
            {
                get
                {
                    return titlebarBackColor;
                }
            }

            public override Color ImageMarginGradientEnd
            {
                get
                {
                    return titlebarBackColor;
                }
            }

            public override Color MenuBorder
            {
                get
                {
                    return borderColor;
                }
            }

            public override Color MenuItemBorder
            {
                get
                {
                    return borderItemColor;
                }
            }

            public override Color MenuItemSelected
            {
                get
                {
                    return menuSelectItemColor;
                }
            }

            public override Color MenuStripGradientBegin
            {
                get
                {
                    return titlebarBackColor;
                }
            }

            public override Color MenuStripGradientEnd
            {
                get
                {
                    return titlebarBackColor;
                }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get
                {
                    return menuSelectItemColor;
                }
            }

            public override Color MenuItemSelectedGradientEnd
            {
                get
                {
                    return menuSelectItemColor;
                }
            }

            public override Color MenuItemPressedGradientBegin
            {
                get
                {
                    return titlebarBackColor;
                }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get
                {
                    return titlebarBackColor;
                }
            }
        }
    }
}