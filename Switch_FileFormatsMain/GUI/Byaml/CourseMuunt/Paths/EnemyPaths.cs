using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class RenderablePaths : AbstractGlDrawable
    {
        private static ShaderProgram defaultShaderProgram;

        public List<EnemyPathGroup> PathGroups = new List<EnemyPathGroup>();

        public override void Prepare(GL_ControlLegacy control)
        {
        }

        public override void Prepare(GL_ControlModern control)
        {
            var defaultFrag = new FragmentShader(
      @"#version 330
				void main(){
					gl_FragColor = vec4(0,1,0,1);
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
                    GL.LineWidth(2f);
                    foreach (var nextPt in path.NextPoints)
                    {
                        GL.Color3(Color.Blue);
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex3(path.Translate);
                        GL.Vertex3(PathGroups[nextPt.PathID].PathPoints[nextPt.PtID].Translate);
                        GL.End();
                    }
                    foreach (var prevPt in path.PrevPoints)
                    {
                        GL.Color3(Color.Blue);
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

            foreach (var group in PathGroups)
            {
                foreach (var path in group.PathPoints)
                {
                    GL.LineWidth(2f);
                    foreach (var nextPt in path.NextPoints)
                    {
                        GL.Color3(Color.Blue);
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex3(path.Translate);
                        GL.Vertex3(PathGroups[nextPt.PathID].PathPoints[nextPt.PtID].Translate);
                        GL.End();
                    }
                    foreach (var prevPt in path.PrevPoints)
                    {
                        GL.Color3(Color.Blue);
                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex3(path.Translate);
                        GL.Vertex3(PathGroups[prevPt.PathID].PathPoints[prevPt.PtID].Translate);
                        GL.End();
                    }
                }
            }
        }
    }

    public class EnemyPathGroup : IObject
    {
        public const string N_EnemyPathGroup = "EnemyPathGroup";
        public const string N_UnitIdNum = "UnitIdNum";

        public List<PathPoint> PathPoints = new List<PathPoint>();

        public EnemyPathGroup(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");

            foreach (var point in this["PathPt"])
            {
                PathPoints.Add(new EnemyPathPoint(point));
            }
        }

        [Browsable(false)]
        public Dictionary<string, dynamic> Prop { get; set; } = new Dictionary<string, dynamic>();

        public int EnemyPathGroupId
        {
            get { return this[N_EnemyPathGroup]; }
            set { this[N_EnemyPathGroup] = value; }
        }

        public int UnitIdNum
        {
            get { return this[N_UnitIdNum]; }
            set { this[N_UnitIdNum] = value; }
        }

        public dynamic this[string name]
        {
            get
            {
                if (Prop.ContainsKey(name)) return Prop[name];
                else return null;
            }
            set
            {
                if (Prop.ContainsKey(name)) Prop[name] = value;
                else Prop.Add(name, value);
            }
        }
    }

    public class EnemyPathPoint : PathPoint
    {
        [Browsable(false)]
        public override RenderablePathPoint RenderablePoint
        {
            get
            {
                var point = new RenderablePathPoint(Translate, Rotate, new OpenTK.Vector3(30), this);
                point.CanConnect = true;
                return point;
            }
        }

        [Category("Properties")]
        public int BattleFlag
        {
            get { return this["BattleFlag"]; }
            set { this["BattleFlag"] = value; }
        }

        [Category("Properties")]
        public int Priority
        {
            get { return this["Priority"]; }
            set { this["Priority"] = value; }
        }

        public EnemyPathPoint(dynamic bymlNode)
        {
            if (bymlNode is Dictionary<string, dynamic>) Prop = (Dictionary<string, dynamic>)bymlNode;
            else throw new Exception("Not a dictionary");

            foreach (var point in this["NextPt"])
            {
                NextPoints.Add(new PointID(point));
            }
            foreach (var point in this["PrevPt"])
            {
                PrevPoints.Add(new PointID(point));
            }
        }
    }
}
