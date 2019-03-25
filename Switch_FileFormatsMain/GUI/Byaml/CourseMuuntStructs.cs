using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGl_EditorFramework;
using OpenTK;
using System.ComponentModel;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class CourseMuuntScene
    {
        private dynamic root;

        [Description("The effect index used globally across the course.")]
        [Category("Scene")]
        [DisplayName("Effect Index")]
        public int EffectSW
        {
            get { return root["EffectSW"]; }
            set { root["EffectSW"] = value; }
        }

        [Description("Enables or disables headlights for the course.")]
        [Category("Scene")]
        [DisplayName("HeadLight")]
        public bool HeadLight
        {
            get { return (root["HeadLight"] == 1) ? true : false; }
            set { root["HeadLight"] = value == true ? 1 : 0; }
        }

        [Description("Determines if First Curve is left or right. This determines Latiku's direction.")]
        [Category("Scene")]
        [DisplayName("IsFirstLeft")]
        public bool IsFirstLeft
        {
            get
            {
                if (FirstCurve == "left")
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                    FirstCurve = "left";
                else
                    FirstCurve = "right";
            }
        }
        private string FirstCurve 
        {
            get { return root["FirstCurve"]; }
            set { root["FirstCurve"] = value; }
        }

        [Description("Unknown Value")]
        [Category("Scene")]
        [DisplayName("IsJugemAbove")]
        public bool IsJugemAbove
        {
            get { return root["IsJugemAbove"]; }
            set { root["IsJugemAbove"] = value; }
        }

        [Description("Unknown Value")]
        [Category("Scene")]
        [DisplayName("JugemAbove")]
        public int JugemAbove
        {
            get { return root["JugemAbove"]; }
            set { root["JugemAbove"] = value; }
        }

        [Description("Unknown Value")]
        [Category("Scene")]
        [DisplayName("LapJugemPos")]
        public int LapJugemPos
        {
            get { return root["LapJugemPos"]; }
            set { root["LapJugemPos"] = value; }
        }

        [Description("The number of laps to be finished during the track.")]
        [Category("Scene")]
        [DisplayName("LapNumber")]
        public int LapNumber
        {
            get { return root["LapNumber"]; }
            set { root["LapNumber"] = value; }
        }

        [Description("The number of pattern sets picked randomly at start.")]
        [Category("Scene")]
        [DisplayName("PatternNum")]
        public int PatternNum
        {
            get { return root["PatternNum"]; }
            set { root["PatternNum"] = value; }
        }


        [Category("Object Parameters")]
        public int OBJPrm1
        {
            get { return root["OBJPrm1"]; }
            set { root["OBJPrm1"] = value; }
        }

        [Category("Object Parameters")]
        public int OBJPrm2
        {
            get { return root["OBJPrm2"]; }
            set { root["OBJPrm2"] = value; }
        }

        [Category("Object Parameters")]
        public int OBJPrm3
        {
            get { return root["OBJPrm3"]; }
            set { root["OBJPrm3"] = value; }
        }

        [Category("Object Parameters")]
        public int OBJPrm4
        {
            get { return root["OBJPrm4"]; }
            set { root["OBJPrm4"] = value; }
        }

        [Category("Object Parameters")]
        public int OBJPrm5
        {
            get { return root["OBJPrm5"]; }
            set { root["OBJPrm5"] = value; }
        }

        [Category("Object Parameters")]
        public int OBJPrm6
        {
            get { return root["OBJPrm6"]; }
            set { root["OBJPrm6"] = value; }
        }

        [Category("Object Parameters")]
        public int OBJPrm7
        {
            get { return root["OBJPrm7"]; }
            set { root["OBJPrm7"] = value; }
        }

        [Category("Object Parameters")]
        public int OBJPrm8
        {
            get { return root["OBJPrm8"]; }
            set { root["OBJPrm8"] = value; }
        }

        public CourseMuuntScene(dynamic rootNode)
        {
            root = rootNode;
        }

        public List<BFRES> BfresObjects = new List<BFRES>();
        public List<KCL> KclObjects = new List<KCL>();

        public void AddRenderableKcl(string FilePath)
        {
            KCL kcl = (KCL)Switch_Toolbox.Library.IO.STFileLoader.OpenFileFormat(FilePath);
            KclObjects.Add(kcl);
        }

        public void AddRenderableBfres(string FilePath)
        {
            BFRES bfres = (BFRES)Switch_Toolbox.Library.IO.STFileLoader.OpenFileFormat(FilePath);
            BfresObjects.Add(bfres);
        }
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
