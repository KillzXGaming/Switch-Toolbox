using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using FirstPlugin.Turbo.CourseMuuntStructs;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin.MuuntEditor
{
    /// <summary>
    /// Represets a path with multiple grouped points that connect to each other
    /// This one in particular includes 2 left and right points that connect together
    /// </summary>
    public class RenderableDoublePointPath : IDrawableObject
    {
        public Color LineColor = Color.Green;

        public List<BasePathGroup> PathGroups = new List<BasePathGroup>();

        public RenderableDoublePointPath(List<BasePathGroup> pathGroups, Color color)
        {
            PathGroups = pathGroups;
            LineColor = color;
        }

        public void Draw(Matrix4 mvp)
        {
            GL.Disable(EnableCap.DepthTest);
            for (int i = 0; i < PathGroups.Count; i++)
            {
                foreach (var path in PathGroups[i].PathPoints)
                {
                    var translate = new Vector3(path.Translate.X, path.Translate.Z, path.Translate.Y);

                    //Draw a line repesenting the size of the path area
                    var point1 = RotatePoint(translate, path.Scale.X / 2, path.Scale.Z / 2, path.Scale.Y / 2, path.Rotate);
                    var point2 = RotatePoint(translate, -(path.Scale.X / 2), -(path.Scale.Z / 2), -(path.Scale.Y / 2), path.Rotate);
//
                    GL.PushMatrix();
                    GL.Translate(translate);
                    GL.Rotate(MathHelper.RadiansToDegrees(path.Rotate.X), 1, 0, 0);
                    GL.Rotate(MathHelper.RadiansToDegrees(path.Rotate.Z), 0, 1, 0);
                    GL.Rotate(MathHelper.RadiansToDegrees(path.Rotate.Y), 0, 0, 1);

                    GL.Color3(LineColor);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex3(point1);
                    GL.Vertex3(point2);
                    GL.End();

                    GL.PopMatrix();

                    //Draw 2 points repesenting the edges of the path


                    /*    if (path.IsSelected)
                            Render2D.DrawFilledCircle(translate, Color.LightGreen, 5, 40, true);
                        else if (path.IsHovered)
                            Render2D.DrawFilledCircle(translate, LineColor.Lighten(40), 6, 40, true);
                        else
                            Render2D.DrawFilledCircle(translate, LineColor.Darken(20), 5, 40, true);
                        */
                    /*     GL.LineWidth(2f);
                         foreach (var nextPt in path.NextPoints)
                         {
                             var nextTranslate = PathGroups[nextPt.PathID].PathPoints[nextPt.PtID].Translate;

                             GL.Color3(LineColor);
                             GL.Begin(PrimitiveType.Lines);
                             GL.Vertex3(translate);
                             GL.Vertex3(nextTranslate.X, nextTranslate.Z, nextTranslate.Y);
                             GL.End();
                         }
                         foreach (var prevPt in path.PrevPoints)
                         {
                             var prevTranslate = PathGroups[prevPt.PathID].PathPoints[prevPt.PtID].Translate;

                             GL.Color3(LineColor);
                             GL.Begin(PrimitiveType.Lines);
                             GL.Vertex3(translate);
                             GL.Vertex3(prevTranslate.X, prevTranslate.Z, prevTranslate.Y);
                             GL.End();
                         }*/
                }
            }
            GL.Enable(EnableCap.DepthTest);
        }

        private void DrawPathPoint(Vector3 center, Vector3 scale, Vector3 rotate, Color color, bool useWireFrame = false)
        {
            GL.Color3(color);

            float sizeX = scale.X;
            float sizeY = scale.Y;
            float sizeZ = scale.Z;


            PrimitiveType primitiveType = PrimitiveType.Quads;
            if (useWireFrame)
            {
                GL.LineWidth(2);
                primitiveType = PrimitiveType.LineLoop;
            }

            GL.Begin(primitiveType);
            GL.Normal3(0.0f, 1.0f, 0.0f);
            GL.Vertex3(center.X + sizeX, center.Y + sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y + sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y + sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y + sizeY, center.Z + sizeZ);
            GL.End();

            GL.Begin(primitiveType);
            GL.Normal3(0.0f, 0.0f, 1.0f);
            GL.Vertex3(center.X + sizeX, center.Y - sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y - sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y - sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y - sizeY, center.Z - sizeZ);
            GL.End();

            GL.Begin(primitiveType);
            GL.Normal3(1.0f, 0.0f, 0.0f);
            GL.Vertex3(center.X + sizeX, center.Y + sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y + sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y - sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y - sizeY, center.Z + sizeZ);
            GL.End();

            GL.Begin(primitiveType);
            GL.Normal3(-1.0f, 0.0f, 0.0f);
            GL.Vertex3(center.X + sizeX, center.Y - sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y - sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y + sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y + sizeY, center.Z - sizeZ);
            GL.End();

            GL.Begin(primitiveType);
            GL.Normal3(0.0f, -1.0f, 0.0f);
            GL.Vertex3(center.X - sizeX, center.Y + sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y + sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y - sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X - sizeX, center.Y - sizeY, center.Z + sizeZ);
            GL.End();

            GL.Begin(primitiveType);
            GL.Normal3(0.0f, 0.0f, -1.0f);
            GL.Vertex3(center.X + sizeX, center.Y + sizeY, center.Z - sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y + sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y - sizeY, center.Z + sizeZ);
            GL.Vertex3(center.X + sizeX, center.Y - sizeY, center.Z - sizeZ);
            GL.End();
        }

        private Vector3 RotatePoint(Vector3 translate, float X, float Y, float Z, Vector3 rotate)
        {
            return new Vector3(X, Y, Z);

            Matrix4 rotationZ = Matrix4.CreateRotationZ(rotate.Y);
            Matrix4 rotationY = Matrix4.CreateRotationY(rotate.Z);
            Matrix4 rotationX = Matrix4.CreateRotationX(rotate.X);

            Matrix4 transMat = Matrix4.CreateTranslation(translate);
            Matrix4 comb = (rotationX * rotationY * rotationZ) * transMat;
            Vector3 pos = new Vector3(X, Y, Z);
            return Vector3.TransformPosition(pos, comb);
        }
    }
}
