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
    public class DrawableBackground : AbstractGlDrawable
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

        public void UpdateVertexData()
        {
            Vector3[] vertices = new Vector3[3];
            vertices[0] = new Vector3(-1f, -1f, 1f);
            vertices[1] = new Vector3(3f, -1f, 1f);
            vertices[2] = new Vector3(-1f, 3f, 1f);

            GL.GenBuffers(1, out vbo_position);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
                                   new IntPtr(vertices.Length * Vector3.SizeInBytes),
                                   vertices, BufferUsageHint.StaticDraw);
        }



        public override void Draw(GL_ControlModern control, Pass pass)
        {
            if (pass == Pass.TRANSPARENT || Runtime.PBR.UseSkybox)
                return;

            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
                UpdateVertexData();

            if (!Runtime.OpenTKInitialized)
                return;

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            control.CurrentShader = defaultShaderProgram;
            control.UpdateModelMatrix(Matrix4.Identity);

            defaultShaderProgram.EnableVertexAttributes();

            Vector3 topColor = ColorUtility.ToVector3(Runtime.backgroundGradientTop);
            Vector3 bottomColor = ColorUtility.ToVector3(Runtime.backgroundGradientBottom);

            defaultShaderProgram.SetVector4("topColor", new Vector4(topColor, 1.0f));
            defaultShaderProgram.SetVector4("bottomColor", new Vector4(bottomColor, 1.0f));
            
            BindBuffer();

            defaultShaderProgram.DisableVertexAttributes();

            GL.UseProgram(0);
            GL.Enable(EnableCap.CullFace);
        }

        private void BindBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            if (!Runtime.OpenTKInitialized)
                return;

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Disable(EnableCap.Texture2D);
            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Color3(Runtime.backgroundGradientTop);
            GL.Vertex3(1, 1, 0.99998);
            GL.Vertex3(-1, 1, 0.99998);
            GL.Color3(Runtime.backgroundGradientBottom);
            GL.Vertex3(1, -1, 0.99998);
            GL.Vertex3(-1, -1, 0.99998);
            GL.End();
            GL.Enable(EnableCap.Texture2D);

            GL.UseProgram(0);
            GL.Enable(EnableCap.CullFace);

            GL.PopMatrix();
        }

       private bool Initialized = false;
        public override void Prepare(GL_ControlModern control)
        {
            if (Initialized)
                return;

            var solidColorFrag = new FragmentShader(
                         @"#version 330
				uniform vec4 bottomColor;
				uniform vec4 topColor;

				in vec2 texCoord;

               out vec4 FragColor;

				void main(){
	                FragColor = mix(bottomColor, topColor, texCoord.y);
				}");
            var solidColorVert = new VertexShader(
              @"#version 330
				layout(location = 0) in vec3 position;

				out vec2 texCoord;

				void main(){
					texCoord.xy = (position.xy + vec2(1.0)) * 0.5;
					gl_Position = vec4(position, 1);
				}");

            defaultShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert, control);
            Initialized = true;
        }

        public override void Prepare(GL_ControlLegacy control)
        {

        }
    }
}
