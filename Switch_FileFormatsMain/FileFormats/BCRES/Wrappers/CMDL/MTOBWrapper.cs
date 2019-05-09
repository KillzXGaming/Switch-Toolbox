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
        }
    }
}
