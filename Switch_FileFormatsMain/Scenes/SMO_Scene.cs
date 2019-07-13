using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Switch_Toolbox.Library;
using ByamlExt.Byaml;
using OdysseyEditor;

namespace FirstPlugin
{
    //Code off of https://github.com/exelix11/OdysseyEditor
    //Note this will purely be for viewing, performance tests, and ripping
    public class SMO_Scene
    {
        public static void LoadStage(string MapName)
        {
            if (File.Exists($"{Runtime.SmoGamePath}StageData\\{MapName}Map.szs"))
            {
                string StageByml = $"{Runtime.SmoGamePath}StageData\\{MapName}Map.szs";
                new Level(StageByml, -1);
            }
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
