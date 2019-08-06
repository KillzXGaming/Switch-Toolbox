using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    /// <summary>
    /// Represets a container for a viewport.
    /// This is used to search for the viewport when it needs updating.
    /// </summary>
    public interface IViewportContainer
    {
        void UpdateViewport();
        Viewport GetViewport();
        AnimationPanel GetAnimationPanel();
    }
}
