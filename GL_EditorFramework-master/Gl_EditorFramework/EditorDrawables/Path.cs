using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GL_EditorFramework.EditorDrawables
{
	//this class is under developemnt and won't work right now

	/*
	public class Path : EditableObject
	{
		private static bool Initialized = false;
		private static ShaderProgram defaultShaderProgram;
		private static ShaderProgram defaultLinesShaderProgram;
		private static ShaderProgram connectLinesShaderProgram;

		private List<PathPoint> pathPoints;

		protected Vector4 lineColor = new Vector4(0.75f, 1f, 1f, 1f);

		public Path()
		{
			pathPoints = new List<PathPoint>();
			pathPoints.Add(new PathPoint(
				new Vector3d(0, 0, 0),
				new Vector3d(0, 0, 0),
				new Vector3d(3, 0, 0)
				));
			pathPoints.Add(new PathPoint(
				new Vector3d(8, 4, 2),
				new Vector3d(-4, 0, 4),
				new Vector3d(4, 0, -4)
				));
			pathPoints.Add(new PathPoint(
				new Vector3d(4, 2, -6),
				new Vector3d(0, 0, 0),
				new Vector3d(0, 0, 0)
				));
		}

		public override void Draw(GL_ControlModern control)
		{
			GL.LineWidth(2f);
			control.CurrentShader = defaultShaderProgram;

			Matrix4 mtx = Matrix4.CreateTranslation(Position);
			control.UpdateModelMatrix(mtx);

			GL.Uniform1(defaultShaderProgram["scale"], 0.5f);
			
			bool picked = EditorScene.IsHovered();

			GL.Begin(PrimitiveType.Points);
			{
				int i = 1;
				foreach (PathPoint point in pathPoints)
				{
					GL.VertexAttrib4(1, (picked || EditorScene.IsHovered(i)) ? CubeColor * 0.5f + hoverColor * 0.5f : CubeColor);
					GL.VertexAttrib3(2, point.controlPoint1);
					GL.VertexAttrib3(3, point.controlPoint2);
					GL.Vertex3(point.position);
					i++;
				}
			}
			GL.End();

			control.CurrentShader = defaultLinesShaderProgram;
			GL.Uniform1(defaultLinesShaderProgram["scale"], 0.5f);

			GL.Begin(PrimitiveType.Points);
			{
				int i = 1;
				foreach (PathPoint point in pathPoints)
				{
					GL.VertexAttrib4(1, (picked || EditorScene.IsHovered(i)) ? hoverColor : lineColor);
					GL.VertexAttrib3(2, point.controlPoint1);
					GL.VertexAttrib3(3, point.controlPoint2);
					GL.Vertex3(point.position);
					i++;
				}
			}
			GL.End();

			control.CurrentShader = connectLinesShaderProgram;

			GL.Uniform4(connectLinesShaderProgram["color"], picked ? hoverColor : CubeColor);
			
			GL.Begin(PrimitiveType.Lines);
			for(int i = 1; i<pathPoints.Count; i++)
			{
				PathPoint p1 = pathPoints[i - 1];
				PathPoint p2 = pathPoints[i];

				GL.VertexAttrib3(1, p1.controlPoint2);
				GL.Vertex3(p1.position);

				GL.VertexAttrib3(1, p2.controlPoint1);
				GL.Vertex3(p2.position);
			}
			GL.End();
		}

		public override void Draw(GL_ControlLegacy control)
		{
			throw new NotImplementedException();
		}

		public override void DrawPicking(GL_ControlModern control)
		{
			control.CurrentShader = connectLinesShaderProgram;

			Matrix4 mtx = Matrix4.CreateTranslation(Position);
			control.UpdateModelMatrix(mtx);

			GL.Uniform4(connectLinesShaderProgram["color"], control.nextPickingColor());

			GL.Begin(PrimitiveType.Lines);
			for (int i = 1; i < pathPoints.Count; i++)
			{
				PathPoint p1 = pathPoints[i - 1];
				PathPoint p2 = pathPoints[i];

				GL.VertexAttrib3(1, p1.controlPoint2);
				GL.Vertex3(p1.position);

				GL.VertexAttrib3(1, p2.controlPoint1);
				GL.Vertex3(p2.position);
			}
			GL.End();

			control.CurrentShader = defaultShaderProgram;
			GL.Uniform1(defaultShaderProgram["scale"], 0.5f);
			GL.Begin(PrimitiveType.Points);
			foreach (PathPoint point in pathPoints)
			{
				System.Drawing.Color c = control.nextPickingColor();
				GL.VertexAttrib4(1, c.R/256f, c.G / 256f, c.B / 256f, c.A / 256f);
				GL.VertexAttrib3(2, point.controlPoint1);
				GL.VertexAttrib3(3, point.controlPoint2);
				GL.Vertex3(point.position);
			}
			GL.End();
		}

		public override void Prepare(GL_ControlModern control)
		{
			if (!Initialized)
			{
				var defaultFrag = new FragmentShader(
			  @"#version 330
				in vec4 fragColor;
				void main(){
					gl_FragColor = fragColor;
				}");

				var defaultVert = new VertexShader(
				  @"#version 330
				in vec4 position;
				
				layout(location = 1) in vec4 color;
				layout(location = 2) in vec3 _cp1;
				layout(location = 3) in vec3 _cp2;

				out vec4 vertColor;
				out vec4 cp1;
				out vec4 cp2;

				void main(){
					cp1 = vec4(_cp1,0);
					cp2 = vec4(_cp2,0);
					vertColor = color;
					gl_Position = position;
				}");

				#region block shader
				defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);

				defaultShaderProgram.AttachShader(new Shader(
				  @"#version 330
                layout(points) in;
				layout(triangle_strip, max_vertices = 72) out;
				
				in vec4 vertColor[];
				in vec4 cp1[];
				in vec4 cp2[];
				out vec4 fragColor;

				uniform mat4 mtxMdl;
				uniform mat4 mtxCam;
				uniform float scale;
				
				float cubeScale;
				vec4 pos;

				mat4 mtx = mtxCam*mtxMdl;
				
				vec4 points[8] = vec4[](
					vec4(-1.0,-1.0,-1.0, 0.0),
					vec4( 1.0,-1.0,-1.0, 0.0),
					vec4(-1.0, 1.0,-1.0, 0.0),
					vec4( 1.0, 1.0,-1.0, 0.0),
					vec4(-1.0,-1.0, 1.0, 0.0),
					vec4( 1.0,-1.0, 1.0, 0.0),
					vec4(-1.0, 1.0, 1.0, 0.0),
					vec4( 1.0, 1.0, 1.0, 0.0)
				);

				void face(int p1, int p2, int p3, int p4){
					gl_Position = mtx * (pos + points[p1]*cubeScale); EmitVertex();
					gl_Position = mtx * (pos + points[p2]*cubeScale); EmitVertex();
					gl_Position = mtx * (pos + points[p3]*cubeScale); EmitVertex();
					gl_Position = mtx * (pos + points[p4]*cubeScale); EmitVertex();
					EndPrimitive();
				}

				void faceInv(int p3, int p4, int p1, int p2){
					gl_Position = mtx * (pos + points[p1]*cubeScale); EmitVertex();
					gl_Position = mtx * (pos + points[p2]*cubeScale); EmitVertex();
					gl_Position = mtx * (pos + points[p3]*cubeScale); EmitVertex();
					gl_Position = mtx * (pos + points[p4]*cubeScale); EmitVertex();
					EndPrimitive();
				}
				
				void main(){
					cubeScale = scale;
					pos = gl_in[0].gl_Position;
					fragColor = vertColor[0];
					face(0,1,2,3);
					faceInv(4,5,6,7);
					faceInv(0,1,4,5);
					face(2,3,6,7);
					face(0,2,4,6);
					faceInv(1,3,5,7);

					cubeScale = scale*0.5;

					if(cp1[0]!=vec4(0,0,0,0)){
						pos = gl_in[0].gl_Position+cp1[0];
						face(0,1,2,3);
						faceInv(4,5,6,7);
						faceInv(0,1,4,5);
						face(2,3,6,7);
						face(0,2,4,6);
						faceInv(1,3,5,7);
					}

					if(cp2[0]!=vec4(0,0,0,0)){
						pos = gl_in[0].gl_Position+cp2[0];
						face(0,1,2,3);
						faceInv(4,5,6,7);
						faceInv(0,1,4,5);
						face(2,3,6,7);
						face(0,2,4,6);
						faceInv(1,3,5,7);
					}
				}
				", ShaderType.GeometryShader));

				defaultShaderProgram.LinkShaders();
				#endregion

				#region lines shader
				defaultLinesShaderProgram = new ShaderProgram(defaultFrag, defaultVert);

				defaultLinesShaderProgram.AttachShader(new Shader(
				  @"#version 330
                layout(points) in;
				layout(line_strip, max_vertices = 72) out;
				
				in vec4 vertColor[];
				in vec4 cp1[];
				in vec4 cp2[];
				out vec4 fragColor;

				uniform mat4 mtxMdl;
				uniform mat4 mtxCam;
				uniform float scale;
				
				float cubeScale;
				vec4 pos;
				
				mat4 mtx = mtxCam*mtxMdl;
				
				vec4 points[8] = vec4[](
					vec4(-1.0,-1.0,-1.0, 0.0),
					vec4( 1.0,-1.0,-1.0, 0.0),
					vec4(-1.0, 1.0,-1.0, 0.0),
					vec4( 1.0, 1.0,-1.0, 0.0),
					vec4(-1.0,-1.0, 1.0, 0.0),
					vec4( 1.0,-1.0, 1.0, 0.0),
					vec4(-1.0, 1.0, 1.0, 0.0),
					vec4( 1.0, 1.0, 1.0, 0.0)
				);

				void face(int p1, int p2, int p4, int p3){
					gl_Position = mtx * (pos + points[p1]*cubeScale); EmitVertex();
					gl_Position = mtx * (pos + points[p2]*cubeScale); EmitVertex();
					gl_Position = mtx * (pos + points[p3]*cubeScale); EmitVertex();
					gl_Position = mtx * (pos + points[p4]*cubeScale); EmitVertex();
					gl_Position = mtx * (pos + points[p1]*cubeScale); EmitVertex();
					EndPrimitive();
				}

				void line(int p1, int p2){
					gl_Position = mtx * (pos + points[p1]*cubeScale); EmitVertex();
					gl_Position = mtx * (pos + points[p2]*cubeScale); EmitVertex();
					EndPrimitive();
				}
				
				void main(){
					cubeScale = scale;
					pos = gl_in[0].gl_Position;
					fragColor = vertColor[0];
					face(0,1,2,3);
					face(4,5,6,7);
					line(0,4);
					line(1,5);
					line(2,6);
					line(3,7);

					cubeScale = scale*0.5;

					if(cp1[0]!=vec4(0,0,0,0)){
						pos = gl_in[0].gl_Position+cp1[0];
						face(0,1,2,3);
						face(4,5,6,7);
						line(0,4);
						line(1,5);
						line(2,6);
						line(3,7);
					}

					if(cp2[0]!=vec4(0,0,0,0)){
						pos = gl_in[0].gl_Position+cp2[0];
						face(0,1,2,3);
						face(4,5,6,7);
						line(0,4);
						line(1,5);
						line(2,6);
						line(3,7);
					}
				}
				", ShaderType.GeometryShader));

				defaultLinesShaderProgram.LinkShaders();
				#endregion

				var connectLinesFrag = new FragmentShader(
				  @"#version 330
				uniform vec4 color;
				void main(){
					gl_FragColor = color;
				}");
				var connectLinesVert = new VertexShader(
				  @"#version 330
				in vec4 position;
				
				layout(location = 1) in vec3 _controlPoint;

				out vec4 controlPoint;

				void main(){
					controlPoint = vec4(_controlPoint,0.0);
					gl_Position = position;
				}");

				#region connections shader
				connectLinesShaderProgram = new ShaderProgram(connectLinesFrag, connectLinesVert);

				connectLinesShaderProgram.AttachShader(new Shader(
				  @"#version 330
                layout(lines) in;
				layout(line_strip, max_vertices = 19) out;
				
				in vec4 controlPoint[];

				uniform mat4 mtxMdl;
				uniform mat4 mtxCam;
				
				mat4 mtx = mtxCam*mtxMdl;
				
				vec4 p0 = gl_in[0].gl_Position;
				vec4 p1 = gl_in[0].gl_Position + controlPoint[0];
				vec4 p2 = gl_in[1].gl_Position + controlPoint[1];
				vec4 p3 = gl_in[1].gl_Position;

				void getPointAtTime(float t){
					float u = 1f - t;
					float tt = t * t;
					float uu = u * u;
					float uuu = uu * u;
					float ttt = tt * t;

					gl_Position = mtx * (uuu    * p0 +
									3 * uu * t * p1 +
									3 * u  *tt * p2 +
										ttt    * p3);
					EmitVertex();
				}


				void main(){
					gl_Position = mtx * p1;
					EmitVertex();
					for(float t = 0; t<=1.0; t+=0.0625){
						getPointAtTime(t);
					}
					gl_Position = mtx * p2;
					EmitVertex();
					EndPrimitive();
				}
				", ShaderType.GeometryShader));
				#endregion

				connectLinesShaderProgram.LinkShaders();

				Initialized = true;
			}
		}

		public override void Prepare(GL_ControlLegacy control)
		{
			throw new NotImplementedException();
		}

		public override int GetPickableSpan() => pathPoints.Count + 1;

		

		struct PathPoint
		{
			public PathPoint(Vector3d position, Vector3d controlPoint1, Vector3d controlPoint2)
			{
				this.position = position;
				this.controlPoint1 = controlPoint1;
				this.controlPoint2 = controlPoint2;
			}
			public Vector3d position;
			public Vector3d controlPoint1;
			public Vector3d controlPoint2;
		}
	}
	*/
}
