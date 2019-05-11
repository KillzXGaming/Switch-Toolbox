using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using BcresLibrary;

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
        public MTOBWrapper(Material material) : base() { Load(material); }

        public override void OnClick(TreeView treeview) {
            BcresParent.LoadEditors(this, OnPropertyChanged);
        }

        private void OnPropertyChanged()
        {

        }

        public void Load(Material material)
        {
            Material = material;

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
            }
        }
    }
}
