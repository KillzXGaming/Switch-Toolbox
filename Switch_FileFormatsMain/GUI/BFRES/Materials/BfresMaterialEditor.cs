using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Syroot.NintenTools.NSW.Bfres;
using OpenTK;
using Switch_Toolbox.Library;
using Bfres.Structs;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class FMATEditor : UserControl
    {
        public FMAT material;

        public string SelectedMatParam = "";

        public FMATEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;

            if (!Runtime.IsDebugMode)
            {
                textBoxShaderArchive.ReadOnly = true;
                textBoxShaderModel.ReadOnly = true;
            }
        }
        private void listView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {

        }
        public void LoadMaterial(FMAT mat)
        {
            if (mat.MaterialU != null)
            {
                TabPage tabPage = null;

                foreach (TabPage page in stTabControl1.TabPages)
                    if (page.Text == "Render State")
                        tabPage = page;

                if (tabPage == null)
                {
                    tabPage = new TabPage();
                    tabPage.Text = "Render State";
                    stTabControl1.TabPages.Add(tabPage);
                }

                RenderStateEditor editor = new RenderStateEditor();
                editor.Dock = DockStyle.Fill;
                editor.LoadRenderState(mat.MaterialU.RenderState);
                tabPage.Controls.Add(editor);
            }

            material = mat;
            textBoxMaterialName.Text = material.Text;

            SetActiveGameByShader(material.shaderassign.ShaderArchive, material.shaderassign.ShaderModel);

            chkboxVisible.Checked = mat.Enabled;

            FillForm();

            uvEditor1.ActiveObjects.Clear();
            uvEditor1.Textures.Clear();

            foreach (var shp in ((FMDL)material.Parent.Parent).shapes)
            {
                if (shp.GetMaterial().Text == material.Text)
                {
                    uvEditor1.ActiveObjects.Add(shp);
                }
            }

            foreach (var ftexContainer in PluginRuntime.ftexContainers)
            {
                foreach (var texmap in material.TextureMaps)
                {
                    if (ftexContainer.ResourceNodes.ContainsKey(texmap.Name))
                    {
                        uvEditor1.Textures.Add(LoadTextureUvMap(texmap, (FTEX)ftexContainer.ResourceNodes[texmap.Name]));
                    }
                }
            }

            foreach (var bntx in PluginRuntime.bntxContainers)
            {
                foreach (var texmap in material.TextureMaps)
                {
                    if (bntx.Textures.ContainsKey(texmap.Name))
                    {
                        uvEditor1.Textures.Add(LoadTextureUvMap(texmap, bntx.Textures[texmap.Name]));
                    }
                }
            }

            uvEditor1.Reset();
            uvEditor1.ActiveMaterial = material;
            uvEditor1.Refresh();
        }
        private UVEditor.ActiveTexture LoadTextureUvMap(STGenericMatTexture texmap, STGenericTexture genericTexture)
        {
            Vector2 scale = new Vector2(1);
            Vector2 trans = new Vector2(0);

            var TextureMap = new UVEditor.ActiveTexture();

            if (texmap.Type == STGenericMatTexture.TextureType.Normal &&
                  material.shaderassign.options.ContainsKey("uking_texture2_texcoord"))
            {
                float value = float.Parse(material.shaderassign.options["uking_texture2_texcoord"]);

                if (value == 1)
                    TextureMap.UvChannelIndex = 1;
            }
            if (texmap.Type == STGenericMatTexture.TextureType.Specular &&
                  material.shaderassign.options.ContainsKey("uking_texture2_texcoord"))
            {
                float value = float.Parse(material.shaderassign.options["uking_texture2_texcoord"]);

                if (value == 1)
                    TextureMap.UvChannelIndex = 1;
            }
            if (texmap.Type == STGenericMatTexture.TextureType.Shadow &&
                material.matparam.ContainsKey("gsys_bake_st0"))
            {
                TextureMap.UvChannelIndex = 1;
                TextureMap.UVScale = Utils.ToVec4(material.matparam["gsys_bake_st0"].ValueFloat).Xy;
                TextureMap.UVTranslate = Utils.ToVec4(material.matparam["gsys_bake_st0"].ValueFloat).Zw;
            }
            if (texmap.Type == STGenericMatTexture.TextureType.Light &&
                 material.matparam.ContainsKey("gsys_bake_st1"))
            {
                TextureMap.UvChannelIndex = 1;
                TextureMap.UVScale = Utils.ToVec4(material.matparam["gsys_bake_st1"].ValueFloat).Xy;
                TextureMap.UVTranslate = Utils.ToVec4(material.matparam["gsys_bake_st1"].ValueFloat).Zw;
            }
            TextureMap.wrapModeS = texmap.wrapModeS;
            TextureMap.wrapModeT = texmap.wrapModeT;
            TextureMap.mipDetail = texmap.mipDetail;
            TextureMap.minFilter = texmap.minFilter;
            TextureMap.magFilter = texmap.mapMode;
            TextureMap.mapMode = texmap.mapMode;

            TextureMap.texture = genericTexture;
            return TextureMap;
        }
        private void SetActiveGameByShader(string ShaderName, string ShaderMdlName)
        {
            textBoxShaderArchive.Text = ShaderName;
            textBoxShaderModel.Text = ShaderMdlName;
        }
        public void FillForm()
        {
           InitializeUserDataList(material);

            samplerEditor1.InitializeTextureListView(material);
            shaderOptionsEditor1.InitializeShaderOptionList(material);
            shaderParamEditor1.InitializeShaderParamList(material);
            renderInfoEditor1.InitializeRenderInfoList(material);

            if (material.MaterialU != null)
            {
            }
        }

        private void InitializeUserDataList( FMAT material)
        {
            if (material.MaterialU != null)
            {
                userDataEditor.LoadUserData(material.MaterialU.UserData);
            }
            else
            {
                userDataEditor.LoadUserData(material.Material.UserDatas.ToList());
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

        private void chkboxVisible_CheckedChanged(object sender, EventArgs e)
        {
            material.Enabled = chkboxVisible.Checked;
        }

        private void textureRefListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void uvEditor1_Load(object sender, EventArgs e)
        {
     
        }

        private void uvEditor1_Click(object sender, EventArgs e)
        {
      
        }

        private void btnViotileFlags_Click(object sender, EventArgs e)
        {
            VolatileFlagEditor editor = new VolatileFlagEditor();

            List<bool> ViotileFlags = new List<bool>();
            if (material.MaterialU != null)
            {
                byte[] flags = material.MaterialU.VolatileFlags;

                if (flags == null)
                    return;

                for (int i = 0; i < flags.Length; i++)
                    ViotileFlags.Add(flags[i] == 1 ? true : false );
                
            }
            else
            {
                byte[] flags = material.Material.VolatileFlags;

                for (int i = 0; i < flags.Length; i++)
                    ViotileFlags.Add(flags[i] == 1 ? true : false);
            }

            editor.LoadFlags(ViotileFlags.ToArray());
            editor.Show(this);
        }

        private void btnSamplerInputEditor_Click(object sender, EventArgs e)
        {
            SamplerInputListEdit editor = new SamplerInputListEdit();
            editor.LoadSamplers(material.shaderassign.samplers);

            if (editor.ShowDialog() == DialogResult.OK) {
                material.shaderassign.attributes = editor.GetNewInputs();
            }
        }

        private void btnAttributeInputEditor_Click(object sender, EventArgs e)
        {
            VertexAttributeInputListEdit editor = new VertexAttributeInputListEdit();
            editor.LoadAtributes(material.shaderassign.attributes);

            if (editor.ShowDialog() == DialogResult.OK) {
                material.shaderassign.attributes = editor.GetNewInputs();
            }
        }
    }
}
