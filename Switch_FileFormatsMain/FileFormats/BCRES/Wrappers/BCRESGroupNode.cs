using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.NodeWrappers;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public enum BCRESGroupType
    {
        Models,
        Textures,
        Lookups,
        Materials,
        Shaders,
        Cameras,
        Lights,
        Fogs,
        Scenes,
        SkeletalAnim,
        MaterialAnim,
        VisibiltyAnim,
        CameraAnim,
        LightAnim,
        EmitterAnim,
        Particles,
    }

    public class BCRESGroupNode : STGenericWrapper
    {
        public Dictionary<string, STGenericWrapper> ResourceNodes = new Dictionary<string, STGenericWrapper>();

        public BCRESGroupType Type;

        public BCRESGroupNode() : base()
        {
            ImageKey = "folder";

            LoadContextMenus();
        }

        public BCRESGroupNode(string name) : base() { Text = name; }
        public BCRESGroupNode(BCRESGroupType type) : base() { Type = type; SetNameByType(); }

        public override void LoadContextMenus()
        {
            ContextMenuStrip = new STContextMenuStrip();

            CanExport = false;
            CanReplace = false;
            CanRename = false;
            CanDelete = false;
        }

        public void SetNameByType()
        {
            Text = SetName();
        }

        private string SetName()
        {
            switch (Type)
            {
                case BCRESGroupType.Models: return "Models";
                case BCRESGroupType.Textures: return "Textures";
                case BCRESGroupType.Lookups: return "Lookups";
                case BCRESGroupType.Materials: return "Materials";
                case BCRESGroupType.Shaders: return "Shaders";
                case BCRESGroupType.Cameras: return "Cameras";
                case BCRESGroupType.Lights: return "Lights";
                case BCRESGroupType.Fogs: return "Fogs";
                case BCRESGroupType.Scenes: return "Scenes";
                case BCRESGroupType.SkeletalAnim: return "Skeletal Animations";
                case BCRESGroupType.MaterialAnim: return "Material Animations";
                case BCRESGroupType.VisibiltyAnim: return "Visibilty Animations";
                case BCRESGroupType.CameraAnim: return "Camera Animations";
                case BCRESGroupType.LightAnim: return "Light Animations";
                case BCRESGroupType.EmitterAnim: return "Emitter Animations";
                case BCRESGroupType.Particles: return "Particles";
                default:
                    throw new System.Exception("Unknown type? " + Type);
            }
        }

        public void AddNode(STGenericWrapper node)
        {
            if (node.Text == string.Empty)
                throw new System.Exception("Text invalid. Must not be empty! ");

            Nodes.Add(node);
            ResourceNodes.Add(node.Text, node);
        }
    }
}
