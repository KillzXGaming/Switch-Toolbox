using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGl_EditorFramework;
using OpenTK;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class Scene
    {
        public int EffectSW;
        public int HeadLight;
        public bool IsFirstLeft;
        public bool IsJugemAbove;
        public int JugemAbove;
        public int LapJugemPos;
        public int LapNumber;
        public int OBJPrm1;
        public int OBJPrm2;
        public int OBJPrm3;
        public int OBJPrm4;
        public int OBJPrm5;
        public int OBJPrm6;
        public int OBJPrm7;
        public int OBJPrm8;
        public int PatternNum;
    }

    public class Path
    {
        public List<object> Properties = new List<object>();

        public List<PathPoint> PathPoints = new List<PathPoint>();
    }

    public class PathPoint
    {
        public List<object> Properties = new List<object>();

        public List<NextPoint> NextPoints = new List<NextPoint>();
        public List<PrevPoint> PrevPoints = new List<PrevPoint>();

        public List<ControlPoint> ControlPoints = new List<ControlPoint>();

        public Vector3 Rotate;
        public Vector3 Scale;
        public Vector3 Translate;
    }

    public class PointId
    {
        public int PathID { get; set; } //For groups
        public int PtID { get; set; } //For points
    }

    public class NextPoint : PointId { };
    public class PrevPoint : PointId { };

    public class ControlPoint
    {
        public Vector3 Point1;
        public Vector3 Point2;
    }

    public class ObjectNode
    {
        public Dictionary<string, dynamic> Properties { get; set; } = new Dictionary<string, dynamic>();

    }
}
