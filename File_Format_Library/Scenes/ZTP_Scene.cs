using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;

namespace FirstPlugin.Scenes
{
    public class ZTP_Scene
    {
        private readonly Dictionary<string, string> TwilightPrincessObjectFiles = new Dictionary<string, string>()
            {
               { "@bg0000","" },
            };

        private void LoadStage()
        {
            string StageDirectory = $"{Runtime.TpGamePath}/res/Stage/";
        }

        //List from https://github.com/magcius/noclip.website/blob/7c8443aab5228811157c30d8985707d23d64ee53/src/j3d/ztp_scenes.ts#L344
        private readonly Dictionary<string, string> TwilightPrincessStageFiles = new Dictionary<string, string>()
            {
               { "F_SP102","Hyrule Field Map 1"},
               { "F_SP121","Hyrule Field Map 2" },
               { "F_SP122","Hyrule Field Map 3" },
               { "F_SP123","Lake Hylia" },

               { "F_SP00","Ordon Ranch" },
               { "F_SP103","Link's House Area" },
               { "F_SP104","Ordon Woods" },
               { "F_SP108","Faron Woods" },
               { "F_SP109","Kakariko Village" },
               { "F_SP110","Death Mountain Trail" },
               { "F_SP111","Kakariko Graveyard" },
               { "F_SP112","Rapids Ride" },
               { "F_SP113","Zora's Domain" },
               { "F_SP114","Snowpeak Mountain" },
               { "F_SP115","Lanayru's Spring" },
               { "F_SP116","Castle Town" },
               { "F_SP117","Sacred Grove" },
               { "F_SP118","Gerudo Desert Bulblin Base" },
               { "F_SP124","Gerudo Desert" },
               { "F_SP125","Arbiter's Grounds Mirror Chamber" },
               { "F_SP126","Zora's River" },
               { "F_SP127","Fishing Pond" },
               { "F_SP128","Hidden Village" },
               { "F_SP200","Wolf Howling Cutscene Map"},

               //Dungeons
               { "D_MN05","Forest Temple"},
               { "D_MN05A","Forest Temple Boss Arena"},
               { "D_MN05B","Forest Temple Mini-Boss Arena"},

               { "D_MN04","Goron Mines"},
               { "D_MN04A","Goron Mines Boss Arena"},
               { "D_MN04B","Goron Mines Mini-Boss Arena"},

               { "D_MN01","Lakebed Temple"},
               { "D_MN01A","Lakebed Temple Boss Arena"},
               { "D_MN01B","Lakebed Temple Mini-Boss Arena"},

            };

    }
}
