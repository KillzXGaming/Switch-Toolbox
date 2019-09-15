using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using ByamlExt.Byaml;
using OdysseyEditor;
using FirstPlugin.Forms;

namespace FirstPlugin
{
    //Code off of https://github.com/exelix11/OdysseyEditor
    //Note this will purely be for viewing, performance tests, and ripping
    public class SMO_Scene
    {
        public static void LoadStage(string MapName)
        {
            string StageByml = $"{Runtime.SmoGamePath}\\StageData\\{MapName}Map.szs";

            Console.WriteLine($"{StageByml} {File.Exists($"{StageByml}")}");
            if (File.Exists($"{StageByml}"))
            {
                var TextureSzs = $"{Runtime.SmoGamePath}\\ObjectData\\{MapName}Texture.szs";

                ObjectEditor editor = new ObjectEditor();
                LibraryGUI.CreateMdiWindow(editor);

                var level = new Level(StageByml, -1);
                foreach (var obj in level.objs)
                {
                    foreach (var ob in obj.Value)
                    {
                        var Transform = Utils.TransformValues(ob.transform.Pos, ob.transform.Rot, ob.transform.Scale);

                        var bfresData = BfresFromSzs(ob.Name);

                        if (bfresData != null)
                        {
                            BFRES bfresFile = (BFRES)STFileLoader.OpenFileFormat(new MemoryStream(bfresData), ob.Name);
                            bfresFile.BFRESRender.ModelTransform = Transform;

                            editor.AddNode(bfresFile);
                            bfresFile.LoadEditors(null);
                            DiableLoadCheck();
                        }
                    }
                }

                TextureSzs = null;
                GC.Collect();
            }

            BfresEditor bfresEditor = (BfresEditor)LibraryGUI.GetActiveContent(typeof(BfresEditor));
            bfresEditor.DisplayAll = true;
        }

        private static void DiableLoadCheck()
        {
            BfresEditor bfresEditor = (BfresEditor)LibraryGUI.GetActiveContent(typeof(BfresEditor));
            bfresEditor.IsLoaded = false;
            bfresEditor.DisplayAllDDrawables();
        }

        private static byte[] BfresFromSzs(string fileName)
        {
            if (File.Exists($"{Runtime.SmoGamePath}\\ObjectData\\{fileName}.szs"))
            {
                var SzsFiles = SARCExt.SARC.UnpackRamN(EveryFileExplorer.YAZ0.Decompress($"{Runtime.SmoGamePath}\\ObjectData\\{fileName}.szs")).Files;
                if (SzsFiles.ContainsKey(fileName + ".bfres"))
                {
                    return SzsFiles[fileName + ".bfres"];
                }
            }
            return null;
        }

        public static Dictionary<string, string> OdysseyStages = new Dictionary<string, string>()
        {
            //Main
            { "CapWorldHomeStage","Cap Kingdom" },
            { "WaterfallWorldHomeStage","Cascade  Kingdom" },
            { "SandWorldHomeStage","Sand Kindom" },
            { "ForestWorldHomeStage","Wodded Kingdom" },
            { "SnowWorldHomeStage","Snow Kingdom" },
            { "SeaWorldHomeStage", "Seaside Kingdom" },
            { "ClashWorldHomeStage","Lost Kingdom" },
            {  "CityWorldHomeStage","Metro Kingdom" },
            { "LakeWorldHomeStage","Lake Kingdom(Starting Area)" },
            { "SkyWorldHomeStage","Bowser\'s Kingdom" },
            { "BossRaidWorldHomeStage","Ruined Kingdom" },
            { "MoonWorldHomeStage","Moon Kingdom" },
            { "LavaWorldHomeStage","Luncheon Kingdom" },
            {"PeachWorldHomeStage", "Mushroom Kingdom" },

            {"Special1WorldHomeStage", "Dark Side of the Moon"},
            {"Special2WorldHomeStage", "Darker Side of the Moon"},

            //Sub areas
            { "MeganeLiftExStage","MoEye moving Platform" },
            { "SandWorldPyramid000Stage","Pyramid(Starting Area)" },
            { "SandWorldPyramid001Stage","Pyramid(Bullet Bill Parkour)" },
            { "SandWorldUnderground000Stage","Ice Underground before Boss" },
            { "SandWorldUnderground001Stage","Ice Underground Boss" },
            { "LakeWorldTownZone","LakeKingdom (Town Area)" },
            { "DemoCrashHomeFallStage","Cloud Kingdom(1. Bowser Fight)" },
            {  "Theater2DExStage","Theater (smb 1-1)" },
        };
    }
}
