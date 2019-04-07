using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class PathPointNode : TreeNodeCustom
    {
        public PathPointNode(string text) { Text = text; Checked = true; }

        public PathPoint PathPoint;

        public void OnChecked(bool Checked)
        {
            if (PathPoint != null)
                PathPoint.RenderablePoint.Visible = Checked;
        }
    }
}
