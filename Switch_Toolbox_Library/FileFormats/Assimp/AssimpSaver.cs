using System;
using System.Collections.Generic;
using System.IO;
using Assimp;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Animations;
using Switch_Toolbox.Library.Rendering;
using System.Windows.Forms;

namespace Switch_Toolbox.Library
{
    public class AssimpSaver
    {
        public List<string> BoneNames = new List<string>();

        public void SaveFromModel(STGenericModel model, string FileName, List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            Scene scene = new Scene();
            scene.RootNode = new Node("RootNode");

            SaveSkeleton(skeleton, scene.RootNode);
            SaveMaterials(scene, model, FileName, Textures);
            SaveMeshes(scene, model, skeleton, FileName, NodeArray);

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

                if (v.ExportFile(scene, FileName, formatID, PostProcessSteps.FlipUVs))
                    MessageBox.Show($"Exported {FileName} Successfuly!");
                else
                    MessageBox.Show($"Failed to export {FileName}!");

                Scene newScene = v.ImportFile(FileName);
            }
        }

        private void SaveMeshes(Scene scene, STGenericModel model, STSkeleton skeleton,  string FileName, List<int> NodeArray)
        {
            int MeshIndex = 0;
            foreach (var obj in model.Nodes[0].Nodes)
            {
                var genericObj = (STGenericObject)obj;

                Mesh mesh = new Mesh(genericObj.Text, PrimitiveType.Triangle);
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
                                bone.OffsetMatrix = transform;

                                mesh.Bones.Add(bone);
                                BoneNames.Add(bone.Name);

                                boneInd = mesh.Bones.IndexOf(bone); //Set the index of the bone for the vertex weight
                            }

                            //Check if the max amount of weights is higher than the current bone id
                            if (v.boneWeights.Count > j && v.boneWeights[j] > 0)
                            {
                                 if (v.boneWeights[j] <= 1)
                                    mesh.Bones[boneInd].VertexWeights.Add(new VertexWeight(vertexID, v.boneWeights[j]));
                                else
                                    mesh.Bones[boneInd].VertexWeights.Add(new VertexWeight(vertexID, 1));
                            }
                            else if (v.boneWeights.Count == 0 || v.boneWeights[j] > 0)
                                mesh.Bones[boneInd].VertexWeights.Add(new VertexWeight(vertexID, 1));
                        }
                    }
                    vertexID++;
                }
                List<int> faces = genericObj.lodMeshes[genericObj.DisplayLODIndex].faces;
                for (int f = 0; f < faces.Count; f++)
                    mesh.Faces.Add(new Face(new int[] { faces[f++], faces[f++], faces[f] }));

                mesh.TextureCoordinateChannels.SetValue(textureCoords0, 0);

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

        private void SaveMaterials(Scene scene, STGenericModel model, string FileName, List<STGenericTexture> Textures)
        {
            string TextureExtension = ".png";
            string TexturePath = System.IO.Path.GetDirectoryName(FileName);

            foreach (var tex in Textures)
            {
                string path = System.IO.Path.Combine(TexturePath, tex.Text + TextureExtension);

                var bitmap = tex.GetBitmap();
                bitmap.Save(path);
                bitmap.Dispose();
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

                    TextureSlot slot = new TextureSlot();
                    slot.FilePath = path;
                    slot.UVIndex = 0;
                    slot.Flags = 0;
                    slot.TextureIndex = 0;
                    slot.BlendFactor = 1.0f;
                    slot.Mapping = TextureMapping.FromUV;
                    slot.Operation = TextureOperation.Add;

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
                    
                    material.AddMaterialTexture(ref slot);
                }
                scene.Materials.Add(material);
            }

        }

        public void SaveFromObject(List<Vertex> vertices, List<int> faces, string MeshName, string FileName)
        {
            Scene scene = new Scene();
            scene.RootNode = new Node("Root");

            Mesh mesh = new Mesh(MeshName, PrimitiveType.Triangle);

            List<Vector3D> textureCoords0 = new List<Vector3D>();
            List<Vector3D> textureCoords1 = new List<Vector3D>();
            List<Vector3D> textureCoords2 = new List<Vector3D>();
            List<Color4D> vertexColors = new List<Color4D>();

            foreach (Vertex v in vertices)
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
            }
            for (int f = 0; f < faces.Count; f++)
            {
                mesh.Faces.Add(new Face(new int[] { faces[f++], faces[f++], faces[f] }));
            }
            mesh.MaterialIndex = 0;

            mesh.TextureCoordinateChannels.SetValue(textureCoords0, 0);
            scene.Meshes.Add(mesh);

            Material material = new Material();
            material.Name = "NewMaterial";
            scene.Materials.Add(material);

            using (var v = new AssimpContext())
            {
                v.ExportFile(scene, FileName, "obj");
            }
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
