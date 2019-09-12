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

        public struct Vertex
        {
            public Vector3 Position;
            public Vector4 Color;
            public Vector2 TexCoord0;
            public Vector2 TexCoord1;
            public Vector2 TexCoord2;

            public static int SizeInBytes = 4 * (3 + 4 + 2 + 2 + 2);
        }

        private void GenerateBuffers(Vector3[] positions, Vector4[] colors, Vector2[] texCoords0)
        {
            GL.GenBuffers(1, out vbo_position);
            UpdateVertexData(positions, colors, texCoords0);
        }

        public void Destroy()
        {
            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
                return;

            GL.DeleteBuffer(vbo_position);
        }

        public Vertex[] Vertices;

        public void Render(Vector3[] positions, Vector4[] colors, Vector2[] texCoords0)
        {
            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
                GenerateBuffers(positions, colors, texCoords0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 0);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 12);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 28);
            GL.VertexAttribPointer(3, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 36);
            GL.VertexAttribPointer(4, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, 44);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        public void UpdateVertexData(Vector3[] positions, Vector4[] colors, Vector2[] texCoords0)
        {
            Vertices = new Vertex[positions.Length];
            for (int v = 0; v < Vertices.Length; v++)
            {
                Vertices[v] = new Vertex();
                Vertices[v].Position = positions[v];
                Vertices[v].Color = colors[v];
                Vertices[v].TexCoord0 = texCoords0[v];
            }

            GL.GenBuffers(1, out vbo_position);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<Vertex>(BufferTarget.ArrayBuffer,
                                   new IntPtr(Vertices.Length * Vertex.SizeInBytes),
                                   Vertices, BufferUsageHint.StaticDraw);
        }
    }
}
