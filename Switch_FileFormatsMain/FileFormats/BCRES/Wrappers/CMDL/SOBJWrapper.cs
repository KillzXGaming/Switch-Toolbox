using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using BcresLibrary;
using OpenTK;

namespace FirstPlugin
{
    public class SOBJWrapper : STGenericObject
    {
        internal Model ParentModel;
        internal BCRES BcresParent;
        internal Mesh Mesh;

        public int[] Faces;
        public int[] display;
        public int DisplayId;

        public int ShapeIndex
        {
            get { return (int)Mesh.ShapeIndex; }
            set { Mesh.ShapeIndex = (uint)value; }
        }

        public Shape Shape
        {
            get { return ParentModel.Shapes[ShapeIndex]; }
            set { ParentModel.Shapes[ShapeIndex] = value; }
        }

        public SOBJWrapper()
        {
            ImageKey = "Material";
            SelectedImageKey = "Material";
        }
        public SOBJWrapper(Model model, Mesh mesh) : base()
        {
            ParentModel = model;
            Load(mesh);
        }

        public List<DisplayVertex> CreateDisplayVertices()
        {
            display = lodMeshes[DisplayLODIndex].getDisplayFace().ToArray();

            List<DisplayVertex> displayVertList = new List<DisplayVertex>();

            if (faces.Count <= 3)
                return displayVertList;

            foreach (Vertex v in vertices)
            {
                DisplayVertex displayVert = new DisplayVertex()
                {
                    pos = v.pos,
                    nrm = v.nrm,
                    col = v.col.Xyz,
                };

                displayVertList.Add(displayVert);
            }

            return displayVertList;
        }

        public struct DisplayVertex
        {
            // Used for rendering.
            public Vector3 pos;
            public Vector3 nrm;
            public Vector3 col;

            public static int Size = 4 * (3 + 3 + 3);
        }

        public override void OnClick(TreeView treeview)
        {
            BcresParent.LoadEditors(this, OnPropertyChanged);
        }

        private void OnPropertyChanged()
        {

        }

        public void Load(Mesh mesh)
        {
            Mesh = mesh;

            Text = mesh.Name;

            lodMeshes = new List<LOD_Mesh>();
            foreach (var group in Shape.FaceGroups)
            {
                foreach (var faceDescriptors in group.FaceDescriptors)
                {
                    foreach (var buffer in faceDescriptors.Buffers)
                    {
                        LOD_Mesh msh = new LOD_Mesh();
                        msh.PrimitiveType = STPolygonType.Triangle;
                        msh.FirstVertex = 0;

                        uint[] indicesArray = buffer.GetIndices().ToArray();
                        for (int face = 0; face < indicesArray.Length; face++)
                            msh.faces.Add((int)indicesArray[face] + (int)msh.FirstVertex);

                        lodMeshes.Add(msh);
                    }
                }
            }
        }
    }
}
