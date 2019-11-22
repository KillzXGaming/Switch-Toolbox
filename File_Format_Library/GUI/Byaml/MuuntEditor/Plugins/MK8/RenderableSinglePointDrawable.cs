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
    /// Represets a single point that draws a 2D circle and optionally a line (for rotation)
    /// </summary>
    public class RenderableSinglePointDrawable : IDrawableObject
    {
        public bool DrawSelectionRotationLine = true;

        public List<BasePathGroup> PathGroups = new List<BasePathGroup>();

        public Color LineColor = Color.Green;

        public RenderableSinglePointDrawable(List<BasePathGroup> pathGroups, Color color)
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
                    {
                        Render2D.DrawFilledCircle(translate, Color.LightGreen, 30, 40, true);

                    }
                    else if (path.IsHovered)
                        Render2D.DrawFilledCircle(translate, LineColor.Lighten(40), 40, 40, true);
                    else
                        Render2D.DrawFilledCircle(translate, LineColor.Darken(20), 30, 40, true);

                    GL.LineWidth(2f);

                    GL.Enable(EnableCap.DepthTest);
                }
            }
        }

        private Vector3 RotatePoint(Vector3 translate, float X, float Y, float Z, Vector3 rotate)
        {
            Matrix4 rotationX = Matrix4.CreateRotationX(rotate.X);
            Matrix4 rotationY = Matrix4.CreateRotationY(rotate.Z);
            Matrix4 rotationZ = Matrix4.CreateRotationZ(-rotate.Y);

            Matrix4 transMat = Matrix4.CreateTranslation(new Vector3(translate.X, translate.Z, -translate.Y));
            Matrix4 comb = (rotationX * rotationY * rotationZ) * transMat;
            Vector3 pos = new Vector3(X, Y, Z);
            return Vector3.TransformPosition(pos, comb);
        }
    }
}
