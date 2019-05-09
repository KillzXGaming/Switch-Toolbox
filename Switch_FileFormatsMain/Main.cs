using PluginContracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class FirstPlugin : IPlugin
    {
        

        private static FirstPlugin _instance;
        public static FirstPlugin Instance { get { return _instance == null ? _instance = new FirstPlugin() : _instance; } }
        public static string executableDir;

        #region IPlugin Members 

        private string name;
        public string Name
        {
            get
            {
                return "First Plugin";
            }
            set
            {
                this.Name = value;
            }
        }

        private string author;
        public string Author
        {
            get
            {
                return "KXG";
            }
            set
            {
                this.name = value;
            }
        }

        public string Description
        {
            get
            {
                return "A cool plugin";
            }
        }

        public string Version
        {
            get
            {
                return "1.0";
            }
        }
        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                foreach (Type T in LoadFileFormats())
                    types.Add(T);
                foreach (Type T in LoadMenus())
                    types.Add(T);

                return types.ToArray();
            }
        }
        public void Load()
        {
            Config.StartupFromFile(Runtime.ExecutableDir + "/Lib/Plugins/config.xml");
        }
        public void Unload()
        {
            PluginRuntime.bntxContainers.Clear();
        }

        class OdysseyCostumeSelectorMenu : IMenuExtension
        {
            public STToolStripItem[] FileMenuExtensions => null;
            public STToolStripItem[] ToolsMenuExtensions => toolsExt;
            public STToolStripItem[] TitleBarExtensions => null;

            readonly STToolStripItem[] toolsExt = new STToolStripItem[1];
            public OdysseyCostumeSelectorMenu()
            {
                toolsExt[0] = new STToolStripItem("Odyssey Costume Selctor");
                toolsExt[0].Click += OpenSelector;
            }
            private void OpenSelector(object sender, EventArgs args)
            {

            }
        }

        private Type[] LoadMenus()
        {
            List<Type> MenuItems = new List<Type>();
            MenuItems.Add(typeof(OdysseyCostumeSelectorMenu));

            return MenuItems.ToArray();
        }
        private Type[] LoadFileFormats()
        {
            List<Type> Formats = new List<Type>();
            Formats.Add(typeof(SARC));
            Formats.Add(typeof(BFRES));
            Formats.Add(typeof(BCRES));
            Formats.Add(typeof(BNTX));
            Formats.Add(typeof(BEA));
            Formats.Add(typeof(BYAML));
            Formats.Add(typeof(XTX));
            Formats.Add(typeof(KCL));
            Formats.Add(typeof(BFFNT));
            Formats.Add(typeof(MSBT));
            Formats.Add(typeof(XLINK));
            
       //     Formats.Add(typeof(BFSAR));
            Formats.Add(typeof(BARS));
      //      Formats.Add(typeof(BFLAN));
       //     Formats.Add(typeof(BFLYT));
            Formats.Add(typeof(GFPAK));
            Formats.Add(typeof(NUTEXB));
            Formats.Add(typeof(NUT));
            Formats.Add(typeof(GTXFile));
            Formats.Add(typeof(AAMP));
            Formats.Add(typeof(PTCL));
            Formats.Add(typeof(EFF));
            Formats.Add(typeof(EFCF));
            
        //    Formats.Add(typeof(NCA));
        //    Formats.Add(typeof(XCI));
        //   Formats.Add(typeof(NSP));
            Formats.Add(typeof(BFSAR));
            Formats.Add(typeof(BNSH));
            Formats.Add(typeof(BFSHA));
            Formats.Add(typeof(BFLIM));
            Formats.Add(typeof(BCLIM));
            Formats.Add(typeof(BFSTM));
            Formats.Add(typeof(BCSTM));
            Formats.Add(typeof(BRSTM));
            Formats.Add(typeof(BFWAV));
            Formats.Add(typeof(BCWAV));
            Formats.Add(typeof(BRWAV));
            Formats.Add(typeof(WAV));
            Formats.Add(typeof(MP3));
            Formats.Add(typeof(OGG));
            Formats.Add(typeof(IDSP));
            Formats.Add(typeof(HPS));
            Formats.Add(typeof(SHARC));
            Formats.Add(typeof(SHARCFB));
            Formats.Add(typeof(NARC));
        //    Formats.Add(typeof(SDF));
            Formats.Add(typeof(TMPK));

            Formats.Add(typeof(Turbo.Course_MapCamera_bin));
            Formats.Add(typeof(Turbo.PartsBIN));


            return Formats.ToArray();
        }
        #endregion
    }
}