using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using BcresLibrary;
using BcresLibrary.Enums;

namespace FirstPlugin
{
    public class MTOBWrapper : STGenericMaterial
    {
        internal BCRES BcresParent;
        internal Material Material;

        public MTOBWrapper()
        {
            ImageKey = "Material";
            SelectedImageKey = "Material";
        }
        public MTOBWrapper(BCRES bcres, Material material) : base() { Load(bcres, material); }

        public override void OnClick(TreeView treeview) {
            BcresParent.LoadEditors(this, OnPropertyChanged);
        }

        private void OnPropertyChanged()
        {

        }

        public void Load(BCRES bcres, Material material)
        {
            Material = material;
            BcresParent = bcres;

            Text = material.Name; 

            int textureUnit = 1;
            if (material.TextureMapInfo1 != null)
            {
                STGenericMatTexture tex1 = new STGenericMatTexture();
                var TexRef = material.TextureMapInfo1.TextureRef;
                var Sampler = material.TextureMapInfo1.Sampler;

                tex1.textureUnit = textureUnit++;
                tex1.Name = TexRef.Reference.Name;
                tex1.Type = STGenericMatTexture.TextureType.Diffuse;
                TextureMaps.Add(tex1);

                switch (material.TextureMapInfo1.WrapU)
                {
                    case PICATextureWrap.Repeat: tex1.wrapModeS = 0; break;
                    case PICATextureWrap.Mirror: tex1.wrapModeS = 1; break;
                    case PICATextureWrap.ClampToEdge: tex1.wrapModeS = 2; break;
                    case PICATextureWrap.ClampToBorder: tex1.wrapModeS = 2; break;
                }
                switch (material.TextureMapInfo1.WrapV)
                {
                    case PICATextureWrap.Repeat: tex1.wrapModeT = 0; break;
                    case PICATextureWrap.Mirror: tex1.wrapModeT = 1; break;
                    case PICATextureWrap.ClampToEdge: tex1.wrapModeT = 2; break;
                    case PICATextureWrap.ClampToBorder: tex1.wrapModeT = 2; break;
                }

                switch (material.TextureMapInfo1.MagFilter)
                {
                    case PICATextureFilter.Linear: tex1.magFilter = 0; break;
                    case PICATextureFilter.Nearest: tex1.magFilter = 1; break;
                }

                switch (material.TextureMapInfo1.MinFilter)
                {
                    case PICATextureFilter.Linear: tex1.minFilter = 0; break;
                    case PICATextureFilter.Nearest: tex1.minFilter = 1; break;
                }

                switch (material.TextureMapInfo1.MipFilter)
                {
                    case PICATextureFilter.Linear: tex1.mipDetail = 0; break;
                    case PICATextureFilter.Nearest: tex1.mipDetail = 1; break;
                }

                switch (material.TextureMapInfo1.WrapV)
                {
                    case PICATextureWrap.Repeat: tex1.wrapModeT = 0; break;
                    case PICATextureWrap.Mirror: tex1.wrapModeT = 1; break;
                    case PICATextureWrap.ClampToEdge: tex1.wrapModeT = 2; break;
                    case PICATextureWrap.ClampToBorder: tex1.wrapModeT = 2; break;
                }


                var wrapperTexMap = new BcresTextureMapWrapper(bcres, material.TextureMapInfo1, tex1);
                Nodes.Add(wrapperTexMap);
            }
        }
    }
}
