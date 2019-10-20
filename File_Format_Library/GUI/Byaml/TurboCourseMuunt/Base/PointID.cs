using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class PointID : IObject
    {
        public int PathID
        {
            get
            {
                return this["PathId"];
            }
            set
            {
                this["PathId"] = value;
            }
        }

        public int PtID
        {
            get
            {
                return this["PtId"];
            }
            set
            {
                this["PtId"] = value;
            }
        }

        public PointID(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");
        }

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
    }

}
