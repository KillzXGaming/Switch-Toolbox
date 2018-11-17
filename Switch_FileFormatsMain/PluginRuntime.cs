using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;

namespace FirstPlugin
{
    public class PluginRuntime
    {
        public static List<BinaryTextureContainer> bntxContainers = new List<BinaryTextureContainer>();
        public static List<FTEXContainer> ftexContainers = new List<FTEXContainer>();
        public static DockState FSHPDockState = DockState.DockRight;
        public static string ExternalFMATPath = "";
        public static string OdysseyGamePath = "";
        public static List<string> SarcHashes = new List<string>(); //Each sarc has their own hash
    }
}
