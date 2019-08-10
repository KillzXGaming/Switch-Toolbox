using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using System.IO;

namespace FirstPlugin
{
    /// <summary>
    /// A class that holds methods to load actor data for botw.
    /// </summary>
    public class BotwActorLoader
    {
        private static string ActorPath = $"/Actor/Pack/";
        private static string CachedActorsPath = $"/Pack/";

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

        public static Dictionary<string, ActorDefineInfo> ArmorActorDefine = new Dictionary<string, ActorDefineInfo>()
        {
            {"001", new ActorDefineInfo("Hylian Tunic Set") },
            {"002",new ActorDefineInfo("Hylian Tunic Set Upgraded") },
            {"003",new ActorDefineInfo("Hylian Tunic Set Upgraded 2") },
            {"004",new ActorDefineInfo("Hylian Tunic Set Upgraded 3") },
            {"005",new ActorDefineInfo("Tunic of the Wild Set") },
            {"006",new ActorDefineInfo("Zora Set") },
            {"007",new ActorDefineInfo("Zora Set Upgraded") },
            {"008",new ActorDefineInfo("Desert Voe Set") },
            {"009",new ActorDefineInfo("Snowquill Set" )},
            {"011",new ActorDefineInfo("Flamebreaker Set") },
            {"012",new ActorDefineInfo("Stealth Set" )},
            {"014",new ActorDefineInfo("Climbing Gear Set" )},
            {"017",new ActorDefineInfo("Radiant  Set" )},
            {"020",new ActorDefineInfo("Soldier's Armor  Set" )},
            {"021",new ActorDefineInfo("Ancient Set") },
            {"022",new ActorDefineInfo("Bokoblin Mask") },
            {"024",new ActorDefineInfo("Diamond Circlet") },
            {"025",new ActorDefineInfo("Ruby Circlet") },
            {"026",new ActorDefineInfo("Sapphire Circlet") },
            {"027",new ActorDefineInfo("Topaz Circlet") },
            {"028",new ActorDefineInfo("Opal Circlet") },
            {"029",new ActorDefineInfo("Amber Circlet") },
        };

        private ObjectEditor editor;

        public BotwActorLoader()
        {
            editor = new ObjectEditor();
            editor.Text = "Actor Editor BOTW";
            LibraryGUI.CreateMdiWindow(editor);

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

            //Load all our actors into a class
            foreach (var file in Directory.GetFiles($"{Runtime.BotwGamePath}{ActorPath}"))
            {
                string name = Path.GetFileNameWithoutExtension(file);

                var actorType = name.Split('_').First();
                var actorID = name.Split('_').Skip(1).FirstOrDefault();
                var actorProperty = name.Split('_').Last();

                if (actorType == "Armor")
                {
                    if (ArmorActorDefine.ContainsKey(actorID))
                    {
                        ActorDefineInfo info = ArmorActorDefine[actorID];

                        ActorEntry entry = new ActorEntry();
                        entry.Text = info.Name;
                        entry.FilePath = file;
                        entry.FileName = name;
                        entry.Category = ActorCategory.Armour;
                        ArmourFolder.Nodes.Add(entry);
                    }
                }
                else if (actorType == "Animal")
                {

                }
                else if (actorType == "Npc")
                {

                }
            }

            //The game also caches certain actors to the pack folder at boot


            if (ArmourFolder.Nodes.Count != 0)
                editor.AddNode(ArmourFolder);
            if (EnemyFolder.Nodes.Count != 0)
                editor.AddNode(EnemyFolder);
            if (ItemsFolder.Nodes.Count != 0)
                editor.AddNode(ItemsFolder);
            if (WeaponsFolder.Nodes.Count != 0)
                editor.AddNode(WeaponsFolder);
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
                    Config.Save();
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

            public void ReloadActorProperties()
            {
                Textures = new ActorTextures();
                Models = new ActorModel();
                Parameters = new ActorParameters();

                //Load our texture paths if they exist

            }

            public override void OnClick(TreeView treeview)
            {

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
