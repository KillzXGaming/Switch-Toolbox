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
    public class EnemyPathGroup : BasePathGroup, IObject
    {
        public const string N_EnemyPathGroup = "EnemyPathGroup";
        public const string N_UnitIdNum = "UnitIdNum";

        public EnemyPathGroup(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");

            foreach (var point in this["PathPt"])
            {
                PathPoints.Add(new EnemyPathPoint(point));
            }
        }

        public int EnemyPathGroupId
        {
            get { return this[N_EnemyPathGroup] != null ? this[N_EnemyPathGroup] : -1; }
            set { this[N_EnemyPathGroup] = value; }
        }

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

    public class EnemyPathPoint : BasePathPoint
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
        public int BattleFlag
        {
            get { return this["BattleFlag"] != null ? this["BattleFlag"] : -1; }
            set { this["BattleFlag"] = value; }
        }

        [Category("Properties")]
        public int Priority
        {
            get { return this["Priority"] != null ? this["Priority"] : -1; }
            set { this["Priority"] = value; }
        }

        public EnemyPathPoint(dynamic bymlNode)
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
