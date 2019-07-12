using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library;
using SuperBMDLib.Materials;

namespace FirstPlugin
{
    public class BMDMaterialWrapper : STGenericMaterial
    {
        Material Material;
        SuperBMDLib.Model ParentModel;

        public BMDMaterialWrapper(Material mat, SuperBMDLib.Model model)
        {
            Material = mat;
            ParentModel = model;

            int textureUnit = 1;
            if (mat.TextureIndices[0] != -1)
            {
                int texIndex = mat.TextureIndices[0];

                STGenericMatTexture matTexture = new STGenericMatTexture();
                matTexture.Name = ParentModel.Textures[texIndex].Name;
                matTexture.Type = STGenericMatTexture.TextureType.Diffuse;
                matTexture.textureUnit = textureUnit++;
                matTexture.wrapModeS = ConvertWrapMode(ParentModel.Textures[texIndex].WrapS);
                matTexture.wrapModeT = ConvertWrapMode(ParentModel.Textures[texIndex].WrapT);

                TextureMaps.Add(matTexture);
            }
        }

        private int ConvertWrapMode(BinaryTextureImage.WrapModes WrapMode)
        {
            switch (WrapMode)
            {
                case BinaryTextureImage.WrapModes.Repeat:
                    return 0;
                case BinaryTextureImage.WrapModes.MirroredRepeat:
                    return 1;
                case BinaryTextureImage.WrapModes.ClampToEdge:
                    return 2;
                default:
                    throw new Exception($"Unknown WrapMode {WrapMode}");
            }
        }
    }
}
