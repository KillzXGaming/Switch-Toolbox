using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.GLTFModel;
using static Toolbox.Library.STGenericObject;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class GLTF
    {
        public class ExportSettings
        {
            public bool UseVertexColors = true;
            public bool FlipTexCoordsVertical = false;
            public bool UseTextureChannelComponents = true;
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
                    bitmap.Dispose();
                    GC.Collect();
                }
                catch (Exception ex)
                {

                }
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
                List<Vector2> UV0 = new List<Vector2>();
                List<Vector4> Colors0 = new List<Vector4>();
                List<int> TriangleFaces = new List<int>();

                bool HasNormals = false;
                bool HasColors = false;
                bool HasColors2 = false;
                bool HasColors3 = false;
                bool HasColors4 = false;
                bool HasUV0 = false;
                bool HasUV1 = false;
                bool HasUV2 = false;
                bool HasUV3 = false;
                bool HasBoneIds = false;

                foreach (var vertex in mesh.vertices)
                {
                    if (vertex.nrm != OpenTK.Vector3.Zero) HasNormals = true;
                    if (settings.UseVertexColors) HasColors = true;
                    if (vertex.col2 != OpenTK.Vector4.One && settings.UseVertexColors) HasColors2 = true;
                    if (vertex.col3 != OpenTK.Vector4.One && settings.UseVertexColors) HasColors3 = true;
                    if (vertex.col4 != OpenTK.Vector4.One && settings.UseVertexColors) HasColors4 = true;

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
                    }
                    else
                    {
                        UV0.Add(new Vector2(vertex.uv0.X, vertex.uv0.Y));
                    }

                    Colors0.Add(new Vector4(vertex.col.X, vertex.col.Y, vertex.col.Z, vertex.col.W));

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

                // TODO: Determine which type of export vertices to use
                //Exporter.AddMeshNormal(Vertices, Normals, TriangleFaces, mesh.MaterialIndex);
                Exporter.AddMeshNormalUV1(meshName, Vertices, Normals, UV0, TriangleFaces, mesh.MaterialIndex);
                //Exporter.AddMeshNormalUV1Color1(meshName, Vertices, Normals, UV0, Colors0, TriangleFaces, mesh.MaterialIndex);
            }

            Exporter.Write(FileName);
        }
    }
}
