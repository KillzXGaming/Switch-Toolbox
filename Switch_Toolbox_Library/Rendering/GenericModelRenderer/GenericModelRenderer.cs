using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using Toolbox.Library.IO;
using Toolbox.Library;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Toolbox.Library.Rendering
{
    public class GenericModelRenderer : AbstractGlDrawable
    {
        public virtual float PreviewScale { get; set; } = 1.0f;

        public static List<ITextureContainer> TextureContainers = new List<ITextureContainer>();
        public List<STGenericTexture> Textures = new List<STGenericTexture>();

        public List<GenericRenderedObject> Meshes = new List<GenericRenderedObject>();
        public STSkeleton Skeleton = new STSkeleton();

        public Matrix4 ModelTransform = Matrix4.Identity;

        public virtual bool UsePBR { get; set; } = false;

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

            GenericRenderedObject.DisplayVertex[] Vertices;
            int[] Faces;

            int poffset = 0;
            int voffset = 0;
            List<GenericRenderedObject.DisplayVertex> Vs = new List<GenericRenderedObject.DisplayVertex>();
            List<int> Ds = new List<int>();

            foreach (GenericRenderedObject shape in Meshes)
            {
                List<GenericRenderedObject.DisplayVertex> pv = shape.CreateDisplayVertices();
                Vs.AddRange(pv);

                int GroupOffset = 0;
                int groupIndex = 0;
                if (shape.PolygonGroups.Count > 0)
                {
                    foreach (var group in shape.PolygonGroups)
                    {
                        group.Offset = poffset * 4;

                        for (int i = 0; i < group.displayFaceSize; i++)
                        {
                            Ds.Add(shape.display[GroupOffset + i] + voffset);
                        }

                        poffset += group.displayFaceSize;
                        GroupOffset += group.displayFaceSize;
                    }

                    voffset += pv.Count;
                }
                else if (shape.lodMeshes.Count > 0)
                {
                    shape.Offset = poffset * 4;
      
                    for (int i = 0; i < shape.lodMeshes[shape.DisplayLODIndex].displayFaceSize; i++)
                    {
                        Ds.Add(shape.display[i] + voffset);
                    }
                    poffset += shape.lodMeshes[shape.DisplayLODIndex].displayFaceSize;
                    voffset += pv.Count;
                }
            }

            // Binds
            Vertices = Vs.ToArray();
            Faces = Ds.ToArray();

            // Bind only once!
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<GenericRenderedObject.DisplayVertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * GenericRenderedObject.DisplayVertex.Size), Vertices, BufferUsageHint.StaticDraw);

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
            if (UsePBR)
            {
                string pathFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "GFBModel.frag");
                string pathVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "GFBModel.vert");
                string pathPbrUtiltyFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Utility") + "\\PbrUtility.frag";
                string pathUtiltyFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Utility") + "\\Utility.frag";

                var defaultFrag = new FragmentShader(File.ReadAllText(pathFrag));
                var defaultVert = new VertexShader(File.ReadAllText(pathVert));
                var pbrUtiltyFrag = new FragmentShader(System.IO.File.ReadAllText(pathPbrUtiltyFrag));
                var utiltyFrag = new FragmentShader(System.IO.File.ReadAllText(pathUtiltyFrag));

                defaultShaderProgram = new ShaderProgram(new Shader[]
                {   utiltyFrag, pbrUtiltyFrag, defaultVert, defaultFrag }, control);
            }
            else
            {
                string pathFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "GenericShader.frag");
                string pathVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "GenericShader.vert");
                string pathUtiltyFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Utility") + "\\Utility.frag";

                var defaultFrag = new FragmentShader(File.ReadAllText(pathFrag));
                var defaultVert = new VertexShader(File.ReadAllText(pathVert));
                var utiltyFrag = new FragmentShader(System.IO.File.ReadAllText(pathUtiltyFrag));

                defaultShaderProgram = new ShaderProgram(new Shader[]
                {   utiltyFrag, defaultVert, defaultFrag }, control);
            }
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
            control.UpdateModelMatrix(Matrix4.CreateScale(Runtime.previewScale * PreviewScale) * ModelTransform);

            OnRender(control);

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
            GL.CullFace(CullFaceMode.Back);
        }

        public virtual void OnRender(GLControl control)
        {

        }

        public virtual void SetBoneUniforms(GLControl control, ShaderProgram shader, STSkeleton Skeleton, STGenericObject mesh)
        {
            int i = 0;
            foreach (var bone in Skeleton.bones)
            {
                Matrix4 transform = bone.invert * bone.Transform;
                GL.UniformMatrix4(GL.GetUniformLocation(shader.programs[control], String.Format("bones[{0}]", i++)), false, ref transform);
            }
        }

        private void SetUniformBlocks(STGenericMaterial mat, ShaderProgram shader, STGenericObject m)
        {
       
        }

        public virtual void SetRenderData(STGenericMaterial mat, ShaderProgram shader, STGenericObject m)
        {

        }

        private static void SetUniforms(STGenericMaterial mat, ShaderProgram shader, STGenericObject m)
        {
            //UV Scale
            shader.SetFloat("ColorUVScaleU", 1);
            shader.SetFloat("ColorUVScaleV", 1);

            //UV Translate
            shader.SetFloat("ColorUVTranslateU", 0);
            shader.SetFloat("ColorUVTranslateV", 0);
        }

        private static void SetUniformData(STGenericMaterial mat, ShaderProgram shader, string propertyName)
        {
          
        }

        private static void LoadDebugTextureMaps(ShaderProgram shader)
        {
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
        }

        private void SetTextureUniforms(STGenericMaterial mat, STGenericObject m, ShaderProgram shader)
        {
            SetDefaultTextureAttributes(mat, shader);
            LoadDebugTextureMaps(shader);

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

        private void TextureUniform(ShaderProgram shader, STGenericMaterial mat, bool hasTex, string name, STGenericMatTexture mattex)
        {
            if (mattex.textureState == STGenericMatTexture.TextureState.Binded)
                return;

            // Bind the texture and create the uniform if the material has the right textures. 
            if (hasTex)
            {
                GL.Uniform1(shader[name], BindTexture(mattex, shader));
            }
        }

        public virtual int BindTexture(STGenericMatTexture tex, ShaderProgram shader)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + tex.textureUnit + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.RenderableTex.TexID);

            string activeTex = tex.Name;

            foreach (var container in TextureContainers)
            {
                for (int i = 0; i < container.TextureList?.Count; i++)
                {
                    if (activeTex == container.TextureList[i].Text)
                    {
                        BindGLTexture(tex, shader, container.TextureList[i]);
                        return tex.textureUnit + 1;
                    }
                }
            }

            for (int i = 0; i < Textures?.Count; i++)
            {
                if (activeTex == Textures[i].Text)
                {
                    BindGLTexture(tex, shader, Textures[i]);
                    return tex.textureUnit + 1;
                }
            }

            return tex.textureUnit + 1;
        }

        public static void BindGLTexture(STGenericMatTexture tex, ShaderProgram shader, STGenericTexture texture)
        {
            if (texture.RenderableTex == null || !texture.RenderableTex.GLInitialized)
                texture.LoadOpenGLTexture();

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

        private static void SetDefaultTextureAttributes(STGenericMaterial mat, ShaderProgram shader)
        {
        }

        private void SetRenderSettings(ShaderProgram shader)
        {
            shader.SetInt("renderType", (int)Runtime.viewportShading);
            shader.SetInt("selectedBoneIndex", Runtime.SelectedBoneIndex);
            shader.SetBoolToInt("renderVertColor", Runtime.renderVertColor);
            shader.SetInt("uvChannel", (int)Runtime.uvChannel);

        }

        public virtual void DrawModels(ShaderProgram shader, GL_ControlModern control)
        {
            shader.EnableVertexAttributes();
            foreach (STGenericObject shp in Meshes)
            {
                if (shp.Checked)
                    DrawModel(control, Skeleton, shp.GetMaterial(), shp, shader);
            }
            shader.DisableVertexAttributes();
        }

        private void SetVertexAttributes(STGenericObject m, ShaderProgram shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, GenericRenderedObject.DisplayVertex.Size, 0); //+12
            GL.VertexAttribPointer(shader.GetAttribute("vNormal"), 3, VertexAttribPointerType.Float, false, GenericRenderedObject.DisplayVertex.Size, 12); //+12
            GL.VertexAttribPointer(shader.GetAttribute("vTangent"), 3, VertexAttribPointerType.Float, false, GenericRenderedObject.DisplayVertex.Size, 24); //+12
            GL.VertexAttribPointer(shader.GetAttribute("vUV0"), 2, VertexAttribPointerType.Float, false, GenericRenderedObject.DisplayVertex.Size, 36); //+8
            GL.VertexAttribPointer(shader.GetAttribute("vColor"), 4, VertexAttribPointerType.Float, false, GenericRenderedObject.DisplayVertex.Size, 44); //+16
            GL.VertexAttribIPointer(shader.GetAttribute("vBone"), 4, VertexAttribIntegerType.Int, GenericRenderedObject.DisplayVertex.Size, new IntPtr(60)); //+16
            GL.VertexAttribPointer(shader.GetAttribute("vWeight"), 4, VertexAttribPointerType.Float, false, GenericRenderedObject.DisplayVertex.Size, 76);//+16
            GL.VertexAttribPointer(shader.GetAttribute("vUV1"), 2, VertexAttribPointerType.Float, false, GenericRenderedObject.DisplayVertex.Size, 92);//+8
            GL.VertexAttribPointer(shader.GetAttribute("vUV2"), 2, VertexAttribPointerType.Float, false, GenericRenderedObject.DisplayVertex.Size, 100);//+8
            GL.VertexAttribPointer(shader.GetAttribute("vBinormal"), 3, VertexAttribPointerType.Float, false, GenericRenderedObject.DisplayVertex.Size, 108); //+12
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
        }

        public void DrawModel(GLControl control, STSkeleton Skeleton, STGenericMaterial Material, STGenericObject m, ShaderProgram shader)
        {
            GL.PushAttrib(AttribMask.ColorBufferBit);

            if (m.PolygonGroups.Count > 0)
            {
                foreach (var group in m.PolygonGroups)
                {
                    if (group.faces.Count <= 3)
                        return;

                    if (group.Material != null)
                        Material = group.Material;

                    SetRenderData(Material, shader, m);
                    SetUniforms(Material, shader, m);
                    SetUniformBlocks(Material, shader, m);
                    SetBoneUniforms(control, shader, Skeleton, m);
                    SetVertexAttributes(m, shader);
                    SetTextureUniforms(Material, m, shader);

                    if (m.IsSelected || Material.IsSelected)
                    {
                        DrawModelSelection(group, shader);
                    }
                    else if (Runtime.RenderModelWireframe)
                    {
                        DrawModelWireframe(group, shader);
                    }
                    else
                    {
                        if (Runtime.RenderModels)
                        {
                            GL.DrawElements(GetPrimitiveType(group), group.displayFaceSize, DrawElementsType.UnsignedInt, group.Offset);
                        }
                    }
                }
            }
            else
            {
                if (m.lodMeshes.Count <= 0 || m.lodMeshes[m.DisplayLODIndex].faces.Count <= 3)
                    return;

                SetUniforms(Material, shader, m);
                SetUniformBlocks(Material, shader, m);
                SetBoneUniforms(control, shader, Skeleton, m);
                SetVertexAttributes(m, shader);
                SetTextureUniforms(Material, m, shader);

                if (m.IsSelected)
                {
                    DrawModelSelection(m, shader);
                }
                else if (Runtime.RenderModelWireframe)
                {
                    DrawModelWireframe(m, shader);
                }
                else
                {
                    if (Runtime.RenderModels)
                    {
                        GL.DrawElements(GetPrimitiveType(m.lodMeshes[m.DisplayLODIndex]), m.lodMeshes[m.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, m.Offset);
                    }
                }
            }

            GL.PopAttrib();
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

        private static  PrimitiveType GetPrimitiveType(STGenericObject.LOD_Mesh p)
        {
            switch (p.PrimativeType)
            {
                case STPrimitiveType.Triangles: return PrimitiveType.Triangles;
                case STPrimitiveType.TrangleStrips: return PrimitiveType.TriangleStrip;
                case STPrimitiveType.Quads: return PrimitiveType.Quads;
                case STPrimitiveType.Points: return PrimitiveType.Points;
                case STPrimitiveType.LineStrips: return PrimitiveType.LineStrip;
                case STPrimitiveType.Lines: return PrimitiveType.Lines;
                default: return PrimitiveType.Triangles;
            }
        }

        private static PrimitiveType GetPrimitiveType(STGenericPolygonGroup p)
        {
            switch (p.PrimativeType)
            {
                case STPrimitiveType.Triangles: return PrimitiveType.Triangles;
                case STPrimitiveType.TrangleStrips: return PrimitiveType.TriangleStrip;
                case STPrimitiveType.Quads: return PrimitiveType.Quads;
                case STPrimitiveType.Points: return PrimitiveType.Points;
                case STPrimitiveType.LineStrips: return PrimitiveType.LineStrip;
                case STPrimitiveType.Lines: return PrimitiveType.Lines;
                default: return PrimitiveType.Triangles;
            }
        }

        private static void DrawModelWireframe(STGenericPolygonGroup p, ShaderProgram shader)
        {
            // use vertex color for wireframe color
            shader.SetInt("colorOverride", 1);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(1.5f);
            GL.DrawElements(GetPrimitiveType(p), p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            shader.SetInt("colorOverride", 0);
        }


        private static void DrawModelSelection(STGenericObject p, ShaderProgram shader)
        {
            GL.Uniform1(shader["colorOverride"], 1);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(1.3f);
            GL.DrawElements(GetPrimitiveType(p.lodMeshes[p.DisplayLODIndex]), p.lodMeshes[p.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Uniform1(shader["colorOverride"], 0);

            GL.DrawElements(GetPrimitiveType(p.lodMeshes[p.DisplayLODIndex]), p.lodMeshes[p.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
        }

        private static void DrawModelSelection(STGenericPolygonGroup p, ShaderProgram shader)
        {
            GL.Uniform1(shader["colorOverride"], 1);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(1.3f);
            GL.DrawElements(GetPrimitiveType(p), p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Uniform1(shader["colorOverride"], 0);

            GL.DrawElements(GetPrimitiveType(p), p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
        }
    }
}
