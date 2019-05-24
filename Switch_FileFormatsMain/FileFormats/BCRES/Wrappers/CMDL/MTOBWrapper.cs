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
using OpenTK;

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

        public TexCoord1 TexCoord1Buffer;
        public struct TexCoord1
        {
            public Vector2 Translate;
            public float Rotate;
            public Vector2 Scale;

            public static readonly int Size =
            BlittableValueType<TexCoord1>.Stride;
        }

        public void Load(BCRES bcres, Material material)
        {
            Material = material;
            BcresParent = bcres;

            Text = material.Name;

            if (material.TexCoordSources.Count > 0)
            {
                TexCoord1Buffer = new TexCoord1()
                {
                    Translate = Utils.ToVec2(material.TexCoordSources[0].Translate),
                    Rotate = material.TexCoordSources[0].Rotate,
                    Scale = Utils.ToVec2(material.TexCoordSources[0].Scale),
                };


            }

            int textureUnit = 1;
            if (material.TextureMapInfo1 != null)
                Nodes.Add(CreateGenericMatTexture(textureUnit, bcres, material.TextureMapInfo1));
            if (material.TextureMapInfo2 != null)
                Nodes.Add(CreateGenericMatTexture(textureUnit, bcres, material.TextureMapInfo2));
            if (material.TextureMapInfo3 != null)
                Nodes.Add(CreateGenericMatTexture(textureUnit, bcres, material.TextureMapInfo3));
        }

        private BcresTextureMapWrapper CreateGenericMatTexture(int textureUnit, BCRES bcres, TextureMapInfo TextureMapInfo)
        {
            STGenericMatTexture tex1 = new STGenericMatTexture();
            var TexRef = TextureMapInfo.TextureRef;
            var Sampler = TextureMapInfo.Sampler;

            tex1.textureUnit = textureUnit++;
            tex1.Name = TexRef.Reference.Name;
            tex1.Type = STGenericMatTexture.TextureType.Diffuse;
            TextureMaps.Add(tex1);

            switch (TextureMapInfo.WrapU)
            {
                case PICATextureWrap.Repeat: tex1.wrapModeS = 0; break;
                case PICATextureWrap.Mirror: tex1.wrapModeS = 1; break;
                case PICATextureWrap.ClampToEdge: tex1.wrapModeS = 2; break;
                case PICATextureWrap.ClampToBorder: tex1.wrapModeS = 2; break;
            }
            switch (TextureMapInfo.WrapV)
            {
                case PICATextureWrap.Repeat: tex1.wrapModeT = 0; break;
                case PICATextureWrap.Mirror: tex1.wrapModeT = 1; break;
                case PICATextureWrap.ClampToEdge: tex1.wrapModeT = 2; break;
                case PICATextureWrap.ClampToBorder: tex1.wrapModeT = 2; break;
            }

            switch (TextureMapInfo.MagFilter)
            {
                case PICATextureFilter.Linear: tex1.magFilter = 0; break;
                case PICATextureFilter.Nearest: tex1.magFilter = 1; break;
            }

            switch (TextureMapInfo.MinFilter)
            {
                case PICATextureFilter.Linear: tex1.minFilter = 0; break;
                case PICATextureFilter.Nearest: tex1.minFilter = 1; break;
            }

            switch (TextureMapInfo.MipFilter)
            {
                case PICATextureFilter.Linear: tex1.mipDetail = 0; break;
                case PICATextureFilter.Nearest: tex1.mipDetail = 1; break;
            }

            switch (TextureMapInfo.WrapV)
            {
                case PICATextureWrap.Repeat: tex1.wrapModeT = 0; break;
                case PICATextureWrap.Mirror: tex1.wrapModeT = 1; break;
                case PICATextureWrap.ClampToEdge: tex1.wrapModeT = 2; break;
                case PICATextureWrap.ClampToBorder: tex1.wrapModeT = 2; break;
            }


            var wrapperTexMap = new BcresTextureMapWrapper(bcres, TextureMapInfo, tex1);
            return wrapperTexMap;
        }
    }
}
