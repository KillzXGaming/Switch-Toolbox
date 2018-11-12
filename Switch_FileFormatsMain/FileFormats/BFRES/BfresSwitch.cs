using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.NintenTools.NSW.Bfres;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;

namespace FirstPlugin
{
    public static class BfresSwitch
    {
        public static void SetSkeleton(this TreeNodeCustom skl, Skeleton skeleton, BFRESRender.FSKL RenderableSkeleton)
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

            foreach (Bone bone in skeleton.Bones)
            {
                BFRESRender.BfresBone STBone = new BFRESRender.BfresBone(RenderableSkeleton);
                SetBone(STBone, bone);
                STBone.BFRESRender = RenderableSkeleton.node.BFRESRender; //to update viewport on bone edits
                RenderableSkeleton.bones.Add(STBone);
            }
            RenderableSkeleton.update();
            RenderableSkeleton.reset();

            foreach (var bone in RenderableSkeleton.bones)
                if (bone.Parent == null)
                    skl.Nodes.Add(bone);

            Runtime.abstractGlDrawables.Add(RenderableSkeleton);
        }
        public static void SetBone(this BFRESRender.BfresBone bone, Bone bn)
        {
            bone.Bone = bn;
            bone.Text = bn.Name;
            bone.BillboardIndex = bn.BillboardIndex;
            bone.parentIndex = bn.ParentIndex;
            bone.scale = new float[3];
            bone.rotation = new float[4];
            bone.position = new float[3];
            if (bn.FlagsRotation == BoneFlagsRotation.Quaternion)
                bone.boneRotationType = 1;
            else
                bone.boneRotationType = 0;
            bone.scale[0] = bn.Scale.X;
            bone.scale[1] = bn.Scale.Y;
            bone.scale[2] = bn.Scale.Z;
            bone.rotation[0] = bn.Rotation.X;
            bone.rotation[1] = bn.Rotation.Y;
            bone.rotation[2] = bn.Rotation.Z;
            bone.rotation[3] = bn.Rotation.W;
            bone.position[0] = bn.Position.X;
            bone.position[1] = bn.Position.Y;
            bone.position[2] = bn.Position.Z;
        }
        public static void SetShape(this BFRESRender.FSHP s, Shape shp)
        {
            shp.Name = s.Text;
            shp.MaterialIndex = (ushort)s.MaterialIndex;
        }
        public static void CreateNewMaterial(string Name)
        {
            BFRESRender.FMAT mat = new BFRESRender.FMAT();
            mat.Text = Name;
            mat.Material = new Material();

            SetMaterial(mat, mat.Material);
        }

        public static void SetMaterial(this BFRESRender.FMAT m, Material mat)
        {
            mat.Name = m.Text;

            if (m.Enabled)
                mat.Flags = MaterialFlags.Visible;
            else
                mat.Flags = MaterialFlags.None;

            if (mat.ShaderParamData == null)
                mat.ShaderParamData = new byte[0];

            byte[] ParamData = WriteShaderParams(m, mat);

            if (ParamData.Length != mat.ShaderParamData.Length)
                throw new Exception("Param size mis match!");
            else
                mat.ShaderParamData = ParamData;

            WriteRenderInfo(m, mat);
            WriteTextureRefs(m, mat);
            WriteShaderAssign(m.shaderassign, mat);
        }
        public static void ReadMaterial(this BFRESRender.FMAT m, Material mat)
        {
            if (mat.Flags == MaterialFlags.Visible)
                m.Enabled = true;
            else
                m.Enabled = false;

            m.ReadRenderInfo(mat);
            m.ReadShaderAssign(mat);
            m.SetActiveGame();
            m.ReadShaderParams(mat);
            m.Material = mat;
            m.ReadTextureRefs(mat);
        }
        public static void ReadTextureRefs(this BFRESRender.FMAT m, Material mat)
        {
            m.textures.Clear();

            int AlbedoCount = 0;
            int id = 0;
            string TextureName = "";
            if (mat.TextureRefs == null)
                mat.TextureRefs = new List<string>();

            foreach (string tex in mat.TextureRefs)
            {
                TextureName = tex;

                BFRESRender.MatTexture texture = new BFRESRender.MatTexture();

                texture.wrapModeS = (int)mat.Samplers[id].WrapModeU;
                texture.wrapModeT = (int)mat.Samplers[id].WrapModeV;
                texture.wrapModeW = (int)mat.Samplers[id].WrapModeW;
                texture.SamplerName = mat.SamplerDict.GetKey(id);

                bool IsAlbedo = BFRESRender.HackyTextureList.Any(TextureName.Contains);

                if (Runtime.activeGame == Runtime.ActiveGame.MK8D)
                {
                    if (texture.SamplerName == "_a0")
                    {
                        if (AlbedoCount == 0)
                        {
                            m.HasDiffuseMap = true;
                            AlbedoCount++;
                            texture.hash = 0;
                            texture.Type = BFRESRender.MatTexture.TextureType.Diffuse;
                        }
                    }
                    if (texture.SamplerName == "_n0")
                    {
                        texture.hash = 1;
                        m.HasNormalMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Normal;
                    }
                    if (texture.SamplerName == "_e0")
                    {
                        texture.hash = 8;
                        m.HasEmissionMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Emission;
                    }
                    if (texture.SamplerName == "_s0")
                    {
                        texture.hash = 4;
                        m.HasSpecularMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Specular;
                    }
                    if (texture.SamplerName == "_x0")
                    {
                        texture.hash = 6;
                        m.HasSphereMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.SphereMap;
                    }
                    if (texture.SamplerName == "_b0")
                    {
                        texture.hash = 2;
                        m.HasShadowMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Shadow;
                    }
                    if (texture.SamplerName == "_b1")
                    {
                        texture.hash = 3;
                        m.HasLightMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Light;
                    }
                }
                else if (Runtime.activeGame == Runtime.ActiveGame.SMO)
                {
                    if (texture.SamplerName == "_a0")
                    {
                        if (AlbedoCount == 0)
                        {
                            m.HasDiffuseMap = true;
                            AlbedoCount++;
                            texture.hash = 0;
                            texture.Type = BFRESRender.MatTexture.TextureType.Diffuse;
                        }
                    }
                    if (texture.SamplerName == "_n0")
                    {
                        texture.hash = 1;
                        m.HasNormalMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Normal;
                    }
                    if (texture.SamplerName == "_e0")
                    {
                        texture.hash = 8;
                        m.HasEmissionMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Emission;
                    }
                    if (TextureName.Contains("mtl"))
                    {
                        texture.hash = 16;
                        m.HasMetalnessMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Metalness;
                    }
                    else if (TextureName.Contains("rgh"))
                    {
                        texture.Type = BFRESRender.MatTexture.TextureType.Roughness;
                        texture.hash = 18;
                        m.HasRoughnessMap = true;
                    }
                    else if (TextureName.Contains("sss"))
                    {

                        texture.Type = BFRESRender.MatTexture.TextureType.SubSurfaceScattering;
                        texture.hash = 19;
                        m.HasSubSurfaceScatteringMap = true;
                    }
                }
                else
                {
                    //This works decently for now. I tried samplers but Kirby Star Allies doesn't map with samplers properly? 
                    if (IsAlbedo)
                    {
                        if (AlbedoCount == 0)
                        {
                            m.HasDiffuseMap = true;
                            AlbedoCount++;
                            texture.hash = 0;
                            texture.Type = BFRESRender.MatTexture.TextureType.Diffuse;
                        }
                        if (AlbedoCount == 1)
                        {
                            //   poly.material.HasDiffuseLayer = true;
                            //    texture.hash = 19;
                            //   texture.Type = MatTexture.TextureType.DiffuseLayer2;

                        }
                    }

                    else if (TextureName.Contains("Nrm") || TextureName.Contains("Norm") || TextureName.Contains("norm") || TextureName.Contains("nrm"))
                    {
                        texture.hash = 1;
                        m.HasNormalMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Normal;
                    }
                    else if (TextureName.Contains("Emm"))
                    {
                        texture.hash = 8;
                        m.HasEmissionMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Emission;
                    }
                    else if (TextureName.Contains("Spm"))
                    {
                        texture.hash = 4;
                        m.HasSpecularMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Specular;
                    }
                    else if (TextureName.Contains("b00"))
                    {
                        texture.hash = 2;
                        m.HasShadowMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Shadow;
                    }
                    else if (TextureName.Contains("Moc") || TextureName.Contains("AO"))
                    {
                        texture.hash = 2;
                        m.HasAmbientOcclusionMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.AO;
                    }
                    else if (TextureName.Contains("b01"))
                    {
                        texture.hash = 3;
                        m.HasLightMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Light;
                    }
                    else if (TextureName.Contains("MRA")) //Metalness, Roughness, and Cavity Map in one
                    {
                        texture.hash = 17;
                        m.HasRoughnessMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.MRA;
                    }
                    else if (TextureName.Contains("mtl"))
                    {
                        texture.hash = 16;
                        m.HasMetalnessMap = true;
                        texture.Type = BFRESRender.MatTexture.TextureType.Metalness;
                    }
                    else if (TextureName.Contains("rgh"))
                    {
                        texture.Type = BFRESRender.MatTexture.TextureType.Roughness;
                        texture.hash = 18;
                        m.HasRoughnessMap = true;
                    }
                    else if (TextureName.Contains("sss"))
                    {

                        texture.Type = BFRESRender.MatTexture.TextureType.SubSurfaceScattering;
                        texture.hash = 19;
                        m.HasSubSurfaceScatteringMap = true;
                    }
                }
                
                texture.Name = TextureName;

                m.textures.Add(texture);

                id++;
            }
        }

        public static void ReadShaderParams(this BFRESRender.FMAT m, Material mat)
        {
            m.matparam.Clear();

            if (mat.ShaderParamData == null)
                return;

            using (FileReader reader = new FileReader(new System.IO.MemoryStream(mat.ShaderParamData)))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                foreach (ShaderParam param in mat.ShaderParams)
                {
                    BFRESRender.BfresShaderParam shaderParam = new BFRESRender.BfresShaderParam();
                    shaderParam.Type = param.Type;
                    shaderParam.Name = param.Name;

                    reader.Seek(param.DataOffset, System.IO.SeekOrigin.Begin);
                    shaderParam.ReadValue(reader, (int)param.DataSize);

                    m.matparam.Add(param.Name, shaderParam);
                }
                reader.Close();
            }
        }
        public static byte[] WriteShaderParams(this BFRESRender.FMAT m, Material mat)
        {
            mat.ShaderParams = new List<ShaderParam>();

            System.IO.MemoryStream mem = new System.IO.MemoryStream();
            using (FileWriter writer = new FileWriter(mem))
            {
                uint Offset = 0;
                int index = 0;
                writer.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                foreach (BFRESRender.BfresShaderParam shaderParam in m.matparam.Values)
                {
                    ShaderParam param = new ShaderParam();
                    param.Name = shaderParam.Name;
                    param.Type = shaderParam.Type;
                    param.DataOffset = (ushort)Offset;
                    param.DependedIndex = (ushort)index;
                    param.DependIndex = (ushort)index;

                    writer.Seek(param.DataOffset, System.IO.SeekOrigin.Begin);
                    shaderParam.WriteValue(writer);

                    Offset += param.DataSize;
                    mat.ShaderParams.Add(param);
                    index++;
                }
                writer.Close();
            }
            return mem.ToArray();
        }
        public static void ReadRenderInfo(this BFRESRender.FMAT m, Material mat)
        {
            m.renderinfo.Clear();

            foreach (RenderInfo rnd in mat.RenderInfos)
            {
                BFRESRender.BfresRenderInfo r = new BFRESRender.BfresRenderInfo();
                r.Name = rnd.Name;
                r.Type = rnd.Type;
                switch (rnd.Type)
                {
                    case RenderInfoType.Int32: r.ValueInt = rnd.GetValueInt32s(); break;
                    case RenderInfoType.Single: r.ValueFloat = rnd.GetValueSingles(); break;
                    case RenderInfoType.String: r.ValueString = rnd.GetValueStrings(); break;
                }
                m.renderinfo.Add(r);
            }
        }
        public static void WriteTextureRefs(this BFRESRender.FMAT m, Material mat)
        {
            mat.TextureRefs = new List<string>();
            mat.TextureRefs.Clear();

            foreach (var textu in m.textures)
                mat.TextureRefs.Add(textu.Name);
            
        }
 
        public static void WriteRenderInfo(this BFRESRender.FMAT m, Material mat)
        {
            if (mat.RenderInfos == null)
                mat.RenderInfos = new List<RenderInfo>();

            mat.RenderInfos.Clear();
            foreach (BFRESRender.BfresRenderInfo rnd in m.renderinfo)
            {
                RenderInfo r = new RenderInfo();
                r.Name = rnd.Name;
                switch (rnd.Type)
                {
                    case RenderInfoType.Int32: r.SetValue(rnd.ValueInt); break;
                    case RenderInfoType.Single: r.SetValue(rnd.ValueFloat); break;
                    case RenderInfoType.String: r.SetValue(rnd.ValueString); break;
                }
                mat.RenderInfos.Add(r);
            }
        }
        public static void ReadShaderAssign(this BFRESRender.FMAT m, Material mat)
        {
            m.shaderassign = new BFRESRender.FMAT.ShaderAssign();

            if (mat.ShaderAssign == null)
                mat.ShaderAssign = new ShaderAssign();
            if (mat.ShaderAssign.ShaderOptions == null)
                mat.ShaderAssign.ShaderOptions = new List<string>();
            if (mat.ShaderAssign.AttribAssigns == null)
                mat.ShaderAssign.AttribAssigns = new List<string>();
            if (mat.ShaderAssign.SamplerAssigns == null)
                mat.ShaderAssign.SamplerAssigns = new List<string>();

            m.shaderassign.options.Clear();
            m.shaderassign.samplers.Clear();
            m.shaderassign.attributes.Clear();

            m.shaderassign = new BFRESRender.FMAT.ShaderAssign();
            m.shaderassign.ShaderArchive = mat.ShaderAssign.ShaderArchiveName;
            m.shaderassign.ShaderModel = mat.ShaderAssign.ShadingModelName;

            for (int op = 0; op < mat.ShaderAssign.ShaderOptions.Count; op++)
                m.shaderassign.options.Add(mat.ShaderAssign.ShaderOptionDict.GetKey(op), mat.ShaderAssign.ShaderOptions[op]);

            if (mat.ShaderAssign.SamplerAssigns != null)
            {
                for (int op = 0; op < mat.ShaderAssign.SamplerAssigns.Count; op++)
                    m.shaderassign.samplers.Add(mat.ShaderAssign.SamplerAssignDict.GetKey(op), mat.ShaderAssign.SamplerAssigns[op]);
            }
            if (mat.ShaderAssign.AttribAssigns != null)
            {
                for (int op = 0; op < mat.ShaderAssign.AttribAssigns.Count; op++)
                    m.shaderassign.attributes.Add(mat.ShaderAssign.AttribAssignDict.GetKey(op), mat.ShaderAssign.AttribAssigns[op]);
            }

     
            
       
            
        }
        public static void WriteShaderAssign(this BFRESRender.FMAT.ShaderAssign shd, Material mat)
        {
            mat.ShaderAssign = new ShaderAssign();
            mat.ShaderAssign.ShaderOptionDict = new ResDict();
            mat.ShaderAssign.SamplerAssignDict = new ResDict();
            mat.ShaderAssign.AttribAssignDict = new ResDict();
            mat.ShaderAssign.ShaderOptions = new List<string>();
            mat.ShaderAssign.AttribAssigns = new List<string>();
            mat.ShaderAssign.SamplerAssigns = new List<string>();

            mat.ShaderAssign.ShaderArchiveName = shd.ShaderArchive;
            mat.ShaderAssign.ShadingModelName = shd.ShaderModel;

            foreach (var option in shd.options)
            {
                mat.ShaderAssign.ShaderOptionDict.Add(option.Key);
                mat.ShaderAssign.ShaderOptions.Add(option.Value);
            }
            foreach (var samp in shd.samplers)
            {
                mat.ShaderAssign.SamplerAssignDict.Add(samp.Key);
                mat.ShaderAssign.SamplerAssigns.Add(samp.Value);
            }
            foreach (var att in shd.attributes)
            {
                mat.ShaderAssign.AttribAssignDict.Add(att.Key);
                mat.ShaderAssign.AttribAssigns.Add(att.Value);
            }
        }
    }
}
