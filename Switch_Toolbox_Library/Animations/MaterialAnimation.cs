using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.NodeWrappers;

namespace Switch_Toolbox.Library.Animations
{
    public class MaterialAnimation : Animation
    {
        public AnimationType AnimType;
        public enum AnimationType
        {
            ShaderParam,
            Color,
            TextureSrt,
            TexturePattern,
            Visibilty,
        }

        public virtual void NextFrame(Viewport viewport)
        {
            if (Frame >= FrameCount) return;
        }

        public List<string> Textures = new List<string>();
        public List<Material> Materials = new List<Material>();

        public class Material : STGenericWrapper
        {
            public List<ParamKeyGroup> Params = new List<ParamKeyGroup>();
            public List<Sampler> Samplers = new List<Sampler>();

            public Material()
            {
                ImageKey = "material";
                SelectedImageKey = "material";
            }
            public Material(string Name)
            {
                Text = Name;
                ImageKey = "material";
                SelectedImageKey = "material";
            }

            public class Sampler : TreeNodeCustom
            {
                public Sampler()
                {
                    ImageKey = "sampler";
                    SelectedImageKey = "sampler";
                }

                public virtual string GetActiveTextureName(int frame) { return ""; }

                public virtual STGenericTexture GetActiveTexture(int Frame) { return null; }

                //Stores texture indices
                public KeyGroup group = new KeyGroup();
            }
        }
        public class ParamKeyGroup : TreeNodeCustom
        {
            public AnimationType Type;

            public string UniformName;

            //Params will have multiple curves for values
            public List<Animation.KeyGroup> Values = new List<Animation.KeyGroup>();
        }
    }
}
