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
            public List<SamplerKeyGroup> Samplers = new List<SamplerKeyGroup>();

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
        }

        public class ParamKeyGroup : TreeNodeCustom
        {
            public AnimationType Type;

            public string UniformName;

            //Params will have multiple curves for values
            public List<KeyGroup> Values = new List<KeyGroup>();
        }

        public class SamplerKeyGroup : KeyGroup
        {
            MaterialAnimation MaterialAnimation;

            public SamplerKeyGroup(MaterialAnimation materialAnimation)
            {
                ImageKey = "sampler";
                SelectedImageKey = "sampler";

                MaterialAnimation = materialAnimation;
            }

            public virtual string GetActiveTextureName(int frame)
            {
                float index = GetValue(frame);
                return MaterialAnimation.Textures[(int)index];
            }

            public virtual void SetActiveTextureName(int Frame, string TextureName)
            {
                if (!MaterialAnimation.Textures.Contains(TextureName))
                    MaterialAnimation.Textures.Add(TextureName);

                int index = MaterialAnimation.Textures.IndexOf(TextureName);

                SetValue(index, Frame);
            }



            public virtual STGenericTexture GetActiveTexture(int Frame) { return null; }
        }
    }
}
