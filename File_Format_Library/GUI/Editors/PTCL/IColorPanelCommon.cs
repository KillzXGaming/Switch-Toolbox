using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FirstPlugin.Forms
{
    public interface IColorPanelCommon 
    {
        void SetColor(Color color);
        Color GetColor();
        event EventHandler ColorSelected;
        void SelectPanel();
        void DeselectPanel();
        bool IsAlpha { get; set; }
    }
}
