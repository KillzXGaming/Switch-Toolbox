using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.Interfaces;

namespace FirstPlugin.MuuntEditor
{
    public interface I3DDrawableContainer
    {
        List<AbstractGlDrawable> Drawables { get; set; }
    }
}
