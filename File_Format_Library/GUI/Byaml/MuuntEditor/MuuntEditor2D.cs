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

        public override void RenderScene()
        {
            foreach (var col in ParentEditor.CollisionObjects)
                col.Renderer.Draw(Camera.ModelViewMatrix);

            foreach (var group in ParentEditor.Groups)
            {
                foreach (var subProp in group.Objects)
                    RenderGroupChildren(subProp);
            }
        }

        private void RenderGroupChildren(PropertyObject propertyObject)
        {
            if (propertyObject is I2DDrawableContainer)
            {
                foreach (var drawable in ((I2DDrawableContainer)propertyObject).Drawables)
                    drawable.Draw(Camera.ModelViewMatrix);
            }

            foreach (var subProperty in propertyObject.SubObjects)
                RenderGroupChildren(subProperty);
        }

        public override List<IPickable2DObject> GetPickableObjects()
        {
            List<IPickable2DObject> picks = new List<IPickable2DObject>();
            foreach (var group in ParentEditor.Groups)
            {
                foreach (var obj in group.Objects)
                    GetPickableSubObjects(obj, picks);
            }

            return picks;
        }

        private void GetPickableSubObjects(PropertyObject prob, List<IPickable2DObject> picks)
        {
            if (prob is IPickable2DObject)
                picks.Add((IPickable2DObject)prob);


            foreach (var subobj in prob.SubObjects)
                GetPickableSubObjects(subobj, picks);
        }
    }
}
