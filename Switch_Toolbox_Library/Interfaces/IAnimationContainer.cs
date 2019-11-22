using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.Animations
{
    public interface IAnimationContainer
    {
        STAnimation AnimationController { get; }
    }
}
