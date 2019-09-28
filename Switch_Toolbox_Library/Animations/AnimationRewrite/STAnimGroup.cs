using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.Animations
{
    /// <summary>
    /// Represents a group that stores animation tracks and sub groups.
    /// </summary>
    public class STAnimGroup
    {
        public string Name { get; set; }

        public List<STAnimGroup> SubAnimGroups = new List<STAnimGroup>();
    }
}
