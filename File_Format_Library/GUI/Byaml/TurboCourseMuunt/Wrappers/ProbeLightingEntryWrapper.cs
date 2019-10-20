using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.Rendering;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class ProbeLightingEntryWrapper : TreeNodeCustom
    {
        public ProbeLighting.Entry entry;

        public void OnChecked(bool IsChecked)
        {
            entry.IsVisable = IsChecked;
        }

        public ProbeLightingEntryWrapper(ProbeLighting.Entry config) {
            Text = config.Name;
            Checked = true;

            entry = config;
        }
    }
}
