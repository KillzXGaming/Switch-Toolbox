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

namespace Switch_Toolbox.Library.Rendering
{
    public class DrawableFloor : AbstractGlDrawable
    {
        public enum Type
        {
            Grid,
            Solid,
            Texture,
        }

        protected static ShaderProgram solidColorShaderProgram;

        int vbo_position;

        public void Destroy()
        {
            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
                return;

            GL.DeleteBuffer(vbo_position);
        }

        public List<Vector3> FillVertices(int amount, int size)
        {
            var vertices = new List<Vector3>();
            for (var i = -amount; i <= amount; i++)
            {
                vertices.Add(new Vector3(-amount * size, 0f, i * size));
                vertices.Add(new Vector3(amount * size, 0f, i * size));
                vertices.Add(new Vector3(i * size, 0f, -amount * size));
                vertices.Add(new Vector3(i * size, 0f, amount * size));
            }
            return vertices;
        }

        public static int CellAmount;
        public static int CellSize;

        Vector3[] Vertices
        {
            get
            {
                return FillVertices(CellAmount, CellSize).ToArray();
            }
        }

        public void UpdateVertexData()
        {
            CellSize = (int)Runtime.gridSettings.CellSize;
            CellAmount = (int)Runtime.gridSettings.CellAmount;

            Vector3[] vertices = Vertices;

            GL.GenBuffers(1, out vbo_position);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(vertices.Length * Vector3.SizeInBytes),
                                   vertices, BufferUsageHint.StaticDraw);
        }

        public override void Draw(GL_ControlModern control, Pass pass)
        {
            if (pass == Pass.TRANSPARENT)
                return;

            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
                UpdateVertexData();

            if (!Runtime.OpenTKInitialized)
                return;

            GL.Disable(EnableCap.CullFace);

            control.CurrentShader = solidColorShaderProgram;

            Matrix4 previewScale = Utils.TransformValues(Vector3.Zero, Vector3.Zero, Runtime.previewScale);
            Matrix4 camMat = previewScale * control.mtxCam * control.mtxProj;
            Matrix4 invertedCamera = camMat.Inverted();
            Vector3 lightDirection = new Vector3(0f, 0f, -1f);

            solidColorShaderProgram.SetMatrix4x4("mvpMatrix", ref camMat);

            solidColorShaderProgram.EnableVertexAttributes();
            Draw(solidColorShaderProgram);
            solidColorShaderProgram.DisableVertexAttributes();

            GL.UseProgram(0);
            GL.Enable(EnableCap.CullFace);
        }

        private void Attributes(ShaderProgram shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, 12, 0);
        }
        private void Uniforms(ShaderProgram shader)
        {
            shader.SetVector3("gridColor", ColorUtility.ToVector3(Runtime.gridSettings.color));
        }
        private void Draw(ShaderProgram shader)
        {
            Uniforms(shader);
            Attributes(shader);

            GL.DrawArrays(PrimitiveType.Lines, 0, Vertices.Length);
        }


        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            if (pass == Pass.TRANSPARENT)
                return;

            var size = Runtime.gridSettings.CellSize;
            var amount = Runtime.gridSettings.CellAmount;
            var color = Runtime.gridSettings.color;

            GL.UseProgram(0);
          //  GL.MatrixMode(MatrixMode.Modelview);
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
        }

        public override void Prepare(GL_ControlModern control)
        {
            var solidColorFrag = new FragmentShader(
                         @"#version 330
				uniform vec3 gridColor;
				void main(){
					gl_FragColor = vec4(gridColor, 1);
				}");
            var solidColorVert = new VertexShader(
              @"#version 330
				in vec3 vPosition;
				uniform mat4 mvpMatrix;

				void main(){
					gl_Position = mvpMatrix * vec4(vPosition.xyz, 1);
				}");

            solidColorShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert);
        }

        public override void Prepare(GL_ControlLegacy control)
        {

        }
    }
}
