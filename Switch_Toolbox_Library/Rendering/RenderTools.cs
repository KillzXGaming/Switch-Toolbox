using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFGraphics.GLObjects.Textures;
using SFGraphics.GLObjects.Textures.TextureFormats;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Switch_Toolbox.Library.IO;
using GL_EditorFramework.GL_Core;

namespace Switch_Toolbox.Library
{
    public class RenderTools
    {
        public static bool PbrSamplersLoaded
        {
            get
            {
                return (diffusepbr != null &&
                       specularpbr != null &&
                       brdfpbr != null);
            }
        }
    
        private static Texture2D defaulttex;
        public static Texture2D defaultTex
        {
            get
            {
                if (defaulttex == null)
                {
                    defaulttex = new Texture2D();
                    defaultTex.LoadImageData(Properties.Resources.DefaultTexture);
                }

                return defaulttex;
            }
            set
            {
                defaulttex = value;
            }
        }

        private static TextureCubeMap diffusepbr;
        public static TextureCubeMap diffusePbr
        {
            get
            {
                if (diffusepbr == null)
                {
                    DDS diffuseSdr = new DDS(Properties.Resources.diffuseSDR);
                    diffusepbr = DDS.CreateGLCubeMap(diffuseSdr);
                }
                return diffusepbr;
            }
            set
            {
                diffusepbr = value;
            }
        }
        private static TextureCubeMap specularpbr;
        public static TextureCubeMap specularPbr
        {
            get
            {
                if (specularpbr == null)
                {
                    DDS specularSdr = new DDS(Properties.Resources.specularSDR);
                    specularpbr = DDS.CreateGLCubeMap(specularSdr);
                }
                return specularpbr;
            }
            set
            {
                specularpbr = value;
            }
        }
        public static Texture2D uvtestPattern;
        public static Texture2D uvTestPattern
        {
            get
            {
                if (uvtestPattern == null)
                {
                    uvtestPattern = new Texture2D();
                    uvtestPattern.LoadImageData(Properties.Resources.UVPattern);
                    uvtestPattern.TextureWrapR = TextureWrapMode.Repeat;
                    uvtestPattern.TextureWrapS = TextureWrapMode.Repeat;
                    uvtestPattern.TextureWrapT = TextureWrapMode.Repeat;
                }
                return uvtestPattern;
            }
            set
            {
                uvtestPattern = value;
            }
        }

        private static Texture2D boneWeightGradient;
        public static Texture2D BoneWeightGradient
        {
            get
            {
                if (boneWeightGradient == null)
                {
                    boneWeightGradient = new Texture2D();
                    boneWeightGradient.LoadImageData(Properties.Resources.boneWeightGradient);
                    boneWeightGradient.TextureWrapR = TextureWrapMode.Repeat;
                    boneWeightGradient.TextureWrapS = TextureWrapMode.Repeat;
                    boneWeightGradient.TextureWrapT = TextureWrapMode.Repeat;
                }
                return boneWeightGradient;
            }
            set
            {
                boneWeightGradient = value;
            }
        }

        private static Texture2D boneWeightGradient2;
        public static Texture2D BoneWeightGradient2
        {
            get
            {
                if (boneWeightGradient2 == null)
                {
                    boneWeightGradient2 = new Texture2D();
                    boneWeightGradient2.LoadImageData(Properties.Resources.boneWeightGradient2);
                    boneWeightGradient2.TextureWrapR = TextureWrapMode.Repeat;
                    boneWeightGradient2.TextureWrapS = TextureWrapMode.Repeat;
                    boneWeightGradient2.TextureWrapT = TextureWrapMode.Repeat;
                }
                return boneWeightGradient2;
            }
            set
            {
                boneWeightGradient = value;
            }
        }

        private static Texture2D brdfpbr;
        public static Texture2D brdfPbr
        {
            get
            {
                if (brdfpbr == null)
                {
                    DDS brdf = new DDS(Properties.Resources.brsf);
                    var tex = DDS.GetArrayFaces(brdf, 1);

                    brdfpbr = new Texture2D();
                    brdfpbr.LoadImageData((int)brdf.header.width, (int)brdf.header.height, tex[0].mipmaps[0],
                        new TextureFormatUncompressed(PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte));

                    // Don't use mipmaps.
                    diffusePbr.MinFilter = TextureMinFilter.Linear;
                    diffusePbr.MagFilter = TextureMagFilter.Linear;
                }
                return brdfpbr;
            }
            set
            {
                brdfpbr = value;
            }
        }

        public static void DisposeTextures()
        {
            defaultTex = null;
            uvTestPattern = null;
            brdfPbr = null;
            diffusePbr = null;
            specularPbr = null;
        }

        public static void DrawSkyBox(Matrix4 RotationMatrix)
        {
      
        }

        public static void DrawScreenQuad()
        {
            var vert = new VertexShader(
            @"#version 330

            in vec3 position;
            out vec2 texCoord;

            void main()
            {
                texCoord.x = (position.x + 1.0) * 0.5;
                texCoord.y = (position.y + 1.0) * 0.5;
                gl_Position = vec4(position, 1);
            }
            ");
            var frag = new FragmentShader(
                @"﻿#version 330 in vec2 texCoord;

                uniform vec3 topColor;
                uniform vec3 bottomColor;

                out vec4 FragColor;

                void main()
                {
	                FragColor = vec4(1);
	                FragColor.rgb = mix(bottomColor, topColor, texCoord.y);
                }");

            ShaderProgram shader = new ShaderProgram(frag, vert);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();


            shader.SetVector3("topColor", new Vector3(0, 1, 0));
            shader.SetVector3("bottomColor", new Vector3(0, 1, 0));

            DrawScreenTriangle(shader);
        }

        private static void DrawScreenTriangle(ShaderProgram shader)
        {
            float[] vertices =
            {
                -1f, -1f, 0.0f,
                 3f, -1f, 0.0f,
                 -1f, 3f, 0.0f
            };

            int vao;
            GL.GenVertexArrays(1, out vao);
            GL.BindVertexArray(vao);

            int vbo;
            GL.GenBuffers(1, out vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * vertices.Length), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(shader.GetAttribute("position"), 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
            GL.EnableVertexAttribArray(0);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        public static void DrawCube(Vector3 center, float size)
        {
            DrawRectangularPrism(center, size, size, size, false);
        }

        public static void DrawRectangularPrism(Vector3 center, float sizeX, float sizeY, float sizeZ, bool useWireFrame = false)
        {
            PrimitiveType primitiveType = PrimitiveType.Quads;
            if (useWireFrame)
            {
                GL.LineWidth(2);
                primitiveType = PrimitiveType.LineLoop;
            }

            GL.Begin(primitiveType);
            GL.Vertex3(center.X + sizeX, center.Y + sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y + sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y + sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y + sizeY, center.Z + sizeZ);
            GL.End();

            GL.Begin(primitiveType);
            GL.Vertex3(center.X + sizeX, center.Y - sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y - sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y - sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y - sizeY, center.Z - sizeZ);
            GL.End();

            GL.Begin(primitiveType);
            GL.Vertex3(center.X + sizeX, center.Y + sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y + sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y - sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y - sizeY, center.Z + sizeZ);
            GL.End();

            GL.Begin(primitiveType);
            GL.Vertex3(center.X + sizeX, center.Y - sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y - sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y + sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y + sizeY, center.Z - sizeZ);
            GL.End();

            GL.Begin(primitiveType);
            GL.Vertex3(center.X - sizeX, center.Y + sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y + sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y - sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y - sizeY, center.Z + sizeZ);
            GL.End();

            GL.Begin(primitiveType);
            GL.Vertex3(center.X + sizeX, center.Y + sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y + sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y - sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y - sizeY, center.Z - sizeZ);
            GL.End();
        }
    }
}
