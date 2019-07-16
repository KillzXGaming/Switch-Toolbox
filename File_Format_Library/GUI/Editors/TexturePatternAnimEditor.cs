using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Bfres.Structs;
using BrightIdeasSoftware;
using Toolbox.Library.Animations;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class TexturePatternAnimEditor : UserControl
    {
        public ImageList textureImageList;

        public TexturePatternAnimEditor()
        {
            InitializeComponent();

            trackbarKeyEditor1.ForeColor = Color.White;
            trackbarKeyEditor1.ForeColor = FormThemes.BaseTheme.FormBackColor;

            textureImageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(30, 30),
            };
        }

        MaterialAnimation.Material material;
        FMAA.BfresSamplerAnim activeSampler;

        public void LoadAnim(FMAA.BfresSamplerAnim samplerAnim)
        {
            activeSampler = samplerAnim;
            material = (MaterialAnimation.Material)samplerAnim.Parent;
            var anim = (MaterialAnimation)material.Parent;
            trackbarKeyEditor1.ClearKeys();
            trackbarKeyEditor1.Maximum = anim.FrameCount;
            samplerTreeView.Nodes.Clear();

            int KeyHeight = trackbarKeyEditor1.ClientSize.Width / anim.FrameCount;
            pictureBoxCustom1.Image = null;

            foreach (FMAA.BfresSamplerAnim sampler in material.Nodes)
            {
                samplerTreeView.Nodes.Add(sampler.Text);
            }
            if (samplerTreeView.Nodes.Count > 0)
                samplerTreeView.SelectedNode = samplerTreeView.Nodes[0];

            animMaxFrameUD.Value = anim.FrameCount;

            for (int Frame = 0; Frame < anim.FrameCount; Frame++)
            {
                int KeyPosY = KeyHeight;
                foreach (FMAA.BfresSamplerAnim sampler in material.Nodes)
                {
                    var keyFrame = sampler.GetKeyFrame(Frame);

                    if (keyFrame.IsKeyed)
                    {
                        var tex = sampler.GetActiveTexture(Frame);

                        if (tex != null)
                        {
                            Bitmap temp = tex.GetBitmap();
                            trackbarKeyEditor1.AddKeyFrameThumbSlider(Frame, KeyPosY, KeyHeight, temp);
                            KeyPosY += KeyHeight;
                        }
                    }
                }
            }
        }

        private void trackbarKeyEditor1_ValueChanged(object sender, EventArgs e)
        {
            if (activeSampler != null)
            {
                int frame = trackbarKeyEditor1.Value;
                var tex = activeSampler.GetActiveTexture(frame);
                pictureBoxCustom1.Image = tex.GetBitmap();

                animCurrentFrameUD.Value = frame;
            }
        }

        private void treeViewCustom1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (samplerTreeView.SelectedNode != null)
            {
                int index = samplerTreeView.SelectedNode.Index;

                activeSampler = (FMAA.BfresSamplerAnim)material.Nodes[index];
            }
        }

        private void stPanel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
