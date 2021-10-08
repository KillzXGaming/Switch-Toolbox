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
using Toolbox.Library.Forms;
using Toolbox.Library;
using Toolbox.Library.Rendering;
using Toolbox.Library.IO;

namespace LayoutBXLYT
{
    public partial class LayoutViewer : LayoutControlDocked
    {
        public LayoutUndoManager UndoManger = new LayoutUndoManager();

        public List<BasePane> SelectedPanes = new List<BasePane>();

        public Camera2D Camera = new Camera2D();

        public class Camera2D
        {
            public Matrix4 ModelViewMatrix => ModelMatrix * ViewMatrix * ProjectionMatrix;

            public Matrix4 ProjectionMatrix = Matrix4.Identity;
            public Matrix4 ViewMatrix = Matrix4.Identity;
            public Matrix4 ModelMatrix = Matrix4.Identity;

            public float Zoom = 1;
            public Vector2 Position;
        }

        private LayoutEditor ParentEditor;

        private List<BasePane> CopiedPanes = new List<BasePane>();

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

        public Dictionary<string, STGenericTexture> GetTextures()
        {
            return Textures;
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
                foreach (var tex in bxlyt.Textures)
                {
                    //Some games use different cases for archives
                    string lowerCase = tex.ToLower();
                    if (textures.ContainsKey(tex))
                        AddTexture(tex, textures[tex]);
                    else if (textures.ContainsKey(lowerCase))
                        AddTexture(tex, textures[lowerCase]);
                }
            }
        }

        private void AddTexture(string name, STGenericTexture tex) {
            if (!Textures.ContainsKey(name))
                Textures.Add(name, tex);
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
            {
                RenderGameWindow();
                RenderScene();
            }
            else
            {
                RenderEditor();
                RenderScene();
            }
        }

        private void RenderGameWindow()
        {
            glControl1.MakeCurrent();

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
                Camera.ProjectionMatrix = orthoMatrix;
            }
            else
            {
                var cameraPosition = new Vector3(0, 0, -600);
                var perspectiveMatrix = Matrix4.CreateTranslation(cameraPosition) * Matrix4.CreatePerspectiveFieldOfView(0.785398f, WindowWidth / WindowHeight, 0.01f, 100000);
                GL.LoadMatrix(ref perspectiveMatrix);
                GL.MatrixMode(MatrixMode.Modelview);
                Camera.ProjectionMatrix = perspectiveMatrix;
            }

            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        private void RenderEditor()
        {
            glControl1.MakeCurrent();

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            if (UseOrtho)
            {
                float halfW = glControl1.Width / 2.0f, halfH = glControl1.Height / 2.0f;
                var orthoMatrix = Matrix4.CreateOrthographic(halfW, halfH, -10000, 10000);
                GL.LoadMatrix(ref orthoMatrix);
                GL.MatrixMode(MatrixMode.Modelview);
                Camera.ProjectionMatrix = orthoMatrix;
            }
            else
            {
                var cameraPosition = new Vector3(Camera.Position.X, Camera.Position.Y, -(Camera.Zoom * 500));
                var perspectiveMatrix = Matrix4.CreateTranslation(cameraPosition) * Matrix4.CreatePerspectiveFieldOfView(1.3f, glControl1.Width / glControl1.Height, 0.01f, 100000);
                GL.LoadMatrix(ref perspectiveMatrix);
                GL.MatrixMode(MatrixMode.Modelview);
                Camera.ProjectionMatrix = perspectiveMatrix;
            }

            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (UseOrtho && !GameWindow)
            {
                GL.PushMatrix();
                GL.Scale(Camera.Zoom, Camera.Zoom, 1);
                GL.Translate(Camera.Position.X, Camera.Position.Y, 0);

                Camera.ViewMatrix *= Matrix4.CreateScale(Camera.Zoom, Camera.Zoom, 1) *
                                   Matrix4.CreateTranslation(Camera.Position.X, Camera.Position.Y, 0);
            }
        }

        private void RenderScene(bool showSelectionBox = false)
        {
            //  GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Always, 0f);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Disable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BlendEquation(BlendEquationMode.FuncAdd);

            if (!GameWindow)
            {
                DrawRootPane(LayoutFile.RootPane);
                DrawGrid();
                DrawXyLines();
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Texture2D);

            if (GlobalShader == null)
            {
                GlobalShader = new BxlytShader();
                GlobalShader.Compile();
            }

            bool PreviewHitbox = false;
            if (PreviewHitbox)
            {
                foreach (var file in LayoutFiles)
                {
                    foreach (var pane in file.PaneLookup.Values)
                    {
                        if (!pane.Visible || !pane.DisplayInEditor && !pane.IsRoot)
                            continue;

                        //Hitbox debug
                        var hitbox = pane.CreateRectangle();
                        hitbox = hitbox.GetTransformedRectangle(pane.Parent, pane.Translate, pane.Rotate, pane.Scale);

                        GL.Begin(PrimitiveType.Quads);
                        GL.Color4(Color.FromArgb(128, 255, 0, 0));
                        GL.Vertex2(hitbox.BottomLeftPoint);
                        GL.Vertex2(hitbox.BottomRightPoint);
                        GL.Vertex2(hitbox.TopRightPoint);
                        GL.Vertex2(hitbox.TopLeftPoint);
                        GL.End();
                    }
                }
            }

            foreach (var layout in LayoutFiles)
                RenderPanes(GlobalShader, layout.RootPane, true, 255, false, null, 0);

            Vector2 TopLeft = new Vector2();
            Vector2 BottomRight = new Vector2();

            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.Blend);
            GL.UseProgram(0);
            foreach (var pane in SelectedPanes)
            {
                var rect = pane.CreateRectangle();
                TopLeft.X = Math.Min(TopLeft.X, rect.LeftPoint);
                TopLeft.Y = Math.Max(TopLeft.Y, rect.TopPoint);
                BottomRight.X = Math.Max(BottomRight.X, rect.RightPoint);
                BottomRight.Y = Math.Min(BottomRight.Y, rect.BottomPoint);

                if (pickAxis == PickAxis.Y)
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color4(Color.Green);
                    GL.Vertex2(pane.Translate.X, -999999);
                    GL.Vertex2(pane.Translate.X, 99999);
                    GL.End();
                }
                if (pickAxis == PickAxis.X)
                {
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color4(Color.Red);
                    GL.Vertex2(-999999, pane.Translate.Y);
                    GL.Vertex2(99999, pane.Translate.Y);
                    GL.End();
                }
            }


            if (showSelectionBox)
            {
                GL.Begin(PrimitiveType.LineLoop);
                GL.Color4(Color.Red);
                GL.Vertex2(SelectionBox.BottomLeftPoint);
                GL.Vertex2(SelectionBox.BottomRightPoint);
                GL.Vertex2(SelectionBox.TopRightPoint);
                GL.Vertex2(SelectionBox.TopLeftPoint);
                GL.End();
            }

            //Create a bounding box for all selected panes
            //This box will allow resizing of all selected panes
            if (SelectedPanes.Count > 0)
                DrawSelectionBox(SelectedPanes, true);

            if (UseOrtho)
                GL.PopMatrix();

            GL.UseProgram(0);

            glControl1.SwapBuffers();
        }

        private static CustomRectangle GetSelectedPanesBounding(List<BasePane> panes)
        {
            CustomRectangle rect = new CustomRectangle(0, 0, 0, 0);

            List<Vector2> points = new List<Vector2>();
            foreach (var pane in panes)
            {
                var paneRect = pane.CreateRectangle();

                rect = paneRect.GetTransformedRectangle(pane.Parent,
                    pane.GetTranslation(), pane.GetRotation(), pane.GetScale());
                points.AddRange(new Vector2[4]
                {
                     rect.TopLeftPoint,
                     rect.TopRightPoint,
                     rect.BottomRightPoint,
                     rect.BottomLeftPoint,
                });

                var minX = (int)points.Min(p => p.X);
                var maxX = (int)points.Max(p => p.X);
                var minY = (int)points.Min(p => p.Y);
                var maxY = (int)points.Max(p => p.Y);

                rect = new CustomRectangle(minX, maxX, maxY, minY);
            }
            points.Clear();

            return rect;
        }

        public static void DrawSelectionBox(List<BasePane> panes, bool isSelected)
        {
            //Create a rectangle with the largest points selected
            CustomRectangle rect = GetSelectedPanesBounding(panes);

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.Texture2D);
            GL.UseProgram(0);

            GL.Begin(PrimitiveType.LineLoop);
            GL.Color4(isSelected ? Color.Red : Color.Green);
            GL.Vertex2(rect.BottomLeftPoint);
            GL.Vertex2(rect.BottomRightPoint);
            GL.Vertex2(rect.TopRightPoint);
            GL.Vertex2(rect.TopLeftPoint);
            GL.End();

            if (isSelected)
            {
                var transformed = rect;
                var leftTop = new Vector2(transformed.LeftPoint, transformed.TopPoint);
                var left = new Vector2(transformed.LeftPoint, (transformed.BottomPoint + transformed.TopPoint) / 2);
                var leftBottom = new Vector2(transformed.LeftPoint, transformed.BottomPoint);
                var rightTop = new Vector2(transformed.RightPoint, transformed.TopPoint);
                var right = new Vector2(transformed.RightPoint, (transformed.BottomPoint + transformed.TopPoint) / 2);
                var rightBottom = new Vector2(transformed.RightPoint, transformed.BottomPoint);
                var top = new Vector2((transformed.RightPoint + transformed.LeftPoint) / 2, transformed.TopPoint);
                var bottom = new Vector2((transformed.RightPoint + transformed.LeftPoint) / 2, transformed.BottomPoint);

                DrawEdgeSquare(leftTop);
                DrawEdgeSquare(left);
                DrawEdgeSquare(leftBottom);
                DrawEdgeSquare(rightTop);
                DrawEdgeSquare(right);
                DrawEdgeSquare(rightBottom);
                DrawEdgeSquare(top);
                DrawEdgeSquare(bottom);
            }

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.Texture2D);
        }

        private static void DrawEdgeSquare(Vector2 position)
        {
            float scale = 5;

            GL.Begin(PrimitiveType.LineLoop);
            GL.Color4(Color.Red);
            GL.Vertex2(position.X + -1 * scale, position.Y + -1 * scale);
            GL.Vertex2(position.X + 1 * scale, position.Y + -1 * scale);
            GL.Vertex2(position.X + 1 * scale, position.Y + 1 * scale);
            GL.Vertex2(position.X + -1 * scale, position.Y + 1 * scale);
            GL.End();
        }

        private void RenderPanes(BxlytShader shader, BasePane pane, bool isRoot, byte parentAlpha, bool parentAlphaInfluence, BasePane partPane = null, int stage = 0)
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

                DrawDefaultPane(shader, pane, false);

                GL.PopMatrix();
            }


            var translate = pane.GetTranslation();
            var rotate = pane.GetRotation();
            var scale = pane.GetScale();

            if (partPane != null)
            {
                translate = translate + pane.Translate;
                scale = scale * pane.Scale;
                rotate = rotate + pane.Rotate;

                var prtPane = (Cafe.PRT1)partPane;
                scale = new Syroot.Maths.Vector2F(
                    scale.X * prtPane.MagnifyX, 
                    scale.Y * prtPane.MagnifyY);
            }


            //Note rotation matrix done by shaders

            GL.Translate(translate.X, translate.Y, 0);
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
                bool isSelected = SelectedPanes.Contains(pane);

                if (!pane.Visible && !pane.animController.Visibile)
                    DrawDefaultPane(shader, pane, isSelected);
                else if (pane is IPicturePane)
                    BxlytToGL.DrawPictureBox(pane, Camera, GameWindow, effectiveAlpha, Textures, isSelected);
                else if (pane is IWindowPane)
                    BxlytToGL.DrawWindowPane(pane, Camera, GameWindow, effectiveAlpha, Textures, isSelected);
                else if (pane is IBoundryPane)
                {
                    shader.Enable();
                    shader.SetBasic(pane, Color.White);
                    BxlytToGL.DrawBoundryPane(pane, GameWindow, effectiveAlpha, isSelected);
                    shader.Disable();
                }
                else if (pane is ITextPane && Runtime.LayoutEditor.DisplayTextPane)
                {
                    var textPane = (ITextPane)pane;
                    Bitmap bitmap = null;

                    foreach (var fontFile in FirstPlugin.PluginRuntime.BxfntFiles) {
                        if (Utils.CompareNoExtension(fontFile.FileName, textPane.FontName))
                            bitmap = fontFile.GetBitmap(textPane.Text, false, pane);
                    }

                    if (bitmap != null)
                        BxlytToGL.DrawTextbox(pane, Camera, GameWindow, bitmap, effectiveAlpha,
                            Textures, SelectedPanes, textPane.RenderableFont == null, isSelected);
                    else
                        DrawDefaultPane(shader, pane, isSelected);
                }
                else if (pane is Cafe.SCR1)
                    BxlytToGL.DrawScissorPane(pane, GameWindow, effectiveAlpha, isSelected);
                else if (pane is Cafe.ALI1)
                    BxlytToGL.DrawAlignmentPane(pane, GameWindow, effectiveAlpha, isSelected);
                else if (pane is Cafe.PRT1)
                    DrawPartsPane(shader, (Cafe.PRT1)pane, effectiveAlpha, isSelected, parentAlphaInfluence);
                else
                    DrawDefaultPane(shader, pane, isSelected);
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
            GL.Vertex2(rect.TopLeftPoint);
            GL.Vertex2(rect.TopRightPoint);
            GL.Vertex2(rect.BottomRightPoint);
            GL.Vertex2(rect.BottomLeftPoint);
            GL.End();

            //Draw outline of root pane
            GL.Begin(PrimitiveType.LineLoop);
            GL.PolygonOffset(0.5f, 2);
            GL.LineWidth(33);
            GL.Color3(color);
            GL.Vertex2(rect.TopLeftPoint);
            GL.Vertex2(rect.TopRightPoint);
            GL.Vertex2(rect.BottomRightPoint);
            GL.Vertex2(rect.BottomLeftPoint);
            GL.End();
        }

        private void DrawDefaultPane(BxlytShader shader, BasePane pane, bool isSelectionBox = false)
        {
            if (!Runtime.LayoutEditor.DisplayNullPane || GameWindow || Runtime.LayoutEditor.IsGamePreview)
                return;

            shader.Enable();
            shader.SetBasic(pane, Color.Black);

            Vector2[] TexCoords = new Vector2[] {
                new Vector2(1,1),
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0)
                };

            Color color = Color.Black;
            if (SelectedPanes.Contains(pane))
                color = Color.Red;

            Color[] Colors = new Color[] {
                color,
                color,
                color,
                color,
                };

            BxlytToGL.DrawRectangle(pane, GameWindow, pane.Rectangle, TexCoords, Colors, true, 255, isSelectionBox);

            shader.Disable();
        }

        private void DrawPartsPane(BxlytShader shader, Cafe.PRT1 pane, byte effectiveAlpha, bool isSelected, bool parentInfluenceAlpha)
        {
            if (Runtime.LayoutEditor.PartsAsNullPanes)
            {
                DrawDefaultPane(shader, pane, isSelected);
                return;
            }

            pane.UpdateTextureData(this.Textures);
            var partPane = pane.GetExternalPane();
            if (partPane != null)
                RenderPanes(shader, partPane, true, effectiveAlpha, parentInfluenceAlpha);
            else
                DrawDefaultPane(shader, pane, isSelected);

            if (pane.Properties != null)
            {
                foreach (var prop in pane.Properties)
                {
                    if (prop.Property != null)
                    {
                        RenderPanes(shader, prop.Property, false, effectiveAlpha, parentInfluenceAlpha || pane.InfluenceAlpha);
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
        }

        private bool mouseCameraDown = false;
        private bool mouseDown = false;

        private List<BasePane> SelectionBoxPanes = new List<BasePane>();
        private bool showSelectionBox = false;
        private bool isPicked = false;
        private bool mouseMoving = false;
        private Point originMouse;
        private Point pickOriginMouse;
        private Point pickMouse;
        private Vector2 pickDistance;
        private PickAction pickAction = PickAction.None;
        private PickAxis pickAxis = PickAxis.All;
        private bool snapToGrid = false;

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (GameWindow)
                return;

            pickAction = PickAction.None;
            pickAxis = PickAxis.All;

            if (Control.ModifierKeys == Keys.Shift && e.Button == MouseButtons.Left ||
               e.Button == MouseButtons.Middle)
            {
                originMouse = e.Location;
                mouseCameraDown = true;
                glControl1.Invalidate();
            }
            //Pick an object for moving
            else if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;

                RenderEditor();
                var coords = OpenGLHelper.convertScreenToWorldCoords(e.Location.X, e.Location.Y);

                GL.PopMatrix();

                bool hasEdgeHit = false;

                var rect = GetSelectedPanesBounding(SelectedPanes);
                var edgePick = SearchEdgePicking(rect, coords.X, coords.Y);
                if (edgePick != PickAction.None)
                {
                    pickAction = edgePick;
                    isPicked = true;
                    hasEdgeHit = true;
                }

                if (hasEdgeHit)
                {
                    UndoManger.AddToUndo(new LayoutUndoManager.UndoActionTransform(SelectedPanes));
                    pickOriginMouse = e.Location;
                    return;
                }


                BasePane hitPane = null;
                SearchHit(LayoutFile.RootPane, coords.X, coords.Y, ref hitPane);
                if (hitPane != null)
                {
                    pickAction = PickAction.Translate;

                    if (!SelectedPanes.Contains(hitPane))
                    {
                        if (Control.ModifierKeys != Keys.Control)
                            SelectedPanes.Clear();

                        SelectedPanes.Add(hitPane);
                    }


                    var paneRect = GetSelectedPanesBounding(SelectedPanes);
                    var paneEdgePick = SearchEdgePicking(paneRect, coords.X, coords.Y);
                    if (paneEdgePick != PickAction.None)
                        pickAction = paneEdgePick;

                    UndoManger.AddToUndo(new LayoutUndoManager.UndoActionTransform(SelectedPanes));

                    ParentEditor.UpdateUndo();
                    ParentEditor.UpdateHiearchyNodeSelection(hitPane);

                    isPicked = true;
                } //Check control key (multi selecting panes)
                else if (Control.ModifierKeys != Keys.Control)
                    SelectedPanes.Clear();


                pickOriginMouse = e.Location;
            }
            else if (e.Button == MouseButtons.Right)
            {
                RenderEditor();
                var coords = OpenGLHelper.convertScreenToWorldCoords(e.Location.X, e.Location.Y);
                pickOriginMouse = coords;

                GL.PopMatrix();

                //Add a content menu 
                var selectOverlapping = new STToolStripItem("Select Overlapping");
                var createPanes = new STToolStripItem("Create Pane");
                createPanes.DropDownItems.Add(new STToolStripItem("Null Pane", CreateNullPaneAction));
                createPanes.DropDownItems.Add(new STToolStripItem("Picture Pane", CreatePicturePaneAction));
                createPanes.DropDownItems.Add(new STToolStripItem("Part Pane", CreatePartPaneAction));
                createPanes.DropDownItems.Add(new STToolStripItem("Text Box Pane", CreateTextPaneAction));
                createPanes.DropDownItems.Add(new STToolStripItem("Window Pane", CreateWindowPaneAction));
                createPanes.DropDownItems.Add(new STToolStripItem("Boundry Pane", CreateBoundryPaneAction));
                var hitPanes = GetHitPanes(LayoutFile.RootPane, coords.X, coords.Y, new List<BasePane>());
                for (int i = 0; i < hitPanes.Count; i++)
                    selectOverlapping.DropDownItems.Add(
                        new STToolStripItem(hitPanes[i].Name, SelectOverlappingAction));

                stContextMenuStrip1.Items.Clear();
                stContextMenuStrip1.Items.Add(createPanes);
                stContextMenuStrip1.Items.Add(selectOverlapping);
                stContextMenuStrip1.Items.Add(new STToolStripItem("Show All Hidden Panes", ShowAllPaneAction));
                stContextMenuStrip1.Items.Add(new STToolStripItem("Paste (Experimental)", PastePaneAction) { Enabled = CopiedPanes.Count > 0 });

                if (SelectedPanes.Count > 0)
                {
                    stContextMenuStrip1.Items.Add(new STToolStripSeparator());
                    stContextMenuStrip1.Items.Add(new STToolStripItem("Copy (Experimental)", CopyPaneAction));
                    stContextMenuStrip1.Items.Add(new STToolStripItem("Edit Group"));
                    stContextMenuStrip1.Items.Add(new STToolStripItem("Delete Selected Panes", DeletePaneAction));
                    stContextMenuStrip1.Items.Add(new STToolStripItem("Hide Selected Panes", HidePaneAction));
                }

                stContextMenuStrip1.Show(Cursor.Position);
            }

            Console.WriteLine("SelectedPanes " + SelectedPanes.Count);
        }

        private void CreatePartPaneAction(object sender, EventArgs e)
        {
            var pane = ParentEditor.AddNewPartPane();
            SetupNewPane(pane, pickOriginMouse);
        }

        private void CreateNullPaneAction(object sender, EventArgs e)
        {
            var pane = ParentEditor.AddNewNullPane();
            SetupNewPane(pane, pickOriginMouse);
        }

        private void CreatePicturePaneAction(object sender, EventArgs e)
        {
            var pane = ParentEditor.AddNewPicturePane();
            SetupNewPane(pane, pickOriginMouse);
        }

        private void CreateWindowPaneAction(object sender, EventArgs e)
        {
            var pane = ParentEditor.AddNewWindowPane();
            SetupNewPane(pane, pickOriginMouse);
        }

        private void CreateTextPaneAction(object sender, EventArgs e)
        {
            var pane = ParentEditor.AddNewTextPane();
            SetupNewPane(pane, pickOriginMouse);
            if (pane is Cafe.TXT1)
                ((Cafe.TXT1)pane).UpdateTextRender();
        }

        private void CreateBoundryPaneAction(object sender, EventArgs e)
        {
            var pane = ParentEditor.AddNewBoundryPane();
            SetupNewPane(pane, pickOriginMouse);
        }

        private void CopyPaneAction(object sender, EventArgs e)
        {
            CopyPanes();
        }

        private void PastePaneAction(object sender, EventArgs e)
        {
            PastePanes();
        }

        private void CopyPanes()
        {
            CopiedPanes.Clear();
            foreach (var pane in SelectedPanes) {
                BasePane copiedPane = (BasePane)pane.Clone();
                CopiedPanes.Add(copiedPane);
            }
        }

        private void PastePanes()
        {
            SelectedPanes.Clear();
            foreach (var copiedPane in CopiedPanes)
            {
                //Copy again as the user may paste the same one multiple times
                //This will make sure it's a completely new instance
                BasePane pane = (BasePane)copiedPane.Clone();
                ParentEditor.AddNewPastedPane(pane);
                SelectedPanes.Add(pane);
            }

            glControl1.Invalidate();
        }

        private void SetupNewPane(BasePane pane, Point point)
        {
            if (pane == null) return;

            SelectedPanes.Clear();
            pane.Translate = new Syroot.Maths.Vector3F(point.X, point.Y, 0);
            ParentEditor.LoadPaneEditorOnSelect(pane);

            glControl1.Invalidate();
        }

        private void DeletePaneAction(object sender, EventArgs e)
        {
            DeleteSelectedPanes();
        }

        private void HidePaneAction(object sender, EventArgs e)
        {
            HideSelectedPanes();
        }

        private void ShowAllPaneAction(object sender, EventArgs e)
        {
            ShowHiddenPanes();
        }

        private void HideSelectedPanes()
        {
            UndoManger.AddToUndo(new LayoutUndoManager.UndoActionPaneHide(SelectedPanes));
            ParentEditor?.UpdateHiearchyTree();
            SelectedPanes.Clear();
            glControl1.Invalidate();
        }

        private void ShowHiddenPanes()
        {
            UndoManger.AddToUndo(new LayoutUndoManager.UndoActionPaneHide(LayoutFile.PaneLookup.Values.ToList(), false));
            ParentEditor?.UpdateHiearchyTree();
            glControl1.Invalidate();
        }

        private void DeleteSelectedPanes()
        {
            if (SelectedPanes.Count == 0) return;
            //Make sure to fill all the children in selected panes!
            for (int i = 0; i < SelectedPanes.Count; i++)
                SelectedPanes.AddRange(GetChildren(SelectedPanes, new List<BasePane>(), SelectedPanes[i]));

            UndoManger.AddToUndo(new LayoutUndoManager.UndoActionPaneDelete(SelectedPanes, LayoutFile));
            LayoutFile.RemovePanes(SelectedPanes, LayoutFile.RootPane);
            SelectedPanes.Clear();
            ParentEditor?.UpdateHiearchyTree();
            glControl1.Invalidate();
        }

        private List<BasePane> GetChildren(List<BasePane> selectedPanes, List<BasePane> childrenPanes, BasePane parent)
        {
            if (!selectedPanes.Contains(parent))
                childrenPanes.Add(parent);

            foreach (var child in parent.Childern)
                GetChildren(selectedPanes, childrenPanes, child);
            return childrenPanes;
        }

        private void SelectOverlappingAction(object sender, EventArgs e)
        {
            var toolMenu = sender as STToolStripItem;
            if (toolMenu != null)
            {
                string name = toolMenu.Text;
                if (Control.ModifierKeys != Keys.Control)
                    SelectedPanes.Clear();

                if (LayoutFile.PaneLookup.ContainsKey(name))
                    SelectedPanes.Add(LayoutFile.PaneLookup[name]);

                glControl1.Invalidate();
            }
        }

        private void SearchHit(BasePane pane, int X, int Y, ref BasePane SelectedPane)
        {
            bool isVisible = true;
            if (!Runtime.LayoutEditor.DisplayPicturePane && pane is IPicturePane)
                isVisible = false;
            if (!Runtime.LayoutEditor.DisplayWindowPane && pane is IWindowPane)
                isVisible = false;
            if (!Runtime.LayoutEditor.DisplayBoundryPane && pane is IBoundryPane)
                isVisible = false;
            if (!Runtime.LayoutEditor.DisplayTextPane && pane is ITextPane)
                isVisible = false;
            if (!Runtime.LayoutEditor.DisplayNullPane && pane.IsNullPane)
                isVisible = false;

            if (isVisible && pane.DisplayInEditor && pane.IsHit(X, Y) && !pane.IsRoot)
            {
                //Select the first possible pane
                //If the pane is selected already, pick that instead
                //This is useful if the selected pane wants to be moved already
                if (SelectedPane == null || SelectedPanes.Contains(pane))
                    SelectedPane = pane;
            }

            //Keep searching even if we found our pane so we can find any that's selected
            foreach (var childPane in pane.Childern)
                SearchHit(childPane, X, Y, ref SelectedPane);
        }

        private List<BasePane> GetHitPanes(BasePane pane, CustomRectangle rect, List<BasePane> SelectedPanes)
        {
            bool isVisible = pane.Visible;
            if (!Runtime.LayoutEditor.DisplayPicturePane && pane is IPicturePane)
                isVisible = false;
            if (!Runtime.LayoutEditor.DisplayWindowPane && pane is IWindowPane)
                isVisible = false;
            if (!Runtime.LayoutEditor.DisplayBoundryPane && pane is IBoundryPane)
                isVisible = false;
            if (!Runtime.LayoutEditor.DisplayTextPane && pane is ITextPane)
                isVisible = false;
            if (!Runtime.LayoutEditor.DisplayNullPane && pane.IsNullPane)
                isVisible = false;

            if (isVisible && pane.DisplayInEditor && pane.IsHit(rect) && pane.Name != "RootPane")
                if (!SelectedPanes.Contains(pane))
                    SelectedPanes.Add(pane);

            foreach (var childPane in pane.Childern)
                GetHitPanes(childPane, rect, SelectedPanes);

            return SelectedPanes;
        }

        private List<BasePane> GetHitPanes(BasePane pane, int X, int Y, List<BasePane> SelectedPanes)
        {
            bool isVisible = pane.Visible;
            if (!Runtime.LayoutEditor.DisplayPicturePane && pane is IPicturePane)
                isVisible = false;
            if (!Runtime.LayoutEditor.DisplayWindowPane && pane is IWindowPane)
                isVisible = false;
            if (!Runtime.LayoutEditor.DisplayBoundryPane && pane is IBoundryPane)
                isVisible = false;
            if (!Runtime.LayoutEditor.DisplayTextPane && pane is ITextPane)
                isVisible = false;
            if (!Runtime.LayoutEditor.DisplayNullPane && pane.IsNullPane)
                isVisible = false;

            if (isVisible && pane.DisplayInEditor && pane.IsHit(X, Y) && pane.Name != "RootPane")
                if (!SelectedPanes.Contains(pane))
                    SelectedPanes.Add(pane);

            foreach (var childPane in pane.Childern)
                GetHitPanes(childPane, X, Y, SelectedPanes);

            return SelectedPanes;
        }

        private PickAction SearchEdgePicking(CustomRectangle transformed, int X, int Y)
        {
            var leftTop = new Point(transformed.LeftPoint, transformed.TopPoint);
            var left = new Point(transformed.LeftPoint, (transformed.BottomPoint + transformed.TopPoint) / 2);
            var leftBottom = new Point(transformed.LeftPoint, transformed.BottomPoint);
            var rightTop = new Point(transformed.RightPoint, transformed.TopPoint);
            var right = new Point(transformed.RightPoint, (transformed.BottomPoint + transformed.TopPoint) / 2);
            var rightBottom = new Point(transformed.RightPoint, transformed.BottomPoint);
            var top = new Point((transformed.RightPoint + transformed.LeftPoint) / 2, transformed.TopPoint);
            var bottom = new Point((transformed.RightPoint + transformed.LeftPoint) / 2, transformed.BottomPoint);

            if (IsEdgeHit(leftTop, X, Y)) return PickAction.DragTopLeft;
            else if (IsEdgeHit(left, X, Y)) return PickAction.DragLeft;
            else if (IsEdgeHit(leftBottom, X, Y)) return PickAction.DragBottomLeft;
            else if (IsEdgeHit(rightTop, X, Y)) return PickAction.DragTopRight;
            else if (IsEdgeHit(rightBottom, X, Y)) return PickAction.DragBottomRight;
            else if (IsEdgeHit(right, X, Y)) return PickAction.DragRight;
            else if (IsEdgeHit(top, X, Y)) return PickAction.DragTop;
            else if (IsEdgeHit(bottom, X, Y)) return PickAction.DragBottom;

            return PickAction.None;
        }

        private bool IsEdgeHit(Point point, int X, int Y, int size = 10)
        {
            if ((X > point.X - size) && (X < point.X + size) &&
                (Y > point.Y - size) && (Y < point.Y + size))
                return true;
            else
                return false;
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
            {
                pickAxis = PickAxis.All;
                mouseCameraDown = false;
                mouseDown = false;
                isPicked = false;
                mouseMoving = false;
                showSelectionBox = false;

                foreach (var pane in SelectionBoxPanes)
                    if (!SelectedPanes.Contains(pane))
                        SelectedPanes.Add(pane);

                SelectionBoxPanes.Clear();

                ParentEditor.RefreshEditors();

                if (SelectedPanes.Count > 0)
                    ParentEditor.UpdateHiearchyNodeSelection(SelectedPanes.FirstOrDefault());
            }

            glControl1.Invalidate();
        }

        public enum PickAction
        {
            None,
            DragTopRight,
            DragTopLeft,
            DragTop,
            DragLeft,
            DragRight,
            DragBottom,
            DragBottomLeft,
            DragBottomRight,
            Translate,
            Scale,
            Rotate
        }

        public enum PickAxis
        {
            All,
            X,
            Y,
            Z,
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (GameWindow)
                return;

            if (UseOrtho)
                GL.PopMatrix();

            if (SelectedPanes.Count > 0 && !showSelectionBox)
            {
                RenderEditor();
                var posWorld = OpenGLHelper.convertScreenToWorldCoords(e.Location.X, e.Location.Y);

                GL.PopMatrix();

                //Setup edge picking with move event
                bool hasPick = false;

                var paneRect = GetSelectedPanesBounding(SelectedPanes);
                var pickState = SearchEdgePicking(paneRect, posWorld.X, posWorld.Y);

                if (pickState != PickAction.None)
                {
                    if (pickState == PickAction.DragTop)
                        Cursor.Current = Cursors.SizeNS;
                    if (pickState == PickAction.DragBottom)
                        Cursor.Current = Cursors.SizeNS;
                    if (pickState == PickAction.DragLeft)
                        Cursor.Current = Cursors.SizeWE;
                    if (pickState == PickAction.DragRight)
                        Cursor.Current = Cursors.SizeWE;
                    if (pickState == PickAction.DragBottomLeft)
                        Cursor.Current = Cursors.SizeNESW;
                    if (pickState == PickAction.DragBottomRight)
                        Cursor.Current = Cursors.SizeNWSE;
                    if (pickState == PickAction.DragTopLeft)
                        Cursor.Current = Cursors.SizeNWSE;
                    if (pickState == PickAction.DragTopRight)
                        Cursor.Current = Cursors.SizeNESW;

                    hasPick = true;
                }
                else if (isPicked && pickAction != PickAction.None)
                {
                    if (pickAction == PickAction.Translate)
                        Cursor.Current = Cursors.SizeAll;
                    if (pickAction == PickAction.Rotate)
                        Cursor.Current = Cursors.SizeAll;
                    if (pickAction == PickAction.Scale)
                        Cursor.Current = Cursors.SizeAll;

                    hasPick = true;
                }

                if (!hasPick)
                    Cursor.Current = Cursors.Default;
            }

            if (isPicked && !showSelectionBox)
            {
                RenderEditor();
                var temp = e.Location;
                var curPos = OpenGLHelper.convertScreenToWorldCoords(temp.X, temp.Y);
                var prevPos = OpenGLHelper.convertScreenToWorldCoords(pickOriginMouse.X, pickOriginMouse.Y);
                var pickMouse = new Point((int)(prevPos.X - curPos.X), (int)(prevPos.Y - curPos.Y));

                if (pickAction == PickAction.Translate)
                {
                    foreach (var pane in SelectedPanes)
                    {
                        if (pickOriginMouse != Point.Empty)
                        {
                            float posX = pane.Translate.X;
                            float posY = pane.Translate.Y;
                            float posZ = pane.Translate.Z;

                            if (pickAxis == PickAxis.X)
                                posX = pane.Translate.X - pickMouse.X;
                            if (pickAxis == PickAxis.Y)
                                posY = pane.Translate.Y - pickMouse.Y;
                            if (pickAxis == PickAxis.All)
                            {
                                posX = pane.Translate.X - pickMouse.X;
                                posY = pane.Translate.Y - pickMouse.Y;
                            }

                            if (Runtime.LayoutEditor.AnimationEditMode)
                            {

                            }
                            else
                            {
                                if (!Runtime.LayoutEditor.TransformChidlren)
                                    pane.KeepChildrenTransform(posX, posY);

                                if (snapToGrid)
                                {
                                    int gridCubeWidth = 16, gridCubeHeight = 16;

                                    pane.Translate = new Syroot.Maths.Vector3F(
                                     (float)(Math.Round(posX / gridCubeWidth) * gridCubeWidth),
                                     (float)(Math.Round(posY / gridCubeHeight) * gridCubeHeight),
                                     posZ);
                                }
                                else
                                {
                                    pane.Translate = new Syroot.Maths.Vector3F(posX, posY, posZ);
                                }
                            }
                        }
                    }
                }
                if (pickAction == PickAction.Scale)
                {
                    foreach (var pane in SelectedPanes)
                    {
                        if (pickOriginMouse != Point.Empty)
                        {
                            float scaX = pane.Scale.X;
                            float scaY = pane.Scale.Y;

                            if (pickAxis == PickAxis.X)
                                scaX = pane.Scale.X - pickMouse.X;
                            if (pickAxis == PickAxis.Y)
                                scaY = pane.Scale.Y - pickMouse.Y;
                            if (pickAxis == PickAxis.All)
                            {
                                scaX = pane.Scale.X - pickMouse.X;
                                scaY = pane.Scale.Y - pickMouse.Y;
                            }

                            if (Runtime.LayoutEditor.AnimationEditMode)
                            {

                            }
                            else
                            {
                                if (snapToGrid)
                                {
                                    int gridCubeWidth = 16, gridCubeHeight = 16;

                                    pane.Scale = new Syroot.Maths.Vector2F(
                                     (float)(Math.Round(scaX / gridCubeWidth) * gridCubeWidth),
                                     (float)(Math.Round(scaY / gridCubeHeight) * gridCubeHeight));
                                }
                                else
                                {
                                    pane.Scale = new Syroot.Maths.Vector2F(scaX, scaY);
                                }
                            }
                        }
                    }
                }
                else if (!showSelectionBox)
                {
                    var selectionBox = GetSelectedPanesBounding(SelectedPanes);

                    //Setup edge picking with move event
                    foreach (var pane in SelectedPanes)
                        pane.TransformRectangle(pickAction, selectionBox, pickMouse.X, pickMouse.Y);
                }

                pickOriginMouse = temp;

                RenderScene();
            }

            if (mouseDown && !isPicked)
            {
                RenderEditor();
                var temp = e.Location;
                var curPos = OpenGLHelper.convertScreenToWorldCoords(temp.X, temp.Y);
                var prevPos = OpenGLHelper.convertScreenToWorldCoords(pickOriginMouse.X, pickOriginMouse.Y);
                DrawSelectionBox(prevPos, curPos);
            }

            if (mouseCameraDown)
            {
                var pos = new Vector2(e.Location.X - originMouse.X, e.Location.Y - originMouse.Y);
                Camera.Position.X += pos.X;
                Camera.Position.Y -= pos.Y;

                originMouse = e.Location;

                glControl1.Invalidate();
            }
        }

        private CustomRectangle SelectionBox;

        private void DrawSelectionBox(Point point1, Point point2)
        {
            SelectionBoxPanes.Clear();

            int left = point1.X;
            int right = point2.X;
            int top = point1.Y;
            int bottom = point2.Y;
            //Determine each point direction to see what is left/right/top/bottom
            if (bottom > top)
            {
                top = point2.Y;
                bottom = point1.Y;
            }
            if (left > right)
            {
                right = point1.X;
                left = point2.X;
            }

            showSelectionBox = true;
            SelectionBox = new CustomRectangle(left, right, top, bottom);
            var hitPanes = GetHitPanes(LayoutFile.RootPane, SelectionBox, new List<BasePane>());
            foreach (var pane in hitPanes)
                if (!SelectionBoxPanes.Contains(pane))
                    SelectionBoxPanes.Add(pane);

            RenderScene(true);
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

        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (isPicked && e.KeyCode == Keys.X)
            {
                pickAxis = PickAxis.X;
                glControl1.Invalidate();
            }
            if (isPicked && e.KeyCode == Keys.Y)
            {
                pickAxis = PickAxis.Y;
                glControl1.Invalidate();
            }
            else if (e.Control && e.KeyCode == Keys.Z) // Ctrl + Z undo
            {
                UndoManger.Undo();
                ParentEditor.UpdateUndo();
                glControl1.Invalidate();
            }
            else if (e.Control && e.KeyCode == Keys.R) // Ctrl + R redo
            {
                UndoManger.Redo();
                ParentEditor.UpdateUndo();
                glControl1.Invalidate();
            }
            else if (e.Control && e.KeyCode == Keys.C) // Ctrl + C copy
            {
                CopyPanes();
                glControl1.Invalidate();
            }
            else if (e.Control && e.KeyCode == Keys.V) // Ctrl + V paste
            {
                PastePanes();
                glControl1.Invalidate();
            }
            else if (e.Control && e.KeyCode == Keys.A) // Ctrl + A select all
            {
                SelectedPanes.Clear();
                foreach (var pane in LayoutFile.PaneLookup.Values)
                    if (!pane.IsRoot)
                        SelectedPanes.Add(pane);

                glControl1.Invalidate();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedPanes();
            }
        }

        private void glControl1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void glControl1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                var item = e.Data.GetData(typeof(ListViewItem)) as ListViewItem;
                string texture = item.Text;

                ParentEditor.DeselectTextureList();
                if (Textures.ContainsKey(texture))
                {
                    var point = this.PointToClient(new Point(e.X, e.Y));

                    RenderEditor();
                    var coords = OpenGLHelper.convertScreenToWorldCoords(point.X, point.Y);
                    GL.PopMatrix();

                    var pane = ParentEditor.AddNewPicturePane();
                    pane.Width = Textures[texture].Width;
                    pane.Height = Textures[texture].Height;
                    ((IPicturePane)pane).Material.AddTexture(texture);
                    SetupNewPane(pane, coords);

                    this.Focus();
                }
            }
        }
    }
}
