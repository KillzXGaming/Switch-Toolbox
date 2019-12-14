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
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class GFLXMaterialEditor : STUserControl
    {
        private GFLXMaterialData MaterialData;

        private ImageList TextureIconList;

        private TextEditor JsonTextEditor;

        public GFLXMaterialEditor()
        {
            InitializeComponent();

            JsonTextEditor = new TextEditor();
            JsonTextEditor.Dock = DockStyle.Fill;
            JsonTextEditor.IsJson = true;
            stPanel7.Controls.Add(JsonTextEditor);

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
            stTabControl2.myBackColor = FormThemes.BaseTheme.FormBackColor;

            stPanel6.BackColor = FormThemes.BaseTheme.ListViewBackColor;

            TextureIconList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(22, 22),
            };

            listViewCustom1.LargeImageList = TextureIconList;
            listViewCustom1.SmallImageList = TextureIconList;

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();

            uvViewport1.UseGrid = false;

            ResetSliders();
        }

        private void ResetSliders()
        {
            param1CB.SetTheme();
            param2CB.SetTheme();
            param4CB.SetTheme();
            param3CB.SetTheme();

            param1CB.Value = 0;
            param2CB.Value = 0;
            param4CB.Value = 0;
            param3CB.Value = 0;

            translateXUD.Value = 0;
            translateYUD.Value = 0;
            scaleXUD.Value = 1;
            scaleYUD.Value = 1;
        }

        public void LoadMaterial(GFLXMaterialData materialData)
        {
            MaterialData = materialData;

            GFLXMaterialParamEditor paramEditor = new GFLXMaterialParamEditor();
            paramEditor.Dock = DockStyle.Fill;
            paramEditor.LoadParams(materialData);
            tabPage2.Controls.Add(paramEditor);

            Thread Thread = new Thread((ThreadStart)(() =>
            {
                foreach (var tex in materialData.TextureMaps)
                {
                    Bitmap image = null;

                    foreach (var bntx in PluginRuntime.bntxContainers)
                    {
                        if (bntx.Textures.ContainsKey(tex.Name)) {
                            try {
                                image = bntx.Textures[tex.Name].GetBitmap();
                            }
                            catch {
                                image = Properties.Resources.TextureError;
                            }
                        }
                    }

                    AddTexture(tex.Name, image);
                }
            }));
            Thread.Start();
        }

        private void AddTexture(string name, Bitmap image)
        {
            if (listViewCustom1.InvokeRequired)
            {
                listViewCustom1.Invoke((MethodInvoker)delegate {
                    // Running on the UI thread
                    ListViewItem item = new ListViewItem(name);
                    listViewCustom1.Items.Add(item);
                    if (image != null)
                    {
                        item.ImageIndex = TextureIconList.Images.Count;
                        TextureIconList.Images.Add(image);
                        var dummy = listViewCustom1.Handle;
                    }
                });
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0) {
                int index = listViewCustom1.SelectedIndices[0];
                var tex = MaterialData.TextureMaps[index];
                var gflxTex = ((GFLXTextureMap)tex).gflxTextureMap;
                stTextBox1.Text = tex.Name;
                stTextBox2.Text = tex.SamplerName;
                uvViewport1.ActiveTextureMap = tex;

                translateXUD.Value = tex.Transform.Translate.X;
                translateYUD.Value = tex.Transform.Translate.Y;
                scaleXUD.Value = tex.Transform.Scale.X;
                scaleYUD.Value = tex.Transform.Scale.Y;

                if (gflxTex.Params != null)
                {
                    wrapModeUCB.LoadEnum(typeof(FlatBuffers.Gfbmdl.WrapMode));
                    wrapModeUCB.SelectedItem = (FlatBuffers.Gfbmdl.WrapMode)gflxTex.Params.WrapModeX;

                    wrapModeVCB.LoadEnum(typeof(FlatBuffers.Gfbmdl.WrapMode));
                    wrapModeVCB.SelectedItem = (FlatBuffers.Gfbmdl.WrapMode)gflxTex.Params.WrapModeY;

                    wrapModeWCB.LoadEnum(typeof(FlatBuffers.Gfbmdl.WrapMode));
                    wrapModeWCB.SelectedItem = (FlatBuffers.Gfbmdl.WrapMode)gflxTex.Params.WrapModeZ;

                    param1CB.Value = gflxTex.Params.Unknown1;
                    param2CB.Value = gflxTex.Params.Unknown5;
                    param3CB.Value = gflxTex.Params.Unknown6;
                    param4CB.Value = gflxTex.Params.Unknown7;
                    param5CB.Value = gflxTex.Params.Unknown8;
                    param6CB.Value = gflxTex.Params.lodBias;
                }

                if (tex.Type == STGenericMatTexture.TextureType.Diffuse) {
                    transformParamTB.Text = "ColorUV";
                }
                else
                    transformParamTB.Text = "";

                //Load mapped meshes
                uvViewport1.ActiveObjects.Clear();
                foreach (var mesh in MaterialData.ParentModel.GenericMeshes)
                {
                    foreach (var poly in mesh.PolygonGroups)
                    {
                        if (poly.Material == MaterialData)
                            uvViewport1.ActiveObjects.Add(mesh);
                    }
                }

                uvViewport1.UpdateViewport();
            }
            else
            {
                ResetSliders();
                transformParamTB.Text = "";
                uvViewport1.ActiveTextureMap = null;
                uvViewport1.UpdateViewport();
            }
        }

        private void barSlider7_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void stTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stTabControl1.SelectedIndex == 2)
            {
                var text = MaterialData.ConvertToJson();
                JsonTextEditor.FillEditor(text);
            }
        }
    }
}
