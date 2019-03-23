using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.Rendering;
using System.Windows.Forms;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.StandardCameras;
using GL_EditorFramework.EditorDrawables;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Switch_Toolbox.Library
{
    public partial class Viewport : UserControl
    {
        public EditorScene scene = new EditorScene();
        public GL_ControlLegacy GL_ControlLegacy;
        public GL_ControlModern GL_ControlModern;

        Runtime.ViewportEditor editor;

        public Viewport()
        {
            this.DoubleBuffered = true;

            InitializeComponent();
            LoadViewport();
            LoadViewportRuntimeValues();
            LoadShadingModes();
            editor = new Runtime.ViewportEditor();
            Runtime.viewportEditors.Add(editor);

            foreach (var type in Enum.GetValues(typeof(Runtime.ViewportShading)).Cast<Runtime.ViewportShading>())
            {
                if (type == Runtime.viewportShading)
                    shadingToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(type.ToString()) { Checked = true });
                else
                    shadingToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(type.ToString()));
            }

            shadingToolStripMenuItem.Text = $"Shading: [{Runtime.viewportShading.ToString()}]";
        }

        private void shadingToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int i = 0;
            foreach (ToolStripMenuItem item in shadingToolStripMenuItem.DropDownItems)
            {
                if (item.Selected)
                {
                    item.Checked = true;

                    Runtime.viewportShading = (Runtime.ViewportShading)i;

                    shadingToolStripMenuItem.Text = $"Shading: [{item.Text}]";

                    UpdateViewport();
                }
                else
                {
                    item.Checked = false;
                }

                i++;
            }
        }

        public void LoadCustomMenuItem(ToolStripMenuItem menu)
        {
            foreach (ToolStripMenuItem item in stContextMenuStrip1.Items)
                if (item.Text == menu.Text)
                    return;

            if (!stContextMenuStrip1.Items.Contains(menu))
                stContextMenuStrip1.Items.Add(menu);
        }

        private void LoadViewport()
        {
            if (Runtime.UseLegacyGL)
            {
                GL_ControlLegacy = new GL_ControlLegacy();
                GL_ControlLegacy.Dock = DockStyle.Fill;

                panelViewport.Controls.Add(GL_ControlLegacy);
            }
            else
            {
                GL_ControlModern = new GL_ControlModern();
                GL_ControlModern.Dock = DockStyle.Fill;
                GL_ControlModern.VSync = true;

                panelViewport.Controls.Add(GL_ControlModern);
            }
        }

        public void UpdateGrid()
        {
            foreach (var obj in scene.staticObjects)
            {
                if (obj is DrawableFloor)
                    ((DrawableFloor)obj).UpdateVertexData();
            }
            UpdateViewport();
        }

        public void AddDrawable(AbstractGlDrawable Drawabale)
        {
            if (Drawabale is EditableObject)
                editor.editableDrawables.Add((EditableObject)Drawabale);
            else
                editor.staticDrawables.Add(Drawabale);
        }

        public void RemoveDrawable(AbstractGlDrawable Drawabale)
        {
            if (Drawabale is EditableObject)
                scene.objects.Remove((EditableObject)Drawabale);
            else
                scene.staticObjects.Remove(Drawabale);
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            var floor = new DrawableFloor();
            scene.staticObjects.Add(floor);
            var xyzLnes = new DrawableXyzLines();
            scene.staticObjects.Add(xyzLnes);

            bool UseSkybox = false;

            if (UseSkybox)
            {
                var skybox = new DrawableSkybox();
                scene.staticObjects.Add(skybox);
            }
            else if (Runtime.renderBackGround)
            {
                var background = new DrawableBackground();
                scene.staticObjects.Add(background);
            }

            LoadFog();

            Runtime.OpenTKInitialized = true;
        }

        public int FogStart = 1;

        public void LoadFog()
        {
        }

        public void LoadObjects()
        {
            foreach (var drawable in editor.editableDrawables)
            {
                scene.objects.Add(drawable);
            }
            foreach (var drawable in editor.staticDrawables)
                scene.staticObjects.Add(drawable);

            UpdateScene();
        }

        private void UpdateScene()
        {
            if (GL_ControlModern != null)
                GL_ControlModern.MainDrawable = scene;
            if (GL_ControlLegacy != null)
                GL_ControlLegacy.MainDrawable = scene;

        }

        public void UpdateViewport()
        {
            if (GL_ControlModern != null)
                GL_ControlModern.Refresh();
            if (GL_ControlLegacy != null)
                GL_ControlLegacy.Refresh();
        }
        public void RenderToTexture()
        {
            if (GL_ControlModern == null)
                return;

            int Framebuffer = 0;
        }
        private void LoadShadingModes()
        {
            foreach (var type in Enum.GetValues(typeof(Runtime.ViewportShading)).Cast<Runtime.ViewportShading>())
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = $"Shading: {type.ToString()}";
                item.TextImageRelation = TextImageRelation.ImageAboveText;

                switch (type)
                {
                    case Runtime.ViewportShading.Default:
                        item.Image = Properties.Resources.diffuseSphere;
                        break;
                    case Runtime.ViewportShading.Normal:
                        item.Image = Properties.Resources.normalsSphere;
                        break;
                    case Runtime.ViewportShading.NormalMap:
                        item.Image = Properties.Resources.normalMapSphere;
                        break;
                }
            }
        }
        public void LoadViewportRuntimeValues()
        {
            if (GL_ControlLegacy != null)
            {
                switch (Runtime.cameraMovement)
                {
                    case Runtime.CameraMovement.Inspect:
                        GL_ControlLegacy.ActiveCamera = new InspectCamera(Runtime.MaxCameraSpeed);
                        break;
                    case Runtime.CameraMovement.Walk:
                        GL_ControlLegacy.ActiveCamera = new WalkaroundCamera(Runtime.MaxCameraSpeed);
                        break;
                }
                GL_ControlLegacy.Stereoscopy = Runtime.stereoscopy;
                GL_ControlLegacy.ZNear = Runtime.CameraNear;
                GL_ControlLegacy.ZFar = Runtime.CameraFar;
            }
            else
            {
                switch (Runtime.cameraMovement)
                {
                    case Runtime.CameraMovement.Inspect:
                        GL_ControlModern.ActiveCamera = new InspectCamera(Runtime.MaxCameraSpeed);
                        break;
                    case Runtime.CameraMovement.Walk:
                        GL_ControlModern.ActiveCamera = new WalkaroundCamera(Runtime.MaxCameraSpeed);
                        break;
                }
                GL_ControlModern.Stereoscopy = Runtime.stereoscopy;
                GL_ControlModern.ZNear = Runtime.CameraNear;
                GL_ControlModern.ZFar = Runtime.CameraFar;
              
            }
        }
        public void SetupViewportRuntimeValues()
        {
            if (GL_ControlLegacy != null)
            {
                if (GL_ControlLegacy.ActiveCamera is InspectCamera)
                    Runtime.cameraMovement = Runtime.CameraMovement.Inspect;
                if (GL_ControlLegacy.ActiveCamera is WalkaroundCamera)
                    Runtime.cameraMovement = Runtime.CameraMovement.Walk;
                Runtime.stereoscopy = GL_ControlLegacy.Stereoscopy;
            }
            else
            {
                if (GL_ControlModern.ActiveCamera is InspectCamera)
                    Runtime.cameraMovement = Runtime.CameraMovement.Inspect;
                if (GL_ControlModern.ActiveCamera is WalkaroundCamera)
                    Runtime.cameraMovement = Runtime.CameraMovement.Walk;
                Runtime.stereoscopy = GL_ControlModern.Stereoscopy;
            }
        }

        private void contextMenuStripDark1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (animationPanelToolStripMenuItem.Checked)
            {
                animationPanelToolStripMenuItem.Checked = false;

            }
            else
            {
                animationPanelToolStripMenuItem.Checked = true;
            }
        }

        public void FormClosing()
        {
        }

        private void animationPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void resetCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GL_ControlLegacy != null)
            {
                GL_ControlLegacy.CamRotX = 0;
                GL_ControlLegacy.CamRotY = 0;
                GL_ControlLegacy.CameraTarget = new OpenTK.Vector3(0);
                GL_ControlLegacy.CameraDistance = -10f;
                GL_ControlLegacy.Refresh();
            }
            else
            {
                GL_ControlModern.CamRotX = 0;
                GL_ControlModern.CamRotY = 0;
                GL_ControlModern.CameraTarget = new OpenTK.Vector3(0);
                GL_ControlModern.CameraDistance = -10f;
                GL_ControlModern.Refresh();
            }
        }

        private void animationLoaderToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void reloadShadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GL_ControlModern != null)
                GL_ControlModern.ReloadShaders();
        }
    }
}
