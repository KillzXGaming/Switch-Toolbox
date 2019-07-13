using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Forms;
using SuperBMDLib.Materials;

namespace FirstPlugin
{
    public class BMDMaterialWrapper : STGenericMaterial
    {
        public Material Material;
        SuperBMDLib.Model ParentModel;

        public bool isTransparent
        {
            get
            {
                return (Material.Flag & 3) == 0;
            }
        }

        public BMDMaterialWrapper(Material mat, SuperBMDLib.Model model)
        {
            Material = mat;
            ParentModel = model;

            Text = mat.Name;

            int textureUnit = 1;
            if (mat.TextureIndices[0] != -1)
            {
                int texIndex = mat.TextureIndices[0];

                BMDTextureMap matTexture = new BMDTextureMap(this);
                matTexture.TextureIndex = texIndex;
                matTexture.Name = ParentModel.Textures[texIndex].Name;
                matTexture.Type = STGenericMatTexture.TextureType.Diffuse;
                matTexture.textureUnit = textureUnit++;

                matTexture.wrapModeS = (int)GXToOpenGL.GetWrapMode(ParentModel.Textures[texIndex].WrapS);
                matTexture.wrapModeT = (int)GXToOpenGL.GetWrapMode(ParentModel.Textures[texIndex].WrapT);
                matTexture.minFilter = (int)GXToOpenGL.GetMinFilter(ParentModel.Textures[texIndex].MinFilter);
                matTexture.magFilter = (int)GXToOpenGL.GetMinFilter(ParentModel.Textures[texIndex].MagFilter);

                TextureMaps.Add(matTexture);

                foreach (var textureIndex in mat.TextureIndices)
                {
                    if (textureIndex != -1)
                        Nodes.Add(ParentModel.Textures[textureIndex].Name);
                }
            }
        }

        public override void OnClick(TreeView treeView)
        {
            STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                LibraryGUI.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.LoadProperty(Material, null);
        }
    }
}
