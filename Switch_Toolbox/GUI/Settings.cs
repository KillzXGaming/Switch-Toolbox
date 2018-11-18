using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using WeifenLuo.WinFormsUI.Docking;
using GL_Core;
using GL_Core.Public_Interfaces;
using GL_Core.Cameras;

namespace Switch_Toolbox
{
    public partial class Settings : Form
    {
        MainForm mainForm;

        public Settings(MainForm main)
        {
            mainForm = main;

            InitializeComponent();

            foreach (Runtime.ViewportShading shading in Enum.GetValues(typeof(Runtime.ViewportShading)))
            {
                shadingComboBox.Items.Add(shading.ToString());
            }
            foreach (Runtime.CameraMovement shading in Enum.GetValues(typeof(Runtime.CameraMovement)))
            {
                camMoveComboBox.Items.Add(shading.ToString());
            }

            chkBoxNormalMap.Checked = Runtime.useNormalMap;
            chkBoxDisplayModels.Checked = Runtime.RenderModels;
            chkBoxDisplayWireframe.Checked = Runtime.RenderModelWireframe;
            chkBoxSpecular.Checked = Runtime.renderSpecular;
            chkBoxStereoscopy.Checked = Runtime.stereoscopy;
            chkBoxDisplayPolyCount.Checked = Runtime.DisplayPolyCount;
            camNearNumUD.Value = (decimal)Runtime.CameraNear;
            camFarNumUD.Value = (decimal)Runtime.CameraFar;
            previewScaleUD.Value = (decimal)Runtime.previewScale;
            yazoCompressionLevelUD.Value = Runtime.Yaz0CompressionLevel;
            disableViewportCHKBX.Checked = Runtime.DisableViewport;

            GLSLVerLabel.Text   = $"Open GL Version: {Runtime.GLSLVersion}";
            openGLVerLabel.Text = $"GLSL Version:     {Runtime.openGLVersion}";

            shadingComboBox.SelectedIndex = (int)Runtime.viewportShading;
            camMoveComboBox.SelectedIndex = (int)Runtime.cameraMovement;
        }

        private void shadingComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Runtime.viewportShading = (Runtime.ViewportShading)shadingComboBox.SelectedIndex;
            Viewport.Instance.UpdateViewport();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.useNormalMap = chkBoxNormalMap.Checked;
            Viewport.Instance.UpdateViewport();
        }

        private void chkBoxDisplayModels_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.RenderModels = chkBoxDisplayModels.Checked;
            Viewport.Instance.UpdateViewport();
        }

        private void chkBoxDisplayWireframe_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.RenderModelWireframe = chkBoxDisplayWireframe.Checked;
            Viewport.Instance.LoadViewportRuntimeValues();
            Viewport.Instance.UpdateViewport();
        }

        private void camMoveComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Runtime.cameraMovement = (Runtime.CameraMovement)camMoveComboBox.SelectedIndex;
            Viewport.Instance.LoadViewportRuntimeValues();
            Viewport.Instance.UpdateViewport();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            Runtime.stereoscopy = chkBoxStereoscopy.Checked;
            Viewport.Instance.LoadViewportRuntimeValues();
            Viewport.Instance.UpdateViewport();
        }
        private void camNearNumUD_ValueChanged(object sender, EventArgs e)
        {
            Runtime.CameraNear = (float)camNearNumUD.Value;
            Viewport.Instance.LoadViewportRuntimeValues();
            Viewport.Instance.UpdateViewport();
        }

        private void camFarNumUD_ValueChanged(object sender, EventArgs e)
        {
            Runtime.CameraFar = (float)camFarNumUD.Value;
            Viewport.Instance.LoadViewportRuntimeValues();
            Viewport.Instance.UpdateViewport();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Viewport.Instance.UpdateViewport();

            Config.Save();
            this.Close();
        }

        private void chkBoxDisplayPolyCount_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.DisplayPolyCount = chkBoxDisplayPolyCount.Checked;
        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Runtime.previewScale = (float)previewScaleUD.Value;
            Viewport.Instance.LoadViewportRuntimeValues();
            Viewport.Instance.UpdateViewport();
        }

        private void yazoCompressionLevelUD_ValueChanged(object sender, EventArgs e)
        {
            Runtime.Yaz0CompressionLevel = (int)yazoCompressionLevelUD.Value;
        }

        private void checkBox1_CheckedChanged_2(object sender, EventArgs e)
        {
            Runtime.DisableViewport = disableViewportCHKBX.Checked;
        }
    }
}
