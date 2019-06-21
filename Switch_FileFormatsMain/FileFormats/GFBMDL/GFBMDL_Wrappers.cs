using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using OpenTK;

namespace FirstPlugin
{
    public class GFBMaterial : STGenericMaterial
    {
        public GFBMDL ParentModel { get; set; }

        public GFBMaterial(GFBMDL model) {
            ParentModel = model;
        }
    }

    public class GFBMesh : STGenericObject
    {
        public int[] display;
        public int DisplayId;

        public GFBMDL ParentModel { get; set; }

        public GFBMaterial GetMaterial(STGenericPolygonGroup polygroup)
        {
           return ParentModel.header.GenericMaterials[polygroup.MaterialIndex];
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

        public GFBMesh(GFBMDL model) {
            ParentModel = model;
        }

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
