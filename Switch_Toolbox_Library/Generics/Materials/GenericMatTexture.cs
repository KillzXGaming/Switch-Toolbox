using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Toolbox.Library
{
    public enum STTextureWrapMode
    {
        Repeat,
        Mirror,
        Clamp,
    }

    public enum STTextureMagFilter
    {
        Nearest,
        Linear,
    }

    public enum STTextureMinFilter
    {
        LinearMipMapNearest,
        Nearest,
        Linear,
        NearestMipmapLinear,
        NearestMipmapNearest,
    }

    public class STTextureTransform
    {
        public Vector2 Scale { get; set; }
        public float Rotate { get; set; }
        public Vector2 Translate { get; set; }
    }

    public class STGenericMatTexture
    {
        public int mapMode = 0;
        public STTextureWrapMode WrapModeS = STTextureWrapMode.Repeat;
        public STTextureWrapMode WrapModeT = STTextureWrapMode.Repeat;
        public STTextureWrapMode WrapModeW = STTextureWrapMode.Clamp;

        public STTextureMinFilter MinFilter = STTextureMinFilter.Linear;
        public STTextureMagFilter MagFilter = STTextureMagFilter.Linear;

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

        /// <summary>
        /// Gets the texture that links to this material texture map
        /// Used for UV editor
        /// </summary>
        /// <returns></returns>
        public virtual STGenericTexture GetTexture()
        {
            return null;
        }

        public virtual STTextureTransform Transform { get; set; }

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

        public static readonly Dictionary<STTextureMinFilter, TextureMinFilter> minfilter = new Dictionary<STTextureMinFilter, TextureMinFilter>()
        {
            {  STTextureMinFilter.LinearMipMapNearest, TextureMinFilter.LinearMipmapLinear},
            {  STTextureMinFilter.Nearest, TextureMinFilter.Nearest},
            {  STTextureMinFilter.Linear, TextureMinFilter.Linear},
            {  STTextureMinFilter.NearestMipmapLinear, TextureMinFilter.NearestMipmapLinear},
            {  STTextureMinFilter.NearestMipmapNearest, TextureMinFilter.NearestMipmapNearest},
        };
        public static readonly Dictionary<STTextureMagFilter, TextureMagFilter> magfilter = new Dictionary<STTextureMagFilter, TextureMagFilter>()
        {
            { STTextureMagFilter.Linear, TextureMagFilter.Linear},
            { STTextureMagFilter.Nearest, TextureMagFilter.Nearest},
            { (STTextureMagFilter)3, TextureMagFilter.Linear},
        };

        public static Dictionary<STTextureWrapMode, TextureWrapMode> wrapmode = new Dictionary<STTextureWrapMode, TextureWrapMode>(){
            { STTextureWrapMode.Repeat, TextureWrapMode.Repeat},
            { STTextureWrapMode.Mirror, TextureWrapMode.MirroredRepeat},
            { STTextureWrapMode.Clamp, TextureWrapMode.ClampToEdge},
            { (STTextureWrapMode)3, TextureWrapMode.ClampToEdge},
            { (STTextureWrapMode)4, TextureWrapMode.ClampToEdge},
            { (STTextureWrapMode)5, TextureWrapMode.ClampToEdge},
        };
    }
}
