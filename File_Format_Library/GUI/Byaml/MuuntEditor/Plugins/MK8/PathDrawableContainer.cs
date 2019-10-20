using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstPlugin.Turbo.CourseMuuntStructs;

namespace FirstPlugin.MuuntEditor
{
    public class PathDrawableContainer : ObjectGroup, IDrawableContainer
    {
        public PathDrawableContainer(string name) {
            Name = name;
        }

        public List<BasePathGroup> PathGroups
        {
            get
            {
                var groups = new List<BasePathGroup>();
                foreach (var group in Objects)
                    groups.Add((BasePathGroup)group);
                return groups;
            }
        }

        private IDrawableObject drawable;
        public IDrawableObject Drawable
        {
            get
            {
                if (drawable == null)
                    drawable = new RenderablePath(PathGroups); 

                return drawable;
            }
        }
    }
}
