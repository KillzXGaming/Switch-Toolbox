using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin
{
    public class SMO_Scene
    {
        public Dictionary<string, string> OdysseyStages = new Dictionary<string, string>()
        {
            //Main
            { "CapWorldHomeStage","Cap Kingdom" },
            { "WaterfallWorldHomeStage","Cascade  Kingdom" },
            { "SandWorldHomeStage","Sand Kindom" },
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
            { "ForestWorldHomeStage","Wodded Kingdom" },
            { "DemoCrashHomeFallStage","Cloud Kingdom(1. Bowser Fight)" },
            {  "Theater2DExStage","Theater (smb 1-1)" },
        };
    }
}
