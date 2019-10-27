using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.OpenGL2D
{
    public enum PickAction
    {
        None,
        DragTopRight,
        DragTopLeft,
        DragTop,
        DragLeft,
        DragRight,
        DragBottom,
        DragBottomLeft,
        DragBottomRight,
        Translate,
        Scale,
        Rotate
    }

    public enum PickAxis
    {
        All,
        X,
        Y,
        Z,
    }
}
