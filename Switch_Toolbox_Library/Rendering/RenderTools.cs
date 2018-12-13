using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFGraphics.GLObjects.Textures;
using SFGraphics.GLObjects.Textures.TextureFormats;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Smash_Forge.Rendering;
using Switch_Toolbox.Library.IO;

namespace Switch_Toolbox.Library
{
    public class RenderTools
    {
        public static Texture2D defaultTex;
        public static TextureCubeMap diffusePbr;
        public static TextureCubeMap specularPbr;
        public static Texture2D uvTestPattern;

        public static void LoadTextures()
        {
            defaultTex = new Texture2D();
            defaultTex.LoadImageData(Properties.Resources.DefaultTexture);
            uvTestPattern = new Texture2D();
            uvTestPattern.LoadImageData(Properties.Resources.UVPattern);

            if (Runtime.EnablePBR)
            {
                byte[] decompSpecular = STLibraryCompression.GZIP.Decompress(Properties.Resources.specularSDR);
                DDS specularSdr = new DDS(decompSpecular);
              //  specularPbr = DDS.CreateGLCubeMap(specularSdr);
            }


            //   DDS diffuseSdr = new DDS(Properties.Resources.diffuseSDR);
            // diffusePbr = CreateTextureCubeMap(bntx.textureData.texture);

            // Don't use mipmaps.
            //    diffusePbr.MinFilter = TextureMinFilter.Linear;
            //     diffusePbr.MagFilter = TextureMagFilter.Linear;
        }

        public static void DrawSkyBox(Matrix4 RotationMatrix)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            GL.Enable(EnableCap.LineSmooth);

            GL.Enable(EnableCap.StencilTest);
            GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);


            SFGraphics.GLObjects.Shaders.Shader shader = OpenTKSharedResources.shaders["HDRSkyBox"];
            shader.UseProgram();
            // enable seamless cubemap sampling for lower mip levels in the pre-filter map.
            GL.Enable(EnableCap.TextureCubeMapSeamless);

            Matrix4 proj = Matrix4.Identity;
            shader.SetMatrix4x4("projection", ref proj);
            Matrix4 rot = RotationMatrix;
            shader.SetMatrix4x4("rotView", ref rot);

          //  shader.SetTexture("environmentMap", diffusePbr, 16);

            DrawCube();
        }

        public static void DrawCube()
        {
            int cubeVBO = 0;

            if (cubeVBO == 0)
            {
                float[] vertices = {
            // back face
            -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f, // bottom-left
             1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f, // top-right
             1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 1.0f, 0.0f, // bottom-right         
             1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 1.0f, 1.0f, // top-right
            -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 0.0f, 0.0f, // bottom-left
            -1.0f,  1.0f, -1.0f,  0.0f,  0.0f, -1.0f, 0.0f, 1.0f, // top-left
            // front face
            -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f, 0.0f, // bottom-left
             1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 0.0f, // bottom-right
             1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f, // top-right
             1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f, 1.0f, // top-right
            -1.0f,  1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f, 1.0f, // top-left
            -1.0f, -1.0f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f, 0.0f, // bottom-left
            // left face
            -1.0f,  1.0f,  1.0f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f, // top-right
            -1.0f,  1.0f, -1.0f, -1.0f,  0.0f,  0.0f, 1.0f, 1.0f, // top-left
            -1.0f, -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f, // bottom-left
            -1.0f, -1.0f, -1.0f, -1.0f,  0.0f,  0.0f, 0.0f, 1.0f, // bottom-left
            -1.0f, -1.0f,  1.0f, -1.0f,  0.0f,  0.0f, 0.0f, 0.0f, // bottom-right
            -1.0f,  1.0f,  1.0f, -1.0f,  0.0f,  0.0f, 1.0f, 0.0f, // top-right
            // right face
             1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f, // top-left
             1.0f, -1.0f, -1.0f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f, // bottom-right
             1.0f,  1.0f, -1.0f,  1.0f,  0.0f,  0.0f, 1.0f, 1.0f, // top-right         
             1.0f, -1.0f, -1.0f,  1.0f,  0.0f,  0.0f, 0.0f, 1.0f, // bottom-right
             1.0f,  1.0f,  1.0f,  1.0f,  0.0f,  0.0f, 1.0f, 0.0f, // top-left
             1.0f, -1.0f,  1.0f,  1.0f,  0.0f,  0.0f, 0.0f, 0.0f, // bottom-left     
            // bottom face
            -1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f, // top-right
             1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f, 1.0f, 1.0f, // top-left
             1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f, // bottom-left
             1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f, 1.0f, 0.0f, // bottom-left
            -1.0f, -1.0f,  1.0f,  0.0f, -1.0f,  0.0f, 0.0f, 0.0f, // bottom-right
            -1.0f, -1.0f, -1.0f,  0.0f, -1.0f,  0.0f, 0.0f, 1.0f, // top-right
            // top face
            -1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f, // top-left
             1.0f,  1.0f , 1.0f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f, // bottom-right
             1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f, 1.0f, 1.0f, // top-right     
             1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  0.0f, 1.0f, 0.0f, // bottom-right
            -1.0f,  1.0f, -1.0f,  0.0f,  1.0f,  0.0f, 0.0f, 1.0f, // top-left
            -1.0f,  1.0f,  1.0f,  0.0f,  1.0f,  0.0f, 0.0f, 0.0f  // bottom-left        
        };

                GL.GenVertexArrays(1, out cubeVBO);
                GL.GenBuffers(1, out cubeVBO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, cubeVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, 4 * vertices.Length, vertices, BufferUsageHint.StaticDraw);
                GL.BindVertexArray(cubeVBO);
                GL.EnableVertexAttribArray(0);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (IntPtr)0);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), (IntPtr)(3 * sizeof(float)));
                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (IntPtr)(6 * sizeof(float)));
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);

            }
            GL.BindVertexArray(cubeVBO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);
        }

        public static void DrawCube(Vector3 center, float size)
        {
            if (Viewport.Instance.gL_ControlModern1 == null)
                return;

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
