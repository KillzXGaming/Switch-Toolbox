using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using System.Drawing;

namespace LayoutBXLYT
{
    public class LytAnimEditorKeyGUI : STUserControl
    {
        public void SetValue(BarSlider.BarSlider numbericUD, float value, bool Keyed)
        {
            numbericUD.Value = value;
            if (Keyed)
            {
                numbericUD.BackColor = Color.FromArgb(255, 150, 106, 18);
                numbericUD.BarInnerColor = Color.FromArgb(255, 150, 106, 18);
                numbericUD.BarPenColorBottom = Color.FromArgb(255, 150, 106, 18);
                numbericUD.BarPenColorMiddle = Color.FromArgb(255, 150, 106, 18);
                numbericUD.BarPenColorTop = Color.FromArgb(255, 150, 106, 18);
                numbericUD.ElapsedInnerColor = Color.FromArgb(255, 150, 106, 18);
                numbericUD.ElapsedPenColorBottom = Color.FromArgb(255, 150, 106, 18);
                numbericUD.ElapsedPenColorMiddle = Color.FromArgb(255, 150, 106, 18);
                numbericUD.ElapsedPenColorTop = Color.FromArgb(255, 150, 106, 18);
            }
            else
            {
                numbericUD.BackColor = FormThemes.BaseTheme.ComboBoxBackColor;
                numbericUD.BarInnerColor = FormThemes.BaseTheme.ComboBoxBackColor;
                numbericUD.BarPenColorBottom = FormThemes.BaseTheme.ComboBoxBackColor;
                numbericUD.BarPenColorMiddle = FormThemes.BaseTheme.ComboBoxBackColor;
                numbericUD.BarPenColorTop = FormThemes.BaseTheme.ComboBoxBackColor;
                numbericUD.ElapsedInnerColor = FormThemes.BaseTheme.ComboBoxBackColor;
                numbericUD.ElapsedPenColorBottom = FormThemes.BaseTheme.ComboBoxBackColor;
                numbericUD.ElapsedPenColorMiddle = FormThemes.BaseTheme.ComboBoxBackColor;
                numbericUD.ElapsedPenColorTop = FormThemes.BaseTheme.ComboBoxBackColor;
            }
        }
    }
}
