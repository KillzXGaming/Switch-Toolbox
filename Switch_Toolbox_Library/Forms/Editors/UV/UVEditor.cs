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

namespace Switch_Toolbox.Library.Forms
{
    public partial class UVEditor : UserControl
    {
        public UVEditor()
        {
            InitializeComponent();

            comboBox1.Items.Add(0);
            comboBox1.Items.Add(1);
            comboBox1.Items.Add(2);

            comboBox1.SelectedIndex = 0;
            barSlider1.Value = 50;
        }

        public class ActiveTexture
        {
            public Vector2 UVScale = new Vector2(1);
            public Vector2 UVTranslate = new Vector2(0);
            public float UVRotate = 0;
            public STGenericTexture texture;
            public int UvChannelIndex;
            public int mapMode = 0;
            public int wrapModeS = 1;
            public int wrapModeT = 1;
            public int minFilter = 3;
            public int magFilter = 2;
            public int mipDetail = 6;
            public uint texWidth = 0;
            public uint texHeight = 0;
        }
        public ActiveTexture activeTexture = new ActiveTexture();

        public float brightness = 0.5f; //To see uv maps easier
        public int UvChannelIndex = 0;
        public STGenericMaterial ActiveMaterial;

        public List<STGenericObject> ActiveObjects = new List<STGenericObject>();

        public List<ActiveTexture> Textures = new List<ActiveTexture>();

        bool IsSRTLoaded = false;
        public void Reset()
        {
            barSlider1.Value = (int)(brightness * 100);

            scaleXUD.Value = 1;
            scaleYUD.Value = 1;
            transXUD.Value = 0;
            transYUD.Value = 0;

            IsSRTLoaded = false;

            comboBox2.Items.Clear();

            if (RenderTools.defaultTex != null)
                texid = RenderTools.defaultTex.RenderableTex.TexID;

            foreach (var item in Textures)
                comboBox2.Items.Add(item.texture.Text);

            if (comboBox2.Items.Count > 0)
                comboBox2.SelectedIndex = 0;
        }

        public int texid;
        public void DrawUVs(List<STGenericObject> genericObjects)
        {
            foreach (var genericObject in genericObjects)
            {
                int divisions = 4;
                int lineWidth = 1;

                Color uvColor = Color.LightGreen;
                Color gridColor = Color.Black;



                List<int> f = genericObject.lodMeshes[0].getDisplayFace();

                for (int v = 0; v < genericObject.lodMeshes[0].displayFaceSize; v += 3)
                {
                    if (genericObject.lodMeshes[0].displayFaceSize < 3 ||
                         genericObject.vertices.Count < 3)
                        return;

                    Vector2 v1 = new Vector2(0);
                    Vector2 v2 = new Vector2(0);
                    Vector2 v3 = new Vector2(0);

                    if (f.Count < (v + 2) && genericObject.vertices.Count > f[v + 2])
                    {
                        if (UvChannelIndex == 0)
                        {
                            v1 = genericObject.vertices[f[v]].uv0;
                            v2 = genericObject.vertices[f[v + 1]].uv0;
                            v3 = genericObject.vertices[f[v + 2]].uv0;
                        }
                        if (UvChannelIndex == 1)
                        {
                            v1 = genericObject.vertices[f[v]].uv1;
                            v2 = genericObject.vertices[f[v + 1]].uv1;
                            v3 = genericObject.vertices[f[v + 2]].uv1;
                        }
                        if (UvChannelIndex == 2)
                        {
                            v1 = genericObject.vertices[f[v]].uv2;
                            v2 = genericObject.vertices[f[v + 1]].uv2;
                            v3 = genericObject.vertices[f[v + 2]].uv2;
                        }

                        v1 = new Vector2(v1.X, 1 - v1.Y);
                        v2 = new Vector2(v2.X, 1 - v2.Y);
                        v3 = new Vector2(v3.X, 1 - v3.Y);

                        DrawUVTriangleAndGrid(v1, v2, v3, divisions, uvColor, lineWidth, gridColor);
                    }
                }
            }
        }
        private void DrawUVTriangleAndGrid(Vector2 v1, Vector2 v2, Vector2 v3, int divisions,
            Color uvColor, int lineWidth, Color gridColor)
        {
            GL.UseProgram(0);

            float bounds = 1;
            Vector2 scaleUv = activeTexture.UVScale * new Vector2(2);
            Vector2 transUv = activeTexture.UVTranslate - new Vector2(1f);

            //Disable textures so they don't affect color
            GL.Disable(EnableCap.Texture2D);
            DrawUvTriangle(v1, v2, v3, uvColor, scaleUv, transUv);

            // Draw Grid
            GL.Color3(gridColor);
            //  DrawHorizontalGrid(divisions, bounds, scaleUv);
            // DrawVerticalGrid(divisions, bounds, scaleUv);
        }

        private static void DrawUvTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Color uvColor, Vector2 scaleUv, Vector2 transUv)
        {
            GL.Color3(uvColor);
            GL.Begin(PrimitiveType.Lines);
            GL.LineWidth(3);
            GL.Vertex2(v1 * scaleUv + transUv);
            GL.Vertex2(v2 * scaleUv + transUv);
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            GL.LineWidth(3);
            GL.Vertex2(v2 * scaleUv + transUv);
            GL.Vertex2(v3 * scaleUv + transUv);
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            GL.LineWidth(3);
            GL.Vertex2(v3 * scaleUv + transUv);
            GL.Vertex2(v1 * scaleUv + transUv);
            GL.End();
        }

        private void SetupRendering(float lineWidth)
        {
            // Go to 2D
            GL.Viewport(0, 0, gL_ControlLegacy2D1.Width, gL_ControlLegacy2D1.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            float aspect = (float)gL_ControlLegacy2D1.Width / (float)gL_ControlLegacy2D1.Height;
            GL.Ortho(-aspect, aspect, -1, 1, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.LineWidth(lineWidth);

            // Draw over everything
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
        }

        private static void DrawVerticalGrid(int divisions, float bounds, Vector2 scaleUv)
        {
            int verticalCount = divisions;
            for (int i = 0; i < verticalCount * bounds; i++)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(new Vector2((1.0f / verticalCount) * i, -bounds) * scaleUv);
                GL.Vertex2(new Vector2((1.0f / verticalCount) * i, bounds) * scaleUv);
                GL.End();
            }
        }

        private static void DrawHorizontalGrid(int divisions, float bounds, Vector2 scaleUv)
        {
            int horizontalCount = divisions;
            for (int i = 0; i < horizontalCount * bounds; i++)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(new Vector2(-bounds, (1.0f / horizontalCount) * i) * scaleUv);
                GL.Vertex2(new Vector2(bounds, (1.0f / horizontalCount) * i) * scaleUv);
                GL.End();
            }
        }

        int PlaneSize = 1;
        float PosX;
        float PosY;
        float ZoomValue = 1;

        Point MouseDownPos;

        private float FindHCF(float m, float n)
        {
            float temp, remainder;
            if (m < n)
            {
                temp = m;
                m = n;
                n = temp;
            }
            while (true)
            {
                remainder = m % n;
                if (remainder == 0)
                    return n;
                else
                    m = n;
                n = remainder;
            }
        }

        private void gL_ControlLegacy2D1_Paint(object sender, PaintEventArgs e)
        {
            if (ActiveObjects.Count <= 0 || ActiveMaterial == null || Runtime.OpenTKInitialized == false)
                return;

            SetupRendering(1);

            gL_ControlLegacy2D1.MakeCurrent();

            GL.ClearColor(System.Drawing.Color.FromArgb(40, 40, 40));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.Texture2D);

            //This usually won't be seen unless the textures aren't repeating much
            DrawBackdrop();

            float PlaneScaleX = 0.5f;
            float PlaneScaleY = 0.5f;

            if (activeTexture.texWidth != 0 && activeTexture.texHeight != 0)
            {
                PlaneScaleX = (float)gL_ControlLegacy2D1.Width / (float)activeTexture.texWidth;
                PlaneScaleY = (float)gL_ControlLegacy2D1.Height / (float)activeTexture.texHeight;
            }



            //Now do the plane with uvs
            GL.PushMatrix();
            GL.Scale(PlaneScaleY * ZoomValue, -PlaneScaleX * ZoomValue, 1);
            GL.Translate(PosX, PosY, 0);
            GL.Rotate(180, 1, 0, 0);

            if (activeTexture.texture != null)
            {
                //Draws a textured plan for our uvs to show on
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, texid);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)STGenericMatTexture.wrapmode[activeTexture.wrapModeS]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)STGenericMatTexture.wrapmode[activeTexture.wrapModeT]);
                //     GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)STGenericMatTexture.wrapmode[activeTexture.wrapModeS]);
                //   GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)STGenericMatTexture.wrapmode[activeTexture.wrapModeT]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)STGenericMatTexture.minfilter[activeTexture.minFilter]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)STGenericMatTexture.magfilter[activeTexture.magFilter]);

            }

            //Params include Amount to repeat
            DrawTexturedPlane(5);

            DrawUVs(ActiveObjects);

            GL.PopMatrix();

            gL_ControlLegacy2D1.SwapBuffers();
        }
        private void DrawTexturedPlane(float scale)
        {
            Vector2 scaleCenter = new Vector2(0.5f, 0.5f);

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

            int brightnessScale = (int)(brightness * 255);

            GL.Begin(PrimitiveType.Quads);
            GL.Color3(brightness, brightness, brightness);
            GL.TexCoord2(TexCoords[0]);
            GL.Vertex2(Positions[0]);
            GL.TexCoord2(TexCoords[1]);
            GL.Vertex2(Positions[1]);
            GL.TexCoord2(TexCoords[2]);
            GL.Vertex2(Positions[2]);
            GL.TexCoord2(TexCoords[3]);
            GL.Vertex2(Positions[3]);
        }
        private void DrawBackdrop()
        {
            //Background
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.FromArgb(40, 40, 40));
            GL.TexCoord2(1, 1);
            GL.Vertex2(PlaneSize, -PlaneSize);
            GL.TexCoord2(0, 1);
            GL.Vertex2(-PlaneSize, -PlaneSize);
            GL.TexCoord2(0, 0);
            GL.Vertex2(-PlaneSize, PlaneSize);
            GL.TexCoord2(1, 0);
            GL.Vertex2(PlaneSize, PlaneSize);
            GL.End();
        }

        private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0 && ZoomValue > 0) ZoomValue += 0.1f;
            if (e.Delta < 0 && ZoomValue < 30 && ZoomValue > 0.2) ZoomValue -= 0.1f;

            Console.WriteLine("ZoomValue " + ZoomValue);

            gL_ControlLegacy2D1.Invalidate();
        }

        private void gL_ControlLegacy2D1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownPos = MousePosition;
            }
        }

        private void gL_ControlLegacy2D1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            Point temp = Control.MousePosition;
            Point res = new Point(MouseDownPos.X - temp.X, MouseDownPos.Y - temp.Y);

            PosX -= res.X * 0.001f;
            PosY -= res.Y * 0.001f;

            gL_ControlLegacy2D1.Invalidate();

            MouseDownPos = temp;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex >= 0)
            {
                activeTexture = Textures[comboBox2.SelectedIndex];
                UvChannelIndex = activeTexture.UvChannelIndex;

                scaleXUD.Value = (decimal)activeTexture.UVScale.X;
                scaleYUD.Value = (decimal)activeTexture.UVScale.Y;
                transXUD.Value = (decimal)activeTexture.UVTranslate.X;
                transYUD.Value = (decimal)activeTexture.UVTranslate.Y;

                var texture = activeTexture.texture;

                if (texture.RenderableTex == null)
                    texture.LoadOpenGLTexture();

                texid = texture.RenderableTex.TexID;
                activeTexture.texWidth = texture.Width;
                activeTexture.texHeight = texture.Height;

                gL_ControlLegacy2D1.Invalidate();

                IsSRTLoaded = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                UvChannelIndex = comboBox1.SelectedIndex;
                gL_ControlLegacy2D1.Invalidate();
            }
        }

        private void OnNumbicValueSRT_ValueChanged(object sender, EventArgs e)
        {
        }

        private void btnApplyTransform_Click(object sender, EventArgs e)
        {
            foreach (var shape in ActiveObjects)
            {
                Vector2 Scale = new Vector2((float)scaleXUD.Value, (float)scaleYUD.Value);
                Vector2 Translate = new Vector2((float)transXUD.Value, (float)transYUD.Value);

                shape.TransformUVs(Translate, Scale, UvChannelIndex);
                shape.UpdateVertexData();
            }

            scaleXUD.Value = 1;
            scaleYUD.Value = 1;
            transXUD.Value = 0;
            transYUD.Value = 0;

            gL_ControlLegacy2D1.Invalidate();
        }

        private void gL_ControlLegacy2D1_Resize(object sender, EventArgs e)
        {
            gL_ControlLegacy2D1.Invalidate();
        }

        private void barSlider1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void barSlider1_ValueChanged(object sender, EventArgs e)
        {
            brightness = (float)barSlider1.Value / 100;
            gL_ControlLegacy2D1.Invalidate();
        }
    }
}