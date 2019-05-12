using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library;
using BcresLibrary;
using Switch_Toolbox.Library.NodeWrappers;
using System.Windows.Forms;

namespace FirstPlugin
{
    public class BcresTextureMapWrapper : STGenericWrapper
    {
        internal BCRES BcresParent;

        public STGenericMatTexture GenericMatTexture;

        public override void OnClick(TreeView treeview) {
            BcresParent.LoadEditors(this, OnPropertyChanged);
        }

        private void OnPropertyChanged() { }

        public BcresTextureMapWrapper(BCRES bcres, TextureMapInfo texInfo, STGenericMatTexture texture)
        {
            CanRename = false;
            CanReplace = false;

            BcresParent = bcres;

            Text = texture.Name;
            GenericMatTexture = texture;
            TextureMapInfo = texInfo;

            ImageKey = "TextureMaterialMap";
            SelectedImageKey = "TextureMaterialMap";
        }

        public TextureMapInfo TextureMapInfo { get; set; }
    }
}
