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
    public class DrawableFloor : AbstractGlDrawable
    {
        public enum Type
        {
            Grid,
            Solid,
            Texture,
        }

        private ShaderProgram gridShaderProgram;

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
            if (!Runtime.displayGrid || pass == Pass.TRANSPARENT || gridShaderProgram == null)
                return;

            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
                UpdateVertexData();

            if (!Runtime.OpenTKInitialized)
                return;

            control.CurrentShader = gridShaderProgram;
            control.UpdateModelMatrix(Matrix4.Identity);

            Matrix4 previewScale = Utils.TransformValues(Vector3.Zero, Vector3.Zero, Runtime.previewScale);
           
            gridShaderProgram.SetMatrix4x4("previewScale", ref previewScale);

            gridShaderProgram.EnableVertexAttributes();
            Draw(gridShaderProgram);
            gridShaderProgram.DisableVertexAttributes();

            GL.UseProgram(0);
          //  GL.Enable(EnableCap.CullFace);
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
            GL.Disable(EnableCap.Texture2D);
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
            GL.Enable(EnableCap.Texture2D);
        }

        public override void Prepare(GL_ControlModern control)
        {
            var solidColorFrag = new FragmentShader(
                         @"#version 330
				uniform vec3 gridColor;
                out vec4 FragColor;

				void main(){
					FragColor = vec4(gridColor, 1);
				}");
            var solidColorVert = new VertexShader(
              @"#version 330
				in vec3 vPosition;

	            uniform mat4 mtxMdl;
				uniform mat4 mtxCam;

				void main(){
					gl_Position = mtxMdl * mtxCam * vec4(vPosition.xyz, 1);
				}");

            gridShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert, control);
        }

        public override void Prepare(GL_ControlLegacy control)
        {

        }
    }
}
