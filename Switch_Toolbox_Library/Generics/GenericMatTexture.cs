using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Toolbox.Library
{
    public class STGenericMatTexture
    {
        public int mapMode = 0;
        public int wrapModeS = 1;
        public int wrapModeT = 1;
        public int wrapModeW = 1; //Used for 3D textures
        public int minFilter = 3;
        public int magFilter = 2;
        public int mipDetail = 6;
        public string Name;
        public string SamplerName;
        public int textureUnit;

        public TextureState textureState = TextureState.Unbinded;

        //Determine the state the texture is in. 
        //If the texture is binded we don't need to bind it again unless it's animated or replaced
        public enum TextureState
        {
            Unbinded,
            Binded,
            Animated,
            Replaced,
        }

        public TextureType Type;

        //An enum for the assumed texture type by sampler
        //Many games have a consistant type of samplers and type. _a0 for diffuse, _n0 for normal, ect
        public enum TextureType
        {
            Unknown = 0,
            Diffuse = 1,
            Normal = 2,
            Specular = 3,
            Emission = 4,
            DiffuseLayer2 = 5,
            TeamColor = 6,
            Transparency = 7,
            Shadow = 8,
            AO = 9,
            Light = 10,
            Roughness = 11,
            Metalness = 12,
            MRA = 13, //Combined pbr texture HAL uses for KSA
            SphereMap = 14,
            SubSurfaceScattering = 15,
        }

        public static readonly Dictionary<int, TextureMinFilter> minfilter = new Dictionary<int, TextureMinFilter>()
        {
            { 0x00, TextureMinFilter.LinearMipmapLinear},
            { 0x01, TextureMinFilter.Nearest},
            { 0x02, TextureMinFilter.Linear},
            { 0x03, TextureMinFilter.NearestMipmapLinear},
        };
        public static readonly Dictionary<int, TextureMagFilter> magfilter = new Dictionary<int, TextureMagFilter>()
        {
            { 0x00, TextureMagFilter.Linear},
            { 0x01, TextureMagFilter.Nearest},
            { 0x02, TextureMagFilter.Linear}
        };
        public static Dictionary<int, TextureWrapMode> wrapmode = new Dictionary<int, TextureWrapMode>(){
                    { 0x00, TextureWrapMode.Repeat},
                    { 0x01, TextureWrapMode.MirroredRepeat},
                    { 0x02, TextureWrapMode.ClampToEdge},
                    { 0x03, TextureWrapMode.MirroredRepeat},
        };
    }
}
