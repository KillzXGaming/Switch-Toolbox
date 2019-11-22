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
                    //Draw a line repesenting the size of the path area
                    var point1 = Matrix2DHelper.RotatePoint(path.Translate, path.Scale.X / 2, path.Scale.Z / 2, path.Scale.Y / 2, path.Rotate);
                    var point2 = Matrix2DHelper.RotatePoint(path.Translate, -(path.Scale.X / 2), -(path.Scale.Z / 2), -(path.Scale.Y / 2), path.Rotate);
                    
                    Color pathColor = LineColor;
                    if (path.IsSelected)
                        pathColor = Color.LightGreen;
                    else if (path.IsHovered)
                        pathColor = LineColor.Lighten(70);

                    GL.LineWidth(3f);
                    GL.Color3(pathColor);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex3(point1);
                    GL.Vertex3(point2);
                    GL.End();


                    //Draw 2 points repesenting the edges of the path

                    GL.LineWidth(2f);

                    foreach (var nextPt in path.NextPoints)
                    {
                        var nextPoint = PathGroups[nextPt.PathID].PathPoints[nextPt.PtID];

                        var nextPoint1 = Matrix2DHelper.RotatePoint(nextPoint.Translate, (nextPoint.Scale.X / 2), nextPoint.Scale.Z / 2, -nextPoint.Scale.Y / 2, nextPoint.Rotate);
                        var nextPoint2 = Matrix2DHelper.RotatePoint(nextPoint.Translate, -(nextPoint.Scale.X / 2), -(nextPoint.Scale.Z / 2), (nextPoint.Scale.Y / 2), nextPoint.Rotate);

                   /*     GL.Color3(Color.Green);
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex3(point1);
                        GL.Vertex3(nextPoint1);
                        GL.End();

                        GL.Color3(Color.Green);
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex3(point2);
                        GL.Vertex3(nextPoint2);
                        GL.End();*/
                    }

                    foreach (var prevPt in path.PrevPoints)
                    {
                        var prevPoint = PathGroups[prevPt.PathID].PathPoints[prevPt.PtID];

                        var prevPoint1 = Matrix2DHelper.RotatePoint(prevPoint.Translate, (prevPoint.Scale.X / 2), prevPoint.Scale.Z / 2, prevPoint.Scale.Y / 2, prevPoint.Rotate);
                        var prevPoint2 = Matrix2DHelper.RotatePoint(prevPoint.Translate, -(prevPoint.Scale.X / 2), -(prevPoint.Scale.Z / 2), -(prevPoint.Scale.Y / 2), prevPoint.Rotate);

                        GL.Color3(Color.Green);
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex3(point1);
                        GL.Vertex3(prevPoint1);
                        GL.End();

                        GL.Color3(Color.Green);
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex3(point2);
                        GL.Vertex3(prevPoint2);
                        GL.End();
                    }

                    GL.LineWidth(1f);
                }
            }
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
