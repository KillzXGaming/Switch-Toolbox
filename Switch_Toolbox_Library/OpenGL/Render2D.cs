using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class Render2D
    {
        public static void DrawRectangle(float width, float height, Vector3 translate, Vector3 rotate, Vector3 scale)
        {
            GL.PushMatrix();

            GL.Scale(scale);
            GL.Rotate(rotate.X, new Vector3(1, 0, 0));
            GL.Rotate(rotate.Y, new Vector3(0, 1, 0));
            GL.Rotate(rotate.Z, new Vector3(0, 0, 1));
            GL.Translate(translate);

            DrawRectangle(width, height, Color.White, false);

            GL.PopMatrix();
        }

        public static void DrawFilledCircle(Vector3 Position, Color color, float radius = 300, byte transparency = 255, bool outline = false)
        {
            GL.PushMatrix();
            GL.Translate(Position.X, Position.Y, Position.Z);
            GL.Scale(radius, radius, 1);

            GL.Color4(color.R, color.G, color.B, transparency);
            GL.Begin(PrimitiveType.TriangleFan);
            for (int i = 0; i <= 300; i++)
            {
                double angle = 2 * Math.PI * i / 300;
                double x = Math.Cos(angle);
                double y = Math.Sin(angle);
                GL.Vertex2(x, y);
            }
            GL.End();

            GL.PopMatrix();

            if (outline)
                DrawCircle(Position, color.Darken(20), radius);
        }

        public static void DrawCircle(Vector3 Position, Color color, float radius = 300)
        {
            GL.PushMatrix();
            GL.Translate(Position.X, Position.Y, Position.Z);
            GL.Scale(radius, radius, 1);

            GL.Color4(color);
            GL.Begin(PrimitiveType.LineLoop);
            for (int i = 0; i <= 300; i++)
            {
                double angle = 2 * Math.PI * i / 300;
                double x = Math.Cos(angle);
                double y = Math.Sin(angle);
                GL.Vertex2(x, y);
            }
            GL.End();

            GL.PopMatrix();
        }

        public static void DrawRectangle(float width, float height, Color color, bool wireframe)
        {
            if (wireframe)
            {
                GL.Begin(PrimitiveType.LineLoop);
                GL.LineWidth(1);
                GL.Color4(color);
                GL.Vertex2(-width, -height);
                GL.Vertex2(width, -height);
                GL.Vertex2(width, height);
                GL.Vertex2(-width, height);
                GL.End();

                GL.PopAttrib();
            }
            else
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Color4(color);
                GL.TexCoord2(-1, -1);
                GL.Vertex2(-width, -height);
                GL.TexCoord2(0, -1);
                GL.Vertex2(width, -height);
                GL.TexCoord2(0, 0);
                GL.Vertex2(width, height);
                GL.TexCoord2(-1, 0);
                GL.Vertex2(-width, height);
                GL.End();
            }
        }

        public static void DrawGrid(Color color)
        {
            var size = 40;
            var amount = 300;

            GL.LineWidth(0.001f);
            GL.Color3(color.Darken(20));
            GL.Begin(PrimitiveType.Lines);

            int squareGridCounter = 0;
            for (var i = -amount; i <= amount; i++)
            {
                if (squareGridCounter > 5)
                {
                    squareGridCounter = 0;
                    GL.LineWidth(33f);
                }
                else
                {
                    GL.LineWidth(0.001f);
                }

                GL.Vertex2(new Vector2(-amount * size, i * size));
                GL.Vertex2(new Vector2(amount * size, i * size));
                GL.Vertex2(new Vector2(i * size, -amount * size));
                GL.Vertex2(new Vector2(i * size, amount * size));

                squareGridCounter++;
            }
            GL.End();
            GL.Color3(Color.Transparent);
            GL.PopAttrib();
        }
    }
}
