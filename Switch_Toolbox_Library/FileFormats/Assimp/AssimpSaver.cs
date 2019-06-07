using System;
using System.Collections.Generic;
using System.IO;
using Assimp;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Animations;
using Switch_Toolbox.Library.Rendering;
using System.Windows.Forms;
using System.Linq;

namespace Switch_Toolbox.Library
{
    public class AssimpSaver
    {
        private List<string> ExtractedTextures = new List<string>();

        public List<string> BoneNames = new List<string>();

        STProgressBar progressBar;

        public void SaveFromModel(STGenericModel model, string FileName, List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            ExtractedTextures.Clear();

            Scene scene = new Scene();
            scene.RootNode = new Node("RootNode");

            progressBar = new STProgressBar();
            progressBar.Task = "Exorting Skeleton...";
            progressBar.Value = 0;
            progressBar.StartPosition = FormStartPosition.CenterScreen;
            progressBar.Show();
            progressBar.Refresh();

            SaveSkeleton(skeleton, scene.RootNode);
            SaveMaterials(scene, model, FileName, Textures);

            progressBar.Task = "Exorting Meshes...";
            progressBar.Value = 50;

            SaveMeshes(scene, model, skeleton, FileName, NodeArray);

            progressBar.Task = "Saving File...";
            progressBar.Value = 80;

            SaveScene(FileName, scene, model);

            progressBar.Value = 100;
            progressBar.Close();
            progressBar.Dispose();
        }

        private void SaveScene(string FileName, Scene scene, STGenericModel model)
        {
            using (var v = new AssimpContext())
            {
                string ext = System.IO.Path.GetExtension(FileName);

                string formatID = "collada";
                if (ext == ".obj")
                    formatID = "obj";
                if (ext == ".3ds")
                    formatID = "3ds";
                if (ext == ".dae")
                    formatID = "collada";
                if (ext == ".ply")
                    formatID = "ply";

                bool ExportSuccessScene = v.ExportFile(scene, FileName, formatID, PostProcessSteps.FlipUVs);
                if (ExportSuccessScene)
                {
                    WriteExtraSkinningInfo(FileName, scene, model);
                    MessageBox.Show($"Exported {FileName} Successfuly!");
                }
                else
                    MessageBox.Show($"Failed to export {FileName}!");
            }

        }

        private void SaveMeshes(Scene scene, STGenericModel model, STSkeleton skeleton,  string FileName, List<int> NodeArray)
        {
            int MeshIndex = 0;
            foreach (var obj in model.Nodes[0].Nodes)
            {
                var mesh = SaveMesh((STGenericObject)obj, MeshIndex, skeleton, NodeArray);
                scene.Meshes.Add(mesh);
                MeshIndex++;
            }
            Node geomNode = new Node(Path.GetFileNameWithoutExtension(FileName), scene.RootNode);

            for (int ob = 0; ob < scene.MeshCount; ob++)
            {
                geomNode.MeshIndices.Add(ob);
            }
            scene.RootNode.Children.Add(geomNode);
        }

        private Mesh SaveMesh(STGenericObject genericObj, int index, STSkeleton skeleton, List<int> NodeArray)
        {
            //Assimp is weird so use mesh_# for the name. We'll change it back after save
            Mesh mesh = new Mesh($"mesh_{ index }", PrimitiveType.Triangle);
            mesh.MaterialIndex = genericObj.MaterialIndex;

            List<Vector3D> textureCoords0 = new List<Vector3D>();
            List<Vector3D> textureCoords1 = new List<Vector3D>();
            List<Vector3D> textureCoords2 = new List<Vector3D>();
            List<Color4D> vertexColors = new List<Color4D>();

            int vertexID = 0;
            foreach (Vertex v in genericObj.vertices)
            {
                mesh.Vertices.Add(new Vector3D(v.pos.X, v.pos.Y, v.pos.Z));
                mesh.Normals.Add(new Vector3D(v.nrm.X, v.nrm.Y, v.nrm.Z));
                textureCoords0.Add(new Vector3D(v.uv0.X, v.uv0.Y, 0));
                textureCoords1.Add(new Vector3D(v.uv1.X, v.uv1.Y, 0));
                textureCoords2.Add(new Vector3D(v.uv2.X, v.uv2.Y, 0));
                vertexColors.Add(new Color4D(v.col.X, v.col.Y, v.col.Z, v.col.W));
                mesh.TextureCoordinateChannels[0] = textureCoords0;
                mesh.TextureCoordinateChannels[1] = textureCoords1;
                mesh.TextureCoordinateChannels[2] = textureCoords2;
                mesh.VertexColorChannels[0] = vertexColors;

                if (skeleton != null)
                {
                    for (int j = 0; j < v.boneIds.Count; j++)
                    {
                        if (j < genericObj.VertexSkinCount)
                        {
                            //Get the bone via the node array and bone index from the vertex
                            STBone STbone = skeleton.bones[NodeArray[v.boneIds[j]]];

                            //Find the index of a bone. If it doesn't exist then we add it
                            int boneInd = mesh.Bones.FindIndex(x => x.Name == STbone.Text);

                            if (boneInd == -1)
                            {
                                var matrices = Switch_Toolbox.Library.IO.MatrixExenstion.CalculateInverseMatrix(STbone);

                                //Set the inverse matrix
                                Matrix4x4 transform = matrices.inverse.FromNumerics();

                                //Create a new assimp bone
                                Bone bone = new Bone();
                                bone.Name = STbone.Text;
                                bone.OffsetMatrix = STbone.invert.ToMatrix4x4();
                                mesh.Bones.Add(bone);
                                BoneNames.Add(bone.Name);

                                boneInd = mesh.Bones.IndexOf(bone); //Set the index of the bone for the vertex weight
                            }

                            int MinWeightAmount = 0;

                            //Check if the max amount of weights is higher than the current bone id
                            if (v.boneWeights.Count > j && v.boneWeights[j] > MinWeightAmount)
                            {
                                if (v.boneWeights[j] <= 1)
                                    mesh.Bones[boneInd].VertexWeights.Add(new VertexWeight(vertexID, v.boneWeights[j]));
                                else
                                    mesh.Bones[boneInd].VertexWeights.Add(new VertexWeight(vertexID, 1));
                            }
                            else if (v.boneWeights.Count == 0 || v.boneWeights[j] > MinWeightAmount)
                                mesh.Bones[boneInd].VertexWeights.Add(new VertexWeight(vertexID, 1));
                        }
                    }
                }

               
                vertexID++;
            }
            List<int> faces = genericObj.lodMeshes[genericObj.DisplayLODIndex].faces;
            for (int f = 0; f < faces.Count; f++)
                mesh.Faces.Add(new Face(new int[] { faces[f++], faces[f++], faces[f] }));

            mesh.TextureCoordinateChannels.SetValue(textureCoords0, 0);

            return mesh;
        }

        //Extra skin data based on https://github.com/Sage-of-Mirrors/SuperBMD/blob/ce1061e9b5f57de112f1d12f6459b938594664a0/SuperBMDLib/source/Model.cs#L193
        //Todo this doesn't quite work yet
        //Need to adjust all mesh name IDs so they are correct
        private void WriteExtraSkinningInfo(string FileName, Scene outScene, STGenericModel Model)
        {
            StreamWriter test = new StreamWriter(FileName + ".tmp");
            StreamReader dae = File.OpenText(FileName);

            int geomIndex = 0;
            while (!dae.EndOfStream)
            {
                string line = dae.ReadLine();

                if (line == "  <library_visual_scenes>")
                {
                    AddControllerLibrary(outScene, test);
                    test.WriteLine(line);
                    test.Flush();
                }
                else if (line.Contains("<node"))
                {
                    test.WriteLine(line);
                    test.Flush();

                    /* string[] testLn = line.Split('\"');
                      string name = testLn[3];

                      string jointLine = line.Replace(">", $" sid=\"{ name }\" type=\"JOINT\">");
                      test.WriteLine(jointLine);
                      test.Flush();*/
                }
                else if (line.Contains("</visual_scene>"))
                {
                    foreach (Mesh mesh in outScene.Meshes)
                    {
                        test.WriteLine($"      <node id=\"{ mesh.Name }\" name=\"{ mesh.Name }\" type=\"NODE\">");

                        test.WriteLine($"       <instance_controller url=\"#{ mesh.Name }-skin\">");
                        test.WriteLine("        <skeleton>#skeleton_root</skeleton>");
                        test.WriteLine("        <bind_material>");
                        test.WriteLine("         <technique_common>");
                        test.WriteLine($"          <instance_material symbol=\"theresonlyone\" target=\"#m{ mesh.MaterialIndex }mat\" />");
                        test.WriteLine("         </technique_common>");
                        test.WriteLine("        </bind_material>");
                        test.WriteLine("       </instance_controller>");

                        test.WriteLine("      </node>");
                        test.Flush();
                    }

                    test.WriteLine(line);
                    test.Flush();
                }
                else if (line.Contains("<geometry"))
                {
                    string RealMeshName = Model.Nodes[0].Nodes[geomIndex].Text;
                    test.WriteLine($"    <geometry id=\"meshId{ geomIndex }\" name=\"{ RealMeshName }\" > ");
                    test.Flush();

                    geomIndex++;
                }
                else if (line.Contains("<matrix"))
                {
                    string matLine = line.Replace("<matrix>", "<matrix sid=\"matrix\">");
                    test.WriteLine(matLine);
                    test.Flush();
                }
                else
                {
                   test.WriteLine(line);
                   test.Flush();
                }
            }

            test.Close();
            dae.Close();

            File.Copy(FileName + ".tmp", FileName, true);
            File.Delete(FileName + ".tmp");
        }

        private void AddControllerLibrary(Scene scene, StreamWriter writer)
        {
            writer.WriteLine("  <library_controllers>");

            for (int i = 0; i < scene.MeshCount; i++)
            {
                Mesh curMesh = scene.Meshes[i];
                curMesh.Name = curMesh.Name.Replace('_', '-');

                writer.WriteLine($"   <controller id=\"{ curMesh.Name }-skin\" name=\"{ curMesh.Name }Skin\">");

                writer.WriteLine($"    <skin source=\"#meshId{ i }\">");

                WriteBindShapeMatrixToStream(writer);
                WriteJointNameArrayToStream(curMesh, writer);
                WriteInverseBindMatricesToStream(curMesh, writer);
                WriteSkinWeightsToStream(curMesh, writer);

                writer.WriteLine("     <joints>");

                writer.WriteLine($"      <input semantic=\"JOINT\" source=\"#{ curMesh.Name }-skin-joints-array\"></input>");
                writer.WriteLine($"      <input semantic=\"INV_BIND_MATRIX\" source=\"#{ curMesh.Name }-skin-bind_poses-array\"></input>");

                writer.WriteLine("     </joints>");
                writer.Flush();

                WriteVertexWeightsToStream(curMesh, writer);

                writer.WriteLine("    </skin>");

                writer.WriteLine("   </controller>");
                writer.Flush();
            }

            writer.WriteLine("  </library_controllers>");
            writer.Flush();
        }

        private void WriteJointNameArrayToStream(Mesh mesh, StreamWriter writer)
        {
            writer.WriteLine($"      <source id =\"{ mesh.Name }-skin-joints-array\">");
            writer.WriteLine($"      <Name_array id=\"{ mesh.Name }-skin-joints-array\" count=\"{ mesh.Bones.Count }\">");

            writer.Write("       ");
            foreach (Bone bone in mesh.Bones)
            {
                writer.Write($"{ bone.Name }");
                if (bone != mesh.Bones.Last())
                    writer.Write(' ');
                else
                    writer.Write('\n');

                writer.Flush();
            }

            writer.WriteLine("      </Name_array>");
            writer.Flush();

            writer.WriteLine("      <technique_common>");
            writer.WriteLine($"       <accessor source=\"#{ mesh.Name }-skin-joints-array\" count=\"{ mesh.Bones.Count }\" stride=\"1\">");
            writer.WriteLine("         <param name=\"JOINT\" type=\"Name\"></param>");
            writer.WriteLine("       </accessor>");
            writer.WriteLine("      </technique_common>");
            writer.WriteLine("      </source>");
            writer.Flush();
        }

        private void WriteInverseBindMatricesToStream(Mesh mesh, StreamWriter writer)
        {
            writer.WriteLine($"      <source id =\"{ mesh.Name }-skin-bind_poses-array\">");
            writer.WriteLine($"      <float_array id=\"{ mesh.Name }-skin-bind_poses-array\" count=\"{ mesh.Bones.Count * 16 }\">");

            foreach (Bone bone in mesh.Bones)
            {
                Matrix4x4 ibm = bone.OffsetMatrix;
                ibm.Transpose();

                writer.WriteLine($"       {ibm.A1.ToString("F")} {ibm.A2.ToString("F")} {ibm.A3.ToString("F")} {ibm.A4.ToString("F")}");
                writer.WriteLine($"       {ibm.B1.ToString("F")} {ibm.B2.ToString("F")} {ibm.B3.ToString("F")} {ibm.B4.ToString("F")}");
                writer.WriteLine($"       {ibm.C1.ToString("F")} {ibm.C2.ToString("F")} {ibm.C3.ToString("F")} {ibm.C4.ToString("F")}");
                writer.WriteLine($"       {ibm.D1.ToString("F")} {ibm.D2.ToString("F")} {ibm.D3.ToString("F")} {ibm.D4.ToString("F")}");

                if (bone != mesh.Bones.Last())
                    writer.WriteLine("");
            }

            writer.WriteLine("      </float_array>");
            writer.Flush();

            writer.WriteLine("      <technique_common>");
            writer.WriteLine($"       <accessor source=\"#{ mesh.Name }-skin-bind_poses-array\" count=\"{ mesh.Bones.Count }\" stride=\"16\">");
            writer.WriteLine("         <param name=\"TRANSFORM\" type=\"float4x4\"></param>");
            writer.WriteLine("       </accessor>");
            writer.WriteLine("      </technique_common>");
            writer.WriteLine("      </source>");
            writer.Flush();
        }

        private void WriteSkinWeightsToStream(Mesh mesh, StreamWriter writer)
        {
            int totalWeightCount = 0;

            foreach (Bone bone in mesh.Bones)
            {
                totalWeightCount += bone.VertexWeightCount;
            }

            writer.WriteLine($"      <source id =\"{ mesh.Name }-skin-weights-array\">");
            writer.WriteLine($"      <float_array id=\"{ mesh.Name }-skin-weights-array\" count=\"{ totalWeightCount }\">");
            writer.Write("       ");

            foreach (Bone bone in mesh.Bones)
            {
                foreach (VertexWeight weight in bone.VertexWeights)
                {
                    writer.Write($"{ weight.Weight } ");
                }

                if (bone == mesh.Bones.Last())
                    writer.WriteLine();
            }

            writer.WriteLine("      </float_array>");
            writer.Flush();

            writer.WriteLine("      <technique_common>");
            writer.WriteLine($"       <accessor source=\"#{ mesh.Name }-skin-weights-array\" count=\"{ totalWeightCount }\" stride=\"1\">");
            writer.WriteLine("         <param name=\"WEIGHT\" type=\"float\"></param>");
            writer.WriteLine("       </accessor>");
            writer.WriteLine("      </technique_common>");
            writer.WriteLine("      </source>");
            writer.Flush();
        }

        private class RiggedWeight
        {
            public List<float> Weights { get; private set; }
            public List<int> BoneIndices { get; private set; }

            public int WeightCount { get; private set; }

            public RiggedWeight()
            {
                Weights = new List<float>();
                BoneIndices = new List<int>();
            }

            public void AddWeight(float weight, int boneIndex)
            {
                Weights.Add(weight);
                BoneIndices.Add(boneIndex);
                WeightCount++;
            }
        }

        private void WriteVertexWeightsToStream(Mesh mesh, StreamWriter writer)
        {
            List<float> weights = new List<float>();
            Dictionary<int, RiggedWeight> vertIDWeights = new Dictionary<int, RiggedWeight>();

            foreach (Bone bone in mesh.Bones)
            {
                foreach (VertexWeight weight in bone.VertexWeights)
                {
                    weights.Add(weight.Weight);

                    if (!vertIDWeights.ContainsKey(weight.VertexID))
                        vertIDWeights.Add(weight.VertexID, new RiggedWeight());

                    vertIDWeights[weight.VertexID].AddWeight(weight.Weight, mesh.Bones.IndexOf(bone));
                }
            }

            writer.WriteLine($"      <vertex_weights count=\"{ vertIDWeights.Count }\">");

            writer.WriteLine($"       <input semantic=\"JOINT\" source=\"#{ mesh.Name }-skin-joints-array\" offset=\"0\"></input>");
            writer.WriteLine($"       <input semantic=\"WEIGHT\" source=\"#{ mesh.Name }-skin-weights-array\" offset=\"1\"></input>");

            writer.WriteLine("       <vcount>");

            writer.Write("        ");
            for (int i = 0; i < vertIDWeights.Count; i++)
                writer.Write($"{ vertIDWeights[i].WeightCount } ");

            writer.WriteLine("\n       </vcount>");

            writer.WriteLine("       <v>");
            writer.Write("        ");

            for (int i = 0; i < vertIDWeights.Count; i++)
            {
                RiggedWeight curWeight = vertIDWeights[i];

                for (int j = 0; j < curWeight.WeightCount; j++)
                {
                    writer.Write($"{ curWeight.BoneIndices[j] } { weights.IndexOf(curWeight.Weights[j]) } ");
                }
            }

            writer.WriteLine("\n       </v>");

            writer.WriteLine($"      </vertex_weights>");
        }

        private void WriteBindShapeMatrixToStream(StreamWriter writer)
        {
            writer.WriteLine("     <bind_shape_matrix>");

            writer.WriteLine("      1 0 0 0");
            writer.WriteLine("      0 1 0 0");
            writer.WriteLine("      0 0 1 0");
            writer.WriteLine("      0 0 0 1");

            writer.WriteLine("     </bind_shape_matrix>");
            writer.Flush();
        }

        private void SaveMaterials(Scene scene, STGenericModel model, string FileName, List<STGenericTexture> Textures)
        {
            string TextureExtension = ".png";
            string TexturePath = System.IO.Path.GetDirectoryName(FileName);

            for (int i = 0; i < Textures.Count; i++)
            {
                string path = System.IO.Path.Combine(TexturePath, Textures[i].Text + TextureExtension);

                if (!ExtractedTextures.Contains(path))
                {
                    ExtractedTextures.Add(path);

                    progressBar.Task = $"Exorting Texture {Textures[i].Text}";
                    progressBar.Value = ((i * 100) / Textures.Count);
                    progressBar.Refresh();

                    var bitmap = Textures[i].GetBitmap();
                    bitmap.Save(path);
                    bitmap.Dispose();

                    GC.Collect();
                }
            }

            foreach (var mat in model.Nodes[1].Nodes)
            {
                var genericMat = (STGenericMaterial)mat;

                Material material = new Material();
                material.Name = genericMat.Text;

                foreach (var tex in genericMat.TextureMaps)
                {
                    int index = Textures.FindIndex(r => r.Text.Equals(tex.Name));

                    string path = System.IO.Path.Combine(TexturePath, tex.Name + TextureExtension);

                    if (!File.Exists(path))
                        continue;

                    if (tex.Type == STGenericMatTexture.TextureType.Diffuse)
                    {
                        TextureSlot slot2 = new TextureSlot(path, Assimp.TextureType.Diffuse, 0,
                    Assimp.TextureMapping.FromUV, 0, 1.0f, Assimp.TextureOperation.Add,
                    0, 0, 0);

                        material.AddMaterialTexture(ref slot2);
                        break;

                        TextureSlot slot = new TextureSlot();
                        slot.FilePath = path;
                        slot.UVIndex = 0;
                        slot.Flags = 0;
                        slot.TextureIndex = 0;
                        slot.BlendFactor = 1.0f;
                        slot.Mapping = TextureMapping.FromUV;
                        slot.Operation = TextureOperation.Add;
                        slot.TextureType = TextureType.Diffuse;


                        if (tex.Type == STGenericMatTexture.TextureType.Diffuse)
                            slot.TextureType = TextureType.Diffuse;
                        else if (tex.Type == STGenericMatTexture.TextureType.Normal)
                            slot.TextureType = TextureType.Normals;
                        else if (tex.Type == STGenericMatTexture.TextureType.Specular)
                            slot.TextureType = TextureType.Specular;
                        else if (tex.Type == STGenericMatTexture.TextureType.Emission)
                            slot.TextureType = TextureType.Emissive;
                        else if (tex.Type == STGenericMatTexture.TextureType.Light)
                        {
                            slot.TextureType = TextureType.Lightmap;
                            slot.UVIndex = 2;
                        }
                        else if (tex.Type == STGenericMatTexture.TextureType.Shadow)
                        {
                            slot.TextureType = TextureType.Ambient;
                            slot.UVIndex = 1;
                        }
                        else
                            slot.TextureType = TextureType.Unknown;

                        if (tex.wrapModeS == 0)
                            slot.WrapModeU = TextureWrapMode.Wrap;
                        else if (tex.wrapModeS == 1)
                            slot.WrapModeU = TextureWrapMode.Mirror;
                        else if (tex.wrapModeS == 2)
                            slot.WrapModeU = TextureWrapMode.Clamp;
                        else
                            slot.WrapModeU = TextureWrapMode.Wrap;

                        if (tex.wrapModeT == 0)
                            slot.WrapModeV = TextureWrapMode.Wrap;
                        else if (tex.wrapModeT == 1)
                            slot.WrapModeV = TextureWrapMode.Mirror;
                        else if (tex.wrapModeT == 2)
                            slot.WrapModeV = TextureWrapMode.Clamp;
                        else
                            slot.WrapModeV = TextureWrapMode.Wrap;
                    }
                }
                scene.Materials.Add(material);
            }

        }

        public void SaveFromObject(STGenericObject genericObject, string FileName)
        {
            Scene scene = new Scene();
            scene.RootNode = new Node("Root");

            var mesh = SaveMesh(genericObject,0, null, null);
            mesh.MaterialIndex = 0;
            scene.Meshes.Add(mesh);

            Material material = new Material();
            material.Name = "NewMaterial";
            scene.Materials.Add(material);

            SaveScene(FileName, scene, new STGenericModel());
        }

        private void SaveSkeleton(STSkeleton skeleton, Node parentNode)
        {
            Node root = new Node("skeleton_root");
            parentNode.Children.Add(root);

            if (skeleton.bones.Count > 0)
            {
                Node boneNode = new Node(skeleton.bones[0].Text);
                boneNode.Transform = AssimpHelper.GetBoneMatrix(skeleton.bones[0]);
                root.Children.Add(boneNode);

                foreach (STBone child in skeleton.bones[0].GetChildren())
                    SaveBones(boneNode, child, skeleton);
            }
        }
        private void SaveBones(Node parentBone, STBone bone, STSkeleton skeleton)
        {
            Node boneNode = new Node(bone.Text);
            parentBone.Children.Add(boneNode);

            boneNode.Transform = AssimpHelper.GetBoneMatrix(bone);

            foreach (STBone child in bone.GetChildren())
                SaveBones(boneNode, child, skeleton);
        }
    }
}
