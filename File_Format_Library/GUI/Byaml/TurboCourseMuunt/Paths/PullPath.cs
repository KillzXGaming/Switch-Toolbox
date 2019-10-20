using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class PullPathGroup : BasePathGroup, IObject
    {
        public const string N_UnitIdNum = "UnitIdNum";

        public PullPathGroup(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");

            foreach (var point in this["PathPt"])
            {
                PathPoints.Add(new PullPathPoint(point));
            }
        }

        [Browsable(false)]
        public Dictionary<string, dynamic> Prop { get; set; } = new Dictionary<string, dynamic>();

        public int UnitIdNum
        {
            get { return this[N_UnitIdNum] != null ? this[N_UnitIdNum] : -1; }
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

    public class PullPathPoint : BasePathPoint
    {
        private RenderablePathPoint renderablePoint;

        [Browsable(false)]
        public override RenderablePathPoint RenderablePoint
        {
            get
            {
                if (renderablePoint == null)
                    renderablePoint = new RenderablePathPoint(new Vector4(1f, 1f, 0f, 1f), Translate, Rotate, new OpenTK.Vector3(30), this);
                return renderablePoint;
            }
        }

        [Category("Properties")]
        public int prm1
        {
            get { return this["prm1"] != null ? this["prm1"] : -1; }
            set { if (value != -1) this["prm1"] = value; }
        }

        [Category("Properties")]
        public int prm2
        {
            get { return this["prm2"] != null ? this["prm2"] : -1; }
            set { if (value != -1) this["prm2"] = value; }
        }

        public PullPathPoint(dynamic bymlNode)
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
