using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Toolbox.Library;
using LibEveryFileExplorer.GFX;

namespace FirstPlugin
{
    public class BXFNT
    {
        public virtual string Name { get; set; }

        public virtual Bitmap GetBitmap(string text, bool reversewh, LayoutBXLYT.BasePane pane)
        {
            return new Bitmap(32,32);
        }

        public virtual BitmapFont GetBitmapFont(bool UseChannelComp = false)
        {
            return null;
        }
    }
}
