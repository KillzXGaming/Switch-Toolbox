using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.GL_Core;
using OpenTK.Graphics.OpenGL;

namespace GL_EditorFramework
{
	public sealed class Framework {
		public static void Initialize()
		{
			if (initialized)
				return;

			//texture sheet
			TextureSheet = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, TextureSheet);

			var bmp = OpenGl_EditorFramework.Properties.Resources.TextureSheet;
			var bmpData = bmp.LockBits(
				new System.Drawing.Rectangle(0, 0, 128*4, 128*2),
				System.Drawing.Imaging.ImageLockMode.ReadOnly,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, 128*4, 128*2, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			bmp.UnlockBits(bmpData);
			
			initialized = true;
		}
		private static bool initialized = false;
		public static int TextureSheet;
	}
}
