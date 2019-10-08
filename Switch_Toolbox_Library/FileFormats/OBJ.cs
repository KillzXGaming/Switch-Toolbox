using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OpenTK;

namespace Toolbox.Library
{
    public class OBJ
    {
        public static void ExportModel(string FileName, STGenericModel Model, List<STGenericTexture> Textures)
        {
            string fileNoExt = Path.GetFileNameWithoutExtension(FileName);
            string fileMtlPath = FileName.Replace("obj", "mtl");

            //Write model
            StringBuilder writer = new StringBuilder();
            SaveMeshes(writer, Model, $"{fileNoExt}.mtl");
            File.WriteAllText(FileName, writer.ToString());

            //Write materials
            StringBuilder writerMtl = new StringBuilder();
      //      SaveMaterials(writerMtl, Model);
            File.WriteAllText(fileMtlPath, writerMtl.ToString());
        }

        public static void ExportMesh(string FileName, STGenericObject genericMesh)
        {
            string fileNoExt = Path.GetFileNameWithoutExtension(FileName);
            string fileMtlPath = FileName.Replace("obj", "mtl");

            //Write mesh
            StringBuilder writer = new StringBuilder();
            SaveMesh(writer, genericMesh, null, 0);
            File.WriteAllText(FileName, writer.ToString());
        }

        private static void SaveMeshes(StringBuilder writer, STGenericModel Model, string MtlName)
        {
            writer.AppendLine($"mtllib {MtlName}");

            int VertexCount = 1;
            foreach (STGenericObject mesh in Model.Objects)
            {
                SaveMesh(writer, mesh, null, VertexCount);
            }
        }

        private static void SaveMesh(StringBuilder writer,  STGenericObject mesh, STGenericMaterial mat, int VertexCount)
        {
            writer.AppendLine($"o {mesh.Text}");
            writer.AppendLine($"g {mesh.Text}");

            foreach (var v in mesh.vertices)
            {
                writer.AppendLine($"v {v.pos.X} {v.pos.Y} {v.pos.Z}");
                writer.AppendLine($"vn {v.nrm.X} {v.nrm.Y} {v.nrm.Z}");
                writer.AppendLine($"vt {v.uv0.X} {v.uv0.Y}");
            }

            if (mat != null)
                writer.AppendLine($"usemtl {mat.Text}");

            for (int i = 0; i < mesh.faces.Count; i++)
            {
                int[] indices = new int[3]
                {
                        mesh.faces[i++],
                        mesh.faces[i++],
                        mesh.faces[i]
                };

                writer.AppendLine($"f {indices[0] + VertexCount}/{indices[0] + VertexCount}/{indices[0] + VertexCount}" +
                                   $" {indices[1] + VertexCount}/{indices[1] + VertexCount}/{indices[1] + VertexCount}" +
                                   $" {indices[2] + VertexCount}/{indices[2] + VertexCount}/{indices[2] + VertexCount}");
            }

            VertexCount += mesh.vertices.Count;
        }

        private static STGenericMaterial GetMaterial(int MaterialIndex, STGenericModel Model)
        {
            int numMaterials = 0;
            foreach (var mat in Model.Materials)
                numMaterials++;

            if (MaterialIndex < numMaterials)
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
                writer.AppendLine($"Ka {Ka.X} {Ka.Y} {Ka.Z}");
                writer.AppendLine($"Kd {Kd.X} {Kd.Y} {Kd.Z}");
                writer.AppendLine($"Ks {Ks.X} {Ks.Y} {Ks.Z}");
                writer.AppendLine($"Ke {Ke.X} {Ke.Y} {Ke.Z}");
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
