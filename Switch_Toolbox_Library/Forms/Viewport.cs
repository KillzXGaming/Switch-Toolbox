using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using Toolbox.Library.Forms;
using Toolbox.Library.Rendering;
using System.Windows.Forms;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.StandardCameras;
using GL_EditorFramework.EditorDrawables;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Toolbox.Library
{
    public partial class Viewport : UserControl
    {
        public bool DisplayAll
        {
            get
            {
                return chkDisplayAllModels.Checked;
            }
            set
            {
                chkDisplayAllModels.Checked = value;
            }
        }

        public bool SuppressUpdating = false;

        public List<DrawableContainer> DrawableContainers;

        public EditorScene scene = new EditorScene();
        public GL_ControlBase GL_Control;

        Runtime.ViewportEditor editor;

        public void DisplayAllDDrawables() { drawContainersCB.SelectedIndex = 0; }

        public Viewport(List<DrawableContainer> container, bool LoadDrawables = true)
        {
            DrawableContainers = container;
            if (DrawableContainers == null)
                DrawableContainers = new List<DrawableContainer>();

            this.DoubleBuffered = true;

            InitializeComponent();

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;

            LoadViewport();
            LoadViewportRuntimeValues();
            LoadShadingModes();
            editor = new Runtime.ViewportEditor();
            Runtime.viewportEditors.Add(editor);

            perspectiveToolStripMenuItem.Checked = Runtime.ViewportCameraMode == Runtime.CameraMode.Perspective;
            orbitToolStripMenuItem.Checked = Runtime.cameraMovement == Runtime.CameraMovement.Inspect;
            walkToolStripMenuItem.Checked = Runtime.cameraMovement == Runtime.CameraMovement.Walk;

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

        public List<DrawableContainer> GetActiveContainers()
        {
            if (drawContainersCB.SelectedIndex < 0)
                return null;

            if (drawContainersCB.SelectedIndex == 0 || DisplayAll)
                return DrawableContainers;

            return new List<DrawableContainer>()
            { DrawableContainers[drawContainersCB.SelectedIndex - 1], };
        }

        //Reloads drawable containers with the active container selected
        public void ReloadDrawables(DrawableContainer ActiveContainer)
        {
            drawContainersCB.Items.Clear();
            drawContainersCB.Items.Add("All Active Drawables");

            for (int i = 0; i < DrawableContainers.Count; i++)
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
            UpdateViewport();
        }

        public bool ContainsDrawable(AbstractGlDrawable Drawable)
        {
            if (Drawable is GL_EditorFramework.EditorDrawables.IEditableObject)
            {
                return scene.staticObjects.Contains(Drawable) ||
                       scene.objects.Contains((GL_EditorFramework.EditorDrawables.IEditableObject)Drawable);
            }
            else
                return scene.staticObjects.Contains(Drawable);
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

        public Bitmap CaptureScreenshot(int width, int height, bool enableAlpha)
        {
            Bitmap bitmap = new Bitmap(width, height);
            return bitmap;
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
                GL_Control = new GL_ControlLegacy();
            else
                GL_Control = new GL_ControlModern();


            GL_Control.Dock = DockStyle.Fill;
            GL_Control.VSync = true;
            panelViewport.Controls.Add(GL_Control);
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
            Runtime.OpenTKInitialized = true;

            var floor = new DrawableFloor();
            var xyzLnes = new DrawableXyzLines();
            var skybox = new DrawableSkybox();
            var background = new DrawableBackground();

            scene.staticObjects.Add(floor);
            scene.staticObjects.Add(skybox);
            scene.staticObjects.Add(background);
            scene.staticObjects.Add(xyzLnes);
            //    scene.objects.Add(new SingleObject(new Vector3(0, 0, 0)));

            // LoadFog();

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

        private void UpdateScene() {
            if (GL_Control != null)
                GL_Control.MainDrawable = scene;
        }

        public void UpdateViewport()
        {
            if (SuppressUpdating) return;

            if (GL_Control != null)
                GL_Control.Refresh();
        }
        public void RenderToTexture()
        {
            if (GL_Control == null)
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
            if (GL_Control != null)
            {
                switch (Runtime.cameraMovement)
                {
                    case Runtime.CameraMovement.Inspect:
                        GL_Control.ActiveCamera = new InspectCamera(Runtime.MaxCameraSpeed);
                        break;
                    case Runtime.CameraMovement.Walk:
                        GL_Control.ActiveCamera = new WalkaroundCamera(Runtime.MaxCameraSpeed);
                        break;
                }
                GL_Control.Stereoscopy = Runtime.stereoscopy;
                GL_Control.ZNear = Runtime.CameraNear;
                GL_Control.ZFar = Runtime.CameraFar;
            }
        }
        public void SetupViewportRuntimeValues()
        {
            if (GL_Control != null)
            {
                if (GL_Control.ActiveCamera is InspectCamera)
                    Runtime.cameraMovement = Runtime.CameraMovement.Inspect;
                if (GL_Control.ActiveCamera is WalkaroundCamera)
                    Runtime.cameraMovement = Runtime.CameraMovement.Walk;
                Runtime.stereoscopy = GL_Control.Stereoscopy;
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
            if (GL_Control != null && GL_Control is GL_ControlModern)
                ((GL_ControlModern)GL_Control).ReloadShaders();
        }

        private void resetPoseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var animationPanel1 = LibraryGUI.GetAnimationPanel();

            if (animationPanel1 != null)
            {
                if (animationPanel1.CurrentAnimation != null ||
                  animationPanel1.CurrentSTAnimation != null)
                    animationPanel1.ResetModels();

                UpdateViewport();
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

            //    if (GL_ControlModern != null)
            //       GL_ControlModern.UseOrthographicView = IsOrtho;
            //   else
            //  GL_ControlLegacy.UseOrthographicView = IsOrtho;

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

            //     if (GL_ControlModern != null)
            //         GL_ControlModern.UseOrthographicView = IsOrtho;
            //     else
            //         GL_ControlLegacy.UseOrthographicView = IsOrtho;

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

            if (GL_Control != null)
                GL_Control.ApplyCameraOrientation(pickingBuffer);

            UpdateViewport();
        }

        private void toOriginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GL_Control != null) {
                GL_Control.ResetCamera(false);
                GL_Control.Refresh();
            }
        }

        private void toActiveModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GL_Control != null) {
                GL_Control.ResetCamera(true);
                GL_Control.Refresh();
            }
        }

        private void drawContainersCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chkDisplayAllModels.Checked)
            {
                DrawAllActive();
                return;
            }

            if (drawContainersCB.SelectedIndex == 0)
                DrawAllActive();
            else if (drawContainersCB.SelectedIndex > 0)
            {
                int index = drawContainersCB.SelectedIndex - 1;

                for (int i = 0; i < DrawableContainers.Count; i++)
                {
                    if (i == index)
                        CenterCamera(GL_Control, new List<DrawableContainer>() { DrawableContainers[i] });

                    for (int a = 0; a < DrawableContainers[i].Drawables.Count; a++)
                    {
                        if (i == index)
                            SetDrawableVisibilty(DrawableContainers[i].Drawables[a], true);
                        else
                            SetDrawableVisibilty(DrawableContainers[i].Drawables[a], false);
                    }
                }
            }

            UpdateViewport();
        }

        private void SetDrawableVisibilty(AbstractGlDrawable drawable, bool show)
        {
            if (drawable is EditableObject)
                ((EditableObject)drawable).Visible = show;
            else
                ((AbstractGlDrawable)drawable).Visible = show;
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

        private void chkDisplayAllModels_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDisplayAllModels.Checked)
            {
                drawContainersCB.SelectedIndex = 0;
                DrawAllActive();
            }
        }

        public void CenterCamera(GL_ControlBase control, List<DrawableContainer> Drawables)
        {
            if (!Runtime.FrameCamera)
                return;

            var spheres = new List<Vector4>();
            for (int i = 0; i < Drawables.Count; i++)
            {
                foreach (var drawable in Drawables[i].Drawables)
                {
                    if (drawable is IMeshContainer)
                    {
                        for (int m = 0; m < ((IMeshContainer)drawable).Meshes.Count; m++)
                        {
                            var mesh = ((IMeshContainer)drawable).Meshes[m];
                            var vertexPositions = mesh.vertices.Select(x => x.pos).Distinct();
                            spheres.Add(control.GenerateBoundingSphere(vertexPositions));
                        }
                    }
                }
            }

            if (spheres.Count > 0)
                control.FrameSelect(spheres);
        }

        private void uVViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Runtime.UseOpenGL)
                return;

            var containers = GetActiveContainers();
            if (containers?.Count == 0) return;

            UVEditorForm uvEditor1 = new UVEditorForm();
            uvEditor1.LoadEditor(containers);
            uvEditor1.Show(this);
        }

        public void SaveScreenshot()
        {
            if (GL_Control == null) return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = BitmapExtension.FileFilter;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                CreateScreenshot(GL_Control.Width, GL_Control.Height, false).Save(sfd.FileName);
            }
        }

        public Bitmap CreateScreenshot(int width, int height, bool useAlpha = false)
        {
            int imageSize = width * height * 4;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            byte[] output = new byte[imageSize];
            GL.ReadPixels(0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, output);

            var bitmap = BitmapExtension.GetBitmap(output, width, height);
            bitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
            return bitmap;
        }

        private void UpdateCameraMovement()
        {
            if (orbitToolStripMenuItem.Checked)
            {
                walkToolStripMenuItem.Checked = true;
                orbitToolStripMenuItem.Checked = false;

                Runtime.cameraMovement = Runtime.CameraMovement.Walk;
            }
            else
            {
                orbitToolStripMenuItem.Checked = true;
                walkToolStripMenuItem.Checked = false;

                Runtime.cameraMovement = Runtime.CameraMovement.Inspect;
            }

            if (GL_Control != null)
                GL_Control.ResetCamera(Runtime.FrameCamera);

            LoadViewportRuntimeValues();
            UpdateViewport();
        }

        private void walkToolStripMenuItem_Click(object sender, EventArgs e) {
            UpdateCameraMovement();
        }

        private void orbitToolStripMenuItem_Click(object sender, EventArgs e) {
            UpdateCameraMovement();
        }

        private void createScreenshotToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveScreenshot();
        }
    }
}
