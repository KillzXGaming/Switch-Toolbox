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

namespace Switch_Toolbox.Library.Forms.test
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
            barSlider1.Value = 0;
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

        public int brightness = 50; //To see uv maps easier
        public int UvChannelIndex = 0;
        public STGenericMaterial ActiveMaterial;

        public List<STGenericObject> ActiveObjects = new List<STGenericObject>();

        public List<ActiveTexture> Textures = new List<ActiveTexture>();

        bool IsSRTLoaded = false;
        public void Reset()
        {
            barSlider1.Value = brightness;

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

            gL_ControlLegacy2D1.MakeCurrent();

            SetupRendering(1);

            GL.ClearColor(System.Drawing.Color.FromArgb(40, 40, 40));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.Texture2D);

            //This usually won't be seen unless the textures aren't repeating much
            DrawBackdrop();

            float PlaneScaleX = 0.5f;
            float PlaneScaleY = 0.5f;

            float HalfWidth = (float)gL_ControlLegacy2D1.Width / 2.0f;
            float HalfHeight = (float)gL_ControlLegacy2D1.Height / 2.0f;

            if (activeTexture.texWidth != 0 && activeTexture.texHeight != 0)
            {
                PlaneScaleX = (float)gL_ControlLegacy2D1.Width / (float)activeTexture.texWidth;
                PlaneScaleY = (float)gL_ControlLegacy2D1.Height / (float)activeTexture.texHeight;
            }



            //Now do the plane with uvs
            GL.PushMatrix();
            GL.Scale(PlaneScaleY * ZoomValue, -PlaneScaleX * ZoomValue, 1);
            GL.Translate(PosX, PosY, 0);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Texture);
            GL.LoadIdentity();

            bool UseBackground = true;

            if (UseBackground)
            {
                float BrightnessAmount = (float)brightness / 100f;
                int brightnessScale = (int)(BrightnessAmount * 255);

                GL.PushAttrib(AttribMask.TextureBit);

                var background = new GenericBitmapTexture(Properties.Resources.CheckerBackground);
                background.LoadOpenGLTexture();

                //Draws a textured plan for our uvs to show on
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, background.RenderableTex.TexID);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                GL.Begin(PrimitiveType.Quads);
                //      GL.Color3(brightnessScale, brightnessScale, brightnessScale);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex2(-HalfWidth, -HalfHeight);
                GL.TexCoord2(PlaneScaleX, 0.0f);
                GL.Vertex2(HalfWidth, -HalfHeight);
                GL.TexCoord2(PlaneScaleX, PlaneScaleY);
                GL.Vertex2(HalfWidth, HalfHeight);
                GL.TexCoord2(0.0f, PlaneScaleY);
                GL.Vertex2(-HalfWidth, HalfHeight);
                GL.End();
                GL.PopAttrib();
            }

            if (activeTexture.texture != null)
            {
                //     DrawTexturedPlane(1, activeTexture.texWidth, activeTexture.texHeight);
            }

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            DrawUVs(ActiveObjects);

            GL.PopMatrix();

            gL_ControlLegacy2D1.SwapBuffers();
        }


        //Aspect ratio stuff from https://github.com/libertyernie/brawltools/blob/40d7431b1a01ef4a0411cd69e51411bd581e93e2/BrawlLib/System/Windows/Controls/TexCoordRenderer.cs
        private void DrawTexturedPlane(float scale, uint TextureWidth, uint TextureHeight)
        {
            //Draws a textured plan for our uvs to show on
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, texid);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)STGenericMatTexture.wrapmode[activeTexture.wrapModeS]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)STGenericMatTexture.wrapmode[activeTexture.wrapModeT]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)STGenericMatTexture.minfilter[activeTexture.minFilter]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)STGenericMatTexture.magfilter[activeTexture.magFilter]);


            float HalfWidth = (float)gL_ControlLegacy2D1.Width / 2.0f;
            float HalfHeight = (float)gL_ControlLegacy2D1.Height / 2.0f;

            Vector2 scaleCenter = new Vector2(0.5f, 0.5f);

            float[] texCoord = new float[8];

            float tAspect = (float)TextureWidth / TextureHeight;
            float wAspect = (float)gL_ControlLegacy2D1.Width / gL_ControlLegacy2D1.Height;

            Vector2 topLeft = new Vector2();
            Vector2 bottomRight = new Vector2();

            float xCorrect = 1.0f, yCorrect = 1.0f;
            if (tAspect > wAspect)
            {
                //Texture is wider, use horizontal fit
                //X touches the edges of the window, Y has top and bottom padding

                texCoord[0] = texCoord[6] = 0.0f;
                texCoord[2] = texCoord[4] = 1.0f;

                texCoord[1] = texCoord[3] = (yCorrect = tAspect / wAspect) / 2.0f + 0.5f;
                texCoord[5] = texCoord[7] = 1.0f - texCoord[1];

                bottomRight = new Vector2(HalfWidth, (((float)gL_ControlLegacy2D1.Height - ((float)gL_ControlLegacy2D1.Width /
                    TextureWidth * TextureHeight)) / (float)gL_ControlLegacy2D1.Height / 2.0f - 0.5f) *
                    (float)gL_ControlLegacy2D1.Height);

                topLeft = new Vector2(-HalfHeight, -bottomRight.Y);
            }
            else
            {
                //Window is wider, use vertical fit
                //Y touches the edges of the window, X has left and right padding

                texCoord[1] = texCoord[3] = 1.0f;
                texCoord[5] = texCoord[7] = 0.0f;

                //X
                texCoord[2] = texCoord[4] = (xCorrect = wAspect / tAspect) / 2.0f + 0.5f;
                texCoord[0] = texCoord[6] = 1.0f - texCoord[2];

                bottomRight = new Vector2(1.0f - (((float)gL_ControlLegacy2D1.Width -
                    ((float)gL_ControlLegacy2D1.Height / TextureHeight * TextureWidth)) / gL_ControlLegacy2D1.Width / 2.0f - 0.5f) *
                    (float)gL_ControlLegacy2D1.Width, -HalfHeight);

                topLeft = new Vector2(-bottomRight.X, HalfHeight);
            }

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(texCoord[0], texCoord[1]);
            GL.Vertex2(-HalfWidth, -HalfHeight);
            GL.TexCoord2(texCoord[2], texCoord[3]);
            GL.Vertex2(HalfWidth, -HalfHeight);
            GL.TexCoord2(texCoord[4], texCoord[5]);
            GL.Vertex2(HalfWidth, HalfHeight);
            GL.TexCoord2(texCoord[6], texCoord[7]);
            GL.Vertex2(-HalfWidth, HalfHeight);
            GL.End();



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
            if (e.Delta < 0 && ZoomValue < 30 && ZoomValue > 0.1) ZoomValue -= 0.1f;

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

        private void stTrackBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void barSlider1_Scroll(object sender, ScrollEventArgs e)
        {
            brightness = barSlider1.Value;
            gL_ControlLegacy2D1.Invalidate();
        }
    }
}
