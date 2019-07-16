using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class PathGroupNode : TreeNodeCustom
    {
        public PathGroupNode(string text) { Text = text; Checked = true; }
    }
}
