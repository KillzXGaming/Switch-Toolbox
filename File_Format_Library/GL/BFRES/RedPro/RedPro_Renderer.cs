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
using FirstPlugin.RedPro;

namespace FirstPlugin
{
    public class RedPro_Renderer : BFRESRenderBase
    {
        public List<SHShaderProgram> ShaderPrograms = new List<SHShaderProgram>();

        Vector3 position = new Vector3(0);

        public static Vector4 hoverColor = new Vector4(1);
        public static Vector4 selectColor = new Vector4(1);

        protected bool Selected = false;
        public bool Hovered = false;

        public bool IsSelected() => Selected;

        public struct DisplayVertex
        {
            // Used for rendering.
            public Vector3 Position;
            public Vector3 Normal;
            public Vector4 BlendWeight;
            public Vector4 BlendIndex;
            public Vector2 TexCoord0;
            public Vector2 TexCoord1;
            public Vector2 TexCoord2;
            public Vector2 TexCoord3;
            public Vector2 TexCoord4;
            public Vector2 TexCoord5;
            public Vector2 TexCoord6;
            public Vector2 TexCoord7;
            public Vector4 Color0;
            public Vector4 Color1;

            public static int Size = 4 * (3 + 3 + 4 + 4 + 2 + 2 + 2 + 2 + 2 + 2 + 2 + 2 + 4 + 4);
        }

        public RedPro_Renderer()
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
            Vector3 difLightDirection = Vector3.TransformNormal(lightDirection, invertedCamera).Normalized();

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

        public override void Draw(GL_ControlModern control, Pass pass)
        {
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

            control.UpdateModelMatrix(ModelTransform * Matrix4.CreateScale(Runtime.previewScale));

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
                        DrawModel(control, transparent[shp], models[m], models[m].IsSelected);
                    }

                    for (int shp = 0; shp < opaque.Count; shp++)
                    {
                        DrawModel(control, opaque[shp], models[m], models[m].IsSelected);
                    }
                }
            }

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        public void DepthSortMeshes(Vector3 cameraPosition)
        {
            foreach (FMDL fmdl in models)
            {
                List<FSHP> unsortedMeshes = new List<FSHP>();

                foreach (FSHP m in fmdl.shapes)
                {
                    m.sortingDistance = m.CalculateSortingDistance(cameraPosition);
                    unsortedMeshes.Add(m);
                }

                fmdl.depthSortedMeshes = unsortedMeshes.OrderBy(o => (o.sortingDistance)).ToList();
            }


            // Order by the distance from the camera to the closest point on the bounding sphere. 
            // Positive values are usually closer to camera. Negative values are usually farther away. 
        }

        private static void SetTextureUniforms(FMAT mat, FSHP m, SHShaderProgram shader)
        {
            for (int t = 0; t < mat.TextureMaps.Count; t++) {
                MatTexture matex = (MatTexture)mat.TextureMaps[t];
                mat.HasDiffuseMap = TextureUniform(shader, mat, $"tex_map{t}", matex);
            }
        }

        private static bool TextureUniform(SHShaderProgram shader, FMAT mat, string name, MatTexture mattex)
        {
            if (mattex.textureState == STGenericMatTexture.TextureState.Binded)
                return true;

            // Bind the texture and create the uniform if the material has the right textures. 
            BindTexture(mattex, mat, shader, mat.GetResFileU() != null);

            return true;
        }
        public static bool BindTexture(MatTexture tex, FMAT material, SHShaderProgram shader, bool IsWiiU)
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
            }

            return true;
        }

        private static bool BindFTEX(BFRESGroupNode ftexContainer, MatTexture tex, SHShaderProgram shader, string activeTex)
        {
            FTEX ftex = (FTEX)ftexContainer.ResourceNodes[activeTex];

            if (ftex.RenderableTex == null || !ftex.RenderableTex.GLInitialized)
                ftex.LoadOpenGLTexture();

            BindGLTexture(tex, shader, ftex);

            return ftex.RenderableTex.GLInitialized;
        }

        private static bool BindBNTX(BNTX bntx, MatTexture tex, SHShaderProgram shader, string activeTex)
        {
            if (bntx.Textures[activeTex].RenderableTex == null ||
                     !bntx.Textures[activeTex].RenderableTex.GLInitialized)
            {
                bntx.Textures[activeTex].LoadOpenGLTexture();
            }

            BindGLTexture(tex, shader, bntx.Textures[activeTex]);

            return bntx.Textures[activeTex].RenderableTex.GLInitialized;
        }

        private static void BindGLTexture(MatTexture tex, SHShaderProgram shader, STGenericTexture texture)
        {
            //If the texture is still not initialized then return
            if (!texture.RenderableTex.GLInitialized)
                return;

            //     GL.ActiveTexture(TextureUnit.Texture0 + texid);
            GL.BindTexture(TextureTarget.Texture2D, texture.RenderableTex.TexID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)MatTexture.wrapmode[tex.WrapModeS]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)MatTexture.wrapmode[tex.WrapModeT]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MatTexture.minfilter[tex.MinFilter]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MatTexture.magfilter[tex.MagFilter]);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, 0.0f);
        }

        private SHShaderProgram SearchProgram(string shaderArchive, string shaderProgram)
        {
            for (int i = 0; i < ShaderPrograms.Count; i++)
                if (ShaderPrograms[i].IsLinked(shaderArchive, shaderProgram))
                    return ShaderPrograms[i];
            return null;
        }

        private void DrawModel(GL_ControlBase control, FSHP m, FMDL mdl, bool ModelSelected)
        {
            if (m.lodMeshes[m.DisplayLODIndex].faces.Count <= 3)
                return;

            var mat = m.GetFMAT();

            SHShaderProgram shader = SearchProgram(
                mat.shaderassign.ShaderArchive,
                mat.shaderassign.ShaderModel);

            if (shader == null) return;

            shader.Enable();
            shader.EnableVertexAttributes();

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);

         //   SetRenderPass(mat);
        //    SetUniforms(mat, shader, m, m.DisplayId);
         //   SetTextureUniforms(mat, m, shader);

            shader.LoadUniforms(mdl, m, mdl.Skeleton, control);

            SetVertexAttributes(m, shader);

            if (m.Checked && mdl.Skeleton.bones.Count > 0 && mdl.Skeleton.bones[m.BoneIndex].Visible && mat.Enabled)
            {
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

            shader.DisableVertexAttributes();
            shader.Disable();
        }

        public void UpdateShaders()
        {
            if (ShaderPrograms.Count > 0) return;

            foreach (var file in ResFileNode.IFileInfo.ArchiveParent.Files) {
                if (Utils.GetExtension(file.FileName) == ".sharc")
                {
                    var fileFormat = file.OpenFile();
                    if (fileFormat != null && fileFormat is SHARC) {
                      ShaderPrograms.AddRange(LoadProgram((SHARC)fileFormat));
                    }
                }
            }
        }

        private List<SHShaderProgram> LoadProgram(SHARC sharc)
        {
            List<SHShaderProgram> programs = new List<SHShaderProgram>();
            foreach (var program in sharc.header.ShaderPrograms)
                programs.Add(new SHShaderProgram(sharc.header, program));

            return programs;
        }

        static bool Loaded = false;
        public override void UpdateVertexData()
        {
            if (!Runtime.OpenTKInitialized)
                return;

            UpdateShaders();
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
                    //CheckRenderPass(models[m].shapes[shp].GetFMAT());

                    models[m].shapes[shp].Offset = poffset * 4;
                    List<DisplayVertex> pv = CreateDisplayVertices(models[m], models[m].shapes[shp]);
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

        static  List<DisplayVertex> CreateDisplayVertices(FMDL model, FSHP mesh)
        {
            // rearrange faces
            mesh.display = mesh.lodMeshes[mesh.DisplayLODIndex].getDisplayFace().ToArray();

            List<DisplayVertex> displayVertList = new List<DisplayVertex>();

            if (mesh.lodMeshes[mesh.DisplayLODIndex].faces.Count <= 3)
                return displayVertList;

            foreach (Vertex v in mesh.vertices)
            {
                model.MaxPosition = OpenGLUtils.GetMax(model.MaxPosition, v.pos);
                model.MinPosition = OpenGLUtils.GetMin(model.MinPosition, v.pos);

                DisplayVertex displayVert = new DisplayVertex()
                {
                    Position = v.pos,
                    Normal = v.nrm,
                    Color0 = v.col,
                    Color1 = v.col2,
                    TexCoord0 = v.uv0,
                    TexCoord1 = v.uv1,
                    TexCoord2 = v.uv2,
                    BlendIndex = new Vector4(
                         v.boneIds.Count > 0 ? v.boneIds[0] : -1,
                         v.boneIds.Count > 1 ? v.boneIds[1] : -1,
                         v.boneIds.Count > 2 ? v.boneIds[2] : -1,
                         v.boneIds.Count > 3 ? v.boneIds[3] : -1),
                    BlendWeight = new Vector4(
                         v.boneWeights.Count > 0 ? v.boneWeights[0] : 0,
                         v.boneWeights.Count > 1 ? v.boneWeights[1] : 0,
                         v.boneWeights.Count > 2 ? v.boneWeights[2] : 0,
                         v.boneWeights.Count > 3 ? v.boneWeights[3] : 0),
                };

                displayVertList.Add(displayVert);
            }

            return displayVertList;
        }

        public void UpdateSingleMaterialTextureMaps(FMAT mat)
        {
            if (!Runtime.OpenTKInitialized)
                return;

            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                foreach (var t in mat.TextureMaps)
                {
                    if (bntx.Textures.ContainsKey(t.Name))
                    {
                        if (!bntx.Textures[t.Name].RenderableTex.GLInitialized)
                            bntx.Textures[t.Name].LoadOpenGLTexture();
                    }
                }
            }

            LibraryGUI.UpdateViewport();
        }
        public void UpdateTextureMaps()
        {
            if (!Runtime.OpenTKInitialized)
                return;

            foreach (BNTX bntx in PluginRuntime.bntxContainers)
            {
                if (!bntx.AllGLInitialized)
                {
                    foreach (var tex in bntx.Textures)
                    {
                        if (tex.Value.RenderableTex != null && !tex.Value.RenderableTex.GLInitialized)
                            tex.Value.LoadOpenGLTexture();
                    }
                }
            }
            foreach (BFRESGroupNode ftexCont in PluginRuntime.ftexContainers)
            {
                foreach (var tex in ftexCont.ResourceNodes)
                {
                    if (!((FTEX)tex.Value).RenderableTex.GLInitialized)
                        ((FTEX)tex.Value).LoadOpenGLTexture();
                }
            }
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
                IsTranslucent = mat.MaterialU.RenderState.FlagsMode == ResU.RenderStateFlagsMode.Translucent;
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

        private static void SetUniforms(FMAT mat, SHShaderProgram shader, FSHP m, int id)
        {
       
        }

        private static void SetUniformData(FMAT mat, SHShaderProgram shader, string propertyName)
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

                    shader.SetVec2(propertyName, Utils.ToVec2(mat.matparam[propertyName].ValueFloat));
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

                    shader.SetVec3(propertyName, Utils.ToVec3(mat.matparam[propertyName].ValueFloat));
                }
                if (mat.matparam[propertyName].Type == ShaderParamType.Float4)
                {
                    if (mat.anims.ContainsKey(propertyName))
                    {
                        mat.matparam[propertyName].ValueFloat = new float[4] {
                                     mat.anims[propertyName][0], mat.anims[propertyName][1],
                                     mat.anims[propertyName][2], mat.anims[propertyName][3]};
                    }

                    shader.SetVec4(propertyName, Utils.ToVec4(mat.matparam[propertyName].ValueFloat));
                }
                if (mat.matparam[propertyName].Type == ShaderParamType.TexSrt)
                {
                    // Vector 2 Scale
                    // 1 roation float
                    // Vector2 translate
                    TexSrt texSRT = mat.matparam[propertyName].ValueTexSrt;

                    shader.SetVec2("SRT_Scale", Utils.ToVec2(texSRT.Scaling));
                    shader.SetFloat("SRT_Rotate", texSRT.Rotation);
                    shader.SetVec2("SRT_Translate", Utils.ToVec2(texSRT.Translation));
                }

                //MTA SRT
                if (propertyName == "texsrt0" && mat.shaderassign.ShaderArchive == "ssg")
                {
                    TexSrt texSRT = mat.matparam[propertyName].ValueTexSrt;

                    shader.SetVec2("SRT_Scale", Utils.ToVec2(texSRT.Scaling));
                    shader.SetFloat("SRT_Rotate", texSRT.Rotation);
                    shader.SetVec2("SRT_Translate", Utils.ToVec2(texSRT.Translation));
                }
            }

            if (mat.animatedMatParams.ContainsKey(propertyName))
            {
                if (mat.animatedMatParams[propertyName].Type == ShaderParamType.Float)
                {
                    if (mat.anims.ContainsKey(propertyName))
                        mat.animatedMatParams[propertyName].ValueFloat[0] = mat.anims[propertyName][0];
                    shader.SetFloat(propertyName, mat.animatedMatParams[propertyName].ValueFloat[0]);
                }

                if (mat.animatedMatParams[propertyName].Type == ShaderParamType.Float2)
                {
                    if (mat.anims.ContainsKey(propertyName))
                    {
                        mat.animatedMatParams[propertyName].ValueFloat = new float[2] {
                            mat.anims[propertyName][0], mat.anims[propertyName][1]};
                    }

                    shader.SetVec2(propertyName, Utils.ToVec2(mat.animatedMatParams[propertyName].ValueFloat));
                }

                if (mat.animatedMatParams[propertyName].Type == ShaderParamType.Float3)
                {
                    Console.WriteLine(propertyName + " " + mat.animatedMatParams[propertyName].ValueFloat);

                    if (mat.anims.ContainsKey(propertyName))
                    {
                        mat.animatedMatParams[propertyName].ValueFloat = new float[3] {
                            mat.anims[propertyName][0],
                            mat.anims[propertyName][1],
                            mat.anims[propertyName][2]};
                    }

                    shader.SetVec3(propertyName, Utils.ToVec3(mat.animatedMatParams[propertyName].ValueFloat));
                }
                if (mat.animatedMatParams[propertyName].Type == ShaderParamType.Float4)
                {
                    if (mat.anims.ContainsKey(propertyName))
                    {
                        mat.animatedMatParams[propertyName].ValueFloat = new float[4] {
                                     mat.anims[propertyName][0], mat.anims[propertyName][1],
                                     mat.anims[propertyName][2], mat.anims[propertyName][3]};
                    }

                    shader.SetVec4(propertyName, Utils.ToVec4(mat.animatedMatParams[propertyName].ValueFloat));
                }

                if (mat.animatedMatParams[propertyName].Type == ShaderParamType.TexSrt)
                {
                    // Vector 2 Scale
                    // 1 roation float
                    // Vector2 translate
                    TexSrt texSRT = mat.animatedMatParams[propertyName].ValueTexSrt;

                    shader.SetVec2("SRT_Scale", Utils.ToVec2(texSRT.Scaling));
                    shader.SetFloat("SRT_Rotate", texSRT.Rotation);
                    shader.SetVec2("SRT_Translate", Utils.ToVec2(texSRT.Translation));
                }
            }
        }
        private void SetVertexAttributes(FSHP m, SHShaderProgram shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("aPosition"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 0);
            GL.VertexAttribPointer(shader.GetAttribute("aNormal"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 12);
            GL.VertexAttribPointer(shader.GetAttribute("aBlendWeight"), 4, VertexAttribPointerType.Float, false, DisplayVertex.Size, 24);
            GL.VertexAttribIPointer(shader.GetAttribute("aBlendIndex"), 4, VertexAttribIntegerType.Int, DisplayVertex.Size, new IntPtr(40));
            GL.VertexAttribPointer(shader.GetAttribute("aTexCoord0"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 56);
            GL.VertexAttribPointer(shader.GetAttribute("aTexCoord1"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 64);
            GL.VertexAttribPointer(shader.GetAttribute("aTexCoord2"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 72);
            GL.VertexAttribPointer(shader.GetAttribute("aTexCoord3"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 80);
            GL.VertexAttribPointer(shader.GetAttribute("aTexCoord4"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 88);
            GL.VertexAttribPointer(shader.GetAttribute("aTexCoord5"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 96);
            GL.VertexAttribPointer(shader.GetAttribute("aTexCoord6"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 104);
            GL.VertexAttribPointer(shader.GetAttribute("aTexCoord7"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 112);
            GL.VertexAttribPointer(shader.GetAttribute("aColor0"), 4, VertexAttribPointerType.Float, false, DisplayVertex.Size, 120);
            GL.VertexAttribPointer(shader.GetAttribute("aColor1"), 4, VertexAttribPointerType.Float, false, DisplayVertex.Size, 136);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
        }

        #endregion
    }
}
