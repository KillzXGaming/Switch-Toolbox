using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirstPlugin.MuuntEditor
{
    public interface IDrawableObject
    {
        void Draw(Matrix4 mvp);
    }
}
