using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using System.Windows.Forms;
using OpenTK;

namespace FirstPlugin
{
    public class CsvModel : IFileFormat
    {
        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "CSV Model" };
        public string[] Extension { get; set; } = new string[] { "*.csv" };
        public string Magic { get; set; } = "";
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }
        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public List<STGenericObject> objects = new List<STGenericObject>();

        public class Model
        {
            public string Name { get; set; }
            public int UVChannelCount { get; set; }
            public DataType type;
            public DataSubType subType;

            public List<string[]> bones = new List<string[]>();
            public List<float[]> weights = new List<float[]>();
        }
        public enum DataType : int
        {
            vertex = 1,
            faces = 2,
            bones = 3,
        }
        public enum DataSubType : int
        {
            position = 0,
            normals = 1,
            colors = 2,
            uv0 = 3,
            uv1 = 4,
            uv2 = 5,
            uv3 = 6,
        }

        float X, Y, Z, W;
        public void Load()
        {
            LoadFile(FilePath);
        }
        public void LoadFile(string FileName, bool IsModel = false)
        {
            if (!IsModel)
            {
                MessageBox.Show("Not valid model csv");
                return;
            }

            string line = null;

            List<Model> models = new List<Model>();

            TextReader csv = new StreamReader(FileName);
            Model model = new Model();
            STGenericObject STobj = new STGenericObject();
            Vertex vtx = new Vertex();
            STGenericObject.LOD_Mesh lod = new STGenericObject.LOD_Mesh();
            int Index = 0;
            int ww = 0;
            while (true)
            {
                line = csv.ReadLine();
                if (line != null)
                {
                    if (line.StartsWith("Obj Name"))
                    {
                        model = new Model();
                        STobj = new STGenericObject();
                        model.Name = line.Split(':')[1].Replace("\n", "");
                        model.subType = DataSubType.position;
                        models.Add(model);

                        STobj.ObjectName = model.Name;
                        lod = new STGenericObject.LOD_Mesh();
                        lod.IndexFormat = STIndexFormat.UInt16;
                        lod.PrimitiveType = STPolygonType.Triangle;
                        STobj.lodMeshes.Add(lod);
                        STobj.VertexBufferIndex = Index;
                        objects.Add(STobj);
                        Index++;
                    }
                    else if (line.StartsWith("tex_Array:"))
                    {

                    }
                    else if (line.StartsWith("Bone_Suport"))
                    {

                    }
                    else if (line.StartsWith("Color_Suport"))
                    {

                    }
                    else if (line.StartsWith("UV_Num:"))
                    {
                        int uvCount;
                        int.TryParse(line.Split(':')[1].Replace("\n", ""), out uvCount);
                        model.UVChannelCount = uvCount;
                    }
                    else if (line.StartsWith("vert_Array"))
                    {
                        model.type = DataType.vertex;
                    }
                    else if (line.StartsWith("face_Array"))
                    {
                        model.type = DataType.faces;
                    }
                    else if (line.StartsWith("bone_Array"))
                    {
                        model.type = DataType.bones;
                    }
                    else
                    {
                        string[] values = line.Replace("\n", "").Replace("\r", "").Split(',');

                        if (model.type == DataType.vertex)
                        {
                            switch (model.subType)
                            {
                                case DataSubType.position:
                                    vtx = new Vertex();
                                    STobj.vertices.Add(vtx);

                                    STobj.HasPos = true;

                                    float.TryParse(values[0], out X);
                                    float.TryParse(values[1], out Y);
                                    float.TryParse(values[2], out Z);
                                    vtx.pos = new Vector3(X, Y, Z);
                                    model.subType = DataSubType.normals;
                                    break;
                                case DataSubType.normals:
                                    STobj.HasNrm = true;

                                    float.TryParse(values[0], out X);
                                    float.TryParse(values[1], out Y);
                                    float.TryParse(values[2], out Z);
                                    vtx.nrm = new Vector3(X, Y, Z);
                                    model.subType = DataSubType.colors;
                                    break;
                                case DataSubType.colors:
                                    STobj.HasVertColors = true;

                                    float.TryParse(values[0], out X);
                                    float.TryParse(values[1], out Y);
                                    float.TryParse(values[2], out Z);
                                    float.TryParse(values[3], out W);
                                    vtx.col = new Vector4(X / 255, Y / 255, Z / 255, W / 255);
                                    model.subType = DataSubType.uv0;
                                    break;
                                case DataSubType.uv0:
                                    STobj.HasUv0 = true;

                                    float.TryParse(values[0], out X);
                                    float.TryParse(values[1], out Y);
                                    vtx.uv0 = new Vector2(X, Y);
                                    if (model.UVChannelCount == 1)
                                        model.subType = DataSubType.position;
                                    else
                                        model.subType = DataSubType.uv1;
                                    break;
                                case DataSubType.uv1:
                                    STobj.HasUv1 = true;

                                    float.TryParse(values[0], out X);
                                    float.TryParse(values[1], out Y);
                                    vtx.uv1 = new Vector2(X, Y);
                                    if (model.UVChannelCount == 2)
                                        model.subType = DataSubType.position;
                                    else
                                        model.subType = DataSubType.uv2;
                                    break;
                                case DataSubType.uv2:
                                    STobj.HasUv2 = true;

                                    float.TryParse(values[0], out X);
                                    float.TryParse(values[1], out Y);
                                    vtx.uv2 = new Vector2(X, Y);
                                    if (model.UVChannelCount == 3)
                                        model.subType = DataSubType.position;
                                    else
                                        model.subType = DataSubType.uv3;
                                    break;
                                case DataSubType.uv3:
                                    float.TryParse(values[0], out X);
                                    float.TryParse(values[1], out Y);
                                    model.subType = DataSubType.position;
                                    break;
                            }
                        }
                        if (model.type == DataType.faces)
                        {
                            int face;
                            foreach (string v in values)
                            {
                                var cleaned = v.Replace(".0", string.Empty);
                                int.TryParse(cleaned, out face);
                                lod.faces.Add(face-1);
                            }
                        }
                        if (model.type == DataType.bones)
                        {
                            STobj.HasWeights = true;
                            STobj.HasIndices = true;

                            Array.Resize(ref values, values.Length - 1);

                            List<string> bones = new List<string>();
                            List<float> weights = new List<float>();

                            int bbs = 0;
                            foreach (string obj in values)
                            {
                                if (bbs == 0)
                                {
                                    bones.Add(obj);
                                    bbs += 1;
                                }
                                else
                                {
                                    float.TryParse(obj, out X);
                                    weights.Add(X);
                                    bbs = 0;
                                }
                            }

                            STobj.bones.Add(bones.ToArray());
                            STobj.weightsT.Add(weights.ToArray());
                        }
                    }
                }
                else
                    break;
            }

            if (objects[0].weightsT.Count != objects[0].vertices.Count)
                throw new Exception("Incorrect vertex amount");

            foreach (STGenericObject obj in objects)
            {
                obj.lodMeshes[0].GenerateSubMesh();
                for (int v = 0; v < obj.vertices.Count; v++)
                {
                    foreach (string bn in obj.bones[v])
                        obj.vertices[v].boneNames.Add(bn);
                    foreach (float f in obj.weightsT[v])
                        obj.vertices[v].boneWeights.Add(f);
                }
                foreach (Vertex v in obj.vertices)
                {
                    if (v.boneNames.Count == 1)
                        Console.WriteLine($"{v.boneNames[0]} {v.boneWeights[0]}");
                    if (v.boneNames.Count == 2)
                        Console.WriteLine($"{v.boneNames[0]} {v.boneWeights[0]} {v.boneNames[1]} {v.boneWeights[1]}");
                    if (v.boneNames.Count == 3)
                        Console.WriteLine($"{v.boneNames[0]} {v.boneWeights[0]} {v.boneNames[1]} {v.boneWeights[1]} {v.boneNames[2]} {v.boneWeights[2]}");
                    if (v.boneNames.Count == 4)
                        Console.WriteLine($"{v.boneNames[0]} {v.boneWeights[0]} {v.boneNames[1]} {v.boneWeights[1]} {v.boneNames[2]} {v.boneWeights[2]} {v.boneNames[3]} {v.boneWeights[3]}");
                }
            }

            csv.Close();
            csv = null;
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(mem))
            {
                foreach (STGenericObject obj in objects)
                {
                    file.WriteLine($"Obj Name:" + obj.ObjectName);
                    file.WriteLine($"Bone_Suport");
                    file.WriteLine($"UV_Num:1");
                    file.WriteLine($"vert_Array");

                    foreach (Vertex v in obj.vertices)
                    {
                        file.WriteLine($"{v.pos.X},{v.pos.Y},{v.pos.Z}");
                        file.WriteLine($"{v.nrm.X},{v.nrm.Y},{v.nrm.Z}");
                        file.WriteLine($"{v.col.X * 255},{v.col.Y * 255},{v.col.Z * 255},{v.col.W * 255}");
                        file.WriteLine($"{v.uv0.X},{v.uv0.Y}");
                   //     file.WriteLine($"{v.uv1.X},{v.uv1.Y}");
                    }
                    file.WriteLine($"face_Array");
                    for (int f = 0; f < obj.faces.Count / 3; f++)
                    {
                        file.WriteLine($"{obj.faces[f] + 1},{obj.faces[f++] + 1},{obj.faces[f++] + 1}");
                    }
                    file.WriteLine($"bone_Array");
                    foreach (Vertex v in obj.vertices)
                    {
                        if (v.boneNames.Count == 1)
                            file.WriteLine($"{v.boneNames[0]} {v.boneWeights[0]}");
                        if (v.boneNames.Count == 2)
                            file.WriteLine($"{v.boneNames[0]} {v.boneWeights[0]} {v.boneNames[1]} {v.boneWeights[1]}");
                        if (v.boneNames.Count == 3)
                            file.WriteLine($"{v.boneNames[0]} {v.boneWeights[0]} {v.boneNames[1]} {v.boneWeights[1]} {v.boneNames[2]} {v.boneWeights[2]}");
                        if (v.boneNames.Count == 4)
                            file.WriteLine($"{v.boneNames[0]} {v.boneWeights[0]} {v.boneNames[1]} {v.boneWeights[1]} {v.boneNames[2]} {v.boneWeights[2]} {v.boneNames[3]} {v.boneWeights[3]}");
                    }
                }
                file.Close();
            }


            return mem.ToArray();
        }
    }
}
