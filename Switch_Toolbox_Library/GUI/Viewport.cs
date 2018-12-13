using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GL_Core;
using GL_Core.Public_Interfaces;
using GL_Core.Cameras;

namespace Switch_Toolbox.Library
{
    public partial class Viewport : DockContentST
    {
        public Viewport()
        {
            InitializeComponent();

            if (Runtime.DisableViewport)
                return;

            SetViewport();
            LoadViewportRuntimeValues();
            LoadShadingModes();
        }
        public GL_Core.GL_ControlModern gL_ControlModern1;
        private void SetViewport()
        {
            gL_ControlModern1 = new GL_ControlModern();
            gL_ControlModern1.ActiveCamera = null;
            gL_ControlModern1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            gL_ControlModern1.BackColor = System.Drawing.Color.Black;
            gL_ControlModern1.CurrentShader = null;
            gL_ControlModern1.DisplayPolyCount = false;
            gL_ControlModern1.Location = new System.Drawing.Point(-3, 66);
            gL_ControlModern1.MainDrawable = null;
            gL_ControlModern1.Name = "gL_ControlModern1";
            gL_ControlModern1.Size = new System.Drawing.Size(791, 388);
            gL_ControlModern1.Stereoscopy = false;
            gL_ControlModern1.TabIndex = 0;
            gL_ControlModern1.VSync = false;
            Controls.Add(this.gL_ControlModern1);
        }

        public static Viewport Instance
        {
            get { return _instance != null ? _instance : (_instance = new Viewport()); }
        }
        private static Viewport _instance;

        public GL_ControlLegacy GL_ControlLegacy;

        public void UpdateViewport()
        {
            if (Runtime.DisableViewport)
                return;

            if (gL_ControlModern1 != null)
                gL_ControlModern1.Refresh();
            if (GL_ControlLegacy != null)
                GL_ControlLegacy.Refresh();
        }
        public void RenderToTexture()
        {
            if (gL_ControlModern1 == null && !Runtime.DisableViewport)
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

                shadingToolStripMenuItem.DropDownItems.Add(item);
            }
        }
        public void LoadViewportRuntimeValues()
        {
            if (gL_ControlModern1 == null)
                return;

            switch (Runtime.cameraMovement)
            {
                case Runtime.CameraMovement.Inspect:
                    gL_ControlModern1.ActiveCamera = new InspectCamera();
                    break;
                case Runtime.CameraMovement.Walk:
                    gL_ControlModern1.ActiveCamera = new WalkaroundCamera();
                    break;
            }
            gL_ControlModern1.Stereoscopy = Runtime.stereoscopy;
            gL_ControlModern1.znear = Runtime.CameraNear;
            gL_ControlModern1.zfar = Runtime.CameraFar;
            gL_ControlModern1.DisplayPolyCount = Runtime.DisplayPolyCount;
            gL_ControlModern1.PolyCount = Runtime.PolyCount;
            gL_ControlModern1.VertCount = Runtime.VertCount;
        }
        public void SetupViewportRuntimeValues()
        {
            if (gL_ControlModern1.ActiveCamera is InspectCamera)
                Runtime.cameraMovement = Runtime.CameraMovement.Inspect;
            if (gL_ControlModern1.ActiveCamera is WalkaroundCamera)
                Runtime.cameraMovement = Runtime.CameraMovement.Walk;

            Runtime.stereoscopy = gL_ControlModern1.Stereoscopy;
            Runtime.DisplayPolyCount = gL_ControlModern1.DisplayPolyCount;
            Runtime.PolyCount = gL_ControlModern1.PolyCount;
            Runtime.VertCount = gL_ControlModern1.VertCount;
        }

        private void contextMenuStripDark1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void shadingToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int i = 0;
            foreach (ToolStripItem item in shadingToolStripMenuItem.DropDownItems)
            {
                if (item.Selected)
                {
                    Runtime.viewportShading = (Runtime.ViewportShading)i;

                    shadingToolStripMenuItem.Text = item.Text;
                    shadingToolStripMenuItem.Image = item.Image;

                    UpdateViewport();
                }
                i++;
            }
        }

        private void Viewport_FormClosing(object sender, FormClosingEventArgs e)
        {
            animationPanel1.ClosePanel();
        }
    }
}
