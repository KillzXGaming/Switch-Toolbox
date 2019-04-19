using System;
using System.Collections.Generic;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using Switch_Toolbox.Library;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class RenderableConnectedPaths : AbstractGlDrawable
    {
        public Color LineColor = Color.Green;

        public RenderableConnectedPaths(Color color) {
            LineColor = color;
        }

        //Lap paths use 4 points for each rectangle
        //This also applies to gravity paths
        public bool Use4PointConnection = false;

        private ShaderProgram defaultShaderProgram;

        public List<BasePathGroup> PathGroups = new List<BasePathGroup>();


        public void AddGroup(BasePathGroup group)
        {
            PathGroups.Add(new BasePathGroup()
            {
                PathPoints = group.PathPoints,
            });
        }

        public override void Prepare(GL_ControlLegacy control)
        {
        }

        public override void Prepare(GL_ControlModern control)
        {
            var defaultFrag = new FragmentShader(
               @"#version 330
                vec4 LineColor;

				void main(){
					gl_FragColor = LineColor;
				}");

            var defaultVert = new VertexShader(
              @"#version 330
				in vec4 position;
			
                uniform mat4 mtxCam;
                uniform mat4 mtxMdl;

				void main(){
					gl_Position = mtxCam  * mtxMdl * vec4(position.xyz, 1);
				}");

            defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);
        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            foreach (var group in PathGroups)
            {
                foreach (var path in group.PathPoints)
                {
                    if (!path.RenderablePoint.Visible)
                        continue;

                    GL.LineWidth(2f);
                    foreach (var nextPt in path.NextPoints)
                    {
                        GL.Color3(LineColor);
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex3(path.Translate);
                        GL.Vertex3(PathGroups[nextPt.PathID].PathPoints[nextPt.PtID].Translate);
                        GL.End();
                    }
                    foreach (var prevPt in path.PrevPoints)
                    {
                        GL.Color3(LineColor);
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex3(path.Translate);
                        GL.Vertex3(PathGroups[prevPt.PathID].PathPoints[prevPt.PtID].Translate);
                        GL.End();
                    }
                }
            }
        }

        public override void Draw(GL_ControlModern control, Pass pass)
        {
            control.CurrentShader = defaultShaderProgram;
            defaultShaderProgram.SetVector4("LineColor", ColorUtility.ToVector4(LineColor));

            int groupID = 0;
            foreach (var group in PathGroups)
            {
                int pathID = 0;

                foreach (var path in group.PathPoints)
                {
                    if (!path.RenderablePoint.Visible)
                        continue;

                    if (Use4PointConnection)
                    {
                        for (int i = 0; i < 4; i++)
                        {

                        }
                    }
                    else
                    {
                        GL.LineWidth(2f);
                        foreach (var nextPt in path.NextPoints)
                        {
                            CheckInvalidConnection(nextPt, groupID, pathID);

                            GL.Color3(LineColor);
                            GL.Begin(PrimitiveType.Lines);
                            GL.Vertex3(path.Translate);
                            GL.Vertex3(PathGroups[nextPt.PathID].PathPoints[nextPt.PtID].Translate);
                            GL.End();
                        }

                        foreach (var prevPt in path.PrevPoints)
                        {
                            CheckInvalidConnection(prevPt, groupID, pathID);

                            GL.Color3(LineColor);
                            GL.Begin(PrimitiveType.Lines);
                            GL.Vertex3(path.Translate);
                            GL.Vertex3(PathGroups[prevPt.PathID].PathPoints[prevPt.PtID].Translate);
                            GL.End();
                        }
                    }

                    pathID++;
                }

                groupID++;
            }
        }

        private void CheckInvalidConnection(PointID pathPoint, int groupID, int pathID)
        {
            try
            {
                var point = PathGroups[pathPoint.PathID].PathPoints[pathPoint.PtID];
            }
            catch (Exception ex)
            {
                Switch_Toolbox.Library.Forms.STErrorDialog.Show($"Invalid path points! Group {groupID} Path {pathID}", "Path Creator",
                    $"Group {groupID} \n" +
                    $"Path {pathID} \n" +
                    $"Group ID {pathPoint.PathID} \n" +
                    $"Point ID {pathPoint.PtID} \n" +
                    $"{ex}");
            }
        }
    }

}
