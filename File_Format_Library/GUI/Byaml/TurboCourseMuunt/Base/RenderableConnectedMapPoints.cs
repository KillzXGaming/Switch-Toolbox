using System;
using System.Collections.Generic;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using Toolbox.Library;
using GL_EditorFramework.EditorDrawables;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class RenderableConnectedMapPoints : AbstractGlDrawable
    {
        public Color LineColor = Color.Green;

        public RenderableConnectedMapPoints(Color color)
        {
            LineColor = color;
        }

        private ShaderProgram defaultShaderProgram;

        public List<RenderablePathPoint> Points = new List<RenderablePathPoint>();

        public void AddRenderable(RenderablePathPoint point) {
            Points.Add(point);
        }

        public override void Prepare(GL_ControlLegacy control)
        {
        }

        public override void Prepare(GL_ControlModern control)
        {
            var defaultFrag = new FragmentShader(
               @"#version 330
                vec4 LineColor;
               out vec4 FragColor;

				void main(){
					FragColor = LineColor;
				}");

            var defaultVert = new VertexShader(
              @"#version 330
				in vec4 position;
			
                uniform mat4 mtxCam;
                uniform mat4 mtxMdl;

				void main(){
					gl_Position = mtxCam  * mtxMdl * vec4(position.xyz, 1);
				}");

            defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert, control);
        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            int p = 0;
            foreach (var point in Points)
            {
                if (!point.Visible)
                    continue;

                if (p < Points.Count)
                {
                    GL.LineWidth(2f);
                    GL.Color3(LineColor);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex3(point.Position);
                    GL.Vertex3(Points[p].Position);
                    GL.End();
                }

                p++;
            }
        }

        public override void Draw(GL_ControlModern control, Pass pass)
        {
            control.CurrentShader = defaultShaderProgram;
            defaultShaderProgram.SetVector4("LineColor", ColorUtility.ToVector4(LineColor));

            int p = 0;
            foreach (var point in Points)
            {
                if (!point.Visible)
                    continue;

                if (p < Points.Count)
                {
                    GL.LineWidth(2f);
                    GL.Color3(LineColor);
                    GL.Begin(PrimitiveType.Lines);
                    GL.Vertex3(point.Position);
                    GL.Vertex3(Points[p].Position);
                    GL.End();
                }

                p++;
            }
        }
    }
}
