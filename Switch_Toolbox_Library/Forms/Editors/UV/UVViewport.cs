using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using Toolbox.Library.IO;

namespace Toolbox.Library.Forms
{
    public class UVViewport : Viewport2D
    {
        public STGenericMatTexture ActiveTextureMap { get; set; }

        public UVViewport()
        {
        }

        public List<STGenericObject> ActiveObjects = new List<STGenericObject>();

        /// <summary>
        /// Selectes the group of polygons to display UVs for. -1 set to use all of them.
        /// </summary>
        public int PolygonGroupIndex = -1;

        public int UvChannelIndex = 0;

        public bool Repeat = true;

        public float Brightness = 1.0f;

        public PickableUVMap PickableUVMap = new PickableUVMap();

        public override List<IPickable2DObject> GetPickableObjects()
        {
            return new List<IPickable2DObject>() { PickableUVMap };
        }

        public override void RenderScene()
        {
            Vector3 Scale = new Vector3(30, 30, 30);

            //Scale the plane by aspect ratio
            GL.PushMatrix();

            bool useTextures = false;
            if (ActiveTextureMap != null) {
                var tex = ActiveTextureMap.GetTexture();

                //Bind texture if not null
                if (tex != null)
                {
                    //Adjust scale via aspect ratio
                    if (Width > Height)
                    {
                        float aspect = (float)tex.Width / (float)tex.Height;
                        Console.WriteLine($"aspect w {aspect}");
                        Scale.X *= aspect;
                    }
                    else
                    {
                        float aspect = (float)tex.Height / (float)tex.Width;
                        Console.WriteLine($"aspect h {aspect}");
                        Scale.Y *= aspect;
                    }

                    if (tex.RenderableTex == null || !tex.RenderableTex.GLInitialized)
                        tex.LoadOpenGLTexture();                        

                    GL.Enable(EnableCap.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, tex.RenderableTex.TexID);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)STGenericMatTexture.wrapmode[ActiveTextureMap.WrapModeS]);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)STGenericMatTexture.wrapmode[ActiveTextureMap.WrapModeT]);
                    //     GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)STGenericMatTexture.wrapmode[activeTexture.wrapModeS]);
                    //   GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)STGenericMatTexture.wrapmode[activeTexture.wrapModeT]);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)STGenericMatTexture.minfilter[ActiveTextureMap.MinFilter]);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)STGenericMatTexture.magfilter[ActiveTextureMap.MagFilter]);

                    useTextures = true;
                }
            }

            GL.Scale(Scale);

            if (useTextures)
            {
                GL.Enable(EnableCap.Texture2D);
                if (Repeat)
                {
                   DrawPlane(50, Color.FromArgb(128, 128,128));
                   DrawPlane(1, Color.White);
                }
                else
                    DrawPlane(1, Color.White);
            }
            else
            {
                DrawPlane(1, BackgroundColor.Lighten(40));
            }

            GL.Disable(EnableCap.Texture2D);

            PickableUVMap.DrawUVs(PolygonGroupIndex, UvChannelIndex, ActiveObjects, ActiveTextureMap);
            DrawPlane(1, Color.Black, true);

            GL.PopMatrix();
        }

        private void DrawPlane(float scale, Color color, bool Outlines = false)
        {
            Vector2 scaleCenter = new Vector2(0.5f, 0.5f);

            color = Color.FromArgb(
                (byte)(color.R * Brightness),
                (byte)(color.G * Brightness),
                (byte)(color.B * Brightness));

            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0),
            };

            Vector2[] Positions = new Vector2[] {
                new Vector2(1,-1),
                new Vector2(-1,-1),
                new Vector2(-1,1),
                new Vector2(1,1),
            };

            TexCoords[0] = (TexCoords[0] - scaleCenter) * scale + scaleCenter;
            TexCoords[1] = (TexCoords[1] - scaleCenter) * scale + scaleCenter;
            TexCoords[2] = (TexCoords[2] - scaleCenter) * scale + scaleCenter;
            TexCoords[3] = (TexCoords[3] - scaleCenter) * scale + scaleCenter;
            Positions[0] = Positions[0] * scale;
            Positions[1] = Positions[1] * scale;
            Positions[2] = Positions[2] * scale;
            Positions[3] = Positions[3] * scale;

            if (Outlines)
            {
                GL.LineWidth(1);
                GL.Begin(PrimitiveType.LineLoop);
                GL.Color3(color);
                GL.Vertex2(Positions[0]);
                GL.Vertex2(Positions[1]);
                GL.Vertex2(Positions[2]);
                GL.Vertex2(Positions[3]);
                GL.End();
            }
            else
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Color3(color);
                GL.TexCoord2(TexCoords[0]);
                GL.Vertex2(Positions[0]);
                GL.TexCoord2(TexCoords[1]);
                GL.Vertex2(Positions[1]);
                GL.TexCoord2(TexCoords[2]);
                GL.Vertex2(Positions[2]);
                GL.TexCoord2(TexCoords[3]);
                GL.Vertex2(Positions[3]);
                GL.End();
            }


        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UVViewport
            // 
            this.Name = "UVViewport";
            this.ResumeLayout(false);

        }
    }
}
