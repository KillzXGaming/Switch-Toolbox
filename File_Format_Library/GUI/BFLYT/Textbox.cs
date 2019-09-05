using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Diagnostics;

namespace LayoutBXLYT
{
    public class Textbox
    {
        public int GlyphsPerLine = 16;
        public int GlyphLineCount = 16;
        public int GlyphWidth = 11;
        public int GlyphHeight = 22;

        public int CharXSpacing = 11;

        public int AtlasOffsetX = -3, AtlassOffsetY = -1;
        public int FontSize = 14;
        public bool BitmapFont = false;
        public string FromFile; //= "joystix monospace.ttf";
        public string FontName = "Consolas";

        public Textbox()
        {
            GenerateFontImage();
        }

        void GenerateFontImage()
        {
            int bitmapWidth = GlyphsPerLine * GlyphWidth;
            int bitmapHeight = GlyphLineCount * GlyphHeight;

            using (Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                Font font;
                if (!String.IsNullOrWhiteSpace(FromFile))
                {
                    var collection = new PrivateFontCollection();
                    collection.AddFontFile(FromFile);
                    var fontFamily = new FontFamily(Path.GetFileNameWithoutExtension(FromFile), collection);
                    font = new Font(fontFamily, FontSize);
                }
                else
                {
                    font = new Font(new FontFamily(FontName), FontSize);
                }

                using (var g = Graphics.FromImage(bitmap))
                {
                    if (BitmapFont)
                    {
                        g.SmoothingMode = SmoothingMode.None;
                        g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
                    }
                    else
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                        //g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    }

                    for (int p = 0; p < GlyphLineCount; p++)
                    {
                        for (int n = 0; n < GlyphsPerLine; n++)
                        {
                            char c = (char)(n + p * GlyphsPerLine);
                            g.DrawString(c.ToString(), font, Brushes.White,
                                n * GlyphWidth + AtlasOffsetX, p * GlyphHeight + AtlassOffsetY);
                        }
                    }
                }
              //  bitmap.Save(FontBitmapFilename);
            }
        //    Process.Start(FontBitmapFilename);
        }

        int TextureWidth;
        int TextureHeight;

        public void DrawText(int x, int y, string text)
        {
            GL.Begin(PrimitiveType.Quads);

            float u_step = (float)GlyphWidth / (float)TextureWidth;
            float v_step = (float)GlyphHeight / (float)TextureHeight;

            for (int n = 0; n < text.Length; n++)
            {
                char idx = text[n];
                float u = (float)(idx % GlyphsPerLine) * u_step;
                float v = (float)(idx / GlyphsPerLine) * v_step;

                GL.TexCoord2(u, v);
                GL.Vertex2(x, y);
                GL.TexCoord2(u + u_step, v);
                GL.Vertex2(x + GlyphWidth, y);
                GL.TexCoord2(u + u_step, v + v_step);
                GL.Vertex2(x + GlyphWidth, y + GlyphHeight);
                GL.TexCoord2(u, v + v_step);
                GL.Vertex2(x, y + GlyphHeight);

                x += CharXSpacing;
            }

            GL.End();
        }
    }
}
