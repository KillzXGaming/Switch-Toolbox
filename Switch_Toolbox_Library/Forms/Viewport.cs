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
        public List<DrawableContainer> DrawableContainers;

        public EditorScene scene = new EditorScene();
        public GL_ControlLegacy GL_ControlLegacy;
        public GL_ControlModern GL_ControlModern;

        Runtime.ViewportEditor editor;

        public Viewport(List<DrawableContainer> container, bool LoadDrawables = true)
        {
            DrawableContainers = container;
            if (DrawableContainers == null)
                DrawableContainers = new List<DrawableContainer>();

            this.DoubleBuffered = true;

            InitializeComponent();
            LoadViewport();
            LoadViewportRuntimeValues();
            LoadShadingModes();
            editor = new Runtime.ViewportEditor();
            Runtime.viewportEditors.Add(editor);

            perspectiveToolStripMenuItem.Checked = Runtime.ViewportCameraMode == Runtime.CameraMode.Perspective;

            foreach (var type in Enum.GetValues(typeof(Runtime.ViewportShading)).Cast<Runtime.ViewportShading>())
            {
                if (type == Runtime.viewportShading)
                    shadingToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(type.ToString()) { Checked = true });
                else
                    shadingToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(type.ToString()));
            }

            shadingToolStripMenuItem.Text = $"Shading: [{Runtime.viewportShading.ToString()}]";


            if (LoadDrawables)
                LoadBaseDrawables();
        }

        //Reloads drawable containers without an active object being selected
        public void ReloadDrawables()
        {
            drawContainersCB.Items.Clear();
            drawContainersCB.Items.Add("All Active Drawables");

            for (int i = 0; i < DrawableContainers.Count; i++)
            {
                drawContainersCB.Items.Add(DrawableContainers[i].Name);

                for (int a = 0; a < DrawableContainers[i].Drawables.Count; a++)
                {
                    if (!ContainsDrawable(DrawableContainers[i].Drawables[a]))
                        AddDrawable((DrawableContainers[i].Drawables[a]));
                }
            }
        }

        //Reloads drawable containers with the active container selected
        public void ReloadDrawables(DrawableContainer ActiveContainer)
        {
            drawContainersCB.Items.Clear();
            drawContainersCB.Items.Add("All Active Drawables");

            for (int i = 0; i < DrawableContainers.Count;i++)
            {
                drawContainersCB.Items.Add(DrawableContainers[i].Name);

                for (int a = 0; a < DrawableContainers[i].Drawables.Count; a++)
                {
                    if (DrawableContainers[i] != ActiveContainer)
                        DrawableContainers[i].Drawables[a].Visible = false;
                    else
                        DrawableContainers[i].Drawables[a].Visible = true;

                    if (!ContainsDrawable(DrawableContainers[i].Drawables[a]))
                        AddDrawable((DrawableContainers[i].Drawables[a]));
                }
            }

            drawContainersCB.SelectItemByText(ActiveContainer.Name);
        }

        public bool ContainsDrawable(AbstractGlDrawable Drawable)
        {
            return scene.staticObjects.Contains(Drawable) || scene.objects.Contains(Drawable);
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
                scene.objects.Add((EditableObject)Drawabale);
            else
                scene.staticObjects.Add(Drawabale);

            UpdateScene();
        }

        public void RemoveDrawable(AbstractGlDrawable Drawabale)
        {
            if (Drawabale is EditableObject)
                scene.objects.Remove((EditableObject)Drawabale);
            else
                scene.staticObjects.Remove(Drawabale);
        }
        private void LoadBaseDrawables()
        {
            var floor = new DrawableFloor();
            scene.staticObjects.Add(floor);
            var xyzLnes = new DrawableXyzLines();
            scene.staticObjects.Add(xyzLnes);

            var skybox = new DrawableSkybox();
            scene.staticObjects.Add(skybox);

            var background = new DrawableBackground();
            scene.staticObjects.Add(background);

            //    scene.objects.Add(new SingleObject(new Vector3(0, 0, 0)));

            // LoadFog();

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

        }

        public void FormClosing()
        {
        }

        private void animationPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void animationLoaderToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void reloadShadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GL_ControlModern != null)
                GL_ControlModern.ReloadShaders();
        }

        private void resetPoseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var animationPanel1 = LibraryGUI.Instance.GetAnimationPanel();

            if (animationPanel1 != null)
            {
                if (animationPanel1.CurrentAnimation != null)
                    animationPanel1.ResetModels();
            }
        }

        private void perspectiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (perspectiveToolStripMenuItem.Checked)
            {
                perspectiveToolStripMenuItem.Checked = false;
                orthographicToolStripMenuItem.Checked = true;
            }
            else
            {
                orthographicToolStripMenuItem.Checked = false;
                perspectiveToolStripMenuItem.Checked = true;
            }

            bool IsOrtho = orthographicToolStripMenuItem.Checked;

            if (GL_ControlModern != null)
                GL_ControlModern.UseOrthographicView = IsOrtho;
            else
                GL_ControlLegacy.UseOrthographicView = IsOrtho;

            UpdateViewport();
        }

        private void orthographicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (orthographicToolStripMenuItem.Checked)
            {
                orthographicToolStripMenuItem.Checked = false;
                perspectiveToolStripMenuItem.Checked = true;
            }
            else
            {
                perspectiveToolStripMenuItem.Checked = false;
                orthographicToolStripMenuItem.Checked = true;
            }

            bool IsOrtho = orthographicToolStripMenuItem.Checked;

            if (GL_ControlModern != null)
                GL_ControlModern.UseOrthographicView = IsOrtho;
            else
                GL_ControlLegacy.UseOrthographicView = IsOrtho;

            UpdateViewport();
        }

        private enum CameraPickBuffer
        {
            Top = 1,
            Bottom = 2,
            Front = 3,
            Back = 4,
            Left = 5,
            Right = 6,
        }

        private void frontToolStripMenuItem_Click(object sender, EventArgs e) {
            ApplyCameraOrientation(CameraPickBuffer.Front);
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e) {
            ApplyCameraOrientation(CameraPickBuffer.Back);
        }

        private void topToolStripMenuItem_Click(object sender, EventArgs e) {
            ApplyCameraOrientation(CameraPickBuffer.Top);
        }

        private void bottomToolStripMenuItem_Click(object sender, EventArgs e) {
            ApplyCameraOrientation(CameraPickBuffer.Bottom);
        }

        private void rightToolStripMenuItem_Click(object sender, EventArgs e) {
            ApplyCameraOrientation(CameraPickBuffer.Right);
        }

        private void leftToolStripMenuItem_Click(object sender, EventArgs e) {
            ApplyCameraOrientation(CameraPickBuffer.Left);
        }

        private void ApplyCameraOrientation(CameraPickBuffer CameraPick)
        {
            int pickingBuffer = (int)CameraPick;

            if (GL_ControlModern != null)
                GL_ControlModern.ApplyCameraOrientation(pickingBuffer);
            else
                GL_ControlModern.ApplyCameraOrientation(pickingBuffer);

            UpdateViewport();
        }

        private void toOriginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GL_ControlLegacy != null)
            {
                GL_ControlLegacy.ResetCamera(false);
                GL_ControlLegacy.Refresh();
            }
            else
            {
                GL_ControlModern.ResetCamera(false);
                GL_ControlModern.Refresh();
            }
        }

        private void toActiveModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GL_ControlLegacy != null)
            {
                GL_ControlLegacy.ResetCamera(true);
                GL_ControlLegacy.Refresh();
            }
            else
            {
                GL_ControlModern.ResetCamera(true);
                GL_ControlModern.Refresh();
            }
        }

        private void drawContainersCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drawContainersCB.SelectedIndex == 0)
                DrawAllActive();
            else if (drawContainersCB.SelectedIndex > 0)
            {
                int index = drawContainersCB.SelectedIndex - 1;

                for (int i = 0; i < DrawableContainers.Count; i++)
                {
                    for (int a = 0; a < DrawableContainers[i].Drawables.Count; a++)
                    {
                        if (i == index)
                            DrawableContainers[i].Drawables[a].Visible = true;
                        else
                            DrawableContainers[i].Drawables[a].Visible = false;
                    }
                }
            }

            UpdateViewport();
        }

        private void DrawAllActive()
        {
            for (int i = 0; i < DrawableContainers.Count; i++)
            {
                for (int a = 0; a < DrawableContainers[i].Drawables.Count; a++)
                {
                    DrawableContainers[i].Drawables[a].Visible = true;
                }
            }
        }

        private void drawContainersCB_MouseDown(object sender, MouseEventArgs e)
        {
            ReloadDrawables();
        }
    }
}
