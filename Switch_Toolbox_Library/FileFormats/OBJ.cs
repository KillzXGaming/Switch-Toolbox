using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK;

namespace Switch_Toolbox.Library
{
    public class OBJ
    {
        public static void ExportModel(string FileName, STGenericModel Model, List<STGenericTexture> Textures)
        {
            string fileNoExt = Path.GetFileNameWithoutExtension(FileName);

            //Write model
            StringBuilder writer = new StringBuilder();
            SaveMeshes(writer, Model, $"{fileNoExt}.mtl");
            File.WriteAllText(FileName, writer.ToString());

            //Write materials
            StringBuilder writerMtl = new StringBuilder();
            SaveMaterials(writer, Model);
            File.WriteAllText($"{fileNoExt}.mtl", writerMtl.ToString());
        }

        private static void SaveMeshes(StringBuilder writer, STGenericModel Model, string MtlName)
        {
            writer.AppendLine($"mtllib {MtlName}");

            int VertexID = 1;
            foreach (STGenericObject mesh in Model.Nodes[0].Nodes)
            {
                writer.AppendLine($"o {mesh.Text}");
                writer.AppendLine($"g {mesh.Text}");

                foreach (var v in mesh.vertices)
                {
                    writer.AppendLine($"v {v.pos.X} {v.pos.Y} {v.pos.Z}");
                    writer.AppendLine($"vn {v.nrm.X} {v.nrm.Y} {v.nrm.Z}");
                    writer.AppendLine($"vt {v.uv0.X} {v.uv0.Y}");
                }
                var mat = GetMaterial(mesh.MaterialIndex, Model);

                if (mat != null)
                    writer.AppendLine($"usemtl {mat.Text}");

                for (int i = 0; i < mesh.faces.Count; i += 3)
                {
                    int FaceInd0 = VertexID + mesh.faces[i];
                    int FaceInd1 = VertexID + mesh.faces[i + 1];
                    int FaceInd2 = VertexID + mesh.faces[i + 2];

                    writer.AppendLine($"f {FaceInd0}/{FaceInd0}/{FaceInd0}" +
                                       $" {FaceInd1}/{FaceInd1}/{FaceInd1}" +
                                       $" {FaceInd2}/{FaceInd2}/{FaceInd2}");
                }
            }
        }

        private static STGenericMaterial GetMaterial(int MaterialIndex, STGenericModel Model)
        {
            if (MaterialIndex < Model.Nodes[1].Nodes.Count)
                return (STGenericMaterial)Model.Nodes[1].Nodes[MaterialIndex];
            else
                return null;
        }

        public static float Ns = -3.921569f;
        public static Vector3 Ka = new Vector3(1.000000f, 1.000000f, 1.000000f);
        public static Vector3 Kd = new Vector3(0.640000f, 0.640000f, 0.640000f);
        public static Vector3 Ks = new Vector3(0.500000f, 0.500000f, 0.500000f);
        public static Vector3 Ke = new Vector3(0.000000f, 0.000000f, 0.000000f);
        public static float Ni = -1f;
        public static float d = -1f;
        public static float illum = 2;

        private static void SaveMaterials(StringBuilder writer, STGenericModel Model)
        {
            foreach (STGenericMaterial mat in Model.Nodes[1].Nodes)
            {
                writer.AppendLine($"newmtl {mat.Text}");
                writer.AppendLine($"Ns {Ns}");
                writer.AppendLine($"Ka {Ka}");
                writer.AppendLine($"Kd {Kd}");
                writer.AppendLine($"Ks {Ks}");
                writer.AppendLine($"Ke {Ke}");
                writer.AppendLine($"Ni {Ni}");
                writer.AppendLine($"d {d}");
                writer.AppendLine($"illum {illum}");

                foreach (var tex in mat.TextureMaps)
                {
                    if (tex.Type == STGenericMatTexture.TextureType.Diffuse)
                        writer.AppendLine($"map_Kd {tex.Name}.png");
                }
            }
        }
    }
}
