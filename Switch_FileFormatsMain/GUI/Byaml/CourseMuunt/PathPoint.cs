using System;
using System.Collections.Generic;
using System.ComponentModel;
using OpenTK;
using GL_EditorFramework.EditorDrawables;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class PathPoint : IObject
    {
        public RenderablePathPoint RenderablePoint
        {
            get
            {
                return new RenderablePathPoint(Translate, Rotate, Scale);
            }
        }

        public const string N_Translate = "Translate";
        public const string N_Rotate = "Rotate";
        public const string N_Scale = "Scale";
        public const string N_Id = "UnitIdNum";
        public const string N_ObjectID = "ObjId";

        public PathPoint(dynamic bymlNode)
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

        public List<object> Properties = new List<object>();

        public List<PointID> NextPoints = new List<PointID>();
        public List<PointID> PrevPoints = new List<PointID>();

        public List<ControlPoint> ControlPoints = new List<ControlPoint>();

        [Category("Rotate")]
        public Vector3 Rotate
        {
            get { return new Vector3(this[N_Rotate]["X"], this[N_Rotate]["Y"], this[N_Rotate]["Z"]); ; }
            set
            {
                this[N_Rotate]["X"] = value.X;
                this[N_Rotate]["Y"] = value.Y;
                this[N_Rotate]["Z"] = value.Z;
            }
        }

        [Category("Scale")]
        public Vector3 Scale
        {
            get { return new Vector3(this[N_Scale]["X"], this[N_Scale]["Y"], this[N_Scale]["Z"]); ; }
            set
            {
                this[N_Scale]["X"] = value.X;
                this[N_Scale]["Y"] = value.Y;
                this[N_Scale]["Z"] = value.Z;
            }
        }

        [Category("Translate")]
        public Vector3 Translate
        {
            get { return new Vector3(this[N_Translate]["X"], this[N_Translate]["Y"], this[N_Translate]["Z"]); ; }
            set
            {
                this[N_Translate]["X"] = value.X;
                this[N_Translate]["Y"] = value.Y;
                this[N_Translate]["Z"] = value.Z;
            }
        }
    }

}
