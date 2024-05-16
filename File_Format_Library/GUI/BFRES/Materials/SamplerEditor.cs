using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;
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

            int CurTex = 0;
            if (PluginRuntime.bntxContainers.Count == 0 &&
                PluginRuntime.ftexContainers.Count == 0)
            {
                foreach (ListViewItem item in textureRefListView.Items)
                {
                    AddBlankTexture(item, item.Text, CurTex++);
                }
            }
            //Still somewhat slower 
         //   ReloadImageList();
        }

        private void ReloadImageList()
        {
            textureImageList.Images.Clear();

            Thread Thread = new Thread((ThreadStart)(() =>
            {
                int listIndex = 0;
                int imageIndex = 0;
                foreach (var matTexture in material.TextureMaps)
                {
                    bool IsFound = false;
                    foreach (BNTX bntx in PluginRuntime.bntxContainers)
                    {
                        if (bntx.Textures.ContainsKey(matTexture.Name))
                        {
                            Bitmap temp = bntx.Textures[matTexture.Name].RenderableTex.ToBitmap();

                            if (textureRefListView.InvokeRequired)
                            {
                                textureRefListView.Invoke((MethodInvoker)delegate {
                                    ListViewItem item = textureRefListView.Items[listIndex++];
                                    item.ImageIndex = imageIndex++;
                                    // Running on the UI thread
                                    textureImageList.Images.Add(temp);
                                    var dummy = textureImageList.Handle;
                                    IsFound = true;
                                });
                            }
                            temp.Dispose();
                        }
                    }

                    if (!IsFound)
                    {
                        Bitmap temp = new Bitmap(Properties.Resources.TextureError);

                        if (textureRefListView.InvokeRequired)
                        {
                            textureRefListView.Invoke((MethodInvoker)delegate {
                                ListViewItem item = textureRefListView.Items[listIndex++];
                                // Running on the UI thread
                                textureImageList.Images.Add(Name, temp);
                                item.ImageIndex = imageIndex++;
                                var dummy = textureImageList.Handle;
                            });
                        }

                        temp.Dispose();
                    }
                }
            }));
            Thread.Start();
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

        public void OnPropertyChanged()
        {
            if (textureRefListView.SelectedIndices.Count <= 0)
                return;

            int index = textureRefListView.SelectedIndices[0];
            MatTexture ActiveMatTexture = (MatTexture)material.TextureMaps[index];

            if (ActiveMatTexture.wiiUSampler != null)
            {
                ActiveMatTexture.WrapModeS = SetWrapMode((int)ActiveMatTexture.wiiUSampler.ClampX);
                ActiveMatTexture.WrapModeT = SetWrapMode((int)ActiveMatTexture.wiiUSampler.ClampY);
                ActiveMatTexture.WrapModeW = SetWrapMode((int)ActiveMatTexture.wiiUSampler.ClampZ);

                if (ActiveMatTexture.wiiUSampler.MinFilter == ResUGX2.GX2TexXYFilterType.Point)
                    ActiveMatTexture.MinFilter = STTextureMinFilter.Nearest;
                if (ActiveMatTexture.wiiUSampler.MagFilter == ResUGX2.GX2TexXYFilterType.Point)
                    ActiveMatTexture.MagFilter = STTextureMagFilter.Nearest;
                if (ActiveMatTexture.wiiUSampler.MinFilter == ResUGX2.GX2TexXYFilterType.Bilinear)
                    ActiveMatTexture.MinFilter = STTextureMinFilter.Linear;
                if (ActiveMatTexture.wiiUSampler.MagFilter == ResUGX2.GX2TexXYFilterType.Bilinear)
                    ActiveMatTexture.MagFilter = STTextureMagFilter.Linear;
            }
            else
            {
                ActiveMatTexture.WrapModeS = SetWrapMode((int)ActiveMatTexture.switchSampler.WrapModeU);
                ActiveMatTexture.WrapModeT = SetWrapMode((int)ActiveMatTexture.switchSampler.WrapModeV);
                ActiveMatTexture.WrapModeW = SetWrapMode((int)ActiveMatTexture.switchSampler.WrapModeW);

                if (ActiveMatTexture.switchSampler.ShrinkXY == ResNX.Sampler.ShrinkFilterModes.Points)
                    ActiveMatTexture.MinFilter = STTextureMinFilter.Nearest;
                if (ActiveMatTexture.switchSampler.ExpandXY == ResNX.Sampler.ExpandFilterModes.Points)
                    ActiveMatTexture.MagFilter = STTextureMagFilter.Nearest;
                if (ActiveMatTexture.switchSampler.ShrinkXY == ResNX.Sampler.ShrinkFilterModes.Linear)
                    ActiveMatTexture.MinFilter = STTextureMinFilter.Linear;
                if (ActiveMatTexture.switchSampler.ExpandXY == ResNX.Sampler.ExpandFilterModes.Linear)
                    ActiveMatTexture.MagFilter = STTextureMagFilter.Linear;
            }

            LibraryGUI.UpdateViewport();
        }

        private STTextureWrapMode SetWrapMode(int wrapMode)
        {
            switch (wrapMode)
            {
                case (int)ResNX.GFX.TexClamp.Repeat:
                    return STTextureWrapMode.Repeat;
                case (int)ResNX.GFX.TexClamp.Mirror:
                    return STTextureWrapMode.Mirror;
                case (int)ResNX.GFX.TexClamp.Clamp:
                case (int)ResNX.GFX.TexClamp.ClampToEdge:
                    return STTextureWrapMode.Clamp;
                default:
                    return STTextureWrapMode.Clamp;
            }
        }

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

                samplerHintTB.Text = matTex.Type.ToString();

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
                            textureBP.Image = Toolbox.Library.Imaging.GetLoadingImage();
                            textureBP.Image = bntx.Textures[name].GetBitmap();
                        }));
                        Thread.Start();
                    }
                }

                if (PluginRuntime.TextureCache.ContainsKey(name))
                {
                    Thread = new Thread((ThreadStart)(() =>
                    {
                        textureBP.Image = Toolbox.Library.Imaging.GetLoadingImage();
                        textureBP.Image = PluginRuntime.TextureCache[name].GetBitmap();
                    }));
                    Thread.Start();
                }

                foreach (BFRESGroupNode ftexCont in PluginRuntime.ftexContainers)
                {
                    if (ftexCont.ResourceNodes.ContainsKey(name))
                    {
                        FTEX ftex = ((FTEX)ftexCont.ResourceNodes[name]);
                        //Make sure to skip textures without image data! This is very important for botw tex2 files.
                        if (ftex.texture.Data == null || ftex.texture.Data.Length == 0)
                            continue;

                        Thread = new Thread((ThreadStart)(() =>
                        {
                            textureBP.Image = Toolbox.Library.Imaging.GetLoadingImage();
                            textureBP.Image = ftex.GetBitmap();
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
            tex.Type = Toolbox.Library.STGenericMatTexture.TextureType.Unknown;
            tex.WrapModeS = STTextureWrapMode.Repeat;
            tex.WrapModeT = STTextureWrapMode.Repeat;
            tex.WrapModeW = STTextureWrapMode.Clamp;

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

                var result = MessageBox.Show("NOTE! Removing texture maps could cause issues with shaders, do you want to continue?", "Material Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

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

         //   samplerHintTB.Text = GetHint(samplerHintTB.Text);

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
