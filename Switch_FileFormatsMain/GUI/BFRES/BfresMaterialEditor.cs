using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Syroot.NintenTools.NSW.Bfres;
using OpenTK;
using Switch_Toolbox.Library;
using Bfres.Structs;

namespace FirstPlugin
{
    public partial class FMATEditor : UserControl
    {
        public FMAT material;

        public BFRESRender bfresRender;

        public ImageList textureImageList;
        public string SelectedMatParam = "";
        public ImageList il = new ImageList();

        public FMATEditor()
        {
            InitializeComponent();

            textureImageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(70, 70),
            };

            if (!Runtime.IsDebugMode)
            {
                textBoxShaderArchive.ReadOnly = true;
                textBoxShaderModel.ReadOnly = true;
            }
            shaderOptionsListView.ListViewItemSorter = new Sorter();
        }
        private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {

        }
        public void LoadMaterial(FMAT mat, BFRESRender bfres)
        {
            bfresRender = bfres;
            material = mat;
            textBoxMaterialName.Text = material.Text;

            SetActiveGameByShader(material.shaderassign.ShaderArchive, material.shaderassign.ShaderModel);

            chkboxVisible.Checked = mat.Enabled;

            FillForm();
        }
        private void SetActiveGameByShader(string ShaderName, string ShaderMdlName)
        {
            textBoxShaderArchive.Text = ShaderName;
            textBoxShaderModel.Text = ShaderMdlName;
        }
        public void FillForm()
        {
            InitializeTextureListView(material);
            InitializeRenderInfoList(material);
            InitializeShaderParamList(material);
            InitializeShaderOptionList(material);
        }
        private string SetValueToString(object values)
        {
            if (values is float[])
                return string.Join(" , ", values as float[]);
            else if (values is bool[])
                return string.Join(" , ", values as bool[]);
            else if (values is int[])
                return string.Join(" , ", values as int[]);
            else if (values is uint[])
                return string.Join(" , ", values as uint[]);
            else
                return "";
        }
        private void InitializeShaderOptionList(FMAT material)
        {
            shaderOptionsListView.Items.Clear();

            foreach (var option in material.shaderassign.options)
            {
                shaderOptionsListView.Items.Add(option.Key).SubItems.Add(option.Value);
            }
        }
        private void InitializeShaderParamList(FMAT material)
        {
            listView1.Items.Clear();

            int CurParam = 0;
            foreach (var prm in material.matparam.Values)
            {
                string DisplayValue = "";

                var item = new ListViewItem(prm.Name);

                Color SetColor = Color.FromArgb(40, 40, 40);
                Vector4 col = new Vector4();

                bool IsColor = prm.Name.Contains("Color") || prm.Name.Contains("color");

                switch (prm.Type)
                {
                    case ShaderParamType.Float:
                    case ShaderParamType.Float2:
                    case ShaderParamType.Float2x2:
                    case ShaderParamType.Float2x3:
                    case ShaderParamType.Float2x4:
                    case ShaderParamType.Float3x2:
                    case ShaderParamType.Float3x3:
                    case ShaderParamType.Float3x4:
                    case ShaderParamType.Float4x2:
                    case ShaderParamType.Float4x3:
                    case ShaderParamType.Float4x4:
                        DisplayValue = SetValueToString(prm.ValueFloat);
                        break;
                    case ShaderParamType.Float3:
                        DisplayValue = SetValueToString(prm.ValueFloat);
                        col = new Vector4(prm.ValueFloat[0], prm.ValueFloat[1], prm.ValueFloat[2], 1);
                        break;
                    case ShaderParamType.Float4:
                        DisplayValue = SetValueToString(prm.ValueFloat);
                        col = new Vector4(prm.ValueFloat[0], prm.ValueFloat[1], prm.ValueFloat[2], prm.ValueFloat[3]);
                        break;
                }
                if (IsColor)
                {
                    int someIntX = (int)Math.Ceiling(col.X * 255);
                    int someIntY = (int)Math.Ceiling(col.Y * 255);
                    int someIntZ = (int)Math.Ceiling(col.Z * 255);
                    int someIntW = (int)Math.Ceiling(col.W * 255);
                    if (someIntX <= 255 && someIntY <= 255 && someIntZ <= 255 && someIntW <= 255)
                    {
                        Console.WriteLine($"{prm.Name} R {someIntX} G {someIntY} B {someIntZ}");

                        SetColor = Color.FromArgb(
                    255,
                    someIntX,
                    someIntY,
                    someIntZ
                    );
                    }
                }

                item.UseItemStyleForSubItems = false;
                item.SubItems.Add(DisplayValue);
                item.SubItems.Add("");
                item.SubItems[2].BackColor = SetColor;
                listView1.View = View.Details;
                listView1.Items.Add(item);
                CurParam++;
            }
            il.ImageSize = new Size(10, 10);
            listView1.SmallImageList = il;
            listView1.FullRowSelect = true;
        }
        public void InitializeTextureListView(FMAT material)
        {
            textureRefListView.Items.Clear();
            textureRefListView.SmallImageList = textureImageList;
            textureRefListView.FullRowSelect = true;

            foreach (MatTexture tex in material.textures)
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
            bool FoundTexture = false;
            foreach (ListViewItem item in textureRefListView.Items)
            {
                foreach (BinaryTextureContainer bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(item.Text))
                    {
                        FoundTexture = true;

                        TextureData tex = bntx.Textures[item.Text];
                        TextureData.BRTI_Texture renderedTex = tex.renderedGLTex;
                        Bitmap temp = tex.GLTextureToBitmap(renderedTex, renderedTex.display);

                        textureImageList.Images.Add(tex.Text, temp);

                        item.ImageIndex = CurTex++;

                        var dummy = textureImageList.Handle;
                        temp.Dispose();
                    }
                }
                foreach (FTEXContainer ftexCont in PluginRuntime.ftexContainers)
                {
                    if (ftexCont.Textures.ContainsKey(item.Text))
                    {
                        FoundTexture = true;

                        FTEX tex = ftexCont.Textures[item.Text];
                        FTEX.RenderableTex renderedTex = tex.renderedTex;
                        Bitmap temp = tex.GLTextureToBitmap(renderedTex, renderedTex.display);

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
        private void InitializeRenderInfoList(FMAT material)
        {
            renderInfoListView.Items.Clear();

            foreach (var rnd in material.renderinfo)
            {
                ListViewItem item = new ListViewItem();
                item.Text = rnd.Name;

                string Value = "";
                switch (rnd.Type)
                {
                    case RenderInfoType.Int32:
                        Value = string.Join(",", rnd.ValueInt);
                        break;
                    case RenderInfoType.Single:
                        Value = string.Join(",", rnd.ValueFloat);
                        break;
                    case RenderInfoType.String:
                        Value = string.Join(",", rnd.ValueString);
                        break;
                }
                item.SubItems.Add(Value);
                item.SubItems.Add(rnd.Type.ToString());
                renderInfoListView.Items.Add(item);
            }
            renderInfoListView.FullRowSelect = true;
        }

        private void textureRefListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            if ((e.ItemIndex % 2) == 1)
            {
                e.Item.BackColor = Color.FromArgb(50, 50, 50);
                e.Item.UseItemStyleForSubItems = true;
            }
        }

        private void FSHPEditor_DockStateChanged(object sender, EventArgs e)
        {
            DockContent doc = sender as DockContent;
            if (doc != null)
            {
                PluginRuntime.FSHPDockState = doc.DockState;

                Console.WriteLine(doc.DockState);
                Console.WriteLine(doc);


                if (doc.DockState != DockState.Unknown)
                    Config.Save();
            }
        }

        private void textureMapTab_DoubleClick(object sender, EventArgs e)
        {
        
        }

        private void textureRefListView_DoubleClick(object sender, EventArgs e)
        {
            Console.WriteLine("click");
            int index = textureRefListView.SelectedIndices[0];
            Texture_Selector tex = new Texture_Selector();
            tex.LoadTexture();
            if (tex.ShowDialog() == DialogResult.OK)
            {
                material.textures[index].Name = tex.GetSelectedTexture();
                InitializeTextureListView(material);
                bfresRender.UpdateSingleMaterialTextureMaps(material);
            }
        }

        private void textureRefListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(50, 50, 50)))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }
            using (SolidBrush foreBrush = new SolidBrush(Color.FromArgb(255, 255, 255)))
            {
                e.Graphics.DrawString(e.Header.Text, e.Font, foreBrush, e.Bounds);
            }
        }
    
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (material.matparam.ContainsKey(listView1.SelectedItems[0].Text))
                {
                    int index = listView1.SelectedIndices[0];

                    SetParamDialog paramDialog = new SetParamDialog();
                    paramDialog.LoadParam(material.matparam[listView1.SelectedItems[0].Text]);
                   
                    if (paramDialog.ShowDialog() == DialogResult.OK)
                    {
                        paramDialog.GetValues();
                        InitializeShaderParamList(material);
                        listView1.Items[index].Selected = true;
                    }
                }
            }
        }

        private void smallToolStripMenuItem_Click(object sender, EventArgs e)
        {
          /*Font normalfont = new Font("Microsoft Sans Serif", 10f);
            foreach (ListViewItem lvi in listView1.Items) lvi.Font = normalfont;
            foreach (ListViewItem lvi in listView1.Items) lvi.SubItems[0].Font = normalfont;
            SetHeight(listView1, 11);*/
        }

        private void SetHeight(ListView listView, int height)
        {
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, height);
            listView.SmallImageList = imgList;
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void largeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void renderInfoListView_DoubleClick(object sender, EventArgs e)
        {
            int ActiveInfoIndex = renderInfoListView.SelectedIndices[0];
            if (renderInfoListView.SelectedItems.Count > 0)
            {
                foreach (var info in material.renderinfo)
                {
                    if (info.Name == renderInfoListView.SelectedItems[0].Text)
                    {
                        RenderInfoValueEditor editor = new RenderInfoValueEditor();
                        editor.LoadValues(info);
                        if (editor.ShowDialog() == DialogResult.OK)
                        {
                            info.ValueFloat = editor.valueFloats.ToArray();
                            info.ValueString = editor.valueStrings.ToArray();
                            info.ValueInt = editor.valueInts.ToArray();

                            ListViewItem item = new ListViewItem();
                            item.Text = info.Name;

                            string Value = "";
                            switch (info.Type)
                            {
                                case RenderInfoType.Int32:
                                    Value = string.Join(",", info.ValueInt);
                                    break;
                                case RenderInfoType.Single:
                                    Value = string.Join(",", info.ValueFloat);
                                    break;
                                case RenderInfoType.String:
                                    Value = string.Join(",", info.ValueString);
                                    break;
                            }
                            item.SubItems.Add(Value);
                            item.SubItems.Add(info.Type.ToString());
                     //       renderInfoListView.Items[ActiveInfoIndex] = item;

                            //     InitializeRenderInfoList(material);
                        }
                    }
                }
           }
        }
 

        private void btnExportParams_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Material Params|*.xml;";
            sfd.DefaultExt = ".xml";
            sfd.FileName = material.Text + ".MatParams";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FMAT2XML.Save(material, sfd.FileName, true);
            }
        }

        private void btnReplaceParams_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Material Params|*.xml;";
            ofd.DefaultExt = ".xml";
            ofd.FileName = material.Text + ".MatParams";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FMAT2XML.Read(material, ofd.FileName, true);
            }
        }

        private void shaderOptionsListView_DoubleClick(object sender, EventArgs e)
        {
            int ActiveIndex = shaderOptionsListView.SelectedIndices[0];
            if (shaderOptionsListView.SelectedItems.Count > 0)
            {
                string Key = shaderOptionsListView.SelectedItems[0].Text;
                string Value = shaderOptionsListView.SelectedItems[0].SubItems[1].Text;

                BfresShaderOptionsEditor edtior = new BfresShaderOptionsEditor();
                edtior.LoadOption(Key, Value);

                if (edtior.ShowDialog() == DialogResult.OK)
                {
                    material.shaderassign.options[shaderOptionsListView.SelectedItems[0].Text] = edtior.textBoxValue.Text;

                    InitializeShaderOptionList(material);

                    shaderOptionsListView.Items[ActiveIndex].Selected = true;
                    shaderOptionsListView.Select();
                }
            }
        }

        private void chkboxVisible_CheckedChanged(object sender, EventArgs e)
        {
            material.Enabled = chkboxVisible.Checked;
        }

        private void btnSamplerEditor_Click(object sender, EventArgs e)
        {
            SamplerEditor samplerEditor = new SamplerEditor();
            foreach (MatTexture tex in material.textures)
            {
                if (tex.Name == textureRefListView.SelectedItems[0].Text)
                {
                    samplerEditor.LoadSampler(tex);
                }
            }
             
            if (samplerEditor.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void textureRefListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (textureRefListView.SelectedItems.Count > 0)
            {
                btnSamplerEditor.Enabled = true;
            }
        }

        private void shaderOptionsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            Sorter s = (Sorter)shaderOptionsListView.ListViewItemSorter;
            s.Column = e.Column;

            if (s.Order == SortOrder.Ascending)
            {
                s.Order = SortOrder.Descending;
            }
            else
            {
                s.Order = SortOrder.Ascending;
            }
            shaderOptionsListView.Sort();
        }
        class Sorter : System.Collections.IComparer
        {
            public int Column = 0;
            public System.Windows.Forms.SortOrder Order = SortOrder.Ascending;
            public int Compare(object x, object y) // IComparer Member
            {
                if (!(x is ListViewItem))
                    return (0);
                if (!(y is ListViewItem))
                    return (0);

                ListViewItem l1 = (ListViewItem)x;
                ListViewItem l2 = (ListViewItem)y;

                if (l1.ListView.Columns[Column].Tag == null)
                {
                    l1.ListView.Columns[Column].Tag = "Text";
                }

                if (l1.ListView.Columns[Column].Tag.ToString() == "Numeric")
                {
                    float fl1 = float.Parse(l1.SubItems[Column].Text);
                    float fl2 = float.Parse(l2.SubItems[Column].Text);

                    if (Order == SortOrder.Ascending)
                    {
                        return fl1.CompareTo(fl2);
                    }
                    else
                    {
                        return fl2.CompareTo(fl1);
                    }
                }
                else
                {
                    string str1 = l1.SubItems[Column].Text;
                    string str2 = l2.SubItems[Column].Text;

                    if (Order == SortOrder.Ascending)
                    {
                        return str1.CompareTo(str2);
                    }
                    else
                    {
                        return str2.CompareTo(str1);
                    }
                }
            }
        }
    }
}
