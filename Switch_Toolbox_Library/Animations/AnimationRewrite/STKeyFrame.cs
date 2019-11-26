using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.Animations
{
    public class STKeyFrame
    {
        public virtual float Frame { get; set; }
        public virtual float Value { get; set; }

        public virtual float Slope { get; set; }

        public STKeyFrame() { }

        public STKeyFrame(int frame, float value) {
            Frame = frame;
            Value = value;
        }

        public STKeyFrame(float frame, float value) {
            Frame = frame;
            Value = value;
        }
    }

    public class STCubicKeyFrame : STKeyFrame
    {
        public virtual float Coef0 { get; set; }
        public virtual float Coef1 { get; set; }
        public virtual float Coef2 { get; set; }
        public virtual float Coef3 { get; set; }
    }

    public class STHermiteKeyFrame : STKeyFrame
    {
        public virtual float TangentIn { get; set; }
        public virtual float TangentOut { get; set; }
    }
}
