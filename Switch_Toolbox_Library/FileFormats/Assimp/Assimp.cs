using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Assimp;
using OpenTK;
using Switch_Toolbox.Library.Rendering;
using System.Windows.Forms;
using Switch_Toolbox.Library.Animations;

namespace Switch_Toolbox.Library
{
    public class AssimpData
    {
        public Scene scene;

        public List<STGenericObject> objects = new List<STGenericObject>();
        public List<STGenericMaterial> materials = new List<STGenericMaterial>();
        public STSkeleton skeleton;
        public List<string> BoneNames = new List<string>();

        public AssimpContext Importer = new AssimpContext();

        public string[] GetSupportedImportFormats()
        {
            return Importer.GetSupportedImportFormats();
        }

        public AssimpData()
        {
        }
        public void LoadFile(string FileName)
        {
            try
            {
                AssimpContext Importer = new AssimpContext();

                STConsole.WriteLine($"Loading File {FileName}", Color.FromArgb(0, 255, 0));

                var Flags = PostProcessSteps.Triangulate;
                Flags |= PostProcessSteps.JoinIdenticalVertices;
                Flags |= PostProcessSteps.FlipUVs;
                Flags |= PostProcessSteps.LimitBoneWeights;
                Flags |= PostProcessSteps.CalculateTangentSpace;
                Flags |= PostProcessSteps.GenerateNormals;

                scene = Importer.ImportFile(FileName, Flags);

                LoadScene();
            }
            catch (Exception e)
            {
                if (e.ToString().Contains("Error loading unmanaged library from path"))
                {
                    MessageBox.Show($"Failed to load assimp! Make sure you have Assimp32.dll next to the program!");
                }
                Console.WriteLine(e);
            }
        }
        public void processNode()
        {
            Matrix4x4 identity = Matrix4x4.Identity;
            if (scene.RootNode != null)
            {
                BuildNode(scene.RootNode, ref identity);
            }
            else
            {
                int Index = 0;
                foreach (Mesh msh in scene.Meshes)
                {
                    objects.Add(CreateGenericObject(msh, Index, Matrix4.Identity));
                    Index++;
                }
            }
        }
        private void BuildNode(Node parent, ref Matrix4x4 rootTransform)
        {
            Matrix4x4 trafo = parent.Transform;
            Matrix4x4 world = trafo * rootTransform;
            Matrix4 worldTK = AssimpHelper.TKMatrix(world);

            foreach (int index in parent.MeshIndices)
                objects.Add(CreateGenericObject(scene.Meshes[index], index, worldTK));

            foreach (Node child in parent.Children)
                BuildNode(child, ref world);
        }
        public void LoadScene()
        {
            objects.Clear();
            materials.Clear();
            skeleton = new STSkeleton();

            processNode();

            var idenity = Matrix4x4.Identity;
            BuildSkeletonNodes(scene.RootNode, BoneNames, skeleton, ref idenity);
            skeleton.update();
            skeleton.reset();

            if (scene.HasMaterials)
            {
                foreach (Material mat in scene.Materials)
                {
                    materials.Add(CreateGenericMaterial(mat));
                }
            }
            foreach (Assimp.Animation animation in scene.Animations)
            {

            }
            foreach (var tex in scene.Textures)
            {
            }
        }
        public Animations.Animation CreateGenericAnimation(Assimp.Animation animation)
        {
            Animations.Animation STanim = new Animations.Animation();
            STanim.Text = animation.Name;
            STanim.FrameCount = (int)animation.DurationInTicks;

            //Load node animations
            if (animation.HasNodeAnimations)
            {
                var _channels = new NodeAnimationChannel[animation.NodeAnimationChannelCount];
                for (int i = 0; i < _channels.Length; i++)
                {
                    _channels[i] = new NodeAnimationChannel();
                }
            }

            //Load mesh animations
            if (animation.HasMeshAnimations)
            {
                var _meshChannels = new MeshAnimationChannel[animation.MeshAnimationChannelCount];
                for (int i = 0; i < _meshChannels.Length; i++)
                {
                    _meshChannels[i] = new MeshAnimationChannel();
                }
            }

            return STanim;
        }
        public STGenericMaterial CreateGenericMaterial(Material material)
        {
            STGenericMaterial mat = new STGenericMaterial();
            mat.Text = material.Name;

            TextureSlot tex;
            if (material.GetMaterialTexture(TextureType.Diffuse, 0, out tex))
                mat.TextureMaps.Add(CreateTextureSlot(tex, TextureType.Diffuse));
            if (material.GetMaterialTexture(TextureType.Normals, 1, out tex))
                mat.TextureMaps.Add(CreateTextureSlot(tex, TextureType.Normals));
            if (material.GetMaterialTexture(TextureType.Specular, 1, out tex))
                mat.TextureMaps.Add(CreateTextureSlot(tex, TextureType.Specular));
            return mat;
        }
        private STGenericMatTexture CreateTextureSlot(TextureSlot tex, TextureType type)
        {
            var matTex = new STGenericMatTexture();

            switch (type)
            {
                case TextureType.Diffuse:
                    matTex.Type = STGenericMatTexture.TextureType.Diffuse;
                    break;
                case TextureType.Normals:
                    matTex.Type = STGenericMatTexture.TextureType.Normal;
                    break;
                case TextureType.Lightmap:
                    matTex.Type = STGenericMatTexture.TextureType.Light;
                    break;
                case TextureType.Emissive:
                    matTex.Type = STGenericMatTexture.TextureType.Emission;
                    break;
                case TextureType.Specular:
                    matTex.Type = STGenericMatTexture.TextureType.Specular;
                    break;
                case TextureType.Shininess:
                    matTex.Type = STGenericMatTexture.TextureType.Metalness;
                    break;
                case TextureType.Opacity:
                    matTex.Type = STGenericMatTexture.TextureType.Transparency;
                    break;
                case TextureType.Displacement:
                    break;
                default:
                    matTex.Type = STGenericMatTexture.TextureType.Unknown;
                    break;
            }

            matTex.Name = System.IO.Path.GetFileNameWithoutExtension(tex.FilePath);
            matTex.wrapModeS = SetWrapMode(tex.WrapModeU);
            matTex.wrapModeT = SetWrapMode(tex.WrapModeV);

            STConsole.WriteLine($"Getting assimp texture slot {matTex.Name} Type {matTex.Type}");

            return matTex;
        }
        private int SetWrapMode(TextureWrapMode wrap)
        {
            switch (wrap)
            {
                case TextureWrapMode.Wrap:
                    return 0;
                case TextureWrapMode.Mirror:
                    return 1;
                case TextureWrapMode.Clamp:
                    return 2;
                case TextureWrapMode.Decal:
                    return 0;
                default:
                    return 0;
            }
        }
        private void BuildSkeleton(Node root)
        {

        }
        private void BuildSkeletonNodes(Node node, List<string> boneNames, STSkeleton skeleton, ref Matrix4x4 rootTransform)
        {
            Matrix4x4 trafo = node.Transform;
            Matrix4x4 world = trafo * rootTransform;
            Matrix4 worldTK = AssimpHelper.TKMatrix(world);



            bool IsBone = boneNames.Contains(node.Name) && !boneNames.Contains(node.Parent.Name) ||
                          node.Name == "Skl_Root" || node.Name == "nw4f_root";

            Console.WriteLine("node list " + node.Name + " " + IsBone);

            short SmoothIndex = 0;
            short RigidIndex = -1;

            //Loop through all the bones. If the parent is not in the bone list, then it's Parent is the root
            if (IsBone)
            {
                var idenity = Matrix4x4.Identity;
                var Root = node.Parent;
                CreateByNode(node, skeleton, SmoothIndex, RigidIndex, true, ref idenity);
            }
            else
            {
                foreach (Node child in node.Children)
                    BuildSkeletonNodes(child,boneNames, skeleton, ref world);
            }
        }

        public const int BoneRotation = 0;

        private List<Node> tempBoneNodes = new List<Node>();
        private void CreateByNode(Node node, STSkeleton skeleton, short SmoothIndex, short RigidIndex, bool IsRoot, ref Assimp.Matrix4x4 rootTransform)
        {
            Matrix4x4 trafo = node.Transform;
            Matrix4x4 world = trafo * rootTransform;
            var transformMat = AssimpHelper.TKMatrix(world);

            int matchedBoneIndex = skeleton.bones.FindIndex(item => item.Name == node.Name);

            if (matchedBoneIndex < 0)
            {
                tempBoneNodes.Add(node);

                STBone bone = new STBone();
                bone.skeletonParent = skeleton;
                skeleton.bones.Add(bone);

                bone.Text = node.Name;
                bone.SmoothMatrixIndex = (short)skeleton.bones.IndexOf(bone);
                bone.RigidMatrixIndex = -1; //Todo calculate these

                STConsole.WriteLine($"-".Repeat(30));
                STConsole.WriteLine($"Processing Bone {bone.Text}");
                STConsole.WriteLine($"SmoothMatrixIndex {bone.SmoothMatrixIndex}");
                STConsole.WriteLine($"RigidMatrixIndex {bone.RigidMatrixIndex}");
                STConsole.WriteLine($"Transform Matrix {transformMat}");
                STConsole.WriteLine($"-".Repeat(30));

                if (IsRoot)
                {
                    bone.parentIndex = -1;
                    transformMat = AssimpHelper.TKMatrix(world * Matrix4x4.FromRotationX(MathHelper.DegreesToRadians(BoneRotation)));
                }
                else
                {
                    if (tempBoneNodes.Contains(node.Parent))
                        bone.parentIndex = tempBoneNodes.IndexOf(node.Parent);
                }


                var scale = transformMat.ExtractScale();
                var rotation = transformMat.ExtractRotation();
                var position = transformMat.ExtractTranslation();

                var rotEular = AssimpHelper.ToEular(rotation);

                bone.position = new float[] { position.X, position.Y, position.Z };
                bone.scale = new float[] { scale.X, scale.Y, scale.Z };
                bone.rotation = new float[] { rotEular.X, rotEular.Y, rotEular.Z, 0 };
            }
            else
            {
                STConsole.WriteLine($"Duplicate node name found for bone {node.Name}!", Color.Red);
            }

            foreach (Node child in node.Children)
                CreateByNode(child, skeleton, SmoothIndex, RigidIndex, false, ref rootTransform);
        }
        public STGenericObject CreateGenericObject(Mesh msh, int Index, Matrix4 transform)
        {
            STGenericObject obj = new STGenericObject();

            if (msh.MaterialIndex != -1)
                obj.MaterialIndex = msh.MaterialIndex;
            else
                scene.Materials.Add(new Material() { Name = msh.Name });

            if (scene.Materials[msh.MaterialIndex].Name == "")
                scene.Materials[msh.MaterialIndex].Name = msh.Name;

            obj.HasPos = msh.HasVertices;
            obj.HasNrm = msh.HasNormals;
            obj.HasUv0 = msh.HasTextureCoords(0);
            obj.HasUv1 = msh.HasTextureCoords(1);
            obj.HasUv2 = msh.HasTextureCoords(2);
            obj.HasIndices = msh.HasBones;
            if (msh.HasBones)
                obj.HasWeights = msh.Bones[0].HasVertexWeights;

            obj.HasTans = msh.HasTangentBasis;
            obj.HasBitans = msh.HasTangentBasis;
            obj.HasVertColors = msh.HasVertexColors(0);
            obj.ObjectName = msh.Name;
            obj.boneList = GetBoneList(msh);
            obj.VertexSkinCount = (byte)GetVertexSkinCount(msh);

            STGenericObject.LOD_Mesh lod = new STGenericObject.LOD_Mesh();
            lod.faces = GetFaces(msh);
            lod.IndexFormat = STIndexFormat.UInt16;
            lod.PrimitiveType = STPolygonType.Triangle;
            lod.GenerateSubMesh();
            obj.lodMeshes.Add(lod);
            obj.vertices = GetVertices(msh, transform, obj);
            obj.VertexBufferIndex = Index;

            return obj;
        }

        public List<int> GetFaces(Mesh msh)
        {
            List<int> faces = new List<int>();

            if (msh.HasFaces)
            {
                foreach (Face f in msh.Faces)
                {
                    if (f.HasIndices)
                    {
                        foreach (int indx in f.Indices)
                            faces.Add(indx);
                    }
                }
            }

            return faces;
        }
        public List<string> GetBoneList(Mesh msh)
        {
            List<string> bones = new List<string>();
            foreach (Bone b in msh.Bones)
            {
                if (!bones.Contains(b.Name))
                    bones.Add(b.Name);
            }
            return bones;
        }
        public int GetVertexSkinCount(Mesh msh)
        {

            List<int> indciesTotal = new List<int>();

            var blendIndexes = new List<List<int>>();
            var blendWeights = new List<List<float>>();

            int i;
            for (i = 0; i < msh.VertexCount; i++)
            {
                blendIndexes.Add(new List<int>());
                blendWeights.Add(new List<float>());
            }

            foreach (var bone in msh.Bones)
            {
                var bi = msh.Bones.IndexOf(bone);
                foreach (var vw in bone.VertexWeights)
                {
                    blendIndexes[vw.VertexID].Add(bi);
                    blendWeights[vw.VertexID].Add(vw.Weight);
                }
            }

            foreach (Bone b in msh.Bones)
                Console.WriteLine(b.VertexWeights.Count);

            if (msh.HasBones)
                return msh.Bones.Max(b => b.VertexWeightCount);

            return 0;
        }
        public List<Vertex> GetVertices(Mesh msh, Matrix4 transform, STGenericObject STobj)
        {

            List<Vertex> vertices = new List<Vertex>();
            for (int v = 0; v < msh.VertexCount; v++)
            {
                Vertex vert = new Vertex();

                if (msh.HasVertices)
                    vert.pos = Vector3.TransformPosition(AssimpHelper.FromVector(msh.Vertices[v]), transform);
                if (msh.HasNormals)
                    vert.nrm = Vector3.TransformNormal(AssimpHelper.FromVector(msh.Normals[v]), transform);
                if (msh.HasTextureCoords(0))
                    vert.uv0 = new Vector2(msh.TextureCoordinateChannels[0][v].X, msh.TextureCoordinateChannels[0][v].Y);
                if (msh.HasTextureCoords(1))
                    vert.uv1 = new Vector2(msh.TextureCoordinateChannels[1][v].X, msh.TextureCoordinateChannels[1][v].Y);
                if (msh.HasTextureCoords(2))
                    vert.uv2 = new Vector2(msh.TextureCoordinateChannels[2][v].X, msh.TextureCoordinateChannels[2][v].Y);
                if (msh.HasTangentBasis)
                    vert.tan = new Vector4(msh.Tangents[v].X, msh.Tangents[v].Y, msh.Tangents[v].Z, 1);
                if (msh.HasVertexColors(0))
                    vert.col = new Vector4(msh.VertexColorChannels[0][v].R, msh.VertexColorChannels[0][v].G, msh.VertexColorChannels[0][v].B, msh.VertexColorChannels[0][v].A);
                if (msh.HasTangentBasis)
                    vert.bitan = new Vector4(msh.BiTangents[v].X, msh.BiTangents[v].Y, msh.BiTangents[v].Z, 1);
                vertices.Add(vert);
            }
            if (msh.HasBones)
            {
                STConsole.WriteLine(msh.Name + " HasBones " + msh.HasBones);
                STConsole.WriteLine(msh.Name + " BoneCount " + msh.BoneCount);

                for (int i = 0; i < msh.BoneCount; i++)
                {
                    Bone bn = msh.Bones[i];
                    if (bn.HasVertexWeights)
                    {
                        foreach (VertexWeight w in bn.VertexWeights)
                        {
                            vertices[w.VertexID].boneWeights.Add(w.Weight);
                            vertices[w.VertexID].boneNames.Add(bn.Name);
                            BoneNames.Add(bn.Name);
                        }
                    }
                }
            }


            return vertices;
        }

        private Matrix4 FromAssimpMatrix(Matrix4x4 mat)
        {
            Vector3D scaling;
            Vector3D tranlation;
            Assimp.Quaternion rot;
            mat.Decompose(out scaling, out rot, out tranlation);

            Console.WriteLine($"rotQ " + rot);

            Matrix4 positionMat = Matrix4.CreateTranslation(AssimpHelper.FromVector(tranlation));
            Matrix4 rotQ = Matrix4.CreateFromQuaternion(AssimpHelper.TKQuaternion(rot));
            Matrix4 scaleMat = Matrix4.CreateScale(AssimpHelper.FromVector(scaling));
            Matrix4 matrixFinal = scaleMat * rotQ * positionMat;

            return matrixFinal;
        }
    }
}
