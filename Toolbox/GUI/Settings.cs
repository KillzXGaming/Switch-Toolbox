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
using Switch_Toolbox.Library.Forms;

namespace Toolbox
{
    public partial class Settings : STForm
    {
        private bool IsStartup = false;
        public Settings()
        {
            CanResize = false;
            IsStartup = true;

            InitializeComponent();
            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            Text = "Settings";

            foreach (Runtime.ViewportShading shading in Enum.GetValues(typeof(Runtime.ViewportShading)))
            {
                shadingComboBox.Items.Add(shading.ToString());
            }
            foreach (Runtime.CameraMovement shading in Enum.GetValues(typeof(Runtime.CameraMovement)))
            {
                camMoveComboBox.Items.Add(shading.ToString());
            }
            foreach (FormThemes.Preset themes in Enum.GetValues(typeof(FormThemes.Preset)))
            {
                formThemeCB.Items.Add(themes);
            }
            foreach (TEX_FORMAT format in Enum.GetValues(typeof(TEX_FORMAT)))
            {
                preferredTexFormatCB.Items.Add(format);
            }

            normalsLineUD.Value = (decimal)Runtime.normalsLineLength;
            normalPointsCB.Checked = Runtime.renderNormalsPoints;
            vtxColorCB.Checked = Runtime.renderVertColor;
            gridColorPB.BackColor = Runtime.gridSettings.color;
            gridCellCountUD.Value = Runtime.gridSettings.CellAmount;
            gridCellSizeUD.Value = (decimal)Runtime.gridSettings.CellSize;
            bgGradientTop.BackColor = Runtime.backgroundGradientTop;
            bgGradientBottom.BackColor = Runtime.backgroundGradientBottom;
            preferredTexFormatCB.SelectedItem = Runtime.PreferredTexFormat;
            formThemeCB.SelectedItem = FormThemes.ActivePreset;
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
            enableVersionCheckCHK.Checked = Runtime.EnableVersionCheck;
            chkBoxEnablePBR.Checked = Runtime.EnablePBR;
            chkBoxMdiMaximized.Checked = Runtime.MaximizeMdiWindow;
            chkBoxDisplayBones.Checked = Runtime.renderBones;
            boneSizeUD.Value = (decimal)Runtime.bonePointSize;
            cameraMaxSpeedUD.Value = (decimal)Runtime.MaxCameraSpeed;
            boneXRayChk.Checked = Runtime.boneXrayDisplay;
            mk8DPathTB.Text = Runtime.Mk8dGamePath;
            mk8PathTB.Text = Runtime.Mk8GamePath;
            SMOPathTB.Text = Runtime.SmoGamePath;
            displayBoundingBoxeChk.Checked = Runtime.renderBoundingBoxes;

            mk8DPathTB.ReadOnly = true;
            mk8PathTB.ReadOnly = true;
            SMOPathTB.ReadOnly = true;

            GLSLVerLabel.Text   = $"Open GL Version: {Runtime.GLSLVersion}";
            openGLVerLabel.Text = $"GLSL Version:     {Runtime.openGLVersion}";

            shadingComboBox.SelectedIndex = (int)Runtime.viewportShading;
            camMoveComboBox.SelectedIndex = (int)Runtime.cameraMovement;

            IsStartup = false;
        }

        private void shadingComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Runtime.viewportShading = (Runtime.ViewportShading)shadingComboBox.SelectedIndex;
            UpdateViewportSettings();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.useNormalMap = chkBoxNormalMap.Checked;
            UpdateViewportSettings();
        }

        private void chkBoxDisplayModels_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.RenderModels = chkBoxDisplayModels.Checked;
            UpdateViewportSettings();
        }

        private void chkBoxDisplayWireframe_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.RenderModelWireframe = chkBoxDisplayWireframe.Checked;
            UpdateViewportSettings();
        }

        private void camMoveComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Runtime.cameraMovement = (Runtime.CameraMovement)camMoveComboBox.SelectedIndex;
            UpdateViewportSettings(true);
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            Runtime.stereoscopy = chkBoxStereoscopy.Checked;
            UpdateViewportSettings(true);
        }
        private void camNearNumUD_ValueChanged(object sender, EventArgs e)
        {
            Runtime.CameraNear = (float)camNearNumUD.Value;
            UpdateViewportSettings();
        }

        private void camFarNumUD_ValueChanged(object sender, EventArgs e)
        {
            Runtime.CameraFar = (float)camFarNumUD.Value;
            UpdateViewportSettings();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
         //   UpdateViewportSettings();

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
            UpdateViewportSettings();
        }

        private void yazoCompressionLevelUD_ValueChanged(object sender, EventArgs e)
        {
            Runtime.Yaz0CompressionLevel = (int)yazoCompressionLevelUD.Value;
        }

        private void checkBox1_CheckedChanged_2(object sender, EventArgs e)
        {
            Runtime.EnableVersionCheck = enableVersionCheckCHK.Checked;
        }

        private void chkBoxEnablePBR_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.EnablePBR = chkBoxEnablePBR.Checked;
            UpdateViewportSettings();
        }

        private void UpdateViewportSettings(bool UpdateRuntimeValues = false)
        {
            if (IsStartup)
                return;

            Viewport viewport = LibraryGUI.Instance.GetActiveViewport();
            if (viewport == null)
                return;

            if (UpdateRuntimeValues) //Update only if necessary since it can be slow
                viewport.LoadViewportRuntimeValues();

            viewport.UpdateViewport();
        }

        private void formThemeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (formThemeCB.SelectedIndex >= 0 && !IsStartup)
            {
               var result = MessageBox.Show("Changing themes will require to restart the program. Do you want to restart now?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

               FormThemes.ActivePreset =  (FormThemes.Preset)formThemeCB.SelectedItem;

                if (result == DialogResult.Yes)
                {
                    Config.Save();
                    this.Close();

                    Application.Restart();
                    Environment.Exit(0);
                }
            }
        }

        private void chkBoxMdiMaximized_CheckedChanged_3(object sender, EventArgs e)
        {
            Runtime.MaximizeMdiWindow = chkBoxMdiMaximized.Checked;
        }

        private void preferredTexFormatCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (preferredTexFormatCB.SelectedIndex >= 0)
            {
                Runtime.PreferredTexFormat = (TEX_FORMAT)preferredTexFormatCB.SelectedItem;
            }
        }

        private void bgGradientTop_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
                Runtime.backgroundGradientTop = dlg.Color;

            bgGradientTop.BackColor = Runtime.backgroundGradientTop;
            UpdateViewportSettings();
        }

        private void bgGradientBottom_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
                Runtime.backgroundGradientBottom = dlg.Color;

            bgGradientBottom.BackColor = Runtime.backgroundGradientBottom;
            UpdateViewportSettings();
        }

        #region Grid Settings

        private void girdCellSizeUD_ValueChanged(object sender, EventArgs e)
        {
            Runtime.gridSettings.CellSize = (float)gridCellSizeUD.Value;
            UpdateViewportGrid();
        }

        private void gridCellCountUD_ValueChanged(object sender, EventArgs e)
        {
            Runtime.gridSettings.CellAmount = (uint)gridCellCountUD.Value;
            UpdateViewportGrid();
        }

        private void gridColorPB_Click(object sender, EventArgs e)
        {
             ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
                Runtime.gridSettings.color = dlg.Color;

            gridColorPB.BackColor = Runtime.gridSettings.color;
            UpdateViewportGrid();
        }

        private void UpdateViewportGrid()
        {
            if (IsStartup)
                return;

            Viewport viewport = LibraryGUI.Instance.GetActiveViewport();
            if (viewport == null)
                return;

            viewport.UpdateGrid();
        }

        #endregion

        private void vtxColorCB_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.renderVertColor = vtxColorCB.Checked;
            UpdateViewportSettings();
        }

        private void normalPointsCB_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.renderNormalsPoints = normalPointsCB.Checked;
            UpdateViewportSettings();
        }

        private void normalsLineUD_ValueChanged(object sender, EventArgs e)
        {
            Runtime.normalsLineLength = (float)normalsLineUD.Value;
            UpdateViewportSettings();
        }

        private void chkBoxDisplayBones_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.renderBones = chkBoxDisplayBones.Checked;
            UpdateViewportSettings();
        }

        private void boneSizeUD_ValueChanged(object sender, EventArgs e)
        {
            Runtime.bonePointSize = (float)boneSizeUD.Value;
            UpdateViewportSettings();
        }

        private void redChannelBtn_Click(object sender, EventArgs e) {
            if (Runtime.renderR) {
                Runtime.renderR = false;
                redChannelBtn.BackColor = FormThemes.BaseTheme.DisabledBorderColor;
                redChannelBtn.ForeColor = FormThemes.BaseTheme.DisabledItemColor;
            }
            else {
                Runtime.renderR = true;
                redChannelBtn.BackColor = Color.FromArgb(192, 0, 0);
                redChannelBtn.ForeColor = FormThemes.BaseTheme.FormForeColor;
            }
            UpdateViewportSettings();
        }

        private void greenChannelBtn_Click(object sender, EventArgs e) {
            if (Runtime.renderG)
            {
                Runtime.renderG = false;
                greenChannelBtn.BackColor = FormThemes.BaseTheme.DisabledBorderColor;
                greenChannelBtn.ForeColor = FormThemes.BaseTheme.DisabledItemColor;
            }
            else
            {
                Runtime.renderG = true;
                greenChannelBtn.BackColor = Color.FromArgb(0, 192, 0);
                greenChannelBtn.ForeColor = FormThemes.BaseTheme.FormForeColor;
            }
            UpdateViewportSettings();
        }

        private void blueChannelBtn_Click(object sender, EventArgs e) {
            if (Runtime.renderB)
            {
                Runtime.renderB = false;
                blueChannelBtn.BackColor = FormThemes.BaseTheme.DisabledBorderColor;
                blueChannelBtn.ForeColor = FormThemes.BaseTheme.DisabledItemColor;
            }
            else
            {
                Runtime.renderB = true;
                blueChannelBtn.BackColor = Color.FromArgb(0, 0, 192);
                blueChannelBtn.ForeColor = FormThemes.BaseTheme.FormForeColor;
            }
            UpdateViewportSettings();
        }

        private void alphaChannelBtn_Click(object sender, EventArgs e) {
            if (Runtime.renderAlpha)
            {
                Runtime.renderAlpha = false;
                alphaChannelBtn.BackColor = FormThemes.BaseTheme.DisabledBorderColor;
                alphaChannelBtn.ForeColor = FormThemes.BaseTheme.DisabledItemColor;
            }
            else
            {
                Runtime.renderAlpha = true;
                alphaChannelBtn.BackColor = Color.Silver;
                alphaChannelBtn.ForeColor = FormThemes.BaseTheme.FormForeColor;
            }
            UpdateViewportSettings();
        }

        private void cameraMaxSpeedUD_ValueChanged(object sender, EventArgs e) {
            Runtime.MaxCameraSpeed = (float)cameraMaxSpeedUD.Value;
        }

        private void boneXRayChk_CheckedChanged(object sender, EventArgs e) {
            Runtime.boneXrayDisplay = boneXRayChk.Checked;
            UpdateViewportSettings();
        }

        private void mk8PathTB_Click(object sender, EventArgs e) {
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK) {
                mk8PathTB.Text = sfd.SelectedPath;
                Runtime.Mk8GamePath = mk8PathTB.Text;
            }
        }

        private void mk8DPathTB_Click(object sender, EventArgs e) {
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                mk8DPathTB.Text = sfd.SelectedPath;
                Runtime.Mk8dGamePath = mk8DPathTB.Text;
            }
        }

        private void SMOPathTB_Click(object sender, EventArgs e) {
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SMOPathTB.Text = sfd.SelectedPath;
                Runtime.SmoGamePath = SMOPathTB.Text;
            }
        }

        private void displayBoundingBoxeChk_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.renderBoundingBoxes = displayBoundingBoxeChk.Checked;
        }
    }
}
