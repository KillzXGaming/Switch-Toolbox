using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Rendering;
using Toolbox.Library;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.CtrH3D.Model;
using OpenTK;
using Toolbox.Library.IO;
using System.Windows.Forms;

namespace FirstPlugin.CtrLibrary
{
    public class H3DMeshWrapper : GenericRenderedObject
    {
        BCH ParentBCH;
        public H3DModelWrapper ParentModel;
        public H3DMesh Mesh;

        public int Layer
        {
            get { return Mesh.Layer; }
            set { Mesh.Layer = value; }
        }

        public override void OnClick(TreeView treeView)
        {
           var editor = ParentBCH.LoadEditor<CtrLibrary.Forms.BCHMeshEditor>();
            editor.LoadMesh(this);
        }

        public override STGenericMaterial GetMaterial() {
            return ParentModel.Materials[Mesh.MaterialIndex];
        }

        public H3DMeshWrapper(BCH bch, H3DModelWrapper parentModel, H3DMesh mesh) : base()
        {
            ParentBCH = bch;
            ParentModel = parentModel;
            Mesh = mesh;

            ImageKey = "mesh";
            SelectedImageKey = "mesh";

            MaterialIndex = Mesh.MaterialIndex;

            foreach (var subMesh in mesh.SubMeshes)
            {
                STGenericPolygonGroup group = new STGenericPolygonGroup();
                for (int i = 0; i < subMesh.Indices.Length; i++)
                    group.faces.Add(subMesh.Indices[i]);

                group.PrimativeType = STPrimitiveType.Triangles;
           /*     switch (subMesh.PrimitiveMode)
                {
                    case SPICA.PICA.Commands.PICAPrimitiveMode.Triangles:
                        group.PrimativeType = STPrimitiveType.Triangles;
                        break;
                    case SPICA.PICA.Commands.PICAPrimitiveMode.TriangleStrip:
                        group.PrimativeType = STPrimitiveType.TrangleStrips;
                        break;
                }*/

                PolygonGroups.Add(group);
            }

            var vertices = mesh.GetVertices();

            List<ushort> boneIndices = new List<ushort>();
            foreach (var subMesh in mesh.SubMeshes)
                if (subMesh.BoneIndicesCount > 0)
                    boneIndices.AddRange(subMesh.BoneIndices.ToArray());

            for (int v = 0; v < vertices.Length; v++)
            {
                Vertex vertex = new Vertex();
                vertex.pos = ConvertVector3(vertices[v].Position);
                vertex.nrm = ConvertVector3(vertices[v].Normal);
                vertex.tan = ConvertVector4(vertices[v].Tangent);
                vertex.uv0 = ConvertVector2(vertices[v].TexCoord0);
                vertex.uv1 = ConvertVector2(vertices[v].TexCoord1);
                vertex.uv2 = ConvertVector2(vertices[v].TexCoord2);
                vertex.col = ConvertVector4(vertices[v].Color);

                //Flip UVs
                vertex.uv0 = new Vector2(vertex.uv0.X, 1 - vertex.uv0.Y);
                if (boneIndices.Count > 0)
                {
                  /*  if (vertices[v].Indices.b0 != -1) vertex.boneIds.Add(boneIndices[vertices[v].Indices.b0]);
                    if (vertices[v].Indices.b1 != -1) vertex.boneIds.Add(boneIndices[vertices[v].Indices.b1]);
                    if (vertices[v].Indices.b2 != -1) vertex.boneIds.Add(boneIndices[vertices[v].Indices.b2]);
                    if (vertices[v].Indices.b3 != -1) vertex.boneIds.Add(boneIndices[vertices[v].Indices.b3]);*/

                    if (mesh.Skinning == H3DMeshSkinning.Rigid)
                    {
                        int index = boneIndices[vertices[v].Indices.b0];
                        vertex.pos = Vector3.TransformPosition(vertex.pos, parentModel.Skeleton.Renderable.bones[index].Transform);
                       // vertex.nrm = Vector3.TransformNormal(vertex.nrm, parentModel.Skeleton.Renderable.bones[index].Transform);
                    }

                    /*    vertex.boneWeights.Add(vertices[v].Weights.w0);
                        vertex.boneWeights.Add(vertices[v].Weights.w1);
                        vertex.boneWeights.Add(vertices[v].Weights.w2);
                        vertex.boneWeights.Add(vertices[v].Weights.w3);*/
                }

                this.vertices.Add(vertex);
            }
        }

        private static Vector2 ConvertVector2(System.Numerics.Vector4 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }

        private static Vector3 ConvertVector3(System.Numerics.Vector4 vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z );
        }

        private static Vector4 ConvertVector4(System.Numerics.Vector4 vec)
        {
            return new Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }
    }
}
