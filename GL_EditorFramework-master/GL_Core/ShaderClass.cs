using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GL_Core
{
	public class ShaderProgram
	{
		private int fragSh, vertSh, program, mtxMdlLoc, mtxCamLoc;
		private Matrix4 modelMatrix;
		private Matrix4 computedCamMtx;

		public ShaderProgram(FragmentShader frag, VertexShader vert)
		{
			fragSh = frag.shader;
			vertSh = vert.shader;
			program = GL.CreateProgram();
			GL.AttachShader(program, vertSh);
			GL.AttachShader(program, fragSh);
			GL.LinkProgram(program);
			Console.WriteLine("fragment:");
			Console.WriteLine(GL.GetShaderInfoLog(fragSh));
			Console.WriteLine("vertex:");
			Console.WriteLine(GL.GetShaderInfoLog(fragSh));

			mtxMdlLoc = GL.GetUniformLocation(program, "mtxMdl");
			mtxCamLoc = GL.GetUniformLocation(program, "mtxCam");
		}

		public void AttachShader(ShaderGL shader)
		{
			GL.AttachShader(program, shader.shader);
		}

		public void DetachShader(ShaderGL shader)
		{
			GL.DetachShader(program, shader.shader);
		}

		public void LinkShaders()
		{
			GL.LinkProgram(program);
		}

		public void SetFragmentShader(FragmentShader shader)
		{
			GL.DetachShader(program, fragSh);
			GL.AttachShader(program, shader.shader);
			fragSh = shader.shader;
			GL.LinkProgram(program);
		}

		public void SetVertexShader(VertexShader shader)
		{
			GL.DetachShader(program, vertSh);
			GL.AttachShader(program, shader.shader);
			vertSh = shader.shader;
			GL.LinkProgram(program);

			mtxMdlLoc = GL.GetUniformLocation(program, "mtxMdl");
			mtxCamLoc = GL.GetUniformLocation(program, "mtxCam");

			GL.UniformMatrix4(mtxMdlLoc, false, ref modelMatrix);
			GL.UniformMatrix4(mtxCamLoc, false, ref computedCamMtx);
		}


		public void Setup(Matrix4 mtxMdl, Matrix4 mtxCam, Matrix4 mtxProj)
		{
			GL.UseProgram(program);
			modelMatrix = mtxMdl;
			GL.UniformMatrix4(mtxMdlLoc, false, ref modelMatrix);
			computedCamMtx = mtxCam * mtxProj;
			GL.UniformMatrix4(mtxCamLoc, false, ref computedCamMtx);
		}

		public void UpdateModelMatrix(Matrix4 matrix)
		{
			modelMatrix = matrix;
			GL.UniformMatrix4(mtxMdlLoc, false, ref matrix);
		}

		public void Activate()
		{
			GL.UseProgram(program);
		}

		public int this[string name]{
			get => GL.GetUniformLocation(program, name);
		}
	}

	public class ShaderGL
	{
		public ShaderGL(string src, ShaderType type)
		{
			shader = GL.CreateShader(type);
			GL.ShaderSource(shader, src);
			GL.CompileShader(shader);
			this.type = type;
		}

		public ShaderType type;

		public int shader;
	}

	public class FragmentShader : ShaderGL
    {
		public FragmentShader(string src)
			:base(src, ShaderType.FragmentShader)
		{

		}
	}

    public class GeomeryShader : ShaderGL
    {
        public GeomeryShader(string src)
            : base(src, ShaderType.GeometryShader)
        {

        }
    }

    public class VertexShader : ShaderGL
    {
		public VertexShader(string src)
			: base(src, ShaderType.VertexShader)
		{

		}
	}
}
