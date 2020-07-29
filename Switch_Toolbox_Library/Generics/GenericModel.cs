using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.NodeWrappers;

namespace Toolbox.Library
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

        public virtual STSkeleton GenericSkeleton { get; set; }

        private IEnumerable<STGenericMaterial> _materials;
        private IEnumerable<STGenericObject> _objects;

        public virtual IEnumerable<STGenericMaterial> Materials
        {
            get
            {
                return _materials;
            }
            set
            {
                _materials = value; 
            }
        }

        public virtual IEnumerable<STGenericObject> Objects
        {
            get
            {
                return _objects;
            }
            set
            {
                _objects = value;
            }
        }
    }
}
