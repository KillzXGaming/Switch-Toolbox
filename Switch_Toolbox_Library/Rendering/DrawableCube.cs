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
using Toolbox.Library;

namespace Toolbox.Library.Rendering
{
    public class DrawableCube : AbstractGlDrawable
    {
        protected static ShaderProgram solidColorShaderProgram;

        int vbo_position;
        int ibo_elements;

        public void Destroy()
        {
            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
                return;

            GL.DeleteBuffer(vbo_position);
        }

            Vector3[] Vertices = new Vector3[]
            {
                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3( 1.0f, -1.0f,  1.0f),
                new Vector3( 1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f)
            };

            int[] Indices = new int[]
            {
                // front face
                0, 1, 2, 2, 3, 0,
                // top face
                3, 2, 6, 6, 7, 3,
                // back face
                7, 6, 5, 5, 4, 7,
                // left face
                4, 0, 3, 3, 7, 4,
                // bottom face
                0, 1, 5, 5, 4, 0,
                // right face
                1, 5, 6, 6, 2, 1,
            };

            Vector3[] Normals = new Vector3[]
            {
                new Vector3(-1.0f, -1.0f,  1.0f),
                new Vector3( 1.0f, -1.0f,  1.0f),
                new Vector3( 1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f,  1.0f,  1.0f),
                new Vector3(-1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f, -1.0f, -1.0f),
                new Vector3( 1.0f,  1.0f, -1.0f),
                new Vector3(-1.0f,  1.0f, -1.0f),
            };

            Vector3[] Colors = new Vector3[]
            {
                ColorUtility.ToVector3(Color.DarkRed),
                ColorUtility.ToVector3(Color.DarkRed),
                ColorUtility.ToVector3(Color.Gold),
                ColorUtility.ToVector3(Color.Gold),
                ColorUtility.ToVector3(Color.DarkRed),
                ColorUtility.ToVector3(Color.DarkRed),
                ColorUtility.ToVector3(Color.Gold),
                ColorUtility.ToVector3(Color.Gold),
            };

            Vector2[] Texcoords = new Vector2[]
            {
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 0),
            };

        public int ElementSize = 0;

        readonly short[] CubeElements = new short[]
        {
        0, 1, 2, 2, 3, 0, // front face
        3, 2, 6, 6, 7, 3, // top face
        7, 6, 5, 5, 4, 7, // back face
        4, 0, 3, 3, 7, 4, // left face
        0, 1, 5, 5, 4, 0, // bottom face
        1, 5, 6, 6, 2, 1, // right face
        };

        public void UpdateVertexData()
        {
            int size;

            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out ibo_elements);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * Vector3.SizeInBytes), Vertices,
                          BufferUsageHint.StaticDraw);  

            GL.BindBuffer(BufferTarget.ArrayBuffer, ibo_elements);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Length * sizeof(int)), Indices,
                    BufferUsageHint.StaticDraw);

            ElementSize = Indices.Length;
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

            GL.UseProgram(0);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Disable(EnableCap.CullFace);

            control.CurrentShader = solidColorShaderProgram;

            Matrix4 previewScale = Utils.TransformValues(Vector3.Zero, Vector3.Zero, Runtime.previewScale);
            Matrix4 camMat = control.ModelMatrix * control.ProjectionMatrix;
            Matrix4 invertedCamera = camMat.Inverted();
            Vector3 lightDirection = new Vector3(0f, 0f, -1f);

            solidColorShaderProgram.SetMatrix4x4("mvpMatrix", ref camMat);

            solidColorShaderProgram.EnableVertexAttributes();

            BindBuffer(solidColorShaderProgram);

            solidColorShaderProgram.DisableVertexAttributes();

            GL.UseProgram(0);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.DepthMask(true);
        }

        private void BindBuffer(ShaderProgram shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, 12, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {

        }

        public override void Prepare(GL_ControlModern control)
        {
            var solidColorFrag = new FragmentShader(
                         @"#version 330

				uniform vec3 color;

                out vec4 FragColor;

				void main(){
	                FragColor = vec4(color, 1);
				}");
            var solidColorVert = new VertexShader(
              @"#version 330
				in vec3 vPosition;
				uniform mat4 mvpMatrix;

				void main(){
				    gl_Position = mvpMatrix * vec4(vPosition.xyz, 1.0);
				}");

            solidColorShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert, control);
        }

        public override void Prepare(GL_ControlLegacy control)
        {

        }
    }
}
