using PluginContracts;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using FirstPlugin.Forms;
using FirstPlugin.LuigisMansion.DarkMoon;
using FirstPlugin.LuigisMansion3;

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
                foreach (Type T in LoadCompressionFormats())
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

            readonly STToolStripItem[] toolsExt = new STToolStripItem[3];

            public MenuExt()
            {
                toolsExt[0] = new STToolStripItem("Super Mario Odyssey");
                toolsExt[0].DropDownItems.Add(new STToolStripItem(" Kingdom Selector", OpenKingdomSelector));
                toolsExt[0].DropDownItems.Add(new STToolStripItem(" Costume Selector", OpenSelector));

                toolsExt[1] = new STToolStripItem("Mario Kart 8");
                toolsExt[1].DropDownItems.Add(new STToolStripItem("Probe Light Converter", GenerateProbeLightBounds));

                toolsExt[2] = new STToolStripItem("Breath Of The Wild");
                toolsExt[2].DropDownItems.Add(new STToolStripItem("Actor Editor", ActorEditor));

                toolsExt[1] = new STToolStripItem("Pokemon Sword/Shield");
                toolsExt[1].DropDownItems.Add(new STToolStripItem("Pokemon Loader", PokemonLoaderSwSh));
            }

            private void PokemonLoaderSwSh(object sender, EventArgs args)
            {
                if (!System.IO.Directory.Exists(Runtime.PkSwShGamePath))
                {
                    var result = MessageBox.Show("Please set your Pokemon Sword/Shield game path!");
                    if (result == DialogResult.OK)
                    {
                        FolderSelectDialog ofd = new FolderSelectDialog();
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            Runtime.PkSwShGamePath = ofd.SelectedPath;
                            Config.Save();
                        }
                    }
                }

                PokemonLoaderSwShForm form = new PokemonLoaderSwShForm();
                if (form.ShowDialog() == DialogResult.OK) {
                    if (form.SelectedPokemon != string.Empty)
                    {
                        string path = $"{Runtime.PkSwShGamePath}/bin/archive/pokemon/{form.SelectedPokemon}";
                        if (System.IO.File.Exists(path)) {
                            var file = STFileLoader.OpenFileFormat(path);

                            var currentForm = Runtime.MainForm.ActiveMdiChild;
                            if (currentForm != null && currentForm is ObjectEditor &&
                                Runtime.AddFilesToActiveObjectEditor)
                            {
                                ObjectEditor editor = currentForm as ObjectEditor;
                                editor.AddIArchiveFile(file);
                            }
                            else
                            {
                                ObjectEditor editor = new ObjectEditor();
                                editor.AddIArchiveFile(file);
                                LibraryGUI.CreateMdiWindow(editor);
                            }
                        }
                    }
                }
            }

            private void ActorEditor(object sender, EventArgs args)
            {
                UKing.Actors.BotwActorLoader actorEditor = new UKing.Actors.BotwActorLoader();
            }

            private void OpenKingdomSelector(object sender, EventArgs args)
            {
                SceneSelector sceneSelect = new SceneSelector();
                sceneSelect.LoadDictionary(SMO_Scene.OdysseyStages);
                if (sceneSelect.ShowDialog() == DialogResult.OK)
                    SMO_Scene.LoadStage(sceneSelect.SelectedFile);
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

                    FrameBfres();
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

                BfresEditor bfresEditor = (BfresEditor)LibraryGUI.GetActiveContent(typeof(BfresEditor));
                bfresEditor.DisplayAll = true;
            }

            private ObjectEditor editor;
            private BFRES MainCostume = null;
            public void LoadCostume(string fileName)
            {
                List<BFRES> bfresFiles = new List<BFRES>();

                var FileFormat = STFileLoader.OpenFileFormat(fileName);
                if (FileFormat is SARC)
                {
                    foreach (var file in ((SARC)FileFormat).Files)
                    {
                        string ext = System.IO.Path.GetExtension(file.FileName);
                        if (ext == ".bfres")
                        {
                            bfresFiles.Add((BFRES)STFileLoader.OpenFileFormat(new System.IO.MemoryStream(file.FileData), file.FileName));
                        }
                    }
                }
                if (FileFormat is BFRES)
                    bfresFiles.Add((BFRES)FileFormat);

                if (editor == null)
                {
                    editor = new ObjectEditor();
                    LibraryGUI.CreateMdiWindow(editor);
                }

                if (MainCostume == null && bfresFiles.Count > 0)
                    MainCostume = bfresFiles[0];
            
                foreach (var bfres in bfresFiles)
                {
                    editor.AddNode(bfres);
                    bfres.LoadEditors(null);
                    DiableLoadCheck();
                }
            }

            private void FrameBfres()
            {
                BfresEditor bfresEditor = (BfresEditor)LibraryGUI.GetActiveContent(typeof(BfresEditor));
                bfresEditor.FrameCamera(MainCostume.BFRESRender);

                MainCostume = null;
            }

            private void DiableLoadCheck()
            {
                BfresEditor bfresEditor = (BfresEditor)LibraryGUI.GetActiveContent(typeof(BfresEditor));
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

        private Type[] LoadCompressionFormats()
        {
            List<Type> Formats = new List<Type>();
            Formats.Add(typeof(MeshCodecFormat));
            return Formats.ToArray();
        }

        private Type[] LoadFileFormats()
        {
            List<Type> Formats = new List<Type>();
            Formats.Add(typeof(BFRES));
            Formats.Add(typeof(MT_TEX));
            Formats.Add(typeof(MT_Model));
            Formats.Add(typeof(DKCTF.CModel));
            Formats.Add(typeof(DKCTF.CTexture));
            Formats.Add(typeof(BCSV));
            Formats.Add(typeof(TVOL));
            Formats.Add(typeof(BTI));
            Formats.Add(typeof(TXE));
            Formats.Add(typeof(SARC));
            Formats.Add(typeof(TRPAK));
            Formats.Add(typeof(BNTX));
            Formats.Add(typeof(BEA));
            Formats.Add(typeof(BYAML));
            Formats.Add(typeof(XTX));
            Formats.Add(typeof(BXFNT));
            Formats.Add(typeof(MSBT));
            Formats.Add(typeof(BARS));
            Formats.Add(typeof(GFPAK));
            Formats.Add(typeof(NUTEXB));
            Formats.Add(typeof(NUT));
            Formats.Add(typeof(KCL));
            Formats.Add(typeof(GTXFile));
            Formats.Add(typeof(AAMP));
            Formats.Add(typeof(PTCL));
            Formats.Add(typeof(EFF));
            Formats.Add(typeof(EFCF));
            Formats.Add(typeof(NSP));
            Formats.Add(typeof(BNSH));
            Formats.Add(typeof(BFSHA));
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
            Formats.Add(typeof(SWU));
            Formats.Add(typeof(SPC));
            Formats.Add(typeof(GameDataToc));
            Formats.Add(typeof(NUSHDB));
            Formats.Add(typeof(MKGPDX_PAC));
            Formats.Add(typeof(LZARC));
            Formats.Add(typeof(IGA_PAK));
            Formats.Add(typeof(MKAGPDX_Model));
            Formats.Add(typeof(GFBMDL));
            Formats.Add(typeof(GFBANM));
            Formats.Add(typeof(GFBANMCFG));
            Formats.Add(typeof(Turbo.Course_MapCamera_bin));
            Formats.Add(typeof(SDF));
            Formats.Add(typeof(IStorage));
            Formats.Add(typeof(NCA));
            Formats.Add(typeof(RARC));
            Formats.Add(typeof(ME01));
            Formats.Add(typeof(LM3_DICT));
            Formats.Add(typeof(LM2_DICT));
            Formats.Add(typeof(GMX));
            Formats.Add(typeof(BMD));
            Formats.Add(typeof(GCDisk));
            Formats.Add(typeof(TPL));
            Formats.Add(typeof(BFTTF));
            Formats.Add(typeof(HedgehogLibrary.PACx));
            Formats.Add(typeof(GAR));
            Formats.Add(typeof(CTXB));
            Formats.Add(typeof(CSAB));
            Formats.Add(typeof(CMB));
            Formats.Add(typeof(G1T));
            Formats.Add(typeof(HyruleWarriors.G1M.G1M));
            Formats.Add(typeof(LayoutBXLYT.Cafe.BFLYT));
            Formats.Add(typeof(LayoutBXLYT.BCLYT));
            Formats.Add(typeof(LayoutBXLYT.BRLYT));
            Formats.Add(typeof(LayoutBXLYT.BFLAN));
            Formats.Add(typeof(LayoutBXLYT.BRLAN));
            Formats.Add(typeof(LayoutBXLYT.BCLAN));
            Formats.Add(typeof(ZSI));
            Formats.Add(typeof(IGZ_TEX));
            Formats.Add(typeof(MOD));
            Formats.Add(typeof(U8));
            Formats.Add(typeof(CTPK));
            Formats.Add(typeof(LINKDATA));
            Formats.Add(typeof(NCCH));
            Formats.Add(typeof(NCSD));
            Formats.Add(typeof(CTR.NCCH.RomFS));
            Formats.Add(typeof(DKCTF.MSBT));
            Formats.Add(typeof(DKCTF.PAK));
            Formats.Add(typeof(WTB));
            Formats.Add(typeof(PKZ));
            Formats.Add(typeof(DARC));
            Formats.Add(typeof(BFLIM));
            Formats.Add(typeof(BCLIM));
            Formats.Add(typeof(DAT_Bayonetta));
            Formats.Add(typeof(XCI));
            Formats.Add(typeof(VIBS));
            Formats.Add(typeof(NLG.StrikersRLT));
            Formats.Add(typeof(NLG.StrikersRLG));
            Formats.Add(typeof(PunchOutWii.PO_DICT));
            Formats.Add(typeof(LM2_ARCADE_Model));
            Formats.Add(typeof(NLG_NLOC));
            Formats.Add(typeof(PCK));
            Formats.Add(typeof(NLG.StrikersSAnim));
            Formats.Add(typeof(APAK));
            Formats.Add(typeof(CtrLibrary.BCH));
            Formats.Add(typeof(LZS));
            Formats.Add(typeof(WTA));
            Formats.Add(typeof(BinGzArchive));
            Formats.Add(typeof(BNR));
            Formats.Add(typeof(PKG));
            Formats.Add(typeof(MTXT));
            Formats.Add(typeof(NKN));
            Formats.Add(typeof(MetroidDreadLibrary.BSMAT));
            Formats.Add(typeof(TRANM));
            Formats.Add(typeof(GFA));
            Formats.Add(typeof(TXTG));

            //Formats.Add(typeof(XLINK_FILE));

            //  Formats.Add(typeof(MPBIN));
            //  Formats.Add(typeof(HSF));
            //   Formats.Add(typeof(ATB));

            //   Formats.Add(typeof(LayoutBXLYT.BLO));

            //   Formats.Add(typeof(MSBP));
            //   Formats.Add(typeof(BFGRP));

            //Unfinished wip formats not ready for use
            if (Runtime.DEVELOPER_DEBUG_MODE)
            {
                Formats.Add(typeof(BFSAR));
            }


            return Formats.ToArray();
        }
        #endregion
    }
}