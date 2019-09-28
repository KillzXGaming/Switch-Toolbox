using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LayoutBXLYT
{
    public class RenderablePane
    {
        int vbo_position;
        int ibo_position;

        public struct Vertex
        {
            public Vector2 Position;
            public Vector4 Color;
            public Vector2 TexCoord0;
            public Vector2 TexCoord1;
            public Vector2 TexCoord2;

            public static int SizeInBytes = 4 * (2 + 4 + 2 + 2 + 2);
        }

        private void GenerateBuffers(Vector2[] positions, Vector4[] colors, Vector2[] texCoords0)
        {
            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out ibo_position);

            UpdateVertexData(positions, colors, texCoords0);
        }

        public void Destroy()
        {
            bool buffersWereInitialized = vbo_position != 0 && ibo_position != 0;
            if (!buffersWereInitialized)
                return;

            GL.DeleteBuffer(vbo_position);
            GL.DeleteBuffer(vbo_position);
        }

        public Vertex[] Vertices;
        ushort[] Indices = new ushort[] { 0, 1, 2, 3 };

        public void Render(BxlytShader shader, Vector2[] positions, Vector4[] colors, Vector2[] texCoords0)
        {
            shader.Enable();

            bool buffersWereInitialized = vbo_position != 0 && ibo_position != 0;
            if (!buffersWereInitialized)
                GenerateBuffers(positions, colors, texCoords0);

            shader.EnableVertexAttributes();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0);
            GL.VertexAttribPointer(shader.GetAttribute("vColor"), 4, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 8);
            GL.VertexAttribPointer(shader.GetAttribute("vTexCoord0"), 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 24);
            GL.VertexAttribPointer(shader.GetAttribute("vTexCoord1"), 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 32);
            GL.VertexAttribPointer(shader.GetAttribute("vTexCoord2"), 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 40);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_position);
            GL.DrawElements(PrimitiveType.Quads, 4, DrawElementsType.UnsignedShort, IntPtr.Zero);
            shader.DisableVertexAttributes();
        }

        public void UpdateVertexData(Vector2[] positions, Vector4[] colors, Vector2[] texCoords0)
        {
            Vertices = new Vertex[positions.Length];
            for (int v = 0; v < Vertices.Length; v++)
            {
                Vertices[v] = new Vertex();
                Vertices[v].Position = positions[v];
                Vertices[v].Color = colors[v];
                Vertices[v].TexCoord0 = texCoords0[v];
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_position);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                (IntPtr)(Indices.Length * sizeof(ushort)),
                Indices,
                BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer,
                                   new IntPtr(Vertices.Length * Vertex.SizeInBytes),
                                   Vertices, BufferUsageHint.StaticDraw);
        }
    }
}
