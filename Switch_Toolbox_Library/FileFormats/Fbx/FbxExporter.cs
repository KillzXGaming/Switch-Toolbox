using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;

namespace Toolbox.Library.FBX
{
    public class FbxExporter
    {
        private enum MaterialTextureType
        {
            Diffuse = 0,
            Normal = 1,
            Specular = 2,
            Emission = 3,
            Alpha = 4,
            NormalFlatColor = 5,
        }

        public static void Export(string FileName, DAE.ExportSettings settings, STGenericModel model, List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            Export(FileName, settings, model.Objects.ToList(), model.Materials.ToList(), Textures, skeleton, NodeArray);
        }

        public static void Export(string FileName, DAE.ExportSettings settings,
           List<STGenericObject> Meshes, List<STGenericMaterial> Materials,
           List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            if (Materials == null)
                Materials = new List<STGenericMaterial>();
            if (skeleton != null && skeleton.BoneIndices != null)
                NodeArray = skeleton.BoneIndices.ToList();

            FbxNativeWrapper.Exp_CreateContext();

            if (!FbxNativeWrapper.Exp_Initialize(FileName))
            {
                MessageBox.Show("Failed to initialize FBX Exporter!");
                FbxNativeWrapper.Exp_DestroyContext();
                return;
            }

            Dictionary<int, string> materialNameMap = RegisterMaterialsAndTextures(FileName, settings, Materials, Textures);

            if (skeleton != null)
            {
                for (int i = 0; i < skeleton.bones.Count; i++)
                {
                    FbxNativeWrapper.Exp_RegisterBone(i, skeleton.bones[i].Text);
                }

                for (int i = 0; i < skeleton.bones.Count; i++)
                {
                    var bone = skeleton.bones[i];
                    string parentName = "";
                    if (bone.parentIndex != -1 && bone.parentIndex < skeleton.bones.Count)
                        parentName = skeleton.bones[bone.parentIndex].Text;

                    FbxNativeWrapper.Exp_CreateNode(
                        bone.Text,
                        parentName,
                        MatrixToDoubleArray(bone.GetTransform())
                    );
                }
            }

            STProgressBar progressBar = new STProgressBar();
            progressBar.Task = "Exporting FBX Meshes...";
            progressBar.Value = 0;
            progressBar.StartPosition = FormStartPosition.CenterScreen;
            progressBar.Show();

            int meshIndex = 0;
            foreach (var mesh in Meshes)
            {
                progressBar.Task = $"Exporting Mesh {mesh.Text}";
                progressBar.Value = Meshes.Count > 0 ? ((meshIndex++ * 100) / Meshes.Count) : 0;
                progressBar.Refresh();

                FbxNativeWrapper.Exp_StartMesh(mesh.Text);

                if (mesh.MaterialIndex != -1 && materialNameMap.TryGetValue(mesh.MaterialIndex, out string materialName))
                    FbxNativeWrapper.Exp_SetMeshMaterial(materialName);

                if (skeleton != null)
                {
                    foreach (var bone in skeleton.bones)
                    {
                        Matrix4 meshMatrix = Matrix4.Identity;
                        Matrix4 linkMatrix = skeleton.GetBoneTransform(bone);

                        FbxNativeWrapper.Exp_SetClusterMatrices(mesh.Text, bone.Text, MatrixToDoubleArray(meshMatrix), MatrixToDoubleArray(linkMatrix));
                    }
                }

                int[] IndexTable = NodeArray?.ToArray();
                Dictionary<int, Vector2> uvTransformMap = BuildUvTransformMap(mesh, settings.TransformColorUVs);

                // Detect what attributes are actually present in the mesh
                // This matches the DAE exporter's approach of checking actual vertex data
                bool hasNormals = false;
                bool hasColors = false;
                bool hasColors2 = false;
                bool hasColors3 = false;
                bool hasColors4 = false;
                bool hasUV0 = false;
                bool hasUV1 = false;
                bool hasUV2 = false;
                bool hasUV3 = false;
                bool hasTangents = false;
                bool hasBitangents = false;

                foreach (var vertex in mesh.vertices)
                {
                    if (vertex.nrm != Vector3.Zero) hasNormals = true;
                    if (settings.UseVertexColors && vertex.col != Vector4.One) hasColors = true;
                    if (settings.UseVertexColors && vertex.col2 != Vector4.One) hasColors2 = true;
                    if (settings.UseVertexColors && vertex.col3 != Vector4.One) hasColors3 = true;
                    if (settings.UseVertexColors && vertex.col4 != Vector4.One) hasColors4 = true;
                    if (vertex.uv0 != Vector2.Zero) hasUV0 = true;
                    if (vertex.uv1 != Vector2.Zero) hasUV1 = true;
                    if (vertex.uv2 != Vector2.Zero) hasUV2 = true;
                    if (vertex.uv3 != Vector2.Zero) hasUV3 = true;
                    // Check for tangents and bitangents (from BFRES _t0 and _b0 attributes)
                    if (vertex.tan != Vector4.Zero && vertex.tan.Xyz != Vector3.Zero) hasTangents = true;
                    if (vertex.bitan != Vector4.Zero && vertex.bitan.Xyz != Vector3.Zero) hasBitangents = true;
                }

                for (int vertexIndex = 0; vertexIndex < mesh.vertices.Count; vertexIndex++)
                {
                    Vertex v = mesh.vertices[vertexIndex];

                    int[] bIndices = new int[4] { -1, -1, -1, -1 };
                    double[] bWeights = new double[4] { 0, 0, 0, 0 };

                    double totalWeight = 0;
                    for (int b = 0; b < v.boneIds.Count && b < 4; b++)
                    {
                        int id = v.boneIds[b];
                        float weight = (b < v.boneWeights.Count) ? v.boneWeights[b] : 0;

                        if (weight <= 0)
                            continue;

                        int globalID = GetGlobalBoneIndex(id, mesh, IndexTable, skeleton);
                        if (globalID != -1)
                        {
                            bIndices[b] = globalID;
                            bWeights[b] = weight;
                            totalWeight += bWeights[b];
                        }
                    }

                    if (totalWeight <= 0 && v.boneIds.Count > 0)
                    {
                        int globalID = GetGlobalBoneIndex(v.boneIds[0], mesh, IndexTable, skeleton);
                        if (globalID != -1)
                        {
                            bIndices[0] = globalID;
                            bWeights[0] = 1.0;
                            totalWeight = 1.0;
                        }
                    }

                    if (totalWeight <= 0 && mesh.BoneIndex != -1)
                    {
                        bIndices[0] = GetGlobalBoneIndex(mesh.BoneIndex, mesh, IndexTable, skeleton);
                        bWeights[0] = 1.0;
                        totalWeight = 1.0;
                    }

                    if (totalWeight > 0 && Math.Abs(totalWeight - 1.0) > 0.001)
                    {
                        for (int b = 0; b < 4; b++)
                            bWeights[b] /= totalWeight;
                    }

                    FbxNativeWrapper.Exp_AddVertexExtended(
                        v.pos.X, v.pos.Y, v.pos.Z,
                        v.nrm.X, v.nrm.Y, v.nrm.Z,
                        bIndices[0], bIndices[1], bIndices[2], bIndices[3],
                        bWeights[0], bWeights[1], bWeights[2], bWeights[3]
                    );

                    // Add tangents (from BFRES _t0 attribute)
                    if (hasTangents)
                    {
                        FbxNativeWrapper.Exp_AddVertexTangent(v.tan.X, v.tan.Y, v.tan.Z);
                    }

                    // Add bitangents (from BFRES _b0 attribute)
                    if (hasBitangents)
                    {
                        FbxNativeWrapper.Exp_AddVertexBitangent(v.bitan.X, v.bitan.Y, v.bitan.Z);
                    }

                    // Add ALL UV channels that exist in the mesh
                    // This matches the DAE exporter which exports all UV sets
                    if (hasUV0 || mesh.HasUv0)
                    {
                        Vector2 uv0 = v.uv0;
                        if (uvTransformMap.TryGetValue(vertexIndex, out Vector2 transformedUv0))
                            uv0 = transformedUv0;
                        float vUV0 = settings.FlipTexCoordsVertical ? (1.0f - uv0.Y) : uv0.Y;
                        FbxNativeWrapper.Exp_AddVertexUV(0, uv0.X, vUV0);
                    }
                    if (hasUV1 || mesh.HasUv1)
                    {
                        float vUV1 = settings.FlipTexCoordsVertical ? (1.0f - v.uv1.Y) : v.uv1.Y;
                        FbxNativeWrapper.Exp_AddVertexUV(1, v.uv1.X, vUV1);
                    }
                    if (hasUV2 || mesh.HasUv2)
                    {
                        float vUV2 = settings.FlipTexCoordsVertical ? (1.0f - v.uv2.Y) : v.uv2.Y;
                        FbxNativeWrapper.Exp_AddVertexUV(2, v.uv2.X, vUV2);
                    }
                    if (hasUV3)
                    {
                        float vUV3 = settings.FlipTexCoordsVertical ? (1.0f - v.uv3.Y) : v.uv3.Y;
                        FbxNativeWrapper.Exp_AddVertexUV(3, v.uv3.X, vUV3);
                    }

                    // Add ALL color channels that exist in the mesh
                    // This matches the DAE exporter which exports all color sets
                    if (settings.UseVertexColors)
                    {
                        if (hasColors || mesh.HasVertColors || v.col != Vector4.One)
                            FbxNativeWrapper.Exp_AddVertexColor(0, v.col.X, v.col.Y, v.col.Z, v.col.W);
                        if (hasColors2 || mesh.HasVertColors2 || v.col2 != Vector4.One)
                            FbxNativeWrapper.Exp_AddVertexColor(1, v.col2.X, v.col2.Y, v.col2.Z, v.col2.W);
                        if (hasColors3 || v.col3 != Vector4.One)
                            FbxNativeWrapper.Exp_AddVertexColor(2, v.col3.X, v.col3.Y, v.col3.Z, v.col3.W);
                        if (hasColors4 || v.col4 != Vector4.One)
                            FbxNativeWrapper.Exp_AddVertexColor(3, v.col4.X, v.col4.Y, v.col4.Z, v.col4.W);
                    }
                    else
                    {
                        FbxNativeWrapper.Exp_AddVertexColor(0, 1.0, 1.0, 1.0, 1.0);
                    }
                }

                foreach (int idx in GetFaces(mesh))
                    FbxNativeWrapper.Exp_AddIndex(idx);

                FbxNativeWrapper.Exp_EndMesh();
            }

            progressBar.Close();

            if (skeleton != null)
                FbxNativeWrapper.Exp_AddPose("Bind Pose");

            bool result = FbxNativeWrapper.Exp_Save();
            FbxNativeWrapper.Exp_DestroyContext();

            if (result && !settings.SuppressConfirmDialog)
                MessageBox.Show($"Exported {FileName} Successfully!");
        }

        public static Dictionary<int, string> RegisterMaterialsAndTextures(
            string fileName,
            DAE.ExportSettings settings,
            List<STGenericMaterial> materials,
            List<STGenericTexture> textures)
        {
            Dictionary<int, string> materialNameMap = new Dictionary<int, string>();
            if (materials == null || materials.Count == 0)
                return materialNameMap;

            string textureFolder = Path.GetDirectoryName(fileName);
            if (string.IsNullOrEmpty(textureFolder))
                textureFolder = Environment.CurrentDirectory;
            Directory.CreateDirectory(textureFolder);

            Dictionary<string, string> exportedTextureNames = ExportTexturesToDisk(textureFolder, settings, textures);
            List<string> usedMaterialNames = new List<string>();
            List<string> usedTextureNames = exportedTextureNames.Values
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            for (int i = 0; i < materials.Count; i++)
            {
                STGenericMaterial mat = materials[i] ?? new STGenericMaterial();
                string sourceMaterialName = string.IsNullOrEmpty(mat.Text) ? $"Material_{i}" : mat.Text;
                string exportMaterialName = Utils.RenameDuplicateString(usedMaterialNames, sourceMaterialName);
                usedMaterialNames.Add(exportMaterialName);
                materialNameMap[i] = exportMaterialName;

                FbxNativeWrapper.Exp_AddMaterial(exportMaterialName, 1.0, 1.0, 1.0);
                if (settings.ForceZeroShininessFbx)
                    FbxNativeWrapper.Exp_SetMaterialShininess(exportMaterialName, 0.0);

                bool hasExplicitTransparencyMap = mat.TextureMaps.Any(
                    x => x != null && x.Type == STGenericMatTexture.TextureType.Transparency);

                foreach (var tex in mat.TextureMaps)
                {
                    MaterialTextureType? textureType = ConvertTextureType(tex?.Type ?? STGenericMatTexture.TextureType.Unknown);
                    if (!textureType.HasValue || string.IsNullOrEmpty(tex?.Name))
                        continue;

                    string textureName = ResolveTextureExportName(tex.Name, exportedTextureNames, usedTextureNames);
                    string texturePath = $"{textureName}.png";

                    // Keep normal map slot disconnected by not exporting normal texture bindings
                    // when the flat-color normal option is enabled.
                    if (textureType.Value == MaterialTextureType.Normal && settings.ExportNormalMapsAsFlatColorFbx)
                        continue;

                    FbxNativeWrapper.Exp_AddMaterialTexture(
                        exportMaterialName,
                        textureName,
                        texturePath,
                        (int)textureType.Value);

                    // DAE import in Blender usually links diffuse alpha automatically.
                    // Mirror this behavior for FBX unless a dedicated transparency map exists.
                    if (textureType.Value == MaterialTextureType.Diffuse && !hasExplicitTransparencyMap)
                    {
                        FbxNativeWrapper.Exp_AddMaterialTexture(
                            exportMaterialName,
                            textureName,
                            texturePath,
                            (int)MaterialTextureType.Alpha);
                    }
                }
            }

            return materialNameMap;
        }

        public static Dictionary<int, Vector2> BuildUvTransformMap(STGenericObject mesh, bool applyUvTransforms)
        {
            Dictionary<int, Vector2> transformedUvs = new Dictionary<int, Vector2>();
            if (!applyUvTransforms || mesh?.PolygonGroups == null || mesh.PolygonGroups.Count == 0)
                return transformedUvs;

            foreach (var poly in mesh.PolygonGroups)
            {
                if (poly?.Material == null)
                    continue;

                var diffuse = poly.Material.TextureMaps.FirstOrDefault(x => x.Type == STGenericMatTexture.TextureType.Diffuse);
                STTextureTransform transform = diffuse?.Transform;
                if (transform == null)
                    continue;

                List<int> faces = poly.GetDisplayFace();
                if (faces == null || faces.Count == 0)
                    continue;

                int faceCount = poly.displayFaceSize > 0 ? Math.Min(poly.displayFaceSize, faces.Count) : faces.Count;
                for (int i = 0; i < faceCount; i++)
                {
                    int vertexIndex = faces[i];
                    if (vertexIndex < 0 || vertexIndex >= mesh.vertices.Count || transformedUvs.ContainsKey(vertexIndex))
                        continue;

                    Vector2 uv = mesh.vertices[vertexIndex].uv0;
                    transformedUvs.Add(vertexIndex, (uv * transform.Scale) + transform.Translate);
                }
            }

            return transformedUvs;
        }

        private static List<int> GetFaces(STGenericObject mesh)
        {
            List<int> faces = new List<int>();

            if (mesh.lodMeshes.Count > 0)
            {
                var lodMesh = mesh.lodMeshes[mesh.DisplayLODIndex];
                if (lodMesh.PrimativeType == STPrimitiveType.TrangleStrips)
                    faces = STGenericObject.ConvertTriangleStripsToTriangles(lodMesh.faces);
                else
                    faces = lodMesh.faces;
            }
            else if (mesh.PolygonGroups.Count > 0)
            {
                foreach (var group in mesh.PolygonGroups)
                {
                    List<int> subFaces;
                    if (group.PrimativeType == STPrimitiveType.TrangleStrips)
                        subFaces = STGenericObject.ConvertTriangleStripsToTriangles(group.faces);
                    else
                        subFaces = group.faces;
                    faces.AddRange(subFaces);
                }
            }

            return faces;
        }

        private static Dictionary<string, string> ExportTexturesToDisk(
            string textureFolder,
            DAE.ExportSettings settings,
            List<STGenericTexture> textures)
        {
            Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!settings.ExportTextures || textures == null)
                return map;

            List<string> usedNames = new List<string>();
            for (int i = 0; i < textures.Count; i++)
            {
                STGenericTexture texture = textures[i];
                string sourceName = texture?.Text;
                if (string.IsNullOrEmpty(sourceName))
                    sourceName = $"Texture_{i}";

                string sanitized = SanitizeTextureName(sourceName, i);
                string uniqueName = Utils.RenameDuplicateString(usedNames, sanitized);
                usedNames.Add(uniqueName);

                map[sourceName] = uniqueName;
                if (!map.ContainsKey(sanitized))
                    map[sanitized] = uniqueName;

                if (texture == null)
                    continue;

                try
                {
                    Bitmap bitmap = texture.GetBitmap();
                    if (bitmap == null)
                        continue;

                    Bitmap exportBitmap = bitmap;
                    if (settings.UseTextureChannelComponents)
                        exportBitmap = texture.GetComponentBitmap(bitmap);

                    string outputPath = Path.Combine(textureFolder, $"{uniqueName}.png");
                    exportBitmap.Save(outputPath);

                    if (!ReferenceEquals(exportBitmap, bitmap))
                        exportBitmap.Dispose();
                    bitmap.Dispose();
                }
                catch
                {
                    // Continue exporting remaining textures/materials.
                }
            }

            return map;
        }

        private static string ResolveTextureExportName(
            string sourceName,
            Dictionary<string, string> exportedTextureNames,
            List<string> usedTextureNames)
        {
            if (exportedTextureNames.TryGetValue(sourceName, out string mappedName))
                return mappedName;

            string sanitized = SanitizeTextureName(sourceName, 0);
            if (exportedTextureNames.TryGetValue(sanitized, out mappedName))
                return mappedName;

            string unique = Utils.RenameDuplicateString(usedTextureNames, sanitized);
            if (!usedTextureNames.Contains(unique))
                usedTextureNames.Add(unique);
            exportedTextureNames[sourceName] = unique;
            return unique;
        }

        private static string SanitizeTextureName(string sourceName, int fallbackIndex)
        {
            if (string.IsNullOrEmpty(sourceName))
                return $"Texture_{fallbackIndex}";

            string name = sourceName.RemoveIllegaleFileNameCharacters();
            return string.IsNullOrEmpty(name) ? $"Texture_{fallbackIndex}" : name;
        }

        private static MaterialTextureType? ConvertTextureType(STGenericMatTexture.TextureType type)
        {
            switch (type)
            {
                case STGenericMatTexture.TextureType.Diffuse:
                    return MaterialTextureType.Diffuse;
                case STGenericMatTexture.TextureType.Normal:
                    return MaterialTextureType.Normal;
                case STGenericMatTexture.TextureType.Specular:
                    return MaterialTextureType.Specular;
                case STGenericMatTexture.TextureType.Emission:
                    return MaterialTextureType.Emission;
                case STGenericMatTexture.TextureType.Transparency:
                    return MaterialTextureType.Alpha;
                default:
                    return null;
            }
        }

        private static int GetGlobalBoneIndex(int localID, STGenericObject mesh, int[] IndexTable, STSkeleton skeleton)
        {
            if (skeleton == null)
                return localID;

            if (mesh.boneList != null && mesh.boneList.Count > 0 && localID >= 0 && localID < mesh.boneList.Count)
                return skeleton.boneIndex(mesh.boneList[localID]);
            if (IndexTable != null && localID >= 0 && localID < IndexTable.Length)
                return IndexTable[localID];
            return localID;
        }

        private static double[] MatrixToDoubleArray(Matrix4 m)
        {
            return new double[] {
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44
            };
        }
    }
}
