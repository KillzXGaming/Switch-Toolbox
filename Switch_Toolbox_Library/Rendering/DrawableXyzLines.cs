using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Toolbox.Library.Rendering
{
    public class DrawableXyzLines : AbstractGlDrawable
    {
        ShaderProgram defaultShaderProgram;

        int vbo_position;

        public void Destroy()
        {
            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
                return;

            GL.DeleteBuffer(vbo_position);
        }

        public static int CellAmount;
        public static int CellSize;

        public struct DisplayVertex
        {
            public Vector3 Position;
            public Vector3 Color;

            public static int Size = 4 * (3 + 3);
        }

        public static float LineLength = 5;

        public DisplayVertex[] Vertices = new DisplayVertex[]
        {
            new DisplayVertex()
            {
                Position = new Vector3(0,0,0),
                Color = new Vector3(0,1,0),
            },
            new DisplayVertex()
            {
                Position = new Vector3(0,LineLength,0),
                Color = new Vector3(0,1,0),
            },
            new DisplayVertex()
            {
                Position = new Vector3(0,0,0),
                Color = new Vector3(1,0,0),
            },
            new DisplayVertex()
            {
                Position = new Vector3(LineLength,0,0),
                Color = new Vector3(1,0,0),
            },
            new DisplayVertex()
            {
                Position = new Vector3(0,0,0),
                Color = new Vector3(0,0,1),
            },
            new DisplayVertex()
            {
                Position = new Vector3(0,0,LineLength),
                Color = new Vector3(0,0,1),
            },
        };

        public void UpdateVertexData()
        {
            CellSize = (int)Runtime.gridSettings.CellSize;
            CellAmount = (int)Runtime.gridSettings.CellAmount;

            GL.GenBuffers(1, out vbo_position);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<DisplayVertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * DisplayVertex.Size), Vertices, BufferUsageHint.StaticDraw);
        }

        public override void Draw(GL_ControlModern control, Pass pass)
        {
            if (!Runtime.displayAxisLines || !Runtime.OpenTKInitialized || pass == Pass.TRANSPARENT)
                return;

            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
                UpdateVertexData();

            GL.UseProgram(0);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            control.CurrentShader = defaultShaderProgram;
            control.UpdateModelMatrix(Matrix4.Identity);

            Matrix4 previewScale = Utils.TransformValues(Vector3.Zero, Vector3.Zero, Runtime.previewScale);

            defaultShaderProgram.EnableVertexAttributes();
            Draw(defaultShaderProgram);
            defaultShaderProgram.DisableVertexAttributes();

            GL.UseProgram(0);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }

        private void Attributes(ShaderProgram shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 0);
            GL.VertexAttribPointer(shader.GetAttribute("vColor"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 12);
        }
        private void Uniforms(ShaderProgram shader)
        {
        }
        private void Draw(ShaderProgram shader)
        {
            Uniforms(shader);
            Attributes(shader);

            GL.DrawArrays(PrimitiveType.Lines, 0, Vertices.Length);
        }


        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            if (!Runtime.OpenTKInitialized || pass == Pass.TRANSPARENT)
                return;

            var size = Runtime.gridSettings.CellSize;
            var amount = Runtime.gridSettings.CellAmount;
            var color = Runtime.gridSettings.color;

            GL.UseProgram(0);
            GL.Disable(EnableCap.Texture2D);
            GL.PushAttrib(AttribMask.AllAttribBits);

            var trans = Matrix4.Identity;
            //     GL.LoadMatrix(ref trans);

            GL.LineWidth(1f);
            GL.Color3(color);
            GL.Begin(PrimitiveType.Lines);
            for (var i = -amount; i <= amount; i++)
            {
                GL.Vertex3(new Vector3(-amount * size, 0f, i * size));
                GL.Vertex3(new Vector3(amount * size, 0f, i * size));
                GL.Vertex3(new Vector3(i * size, 0f, -amount * size));
                GL.Vertex3(new Vector3(i * size, 0f, amount * size));
            }
            GL.End();
            GL.Color3(Color.Transparent);
            GL.PopAttrib();
            GL.Enable(EnableCap.Texture2D);
        }

        private bool Initialized = false;
        public override void Prepare(GL_ControlModern control)
        {
            if (Initialized)
                return;

            var solidColorFrag = new FragmentShader(
                         @"#version 330
                in vec3 color;
				out vec4 FragColor;

				void main(){
					FragColor = vec4(color, 0.2);
				}");
            var solidColorVert = new VertexShader(
              @"#version 330
				in vec3 vPosition;
				in vec3 vColor;

				out vec3 color;
 
                uniform mat4 mtxCam;
                uniform mat4 mtxMdl;

				void main(){
                    color = vColor;
					gl_Position = mtxCam  * mtxMdl * vec4(vPosition.xyz, 1);
				}");

            defaultShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert, control);
            Initialized = true;
        }

        public override void Prepare(GL_ControlLegacy control)
        {

        }
    }
}
