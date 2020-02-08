using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace Toolbox.Library.Collada
{
    public class ColladaWriter : IDisposable
    {
        //Based around this nice Writer https://raw.githubusercontent.com/Ploaj/SSBHLib/master/CrossMod/IO/DAEWriter.cs
        //With adjustments and expanded usage.

        private XmlTextWriter Writer;
        private DAE.ExportSettings Settings;
        private DAE.Version Version;

        private List<Tuple<string, SemanticType, TriangleList[], int>> GeometrySources = new List<Tuple<string, SemanticType, TriangleList[], int>>();

        private Dictionary<string, int> AttributeIdList = new Dictionary<string, int>();

        private Dictionary<string, Tuple<List<int[]>, List<float[]>>> GeometryControllers = new Dictionary<string, Tuple<List<int[]>, List<float[]>>>();

        //Keep a dictionary of names and assigned id names
        private Dictionary<string, string> MeshIdList = new Dictionary<string, string>();
        private Dictionary<string, List<string>> MaterialIdList = new Dictionary<string, List<string>>();

        private Dictionary<string, string> MeshSkinIdList = new Dictionary<string, string>();

        private List<Joint> Joints = new List<Joint>();

        public string CurrentGeometryID;
        public string CurrentMaterial;

        public bool Optimize = false;

        public ColladaWriter(string fileName, DAE.ExportSettings settings)
        {
            Settings = settings;
            Version = settings.FileVersion;
            Writer = new XmlTextWriter(fileName, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                Indentation = 2,
            };

            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            WriteDAEHeader();
        }

        public void WriteDAEHeader()
        {
            Writer.WriteStartDocument();
            Writer.WriteStartElement("COLLADA");
            Writer.WriteAttributeString("xmlns", "http://www.collada.org/2005/11/COLLADASchema");
            Writer.WriteAttributeString("version", $"{Version.Major}.{Version.Minor}.{Version.Micro}");
        }

        public void WriteAsset()
        {
            Writer.WriteStartElement("asset");
            Writer.WriteEndElement();
        }

        public void WriteFileSettings()
        {
            Writer.WriteStartElement("asset");
            {
                Writer.WriteStartElement("contributor");
                Writer.WriteElementString("authoring_tool", System.Windows.Forms.Application.ProductName);
                Writer.WriteEndElement();

                Writer.WriteStartElement("created");
                Writer.WriteString(DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z");
                Writer.WriteEndElement();

                Writer.WriteStartElement("modified");
                Writer.WriteString(DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z");
                Writer.WriteEndElement();

                Writer.WriteStartElement("unit");
                if (Settings.Preset == ProgramPreset.MAX)
                {
                    Writer.WriteAttributeString("meter", "1");
                    Writer.WriteAttributeString("name", "centimeter");
                }
                else
                {
                    Writer.WriteAttributeString("meter", "0.01");
                    Writer.WriteAttributeString("name", "centimeter");
                }
                Writer.WriteEndElement();

                Writer.WriteStartElement("up_axis");
                Writer.WriteString("Y_UP");
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();
        }

        public void WriteLibraryImages(string[] textureNames = null)
        {
            Writer.WriteStartElement("library_images");
            for (int i = 0; i < textureNames?.Length; i++)
            {
                Writer.WriteStartElement("image");
                Writer.WriteAttributeString("id", textureNames[i]);
                Writer.WriteStartElement("init_from");
                Writer.WriteString($"{Settings.ImageFolder}{textureNames[i]}{Settings.ImageExtension}");
                Writer.WriteEndElement();
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();
        }

        public void WriteLibraryMaterials(List<Material> materials)
        {
            Writer.WriteStartElement("library_materials");
            for (int i = 0; i < materials?.Count; i++)
            {
                Writer.WriteStartElement("material");
                Writer.WriteAttributeString("id", materials[i].Name);
                Writer.WriteStartElement("instance_effect");
                Writer.WriteAttributeString("url", "#Effect_" + materials[i].Name);
                Writer.WriteEndElement();
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();
        }

        public void WriteLibraryEffects(List<Material> materials)
        {
            Writer.WriteStartElement("library_effects");
            for (int i = 0; i < materials?.Count; i++)
            {
                Writer.WriteStartElement("effect");
                Writer.WriteAttributeString("id", $"Effect_{materials[i].Name}");
                Writer.WriteStartElement("profile_COMMON");
                {
                    for (int t = 0; t < materials[i].Textures.Count; t++)
                    {
                        var tex = materials[i].Textures[t];
                        //Here we write 2 things. Surface then sampler info

                        //Surface info
                        Writer.WriteStartElement("newparam");
                        Writer.WriteAttributeString("sid", $"surface_{materials[i].Name}-{tex.Type}");
                        Writer.WriteStartElement("surface");
                        Writer.WriteAttributeString("type", "2D");
                        {
                            Writer.WriteStartElement("init_from");
                            Writer.WriteString(tex.Name);
                            Writer.WriteEndElement();
                            Writer.WriteStartElement("format");
                            //Formats are uncompressed types. 
                            //Todo these should be adjusted for DDS
                            Writer.WriteString("A8R8G8B8");
                            Writer.WriteEndElement();
                        }
                        Writer.WriteEndElement();
                        Writer.WriteEndElement();

                        {
                            //Sampler info
                            Writer.WriteStartElement("newparam");
                            Writer.WriteAttributeString("sid", $"sampler_{materials[i].Name}-{tex.Type}");
                            Writer.WriteStartElement("sampler2D");

                            Writer.WriteStartElement("source");
                            Writer.WriteString($"surface_{materials[i].Name}-{tex.Type}");
                            Writer.WriteEndElement();

                            //Add sampler wrap modes
                            Writer.WriteStartElement("wrap_s");
                            Writer.WriteString(tex.WrapModeS.ToString());
                            Writer.WriteEndElement();

                            Writer.WriteStartElement("wrap_t");
                            Writer.WriteString(tex.WrapModeT.ToString());
                            Writer.WriteEndElement();


                            Writer.WriteEndElement();
                            Writer.WriteEndElement();
                        }

                    }

                    //Now write texture info
                    //Todo support PBR workflow

                    Writer.WriteStartElement("technique");
                    Writer.WriteAttributeString("sid", "common");
                    Writer.WriteStartElement("phong");

                    bool HasSpecular = false;
                    bool HasEmission = false;
                    bool HasDiffuse = false;

                    for (int t = 0; t < materials[i].Textures.Count; t++)
                    {
                        var tex = materials[i].Textures[t];

                        if (tex.Type == PhongTextureType.diffuse)
                            HasDiffuse = true;
                        if (tex.Type == PhongTextureType.emission)
                            HasEmission = true;
                        if (tex.Type == PhongTextureType.specular)
                            HasSpecular = true;

                        Writer.WriteStartElement($"{tex.Type}");
                        Writer.WriteStartElement("texture");
                        Writer.WriteAttributeString("texture", $"sampler_{materials[i].Name}-{tex.Type}");
                        Writer.WriteAttributeString("texcoord", $"{tex.TextureChannel}");

                        Writer.WriteEndElement();
                        Writer.WriteEndElement();
                    }

                    if (!HasDiffuse)
                        WriteMaterialColor("diffuse", new float[] { 1, 1, 1, 1 });
                    if (!HasEmission)
                        WriteMaterialColor("emission", new float[] { 0, 0, 0, 1 });
                    if (!HasSpecular)
                        WriteMaterialColor("specular", new float[] { 0, 0, 0, 1 });

                    Writer.WriteEndElement();
                    Writer.WriteEndElement();
                }
                Writer.WriteEndElement();
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();
        }

        public void WriteMaterialColor(string type, float[] color)
        {
            Writer.WriteStartElement(type);
            Writer.WriteStartElement("color");
            Writer.WriteAttributeString("sid", type);
            Writer.WriteString(string.Join(" ", color));
            Writer.WriteEndElement();
            Writer.WriteEndElement();
        }

        public void StartLibraryGeometries() {
            Writer.WriteStartElement("library_geometries");
        }

        public void StartGeometry(string name)
        {
            CurrentGeometryID = GetUniqueID($"{name}-mesh");
            MeshSkinIdList.Add(CurrentGeometryID, "");
            MeshIdList.Add(CurrentGeometryID, name);
            Writer.WriteStartElement("geometry");
            Writer.WriteAttributeString("id", CurrentGeometryID);
            Writer.WriteAttributeString("name", name);
            Writer.WriteStartElement("mesh");
        }

        private void OptimizeSource<T>(T[] values, TriangleList[] triangleLists, int stride, out T[] newValues, out TriangleList[] newIndices) where T : struct
        {
            var optimizedIndices = new List<uint>();
            var optimizedValues = new List<T>();

            // Use a special class to store values, so we can have the performance benefits of a dictionary.
            var vertexBank = new Dictionary<ValueContainer<T>, uint>();

            for (int t = 0; t < triangleLists.Length; t++)
            {
                for (int i = 0; i < triangleLists[t].Indices.Count; i++)
                {
                    var vertexValues = new ValueContainer<T>(GetVertexValues(values, triangleLists[t].Indices.ToArray(), stride, i));

                    if (!vertexBank.ContainsKey(vertexValues))
                    {
                        uint index = (uint)vertexBank.Count;
                        optimizedIndices.Add(index);
                        vertexBank.Add(vertexValues, index);
                        optimizedValues.AddRange(vertexValues.Values);
                    }
                    else
                    {
                        optimizedIndices.Add(vertexBank[vertexValues]);

                    }
                }

                triangleLists[t].Indices = optimizedIndices;
            }

            newIndices = triangleLists;
            newValues = optimizedValues.ToArray();
        }

        private static T[] GetVertexValues<T>(T[] values, uint[] indices, int stride, int i)
        {
            var vertexValues = new T[stride];
            for (int j = 0; j < stride; j++)
                vertexValues[j] = values[indices[i] * stride + j];

            return vertexValues;
        }

        public void WriteGeometrySource(string name, SemanticType semantic, float[] values, TriangleList[] indices, int set = -1)
        {
            int stride = GetStride(semantic);

            if (Optimize)
                OptimizeSource(values, indices, stride, out values, out indices);

            string sourceid = GetUniqueID(name + "-" + semantic.ToString().ToLower());
            Writer.WriteStartElement("source");
            Writer.WriteAttributeString("id", sourceid);

            Writer.WriteStartElement("float_array");
            string FloatArrayID = GetUniqueID(name + "-" + semantic.ToString().ToLower() + "-array");
            Writer.WriteAttributeString("id", FloatArrayID);
            Writer.WriteAttributeString("count", values.Length.ToString());
            Writer.WriteString(string.Join(" ", values));
            Writer.WriteEndElement();

            Writer.WriteStartElement("technique_common");
            {
                Writer.WriteStartElement("accessor");
                Writer.WriteAttributeString("source", $"#{FloatArrayID}");
                Writer.WriteAttributeString("count", (values.Length / stride).ToString());
                Writer.WriteAttributeString("stride", stride.ToString());
                if (semantic == SemanticType.NORMAL || semantic == SemanticType.POSITION)
                {
                    WriteParam("X", "float");
                    WriteParam("Y", "float");
                    WriteParam("Z", "float");
                }
                if (semantic == SemanticType.TEXCOORD)
                {
                    WriteParam("S", "float");
                    WriteParam("T", "float");
                }
                if (semantic == SemanticType.COLOR)
                {
                    WriteParam("R", "float");
                    WriteParam("G", "float");
                    WriteParam("B", "float");
                    WriteParam("A", "float");
                }
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();
            Writer.WriteEndElement();

            GeometrySources.Add(new Tuple<string, SemanticType, TriangleList[], int>(sourceid, semantic, indices, set));
        }

        private int GetStride(SemanticType semantic) {
            if (semantic == SemanticType.COLOR)
                return 4;
            else if (semantic == SemanticType.TEXCOORD)
                return 2;
            else if (semantic == SemanticType.JOINT)
                return 1;
            else if (semantic == SemanticType.INV_BIND_MATRIX)
                return 16;
            else if (semantic == SemanticType.WEIGHT)
                return 1;
            else
                return 3;
        }

        private void WriteParam(string Name, string Type)
        {
            Writer.WriteStartElement("param");
            Writer.WriteAttributeString("name", Name);
            Writer.WriteAttributeString("type", Type);
            Writer.WriteEndElement();
        }

        public void EndGeometryMesh()
        {
            Tuple<string, SemanticType, TriangleList[], int> Position = null;
            foreach (var v in GeometrySources)
            {
                if (v.Item2 == SemanticType.POSITION)
                {
                    Position = v;
                    break;
                }
            }

            // can only write vertex information if there is at least position data
            if (Position != null)
            {
                // vertices
                string verticesid = GetUniqueID(Position.Item1.Replace("position", "vertex"));
                Writer.WriteStartElement("vertices");
                Writer.WriteAttributeString("id", verticesid);
                WriteInput(Position.Item2.ToString(), Position.Item1);
                Writer.WriteEndElement();

                List<string> triangleMaterials = new List<string>();

                // triangles
                int triIndex = 0;
                foreach (TriangleList triangleList in Position.Item3)
                {
                    Writer.WriteStartElement("triangles");
                    if (triangleList.Material != "")
                    {
                        Writer.WriteAttributeString("material", $"{triangleList.Material}");
                        triangleMaterials.Add(triangleList.Material);
                    }
                    else if (CurrentMaterial != "")
                    {
                        Writer.WriteAttributeString("material", $"{CurrentMaterial}");
                        triangleMaterials.Add(CurrentMaterial);
                        CurrentMaterial = "";
                    }

                    Writer.WriteAttributeString("count", (triangleList.Indices.Count / 3).ToString());
                    WriteInput("VERTEX", $"{verticesid}", 0);
                    int offset = 1;
                    foreach (var v in GeometrySources)
                        if (v.Item2 != SemanticType.POSITION)
                        {
                            WriteInput(v.Item2.ToString(), $"{v.Item1}", offset++, v.Item4);
                        }
                    // write p
                    StringBuilder p = new StringBuilder();
                    for (int i = 0; i < triangleList.Indices.Count; i++)
                    {
                        p.Append(triangleList.Indices[i] + " ");
                        foreach (var v in GeometrySources)
                            if (v.Item2 != SemanticType.POSITION)
                                p.Append(v.Item3[triIndex].Indices[i] + " ");
                    }
                    Writer.WriteStartElement("p");
                    Writer.WriteString(p.ToString());
                    Writer.WriteEndElement();

                    Writer.WriteEndElement();

                    triIndex++;
                }

                MaterialIdList.Add(CurrentGeometryID, triangleMaterials);
            }

            GeometrySources.Clear();
            Writer.WriteEndElement();
            Writer.WriteEndElement();
        }

        public void WriteInput(string semantic, string sourceid, int offset = -1, int set = -1)
        {
            Writer.WriteStartElement("input");
            Writer.WriteAttributeString("semantic", semantic);
            Writer.WriteAttributeString("source", $"#{sourceid}");
            if (offset != -1)
                Writer.WriteAttributeString("offset", offset.ToString());
            if (set != -1)
                Writer.WriteAttributeString("set", set.ToString());
            Writer.WriteEndElement();
        }

        /// <summary>
        /// A function for automatically creating and handling the controllers
        /// The Joints must be added before this function is used
        /// </summary>
        /// <param name="BoneIndices">A list of arrays of bone indices for each vertex</param>
        /// <param name="Weights">A list that contains an array of weights per vertex</param>
        public void AttachGeometryController(List<int[]> BoneIndices, List<float[]> Weights)
        {
            GeometryControllers.Add(CurrentGeometryID, new Tuple<List<int[]>, List<float[]>>(BoneIndices, Weights));
        }

        /// <summary>
        /// Ends Writing the Geometry Section
        /// </summary>
        public void EndGeometrySection()
        {
            Writer.WriteEndElement();

            if (Joints?.Count > 0)
                CreateControllerSection();

            CreateVisualNodeSection();
        }

        private void RecursivlyWriteJoints(Joint joint)
        {
            Writer.WriteStartElement("node");
            Writer.WriteAttributeString("id", $"Armature_{joint.Name}");
            Writer.WriteAttributeString("name", joint.Name);
            Writer.WriteAttributeString("sid", joint.Name);
            Writer.WriteAttributeString("type", "JOINT");

            Writer.WriteStartElement("matrix");
            Writer.WriteAttributeString("sid", "transform");
            Writer.WriteString(string.Join(" ", joint.Transform));
            Writer.WriteEndElement();

            foreach (var child in GetChildren(joint))
            {
                RecursivlyWriteJoints(child);
            }

            Writer.WriteEndElement();
        }

        private void WriteTransform(float[] scale, float[] rotate, float[] translate)
        {
            WriteNodeElement("translate", "location", translate);
            WriteNodeElement("rotate", "rotationZ", new float[] { 0, 0, 1, rotate[2] });
            WriteNodeElement("rotate", "rotationY", new float[] { 0, 1, 0, rotate[1] });
            WriteNodeElement("rotate", "rotationX", new float[] { 1, 0, 0, rotate[0] });
            WriteNodeElement("scale", "scale", scale);
        }

        private void WriteNodeElement(string name, string sid, float[] values)
        {
            Writer.WriteStartElement(name);
            Writer.WriteAttributeString("sid", sid);
            Writer.WriteString(string.Join(" ", values));
            Writer.WriteEndElement();
        }

        private Joint[] GetChildren(Joint j)
        {
            int parentindex = Joints.IndexOf(j);
            List<Joint> Children = new List<Joint>();
            foreach (var child in Joints)
            {
                if (child.ParentIndex == parentindex)
                    Children.Add(child);
            }
            return Children.ToArray();
        }

        /// <summary>
        /// Starts the library controller section
        /// </summary>
        public void BeginLibraryControllers()
        {
            Writer.WriteStartElement("library_controllers");
        }

        /// <summary>
        /// automatically generates the controller nodes
        /// </summary>
        private void CreateControllerSection()
        {
            BeginLibraryControllers();

            foreach (var v in GeometryControllers.Keys)
            {
                WriteLibraryController(v);
            }

            EndLibraryControllers();
        }


        /// <summary>
        /// Adds a new joint to the default skeletal tree
        /// </summary>
        public void AddJoint(string name, string parentName,
            float[] Transform, float[] InvWorldTransform,
            float[] translate, float[] rotate, float[] scale)
        {
            Joint j = new Joint();
            j.Name = name;
            j.Transform = Transform;
            j.BindPose = InvWorldTransform;
            j.Scale = scale;
            j.Translate = translate;
            j.Rotate = rotate;
            foreach (var joint in Joints)
                if (joint.Name.Equals(parentName))
                    j.ParentIndex = Joints.IndexOf(joint);
            Joints.Add(j);
        }

        public void WriteLibraryController(string Name)
        {
            if (!GeometryControllers.ContainsKey(Name)) return;

            var BoneWeight = GeometryControllers[Name];
            string SkinID = $"Armature_{Name}";
            MeshSkinIdList[Name] = SkinID;

            Writer.WriteStartElement("controller");
            Writer.WriteAttributeString("id", SkinID);

            Writer.WriteStartElement("skin");
            Writer.WriteAttributeString("source", $"#{Name}");
            {
                Writer.WriteStartElement("bind_shape_matrix");
                Writer.WriteString("1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 1");
                Writer.WriteEndElement();

                List<Joint> meshJoints = new List<Joint>();
                List<int> indexTable = new List<int>();

                //Go through each bone index and find each rigged joint the mesh uses
                for (int i = 0; i < BoneWeight.Item1.Count; i++)
                {
                    for (int j = 0; j < BoneWeight.Item1[i].Length; j++)
                    {
                        int index = BoneWeight.Item1[i][j];
                        if (index < Joints.Count && index != -1 && !meshJoints.Contains(Joints[index]))
                            meshJoints.Add(Joints[index]);
                    }
                }

                //Create a lookup table to find the joint list from all the joints that get indexed
                for (int i = 0; i < Joints.Count; i++)
                {
                    indexTable.Add(meshJoints.IndexOf(Joints[i]));
                }

                object[] BoneNames = new string[meshJoints.Count];
                object[] InvBinds = new object[meshJoints.Count * 16];
                for (int i = 0; i < BoneNames.Length; i++)
                {
                    BoneNames[i] = meshJoints[i].Name;
                    for (int j = 0; j < 16; j++)
                    {
                        InvBinds[i * 16 + j] = meshJoints[i].BindPose[j];
                    }
                }

                var Weights = new List<object>();
                var WeightIndices = new List<int>();
                foreach (var v in BoneWeight.Item2)
                {
                    foreach (var w in v)
                    {
                        int index = Weights.IndexOf(w);

                        if (index == -1)
                        {
                            WeightIndices.Add(Weights.Count);
                            Weights.Add(w);
                        }
                        else
                        {
                            WeightIndices.Add(index);
                        }
                    }
                }
                string Jointid = WriteSkinSource(Name, SemanticType.JOINT, BoneNames);
                string Bindid = WriteSkinSource(Name, SemanticType.INV_BIND_MATRIX, InvBinds);
                string Weightid = WriteSkinSource(Name, SemanticType.WEIGHT, Weights.ToArray());


                Writer.WriteStartElement("joints");
                WriteInput(SemanticType.JOINT.ToString(), Jointid);
                WriteInput(SemanticType.INV_BIND_MATRIX.ToString(), Bindid);
                Writer.WriteEndElement();

                Writer.WriteStartElement("vertex_weights");
                Writer.WriteAttributeString("count", BoneWeight.Item1.Count.ToString());
                WriteInput(SemanticType.JOINT.ToString(), Jointid, 0);
                WriteInput(SemanticType.WEIGHT.ToString(), Weightid, 1);

                // now writing out the counts and such...
                //vcount
                {
                    StringBuilder values = new StringBuilder();
                    foreach (var v in BoneWeight.Item1)
                    {
                        values.Append($"{v.Length} ");
                    }
                    Writer.WriteStartElement("vcount");
                    Writer.WriteString(values.ToString());
                    Writer.WriteEndElement();
                }

                //v
                {
                    StringBuilder values = new StringBuilder();
                    int weightindexcount = 0;
                    for (int i = 0; i < BoneWeight.Item1.Count; i++)
                    {
                        for (int j = 0; j < BoneWeight.Item1[i].Length; j++)
                        {
                            values.Append($"{indexTable[BoneWeight.Item1[i][j]]} {WeightIndices[weightindexcount++]} ");
                        }
                    }
                    Writer.WriteStartElement("v");
                    Writer.WriteString(values.ToString());
                    Writer.WriteEndElement();
                }

                Writer.WriteEndElement();
            }

            Writer.WriteEndElement();
            Writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a skin source accessor for the current skin controller
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Semantic"></param>
        /// <param name="Values"></param>
        /// <returns>the id of the newly created skin source</returns>
        public string WriteSkinSource(string Name, SemanticType Semantic, object[] Values)
        {
            int Stride = GetStride(Semantic);

            string sourceid = GetUniqueID(Name + "-" + Semantic.ToString().ToLower());
            Writer.WriteStartElement("source");
            Writer.WriteAttributeString("id", sourceid);

            Writer.WriteStartElement(Semantic == SemanticType.JOINT ? "Name_array" : "float_array");
            string FloatArrayID = GetUniqueID(Name + "-" + Semantic.ToString().ToLower() + "-array");
            Writer.WriteAttributeString("id", FloatArrayID);
            Writer.WriteAttributeString("count", Values.Length.ToString());
            Writer.WriteString(string.Join(" ", Values));
            Writer.WriteEndElement();

            Writer.WriteStartElement("technique_common");
            {
                Writer.WriteStartElement("accessor");
                Writer.WriteAttributeString("source", $"#{FloatArrayID}");
                Writer.WriteAttributeString("count", (Values.Length / Stride).ToString());
                Writer.WriteAttributeString("stride", Stride.ToString());
                if (Semantic == SemanticType.JOINT)
                {
                    WriteParam("JOINT", "name");
                }
                if (Semantic == SemanticType.INV_BIND_MATRIX)
                {
                    WriteParam("TRANSFORM", "float4x4");
                }
                if (Semantic == SemanticType.WEIGHT)
                {
                    WriteParam("WEIGHT", "float");
                }
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();

            Writer.WriteEndElement();

            return sourceid;
        }

        /// <summary>
        /// Ends the Library Controller Section
        /// </summary>
        public void EndLibraryControllers()
        {
            Writer.WriteEndElement();
        }



        public void BeginVisualNodeSection()
        {
            Writer.WriteStartElement("library_visual_scenes");
            Writer.WriteStartElement("visual_scene");
            Writer.WriteAttributeString("id", "Scene");
            Writer.WriteAttributeString("name", "Scene");
        }

        public void CreateVisualNodeSection()
        {
            BeginVisualNodeSection();
            Writer.WriteStartElement("node");
            Writer.WriteAttributeString("id", "Armature");
            Writer.WriteAttributeString("name", "Armature");
            Writer.WriteAttributeString("type", "NODE");

            Writer.WriteStartElement("matrix");
            Writer.WriteAttributeString("sid", "transform");
            Writer.WriteString("1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 1");
            Writer.WriteEndElement();

            foreach (var joint in Joints)
                if (joint.ParentIndex == -1)
                    RecursivlyWriteJoints(joint);

            Writer.WriteEndElement();

            // write geometry nodes

            foreach (var m in MeshSkinIdList)
            {
                Writer.WriteStartElement("node");
                Writer.WriteAttributeString("id", MeshIdList[m.Key]);
                Writer.WriteAttributeString("name", MeshIdList[m.Key]);
                Writer.WriteAttributeString("type", "NODE");

                if (m.Value.Equals(""))
                {
                    Writer.WriteStartElement("instance_geometry");
                    Writer.WriteAttributeString("url", $"#{m.Key}");
                    Writer.WriteAttributeString("name", MeshIdList[m.Key]);

                    if (MaterialIdList.ContainsKey(m.Key))
                    {
                        Writer.WriteStartElement("bind_material");
                        Writer.WriteStartElement("technique_common");
                        foreach (var mat in MaterialIdList[m.Key])
                        {
                            Writer.WriteStartElement("instance_material");
                            Writer.WriteAttributeString("symbol", mat);
                            Writer.WriteAttributeString("target", "#" + mat);
                            WriteChannel(0);
                            WriteChannel(1);
                            WriteChannel(2);
                            Writer.WriteEndElement();
                        }
                        Writer.WriteEndElement();
                        Writer.WriteEndElement();
                    }

                    Writer.WriteEndElement();
                }
                else
                {
                    Writer.WriteStartElement("instance_controller");
                    Writer.WriteAttributeString("url", $"#{m.Value}");
                    Writer.WriteStartElement("skeleton");
                    Writer.WriteString("#Armature_" + Joints[0].Name);
                    Writer.WriteEndElement();

                    if (MaterialIdList.ContainsKey(m.Key))
                    {
                        Writer.WriteStartElement("bind_material");
                        Writer.WriteStartElement("technique_common");
                        foreach (var mat in MaterialIdList[m.Key])
                        {
                            Writer.WriteStartElement("instance_material");
                            Writer.WriteAttributeString("symbol", mat);
                            Writer.WriteAttributeString("target", "#" + mat);
                            Writer.WriteEndElement();

                            WriteChannel(0);
                            WriteChannel(1);
                            WriteChannel(2);
                        }
                        Writer.WriteEndElement();
                        Writer.WriteEndElement();
                    }
                    Writer.WriteEndElement();
                }


                Writer.WriteEndElement();
            }

            EndVisualNodeSection();
        }

        public void WriteChannel(int index)
        {
            Writer.WriteStartElement("bind_vertex_input");
            Writer.WriteAttributeString("semantic", $"CHANNEL{index}");
            Writer.WriteAttributeString("input_semantic", "TEXCOORD");
            Writer.WriteAttributeString("input_set", index.ToString());
            Writer.WriteEndElement();
        }

        public void WriteNode(string Name)
        {
            Writer.WriteStartElement("node");
            Writer.WriteAttributeString("id", Name);
            Writer.WriteAttributeString("name", Name);
            Writer.WriteAttributeString("type", "NODE");
            Writer.WriteEndElement();
        }

        public void EndVisualNodeSection()
        {
            Writer.WriteEndElement();
            Writer.WriteEndElement();

            Writer.WriteStartElement("scene");
            Writer.WriteStartElement("instance_visual_scene");
            Writer.WriteAttributeString("url", "#Scene");
            Writer.WriteEndElement();
            Writer.WriteEndElement();
        }

        public string GetUniqueID(string id)
        {
            string name = id.Replace(" ", string.Empty);

            if (AttributeIdList.ContainsKey(name))
            {
                AttributeIdList[name]++;
                return $"{name}_{AttributeIdList[name]}";//
            }
            else
            {
                AttributeIdList.Add(name, 0);
                return name;
            }
        }

        public void Dispose()
        {
            Writer?.WriteEndElement();
            Writer?.WriteEndDocument();
            Writer?.Close();
        }

        private static void WriteFloats(object[] values)
        {

        }

        private class ValueContainer<T> : IEquatable<ValueContainer<T>> where T : struct
        {
            public T[] Values { get; }
            private readonly int hashCode;

            public ValueContainer(T[] values)
            {
                Values = values;
                hashCode = ((System.Collections.IStructuralEquatable)Values).GetHashCode(EqualityComparer<T>.Default);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as ValueContainer<T>);
            }

            public bool Equals(ValueContainer<T> other)
            {
                // Compare precalculated hash first for performance reasons.
                // The entire sequence needs to be compared to resolve collisions.
                return other != null && hashCode == other.hashCode && Enumerable.SequenceEqual(Values, other.Values);
            }

            public override int GetHashCode()
            {
                return hashCode;
            }
        }
    }

    public class Material
    {
        public string Name { get; set; }

        public List<TextureMap> Textures = new List<TextureMap>();
    }

    public class Geometry
    {
        public string Name { get; set; }

        public Material Material { get; set; }

        public List<float> Position = new List<float>();
        public List<float> Normal = new List<float>();
        public List<float> UV0 = new List<float>();
        public List<float> UV1 = new List<float>();
        public List<float> UV2 = new List<float>();
        public List<float> UV3 = new List<float>();
        public List<float> Color = new List<float>();
        public List<int[]> BoneIndices = new List<int[]>();
        public List<float[]> BoneWeights = new List<float[]>();

        public bool HasNormals = false;
        public bool HasUV0 = false;
        public bool HasUV1 = false;
        public bool HasUV2 = false;
        public bool HasUV3 = false;
        public bool HasColors = false;
        public bool HasBoneIndices = false;
        public bool HasWeights = false;
    }

    public enum PhongTextureType
    {
        diffuse,
        specular,
        emission,
        bump,
    }

    public enum TEXCOORD
    {
        CHANNEL0,
        CHANNEL1,
        CHANNEL2,
        CHANNEL3,
    }

    public enum SemanticType
    {
        None,
        POSITION,
        VERTEX,
        NORMAL,
        TEXCOORD,
        COLOR,
        WEIGHT,
        JOINT,
        INV_BIND_MATRIX,
        TEXTANGENT,
        TEXBINORMAL,
        INPUT,
        OUTPUT,
        INTERPOLATION,
    }

    public enum ProgramPreset
    {
        NONE,
        MAX,
        MAYA,
        BLENDER,
    }

    public enum SamplerWrapMode
    {
        NONE,
        WRAP,
        MIRROR,
        CLAMP,
        BORDER,
    }

    public class TextureMap
    {
        public TEXCOORD TextureChannel { get; set; }
        public string Name { get; set; }
        public PhongTextureType Type { get; set; }

        public SamplerWrapMode WrapModeS = SamplerWrapMode.WRAP;
        public SamplerWrapMode WrapModeT = SamplerWrapMode.WRAP;

        public TextureMap()
        {
            TextureChannel = TEXCOORD.CHANNEL0;
            Type = PhongTextureType.diffuse;
            Name = "";
        }
    }

    public class TriangleList
    {
        public string Material = "";
        public List<uint> Indices = new List<uint>();
    }

    public class Joint
    {
        public string Name;
        public int ParentIndex = -1;
        public float[] Transform;
        public float[] BindPose;

        public float[] Rotate;
        public float[] Translate;
        public float[] Scale;
    }

    public class Animation
    {
        public string ID;
        public string Name;

        public Sampler Sampler = new Sampler();
        public Channel Channel = new Channel();
    }

    public class Sampler
    {
        public string ID;

        public Dictionary<string, SemanticType> Inputs = new Dictionary<string, SemanticType>();
    }

    public class Channel
    {
        public string Source;
        public string Target;
    }
}
