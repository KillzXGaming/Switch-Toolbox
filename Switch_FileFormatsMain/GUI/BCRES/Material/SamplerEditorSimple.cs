using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using BcresLibrary;

namespace FirstPlugin.Forms
{
    public partial class BcresSamplerEditorSimple : UserControl
    {
        private Thread Thread;

        public BcresSamplerEditorSimple()
        {
            InitializeComponent();

            SetEditorOrientation(true);
            DisplayVertical();
        }

        public void LoadTexture(BcresTextureMapWrapper wrapper)
        {
            var texture = wrapper.GenericMatTexture;

            nameTB.Text = texture.Name;

            stPropertyGrid1.LoadProperty(wrapper.TextureMapInfo, OnPropertyChanged);

            foreach (BCRESGroupNode bcresGrp in PluginRuntime.bcresTexContainers)
            {
                if (bcresGrp.ResourceNodes.ContainsKey(texture.Name))
                {
                    Thread = new Thread((ThreadStart)(() =>
                    {
                        textureBP.Image = Switch_Toolbox.Library.Imaging.GetLoadingImage();
                        textureBP.Image = ((TXOBWrapper)bcresGrp.ResourceNodes[texture.Name]).GetBitmap();
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
