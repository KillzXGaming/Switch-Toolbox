using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.ComponentModel;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class BaseControlPoint : IObject
    {
        [Browsable(false)]
        public Dictionary<string, dynamic> Prop { get; set; } = new Dictionary<string, dynamic>();

        public dynamic this[string name]
        {
            get
            {
                if (Prop.ContainsKey(name)) return Prop[name];
                else return null;
            }
            set
            {
                if (Prop.ContainsKey(name)) Prop[name] = value;
                else Prop.Add(name, value);
            }
        }

        public BaseControlPoint(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");
        }

        [Category("Transform")]
        public Vector3 Translate
        {
            get
            {
                return new Vector3(
                 this["X"] != null ? this["X"] : 0,
                 this["Y"] != null ? this["Y"] : 0,
                 this["Z"] != null ? this["Z"] : 0);
            }
            set
            {
                this["X"] = value.X;
                this["Y"] = value.Y;
                this["Z"] = value.Z;
            }
        }
    }
}
