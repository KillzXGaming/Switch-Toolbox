using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Bfres.Structs;
using ResUGX2 = Syroot.NintenTools.Bfres.GX2;
using ResGFX = Syroot.NintenTools.NSW.Bfres.GFX;

namespace FirstPlugin.Forms
{
    public partial class SamplerEditor2 : STUserControl
    {
        FMAT material;
        public ImageList textureImageList;
        private Thread Thread;

        public SamplerEditor2()
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
            wrapUCB.Items.Clear();
            wrapVCB.Items.Clear();
            wrapWCB.Items.Clear();
            depthFuncCB.Items.Clear();
            samplerCB.Items.Clear();
            filterBorderCB.Items.Clear();
            filterAntitropicCB.Items.Clear();
            filterExpandCB.Items.Clear();
            filterMipmapCB.Items.Clear();
            filterShrinkCB.Items.Clear();
            filterZCB.Items.Clear();
            samplerHintTB.Text = "";
            textureNameTB.Text = "";
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
        private void textureRefListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Do this only if none are selected so they don't flicker off/on on change
            if (textureRefListView.SelectedIndices.Count <= 0)
            {
                btnEdit.Enabled = false;
                btnSamplerEdit.Enabled = false;
            }

            Reset();

            if (textureRefListView.SelectedIndices.Count > 0)
            {
                btnEdit.Enabled = true;
                btnSamplerEdit.Enabled = true;

                int index = textureRefListView.SelectedIndices[0];

                MatTexture matTex = (MatTexture)material.TextureMaps[index];

                string name = matTex.Name;
                textureNameTB.Text = name;

                if (matTex.SamplerName != null)
                {
                    if (!samplerCB.Items.Contains(matTex.SamplerName))
                        samplerCB.Items.Add(matTex.SamplerName);

                    samplerCB.SelectedItem = matTex.SamplerName;
                }

                if (matTex.switchSampler != null)
                {
                    
                }
                if (matTex.wiiUSampler != null)
                {
                    LoadWiiUSamplerEnums();

                    wrapUCB.SelectedItem = matTex.wiiUSampler.ClampX;
                    wrapVCB.SelectedItem = matTex.wiiUSampler.ClampY;
                    wrapWCB.SelectedItem = matTex.wiiUSampler.ClampZ;

                    deptComparechkBox.Checked = matTex.wiiUSampler.DepthCompareEnabled;
                    depthFuncCB.SelectedItem = matTex.wiiUSampler.DepthCompareFunc;
                    filterShrinkCB.SelectedItem = matTex.wiiUSampler.MinFilter;
                    filterExpandCB.SelectedItem = matTex.wiiUSampler.MagFilter;
                    filterBorderCB.SelectedItem = matTex.wiiUSampler.BorderType;
                    filterMipmapCB.SelectedItem = matTex.wiiUSampler.MipFilter;
                    filterAntitropicCB.SelectedItem = matTex.wiiUSampler.MaxAnisotropicRatio;
                    filterZCB.SelectedItem = matTex.wiiUSampler.ZFilter;
                }
                lodMaxUD.Value = (decimal)matTex.MaxLod;
                lodMinUD.Value = (decimal)matTex.MinLod;
                biasUD.Value = (decimal)matTex.BiasLod;

                foreach (BNTX bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(name))
                    {
                        Thread = new Thread((ThreadStart)(() =>
                        {
                            textureBP.Image = Toolbox.Library.Imaging.GetLoadingImage();
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
                            textureBP.Image = Toolbox.Library.Imaging.GetLoadingImage();
                            textureBP.Image = ((FTEX)ftexCont.ResourceNodes[name]).GetBitmap();
                        }));
                        Thread.Start();
                    }
                }
            }
        }

        public ResUGX2.GX2TexClamp ClampX;
        public ResUGX2.GX2TexClamp ClampY;
        public ResUGX2.GX2TexClamp ClampZ;
        public ResUGX2.GX2TexXYFilterType MagFilter;
        public ResUGX2.GX2TexXYFilterType MinFilter;
        public ResUGX2.GX2TexZFilterType ZFilter;
        public ResUGX2.GX2TexMipFilterType MipFilter;
        public ResUGX2.GX2TexAnisoRatio MaxAnisotropicRatio;
        public ResUGX2.GX2TexBorderType BorderType;
        public ResUGX2.GX2CompareFunction DepthCompareFunc;

        private void LoadWiiUSamplerEnums()
        {
            foreach (var type in Enum.GetValues(typeof(ResUGX2.GX2TexClamp)).Cast<ResUGX2.GX2TexClamp>())
            {
                wrapUCB.Items.Add(type);
                wrapVCB.Items.Add(type);
                wrapWCB.Items.Add(type);
            }

            foreach (var type in Enum.GetValues(typeof(ResUGX2.GX2CompareFunction)).Cast<ResUGX2.GX2CompareFunction>())
                depthFuncCB.Items.Add(type);

            foreach (var type in Enum.GetValues(typeof(ResUGX2.GX2TexXYFilterType)).Cast<ResUGX2.GX2TexXYFilterType>())
            {
                filterShrinkCB.Items.Add(type);
                filterExpandCB.Items.Add(type);
            }

            foreach (var type in Enum.GetValues(typeof(ResUGX2.GX2TexBorderType)).Cast<ResUGX2.GX2TexBorderType>())
                filterBorderCB.Items.Add(type);

            foreach (var type in Enum.GetValues(typeof(ResUGX2.GX2TexMipFilterType)).Cast<ResUGX2.GX2TexMipFilterType>())
                filterMipmapCB.Items.Add(type);
           
            foreach (var type in Enum.GetValues(typeof(ResUGX2.GX2TexAnisoRatio)).Cast<ResUGX2.GX2TexAnisoRatio>())
                filterAntitropicCB.Items.Add(type);

            foreach (var type in Enum.GetValues(typeof(ResUGX2.GX2TexZFilterType)).Cast<ResUGX2.GX2TexZFilterType>())
                filterZCB.Items.Add(type);
        }

        private void LoadSwitchSamplerEnums()
        {

        }

        private void samplerCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (samplerCB.SelectedIndex >= 0 && textureRefListView.SelectedIndices.Count > 0)
            {
                int index = textureRefListView.SelectedIndices[0];
                MatTexture matTex = (MatTexture)material.TextureMaps[index];

                matTex.SamplerName = samplerCB.GetItemText(samplerCB.SelectedItem);

               samplerHintTB.Text = GetHint(matTex.SamplerName);
            }
        }

        private void btnSamplerEdit_Click(object sender, EventArgs e)
        {
            SamplerListEdit editor = new SamplerListEdit();

            List<string> Samplers = new List<string>();
            foreach (var item in material.TextureMaps)
                Samplers.Add(item.SamplerName);

            editor.LoadSamplers(Samplers);

            if (editor.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void SamplerEditor_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = textureRefListView.SelectedIndices[0];
            MatTexture matTex = (MatTexture)material.TextureMaps[index];

            if (matTex.switchSampler != null)
            {
                var samp = matTex.switchSampler;
                samp.BorderColorType = (ResGFX.TexBorderType)filterBorderCB.SelectedItem;
                samp.WrapModeU = (ResGFX.TexClamp)wrapUCB.SelectedItem;
                samp.WrapModeV = (ResGFX.TexClamp)wrapVCB.SelectedItem;
                samp.WrapModeW = (ResGFX.TexClamp)wrapWCB.SelectedItem;
                samp.CompareFunc = (ResGFX.CompareFunction)depthFuncCB.SelectedItem;
                // samp.MaxAnisotropic = (ResGFX.GX2TexAnisoRatio)lodMinUD.Value;
              //  samp.FilterMode = (ResUGX2.GX2TexXYFilterType)lodMinUD.Value;
              //  samp.MipFilter = (ResUGX2.GX2TexMipFilterType)filterZCB.SelectedItem;
             //   samp.ZFilter = (ResUGX2.GX2TexZFilterType)filterZCB.SelectedItem;
             //   samp.DepthCompareEnabled = deptComparechkBox.Checked;
            }
            else
            {
                var samp = matTex.wiiUSampler;

                if (filterBorderCB.SelectedItem != null)
                    samp.BorderType = (ResUGX2.GX2TexBorderType)filterBorderCB.SelectedItem;
                if (wrapUCB.SelectedItem != null)
                    samp.ClampX = (ResUGX2.GX2TexClamp)wrapUCB.SelectedItem;
                if (wrapVCB.SelectedItem != null)
                    samp.ClampY = (ResUGX2.GX2TexClamp)wrapVCB.SelectedItem;
                if (wrapWCB.SelectedItem != null)
                    samp.ClampZ = (ResUGX2.GX2TexClamp)wrapWCB.SelectedItem;
                if (depthFuncCB.SelectedItem != null)
                    samp.DepthCompareFunc = (ResUGX2.GX2CompareFunction)depthFuncCB.SelectedItem;
                if (filterAntitropicCB.SelectedItem != null)
                    samp.MaxAnisotropicRatio = (ResUGX2.GX2TexAnisoRatio)filterAntitropicCB.SelectedItem;
                if (filterShrinkCB.SelectedItem != null)
                    samp.MinFilter = (ResUGX2.GX2TexXYFilterType)filterShrinkCB.SelectedItem;
                if (filterExpandCB.SelectedItem != null)
                    samp.MagFilter = (ResUGX2.GX2TexXYFilterType)filterExpandCB.SelectedItem;
                if (filterMipmapCB.SelectedItem != null)
                    samp.MipFilter = (ResUGX2.GX2TexMipFilterType)filterMipmapCB.SelectedItem;
                if (filterZCB.SelectedItem != null)
                    samp.ZFilter = (ResUGX2.GX2TexZFilterType)filterZCB.SelectedItem;
                samp.DepthCompareEnabled = deptComparechkBox.Checked;
            }
            matTex.BiasLod = (float)biasUD.Value;
            matTex.MaxLod = (float)lodMaxUD.Value;
            matTex.MinLod = (float)lodMinUD.Value;
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var tex = new MatTexture();
            tex.SamplerName = "_a1";
            tex.FragShaderSampler = "_a1";
            tex.Name = "Untitled";
            tex.Type = Toolbox.Library.STGenericMatTexture.TextureType.Unknown;
            tex.WrapModeS = Toolbox.Library.STTextureWrapMode.Repeat;
            tex.WrapModeT = Toolbox.Library.STTextureWrapMode.Repeat;
            tex.WrapModeW = Toolbox.Library.STTextureWrapMode.Clamp;

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
                texSampler.MaxAnisotropicRatio = ResUGX2.GX2TexAnisoRatio.Ratio_1_1;
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
            var index = textureRefListView.SelectedIndices[0];

            if (index >= 0)
            { 
                material.TextureMaps.RemoveAt(index);
                textureRefListView.Items.RemoveAt(index);
            }
        }
    }
}
