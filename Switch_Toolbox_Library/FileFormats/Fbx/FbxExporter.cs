using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using Toolbox.Library.Rendering;
using System.Windows.Forms;

namespace Toolbox.Library.FBX
{
    public class FbxExporter
    {
        public static void Export(string FileName, DAE.ExportSettings settings, STGenericModel model, List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            Export(FileName, settings, model.Objects.ToList(), model.Materials.ToList(), Textures, skeleton, NodeArray);
        }

        public static void Export(string FileName, DAE.ExportSettings settings,
           List<STGenericObject> Meshes, List<STGenericMaterial> Materials,
           List<STGenericTexture> Textures, STSkeleton skeleton = null, List<int> NodeArray = null)
        {
            if (Materials == null) Materials = new List<STGenericMaterial>();
            if (skeleton != null && skeleton.BoneIndices != null)
                NodeArray = skeleton.BoneIndices.ToList();

            // Initialize Context
            FbxNativeWrapper.Exp_CreateContext();

            if (!FbxNativeWrapper.Exp_Initialize(FileName))
            {
                MessageBox.Show("Failed to initialize FBX Exporter!");
                FbxNativeWrapper.Exp_DestroyContext();
                return;
            }

            // Export Materials
            if (settings.ExportTextures)
            {
                foreach (var mat in Materials)
                {
                     FbxNativeWrapper.Exp_AddMaterial(mat.Text, 1.0, 1.0, 1.0);
                }
            }

            // Export Skeleton
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
                    if (bone.parentIndex != -1 && bone.parentIndex < skeleton.bones.Count) {
                        parentName = skeleton.bones[bone.parentIndex].Text;
                    }

                    FbxNativeWrapper.Exp_CreateNode(
                        bone.Text,
                        parentName,
                        MatrixToDoubleArray(bone.GetTransform())
                    );
                }
            }

            // Export Meshes
            STProgressBar progressBar = new STProgressBar();
            progressBar.Task = "Exporting FBX Meshes...";
            progressBar.Value = 0;
            progressBar.StartPosition = FormStartPosition.CenterScreen;
            progressBar.Show();
            
            int meshIndex = 0;
            foreach (var mesh in Meshes)
            {
                progressBar.Task = $"Exporting Mesh {mesh.Text}";
                progressBar.Value = ((meshIndex++ * 100) / Meshes.Count);
                progressBar.Refresh();

                FbxNativeWrapper.Exp_StartMesh(mesh.Text);
                
                if (settings.ExportTextures && mesh.MaterialIndex != -1 && mesh.MaterialIndex < Materials.Count) {
                   FbxNativeWrapper.Exp_SetMeshMaterial(Materials[mesh.MaterialIndex].Text);
                }

                // Setup Bind Pose Matrices for this mesh
                if (skeleton != null)
                {
                    foreach (var bone in skeleton.bones)
                    {
                        var meshMatrix = Matrix4.Identity; 
                        var linkMatrix = skeleton.GetBoneTransform(bone);

                        FbxNativeWrapper.Exp_SetClusterMatrices(mesh.Text, bone.Text, MatrixToDoubleArray(meshMatrix), MatrixToDoubleArray(linkMatrix));
                    }
                }
                
                // Process Vertices
                    int[] IndexTable = null;
                    if (NodeArray != null)
                        IndexTable = NodeArray.ToArray();

                foreach (var v in mesh.vertices)
                {
                     // Remap Bones
                     int[] bIndices = new int[4] { -1, -1, -1, -1 };
                     double[] bWeights = new double[4] { 0, 0, 0, 0 };

                     double totalWeight = 0;
                     for(int b=0; b < v.boneIds.Count && b < 4; b++)
                     {
                         int id = v.boneIds[b];
                         float weight = (b < v.boneWeights.Count) ? v.boneWeights[b] : 0;

                         if (weight <= 0) continue;

                         int globalID = GetGlobalBoneIndex(id, mesh, IndexTable, skeleton);
                         
                         if (globalID != -1) {
                            bIndices[b] = globalID;
                            bWeights[b] = (double)weight;
                            totalWeight += bWeights[b];
                         }
                     }

                     // If weight sum is 0 but indices exist, default to 1.0 on the first bone
                     // This prevents "Skin Count 0" issues on rigid/optimized meshes
                     if (totalWeight <= 0 && v.boneIds.Count > 0) {
                        int globalID = GetGlobalBoneIndex(v.boneIds[0], mesh, IndexTable, skeleton);
                        if (globalID != -1) {
                            bIndices[0] = globalID;
                            bWeights[0] = 1.0;
                            totalWeight = 1.0;
                        }
                     }

                     // Handle Rigid Skinning from Mesh properties if no vertex weights present
                     if (totalWeight <= 0 && mesh.BoneIndex != -1) {
                        bIndices[0] = GetGlobalBoneIndex(mesh.BoneIndex, mesh, IndexTable, skeleton);
                        bWeights[0] = 1.0;
                        totalWeight = 1.0;
                     }

                     // Normalize weights to ensure they sum to 1.0
                     if (totalWeight > 0 && Math.Abs(totalWeight - 1.0) > 0.001) {
                         for (int b = 0; b < 4; b++)
                             bWeights[b] /= totalWeight;
                     }

                    float vUV = settings.FlipTexCoordsVertical ? (1.0f - v.uv0.Y) : v.uv0.Y;
                    var vCol = settings.UseVertexColors ? v.col : OpenTK.Vector4.One;

                    FbxNativeWrapper.Exp_AddVertex(
                        (double)v.pos.X, (double)v.pos.Y, (double)v.pos.Z,
                        (double)v.nrm.X, (double)v.nrm.Y, (double)v.nrm.Z,
                        (double)v.uv0.X, (double)vUV, 
                        (double)vCol.X, (double)vCol.Y, (double)vCol.Z, (double)vCol.W,
                        bIndices[0], bIndices[1], bIndices[2], bIndices[3],
                        bWeights[0], bWeights[1], bWeights[2], bWeights[3]
                    );
                }

                // Indices
                List<int> faces = new List<int>();

                 if (mesh.lodMeshes.Count > 0)
                {
                    var lodMesh = mesh.lodMeshes[mesh.DisplayLODIndex]; // Use Display LOD
                     if (lodMesh.PrimativeType == STPrimitiveType.TrangleStrips)
                            faces = STGenericObject.ConvertTriangleStripsToTriangles(lodMesh.faces);
                        else
                            faces = lodMesh.faces;
                }
                else if (mesh.PolygonGroups.Count > 0) {
                     foreach(var group in mesh.PolygonGroups) {
                         List<int> subFaces;
                         if (group.PrimativeType == STPrimitiveType.TrangleStrips)
                                subFaces = STGenericObject.ConvertTriangleStripsToTriangles(group.faces);
                            else
                                subFaces = group.faces;
                         faces.AddRange(subFaces);
                     }
                }

                foreach(int idx in faces) {
                    FbxNativeWrapper.Exp_AddIndex(idx);
                }

                FbxNativeWrapper.Exp_EndMesh();
            }
            
            progressBar.Close();

            if (skeleton != null)
                FbxNativeWrapper.Exp_AddPose("Bind Pose");

            bool result = FbxNativeWrapper.Exp_Save();
            FbxNativeWrapper.Exp_DestroyContext();

            if (result && !settings.SuppressConfirmDialog) {
                MessageBox.Show($"Exported {FileName} Successfully!");
            }
        }

        private static int GetGlobalBoneIndex(int localID, STGenericObject mesh, int[] IndexTable, STSkeleton skeleton)
        {
            if (skeleton == null) return localID;

            if (mesh.boneList != null && mesh.boneList.Count > 0 && localID >= 0 && localID < mesh.boneList.Count)
            {
                return skeleton.boneIndex(mesh.boneList[localID]);
            }
            if (IndexTable != null && localID >= 0 && localID < IndexTable.Length)
            {
                return IndexTable[localID];
            }
            return localID;
        }

        private static double[] MatrixToDoubleArray(Matrix4 m)
        {
            return new double[] {
                (double)m.M11, (double)m.M12, (double)m.M13, (double)m.M14,
                (double)m.M21, (double)m.M22, (double)m.M23, (double)m.M24,
                (double)m.M31, (double)m.M32, (double)m.M33, (double)m.M34,
                (double)m.M41, (double)m.M42, (double)m.M43, (double)m.M44
            };
        }
    }
}

