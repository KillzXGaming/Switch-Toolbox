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
using LayoutBXLYT.Cafe;

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
        private Dictionary<string, STGenericTexture> Textures;

        private void glControl1_Load(object sender, EventArgs e)
        {
        }

        public void ResetCamera()
        {
            Camera = new Camera2D();
        }

        public void ResetLayout(BxlytHeader bxlyt)
        {
            LayoutFile = bxlyt;
            UpdateViewport();
        }

        public LayoutViewer(BxlytHeader bxlyt, Dictionary<string, STGenericTexture> textures)
        {
            InitializeComponent();
            LayoutFile = bxlyt;
            Text = bxlyt.FileName;

            Textures = textures;
            if (bxlyt.Textures.Count > 0)
            {
                Textures = bxlyt.GetTextures;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (LayoutEditor.IsSaving)
            {
                base.OnFormClosing(e);
                return;
            }

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

        public bool UseOrtho = true;
        private Color BackgroundColor => Runtime.LayoutEditor.BackgroundColor;
        private void OnRender()
        {
            if (LayoutFile == null) return;

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            if (UseOrtho)
            {
                GL.Ortho(-(glControl1.Width / 2.0f), glControl1.Width / 2.0f, glControl1.Height / 2.0f, -(glControl1.Height / 2.0f), -10000, 10000);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
            }
            else
            {
                var cameraPosition = new Vector3(Camera.Position.X, Camera.Position.Y, -(Camera.Zoom * 500));
                var perspectiveMatrix = Matrix4.CreateTranslation(cameraPosition) * Matrix4.CreatePerspectiveFieldOfView(1.3f, glControl1.Width / (float)glControl1.Height, 0.01f, 100000);
                GL.LoadMatrix(ref perspectiveMatrix);
                GL.MatrixMode(MatrixMode.Modelview);
            }

            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

          //  GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Always, 0f);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            if (UseOrtho)
            {
                GL.PushMatrix();
                GL.Scale(1 * Camera.Zoom, -1 * Camera.Zoom, 1);
                GL.Translate(Camera.Position.X, Camera.Position.Y, 0);
            }

            DrawRootPane(LayoutFile.RootPane);
            DrawGrid();
            DrawXyLines();

            GL.BindTexture(TextureTarget.Texture2D, 0);

            RenderPanes(LayoutFile.RootPane, true, 255, false, null, 0);

            if (UseOrtho)
                GL.PopMatrix();

            GL.UseProgram(0);

            glControl1.SwapBuffers();
        }

        private void RenderPanes(BasePane pane, bool isRoot, byte parentAlpha, bool parentAlphaInfluence, BasePane partPane = null, int stage = 0)
        {
            if (!pane.DisplayInEditor)
                return;

            GL.PushMatrix();

            //Check XY rotation and draw the pane before it was rotated
            bool isRotatedXY = pane.Rotate.X != 0 || pane.Rotate.Y != 0;
            if (isRotatedXY && SelectedPanes.Contains(pane))
            {
                GL.PushMatrix();
                GL.Translate(pane.Translate.X, pane.Translate.Y, 0);
                GL.Rotate(pane.Rotate.Z, 0, 0, 1);
                GL.Scale(pane.Scale.X, pane.Scale.Y, 1);

                if (pane is BFLYT.PAN1)
                    DrawDefaultPane((BFLYT.PAN1)pane);
                else if (pane is BCLYT.PAN1)
                    DrawDefaultPane((BCLYT.PAN1)pane);
                else if (pane is BRLYT.PAN1)
                    DrawDefaultPane((BRLYT.PAN1)pane);

                GL.PopMatrix();
            }

            if (partPane != null)
            {
                var translate = partPane.Translate + pane.Translate;
                var scale = partPane.Scale * pane.Scale;
                var rotate = partPane.Rotate + pane.Rotate;

                GL.Translate(translate.X + translate.X, translate.Y, 0);
                GL.Rotate(rotate.X, 1, 0, 0);
                GL.Rotate(rotate.Y, 0, 1, 0);
                GL.Rotate(rotate.Z, 0, 0, 1);
                GL.Scale(scale.X, scale.Y, 1);
            }
            else
            {
                GL.Translate(pane.Translate.X, pane.Translate.Y, 0);
                GL.Rotate(pane.Rotate.X, 1, 0, 0);
                GL.Rotate(pane.Rotate.Y, 0, 1, 0);
                GL.Rotate(pane.Rotate.Z, 0, 0, 1);
                GL.Scale(pane.Scale.X, pane.Scale.Y, 1);
            }

            byte effectiveAlpha = (byte)(parentAlpha == 255 ? pane.Alpha : (pane.Alpha * parentAlpha) / 255);
            if (!parentAlphaInfluence)
                effectiveAlpha = pane.Alpha;

            parentAlphaInfluence = parentAlphaInfluence || pane.InfluenceAlpha;

            if (!isRoot)
            {
                if (pane is BFLYT.PIC1 || pane is BCLYT.PIC1 || pane is BRLYT.PIC1)
                    BxlytToGL.DrawPictureBox(pane, effectiveAlpha, Textures);
                else if (pane is BFLYT.BND1 || pane is BCLYT.BND1 || pane is BRLYT.BND1)
                    BxlytToGL.DrawBoundryPane(pane, effectiveAlpha, SelectedPanes);
                else if (pane is BFLYT.WND1)
                    DrawWindowPane((BFLYT.WND1)pane, effectiveAlpha);
                else if (pane is BFLYT.PRT1)
                    DrawPartsPane((BFLYT.PRT1)pane, effectiveAlpha, parentAlphaInfluence);
                else
                    DrawDefaultPane(pane);
            }
            else
                isRoot = false;

            byte childAlpha = pane.InfluenceAlpha || parentAlphaInfluence ? effectiveAlpha : byte.MaxValue;
            foreach (var childPane in pane.Childern)
                RenderPanes(childPane, isRoot, childAlpha, parentAlphaInfluence, partPane);

            GL.PopMatrix();
        }

        private void DrawRootPane(BasePane pane)
        {
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
        }

        private void DrawDefaultPane(BasePane pane)
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

            BxlytToGL.DrawRectangle(pane.Rectangle, TexCoords, Colors);
        }

        private void DrawPartsPane(BFLYT.PRT1 pane, byte effectiveAlpha, bool parentInfluenceAlpha)
        {
            pane.UpdateTextureData(this.Textures);
            var partPane = pane.GetExternalPane();
            if (partPane != null)
                RenderPanes(partPane, true, effectiveAlpha, parentInfluenceAlpha);
            else
                DrawDefaultPane(pane);

            if (pane.Properties != null)
            {
                foreach (var prop in pane.Properties)
                {
                    if (prop.Property != null)
                    {
                        RenderPanes(prop.Property, false, effectiveAlpha, parentInfluenceAlpha || pane.InfluenceAlpha);
                    }
                }
            }
        }

        private void DrawWindowPane(BFLYT.WND1 pane, byte effectiveAlpha)
        {
            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0)
                };

            Color[] Colors = new Color[] {
                pane.Content.ColorTopLeft.Color,
                pane.Content.ColorTopRight.Color,
                pane.Content.ColorBottomRight.Color,
                pane.Content.ColorBottomLeft.Color,
                };


            float frameLeft = 0;
            float frameTop = 0;
            float frameRight = 0;
            float frameBottom = 0;
            if (pane.FrameCount == 1)
            {
            }
            else if (pane.FrameCount == 4)
            {

            }
            else if (pane.FrameCount == 8)
            {

            }

            var mat = pane.Content.Material;
            if (mat.Shader == null)
            {
                mat.Shader = new BflytShader(mat);
                mat.Shader.Compile();
            }

            mat.Shader.Enable();
            ((BflytShader)mat.Shader).SetMaterials(Textures);
            if (pane.Content.TexCoords.Count > 0)
            {
                TexCoords = new Vector2[] {
                        pane.Content.TexCoords[0].TopLeft.ToTKVector2(),
                        pane.Content.TexCoords[0].TopRight.ToTKVector2(),
                        pane.Content.TexCoords[0].BottomRight.ToTKVector2(),
                        pane.Content.TexCoords[0].BottomLeft.ToTKVector2(),
                   };
            }

            BxlytToGL.DrawRectangle(pane.Rectangle, TexCoords, Colors, false, effectiveAlpha);

            mat.Shader.Disable();

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.PopAttrib();
        }

        private void DrawPicturePane(BCLYT.PIC1 pane, byte effectiveAlpha)
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

            var mat = pane.Material;
            if (pane.TexCoords.Length > 0)
            {
                string textureMap0 = "";
                if (mat.TextureMaps.Count > 0)
                    textureMap0 = mat.GetTexture(0);

              //  if (Textures.ContainsKey(textureMap0))
                  //  BindGLTexture(mat.TextureMaps[0], Textures[textureMap0]);

                TexCoords = new Vector2[] {
                        pane.TexCoords[0].TopLeft.ToTKVector2(),
                        pane.TexCoords[0].TopRight.ToTKVector2(),
                        pane.TexCoords[0].BottomRight.ToTKVector2(),
                        pane.TexCoords[0].BottomLeft.ToTKVector2(),
                   };
            }

            BxlytToGL.DrawRectangle(pane.Rectangle, TexCoords, Colors, false, effectiveAlpha);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void DrawPicturePane(BRLYT.PIC1 pane, byte effectiveAlpha)
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

            if (pane.TexCoords.Length > 0)
            {
                var mat = pane.GetMaterial();
                string textureMap0 = "";
                if (mat.TextureMaps.Count > 0)
                    textureMap0 = mat.GetTexture(0);

                //  if (Textures.ContainsKey(textureMap0))
                //  BindGLTexture(mat.TextureMaps[0], Textures[textureMap0]);
                if (Runtime.LayoutEditor.Shading == Runtime.LayoutEditor.DebugShading.UVTestPattern)
                    GL.BindTexture(TextureTarget.Texture2D, RenderTools.uvTestPattern.RenderableTex.TexID);

                TexCoords = new Vector2[] {
                        pane.TexCoords[0].TopLeft.ToTKVector2(),
                        pane.TexCoords[0].TopRight.ToTKVector2(),
                        pane.TexCoords[0].BottomRight.ToTKVector2(),
                        pane.TexCoords[0].BottomLeft.ToTKVector2(),
                   };
            }

            BxlytToGL.DrawRectangle(pane.Rectangle, TexCoords, Colors, false, effectiveAlpha);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void DrawPicturePane(BFLYT.PIC1 pane, byte effectiveAlpha, int stage = 0)
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

            var mat = pane.Material;
            if (mat.Shader == null)
            {
                mat.Shader = new BflytShader(mat);
                mat.Shader.Compile();
            }

             mat.Shader.Enable();
            ((BflytShader)mat.Shader).SetMaterials(Textures);

            if (pane.TexCoords.Length > 0)
            {
                TexCoords = new Vector2[] {
                        pane.TexCoords[0].TopLeft.ToTKVector2(),
                        pane.TexCoords[0].TopRight.ToTKVector2(),
                        pane.TexCoords[0].BottomRight.ToTKVector2(),
                        pane.TexCoords[0].BottomLeft.ToTKVector2(),
                   };
            }

            BxlytToGL.DrawRectangle(pane.Rectangle, TexCoords, Colors, false, effectiveAlpha);

            mat.Shader.Disable();

            GL.BindTexture(TextureTarget.Texture2D,  0);
            GL.PopAttrib();
        }

        private static BlendingFactor ConvertBlendFactor(BlendMode.GX2BlendFactor blendFactor)
        {
            switch (blendFactor)
            {
                case BlendMode.GX2BlendFactor.DestAlpha: return BlendingFactor.DstAlpha;
                case BlendMode.GX2BlendFactor.DestColor: return BlendingFactor.DstColor;
                case BlendMode.GX2BlendFactor.DestInvAlpha: return BlendingFactor.OneMinusDstAlpha;
                case BlendMode.GX2BlendFactor.DestInvColor: return BlendingFactor.OneMinusDstColor;
                case BlendMode.GX2BlendFactor.Factor0: return BlendingFactor.Zero;
                case BlendMode.GX2BlendFactor.Factor1: return BlendingFactor.One;
                case BlendMode.GX2BlendFactor.SourceAlpha: return BlendingFactor.SrcAlpha;
                case BlendMode.GX2BlendFactor.SourceColor: return BlendingFactor.SrcColor;
                case BlendMode.GX2BlendFactor.SourceInvAlpha: return BlendingFactor.OneMinusSrcAlpha;
                case BlendMode.GX2BlendFactor.SourceInvColor: return BlendingFactor.OneMinusSrcColor;
                default: return BlendingFactor.Zero;
            }
        }

        private static LogicOp ConvertLogicOperation(BlendMode.GX2LogicOp blendOp)
        {
            switch (blendOp)
            {
                case BlendMode.GX2LogicOp.And: return LogicOp.And;
                case BlendMode.GX2LogicOp.Clear: return LogicOp.Clear;
                case BlendMode.GX2LogicOp.Copy: return LogicOp.Copy;
                case BlendMode.GX2LogicOp.Equiv: return LogicOp.Equiv;
                case BlendMode.GX2LogicOp.Inv: return LogicOp.Invert;
                case BlendMode.GX2LogicOp.Nand: return LogicOp.Nand;
                case BlendMode.GX2LogicOp.NoOp: return LogicOp.Noop;
                case BlendMode.GX2LogicOp.Nor: return LogicOp.Nor;
                case BlendMode.GX2LogicOp.Or: return LogicOp.Or;
                case BlendMode.GX2LogicOp.RevAnd: return LogicOp.AndReverse;
                case BlendMode.GX2LogicOp.RevOr: return LogicOp.OrReverse;
                case BlendMode.GX2LogicOp.Set: return LogicOp.Set;
                case BlendMode.GX2LogicOp.Xor: return LogicOp.Xor;
                case BlendMode.GX2LogicOp.Disable:
                    GL.Disable(EnableCap.ColorLogicOp);
                    return LogicOp.Noop;
                default: return LogicOp.Noop;

            }
        }

        private static BlendEquationMode ConvertBlendOperation(BlendMode.GX2BlendOp blendOp)
        {
            switch (blendOp)
            {
                case BlendMode.GX2BlendOp.Add: return BlendEquationMode.FuncAdd;
                case BlendMode.GX2BlendOp.ReverseSubtract: return BlendEquationMode.FuncReverseSubtract;
                case BlendMode.GX2BlendOp.SelectMax: return BlendEquationMode.Max;
                case BlendMode.GX2BlendOp.SelectMin: return BlendEquationMode.Min;
                case BlendMode.GX2BlendOp.Subtract: return BlendEquationMode.FuncSubtract;
                case BlendMode.GX2BlendOp.Disable:
                    GL.Disable(EnableCap.Blend);
                    return BlendEquationMode.FuncAdd;
                default: return BlendEquationMode.FuncAdd;
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
            Runtime.LayoutEditor.BackgroundColor = color;
            glControl1.Invalidate();
            Config.Save();
        }

        private void DrawXyLines()
        {
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
        }

        private void DrawGrid()
        {
            var size = 40;
            var amount = 300;

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

                GL.Vertex2(new Vector2(-amount * size, i * size));
                GL.Vertex2(new Vector2(amount * size, i * size));
                GL.Vertex2(new Vector2(i * size, -amount * size));
                GL.Vertex2(new Vector2(i * size, amount * size));

                squareGridCounter++;
            }
            GL.End();
            GL.Color3(Color.Transparent);
            GL.PopAttrib();
        }

        private bool mouseHeldDown = false;
        private bool isPicked = false;
        private Point originMouse;
        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            SelectedPanes.Clear();

            //Pick an object for moving
            if (Control.ModifierKeys == Keys.Alt && e.Button == MouseButtons.Left)
            {
                BasePane hitPane = null;
                SearchHit(LayoutFile.RootPane, e.X, e.Y, ref hitPane);
                if (hitPane != null)
                {
                    SelectedPanes.Add(hitPane);
                    UpdateViewport();

                    isPicked = true;
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                mouseHeldDown = true;
                originMouse = e.Location;

                BasePane hitPane = null;
                foreach (var child in LayoutFile.RootPane.Childern)
                    SearchHit(child, e.X, e.Y, ref hitPane);
                Console.WriteLine($"Has Hit " + hitPane != null);
                if (hitPane != null)
                {
                    SelectedPanes.Add(hitPane);
                    UpdateViewport();
                }

                glControl1.Invalidate();
            }

            Console.WriteLine("SelectedPanes " + SelectedPanes.Count);
        }

        private void SearchHit(BasePane pane, int X, int Y, ref BasePane SelectedPane)
        {
            if (pane.IsHit(X, Y)){
                SelectedPane = pane;
                return;
            }

            foreach (var childPane in pane.Childern)
                 SearchHit(childPane, X, Y, ref SelectedPane);
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseHeldDown = false;
                isPicked = false;
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

        protected override void OnClosed(EventArgs e)
        {
            foreach (var tex in LayoutFile.Textures)
            {
                if (Textures.ContainsKey(tex))
                {
                    Textures[tex].DisposeRenderable();
                    Textures.Remove(tex);
                }
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (UseOrtho)
            {
                if (e.Delta > 0 && Camera.Zoom > 0)
                    Camera.Zoom += 0.1f;
                if (e.Delta < 0 && Camera.Zoom < 100 && Camera.Zoom > 0.1)
                    Camera.Zoom -= 0.1f;
            }
            else
            {
                if (e.Delta > 0 && Camera.Zoom > 0.1)
                    Camera.Zoom -= 0.1f;
                if (e.Delta < 0 && Camera.Zoom < 100 && Camera.Zoom > 0)
                    Camera.Zoom += 0.1f;
            }

            glControl1.Invalidate();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }
    }
}
