using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SARCExt;
using ByamlExt.Byaml;
using EveryFileExplorer;

namespace OdysseyEditor
{
    public class ObjList : List<LevelObj>
    {
        IList<dynamic> bymlNode;
        public ObjList(string _name, IList<dynamic> _bymlNode)
        {
            name = _name;
            if (_bymlNode == null)
            {
                bymlNode = new List<dynamic>();
                return;
            }
            bymlNode = _bymlNode;
            foreach (var o in bymlNode) this.Add(new LevelObj(o));
        }

        public void ApplyToNode()
        {
            bymlNode.Clear();
            foreach (var o in this) bymlNode.Add(o.Prop);
        }    

        public bool IsHidden = false;
        public string name = "";
    }

    public class Level
    {
        public Dictionary<string, byte[]> SzsFiles;
        public Dictionary<string, ObjList> objs = new Dictionary<string, ObjList>();
        dynamic LoadedByml = null;
        public string Filename = "";
        int _ScenarioIndex = -1;

        public Level(bool empty, string levelN)
        {
            if (!empty) throw new Exception();
            SzsFiles = new Dictionary<string, byte[]>();
            Filename = levelN;
            LoadedByml = new dynamic[15];
            for (int i = 0; i < 15; i++)
                LoadedByml[i] = new Dictionary<string, dynamic>();
          //  SzsFiles.Add(Path.GetFileNameWithoutExtension(Filename) + ".byml", ByamlFile.SaveN(LoadedByml,false, Syroot.BinaryData.ByteOrder.LittleEndian));
            LoadObjects();
        }

        public Level (string path, int scenarioIndex = -1)
        {
            Filename = path;
            Load(File.ReadAllBytes(path), scenarioIndex);
        }        

        void Load(byte[] file, int scenarioIndex = -1)
        {
            SzsFiles = SARC.UnpackRamN(YAZ0.Decompress(file)).Files;
            LoadObjects(scenarioIndex);
        }

        void LoadObjects(int scenarioIndex = -1)
        {
            Stream s = new MemoryStream(SzsFiles[Path.GetFileNameWithoutExtension(Filename) + ".byml"]);
            LoadedByml = ByamlFile.LoadN(s,false, Syroot.BinaryData.ByteOrder.LittleEndian);

            if (scenarioIndex == -1)
            {
                string res = "0";
                InputDialog.Show("Select scenario", $"enter scenario value [0,{LoadedByml.Count- 1}]", ref res);
                if (!int.TryParse(res, out scenarioIndex)) scenarioIndex = 0;
            }

            _ScenarioIndex = scenarioIndex;
            var Scenario = (Dictionary<string, dynamic>)LoadedByml[scenarioIndex];
            if (Scenario.Keys.Count == 0)
                Scenario.Add("ObjectList", new List<dynamic>());
            foreach (string k in Scenario.Keys)
            {
                objs.Add(k, new ObjList(k,Scenario[k]));
            }
        }

        public void OpenBymlViewer()
        {
       
        }

        void ApplyChangesToByml() //this makes sure new objects are added
        {
            objs.OrderBy(k => k.Key);
            for (int i = 0; i < objs.Count; i++)
            {
                var values = objs.Values.ToArray();
                if (values[i].Count == 0) objs.Remove(objs.Keys.ToArray()[i--]);
                else values[i].ApplyToNode();
            }
        }

        public byte[] ToByaml()
        {
            ApplyChangesToByml();
            MemoryStream mem = new MemoryStream();
        //    ByamlFile.Save(mem, LoadedByml, false, Syroot.BinaryData.ByteOrder.LittleEndian);
            var res = mem.ToArray();
            return res;
        }

        public byte[] SaveSzs()
        {
            SzsFiles[Path.GetFileNameWithoutExtension(Filename) + ".byml"] = ToByaml();
            return YAZ0.Compress(SARC.pack(SzsFiles));
        }

        public bool HasList(string name) { return objs.ContainsKey(name); }

        public struct SearchResult
        {
            public LevelObj obj;
            public int Index;
            public string ListName;
        }

        public SearchResult FindObjById(string ID)
        {
            foreach (string k in objs.Keys)
            {
                for (int i = 0; i < objs[k].Count; i++)
                {
                    if (objs[k][i].ID == ID)
                        return new SearchResult
                        {
                            obj = objs[k][i],
                            Index = i,
                            ListName = k
                        };
                }
            }
            return new SearchResult
            {
                obj = null,
                Index = -1,
                ListName = ""
            };
        }

        public ObjList FindListByObj(LevelObj o)
        {
            foreach (string k in objs.Keys)
            {
                if (objs[k].Contains(o)) return objs[k];
            }
            return null;
        }
    }
}
