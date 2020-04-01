using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.NodeWrappers;

namespace Toolbox.Library.Animations
{
    public class MaterialAnimation : Animation
    {
        public virtual AnimationType AnimType { get; set; }
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
            if (Frame > FrameCount) return;
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

            public virtual string GetActiveTextureNameByIndex(int index)
            {
                return MaterialAnimation.Textures[(int)index];
            }

            public virtual string GetActiveTextureNameByFrame(float frame)
            {
                var keyFrame = GetKeyFrame(frame);
                if (keyFrame == null) return "";
                return MaterialAnimation.Textures[(int)keyFrame.Value];
            }

            public virtual void SetActiveTextureName(int Frame, string TextureName)
            {
                if (!MaterialAnimation.Textures.Contains(TextureName))
                    MaterialAnimation.Textures.Add(TextureName);

                int index = MaterialAnimation.Textures.IndexOf(TextureName);

                SetValue(index, Frame);
            }

            public virtual void ShiftKeyUp(int Frame)
            {
                MaterialAnimation.IsEdited = true;
            }

            public virtual void ShiftKeyDown(int Frame)
            {
                MaterialAnimation.IsEdited = true;
            }

            public virtual void RemoveKeyFrame(int Frame)
            {
                MaterialAnimation.IsEdited = true;
            }

            public virtual void AddKeyFrame(string TextureName, float Frame = -1, bool IsConstant = false)
            {
                Constant = IsConstant;

                //Add the texture if it does not exist for both wrapper and bfres itself
                if (!MaterialAnimation.Textures.Contains(TextureName))
                    MaterialAnimation.Textures.Add(TextureName);

                //Set our index
                int index = MaterialAnimation.Textures.IndexOf(TextureName);

                if (Frame == -1)
                {
                    Frame = EndFrame + 1; //Add to the end of the list by default
                }

                //For non constants we load a curve
                if (!Constant)
                {
                }

                MaterialAnimation.IsEdited = true;
            }

            public virtual STGenericTexture GetActiveTexture(int Frame) { return null; }
        }
    }
}
