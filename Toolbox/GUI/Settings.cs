using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;

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

            uvChannelRB.Checked = true;
            uvChannelRB.Visible = false;
            uvChannelRB2.Visible = false;
            uvChannelRB3.Visible = false;

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
            botwGamePathTB.Text = Runtime.BotwGamePath;
            tpGamePathTB.Text = Runtime.TpGamePath;
            modelLoadArchive.Checked = Runtime.ObjectEditor.OpenModelsOnOpen;
            specularCubemapPathTB.Text = System.IO.Path.GetFileName(Runtime.PBR.SpecularCubeMapPath);
            diffuseCubemapPathTB.Text = System.IO.Path.GetFileName(Runtime.PBR.DiffuseCubeMapPath);
            chkUseSkyobx.Checked = Runtime.PBR.UseSkybox;
            chkDiffyseSkybox.Checked = Runtime.PBR.UseDiffuseSkyTexture;
            chkDiffyseSkybox.Enabled = chkUseSkyobx.Checked;
            chkBotwFileTable.Checked = Runtime.ResourceTables.BotwTable;
            chkTpFileTable.Checked = Runtime.ResourceTables.TpTable;
            chkFrameCamera.Checked = Runtime.FrameCamera;
            chkAlwaysCompressOnSave.Checked = Runtime.AlwaysCompressOnSave;
            chkViewportGrid.Checked = Runtime.displayGrid;
            chkViewportAxisLines.Checked = Runtime.displayAxisLines;

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
            if (Runtime.viewportShading == Runtime.ViewportShading.UVCoords ||
                Runtime.viewportShading == Runtime.ViewportShading.UVTestPattern)
            {
                uvChannelRB.Visible = true;
                uvChannelRB2.Visible = true;
                uvChannelRB3.Visible = true;
            }
            else
            {
                uvChannelRB.Visible = false;
                uvChannelRB2.Visible = false;
                uvChannelRB3.Visible = false;
            }

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
            UpdateViewportSettings(true);
        }

        private void camFarNumUD_ValueChanged(object sender, EventArgs e)
        {
            Runtime.CameraFar = (float)camFarNumUD.Value;
            UpdateViewportSettings(true);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            UpdateViewportSettings(true);

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

            Viewport viewport = LibraryGUI.GetActiveViewport();
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
               var result = MessageBox.Show("Changing themes will require to restart the program. Do you want to restart now?", "Toolbox Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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

            Viewport viewport = LibraryGUI.GetActiveViewport();
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

        private void tpGamePathTB_Click(object sender, EventArgs e) {
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                tpGamePathTB.Text = sfd.SelectedPath;
                Runtime.TpGamePath = tpGamePathTB.Text;
            }
        }

        private void botwGamePathTB_Click(object sender, EventArgs e) {
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (!IsValidBotwDirectory(sfd.SelectedPath))
                    throw new Exception("Invalid path choosen. Make sure you have atleast an RSTB file in the path! |System/Resource/ResourceSizeTable.product.srsizetable|");

                botwGamePathTB.Text = sfd.SelectedPath;
                Runtime.BotwGamePath = botwGamePathTB.Text;
            }
        }

        private void displayBoundingBoxeChk_CheckedChanged(object sender, EventArgs e) {
            Runtime.renderBoundingBoxes = displayBoundingBoxeChk.Checked;
        }

        private void modelLoadArchive_CheckedChanged(object sender, EventArgs e) {
            Runtime.ObjectEditor.OpenModelsOnOpen = modelLoadArchive.Checked;
        }

        private void cubemapPathTB_Click(object sender, EventArgs e) {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "DDS |*.dds;";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (IsValidCubeMap(sfd.FileName))
                {
                    specularCubemapPathTB.Text = System.IO.Path.GetFileName(sfd.FileName);
                    Runtime.PBR.SpecularCubeMapPath = sfd.FileName;
                    RenderTools.ResetSpecularCubeMap();
                    UpdateViewportSettings();
                }
                else
                    MessageBox.Show("Invalid cube map file. Make sure it is a DDS with a cube map.");
            }
        }

        private void diffuseCubemapPathTBB_Click(object sender, EventArgs e) {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "DDS |*.dds;";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (IsValidCubeMap(sfd.FileName))
                {
                    diffuseCubemapPathTB.Text = System.IO.Path.GetFileName(sfd.FileName);
                    Runtime.PBR.DiffuseCubeMapPath = sfd.FileName;
                    RenderTools.ResetDiffuseCubeMap();
                    UpdateViewportSettings();
                }
                else
                    MessageBox.Show("Invalid cube map file. Make sure it is a DDS with a cube map.");
            }
        }

        private bool IsValidCubeMap(string FilePath)
        {
            try
            {
                DDS dds = new DDS(FilePath);
                if (dds.ArrayCount == 6)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        private void chkUseSkyobx_CheckedChanged(object sender, EventArgs e) {
            Runtime.PBR.UseSkybox = chkUseSkyobx.Checked;
            chkDiffyseSkybox.Enabled = chkUseSkyobx.Checked;
            UpdateViewportSettings();
        }

        private void chkDiffyseSkybox_CheckedChanged(object sender, EventArgs e) {
            Runtime.PBR.UseDiffuseSkyTexture = chkDiffyseSkybox.Checked;
            UpdateViewportSettings();
        }

        private void clearSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Try to cast the sender to a ToolStripItem
            ToolStripItem menuItem = sender as ToolStripItem;
            if (menuItem != null)
            {
                // Retrieve the ContextMenuStrip that owns this ToolStripItem
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    // Get the control that is displaying this context menu
                    Control sourceControl = owner.SourceControl;
                    switch (sourceControl.Name)
                    {
                        case "diffuseCubemapPathTB":
                            diffuseCubemapPathTB.Text = "";
                            Runtime.PBR.DiffuseCubeMapPath = "";
                            RenderTools.ResetDiffuseCubeMap();
                            break;
                        case "specularCubemapPathTB":
                            specularCubemapPathTB.Text = "";
                            Runtime.PBR.SpecularCubeMapPath = "";
                            RenderTools.ResetSpecularCubeMap();
                            break;
                        case "mk8DPathTB":
                            mk8DPathTB.Text = "";
                            Runtime.Mk8dGamePath = "";
                            break;
                        case "mk8PathTB":
                            mk8PathTB.Text = "";
                            Runtime.Mk8GamePath = "";
                            break;
                        case "SMOPathTB":
                            SMOPathTB.Text = "";
                            Runtime.SmoGamePath = "";
                            break;
                        case "tpGamePathTB":
                            tpGamePathTB.Text = "";
                            Runtime.TpGamePath = "";
                            break;
                        case "botwGamePathTB":
                            botwGamePathTB.Text = "";
                            Runtime.BotwGamePath = "";
                            break;
                        case "pathPokemonSwShTB":
                            pathPokemonSwShTB.Text = "";
                            Runtime.PkSwShGamePath = "";
                            break;
                    }
                }
            }
        }

        private void specularCubemapPathTB_TextChanged(object sender, EventArgs e)
        {

        }

        private void uvChannelRB_CheckedChanged(object sender, EventArgs e) {
            Runtime.uvChannel = Runtime.UVChannel.Channel1;
            UpdateViewportSettings();
        }

        private void uvChannelRB2_CheckedChanged(object sender, EventArgs e) {
            Runtime.uvChannel = Runtime.UVChannel.Channel2;
            UpdateViewportSettings();
        }

        private void uvChannelRB3_CheckedChanged(object sender, EventArgs e)  {
            Runtime.uvChannel = Runtime.UVChannel.Channel3;
            UpdateViewportSettings();
        }

        private void chkBotwFileTable_CheckedChanged(object sender, EventArgs e) {
            if (!System.IO.Directory.Exists(Runtime.BotwGamePath) || !IsValidBotwDirectory(Runtime.BotwGamePath))
            {
                FolderSelectDialog sfd = new FolderSelectDialog();
                sfd.Title = "Select Modded Game Path!!!";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (!IsValidBotwDirectory(sfd.SelectedPath))
                        throw new Exception($"Invalid path choosen. Make sure you have atleast an RSTB file in the path! |{sfd.SelectedPath}/System/Resource/ResourceSizeTable.product.srsizetable|");

                    botwGamePathTB.Text = sfd.SelectedPath;
                    Runtime.BotwGamePath = botwGamePathTB.Text;
                }
            }

            Runtime.ResourceTables.BotwTable = chkBotwFileTable.Checked;
        }

        private bool IsValidBotwDirectory(string GamePath)
        {
            //This is the only file i care about
            string RstbPath = System.IO.Path.Combine($"{GamePath}",
                 "System", "Resource", "ResourceSizeTable.product.srsizetable");

            return System.IO.File.Exists(RstbPath);
        }

        private bool IsValidTPHDDirectory(string GamePath)
        {
            //This is the only file i care about
            string DecompressedSizeList = System.IO.Path.Combine($"{GamePath}", "DecompressedSizeList.txt");
            string FileSizeList = System.IO.Path.Combine($"{GamePath}", "FileSizeList.txt");

            return System.IO.File.Exists(DecompressedSizeList) && System.IO.File.Exists(FileSizeList);
        }

        private void chkTpFileTable_CheckedChanged(object sender, EventArgs e) {
            if (!System.IO.Directory.Exists(Runtime.TpGamePath) || !IsValidTPHDDirectory(Runtime.TpGamePath))
            {
                FolderSelectDialog sfd = new FolderSelectDialog();
                sfd.Title = "Select Modded Game Path!!!";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (!IsValidTPHDDirectory(sfd.SelectedPath))
                        throw new Exception($"Invalid path choosen. Make sure you have atleast a FileSizeList.txt and DecompressedSizeList.txt file in the path!");

                    tpGamePathTB.Text = sfd.SelectedPath;
                    Runtime.TpGamePath = tpGamePathTB.Text;
                }
            }

            Runtime.ResourceTables.TpTable = chkTpFileTable.Checked;
        }

        private void chkFrameCamera_CheckedChanged(object sender, EventArgs e) {
            Runtime.FrameCamera = chkFrameCamera.Checked;
        }

        private void chkAlwaysCompressOnSave_CheckedChanged(object sender, EventArgs e) {
           Runtime.AlwaysCompressOnSave = chkAlwaysCompressOnSave.Checked;
        }

        private void pathPokemonSwShTB_TextChanged(object sender, EventArgs e)
        {

        }

        private void pathPokemonSwShTB_Click(object sender, EventArgs e) {
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (!IsValidPokemonSwShDirectory(sfd.SelectedPath))
                    throw new Exception("Invalid path choosen. You need Bin/appli/icon_pokemon atleast for pokemon icons");

                pathPokemonSwShTB.Text = sfd.SelectedPath;
                Runtime.PkSwShGamePath = pathPokemonSwShTB.Text;
            }
        }

        private bool IsValidPokemonSwShDirectory(string GamePath)
        {
            //Search for pokemon icons
            string RstbPath = System.IO.Path.Combine($"{GamePath}",
                 "Bin", "appli", "icon_pokemon", "poke_icon_0000_00s_n.bntx");

            return System.IO.File.Exists(RstbPath);
        }

        private void btnReset_Click(object sender, EventArgs e) {
            var result = MessageBox.Show("Resetting the settings will require to restart the program. Do you want to restart now?", "Toolbox Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (System.IO.File.Exists($"{Runtime.ExecutableDir}\\config.xml"))
                    System.IO.File.Delete($"{Runtime.ExecutableDir}\\config.xml");

                this.Close();

                Application.Restart();
                Environment.Exit(0);
            }
        }

        private void chkViewportGrid_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.displayGrid = chkViewportGrid.Checked;
            UpdateViewportSettings();
        }

        private void chkViewportAxisLines_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.displayAxisLines = chkViewportAxisLines.Checked;
            UpdateViewportSettings();
        }
    }
}
