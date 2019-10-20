using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin.MuuntEditor
{
    public interface IDrawableContainer
    {
        IDrawableObject Drawable { get; }
    }
}
