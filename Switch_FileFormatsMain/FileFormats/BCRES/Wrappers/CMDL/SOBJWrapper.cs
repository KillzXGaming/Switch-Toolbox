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
using BcresLibrary.Enums;

namespace FirstPlugin
{
    public class SOBJWrapper : STGenericObject
    {
        internal CMDLWrapper ParentModelWrapper;
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
            get { return ParentModelWrapper.Model.Shapes[ShapeIndex]; }
            set { ParentModelWrapper.Model.Shapes[ShapeIndex] = value; }
        }

        public MTOBWrapper MaterialWrapper
        {
            get { return ParentModelWrapper.Materials[ShapeIndex]; }
            set { ParentModelWrapper.Materials[ShapeIndex] = value; }
        }

        public SOBJWrapper()
        {
            ImageKey = "Material";
            SelectedImageKey = "Material";
        }
        public SOBJWrapper(CMDLWrapper model, Mesh mesh) : base()
        {
            ParentModelWrapper = model;
            Load(mesh);
        }

        public List<DisplayVertex> CreateDisplayVertices()
        {
            display = lodMeshes[DisplayLODIndex].getDisplayFace().ToArray();

            List<DisplayVertex> displayVertList = new List<DisplayVertex>();

            if (lodMeshes[DisplayLODIndex].faces.Count <= 3)
                return displayVertList;

            foreach (Vertex v in vertices)
            {
                DisplayVertex displayVert = new DisplayVertex()
                {
                    pos = v.pos,
                    nrm = v.nrm,
                    tan = v.tan.Xyz,
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

            public static int Size = 4 * (3 + 3 + 3 + 2 + 4 + 4 + 4 + 2 + 2 );
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
                LOD_Mesh msh = new LOD_Mesh();

                foreach (var faceDescriptors in group.FaceDescriptors)
                {
                    foreach (var buffer in faceDescriptors.Buffers)
                    {
                        msh.PrimitiveType = STPolygonType.Triangle;
                        msh.FirstVertex = 0;

                        uint[] indicesArray = buffer.GetIndices().ToArray();
                        for (int face = 0; face < indicesArray.Length; face++)
                        {
                            msh.faces.Add((int)indicesArray[face] + (int)msh.FirstVertex);
                        }
                    }
                }

                lodMeshes.Add(msh);
            }

            for (int vtxGrp = 0; vtxGrp < Shape.VertexGroups.Count; vtxGrp++)
            {
                if (Shape.VertexGroups[vtxGrp].VertexBufferInterleaved != null)
                {
                    var FaceGroup = Shape.FaceGroups[vtxGrp];

                    int BoneIndex = 0;
                    if (FaceGroup.BoneIndexList != null && FaceGroup.BoneIndexList.Length > 0)
                        BoneIndex = (int)FaceGroup.BoneIndexList[0];

                    STSkeleton skeleton = new STSkeleton();
                    if (ParentModelWrapper.Skeleton != null && ParentModelWrapper.Skeleton.Renderable != null)
                        skeleton = ParentModelWrapper.Skeleton.Renderable;

                    var buffer = Shape.VertexGroups[vtxGrp].VertexBufferInterleaved;

                    //Set each array first from the lib if exist. Then add the data all in one loop
                    Syroot.Maths.Vector4F[] vec4Positions = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4Normals = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4uv0 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4uv1 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4uv2 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4c0 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4t0 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4w0 = new Syroot.Maths.Vector4F[0];
                    Syroot.Maths.Vector4F[] vec4i0 = new Syroot.Maths.Vector4F[0];

                    foreach (var attribute in buffer.Attributes)
                    {
                        switch (attribute.AttributeType)
                        {
                            case GfxAttributeType.Position: vec4Positions = attribute.Data; break;
                            case GfxAttributeType.Normal: vec4Normals = attribute.Data; break;
                            case GfxAttributeType.TextureCoordinate0: vec4uv0 = attribute.Data; break;
                            case GfxAttributeType.TextureCoordinate1: vec4uv1 = attribute.Data; break;
                            case GfxAttributeType.TextureCoordinate2: vec4uv2 = attribute.Data; break;
                            case GfxAttributeType.Tangent: vec4t0 = attribute.Data; break;
                            case GfxAttributeType.Color: vec4c0 = attribute.Data; break;
                            case GfxAttributeType.BoneWeight: vec4w0 = attribute.Data; break;
                            case GfxAttributeType.BoneIndex: vec4i0 = attribute.Data; break;
                        }
                    }

                    for (int i = 0; i < vec4Positions.Length; i++)
                    {
                        Vertex v = new Vertex();
                        if (vec4Positions.Length > 0)
                            v.pos = new Vector3(vec4Positions[i].X, vec4Positions[i].Y, vec4Positions[i].Z);
                        if (vec4Normals.Length > 0)
                            v.nrm = new Vector3(vec4Normals[i].X, vec4Normals[i].Y, vec4Normals[i].Z);
                        if (vec4uv0.Length > 0)
                            v.uv0 = new Vector2(vec4uv0[i].X, vec4uv0[i].Y);
                        if (vec4uv1.Length > 0)
                            v.uv1 = new Vector2(vec4uv1[i].X, vec4uv1[i].Y);
                        if (vec4uv2.Length > 0)
                            v.uv2 = new Vector2(vec4uv2[i].X, vec4uv2[i].Y);
                        if (vec4w0.Length > 0)
                        {
                            v.boneWeights.Add(vec4w0[i].X);
                            v.boneWeights.Add(vec4w0[i].Y);
                            v.boneWeights.Add(vec4w0[i].Z);
                            v.boneWeights.Add(vec4w0[i].W);
                        }
                        if (vec4i0.Length > 0)
                        {
                            v.boneIds.Add((int)vec4i0[i].X);
                            v.boneIds.Add((int)vec4i0[i].Y);
                            v.boneIds.Add((int)vec4i0[i].Z);
                            v.boneIds.Add((int)vec4i0[i].W);
                        }

                        if (vec4t0.Length > 0)
                            v.tan = new Vector4(vec4t0[i].X, vec4t0[i].Y, vec4t0[i].Z, vec4t0[i].W);
                        if (vec4c0.Length > 0)
                            v.col = new Vector4(vec4c0[i].X, vec4c0[i].Y, vec4c0[i].Z, vec4c0[i].W);

                        if (FaceGroup.SkinnningMode == SkinnningMode.Rigid)
                        {
                            Matrix4 sb = skeleton.bones[v.boneIds[0]].Transform;
                            v.pos = Vector3.TransformPosition(v.pos, sb);
                            v.nrm = Vector3.TransformNormal(v.nrm, sb);
                        }
                        if (FaceGroup.SkinnningMode == SkinnningMode.None)
                        {
                            if (skeleton.bones.Count > 0)
                            {
                                Matrix4 NoBindFix = ParentModelWrapper.Skeleton.Renderable.bones[BoneIndex].Transform;
                                v.pos = Vector3.TransformPosition(v.pos, NoBindFix);
                                v.nrm = Vector3.TransformNormal(v.nrm, NoBindFix);
                            }
                        }
                        vertices.Add(v);
                    }
                }
            }
        }
    }
}
