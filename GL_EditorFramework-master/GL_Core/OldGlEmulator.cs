using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GL_Core
{
	public class OldGlEmulator : IDisposable
	{
		List<float> dataList = new List<float>();

		float current_r = 255f;
		float current_g = 255f;
		float current_b = 255f;
		float current_a = 255f;
		public OldGlEmulator()
		{

		}

		public void Vertex3(float x, float y, float z)
		{
			dataList.Add(x); dataList.Add(y); dataList.Add(z);
			dataList.Add(current_r); dataList.Add(current_g); dataList.Add(current_b); dataList.Add(current_a);
		}

		public void Color3(float r, float g, float b)
		{
			current_r = r;
			current_g = g;
			current_b = b;
		}

		public void WriteToBuffer(int buffer) {
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
			float[] arrayData = dataList.ToArray();
			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * arrayData.Length, arrayData, BufferUsageHint.StaticDraw);

			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 7, 0);
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, sizeof(float) * 7, sizeof(float) * 3);
		}

		public void Dispose()
		{
			
		}
	}
}
