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

namespace FirstPlugin.Forms
{
    public partial class SamplerEditorSimple : UserControl
    {
        private Thread Thread;

        public SamplerEditorSimple()
        {
            InitializeComponent();
        }

        public void LoadTexture(MatTexture texture)
        {
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

        }

        private void stTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
