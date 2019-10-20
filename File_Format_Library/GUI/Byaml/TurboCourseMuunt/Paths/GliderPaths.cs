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
using Toolbox.Library;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class GlidePathGroup : BasePathGroup, IObject
    {
        public const string N_GlidePathGroup = "GlidePathGroup";
        public const string N_UnitIdNum = "UnitIdNum";

        public GlidePathGroup(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");

            foreach (var point in this["PathPt"])
            {
                PathPoints.Add(new GlidePathPoint(point));
            }

            ConnectGroups = false;
        }

        [Browsable(false)]
        public Dictionary<string, dynamic> Prop { get; set; } = new Dictionary<string, dynamic>();

        public int GlidePathGroupId
        {
            get { return this[N_GlidePathGroup]; }
            set { this[N_GlidePathGroup] = value; }
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

    public class GlidePathPoint : BasePathPoint
    {
        private RenderablePathPoint renderablePoint;

        [Browsable(false)]
        public override RenderablePathPoint RenderablePoint
        {
            get
            {
                if (renderablePoint == null)
                    renderablePoint = new RenderablePathPoint(ColorUtility.ToVector4(Color.FromArgb(252, 93, 11)), Translate, Rotate, new OpenTK.Vector3(30), this);
                return renderablePoint;
            }
        }

        [Category("Properties")]
        public int GlideType
        {
            get { return (this["GlideType"] != null) ? this["GlideType"] : -1; }
            set { this["GlideType"] = value; }
        }

        [Category("Properties")]
        public bool IsUp
        {
            get { return (this["IsUp"] != null) ? this["IsUp"] : false; }
            set { this["IsUp"] = value; }
        }

        [Category("Properties")]
        public int Ascend
        {
            get { return (this["Ascend"] != null) ? this["Ascend"] : -1; }
            set { this["Ascend"] = value; }
        }

        [Category("Properties")]
        public bool Cannon
        {
            get { return (this["Cannon"] != null) ? this["Cannon"] : false; }
            set { this["Cannon"] = value; }
        }

        public GlidePathPoint(dynamic bymlNode)
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
