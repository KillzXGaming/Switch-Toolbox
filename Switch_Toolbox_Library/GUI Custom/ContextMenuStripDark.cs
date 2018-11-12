using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Switch_Toolbox.Library.Forms
{
    public class ToolStripItemDark : ToolStripMenuItem
    {
        public ToolStripItemDark()
        {
        }
        public ToolStripItemDark(string Name)
        {
            this.Text = Name;
            this.ForeColor = Color.FromArgb(255, 255, 255);
        }
    }
    public class ContextMenuStripDark : MenuStrip
    {
        private static Color titlebarColor = Color.FromArgb(33, 33, 33);
        private static Color titlebarColorWhite = Color.FromArgb(255, 255, 255);

        public ContextMenuStripDark()
        {
            this.BackColor = titlebarColor;
            this.ForeColor = Color.FromArgb(255, 255, 255);

            this.Renderer = new ToolStripProfessionalRenderer(new ColorTable());

        }
        public class ColorTable : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground
            {
                get
                {
                    return titlebarColor;
                }
            }

            public override Color ImageMarginGradientBegin
            {
                get
                {
                    return titlebarColor;
                }
            }

            public override Color ImageMarginGradientMiddle
            {
                get
                {
                    return titlebarColor;
                }
            }

            public override Color ImageMarginGradientEnd
            {
                get
                {
                    return titlebarColor;
                }
            }

            public override Color MenuBorder
            {
                get
                {
                    return Color.White;
                }
            }

            public override Color MenuItemBorder
            {
                get
                {
                    return Color.Black;
                }
            }

            public override Color MenuItemSelected
            {
                get
                {
                    return Color.FromArgb(80, 80, 80);
                }
            }

            public override Color MenuStripGradientBegin
            {
                get
                {
                    return titlebarColor;
                }
            }

            public override Color MenuStripGradientEnd
            {
                get
                {
                    return titlebarColor;
                }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get
                {
                    return Color.FromArgb(80, 80, 80);
                }
            }

            public override Color MenuItemSelectedGradientEnd
            {
                get
                {
                    return Color.FromArgb(80, 80, 80);
                }
            }

            public override Color MenuItemPressedGradientBegin
            {
                get
                {
                    return titlebarColor;
                }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get
                {
                    return titlebarColor;
                }
            }
        }
    }
}