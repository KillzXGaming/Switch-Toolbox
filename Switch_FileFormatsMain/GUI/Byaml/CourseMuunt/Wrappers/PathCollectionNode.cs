using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class PathCollectionNode : TreeNodeCustom
    {
        public PathCollectionNode(string text) { Text = text; Checked = true; }

        public List<BasePathGroup> PathGroups = new List<BasePathGroup>();
    }
}
