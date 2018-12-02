using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using Syroot.NintenTools.NSW.Bfres;
using Syroot.NintenTools.NSW.Bfres.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using Smash_Forge.Rendering;
using SFGraphics.GLObjects.Shaders;
using GL_Core;
using GL_Core.Interfaces;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using WeifenLuo.WinFormsUI.Docking;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using ResU = Syroot.NintenTools.Bfres;
using Bfres.Structs;

namespace FirstPlugin
{
    public class BFRESRender : AbstractGlDrawable
    {
        // gl buffer objects
        int vbo_position;
        int ibo_elements;

        public Shader shader = null;
        public List<FMDL> models = new List<FMDL>();

        public BFRES ResFileNode;

        public BFRESRender()
        {

        }
        public void DisposeFile()
        {
            Destroy();
            ResFileNode.Nodes.Clear();
            ResFileNode.Remove();
        }
        private void GenerateBuffers()
        {
            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out ibo_elements);
        }
        public void LoadFile(ResU.ResFile resFileU)
        {
            ResFileNode.Load(resFileU);

            BfresWiiU.Read(this, resFileU, ResFileNode);
            UpdateVertexData();
            UpdateTextureMaps();
        }
        public void LoadFile(ResFile resFile)
        {
            ResFileNode.Load(resFile);

            BfresSwitch.Read(this, resFile, ResFileNode);
            UpdateVertexData();
            UpdateTextureMaps();
        }

        public void Destroy()
        {
            GL.DeleteBuffer(vbo_position);
            GL.DeleteBuffer(ibo_elements);
        }

        #region Rendering

        bool RanOnce = false;

        public override void Prepare(GL_ControlModern control)
        {
        }

        public override void Prepare(GL_ControlLegacy control)
        {

        }

        private void DrawBoundingBoxes()
        {
            foreach (FMDL mdl in models)
            {
                foreach (FSHP m in mdl.shapes)
                {
                    if (m.IsSelected)
                        GL.Color4(Color.GhostWhite);
                    else
                        GL.Color4(Color.OrangeRed);


                    foreach (FSHP.BoundingBox box in m.boundingBoxes)
                    {
                        RenderTools.DrawRectangularPrism(box.Center, box.Extend.X, box.Extend.Y, box.Extend.Z, true);
                    }
                }
            }
        }

        public override void Draw(GL_ControlLegacy control)
        {
            bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
            if (!buffersWereInitialized)
                GenerateBuffers();

            if (OpenTKSharedResources.SetupStatus == OpenTKSharedResources.SharedResourceStatus.Unitialized)
                return;

            //  if (Runtime.renderBoundingBoxes)
            //    DrawBoundingBoxes();

            if (Runtime.viewportShading != Runtime.ViewportShading.Default)
                shader = OpenTKSharedResources.shaders["BFRES_Debug"];
            else
                shader = OpenTKSharedResources.shaders["BFRES"];

            shader.UseProgram();

            if (RanOnce == false)
            {
                //   ShaderTools.SaveErrorLogs();
                RanOnce = true;
            }

            Matrix4 camMat = control.mtxCam * control.mtxProj;

            shader.SetVector3("difLightDirection", Vector3.TransformNormal(new Vector3(0f, 0f, -1f), camMat.Inverted()).Normalized());
            shader.EnableVertexAttributes();
            SetRenderSettings(shader);

            shader.SetMatrix4x4("mvpMatrix", ref camMat);

            foreach (FMDL mdl in models)
            {
                foreach (FSHP shp in mdl.shapes)
                {
                    DrawModel(shp, mdl, shader);
                }
            }

            shader.DisableVertexAttributes();
            GL.UseProgram(0);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthTest);
        }

        public override void Draw(GL_ControlModern control)
        {
            bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
            if (!buffersWereInitialized)
                GenerateBuffers();

            if (OpenTKSharedResources.SetupStatus == OpenTKSharedResources.SharedResourceStatus.Unitialized)
                return;

            //     if (Runtime.renderBoundingBoxes)
            //         DrawBoundingBoxes();

            if (Runtime.viewportShading != Runtime.ViewportShading.Default)
                shader = OpenTKSharedResources.shaders["BFRES_Debug"];
            else
                shader = OpenTKSharedResources.shaders["BFRES"];

            shader.UseProgram();

            if (RanOnce == false)
            {
                //   ShaderTools.SaveErrorLogs();


                RanOnce = true;
            }
            Matrix4 previewScale = Utils.TransformValues(Vector3.Zero, Vector3.Zero, Runtime.previewScale);

            Matrix4 camMat = previewScale * control.mtxCam * control.mtxProj;

            shader.SetVector3("difLightDirection", Vector3.TransformNormal(new Vector3(0f, 0f, -1f), camMat.Inverted()).Normalized());
            shader.EnableVertexAttributes();
            SetRenderSettings(shader);

            shader.SetMatrix4x4("mvpMatrix", ref camMat);

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);

            foreach (FMDL mdl in models)
            {
                foreach (FSHP shp in mdl.shapes)
                {
                    DrawModel(shp, mdl, shader);
                }
            }

            shader.DisableVertexAttributes();
            GL.UseProgram(0);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthTest);
        }
        private void SetRenderSettings(Shader shader)
        {
            shader.SetBoolToInt("renderVertColor", Runtime.renderVertColor);
            shader.SetInt("renderType", (int)Runtime.viewportShading);
            shader.SetBoolToInt("useNormalMap", Runtime.useNormalMap);
            shader.SetInt("uvChannel", (int)Runtime.uvChannel);

            shader.SetBoolToInt("renderR", Runtime.renderR);
            shader.SetBoolToInt("renderG", Runtime.renderG);
            shader.SetBoolToInt("renderB", Runtime.renderB);
            shader.SetBoolToInt("renderAlpha", Runtime.renderAlpha);
            shader.SetInt("isTransparent", 1);
        }
        private static void SetDefaultTextureAttributes(FMAT mat, Shader shader)
        {
            shader.SetBoolToInt("HasDiffuse", mat.HasDiffuseMap);
            shader.SetBoolToInt("HasDiffuseLayer", mat.HasDiffuseLayer);
            shader.SetBoolToInt("HasNormalMap", mat.HasNormalMap);
            shader.SetBoolToInt("HasEmissionMap", mat.HasEmissionMap);
            shader.SetBoolToInt("HasLightMap", mat.HasLightMap);
            shader.SetBoolToInt("HasShadowMap", mat.HasShadowMap);
            shader.SetBoolToInt("HasSpecularMap", mat.HasSpecularMap);
            shader.SetBoolToInt("HasTeamColorMap", mat.HasTeamColorMap);
            shader.SetBoolToInt("HasSphereMap", mat.HasSphereMap);
            shader.SetBoolToInt("HasSubSurfaceScatteringMap", mat.HasSubSurfaceScatteringMap);

            //Unused atm untill I do PBR shader
            shader.SetBoolToInt("HasMetalnessMap", mat.HasMetalnessMap);
            shader.SetBoolToInt("HasRoughnessMap", mat.HasRoughnessMap);
            shader.SetBoolToInt("HasMRA", mat.HasMRA);
        }
        private static void SetBoneUniforms(Shader shader, FMDL fmdl)
        {
            for (int i = 0; i < fmdl.Skeleton.Node_Array.Length; i++)
            {
                Matrix4 transform = fmdl.Skeleton.bones[fmdl.Skeleton.Node_Array[i]].invert * fmdl.Skeleton.bones[fmdl.Skeleton.Node_Array[i]].transform;
                GL.UniformMatrix4(GL.GetUniformLocation(shader.Id, String.Format("bones[{0}]", i)), false, ref transform);
            }
        }
        private static void SetTextureUniforms(FMAT mat, FSHP m, Shader shader)
        {
            SetDefaultTextureAttributes(mat, shader);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.Id);

            shader.SetTexture("UVTestPattern", RenderTools.uvTestPattern, 10);

            GL.Uniform1(shader.GetAttribLocation("normalMap"), 0);
            GL.Uniform1(shader.GetAttribLocation("BakeShadowMap"), 0);

            // PBR IBL
            //      shader.SetTexture("irradianceMap", RenderTools.diffusePbr, 18);
            //      shader.SetTexture("specularIbl", RenderTools.specularPbr, 19);

            foreach (MatTexture matex in mat.textures)
            {
                if (matex.Type == MatTexture.TextureType.Diffuse)
                    TextureUniform(shader, mat, mat.HasDiffuseMap, "DiffuseMap", matex);
                else if (matex.Type == MatTexture.TextureType.Normal)
                    TextureUniform(shader, mat, mat.HasNormalMap, "NormalMap", matex);
                else if (matex.Type == MatTexture.TextureType.Emission)
                    TextureUniform(shader, mat, mat.HasEmissionMap, "EmissionMap", matex);
                else if (matex.Type == MatTexture.TextureType.Specular)
                    TextureUniform(shader, mat, mat.HasSpecularMap, "SpecularMap", matex);
                else if (matex.Type == MatTexture.TextureType.Shadow)
                    TextureUniform(shader, mat, mat.HasShadowMap, "BakeShadowMap", matex);
                else if (matex.Type == MatTexture.TextureType.Light)
                    TextureUniform(shader, mat, mat.HasLightMap, "BakeLightMap", matex);
                else if (matex.Type == MatTexture.TextureType.Metalness)
                    TextureUniform(shader, mat, mat.HasMetalnessMap, "MetalnessMap", matex);
                else if (matex.Type == MatTexture.TextureType.Roughness)
                    TextureUniform(shader, mat, mat.HasRoughnessMap, "RoughnessMap", matex);
                else if (matex.Type == MatTexture.TextureType.TeamColor)
                    TextureUniform(shader, mat, mat.HasTeamColorMap, "TeamColorMap", matex);
                else if (matex.Type == MatTexture.TextureType.Transparency)
                    TextureUniform(shader, mat, mat.HasTransparencyMap, "TransparencyMap", matex);
                else if (matex.Type == MatTexture.TextureType.DiffuseLayer2)
                    TextureUniform(shader, mat, mat.HasDiffuseLayer, "DiffuseLayer", matex);
                else if (matex.Type == MatTexture.TextureType.SphereMap)
                    TextureUniform(shader, mat, mat.HasSphereMap, "SphereMap", matex);
                else if (matex.Type == MatTexture.TextureType.SubSurfaceScattering)
                    TextureUniform(shader, mat, mat.HasSubSurfaceScatteringMap, "SubSurfaceScatteringMap", matex);
                else if (matex.Type == MatTexture.TextureType.MRA)
                    TextureUniform(shader, mat, mat.HasMRA, "MRA", matex);
            }
        }
        private static void TextureUniform(Shader shader, FMAT mat, bool hasTex, string name, MatTexture mattex)
        {
            // Bind the texture and create the uniform if the material has the right textures. 
            if (hasTex)
            {
                GL.Uniform1(shader.GetUniformLocation(name), BindTexture(mattex));
            }
        }
        public static int BindTexture(MatTexture tex)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + tex.hash + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.Id);

            if (BFRES.Instance.IsWiiU)
            {
                foreach (var ftexContainer in PluginRuntime.ftexContainers)
                {
                    if (ftexContainer.Textures.ContainsKey(tex.Name))
                    {
                        BindGLTexture(tex, ftexContainer.Textures[tex.Name].renderedTex.display);
                    }
                }
            }
            else
            {
                foreach (var bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(tex.Name))
                    {
                        BindGLTexture(tex, bntx.Textures[tex.Name].renderedGLTex.display);
                    }
                }
            }


            return tex.hash + 1;
        }
        private static void BindGLTexture(MatTexture tex, int texid)
        {
            //   GL.ActiveTexture(TextureUnit.Texture0 + texid);
            GL.BindTexture(TextureTarget.Texture2D, texid);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)MatTexture.wrapmode[tex.wrapModeS]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)MatTexture.wrapmode[tex.wrapModeT]);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, 0.0f);
        }
        private void DrawModel(FSHP m, FMDL mdl, Shader shader, bool drawSelection = false)
        {
            if (m.lodMeshes[m.DisplayLODIndex].faces.Count <= 3)
                return;

            SetUniforms(m.GetMaterial(), shader, m, m.DisplayId);
            SetTextureUniforms(m.GetMaterial(), m, shader);
            SetBoneUniforms(shader, mdl);
            SetVertexAttributes(m, shader);
            ApplyTransformFix(mdl, m, shader);

            if (m.Checked)
            {
                if ((m.IsSelected))
                {
                    DrawModelSelection(m, shader);
                }
                else
                {
                    if (Runtime.RenderModelWireframe)
                    {
                        DrawModelWireframe(m, shader);
                    }
                    if (Runtime.RenderModels)
                    {
                        PrimitiveType primitiveType = PrimitiveType.Triangles;

                        switch (m.lodMeshes[m.DisplayLODIndex].PrimitiveType)
                        {
                            case STPolygonType.Line:
                                primitiveType = PrimitiveType.Lines;
                                break;
                            case STPolygonType.LineStrip:
                                primitiveType = PrimitiveType.LineStrip;
                                break;
                            case STPolygonType.Point:
                                primitiveType = PrimitiveType.Points;
                                break;
                            case STPolygonType.Triangle:
                                primitiveType = PrimitiveType.Triangles;
                                break;
                        }

                        GL.DrawElements(primitiveType, m.lodMeshes[m.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, m.Offset);
                    }
                }
            }
        }
        private static void ApplyTransformFix(FMDL fmdl, FSHP m, Shader shader)
        {
            shader.SetInt("NoSkinning", 0);
            shader.SetInt("RigidSkinning", 0);
            //Some objects will have no weights or indices. These will weigh to the bone index in the shape section.
            shader.SetInt("SingleBoneIndex", m.boneIndx);

            if (m.VertexSkinCount == 1)
            {
                shader.SetInt("RigidSkinning", 1);
            }
            if (m.VertexSkinCount == 0)
            {
                Matrix4 transform = fmdl.Skeleton.bones[m.boneIndx].invert * fmdl.Skeleton.bones[m.boneIndx].transform;
                shader.SetMatrix4x4("singleBoneBindTransform", ref transform);

                shader.SetInt("NoSkinning", 1);
            }
        }
        public void UpdateVertexData()
        {
            if (OpenTKSharedResources.SetupStatus == OpenTKSharedResources.SharedResourceStatus.Unitialized)
                return;

            DisplayVertex[] Vertices;
            int[] Faces;

            int poffset = 0;
            int voffset = 0;
            List<DisplayVertex> Vs = new List<DisplayVertex>();
            List<int> Ds = new List<int>();

            foreach (FMDL mdl in models)
            {
                foreach (FSHP m in mdl.shapes)
                {
                    m.Offset = poffset * 4;
                    List<DisplayVertex> pv = m.CreateDisplayVertices();
                    Vs.AddRange(pv);

                    for (int i = 0; i < m.lodMeshes[m.DisplayLODIndex].displayFaceSize; i++)
                    {
                        Ds.Add(m.display[i] + voffset);
                    }
                    poffset += m.lodMeshes[m.DisplayLODIndex].displayFaceSize;
                    voffset += pv.Count;
                }
            }

            // Binds
            Vertices = Vs.ToArray();
            Faces = Ds.ToArray();

            // Bind only once!
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<DisplayVertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * DisplayVertex.Size), Vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            GL.BufferData<int>(BufferTarget.ElementArrayBuffer, (IntPtr)(Faces.Length * sizeof(int)), Faces, BufferUsageHint.StaticDraw);

            Viewport.Instance.UpdateViewport();
        }
        public void UpdateSingleMaterialTextureMaps(FMAT mat)
        {
            if (OpenTKSharedResources.SetupStatus == OpenTKSharedResources.SharedResourceStatus.Unitialized)
                return;

            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                foreach (var t in mat.textures)
                {
                    if (bntx.Textures.ContainsKey(t.Name))
                    {
                        if (!bntx.Textures[t.Name].GLInitialized)
                            bntx.Textures[t.Name].LoadOpenGLTexture();
                    }
                }
            }

            Viewport.Instance.UpdateViewport();
        }
        public void UpdateTextureMaps()
        {
            if (OpenTKSharedResources.SetupStatus == OpenTKSharedResources.SharedResourceStatus.Unitialized)
                return;

            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                if (!bntx.AllGLInitialized)
                {
                    foreach (var tex in bntx.Textures)
                    {
                        if (!tex.Value.GLInitialized)
                            tex.Value.LoadOpenGLTexture();
                    }
                }
            }
            foreach (FTEXContainer ftexCont in PluginRuntime.ftexContainers)
            {
                foreach (var tex in ftexCont.Textures)
                {
                    tex.Value.LoadOpenGLTexture();
                }
            }

            Viewport.Instance.UpdateViewport();
        }
        private static void SetUniforms(FMAT mat, Shader shader, FSHP m, int id)
        {
            shader.SetVector4("gsys_bake_st0", new Vector4(1, 1, 0, 0));
            shader.SetVector4("gsys_bake_st1", new Vector4(1, 1, 0, 0));
            shader.SetVector4("const_color0", new Vector4(1, 1, 1, 1));
            shader.SetVector4("base_color_mul_color", new Vector4(1, 1, 1, 1));
            
            shader.SetInt("enableCellShading", 0);
            bool HasTans = m.vertexAttributes.Any(x => x.Name == "_t0");
            shader.SetBoolToInt("hasTangents", HasTans);

            SetUniformData(mat, shader, "gsys_bake_st0");
            SetUniformData(mat, shader, "gsys_bake_st1");
            SetUniformData(mat, shader, "const_color0");
            SetUniformData(mat, shader, "base_color_mul_color");
        }
        private static void SetUniformData(FMAT mat, Shader shader, string propertyName)
        {
            if (mat.shaderassign.options.ContainsKey(propertyName))
            {
                float value = float.Parse(mat.shaderassign.options[propertyName]);
                shader.SetFloat(propertyName, value);
            }
            if (mat.matparam.ContainsKey(propertyName))
            {
                if (mat.matparam[propertyName].Type == ShaderParamType.Float)
                {
                    if (mat.anims.ContainsKey(propertyName))
                        mat.matparam[propertyName].ValueFloat[0] = mat.anims[propertyName][0];
                    shader.SetFloat(propertyName, mat.matparam[propertyName].ValueFloat[0]);
                }

                if (mat.matparam[propertyName].Type == ShaderParamType.Float2)
                {
                    if (mat.anims.ContainsKey(propertyName))
                    {
                        mat.matparam[propertyName].ValueFloat = new float[2] {
                            mat.anims[propertyName][0], mat.anims[propertyName][1]};
                    }

                    shader.SetVector2(propertyName, Utils.ToVec2(mat.matparam[propertyName].ValueFloat));
                }

                if (mat.matparam[propertyName].Type == ShaderParamType.Float3)
                {
                    if (mat.anims.ContainsKey(propertyName))
                    {
                        mat.matparam[propertyName].ValueFloat = new float[3] {
                            mat.anims[propertyName][0],
                            mat.anims[propertyName][1],
                            mat.anims[propertyName][2]};
                    }

                    shader.SetVector3(propertyName, Utils.ToVec3(mat.matparam[propertyName].ValueFloat));
                }
                if (mat.matparam[propertyName].Type == ShaderParamType.Float4)
                {
                    if (mat.anims.ContainsKey(propertyName))
                    {
                        mat.matparam[propertyName].ValueFloat = new float[4] {
                                     mat.anims[propertyName][0], mat.anims[propertyName][1],
                                     mat.anims[propertyName][2], mat.anims[propertyName][3]};
                    }

                    shader.SetVector4(propertyName, Utils.ToVec4(mat.matparam[propertyName].ValueFloat));
                }
                if (mat.matparam[propertyName].Type == ShaderParamType.TexSrt)
                {
                    // Vector 2 Scale
                    // 1 roation float
                    // Vector2 translate
                    TexSrt texSRT = mat.matparam[propertyName].ValueTexSrt;

                    shader.SetVector2("SRT_Scale", Utils.ToVec2(texSRT.Scaling));
                    shader.SetFloat("SRT_Rotate", texSRT.Rotation);
                    shader.SetVector2("SRT_Translate", Utils.ToVec2(texSRT.Translation));
                }

            }
        }
        private void SetVertexAttributes(FSHP m, Shader shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribLocation("vPosition"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 0);
            GL.VertexAttribPointer(shader.GetAttribLocation("vNormal"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 12);
            GL.VertexAttribPointer(shader.GetAttribLocation("vTangent"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 24);
            GL.VertexAttribPointer(shader.GetAttribLocation("vBitangent"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 36);
            GL.VertexAttribPointer(shader.GetAttribLocation("vUV0"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 48);
            GL.VertexAttribPointer(shader.GetAttribLocation("vColor"), 4, VertexAttribPointerType.Float, false, DisplayVertex.Size, 56);
            GL.VertexAttribIPointer(shader.GetAttribLocation("vBone"), 4, VertexAttribIntegerType.Int, DisplayVertex.Size, new IntPtr(72));
            GL.VertexAttribPointer(shader.GetAttribLocation("vWeight"), 4, VertexAttribPointerType.Float, false, DisplayVertex.Size, 88);
            GL.VertexAttribPointer(shader.GetAttribLocation("vUV1"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 104);
            GL.VertexAttribPointer(shader.GetAttribLocation("vUV2"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 112);
            GL.VertexAttribPointer(shader.GetAttribLocation("vPosition2"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 124);
            GL.VertexAttribPointer(shader.GetAttribLocation("vPosition3"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 136);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
        }

        private static void DrawModelWireframe(STGenericObject p, Shader shader)
        {
            // use vertex color for wireframe color
            shader.SetInt("colorOverride", 1);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(1.5f);
            GL.DrawElements(PrimitiveType.Triangles, p.lodMeshes[p.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            shader.SetInt("colorOverride", 0);
        }
        private static void DrawModelSelection(STGenericObject p, Shader shader)
        {
            //This part needs to be reworked for proper outline. Currently would make model disappear
            /*     GL.Enable(EnableCap.DepthTest);
                 GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

                 GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

                 GL.StencilMask(0x00);   

                 GL.StencilFunc(StencilFunction.Always, 1, 0xFF); // all fragments should update the stencil buffer
                 GL.StencilMask(0xFF); // enable writing to the stencil buffer
                 GL.DrawElements(PrimitiveType.Triangles, p.lodMeshes[p.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);

                 GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
                 GL.StencilMask(0x00); // enable writing to the stencil buffer
                 GL.Disable(EnableCap.DepthTest);

                 shader.SetInt("colorOverride", 1);

                 GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
                 GL.LineWidth(2.0f);
                 GL.DrawElements(PrimitiveType.Triangles, p.lodMeshes[p.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
                 GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                 shader.SetInt("colorOverride", 0);

                 GL.StencilMask(0xFF);
                 GL.Enable(EnableCap.DepthTest);*/

            // Override the model color with white in the shader.

            shader.SetInt("colorOverride", 1);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(1.3f);
            GL.DrawElements(PrimitiveType.Triangles, p.lodMeshes[p.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            shader.SetInt("colorOverride", 0);

            GL.DrawElements(PrimitiveType.Triangles, p.lodMeshes[p.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
        }

        #endregion
    }
}
