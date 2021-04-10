using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.StandardCameras;
using Toolbox.Library;
using Toolbox.Library.Rendering;

namespace Toolbox.Library.Forms
{
    public partial class CubeMapFaceViewer3D : STForm
    {
        private GL_ControlBase glControl;
        public CubeMapFaceViewer3D()
        {
            InitializeComponent();

            gammaUD.Value = 2.2m;

            if (Runtime.UseLegacyGL)
                glControl = new GL_ControlLegacy();
            else
                glControl = new GL_ControlModern();

            glControl.Dock = DockStyle.Fill;
            glControl.ActiveCamera = new InspectCamera(Runtime.MaxCameraSpeed);
            glControl.Fov = 1.8f;
            stPanel1.Controls.Add(glControl);
        }

        private STGenericTexture ActiveTexture;
        private DrawableSkybox skybox = new DrawableSkybox();

        public void LoadTexture(STGenericTexture texture)
        {
            ActiveTexture = texture;

            skybox.ForceDisplay = true;
            skybox.LoadCustomTexture(ActiveTexture);
            glControl.MainDrawable = skybox;
        }

        private void encodeHDRAlphaChk_CheckedChanged(object sender, EventArgs e) {
            skybox.HDREncoded = encodeHDRAlphaChk.Checked;
            glControl?.Invalidate();
        }

        private void gammaUD_ValueChanged(object sender, EventArgs e) {
            skybox.Gamma = (float)gammaUD.Value;
            glControl?.Invalidate();
        }
    }
}
