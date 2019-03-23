using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using Bfres.Structs;
using ResUGX2 = Syroot.NintenTools.Bfres.GX2;
using ResGFX = Syroot.NintenTools.NSW.Bfres.GFX;
using ResNX = Syroot.NintenTools.NSW.Bfres;

namespace FirstPlugin.Forms
{
    public partial class SamplerEditor : STUserControl
    {
        FMAT material;
        public ImageList textureImageList;
        private Thread Thread;

        public SamplerEditor()
        {
            InitializeComponent();

            textureImageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(30, 30),
            };
        }

        public void FillForm()
        {
            InitializeTextureListView(material);
        }


        private void textureRefListView_DoubleClick(object sender, EventArgs e)
        {
            if (textureRefListView.SelectedIndices.Count <= 0)
                return;

            int index = textureRefListView.SelectedIndices[0];
            Texture_Selector tex = new Texture_Selector();
            tex.LoadTexture(textureRefListView.SelectedItems[0].Text, material.GetResFileU() != null);
            if (tex.ShowDialog() == DialogResult.OK)
            {
                material.TextureMaps[index].Name = tex.GetSelectedTexture();
                InitializeTextureListView(material);
                material.UpdateTextureMaps();
            }
        }

        public void InitializeTextureListView(FMAT fmat)
        {
            material = fmat;

            textureRefListView.Items.Clear();
            textureRefListView.SmallImageList = textureImageList;
            textureRefListView.FullRowSelect = true;

            foreach (MatTexture tex in material.TextureMaps)
            {
                ListViewItem item = new ListViewItem();
                item.Text = tex.Name;
                item.SubItems.Add(tex.SamplerName);

                if (material.shaderassign.samplers.ContainsValue(tex.SamplerName))
                {
                    var FragSampler = material.shaderassign.samplers.FirstOrDefault(x => x.Value == tex.SamplerName).Key;
                    item.SubItems.Add(FragSampler.ToString());
                }

                textureRefListView.Items.Add(item);
            }

            textureImageList.Images.Clear();

            int CurTex = 0;
            if (PluginRuntime.bntxContainers.Count == 0 &&
                PluginRuntime.ftexContainers.Count == 0)
            {
                foreach (ListViewItem item in textureRefListView.Items)
                {
                    AddBlankTexture(item, item.Text, CurTex++);
                }
            }
            return;


            bool FoundTexture = false;
            foreach (ListViewItem item in textureRefListView.Items)
            {
                foreach (BNTX bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(item.Text))
                    {
                        FoundTexture = true;

                        Thread Thread = new Thread((ThreadStart)(() =>
                        {
                            Bitmap temp = bntx.Textures[item.Text].GetBitmap();

                            textureRefListView.Invoke(new Action(() =>
                            {
                                textureImageList.Images.Add(item.Text, temp);
                            }));

                            temp.Dispose();
                        }));
                        Thread.Start();

                        item.ImageIndex = CurTex++;
                    }
                }
                foreach (BFRESGroupNode ftexCont in PluginRuntime.ftexContainers)
                {
                    if (ftexCont.ResourceNodes.ContainsKey(item.Text))
                    {
                        FoundTexture = true;

                        FTEX tex = (FTEX)ftexCont.ResourceNodes[item.Text];
                        var renderedTex = tex.RenderableTex;
                        Bitmap temp = tex.GetBitmap();

                        textureImageList.Images.Add(tex.Text, temp);

                        item.ImageIndex = CurTex++;

                        var dummy = textureImageList.Handle;
                        temp.Dispose();
                    }
                }
                if (FoundTexture == false)
                {
                    AddBlankTexture(item, item.Text, CurTex++);
                }
            }
        }
        private void AddBlankTexture(ListViewItem item, string Name, int ImageIndex)
        {
            Bitmap temp = new Bitmap(Properties.Resources.TextureError);

            textureImageList.Images.Add(Name, temp);
            item.ImageIndex = ImageIndex;

            var dummy = textureImageList.Handle;
            temp.Dispose();
        }
        private void Reset()
        {
            stPropertyGrid1.LoadProperty(null, null);
        }

        private string GetHint(string Sampler)
        {
            switch (Sampler)
            {
                case "_a0": return "albedo 0";
                case "_a1": return "albedo 1";
                case "_a2": return "albedo 2";
                case "_a3": return "albedo 3";
                case "_n0": return "normal 0";
                case "_n1": return "normal 1";
                case "_n2": return "normal 2";
                case "_n3": return "normal 3";
                default:
                    return "";
            }
        }

        public void OnPropertyChanged() { }

        private void textureRefListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Do this only if none are selected so they don't flicker off/on on change
            if (textureRefListView.SelectedIndices.Count <= 0)
            {
                btnEdit.Enabled = false;
                samplerHintTB.Text = "";
                samplerTB.Text = "";
                btnRemove.Enabled = false;
            }

            Reset();

            if (textureRefListView.SelectedIndices.Count > 0)
            {
                btnEdit.Enabled = true;
                btnRemove.Enabled = true;

                int index = textureRefListView.SelectedIndices[0];

                MatTexture matTex = (MatTexture)material.TextureMaps[index];

                string name = matTex.Name;
                textureNameTB.Text = name;

                if (matTex.SamplerName != null)
                {
                    samplerTB.Text = matTex.SamplerName;
                }

                if (matTex.switchSampler != null)
                {
                    stPropertyGrid1.LoadProperty(matTex.switchSampler, OnPropertyChanged);

                }
                if (matTex.wiiUSampler != null)
                {
                    stPropertyGrid1.LoadProperty(matTex.wiiUSampler, OnPropertyChanged);
                }

                foreach (BNTX bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(name))
                    {
                        Thread = new Thread((ThreadStart)(() =>
                        {
                            textureBP.Image = Switch_Toolbox.Library.Imaging.GetLoadingImage();
                            textureBP.Image = bntx.Textures[name].GetBitmap();
                        }));
                        Thread.Start();
                    }
                }
                foreach (BFRESGroupNode ftexCont in PluginRuntime.ftexContainers)
                {
                    if (ftexCont.ResourceNodes.ContainsKey(name))
                    {
                        Thread = new Thread((ThreadStart)(() =>
                        {
                            textureBP.Image = Switch_Toolbox.Library.Imaging.GetLoadingImage();
                            textureBP.Image = ((FTEX)ftexCont.ResourceNodes[name]).GetBitmap();
                        }));
                        Thread.Start();
                    }
                }
            }
        }

        private void LoadWiiUSamplerEnums()
        {
     
        }

        private void LoadSwitchSamplerEnums()
        {

        }

        private void SamplerEditor_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = textureRefListView.SelectedIndices[0];
            MatTexture matTex = (MatTexture)material.TextureMaps[index];

        
        }

        private void lodMinUD_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lodMaxUD_ValueChanged(object sender, EventArgs e)
        {

        }

        private void biasUD_ValueChanged(object sender, EventArgs e)
        {

        }

        private void deptComparechkBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        int SamplerDupeIndex = 0;
        private string GetSamplerName(string Sampler)
        {
            foreach (MatTexture tex in material.TextureMaps)
            {
                if (tex.SamplerName == Sampler)
                {
                    var output = Regex.Replace(Sampler, @"[\d-]", string.Empty);
                        
                    Sampler = $"{output}{SamplerDupeIndex++}";
                    return GetSamplerName(Sampler);
                }
            }
            return Sampler;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var tex = new MatTexture();
            tex.SamplerName = GetSamplerName("_a0");
            tex.FragShaderSampler = "_a0";
            tex.Name = "Untitled";
            tex.Type = Switch_Toolbox.Library.STGenericMatTexture.TextureType.Unknown;
            tex.wrapModeS = 0;
            tex.wrapModeT = 0;
            tex.wrapModeW = 2;

            if (material.GetResFileU() != null)
            {
                var texSampler = new ResUGX2.TexSampler();
                texSampler.BorderType = ResUGX2.GX2TexBorderType.ClearBlack;
                texSampler.ClampX = ResUGX2.GX2TexClamp.Wrap;
                texSampler.ClampY = ResUGX2.GX2TexClamp.Wrap;
                texSampler.ClampZ = ResUGX2.GX2TexClamp.Clamp;
                texSampler.DepthCompareEnabled = false;
                texSampler.DepthCompareFunc = ResUGX2.GX2CompareFunction.Never;
                texSampler.MagFilter = ResUGX2.GX2TexXYFilterType.Point;
                texSampler.MaxAnisotropicRatio = ResUGX2.GX2TexAnisoRatio.TwoToOne;
                texSampler.MinFilter = ResUGX2.GX2TexXYFilterType.Point;
                texSampler.MipFilter = ResUGX2.GX2TexMipFilterType.Linear;
                texSampler.ZFilter = 0;
                texSampler.MaxLod = 13;
                texSampler.MinLod = 0;
                texSampler.LodBias = 0;
                tex.wiiUSampler = texSampler;
            }
            else
            {
                var texSampler = new ResNX.Sampler();
                tex.switchSampler = texSampler;
            }
            material.TextureMaps.Add(tex);

            var item = new ListViewItem();
            item.Text = "Untitled";
            item.SubItems.Add(tex.SamplerName);
            item.SubItems.Add(tex.FragShaderSampler);
            textureRefListView.Items.Add(item);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (textureRefListView.SelectedIndices.Count > 0)
            {
                string FirstSelecteItem = textureRefListView.SelectedItems[0].Text;

                DialogResult result;
                if (textureRefListView.SelectedIndices.Count == 1)
                    result = MessageBox.Show($"Are you sure you want to remove texture {FirstSelecteItem}? This could potentially break things!",
                        "Texture Map Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                else
                    result = MessageBox.Show("Are you sure you want to remove these textures? This could potentially break things!",
                         "Texture Map Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    foreach (ListViewItem tex in textureRefListView.SelectedItems)
                    {
                        textureRefListView.Items.Remove(tex);

                        int index = material.TextureMaps.IndexOf(material.TextureMaps.Where(p => p.Name == tex.Text).FirstOrDefault());
                        material.TextureMaps.RemoveAt(index);
                    }
                }
            }
        }

        private void samplerTB_TextChanged(object sender, EventArgs e)
        {
            if (textureRefListView.SelectedItems.Count <= 0)
                return;

            var index = textureRefListView.SelectedIndices[0];

            samplerHintTB.Text = GetHint(samplerHintTB.Text);

            bool IsDuped = false;

            List<string> KeyCheck = new List<string>();
            foreach (MatTexture item in material.TextureMaps)
            {
                if (!KeyCheck.Contains(item.SamplerName))
                    KeyCheck.Add(item.SamplerName);
                else
                {
                    IsDuped = true;
                    STErrorDialog.Show($"A sampler with the same name already exists! {item.SamplerName}",
                        this.Text, "");
                }
            }
            if (!IsDuped)
            {
                material.TextureMaps[index].SamplerName = samplerTB.Text;
            }

            KeyCheck.Clear();
        }
    }
}
