using PluginContracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.IO;
using FirstPlugin.Forms;

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

        class MenuExt : IMenuExtension
        {
            public STToolStripItem[] FileMenuExtensions => null;
            public STToolStripItem[] ToolsMenuExtensions => toolsExt;
            public STToolStripItem[] TitleBarExtensions => null;

            readonly STToolStripItem[] toolsExt = new STToolStripItem[2];
            public MenuExt()
            {
                toolsExt[0] = new STToolStripItem("Super Mario Odyssey");
                toolsExt[0].DropDownItems.Add(new STToolStripItem(" Costume Selector", OpenSelector));

                toolsExt[1] = new STToolStripItem("Mario Kart 8");
                toolsExt[1].DropDownItems.Add(new STToolStripItem("Probe Light Converter", GenerateProbeLightBounds));
            }

            private void GenerateProbeLightBounds(object sender, EventArgs args) {
                AAMP.GenerateProbeBoundings();
            }

            private void OpenSelector(object sender, EventArgs args)
            {
                if (System.IO.Directory.Exists(Runtime.SmoGamePath))
                {
                    OpenCostumeDialog(Runtime.SmoGamePath);
                }
                else
                {
                    var result = MessageBox.Show("Please set your Mario Odyssey game path!");
                    if (result == DialogResult.OK)
                    {
                        FolderSelectDialog ofd = new FolderSelectDialog();
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            Runtime.SmoGamePath = ofd.SelectedPath;
                            Config.Save();
                            OpenCostumeDialog(Runtime.SmoGamePath);
                        }
                    }
                }
            }

            private void OpenCostumeDialog(string GamePath)
            {
                var costumSelector = new OdysseyCostumeSelector(GamePath);
                if (costumSelector.ShowDialog() == DialogResult.OK)
                {
                    LoadCostumes(costumSelector.SelectedCostumeName);
                }
            }

            public void LoadCostumes(string fileName)
            {
                editor = null;

                fileName = System.IO.Path.ChangeExtension(fileName, null);

                List<string> CostumeNames = new List<string>();
                CostumeNames.Add($"{fileName}.szs");
                CostumeNames.Add($"{fileName}Face.szs");
                CostumeNames.Add($"{fileName}Eye.szs");
                CostumeNames.Add($"{fileName}Head.szs");
                CostumeNames.Add($"{fileName}HeadTexture.szs");
                CostumeNames.Add($"{fileName}Under.szs");
                CostumeNames.Add($"{fileName}HandL.szs");
                CostumeNames.Add($"{fileName}HandR.szs");
                CostumeNames.Add($"{fileName}HandTexture.szs");
                CostumeNames.Add($"{fileName}BodyTexture.szs");
                CostumeNames.Add($"{fileName}Shell.szs");
                CostumeNames.Add($"{fileName}Tail.szs");
                CostumeNames.Add($"{fileName}Hair.szs");
                //     CostumeNames.Add($"{fileName}Hakama.szs");
                CostumeNames.Add($"{fileName}Skirt.szs");
                //     CostumeNames.Add($"{fileName}Poncho.szs");
                CostumeNames.Add($"{fileName}Guitar.szs");

                foreach (string path in CostumeNames)
                {
                    Console.WriteLine("Path = " + path);

                    if (System.IO.File.Exists(path))
                    {
                        LoadCostume(path);
                    }
                    else
                    {
                        //Load default meshes unless it's these file names
                        List<string> ExcludeFileList = new List<string>(new string[] {
                    "MarioHack","MarioDot",
                     });

                        bool Exluded = ExcludeFileList.Any(path.Contains);

                        if (Exluded == false)
                        {
                            string parent = System.IO.Directory.GetParent(path).FullName;

                            if (path.Contains($"{fileName}Face"))
                                LoadCostume($"{parent}\\MarioFace.szs");
                            else if (path.Contains($"{fileName}Eye"))
                                LoadCostume($"{parent}\\MarioEye.szs");
                            else if (path.Contains($"{fileName}HeadTexture"))
                                LoadCostume($"{parent}\\MarioHeadTexture.szs");
                            else if (path.Contains($"{fileName}Head"))
                                LoadCostume($"{parent}\\MarioHead.szs");
                            else if (path.Contains($"{fileName}HandL"))
                                LoadCostume($"{parent}\\MarioHandL.szs");
                            else if (path.Contains($"{fileName}HandR"))
                                LoadCostume($"{parent}\\MarioHandR.szs");
                            else if (path.Contains($"{fileName}HandTexture"))
                                LoadCostume($"{parent}\\MarioHandTexture.szs");

                        }
                    }
                }
            }

            private ObjectEditor editor;
            public void LoadCostume(string fileName)
            {
                List<BFRES> bfresFiles = new List<BFRES>();

                var FileFormat = STFileLoader.OpenFileFormat(fileName);
                if (FileFormat is SARC)
                {
                    foreach (var file in ((SARC)FileFormat).Files)
                    {
                        string ext = System.IO.Path.GetExtension(file.Key);
                        if (ext == ".bfres")
                        {
                            bfresFiles.Add((BFRES)STFileLoader.OpenFileFormat(file.Key, file.Value));
                        }
                    }
                }
                if (FileFormat is BFRES)
                    bfresFiles.Add((BFRES)FileFormat);

                if (editor == null)
                {
                    editor = new ObjectEditor();
                    LibraryGUI.Instance.CreateMdiWindow(editor);
                }

                foreach (var bfres in bfresFiles)
                {
                    editor.AddNode(bfres);
                    bfres.LoadEditors(null);
                    DiableLoadCheck();
                }
            }

            private void DiableLoadCheck()
            {
                BfresEditor bfresEditor = (BfresEditor)LibraryGUI.Instance.GetActiveContent(typeof(BfresEditor));
                bfresEditor.IsLoaded = false;
                bfresEditor.DisplayAllDDrawables();
            }
        }

        private Type[] LoadMenus()
        {
            List<Type> MenuItems = new List<Type>();
            MenuItems.Add(typeof(MenuExt));
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

            //     Formats.Add(typeof(BFSAR));
            Formats.Add(typeof(BARS));

            Formats.Add(typeof(GFPAK));
            Formats.Add(typeof(NUTEXB));
            Formats.Add(typeof(NUT));
            Formats.Add(typeof(GTXFile));
            Formats.Add(typeof(AAMP));
            Formats.Add(typeof(PTCL));
            Formats.Add(typeof(EFF));
            Formats.Add(typeof(EFCF));
            

            Formats.Add(typeof(NSP));
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
            Formats.Add(typeof(TMPK));
            Formats.Add(typeof(TEX3DS));
            Formats.Add(typeof(NXARC));
            Formats.Add(typeof(SP2));
            Formats.Add(typeof(NUSHDB));
            Formats.Add(typeof(MKGPDX_PAC));
            Formats.Add(typeof(LZARC));
            Formats.Add(typeof(IGA_PAK));
            Formats.Add(typeof(MKAGPDX_Model));

            Formats.Add(typeof(Turbo.Course_MapCamera_bin));
            Formats.Add(typeof(Turbo.PartsBIN));
            Formats.Add(typeof(SDF));

            //Unfinished wip formats not ready for use
            if (Runtime.DEVELOPER_DEBUG_MODE)
            {
                Formats.Add(typeof(GFBMDL));
                Formats.Add(typeof(NCA));
                Formats.Add(typeof(XCI));
                Formats.Add(typeof(BFLAN));
                Formats.Add(typeof(BFLYT));
                Formats.Add(typeof(XLINK));
            }


            return Formats.ToArray();
        }
        #endregion
    }
}