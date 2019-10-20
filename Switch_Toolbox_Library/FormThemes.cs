using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.Forms
{
    public class FormThemes
    {
        public enum Preset
        {
            White,
            Dark,
            Custom,
        }
        private static Preset activePreset = Preset.Dark;
        public static Preset ActivePreset
        {
            set {
                activePreset = value;
                SetTheme();
            }
            get {
                return activePreset;
            }
        }

        public static void SetTheme()
        {
            if (activePreset == Preset.White)       SetWhiteTheme();
            else if (activePreset == Preset.Dark)   SetDarkTheme();
            else if (activePreset == Preset.Custom) SetCustomTheme();
        }
        private static void SetWhiteTheme()
        {
            BaseTheme.FormBackColor = SystemColors.Control;
            BaseTheme.FormForeColor = Color.Black;
            BaseTheme.MDIParentBackColor = Color.FromArgb(170, 170, 170);
            BaseTheme.FormContextMenuBackColor = SystemColors.Control;
            BaseTheme.FormContextMenuForeColor = Color.Black;
            BaseTheme.TabPageInactive = Color.FromArgb(220, 220, 220);
            BaseTheme.TabPageActive = Color.White;
            BaseTheme.MDIChildBorderColor = Color.FromArgb(230, 230, 230);
            BaseTheme.DisabledItemColor = Color.FromArgb(127, 127, 127);
            BaseTheme.DisabledBorderColor = Color.FromArgb(220, 220, 220);
            BaseTheme.TextForeColor = Color.Black;

            BaseTheme.ListViewBackColor = Color.White;

            BaseTheme.DropdownButtonBackColor = Color.FromArgb(220, 220, 220);
            BaseTheme.DropdownPanelBackColor = Color.White;

            BaseTheme.DropdownPanelBackColor = Color.White;

            BaseTheme.ObjectEditorBackColor = Color.White;
            BaseTheme.TextEditorBackColor = Color.White;
            BaseTheme.ConsoleEditorBackColor = Color.FromArgb(200, 200, 200);

            BaseTheme.CheckBoxBackColor = Color.FromArgb(190, 190, 190);
            BaseTheme.CheckBoxForeColor = Color.White;
            BaseTheme.CheckBoxEnabledBackColor = Color.FromArgb(83, 121, 180);
            BaseTheme.CheckBoxEnabledForeColor = Color.White;

            BaseTheme.ComboBoxBackColor = SystemColors.Control;
            BaseTheme.ComboBoxBorderColor = Color.White;
            BaseTheme.ComboBoxArrowColor = Color.FromArgb(170, 170, 170);

            BaseTheme.TimelineThumbColor = Color.Yellow;

            BaseTheme.ComboBoxArrowColor = Color.Black;

            BaseTheme.ValueBarSliderElapseTopColor = Color.FromArgb(140, 140, 140);
            BaseTheme.ValueBarSliderElapseMiddleColor = Color.FromArgb(140, 140, 140);
            BaseTheme.ValueBarSliderElapseBottmColor = Color.FromArgb(140, 140, 140);

            BaseTheme.ValueBarSliderTopColor = Color.FromArgb(140, 140, 140);
            BaseTheme.ValueBarSliderMiddleColor = Color.FromArgb(140, 140, 140);
            BaseTheme.ValueBarSliderBottmColor = Color.FromArgb(140, 140, 140);


            BaseTheme.ValueBarSliderElapseTopColor = Color.FromArgb(140, 140, 140);
            BaseTheme.ValueBarSliderElapseMiddleColor = Color.FromArgb(140, 140, 140);
            BaseTheme.ValueBarSliderElapseBottmColor = Color.FromArgb(140, 140, 140);
        }
        private static void SetDarkTheme()
        {
            BaseTheme.FormBackColor = Color.FromArgb(50, 50, 50);
            BaseTheme.FormForeColor = Color.FromArgb(240, 240, 240);
            BaseTheme.FormContextMenuBackColor = Color.FromArgb(50, 50, 50);
            BaseTheme.FormContextMenuForeColor = Color.White;
            BaseTheme.FormContextMenuSelectColor = Color.FromArgb(80, 80, 80);
            BaseTheme.MDIParentBackColor = Color.FromArgb(60, 60, 60);
            BaseTheme.MDIChildBorderColor = Color.FromArgb(40, 40, 40);
            BaseTheme.DisabledItemColor = Color.FromArgb(127, 127, 127);
            BaseTheme.TabPageInactive = Color.FromArgb(50, 50, 50);
            BaseTheme.DisabledBorderColor = Color.FromArgb(70, 70, 70);
            BaseTheme.ObjectEditorBackColor = Color.FromArgb(45, 45, 45);
            BaseTheme.TextEditorBackColor = Color.FromArgb(45, 45, 45);
            BaseTheme.ConsoleEditorBackColor = Color.FromArgb(35, 35, 35);
            BaseTheme.ListViewBackColor = Color.FromArgb(43, 43, 43);

            BaseTheme.DropdownPanelBackColor = Color.FromArgb(35, 35, 35);
            BaseTheme.DropdownButtonBackColor = Color.FromArgb(60, 60, 60);

            BaseTheme.CheckBoxBackColor = Color.FromArgb(40, 40, 40);
            BaseTheme.CheckBoxForeColor = Color.White;
            BaseTheme.CheckBoxEnabledBackColor = Color.FromArgb(83, 121, 180);
            BaseTheme.CheckBoxEnabledForeColor = Color.White;


            BaseTheme.ValueBarSliderElapseTopColor = Color.FromArgb(180, 180, 180);
            BaseTheme.ValueBarSliderElapseMiddleColor = Color.FromArgb(185, 185, 185);
            BaseTheme.ValueBarSliderElapseBottmColor = Color.FromArgb(180, 180, 180);

            BaseTheme.ValueBarSliderTopColor = Color.FromArgb(140, 140, 140);
            BaseTheme.ValueBarSliderMiddleColor = Color.FromArgb(145, 145, 145);
            BaseTheme.ValueBarSliderBottmColor = Color.FromArgb(150, 150, 150);


            BaseTheme.TimelineThumbColor = Color.Olive;

            BaseTheme.ComboBoxBackColor = Color.FromArgb(70, 70, 70);
            BaseTheme.ComboBoxArrowColor = Color.FromArgb(170, 170, 170);
            BaseTheme.ComboBoxBorderColor = Color.FromArgb(70, 70, 70);

            BaseTheme.ValueBarSliderElapseTopColor = Color.FromArgb(60, 60, 60);
            BaseTheme.ValueBarSliderElapseMiddleColor = Color.FromArgb(65, 65, 65);
            BaseTheme.ValueBarSliderElapseBottmColor = Color.FromArgb(60, 60, 60);

            BaseTheme.ValueBarSliderTopColor = Color.FromArgb(40, 40, 40);
            BaseTheme.ValueBarSliderMiddleColor = Color.FromArgb(45, 45, 45);
            BaseTheme.ValueBarSliderBottmColor = Color.FromArgb(50, 50, 50);

            BaseTheme.TabPageActive = Color.FromArgb(60, 60, 60);
            BaseTheme.TextForeColor = Color.White;
        }
        private static void SetCustomTheme()
        {

        }
        public class BaseTheme
        {
            public static Color ComboBoxBackColor { get; set; }
            public static Color ComboBoxArrowColor { get; set; }
            public static Color ComboBoxBorderColor { get; set; }

            public static Color CheckBoxBackColor { get; set; }
            public static Color CheckBoxForeColor { get; set; }
            public static Color CheckBoxEnabledForeColor { get; set; }
            public static Color CheckBoxEnabledBackColor { get; set; }

            public static Color ListViewBackColor { get; set; }

            public static Color ConsoleEditorBackColor { get; set; }

            public static Color TextEditorBackColor { get; set; }
            public static Color ObjectEditorBackColor { get; set; }
            public static Color DisabledBorderColor { get; set; }
            public static Color TabPageInactive { get; set; }
            public static Color TabPageActive { get; set; }
            public static Color DisabledItemColor { get; set; }
            public static Color MDIChildBorderColor { get; set; }
            public static Color MDIParentBackColor { get; set; }
            public static Color FormBackColor { get; set; }
            public static Color FormForeColor { get; set; }
            public static Color TextForeColor { get; set; }
            public static Color TextBackColor { get; set; }
            public static Color TreeViewBackColor { get; set; }
            public static Color TreeViewForeColor { get; set; }
            public static Color FormContextMenuBackColor { get; set; }
            public static Color FormContextMenuForeColor { get; set; }
            public static Color FormContextMenuSelectColor { get; set; }
            public static Color DropdownButtonBackColor { get; set; }
            public static Color DropdownPanelBackColor { get; set; }

            public static Color ValueBarSliderBackColor { get; set; }

            public static Color TimelineThumbColor { get; set; }

            public static Color ValueBarSliderElapseTopColor { get; set; }
            public static Color ValueBarSliderElapseMiddleColor { get; set; }
            public static Color ValueBarSliderElapseBottmColor { get; set; }

            public static Color ValueBarSliderTopColor { get; set; }
            public static Color ValueBarSliderMiddleColor { get; set; }
            public static Color ValueBarSliderBottmColor { get; set; }

            public static Font FontType { get; set; }
        }
    }
}
