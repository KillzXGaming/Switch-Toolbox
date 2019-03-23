using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Switch_Toolbox.Library.Forms
{
    public class STToolStripItem : ToolStripMenuItem
    {
        public STToolStripItem()
        {
            this.ForeColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        }
        public STToolStripItem(string Name, System.EventHandler eventHandler)
        {
            this.Text = Name;
            this.Click += eventHandler;
        }
        public STToolStripItem(string Name)
        {
            this.Text = Name;
        }
    }

    public class STMenuStrip : MenuStrip
    {
        private static Color titlebarBackColor = FormThemes.BaseTheme.FormContextMenuBackColor;
        private static Color titlebarForeColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        private static Color borderColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        private static Color borderItemColor = Color.Black;
        private static Color menuSelectItemColor = FormThemes.BaseTheme.FormContextMenuSelectColor;

        public STMenuStrip()
        {
            if (FormThemes.ActivePreset != FormThemes.Preset.White)
            {
                this.BackColor = titlebarBackColor;
                this.ForeColor = titlebarForeColor;
                this.Renderer = new MenuRenderer();
            }
        }
    }

    public class STContextMenuStrip : ContextMenuStrip
    {
        private static Color titlebarBackColor = FormThemes.BaseTheme.FormContextMenuBackColor;
        private static Color titlebarForeColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        private static Color borderColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        private static Color borderItemColor = Color.Black;
        private static Color menuSelectItemColor = FormThemes.BaseTheme.FormContextMenuSelectColor;

        public STContextMenuStrip()
        {
            if (FormThemes.ActivePreset != FormThemes.Preset.White)
            {
                this.BackColor = titlebarBackColor;
                this.ForeColor = titlebarForeColor;
                this.Renderer = new MenuRenderer();
            }
        }
    }

    public class MenuRenderer : ToolStripProfessionalRenderer
    {
        private static Color titlebarForeColor = FormThemes.BaseTheme.FormContextMenuForeColor;

        public MenuRenderer() : base(new ColorTable())
        {

        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip.GetType() == typeof(ToolStrip))
            {
                // skip render border
            }
            else
            {
                // do render border
                base.OnRenderToolStripBorder(e);
            }
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
        private static Color titlebarBackColor = FormThemes.BaseTheme.FormContextMenuBackColor;
        private static Color titlebarForeColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        private static Color borderColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        private static Color borderItemColor = Color.Black;
        private static Color menuSelectItemColor = FormThemes.BaseTheme.FormContextMenuSelectColor;


        public override Color ToolStripDropDownBackground => titlebarBackColor;
        public override Color ToolStripBorder => titlebarBackColor;
        public override Color ToolStripContentPanelGradientBegin => titlebarBackColor;
        public override Color ToolStripContentPanelGradientEnd => titlebarBackColor;
        public override Color ToolStripPanelGradientBegin => titlebarBackColor;
        public override Color ToolStripPanelGradientEnd => titlebarBackColor;
        public override Color ToolStripGradientBegin => titlebarBackColor;
        public override Color ToolStripGradientMiddle => titlebarBackColor;
        public override Color ToolStripGradientEnd => titlebarBackColor;

        public override Color ImageMarginGradientBegin => titlebarBackColor;
        public override Color ImageMarginGradientMiddle => titlebarBackColor;
        public override Color ImageMarginGradientEnd => titlebarBackColor;

        public override Color MenuBorder => borderColor;
        public override Color MenuItemBorder => borderItemColor;
        public override Color MenuItemSelected => menuSelectItemColor;

        public override Color MenuStripGradientBegin => titlebarBackColor;
        public override Color MenuStripGradientEnd => titlebarBackColor;

        public override Color MenuItemSelectedGradientBegin => menuSelectItemColor;
        public override Color MenuItemSelectedGradientEnd => menuSelectItemColor;

        public override Color MenuItemPressedGradientBegin => titlebarBackColor;
        public override Color MenuItemPressedGradientEnd => titlebarBackColor;

    }
}