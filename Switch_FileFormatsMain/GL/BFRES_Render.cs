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
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using ResU = Syroot.NintenTools.Bfres;
using Bfres.Structs;
using GL_EditorFramework.EditorDrawables;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase;

namespace FirstPlugin
{
    public class BFRESRender : EditableObject
    {
        public Matrix4 ModelTransform = Matrix4.Identity;
        Vector3 position = new Vector3(0);

        protected bool Selected = false;
        public bool Hovered = false;

        public override bool IsSelected() => Selected;

        public override bool IsSelected(int partIndex) => Selected;

        // gl buffer objects
        int vbo_position;
        int ibo_elements;

        public List<FMDL> models
        {
            get
            {
                List<FMDL> fmdls = new List<FMDL>();
                foreach (var node in ResFileNode.Nodes)
                {
                    if (node is BFRESGroupNode &&
                        ((BFRESGroupNode)node).Type == BRESGroupType.Models)
                    {
                        foreach (FMDL mdl in ((BFRESGroupNode)node).Nodes)
                            fmdls.Add(mdl);
                    }
                }
                return fmdls;
            }
        }

        public BFRES ResFileNode;

        public BFRESRender()
        {

        }
        private void GenerateBuffers()
        {
            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out ibo_elements);

            UpdateVertexData();
            UpdateTextureMaps();
        }

        public void Destroy()
        {
            bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;

            if (!buffersWereInitialized)
                return;

            GL.DeleteBuffer(vbo_position);
            GL.DeleteBuffer(ibo_elements);
        }

        #region Rendering


        public ShaderProgram BotwShaderProgram;
        public ShaderProgram normalsShaderProgram;
        public ShaderProgram debugShaderProgram;
        public ShaderProgram pbrShaderProgram;
        public ShaderProgram defaultShaderProgram;
        public ShaderProgram solidColorShaderProgram;

        public override void Prepare(GL_ControlModern control)
        {
            string pathFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bfres") + "\\BFRES.frag";
            string pathVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bfres") + "\\BFRES.vert";


            string pathBotwFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bfres") + "\\BFRES_Botw.frag";

            string pathPbrFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bfres") + "\\BFRES_PBR.frag";

            string pathBfresUtiltyFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bfres") + "\\BFRES_utility.frag";
            string pathBfresTurboShadow = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bfres") + "\\BFRESTurboShadow.frag";

            string pathUtiltyFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Utility") + "\\Utility.frag";



            string pathDebugFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bfres") + "\\BFRES_Debug.frag";
            string pathNormalsFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bfres") + "\\Normals.frag";
            string pathNormalsVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bfres") + "\\Normals.vert";
            string pathNormalGeom = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bfres") + "\\Normals.geom";

            var defaultFrag = new FragmentShader(System.IO.File.ReadAllText(pathFrag));
            var defaultVert = new VertexShader(System.IO.File.ReadAllText(pathVert));

            var BotwtFrag = new FragmentShader(System.IO.File.ReadAllText(pathBotwFrag));

            var shadowMapAGL = new FragmentShader(System.IO.File.ReadAllText(pathBfresTurboShadow));

            var PbrFrag = new FragmentShader(System.IO.File.ReadAllText(pathPbrFrag));

            var debugFrag = new FragmentShader(System.IO.File.ReadAllText(pathDebugFrag));
            var normalsVert = new VertexShader(System.IO.File.ReadAllText(pathNormalsVert));
            var normalsFrag = new FragmentShader(System.IO.File.ReadAllText(pathNormalsFrag));
            var normalsGeom = new GeomertyShader(System.IO.File.ReadAllText(pathNormalGeom));

            var bfresUtiltyFrag = new FragmentShader(System.IO.File.ReadAllText(pathBfresUtiltyFrag));
            var utiltyFrag = new FragmentShader(System.IO.File.ReadAllText(pathUtiltyFrag));

            var solidColorFrag = new FragmentShader(
      @"#version 330
				uniform vec4 color;
				out vec4 fragColor;

				void main(){
					fragColor = color;
				}");

            var solidColorVert = new VertexShader(
          @"#version 330
                in vec3 vPosition;
                in vec3 vNormal;
                in vec3 vColor;

				uniform mat4 mtxMdl;
				uniform mat4 mtxCam;

                out vec3 normal;
                out vec3 color;
                out vec3 position;

				void main(){
                    normal = vNormal;
                    color = vColor;
	                position = vPosition;

                    gl_Position = mtxMdl * mtxCam * vec4(vPosition.xyz, 1.0);
				}");

            defaultShaderProgram = new ShaderProgram(new Shader[] { bfresUtiltyFrag, utiltyFrag, defaultFrag, defaultVert, utiltyFrag, shadowMapAGL });
            BotwShaderProgram = new ShaderProgram(new Shader[] { bfresUtiltyFrag, utiltyFrag, BotwtFrag, defaultVert, utiltyFrag, shadowMapAGL });

            normalsShaderProgram = new ShaderProgram(new Shader[] { normalsFrag, normalsVert, normalsGeom });
            debugShaderProgram = new ShaderProgram(new Shader[] { bfresUtiltyFrag, utiltyFrag, debugFrag, defaultVert, utiltyFrag, shadowMapAGL });
            pbrShaderProgram = new ShaderProgram(new Shader[] { bfresUtiltyFrag, utiltyFrag, PbrFrag, defaultVert, shadowMapAGL });
            solidColorShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert);
        }

        public override void Prepare(GL_ControlLegacy control)
        {

        }

        private void DrawBoundingBoxes()
        {
            var boundings = GetSelectionBox();

            DrawableBoundingBox.DrawBoundingBox(
                new Vector3(boundings.minX, boundings.minY, boundings.minZ),
                new Vector3(boundings.maxX, boundings.maxY, boundings.maxZ)
                );

            return;

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

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
            if (!buffersWereInitialized)
                GenerateBuffers();

            if (!Runtime.OpenTKInitialized)
                return;

        }

        public void FrameSelection(GL_ControlModern control)
        {
            var spheres = new List<Vector4>();
            foreach (var model in models)
            {
                foreach (var shape in model.shapes)
                {
                    List<Vector3> vertexPositions = new List<Vector3>();
                    foreach (var vertex in shape.vertices)
                    {
                        vertexPositions.Add(vertex.pos);
                    }
                    spheres.Add(SFGraphics.Utils.BoundingSphereGenerator.GenerateBoundingSphere(vertexPositions));
                }
            }


            //Depth sort meshes
            DepthSortMeshes(control.CameraTarget);
        }
        public static Vector4 GenerateBoundingSphere(IEnumerable<Vector4> boundingSpheres)
        {
            // The initial max/min should be the first point.
            Vector3 min = boundingSpheres.FirstOrDefault().Xyz - new Vector3(boundingSpheres.FirstOrDefault().W);
            Vector3 max = boundingSpheres.FirstOrDefault().Xyz + new Vector3(boundingSpheres.FirstOrDefault().W);

            // Calculate the end points using the center and radius
            foreach (var sphere in boundingSpheres)
            {
                min = Vector3.ComponentMin(min, sphere.Xyz - new Vector3(sphere.W));
                max = Vector3.ComponentMax(max, sphere.Xyz + new Vector3(sphere.W));
            }

            return GetBoundingSphereFromSpheres(min, max);
        }
        private static Vector4 GetBoundingSphereFromSpheres(Vector3 min, Vector3 max)
        {
            Vector3 lengths = max - min;
            float maxLength = Math.Max(lengths.X, Math.Max(lengths.Y, lengths.Z));
            Vector3 center = (max + min) / 2.0f;
            float radius = maxLength / 2.0f;
            return new Vector4(center, radius);
        }

        public override void Draw(GL_ControlModern control, Pass pass) {
            DrawBfres(control, pass);
        }

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene) {
            DrawBfres(control, pass);
        }

        private void DrawBfres(GL_ControlModern control, Pass pass)
        {
            if (!Runtime.OpenTKInitialized || pass == Pass.TRANSPARENT)
                return;

            bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
            if (!buffersWereInitialized)
                GenerateBuffers();

            if (Hovered == true)
                throw new Exception("model selected");

            ShaderProgram shader = defaultShaderProgram;

            if (Runtime.EnablePBR)
                shader = pbrShaderProgram;

            if (models.Count > 0)
            {
                if (models[0].shapes.Count > 0)
                {
                    if (models[0].shapes[0].GetMaterial().shaderassign.ShaderModel == "uking_mat")
                        shader = BotwShaderProgram;
                }
            }

            if (Runtime.viewportShading != Runtime.ViewportShading.Default)
                shader = debugShaderProgram;


            control.CurrentShader = shader;

            control.UpdateModelMatrix(Matrix4.CreateScale(Runtime.previewScale) * ModelTransform);

            Matrix4 camMat = control.ModelMatrix * control.CameraMatrix * control.ProjectionMatrix;

            Matrix4 sphereMatrix = camMat;

            if (sphereMatrix.Determinant != 0)
                sphereMatrix.Invert();

            sphereMatrix.Transpose();
            shader.SetMatrix4x4("sphereMatrix", ref sphereMatrix);

            SetRenderSettings(shader);

            Vector4 pickingColor = control.NextPickingColor();

            shader.SetVector3("difLightColor", new Vector3(1));
            shader.SetVector3("ambLightColor", new Vector3(1));

            Matrix4 invertedCamera = Matrix4.Identity;
            if (invertedCamera.Determinant != 0)
                invertedCamera = camMat.Inverted();

            Vector3 lightDirection = new Vector3(0f, 0f, -1f);

            //Todo. Maybe change direction via AAMP file (configs shader data)
            shader.SetVector3("specLightDirection", Vector3.TransformNormal(lightDirection, invertedCamera).Normalized());
            shader.SetVector3("difLightDirection", Vector3.TransformNormal(lightDirection, invertedCamera).Normalized());


            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);

            DrawModels(shader, control);

            if (Runtime.renderNormalsPoints)
            {
                control.CurrentShader = normalsShaderProgram;

                Matrix4 projection = control.ProjectionMatrix;
                Matrix4 camMtx = control.CameraMatrix;

                normalsShaderProgram.SetMatrix4x4("mtxProj", ref projection);
                normalsShaderProgram.SetMatrix4x4("camMtx", ref camMtx);

                normalsShaderProgram.SetFloat("normalsLength", Runtime.normalsLineLength);

                DrawModels(normalsShaderProgram, control);
            }

            GL.UseProgram(0);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        private void DrawModels(ShaderProgram shader, GL_ControlModern control)
        {
            shader.EnableVertexAttributes();
            foreach (FMDL mdl in models)
            {
                if (mdl.Checked)
                {
                    List<FSHP> opaque = new List<FSHP>();
                    List<FSHP> transparent = new List<FSHP>();

                    foreach (FSHP m in mdl.depthSortedMeshes)
                    {
                        if (m.GetMaterial().isTransparent)
                            transparent.Add(m);
                        else
                            opaque.Add(m);
                    }


                    foreach (FSHP shp in mdl.shapes)
                    {
                        DrawModel(shp, mdl, shader, mdl.IsSelected);
                    }
                }
            }
            shader.DisableVertexAttributes();
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

        private void SetRenderSettings(ShaderProgram shader)
        {
            shader.SetBoolToInt("renderVertColor", Runtime.renderVertColor);
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
        private static void SetDefaultTextureAttributes(FMAT mat, ShaderProgram shader)
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
        private static void SetBoneUniforms(ShaderProgram shader, FMDL fmdl, FSHP fshp)
        {
            for (int i = 0; i < fmdl.Skeleton.Node_Array.Length; i++)
            {
                GL.Uniform1(GL.GetUniformLocation(shader.program, String.Format("boneIds[{0}]", i)), fmdl.Skeleton.Node_Array[i]);

                Matrix4 transform = fmdl.Skeleton.bones[fmdl.Skeleton.Node_Array[i]].invert * fmdl.Skeleton.bones[fmdl.Skeleton.Node_Array[i]].Transform;
                GL.UniformMatrix4(GL.GetUniformLocation(shader.program, String.Format("bones[{0}]", i)), false, ref transform);
            }
        }

        private static void SetTextureUniforms(FMAT mat, FSHP m, ShaderProgram shader)
        {
            SetDefaultTextureAttributes(mat, shader);

            GL.ActiveTexture(TextureUnit.Texture0 + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.Id);

            GL.ActiveTexture(TextureUnit.Texture11);
            GL.Uniform1(shader["weightRamp1"], 11);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.BoneWeightGradient.Id);

            GL.ActiveTexture(TextureUnit.Texture12);
            GL.Uniform1(shader["weightRamp2"], 12);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.BoneWeightGradient2.Id);

            GL.Uniform1(shader["debugOption"], 2);

            
            GL.ActiveTexture(TextureUnit.Texture10);
            GL.Uniform1(shader["UVTestPattern"], 10);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.uvTestPattern.Id);

            GL.Uniform1(shader["normalMap"], 0);
            GL.Uniform1(shader["BakeShadowMap"], 0);

            LoadPBRMaps(shader);

            foreach (MatTexture matex in mat.TextureMaps)
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

        private static void LoadPBRMaps(ShaderProgram shader)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + 26);
            RenderTools.specularPbr.Bind();
            GL.Uniform1(shader["specularIbl"], 26);
        //    GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

            // PBR IBL
            GL.ActiveTexture(TextureUnit.Texture0 + 25);
            RenderTools.diffusePbr.Bind();
            GL.Uniform1(shader["irradianceMap"], 25);

            GL.ActiveTexture(TextureUnit.Texture0 + 27);
            RenderTools.brdfPbr.Bind();
            GL.Uniform1(shader["brdfLUT"], 27);

        }

        private static void TextureUniform(ShaderProgram shader, FMAT mat, bool hasTex, string name, MatTexture mattex)
        {
            if (mattex.textureState == STGenericMatTexture.TextureState.Binded)
                return;

            // Bind the texture and create the uniform if the material has the right textures. 
            if (hasTex)
            {
                GL.Uniform1(shader[name], BindTexture(mattex, mat.GetResFileU() != null));
            }
        }
        public static int BindTexture(MatTexture tex, bool IsWiiU)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + tex.textureUnit + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.Id);

            string activeTex = tex.Name;
            if (tex.animatedTexName != "")
                activeTex = tex.animatedTexName;

            if (IsWiiU)
            {
                foreach (var ftexContainer in PluginRuntime.ftexContainers)
                {

                    if (ftexContainer.ResourceNodes.ContainsKey(activeTex))
                    {
                        FTEX ftex = (FTEX)ftexContainer.ResourceNodes[activeTex];

                        if (ftex.RenderableTex == null || !ftex.RenderableTex.GLInitialized)
                            ftex.LoadOpenGLTexture();

                        BindGLTexture(tex, ftex.RenderableTex.TexID);
                    }
                }
            }
            else
            {
                foreach (var bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(activeTex))
                    {
                        if (bntx.Textures[activeTex].RenderableTex == null ||
                            !bntx.Textures[activeTex].RenderableTex.GLInitialized)
                        {
                            bntx.Textures[activeTex].LoadOpenGLTexture();
                        }

                        BindGLTexture(tex, bntx.Textures[activeTex].RenderableTex.TexID);
                    }
                }
            }
            return tex.textureUnit + 1;
        }
        private static void BindGLTexture(MatTexture tex, int texid)
        {
       //     GL.ActiveTexture(TextureUnit.Texture0 + texid);
            GL.BindTexture(TextureTarget.Texture2D, texid);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)MatTexture.wrapmode[tex.wrapModeS]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)MatTexture.wrapmode[tex.wrapModeT]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MatTexture.minfilter[tex.minFilter]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MatTexture.magfilter[tex.magFilter]);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, 0.0f);
        }
        private void DrawModel(FSHP m, FMDL mdl, ShaderProgram shader, bool drawSelection)
        {
            if (m.lodMeshes[m.DisplayLODIndex].faces.Count <= 3)
                return;

            var mat = m.GetMaterial();

            if (shader != normalsShaderProgram)
            {
                SetRenderPass(mat, shader, m, m.DisplayId);
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
                else if ((m.IsSelected))
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
        private static void ApplyTransformFix(FMDL fmdl, FSHP m, ShaderProgram shader)
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
        public void UpdateVertexData()
        {
            if (!Runtime.OpenTKInitialized)
                return;

            STProgressBar progressBar = new STProgressBar();
            progressBar.Task = "Updating Vertex Data...";
            progressBar.Value = 0;
            progressBar.StartPosition = FormStartPosition.CenterScreen;
            progressBar.Show();
            progressBar.Refresh();

            Loaded = false;

            DisplayVertex[] Vertices;
            int[] Faces;

            int poffset = 0;
            int voffset = 0;
            List<DisplayVertex> Vs = new List<DisplayVertex>();
            List<int> Ds = new List<int>();

            int TotalShapeCount = models.Sum(b => b.shapes.Count);

            int curShape = 0;
            int value = 0;

            foreach (FMDL mdl in models)
            {
                foreach (FSHP m in mdl.shapes)
                {
                    progressBar.Task = "Updating Shape... " + m.Text;
                    value = ((curShape * 100) / TotalShapeCount);
                    progressBar.Value = value;
                    progressBar.Refresh();

                    m.Offset = poffset * 4;
                    List<DisplayVertex> pv = m.CreateDisplayVertices();
                    Vs.AddRange(pv);

                    for (int i = 0; i < m.lodMeshes[m.DisplayLODIndex].displayFaceSize; i++)
                    {
                        Ds.Add(m.display[i] + voffset);
                    }
                    poffset += m.lodMeshes[m.DisplayLODIndex].displayFaceSize;
                    voffset += pv.Count;

                    curShape++; 
                }
            }

            progressBar.Value = 100;
            progressBar.Close();

            // Binds
            Vertices = Vs.ToArray();
            Faces = Ds.ToArray();

            // Bind only once!
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<DisplayVertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * DisplayVertex.Size), Vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            GL.BufferData<int>(BufferTarget.ElementArrayBuffer, (IntPtr)(Faces.Length * sizeof(int)), Faces, BufferUsageHint.StaticDraw);

            LibraryGUI.Instance.UpdateViewport();

            Loaded = true;
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

            LibraryGUI.Instance.UpdateViewport();
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
                        if (!tex.Value.RenderableTex.GLInitialized)
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

            LibraryGUI.Instance.UpdateViewport();
        }

        private static void SetRenderPass(FMAT mat, ShaderProgram shader, FSHP m, int id)
        {
            if (mat.shaderassign.ShaderArchive == "Turbo_UBER")
            {
                AglShaderTurbo aglShader = new AglShaderTurbo();

                foreach (var renderInfo in mat.renderinfo)
                    aglShader.LoadRenderInfo(renderInfo);

                aglShader.LoadRenderPass(mat, shader);
            }
        }

        private static void SetUniforms(FMAT mat, ShaderProgram shader, FSHP m, int id)
        {
            shader.SetBoolToInt("isTransparent", mat.isTransparent);

            shader.SetFloat("ao_density", 1);
            shader.SetFloat("normal_map_weight", 1);

            //Bake map UV coordinate ST
            shader.SetVector4("gsys_bake_st0", new Vector4(1, 1, 0, 0));
            shader.SetVector4("gsys_bake_st1", new Vector4(1, 1, 0, 0));


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
            SetUniformData(mat, shader, "normal_map_weight");

            SetUniformData(mat, shader, "const_color0");
            SetUniformData(mat, shader, "base_color_mul_color");
            SetUniformData(mat, shader, "albedo_tex_color");
            SetUniformData(mat, shader, "emission_color");
            SetUniformData(mat, shader, "specular_color");


            //This uniform sets various maps for BOTW to use second UV channel
            SetUniformData(mat, shader, "uking_texture2_texcoord");

            SetUniformData(mat, shader, "tex_mtx0");

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
        private static void SetUniformData(FMAT mat, ShaderProgram shader, string propertyName)
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

                    shader.SetVector2(propertyName, Utils.ToVec2(mat.animatedMatParams[propertyName].ValueFloat));
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

                    shader.SetVector3(propertyName, Utils.ToVec3(mat.animatedMatParams[propertyName].ValueFloat));
                }
                if (mat.animatedMatParams[propertyName].Type == ShaderParamType.Float4)
                {
                    if (mat.anims.ContainsKey(propertyName))
                    {
                        mat.animatedMatParams[propertyName].ValueFloat = new float[4] {
                                     mat.anims[propertyName][0], mat.anims[propertyName][1],
                                     mat.anims[propertyName][2], mat.anims[propertyName][3]};
                    }

                    shader.SetVector4(propertyName, Utils.ToVec4(mat.animatedMatParams[propertyName].ValueFloat));
                }
                if (mat.animatedMatParams[propertyName].Type == ShaderParamType.TexSrt)
                {
                    // Vector 2 Scale
                    // 1 roation float
                    // Vector2 translate
                    TexSrt texSRT = mat.animatedMatParams[propertyName].ValueTexSrt;

                    shader.SetVector2("SRT_Scale", Utils.ToVec2(texSRT.Scaling));
                    shader.SetFloat("SRT_Rotate", texSRT.Rotation);
                    shader.SetVector2("SRT_Translate", Utils.ToVec2(texSRT.Translation));
                }
            }
        }
        private void SetVertexAttributes(FSHP m, ShaderProgram shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 0);
            GL.VertexAttribPointer(shader.GetAttribute("vNormal"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 12);
            GL.VertexAttribPointer(shader.GetAttribute("vTangent"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 24);
            GL.VertexAttribPointer(shader.GetAttribute("vBitangent"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 36);
            GL.VertexAttribPointer(shader.GetAttribute("vUV0"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 48);
            GL.VertexAttribPointer(shader.GetAttribute("vColor"), 4, VertexAttribPointerType.Float, false, DisplayVertex.Size, 56);
            GL.VertexAttribIPointer(shader.GetAttribute("vBone"), 4, VertexAttribIntegerType.Int, DisplayVertex.Size, new IntPtr(72));
            GL.VertexAttribPointer(shader.GetAttribute("vWeight"), 4, VertexAttribPointerType.Float, false, DisplayVertex.Size, 88);
            GL.VertexAttribPointer(shader.GetAttribute("vUV1"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 104);
            GL.VertexAttribPointer(shader.GetAttribute("vUV2"), 2, VertexAttribPointerType.Float, false, DisplayVertex.Size, 112);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition2"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 124);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition3"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 136);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
        }

        private static void DrawMdoelHoverSelection(STGenericObject p, ShaderProgram shader,
            bool IsSelected, bool IsHovered)
        {
            if (IsHovered && IsSelected)
                GL.Uniform4(shader["pickingColor"], hoverColor);
            else if (IsHovered || IsSelected)
                GL.Uniform4(shader["pickingColor"], selectColor);
            else
                GL.Uniform4(shader["pickingColor"], new Vector4(1));
        }

        private static void DrawModelWireframe(STGenericObject p, ShaderProgram shader)
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
        private static void DrawModelSelection(STGenericObject p, ShaderProgram shader)
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

            GL.Uniform1(shader["colorOverride"], 1);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(1.3f);
            GL.DrawElements(PrimitiveType.Triangles, p.lodMeshes[p.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Uniform1(shader["colorOverride"], 0);

            GL.DrawElements(PrimitiveType.Triangles, p.lodMeshes[p.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
        }

        public override BoundingBox GetSelectionBox()
        {
            Vector3 Min = new Vector3(0);
            Vector3 Max = new Vector3(0);

            foreach (var model in models)
            {
                foreach (var shape in model.shapes)
                {
                    foreach (var vertex in shape.vertices)
                    {
                        Min.X = Math.Min(Min.X, vertex.pos.X);
                        Min.Y = Math.Min(Min.Y, vertex.pos.Y);
                        Min.Z = Math.Min(Min.Z, vertex.pos.Z);
                        Max.X = Math.Max(Max.X, vertex.pos.X);
                        Max.Y = Math.Max(Max.Y, vertex.pos.Y);
                        Max.Z = Math.Max(Max.Z, vertex.pos.Z);
                    }
                }
            }

            return new BoundingBox()
            {
                minX = Min.X,
                minY = Min.Y,
                minZ = Min.Z,
                maxX = Max.X,
                maxY = Max.Y,
                maxZ = Max.Z,
            };
        }


        public override uint SelectAll(GL_ControlBase control)
        {
            Selected = true;
            return REDRAW;
        }

        public override uint SelectDefault(GL_ControlBase control)
        {
            Selected = true;
            return REDRAW;
        }

        public override uint Select(int partIndex, GL_ControlBase control)
        {
            Selected = true;
            return REDRAW;
        }

        public override uint Deselect(int partIndex, GL_ControlBase control)
        {
            Selected = false;
            return REDRAW;
        }

        public override LocalOrientation GetLocalOrientation(int partIndex)
        {
            return new LocalOrientation(position);
        }

        public override bool TryStartDragging(DragActionType actionType, int hoveredPart, out LocalOrientation localOrientation, out bool dragExclusively)
        {
            localOrientation = new LocalOrientation(position);
            dragExclusively = false;
            return Selected;
        }

        public override bool IsInRange(float range, float rangeSquared, Vector3 pos)
        {
            range = 20000; //Make the range large for now. Todo go back to this

            BoundingBox box = GetSelectionBox();

            if (pos.X < box.maxX + range && pos.X > box.minX - range &&
              pos.Y < box.maxY + range && pos.Y > box.minY - range &&
              pos.Z < box.maxZ + range && pos.Z > box.minZ - range)
                return true;

            return false;
        }

        public override uint DeselectAll(GL_ControlBase control)
        {
            Selected = false;
            return REDRAW;
        }

        public override Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        #endregion
    }
}
