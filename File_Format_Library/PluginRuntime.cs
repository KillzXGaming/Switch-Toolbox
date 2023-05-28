using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfres.Structs;
using Toolbox.Library;

namespace FirstPlugin
{
    public class PluginRuntime
    {
        public static List<BXFNT> BxfntFiles = new List<BXFNT>();

        public static List<SARC> SarcArchives = new List<SARC>();

        public class MaterialReplace
        {
            public static bool SwapShaderParams = true;
            public static bool SwapTextures = false;
            public static bool SwapShaderOptions = true;
            public static bool SwapRenderInfos = true;
            public static bool SwapUserData = true;
        }

        public static bool UseSimpleBfresEditor = false;

        //I really don't recall why I never just used a global texture cache 
        public static Dictionary<string, STGenericTexture> TextureCache = new Dictionary<string, STGenericTexture>();

        public static Dictionary<string, BFLIM> bflimTextures = new Dictionary<string, BFLIM>();
        public static List<BNTX> bntxContainers = new List<BNTX>();
        public static List<BFRESGroupNode> ftexContainers = new List<BFRESGroupNode>();
        public static List<CtrLibrary.BCHGroupNode> bchTexContainers = new List<CtrLibrary.BCHGroupNode>();
        public static List<NLG.StrikersRLT.TextureEntry> stikersTextures = new List<NLG.StrikersRLT.TextureEntry>();

        public static List<G1T> G1TextureContainers = new List<G1T>();

        public static string ExternalFMATPath = "";
        public static string OdysseyGamePath = "";
        public static string Mk8GamePath = "";
        public static List<string> SarcHashes = new List<string>(); //Each sarc has their own hash
    }
}
