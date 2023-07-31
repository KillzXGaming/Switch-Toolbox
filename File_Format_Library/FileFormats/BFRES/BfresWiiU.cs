using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Syroot.NintenTools.Bfres;
using Syroot.NintenTools.Bfres.Helpers;
using Syroot.NintenTools.Bfres.GX2;
using System.Windows.Forms;
using Bfres.Structs;
using Toolbox.Library.IO;
using Toolbox.Library;
using Toolbox.Library.Rendering;
using OpenTK;
using ResNSW = Syroot.NintenTools.NSW.Bfres;

namespace FirstPlugin
{
    public static class BfresWiiU
    {
        public static byte[] CreateNewBFRES(string Name)
        {
            MemoryStream mem = new MemoryStream();

            ResFile resFile = new ResFile();
            resFile.Name = Name;            

            resFile.Save(mem);
            var data = mem.ToArray();

            mem.Close();
            mem.Dispose();
            return data;
        }

        public static Model SetModel(FMDL fmdl)
        {
            Model model = new Model();
            model.Name = fmdl.Text;
            model.Path = "";

            model.Shapes = new ResDict<Shape>();
            model.VertexBuffers = new List<VertexBuffer>();
            model.Materials = new ResDict<Material>();
            model.UserData = new ResDict<UserData>();
            model.Skeleton = new Skeleton();
            model.Skeleton = fmdl.Skeleton.node.SkeletonU;
            model.UserData = fmdl.ModelU.UserData;

            fmdl.Skeleton.CalculateIndices();

            int i = 0;
            var duplicates = fmdl.shapes.GroupBy(c => c.Text).Where(g => g.Skip(1).Any()).SelectMany(c => c);
            foreach (var shape in duplicates)
                shape.Text += i++;

            foreach (FSHP shape in fmdl.shapes)
            {
                BFRES.CheckMissingTextures(shape);
                SetShape(shape, shape.ShapeU);

                model.Shapes.Add(shape.Text, shape.ShapeU);
                model.VertexBuffers.Add(shape.VertexBufferU);
                shape.ShapeU.VertexBufferIndex = (ushort)(model.VertexBuffers.Count - 1);

             //   BFRES.SetShaderAssignAttributes(shape.GetMaterial().shaderassign, shape);
            }

            foreach (FMAT mat in fmdl.materials.Values)
            {
                SetMaterial(mat, mat.MaterialU, fmdl.GetResFileU());
                model.Materials.Add(mat.Text, mat.MaterialU);
            }
            return model;
        }
        public static void ReadModel(FMDL model, Model mdl)
        {
            if (model == null)
                model = new FMDL();

            model.Text = mdl.Name;
            model.Skeleton = new FSKL(mdl.Skeleton);

            model.Nodes.RemoveAt(2);
            model.Nodes.Add(model.Skeleton.node);

            model.ModelU = mdl;
            foreach (Material mat in mdl.Materials.Values)
            {
                FMAT FMAT = new FMAT();
                FMAT.Text = mat.Name;
                FMAT.ReadMaterial(mat);
                model.Nodes["FmatFolder"].Nodes.Add(FMAT);
                model.materials.Add(FMAT.Text, FMAT);
            }
            foreach (Shape shp in mdl.Shapes.Values)
            {
                VertexBuffer vertexBuffer = mdl.VertexBuffers[shp.VertexBufferIndex];
                Material material = mdl.Materials[shp.MaterialIndex];
                FSHP mesh = new FSHP();
                ReadShapesVertices(mesh, shp, vertexBuffer, model);
                mesh.MaterialIndex = shp.MaterialIndex;

                model.Nodes["FshpFolder"].Nodes.Add(mesh);
                model.shapes.Add(mesh);
            }
        }

        public static void ReadShapesVertices(FSHP fshp, Shape shp, VertexBuffer vertexBuffer, FMDL model)
        {
            fshp.boundingBoxes.Clear();
            fshp.boundingRadius.Clear();
            fshp.BoneIndices.Clear();

            foreach (Bounding bnd in shp.SubMeshBoundings)
            {
                FSHP.BoundingBox box = new FSHP.BoundingBox();
                box.Center = new Vector3(bnd.Center.X, bnd.Center.Y, bnd.Center.Z);
                box.Extend = new Vector3(bnd.Extent.X, bnd.Extent.Y, bnd.Extent.Z);
                fshp.boundingBoxes.Add(box);
            }

            foreach (float rad in shp.RadiusArray)
            {
                fshp.boundingRadius.Add(rad);
            }

            fshp.VertexBufferIndex = shp.VertexBufferIndex;
            fshp.ShapeU = shp;
            fshp.VertexBufferU = vertexBuffer;
            fshp.VertexSkinCount = shp.VertexSkinCount;
            fshp.BoneIndex = shp.BoneIndex;
            fshp.Text = shp.Name;
            fshp.TargetAttribCount = shp.TargetAttribCount;
            fshp.MaterialIndex = shp.MaterialIndex;

            if (shp.SkinBoneIndices != null)
            {
                foreach (ushort bn in shp.SkinBoneIndices)
                    fshp.BoneIndices.Add(bn);
            }

            ReadMeshes(fshp, shp);
            ReadVertexBuffer(fshp, vertexBuffer, model);
        }
        private static void ReadMeshes(FSHP fshp, Shape shp)
        {
            fshp.lodMeshes.Clear();

            foreach (Mesh msh in shp.Meshes)
            {
                uint FaceCount = msh.IndexCount;
                uint[] indicesArray = msh.GetIndices().ToArray();

                FSHP.LOD_Mesh lod = new FSHP.LOD_Mesh();
                foreach (SubMesh subMsh in msh.SubMeshes)
                {
                    FSHP.LOD_Mesh.SubMesh sub = new FSHP.LOD_Mesh.SubMesh();
                    sub.size = subMsh.Count;
                    sub.offset = subMsh.Offset;
                    lod.subMeshes.Add(sub);
                }
                lod.IndexFormat = (STIndexFormat)msh.IndexFormat;
                switch (msh.PrimitiveType)
                {
                    case GX2PrimitiveType.Triangles:
                        lod.PrimativeType = STPrimitiveType.Triangles;
                        break;
                    case GX2PrimitiveType.TriangleStrip:
                        lod.PrimativeType = STPrimitiveType.TrangleStrips;
                        break;
                    case GX2PrimitiveType.Quads:
                        lod.PrimativeType = STPrimitiveType.Quads;
                        break;
                    case GX2PrimitiveType.Lines:
                        lod.PrimativeType = STPrimitiveType.Lines;
                        break;
                    case GX2PrimitiveType.LineStrip:
                        lod.PrimativeType = STPrimitiveType.LineStrips;
                        break;
                    case GX2PrimitiveType.Points:
                        lod.PrimativeType = STPrimitiveType.Points;
                        break;
                }

                lod.FirstVertex = msh.FirstVertex;

                for (int face = 0; face < FaceCount; face++)
                    lod.faces.Add((int)indicesArray[face] + (int)msh.FirstVertex);

                fshp.lodMeshes.Add(lod);
            }
        }
        private static void ReadVertexBuffer(FSHP fshp, VertexBuffer vtx, FMDL model)
        {
            fshp.vertices.Clear();
            fshp.vertexAttributes.Clear();

            //Create a buffer instance which stores all the buffer data
            VertexBufferHelper helper = new VertexBufferHelper(vtx, Syroot.BinaryData.ByteOrder.BigEndian);

            //Set each array first from the lib if exist. Then add the data all in one loop
            Syroot.Maths.Vector4F[] vec4Positions = new Syroot.Maths.Vector4F[0];
            Syroot.Maths.Vector4F[] vec4Normals = new Syroot.Maths.Vector4F[0];
            Syroot.Maths.Vector4F[] vec4uv0 = new Syroot.Maths.Vector4F[0];
            Syroot.Maths.Vector4F[] vec4uv1 = new Syroot.Maths.Vector4F[0];
            Syroot.Maths.Vector4F[] vec4uv2 = new Syroot.Maths.Vector4F[0];
            Syroot.Maths.Vector4F[] vec4c0 = new Syroot.Maths.Vector4F[0];
            Syroot.Maths.Vector4F[] vec4t0 = new Syroot.Maths.Vector4F[0];
            Syroot.Maths.Vector4F[] vec4b0 = new Syroot.Maths.Vector4F[0];
            Syroot.Maths.Vector4F[] vec4w0 = new Syroot.Maths.Vector4F[0];
            Syroot.Maths.Vector4F[] vec4i0 = new Syroot.Maths.Vector4F[0];

            //For shape morphing
            Syroot.Maths.Vector4F[] vec4Positions1 = new Syroot.Maths.Vector4F[0];
            Syroot.Maths.Vector4F[] vec4Positions2 = new Syroot.Maths.Vector4F[0];

            List<VertexAttrib> SortedList = vtx.Attributes.Values.OrderBy(o => o.BufferIndex).ToList();

            foreach (VertexAttrib att in SortedList)
                Console.WriteLine($"{att.Name} {att.BufferIndex} {att.Offset} {att.Format} ");

            foreach (VertexAttrib att in vtx.Attributes.Values)
            {
                FSHP.VertexAttribute attr = new FSHP.VertexAttribute();
                attr.Name = att.Name;
                attr.Format = attr.GetTypeWiiU(att.Format);
                attr.BufferIndex = att.BufferIndex;

                if (att.Name == "_p0")
                    vec4Positions = AttributeData(att, helper, "_p0");
                if (att.Name == "_n0")
                    vec4Normals = AttributeData(att, helper, "_n0");
                if (att.Name == "_u0")
                    vec4uv0 = AttributeData(att, helper, "_u0");
                if (att.Name == "_u1")
                    vec4uv1 = AttributeData(att, helper, "_u1");
                if (att.Name == "_u2")
                    vec4uv2 = AttributeData(att, helper, "_u2");
                if (att.Name == "_c0")
                    vec4c0 = AttributeData(att, helper, "_c0");
                if (att.Name == "_t0")
                    vec4t0 = AttributeData(att, helper, "_t0");
                if (att.Name == "_b0")
                    vec4b0 = AttributeData(att, helper, "_b0");
                if (att.Name == "_w0")
                    vec4w0 = AttributeData(att, helper, "_w0");
                if (att.Name == "_i0")
                    vec4i0 = AttributeData(att, helper, "_i0");

                if (att.Name == "_p1")
                    vec4Positions1 = AttributeData(att, helper, "_p1");
                if (att.Name == "_p2")
                    vec4Positions2 = AttributeData(att, helper, "_p2");

                fshp.vertexAttributes.Add(attr);
            }
            for (int i = 0; i < vec4Positions.Length; i++)
            {
                Vertex v = new Vertex();
                if (vec4Positions.Length > 0)
                    v.pos = new Vector3(vec4Positions[i].X, vec4Positions[i].Y, vec4Positions[i].Z);
                if (vec4Positions1.Length > 0)
                    v.pos1 = new Vector3(vec4Positions1[i].X, vec4Positions1[i].Y, vec4Positions1[i].Z);
                if (vec4Positions2.Length > 0)
                    v.pos2 = new Vector3(vec4Positions2[i].X, vec4Positions2[i].Y, vec4Positions2[i].Z);
                if (vec4Normals.Length > 0)
                    v.nrm = new Vector3(vec4Normals[i].X, vec4Normals[i].Y, vec4Normals[i].Z);
                if (vec4uv0.Length > 0)
                    v.uv0 = new Vector2(vec4uv0[i].X, vec4uv0[i].Y);
                if (vec4uv1.Length > 0)
                    v.uv1 = new Vector2(vec4uv1[i].X, vec4uv1[i].Y);
                if (vec4uv2.Length > 0)
                    v.uv2 = new Vector2(vec4uv2[i].X, vec4uv2[i].Y);
                if (vec4w0.Length > 0)
                {
                    v.boneWeights.Add(vec4w0[i].X);
                    v.boneWeights.Add(vec4w0[i].Y);
                    v.boneWeights.Add(vec4w0[i].Z);
                    v.boneWeights.Add(vec4w0[i].W);
                }
                if (vec4i0.Length > 0)
                {
                    v.boneIds.Add((int)vec4i0[i].X);
                    v.boneIds.Add((int)vec4i0[i].Y);
                    v.boneIds.Add((int)vec4i0[i].Z);
                    v.boneIds.Add((int)vec4i0[i].W);
                }

                if (vec4t0.Length > 0)
                    v.tan = new Vector4(vec4t0[i].X, vec4t0[i].Y, vec4t0[i].Z, vec4t0[i].W);
                if (vec4b0.Length > 0)
                    v.bitan = new Vector4(vec4b0[i].X, vec4b0[i].Y, vec4b0[i].Z, vec4b0[i].W);
                if (vec4c0.Length > 0)
                    v.col = new Vector4(vec4c0[i].X, vec4c0[i].Y, vec4c0[i].Z, vec4c0[i].W);

                if (fshp.VertexSkinCount == 1)
                {
                    int boneIndex = fshp.BoneIndex;
                    if (v.boneIds.Count > 0)
                        boneIndex = model.Skeleton.Node_Array[v.boneIds[0]];

                    //Check if the bones are a rigid type
                    //In game it seems to not transform if they are not rigid
                    if (model.Skeleton.bones[boneIndex].RigidMatrixIndex != -1)
                    {
                        Matrix4 sb = model.Skeleton.bones[boneIndex].Transform;
                        v.pos = Vector3.TransformPosition(v.pos, sb);
                        v.nrm = Vector3.TransformNormal(v.nrm, sb);
                    }
                }
                if (fshp.VertexSkinCount == 0)
                {
                    try
                    {
                        if (model.Skeleton.bones.Count > 0) {
                            int boneIndex = fshp.BoneIndex;

                            Matrix4 NoBindFix = model.Skeleton.bones[boneIndex].Transform;
                            v.pos = Vector3.TransformPosition(v.pos, NoBindFix);
                            v.nrm = Vector3.TransformNormal(v.nrm, NoBindFix);
                        }
                    }
                    catch //Matrix failed. Print the coordinate data of the bone
                    {
                        Console.WriteLine(model.Skeleton.bones[fshp.BoneIndex].Text);
                        Console.WriteLine(model.Skeleton.bones[fshp.BoneIndex].GetPosition());
                        Console.WriteLine(model.Skeleton.bones[fshp.BoneIndex].GetRotation());
                        Console.WriteLine(model.Skeleton.bones[fshp.BoneIndex].GetScale());
                    }
                }
                fshp.vertices.Add(v);
            }
        }
        private static Syroot.Maths.Vector4F[] AttributeData(VertexAttrib att, VertexBufferHelper helper, string attName)
        {
            VertexBufferHelperAttrib attd = helper[attName];
            return attd.Data;
        }

        public static void ReadSkeleton(this TreeNodeCustom skl, Skeleton skeleton, FSKL RenderableSkeleton)
        {
            if (skeleton.MatrixToBoneList == null)
                skeleton.MatrixToBoneList = new List<ushort>();

            RenderableSkeleton.Node_Array = new int[skeleton.MatrixToBoneList.Count];
            int nodes = 0;
            foreach (ushort node in skeleton.MatrixToBoneList)
            {
                RenderableSkeleton.Node_Array[nodes] = node;
                nodes++;
            }

            RenderableSkeleton.bones.Clear();
            foreach (Bone bone in skeleton.Bones.Values)
            {
                BfresBone STBone = new BfresBone(RenderableSkeleton);
                ReadBone(STBone, bone);
                RenderableSkeleton.bones.Add(STBone);

                if (skeleton.FlagsScaling == SkeletonFlagsScaling.Maya)
                    STBone.UseSegmentScaleCompensate = true;
            }

            skl.Nodes.Clear();
            foreach (var bone in RenderableSkeleton.bones) {
                if (bone.Parent == null)
                {
                    skl.Nodes.Add(bone);
                }
            }

            RenderableSkeleton.update();
            RenderableSkeleton.reset();
        }
        public static void ReadBone(this BfresBone bone, Bone bn, bool SetParent = true)
        {
            bone.BoneU = bn;
            bone.FlagVisible = bn.Flags.HasFlag(BoneFlags.Visible);
            bone.Text = bn.Name;
            bone.RigidMatrixIndex = bn.RigidMatrixIndex;
            bone.SmoothMatrixIndex = bn.SmoothMatrixIndex;
            bone.BillboardIndex = bn.BillboardIndex;
            if (SetParent)
                bone.parentIndex = bn.ParentIndex;
            if (bn.FlagsRotation == BoneFlagsRotation.Quaternion)
                bone.RotationType = STBone.BoneRotationType.Quaternion;
            else
                bone.RotationType = STBone.BoneRotationType.Euler;

            bone.Position = new OpenTK.Vector3(
                         bn.Position.X,
                         bn.Position.Y,
                         bn.Position.Z);
            bone.Rotation = new OpenTK.Quaternion(
                          bn.Rotation.X,
                          bn.Rotation.Y,
                          bn.Rotation.Z,
                          bn.Rotation.W);
            bone.Scale = new OpenTK.Vector3(
                          bn.Scale.X,
                          bn.Scale.Y,
                          bn.Scale.Z);
        }

        public static void SetShape(this FSHP s, Shape shp)
        {
            shp.Name = s.Text;
            shp.MaterialIndex = (ushort)s.MaterialIndex;
            shp.BoneIndex = (ushort)s.BoneIndex;

            if (shp.SubMeshBoundingNodes == null)
                shp.SubMeshBoundingNodes = new List<BoundingNode>();
            if (shp.SubMeshBoundingIndices == null)
                shp.SubMeshBoundingIndices = new List<ushort>();
            if (shp.SkinBoneIndices == null)
                shp.SkinBoneIndices = new List<ushort>();

            int indx = 0;
            foreach (var mesh in shp.Meshes)
            {
                switch (s.lodMeshes[indx].PrimativeType)
                {
                    case STPrimitiveType.Triangles:
                        mesh.PrimitiveType = GX2PrimitiveType.Triangles;
                        break;
                    case STPrimitiveType.Lines:
                        mesh.PrimitiveType = GX2PrimitiveType.Lines;
                        break;
                    case STPrimitiveType.LineStrips:
                        mesh.PrimitiveType = GX2PrimitiveType.LineStrip;
                        break;
                    case STPrimitiveType.Points:
                        mesh.PrimitiveType = GX2PrimitiveType.Points;
                        break;
                }
                indx++;
            }
        }

        public static void SetMaterial(this FMAT m, Material mat, ResFile ResFile)
        {
            mat.Name = m.Text;

            if (m.Enabled)
                mat.Flags = MaterialFlags.Visible;
            else
                mat.Flags = MaterialFlags.None;

            if (mat.ShaderParamData == null)
                mat.ShaderParamData = new byte[0];

            byte[] ParamData = WriteShaderParams(m, mat);
            mat.ShaderParamData = ParamData;

            WriteRenderInfo(m, mat);
            WriteTextureRefs(m, mat, ResFile);
            WriteShaderAssign(m.shaderassign, mat);
        }
        public static void ReadMaterial(this FMAT m, Material mat)
        {
            m.MaterialU = mat;

            if (mat.Flags == MaterialFlags.Visible)
                m.Enabled = true;
            else
                m.Enabled = false;

            m.ReadRenderInfo(mat);
            m.ReadShaderAssign(mat);
            m.SetActiveGame();
            m.ReadShaderParams(mat);
            m.ReadTextureRefs(mat);
            m.ReadRenderState(mat.RenderState);
            m.UpdateRenderPass();
        }
        public static void ReadRenderState(this FMAT m, RenderState renderState)
        {

        }
        public static void ReadTextureRefs(this FMAT m, Material mat)
        {
            m.TextureMaps.Clear();

            int AlbedoCount = 0;
            int id = 0;
            string TextureName = "";
            if (mat.TextureRefs == null)
                mat.TextureRefs = new List<TextureRef>();

            int textureUnit = 1;
            foreach (var tex in mat.TextureRefs)
            {
                TextureName = tex.Name;

                MatTexture texture = new MatTexture();
                texture.wiiUSampler = mat.Samplers[id].TexSampler;

                texture.MinLod = mat.Samplers[id].TexSampler.MinLod;
                texture.MaxLod = mat.Samplers[id].TexSampler.MaxLod;
                texture.BiasLod = mat.Samplers[id].TexSampler.LodBias;

                texture.WrapModeS = (STTextureWrapMode)mat.Samplers[id].TexSampler.ClampX;
                texture.WrapModeT = (STTextureWrapMode)mat.Samplers[id].TexSampler.ClampY;
                texture.WrapModeW = (STTextureWrapMode)mat.Samplers[id].TexSampler.ClampZ;
                mat.Samplers.TryGetKey(mat.Samplers[id], out texture.SamplerName);

                string useSampler = texture.SamplerName;

                //Use the fragment sampler in the shader assign section. It's usually more accurate this way
                if (mat.ShaderAssign.SamplerAssigns.ContainsKey(texture.SamplerName))
                    useSampler = mat.ShaderAssign.SamplerAssigns[texture.SamplerName];
                

                if (mat.Samplers[id].TexSampler.MinFilter == GX2TexXYFilterType.Point)
                    texture.MinFilter = STTextureMinFilter.Nearest;
                if (mat.Samplers[id].TexSampler.MagFilter == GX2TexXYFilterType.Point)
                    texture.MagFilter = STTextureMagFilter.Nearest;
                if (mat.Samplers[id].TexSampler.MinFilter == GX2TexXYFilterType.Bilinear)
                    texture.MinFilter = STTextureMinFilter.Linear;
                if (mat.Samplers[id].TexSampler.MagFilter == GX2TexXYFilterType.Bilinear)
                    texture.MagFilter = STTextureMagFilter.Linear;

                if (Runtime.activeGame == Runtime.ActiveGame.MK8D)
                {
                    if (useSampler == "_a0")
                    {
                        m.HasDiffuseMap = true;
                        AlbedoCount++;
                        texture.Type = MatTexture.TextureType.Diffuse;
                    }
                    else if (useSampler == "s_diffuse")
                    {
                        m.HasDiffuseMap = true;
                        AlbedoCount++;
                        texture.Type = MatTexture.TextureType.Diffuse;
                    }
                    else if (useSampler == "s_normal")
                    {
                        m.HasNormalMap = true;
                        texture.Type = MatTexture.TextureType.Normal;
                    }
                    else if (useSampler == "s_specmask")
                    {
                        m.HasSpecularMap = true;
                        texture.Type = MatTexture.TextureType.Specular;
                    }
                    else if (useSampler == "_a1")
                    {
                        m.HasDiffuseLayer = true;
                        texture.Type = MatTexture.TextureType.DiffuseLayer2;
                    }
                    else if (useSampler == "_n0")
                    {
                        m.HasNormalMap = true;
                        texture.Type = MatTexture.TextureType.Normal;
                    }
                    else if (useSampler == "_e0")
                    {
                        m.HasEmissionMap = true;
                        texture.Type = MatTexture.TextureType.Emission;
                    }
                    else if (texture.SamplerName == "_s0" || useSampler == "_s0")
                    {
                        m.HasSpecularMap = true;
                        texture.Type = MatTexture.TextureType.Specular;
                    }
                    else if (useSampler == "_x0")
                    {
                        m.HasSphereMap = true;
                        texture.Type = MatTexture.TextureType.SphereMap;
                    }
                    else if (useSampler == "_b0")
                    {
                        m.HasShadowMap = true;
                        texture.Type = MatTexture.TextureType.Shadow;
                    }
                    else if (texture.SamplerName == "bake0")
                    {
                        m.HasShadowMap = true;
                        texture.Type = MatTexture.TextureType.Shadow;
                    }
                    else if (useSampler == "_b1")
                    {
                        m.HasLightMap = true;
                        texture.Type = MatTexture.TextureType.Light;
                    }
                    else if (texture.SamplerName == "_ao0")
                    {
                        texture.Type = MatTexture.TextureType.AO;
                        m.HasAmbientOcclusionMap = true;
                    }
                }
                else
                {
                    if (useSampler == "s_diffuse")
                    {
                        m.HasDiffuseMap = true;
                        AlbedoCount++;
                        texture.Type = MatTexture.TextureType.Diffuse;
                    }
                    else if (useSampler == "s_normal")
                    {
                        m.HasNormalMap = true;
                        texture.Type = MatTexture.TextureType.Normal;
                    }
                    else if (useSampler == "s_specmask")
                    {
                        m.HasSpecularMap = true;
                        texture.Type = MatTexture.TextureType.Specular;
                    }
                    else if (useSampler == "_a0")
                    {
                        m.HasDiffuseMap = true;
                        AlbedoCount++;
                        texture.Type = MatTexture.TextureType.Diffuse;
                    }
                    else if (useSampler == "_n0")
                    {
                        m.HasNormalMap = true;
                        texture.Type = MatTexture.TextureType.Normal;
                    }
                    else if (TextureName.Contains("Emm"))
                    {
                        m.HasEmissionMap = true;
                        texture.Type = MatTexture.TextureType.Emission;
                    }
                    else if (TextureName.Contains("Spm"))
                    {
                        m.HasSpecularMap = true;
                        texture.Type = MatTexture.TextureType.Specular;
                    }
                    else if (TextureName.Contains("b00"))
                    {
                        m.HasShadowMap = true;
                        texture.Type = MatTexture.TextureType.Shadow;
                    }
                    else if (texture.SamplerName == "bake0")
                    {
                        m.HasShadowMap = true;
                        texture.Type = MatTexture.TextureType.Shadow;
                    }
                    else if (TextureName.Contains("Moc") || TextureName.Contains("AO"))
                    {
                        m.HasAmbientOcclusionMap = true;
                        texture.Type = MatTexture.TextureType.AO;
                    }
                    else if (TextureName.Contains("b01"))
                    {
                        m.HasLightMap = true;
                        texture.Type = MatTexture.TextureType.Light;
                    }
                    else if (TextureName.Contains("MRA")) //Metalness, Roughness, and Cavity Map in one
                    {
                        m.HasRoughnessMap = true;
                        texture.Type = MatTexture.TextureType.MRA;
                    }
                    else if (TextureName.Contains("mtl"))
                    {
                        m.HasMetalnessMap = true;
                        texture.Type = MatTexture.TextureType.Metalness;
                    }
                    else if (TextureName.Contains("rgh"))
                    {
                        texture.Type = MatTexture.TextureType.Roughness;
                        m.HasRoughnessMap = true;
                    }
                    else if (TextureName.Contains("sss"))
                    {
                        texture.Type = MatTexture.TextureType.SubSurfaceScattering;
                        m.HasSubSurfaceScatteringMap = true;
                    }
                    else if (texture.SamplerName == "_ao0")
                    {
                        texture.Type = MatTexture.TextureType.AO;
                        m.HasAmbientOcclusionMap = true;
                    }
                }

                Console.WriteLine($"{useSampler} {texture.Type}");

                texture.textureUnit = textureUnit++;

                texture.Name = TextureName;
                m.TextureMaps.Add(texture);

                id++;
            }
        }
        public static void ReadShaderParams(this FMAT m, Material mat)
        {
            m.matparam.Clear();

            if (mat.ShaderParamData == null)
                return;

            using (FileReader reader = new FileReader(new System.IO.MemoryStream(mat.ShaderParamData)))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                foreach (ShaderParam param in mat.ShaderParams.Values)
                {
                    BfresShaderParam shaderParam = new BfresShaderParam();
                    shaderParam.Type = shaderParam.GetTypeWiiU(param.Type);
                    shaderParam.Name = param.Name;
                    shaderParam.HasPadding = param.UsePadding;
                    shaderParam.PaddingLength = param.PaddingLength;
                    shaderParam.DependedIndex = param.DependedIndex;
                    shaderParam.DependIndex = param.DependIndex;

                    reader.Seek(param.DataOffset, System.IO.SeekOrigin.Begin);
                    shaderParam.ReadValue(reader, (int)param.DataSize);

                    m.matparam.Add(param.Name, shaderParam);
                }
                reader.Close();
            }
        }
        public static byte[] WriteShaderParams(this FMAT m, Material mat)
        {
            mat.ShaderParams = new ResDict<ShaderParam>();

            System.IO.MemoryStream mem = new System.IO.MemoryStream();
            using (FileWriter writer = new FileWriter(mem))
            {
                uint Offset = 0;
                int index = 0;
                writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                foreach (BfresShaderParam shaderParam in m.matparam.Values)
                {
                    ShaderParam param = new ShaderParam();
                    param.Name = shaderParam.Name;
                    param.Type = shaderParam.SetTypeWiiU(shaderParam.Type);
                    param.DataOffset = (ushort)Offset;
                    param.offset = -1;
                    param.callbackPointer = 0;
                    param.PaddingLength = shaderParam.PaddingLength;

                    param.DependedIndex = shaderParam.DependedIndex;
                    param.DependIndex = shaderParam.DependIndex;

                    writer.Seek(param.DataOffset, System.IO.SeekOrigin.Begin);
                    shaderParam.WriteValue(writer);

                    Offset += (param.DataSize + (uint)shaderParam.PaddingLength);
                    mat.ShaderParams.Add(param.Name, param);
                    index++;
                }
                writer.Close();
            }
            return mem.ToArray();
        }
        public static void ReadRenderInfo(this FMAT m, Material mat)
        {
            m.renderinfo.Clear();

            foreach (RenderInfo rnd in mat.RenderInfos.Values)
            {
                BfresRenderInfo r = new BfresRenderInfo();
                r.Name = rnd.Name;
                r.Type = r.GetTypeWiiU(rnd.Type);
                switch (rnd.Type)
                {
                    case RenderInfoType.Int32: r.ValueInt = rnd.GetValueInt32s(); break;
                    case RenderInfoType.Single: r.ValueFloat = rnd.GetValueSingles(); break;
                    case RenderInfoType.String: r.ValueString = rnd.GetValueStrings(); break;
                }
                m.renderinfo.Add(r);
            }
        }
        public static void WriteTextureRefs(this FMAT m, Material mat, ResFile resFile)
        {
            mat.TextureRefs = new List<TextureRef>();
            mat.TextureRefs.Clear();
            mat.Samplers.Clear();

            int index = 0;
            foreach (MatTexture textu in m.TextureMaps)
            {
                TextureRef texref = new TextureRef();
                texref.Name = textu.Name;

                Sampler sampler = new Sampler();
                sampler.TexSampler = textu.wiiUSampler;
                sampler.Name = textu.SamplerName;
                mat.Samplers.Add(textu.SamplerName, sampler);

                Texture texMapped = new Texture();
                resFile.Textures.TryGetValue(textu.Name, out texMapped);
                texref.Texture = texMapped;

                mat.TextureRefs.Add(texref);
                index++;
            }
        }
        public static void WriteRenderInfo(this FMAT m, Material mat)
        {
            if (mat.RenderInfos == null)
                mat.RenderInfos = new ResDict<RenderInfo>();

            mat.RenderInfos.Clear();
            foreach (BfresRenderInfo rnd in m.renderinfo)
            {
                RenderInfo r = new RenderInfo();
                r.Name = rnd.Name;

                switch (rnd.Type)
                {
                    case ResNSW.RenderInfoType.Int32: r.SetValue(rnd.ValueInt); break;
                    case ResNSW.RenderInfoType.Single: r.SetValue(rnd.ValueFloat); break;
                    case ResNSW.RenderInfoType.String: r.SetValue(rnd.ValueString); break;
                }
                mat.RenderInfos.Add(r.Name, r);
            }
        }
        public static void ReadShaderAssign(this FMAT m, Material mat)
        {
            m.shaderassign = new FMAT.ShaderAssign();

            if (mat.ShaderAssign == null)
                mat.ShaderAssign = new ShaderAssign();
            if (mat.ShaderAssign.ShaderOptions == null)
                mat.ShaderAssign.ShaderOptions = new ResDict<ResString>();
            if (mat.ShaderAssign.AttribAssigns == null)
                mat.ShaderAssign.AttribAssigns = new ResDict<ResString>();
            if (mat.ShaderAssign.SamplerAssigns == null)
                mat.ShaderAssign.SamplerAssigns = new ResDict<ResString>();

            m.shaderassign.options.Clear();
            m.shaderassign.samplers.Clear();
            m.shaderassign.attributes.Clear();

            m.shaderassign = new FMAT.ShaderAssign();
            m.shaderassign.ShaderArchive = mat.ShaderAssign.ShaderArchiveName;
            m.shaderassign.ShaderModel = mat.ShaderAssign.ShadingModelName;

            foreach (var op in mat.ShaderAssign.ShaderOptions)
                m.shaderassign.options.Add(op.Key, op.Value);

            if (mat.ShaderAssign.SamplerAssigns != null)
            {
                foreach (var op in mat.ShaderAssign.SamplerAssigns)
                    m.shaderassign.samplers.Add(op.Key, op.Value);
                
            }
            if (mat.ShaderAssign.AttribAssigns != null)
            {
                foreach (var op in mat.ShaderAssign.AttribAssigns)
                    m.shaderassign.attributes.Add(op.Key, op.Value);
            }
        }
        public static void WriteShaderAssign(this FMAT.ShaderAssign shd, Material mat)
        {
            mat.ShaderAssign = new ShaderAssign();
            mat.ShaderAssign.ShaderOptions = new ResDict<ResString>();
            mat.ShaderAssign.AttribAssigns = new ResDict<ResString>();
            mat.ShaderAssign.SamplerAssigns = new ResDict<ResString>();

            mat.ShaderAssign.ShaderArchiveName = shd.ShaderArchive;
            mat.ShaderAssign.ShadingModelName = shd.ShaderModel;
            foreach (var option in shd.options)
                mat.ShaderAssign.ShaderOptions.Add(option.Key, option.Value);
            foreach (var samp in shd.samplers)
                mat.ShaderAssign.SamplerAssigns.Add(samp.Key, samp.Value);
            foreach (var att in shd.attributes)
                mat.ShaderAssign.AttribAssigns.Add(att.Key, att.Value);
        }

        public static Shape SaveShape(FSHP fshp)
        {
            Shape ShapeU = new Shape();
            ShapeU.VertexSkinCount = (byte)fshp.VertexSkinCount;
            ShapeU.Flags = ShapeFlags.HasVertexBuffer;
            ShapeU.BoneIndex = (ushort)fshp.BoneIndex;
            ShapeU.MaterialIndex = (ushort)fshp.MaterialIndex;
            ShapeU.VertexBufferIndex = (ushort)fshp.VertexBufferIndex;
            ShapeU.KeyShapes = new ResDict<KeyShape>();
            ShapeU.Name = fshp.Text;
            ShapeU.TargetAttribCount = (byte)fshp.TargetAttribCount;
            ShapeU.SubMeshBoundings = new List<Bounding>();
            ShapeU.RadiusArray = new List<float>();
            ShapeU.Meshes = new List<Mesh>();

            foreach (ushort index in fshp.BoneIndices)
                ShapeU.SkinBoneIndices.Add(index);

            foreach (float radius in fshp.boundingRadius)
                ShapeU.RadiusArray.Add(radius);

            foreach (FSHP.BoundingBox box in fshp.boundingBoxes)
            {
                Bounding bnd = new Bounding();
                bnd.Center = new Syroot.Maths.Vector3F(box.Center.X, box.Center.Y, box.Center.Z);
                bnd.Extent = new Syroot.Maths.Vector3F(box.Extend.X, box.Extend.Y, box.Extend.Z);
                ShapeU.SubMeshBoundings.Add(bnd);
            }

            foreach (FSHP.LOD_Mesh mesh in fshp.lodMeshes)
            {
                Mesh msh = new Mesh();
                msh.SubMeshes = new List<SubMesh>();
                switch (mesh.PrimativeType)
                {
                    case STPrimitiveType.Triangles:
                        msh.PrimitiveType = GX2PrimitiveType.Triangles;
                        break;
                    case STPrimitiveType.TrangleStrips:
                        msh.PrimitiveType = GX2PrimitiveType.TriangleStrip;
                        break;
                    case STPrimitiveType.Quads:
                        msh.PrimitiveType = GX2PrimitiveType.Quads;
                        break;
                    case STPrimitiveType.Lines:
                        msh.PrimitiveType = GX2PrimitiveType.Lines;
                        break;
                    case STPrimitiveType.LineStrips:
                        msh.PrimitiveType = GX2PrimitiveType.LineStrip;
                        break;
                    case STPrimitiveType.Points:
                        msh.PrimitiveType = GX2PrimitiveType.Points;
                        break;
                }
                msh.FirstVertex = mesh.FirstVertex;

                foreach (FSHP.LOD_Mesh.SubMesh sub in mesh.subMeshes)
                {
                    SubMesh subMesh = new SubMesh();
                    subMesh.Offset = sub.offset;
                    subMesh.Count = (uint)mesh.faces.Count;
                    msh.SubMeshes.Add(subMesh);
                }

                IList<uint> faceList = new List<uint>();
                msh.IndexBuffer = new Syroot.NintenTools.Bfres.Buffer();
                foreach (int f in mesh.faces)
                {
                    faceList.Add((uint)f);
                }
                if (faceList.Count > 65000)
                {
                    msh.SetIndices(faceList, GX2IndexFormat.UInt32);
                }
                else
                    msh.SetIndices(faceList, GX2IndexFormat.UInt16);

                ShapeU.Meshes.Add(msh);
            }
            return ShapeU;
        }
        public static void SaveSkeleton(FSKL fskl, List<STBone> Bones)
        {
            if (fskl.node.SkeletonU == null)
                fskl.node.SkeletonU = new Skeleton();

            fskl.node.SkeletonU.Bones.Clear();
            fskl.node.SkeletonU.InverseModelMatrices = new List<Syroot.Maths.Matrix3x4>();

            fskl.node.Nodes.Clear();

            ushort SmoothIndex = 0;
            foreach (STBone genericBone in Bones)
            {
                genericBone.BillboardIndex = -1;

                //Clone a generic bone with the generic data
                BfresBone bn = new BfresBone(fskl);
                bn.CloneBaseInstance(genericBone);

                //Set the bfres bone data
                if (bn.BoneU == null)
                    bn.BoneU = new Bone();
                bn.GenericToBfresBone();

                //Check duplicated names
                List<string> names = fskl.bones.Select(o => o.Text).ToList();
                bn.Text = Utils.RenameDuplicateString(names, bn.Text);

                fskl.node.SkeletonU.InverseModelMatrices.Add(Syroot.Maths.Matrix3x4.Zero);
                fskl.bones.Add(bn);

                bn.BoneU.Name = bn.Text;
                fskl.node.SkeletonU.Bones.Add(bn.Text, bn.BoneU);

                //Add bones to tree
                if (bn.Parent == null)
                {
                    fskl.node.Nodes.Add(bn);
                }
            }


            fskl.update();
            fskl.reset();
        }
        public static void SaveVertexBuffer(FSHP fshp)
        {
            VertexBuffer buffer = new VertexBuffer();
            buffer.Attributes = new ResDict<VertexAttrib>();

            VertexBufferHelper helper = new VertexBufferHelper(buffer, Syroot.BinaryData.ByteOrder.BigEndian);
            List<VertexBufferHelperAttrib> atrib = new List<VertexBufferHelperAttrib>();

            fshp.UpdateVertices();

            Console.WriteLine("Creating Buffer");

            foreach (FSHP.VertexAttribute att in fshp.vertexAttributes)
            {
                if (att.Name == "_p0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = fshp.verts.ToArray();
                    vert.Format = att.SetTypeWiiU(att.Format);
                    vert.BufferIndex = att.BufferIndex;
                    atrib.Add(vert);
                }
                if (att.Name == "_n0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = fshp.norms.ToArray();
                    vert.Format = att.SetTypeWiiU(att.Format);
                    vert.BufferIndex = att.BufferIndex;
                    atrib.Add(vert);
                }
                if (att.Name == "_u0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = fshp.uv0.ToArray();
                    vert.Format = att.SetTypeWiiU(att.Format);
                    vert.BufferIndex = att.BufferIndex;
                    atrib.Add(vert);
                }
                if (att.Name == "_u1")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = fshp.uv1.ToArray();
                    vert.Format = att.SetTypeWiiU(att.Format);
                    vert.BufferIndex = att.BufferIndex;
                    atrib.Add(vert);
                }
                if (att.Name == "_u2")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = fshp.uv2.ToArray();
                    vert.Format = att.SetTypeWiiU(att.Format);
                    vert.BufferIndex = att.BufferIndex;
                    atrib.Add(vert);
                }
                if (att.Name == "_b0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = fshp.bitans.ToArray();
                    vert.Format = att.SetTypeWiiU(att.Format);
                    vert.BufferIndex = att.BufferIndex;
                    atrib.Add(vert);
                }
                if (att.Name == "_t0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = fshp.tans.ToArray();
                    vert.Format = att.SetTypeWiiU(att.Format);
                    vert.BufferIndex = att.BufferIndex;
                    atrib.Add(vert);
                }
                if (att.Name == "_c0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = fshp.colors.ToArray();
                    vert.Format = att.SetTypeWiiU(att.Format);
                    vert.BufferIndex = att.BufferIndex;
                    atrib.Add(vert);
                }

                // Set _w and _i 
                for (int i = 0; i < fshp.weights.Count; i++)
                {
                    if (att.Name == "_w" + i.ToString())
                    {
                        VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                        vert.Name = att.Name;
                        vert.Data = fshp.weights[i].ToArray();
                        vert.Format = att.SetTypeWiiU(att.Format);
                        atrib.Add(vert);

                        for (int j = 0; j < fshp.weights.Count; j++)
                        {
                            Console.WriteLine($"w {j} {fshp.weights[j]}");
                        }

                    }
                    if (att.Name == "_i" + i.ToString())
                    {
                        VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                        vert.Name = att.Name;
                        vert.Data = fshp.boneInd[i].ToArray();
                        vert.Format = att.SetTypeWiiU(att.Format);
                        atrib.Add(vert);
                    }
                }
            }
            if (atrib.Count == 0)
            {
                MessageBox.Show("Attributes are empty?");
                return;
            }
            helper.Attributes = atrib;
            fshp.VertexBufferU = helper.ToVertexBuffer();
            fshp.VertexBufferU.VertexSkinCount = fshp.VertexSkinCount;
        }

        public static void WriteExternalFiles(ResFile resFile, TreeNode EditorRoot)
        {
            resFile.ExternalFiles.Clear();
            if (EditorRoot.Nodes.ContainsKey("EXT"))
            {
                foreach (TreeNode node in EditorRoot.Nodes["EXT"].Nodes)
                {
                    ExternalFile ext = new ExternalFile();
                    if (node is BNTX)
                    {
                        var mem = new System.IO.MemoryStream();
                        ((BNTX)node).Save(mem);
                        ext.Data = mem.ToArray();
                    }
                    else
                        ext.Data = ((ExternalFileData)node).Data;

                    resFile.ExternalFiles.Add(node.Text, ext);
                }
            }
        }
    }
}
