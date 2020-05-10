using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using Toolbox.Library.IO;
using Toolbox.Library;
using Toolbox.Library.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FirstPlugin
{
    public class GFBMDL_Render : AbstractGlDrawable, IMeshContainer
    {
        public GFBMDL GfbmdlFile;

        public List<STGenericObject> Meshes { get; set; } = new List<STGenericObject>();

        public Matrix4 ModelTransform = Matrix4.Identity;

        // gl buffer objects
        int vbo_position;
        int ibo_elements;

        private void GenerateBuffers()
        {
            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out ibo_elements);

            UpdateVertexData();
            UpdateTextureMaps();
        }

        public void Destroy()
        {
            GL.DeleteBuffer(vbo_position);
            GL.DeleteBuffer(ibo_elements);
        }

        public void UpdateVertexData()
        {
            if (!Runtime.OpenTKInitialized)
                return;

            GFLXMesh.DisplayVertex[] Vertices;
            int[] Faces;

            int poffset = 0;
            int voffset = 0;
            List<GFLXMesh.DisplayVertex> Vs = new List<GFLXMesh.DisplayVertex>();
            List<int> Ds = new List<int>();

            foreach (GFLXMesh shape in Meshes)
            {
                List<GFLXMesh.DisplayVertex> pv = shape.CreateDisplayVertices();
                Vs.AddRange(pv);

                int GroupOffset = 0;
                int groupIndex = 0;
                foreach (var group in shape.PolygonGroups)
                {
                    group.Offset = poffset * 4;

                    for (int i = 0; i < group.displayFaceSize; i++)
                    {
                        Ds.Add(shape.display[GroupOffset + i] + voffset);
                    }

                    poffset += group.displayFaceSize;
                    GroupOffset += group.displayFaceSize;

                    Console.WriteLine($"GroupOffset {groupIndex++} " + GroupOffset);
                }

                voffset += pv.Count;
            }

            // Binds
            Vertices = Vs.ToArray();
            Faces = Ds.ToArray();

            // Bind only once!
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<GFLXMesh.DisplayVertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * GFLXMesh.DisplayVertex.Size), Vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            GL.BufferData<int>(BufferTarget.ElementArrayBuffer, (IntPtr)(Faces.Length * sizeof(int)), Faces, BufferUsageHint.StaticDraw);

            LibraryGUI.UpdateViewport();
        }

        public void UpdateTextureMaps()
        {
            if (!Runtime.OpenTKInitialized)
                return;

            LibraryGUI.UpdateViewport();
        }

        public ShaderProgram defaultShaderProgram;

        public override void Prepare(GL_ControlModern control)
        {
            string pathFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "GFBModel.frag");
            string pathVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "GFBModel.vert");
            string pathUtiltyFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Utility") + "\\Utility.frag";
            string pathPbrUtiltyFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Utility") + "\\PbrUtility.frag";

            
            var defaultFrag = new FragmentShader(File.ReadAllText(pathFrag));
            var defaultVert = new VertexShader(File.ReadAllText(pathVert));
            var utiltyFrag = new FragmentShader(System.IO.File.ReadAllText(pathUtiltyFrag));
            var pbrUtiltyFrag = new FragmentShader(System.IO.File.ReadAllText(pathPbrUtiltyFrag));

            defaultShaderProgram = new ShaderProgram(new Shader[]
            {   utiltyFrag, pbrUtiltyFrag, defaultVert, defaultFrag }, control);
        }

        public override void Prepare(GL_ControlLegacy control)
        {

        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            if (!Runtime.OpenTKInitialized)
                return;
        }

        public override void Draw(GL_ControlModern control, Pass pass)
        {
            if (!Runtime.OpenTKInitialized || pass == Pass.TRANSPARENT)
                return;

            bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
            if (!buffersWereInitialized)
                GenerateBuffers();

            ShaderProgram shader = defaultShaderProgram;
            control.CurrentShader = shader;
            control.UpdateModelMatrix(Matrix4.CreateScale(Runtime.previewScale) * ModelTransform);

            Matrix4 camMat = control.ModelMatrix * control.CameraMatrix * control.ProjectionMatrix;

            Matrix4 invertedCamera = Matrix4.Identity;
            if (invertedCamera.Determinant != 0)
                invertedCamera = camMat.Inverted();

            Vector3 lightDirection = new Vector3(0f, 0f, -1f);

            shader.SetVector3("difLightDirection", Vector3.TransformNormal(lightDirection, invertedCamera).Normalized());

            // GL.Enable(EnableCap.AlphaTest);
            // GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);

            SetRenderSettings(shader);

            DrawModels(shader, control);


            GL.UseProgram(0);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        private static void SetBoneUniforms(GLControl control, ShaderProgram shader, GFLXMesh mesh)
        {
            int i = 0;
            foreach (var bone in mesh.ParentModel.Skeleton.bones)
            {
                Matrix4 transform = bone.invert * bone.Transform;

                GL.UniformMatrix4(GL.GetUniformLocation(shader.programs[control], String.Format("bones[{0}]", i++)), false, ref transform);
            }

            /*  foreach (var FaceGroup in fshp.Shape.FaceGroups)
              {
                  if (FaceGroup.BoneIndexList == null)
                      continue;

                  for (int i = 0; i < FaceGroup.BoneIndexList.Length; i++)
                  {
                      GL.Uniform1(GL.GetUniformLocation(shader.programs[control], String.Format("boneIds[{0}]", i)), FaceGroup.BoneIndexList[i]);

                      Matrix4 transform = fmdl.Skeleton.Renderable.bones[(int)FaceGroup.BoneIndexList[i]].invert * fmdl.Skeleton.Renderable.bones[(int)FaceGroup.BoneIndexList[i]].Transform;
                      GL.UniformMatrix4(GL.GetUniformLocation(shader.programs[control], String.Format("bones[{0}]", i)), false, ref transform);
                  }
              }*/
        }

        private void SetUniformBlocks(GFLXMaterialData mat, ShaderProgram shader, GFLXMesh m, int id)
        {
            /*     shader.UniformBlockBinding("TexCoord1", 3);
                 GL.GetActiveUniformBlock(shader.program,
                                 shader.GetUniformBlockIndex("TexCoord1"),
                                 ActiveUniformBlockParameter.UniformBlockBinding, out int binding);*/

            /*      GL.BindBuffer(BufferTarget.UniformBuffer, TexCoord1Buffer);
                  GL.BufferData(BufferTarget.UniformBuffer,
                  (IntPtr)MTOBWrapper.TexCoord1.Size,
                  ref mat.TexCoord1Buffer,
                  BufferUsageHint.StaticDraw);
                  GL.BindBuffer(BufferTarget.UniformBuffer, 0);
                  GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 0, TexCoord1Buffer, (IntPtr)0,
                      MTOBWrapper.TexCoord1.Size);
                  GL.BindBuffer(BufferTarget.UniformBuffer, TexCoord1Buffer);
                  GL.BINDBUFFER*/
        }

        private static void SetUniforms(GFLXMaterialData mat, ShaderProgram shader, GFLXMesh m, int id)
        {
            // Texture Maps
          /*  shader.SetBoolToInt("useColorTex", false);
            shader.SetBoolToInt("EmissionMaskUse", false);
            shader.SetBoolToInt("SwitchPriority", false);
            shader.SetBoolToInt("Layer1Enable", false);
            shader.SetBoolToInt("AmbientMapEnable", false);
            shader.SetBoolToInt("NormalMapEnable", false);
            shader.SetBoolToInt("LightTableEnable", false);
            shader.SetBoolToInt("BaseColorAddEnable", false);
            shader.SetBoolToInt("SphereMapEnable", false);
            shader.SetBoolToInt("EffectVal", false);*/

            //Switch UVs
            shader.SetBoolToInt("SwitchEmissionMaskTexUV", false);
            shader.SetBoolToInt("SwitchAmbientTexUV", false);
            shader.SetBoolToInt("SwitchNormalMapUV", false);

            //UV Scale
            shader.SetFloat("ColorUVScaleU", 1);
            shader.SetFloat("ColorUVScaleV", 1);

            //UV Translate
            shader.SetFloat("ColorUVTranslateU", 0);
            shader.SetFloat("ColorUVTranslateV", 0);

            SetUniformData(mat, shader, "ColorUVScaleU");
            SetUniformData(mat, shader, "ColorUVScaleV");
            SetUniformData(mat, shader, "ColorUVTranslateU");
            SetUniformData(mat, shader, "ColorUVTranslateV");
            SetUniformData(mat, shader, "ColorBaseU");
            SetUniformData(mat, shader, "ColorBaseV");
        }

        private static void SetUniformData(GFLXMaterialData mat, ShaderProgram shader, string propertyName)
        {
            if (mat.SwitchParams.ContainsKey(propertyName))
            {
                bool Value = (bool)mat.SwitchParams[propertyName].Value;
                shader.SetBoolToInt(propertyName, Value);
            }
            if (mat.ValueParams.ContainsKey(propertyName))
            {
                var Value = mat.ValueParams[propertyName].Value;
                shader.SetFloat(propertyName, (float)Value);
            }
            if (mat.ColorParams.ContainsKey(propertyName))
            {
                Vector3 Value = (Vector3)mat.ColorParams[propertyName].Value;
                shader.SetVector3(propertyName, Value);
            }
        }

        private static void SetTextureUniforms(GFLXMaterialData mat, GFLXMesh m, ShaderProgram shader)
        {
            if (!mat.HasVertexColors())
                shader.SetBoolToInt("renderVertColor", false);
            else
                shader.SetBoolToInt("renderVertColor", Runtime.renderVertColor);

            SetDefaultTextureAttributes(mat, shader);

            GL.ActiveTexture(TextureUnit.Texture0 + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.RenderableTex.TexID);

            GL.Uniform1(shader["debugOption"], 2);

            GL.ActiveTexture(TextureUnit.Texture11);
            GL.Uniform1(shader["weightRamp1"], 11);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.BoneWeightGradient.Id);

            GL.ActiveTexture(TextureUnit.Texture12);
            GL.Uniform1(shader["weightRamp2"], 12);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.BoneWeightGradient2.Id);


            GL.ActiveTexture(TextureUnit.Texture10);
            GL.Uniform1(shader["UVTestPattern"], 10);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.uvTestPattern.RenderableTex.TexID);

            shader.SetInt("RedChannel", 0);
            shader.SetInt("GreenChannel", 1);
            shader.SetInt("BlueChannel", 2);
            shader.SetInt("AlphaChannel", 3);

            LoadPBRMaps(shader);

            foreach (STGenericMatTexture matex in mat.TextureMaps)
            {
                if (matex.Type == STGenericMatTexture.TextureType.Diffuse)
                {
                    shader.SetBoolToInt("HasDiffuse", true);
                    TextureUniform(shader, mat, true, "DiffuseMap", matex);
                }
                if (matex.Type == STGenericMatTexture.TextureType.Normal)
                {
                    shader.SetBoolToInt("HasNormalMap", true);
                    TextureUniform(shader, mat, true, "NormalMap", matex);
                }
                if (matex.Type == STGenericMatTexture.TextureType.AO)
                {
                    shader.SetBoolToInt("HasAmbientMap", true);
                    TextureUniform(shader, mat, true, "AmbientMap", matex);
                }
            }
        }

        private static void LoadPBRMaps(ShaderProgram shader)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + 26);
            RenderTools.specularPbr.Bind();
            shader.SetInt("specularIbl", 26);
            //    GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

            // PBR IBL
            GL.ActiveTexture(TextureUnit.Texture0 + 25);
            RenderTools.diffusePbr.Bind();
            shader.SetInt("irradianceMap", 25);

            GL.ActiveTexture(TextureUnit.Texture0 + 27);
            RenderTools.brdfPbr.Bind();
            shader.SetInt("brdfLUT", 27);
        }

        private static void TextureUniform(ShaderProgram shader, GFLXMaterialData mat, bool hasTex, string name, STGenericMatTexture mattex)
        {
            if (mattex.textureState == STGenericMatTexture.TextureState.Binded)
                return;

            // Bind the texture and create the uniform if the material has the right textures. 
            if (hasTex)
            {
                GL.Uniform1(shader[name], BindTexture(mattex, shader));
            }
        }

        public static int BindTexture(STGenericMatTexture tex, ShaderProgram shader)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + tex.textureUnit + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.RenderableTex.TexID);

            string activeTex = tex.Name;

            foreach (var bntx in PluginRuntime.bntxContainers)
            {
                if (bntx.Textures.ContainsKey(activeTex))
                {
                    BindBNTX(bntx, tex, shader, activeTex);
                    return tex.textureUnit + 1;
                }
            }

            return tex.textureUnit + 1;
        }

        private static void BindBNTX(BNTX bntx, STGenericMatTexture tex, ShaderProgram shader, string activeTex)
        {
            if (bntx.Textures[activeTex].RenderableTex == null ||
                     !bntx.Textures[activeTex].RenderableTex.GLInitialized)
            {
                bntx.Textures[activeTex].LoadOpenGLTexture();
            }

            BindGLTexture(tex, shader, bntx.Textures[activeTex]);
        }

        private static void BindGLTexture(STGenericMatTexture tex, ShaderProgram shader, STGenericTexture texture)
        {
            if (tex.Type == STGenericMatTexture.TextureType.Diffuse)
            {
                shader.SetInt("RedChannel", (int)texture.RedChannel);
                shader.SetInt("GreenChannel", (int)texture.GreenChannel);
                shader.SetInt("BlueChannel", (int)texture.BlueChannel);
                shader.SetInt("AlphaChannel", (int)texture.AlphaChannel);
            }

            //     GL.ActiveTexture(TextureUnit.Texture0 + texid);
            GL.BindTexture(TextureTarget.Texture2D, texture.RenderableTex.TexID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)STGenericMatTexture.wrapmode[tex.WrapModeS]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)STGenericMatTexture.wrapmode[tex.WrapModeT]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)STGenericMatTexture.minfilter[tex.MinFilter]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)STGenericMatTexture.magfilter[tex.MagFilter]);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, 0.0f);
        }

        private static void SetDefaultTextureAttributes(GFLXMaterialData mat, ShaderProgram shader)
        {
        }

        private void SetRenderSettings(ShaderProgram shader)
        {
            shader.SetInt("renderType", (int)Runtime.viewportShading);
            shader.SetInt("selectedBoneIndex", Runtime.SelectedBoneIndex);
            shader.SetBoolToInt("renderVertColor", Runtime.renderVertColor);
        }

        private void DrawModels(ShaderProgram shader, GL_ControlModern control)
        {
            shader.EnableVertexAttributes();
            foreach (GFLXMesh shp in Meshes)
            {
                if (shp.Checked && shp.AnimationController.IsVisible)
                    DrawModel(control, shp, shader);
            }
            shader.DisableVertexAttributes();
        }

        private void SetVertexAttributes(GFLXMesh m, ShaderProgram shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, GFLXMesh.DisplayVertex.Size, 0); //+12
            GL.VertexAttribPointer(shader.GetAttribute("vNormal"), 3, VertexAttribPointerType.Float, false, GFLXMesh.DisplayVertex.Size, 12); //+12
            GL.VertexAttribPointer(shader.GetAttribute("vTangent"), 3, VertexAttribPointerType.Float, false, GFLXMesh.DisplayVertex.Size, 24); //+12
            GL.VertexAttribPointer(shader.GetAttribute("vUV0"), 2, VertexAttribPointerType.Float, false, GFLXMesh.DisplayVertex.Size, 36); //+8
            GL.VertexAttribPointer(shader.GetAttribute("vColor"), 4, VertexAttribPointerType.Float, false, GFLXMesh.DisplayVertex.Size, 44); //+16
            GL.VertexAttribIPointer(shader.GetAttribute("vBone"), 4, VertexAttribIntegerType.Int, GFLXMesh.DisplayVertex.Size, new IntPtr(60)); //+16
            GL.VertexAttribPointer(shader.GetAttribute("vWeight"), 4, VertexAttribPointerType.Float, false, GFLXMesh.DisplayVertex.Size, 76);//+16
            GL.VertexAttribPointer(shader.GetAttribute("vUV1"), 2, VertexAttribPointerType.Float, false, GFLXMesh.DisplayVertex.Size, 92);//+8
            GL.VertexAttribPointer(shader.GetAttribute("vUV2"), 2, VertexAttribPointerType.Float, false, GFLXMesh.DisplayVertex.Size, 100);//+8
            GL.VertexAttribPointer(shader.GetAttribute("vBinormal"), 3, VertexAttribPointerType.Float, false, GFLXMesh.DisplayVertex.Size, 108); //+12
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
        }

        private void DrawModel(GLControl control, GFLXMesh m, ShaderProgram shader)
        {
            foreach (var group in m.PolygonGroups)
            {
                if (group.faces.Count <= 3)
                    return;

                var Material = m.ParentModel.GenericMaterials[group.MaterialIndex];

                SetUniforms(m.GetMaterial(group), shader, m, m.DisplayId);
                SetUniformBlocks(m.GetMaterial(group), shader, m, m.DisplayId);
                SetBoneUniforms(control, shader, m);
                SetVertexAttributes(m, shader);
                SetTextureUniforms(m.GetMaterial(group), m, shader);

                if (m.IsSelected)
                {
                    DrawModelSelection(group, shader);
                }
                else
                {
                    if (Runtime.RenderModels)
                    {
                        GL.DrawElements(PrimitiveType.Triangles, group.displayFaceSize, DrawElementsType.UnsignedInt, group.Offset);
                    }
                }
            }
        }

        private static void DrawModelSelection(STGenericPolygonGroup p, ShaderProgram shader)
        {
            GL.Uniform1(shader["colorOverride"], 1);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(1.3f);
            GL.DrawElements(PrimitiveType.Triangles, p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Uniform1(shader["colorOverride"], 0);

            GL.DrawElements(PrimitiveType.Triangles, p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
        }
    }
}
