using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Toolbox.Library
{
    //Class based on https://github.com/ScanMountGoat/SFGraphics/blob/2cba15420b40d42c4254583336dbc3ca6a0d28dc/Projects/SFGraphics/GLObjects/Textures/Texture.cs
    //This makes managing textures easier
    public class OpenGLTexture
    {
        public int TexID;

        public int Width { get; protected set; }

        public int Height { get; protected set; }

        public TextureTarget TextureTarget { get; }

        private TextureMinFilter minFilter;
        public TextureMinFilter MinFilter
        {
            get { return minFilter; }
            set
            {
                minFilter = value;
                SetTexParameter(TextureParameterName.TextureMinFilter, (int)value);
            }
        }

        private TextureMagFilter magFilter;
        public TextureMagFilter MagFilter
        {
            get { return magFilter; }
            set
            {
                magFilter = value;
                SetTexParameter(TextureParameterName.TextureMagFilter, (int)value);
            }
        }

        private TextureWrapMode textureWrapS;
        public TextureWrapMode TextureWrapS
        {
            get { return textureWrapS; }
            set
            {
                textureWrapS = value;
                SetTexParameter(TextureParameterName.TextureWrapS, (int)value);
            }
        }

        private TextureWrapMode textureWrapT;
        public TextureWrapMode TextureWrapT
        {
            get { return textureWrapT; }
            set
            {
                textureWrapT = value;
                SetTexParameter(TextureParameterName.TextureWrapT, (int)value);
            }
        }

        private TextureWrapMode textureWrapR;
        public TextureWrapMode TextureWrapR
        {
            get { return textureWrapR; }
            set
            {
                textureWrapR = value;
                SetTexParameter(TextureParameterName.TextureWrapR, (int)value);
            }
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget, TexID);
        }

        private void SetTexParameter(TextureParameterName param, int value)
        {
            Bind();
            GL.TexParameter(TextureTarget, param, value);
        }
    }
}
