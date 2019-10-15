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

        public bool HighlightSelectedTab
        {
            get {
                if (Renderer == null || !(Renderer is MenuRenderer))
                    return false;

                return ((MenuRenderer)this.Renderer).HighlightMenuBar;
            }
            set {
                if (Renderer != null && (Renderer is MenuRenderer))
                    ((MenuRenderer)this.Renderer).HighlightMenuBar = value;
            }
        }

        public STMenuStrip()
        {
            if (FormThemes.ActivePreset != FormThemes.Preset.White)
            {
                this.BackColor = titlebarBackColor;
                this.ForeColor = titlebarForeColor;
                this.Renderer = new MenuRenderer();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
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

        public STContextMenuStrip(IContainer container)
        {
                 this.ForeColor = FormThemes.BaseTheme.FormContextMenuForeColor;

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

        public bool HighlightMenuBar
        {
            get { return ((ColorTable)ColorTable).HighlightMenuBar; }
            set { ((ColorTable)ColorTable).HighlightMenuBar = value; }
        }

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
        public bool HighlightMenuBar = false;

        private static Color titlebarBackColor = FormThemes.BaseTheme.FormContextMenuBackColor;
        private static Color titlebarForeColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        private static Color borderColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        private static Color borderItemColor = Color.Black;
        private static Color menuSelectItemColor = FormThemes.BaseTheme.FormContextMenuSelectColor;
        private static Color checkedHighlightBackColor = FormThemes.BaseTheme.CheckBoxEnabledBackColor;


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

        public override Color MenuItemSelectedGradientBegin => HighlightMenuBar ? checkedHighlightBackColor : menuSelectItemColor;
        public override Color MenuItemSelectedGradientEnd => HighlightMenuBar ? checkedHighlightBackColor : menuSelectItemColor;


        public override Color MenuItemPressedGradientBegin => HighlightMenuBar ? checkedHighlightBackColor : titlebarBackColor;
        public override Color MenuItemPressedGradientEnd => HighlightMenuBar ? checkedHighlightBackColor : titlebarBackColor;

        public override Color ButtonCheckedGradientBegin => HighlightMenuBar ? checkedHighlightBackColor : titlebarBackColor;
        public override Color ButtonCheckedGradientEnd => HighlightMenuBar ? checkedHighlightBackColor : titlebarBackColor;
        public override Color ButtonCheckedGradientMiddle => HighlightMenuBar ? checkedHighlightBackColor : titlebarBackColor;
    }
}