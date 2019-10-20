using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FirstPlugin.MuuntEditor
{
    public class MapObject : PropertyObject
    {
        public IDrawableObject Drawable { get; set; }
    }
}
