using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class ProbeLightingWrapper : TreeNodeCustom
    {
        public ProbeLighting ProbeLightingConfig;

        public ProbeLightingWrapper(ProbeLighting config) {
            Text = "course.bglpbd (Probe Lighting)";
            ProbeLightingConfig = config;
            Checked = true;
        }
    }
}
