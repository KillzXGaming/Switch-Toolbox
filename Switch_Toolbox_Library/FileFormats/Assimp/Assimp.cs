using System;
using System.Drawing;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using Assimp;
using OpenTK;
using Toolbox.Library.Rendering;
using System.Windows.Forms;
using Toolbox.Library.Animations;
using Toolbox.Library.Forms;

namespace Toolbox.Library
{
    public class AssimpData
    {
        private float UnitScale = 1;

        public bool UseTransformMatrix = true;
        public bool RotateSkeleton = false;
        public float RotateSkeletonAmount = 90;

        static bool isDae;

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

        public DAEHelper DaeHelper = new DAEHelper();

        public class DAEHelper
        {
            //Gets the real node name based on ID
            //Assimp is weird and sometimes uses ID for name so we need to get it manually
            public Dictionary<string, string> IDMapToName = new Dictionary<string, string>();

            public Dictionary<string, string> VisualSceneNodeTypes = new Dictionary<string, string>();
        }

        public bool LoadFile(string FileName)
        {
            try
            {
                var settings = new Assimp_Settings();
                if (settings.ShowDialog() == DialogResult.OK)
                {
                    UseTransformMatrix = settings.UseNodeTransform;
                    RotateSkeleton = settings.RotateSkeleton;
                    RotateSkeletonAmount = settings.RotateSkeletonAmount;

                    AssimpContext Importer = new AssimpContext();

                    STConsole.WriteLine($"Loading File {FileName}", Color.FromArgb(0, 255, 0));

                    /*  var Flags = PostProcessSteps.Triangulate;
                      Flags |= PostProcessSteps.JoinIdenticalVertices;
                      Flags |= PostProcessSteps.FlipUVs;
                      Flags |= PostProcessSteps.LimitBoneWeights;
                      Flags |= PostProcessSteps.CalculateTangentSpace;
                      Flags |= PostProcessSteps.GenerateNormals;*/

                   
                    scene = Importer.ImportFile(FileName, settings.GetFlags());
                    if (Utils.GetExtension(FileName) == ".dae") {
                        GetRealNodeNames(FileName);
                        isDae = true;
                    }

                    STConsole.WriteLine($"UnitScale {UnitScale}");

                    LoadScene();

                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                if (e.ToString().Contains("Error loading unmanaged library from path"))
                {
                    MessageBox.Show($"Failed to load assimp! Make sure you have Assimp32.dll next to the program!");
                }
                else
                    MessageBox.Show($"{e.ToString()}");

                return false;
            }
        }

        private void GetRealNodeNames(string FileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);
            XmlNodeList elemList = doc.GetElementsByTagName("node");
            for (int i = 0; i < elemList.Count; i++)
            {
                if (elemList[i].Name == "node")
                {
                    string nodeName = "";
                    string typeName = "";

                    foreach (XmlAttribute nodeAtt in elemList[i].Attributes)
                    {
                        if (nodeAtt.Name == "name")
                            nodeName = nodeAtt.InnerText;

                        if (nodeAtt.Name == "type")
                            typeName = nodeAtt.InnerText;
                    }

                    if (!DaeHelper.VisualSceneNodeTypes.ContainsKey(nodeName))
                        DaeHelper.VisualSceneNodeTypes.Add(nodeName, typeName);
                }

                string ID = "";
                string Name = "";
                foreach (XmlAttribute att in elemList[i].Attributes)
                {
                    if (att.Name == "id")
                        ID = att.InnerText;
                    if (att.Name == "name")
                        Name = att.InnerText;
                }

                if (!DaeHelper.IDMapToName.ContainsKey(ID))
                    DaeHelper.IDMapToName.Add(ID, Name);
            }
            XmlNodeList elemListScale = doc.GetElementsByTagName("unit");
            for (int i = 0; i < elemListScale.Count; i++)
            {
                foreach (XmlAttribute att in elemListScale[i].Attributes)
                {
                    if (att.Name == "meter")
                        float.TryParse(att.InnerText, out UnitScale);
                }
            }
        }

        public void processNode()
        {
            Console.WriteLine($"Mesh Count " + scene.MeshCount);
            Matrix4x4 identity = Matrix4x4.Identity;
            if (scene.RootNode != null)
            {
                BuildNode(scene.RootNode, ref identity);
                Console.WriteLine($"BuildNode Mesh Count " + scene.MeshCount);
            }
            else
            {
                STConsole.WriteLine("No root node found! Will default to Idenity Matrix!", Color.Red);

                int Index = 0;
                foreach (Mesh msh in scene.Meshes)
                {
                    objects.Add(CreateGenericObject(null, msh, Index, Matrix4.Identity));
                    Index++;
                }
            }
        }

        public static Animations.Animation[] ImportAnimations(string FileName)
        {
            AssimpContext Importer = new AssimpContext();
            STConsole.WriteLine($"Loading File {FileName}", Color.FromArgb(0, 255, 0));

            var scene = Importer.ImportFile(FileName);

            List<Animations.Animation> Anims = new List<Animations.Animation>();
            foreach (var anim in scene.Animations)
                Anims.Add(CreateGenericAnimation(anim));

            return Anims.ToArray();
        }

        private void BuildNode(Node parent, ref Matrix4x4 rootTransform)
        {
            Matrix4x4 world = rootTransform;
            Matrix4 worldTK = Matrix4.Identity;
            if (UseTransformMatrix)
            {
                 Matrix4x4 trafo = parent.Transform;
                 world = trafo * rootTransform;
                 worldTK = AssimpHelper.TKMatrix(world);
            }

            if (parent.MeshCount > 0)
            {
                STConsole.WriteLine($"Use Transform Matrix {UseTransformMatrix}");
                STConsole.WriteLine($"Transform node {parent.Name}");
                STConsole.WriteLine($"Translation {worldTK.ExtractTranslation()}");
                STConsole.WriteLine($"Rotation {worldTK.ExtractRotation()}");
                STConsole.WriteLine($"Scale {worldTK.ExtractScale()}");
            }

            foreach (int index in parent.MeshIndices)
                objects.Add(CreateGenericObject(parent, scene.Meshes[index], index, worldTK));
            
            foreach (Node child in parent.Children)
                BuildNode(child, ref world);
        }
        public void LoadScene()
        {
            objects.Clear();
            materials.Clear();
            BoneNames.Clear();
            skeleton = new STSkeleton();

            processNode();

            if (scene.RootNode != null)
            {
                var rootTransform = scene.RootNode.Transform;
                Matrix4 transformMat = AssimpHelper.TKMatrix(rootTransform);

                var scale = transformMat.ExtractScale();
                var rotation = transformMat.ExtractRotation();
                var position = transformMat.ExtractTranslation();

                STConsole.WriteLine($"-".Repeat(30));
                STConsole.WriteLine($"rootTransform {transformMat}");
                STConsole.WriteLine($"scale {scale}");
                STConsole.WriteLine($"rotation {rotation}");
                STConsole.WriteLine($"position {position}");
                STConsole.WriteLine($"-".Repeat(30));

                var SklRoot = GetSklRoot(scene.RootNode, BoneNames);
                if (SklRoot != null)
                {
                    BuildSkeletonNodes(SklRoot, BoneNames, skeleton, ref rootTransform);
                }
                else
                {
                    BuildSkeletonNodes(scene.RootNode, BoneNames, skeleton, ref rootTransform);
                }

                skeleton.update();
                skeleton.reset();
            }


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
        public static Animations.Animation CreateGenericAnimation(Assimp.Animation animation)
        {
            Animations.Animation STanim = new Animations.Animation();
            STanim.Text = animation.Name;
            float TicksPerSecond =  animation.TicksPerSecond != 0 ? (float)animation.TicksPerSecond : 25.0f;
            float Duriation = (float)animation.DurationInTicks;
            STanim.FrameCount = (int)(Duriation * 30);


            //Load node animations
            if (animation.HasNodeAnimations)
            {
                var _channels = new NodeAnimationChannel[animation.NodeAnimationChannelCount];
                for (int i = 0; i < _channels.Length; i++)
                {
                    _channels[i] = new NodeAnimationChannel();
                    var boneAnim = new Animations.Animation.KeyNode(_channels[i].NodeName);
                    boneAnim.RotType = Animations.Animation.RotationType.EULER;
                    STanim.Bones.Add(boneAnim);

                    STConsole.WriteLine($"Creating Bone Anims {boneAnim.Text} ");

                    for (int frame = 0; frame < STanim.FrameCount; i++)
                    {
                        if (_channels[i].HasPositionKeys)
                        {
                            for (int key = 0; key < _channels[i].PositionKeyCount; key++)
                            {
                                if (frame == _channels[i].PositionKeys[key].Time)
                                {
                                    boneAnim.XPOS.Keys.Add(new Animations.Animation.KeyFrame()
                                    {
                                        Value = _channels[i].PositionKeys[key].Value.X,
                                        Frame = frame,
                                });
                                    boneAnim.YPOS.Keys.Add(new Animations.Animation.KeyFrame()
                                    {
                                        Value = _channels[i].PositionKeys[key].Value.Y,
                                        Frame = frame,
                                    });
                                    boneAnim.ZPOS.Keys.Add(new Animations.Animation.KeyFrame()
                                    {
                                        Value = _channels[i].PositionKeys[key].Value.Z,
                                        Frame = frame,
                                    });
                                }
                            }
                        }
                        if (_channels[i].HasRotationKeys)
                        {
                            for (int key = 0; key < _channels[i].RotationKeyCount; key++)
                            {
                                if (frame == _channels[i].RotationKeys[key].Time)
                                {
                                    var quat = _channels[i].RotationKeys[key].Value;
                                    var euler = STMath.ToEulerAngles(quat.X, quat.Y, quat.Z, quat.W);

                                    boneAnim.XROT.Keys.Add(new Animations.Animation.KeyFrame()
                                    {
                                        Value = euler.X,
                                        Frame = frame,
                                    });
                                    boneAnim.YROT.Keys.Add(new Animations.Animation.KeyFrame()
                                    {
                                        Value = euler.Y,
                                        Frame = frame,
                                    });
                                    boneAnim.ZROT.Keys.Add(new Animations.Animation.KeyFrame()
                                    {
                                        Value = euler.Z,
                                        Frame = frame,
                                    });
                                    boneAnim.WROT.Keys.Add(new Animations.Animation.KeyFrame()
                                    {
                                        Value = 1,
                                        Frame = frame,
                                    });
                                }
                            }
                        }
                        if (_channels[i].HasScalingKeys)
                        {
                            for (int key = 0; key < _channels[i].ScalingKeyCount; key++)
                            {
                                if (frame == _channels[i].ScalingKeys[key].Time)
                                {
                                    boneAnim.XSCA.Keys.Add(new Animations.Animation.KeyFrame()
                                    {
                                        Value = _channels[i].ScalingKeys[key].Value.X,
                                        Frame = frame,
                                    });
                                    boneAnim.YSCA.Keys.Add(new Animations.Animation.KeyFrame()
                                    {
                                        Value = _channels[i].ScalingKeys[key].Value.Y,
                                        Frame = frame,
                                    });
                                    boneAnim.ZSCA.Keys.Add(new Animations.Animation.KeyFrame()
                                    {
                                        Value = _channels[i].ScalingKeys[key].Value.Z,
                                        Frame = frame,
                                    });
                                }
                            }
                        }
                    }
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
            matTex.WrapModeS = SetWrapMode(tex.WrapModeU);
            matTex.WrapModeT = SetWrapMode(tex.WrapModeV);

            STConsole.WriteLine($"Getting assimp texture slot {matTex.Name} Type {matTex.Type}");

            return matTex;
        }
        private STTextureWrapMode SetWrapMode(TextureWrapMode wrap)
        {
            switch (wrap)
            {
                case TextureWrapMode.Wrap:
                    return STTextureWrapMode.Repeat;
                case TextureWrapMode.Mirror:
                    return STTextureWrapMode.Mirror;
                case TextureWrapMode.Clamp:
                    return STTextureWrapMode.Clamp;
                case TextureWrapMode.Decal:
                    return STTextureWrapMode.Repeat;
                default:
                    return STTextureWrapMode.Repeat;
            }
        }

        private Node GetSklRoot(Node node, List<string> boneNames)
        {
            string Name = node.Name;

            if (DaeHelper.IDMapToName.ContainsKey(node.Name))
                Name = DaeHelper.IDMapToName[node.Name];

            string ParentArmatureName = node.Parent != null ? node.Parent.Name : "";
            if (ParentArmatureName != string.Empty && DaeHelper.IDMapToName.ContainsKey(ParentArmatureName))
                ParentArmatureName = DaeHelper.IDMapToName[ParentArmatureName];

            bool IsBone = boneNames.Contains(Name) && !boneNames.Contains(ParentArmatureName) ||
                       Name.Contains("Skl_Root") || Name.Contains("nw4f_root") ||
                       Name.Contains("skl_root") || Name == "Root";

            if (DaeHelper.VisualSceneNodeTypes.ContainsKey(Name)) {
                if (DaeHelper.VisualSceneNodeTypes[Name] == "JOINT")
                    IsBone = true;
            }

            if (IsBone)
                return node;

            if (node.HasChildren)
            {
                foreach (Node child in node.Children)
                {
                    Node ChildNode = GetSklRoot(child, boneNames);
                    if (ChildNode == null)
                    {
                        continue;
                    }
                    return ChildNode;
                }
                    
            }
            return null;
        }

        private void BuildSkeletonNodes(Node node, List<string> boneNames, STSkeleton skeleton, ref Matrix4x4 rootTransform)
        {
            Matrix4x4 trafo = node.Transform;
            Matrix4x4 world = trafo * rootTransform;
            Matrix4 worldTK = AssimpHelper.TKMatrix(world);

            string Name = node.Name;
            string ParentArmatureName = node.Parent != null ? node.Parent.Name : "";

            if (DaeHelper.IDMapToName.ContainsKey(node.Name))
                Name = DaeHelper.IDMapToName[node.Name];

            if (ParentArmatureName != string.Empty && DaeHelper.IDMapToName.ContainsKey(ParentArmatureName))
                ParentArmatureName = DaeHelper.IDMapToName[ParentArmatureName];

            bool IsBone = boneNames.Contains(Name) && !boneNames.Contains(ParentArmatureName) ||
                          Name.Contains("Skl_Root") || Name.Contains("nw4f_root") ||
                          Name.Contains("skl_root") || Name.Contains("all_root") || Name.Contains("_root") || Name.Contains("Root");

            if (DaeHelper.VisualSceneNodeTypes.ContainsKey(Name)) {
                if (DaeHelper.VisualSceneNodeTypes[Name] == "JOINT")
                    IsBone = true;
            }

            //Root set saved by this tool
            //Get our root manually as it's a child to this
            bool IsRootSkeleton = Name == "skeleton_root";

            short SmoothIndex = 0;
            short RigidIndex = -1;

            //Loop through all the bones. If the parent is not in the bone list, then it's Parent is the root
            if (IsBone)
            {
                var idenity = Matrix4x4.Identity;
                CreateByNode(node, skeleton, ParentArmatureName, SmoothIndex, RigidIndex, true, ref rootTransform);
            }
            else if (IsRootSkeleton && node.HasChildren)
            {
                var idenity = Matrix4x4.Identity;
                CreateByNode(node.Children[0], skeleton, ParentArmatureName, SmoothIndex, RigidIndex, true, ref world);
            }
            else
            {
                if (node.HasChildren)
                {
                    foreach (Node child in node.Children)
                        BuildSkeletonNodes(child, boneNames, skeleton, ref world);
                }
            }
        }

        private List<Node> tempBoneNodes = new List<Node>();
        private void CreateByNode(Node node, STSkeleton skeleton, string ParentArmatureName, 
            short SmoothIndex, short RigidIndex, bool IsRoot, ref Assimp.Matrix4x4 rootTransform)
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
                bone.RotationType = STBone.BoneRotationType.Euler;
                skeleton.bones.Add(bone);

                if (DaeHelper.IDMapToName.ContainsKey(node.Name))
                    bone.Text = DaeHelper.IDMapToName[node.Name];
                else
                    bone.Text = node.Name;

                bone.SmoothMatrixIndex = (short)skeleton.bones.IndexOf(bone);
                bone.RigidMatrixIndex = -1; //Todo calculate these

                if (IsRoot)
                {
                    bone.parentIndex = -1;

                    if (RotateSkeleton)
                        transformMat = AssimpHelper.TKMatrix(world * Matrix4x4.FromRotationX(MathHelper.DegreesToRadians(RotateSkeletonAmount)));
                    else
                        transformMat = AssimpHelper.TKMatrix(world);
                }
                else
                {
                    if (tempBoneNodes.Contains(node.Parent))
                        bone.parentIndex = tempBoneNodes.IndexOf(node.Parent);
                }


                var scale = transformMat.ExtractScale();
                var rotation = transformMat.ExtractRotation();
                var position = transformMat.ExtractTranslation();

                STConsole.WriteLine($"-".Repeat(30));
                STConsole.WriteLine($"Processing Bone {bone.Text}");
                STConsole.WriteLine($"scale {scale}");
                STConsole.WriteLine($"rotation {rotation}");
                STConsole.WriteLine($"position {position}");
                STConsole.WriteLine($"-".Repeat(30));

                bone.FromTransform(transformMat);
            }
            else
            {
                STConsole.WriteLine($"Duplicate node name found for bone {node.Name}!", Color.Red);
            }

            var identity = Matrix4x4.Identity;
            foreach (Node child in node.Children)
                CreateByNode(child, skeleton, ParentArmatureName, SmoothIndex, RigidIndex, false, ref identity);
        }
        public STGenericObject CreateGenericObject(Node parentNode,Mesh msh, int Index, Matrix4 transform)
        {
            STGenericObject obj = new STGenericObject();
            obj.BoneIndex = 0;

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
            obj.HasVertColors2 = msh.HasVertexColors(1);

            obj.ObjectName = msh.Name;
            if (parentNode != null && msh.Name == string.Empty)
                obj.ObjectName = parentNode.Name;

            obj.boneList = GetBoneList(msh);

            STGenericObject.LOD_Mesh lod = new STGenericObject.LOD_Mesh();
            lod.faces = GetFaces(msh);
            lod.IndexFormat = STIndexFormat.UInt16;
            lod.PrimativeType = STPrimitiveType.Triangles;
            lod.GenerateSubMesh();
            obj.lodMeshes.Add(lod);
            obj.vertices = GetVertices(msh, transform, obj);
            obj.VertexBufferIndex = Index;
            obj.VertexSkinCount = (byte)obj.vertices.Max(x => x.boneNames.Count);

            return obj;
        }

        public List<int> GetFaces(Mesh msh)
        {
            List<int> faces = new List<int>();

            if (msh.HasFaces)
            {
                foreach (Face f in msh.Faces)
                {
                    if (f.HasIndices) {
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
                string name = b.Name;
                if (DaeHelper.IDMapToName.ContainsKey(name))
                    name = DaeHelper.IDMapToName[name];

                if (!bones.Contains(name))
                    bones.Add(name);
            }
            return bones;
        }

        public List<Vertex> GetVertices(Mesh msh, Matrix4 transform, STGenericObject STobj)
        {
            Matrix4 NormalsTransform = Matrix4.CreateFromQuaternion(transform.ExtractRotation());

            List<Vertex> vertices = new List<Vertex>();
            for (int v = 0; v < msh.VertexCount; v++)
            {
                Vertex vert = new Vertex();

                if (msh.HasVertices)
                    vert.pos = Vector3.TransformPosition(AssimpHelper.FromVector(msh.Vertices[v]), transform);
                if (msh.HasNormals)
                    vert.nrm = Vector3.TransformNormal(AssimpHelper.FromVector(msh.Normals[v]), NormalsTransform);
                if (msh.HasTextureCoords(0))
                    vert.uv0 = new Vector2(msh.TextureCoordinateChannels[0][v].X, msh.TextureCoordinateChannels[0][v].Y);
                if (msh.HasTextureCoords(1))
                    vert.uv1 = new Vector2(msh.TextureCoordinateChannels[1][v].X, msh.TextureCoordinateChannels[1][v].Y);
                if (msh.HasTextureCoords(2))
                    vert.uv2 = new Vector2(msh.TextureCoordinateChannels[2][v].X, msh.TextureCoordinateChannels[2][v].Y);
                if (msh.HasTangentBasis)
                    vert.tan = new Vector4(msh.Tangents[v].X, msh.Tangents[v].Y, msh.Tangents[v].Z, 1);
                if (msh.HasVertexColors(0) && !isDae)
                    vert.col = new Vector4(msh.VertexColorChannels[0][v].R, msh.VertexColorChannels[0][v].G, msh.VertexColorChannels[0][v].B, msh.VertexColorChannels[0][v].A);
                if (msh.HasVertexColors(1) && !isDae)
                    vert.col2 = new Vector4(msh.VertexColorChannels[1][v].R, msh.VertexColorChannels[1][v].G, msh.VertexColorChannels[1][v].B, msh.VertexColorChannels[1][v].A);
                if (msh.HasTangentBasis)
                    vert.bitan = new Vector4(msh.BiTangents[v].X, msh.BiTangents[v].Y, msh.BiTangents[v].Z, 1);
                vertices.Add(vert);
            }
            if (msh.HasBones)
            {
                for (int i = 0; i < msh.BoneCount; i++)
                {
                    Bone bn = msh.Bones[i];
                    if (bn.HasVertexWeights)
                    {
                        foreach (VertexWeight w in bn.VertexWeights)
                        {
                            if (DaeHelper.IDMapToName.ContainsKey(bn.Name))
                                bn.Name = DaeHelper.IDMapToName[bn.Name];

                            vertices[w.VertexID].boneWeights.Add(w.Weight);
                            vertices[w.VertexID].boneNames.Add(bn.Name);

                            if (!BoneNames.Contains(bn.Name))
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
