using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using Toolbox.Library.IO;

namespace Toolbox.Library.GLTFModel
{
    public class GLTFExporter
    {
        private SceneBuilder Scene;
        private MaterialBuilder DefaultMaterial;
        private Dictionary<string, ImageBuilder> TextureMaps;
        private List<MaterialBuilder> Materials;
        private List<NodeBuilder> Nodes;
        private int ArmatureCount;

        public GLTFExporter()
        {
            Scene = new SceneBuilder();
            DefaultMaterial = new MaterialBuilder("Default");
            TextureMaps = new Dictionary<string, ImageBuilder>();
            Materials = new List<MaterialBuilder>();
            Nodes = new List<NodeBuilder>();
            ArmatureCount = 0;
        }

        private MaterialBuilder GetMaterial(int MaterialIndex)
        {
            if (MaterialIndex >= 0 && MaterialIndex < Materials.Count)
                return Materials[MaterialIndex];
            return DefaultMaterial;
        }

        public void AddTextureMap(string textureName, Bitmap bitmap)
        {
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            byte[] bytes = ms.ToArray();
            ImageBuilder image = ImageBuilder.From(bytes);
            image.AlternateWriteFileName = textureName.RemoveIllegaleFileNameCharacters() + ".png";
            TextureMaps.Add(textureName, image);
        }

        public void AddMaterial(STGenericMaterial mat)
        {
            string materialName = mat.Text;
            MaterialBuilder material = new MaterialBuilder(materialName)
                .WithMetallicRoughnessShader();

            foreach (var tex in mat.TextureMaps)
            {
                ImageBuilder image = TextureMaps[tex.Name];
                switch (tex.Type)
                {
                    case STGenericMatTexture.TextureType.Diffuse:
                        material = material.WithChannelImage(KnownChannel.BaseColor, image);
                        break;
                    case STGenericMatTexture.TextureType.Normal:
                        material = material.WithChannelImage(KnownChannel.Normal, image);
                        break;
                }
            }    

            Materials.Add(material);
        }

        public void AddNode(string nodeName, Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            NodeBuilder node = new NodeBuilder(nodeName);
            node.WithLocalTranslation(translation);
            node.WithLocalRotation(rotation);
            node.WithLocalScale(scale);
            Nodes.Add(node);
        }

        public void ParentNode(int nodeId, int parentId)
        {
            if (nodeId < 0 || nodeId >= Nodes.Count || parentId >= Nodes.Count) return;
            if (parentId < 0)
            {
                // Organize skeleton under armature node and add to scene
                string armatureName = "Armature";
                if (ArmatureCount > 0) armatureName += ArmatureCount++;

                NodeBuilder node = new NodeBuilder(armatureName);
                node.AddNode(Nodes[nodeId]);
                
                Scene.AddNode(node);
                return;
            }
            Nodes[parentId].AddNode(Nodes[nodeId]);
        }

        public void AddMesh(string meshName, List<Vector3> Vertices, List<int> TriangleFaces, int MaterialIndex)
        {
            var mesh = new MeshBuilder<VertexPosition>(meshName);
            var prim = mesh.UsePrimitive(GetMaterial(MaterialIndex));

            if (TriangleFaces.Count % 3 != 0)
                throw new Exception("Expected a multiple of 3!");

            for (int i = 0; i < TriangleFaces.Count; i += 3)
            {
                var v0 = new VertexPosition(Vertices[TriangleFaces[i]]);
                var v1 = new VertexPosition(Vertices[TriangleFaces[i + 1]]);
                var v2 = new VertexPosition(Vertices[TriangleFaces[i + 2]]);

                prim.AddTriangle(v0, v1, v2);
            }

            Scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        }

        public void AddMeshNormal(string meshName, List<Vector3> Vertices, List<Vector3> Normals, List<int> TriangleFaces, int MaterialIndex)
        {
            var mesh = new MeshBuilder<VertexPositionNormal>(meshName);
            var prim = mesh.UsePrimitive(GetMaterial(MaterialIndex));

            if (TriangleFaces.Count % 3 != 0)
                throw new Exception("Expected a multiple of 3!");

            for (int i = 0; i < TriangleFaces.Count; i += 3)
            {
                var v0 = new VertexPositionNormal(
                    Vertices[TriangleFaces[i]],
                    Normals[TriangleFaces[i]]);
                var v1 = new VertexPositionNormal(
                    Vertices[TriangleFaces[i + 1]],
                    Normals[TriangleFaces[i + 1]]);
                var v2 = new VertexPositionNormal(
                    Vertices[TriangleFaces[i + 2]],
                    Normals[TriangleFaces[i + 2]]);

                prim.AddTriangle(v0, v1, v2);
            }

            Scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        }

        public void AddMeshNormalUV1(string meshName, List<Vector3> Vertices, List<Vector3> Normals, List<Vector2> UV0, List<int> TriangleFaces, int MaterialIndex)
        {
            var mesh = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(meshName);
            var prim = mesh.UsePrimitive(GetMaterial(MaterialIndex));

            if (TriangleFaces.Count % 3 != 0)
                throw new Exception("Expected a multiple of 3!");

            for (int i = 0; i < TriangleFaces.Count; i += 3)
            {
                var v0 = new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>();
                v0.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i]], Normals[TriangleFaces[i]]);
                v0.Material = new VertexTexture1(UV0[TriangleFaces[i]]);
                var v1 = new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>();
                v1.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i+1]], Normals[TriangleFaces[i+1]]);
                v1.Material = new VertexTexture1(UV0[TriangleFaces[i+1]]);
                var v2 = new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>();
                v2.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i+2]], Normals[TriangleFaces[i+2]]);
                v2.Material = new VertexTexture1(UV0[TriangleFaces[i+2]]);

                prim.AddTriangle(v0, v1, v2);
            }

            Scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        }

        public void AddMeshNormalColor1(string meshName, List<Vector3> Vertices, List<Vector3> Normals, List<Vector4> Colors0, List<int> TriangleFaces, int MaterialIndex)
        {
            var mesh = new MeshBuilder<VertexPositionNormal, VertexColor1, VertexEmpty>(meshName);
            var prim = mesh.UsePrimitive(GetMaterial(MaterialIndex));

            if (TriangleFaces.Count % 3 != 0)
                throw new Exception("Expected a multiple of 3!");

            for (int i = 0; i < TriangleFaces.Count; i += 3)
            {
                var v0 = new VertexBuilder<VertexPositionNormal, VertexColor1, VertexEmpty>();
                v0.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i]], Normals[TriangleFaces[i]]);
                v0.Material = new VertexColor1(Colors0[TriangleFaces[i]]);
                var v1 = new VertexBuilder<VertexPositionNormal, VertexColor1, VertexEmpty>();
                v1.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 1]], Normals[TriangleFaces[i + 1]]);
                v1.Material = new VertexColor1(Colors0[TriangleFaces[i + 1]]);
                var v2 = new VertexBuilder<VertexPositionNormal, VertexColor1, VertexEmpty>();
                v2.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 2]], Normals[TriangleFaces[i + 2]]);
                v2.Material = new VertexColor1(Colors0[TriangleFaces[i + 2]]);

                prim.AddTriangle(v0, v1, v2);
            }

            Scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        }

        public void AddMeshNormalUV1Color1(string meshName, List<Vector3> Vertices, List<Vector3> Normals, List<Vector2> UV0, List<Vector4> Colors0, List<int> TriangleFaces, int MaterialIndex)
        {
            var mesh = new MeshBuilder<VertexPositionNormal, VertexColor1Texture1, VertexEmpty>(meshName);
            var prim = mesh.UsePrimitive(GetMaterial(MaterialIndex));

            if (TriangleFaces.Count % 3 != 0)
                throw new Exception("Expected a multiple of 3!");

            for (int i = 0; i < TriangleFaces.Count; i += 3)
            {
                var v0 = new VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexEmpty>();
                v0.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i]], Normals[TriangleFaces[i]]);
                v0.Material = new VertexColor1Texture1(Colors0[TriangleFaces[i]], UV0[TriangleFaces[i]]);
                var v1 = new VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexEmpty>();
                v1.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 1]], Normals[TriangleFaces[i + 1]]);
                v1.Material = new VertexColor1Texture1(Colors0[TriangleFaces[i + 1]], UV0[TriangleFaces[i + 1]]);
                var v2 = new VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexEmpty>();
                v2.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 2]], Normals[TriangleFaces[i + 2]]);
                v2.Material = new VertexColor1Texture1(Colors0[TriangleFaces[i + 2]], UV0[TriangleFaces[i + 2]]);

                prim.AddTriangle(v0, v1, v2);
            }

            Scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        }

        public void Write(string FileName)
        {
            var model = Scene.ToGltf2();
            model.SaveGLTF(FileName);
        }
    }
}
