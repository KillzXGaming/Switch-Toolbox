using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;

namespace LayoutBXLYT
{
    public partial class PaneMatTextureMapsEditor : EditorPanelBase
    {
        public EventHandler OnTextureSelected;

        public PaneMatTextureMapsEditor()
        {
            InitializeComponent();

            projectionParamsPanel.Visible = false;

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();
            stDropDownPanel3.ResetColors();

            wrapModeUCB.SelectedValueChanged += PropertyChanged;
            wrapModeVCB.SelectedValueChanged += PropertyChanged;
            shrinkCB.SelectedValueChanged += PropertyChanged;
            expandCB.SelectedValueChanged += PropertyChanged;

            OnTextureSelected += TextureSelectedIndexChanged;
        }

        private Dictionary<string, STGenericTexture> textureList;
        private PaneEditor ParentEditor;
        private BxlytMaterial ActiveMaterial;
        private List<Bitmap> Images = new List<Bitmap>();
        private int SelectedIndex = -1;
        private bool Loaded = false;
        private int TexCoordinateCount = 1;

        public void LoadMaterial(BxlytMaterial material, PaneEditor paneEditor, 
            Dictionary<string, STGenericTexture> textures, int numTextureCoordinates = 1)
        {
            TexCoordinateCount = numTextureCoordinates;
            textureList = textures;
            ParentEditor = paneEditor;
            ActiveMaterial = material;

            ReloadTexture();
        }

        private void ReloadTexture()
        {
            Loaded = false;

            ResetImagePanels();
            Images.Clear();
            transformTypeCB.Items.Clear();

            textureNameTB.Text = "";

            for (int i = 0; i < ActiveMaterial.TextureMaps?.Length; i++)
            {
                string name = ActiveMaterial.TextureMaps[i].Name;

                Bitmap bitmap = FirstPlugin.Properties.Resources.TextureError;
                if (textureList.ContainsKey(name))
                    bitmap = textureList[name].GetBitmap();

                Images.Add(bitmap);

                if (i == 0) SetPanel(stPanel1, bitmap);
                if (i == 1) SetPanel(stPanel2, bitmap);
                if (i == 2) SetPanel(stPanel3, bitmap);
            }

            for (int i = 0; i < TexCoordinateCount; i++)
                transformTypeCB.Items.Add((TexGenType)i);

            transformTypeCB.Items.Add(TexGenType.OrthographicProjection);
            transformTypeCB.Items.Add(TexGenType.PaneBasedProjection);
            transformTypeCB.Items.Add(TexGenType.PerspectiveProjection);

            ReloadButtons();

            Loaded = true;
            SelectImage(0);
        }

        private void ReloadButtons()
        {
            if (ActiveMaterial.TextureMaps.Length == 0)
            {
                editBtn.Enabled = false;
                removebtn.Enabled = false;
            }
            else if (ActiveMaterial.TextureMaps.Length == 3)
            {
                editBtn.Enabled = true;
                removebtn.Enabled = true;
                addbtn.Enabled = false;
            }
            else
            {
                textureNameTB.Text = "";
                editBtn.Enabled = true;
                removebtn.Enabled = true;
                addbtn.Enabled = true;
            }
        }

        private void SelectImage(int index)
        {
            if (index == 0) {
                stPanel1_Click(stPanel1, EventArgs.Empty);
                stPanel1.Refresh();
            }
            if (index == 1) {
                stPanel2_Click(stPanel2, EventArgs.Empty);
                stPanel2.Refresh();
            }
            if (index == 2) {
                stPanel3_Click(stPanel3, EventArgs.Empty);
                stPanel3.Refresh();
            }
        }

        private void ResetImagePanels()
        {
            pictureBoxCustom1.Image?.Dispose();
            pictureBoxCustom1.Image = null;

            stPanel1.ResetImage();
            stPanel2.ResetImage();
            stPanel3.ResetImage();

            stPanel1.Enabled = false;
            stPanel2.Enabled = false;
            stPanel3.Enabled = false;

            stPanel1.BorderColor = FormThemes.BaseTheme.DisabledBorderColor;
            stPanel2.BorderColor = FormThemes.BaseTheme.DisabledBorderColor;
            stPanel3.BorderColor = FormThemes.BaseTheme.DisabledBorderColor;

            wrapModeUCB.ResetBind();
            wrapModeVCB.ResetBind();
            shrinkCB.ResetBind();
            expandCB.ResetBind();
            ResetUVTransformUI();
        }

        public override void OnControlClosing()
        {
            foreach (var img in Images)
                img?.Dispose();

            Images.Clear();
        }

        private bool texLoaded = false;
        private void TextureSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded) return;

            texLoaded = false;

            var texMap = ActiveMaterial.TextureMaps[SelectedIndex];
            pictureBoxCustom1.Image = Images[SelectedIndex];

            wrapModeUCB.Bind(typeof(WrapMode), texMap, "WrapModeU");
            wrapModeVCB.Bind(typeof(WrapMode), texMap, "WrapModeV");
            expandCB.Bind(typeof(FilterMode), texMap, "MaxFilterMode");
            shrinkCB.Bind(typeof(FilterMode), texMap, "MinFilterMode");
            wrapModeUCB.SelectedItem = texMap.WrapModeU;
            wrapModeVCB.SelectedItem = texMap.WrapModeV;
            expandCB.SelectedItem = texMap.MaxFilterMode;
            shrinkCB.SelectedItem = texMap.MinFilterMode;
            textureNameTB.Text = texMap.Name;

            if (ActiveMaterial.TextureTransforms?.Length > SelectedIndex)
            {
                var transform = ActiveMaterial.TextureTransforms[SelectedIndex];
                scaleXUD.Value = transform.Scale.X;
                scaleYUD.Value = transform.Scale.Y;
                rotUD.Value = transform.Rotate;
                transXUD.Value = transform.Translate.X;
                transYUD.Value = transform.Translate.Y;
            }
            else
                ResetUVTransformUI();

            if (ActiveMaterial.TexCoordGens?.Length > SelectedIndex)
            {
                var texGen = ActiveMaterial.TexCoordGens[SelectedIndex];
                transformTypeCB.SelectedItem = texGen.Source;
            }

            texLoaded = true;
        }

        private void ResetUVTransformUI()
        {
            texLoaded = false;
            scaleXUD.Value = 1.0f;
            scaleYUD.Value = 1.0f;
            rotUD.Value = 0.0f;
            transXUD.Value = 0.0f;
            transYUD.Value = 0.0f;
            texLoaded = true;
        }

        private void PropertyChanged(object sender, EventArgs e) {
            if (!Loaded || !texLoaded) return;

            ParentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void SetPanel(ImagePanel panel, Bitmap image)
        {
            panel.Enabled = true;
            panel.BackgroundImage = image;
            panel.Refresh();
        }

        private void stPanel1_Click(object sender, EventArgs e)
        {
            if (!stPanel1.Enabled)
                return;

            UpdatePanels(new ImagePanel[] { stPanel1, stPanel2, stPanel3 }, 0);
            OnTextureSelected?.Invoke(sender, e);
        }

        private void UpdatePanels(ImagePanel[] panels, int selectedIndex)
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

                if (ActiveMaterial.TextureMaps.Length > 1)
                    arrowRight.Enabled = true;
            }
            if (SelectedIndex == 1)
            {
                stPanel2.Selected = true;
                arrowLeft.Enabled = true;

                if (ActiveMaterial.TextureMaps.Length > 2)
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

        private void stPanel2_Click(object sender, EventArgs e)
        {
            if (!stPanel2.Enabled)
                return;

            UpdatePanels(new ImagePanel[] { stPanel1, stPanel2, stPanel3 }, 1);
            OnTextureSelected?.Invoke(sender, e);
        }

        private void stPanel3_Click(object sender, EventArgs e)
        {
            if (!stPanel3.Enabled)
                return;

            UpdatePanels(new ImagePanel[] { stPanel1, stPanel2, stPanel3 }, 2);
            OnTextureSelected?.Invoke(sender, e);
        }

        private void addbtn_Click(object sender, EventArgs e)
        {
            if (ActiveMaterial.TextureMaps.Length == 3)
                return;

            Texture_Selector selector = new Texture_Selector();
            selector.LoadTextures(textureList, "", ActiveMaterial.ParentLayout);
            if (selector.ShowDialog() == DialogResult.OK)
            {
                string newTexture = selector.GetSelectedTexture();
                ActiveMaterial.AddTexture(newTexture);

                //Apply to all selected panes
                foreach (BasePane pane in ParentEditor.SelectedPanes)
                {
                    var mat = pane.TryGetActiveMaterial();
                    if (mat != null && mat != ActiveMaterial && mat.TextureMaps?.Length != 3)
                        mat.AddTexture(newTexture);
                }

                ReloadTexture();
                ParentEditor.PropertyChanged?.Invoke(sender, e);
            }

            if (selector.UpdateTextures)
            {
                ParentEditor.UpdateTextureList();
            }
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            if (SelectedIndex != -1)
            {
                var texMap = ActiveMaterial.TextureMaps[SelectedIndex];

                Texture_Selector selector = new Texture_Selector();
                selector.LoadTextures(textureList, texMap.Name, ActiveMaterial.ParentLayout);
                if (selector.ShowDialog() == DialogResult.OK)
                {
                    string newTexture = selector.GetSelectedTexture();
                    texMap.ID = (short)ActiveMaterial.ParentLayout.AddTexture(newTexture);
                    texMap.Name = newTexture;
                    ReloadTexture();
                    ParentEditor.PropertyChanged?.Invoke(sender, e);

                    //Apply to all selected panes
                    foreach (BasePane pane in ParentEditor.SelectedPanes)
                    {
                        var mat = pane.TryGetActiveMaterial();
                        if (mat != null && mat != ActiveMaterial)
                        {
                            if (mat.TextureMaps?.Length > SelectedIndex)
                                mat.TextureMaps[SelectedIndex] = texMap;
                            else
                                mat.AddTexture(newTexture);
                        }
                    }
                }
            }
        }

        private void removebtn_Click(object sender, EventArgs e)
        {
            if (ActiveMaterial.TextureMaps.Length > SelectedIndex && SelectedIndex >= 0)
            {
                ActiveMaterial.RemoveTexture(SelectedIndex);

                //Apply to all selected panes
                foreach (BasePane pane in ParentEditor.SelectedPanes)
                {
                    var mat = pane.TryGetActiveMaterial();
                    if (mat != null && mat != ActiveMaterial && mat.TextureMaps?.Length > SelectedIndex)
                        mat.RemoveTexture(SelectedIndex);
                }
            }

            if (ActiveMaterial.TextureMaps.Length == 0)
            {
                SelectedIndex = -1;
                ResetImagePanels();
                UpdatePanels(new ImagePanel[] { stPanel1, stPanel2, stPanel3 }, -1);
            }
            else
            {
                SelectedIndex = SelectedIndex - 1;
                ReloadTexture();
                SelectImage(SelectedIndex);
                ParentEditor.PropertyChanged?.Invoke(sender, e);
            }

            ParentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void transformUV_ValueChanged(object sender, EventArgs e)
        {
            if (!Loaded || !texLoaded) return;

            if (ActiveMaterial.TextureTransforms.Length > SelectedIndex && SelectedIndex != -1)
            {
                UpdateTransform(ActiveMaterial.TextureTransforms[SelectedIndex]);
            }
            else if (SelectedIndex != -1)
            {

            }

            ParentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void UpdateTransform(BxlytTextureTransform transform)
        {
            transform.Scale = new Syroot.Maths.Vector2F(
                   scaleXUD.Value, scaleYUD.Value);

            transform.Translate = new Syroot.Maths.Vector2F(
             transXUD.Value, transYUD.Value);

            transform.Rotate = rotUD.Value;
        }

        private void textureSettingsCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded || !texLoaded) return;

            if (ActiveMaterial.TextureMaps.Length > SelectedIndex && SelectedIndex != -1)
            {
                ActiveMaterial.TextureMaps[SelectedIndex].WrapModeU = (WrapMode)wrapModeUCB.SelectedValue;
                ActiveMaterial.TextureMaps[SelectedIndex].WrapModeV = (WrapMode)wrapModeVCB.SelectedValue;
                ActiveMaterial.TextureMaps[SelectedIndex].MaxFilterMode = (FilterMode)expandCB.SelectedValue;
                ActiveMaterial.TextureMaps[SelectedIndex].MinFilterMode = (FilterMode)shrinkCB.SelectedValue;

                Console.WriteLine("Updating wrap mode! " + ActiveMaterial.TextureMaps[SelectedIndex].WrapModeU);
            }

            ParentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void transformTypeCB_SelectedIndexChanged(object sender, EventArgs e) {
            if (transformTypeCB.SelectedIndex != -1)
            {
                var type = transformTypeCB.SelectedItem;
                if (type.ToString().Contains("Projection")) {
                    projectionParamsPanel.Visible = true;
                }
                else
                    projectionParamsPanel.Visible = false;
            }
        }
    }

    public class ImagePanel : PictureBox
    {
        private bool selected = false;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
            }
        }

        public Color BorderColor;

        public ImagePanel()
        {
            Paint += pictureBox1_Paint;

            BackColor = FormThemes.BaseTheme.FormContextMenuBackColor;
            this.SizeMode = PictureBoxSizeMode.Zoom;
        }

        public void ResetImage()
        {
            BackgroundImage?.Dispose();
            BackgroundImage = null;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, BorderColor, ButtonBorderStyle.Solid);
        }
    }
}
