using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library
{
    public class STGenericMaterial : TreeNodeCustom
    {
        public List<STGenericMatTexture> TextureMaps = new List<STGenericMatTexture>();

        public override void OnClick(TreeView treeView)
        {

        }
    }
}
