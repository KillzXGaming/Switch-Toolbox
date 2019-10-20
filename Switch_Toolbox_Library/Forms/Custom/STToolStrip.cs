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
    public class STToolStipMenuButton : ToolStripButton
    {
        public STToolStipMenuButton(string name, Image image, EventHandler eventHandler)
        {
            Text = name;
            Image = image;
            Click += eventHandler;
        }
    }

    public class STToolStrip : ToolStrip
    {
        public int SelectedTabIndex = -1;

        private static Color titlebarBackColor = FormThemes.BaseTheme.FormContextMenuBackColor;
        private static Color titlebarForeColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        private static Color borderColor = FormThemes.BaseTheme.FormContextMenuForeColor;
        private static Color borderItemColor = Color.Black;
        private static Color menuSelectItemColor = FormThemes.BaseTheme.FormContextMenuSelectColor;

        public bool HighlightSelectedTab
        {
            get
            {
                if (Renderer == null || !(Renderer is MenuRenderer))
                    return false;

                return ((MenuRenderer)this.Renderer).HighlightMenuBar;
            }
            set
            {
                if (Renderer != null && Renderer is MenuRenderer)
                    ((MenuRenderer)this.Renderer).HighlightMenuBar = value;
            }
        }

        public STToolStrip()
        {
            if (FormThemes.ActivePreset != FormThemes.Preset.White)
            {
                this.BackColor = titlebarBackColor;
                this.ForeColor = titlebarForeColor;
                this.Renderer = new MenuRenderer();
            }
        }

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            if (HighlightSelectedTab)
            {
                foreach (var item in Items)
                {
                    var toolStripBtn = item as ToolStripButton;
                    if (toolStripBtn != null)
                    {
                        if (toolStripBtn != e.ClickedItem)
                            toolStripBtn.Checked = false;
                        else
                            toolStripBtn.Checked = true;
                    }
                }
            }

            SelectedTabIndex = Items.IndexOf(e.ClickedItem);
        }

        public class MenuRenderer : ToolStripProfessionalRenderer
        {
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
            public bool HighlightMenuBar = false;

            private static Color checkedHighlightBackColor = FormThemes.BaseTheme.FormBackColor;
            private static Color uncheckedHighlightBackColor = FormThemes.BaseTheme.FormBackColor;

            public override Color ToolStripDropDownBackground
            {
                get
                {
                    return titlebarBackColor;
                }
            }

            public override Color ImageMarginGradientBegin => HighlightMenuBar ? uncheckedHighlightBackColor : titlebarBackColor;
            public override Color ImageMarginGradientMiddle => HighlightMenuBar ? uncheckedHighlightBackColor : titlebarBackColor;
            public override Color ImageMarginGradientEnd => HighlightMenuBar ? uncheckedHighlightBackColor : titlebarBackColor;
            public override Color MenuBorder => HighlightMenuBar ? uncheckedHighlightBackColor : borderItemColor;
            public override Color MenuItemBorder => HighlightMenuBar ? uncheckedHighlightBackColor : borderItemColor;

            public override Color MenuStripGradientBegin => HighlightMenuBar ? uncheckedHighlightBackColor : titlebarBackColor;
            public override Color MenuStripGradientEnd => HighlightMenuBar ? uncheckedHighlightBackColor : titlebarBackColor;

            public override Color MenuItemSelected => HighlightMenuBar ? checkedHighlightBackColor : menuSelectItemColor;

            public override Color ButtonSelectedBorder => HighlightMenuBar ? checkedHighlightBackColor : titlebarBackColor;

            public override Color MenuItemSelectedGradientBegin => HighlightMenuBar ? checkedHighlightBackColor : menuSelectItemColor;
            public override Color MenuItemSelectedGradientEnd => HighlightMenuBar ? checkedHighlightBackColor : menuSelectItemColor;

            public override Color MenuItemPressedGradientBegin => HighlightMenuBar ? checkedHighlightBackColor : titlebarBackColor;
            public override Color MenuItemPressedGradientEnd => HighlightMenuBar ? checkedHighlightBackColor : titlebarBackColor;

            public override Color ButtonCheckedGradientBegin => HighlightMenuBar ? checkedHighlightBackColor : titlebarBackColor;
            public override Color ButtonCheckedGradientEnd => HighlightMenuBar ? checkedHighlightBackColor : titlebarBackColor;
            public override Color ButtonCheckedGradientMiddle => HighlightMenuBar ? checkedHighlightBackColor : titlebarBackColor;
        }
    }
}