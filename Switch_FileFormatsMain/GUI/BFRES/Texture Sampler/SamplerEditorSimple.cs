using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Bfres.Structs;
using ResUGX2 = Syroot.NintenTools.Bfres.GX2;
using Syroot.NintenTools.NSW.Bfres.GX2;
using Syroot.NintenTools.NSW.Bfres;
using Switch_Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class SamplerEditorSimple : UserControl
    {
        private Thread Thread;

        public SamplerEditorSimple()
        {
            InitializeComponent();
            SetEditorOrientation(true);
            DisplayVertical();
        }

        private MatTexture ActiveMatTexture;

        public void LoadTexture(MatTexture texture)
        {
            ActiveMatTexture = texture;

            nameTB.Text = texture.Name;

            samplerCB.Items.Clear();
            samplerCB.Items.Add(texture.SamplerName);
            samplerCB.SelectedItem = texture.SamplerName;

            if (texture.wiiUSampler != null)
            {
                stPropertyGrid1.LoadProperty(texture.wiiUSampler , OnPropertyChanged);
            }
            else
            {
                stPropertyGrid1.LoadProperty(texture.switchSampler, OnPropertyChanged);
            }

            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                if (bntx.Textures.ContainsKey(texture.Name))
                {
                    Thread = new Thread((ThreadStart)(() =>
                    {
                        textureBP.Image = Switch_Toolbox.Library.Imaging.GetLoadingImage();
                        textureBP.Image = bntx.Textures[texture.Name].GetBitmap();
                    }));
                    Thread.Start();
                }
            }
            foreach (BFRESGroupNode ftexCont in PluginRuntime.ftexContainers)
            {
                if (ftexCont.ResourceNodes.ContainsKey(texture.Name))
                {
                    Thread = new Thread((ThreadStart)(() =>
                    {
                        textureBP.Image = Switch_Toolbox.Library.Imaging.GetLoadingImage();
                        textureBP.Image = ((FTEX)ftexCont.ResourceNodes[texture.Name]).GetBitmap();
                    }));
                    Thread.Start();
                }
            }
        }

        private void OnPropertyChanged()
        {
            if (ActiveMatTexture.wiiUSampler != null)
            {
                ActiveMatTexture.wrapModeS = (int)ActiveMatTexture.wiiUSampler.ClampX;
                ActiveMatTexture.wrapModeT = (int)ActiveMatTexture.wiiUSampler.ClampY;
                ActiveMatTexture.wrapModeW = (int)ActiveMatTexture.wiiUSampler.ClampZ;

                if (ActiveMatTexture.wiiUSampler.MinFilter == ResUGX2.GX2TexXYFilterType.Point)
                    ActiveMatTexture.minFilter = 1;
                if (ActiveMatTexture.wiiUSampler.MagFilter == ResUGX2.GX2TexXYFilterType.Point)
                    ActiveMatTexture.magFilter = 1;
                if (ActiveMatTexture.wiiUSampler.MinFilter == ResUGX2.GX2TexXYFilterType.Bilinear)
                    ActiveMatTexture.minFilter = 0;
                if (ActiveMatTexture.wiiUSampler.MagFilter == ResUGX2.GX2TexXYFilterType.Bilinear)
                    ActiveMatTexture.magFilter = 0;
            }
            else
            {
                ActiveMatTexture.wrapModeS = (int)ActiveMatTexture.switchSampler.WrapModeU;
                ActiveMatTexture.wrapModeT = (int)ActiveMatTexture.switchSampler.WrapModeV;
                ActiveMatTexture.wrapModeW = (int)ActiveMatTexture.switchSampler.WrapModeW;

                if (ActiveMatTexture.switchSampler.ShrinkXY == Sampler.ShrinkFilterModes.Points)
                    ActiveMatTexture.minFilter = 1;
                if (ActiveMatTexture.switchSampler.ExpandXY == Sampler.ExpandFilterModes.Points)
                    ActiveMatTexture.magFilter = 1;
                if (ActiveMatTexture.switchSampler.ShrinkXY == Sampler.ShrinkFilterModes.Linear)
                    ActiveMatTexture.minFilter = 0;
                if (ActiveMatTexture.switchSampler.ExpandXY == Sampler.ExpandFilterModes.Linear)
                    ActiveMatTexture.magFilter = 0;
            }

            LibraryGUI.Instance.UpdateViewport();
        }

        private void stTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        public void SetEditorOrientation(bool ToVertical)
        {
            displayVerticalToolStripMenuItem.Checked = ToVertical;
        }

        private void displayVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (displayVerticalToolStripMenuItem.Checked)
            {
                DisplayHorizontal();
            }
            else
            {
                DisplayVertical();
            }
        }

        private void DisplayHorizontal()
        {
            if (splitContainer2.Panel1Collapsed)
                return;

            var ImagePanel = stPanel1;
            var PropertiesEditor = stPropertyGrid1;

            //Swap panels
            splitContainer2.Panel1.Controls.Clear();
            splitContainer2.Panel2.Controls.Clear();

            splitContainer2.Orientation = Orientation.Vertical;
            splitContainer2.Panel1.Controls.Add(ImagePanel);
            splitContainer2.Panel2.Controls.Add(PropertiesEditor);
            stPropertyGrid1.ShowHintDisplay = true;

            PropertiesEditor.Width = this.Width / 2;

            splitContainer2.SplitterDistance = this.Width / 2;
        }

        private void DisplayVertical()
        {
            if (splitContainer2.Panel2Collapsed)
                return;

            var ImagePanel = stPanel1;
            var PropertiesEditor = stPropertyGrid1;

            //Swap panels
            splitContainer2.Panel1.Controls.Clear();
            splitContainer2.Panel2.Controls.Clear();

            splitContainer2.Orientation = Orientation.Horizontal;
            splitContainer2.Panel2.Controls.Add(ImagePanel);
            splitContainer2.Panel1.Controls.Add(PropertiesEditor);

            splitContainer2.SplitterDistance = this.Height / 2;

            stPropertyGrid1.ShowHintDisplay = false;
        }
    }
}
