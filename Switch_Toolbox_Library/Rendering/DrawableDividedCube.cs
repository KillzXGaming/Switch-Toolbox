using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Toolbox.Library.Rendering
{
    public class DrawableDividedCube
    {
        public static List<Vector3> GetBoundingVertices(Vector3 Min, Vector3 Max)
        {
            var vertices = new List<Vector3>();
            vertices.Add(new Vector3(Min.X, Min.Y, Min.Z));
            vertices.Add(new Vector3(Min.X, Min.Y, Max.Z));
            vertices.Add(new Vector3(Min.X, Max.Y, Min.Z));
            vertices.Add(new Vector3(Min.X, Max.Y, Max.Z));
            vertices.Add(new Vector3(Max.X, Min.Y, Min.Z));
            vertices.Add(new Vector3(Max.X, Min.Y, Max.Z));
            vertices.Add(new Vector3(Max.X, Max.Y, Min.Z));
            vertices.Add(new Vector3(Max.X, Max.Y, Max.Z));
            return vertices;
        }

        public static void Draw(Vector3 Min, Vector3 Max)
        {
            var vertices = GetBoundingVertices(Min, Max);

            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(vertices[0]);
            GL.Vertex3(vertices[1]);
            GL.Vertex3(vertices[3]);
            GL.Vertex3(vertices[2]);
            GL.End();

            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex3(vertices[4]);
            GL.Vertex3(vertices[5]);
            GL.Vertex3(vertices[7]);
            GL.Vertex3(vertices[6]);
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(vertices[0]);
            GL.Vertex3(vertices[4]);
            GL.Vertex3(vertices[1]);
            GL.Vertex3(vertices[5]);
            GL.Vertex3(vertices[3]);
            GL.Vertex3(vertices[7]);
            GL.Vertex3(vertices[2]);
            GL.Vertex3(vertices[6]);
            GL.End();
        }
    }
}
