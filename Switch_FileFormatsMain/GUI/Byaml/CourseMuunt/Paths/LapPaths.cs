using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.EditorDrawables;
using OpenTK;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class LapPathGroup : PathGroup, IObject
    {
        public const string N_LapPathGroup = "LapPathGroup";
        public const string N_ReturnPointsError = "ReturnPointsError";
        public const string N_UnitIdNum = "UnitIdNum";

        public List<ReturnPoint> ReturnPoints = new List<ReturnPoint>();

        public LapPathGroup(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");

            foreach (var point in this["PathPt"])
            {
                PathPoints.Add(new LapPathPoint(point));
            }

            foreach (var point in this["ReturnPoints"])
            {
                ReturnPoints.Add(new ReturnPoint(point));
            }
        }

        [Browsable(false)]
        public Dictionary<string, dynamic> Prop { get; set; } = new Dictionary<string, dynamic>();

        public int LapPathGroupId
        {
            get { return this[N_LapPathGroup]; }
            set { this[N_LapPathGroup] = value; }
        }

        public int ReturnPointsError
        {
            get { return this[N_ReturnPointsError]; }
            set { this[N_ReturnPointsError] = value; }
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

    public class LapPathPoint : PathPoint
    {

        [Browsable(false)]
        public override RenderablePathPoint RenderablePoint
        {
            get
            {
                var point = new RenderablePathPoint(new Vector4(0f, 0.25f, 1f, 0.1f), Translate, Rotate, Scale, this);
                return point;
            }
        }

        [Category("Properties")]
        public int CheckPoint
        {
            get { return this["CheckPoint"]; }
            set { this["CheckPoint"] = value; }
        }

        [Category("Properties")]
        public int ClipIdx
        {
            get { return this["ClipIdx"]; }
            set { this["ClipIdx"] = value; }
        }

        [Category("Properties")]
        public bool HeadLightSW
        {
            get { return this["HeadLightSW"]; }
            set { this["HeadLightSW"] = value; }
        }

        [Category("Properties")]
        public int LapCheck
        {
            get { return this["LapCheck"]; }
            set { this["LapCheck"] = value; }
        }

        [Category("Properties")]
        public int MapCameraFovy
        {
            get { return this["MapCameraFovy"]; }
            set { this["MapCameraFovy"] = value; }
        }

        [Category("Properties")]
        public int MapCameraY
        {
            get { return this["MapCameraY"]; }
            set { this["MapCameraY"] = value; }
        }

        public LapPathPoint(dynamic bymlNode)
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
