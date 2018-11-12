using GL_Core;
using GL_Core.Interfaces;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Testing
{
	public class MarioTposeTest : AbstractGlDrawable
	{
		int testVao;
		int marioTestVao;
		ShaderProgram defaultShaderProgram;
		FragmentShader defaultFrag;
		VertexShader defaultVert, pickingVert;
		ShaderProgram testShaderProgram;

		uint pickedIndex = 0;
		uint selectedIndex = 0;

		public MarioTposeTest()
		{

		}

		public override void Draw(GL_ControlModern control)
		{
			control.CurrentShader = testShaderProgram;
			control.ResetModelMatrix();
			GL.BindVertexArray(testVao);
			GL.DrawArrays(PrimitiveType.Quads, 0, 4);

			GL.BindVertexArray(marioTestVao);

			control.CurrentShader = defaultShaderProgram;
			defaultShaderProgram.SetVertexShader(defaultVert);
			control.ApplyModelTransform(Matrix4.CreateTranslation(-8, 0, 0));

			for (int i = 0; i < 5; i++)
			{
				if (i + 1 == selectedIndex)
					GL.Uniform4(defaultShaderProgram["highlightColor"], 1f, 1f, 0.2f, 0.5f);
				else if (i + 1 == pickedIndex)
					GL.Uniform4(defaultShaderProgram["highlightColor"], 1f, 1f, 0.5f, 0.2f);
				else
					GL.Uniform4(defaultShaderProgram["highlightColor"], 0f, 0f, 0f, 0f);
				GL.DrawArrays(PrimitiveType.Quads, 0, 11 * 4);

				control.ApplyModelTransform(Matrix4.CreateTranslation(4, 0, 0));
			}
		}

		public override void Draw(GL_ControlLegacy control)
		{
			control.ResetModelMatrix();
			GL.Begin(PrimitiveType.Quads);
			GL.Color3(1f, 0f, 0f);
			GL.Vertex3(-10f, -0.25f, 5f);

			GL.Color3(0f, 1f, 0f);
			GL.Vertex3(10f, -0.25f, 5f);

			GL.Color3(1f, 1f, 0f);
			GL.Vertex3(10f, -0.25f, -5f);

			GL.Color3(0f, 0f, 1f);
			GL.Vertex3(-10f, -0.25f, -5f);
			GL.End();

			control.ApplyModelTransform(Matrix4.CreateTranslation(-8, 0, 0));
			
			for (int i = 0; i < 5; i++)
			{
				GL.Begin(PrimitiveType.Quads);
				if(i+1==pickedIndex)
					GL.Color3(0f, 1f, 0f);
				else
					GL.Color3(1f, 0f, 0f);
				GL.Vertex3(-0.5f, 2f, 0f);
				GL.Vertex3(0.5f, 2f, 0f);
				GL.Vertex3(0.5f, 1.25f, 0f);
				GL.Vertex3(-0.5f, 1.25f, 0f);

				GL.Vertex3(-1.5f, 2.5f, 0f);
				GL.Vertex3(1.5f, 2.5f, 0f);
				GL.Vertex3(1.5f, 2f, 0f);
				GL.Vertex3(-1.5f, 2f, 0f);

				//trowsers
				if (i + 1 == selectedIndex)
					GL.Color3(1f, 1f, 1f);
				else
					GL.Color3(0, 0.2f, 1f);
				GL.Vertex3(-0.5f, 1.25f, 0f);
				GL.Vertex3(0.5f, 1.25f, 0f);
				GL.Vertex3(0.5f, 0.75f, 0f);
				GL.Vertex3(-0.5f, 0.75f, 0f);

				GL.Vertex3(-0.5f, 0.75f, 0f);
				GL.Vertex3(-0.125f, 0.75f, 0f);
				GL.Vertex3(-0.125f, -0.25f, 0f);
				GL.Vertex3(-0.5f, -0.25f, 0f);

				GL.Vertex3(0.125f, 0.75f, 0f);
				GL.Vertex3(0.5f, 0.75f, 0f);
				GL.Vertex3(0.5f, -0.25f, 0f);
				GL.Vertex3(0.125f, -0.25f, 0f);

				//stripes
				GL.Vertex3(-0.375f, 2.5f, -0.02f);
				GL.Vertex3(-0.125f, 2.5f, -0.02f);
				GL.Vertex3(-0.125f, 1.25f, -0.02f);
				GL.Vertex3(-0.375f, 1.25f, -0.02f);

				GL.Vertex3(0.125f, 2.5f, -0.02f);
				GL.Vertex3(0.375f, 2.5f, -0.02f);
				GL.Vertex3(0.375f, 1.25f, -0.02f);
				GL.Vertex3(0.125f, 1.25f, -0.02f);

				GL.Vertex3(-0.375f, 2.5f, 0.02f);
				GL.Vertex3(-0.125f, 2.5f, 0.02f);
				GL.Vertex3(-0.125f, 1.25f, 0.02f);
				GL.Vertex3(-0.375f, 1.25f, 0.02f);

				GL.Vertex3(0.125f, 2.5f, 0.02f);
				GL.Vertex3(0.375f, 2.5f, 0.02f);
				GL.Vertex3(0.375f, 1.25f, 0.02f);
				GL.Vertex3(0.125f, 1.25f, 0.02f);

				//knobs
				GL.Color3(1f, 1f, 0f);
				GL.Vertex3(-0.375f, 2.25f, 0.04f);
				GL.Vertex3(-0.125f, 2.25f, 0.04f);
				GL.Vertex3(-0.125f, 2f, 0.04f);
				GL.Vertex3(-0.375f, 2f, 0.04f);

				GL.Vertex3(0.125f, 2.25f, 0.04f);
				GL.Vertex3(0.375f, 2.25f, 0.04f);
				GL.Vertex3(0.375f, 2f, 0.04f);
				GL.Vertex3(0.125f, 2f, 0.04f);
				GL.End();

				control.ApplyModelTransform(Matrix4.CreateTranslation(4, 0, 0));
			}
		}

		public override void Prepare(GL_ControlModern control)
		{
			defaultFrag = new FragmentShader(
			  @"#version 330
				in vec4 fragColor;
				void main(){
					gl_FragColor = fragColor;
				}");
			defaultVert = new VertexShader(
			  @"#version 330
				layout(location = 0) in vec4 position;
				layout(location = 1) in vec4 color;
				uniform mat4 mtxMdl;
				uniform mat4 mtxCam;
				uniform vec4 highlightColor;
				out vec4 fragColor;
				void main(){
					gl_Position = mtxCam*mtxMdl*position;
					fragColor = color+highlightColor.xyz*highlightColor.w;
				}");
			pickingVert = new VertexShader(
			  @"#version 330
				layout(location = 0) in vec4 position;
				uniform vec4 color;
				uniform mat4 mtxMdl;
				uniform mat4 mtxCam;
				out vec4 fragColor;
				void main(){
					gl_Position = mtxCam*mtxMdl*position;
					fragColor = color;
				}");
			defaultShaderProgram = new ShaderProgram(
				defaultFrag,defaultVert
			  );

			testShaderProgram = new ShaderProgram(
				new FragmentShader(
			  @"#version 330
				in vec4 vertPosition;
				in vec4 fragColor;
				void main(){
					float v = round(abs(mod(vertPosition.x,2.0)-1.0)+
							  abs(mod(vertPosition.z,2.0)-1.0));
					gl_FragColor = fragColor*v;
				}"),
				new VertexShader(
			  @"#version 330
				layout(location = 0) in vec4 position;
				layout(location = 1) in vec4 color;
				uniform mat4 mtxMdl;
				uniform mat4 mtxCam;
				out vec4 vertPosition;
				out vec4 fragColor;
				void main(){
					gl_Position = mtxMdl*mtxCam*position;
					vertPosition = position;
					fragColor = color;
				}"));

			control.CurrentShader = defaultShaderProgram;

			int buffer;

			GL.BindVertexArray(testVao = GL.GenVertexArray());

			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer = GL.GenBuffer());
			float[] data = new float[] {
				-10f, -0.25f, 5f,   1f, 0f, 0f, 1f,
				10f, -0.25f, 5f,    0f, 1f, 0f, 1f,
				10f, -0.25f, -5f,   1f, 1f, 0f, 1f,
				-10f, -0.25f, -5f,  0f, 0f, 1f, 1f,
			};
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 7, 0);
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, sizeof(float) * 7, sizeof(float) * 3);

			GL.BindVertexArray(marioTestVao = GL.GenVertexArray());
			using (OldGlEmulator GL = new OldGlEmulator())
			{
				//body
				GL.Color3(1f, 0f, 0f);
				GL.Vertex3(-0.5f, 2f, 0f);
				GL.Vertex3(0.5f, 2f, 0f);
				GL.Vertex3(0.5f, 1.25f, 0f);
				GL.Vertex3(-0.5f, 1.25f, 0f);

				GL.Vertex3(-1.5f, 2.5f, 0f);
				GL.Vertex3(1.5f, 2.5f, 0f);
				GL.Vertex3(1.5f, 2f, 0f);
				GL.Vertex3(-1.5f, 2f, 0f);

				//trowsers
				GL.Color3(0, 0.2f, 1f);
				GL.Vertex3(-0.5f, 1.25f, 0f);
				GL.Vertex3(0.5f, 1.25f, 0f);
				GL.Vertex3(0.5f, 0.75f, 0f);
				GL.Vertex3(-0.5f, 0.75f, 0f);

				GL.Vertex3(-0.5f, 0.75f, 0f);
				GL.Vertex3(-0.125f, 0.75f, 0f);
				GL.Vertex3(-0.125f, -0.25f, 0f);
				GL.Vertex3(-0.5f, -0.25f, 0f);

				GL.Vertex3(0.125f, 0.75f, 0f);
				GL.Vertex3(0.5f, 0.75f, 0f);
				GL.Vertex3(0.5f, -0.25f, 0f);
				GL.Vertex3(0.125f, -0.25f, 0f);

				//stripes
				GL.Vertex3(-0.375f, 2.5f, -0.02f);
				GL.Vertex3(-0.125f, 2.5f, -0.02f);
				GL.Vertex3(-0.125f, 1.25f, -0.02f);
				GL.Vertex3(-0.375f, 1.25f, -0.02f);

				GL.Vertex3(0.125f, 2.5f, -0.02f);
				GL.Vertex3(0.375f, 2.5f, -0.02f);
				GL.Vertex3(0.375f, 1.25f, -0.02f);
				GL.Vertex3(0.125f, 1.25f, -0.02f);

				GL.Vertex3(-0.375f, 2.5f, 0.02f);
				GL.Vertex3(-0.125f, 2.5f, 0.02f);
				GL.Vertex3(-0.125f, 1.25f, 0.02f);
				GL.Vertex3(-0.375f, 1.25f, 0.02f);

				GL.Vertex3(0.125f, 2.5f, 0.02f);
				GL.Vertex3(0.375f, 2.5f, 0.02f);
				GL.Vertex3(0.375f, 1.25f, 0.02f);
				GL.Vertex3(0.125f, 1.25f, 0.02f);

				//knobs
				GL.Color3(1f, 1f, 0f);
				GL.Vertex3(-0.375f, 2.25f, 0.04f);
				GL.Vertex3(-0.125f, 2.25f, 0.04f);
				GL.Vertex3(-0.125f, 2f, 0.04f);
				GL.Vertex3(-0.375f, 2f, 0.04f);

				GL.Vertex3(0.125f, 2.25f, 0.04f);
				GL.Vertex3(0.375f, 2.25f, 0.04f);
				GL.Vertex3(0.375f, 2f, 0.04f);
				GL.Vertex3(0.125f, 2f, 0.04f);

				GL.WriteToBuffer(buffer = OpenTK.Graphics.OpenGL.GL.GenBuffer());
			}
		}

		public override void Prepare(GL_ControlLegacy control)
		{

		}

		public override bool MouseClick(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				selectedIndex = pickedIndex;
				return true;
			}
			return false;
		}

		public override void DrawPicking(GL_ControlModern control)
		{
			defaultShaderProgram.SetVertexShader(pickingVert);
			control.ResetModelMatrix();
			GL.Uniform4(defaultShaderProgram["color"], 0f, 0f, 0f, 0f);
			GL.BindVertexArray(testVao);
			GL.DrawArrays(PrimitiveType.Quads, 0, 4);

			GL.BindVertexArray(marioTestVao);

			control.ApplyModelTransform(Matrix4.CreateTranslation(-8, 0, 0));
			
			for (int i = 0; i < 5; i++)
			{
				GL.Uniform4(defaultShaderProgram["color"], control.nextPickingColor());
				GL.DrawArrays(PrimitiveType.Quads, 0, 11 * 4);

				control.ApplyModelTransform(Matrix4.CreateTranslation(4, 0, 0));
			}
		}

		public override void DrawPicking(GL_ControlLegacy control)
		{
			control.ResetModelMatrix();
			GL.Begin(PrimitiveType.Quads);
			GL.Color4(0,0,0,0);
			GL.Vertex3(-10f, -0.25f, 5f);
			
			GL.Vertex3(10f, -0.25f, 5f);
			
			GL.Vertex3(10f, -0.25f, -5f);
			
			GL.Vertex3(-10f, -0.25f, -5f);
			GL.End();

			control.ApplyModelTransform(Matrix4.CreateTranslation(-8, 0, 0));

			for (int i = 0; i < 5; i++)
			{
				GL.Begin(PrimitiveType.Quads);
				GL.Color4(control.nextPickingColor());
				GL.Vertex3(-0.5f, 2f, 0f);
				GL.Vertex3(0.5f, 2f, 0f);
				GL.Vertex3(0.5f, 1.25f, 0f);
				GL.Vertex3(-0.5f, 1.25f, 0f);

				GL.Vertex3(-1.5f, 2.5f, 0f);
				GL.Vertex3(1.5f, 2.5f, 0f);
				GL.Vertex3(1.5f, 2f, 0f);
				GL.Vertex3(-1.5f, 2f, 0f);

				//trowsers
				GL.Vertex3(-0.5f, 1.25f, 0f);
				GL.Vertex3(0.5f, 1.25f, 0f);
				GL.Vertex3(0.5f, 0.75f, 0f);
				GL.Vertex3(-0.5f, 0.75f, 0f);

				GL.Vertex3(-0.5f, 0.75f, 0f);
				GL.Vertex3(-0.125f, 0.75f, 0f);
				GL.Vertex3(-0.125f, -0.25f, 0f);
				GL.Vertex3(-0.5f, -0.25f, 0f);

				GL.Vertex3(0.125f, 0.75f, 0f);
				GL.Vertex3(0.5f, 0.75f, 0f);
				GL.Vertex3(0.5f, -0.25f, 0f);
				GL.Vertex3(0.125f, -0.25f, 0f);

				//stripes
				GL.Vertex3(-0.375f, 2.5f, -0.02f);
				GL.Vertex3(-0.125f, 2.5f, -0.02f);
				GL.Vertex3(-0.125f, 1.25f, -0.02f);
				GL.Vertex3(-0.375f, 1.25f, -0.02f);

				GL.Vertex3(0.125f, 2.5f, -0.02f);
				GL.Vertex3(0.375f, 2.5f, -0.02f);
				GL.Vertex3(0.375f, 1.25f, -0.02f);
				GL.Vertex3(0.125f, 1.25f, -0.02f);

				GL.Vertex3(-0.375f, 2.5f, 0.02f);
				GL.Vertex3(-0.125f, 2.5f, 0.02f);
				GL.Vertex3(-0.125f, 1.25f, 0.02f);
				GL.Vertex3(-0.375f, 1.25f, 0.02f);

				GL.Vertex3(0.125f, 2.5f, 0.02f);
				GL.Vertex3(0.375f, 2.5f, 0.02f);
				GL.Vertex3(0.375f, 1.25f, 0.02f);
				GL.Vertex3(0.125f, 1.25f, 0.02f);

				//knobs
				GL.Vertex3(-0.375f, 2.25f, 0.04f);
				GL.Vertex3(-0.125f, 2.25f, 0.04f);
				GL.Vertex3(-0.125f, 2f, 0.04f);
				GL.Vertex3(-0.375f, 2f, 0.04f);

				GL.Vertex3(0.125f, 2.25f, 0.04f);
				GL.Vertex3(0.375f, 2.25f, 0.04f);
				GL.Vertex3(0.375f, 2f, 0.04f);
				GL.Vertex3(0.125f, 2f, 0.04f);
				GL.End();

				control.ApplyModelTransform(Matrix4.CreateTranslation(4, 0, 0));
			}
		}

		public override bool Picked(uint index)
		{
			pickedIndex = index;
			Console.WriteLine(index);
			return true;
		}

		public override uint GetPickableSpan(){
			return 5;
		}
	}
}
