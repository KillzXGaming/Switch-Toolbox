using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SPICA.Formats.CtrH3D.Model.Material;
using Toolbox.Library.Forms;

namespace FirstPlugin.CtrLibrary.Forms
{
    public partial class BCHMaterialColorEditor : UserControl, IMaterialLoader
    {
        public BCHMaterialColorEditor()
        {
            InitializeComponent();

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();

            listViewCustom1.FullRowSelect = false;
            listViewCustom1.HideSelection = true;

            stColorControl1.Enabled = false;
            stColorControl1.ColorChanged += OnColorChanged;
        }

        public void LoadMaterial(H3DMaterialWrapper wrapper)
        {
            var ActiveMaterial = wrapper.Material;
            var ActiveMaterialParams = ActiveMaterial.MaterialParams;

            listViewCustom1.BeginUpdate();
            listViewCustom1.Items.Clear();
            listViewCustom1.Items.Add(CreateColorEntry("Diffuse", ActiveMaterialParams.DiffuseColor, true));
            listViewCustom1.Items.Add(CreateColorEntry("Ambient", ActiveMaterialParams.AmbientColor));
            listViewCustom1.Items.Add(CreateColorEntry("Emissive", ActiveMaterialParams.EmissionColor));
            listViewCustom1.Items.Add(CreateColorEntry("Specular0", ActiveMaterialParams.Specular0Color));
            listViewCustom1.Items.Add(CreateColorEntry("Specular1", ActiveMaterialParams.Specular1Color));

            listViewCustom1.Items.Add(CreateColorEntry("Constant0", ActiveMaterialParams.Constant0Color, true));
            listViewCustom1.Items.Add(CreateColorEntry("Constant1", ActiveMaterialParams.Constant1Color, true));
            listViewCustom1.Items.Add(CreateColorEntry("Constant2", ActiveMaterialParams.Constant2Color, true));
            listViewCustom1.Items.Add(CreateColorEntry("Constant3", ActiveMaterialParams.Constant3Color, true));
            listViewCustom1.Items.Add(CreateColorEntry("Constant4", ActiveMaterialParams.Constant4Color, true));
            listViewCustom1.Items.Add(CreateColorEntry("Constant5", ActiveMaterialParams.Constant5Color, true));

            listViewCustom1.Items.Add(CreateColorEntry("BlendColor", ActiveMaterialParams.BlendColor, true));
            listViewCustom1.Items.Add(CreateColorEntry("TexEnvBufferColor", ActiveMaterialParams.TexEnvBufferColor, true));

            if (ActiveMaterial.TextureMappers?.Length > 0)
                listViewCustom1.Items.Add(CreateColorEntry("BorderColor (texture0))", ActiveMaterial.TextureMappers[0].BorderColor));
            if (ActiveMaterial.TextureMappers?.Length > 0)
                listViewCustom1.Items.Add(CreateColorEntry("BorderColor (texture1)", ActiveMaterial.TextureMappers[1].BorderColor));
            if (ActiveMaterial.TextureMappers?.Length > 0)
                listViewCustom1.Items.Add(CreateColorEntry("BorderColor (texture2)", ActiveMaterial.TextureMappers[2].BorderColor));

            listViewCustom1.EndUpdate();
        }

        private ListViewItem CreateColorEntry(string name, SPICA.Math3D.RGBA rgba, bool useAlpha = false)
        {
            ListViewItem item = new ListViewItem();
            LoadColor(name, rgba, item, useAlpha);
            return item;
        }

        private void LoadColor(string name, SPICA.Math3D.RGBA rgba, ListViewItem item, bool useAlpha = false)
        {
            Color color = Color.FromArgb(rgba.R, rgba.G, rgba.B);
            Color alpha = Color.FromArgb(rgba.A, rgba.A, rgba.A);

            item.Tag = rgba;
            item.SubItems.Clear();
            item.Text = name;
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add("");
            item.SubItems.Add("");
            item.SubItems[1].BackColor = color;
            if (useAlpha)
                item.SubItems[2].BackColor = alpha;
        }

        private bool IsColorLoaded = false;
        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            IsColorLoaded = false;
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                if (!stColorControl1.Enabled)
                    stColorControl1.Enabled = true;

                ListViewItem item = listViewCustom1.SelectedItems[0];
                var rgba = (SPICA.Math3D.RGBA)item.Tag;

                selectedColorLabel.Text = item.Text;

                stColorControl1.ColorRGB = Color.FromArgb(rgba.R, rgba.G, rgba.B);
                stColorControl1.Alpha = rgba.A;
            }
            else
            {
                if (stColorControl1.Enabled)
                    stColorControl1.Enabled = false;

                selectedColorLabel.Text = "";
                stColorControl1.ColorRGB = Color.White;
                stColorControl1.Alpha = 255;
            }
            IsColorLoaded = true;
        }

        private void OnColorChanged(object sender, EventArgs e)
        {
            if (!IsColorLoaded) return;

            if (listViewCustom1.SelectedItems.Count > 0)
            {
                ListViewItem item = listViewCustom1.SelectedItems[0];
                var rgba = ((SPICA.Math3D.RGBA)item.Tag);
                rgba.R = stColorControl1.NewColor.R;
                rgba.G = stColorControl1.NewColor.G;
                rgba.B = stColorControl1.NewColor.B;
                rgba.A = stColorControl1.NewColor.A;

                stColorControl1.Refresh();

                LoadColor(item.Text, rgba, item, HasAlpha(item.Text));
            }
        }

        private bool HasAlpha(string text)
        {
            switch (text)
            {
                case "Diffuse": return true;
                case "Constant0": return true;
                case "Constant1": return true;
                case "Constant2": return true;
                case "Constant3": return true;
                case "Constant4": return true;
                case "Constant5": return true;
                case "BlendColor": return true;
                case "TexEnvBufferColor": return true;
                default: return false;
            }
        }
    }
}
