using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using System.IO;
using OpenTK;
using FirstPlugin;
using UKing.Actors.Forms;

namespace UKing.Actors
{
    /// <summary>
    /// A class that holds methods to load actor data for botw.
    /// </summary>
    public class BotwActorLoader
    {
        private static string ActorPath = $"/Actor/Pack/";
        private static string CachedActorsPath = $"/Pack/";

        private static string ActorInfoTable = $"/Actor/ActorInfo.product.sbyml";

        //MSBT stores certain properties and helps define our actor names
        private string ActorMessageData
        {
            get { return $"/Pack/Bootup_{MessageRegion}{MessageLanguage}.pack"; }
        }

        public Language MessageLanguage = Language.en;
        public Region MessageRegion = Region.US;

        public enum Region
        {
            US,
            EU,
            JP,
        }

        public enum Language
        {
            en, //English
            de, //German
            es, //Spanish
            fr, //Frence
            it, //Italian
            nl, //
            ru, //Russian
            ja, //Japanese
        }

        public enum ActorCategory
        {
            Armour,
            Animal,
            Weapon,
            Item,
            Enemy,
            Npc,
        }

        public class ActorDefineInfo
        {
            public string Name { get; set; }

            public ActorDefineInfo(string name)
            {
                Name = name;
            }
        }

        public class ActorInfo
        {
            private const string N_name = "name";
            private const string N_bfres = "bfres";
            private const string N_aabbMin = "aabbMin";
            private const string N_aabbMax = "aabbMax";

            public string MessageName = "";
            public string MessageDescription = "";
            public string MessageFile = "";

            public string Name
            {
                get { return this[N_name] != null ? this[N_name] : ""; }
                set { this[N_name] = value; }
            }

            public string BfresName
            {
                get { return this[N_bfres] != null ? this[N_bfres] : ""; }
                set { this[N_bfres] = value; }
            }

            public Vector3 AABMin
            {
                get
                {
                    if (this[N_aabbMin] == null)
                        return new Vector3(0, 0, 0);

                    return new Vector3(
                     this[N_aabbMin]["X"] != null ? this[N_aabbMin]["X"] : 0,
                     this[N_aabbMin]["Y"] != null ? this[N_aabbMin]["Y"] : 0,
                     this[N_aabbMin]["Z"] != null ? this[N_aabbMin]["Z"] : 0);
                }
                set
                {
                    this[N_aabbMin]["X"] = value.X;
                    this[N_aabbMin]["Y"] = value.Y;
                    this[N_aabbMin]["Z"] = value.Z;
                }
            }

            public Vector3 AABMax
            {
                get
                {
                    if (this[N_aabbMax] == null)
                        return new Vector3(0, 0, 0);

                    return new Vector3(
                     this[N_aabbMax]["X"] != null ? this[N_aabbMax]["X"] : 0,
                     this[N_aabbMax]["Y"] != null ? this[N_aabbMax]["Y"] : 0,
                     this[N_aabbMax]["Z"] != null ? this[N_aabbMax]["Z"] : 0);
                }
                set
                {
                    this[N_aabbMax]["X"] = value.X;
                    this[N_aabbMax]["Y"] = value.Y;
                    this[N_aabbMax]["Z"] = value.Z;
                }
            }

            public ActorInfo(dynamic  node)
            {
                if (node is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)node;
            }

            public Dictionary<string, dynamic> Prop { get; set; } = new Dictionary<string, dynamic>();

            public dynamic this[string name]
            {
                get
                {
                    if (Prop.ContainsKey(name)) return Prop[name];
                    else return null;
                }
                set
                {
                    if (Prop.ContainsKey(name)) Prop[name] = value;
                    else Prop.Add(name, value);
                }
            }
        }

        private ObjectEditor editor;

        public BotwActorLoader()
        {
            editor = new ObjectEditor();
            editor.Text = "Actor Editor BOTW";
            LibraryGUI.CreateMdiWindow(editor);
            editor.SortTreeAscending();

            LoadActors();
        }

        private void LoadActors()
        {
            //Setup a list of nodes based on category
            TreeNode ArmourFolder = new TreeNode("Armours");
            TreeNode WeaponsFolder = new TreeNode("Weapons");
            TreeNode ItemsFolder = new TreeNode("Items");
            TreeNode EnemyFolder = new TreeNode("Enemies");

            if (!Directory.Exists(Runtime.BotwGamePath))
            {
                bool IsValid = NotifySetGamePath();
                if (!IsValid) //Give up loading it if it's wrong
                    return;
            }

            Dictionary<string, TreeNode> ActorIDS = new Dictionary<string, TreeNode>();
            Dictionary<string, ActorInfo> Actors = new Dictionary<string, ActorInfo>();

            if (File.Exists($"{Runtime.BotwGamePath}{ActorInfoTable}"))
            {
                var byml = EveryFileExplorer.YAZ0.Decompress($"{Runtime.BotwGamePath}{ActorInfoTable}");
                var actorInfoProductRoot = ByamlExt.Byaml.ByamlFile.FastLoadN(new MemoryStream(byml)).RootNode;

                if (actorInfoProductRoot.ContainsKey("Actors"))
                {
                    foreach (var actor in actorInfoProductRoot["Actors"])
                    {
                        ActorInfo info = new ActorInfo(actor);
                        if (info.Name != string.Empty)
                        {
                            Actors.Add(info.Name, info);
                        }
                    }
                }
            }

            //Parse message data for our actor names, and additional info to add to the editor

            Console.WriteLine("msbtEXT " + File.Exists($"{Runtime.BotwGamePath}{ActorMessageData}"));
            Console.WriteLine($"{Runtime.BotwGamePath}{ActorMessageData}");

            if (File.Exists($"{Runtime.BotwGamePath}{ActorMessageData}"))
            {
                var msgPack = SARCExt.SARC.UnpackRamN(File.Open($"{Runtime.BotwGamePath}{ActorMessageData}", FileMode.Open));

                //Get the other sarc inside
                foreach (var pack in msgPack.Files)
                {
                    var msgProductPack = SARCExt.SARC.UnpackRamN(EveryFileExplorer.YAZ0.Decompress(pack.Value));

                    //Folders are setup with actors
                    foreach (var msbtFile in msgProductPack.Files)
                    {
                        using (var stream = new MemoryStream(msbtFile.Value))
                        {
                            MSBT msbt = new MSBT();
                            if (!msbt.Identify(stream))
                                continue;

                            msbt.Load(new MemoryStream(msbtFile.Value));


                            //Get our labels and match up with our actors
                            if (msbt.header.Label1 != null)
                            {
                                for (int i = 0; i < msbt.header.Label1.Labels.Count; i++)
                                {
                                    var lbl = msbt.header.Label1.Labels[i];

                                    if (lbl.Name.Contains("_Name"))
                                    {
                                        string actorName = lbl.Name.Replace("_Name", string.Empty);
                                        if (Actors.ContainsKey(actorName))
                                        {
                                            Actors[actorName].MessageFile = Path.GetFileNameWithoutExtension(msbtFile.Key);
                                            Actors[actorName].MessageName = lbl.String.GetText(msbt.header.StringEncoding);
                                        }
                                    }
                                    if (lbl.Name.Contains("_Desc"))
                                    {
                                        string actorName = lbl.Name.Replace("_Desc", string.Empty);
                                        if (Actors.ContainsKey(actorName))
                                        {
                                            Actors[actorName].MessageFile = Path.GetFileNameWithoutExtension(msbtFile.Key);
                                            Actors[actorName].MessageDescription = lbl.String.GetText(msbt.header.StringEncoding);
                                        }
                                    }
                                }
                            }

                            msbt.Unload();
                        }
                    }
                }
            }

            Dictionary<string, TreeNode> Categories = new Dictionary<string, TreeNode>();

            foreach (var info in Actors)
            {
                if (info.Value.MessageName != string.Empty)
                {
                    //Temp atm. Use message file name for organing
                    string catgeory = info.Value.MessageFile;

                    if (!Categories.ContainsKey(catgeory))
                    {
                        TreeNode node = new TreeNode(catgeory);
                        editor.AddNode(node);
                        Categories.Add(catgeory, node);
                    }

                    ActorEntry entry = new ActorEntry();
                    entry.Info = info.Value;
                    entry.Text = info.Value.MessageName;
                    Categories[catgeory].Nodes.Add(entry);
                    entry.ReloadActorProperties();
                }
            }

            Categories.Clear();
            GC.Collect();
        }

        private bool NotifySetGamePath()
        {
            string dir = "";

            var result = MessageBox.Show("Please set your game path for botw", "Actor Loader", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            if (result == DialogResult.OK)
            {
                FolderSelectDialog folderSelect = new FolderSelectDialog();
                if (folderSelect.ShowDialog() == DialogResult.OK)
                {
                    dir = folderSelect.SelectedPath;
                    Runtime.BotwGamePath = dir;
                    Toolbox.Library.Config.Save();
                }
            }

            return IsValidDirectory(dir);
        }

        private bool IsValidDirectory(string directory)
        {
            bool HasActors = Directory.Exists($"{directory}\\Actor");
            return HasActors ;
        }

        /// <summary>
        /// A container for multiple actors that link to each other.
        /// </summary>
        public class ActorCotainer : TreeNodeCustom
        {
            public List<ActorEntry> SubActors = new List<ActorEntry>();
        }

        public class ActorEntry : TreeNodeCustom
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }

            public ActorCategory Category { get; set; }

            public ActorParameters Parameters { get; set; }
            public ActorModel Models { get; set; }
            public ActorTextures Textures { get; set; }

            public ActorInfo Info { get; set; }

            public void ReloadActorProperties()
            {
                Textures = new ActorTextures();
                Models = new ActorModel();
                Parameters = new ActorParameters();

                string bfresName = Info.BfresName;

                //Load our texture paths if they exist
                string texPathNX = $"{Runtime.BotwGamePath}/Model/{bfresName}.Tex.sbfres";
                string tex1Path =  $"{Runtime.BotwGamePath}/Model/{bfresName}.Tex1.sbfres";
                string tex2Path =  $"{Runtime.BotwGamePath}/Model/{bfresName}.Tex2.sbfres";

                if (File.Exists(texPathNX))
                    Textures.FilePathTex1 = texPathNX;
                if (File.Exists(tex1Path))
                    Textures.FilePathTex1 = tex1Path;
                if (File.Exists(tex2Path))
                    Textures.FilePathTex2 = tex2Path;

                //Load model and animation paths if they exist
                string modelPath = $"{Runtime.BotwGamePath}/Model/{bfresName}.sbfres";
                string animationPath = $"{Runtime.BotwGamePath}/Model/{bfresName}_Animation.sbfres";

                if (File.Exists(modelPath))
                    Models.FilePathModel = modelPath;
                if (File.Exists(animationPath))
                    Models.FilePathAnimation = animationPath;

                //Load any cached paths

            }

            public override void OnClick(TreeView treeview)
            {
                UpdateEditor();
            }
            public void UpdateEditor()
            {
                BotwActorEditorControl editor = (BotwActorEditorControl)LibraryGUI.GetActiveContent(typeof(BotwActorEditorControl));
                if (editor == null)
                {
                    editor = new BotwActorEditorControl();
                    editor.Dock = DockStyle.Fill;

                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.LoadActor(this);
            }
        }

        public class ActorParameters
        {

        }

        public class ActorTextures
        {
            /// <summary>
            /// The file path for the main texture data (no mips for wii u)
            /// For Switch this stores both
            /// </summary>
            public string FilePathTex1 { get; set; }

            /// <summary>
            /// The file path for the mip map texture data (for wii u)
            /// </summary>
            public string FilePathTex2 { get; set; }
        }

        public class ActorModel
        {
            /// <summary>
            /// The file path that stores the model for an actor
            /// </summary>
            public string FilePathModel { get; set; }

            /// <summary>
            /// The file path that stores the animation for an actor
            /// </summary>
            public string FilePathAnimation { get; set; }
        }
    }
}
