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
    /// </summary>
    public class RenderablePath : IDrawableObject
    {
        public Color LineColor = Color.Green;

        public List<BasePathGroup> PathGroups = new List<BasePathGroup>();

        public RenderablePath(List<BasePathGroup> pathGroups, Color color)
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

                    if (path.IsSelected)
                        Render2D.DrawFilledCircle(translate, Color.LightGreen, 30, 40, true);
                    else if (path.IsHovered)
                        Render2D.DrawFilledCircle(translate, LineColor.Lighten(40), 40, 40, true);
                    else
                        Render2D.DrawFilledCircle(translate, LineColor.Darken(20), 30, 40, true);

                    GL.LineWidth(2f);
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
                    }
                }
            }
            GL.Enable(EnableCap.DepthTest);
        }
    }
}
