using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.GL_Core;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.Rendering;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.Interfaces;
using OpenTK;

namespace FirstPlugin.MuuntEditor
{
    public class MuuntEditor3D : STUserControl
    {
        public MuuntEditor ParentEditor;

        public GL_ControlBase gl_Control;

        private EditorScene scene;
        private bool loaded = false;

        public MuuntEditor3D(MuuntEditor muuntEditor)
        {
            ParentEditor = muuntEditor;

            if (Runtime.UseLegacyGL)
                gl_Control = new GL_ControlLegacy();
            else
                gl_Control = new GL_ControlModern();

            gl_Control.Dock = System.Windows.Forms.DockStyle.Fill;
            Controls.Add(gl_Control);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            scene = new EditorScene();
         //   LoadBaseDrawables();

            scene.objects.Add(new TransformableObject(new Vector3(), Quaternion.Identity, new Vector3(1)));

            foreach (var col in ParentEditor.CollisionObjects)
                AddDrawable(col.CollisionFile.DrawableContainer.Drawables[0]);

            foreach (var group in ParentEditor.Groups)
            {
                foreach (var subProp in group.Objects)
                    AddGroupChildren(subProp);
            }

            gl_Control.MainDrawable = scene;
            gl_Control.ActiveCamera = new GL_EditorFramework.StandardCameras.InspectCamera(1f);
            gl_Control.ZFar = 100000;
            gl_Control.ZNear = 0.1f;
            gl_Control.CameraDistance = 20;
            scene.ObjectsMoved += Scene_ObjectsMoved;

            loaded = true;
        }

        private void Scene_ObjectsMoved(object sender, EventArgs e)
        {

        }

        private void AddGroupChildren(PropertyObject propertyObject)
        {
            if (propertyObject is I3DDrawableContainer){
                foreach (var drawable in ((I3DDrawableContainer)propertyObject).Drawables)
                    AddDrawable(drawable);
            }

            foreach (var subProperty in propertyObject.SubObjects)
                AddGroupChildren(subProperty);
        }

        public void AddDrawable(AbstractGlDrawable drawable)
        {
            Console.WriteLine($"drawable " + drawable);
            if (drawable is IEditableObject)
                scene.objects.Add((IEditableObject)drawable);
            else
                scene.staticObjects.Add(drawable);

            if (loaded)
                UpdateScene();
        }

        private void UpdateScene() {
            gl_Control.MainDrawable = scene;
        }

        public void UpdateViewport()
        {
            gl_Control.Invalidate();
        }

        private void LoadBaseDrawables()
        {
            Runtime.OpenTKInitialized = true;

            var floor = new DrawableFloor();
            var xyzLnes = new DrawableXyzLines();
            var skybox = new DrawableSkybox();
            var background = new DrawableBackground();

            scene.staticObjects.Add(floor);
            scene.staticObjects.Add(xyzLnes);
            scene.staticObjects.Add(skybox);
            scene.staticObjects.Add(background);
        }
    }
}
