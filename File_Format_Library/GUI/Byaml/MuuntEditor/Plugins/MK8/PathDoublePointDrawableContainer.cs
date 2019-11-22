using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using FirstPlugin.Turbo.CourseMuuntStructs;

namespace FirstPlugin.MuuntEditor
{
    public class PathDoublePointDrawableContainer : PropertyObject, I2DDrawableContainer
    {
        public Color PathColor;

        public PathDoublePointDrawableContainer(string name, Color color)
        {
            Name = name;
            PathColor = color;
        }

        public List<BasePathGroup> PathGroups
        {
            get
            {
                var groups = new List<BasePathGroup>();
                foreach (var group in SubObjects)
                    groups.Add((BasePathGroup)group);
                return groups;
            }
        }

        private List<IDrawableObject> drawables;
        public List<IDrawableObject> Drawables
        {
            get
            {
                if (drawables == null)
                {
                    drawables = new List<IDrawableObject>();
                    drawables.Add(new RenderableDoublePointPath(PathGroups, PathColor));
                }
                return drawables;
            }
        }
    }
}
