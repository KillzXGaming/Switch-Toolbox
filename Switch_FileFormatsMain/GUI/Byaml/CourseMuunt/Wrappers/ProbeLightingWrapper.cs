using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.Rendering;

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
