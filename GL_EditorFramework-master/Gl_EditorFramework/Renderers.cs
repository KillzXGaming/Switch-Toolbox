using GL_EditorFramework;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGl_EditorFramework
{
	public static class Renderers
	{
		private static void face(ref float[][] points, ref List<float> data, int p1, int p2, int p4, int p3)
		{
			data.AddRange(new float[] {
				points[p1][0], points[p1][1], points[p1][2],
				points[p2][0], points[p2][1], points[p2][2],
				points[p3][0], points[p3][1], points[p3][2],
				points[p4][0], points[p4][1], points[p4][2]
			});
		}

		private static void lineFace(ref float[][] points, ref List<float> data, int p1, int p2, int p4, int p3)
		{
			data.AddRange(new float[] {
				points[p1][0], points[p1][1], points[p1][2],
				points[p2][0], points[p2][1], points[p2][2],
				points[p2][0], points[p2][1], points[p2][2],
				points[p3][0], points[p3][1], points[p3][2],
				points[p3][0], points[p3][1], points[p3][2],
				points[p4][0], points[p4][1], points[p4][2],
				points[p4][0], points[p4][1], points[p4][2],
				points[p1][0], points[p1][1], points[p1][2]
			});
		}

		private static void line(ref float[][] points, ref List<float> data, int p1, int p2)
		{
			data.AddRange(new float[] {
				points[p1][0], points[p1][1], points[p1][2],
				points[p2][0], points[p2][1], points[p2][2]
			});
		}

		private static void faceInv(ref float[][] points, ref List<float> data, int p2, int p1, int p3, int p4)
		{
			data.AddRange(new float[] {
				points[p1][0], points[p1][1], points[p1][2],
				points[p2][0], points[p2][1], points[p2][2],
				points[p3][0], points[p3][1], points[p3][2],
				points[p4][0], points[p4][1], points[p4][2]
			});
		}

		public static class ColorBlockRenderer {
			private static bool Initialized = false;
		
			private static ShaderProgram defaultShaderProgram;
			private static ShaderProgram solidColorShaderProgram;

			private static int linesVao;
			private static int blockVao;

			private static float[][] points = new float[][]
			{
				new float[]{-1,-1, 1},
				new float[]{ 1,-1, 1},
				new float[]{-1, 1, 1},
				new float[]{ 1, 1, 1},
				new float[]{-1,-1,-1},
				new float[]{ 1,-1,-1},
				new float[]{-1, 1,-1},
				new float[]{ 1, 1,-1}
			};


			public static void Initialize()
			{
				if (!Initialized)
				{
					var defaultFrag = new FragmentShader(
						@"#version 330
						uniform sampler2D tex;
						in vec4 fragColor;
						in vec3 fragPosition;
						in vec2 uv;
				
						void main(){
							gl_FragColor = fragColor*((fragPosition.y+2)/3)*texture(tex, uv);
						}");
					var solidColorFrag = new FragmentShader(
						@"#version 330
						uniform vec4 color;
						void main(){
							gl_FragColor = color;
						}");
					var defaultVert = new VertexShader(
						@"#version 330
						layout(location = 0) in vec4 position;
						uniform vec4 color;
						uniform mat4 mtxMdl;
						uniform mat4 mtxCam;
						out vec4 fragColor;
						out vec3 fragPosition;
						out vec2 uv;

						vec2 map(vec2 value, vec2 min1, vec2 max1, vec2 min2, vec2 max2) {
							return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
						}

						void main(){
							fragPosition = position.xyz;
							uv = map(fragPosition.xz,vec2(-1.0625,-1.0625),vec2(1.0625,1.0625), vec2(0.5,0.5), vec2(0.75,1.0));
							gl_Position = mtxCam*mtxMdl*position;
							fragColor = color;
						}");
					var solidColorVert = new VertexShader(
						@"#version 330
						layout(location = 0) in vec4 position;
						uniform mat4 mtxMdl;
						uniform mat4 mtxCam;
						void main(){
							gl_Position = mtxCam*mtxMdl*position;
						}");
					defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);
					solidColorShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert);

					int buffer;

					#region block
					GL.BindVertexArray(blockVao = GL.GenVertexArray());

					GL.BindBuffer(BufferTarget.ArrayBuffer, buffer = GL.GenBuffer());
					List<float> list = new List<float>();

					face(ref points, ref list, 0, 1, 2, 3);
					faceInv(ref points, ref list, 4, 5, 6, 7);
					faceInv(ref points, ref list, 0, 1, 4, 5);
					face(ref points, ref list, 2, 3, 6, 7);
					face(ref points, ref list, 0, 2, 4, 6);
					faceInv(ref points, ref list, 1, 3, 5, 7);

					float[] data = list.ToArray();
					GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);

					GL.EnableVertexAttribArray(0);
					GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
					#endregion

					#region lines
					GL.BindVertexArray(linesVao = GL.GenVertexArray());

					GL.BindBuffer(BufferTarget.ArrayBuffer, buffer = GL.GenBuffer());
					list = new List<float>();

					lineFace(ref points, ref list, 0, 1, 2, 3);
					lineFace(ref points, ref list, 4, 5, 6, 7);
					line(ref points, ref list, 0, 4);
					line(ref points, ref list, 1, 5);
					line(ref points, ref list, 2, 6);
					line(ref points, 
ref list, 3, 7);

					data = list.ToArray();
					GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);

					GL.EnableVertexAttribArray(0);
					GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
					#endregion


					Initialized = true;
				}
			}

			public static void Draw(GL_ControlModern control, Pass pass, Vector4 blockColor, Vector4 lineColor, Color pickingColor)
			{
				control.CurrentShader = solidColorShaderProgram;

				if (pass == Pass.OPAQUE)
				{

					#region outlines
					GL.LineWidth(2.0f);

					GL.Uniform4(solidColorShaderProgram["color"], lineColor);
					GL.BindVertexArray(linesVao);
					GL.DrawArrays(PrimitiveType.Lines, 0, 24);
					#endregion
					control.CurrentShader = defaultShaderProgram;

					GL.Uniform1(defaultShaderProgram["tex"], Framework.TextureSheet - 1);

					GL.Uniform4(defaultShaderProgram["color"], blockColor);
				}
				else
					GL.Uniform4(solidColorShaderProgram["color"], pickingColor);

				GL.BindVertexArray(blockVao);
				GL.DrawArrays(PrimitiveType.Quads, 0, 24);
			}

			public static void Draw(GL_ControlLegacy control, Pass pass, Vector4 blockColor, Vector4 lineColor, Color pickingColor)
			{
				GL.Disable(EnableCap.Texture2D);

				GL.BindTexture(TextureTarget.Texture2D, 0);

				if (pass == Pass.OPAQUE)
				{
					#region draw lines
					GL.Color4(lineColor);

					GL.LineWidth(2.0f);

					GL.Begin(PrimitiveType.LineStrip);
					GL.Vertex3(points[6]);
					GL.Vertex3(points[2]);
					GL.Vertex3(points[3]);
					GL.Vertex3(points[7]);
					GL.Vertex3(points[6]);

					GL.Vertex3(points[4]);
					GL.Vertex3(points[5]);
					GL.Vertex3(points[1]);
					GL.Vertex3(points[0]);
					GL.Vertex3(points[4]);
					GL.End();

					GL.Begin(PrimitiveType.Lines);
					GL.Vertex3(points[2]);
					GL.Vertex3(points[0]);
					GL.Vertex3(points[3]);
					GL.Vertex3(points[1]);
					GL.Vertex3(points[7]);
					GL.Vertex3(points[5]);
					GL.End();
					#endregion
				}
				else
				{
					GL.Color4(pickingColor);


					GL.Begin(PrimitiveType.Quads);
					GL.Vertex3(points[7]);
					GL.Vertex3(points[6]);
					GL.Vertex3(points[2]);
					GL.Vertex3(points[3]);

					GL.Vertex3(points[4]);
					GL.Vertex3(points[5]);
					GL.Vertex3(points[1]);
					GL.Vertex3(points[0]);
					GL.End();

					GL.Begin(PrimitiveType.QuadStrip);
					GL.Vertex3(points[7]);
					GL.Vertex3(points[5]);
					GL.Vertex3(points[6]);
					GL.Vertex3(points[4]);
					GL.Vertex3(points[2]);
					GL.Vertex3(points[0]);
					GL.Vertex3(points[3]);
					GL.Vertex3(points[1]);
					GL.Vertex3(points[7]);
					GL.Vertex3(points[5]);
					GL.End();
				}
				GL.Enable(EnableCap.Texture2D);

				if (pass == Pass.OPAQUE)
				{
					GL.BindTexture(TextureTarget.Texture2D, Framework.TextureSheet);

					#region draw faces
					Vector4 darkerColor = blockColor * 0.71875f;
					GL.Begin(PrimitiveType.Quads);
					GL.Color4(blockColor);
					GL.TexCoord2(0.71875f, 0.515625f);
					GL.Vertex3(points[7]);
					GL.TexCoord2(0.53125f, 0.515625f);
					GL.Vertex3(points[6]);
					GL.TexCoord2(0.53125f, 0.984375f);
					GL.Vertex3(points[2]);
					GL.TexCoord2(0.71875f, 0.984375f);
					GL.Vertex3(points[3]);

					GL.Color4(darkerColor);
					GL.TexCoord2(0.53125f, 0.515625f);
					GL.Vertex3(points[4]);
					GL.TexCoord2(0.71875f, 0.515625f);
					GL.Vertex3(points[5]);
					GL.TexCoord2(0.71875f, 0.984375f);
					GL.Vertex3(points[1]);
					GL.TexCoord2(0.53125f, 0.984375f);
					GL.Vertex3(points[0]);
					GL.End();

					GL.Begin(PrimitiveType.QuadStrip);
					GL.TexCoord2(0.71875f, 0.515625f);
					GL.Color4(blockColor);
					GL.Vertex3(points[7]);
					GL.Color4(darkerColor);
					GL.Vertex3(points[5]);
					GL.Color4(blockColor);
					GL.Vertex3(points[6]);
					GL.Color4(darkerColor);
					GL.Vertex3(points[4]);
					GL.Color4(blockColor);
					GL.Vertex3(points[2]);
					GL.Color4(darkerColor);
					GL.Vertex3(points[0]);
					GL.Color4(blockColor);
					GL.Vertex3(points[3]);
					GL.Color4(darkerColor);
					GL.Vertex3(points[1]);
					GL.Color4(blockColor);
					GL.Vertex3(points[7]);
					GL.Color4(darkerColor);
					GL.Vertex3(points[5]);
					GL.End();
					#endregion
				}
			}
		}

		public static class LineBoxRenderer
		{

			private static bool Initialized = false;
			
			private static ShaderProgram defaultShaderProgram;

			private static int linesVao;

			private static float[][] points = new float[][]
			{
				new float[]{-1,-1, 1},
				new float[]{ 1,-1, 1},
				new float[]{-1, 1, 1},
				new float[]{ 1, 1, 1},
				new float[]{-1,-1,-1},
				new float[]{ 1,-1,-1},
				new float[]{-1, 1,-1},
				new float[]{ 1, 1,-1}
			};


			public static void Initialize()
			{
				if (!Initialized)
				{
					var defaultFrag = new FragmentShader(
						@"#version 330
						uniform vec4 color;
						void main(){
							gl_FragColor = color;
						}");
					var defaultVert = new VertexShader(
						@"#version 330
						layout(location = 0) in vec4 position;
						uniform mat4 mtxMdl;
						uniform mat4 mtxCam;
						void main(){
							gl_Position = mtxCam*mtxMdl*position;
						}");
					defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);

					int buffer;

					#region lines
					GL.BindVertexArray(linesVao = GL.GenVertexArray());

					GL.BindBuffer(BufferTarget.ArrayBuffer, buffer = GL.GenBuffer());
					List<float> list = new List<float>();

					lineFace(ref points, ref list, 0, 1, 2, 3);
					lineFace(ref points, ref list, 4, 5, 6, 7);
					line(ref points, ref list, 0, 4);
					line(ref points, ref list, 1, 5);
					line(ref points, ref list, 2, 6);
					line(ref points, ref list, 3, 7);

					float[] data = list.ToArray();
					GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * data.Length, data, BufferUsageHint.StaticDraw);

					GL.EnableVertexAttribArray(0);
					GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
					#endregion


					Initialized = true;
				}
			}

			public static void Draw(GL_ControlModern control, Pass pass, Vector4 color, Color pickingColor)
			{
				control.CurrentShader = defaultShaderProgram;

				if (pass == Pass.OPAQUE)
				{
					GL.LineWidth(4.0f);
					GL.Uniform4(defaultShaderProgram["color"], color);
				}
				else
				{
					GL.LineWidth(6.0f);
					GL.Uniform4(defaultShaderProgram["color"], pickingColor);
				}

				GL.BindVertexArray(linesVao);
				GL.DrawArrays(PrimitiveType.Lines, 0, 24);
			}

			public static void Draw(GL_ControlLegacy control, Pass pass, Vector4 color, Color pickingColor)
			{
				GL.Disable(EnableCap.Texture2D);

				GL.BindTexture(TextureTarget.Texture2D, 0);

				if (pass == Pass.OPAQUE)
				{
					GL.LineWidth(4.0f);
					GL.Color4(color);
				}
				else
				{
					GL.LineWidth(6.0f);
					GL.Color4(pickingColor);
				}

				GL.Begin(PrimitiveType.LineStrip);
				GL.Vertex3(points[6]);
				GL.Vertex3(points[2]);
				GL.Vertex3(points[3]);
				GL.Vertex3(points[7]);
				GL.Vertex3(points[6]);

				GL.Vertex3(points[4]);
				GL.Vertex3(points[5]);
				GL.Vertex3(points[1]);
				GL.Vertex3(points[0]);
				GL.Vertex3(points[4]);
				GL.End();

				GL.Begin(PrimitiveType.Lines);
				GL.Vertex3(points[2]);
				GL.Vertex3(points[0]);
				GL.Vertex3(points[3]);
				GL.Vertex3(points[1]);
				GL.Vertex3(points[7]);
				GL.Vertex3(points[5]);
				GL.End();
				
				GL.Enable(EnableCap.Texture2D);
			}
		}
	}
}
