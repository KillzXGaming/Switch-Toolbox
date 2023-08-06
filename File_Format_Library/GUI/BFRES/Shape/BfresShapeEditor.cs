using System;
using FirstPlugin.Forms;
using OpenTK;
using System.Windows.Forms;
using Toolbox.Library;
using Bfres.Structs;
using Toolbox.Library.Forms;
using Toolbox.Library.Rendering;
using Syroot.NintenTools.NSW.Bfres;
using ResU = Syroot.NintenTools.Bfres;

namespace FirstPlugin
{
    public partial class BfresShapeEditor : UserControl
    {
        public BfresShapeEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            lodListView.FullRowSelect = true;

            btnFacesView.Enabled = false;
            vertexBufferDropDownPanel.IsExpanded = false;
            levelOfDetailDropDownPanel.IsExpanded = false;
            keyShapeDropDownPanel.IsExpanded = false;

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
        }
        FSHP activeShape;
        FSHP.LOD_Mesh activeLodMesh;

        bool IsLoaded = false;
        bool IsBoneListLoaded = false;
        bool IsMatListLoaded = false;

        public void LoadShape(FSHP fshp)
        {
            InitializeControls();

            activeShape = fshp;

            // Disable skin count adjustment if no skinning
            shapeVertexSkinCountUD.ReadOnly = fshp.VertexSkinCount == 0;

            // Setup skin count adjustment
            int lowestVertexSkinCount = fshp.GetLowestPossibleVertexSkinCount();
            if (shapeVertexSkinCountUD.Minimum > lowestVertexSkinCount &&
                shapeVertexSkinCountUD.Value > lowestVertexSkinCount)
            {
                shapeVertexSkinCountUD.Minimum = lowestVertexSkinCount;
                shapeVertexSkinCountUD.Value = fshp.VertexSkinCount;
            }
            else
            {
                shapeVertexSkinCountUD.Value = fshp.VertexSkinCount;
                shapeVertexSkinCountUD.Minimum = lowestVertexSkinCount;
            }

            FMDL fmdl = fshp.GetParentModel();

            //Load info
            nameTB.Bind(fshp, "Text");
            shapeIndexUD.Value = fmdl.shapes.IndexOf(fshp);

            //Load meshes to toggle LOD display
            for (int i = 0; i < fshp.lodMeshes.Count; i++)
            {
                lodDisplayCB.Items.Add($"mesh {i}");
            }
            lodDisplayCB.SelectedIndex = activeShape.DisplayLODIndex;

            //Load material (all materials will load when activated)
            materialComboBox1.Items.Add(fshp.GetMaterial().Text);
            materialComboBox1.SelectedIndex = 0;

            //Load bone binded (all bones will load when activated)
            bonesCB.Items.Add(fmdl.Skeleton.bones[fshp.BoneIndex].Text);
            bonesCB.SelectedIndex = 0;

            if (fshp.VertexBufferU != null)
            {
                vertexBufferSkinCountUD.Value = fshp.VertexBufferU.VertexSkinCount;
                vertexBufferList1.LoadVertexBuffers(fshp, fshp.VertexBufferU);
            }
            else
            {
                vertexBufferSkinCountUD.Value = fshp.VertexBuffer.VertexSkinCount;
                vertexBufferList1.LoadVertexBuffers(fshp, fshp.VertexBuffer);
            }

            vtxCountUD.Maximum = fshp.vertices.Count;
            vtxCountUD.Value = fshp.vertices.Count;


            if (fshp.ShapeU != null)
                keyShapeList1.LoadKeyShapes(fshp.ShapeU.KeyShapes);
            else
                keyShapeList1.LoadKeyShapes(fshp.Shape.KeyShapes, fshp.Shape.KeyShapeDict);

            int lodIndx = 0;
            foreach (var mesh in fshp.lodMeshes)
                lodListView.Items.Add($"Detail Level {lodIndx++}");

            IsLoaded = true;
        }

        private void InitializeControls()
        {
            IsLoaded = false;
            IsBoneListLoaded = false;
            IsMatListLoaded = false;
            lodDisplayCB.Items.Clear();
            lodListView.Items.Clear();
            bonesCB.Items.Clear();
            materialComboBox1.Items.Clear();
        }

        private void ReloadBoneList()
        {
            if (!IsLoaded)
                return;

            if (!IsBoneListLoaded)
            {
                bonesCB.Items.Clear();
                foreach (var bn in activeShape.GetParentModel().Skeleton.bones)
                    bonesCB.Items.Add(bn.Text);

                bonesCB.SelectedIndex = activeShape.BoneIndex;
                IsBoneListLoaded = true;
            }
        }

        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoaded)
                return;

            if (materialComboBox1.SelectedIndex >= 0 && IsMatListLoaded)
            {
                activeShape.MaterialIndex = materialComboBox1.SelectedIndex;
                materialIndexUD.Value = materialComboBox1.SelectedIndex;
            }

            LibraryGUI.UpdateViewport();
        }

        private void ReloadMaterialList()
        {
            if (!IsLoaded)
                return;

            //For optmization purposes. Load a list when used instead
            if (!IsMatListLoaded)
            {
                materialComboBox1.Items.Clear();
                foreach (FMAT mat in activeShape.GetParentModel().materials.Values)
                    materialComboBox1.Items.Add(mat.Text);

                materialComboBox1.SelectedIndex = activeShape.MaterialIndex;
                IsMatListLoaded = true;
            }
        }

        private void bonesCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoaded)
                return;

            if (bonesCB.SelectedIndex >= 0 && IsBoneListLoaded)
            {
                activeShape.BoneIndex = bonesCB.SelectedIndex;
                boneIndexUD.Value = bonesCB.SelectedIndex;
            }

            LibraryGUI.UpdateViewport();
        }

        private void valueUD_ValueChanged(object sender, EventArgs e)
        {

        }

        private void stDropDownPanel1_Load(object sender, EventArgs e)
        {

        }

        private void rotModeCB_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void materialComboBox1_Click(object sender, EventArgs e) {
        }

        private void materialComboBox1_KeyDown(object sender, KeyEventArgs e) {
            ReloadMaterialList();
        }

        private void materialComboBox1_MouseDown(object sender, MouseEventArgs e){
            ReloadMaterialList();
        }

        private void bonesCB_KeyDown(object sender, KeyEventArgs e) {
            ReloadBoneList();
        }

        private void bonesCB_MouseDown(object sender, MouseEventArgs e) {
            ReloadBoneList();
        }

        private void boneListBtn_Click(object sender, EventArgs e)
        {
            BoneIndexList indexViewer = new BoneIndexList("Bone Index List");

            if (activeShape.ShapeU != null)
            {
                indexViewer.LoadIndices(activeShape.ShapeU.SkinBoneIndices,
                                        activeShape.GetParentModel().Skeleton);
            }
            else
            {
                indexViewer.LoadIndices(activeShape.Shape.SkinBoneIndices,
                        activeShape.GetParentModel().Skeleton);
            }

            indexViewer.Show(this);
        }

        private void lodDisplayCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoaded)
                return;

            if (lodDisplayCB.SelectedIndex != -1)
            {
                activeShape.DisplayLODIndex = lodDisplayCB.SelectedIndex;
                activeShape.UpdateVertexData();
                LibraryGUI.UpdateViewport();
            }
        }

        private void stDropDownPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lodListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lodListView.SelectedIndices.Count > 0)
            {
                btnFacesView.Enabled = true;
                subMeshesViewBtn.Enabled = true;

                int index = lodListView.SelectedIndices[0];
                activeLodMesh = activeShape.lodMeshes[index];

                lodFormatCB.Bind(typeof(STIndexFormat), activeLodMesh, "IndexFormat");
                lodFormatCB.SelectedItem = activeLodMesh.IndexFormat;

                lodPrimativeTypeCB.Bind(typeof(STPrimitiveType), activeLodMesh, "PrimitiveType");
                lodPrimativeTypeCB.SelectedItem = activeLodMesh.PrimativeType;

                lodVertexSkipUD.Value = activeLodMesh.FirstVertex;
                lodFaceCountUD.Value = activeLodMesh.faces.Count;
            }
            else
            {
                btnFacesView.Enabled = false;
                activeLodMesh = null;
                lodFormatCB.DataSource = null;
                lodPrimativeTypeCB.DataSource = null;
                lodFormatCB.Items.Clear();
                lodPrimativeTypeCB.Items.Clear();

                lodVertexSkipUD.Value = 0;
                lodFaceCountUD.Value = 0;
                subMeshesViewBtn.Enabled = false;
            }
        }

        private void btnFacesView_Click(object sender, EventArgs e)
        {
            if (activeLodMesh != null)
            {
                FaceIndiceListViewer viewer = new FaceIndiceListViewer();
                viewer.LoadIndices(activeLodMesh.faces);
                viewer.Show(this);
            }
        }

        private void subMeshesViewBtn_Click(object sender, EventArgs e)
        {
            if (activeLodMesh != null)
            {
                SubMeshEditor editor = new SubMeshEditor();
                editor.LoadMesh(activeLodMesh, activeShape);
                if (editor.ShowDialog() == DialogResult.OK)
                {

                }
            }
        }

        private void SkinCountUD_ValueChanged(object sender, System.EventArgs e)
        {
            if (!(sender is NumericUpDownInt valueSelector))
            { 
                return; 
            }

            // Skip if already the same or 0
            if (activeShape.VertexSkinCount == valueSelector.Value || valueSelector.Value == 0)
            {
                return;
            }

            activeShape.UpdateVertexSkinCount((int)valueSelector.Value);
        }
    }
}
