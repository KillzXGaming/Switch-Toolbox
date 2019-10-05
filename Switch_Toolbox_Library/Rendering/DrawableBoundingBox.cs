using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Toolbox.Library.Rendering
{
    public class DrawableBoundingBox
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

        public static Vector3[] points = new Vector3[]
     {
                new Vector3(-1,-1, 1),
                new Vector3( 1,-1, 1),
                new Vector3(-1, 1, 1),
                new Vector3( 1, 1, 1),
                new Vector3(-1,-1,-1),
                new Vector3( 1,-1,-1),
                new Vector3(-1, 1,-1),
                new Vector3( 1, 1,-1)
     };

        public static void DrawBoundingBox(Matrix4 mvp, Vector3 Scale, Vector3 Position, System.Drawing.Color color)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref mvp);

            GL.Disable(EnableCap.CullFace);
            GL.UseProgram(0);

            GL.LineWidth(5);
            GL.Color3(color);
            GL.Begin(PrimitiveType.LineStrip);
            GL.Vertex3(Position + points[6] * Scale);
            GL.Vertex3(Position + points[2] * Scale);
            GL.Vertex3(Position + points[3] * Scale);
            GL.Vertex3(Position + points[7] * Scale);
            GL.Vertex3(Position + points[6] * Scale);

            GL.Vertex3(Position + points[4] * Scale);
            GL.Vertex3(Position + points[5] * Scale);
            GL.Vertex3(Position + points[1] * Scale);
            GL.Vertex3(Position + points[0] * Scale);
            GL.Vertex3(Position + points[4] * Scale);
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(color);
            GL.Vertex3(Position + points[2] * Scale);
            GL.Vertex3(Position + points[0] * Scale);
            GL.Vertex3(Position + points[3] * Scale);
            GL.Vertex3(Position + points[1] * Scale);
            GL.Vertex3(Position + points[7] * Scale);
            GL.Vertex3(Position + points[5] * Scale);
            GL.End();

            GL.LineWidth(1);
            GL.Enable(EnableCap.CullFace);
            GL.UseProgram(0);
        }

        public static void DrawBoundingBox(Matrix4 mvp, Vector3 Min, Vector3 Max, Vector3 Position, System.Drawing.Color color)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref mvp);

            GL.Disable(EnableCap.CullFace);
            GL.UseProgram(0);

            var vertices = GetBoundingVertices(Min, Max);

            GL.LineWidth(5);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Color3(color);
            GL.Vertex3(Position + vertices[0]);
            GL.Vertex3(Position + vertices[1]);
            GL.Vertex3(Position + vertices[3]);
            GL.Vertex3(Position + vertices[2]);
            GL.End();

            GL.Begin(PrimitiveType.LineLoop);
            GL.Color3(color);
            GL.Vertex3(Position + vertices[4]);
            GL.Vertex3(Position + vertices[5]);
            GL.Vertex3(Position + vertices[7]);
            GL.Vertex3(Position + vertices[6]);
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(color);
            GL.Vertex3(Position + vertices[0]);
            GL.Vertex3(Position + vertices[4]);
            GL.Vertex3(Position + vertices[1]);
            GL.Vertex3(Position + vertices[5]);
            GL.Vertex3(Position + vertices[3]);
            GL.Vertex3(Position + vertices[7]);
            GL.Vertex3(Position + vertices[2]);
            GL.Vertex3(Position + vertices[6]);
            GL.End();

            GL.LineWidth(1);
            GL.Enable(EnableCap.CullFace);
            GL.UseProgram(0);
        }
    }
}
