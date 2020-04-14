using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.CtrH3D.Model;
using SPICA.PICA.Commands;
using System.Windows.Forms;
using FirstPlugin.CtrLibrary.Forms;
using Newtonsoft.Json;

namespace FirstPlugin.CtrLibrary
{
    public class H3DMaterialWrapper : STGenericMaterial, IContextMenuNode
    {
        public H3DModelWrapper ParentModel;
        public BCH ParentBCH;
        public H3DMaterial Material;

        public virtual List<STGenericObject> FindMappedMeshes()
        {
            List<STGenericObject> meshes = new List<STGenericObject>();
            foreach (var mesh in ParentModel.Meshes)
            {
                if (mesh.GetMaterial() == this)
                    meshes.Add(mesh);
            }
            return meshes;
        }

        public void UpdateViewport() {
            ParentBCH.UpdateViewport();
        }

        public override void OnClick(TreeView treeView)
        {
            var editor = ParentBCH.LoadEditor<BCHMaterialEditor>();
            editor.LoadMaterial(this);
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Export", null, ExportAction, Keys.Control | Keys.E));
            Items.Add(new ToolStripMenuItem("Replace", null, ReplaceAction, Keys.Control | Keys.R));
            Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.Control | Keys.N));

            return Items.ToArray();
        }

        private void RenameAction(object sender, EventArgs e)
        {

        }

        private void ExportAction(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.json;";
            sfd.FileName = Text;
            sfd.DefaultExt = "json";
            if (sfd.ShowDialog() == DialogResult.OK) {
                string ext = Utils.GetExtension(sfd.FileName);
                if (ext == ".json") {
                    string json = JsonConvert.SerializeObject(Material, Formatting.Indented);
                    System.IO.File.WriteAllText(sfd.FileName, json);
                }
            }
        }

        private void ReplaceAction(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.json;";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Material = JsonConvert.DeserializeObject<H3DMaterial>(
                    System.IO.File.ReadAllText(ofd.FileName));

                ReloadMaterial();

                var editor = ParentBCH.LoadEditor<BCHMaterialEditor>();
                editor.LoadMaterial(this);
            }
        }

        public H3DMaterialWrapper(H3DMaterial mat) : base()
        {
            Material = mat;

            ImageKey = "material";
            SelectedImageKey = "material";
        }

        public H3DMaterialWrapper(BCH bch, H3DModelWrapper model,  H3DMaterial mat) : base()
        {
            ParentModel = model;
            ParentBCH = bch;
            Material = mat;

            ImageKey = "material";
            SelectedImageKey = "material";

            ReloadMaterial();
        }

        private void ReloadMaterial()
        {
            for (int i = 0; i < Material.EnabledTextures?.Length; i++)
            {
                if (Material.EnabledTextures[i])
                {
                    H3DTextureMapWrapper matTexture = new H3DTextureMapWrapper();
                    if (i == 0) matTexture.Name = Material.Texture0Name;
                    if (i == 1) matTexture.Name = Material.Texture1Name;
                    if (i == 2) matTexture.Name = Material.Texture2Name;

                    if (Material.TextureMappers.Length > i)
                    {
                        var mapper = Material.TextureMappers[i];
                        if (TextureMaps.Count == 0) //first texture added
                        {
                            matTexture.Type = STGenericMatTexture.TextureType.Diffuse;
                        }

                        matTexture.WrapModeS = ConvertWrapMode(mapper.WrapU);
                        matTexture.WrapModeT = ConvertWrapMode(mapper.WrapV);
                    }

                    TextureMaps.Add(matTexture);
                }
            }
        }

        private static STTextureWrapMode ConvertWrapMode(PICATextureWrap wrapMode)
        {
            switch (wrapMode)
            {
                case PICATextureWrap.Repeat: return STTextureWrapMode.Repeat;
                case PICATextureWrap.Mirror: return STTextureWrapMode.Mirror;
                case PICATextureWrap.ClampToEdge: return STTextureWrapMode.Clamp;
                case PICATextureWrap.ClampToBorder: return STTextureWrapMode.Clamp;
                default:
                    return STTextureWrapMode.Repeat;
            }
        }
    }

    public class H3DTextureMapWrapper : STGenericMatTexture
    {
        public override STGenericTexture GetTexture()
        {
            foreach (var bch in PluginRuntime.bchTexContainers)
            {
                if (bch.ResourceNodes.ContainsKey(Name))
                    return (STGenericTexture)bch.ResourceNodes[Name];
            }
            return null;
        }
    }
}
