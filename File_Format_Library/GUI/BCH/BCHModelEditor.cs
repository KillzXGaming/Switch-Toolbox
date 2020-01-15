using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Mesh;
using Toolbox.Library.Forms;

namespace FirstPlugin.CtrLibrary.Forms
{
    public partial class BCHModelEditor : UserControl
    {
        H3DModelWrapper ActiveModelWrapper;

        private bool Loaded = false;

        public BCHModelEditor()
        {
            InitializeComponent();

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            tranXUD.ValueChanged += OnTransformChanged;
            tranYUD.ValueChanged += OnTransformChanged;
            tranZUD.ValueChanged += OnTransformChanged;
            rotXUD.ValueChanged += OnTransformChanged;
            rotYUD.ValueChanged += OnTransformChanged;
            rotZUD.ValueChanged += OnTransformChanged;
            scaleXUD.ValueChanged += OnTransformChanged;
            scaleYUD.ValueChanged += OnTransformChanged;
            scaleZUD.ValueChanged += OnTransformChanged;

            meshLayerCB.Items.Add("0 (Opaque)");
            meshLayerCB.Items.Add("1 (Translucent)");
            meshLayerCB.Items.Add("2 (Subtractive Blend)");
            meshLayerCB.Items.Add("3 (Additive Blend)");

            listViewCustom1.FullRowSelect = true;

            tranXUD.SetTheme();
        }

        public void LoadModel(H3DModelWrapper wrapper)
        {
            ActiveModelWrapper = wrapper;

            Loaded = false;
            var model = wrapper.Model;
            var transform = model.WorldTransform;

            if (model.MetaData != null)
                bchUserDataEditor1.LoadUserData(model.MetaData);

            listViewCustom1.BeginUpdate();
            listViewCustom1.Items.Clear();
            for (int i = 0; i < model.Meshes.Count; i++) {
                ListViewItem item = new ListViewItem();
                UpdateMeshListItem(model.Meshes[i], item);
                listViewCustom1.Items.Add(item);
            }
            listViewCustom1.EndUpdate();

            if (listViewCustom1.Items.Count > 0)
                listViewCustom1.TrySelectItem(0);

            Loaded = true;
        }

        private static void UpdateMeshListItem(H3DMesh mesh, ListViewItem item)
        {
            string materialName = "";
            if (mesh.Parent.Materials.Count > mesh.MaterialIndex)
                materialName = mesh.Parent.Materials[mesh.MaterialIndex].Name;

            item.SubItems.Clear();
            item.Tag = mesh;
            item.Text = mesh.NodeIndex.ToString();
            item.SubItems.Add(materialName);
            item.SubItems.Add($"mesh_{materialName}");
            item.SubItems.Add(GetLayerString(mesh.Layer));
            item.SubItems.Add(mesh.Priority.ToString());
        }

        private static string GetLayerString(int layer)
        {
            switch (layer)
            {
                case 0: return $"{layer} (Opaque)";
                case 1: return $"{layer} (Translucent)";
                case 2: return $"{layer} (Subtractive Blend)";
                case 3: return $"{layer} (Additive Blend)";
                default: return layer.ToString();
            }
        }

        private void OnTransformChanged(object sender, EventArgs e)
        {
            if (!Loaded) return;

        }

        private void BCHModelEditor_Load(object sender, EventArgs e)
        {

        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e) {
            var mesh = GetActiveMesh();
            if (mesh != null && Loaded) {
                meshLayerCB.SelectedIndex = mesh.Layer;
                renderPriorityUD.Value = mesh.Priority;
            }
        }

        private void meshLayerCB_SelectedIndexChanged(object sender, EventArgs e) {
            var mesh = GetActiveMesh();
            if (mesh != null && Loaded) {
                ActiveModelWrapper.Model.EditLayer(mesh, meshLayerCB.SelectedIndex);
                UpdateMeshListItem(mesh, listViewCustom1.SelectedItems[0]);
            }
        }

        private H3DMesh GetActiveMesh()
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
                return (H3DMesh)listViewCustom1.SelectedItems[0].Tag;
            return null;
        }

        private void renderPriorityUD_Scroll(object sender, ScrollEventArgs e) {
            var mesh = GetActiveMesh();
            if (mesh != null && Loaded) {
                mesh.Priority = (int)renderPriorityUD.Value;
                UpdateMeshListItem(mesh, listViewCustom1.SelectedItems[0]);
            }
        }
    }
}