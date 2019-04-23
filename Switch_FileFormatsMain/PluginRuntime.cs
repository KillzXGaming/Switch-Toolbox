using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfres.Structs;

namespace FirstPlugin
{
    public class PluginRuntime
    {
        public static bool UseSimpleBfresEditor = true;

        public static Dictionary<string, BFLIM> bflimTextures = new Dictionary<string, BFLIM>();
        public static List<BNTX> bntxContainers = new List<BNTX>();
        public static List<BFRESGroupNode> ftexContainers = new List<BFRESGroupNode>();
        public static string ExternalFMATPath = "";
        public static string OdysseyGamePath = "";
        public static string Mk8GamePath = "";
        public static List<string> SarcHashes = new List<string>(); //Each sarc has their own hash
    }
}
