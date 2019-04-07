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
    public class SteerAssistPathGroup : PathGroup, IObject
    {
        public const string N_SteerAssistPathGroup = "SteerAssistPathGroup";
        public const string N_UnitIdNum = "UnitIdNum";

        public SteerAssistPathGroup(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");

            foreach (var point in this["PathPt"])
            {
                PathPoints.Add(new EnemyPathPoint(point));
            }
        }

        [Browsable(false)]
        public Dictionary<string, dynamic> Prop { get; set; } = new Dictionary<string, dynamic>();

        public int SteerAssistPathGroupId
        {
            get { return this[N_SteerAssistPathGroup]; }
            set { this[N_SteerAssistPathGroup] = value; }
        }

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

    public class SteerAssistPathPoint : PathPoint
    {
        [Browsable(false)]
        public override RenderablePathPoint RenderablePoint
        {
            get
            {
                var point = new RenderablePathPoint(new Vector4(0f, 1f, 0f, 1f), Translate, Rotate, new OpenTK.Vector3(30), this);
                point.CanConnect = true;
                return point;
            }
        }

        [Category("Properties")]
        public int Priority
        {
            get { return this["Priority"]; }
            set { this["Priority"] = value; }
        }

        public SteerAssistPathPoint(dynamic bymlNode)
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
