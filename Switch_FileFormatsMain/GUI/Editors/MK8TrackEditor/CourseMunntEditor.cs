using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using FirstPlugin.Turbo.CourseMuuntStructs;
using ByamlExt.Byaml;

namespace FirstPlugin.Turbo
{
    public partial class CourseMunntEditor : STForm
    {
        static Dictionary<int, string> ObjIDNameList = new Dictionary<int, string>();

        public CourseMunntEditor()
        {
            InitializeComponent();
        }
        public void LoadObjList()
        {
            if (PluginRuntime.Mk8GamePath == "")
            {
                FolderSelectDialog dlg = new FolderSelectDialog();

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    PluginRuntime.Mk8GamePath = dlg.SelectedPath;
                }
            }

            var byml = ByamlFile.LoadN($"{PluginRuntime.Mk8GamePath}/Data/Objflow.byaml", true).RootNode;
            foreach (var item in byml)
            {
                int ID = item["ObjId"];
                List<string> files = ((IList<object>)item["ResName"]).Cast<string>().ToList();
                ObjIDNameList.Add(ID, files[0]);

                Console.WriteLine(ID + " " + files[0]);
            }
            byml = null;
            GC.Collect();
        }

        public void LoadData(dynamic byaml)
        {
            LoadObjList();

            if (byaml.ContainsKey("Area"))
            {

            }
            if (byaml.ContainsKey("Clip"))
            {

            }
            if (byaml.ContainsKey("ClipArea"))
            {

            }
            if (byaml.ContainsKey("ClipPattern"))
            {

            }
            if (byaml.ContainsKey("CurrentArea"))
            {

            }
            if (byaml.ContainsKey("EffectArea"))
            {

            }
            if (byaml.ContainsKey("EnemyPath"))
            {
                LoadEnemyPaths(byaml);
            }
            if (byaml.ContainsKey("FirstCurve"))
            {

            }
            if (byaml.ContainsKey("GravityPath"))
            {

            }
            if (byaml.ContainsKey("IntroCamera"))
            {

            }
            if (byaml.ContainsKey("JugemPath"))
            {

            }
            if (byaml.ContainsKey("LapPath"))
            {

            }
            if (byaml.ContainsKey("MapObjIdList"))
            {

            }
            if (byaml.ContainsKey("MapObjResList"))
            {

            }
            if (byaml.ContainsKey("Path"))
            {

            }
            if (byaml.ContainsKey("ReplayCamera"))
            {

            }
            if (byaml.ContainsKey("SoundObj"))
            {

            }
            if (byaml.ContainsKey("SoundObj"))
            {

            }
        }
        private void LoadEnemyPaths(dynamic byaml)
        {
            foreach (dynamic group in byaml["EnemyPath"])
            {
                foreach (dynamic path in group)
                {
                }
            }
        }
    }
}
