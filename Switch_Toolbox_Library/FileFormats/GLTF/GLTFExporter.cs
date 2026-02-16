using Octokit;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using SharpGLTF.Transforms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace Toolbox.Library.GLTFModel
{
    public class GLTFExporter
    {
        private SceneBuilder Scene;
        private MaterialBuilder DefaultMaterial;
        private Dictionary<string, Bitmap> TextureMaps;
        private List<MaterialBuilder> Materials;
        private List<NodeBuilder> Nodes;
        private int ArmatureCount;

        public GLTFExporter()
        {
            Scene = new SceneBuilder();
            DefaultMaterial = new MaterialBuilder("Default");
            TextureMaps = new Dictionary<string, Bitmap>();
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
            TextureMaps.Add(textureName, bitmap);
        }

        private static Bitmap MergeDiffuseAndOpacity(Bitmap diffuse, Bitmap opacity)
        {
            int width = diffuse.Width;
            int height = diffuse.Height;

            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            var rect = new Rectangle(0, 0, width, height);

            var diffuseData = diffuse.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var opacityData = opacity.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var resultData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int bytes = diffuseData.Stride * height;

            byte[] diffuseBuffer = new byte[bytes];
            byte[] opacityBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];

            Marshal.Copy(diffuseData.Scan0, diffuseBuffer, 0, bytes);
            Marshal.Copy(opacityData.Scan0, opacityBuffer, 0, bytes);

            for (int i = 0; i < bytes; i += 4)
            {
                resultBuffer[i + 0] = diffuseBuffer[i + 0]; // B
                resultBuffer[i + 1] = diffuseBuffer[i + 1]; // G
                resultBuffer[i + 2] = diffuseBuffer[i + 2]; // R

                resultBuffer[i + 3] = opacityBuffer[i + 2]; // A
            }

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, bytes);

            diffuse.UnlockBits(diffuseData);
            opacity.UnlockBits(opacityData);
            result.UnlockBits(resultData);

            return result;
        }


        public static unsafe Bitmap Swizzle_Mix_to_ARM(Bitmap source)
        {
            Bitmap result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);

            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);

            BitmapData srcData = source.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData dstData = result.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int bytes = Math.Abs(srcData.Stride) * source.Height;

            byte* srcPtr = (byte*)srcData.Scan0;
            byte* dstPtr = (byte*)dstData.Scan0;

            for (int i = 0; i < bytes; i += 4)
            {
                byte b = srcPtr[i + 0];
                byte g = srcPtr[i + 1];
                byte r = srcPtr[i + 2];
                byte a = srcPtr[i + 3];

                dstPtr[i + 0] = r; // B = Metallic
                dstPtr[i + 1] = b; // G = Roughness
                dstPtr[i + 2] = g; // R = AO
                dstPtr[i + 3] = a; // preserve alpha
            }

            source.UnlockBits(srcData);
            result.UnlockBits(dstData);

            return result;
        }

        private static ImageBuilder BitmapToImageBuilder(string textureName, Bitmap bitmap)
        {
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            byte[] bytes = ms.ToArray();
            ImageBuilder image = ImageBuilder.From(bytes);
            image.AlternateWriteFileName = textureName.RemoveIllegaleFileNameCharacters() + ".png";
            return image;
        }

        public void AddMaterial(STGenericMaterial mat)
        {
            string materialName = mat.Text;
            MaterialBuilder material = new MaterialBuilder(materialName)
                .WithMetallicRoughnessShader();

            string diffuseTexture = null;
            string opacityTexture = null;
            string aoTexture = null;
            string metallicTexture = null;
            string roughnessTexture = null;

            foreach (var tex in mat.TextureMaps)
            {
                try
                {
                    string textureName = tex.Name;
                    Bitmap bitmap = TextureMaps[textureName];

                    switch (tex.Type)
                    {
                        case STGenericMatTexture.TextureType.Diffuse:
                            diffuseTexture = textureName;
                            break;
                        case STGenericMatTexture.TextureType.Normal:
                            material = material.WithChannelImage(KnownChannel.Normal, BitmapToImageBuilder(textureName, bitmap));
                            break;
                        case STGenericMatTexture.TextureType.AO:
                            aoTexture = textureName;
                            break;
                        case STGenericMatTexture.TextureType.Emission:
                            material = material.WithChannelImage(KnownChannel.Emissive, BitmapToImageBuilder(textureName, bitmap));
                            break;
                        case STGenericMatTexture.TextureType.Metalness:
                            metallicTexture = textureName;
                            break;
                        case STGenericMatTexture.TextureType.Roughness:
                            roughnessTexture = textureName;
                            break;
                        case STGenericMatTexture.TextureType.Opacity:
                            opacityTexture = textureName;
                            break;
                        case STGenericMatTexture.TextureType.Mix:
                            // Need more research on ACNH to understand channel rules
                            Bitmap ARM = Swizzle_Mix_to_ARM(bitmap);
                            material = material
                                .WithChannelImage(KnownChannel.Occlusion, BitmapToImageBuilder(textureName, ARM))
                                .WithChannelImage(KnownChannel.MetallicRoughness, BitmapToImageBuilder(textureName, ARM));
                            break;
                        default:
                            // Unknown use
                            break;
                    }
                }
                catch (Exception e) {}
            }

            if (diffuseTexture != null)
            {
                Bitmap diffuseBitmap = TextureMaps[diffuseTexture];
                if (opacityTexture != null)
                {
                    Bitmap opacityBitmap = TextureMaps[opacityTexture];
                    Bitmap basecolorBitmap = MergeDiffuseAndOpacity(diffuseBitmap, opacityBitmap);
                    material = material.WithChannelImage(KnownChannel.BaseColor, BitmapToImageBuilder(diffuseTexture, basecolorBitmap));
                }
                else
                {
                    material = material.WithChannelImage(KnownChannel.BaseColor, BitmapToImageBuilder(diffuseTexture, diffuseBitmap));
                }
            }

            // TODO: Combine AO, Roughness, and Metalness and assign to material

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

        public void AddMesh(string meshName, int materialIndex,
            List<Vector3> Vertices, List<Vector3> Normals,
            List<Vector4> Colors0, List<Vector4> Colors1,
            List<Vector2> UV0, List<Vector2> UV1, List<Vector2> UV2, List<Vector2> UV3,            
            List<SparseWeight8> Skins,
            List<int> TriangleFaces,
            int ColorsCount, int UVCount, int SkinsCount)
        {
            if (ColorsCount == 0 && UVCount == 0 && SkinsCount == 0)
                AddMeshNormal(meshName, materialIndex, Vertices, Normals, TriangleFaces);
            else if (ColorsCount == 0 && UVCount <= 1 && SkinsCount == 0)
                AddMeshNormalUV1(meshName, materialIndex, Vertices,
                    Normals, UV0, TriangleFaces);
            else if (ColorsCount <= 1 && UVCount <= 1 && SkinsCount == 0)
                AddMeshNormalColor1UV1(meshName, materialIndex, Vertices,
                    Normals, Colors0, UV0, TriangleFaces);
            else if (ColorsCount == 0 && UVCount <= 1 && SkinsCount == 4)
                AddMeshNormalUV1Skin4(meshName, materialIndex, Vertices,
                    Normals, UV0, Skins, TriangleFaces);
            else if (ColorsCount <= 1 && UVCount <= 1 && SkinsCount <= 4)
                AddMeshNormalColor1UV1Skin4(meshName, materialIndex, Vertices,
                    Normals, Colors0, UV0, Skins, TriangleFaces);
            else
                AddMeshNormalColor2UV4Skin8(meshName, materialIndex, Vertices, 
                    Normals, Colors0, Colors1, UV0, UV1, UV2, UV3, Skins,
                    TriangleFaces);
        }

        public void AddMeshNormal(string meshName, int materialIndex,
            List<Vector3> Vertices, List<Vector3> Normals,
            List<int> TriangleFaces)
        {
            var mesh = new MeshBuilder<VertexPositionNormal, VertexEmpty, VertexEmpty>(meshName);
            var prim = mesh.UsePrimitive(GetMaterial(materialIndex));

            if (TriangleFaces.Count % 3 != 0)
                throw new Exception("Expected a multiple of 3!");

            for (int i = 0; i < TriangleFaces.Count; i += 3)
            {
                var v0 = new VertexBuilder<VertexPositionNormal, VertexEmpty, VertexEmpty>();
                var v1 = new VertexBuilder<VertexPositionNormal, VertexEmpty, VertexEmpty>();
                var v2 = new VertexBuilder<VertexPositionNormal, VertexEmpty, VertexEmpty>();
                v0.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i]], Normals[TriangleFaces[i]]);
                v1.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 1]], Normals[TriangleFaces[i + 1]]);
                v2.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 2]], Normals[TriangleFaces[i + 2]]);
                
                prim.AddTriangle(v0, v1, v2);
            }

            Scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        }

        public void AddMeshNormalUV1(string meshName, int materialIndex, 
            List<Vector3> Vertices, List<Vector3> Normals, 
            List<Vector2> UV0, 
            List<int> TriangleFaces)
        {
            var mesh = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(meshName);
            var prim = mesh.UsePrimitive(GetMaterial(materialIndex));

            if (TriangleFaces.Count % 3 != 0)
                throw new Exception("Expected a multiple of 3!");

            for (int i = 0; i < TriangleFaces.Count; i += 3)
            {
                var v0 = new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>();
                var v1 = new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>();
                var v2 = new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>();
                v0.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i]], Normals[TriangleFaces[i]]);
                v1.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 1]], Normals[TriangleFaces[i + 1]]);
                v2.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 2]], Normals[TriangleFaces[i + 2]]);
                v0.Material = new VertexTexture1(UV0[TriangleFaces[i]]);
                v1.Material = new VertexTexture1(UV0[TriangleFaces[i + 1]]);
                v2.Material = new VertexTexture1(UV0[TriangleFaces[i + 2]]);
                
                prim.AddTriangle(v0, v1, v2);
            }

            Scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        }

        public void AddMeshNormalColor1UV1(string meshName, int materialIndex, 
            List<Vector3> Vertices, List<Vector3> Normals,
            List<Vector4> Colors0,
            List<Vector2> UV0, 
            List<int> TriangleFaces)
        {
            var mesh = new MeshBuilder<VertexPositionNormal, VertexColor1Texture1, VertexEmpty>(meshName);
            var prim = mesh.UsePrimitive(GetMaterial(materialIndex));

            if (TriangleFaces.Count % 3 != 0)
                throw new Exception("Expected a multiple of 3!");

            for (int i = 0; i < TriangleFaces.Count; i += 3)
            {
                var v0 = new VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexEmpty>();
                var v1 = new VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexEmpty>();
                var v2 = new VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexEmpty>();
                v0.Geometry = new VertexPositionNormal();
                v1.Geometry = new VertexPositionNormal();
                v2.Geometry = new VertexPositionNormal();
                v0.Geometry.Position = Vertices[TriangleFaces[i]];
                v1.Geometry.Position = Vertices[TriangleFaces[i + 1]];
                v2.Geometry.Position = Vertices[TriangleFaces[i + 2]];
                v0.Geometry.Normal = Normals[TriangleFaces[i]];
                v1.Geometry.Normal = Normals[TriangleFaces[i + 1]];
                v2.Geometry.Normal = Normals[TriangleFaces[i + 2]];
                v0.Material = new VertexColor1Texture1();
                v1.Material = new VertexColor1Texture1();
                v2.Material = new VertexColor1Texture1();
                v0.Material.Color = Colors0[TriangleFaces[i]];
                v1.Material.Color = Colors0[TriangleFaces[i + 1]];
                v2.Material.Color = Colors0[TriangleFaces[i + 2]];
                v0.Material.TexCoord = UV0[TriangleFaces[i]];
                v1.Material.TexCoord = UV0[TriangleFaces[i + 1]];
                v2.Material.TexCoord = UV0[TriangleFaces[i + 2]];

                prim.AddTriangle(v0, v1, v2);
            }

            Scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        }

        public void AddMeshNormalUV1Skin4(string meshName, int materialIndex,
            List<Vector3> Vertices, List<Vector3> Normals,
            List<Vector2> UV0,
            List<SparseWeight8> Skins,
            List<int> TriangleFaces)
        {
            var mesh = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>(meshName);
            var prim = mesh.UsePrimitive(GetMaterial(materialIndex));

            if (TriangleFaces.Count % 3 != 0)
                throw new Exception("Expected a multiple of 3!");

            for (int i = 0; i < TriangleFaces.Count; i += 3)
            {
                var v0 = new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>();
                var v1 = new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>();
                var v2 = new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>();
                v0.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i]], Normals[TriangleFaces[i]]);
                v1.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 1]], Normals[TriangleFaces[i + 1]]);
                v2.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 2]], Normals[TriangleFaces[i + 2]]);
                v0.Material = new VertexTexture1(UV0[TriangleFaces[i]]);
                v1.Material = new VertexTexture1(UV0[TriangleFaces[i + 1]]);
                v2.Material = new VertexTexture1(UV0[TriangleFaces[i + 2]]);
                v0.Skinning = new VertexJoints4(Skins[TriangleFaces[i]]);
                v1.Skinning = new VertexJoints4(Skins[TriangleFaces[i + 1]]);
                v2.Skinning = new VertexJoints4(Skins[TriangleFaces[i + 2]]);

                prim.AddTriangle(v0, v1, v2);
            }

            Scene.AddSkinnedMesh(mesh, Matrix4x4.Identity, Nodes.ToArray());
        }

        public void AddMeshNormalColor1UV1Skin4(string meshName, int materialIndex,
            List<Vector3> Vertices, List<Vector3> Normals,
            List<Vector4> Colors0,
            List<Vector2> UV0,
            List<SparseWeight8> Skins,
            List<int> TriangleFaces)
        {
            var mesh = new MeshBuilder<VertexPositionNormal, VertexColor1Texture1, VertexJoints4>(meshName);
            var prim = mesh.UsePrimitive(GetMaterial(materialIndex));

            if (TriangleFaces.Count % 3 != 0)
                throw new Exception("Expected a multiple of 3!");

            for (int i = 0; i < TriangleFaces.Count; i += 3)
            {
                var v0 = new VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexJoints4>();
                var v1 = new VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexJoints4>();
                var v2 = new VertexBuilder<VertexPositionNormal, VertexColor1Texture1, VertexJoints4>();
                v0.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i]], Normals[TriangleFaces[i]]);
                v1.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 1]], Normals[TriangleFaces[i + 1]]);
                v2.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 2]], Normals[TriangleFaces[i + 2]]);
                v0.Material = new VertexColor1Texture1(Colors0[TriangleFaces[i]], UV0[TriangleFaces[i]]);
                v1.Material = new VertexColor1Texture1(Colors0[TriangleFaces[i + 1]], UV0[TriangleFaces[i + 1]]);
                v2.Material = new VertexColor1Texture1(Colors0[TriangleFaces[i + 2]], UV0[TriangleFaces[i + 2]]);
                v0.Skinning = new VertexJoints4(Skins[TriangleFaces[i]]);
                v1.Skinning = new VertexJoints4(Skins[TriangleFaces[i + 1]]);
                v2.Skinning = new VertexJoints4(Skins[TriangleFaces[i + 2]]);

                prim.AddTriangle(v0, v1, v2);
            }

            Scene.AddSkinnedMesh(mesh, Matrix4x4.Identity, Nodes.ToArray());
        }

        public void AddMeshNormalColor2UV4Skin8(string meshName, int materialIndex,
            List<Vector3> Vertices, List<Vector3> Normals,
            List<Vector4> Colors0, List<Vector4> Colors1,
            List<Vector2> UV0, List<Vector2> UV1, List<Vector2> UV2, List<Vector2> UV3,
            List<SparseWeight8> Skins,
            List<int> TriangleFaces)
        {
            var mesh = new MeshBuilder<VertexPositionNormal, VertexColor2Texture4, VertexJoints8>(meshName);
            var prim = mesh.UsePrimitive(GetMaterial(materialIndex));

            if (TriangleFaces.Count % 3 != 0)
                throw new Exception("Expected a multiple of 3!");

            for (int i = 0; i < TriangleFaces.Count; i += 3)
            {
                var v0 = new VertexBuilder<VertexPositionNormal, VertexColor2Texture4, VertexJoints4>();
                var v1 = new VertexBuilder<VertexPositionNormal, VertexColor2Texture4, VertexJoints4>();
                var v2 = new VertexBuilder<VertexPositionNormal, VertexColor2Texture4, VertexJoints4>();
                v0.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i]], Normals[TriangleFaces[i]]);
                v1.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 1]], Normals[TriangleFaces[i + 1]]);
                v2.Geometry = new VertexPositionNormal(Vertices[TriangleFaces[i + 2]], Normals[TriangleFaces[i + 2]]);
                v0.Material = new VertexColor2Texture4(Colors0[TriangleFaces[i]], Colors1[TriangleFaces[i]], 
                    UV0[TriangleFaces[i]], UV1[TriangleFaces[i]], UV2[TriangleFaces[i]], UV3[TriangleFaces[i]]);
                v1.Material = new VertexColor2Texture4(Colors0[TriangleFaces[i + 1]], Colors1[TriangleFaces[i + 1]],
                    UV0[TriangleFaces[i + 1]], UV1[TriangleFaces[i + 1]], UV2[TriangleFaces[i + 1]], UV3[TriangleFaces[i + 1]]);
                v2.Material = new VertexColor2Texture4(Colors0[TriangleFaces[i + 2]], Colors1[TriangleFaces[i + 2]],
                    UV0[TriangleFaces[i + 2]], UV1[TriangleFaces[i + 2]], UV2[TriangleFaces[i + 2]], UV3[TriangleFaces[i + 2]]);
                v0.Skinning = new VertexJoints4(Skins[TriangleFaces[i]]);
                v1.Skinning = new VertexJoints4(Skins[TriangleFaces[i + 1]]);
                v2.Skinning = new VertexJoints4(Skins[TriangleFaces[i + 2]]);

                prim.AddTriangle(v0, v1, v2);
            }

            Scene.AddSkinnedMesh(mesh, Matrix4x4.Identity, Nodes.ToArray());
        }

        public void WriteGLTF(string FileName)
        {
            var model = Scene.ToGltf2();
            model.SaveGLTF(FileName);
        }

        public void WriteGLB(string FileName)
        {
            var model = Scene.ToGltf2();
            model.SaveGLB(FileName);
        }
    }
}
