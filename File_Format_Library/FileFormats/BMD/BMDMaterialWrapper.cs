using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
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

                matTexture.WrapModeS = ConvertWrapMode(ParentModel.Textures[texIndex].WrapS);
                matTexture.WrapModeT = ConvertWrapMode(ParentModel.Textures[texIndex].WrapT);
                matTexture.MinFilter = ConvertMinFilter(ParentModel.Textures[texIndex].MinFilter);
                matTexture.MagFilter = ConvertMagFilter(ParentModel.Textures[texIndex].MagFilter);

                TextureMaps.Add(matTexture);

                foreach (var textureIndex in mat.TextureIndices)
                {
                    if (textureIndex != -1)
                        Nodes.Add(ParentModel.Textures[textureIndex].Name);
                }
            }
        }

        private STTextureWrapMode ConvertWrapMode(BinaryTextureImage.WrapModes wrapMode)
        {
            switch (wrapMode)
            {
                case BinaryTextureImage.WrapModes.Repeat: return STTextureWrapMode.Repeat;
                case BinaryTextureImage.WrapModes.MirroredRepeat: return STTextureWrapMode.Mirror;
                case BinaryTextureImage.WrapModes.ClampToEdge: return STTextureWrapMode.Clamp;
                default:
                    return STTextureWrapMode.Repeat;
            }
        }

        private STTextureMinFilter ConvertMinFilter(BinaryTextureImage.FilterMode filterMode)
        {
            switch (filterMode)
            {
                case BinaryTextureImage.FilterMode.Linear: return STTextureMinFilter.Linear;
                case BinaryTextureImage.FilterMode.LinearMipmapLinear: return STTextureMinFilter.Linear;
                case BinaryTextureImage.FilterMode.LinearMipmapNearest: return STTextureMinFilter.LinearMipMapNearest;
                case BinaryTextureImage.FilterMode.Nearest: return STTextureMinFilter.Nearest;
                case BinaryTextureImage.FilterMode.NearestMipmapLinear: return STTextureMinFilter.NearestMipmapLinear;
                case BinaryTextureImage.FilterMode.NearestMipmapNearest: return STTextureMinFilter.NearestMipmapNearest;
                default:
                    return STTextureMinFilter.Linear;
            }
        }

        private STTextureMagFilter ConvertMagFilter(BinaryTextureImage.FilterMode filterMode)
        {
            switch (filterMode)
            {
                case BinaryTextureImage.FilterMode.Linear: return STTextureMagFilter.Linear;
                case BinaryTextureImage.FilterMode.LinearMipmapLinear: return STTextureMagFilter.Linear;
                case BinaryTextureImage.FilterMode.LinearMipmapNearest: return STTextureMagFilter.Linear;
                case BinaryTextureImage.FilterMode.Nearest: return STTextureMagFilter.Nearest;
                case BinaryTextureImage.FilterMode.NearestMipmapLinear: return STTextureMagFilter.Nearest;
                case BinaryTextureImage.FilterMode.NearestMipmapNearest: return STTextureMagFilter.Nearest;
                default:
                    return STTextureMagFilter.Linear;
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
