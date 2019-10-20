using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using Toolbox.Library;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FirstPlugin.MuuntEditor
{
    public class MuuntEditor2D : Viewport2D
    {
        private MuuntEditor ParentEditor;

        public MuuntEditor2D(MuuntEditor editor)
        {
            ParentEditor = editor;
        }

        public override void RenderSceme()
        {
            foreach (var group in ParentEditor.Groups)
            {
                if (group is IDrawableContainer)
                    ((IDrawableContainer)group).Drawable?.Draw(Camera.ModelMatrix);
            }
        }

        public override List<IPickable2DObject> GetPickableObjects()
        {
            List<IPickable2DObject> picks = new List<IPickable2DObject>();
            foreach (var group in ParentEditor.Groups)
            {
                foreach (var obj in group.Objects)
                {
                    if (obj is IPickable2DObject)
                        picks.Add((IPickable2DObject)obj);

                    foreach (var subobj in obj.SubObjects)
                    {
                        if (subobj is IPickable2DObject)
                            picks.Add((IPickable2DObject)subobj);
                    }
                }
            }

            return picks;
        }
    }
}
