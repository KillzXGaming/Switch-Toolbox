using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.Animations
{
    public class STKeyFrame
    {
        public virtual float Slope { get; set; }
        public virtual float Frame { get; set; }
        public virtual float Value { get; set; }
    }

    public class STHermiteKeyFrame : STKeyFrame
    {
        public float Slope { get; set; }
    }

    public class STLinearKeyFrame : STKeyFrame
    {
        public float Delta { get; set; }
    }
}
