using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.NodeWrappers;

namespace Switch_Toolbox.Library
{
    public class STGenericModel : STGenericWrapper
    {
        public STGenericModel()
        {
            Checked = true;
        }

        public override void OnClick(TreeView treeView)
        {

        }

        public List<STGenericObject> GetObjects()
        {
            List<STGenericObject> objects = new List<STGenericObject>();
            foreach (TreeNode node in Nodes)
            {
                if (node is STGenericObject)
                    objects.Add(node as STGenericObject);

                if (node.Nodes.Count > 0)
                {
                    foreach (TreeNode childNode in node.Nodes)
                        if (childNode is STGenericObject)
                            objects.Add(childNode as STGenericObject);
                }
            }

            return objects;
        }
    }
}
