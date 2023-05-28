using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Syroot.NintenTools.NSW.Bfres;
using Syroot.NintenTools.NSW.Bfres.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using Toolbox.Library;
using Toolbox.Library.Rendering;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using ResU = Syroot.NintenTools.Bfres;
using Bfres.Structs;
using SF = SFGraphics.GLObjects.Shaders;

namespace FirstPlugin
{
    public class BFRESRender : BFRESRenderBase
    {
        Vector3 position = new Vector3(0);

        public static Vector4 hoverColor = new Vector4(1);
        public static Vector4 selectColor = new Vector4(1);

        protected bool Selected = false;
        public bool Hovered = false;

        public bool IsSelected() => Selected;

        public BFRESRender()
        {

        }

        #region Rendering

        public override void Prepare(GL_ControlModern control)
        {
        }

        public override void Prepare(GL_ControlLegacy control)
        {
        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            if (Disposing || pass == Pass.TRANSPARENT) return;

            bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
            if (!buffersWereInitialized)
                GenerateBuffers();

            if (!Runtime.OpenTKInitialized)
                return;

            Matrix4 mvpMat = control.ModelMatrix * control.CameraMatrix * control.ProjectionMatrix;

            Matrix4 invertedCamera = Matrix4.Identity;
            if (invertedCamera.Determinant != 0)
                invertedCamera = mvpMat.Inverted();

            Vector3 lightDirection = new Vector3(0f, 0f, -1f);
            Vector3 difLightDirection  = Vector3.TransformNormal(lightDirection, invertedCamera).Normalized();

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);

            foreach (var model in models)
            {
                foreach (var shape in model.shapes)
                {
                    if (Runtime.RenderModels && model.Checked && shape.Checked)
                    {
                        var mat = shape.GetMaterial();

                        List<int> faces = shape.lodMeshes[shape.DisplayLODIndex].getDisplayFace();

                        GL.Begin(PrimitiveType.Triangles);
                        foreach (var index in faces)
                        {
                            Vertex vert = shape.vertices[index];
                            float normal = Vector3.Dot(difLightDirection, vert.nrm) * 0.5f + 0.5f;
                            GL.Color3(new Vector3(normal));
                            GL.TexCoord2(vert.uv0);
                            GL.Vertex3(vert.pos);
                        }
                        GL.End();
                    }
                }
            }

            GL.Enable(EnableCap.Texture2D);
        }

        public override void Draw(GL_ControlModern control, Pass pass) {
            DrawBfres(control, pass);
        }

        private void DrawBfres(GL_ControlModern control, Pass pass)
        {
            if (!Runtime.OpenTKInitialized || pass == Pass.TRANSPARENT || Disposing)
                return;

            bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
            if (!buffersWereInitialized)
                GenerateBuffers();

            if (Hovered == true)
                throw new Exception("model selected");

            //Temporarily revert to using this shader system as it is easy to port back
            //This is much quicker. Will change after shaders are handled faster
            SF.Shader shader = OpenTKSharedResources.shaders["BFRES"];

            if (Runtime.EnablePBR)
                shader = OpenTKSharedResources.shaders["BFRES_PBR"];

            if (models.Count > 0)
            {
                if (models[0].shapes.Count > 0)
                {
                    if (models[0].shapes[0].GetFMAT().shaderassign.ShaderModel == "uking_mat")
                    {
                        shader = OpenTKSharedResources.shaders["BFRES_Botw"];

                        //Botw uses small models so lower the bone size
                        Runtime.bonePointSize = 0.040f;
                    }
                }
            }

            if (Runtime.viewportShading != Runtime.ViewportShading.Default)
                shader = OpenTKSharedResources.shaders["BFRES_Debug"];

            if (Runtime.viewportShading == Runtime.ViewportShading.Lighting && Runtime.EnablePBR)
                shader = OpenTKSharedResources.shaders["BFRES_PBR"];

            shader.UseProgram();
            control.UpdateModelMatrix(ModelTransform * Matrix4.CreateScale(Runtime.previewScale));

            Matrix4 camMat = control.CameraMatrix;
            Matrix4 mdlMat = control.ModelMatrix;
            Matrix4 projMat = control.ProjectionMatrix;
            Matrix4 computedCamMtx = camMat * projMat;
            Matrix4 mvpMat = control.ModelMatrix * control.CameraMatrix * control.ProjectionMatrix;

            Matrix4 sphereMatrix = mvpMat;

            Matrix4 invertedCamera = Matrix4.Identity;
         //   invertedCamera = mvpMat.Inverted();
          //  if (invertedCamera.Determinant == 0)
          //      invertedCamera = Matrix4.Identity;

            sphereMatrix = invertedCamera;
            sphereMatrix.Transpose();

            invertedCamera = mvpMat.Inverted();
            Vector3 lightDirection = new Vector3(0f, 0f, -1f);

            shader.SetVector3("specLightDirection", Vector3.TransformNormal(lightDirection, invertedCamera).Normalized());
            shader.SetVector3("difLightDirection", Vector3.TransformNormal(lightDirection, invertedCamera).Normalized());

            shader.SetMatrix4x4("sphereMatrix", ref sphereMatrix);

            shader.SetMatrix4x4("mtxCam", ref computedCamMtx);
            shader.SetMatrix4x4("mtxMdl", ref mdlMat);
            shader.SetVector3("cameraPosition", control.CameraPosition);

            Vector4 pickingColor = control.NextPickingColor();

            shader.SetVector3("difLightColor", new Vector3(1));
            shader.SetVector3("ambLightColor", new Vector3(1));

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);

            DrawModels(shader, control);

            if (Runtime.renderNormalsPoints)
            {
                shader = OpenTKSharedResources.shaders["BFRES_Normals"];
                shader.UseProgram();

                shader.SetMatrix4x4("camMtx", ref camMat);
                shader.SetMatrix4x4("mtxProj", ref projMat);
                shader.SetMatrix4x4("mtxCam", ref computedCamMtx);
                shader.SetMatrix4x4("mtxMdl", ref mdlMat);

                shader.SetFloat("normalsLength", Runtime.normalsLineLength);

                DrawModels(shader, control);
            }

            GL.UseProgram(0);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        private void DrawModels(SF.Shader shader, GL_ControlModern control)
        {
            shader.EnableVertexAttributes();
            for (int m = 0; m < models.Count; m++)
            {
                if (models[m].Checked)
                {
                    List<FSHP> opaque = new List<FSHP>();
                    List<FSHP> transparent = new List<FSHP>();

                    for (int shp = 0; shp < models[m].shapes.Count; shp++)
                    {
                        if (models[m].shapes[shp].GetFMAT().isTransparent)
                            transparent.Add(models[m].shapes[shp]);
                        else
                            opaque.Add(models[m].shapes[shp]);
                    }

                    for (int shp = 0; shp < transparent.Count; shp++)
                    {
                        DrawModel(transparent[shp], models[m], shader, models[m].IsSelected);
                    }

                    for (int shp = 0; shp < opaque.Count; shp++)
                    {
                        DrawModel(opaque[shp], models[m], shader, models[m].IsSelected);
                    }
                }
            }

            shader.DisableVertexAttributes();
        }

        private void SetRenderSettings(SF.Shader shader, bool useVertexColors)
        {
            shader.SetBoolToInt("renderVertColor", Runtime.renderVertColor && useVertexColors);
            shader.SetBoolToInt("useNormalMap", Runtime.useNormalMap);
            shader.SetBoolToInt("renderR", Runtime.renderR);
            shader.SetBoolToInt("renderG", Runtime.renderG);
            shader.SetBoolToInt("renderB", Runtime.renderB);
            shader.SetBoolToInt("renderAlpha", Runtime.renderAlpha);

            shader.SetInt("renderType", (int)Runtime.viewportShading);
            shader.SetInt("uvChannel", (int)Runtime.uvChannel);

            shader.SetBoolToInt("renderFog", Runtime.renderFog);

            shader.SetBoolToInt("renderDiffuse", Runtime.renderDiffuse);
            shader.SetBoolToInt("renderSpecular", Runtime.renderSpecular);
            shader.SetBoolToInt("renderFresnel", Runtime.renderFresnel);
        }
        private static void SetDefaultTextureAttributes(FMAT mat, SF.Shader shader)
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
        private static void SetBoneUniforms(SF.Shader shader, FMDL fmdl, FSHP fshp)
        {
            for (int i = 0; i < fmdl.Skeleton.Node_Array.Length; i++)
            {
                GL.Uniform1(GL.GetUniformLocation(shader.Id, String.Format("boneIds[{0}]", i)), fmdl.Skeleton.Node_Array[i]);

                Matrix4 transform = fmdl.Skeleton.bones[fmdl.Skeleton.Node_Array[i]].invert * fmdl.Skeleton.bones[fmdl.Skeleton.Node_Array[i]].Transform;
                GL.UniformMatrix4(GL.GetUniformLocation(shader.Id, String.Format("bones[{0}]", i)), false, ref transform);
            }
        }

        private static void SetTextureUniforms(FMAT mat, FSHP m, SF.Shader shader)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.RenderableTex.TexID);

            GL.ActiveTexture(TextureUnit.Texture11);
            GL.Uniform1(shader.GetUniformLocation("weightRamp1"), 11);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.BoneWeightGradient.Id);

            GL.ActiveTexture(TextureUnit.Texture12);
            GL.Uniform1(shader.GetUniformLocation("weightRamp2"), 12);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.BoneWeightGradient2.Id);

            GL.Uniform1(shader.GetUniformLocation("debugOption"), 2);


            GL.ActiveTexture(TextureUnit.Texture10);
            GL.Uniform1(shader.GetUniformLocation("UVTestPattern"), 10);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.uvTestPattern.RenderableTex.TexID);

            GL.Uniform1(shader.GetUniformLocation("normalMap"), 0);
            GL.Uniform1(shader.GetUniformLocation("BakeShadowMap"), 0);

            shader.SetInt("RedChannel", 0);
            shader.SetInt("GreenChannel", 1);
            shader.SetInt("BlueChannel", 2);
            shader.SetInt("AlphaChannel", 3);

            LoadPBRMaps(shader);

            for (int t = 0; t < mat.TextureMaps.Count; t++)
            {
                MatTexture matex = (MatTexture)mat.TextureMaps[t];

                if (matex.Type == MatTexture.TextureType.Diffuse)
                    mat.HasDiffuseMap = TextureUniform(shader, mat, "DiffuseMap", matex);
                else if (matex.Type == MatTexture.TextureType.Normal)
                    mat.HasNormalMap = TextureUniform(shader, mat, "NormalMap", matex);
                else if (matex.Type == MatTexture.TextureType.Emission)
                    mat.HasEmissionMap = TextureUniform(shader, mat, "EmissionMap", matex);
                else if (matex.Type == MatTexture.TextureType.Specular)
                    mat.HasSpecularMap = TextureUniform(shader, mat, "SpecularMap", matex);
                else if (matex.Type == MatTexture.TextureType.Shadow)
                    mat.HasShadowMap = TextureUniform(shader, mat, "BakeShadowMap", matex);
                else if (matex.Type == MatTexture.TextureType.Light)
                    mat.HasLightMap = TextureUniform(shader, mat, "BakeLightMap", matex);
                else if (matex.Type == MatTexture.TextureType.Metalness)
                    mat.HasMetalnessMap = TextureUniform(shader, mat, "MetalnessMap", matex);
                else if (matex.Type == MatTexture.TextureType.Roughness)
                    mat.HasRoughnessMap = TextureUniform(shader, mat, "RoughnessMap", matex);
                else if (matex.Type == MatTexture.TextureType.TeamColor)
                    mat.HasTeamColorMap = TextureUniform(shader, mat, "TeamColorMap", matex);
                else if (matex.Type == MatTexture.TextureType.Transparency)
                    mat.HasTransparencyMap =  TextureUniform(shader, mat,  "TransparencyMap", matex);
                else if (matex.Type == MatTexture.TextureType.DiffuseLayer2)
                    mat.HasDiffuseLayer = TextureUniform(shader, mat, "DiffuseLayer", matex);
                else if (matex.Type == MatTexture.TextureType.SphereMap)
                    mat.HasSphereMap =  mat.HasSphereMap = TextureUniform(shader, mat, "SphereMap", matex);
                else if (matex.Type == MatTexture.TextureType.SubSurfaceScattering)
                    mat.HasSubSurfaceScatteringMap = TextureUniform(shader, mat, "SubSurfaceScatteringMap", matex);
                else if (matex.Type == MatTexture.TextureType.MRA)
                    mat.HasMRA = TextureUniform(shader, mat, "MRA", matex);
            }

            SetDefaultTextureAttributes(mat, shader);
        }

        private static void LoadPBRMaps(SF.Shader shader)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + 26);
            RenderTools.specularPbr.Bind();
            GL.Uniform1(shader.GetUniformLocation("specularIbl"), 26);

            //    GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

            // PBR IBL
            GL.ActiveTexture(TextureUnit.Texture0 + 25);
            RenderTools.diffusePbr.Bind();
            GL.Uniform1(shader.GetUniformLocation("irradianceMap"), 25);

            GL.ActiveTexture(TextureUnit.Texture0 + 27);
            RenderTools.brdfPbr.Bind();
            GL.Uniform1(shader.GetUniformLocation("brdfLUT"), 27);
        }

        private static bool TextureUniform(SF.Shader shader, FMAT mat, string name, MatTexture mattex)
        {
            if (mattex.textureState == STGenericMatTexture.TextureState.Binded)
                return true;

            // Bind the texture and create the uniform if the material has the right textures. 
            bool IsBound = BindTexture(mattex, mat, shader, mat.GetResFileU() != null);
            int texId = mattex.textureUnit + 1;

            if (IsBound)
                GL.Uniform1(shader.GetUniformLocation(name), texId);
            else
                return false;

            return true;
        }
        public static bool BindTexture(MatTexture tex, FMAT material, SF.Shader shader, bool IsWiiU)
        {
            BFRES bfres = (BFRES)material.Parent.Parent.Parent.Parent;

            GL.ActiveTexture(TextureUnit.Texture0 + tex.textureUnit + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.RenderableTex.TexID);

            string activeTex = tex.Name;
            if (tex.animatedTexName != "")
                activeTex = tex.animatedTexName;

            if (IsWiiU)
            {
                if (bfres.HasTextures)
                {
                    var ftexCont = bfres.GetFTEXContainer;
                    if (ftexCont != null)
                    {
                        if (ftexCont.ResourceNodes.ContainsKey(activeTex))
                        {
                            return BindFTEX(ftexCont, tex, shader, activeTex);
                        }
                    }
                }

                foreach (var ftexContainer in PluginRuntime.ftexContainers)
                {
                    if (ftexContainer.ResourceNodes.ContainsKey(activeTex))
                    {
                        return BindFTEX(ftexContainer, tex, shader, activeTex);
                    }
                }
            }
            else
            {
                if (bfres.HasTextures)
                {
                    var bntx = bfres.GetBNTX;
                    if (bntx != null)
                    {
                        if (bntx.Textures.ContainsKey(activeTex))
                        {
                            return BindBNTX(bntx, tex, shader, activeTex);
                        }
                    }
                }

                foreach (var bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(activeTex))
                    {
                        return BindBNTX(bntx, tex, shader, activeTex);
                    }
                }

                foreach (var bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(activeTex))
                    {
                        return BindBNTX(bntx, tex, shader, activeTex);
                    }
                }
                if (PluginRuntime.TextureCache.ContainsKey(activeTex))
                {
                    var t = PluginRuntime.TextureCache[activeTex];
                    if (t.RenderableTex == null || !t.RenderableTex.GLInitialized)
                        t.LoadOpenGLTexture();

                    BindGLTexture(tex, shader, t);
                }
            }

            return true;
        }

        private static bool BindFTEX(BFRESGroupNode ftexContainer, MatTexture tex, SF.Shader shader, string activeTex)
        {
            FTEX ftex = (FTEX)ftexContainer.ResourceNodes[activeTex];

            if (ftex.RenderableTex == null || !ftex.RenderableTex.GLInitialized)
                ftex.LoadOpenGLTexture();

            BindGLTexture(tex, shader, ftex);

            return ftex.RenderableTex.GLInitialized;
        }

        private static bool BindBNTX(BNTX bntx, MatTexture tex, SF.Shader shader, string activeTex)
        {
            if (bntx.Textures[activeTex].RenderableTex == null ||
                     !bntx.Textures[activeTex].RenderableTex.GLInitialized)
            {
                bntx.Textures[activeTex].LoadOpenGLTexture();
            }

            BindGLTexture(tex, shader, bntx.Textures[activeTex]);

            return bntx.Textures[activeTex].RenderableTex.GLInitialized;
        }

        private static void BindGLTexture(MatTexture tex, SF.Shader shader, STGenericTexture texture)
        {
            //If the texture is still not initialized then return
            if (!texture.RenderableTex.GLInitialized)
                return;

            if (tex.Type == STGenericMatTexture.TextureType.Diffuse)
            {
                shader.SetInt("RedChannel", (int)texture.RedChannel);
                shader.SetInt("GreenChannel", (int)texture.GreenChannel);
                shader.SetInt("BlueChannel", (int)texture.BlueChannel);
                shader.SetInt("AlphaChannel", (int)texture.AlphaChannel);
            }


            //     GL.ActiveTexture(TextureUnit.Texture0 + texid);
            GL.BindTexture(TextureTarget.Texture2D, texture.RenderableTex.TexID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)MatTexture.wrapmode[tex.WrapModeS]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)MatTexture.wrapmode[tex.WrapModeT]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MatTexture.minfilter[tex.MinFilter]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MatTexture.magfilter[tex.MagFilter]);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, 0.0f);
        }

        private void DrawModel(FSHP m, FMDL mdl, SF.Shader shader, bool ModelSelected)
        {
            if (m.lodMeshes[m.DisplayLODIndex].faces.Count <= 3)
                return;

            var mat = m.GetFMAT();

            if (shader != OpenTKSharedResources.shaders["BFRES_Normals"])
            {
                bool useVertexColors = true;
                if (mat.shaderassign.ShaderArchive == "Park_UBER")
                    useVertexColors = false;

                SetRenderSettings(shader, useVertexColors);
                SetRenderPass(mat);
                SetUniforms(mat, shader, m, m.DisplayId);
                SetTextureUniforms(mat, m, shader);
            }
            SetBoneUniforms(shader, mdl, m);
            ApplyTransformFix(mdl, m, shader);
            SetVertexAttributes(m, shader);

            //Check the binded bone if it's visible from bone visual anims
       //     if (!mdl.Skeleton.bones[m.boneIndx].Visible)
       //         m.Checked = false;

            if (m.Checked && mdl.Skeleton.bones.Count > 0 && mdl.Skeleton.bones[m.BoneIndex].Visible && mat.Enabled)
            {
                shader.SetVector3("materialSelectColor", new Vector3(0));
                if (m.GetMaterial().IsSelected)
                {
                    shader.SetVector3("materialSelectColor", ColorUtility.ToVector3(Color.FromArgb(0,163,204)));
                    DrawModelSelection(m, shader);
                }
                else if (m.IsSelected || ModelSelected)
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
                        DrawMdoelHoverSelection(m, shader, IsSelected(), Hovered);

                        PrimitiveType primitiveType = PrimitiveType.Triangles;

                        switch (m.lodMeshes[m.DisplayLODIndex].PrimativeType)
                        {
                            case STPrimitiveType.Lines:
                                primitiveType = PrimitiveType.Lines;
                                break;
                            case STPrimitiveType.LineStrips:
                                primitiveType = PrimitiveType.LineStrip;
                                break;
                            case STPrimitiveType.Points:
                                primitiveType = PrimitiveType.Points;
                                break;
                            case STPrimitiveType.Triangles:
                                primitiveType = PrimitiveType.Triangles;
                                break;
                        }

                        GL.DrawElements(primitiveType, m.lodMeshes[m.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, m.Offset);
                    }
                }
            }
        }
        private static void ApplyTransformFix(FMDL fmdl, FSHP m, SF.Shader shader)
        {
            Matrix4 idenity = Matrix4.Identity;
            shader.SetInt("NoSkinning", 0);
            shader.SetInt("RigidSkinning", 0);
            shader.SetInt("SingleBoneIndex", m.BoneIndex);

            shader.SetMatrix4x4("SingleBoneBindTransform", ref idenity);
            //Some objects will have no weights or indices. These will weigh to the bone index in the shape section.

            if (m.VertexSkinCount == 1)
            {
                shader.SetInt("RigidSkinning", 1);
            }
            if (m.VertexSkinCount == 0)
            {
                if (fmdl.Skeleton.bones.Count > 0)
                {
                    Matrix4 transform = fmdl.Skeleton.bones[m.BoneIndex].invert * fmdl.Skeleton.bones[m.BoneIndex].Transform;

                    shader.SetMatrix4x4("SingleBoneBindTransform", ref transform);
                    shader.SetInt("NoSkinning", 1);
                }
            }
        }

        static bool Loaded = false;
        public override void UpdateVertexData()
        {
            if (!Runtime.OpenTKInitialized)
                return;

            UpdateModelList();
            Loaded = false;

            DisplayVertex[] Vertices;
            int[] Faces;

            int poffset = 0;
            int voffset = 0;
            List<DisplayVertex> Vs = new List<DisplayVertex>();
            List<int> Ds = new List<int>();

            int TotalShapeCount = models.Sum(b => b.shapes.Count);

            int value = 0;

            for (int m = 0; m < models.Count; m++)
            {
                //Reset min/max
                models[m].MaxPosition = new Vector3(0);
                models[m].MinPosition = new Vector3(0);

                for (int shp = 0; shp < models[m].shapes.Count; shp++)
                {
                    //Update render pass aswell
                    CheckRenderPass(models[m].shapes[shp].GetFMAT());

                    models[m].shapes[shp].Offset = poffset * 4;
                    List<DisplayVertex> pv = models[m].shapes[shp].CreateDisplayVertices(models[m]);
                    Vs.AddRange(pv);

                    for (int i = 0; i < models[m].shapes[shp].lodMeshes[models[m].shapes[shp].DisplayLODIndex].displayFaceSize; i++)
                    {
                        Ds.Add(models[m].shapes[shp].display[i] + voffset);
                    }
                    poffset += models[m].shapes[shp].lodMeshes[models[m].shapes[shp].DisplayLODIndex].displayFaceSize;
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

            LibraryGUI.UpdateViewport();

            Loaded = true;
        }

        private static void SetRenderPass(FMAT mat)
        {
            bool NoCull = false;
            bool CullBack = false;
            bool CullFront = false;

            for (int i = 0; i < mat.renderinfo.Count; i++)
            {
                if (mat.renderinfo[i].Name == "display_face")
                {
                    NoCull = mat.renderinfo[i].ValueString.Contains("both");
                    CullFront = mat.renderinfo[i].ValueString.Contains("back");
                    CullBack = mat.renderinfo[i].ValueString.Contains("front");
                }

                if (mat.shaderassign.ShaderArchive == "Turbo_UBER")
                {
                    AglShaderTurbo aglShader = new AglShaderTurbo();
                    aglShader.LoadRenderInfo(mat.renderinfo[i]);
                }
            }

            if (NoCull)
            {
                GL.Disable(EnableCap.CullFace);
            }
            else if (CullFront)
            {
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Front);
            }
            else if (CullBack)
            {
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Back);
            }
        }

        private static void CheckRenderPass(FMAT mat)
        {
            if (mat.ImageKey != "material")
            {
                mat.ImageKey = "material";
                mat.SelectedImageKey = "material";
            }

            bool IsTranslucent = false;
            bool IsTransparentMask = false;


            for (int i = 0; i < mat.renderinfo.Count; i++)
            {
                if (mat.renderinfo[i].Name == "gsys_render_state_mode")
                {
                    IsTranslucent = mat.renderinfo[i].ValueString.Contains("translucent");
                    IsTransparentMask = mat.renderinfo[i].ValueString.Contains("mask");
                }
                if (mat.renderinfo[i].Name == "renderPass")
                {
                    IsTransparentMask = mat.renderinfo[i].ValueString.Contains("xlu");
                }
            }

            if (mat.shaderassign.options.ContainsKey("enable_translucent"))
                IsTranslucent = mat.shaderassign.options["enable_translucent"] == "1";
            if (mat.shaderassign.options.ContainsKey("enable_translucent"))
                IsTransparentMask = mat.shaderassign.options["enable_transparent"] == "1";

            if (mat.MaterialU != null)
            {
                IsTranslucent =  mat.MaterialU.RenderState.FlagsMode == ResU.RenderStateFlagsMode.Translucent;
                IsTransparentMask = mat.MaterialU.RenderState.FlagsMode == ResU.RenderStateFlagsMode.AlphaMask;
            }

            mat.isTransparent = IsTransparentMask || IsTranslucent;

            SetMaterialIcon(mat, IsTranslucent, "MaterialTranslucent");
            SetMaterialIcon(mat, IsTransparentMask, "MaterialTransparent");
        }

        private static void SetMaterialIcon(FMAT mat, bool IsEffect, string Key)
        {
            if (IsEffect)
            {
                mat.ImageKey = Key;
                mat.SelectedImageKey = Key;
            }
        }

        private static void SetUniforms(FMAT mat, SF.Shader shader, FSHP m, int id)
        {
             shader.SetBoolToInt("isTransparent", mat.isTransparent);

            shader.SetFloat("ao_density", 1);
            shader.SetFloat("shadow_density", 1);
            
            shader.SetFloat("normal_map_weight", 1);

            //Bake map UV coordinate ST
            shader.SetVector4("gsys_bake_st0", new Vector4(1, 1, 0, 0));
            shader.SetVector4("gsys_bake_st1", new Vector4(1, 1, 0, 0));

            shader.SetBoolToInt("UseSpecularColor",
                 (mat.GetOptionValue("specular_mask_is_color") == 1) ||
                 mat.GetOptionValue("enable_specular_color") == 1);

            shader.SetBoolToInt("UseMultiTexture", mat.GetOptionValue("enable_multi_texture") == 1);

            //Colors
            shader.SetVector4("const_color0", new Vector4(1, 1, 1, 1));
            shader.SetVector4("base_color_mul_color", new Vector4(1, 1, 1, 1));
            shader.SetVector3("albedo_tex_color", new Vector3(1, 1, 1));
            shader.SetVector3("emission_color", new Vector3(1, 1, 1));
            shader.SetVector3("specular_color", new Vector3(1, 1, 1));
            
            shader.SetFloat("fuv1_mtx", 0);

            //SRT
            shader.SetVector4("tex_mtx0", new Vector4(1, 1, 1, 1));
            shader.SetVector2("SRT_Scale", new Vector2(1, 1));
            shader.SetFloat("SRT_Rotate", 0);
            shader.SetVector2("SRT_Translate", new Vector2(0, 0));

            shader.SetInt("selectedBoneIndex", Runtime.SelectedBoneIndex);

            SetUniformData(mat, shader, "base_color_mul_color");

            shader.SetInt("enableCellShading", 0);
            bool HasTans = m.vertexAttributes.Any(x => x.Name == "_t0");
            shader.SetBoolToInt("hasTangents", HasTans);

            SetUniformData(mat, shader, "fuv1_mtx");

            SetUniformData(mat, shader, "gsys_bake_st0");
            SetUniformData(mat, shader, "gsys_bake_st1");

            SetUniformData(mat, shader, "ao_density");
            SetUniformData(mat, shader, "shadow_density");
            SetUniformData(mat, shader, "normal_map_weight");

            SetUniformData(mat, shader, "const_color0");
            SetUniformData(mat, shader, "base_color_mul_color");
            SetUniformData(mat, shader, "albedo_tex_color");
            SetUniformData(mat, shader, "emission_color");
            SetUniformData(mat, shader, "specular_color");


            //This uniform sets various maps for BOTW to use second UV channel
            SetUniformData(mat, shader, "uking_texture2_texcoord");

            SetUniformData(mat, shader, "cIsEnableNormalMap");

            SetUniformData(mat, shader, "texsrt0");
            SetUniformData(mat, shader, "tex_mtx0");
            SetUniformData(mat, shader, "texmtx0");
            
            //Sets shadow type
            //0 = Ambient occusion bake map
            //1 = Shadow 
            //2 = Shadow + Ambient occusion map
            SetUniformData(mat, shader, "bake_shadow_type");
            SetUniformData(mat, shader, "bake_light_type");
            SetUniformData(mat, shader, "gsys_bake_light_scale");

            SetUniformData(mat, shader, "enable_projection_light");
            SetUniformData(mat, shader, "enable_actor_light");

            SetUniformData(mat, shader, "bake_calc_type");
        }
        private static void SetUniformData(FMAT mat, SF.Shader shader, string propertyName)
        {
            if (mat.shaderassign.options.ContainsKey(propertyName))
            {
                //float value = float.Parse(mat.shaderassign.options[propertyName]);
                //shader.SetFloat(propertyName, value);
            }

            Dictionary<string, BfresShaderParam> matParams = mat.matparam;
            if (mat.animatedMatParams.ContainsKey(propertyName))
                matParams = mat.animatedMatParams;

            if (matParams.ContainsKey(propertyName))
            {
                if (matParams[propertyName].Type == ShaderParamType.Float)
                {
                    if (mat.anims.ContainsKey(propertyName))
                        matParams[propertyName].ValueFloat[0] = mat.anims[propertyName][0];
                    shader.SetFloat(propertyName, matParams[propertyName].ValueFloat[0]);
                }

                if (matParams[propertyName].Type == ShaderParamType.Float2)
                {
                    if (mat.anims.ContainsKey(propertyName))
                    {
                        matParams[propertyName].ValueFloat = new float[2] {
                            mat.anims[propertyName][0], mat.anims[propertyName][1]};
                    }

                    shader.SetVector2(propertyName, Utils.ToVec2(matParams[propertyName].ValueFloat));
                }

                if (matParams[propertyName].Type == ShaderParamType.Float3)
                {
                    if (mat.anims.ContainsKey(propertyName))
                    {
                        matParams[propertyName].ValueFloat = new float[3] {
                            mat.anims[propertyName][0],
                            mat.anims[propertyName][1],
                            mat.anims[propertyName][2]};
                    }

                    shader.SetVector3(propertyName, Utils.ToVec3(matParams[propertyName].ValueFloat));
                }
                if (matParams[propertyName].Type == ShaderParamType.Float4)
                {
                    if (mat.anims.ContainsKey(propertyName))
                    {
                        matParams[propertyName].ValueFloat = new float[4] {
                                     mat.anims[propertyName][0], mat.anims[propertyName][1],
                                     mat.anims[propertyName][2], mat.anims[propertyName][3]};
                    }

                    shader.SetVector4(propertyName, Utils.ToVec4(matParams[propertyName].ValueFloat));
                }
                if (matParams[propertyName].Type == ShaderParamType.TexSrt)
                {
                    // Vector 2 Scale
                    // 1 roation float
                    // Vector2 translate
                    TexSrt texSRT = matParams[propertyName].ValueTexSrt;

                    shader.SetVector2("SRT_Scale", Utils.ToVec2(texSRT.Scaling));
                    shader.SetFloat("SRT_Rotate", texSRT.Rotation);
                    shader.SetVector2("SRT_Translate", Utils.ToVec2(texSRT.Translation));
                }
                if (matParams[propertyName].Type == ShaderParamType.TexSrtEx)
                {
                    // Vector 2 Scale
                    // 1 roation float
                    // Vector2 translate
                    TexSrtEx texSRT = matParams[propertyName].ValueTexSrtEx;

                    shader.SetVector2("SRT_Scale", Utils.ToVec2(texSRT.Scaling));
                    shader.SetFloat("SRT_Rotate", texSRT.Rotation);
                    shader.SetVector2("SRT_Translate", Utils.ToVec2(texSRT.Translation));
                }

                //MTA SRT
                if (propertyName == "texsrt0" && mat.shaderassign.ShaderArchive == "ssg")
                {
                    TexSrt texSRT = matParams[propertyName].ValueTexSrt;

                    shader.SetVector2("SRT_Scale", Utils.ToVec2(texSRT.Scaling));
                    shader.SetFloat("SRT_Rotate", texSRT.Rotation);
                    shader.SetVector2("SRT_Translate", Utils.ToVec2(texSRT.Translation));
                }
            }
        }

        private void SetVertexAttributes(FSHP m, SF.Shader shader)
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

        private static void DrawMdoelHoverSelection(STGenericObject p, SF.Shader shader,
            bool IsSelected, bool IsHovered)
        {
            if (IsHovered && IsSelected)
                shader.SetVector4("pickingColor", hoverColor);
            else if (IsHovered || IsSelected)
                shader.SetVector4("pickingColor", selectColor);
            else
                shader.SetVector4("pickingColor", new Vector4(1));
        }

        private static void DrawModelWireframe(STGenericObject p, SF.Shader shader)
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
        private static void DrawModelSelection(STGenericObject p, SF.Shader shader)
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
