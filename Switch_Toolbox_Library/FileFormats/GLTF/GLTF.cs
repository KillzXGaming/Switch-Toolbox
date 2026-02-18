using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Toolbox.Library.GLTFModel;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class GLTF
    {
        public class ExportSettings
        {
            public bool UseVertexColors = false;
            public bool FlipTexCoordsVertical = false;
            public bool UseTextureChannelComponents = true;
            public bool ExportTextures = true;
        }

        public static void RawTextureExport(string FileName, List<STGenericTexture> Textures, List<STGenericMaterial> Materials, ExportSettings settings)
        {
            if (Textures?.Count == 0)
                return;

            STProgressBar progressBar = new STProgressBar();
            progressBar.Task = "Exporting Model...";
            progressBar.Value = 0;
            progressBar.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            progressBar.Show();
            progressBar.Refresh();

            string TexturePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FileName), "Raw");
            System.IO.Directory.CreateDirectory(TexturePath);
            List<string> failedTextureExport = new List<string>();

            for (int i = 0; i < Textures?.Count; i++)
            {
                progressBar.Task = $"Exporting Texture {Textures[i].Text}";
                progressBar.Value = ((i * 100) / Textures.Count);
                progressBar.Refresh();

                try
                {
                    var bitmap = Textures[i].GetBitmap();
                    if (bitmap != null)
                    {
                        if (settings.UseTextureChannelComponents)
                            bitmap = Textures[i].GetComponentBitmap(bitmap);
                        string textureName = Textures[i].Text;
                        if (textureName.RemoveIllegaleFileNameCharacters() != textureName)
                        {
                            string properName = textureName.RemoveIllegaleFileNameCharacters();
                            for (int m = 0; m < Materials?.Count; m++)
                            {
                                foreach (var tex in Materials[m].TextureMaps)
                                {
                                    if (tex.Name == textureName)
                                        tex.Name = properName;
                                }
                            }

                            textureName = properName;
                        }

                        bitmap.Save($"{TexturePath}/{textureName}.png");
                        bitmap.Dispose();

                        GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    failedTextureExport.Add(Textures[i].Text);
                }
            }

            progressBar?.Close();
        }

        public static void Export(string FileName, ExportSettings settings, STGenericModel model, List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            Export(FileName, settings, model.Objects.ToList(), model.Materials.ToList(), Textures, skeleton, NodeArray);
        }

        public static void Export(string FileName, ExportSettings settings,
            List<STGenericObject> Meshes, List<STGenericMaterial> Materials,
            List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            var Exporter = new GLTFExporter();

            if (settings.ExportTextures)
                RawTextureExport(FileName, Textures, Materials, settings);

            for (int i = 0; i < Textures?.Count; i++)
            {
                try
                {
                    var bitmap = Textures[i].GetBitmap();
                    if (bitmap != null)
                    {
                        if (settings.UseTextureChannelComponents)
                            bitmap = Textures[i].GetComponentBitmap(bitmap);
                        string textureName = Textures[i].Text;
                        Exporter.AddTextureMap(textureName, bitmap);
                    }
                }
                catch (Exception ex) {}
            }

            foreach (var mat in Materials)
            {
                Exporter.AddMaterial(mat);
            }

            foreach (var bone in skeleton.bones)
            {
                var translation = bone.GetPosition();
                var rotation = bone.GetRotation();
                var scale = bone.GetScale();
                Exporter.AddNode(bone.Text, 
                    new Vector3(translation.X, translation.Y, translation.Z),
                    new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W),
                    new Vector3(scale.X, scale.Y, scale.Z));
            }

            for (int i = 0; i < skeleton.bones.Count; i++)
            {
                Exporter.ParentNode(i, skeleton.bones[i].parentIndex);
            }

            foreach (var mesh in Meshes)
            {
                string meshName = mesh.Text;
                List<Vector3> Vertices = new List<Vector3>();
                List<Vector3> Normals = new List<Vector3>();
                List<Vector4> Colors0 = new List<Vector4>();
                List<Vector4> Colors1 = new List<Vector4>();
                List<Vector2> UV0 = new List<Vector2>();
                List<Vector2> UV1 = new List<Vector2>();
                List<Vector2> UV2 = new List<Vector2>();
                List<Vector2> UV3 = new List<Vector2>();
                List<SparseWeight8> Skins = new List<SparseWeight8>();
                List<int> TriangleFaces = new List<int>();

                bool HasNormals = false;
                bool HasColors0 = false;
                bool HasColors1 = false;
                bool HasUV0 = false;
                bool HasUV1 = false;
                bool HasUV2 = false;
                bool HasUV3 = false;
                bool HasBoneIds = false;
                int SkinsCount = 0;

                foreach (var vertex in mesh.vertices)
                {
                    if (vertex.nrm != OpenTK.Vector3.Zero) HasNormals = true;
                    if (settings.UseVertexColors) HasColors0 = true;
                    if (vertex.col2 != OpenTK.Vector4.One && settings.UseVertexColors) HasColors1 = true;
                    if (vertex.uv0 != OpenTK.Vector2.Zero) HasUV0 = true;
                    if (vertex.uv1 != OpenTK.Vector2.Zero) HasUV1 = true;
                    if (vertex.uv2 != OpenTK.Vector2.Zero) HasUV2 = true;
                    if (vertex.uv3 != OpenTK.Vector2.Zero) HasUV3 = true;
                    if (vertex.boneIds.Count > 0) HasBoneIds = true;

                    Vertices.Add(new Vector3(vertex.pos.X, vertex.pos.Y, vertex.pos.Z));
                    Normals.Add(new Vector3(vertex.nrm.X, vertex.nrm.Y, vertex.nrm.Z));

                    if (settings.FlipTexCoordsVertical)
                    {
                        UV0.Add(new Vector2(vertex.uv0.X, 1 - vertex.uv0.Y));
                        UV1.Add(new Vector2(vertex.uv1.X, 1 - vertex.uv1.Y));
                        UV2.Add(new Vector2(vertex.uv2.X, 1 - vertex.uv2.Y));
                        UV3.Add(new Vector2(vertex.uv3.X, 1 - vertex.uv3.Y));
                    }
                    else
                    {
                        UV0.Add(new Vector2(vertex.uv0.X, vertex.uv0.Y));
                        UV1.Add(new Vector2(vertex.uv1.X, vertex.uv1.Y));
                        UV2.Add(new Vector2(vertex.uv2.X, vertex.uv2.Y));
                        UV3.Add(new Vector2(vertex.uv3.X, vertex.uv3.Y));
                    }

                    Colors0.Add(new Vector4(vertex.col.X, vertex.col.Y, vertex.col.Z, vertex.col.W));
                    Colors1.Add(new Vector4(vertex.col2.X, vertex.col2.Y, vertex.col2.Z, vertex.col2.W));

                    List<int> bIndices = new List<int>();
                    List<float> bWeights = new List<float>();
                    float weightSum = 0;
                    for (int b = 0; b < Math.Min(vertex.boneIds.Count, mesh.VertexSkinCount); b++)
                    {
                        int boneIndex = vertex.boneIds[b];
                        float boneWeight = (b < vertex.boneWeights.Count) ? vertex.boneWeights[b] : 0;

                        if (boneIndex >= 0 && boneIndex < skeleton?.bones.Count)
                        {
                            bIndices.Add(boneIndex);
                            bWeights.Add(boneWeight);
                            weightSum += boneWeight;
                        }
                    }
                    // Rigid bodies with a valid bone index
                    if (bIndices.Count == 0 && mesh.boneList.Count > 0 && mesh.BoneIndex >= 0 && mesh.BoneIndex < skeleton?.bones.Count)
                    {
                        bIndices.Add(mesh.BoneIndex);
                        bWeights.Add(1f);
                    }
                    // Bone indices exist with no weights mapped
                    if (weightSum < 1e-6f && bIndices.Count > 0)
                    {
                        bIndices.RemoveRange(1, bIndices.Count-1);
                        bWeights[0] = 1f;
                    }
                    SkinsCount = Math.Max(SkinsCount, bIndices.Count);

                    // Fill enough to define SparseWeight8
                    for (int i = bIndices.Count; i < 8; i++)
                    {
                        bIndices.Add(0);
                        bWeights.Add(0);
                    }
                    Skins.Add(SparseWeight8.Create(
                        new Vector4(bIndices[0], bIndices[1], bIndices[2], bIndices[3]),
                        new Vector4(bIndices[4], bIndices[5], bIndices[6], bIndices[7]),
                        new Vector4(bWeights[0], bWeights[1], bWeights[2], bWeights[3]),
                        new Vector4(bWeights[4], bWeights[5], bWeights[6], bWeights[7])
                    ));
                }

                if (mesh.lodMeshes.Count > 0)
                {
                    var lodMesh = mesh.lodMeshes[mesh.DisplayLODIndex];

                    List<int> faces = new List<int>();
                    if (lodMesh.PrimativeType == STPrimitiveType.TrangleStrips)
                        faces = STGenericObject.ConvertTriangleStripsToTriangles(lodMesh.faces);
                    else
                        faces = lodMesh.faces;

                    if (faces.Count % 3 != 0)
                        throw new Exception("Expected a multiple of 3!");

                    TriangleFaces.AddRange(faces);
                }
                foreach (var group in mesh.PolygonGroups)
                {
                    List<int> faces = new List<int>();
                    if (group.PrimativeType == STPrimitiveType.TrangleStrips)
                        faces = STGenericObject.ConvertTriangleStripsToTriangles(group.faces);
                    else
                        faces = group.faces;

                    if (faces.Count % 3 != 0)
                        throw new Exception("Expected a multiple of 3!");

                    TriangleFaces.AddRange(faces);
                }

                int ColorsCount =
                    HasColors1 ? 2 :
                    HasColors0 ? 1 :
                    0;
                int UVCount =
                    HasUV3 ? 4 :
                    HasUV2 ? 3 :
                    HasUV1 ? 2 :
                    HasUV0 ? 1 :
                    0;

                Exporter.AddMesh(meshName, mesh.MaterialIndex,
                    Vertices, Normals, Colors0, Colors1,
                    UV0, UV1, UV2, UV3, Skins, TriangleFaces,
                    ColorsCount, UVCount, SkinsCount);
            }

            string ext = System.IO.Path.GetExtension(FileName);
            if (ext == ".glb")
                Exporter.WriteGLB(FileName);
            else
                Exporter.WriteGLTF(FileName);
        }
    }
}
