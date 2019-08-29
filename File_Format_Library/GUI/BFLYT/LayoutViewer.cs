using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Toolbox.Library;
using Toolbox.Library.Rendering;
using Toolbox.Library.IO;

namespace LayoutBXLYT
{
    public partial class LayoutViewer : LayoutDocked
    {
        public List<BasePane> SelectedPanes = new List<BasePane>();

        public Camera2D Camera = new Camera2D();

        public class Camera2D
        {
            public float Zoom = 1;
            public Vector2 Position;
        }

        private RenderableTex backgroundTex;

        public BxlytHeader LayoutFile;

        private static Dictionary<string, STGenericTexture> Textures;

        public LayoutViewer(BFLYT.Header bflyt)
        {
            InitializeComponent();
            LayoutFile = bflyt;
            Text = bflyt.FileName;

            Textures = new Dictionary<string, STGenericTexture>();
            if (bflyt.TextureList.Textures.Count > 0)
                Textures = ((BFLYT)bflyt.FileInfo).GetTextures();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to close this file? You will lose any unsaved progress!", "Layout Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                e.Cancel = true;
            else
                LayoutFile.Dispose();

            base.OnFormClosing(e);
        }

        public void UpdateViewport()
        {
            glControl1.Invalidate();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!Runtime.OpenTKInitialized)
                return;

            glControl1.Context.MakeCurrent(glControl1.WindowInfo);
            OnRender();
        }

        private Color BackgroundColor = Color.FromArgb(130, 130, 130);
        private void OnRender()
        {
            if (LayoutFile == null) return;

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, glControl1.Width, glControl1.Height, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            DrawRootPane(LayoutFile.RootPane);
            DrawGrid();
            DrawXyLines();

            GL.Scale(1 * Camera.Zoom, -1 * Camera.Zoom, 1);
            GL.Translate(Camera.Position.X, Camera.Position.Y, 0);

            RenderPanes(LayoutFile.RootPane, true);

            glControl1.SwapBuffers();
        }

        private void RenderPanes(BasePane pane, bool isRoot)
        {
            if (!pane.DisplayInEditor)
                return;

            GL.PushMatrix();
            GL.Translate(pane.Translate.X, pane.Translate.Y, 0);
            GL.Rotate(pane.Rotate.Z, pane.Rotate.X, pane.Rotate.Y, pane.Rotate.Z);
            GL.Scale(pane.Scale.X, pane.Scale.Y, 1);

            if (!isRoot)
            {
                if (pane is BFLYT.PIC1)
                    DrawPicturePane((BFLYT.PIC1)pane);
                else if (pane is BFLYT.PAN1)
                    DrawDefaultPane((BFLYT.PAN1)pane);
            }
            else
                isRoot = false;

            foreach (var childPane in pane.Childern)
                RenderPanes(childPane, isRoot);

            GL.PopMatrix();
        }

        private void DrawRootPane(BasePane pane)
        {
            GL.LoadIdentity();
            GL.PushMatrix();
            GL.Scale(pane.Scale.X * Camera.Zoom, -pane.Scale.Y * Camera.Zoom, 1);
            GL.Rotate(pane.Rotate.Z, pane.Rotate.X, pane.Rotate.Y, pane.Rotate.Z);
            GL.Translate(pane.Translate.X + Camera.Position.X, pane.Translate.Y + Camera.Position.Y, 0);

            Color color = Color.Black;
            if (SelectedPanes.Contains(pane))
                color = Color.Red;

            CustomRectangle rect = pane.CreateRectangle();

            //Draw a quad which is the backcolor but lighter
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(BackgroundColor.Lighten(10));
            GL.Vertex2(rect.LeftPoint, rect.TopPoint);
            GL.Vertex2(rect.RightPoint, rect.TopPoint);
            GL.Vertex2(rect.RightPoint, rect.BottomPoint);
            GL.Vertex2(rect.LeftPoint, rect.BottomPoint);
            GL.End();

            //Draw outline of root pane
            GL.Begin(PrimitiveType.LineLoop);
            GL.PolygonOffset(0.5f, 2);
            GL.LineWidth(33);
            GL.Color3(color);
            GL.Vertex2(rect.LeftPoint, rect.TopPoint);
            GL.Vertex2(rect.RightPoint, rect.TopPoint);
            GL.Vertex2(rect.RightPoint, rect.BottomPoint);
            GL.Vertex2(rect.LeftPoint, rect.BottomPoint);
            GL.End();

            GL.PopMatrix();
        }

        private void DrawDefaultPane(BFLYT.PAN1 pane)
        {
            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0)
                };

            Color color = Color.White;
            if (SelectedPanes.Contains(pane))
                color = Color.Red;

            Color[] Colors = new Color[] {
                color,
                color,
                color,
                color,
                };

            DrawRectangle(pane.CreateRectangle(), TexCoords, Colors);
        }

        private void DrawPicturePane(BFLYT.PIC1 pane)
        {
            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0)
                };

            Color[] Colors = new Color[] {
                pane.ColorTopLeft.Color,
                pane.ColorTopRight.Color,
                pane.ColorBottomRight.Color,
                pane.ColorBottomLeft.Color,
                };

            GL.Enable(EnableCap.Texture2D);

            if (pane.TexCoords.Length > 0)
            {
                var mat = pane.GetMaterial();
                string textureMap0 = "";
                if (mat.TextureMaps.Count > 0)
                    textureMap0 = mat.GetTexture(0);

                if (Textures.ContainsKey(textureMap0))
                    BindGLTexture(mat.TextureMaps[0], Textures[textureMap0]);

                TexCoords = new Vector2[] {
                        pane.TexCoords[0].TopLeft.ToTKVector2(),
                        pane.TexCoords[0].TopRight.ToTKVector2(),
                        pane.TexCoords[0].BottomRight.ToTKVector2(),
                        pane.TexCoords[0].BottomLeft.ToTKVector2(),
                   };
            }

            DrawRectangle(pane.CreateRectangle(), TexCoords, Colors, false);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void DrawRectangle(CustomRectangle rect, Vector2[] texCoords, Color[] colors, bool useLines = true)
        {
            if (useLines)
            {
                GL.Begin(PrimitiveType.LineLoop);
                GL.Color3(colors[0]);
                GL.Vertex2(rect.LeftPoint, rect.BottomPoint);
                GL.Vertex2(rect.RightPoint, rect.BottomPoint);
                GL.Vertex2(rect.RightPoint, rect.TopPoint);
                GL.Vertex2(rect.LeftPoint, rect.TopPoint);
                GL.End();
            }
            else
            {
                GL.Begin(PrimitiveType.Quads);
                GL.Color3(colors[0]);
                GL.TexCoord2(texCoords[0]);
                GL.Vertex2(rect.LeftPoint, rect.BottomPoint);
                GL.Color3(colors[1]);
                GL.TexCoord2(texCoords[1]);
                GL.Vertex2(rect.RightPoint, rect.BottomPoint);
                GL.Color3(colors[2]);
                GL.TexCoord2(texCoords[2]);
                GL.Vertex2(rect.RightPoint, rect.TopPoint);
                GL.Color3(colors[3]);
                GL.TexCoord2(texCoords[3]);
                GL.Vertex2(rect.LeftPoint, rect.TopPoint);
                GL.End();
            }
        }

        private static void BindGLTexture(BFLYT.TextureRef tex, STGenericTexture texture)
        {
            if (texture.RenderableTex == null || !texture.RenderableTex.GLInitialized)
                texture.LoadOpenGLTexture();

            //If the texture is still not initialized then return
            if (!texture.RenderableTex.GLInitialized)
                return;

            //     GL.ActiveTexture(TextureUnit.Texture0 + texid);
            GL.BindTexture(TextureTarget.Texture2D, texture.RenderableTex.TexID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ConvertTextureWrap(tex.WrapModeU));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ConvertTextureWrap(tex.WrapModeV));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ConvertMagFilterMode(tex.MaxFilterMode));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ConvertMinFilterMode(tex.MinFilterMode));
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, 0.0f);
        }

        private static int ConvertTextureWrap(BFLYT.TextureRef.WrapMode wrapMode)
        {
            switch (wrapMode)
            {
                case BFLYT.TextureRef.WrapMode.Clamp: return (int)TextureWrapMode.Clamp;
                case BFLYT.TextureRef.WrapMode.Mirror: return (int)TextureWrapMode.MirroredRepeat;
                case BFLYT.TextureRef.WrapMode.Repeat: return (int)TextureWrapMode.Repeat;
                default: return (int)TextureWrapMode.Clamp;
            }
        }

        private static int ConvertMagFilterMode(BFLYT.TextureRef.FilterMode filterMode)
        {
            switch (filterMode)
            {
                case BFLYT.TextureRef.FilterMode.Linear: return (int)TextureMagFilter.Linear;
                case BFLYT.TextureRef.FilterMode.Near: return (int)TextureMagFilter.Nearest;
                default: return (int)BFLYT.TextureRef.FilterMode.Linear;
            }
        }

        private static int ConvertMinFilterMode(BFLYT.TextureRef.FilterMode filterMode)
        {
            switch (filterMode)
            {
                case BFLYT.TextureRef.FilterMode.Linear: return (int)TextureMinFilter.Linear;
                case BFLYT.TextureRef.FilterMode.Near: return (int)TextureMinFilter.Nearest;
                default: return (int)BFLYT.TextureRef.FilterMode.Linear;
            }
        }

        private void DrawBackground()
        {
            if (backgroundTex == null)
            {
            /*    backgroundTex = RenderableTex.FromBitmap(Properties.Resources.GridBackground);
                backgroundTex.TextureWrapR = TextureWrapMode.Repeat;
                backgroundTex.TextureWrapT = TextureWrapMode.Repeat;


                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, backgroundTex.TexID);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)backgroundTex.TextureWrapR);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)backgroundTex.TextureWrapT);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)backgroundTex.TextureMagFilter);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)backgroundTex.TextureMinFilter);

                float UVscale = 15;

                int PanelWidth = 9000;
                int PanelWHeight = 9000;

                Vector2 scaleCenter = new Vector2(0.5f, 0.5f);

                Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0),
            };

                for (int i = 0; i < TexCoords.Length; i++)
                    TexCoords[i] = (TexCoords[i] - scaleCenter) * 20 + scaleCenter;

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
                GL.PushMatrix();
                GL.Scale(1, 1, 1);
                GL.Translate(0, 0, 0);

                GL.Color4(Color.White);

                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(TexCoords[0]);
                GL.Vertex3(PanelWidth, PanelWHeight, 0);
                GL.TexCoord2(TexCoords[1]);
                GL.Vertex3(-PanelWidth, PanelWHeight, 0);
                GL.TexCoord2(TexCoords[2]);
                GL.Vertex3(-PanelWidth, -PanelWHeight, 0);
                GL.TexCoord2(TexCoords[3]);
                GL.Vertex3(PanelWidth, -PanelWHeight, 0);
                GL.End();

                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.PopMatrix();*/
            }
        }

        public void UpdateBackgroundColor(Color color)
        {
            BackgroundColor = color;
            glControl1.Invalidate();
        }

        private void DrawXyLines()
        {
            GL.LoadIdentity();
            GL.PushMatrix();
            GL.Scale(1 * Camera.Zoom, -1 * Camera.Zoom, 1);
            GL.Translate(Camera.Position.X, Camera.Position.Y, 0);

            int lineLength = 20;

            GL.Color3(Color.Green);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(0, 0);
            GL.Vertex2(0, lineLength);
            GL.End();

            GL.Color3(Color.Red);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(0, 0);
            GL.Vertex2(lineLength, 0);
            GL.End();

            GL.PopMatrix();
        }

        private void DrawGrid()
        {
            var size = 40;
            var amount = 300;

            GL.LoadIdentity();
            GL.PushMatrix();
            GL.Scale(1 * Camera.Zoom, -1 * Camera.Zoom, 1);
            GL.Translate(Camera.Position.X, Camera.Position.Y, 0);
            GL.Rotate(90, new Vector3(1,0,0));

            GL.LineWidth(0.001f);
            GL.Color3(BackgroundColor.Darken(20));
            GL.Begin(PrimitiveType.Lines);

            int squareGridCounter = 0;
            for (var i = -amount; i <= amount; i++)
            {
                if (squareGridCounter > 5)
                {
                    squareGridCounter = 0;
                    GL.LineWidth(33f);
                }
                else
                {
                    GL.LineWidth(0.001f);
                }

                GL.Vertex3(new Vector3(-amount * size, 0f, i * size));
                GL.Vertex3(new Vector3(amount * size, 0f, i * size));
                GL.Vertex3(new Vector3(i * size, 0f, -amount * size));
                GL.Vertex3(new Vector3(i * size, 0f, amount * size));

                squareGridCounter++;
            }
            GL.End();
            GL.Color3(Color.Transparent);
            GL.PopAttrib();
            GL.Enable(EnableCap.Texture2D);
            GL.PopMatrix();
        }

        private bool mouseHeldDown = false;
        private Point originMouse;
        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseHeldDown = true;
                originMouse = e.Location;
                glControl1.Invalidate();
            }
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseHeldDown = false;
                glControl1.Invalidate();
            }
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHeldDown)
            {
                var pos = new Vector2(e.Location.X - originMouse.X, e.Location.Y - originMouse.Y);
                Camera.Position.X += pos.X;
                Camera.Position.Y -= pos.Y;

                originMouse = e.Location;

                glControl1.Invalidate();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0 && Camera.Zoom > 0)
                Camera.Zoom += 0.1f;
            if (e.Delta < 0 && Camera.Zoom < 10 && Camera.Zoom > 0.1)
                Camera.Zoom -= 0.1f;

            glControl1.Invalidate();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }
    }
}
