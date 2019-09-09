using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;
using Toolbox.Library.IO;
using OpenTK;

namespace LayoutBXLYT
{
    public class BxlytToGL
    {
        public static int ConvertTextureWrap(WrapMode wrapMode)
        {
            switch (wrapMode)
            {
                case WrapMode.Clamp: return (int)TextureWrapMode.Clamp;
                case WrapMode.Mirror: return (int)TextureWrapMode.MirroredRepeat;
                case WrapMode.Repeat: return (int)TextureWrapMode.Repeat;
                default: return (int)TextureWrapMode.Clamp;
            }
        }

        public static int ConvertMagFilterMode(FilterMode filterMode)
        {
            switch (filterMode)
            {
                case FilterMode.Linear: return (int)TextureMagFilter.Linear;
                case FilterMode.Near: return (int)TextureMagFilter.Nearest;
                default: return (int)TextureMagFilter.Linear;
            }
        }

        public static int ConvertMinFilterMode(FilterMode filterMode)
        {
            switch (filterMode)
            {
                case FilterMode.Linear: return (int)TextureMinFilter.Linear;
                case FilterMode.Near: return (int)TextureMinFilter.Nearest;
                default: return (int)TextureMinFilter.Linear;
            }
        }

        public static void DrawPictureBox(BasePane pane, byte effectiveAlpha, Dictionary<string, STGenericTexture> Textures)
        {
            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0)
                };

            if (pane is Cafe.BFLYT.PIC1)
            {
                var pic1Pane = pane as Cafe.BFLYT.PIC1;

                Color[] Colors = new Color[] {
                    pic1Pane.ColorTopLeft.Color,
                    pic1Pane.ColorTopRight.Color,
                    pic1Pane.ColorBottomRight.Color,
                    pic1Pane.ColorBottomLeft.Color,
                };

                var mat = pic1Pane.Material;
                if (mat.Shader == null)
                {
                    mat.Shader = new BflytShader(mat);
                    mat.Shader.Compile();
                }

                mat.Shader.Enable();
                ((BflytShader)mat.Shader).SetMaterials(Textures);

                if (pic1Pane.TexCoords.Length > 0)
                {
                    TexCoords = new Vector2[] {
                        pic1Pane.TexCoords[0].BottomLeft.ToTKVector2(),
                        pic1Pane.TexCoords[0].BottomRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopLeft.ToTKVector2(),
                   };
                }

                DrawRectangle(pane.Rectangle, TexCoords, Colors, false, effectiveAlpha);

                mat.Shader.Disable();

                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.PopAttrib();
            }
            else if (pane is BCLYT.PIC1)
            {
                var pic1Pane = pane as BCLYT.PIC1;

                Color[] Colors = new Color[] {
                    pic1Pane.ColorBottomLeft.Color,
                    pic1Pane.ColorBottomRight.Color,
                    pic1Pane.ColorTopRight.Color,
                    pic1Pane.ColorTopLeft.Color,
                };

                var mat = pic1Pane.Material;
                if (mat.Shader == null)
                {
                    mat.Shader = new BclytShader(mat);
                    mat.Shader.Compile();
                }

                mat.Shader.Enable();
                ((BclytShader)mat.Shader).SetMaterials(Textures);

                if (pic1Pane.TexCoords.Length > 0)
                {
                    TexCoords = new Vector2[] {
                        pic1Pane.TexCoords[0].BottomLeft.ToTKVector2(),
                        pic1Pane.TexCoords[0].BottomRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopLeft.ToTKVector2(),
                   };
                }

                DrawRectangle(pane.Rectangle, TexCoords, Colors, false, effectiveAlpha);

                mat.Shader.Disable();

                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.PopAttrib();
            }
            else if (pane is BRLYT.PIC1)
            {
                var pic1Pane = pane as BRLYT.PIC1;

                Color[] Colors = new Color[] {
                    pic1Pane.ColorBottomLeft.Color,
                    pic1Pane.ColorBottomRight.Color,
                    pic1Pane.ColorTopRight.Color,
                    pic1Pane.ColorTopLeft.Color,
                };

                var mat = pic1Pane.Material;
                if (mat.Shader == null)
                {
                    mat.Shader = new BrlytShader(mat);
                    mat.Shader.Compile();
                }

                mat.Shader.Enable();
                ((BrlytShader)mat.Shader).SetMaterials(Textures);

                if (pic1Pane.TexCoords.Length > 0)
                {
                    TexCoords = new Vector2[] {
                        pic1Pane.TexCoords[0].BottomLeft.ToTKVector2(),
                        pic1Pane.TexCoords[0].BottomRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopRight.ToTKVector2(),
                        pic1Pane.TexCoords[0].TopLeft.ToTKVector2(),
                   };
                }

                DrawRectangle(pane.Rectangle, TexCoords, Colors, false, effectiveAlpha);

                mat.Shader.Disable();

                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.PopAttrib();
            }
        }
    
        public static void DrawBoundryPane(BasePane pane, byte effectiveAlpha, List<BasePane> SelectedPanes)
        {
            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0)
                };

            Color color = Color.DarkGreen;
            if (SelectedPanes.Contains(pane))
                color = Color.Red;

            color = Color.FromArgb(70, color);

            Color[] Colors = new Color[] {
                color,
                color,
                color,
                color,
                };

            BxlytToGL.DrawRectangle(pane.Rectangle, TexCoords, Colors, false, effectiveAlpha);
        }

        public static bool BindGLTexture(BxlytTextureRef tex, STGenericTexture texture)
        {
            if (texture.RenderableTex == null || !texture.RenderableTex.GLInitialized)
                texture.LoadOpenGLTexture();

            //If the texture is still not initialized then return
            if (!texture.RenderableTex.GLInitialized)
                return false;

            GL.BindTexture(TextureTarget.Texture2D, texture.RenderableTex.TexID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleR, ConvertChannel(texture.RedChannel));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleG, ConvertChannel(texture.GreenChannel));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleB, ConvertChannel(texture.BlueChannel));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleA, ConvertChannel(texture.AlphaChannel));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ConvertTextureWrap(tex.WrapModeU));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ConvertTextureWrap(tex.WrapModeV));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ConvertMagFilterMode(tex.MaxFilterMode));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ConvertMinFilterMode(tex.MinFilterMode));

            return true;
        }

        private static int ConvertChannel(STChannelType type)
        {
            switch (type)
            {
                case STChannelType.Red: return (int)TextureSwizzle.Red;
                case STChannelType.Green: return (int)TextureSwizzle.Green;
                case STChannelType.Blue: return (int)TextureSwizzle.Blue;
                case STChannelType.Alpha: return (int)TextureSwizzle.Alpha;
                case STChannelType.Zero: return (int)TextureSwizzle.Zero;
                case STChannelType.One: return (int)TextureSwizzle.One;
                default: return 0;
            }
        }

        enum TextureSwizzle
        {
            Zero = All.Zero,
            One = All.One,
            Red = All.Red,
            Green = All.Green,
            Blue = All.Blue,
            Alpha = All.Alpha,
        }

        public static void DrawRectangle(CustomRectangle rect, Vector2[] texCoords,
    Color[] colors, bool useLines = true, byte alpha = 255)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                uint setalpha = (uint)((colors[i].A * alpha) / 255);
                colors[i] = Color.FromArgb((int)setalpha, colors[i]);
            }

            if (useLines)
            {
                GL.Begin(PrimitiveType.LineLoop);
                GL.Color4(colors[0]);
                GL.Vertex2(rect.LeftPoint, rect.BottomPoint);
                GL.Vertex2(rect.RightPoint, rect.BottomPoint);
                GL.Vertex2(rect.RightPoint, rect.TopPoint);
                GL.Vertex2(rect.LeftPoint, rect.TopPoint);
                GL.End();
            }
            else
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Color4(colors[0]);
                GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[0].X, texCoords[0].Y);
                GL.Vertex2(rect.LeftPoint, rect.BottomPoint);
                GL.Color4(colors[1]);
                GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[1].X, texCoords[1].Y);
                GL.Vertex2(rect.RightPoint, rect.BottomPoint);
                GL.Color4(colors[2]);
                GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[2].X, texCoords[2].Y);
                GL.Vertex2(rect.RightPoint, rect.TopPoint);
                GL.Color4(colors[3]);
                GL.MultiTexCoord2(TextureUnit.Texture0, texCoords[3].X, texCoords[3].Y);
                GL.Vertex2(rect.LeftPoint, rect.TopPoint);
                GL.End();

                //Draw outline
                GL.Begin(PrimitiveType.LineLoop);
                GL.LineWidth(3);
                GL.Color4(colors[0]);
                GL.Vertex2(rect.LeftPoint, rect.BottomPoint);
                GL.Vertex2(rect.RightPoint, rect.BottomPoint);
                GL.Vertex2(rect.RightPoint, rect.TopPoint);
                GL.Vertex2(rect.LeftPoint, rect.TopPoint);
                GL.End();
            }
        }
    }
}
