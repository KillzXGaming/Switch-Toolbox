using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.EditorDrawables;
using OpenTK;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class GravityPathGroup : BasePathGroup, IObject
    {
        public const string N_UnitIdNum = "UnitIdNum";

        public GravityPathGroup(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");

            foreach (var point in this["PathPt"])
            {
                PathPoints.Add(new GravityPathPoint(point));
            }
        }

        [Browsable(false)]
        public Dictionary<string, dynamic> Prop { get; set; } = new Dictionary<string, dynamic>();

        public int UnitIdNum
        {
            get { return this[N_UnitIdNum]; }
            set { this[N_UnitIdNum] = value; }
        }

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

    public class GravityPathPoint : BasePathPoint
    {
        private RenderablePathPoint renderablePoint;

        [Browsable(false)]
        public override RenderablePathPoint RenderablePoint
        {
            get
            {
                if (renderablePoint == null)
                    renderablePoint = new RenderablePathPoint(new Vector4(0.5f, 0f, 0.5f, 0.1f), Translate, Rotate, Scale, this);
                return renderablePoint;
            }
        }

        [Category("Properties")]
        public int CameraHeight
        {
            get { return (this["CameraHeight"] != null) ? this["CameraHeight"] : -1; }
            set { if (value != -1) this["CameraHeight"] = value; }
        }

        [Category("Properties")]
        public bool GlideOnly
        {
            get { return (this["GlideOnly"] != null) ? this["GlideOnly"] : false; }
            set { this["GlideOnly"] = value; }
        }

        [Category("Properties")]
        public bool Transform
        {
            get { return (this["Transform"] != null) ? this["Transform"] : false; }
            set { this["Transform"] = value; }
        }

        public GravityPathPoint(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");

            foreach (var point in this["NextPt"])
            {
                NextPoints.Add(new PointID(point));
            }
            foreach (var point in this["PrevPt"])
            {
                PrevPoints.Add(new PointID(point));
            }
        }
    }

}
