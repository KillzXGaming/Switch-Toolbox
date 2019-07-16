using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.Animations
{
    public class BooleanKeyFrame
    {
        public InterpolationType InterType = InterpolationType.STEPBOOL;

        public bool Visible;
        public bool IsKeyed = false;

        public float Frame
        {
            get { return _frame; }
            set { _frame = value; }
        }
        private float _frame;

    }
}
