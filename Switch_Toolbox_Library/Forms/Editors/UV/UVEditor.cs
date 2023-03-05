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

namespace Toolbox.Library.Forms
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
            barSlider1.Value = barSlider1.Maximum;
        }

        public class ActiveTexture
        {
            public STGenericMatTexture MatTexture;

            public Vector2 UVScale = new Vector2(1);
            public Vector2 UVTranslate = new Vector2(0);
            public float UVRotate = 0;
            public int UvChannelIndex;
            public int TextureIndex = -1;

            public STTextureWrapMode WrapModeS = STTextureWrapMode.Repeat;
            public STTextureWrapMode WrapModeT = STTextureWrapMode.Repeat;
            public STTextureWrapMode WrapModeW = STTextureWrapMode.Clamp;

            public STTextureMinFilter MinFilter = STTextureMinFilter.Linear;
            public STTextureMagFilter MagFilter = STTextureMagFilter.Linear;

            public uint Width = 0;
            public uint Height = 0;
        }
        public ActiveTexture activeTexture = new ActiveTexture();

        public float brightness = 1.0f; //To see uv maps easier
        public int UvChannelIndex = 0;

        public List<STGenericObject> Objects = new List<STGenericObject>();
        public List<DrawableContainer> Containers = new List<DrawableContainer>();

        public List<STGenericObject> ActiveObjects
        {
            get
            {
                List<STGenericObject> objects = new List<STGenericObject>();
                for (int i = 0; i < Objects.Count; i++)
                    objects.Add(Objects[i]);

                return objects;
            }
        }

        public List<STGenericMaterial> Materials = new List<STGenericMaterial>();
        public List<STGenericTexture> Textures = new List<STGenericTexture>();

        public List<ActiveTexture> ChannelTextures = new List<ActiveTexture>();

        public STGenericMaterial ActiveMaterial;

        public void ResetContainerList()
        {
            for (int i =0; i < Containers.Count; i++)
            {
                drawableContainerCB.Items.Add(Containers[i].Name);
            }

            drawableContainerCB.SelectedIndex = 0;
        }

        public int texid;

        bool IsSRTLoaded = false;
        public void Reset()
        {
            scaleXUD.Value = 1;
            scaleYUD.Value = 1;
            transXUD.Value = 0;
            transYUD.Value = 0;

            IsSRTLoaded = false;

            meshesCB.Items.Clear();

            if (RenderTools.defaultTex != null)
                texid = RenderTools.defaultTex.RenderableTex.TexID;

            foreach (var mat in Materials)
            {
                meshesCB.Items.Add(mat.Text);
            }

            if (meshesCB.Items.Count > 0)
                meshesCB.SelectedIndex = 0;
        }

        private void BindTexture()
        {
            if (activeTexture.TextureIndex != -1)
            {
                //Draws a textured plan for our uvs to show on
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, texid);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)STGenericMatTexture.wrapmode[activeTexture.WrapModeS]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)STGenericMatTexture.wrapmode[activeTexture.WrapModeT]);
                //     GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)STGenericMatTexture.wrapmode[activeTexture.wrapModeS]);
                //   GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)STGenericMatTexture.wrapmode[activeTexture.wrapModeT]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)STGenericMatTexture.minfilter[activeTexture.MinFilter]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)STGenericMatTexture.magfilter[activeTexture.MagFilter]);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (textureCB.SelectedIndex >= 0)
            {
                activeTexture = ChannelTextures[textureCB.SelectedIndex];
                UvChannelIndex = activeTexture.UvChannelIndex;

                scaleXUD.Value = (decimal)activeTexture.UVScale.X;
                scaleYUD.Value = (decimal)activeTexture.UVScale.Y;
                transXUD.Value = (decimal)activeTexture.UVTranslate.X;
                transYUD.Value = (decimal)activeTexture.UVTranslate.Y;

                uvViewport1.ActiveTextureMap = activeTexture.MatTexture;

                var texture = Textures[activeTexture.TextureIndex];

                if (texture.RenderableTex == null)
                    texture.LoadOpenGLTexture();

                texid = texture.RenderableTex.TexID;
                activeTexture.Width = texture.Width;
                activeTexture.Height = texture.Height;

                uvViewport1.UpdateViewport();

                IsSRTLoaded = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                uvViewport1.UvChannelIndex = comboBox1.SelectedIndex;
                uvViewport1.UpdateViewport();
            }
        }

        private void barSlider1_Scroll(object sender, EventArgs e)
        {
        }

        private void barSlider1_ValueChanged(object sender, EventArgs e)
        {
            brightness = (float)barSlider1.Value / 100;
            uvViewport1.Brightness = brightness;
            uvViewport1.UpdateViewport();
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
                shape.SaveVertexBuffer();
                shape.UpdateVertexData();
            }

            scaleXUD.Value = 1;
            scaleYUD.Value = 1;
            transXUD.Value = 0;
            transYUD.Value = 0;

            uvViewport1.UpdateViewport();
        }

        private void meshesCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (meshesCB.SelectedIndex >= 0)
            {

                ActiveMaterial = Materials[meshesCB.SelectedIndex];

                uvViewport1.ActiveObjects.Clear();
                foreach (var obj in Objects)
                {
                    if (obj.GetMaterial() == ActiveMaterial)
                        uvViewport1.ActiveObjects.Add(obj);

                    foreach (var p in obj.PolygonGroups)
                    {
                        if (p.Material == ActiveMaterial && !uvViewport1.ActiveObjects.Contains(obj))
                            uvViewport1.ActiveObjects.Add(obj);
                    }
                }

                ChannelTextures.Clear();
                Textures.Clear();
                textureCB.Items.Clear();

                foreach (var texMap in ActiveMaterial.TextureMaps)
                {
                    var texture = texMap.GetTexture();
                    if (texture != null && !Textures.Contains(texture))
                    {
                        textureCB.Items.Add(texture.Text);

                        Textures.Add(texture);
                        ActiveTexture tex = new ActiveTexture();
                        tex.MatTexture = texMap;
                        tex.TextureIndex = Textures.IndexOf(texture);
                        tex.Width = texture.Width;
                        tex.Height = texture.Height;
                        tex.MagFilter = texMap.MagFilter;
                        tex.MinFilter = texMap.MinFilter;
                        tex.UvChannelIndex = 0;
                        ChannelTextures.Add(tex);
                    }
                }

                if (textureCB.Items.Count > 0)
                    textureCB.SelectedIndex = 0;

                uvViewport1.UpdateViewport();
            }
        }

        private void barSlider1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void stButton1_Click(object sender, EventArgs e)
        {

        }

        private void drawableContainerCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drawableContainerCB.SelectedIndex >= 0)
            {
                int index = drawableContainerCB.SelectedIndex;
                DrawableContainer container = Containers[index];

                Materials.Clear();
                Textures.Clear();
                Objects.Clear();

                for (int i = 0; i < container.Drawables.Count; i++)
                {
                    if (container.Drawables[i] is IMeshContainer && container.Drawables[i].Visible)
                    {
                        for (int m = 0; m < ((IMeshContainer)container.Drawables[i]).Meshes.Count; m++)
                        {
                            var mesh = ((IMeshContainer)container.Drawables[i]).Meshes[m];
                            Objects.Add(mesh);

                            if (mesh.PolygonGroups.Count > 0)
                            {
                                foreach (var group in mesh.PolygonGroups) {
                                    var mat = group.Material;
                                    if (mat != null && !Materials.Contains(mat))
                                        Materials.Add(mat);
                                }
                            }
                            else if (mesh.GetMaterial() != null)
                            {
                                var mat = mesh.GetMaterial();
                                if (!Materials.Contains(mat))
                                {
                                    Materials.Add(mat);
                                }
                            }
                        }
                    }
                    if (container.Drawables[i] is Rendering.GenericModelRenderer && container.Drawables[i].Visible)
                    {
                        for (int m = 0; m < ((Rendering.GenericModelRenderer)container.Drawables[i]).Meshes.Count; m++)
                        {
                            var mesh = ((Rendering.GenericModelRenderer)container.Drawables[i]).Meshes[m];
                            if (mesh.GetMaterial() != null)
                            {
                                Objects.Add(mesh);
                                var mat = mesh.GetMaterial();
                                if (!Materials.Contains(mat))
                                {
                                    Materials.Add(mat);
                                }
                            }
                        }
                    }
                }

                Reset();
                Refresh();
                uvViewport1.UpdateViewport();
            }
        }
    }
}