using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public interface IColorPanelCommon 
    {
        STColor SelectedColor { get; }
        void LoadColors(STColor[] colors, int keyCount);
        void UpdateSelectedColor();
        void SetColor(Color color);
        Color GetColor();
        event EventHandler ColorSelected;
        void DeselectPanel();
        bool IsAlpha { get; set; }
    }
}
