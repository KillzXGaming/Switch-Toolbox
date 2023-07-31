using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.Rendering;
using OpenTK;
using Toolbox.Library.Forms;
using System.Windows.Forms;
using FirstPlugin.Forms;

namespace FirstPlugin
{
    public class GFLXMesh : STGenericObject, IContextMenuNode
    {
        public int[] display;
        public int DisplayId;

        public GFLXModel ParentModel { get; set; }

        public GFBANM.MeshAnimationController AnimationController = new GFBANM.MeshAnimationController();

        public Matrix4 Transform { get; set; }

        public GFMDLStructs.Mesh MeshData { get; set; }
        public GFMDLStructs.Group GroupData { get; set; }

        public override void OnClick(TreeView treeView) {
            var editor = ParentModel.LoadEditor<GFLXMeshEditor>();
            editor.LoadMesh(this);
        }

        public GFLXMaterialData GetMaterial(STGenericPolygonGroup polygroup)
        {
           return ParentModel.GenericMaterials[polygroup.MaterialIndex];
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            var uvMenu = new ToolStripMenuItem("UVs");
            var normalsMenu = new ToolStripMenuItem("Normals");
            Items.Add(uvMenu);
            Items.Add(normalsMenu);

            Items.Add(new ToolStripMenuItem("Recalculate Bitangents", null, CalculateTangentBitangenAction, Keys.Control | Keys.T));
            uvMenu.DropDownItems.Add(new ToolStripMenuItem("Flip Vertical", null, FlipVerticalAction, Keys.Control | Keys.V));
            uvMenu.DropDownItems.Add(new ToolStripMenuItem("Flip Horizontal", null, FlipHorizontalAction, Keys.Control | Keys.H));
            var colorMenu = new ToolStripMenuItem("Vertex Colors");
            colorMenu.DropDownItems.Add(new ToolStripMenuItem("Set Color", null, SetVertexColorDialog, Keys.Control | Keys.C));
            colorMenu.DropDownItems.Add(new ToolStripMenuItem("Convert Normals", null, SetVertexColorNormals, Keys.Control | Keys.N));
            normalsMenu.DropDownItems.Add(new ToolStripMenuItem("Recalculate", null, RecalculateNormals, Keys.Control | Keys.R));
            normalsMenu.DropDownItems.Add(new ToolStripMenuItem("Smooth", null, SmoothNormals, Keys.Control | Keys.S));

            
            Items.Add(colorMenu);

            return Items.ToArray();
        }

        private void SetVertexColorNormals(object sender, EventArgs args)
        {
            if (!MeshData.Attributes.Any(x => x.VertexType == (uint)GFMDLStructs.VertexType.Color2))
                return;

            SetVertexColorNormals();
            UpdateMesh();
        }

        public void SetVertexColorNormals()
        {
            for (int v = 0; v < vertices.Count; v++)
                vertices[v].col2 = new Vector4(
                    vertices[v].nrm.X * 0.5f + 0.5f,
                    vertices[v].nrm.Y * 0.5f + 0.5f,
                    vertices[v].nrm.Z * 0.5f + 0.5f,
                    1);
        }

        private void SetVertexColorDialog(object sender, EventArgs args)
        {
            if (!MeshData.Attributes.Any(x => x.VertexType == (uint)GFMDLStructs.VertexType.Color1))
                return;

            ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == DialogResult.OK) {
                SetVertexColor(new Vector4(
                    dlg.Color.R / 255.0f,
                    dlg.Color.G / 255.0f,
                    dlg.Color.B / 255.0f,
                    dlg.Color.A / 255.0f));

                UpdateMesh();
            }
        }

        private void FlipVerticalAction(object sender, EventArgs args) {
            this.FlipUvsVertical();
            UpdateMesh();
        }

        private void FlipHorizontalAction(object sender, EventArgs args) {
            this.FlipUvsHorizontal();
            UpdateMesh();
        }

        private void RecalculateNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            CalculateNormals();
            UpdateMesh();
            Cursor.Current = Cursors.Default;
        }

        private void SmoothNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            SmoothNormals();
            UpdateMesh();
            Cursor.Current = Cursors.Default;
        }

        private void CalculateTangentBitangenAction(object sender, EventArgs args)
        {
            this.CalculateTangentBitangent(0);
            UpdateMesh();
        }


        private void UpdateMesh() {
            MeshData.SetData(GFLXMeshBufferHelper.CreateVertexDataBuffer(this));
            ParentModel.UpdateVertexData(true);
        }

        public struct DisplayVertex
        {
            // Used for rendering.
            public Vector3 pos;
            public Vector3 nrm;
            public Vector3 tan;
            public Vector2 uv;
            public Vector4 col;
            public Vector4 node;
            public Vector4 weight;
            public Vector2 uv2;
            public Vector2 uv3;
            public Vector3 binorm;

            public static int Size = 4 * (3 + 3 + 3 + 2 + 4 + 4 + 4 + 2 + 2 + 3);
        }

        public GFLXMesh(GFLXModel model, 
            GFMDLStructs.Group group,
            GFMDLStructs.Mesh mesh)
        {
            ParentModel = model;
            GroupData = group;
            MeshData = mesh;
        }

        public int MeshIndex { get; set; }

        public List<DisplayVertex> CreateDisplayVertices()
        {
            List<int> Faces = new List<int>();
            foreach (var group in PolygonGroups)
                Faces.AddRange(group.GetDisplayFace());

            display = Faces.ToArray();

            List<DisplayVertex> displayVertList = new List<DisplayVertex>();

            if (display.Length <= 3)
                return displayVertList;

            foreach (Vertex v in vertices)
            {
                DisplayVertex displayVert = new DisplayVertex()
                {
                    pos = v.pos,
                    nrm = v.nrm,
                    tan = v.tan.Xyz,
                    binorm = v.bitan.Xyz,
                    col = v.col,
                    uv = v.uv0,
                    uv2 = v.uv1,
                    uv3 = v.uv2,
                    node = new Vector4(
                             v.boneIds.Count > 0 ? v.boneIds[0] : -1,
                             v.boneIds.Count > 1 ? v.boneIds[1] : -1,
                             v.boneIds.Count > 2 ? v.boneIds[2] : -1,
                             v.boneIds.Count > 3 ? v.boneIds[3] : -1),
                    weight = new Vector4(
                             v.boneWeights.Count > 0 ? v.boneWeights[0] : 0,
                             v.boneWeights.Count > 1 ? v.boneWeights[1] : 0,
                             v.boneWeights.Count > 2 ? v.boneWeights[2] : 0,
                             v.boneWeights.Count > 3 ? v.boneWeights[3] : 0),
                };

                displayVertList.Add(displayVert);

                /*      Console.WriteLine($"---------------------------------------------------------------------------------------");
                       Console.WriteLine($"Position   {displayVert.pos.X} {displayVert.pos.Y} {displayVert.pos.Z}");
                       Console.WriteLine($"Normal     {displayVert.nrm.X} {displayVert.nrm.Y} {displayVert.nrm.Z}");
                       Console.WriteLine($"Tanget     {displayVert.tan.X} {displayVert.tan.Y} {displayVert.tan.Z}");
                       Console.WriteLine($"Color      {displayVert.col.X} {displayVert.col.Y} {displayVert.col.Z} {displayVert.col.W}");
                       Console.WriteLine($"UV Layer 1 {displayVert.uv.X} {displayVert.uv.Y}");
                       Console.WriteLine($"UV Layer 2 {displayVert.uv2.X} {displayVert.uv2.Y}");
                       Console.WriteLine($"UV Layer 3 {displayVert.uv3.X} {displayVert.uv3.Y}");
                       Console.WriteLine($"Bone Index {displayVert.node.X} {displayVert.node.Y} {displayVert.node.Z} {displayVert.node.W}");
                       Console.WriteLine($"Weights    {displayVert.weight.X} {displayVert.weight.Y} {displayVert.weight.Z} {displayVert.weight.W}");
                       Console.WriteLine($"---------------------------------------------------------------------------------------");*/
            }

            return displayVertList;
        }
    }
}
