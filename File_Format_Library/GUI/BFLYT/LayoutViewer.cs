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
    public partial class LayoutViewer : LayoutControlDocked
    {
        public List<BasePane> SelectedPanes = new List<BasePane>();

        public Camera2D Camera = new Camera2D();

        public class Camera2D
        {
            public Matrix4 ModelViewMatrix = Matrix4.Identity;
            public float Zoom = 1;
            public Vector2 Position;
        }

        private LayoutEditor ParentEditor;

        private RenderableTex backgroundTex;
        public BxlytHeader LayoutFile;
        public List<BxlytHeader> LayoutFiles = new List<BxlytHeader>();
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

        public GLControl GetGLControl() => glControl1;

        public LayoutViewer(LayoutEditor editor, BxlytHeader bxlyt, Dictionary<string, STGenericTexture> textures)
        {
            InitializeComponent();

            ParentEditor = editor;

            Text = bxlyt.FileName;

            Textures = textures;
            LoadLayout(bxlyt);
        }

        public void LoadLayout(BxlytHeader bxlyt)
        {
            LayoutFile = bxlyt;
            LayoutFiles.Add(bxlyt);

            if (bxlyt.Textures.Count > 0)
            {
                var textures = bxlyt.GetTextures;
                foreach (var tex in textures)
                    if (!Textures.ContainsKey(tex.Key))
                        Textures.Add(tex.Key, tex.Value);
            }
        }

        public override void OnControlClosing()
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

        private BxlytShader GlobalShader;
        public bool GameWindow = false;
        public bool UseOrtho => Runtime.LayoutEditor.UseOrthographicView;
        private Color BackgroundColor => Runtime.LayoutEditor.BackgroundColor;
        private void OnRender()
        {
            if (LayoutFile == null) return;

            if (!GameWindow)
            {
                if (ParentEditor != null)
                    ParentEditor.GamePreviewWindow?.UpdateViewport();
            }

            if (GameWindow)
                RenderGameWindow();
            else
                RenderEditor();
        }

        private void RenderGameWindow()
        {
            int WindowWidth = (int)LayoutFile.RootPane.Width;
            int WindowHeight = (int)LayoutFile.RootPane.Height;

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            if (UseOrtho)
            {
                float halfW = WindowWidth, halfH = WindowHeight;
                var orthoMatrix = Matrix4.CreateOrthographic(halfW, halfH, -10000, 10000);
                GL.LoadMatrix(ref orthoMatrix);
                GL.MatrixMode(MatrixMode.Modelview);
                Camera.ModelViewMatrix = orthoMatrix;
            }
            else
            {
                var cameraPosition = new Vector3(0, 0, -600);
                var perspectiveMatrix = Matrix4.CreateTranslation(cameraPosition) * Matrix4.CreatePerspectiveFieldOfView(1.3f, WindowWidth / WindowHeight, 0.01f, 100000);
                GL.LoadMatrix(ref perspectiveMatrix);
                GL.MatrixMode(MatrixMode.Modelview);
                Camera.ModelViewMatrix = perspectiveMatrix;
            }

            RenderScene();
        }

        private void RenderEditor()
        {
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            if (UseOrtho)
            {
                float halfW = glControl1.Width / 2.0f, halfH = glControl1.Height / 2.0f;
                var orthoMatrix = Matrix4.CreateOrthographic(halfW, halfH, -10000, 10000);
                GL.LoadMatrix(ref orthoMatrix);
                GL.MatrixMode(MatrixMode.Modelview);
                Camera.ModelViewMatrix = orthoMatrix;
            }
            else
            {
                var cameraPosition = new Vector3(Camera.Position.X, Camera.Position.Y, -(Camera.Zoom * 500));
                var perspectiveMatrix = Matrix4.CreateTranslation(cameraPosition) * Matrix4.CreatePerspectiveFieldOfView(1.3f, glControl1.Width / glControl1.Height, 0.01f, 100000);
                GL.LoadMatrix(ref perspectiveMatrix);
                GL.MatrixMode(MatrixMode.Modelview);
                Camera.ModelViewMatrix = perspectiveMatrix;
            }

            RenderScene();
        }

        private void RenderScene()
        {
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
            GL.BlendEquation(BlendEquationMode.FuncAdd);

            if (UseOrtho && !GameWindow)
            {
                GL.PushMatrix();
                GL.Scale(Camera.Zoom, Camera.Zoom, 1);
                GL.Translate(Camera.Position.X, Camera.Position.Y, 0);
            }

            if (!GameWindow)
            {
                DrawRootPane(LayoutFile.RootPane);
                DrawGrid();
                DrawXyLines();
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);

            if (GlobalShader == null)
            {
                GlobalShader = new BxlytShader();
                GlobalShader.Compile();
            }

            foreach (var layout in LayoutFiles)
                RenderPanes(GlobalShader, layout.RootPane, true, 255, false, null, 0);

            if (UseOrtho)
                GL.PopMatrix();

            GL.UseProgram(0);

            glControl1.SwapBuffers();
        }

        private bool test = true;
        private void RenderPanes(BxlytShader shader, BasePane pane, bool isRoot, byte parentAlpha, bool parentAlphaInfluence, BasePane partPane = null, int stage = 0)
        {
            if (!pane.DisplayInEditor || !pane.animController.Visibile)
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

                DrawDefaultPane(shader, pane, true);

                GL.PopMatrix();
            }


            var translate = pane.Translate;
            var rotate = pane.Rotate;
            var scale = pane.Scale;

            foreach (var animItem in pane.animController.PaneSRT)
            {
                switch (animItem.Key)
                {
                    case LPATarget.RotateX: rotate.X = animItem.Value; break;
                    case LPATarget.RotateY: rotate.Y = animItem.Value; break;
                    case LPATarget.RotateZ: rotate.Z = animItem.Value; break;
                    case LPATarget.ScaleX: scale.X = animItem.Value; break;
                    case LPATarget.ScaleY: scale.Y = animItem.Value; break;
                    case LPATarget.TranslateX: translate.X = animItem.Value; break;
                    case LPATarget.TranslateY: translate.Y = animItem.Value; break;
                    case LPATarget.TranslateZ: translate.Z = animItem.Value; break;
                }
            }

            if (partPane != null)
            {
                translate = translate + pane.Translate;
                scale = scale * pane.Scale;
                rotate = rotate + pane.Rotate;
            }

            GL.Translate(translate.X, translate.Y, 0);

            //Rotate normally unless the object uses shaders/materials
            //Rotation matrix + shaders works accurately with X/Y rotation axis
            //Todo, do everything by shaders
            bool HasMaterials = pane is IWindowPane || pane is IPicturePane || pane is BFLYT.PRT1;
            if (!HasMaterials)
            {
                GL.Rotate(rotate.X, 1, 0, 0);
                GL.Rotate(rotate.Y, 0, 1, 0);
                GL.Rotate(rotate.Z, 0, 0, 1);
            }

            GL.Scale(scale.X, scale.Y, 1);

            byte alpha = pane.Alpha;
            if (pane.animController.PaneVertexColors.ContainsKey(LVCTarget.PaneAlpha))
                alpha = (byte)pane.animController.PaneVertexColors[LVCTarget.PaneAlpha];

            byte effectiveAlpha = (byte)(parentAlpha == 255 ? alpha : (alpha * parentAlpha) / 255);
            if (!parentAlphaInfluence)
                effectiveAlpha = alpha;

            parentAlphaInfluence = parentAlphaInfluence || pane.InfluenceAlpha;

            if (!isRoot)
            {
                if (pane is IPicturePane)
                    BxlytToGL.DrawPictureBox(pane, GameWindow, effectiveAlpha, Textures);
                else if (pane is IWindowPane)
                    BxlytToGL.DrawWindowPane(pane, GameWindow, effectiveAlpha, Textures);
                else if (pane is IBoundryPane)
                    BxlytToGL.DrawBoundryPane(pane, GameWindow, effectiveAlpha, SelectedPanes);
                else if (pane is ITextPane && Runtime.LayoutEditor.DisplayTextPane)
                {
                    var textPane = (ITextPane)pane;
                    Bitmap bitmap = null;
                    if (textPane.RenderableFont == null)
                    {
                        if (pane is BFLYT.TXT1)
                        {
                            foreach (var fontFile in FirstPlugin.PluginRuntime.BxfntFiles)
                            {
                                if (Utils.CompareNoExtension(fontFile.Name, textPane.FontName))
                                {
                                    bitmap = fontFile.GetBitmap(textPane.Text, false, pane);
                                }
                            }
                        }
                    }
                    if (bitmap != null)
                        BxlytToGL.DrawTextbox(pane, GameWindow, bitmap, effectiveAlpha, Textures, SelectedPanes, textPane.RenderableFont == null);
                    else
                        DrawDefaultPane(shader, pane);
                }
                else if (pane is BFLYT.SCR1)
                    BxlytToGL.DrawScissorPane(pane, GameWindow, effectiveAlpha, SelectedPanes);
                else if (pane is BFLYT.ALI1)
                    BxlytToGL.DrawAlignmentPane(pane, GameWindow, effectiveAlpha, SelectedPanes);
                else if (pane is BFLYT.PRT1)
                    DrawPartsPane(shader, (BFLYT.PRT1)pane, effectiveAlpha, parentAlphaInfluence);
                else
                    DrawDefaultPane(shader, pane);
            }
            else
                isRoot = false;

            byte childAlpha = pane.InfluenceAlpha || parentAlphaInfluence ? effectiveAlpha : byte.MaxValue;
            foreach (var childPane in pane.Childern)
                RenderPanes(shader, childPane, isRoot, childAlpha, parentAlphaInfluence, partPane);

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

        private void DrawDefaultPane(BxlytShader shader, BasePane pane, bool isSelectionBox = false)
        {
            if (!Runtime.LayoutEditor.DisplayNullPane && !isSelectionBox || GameWindow || Runtime.LayoutEditor.IsGamePreview)
                return;

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

            BxlytToGL.DrawRectangle(pane, GameWindow, pane.Rectangle, TexCoords, Colors);
        }

        private void DrawPartsPane(BxlytShader shader, BFLYT.PRT1 pane, byte effectiveAlpha, bool parentInfluenceAlpha)
        {
            pane.UpdateTextureData(this.Textures);
            var partPane = pane.GetExternalPane();
            if (partPane != null)
                RenderPanes(shader,partPane, true, effectiveAlpha, parentInfluenceAlpha);
            else
                DrawDefaultPane(shader, pane);

            if (pane.Properties != null)
            {
                foreach (var prop in pane.Properties)
                {
                    if (prop.Property != null)
                    {
                        RenderPanes(shader,prop.Property, false, effectiveAlpha, parentInfluenceAlpha || pane.InfluenceAlpha);
                    }
                }
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
            if (GameWindow || Runtime.LayoutEditor.IsGamePreview)
                return;

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
            if (!Runtime.LayoutEditor.DisplayGrid)
                return;

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
            if (pane.IsHit(X, Y))
            {
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
