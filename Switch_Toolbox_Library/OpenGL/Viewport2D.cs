using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;
using Toolbox.Library.OpenGL2D;

namespace Toolbox.Library.Forms
{
    public class Viewport2D : UserControl
    {
        public virtual float PreviewScale => 1f;

        public virtual bool UseOrtho { get; set; } = true;
        public virtual bool UseGrid { get; set; } = true;

        public Camera2D Camera = new Camera2D();

        public class Camera2D
        {
            public Matrix4 ViewMatrix = Matrix4.Identity;
            public Matrix4 ProjectionMatrix = Matrix4.Identity;

            public Matrix4 ModelViewMatrix
            {
                get
                {
                    return ViewMatrix * ProjectionMatrix;
                }
            }

            public float Zoom = 1;
            public Vector2 Position;
        }

        private GLControl glControl1;
        public Color BackgroundColor = Color.FromArgb(40, 40, 40);

        private List<IPickable2DObject> SelectedObjects = new List<IPickable2DObject>();

        private PickAction pickAction = PickAction.None;
        private PickAxis pickAxis = PickAxis.All;

        public Viewport2D()
        {
            glControl1 = new GLControl();
            glControl1.Dock = DockStyle.Fill;
            glControl1.MouseDown += glControl1_MouseDown;
            glControl1.MouseUp += glControl1_MouseUp;
            glControl1.MouseMove += glControl1_MouseMove;
            glControl1.KeyDown += glControl1_KeyDown;

            glControl1.Paint += glControl1_Paint;
            glControl1.Resize += glControl1_Resize;
            Controls.Add(glControl1);
        }


        public void UpdateViewport() {
            if (!Runtime.OpenTKInitialized)
                return;

            glControl1.Invalidate();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!Runtime.OpenTKInitialized)
                return;

            glControl1.Context.MakeCurrent(glControl1.WindowInfo);

            RenderEditor();
            SetupScene();
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

                Matrix4 scaleMat = Matrix4.CreateScale(Camera.Zoom * PreviewScale, Camera.Zoom * PreviewScale, 1);
                Matrix4 transMat = Matrix4.CreateTranslation(Camera.Position.X, Camera.Position.Y, 0);

                Camera.ViewMatrix = scaleMat * transMat;
            }
            else
            {
                var cameraPosition = new Vector3(Camera.Position.X, Camera.Position.Y, -(Camera.Zoom * 500));

                var perspectiveMatrix = Matrix4.CreatePerspectiveFieldOfView(1.3f, glControl1.Width / glControl1.Height, 0.01f, 100000);
                GL.LoadMatrix(ref perspectiveMatrix);
                GL.MatrixMode(MatrixMode.Modelview);

                Camera.ViewMatrix =  Matrix4.CreateTranslation(cameraPosition);
                Camera.ProjectionMatrix = perspectiveMatrix;

                GL.LoadMatrix(ref Camera.ViewMatrix);
            }

            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (UseOrtho)
            {
                GL.PushMatrix();
                GL.Scale(Camera.Zoom * PreviewScale, Camera.Zoom * PreviewScale, 1);
                GL.Translate(Camera.Position.X, Camera.Position.Y, 0);
            }
        }

        public virtual List<IPickable2DObject> GetPickableObjects()
        {
            return new List<IPickable2DObject>();
        }

        private List<IPickable2DObject> SearchHit(float X, float Y)
        {
            List<IPickable2DObject> picks = new List<IPickable2DObject>();
            foreach (var pickObj in GetPickableObjects())
            {
                if (pickObj.IsHit(X, Y))
                    picks.Add(pickObj);
            }
            return picks;
        }

        private void SetupScene()
        {
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Always, 0f);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BlendEquation(BlendEquationMode.FuncAdd);

            if (UseGrid)
                Render2D.DrawGrid(BackgroundColor);

            RenderScene();

            if (showSelectionBox)
            {
                GL.PushAttrib(AttribMask.DepthBufferBit);

                GL.Disable(EnableCap.DepthTest);
                GL.Begin(PrimitiveType.LineLoop);
                GL.Color4(Color.Red);
                GL.Vertex2(SelectionBox.LeftPoint, SelectionBox.BottomPoint);
                GL.Vertex2(SelectionBox.RightPoint, SelectionBox.BottomPoint);
                GL.Vertex2(SelectionBox.RightPoint, SelectionBox.TopPoint);
                GL.Vertex2(SelectionBox.LeftPoint, SelectionBox.TopPoint);
                GL.End();
                GL.Enable(EnableCap.DepthTest);

                GL.PopAttrib();
            }

            if (UseOrtho)
                GL.PopMatrix();

            GL.UseProgram(0);
            glControl1.SwapBuffers();
        }

        private STRectangle SelectionBox;

        private bool showSelectionBox = false;

        private void DrawSelectionBox(Point point1, Point point2)
        {
            SelectedObjects.Clear();

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
            SelectionBox = new STRectangle(left, right, top, bottom);

            UpdateViewport();
        }

        public virtual void RenderScene()
        {

        }

        private Point pickOriginMouse;
        private Point originMouse;
        private bool mouseCameraDown;
        private bool isPicked;
        private bool mouseDown = false;

        private Vector2 GetMouseCoords(Point screenMouse)
        {
            RenderEditor();
            var coords = OpenGLHelper.convertScreenToWorldCoords(screenMouse.X, screenMouse.Y);
            GL.PopMatrix();
            return new Vector2(coords.X, coords.Y);
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift && e.Button == MouseButtons.Left ||
              e.Button == MouseButtons.Middle)
            {
                originMouse = e.Location;
                mouseCameraDown = true;
                glControl1.Invalidate();
            }
            else if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;

                var mouseCoords = GetMouseCoords(e.Location);

                var picks = SearchHit(mouseCoords.X, mouseCoords.Y);
                if (picks.Count > 0)
                {
                    if (!SelectedObjects.Contains(picks[0]))
                    {
                        if (Control.ModifierKeys != Keys.Control)
                            UnselectAll();

                        SelectedObjects.Add(picks[0]);
                        picks[0].IsSelected = true;
                    }

                    pickAction = PickAction.Translate;
                    isPicked = true;
                }
                else if (Control.ModifierKeys != Keys.Control)
                    UnselectAll();

                pickOriginMouse = e.Location;
                glControl1.Invalidate();
            }
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
            {
                pickAction = PickAction.None;
                mouseCameraDown = false;
                mouseDown = false;
                isPicked = false;
                showSelectionBox = false;

                glControl1.Invalidate();
            }
        }

        private void UnselectAll()
        {
            foreach (var pick in SelectedObjects)
                pick.IsSelected = false;

            SelectedObjects.Clear();
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            var mouseCoords = GetMouseCoords(e.Location);
            var picks = SearchHit(mouseCoords.X, mouseCoords.Y);

            if (picks.Count > 0)
            {
                if (!picks[0].IsSelected)
                {
                    picks[0].IsHovered = true;

                    glControl1.Invalidate();
                }
            }
            else
            {
                foreach (var obj in GetPickableObjects())
                    obj.IsHovered = false;

                glControl1.Invalidate();
            }

            if (mouseCameraDown)
            {
                var pos = new Vector2(e.Location.X - originMouse.X, e.Location.Y - originMouse.Y);
                Camera.Position.X += pos.X;
                Camera.Position.Y -= pos.Y;

                originMouse = e.Location;

                glControl1.Invalidate();
            }

            if (!showSelectionBox && isPicked)
            {
                Console.WriteLine(pickAction);

                RenderEditor();
                var temp = e.Location;
                var curPos = OpenGLHelper.convertScreenToWorldCoords(temp.X, temp.Y);
                var prevPos = OpenGLHelper.convertScreenToWorldCoords(pickOriginMouse.X, pickOriginMouse.Y);
                var pickMouse = new Point((int)(prevPos.X - curPos.X), (int)(prevPos.Y - curPos.Y));

                Console.WriteLine("curPos " + curPos);
                Console.WriteLine("prevPos " + prevPos);

                GL.PopMatrix();

                if (pickAction == PickAction.Translate)
                {
                    foreach (var pickObject in SelectedObjects)
                    {
                        if (pickOriginMouse != Point.Empty)
                        {
                            float posX = 0;
                            float posY = 0;
                            float posZ = 0;

                            if (pickAxis == PickAxis.X)
                                posX = pickMouse.X;
                            if (pickAxis == PickAxis.Y)
                                posY = pickMouse.Y;
                            if (pickAxis == PickAxis.All)
                            {
                                posX = pickMouse.X;
                                posY = pickMouse.Y;
                            }

                            pickObject.PickTranslate(posX, posY, posZ);
                        }
                    }
                }
                if (pickAction == PickAction.Rotate)
                {
                    foreach (var pickObject in SelectedObjects)
                    {
                        if (pickOriginMouse != Point.Empty)
                        {
                            float rotX = 0;
                            float rotY = 0;
                            float rotZ = 0;

                            if (pickAxis == PickAxis.X)
                                rotX = pickMouse.X * -0.015625f;
                            if (pickAxis == PickAxis.Y)
                                rotY = pickMouse.Y;
                            if (pickAxis == PickAxis.All)
                            {
                                rotX = pickMouse.X * -0.015625f;
                              //  rotY = pickMouse.Y;
                            }

                            pickObject.PickRotate(rotX, rotY, rotZ);
                        }
                    }
                }

                pickOriginMouse = temp;

                glControl1.Invalidate();
            }

            if (mouseDown && !isPicked)
            {
                RenderEditor();

                var temp = e.Location;
                var curPos = OpenGLHelper.convertScreenToWorldCoords(temp.X, temp.Y);
                var prevPos = OpenGLHelper.convertScreenToWorldCoords(pickOriginMouse.X, pickOriginMouse.Y);

                GL.PopMatrix();

                DrawSelectionBox(prevPos, curPos);
            }
        }
        
        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                if (isPicked)
                    pickAction = PickAction.Rotate;
            }
            if (e.KeyCode == Keys.G)
            {
                if (isPicked)
                    pickAction = PickAction.Translate;
            }
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.Invalidate();
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Viewport2D
            // 
            this.Name = "Viewport2D";
            this.Size = new System.Drawing.Size(405, 404);
            this.ResumeLayout(false);

        }
    }
}
