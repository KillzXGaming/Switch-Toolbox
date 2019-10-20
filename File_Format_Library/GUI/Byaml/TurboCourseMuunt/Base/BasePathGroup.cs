using System.Collections.Generic;
using FirstPlugin.MuuntEditor;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class BasePathGroup : PropertyObject
    {
        public bool ConnectGroups = true;

        public List<BasePathPoint> PathPoints = new List<BasePathPoint>();

        public override List<PropertyObject> SubObjects
        {
            get {
                var props = new List<PropertyObject>();
                foreach (var path in PathPoints)
                    props.Add(path);
                return props;
            }
        }
    }
}
