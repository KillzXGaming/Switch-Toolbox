using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.GL_Core;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Toolbox.Library.Rendering
{
    public class PBRMapGenerator
    {
        public int BrdfLUTMap = -1;

        private ShaderProgram BrdfShader;

        private int captureFBO = 1;
        private int captureRBO = 1;

        private bool IsBufferCreated()
        {
            return (captureFBO != -1 && captureRBO != -1);
         }

        private void GenerateFrameBuffer()
        {
            // setup framebuffer
            GL.GenFramebuffers(1, out captureFBO);
            GL.GenFramebuffers(1, out captureRBO);
        }

        public void GeneratImageBasedLightingMap()
        {
            // enable seamless cubemap sampling for lower mip levels in the pre-filter map.
            GL.Enable(EnableCap.TextureCubeMapSeamless);

            if (!IsBufferCreated())
                GenerateFrameBuffer();
        }

        public void GenerateBrdfMap(GLControl control)
        {
            if (!IsBufferCreated())
                GenerateFrameBuffer();

            BrdfShader.Use(control);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            RenderQuad();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            BrdfLUTMap = LoadBrdfLUTTexture();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, captureFBO);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, captureRBO);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, 512, 512);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, BrdfLUTMap, 0);

            Matrix4 proj = Matrix4.Identity;
        }

        public static int LoadBrdfLUTTexture()
        {
            int texture;
            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rg32f, 512, 512, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rg, PixelType.Float, (IntPtr)0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            return texture;
        }

        private static void RenderQuad()
        {
            int quadVAO = 0;
            int quadVBO;
            if (quadVAO == 0)
            {
                float[] vertices = {
          // positions        // texture Coords
            -1.0f,  1.0f, 0.0f, 0.0f, 1.0f,
            -1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
             1.0f,  1.0f, 0.0f, 1.0f, 1.0f,
             1.0f, -1.0f, 0.0f, 1.0f, 0.0f,
        };

                GL.GenVertexArrays(1, out quadVAO);
                GL.GenBuffers(1, out quadVBO);
                GL.BindVertexArray(quadVAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, 4 * vertices.Length, vertices, BufferUsageHint.StaticDraw);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (IntPtr)0);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            }
            GL.BindVertexArray(quadVAO);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
            GL.BindVertexArray(0);
        }
    }
}
