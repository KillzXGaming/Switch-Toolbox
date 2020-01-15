using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model;
using FirstPlugin;
using Toolbox.Library;
using Toolbox.Library.Forms;

namespace FirstPlugin.CtrLibrary.Forms
{
    public partial class BCHTextureMapEditor : UserControl, IMaterialLoader
    {
        public EventHandler OnTextureSelected;

        public H3DMaterialWrapper ActiveMaterial;

        private int SelectedIndex = -1;

        public BCHTextureMapEditor()
        {
            InitializeComponent();

            stPanel4.BackColor = FormThemes.BaseTheme.TabPageActive;

            uvViewport1.UseGrid = false;

            stDropDownPanel7.ResetColors();
            stDropDownPanel5.ResetColors();
            stDropDownPanel4.ResetColors();

            bumpMapTypeCB.Items.Add("Bump Map");
            bumpMapTypeCB.Items.Add("Tangent Map");
            bumpMapTypeCB.SelectedIndex = 0;

            bumpMapCB.Items.Add("None");
            bumpMapCB.Items.Add("Texture 0");
            bumpMapCB.Items.Add("Texture 1");
            bumpMapCB.Items.Add("Texture 2");
            bumpMapCB.SelectedIndex = 0;

            uvSetCB.Items.Add(0);
            uvSetCB.Items.Add(1);
            uvSetCB.Items.Add(2);
            uvSetCB.SelectedIndex = 0;

            transformModeCB.LoadEnum(typeof(H3DTextureTransformType));
            transformMethodCB.LoadEnum(typeof(H3DTextureMappingType));

            wrapModeUCB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureWrap));
            wrapModeVCB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureWrap));
            magFilterCB.LoadEnum(typeof(H3DTextureMagFilter));
            minFilterCB.LoadEnum(typeof(H3DTextureMinFilter));

            OnTextureSelected += TextureSelectedIndexChanged;

            ResetSliders();
        }

        private void ResetSliders()
        {
            translateXUD.SetTheme();
            translateYUD.SetTheme();
            scaleXUD.SetTheme();
            scaleYUD.SetTheme();
            rotateUD.SetTheme();
            cameraIndexUD.SetTheme();

            translateXUD.Value = 0;
            translateYUD.Value = 0;
            scaleXUD.Value = 1;
            scaleYUD.Value = 1;
            rotateUD.Value = 0;
            cameraIndexUD.Value = 0;
        }

        public void LoadMaterial(H3DMaterialWrapper material)
        {
            ActiveMaterial = material;
            var matParams = material.Material.MaterialParams;
            chkRecalculateZ.Checked = matParams.FragmentFlags.HasFlag(
                H3DFragmentFlags.IsBumpRenormalizeEnabled);


            Thread Thread = new Thread((ThreadStart)(() =>
            {
                Bitmap image = Properties.Resources.TextureError;
                for (int i = 0; i < material.TextureMaps?.Count; i++)
                {
                    var texMap = material.TextureMaps[i];
                    var texture = texMap.GetTexture();
                    if (texture != null)
                    {
                        try
                        {
                            image = texture.GetBitmap();
                        }
                        catch
                        {
                            image = Properties.Resources.TextureError;
                        }
                    }
                    else if (texMap.Name != string.Empty)
                        image = Properties.Resources.TextureError;


                    if (i == 0) SetPanel(stPanel1, image);
                    if (i == 1) SetPanel(stPanel2, image);
                    if (i == 2) SetPanel(stPanel3, image);
                }
            }));
            Thread.Start();

            OnTextureSelected.Invoke(null, EventArgs.Empty);
        }

        private void SetPanel(LayoutBXLYT.ImagePanel panel, Bitmap image)
        {
            panel.Invoke((MethodInvoker)delegate {
                panel.Enabled = true;
                panel.BackgroundImage = image;
                panel.Refresh();
            });
        }

        private void UpdatePanels(LayoutBXLYT.ImagePanel[] panels, int selectedIndex)
        {
            stPanel1.Selected = false;
            stPanel2.Selected = false;
            stPanel3.Selected = false;
            arrowLeft.Enabled = false;
            arrowRight.Enabled = false;

            SelectedIndex = selectedIndex;
            if (SelectedIndex == 0)
            {
                stPanel1.Selected = true;

                if (ActiveMaterial.TextureMaps.Count > 1)
                    arrowRight.Enabled = true;
            }
            if (SelectedIndex == 1)
            {
                stPanel2.Selected = true;
                arrowLeft.Enabled = true;

                if (ActiveMaterial.TextureMaps.Count > 2)
                    arrowRight.Enabled = true;
            }
            if (SelectedIndex == 2)
            {
                stPanel3.Selected = true;
                arrowLeft.Enabled = true;
                arrowRight.Enabled = false;
            }

            for (int i = 0; i < panels.Length; i++)
            {
                panels[i].BorderColor = FormThemes.BaseTheme.DisabledBorderColor;
                panels[i].BorderColor = FormThemes.BaseTheme.DisabledBorderColor;
                if (panels[i].Selected)
                    panels[i].BorderColor = Color.Cyan;

                panels[i].Refresh();
            }

            ReloadButtons();
        }

        private void TextureSelectedIndexChanged(object sender, EventArgs e)
        {
            translateXUD.Value = 0;
            translateYUD.Value = 0;
            scaleXUD.Value = 1;
            scaleYUD.Value = 1;
            rotateUD.Value = 0;

            if (SelectedIndex != -1 && SelectedIndex < ActiveMaterial?.TextureMaps?.Count) {
                var texMap = ActiveMaterial.TextureMaps[SelectedIndex];
                nameTB.Text = texMap.Name;
                uvViewport1.ActiveTextureMap = texMap;
                var matParams = ActiveMaterial.Material.MaterialParams;

                if (ActiveMaterial.Material.TextureMappers?.Length > SelectedIndex)
                {
                    var mapper = ActiveMaterial.Material.TextureMappers[SelectedIndex];
                    wrapModeUCB.SelectedItem = mapper.WrapU;
                    wrapModeVCB.SelectedItem = mapper.WrapV;
                    magFilterCB.SelectedItem = mapper.MagFilter;
                    minFilterCB.SelectedItem = mapper.MinFilter;
                    lodBiasUD.Value = mapper.LODBias;
                }
                if (matParams.TextureCoords?.Length > SelectedIndex)
                {
                    var transform = matParams.TextureCoords[SelectedIndex];
                    translateXUD.Value = transform.Translation.X;
                    translateYUD.Value = transform.Translation.Y;
                    scaleXUD.Value = transform.Scale.X;
                    scaleYUD.Value = transform.Scale.Y;
                    rotateUD.Value = transform.Rotation;

                    transformMethodCB.SelectedItem = transform.MappingType;
                    transformModeCB.SelectedItem = transform.TransformType;
                    cameraIndexUD.Value = transform.ReferenceCameraIndex;
                }

                var texture = texMap.GetTexture();
                if (texture != null) {
                    texInfoLabel.Text = $"Format : {texture.Format}\n" +
                    $"Width  : {texture.Width}\n" +
                    $"Height : {texture.Height}\n" +
                    $"Size : {texture.DataSize}\n";
                }

                //Load mapped meshes
                uvViewport1.ActiveObjects.Clear();

                foreach (var mesh in ActiveMaterial.FindMappedMeshes())
                    uvViewport1.ActiveObjects.Add(mesh);

                uvViewport1.UpdateViewport();
            }
            else
            {
               
                nameTB.Text = "";
                texInfoLabel.Text = "";
                uvViewport1.ActiveTextureMap = null; ResetSliders();

            }
        }

        private void ReloadButtons()
        {
            if (ActiveMaterial.TextureMaps.Count == 0)
            {
                editBtn.Enabled = false;
                removebtn.Enabled = false;
            }
            else if (ActiveMaterial.TextureMaps.Count == 3)
            {
                editBtn.Enabled = true;
                removebtn.Enabled = true;
                addbtn.Enabled = false;
            }
            else
            {
                editBtn.Enabled = true;
                removebtn.Enabled = true;
                addbtn.Enabled = true;
            }
        }

        private void stPanel1_Click(object sender, EventArgs e)
        {
            if (!stPanel1.Enabled)
                return;

            UpdatePanels(new LayoutBXLYT.ImagePanel[] { stPanel1, stPanel2, stPanel3 }, 0);
            OnTextureSelected?.Invoke(sender, e);
        }

        private void stPanel2_Click(object sender, EventArgs e)
        {
            if (!stPanel2.Enabled)
                return;

            UpdatePanels(new LayoutBXLYT.ImagePanel[] { stPanel1, stPanel2, stPanel3 }, 1);
            OnTextureSelected?.Invoke(sender, e);
        }

        private void stPanel3_Click(object sender, EventArgs e)
        {
            if (!stPanel3.Enabled)
                return;

            UpdatePanels(new LayoutBXLYT.ImagePanel[] { stPanel1, stPanel2, stPanel3 }, 2);
            OnTextureSelected?.Invoke(sender, e);
        }

        private void chkRecalculateZ_CheckedChanged(object sender, EventArgs e) {
            if (chkRecalculateZ.Checked)
                ActiveMaterial.Material.MaterialParams.FragmentFlags |= H3DFragmentFlags.IsBumpRenormalizeEnabled;
            else
                ActiveMaterial.Material.MaterialParams.FragmentFlags &= H3DFragmentFlags.IsBumpRenormalizeEnabled;
        }
    }
}
