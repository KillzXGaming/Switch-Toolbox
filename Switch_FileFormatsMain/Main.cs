using PluginContracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
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

        public Form MainForm { get; set; }
        public DockContentST DockedEditor { get; set; }

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
        public static Form MainF;
        public static DockContent DockedViewport;
        public static DockContent DockedEditorS;

        public void Load()
        {
            MainF = MainForm;

            Config.StartupFromFile("Lib/Plugins/config.xml");
            if (!Runtime.DisableViewport)
            {
                DockedViewport = DockedEditor;
                DockedEditor = Viewport.Instance;
            }
            else
            {
                PluginRuntime.FSHPDockState = DockState.Document;
            }


        }
        public void Unload()
        {
            PluginRuntime.bntxContainers.Clear();
        }

        class OdysseyCostumeSelectorMenu : IMenuExtension
        {
            public ToolStripItemDark[] FileMenuExtensions => null;
            public ToolStripItemDark[] ToolsMenuExtensions => toolsExt;
            public ToolStripItemDark[] TitleBarExtensions => null;

            readonly ToolStripItemDark[] toolsExt = new ToolStripItemDark[1];
            public OdysseyCostumeSelectorMenu()
            {
                toolsExt[0] = new ToolStripItemDark("Odyssey Costume Selctor");
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
            Formats.Add(typeof(BNTX));
            Formats.Add(typeof(BEA));
            Formats.Add(typeof(BYAML));
            Formats.Add(typeof(XTX));
            Formats.Add(typeof(KCL));
            Formats.Add(typeof(BFFNT));
            Formats.Add(typeof(MSBT));
            Formats.Add(typeof(BFSAR));
            Formats.Add(typeof(BARS));
            Formats.Add(typeof(BFLAN));
            Formats.Add(typeof(BFLYT));
            Formats.Add(typeof(CsvModel));
            Formats.Add(typeof(GFPAK));

            
            return Formats.ToArray();
        }
        #endregion
    }
}