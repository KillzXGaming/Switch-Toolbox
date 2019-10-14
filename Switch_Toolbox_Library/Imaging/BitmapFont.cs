using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

//Thanks EFE!
//https://github.com/Gericom/EveryFileExplorer/blob/f9f00d193c9608d71c9a23d9f3ab7e752f4ada2a/LibEveryFileExplorer/GFX/BitmapFont.cs
namespace LibEveryFileExplorer.GFX
{
    public class BitmapFont : IDisposable
    {
        public BitmapFont()
        {
            Characters = new Dictionary<char, Character>();
        }

        public Dictionary<char, Character> Characters { get; private set; }
        public int LineHeight { get; set; }

        public int GetLineWidth(String Text, FontRenderSettings Settings, out int LineEnd)
        {
            LineEnd = -1;
            int result = 0;
            int i = 0;
            foreach (char c in Text)
            {
                if (c == '\n')
                {
                    if (Text.Length > i + 1) LineEnd = i + 1;
                    break;
                }
                else if (c == '\r') { i++; continue; }
                if (!Characters.ContainsKey(c)) result += Settings.CharSpacing * 4;
                else
                {
                    Character info = Characters[c];
                    result += (int)(info.CharWidth * Settings.XScale) + Settings.CharSpacing;
                }
                i++;
            }
            if (result > 0) result -= Settings.CharSpacing;
            return result;
        }

        public Bitmap PrintToBitmap(String Text, FontRenderSettings Settings)
        {
            int Width = 0;
            int Height = 0;

            int linestart = 0;
            do
            {
                int lns;
                int linewidth = GetLineWidth(Text.Substring(linestart), Settings, out lns);
                if (lns != -1) linestart += lns;
                else linestart = -1;
                if (linewidth > Width) Width = linewidth;
                Height += (int)(LineHeight * Settings.YScale) + Settings.LineSpacing;
            }
            while (linestart != -1);
            if (Width == 0 || Height == 0) return null;
            Height -= Settings.LineSpacing;
            Width += 2;
            Bitmap b = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
                int X = 0;
                int Y = 0;
                foreach (char c in Text)
                {
                    if (c == '\n')
                    {
                        X = 0;
                        Y += (int)(LineHeight * Settings.YScale) + Settings.LineSpacing;
                        continue;
                    }
                    else if (c == '\r') continue;
                    if (!Characters.ContainsKey(c)) X += Settings.CharSpacing * 4;
                    else
                    {
                        Character info = Characters[c];
                        g.DrawImage(info.CharBitmap,
                            new RectangleF(X + info.LeftOffset, Y, info.GlyphWidth * Settings.XScale, LineHeight * Settings.YScale),
                            new Rectangle(0, 0, info.GlyphWidth, LineHeight),
                            GraphicsUnit.Pixel);
                        X += (int)(info.CharWidth * Settings.XScale) + Settings.CharSpacing;
                    }
                }
            }
            return b;
        }

        public void Dispose()
        {
            if (Characters == null) return;

            foreach (var character in Characters.Values)
                character.CharBitmap?.Dispose();

            Characters.Clear();

            GC.SuppressFinalize(this);
        }

        public class Character
        {
            public Character(Bitmap CharBitmap, int LeftOffset, int GlyphWidth, int CharWidth)
            {
                this.CharBitmap = CharBitmap;
                this.LeftOffset = LeftOffset;
                this.GlyphWidth = GlyphWidth;
                this.CharWidth = CharWidth;
            }
            public Bitmap CharBitmap { get; set; }
            public int LeftOffset { get; set; }
            public int GlyphWidth { get; set; }
            public int CharWidth { get; set; }
        }

        public class FontRenderSettings
        {
            public FontRenderSettings()
            {
                CharSpacing = 3;
                XScale = YScale = 1;
                HAlignment = XAlignment.Left;
                VAlignment = YAlignment.Top;
            }

            public Color BottomColor;
            public Color TopColor;

            public int CharSpacing { get; set; }
            public int LineSpacing { get; set; }

            public float XScale { get; set; }
            public float YScale { get; set; }

            public XAlignment HAlignment { get; set; }
            public YAlignment VAlignment { get; set; }

            public enum XAlignment
            {
                Left, Center, Right
            }

            public enum YAlignment
            {
                Top, Center, Bottom
            }
        }
    }
}