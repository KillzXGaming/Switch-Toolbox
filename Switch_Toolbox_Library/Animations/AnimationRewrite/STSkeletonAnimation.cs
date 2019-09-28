using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.Animations
{
    public class STSkeletonAnimation : STAnimation
    {
        public virtual STSkeleton GetActiveSkeleton()
        {
            Viewport viewport = LibraryGUI.GetActiveViewport();
            if (viewport == null) return null;

            foreach (var drawable in viewport.scene.objects) {
                if (drawable is STSkeleton)
                    return (STSkeleton)drawable;
            }

            return null;
        }
    }
}
