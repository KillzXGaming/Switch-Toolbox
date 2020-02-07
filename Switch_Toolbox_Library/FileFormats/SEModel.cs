using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SELib;
using Toolbox.Library.Rendering;
using OpenTK;

namespace Toolbox.Library
{
    public class SEModel
    {
        public List<STGenericObject> objects = new List<STGenericObject>();
        public List<STGenericMaterial> materials = new List<STGenericMaterial>();
        public STSkeleton skeleton;

        public static void Save(string FileName, List<STGenericObject> Meshes, List<STGenericMaterial> Materials, List<STBone> bones)
        {
            SELib.SEModel seModel = new SELib.SEModel();

            for (int i = 0; i < Meshes.Count; i++)
                seModel.AddMesh(SaveMesh(Meshes[i]));

            for (int i = 0; i < Materials.Count; i++)
                seModel.AddMaterial(SaveMaterial(Materials[i]));

        //    for (int i = 0; i < bones.Count; i++)
        //        seModel.AddBone(SaveBone(bones[i]));
        }

        private static SELib.SEModelMesh SaveMesh(STGenericObject mesh)
        {
            var seMesh = new SELib.SEModelMesh();

            var MeshLevel = mesh.lodMeshes[mesh.DisplayLODIndex];
            for (int i = 0; i < MeshLevel.faces.Count; i++)
            {
                seMesh.AddFace((uint)MeshLevel.faces[i], (uint)MeshLevel.faces[i++], (uint)MeshLevel.faces[i++]);
            }
            return seMesh;
        }

        private static SELib.SEModelMaterial SaveMaterial(STGenericMaterial material)
        {
            var seMaterial = new SELib.SEModelMaterial();
            return seMaterial;
        }

        private static SELib.SEModelBone SaveBone(STBone bone)
        {
            var seBone = new SELib.SEModelBone();
            return seBone;
        }

        public void CreateGenericModel(string FileName)
        {
            skeleton = new STSkeleton();

            var seModel = SELib.SEModel.Read(FileName);
            for (int i = 0; i < seModel.MeshCount; i++)
                objects.Add(CreateGenericObject(seModel, seModel.Meshes[i]));

            for (int i = 0; i < seModel.MaterialCount; i++)
                materials.Add(CreateGenericMaterial(seModel, seModel.Materials[i]));

            for (int i = 0; i < seModel.BoneCount; i++)
                skeleton.bones.Add(CreateGenericBone(seModel, seModel.Bones[i]));

        }

        public STBone CreateGenericBone(SELib.SEModel seModel, SELib.SEModelBone seBone)
        {
            STBone bone = new STBone(skeleton);
            bone.Text = seBone.BoneName;
            bone.parentIndex = seBone.BoneParent;
            bone.RotationType = STBone.BoneRotationType.Euler;
           
            bone.Position = new Vector3(
                (float)seBone.LocalPosition.X,
                (float)seBone.LocalPosition.Y, 
                (float)seBone.LocalPosition.Z);
            bone.Scale = new Vector3(
                (float)seBone.Scale.X, 
                (float)seBone.Scale.Y,
                (float)seBone.Scale.Z);
            bone.Rotation = new Quaternion(
                (float)seBone.LocalRotation.X,
                (float)seBone.LocalRotation.Y,
                (float)seBone.LocalRotation.Z, 
                (float)seBone.LocalRotation.W);

            return bone;
        }

        public STGenericMaterial CreateGenericMaterial(SELib.SEModel seModel, SELib.SEModelMaterial seMaterial)
        {
            STGenericMaterial material = new STGenericMaterial();
            material.Text = seMaterial.Name;
            if (seMaterial.MaterialData is SEModelSimpleMaterial)
            {
                if (((SEModelSimpleMaterial)seMaterial.MaterialData).DiffuseMap != string.Empty)
                {
                    string TextureName = ((SEModelSimpleMaterial)seMaterial.MaterialData).DiffuseMap;
                    material.TextureMaps.Add(new STGenericMatTexture()
                    {
                        Name = TextureName,
                        Type = STGenericMatTexture.TextureType.Diffuse
                    });
                }
                if (((SEModelSimpleMaterial)seMaterial.MaterialData).NormalMap != string.Empty)
                {
                    string TextureName = ((SEModelSimpleMaterial)seMaterial.MaterialData).NormalMap;
                    material.TextureMaps.Add(new STGenericMatTexture()
                    {
                        Name = TextureName,
                        Type = STGenericMatTexture.TextureType.Normal
                    });
                }
                if (((SEModelSimpleMaterial)seMaterial.MaterialData).SpecularMap != string.Empty)
                {
                    string TextureName = ((SEModelSimpleMaterial)seMaterial.MaterialData).SpecularMap;
                    material.TextureMaps.Add(new STGenericMatTexture()
                    {
                        Name = TextureName,
                        Type = STGenericMatTexture.TextureType.Specular
                    });
                }
            }
            return material;
        }

        public STGenericObject CreateGenericObject(SELib.SEModel seModel, SELib.SEModelMesh seMesh)
        {
            int Index = seModel.Meshes.IndexOf(seMesh);

            STGenericObject mesh = new STGenericObject();
            mesh.ObjectName = $"Mesh_{Index}";
            if (seMesh.MaterialReferenceIndicies.Count > 0)
                mesh.MaterialIndex = seMesh.MaterialReferenceIndicies[0];

            mesh.HasPos = true;
            for (int v = 0; v < seMesh.VertexCount; v++)
            {
                if (seMesh.Verticies[v].UVSets.Count > 0)
                    mesh.HasUv0 = true;
                if (seMesh.Verticies[v].Weights.Count > 0)
                {
                    mesh.HasIndices = true;
                    for (int w = 0; w < seMesh.Verticies[v].WeightCount; w++)
                    {
                        if (seMesh.Verticies[v].Weights[w].BoneWeight != 0)
                            mesh.HasWeights = true;
                    }
                }
                if (seMesh.Verticies[v].VertexColor != SELib.Utilities.Color.White)
                    mesh.HasVertColors = true;
                if (seMesh.Verticies[v].VertexNormal != SELib.Utilities.Vector3.Zero)
                    mesh.HasNrm = true;

                Vertex vertex = new Vertex();
                mesh.vertices.Add(vertex);

                vertex.pos = ToTKVector3(seMesh.Verticies[v].Position);
                vertex.nrm = ToTKVector3(seMesh.Verticies[v].VertexNormal);
                vertex.col = ToTKVector4(seMesh.Verticies[v].VertexColor);

                for (int u = 0; u < seMesh.Verticies[v].UVSetCount; u++)
                {
                    if (u == 0)
                        vertex.uv0 = ToTKVector2(seMesh.Verticies[v].UVSets[u]);
                    if (u == 1)
                        vertex.uv1 = ToTKVector2(seMesh.Verticies[v].UVSets[u]);
                    if (u == 2)
                        vertex.uv2 = ToTKVector2(seMesh.Verticies[v].UVSets[u]);
                }

                for (int w = 0; w < seMesh.Verticies[v].WeightCount; w++)
                {
                    //Get the bone name from the index. Indices for formats get set after the importer
                    string BoneName = seModel.Bones[(int)seMesh.Verticies[v].Weights[w].BoneIndex].BoneName;
                    float BoneWeight = seMesh.Verticies[v].Weights[w].BoneWeight;

                    vertex.boneNames.Add(BoneName);
                    vertex.boneWeights.Add(BoneWeight);
                }
            }

            

            mesh.lodMeshes = new List<STGenericObject.LOD_Mesh>();
            var lodMesh = new STGenericObject.LOD_Mesh();
            lodMesh.PrimativeType = STPrimitiveType.Triangles;
            mesh.lodMeshes.Add(lodMesh);
            for (int f = 0; f < seMesh.FaceCount; f++)
            {
                lodMesh.faces.Add((int)seMesh.Faces[f].FaceIndex1);
                lodMesh.faces.Add((int)seMesh.Faces[f].FaceIndex2);
                lodMesh.faces.Add((int)seMesh.Faces[f].FaceIndex3);
            }

            return mesh;
        }

        private Vector2 ToTKVector2(SELib.Utilities.Vector2 value) {
            return new Vector2((float)value.X, (float)value.Y);
        }

        private Vector3 ToTKVector3(SELib.Utilities.Vector3 value) {
            return new Vector3((float)value.X, (float)value.Y, (float)value.Z);
        }

        private Vector4 ToTKVector4(SELib.Utilities.Color value) {
            return new Vector4((float)(value.R / 255),
                               (float)(value.G / 255),
                               (float)(value.B / 255),
                               (float)(value.A / 255));
        }
    }
}
