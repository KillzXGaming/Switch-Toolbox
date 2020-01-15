using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.NodeWrappers;

namespace FirstPlugin.CtrLibrary
{
    public enum BCHGroupType
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


    public class BCHGroupNode : STGenericWrapper
    {
        public Dictionary<string, STGenericWrapper> ResourceNodes = new Dictionary<string, STGenericWrapper>();

        public BCHGroupType Type;

        public BCHGroupNode() : base()
        {
            ImageKey = "folder";
        }

        public BCHGroupNode(string name) : base() { Text = name; }
        public BCHGroupNode(BCHGroupType type) : base() { Type = type; SetNameByType(); }

        public void SetNameByType()
        {
            Text = SetName();
        }

        private string SetName()
        {
            switch (Type)
            {
                case BCHGroupType.Models: return "Models";
                case BCHGroupType.Textures: return "Textures";
                case BCHGroupType.Lookups: return "Lookups";
                case BCHGroupType.Materials: return "Materials";
                case BCHGroupType.Shaders: return "Shaders";
                case BCHGroupType.Cameras: return "Cameras";
                case BCHGroupType.Lights: return "Lights";
                case BCHGroupType.Fogs: return "Fogs";
                case BCHGroupType.Scenes: return "Scenes";
                case BCHGroupType.SkeletalAnim: return "Skeletal Animations";
                case BCHGroupType.MaterialAnim: return "Material Animations";
                case BCHGroupType.VisibiltyAnim: return "Visibilty Animations";
                case BCHGroupType.CameraAnim: return "Camera Animations";
                case BCHGroupType.LightAnim: return "Light Animations";
                case BCHGroupType.EmitterAnim: return "Emitter Animations";
                case BCHGroupType.Particles: return "Particles";
                default:
                    throw new System.Exception("Unknown type? " + Type);
            }
        }

        public void AddNode(STGenericWrapper node)
        {
            if (node.Text == string.Empty)
                throw new System.Exception("Text invalid. Must not be empty!");

            Nodes.Add(node);
            ResourceNodes.Add(node.Text, node);
        }
    }
}
