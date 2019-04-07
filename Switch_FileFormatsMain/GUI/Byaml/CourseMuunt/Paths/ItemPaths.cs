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
    public class ItemPathGroup : PathGroup, IObject
    {
        public const string N_EnemyPathGroup = "ItemPathGroup";
        public const string N_UnitIdNum = "UnitIdNum";

        public ItemPathGroup(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");

            foreach (var point in this["PathPt"])
            {
                PathPoints.Add(new ItemPathPoint(point));
            }
        }

        [Browsable(false)]
        public Dictionary<string, dynamic> Prop { get; set; } = new Dictionary<string, dynamic>();

        public int EnemyPathGroupId
        {
            get { return this[N_EnemyPathGroup]; }
            set { this[N_EnemyPathGroup] = value; }
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

    public class ItemPathPoint : PathPoint
    {
        private RenderablePathPoint renderablePoint;

        [Browsable(false)]
        public override RenderablePathPoint RenderablePoint
        {
            get
            {
                if (renderablePoint == null)
                    renderablePoint = new RenderablePathPoint(new Vector4(1f, 0f, 0f, 1f), Translate, Rotate, new OpenTK.Vector3(30), this);
                return renderablePoint;
            }
        }

        [Category("Properties")]
        public int Hover
        {
            get { return this["Hover"]; }
            set { this["Hover"] = value; }
        }

        [Category("Properties")]
        public int ItemPriority
        {
            get { return this["ItemPriority"]; }
            set { this["ItemPriority"] = value; }
        }

        [Category("Properties")]
        public int SearchArea
        {
            get { return this["SearchArea"]; }
            set { this["SearchArea"] = value; }
        }

        public ItemPathPoint(dynamic bymlNode)
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
