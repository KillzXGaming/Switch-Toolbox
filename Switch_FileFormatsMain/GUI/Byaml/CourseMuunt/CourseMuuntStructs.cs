using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            get { return (root["EffectSW"] != null) ? root["EffectSW"] : -255; }
            set { if (value != -255) root["EffectSW"] = value; }
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
            get { return (root["FirstCurve"] != null) ? root["FirstCurve"] : "left"; }
            set { root["FirstCurve"] = value; }
        }

        [Description("Unknown Value")]
        [Category("Scene")]
        [DisplayName("IsJugemAbove")]
        public bool IsJugemAbove
        {
            get { return (root["IsJugemAbove"] != null) ? root["IsJugemAbove"] : false; }
            set { root["IsJugemAbove"] = value; }
        }

        [Description("Unknown Value")]
        [Category("Scene")]
        [DisplayName("JugemAbove")]
        public int JugemAbove
        {
            get { return (root["JugemAbove"] != null) ? root["JugemAbove"] : -1; }
            set { if (value != -1) root["JugemAbove"] = value; }
        }

        [Description("Unknown Value")]
        [Category("Scene")]
        [DisplayName("LapJugemPos")]
        public int LapJugemPos
        {
            get { return (root["LapJugemPos"] != null) ? root["LapJugemPos"] : -1; }
            set { if (value != -1) root["LapJugemPos"] = value; }
        }

        [Description("The number of laps to be finished during the track.")]
        [Category("Scene")]
        [DisplayName("LapNumber")]
        public int LapNumber
        {
            get { return (root["LapNumber"] != null) ? root["LapNumber"] : -1; }
            set { if (value != -1) root["LapNumber"] = value; }
        }

        [Description("The number of pattern sets picked randomly at start.")]
        [Category("Scene")]
        [DisplayName("PatternNum")]
        public int PatternNum
        {
            get { return (root["PatternNum"] != null) ? root["PatternNum"] : -1; }
            set { if (value != -1) root["PatternNum"] = value; }
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

        public List<LapPathGroup> LapPaths;
        public List<EnemyPathGroup> EnemyPaths;
        public List<ItemPathGroup> ItemPaths;
        public List<GlidePathGroup> GlidePaths;
        public List<SteerAssistPathGroup> SteerAssistPaths;
        public List<GravityPathGroup> GravityPaths;
        public List<PullPathGroup> PullPaths;
        public List<PathGroup> Paths;
        public List<ObjPathGroup> ObjPaths;
        public List<JugemPathGroup> JugemPaths;
        public List<IntroCamera> IntroCameras;
        
        public List<int> MapObjIdList;
        public List<string> MapObjResList;

        public CourseMuuntScene(dynamic rootNode)
        {
            root = rootNode;

            LapPaths = new List<LapPathGroup>();
            EnemyPaths = new List<EnemyPathGroup>();
            GlidePaths = new List<GlidePathGroup>();
            ItemPaths = new List<ItemPathGroup>();
            SteerAssistPaths = new List<SteerAssistPathGroup>();
            GravityPaths = new List<GravityPathGroup>();
            PullPaths = new List<PullPathGroup>();
            Paths = new List<PathGroup>();
            ObjPaths = new List<ObjPathGroup>();
            JugemPaths = new List<JugemPathGroup>();
            IntroCameras = new List<IntroCamera>();

            MapObjIdList = new List<int>();
            MapObjResList = new List<string>();

            if (root.ContainsKey("Area")) {
                foreach (var area in root["Area"])
                { }
            }
            if (root.ContainsKey("Clip")) {
                foreach (var clip in root["Clip"])
                { }
            }
            if (root.ContainsKey("ClipArea")) {
                foreach (var clipArea in root["ClipArea"])
                { }
            }
            if (root.ContainsKey("ClipPattern")) {
                foreach (var clipPattern in root["ClipPattern"])
                { }
            }
            if (root.ContainsKey("CurrentArea")) {
                foreach (var currentArea in root["CurrentArea"])
                { }
            }
            if (root.ContainsKey("EffectArea")) {
                foreach (var effectArea in root["EffectArea"])
                { }
            }
            if (root.ContainsKey("ItemPath")) {
                foreach (var itemPath in root["ItemPath"]) {
                    ItemPaths.Add(new ItemPathGroup(itemPath));
                }
            }
            if (root.ContainsKey("EnemyPath")) {
                foreach (var enemyPath in root["EnemyPath"]) {
                    EnemyPaths.Add(new EnemyPathGroup(enemyPath));
                }
            }
            if (root.ContainsKey("GlidePath")) {
                foreach (var glidePath in root["GlidePath"]) {
                    GlidePaths.Add(new GlidePathGroup(glidePath));
                }
            }
            if (root.ContainsKey("GravityPath")) {
                foreach (var gravityPath in root["GravityPath"]) {
                    GravityPaths.Add(new GravityPathGroup(gravityPath));
                }
            }
            if (root.ContainsKey("IntroCamera")) {
                foreach (var introCamera in root["IntroCamera"]) {
                    IntroCameras.Add(new IntroCamera(introCamera));
                }
            }
            if (root.ContainsKey("JugemPath")) {
                foreach (var jugemPath in root["JugemPath"]) {
                   JugemPaths.Add(new JugemPathGroup(jugemPath));
                }
            }
            if (root.ContainsKey("LapPath")) {
                foreach (var lapPath in root["LapPath"]) {
                    LapPaths.Add(new LapPathGroup(lapPath));
                }
            }
            if (root.ContainsKey("MapObjIdList")) {
                foreach (var mapObjIdList in root["MapObjIdList"]) {
                    MapObjIdList.Add(mapObjIdList);
                }
            }
            if (root.ContainsKey("MapObjResList")) {
                foreach (var mapObjResList in root["MapObjResList"]) {
                    MapObjResList.Add(mapObjResList);
                }
            }
            if (root.ContainsKey("Obj")) {
                foreach (var objPath in root["Obj"]) {
                }
            }
            if (root.ContainsKey("ObjPath")) {
                foreach (var objPath in root["ObjPath"]) {
              //      ObjPaths.Add(new ObjPathGroup(objPath));
                }
            }
            if (root.ContainsKey("PullPath")) {
                foreach (var pullPath in root["PullPath"])
                    PullPaths.Add(new PullPathGroup(pullPath));
            }
            if (root.ContainsKey("Path")) {
                foreach (var path in root["Path"]) {
                   Paths.Add(new PathGroup(path));
                }
            }
            if (root.ContainsKey("ReplayCamera")) {
                foreach (var replayCamera in root["ReplayCamera"])
                { }
            }
            if (root.ContainsKey("SoundObj")) {
                foreach (var soundObj in root["SoundObj"])
                { }
            }
            if (root.ContainsKey("SteerAssistPath")) {
                foreach (var steerAssistPath in root["SteerAssistPath"]) {
                    SteerAssistPaths.Add(new SteerAssistPathGroup(steerAssistPath));
                }
            }
        }


        public List<BFRES> BfresObjects = new List<BFRES>();
        public List<KCL> KclObjects = new List<KCL>();

        public void AddRenderableKcl(string FilePath)
        {
            if (!System.IO.File.Exists(FilePath))
                return;

            KCL kcl = (KCL)Switch_Toolbox.Library.IO.STFileLoader.OpenFileFormat(FilePath);

            if (kcl != null)
                KclObjects.Add(kcl);
        }

        public void AddRenderableBfres(string FilePath)
        {
            if (!System.IO.File.Exists(FilePath))
                return;

            BFRES bfres = (BFRES)Switch_Toolbox.Library.IO.STFileLoader.OpenFileFormat(FilePath);

            if (bfres != null)
                BfresObjects.Add(bfres);
        }
    }
}
