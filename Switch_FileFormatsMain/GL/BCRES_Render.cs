using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using BcresLibrary;

namespace FirstPlugin
{
    public class BCRES_Render : AbstractGlDrawable
    {
        public Matrix4 ModelTransform = Matrix4.Identity;

        public List<CMDLWrapper> Models = new List<CMDLWrapper>();

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

            SOBJWrapper.DisplayVertex[] Vertices;
            int[] Faces;

            int poffset = 0;
            int voffset = 0;
            List<SOBJWrapper.DisplayVertex> Vs = new List<SOBJWrapper.DisplayVertex>();
            List<int> Ds = new List<int>();
            foreach (CMDLWrapper m in Models)
            {
                foreach (SOBJWrapper shape in m.Shapes)
                {
                    shape.Offset = poffset * 4;
                    List<SOBJWrapper.DisplayVertex> pv = shape.CreateDisplayVertices();
                    Vs.AddRange(pv);

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
            GL.BufferData<SOBJWrapper.DisplayVertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * SOBJWrapper.DisplayVertex.Size), Vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            GL.BufferData<int>(BufferTarget.ElementArrayBuffer, (IntPtr)(Faces.Length * sizeof(int)), Faces, BufferUsageHint.StaticDraw);

            LibraryGUI.Instance.UpdateViewport();
        }

        public void UpdateTextureMaps()
        {
            if (!Runtime.OpenTKInitialized)
                return;

            foreach (BCRESGroupNode bcresTexGroup in PluginRuntime.bcresTexContainers)
            {
                foreach (var tex in bcresTexGroup.ResourceNodes)
                {
                    if (!((TXOBWrapper)tex.Value).RenderableTex.GLInitialized)
                        ((TXOBWrapper)tex.Value).LoadOpenGLTexture();
                }
            }

            LibraryGUI.Instance.UpdateViewport();
        }

        public ShaderProgram defaultShaderProgram;

        public override void Prepare(GL_ControlModern control)
        {
            string pathFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bcres") + "\\BCRES.frag";
            string pathVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bcres") + "\\BCRES.vert";

            var defaultFrag = new FragmentShader(File.ReadAllText(pathFrag));
            var defaultVert = new VertexShader(File.ReadAllText(pathVert));

            defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);
        }

        public override void Prepare(GL_ControlLegacy control)
        {
            string pathFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bcres") + "\\BCRES.frag";
            string pathVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bcres") + "\\BCRES.vert";

            var defaultFrag = new FragmentShader(File.ReadAllText(pathFrag));
            var defaultVert = new VertexShader(File.ReadAllText(pathVert));

            defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);
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

        private static void SetBoneUniforms(ShaderProgram shader, CMDLWrapper fmdl, SOBJWrapper fshp)
        {
            foreach (var FaceGroup in fshp.Shape.FaceGroups)
            {
                for (int i = 0; i < FaceGroup.BoneIndexList.Length; i++)
                {
                    GL.Uniform1(GL.GetUniformLocation(shader.program, String.Format("boneIds[{0}]", i)), i);

                    Matrix4 transform = fmdl.Skeleton.Renderable.bones[(int)FaceGroup.BoneIndexList[i]].invert * fmdl.Skeleton.Renderable.bones[(int)FaceGroup.BoneIndexList[i]].Transform;
                    GL.UniformMatrix4(GL.GetUniformLocation(shader.program, String.Format("bones[{0}]", i)), false, ref transform);
                }
            }
        }

        private static void SetUniforms(MTOBWrapper mat, ShaderProgram shader, SOBJWrapper m, int id)
        {

        }

        private static void SetTextureUniforms(MTOBWrapper mat, SOBJWrapper m, ShaderProgram shader)
        {
            SetDefaultTextureAttributes(mat, shader);

            GL.ActiveTexture(TextureUnit.Texture0 + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.Id);

            GL.Uniform1(shader["debugOption"], 2);

            GL.ActiveTexture(TextureUnit.Texture11);
            GL.Uniform1(shader["weightRamp1"], 11);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.BoneWeightGradient.Id);

            GL.ActiveTexture(TextureUnit.Texture12);
            GL.Uniform1(shader["weightRamp2"], 12);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.BoneWeightGradient2.Id);


            GL.ActiveTexture(TextureUnit.Texture10);
            GL.Uniform1(shader["UVTestPattern"], 10);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.uvTestPattern.Id);

            foreach (STGenericMatTexture matex in mat.TextureMaps)
            {
                if (matex.Type == STGenericMatTexture.TextureType.Diffuse)
                    TextureUniform(shader, mat, true, "DiffuseMap", matex);
            }
        }

        private static void TextureUniform(ShaderProgram shader, MTOBWrapper mat, bool hasTex, string name, STGenericMatTexture mattex)
        {
            if (mattex.textureState == STGenericMatTexture.TextureState.Binded)
                return;

            // Bind the texture and create the uniform if the material has the right textures. 
            if (hasTex)
            {
                GL.Uniform1(shader[name], BindTexture(mattex));
            }
        }

        public static int BindTexture(STGenericMatTexture tex)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + tex.textureUnit + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.Id);

            string activeTex = tex.Name;

            foreach (var bcresTexContainer in PluginRuntime.bcresTexContainers)
            {

                if (bcresTexContainer.ResourceNodes.ContainsKey(activeTex))
                {
                    TXOBWrapper txob = (TXOBWrapper)bcresTexContainer.ResourceNodes[activeTex];

                    if (txob.RenderableTex == null || !txob.RenderableTex.GLInitialized)
                        txob.LoadOpenGLTexture();

                    BindGLTexture(tex, txob.RenderableTex.TexID);
                }
            }
            return tex.textureUnit + 1;
        }
        private static void BindGLTexture(STGenericMatTexture tex, int texid)
        {
            //     GL.ActiveTexture(TextureUnit.Texture0 + texid);
            GL.BindTexture(TextureTarget.Texture2D, texid);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)STGenericMatTexture.wrapmode[tex.wrapModeS]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)STGenericMatTexture.wrapmode[tex.wrapModeT]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)STGenericMatTexture.minfilter[tex.minFilter]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)STGenericMatTexture.magfilter[tex.magFilter]);
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, 0.0f);
        }

        private static void SetDefaultTextureAttributes(MTOBWrapper mat, ShaderProgram shader)
        {
        }

        private void SetRenderSettings(ShaderProgram shader)
        {
            shader.SetInt("renderType", (int)Runtime.viewportShading);
            shader.SetInt("selectedBoneIndex", Runtime.SelectedBoneIndex);
        }

        private void DrawModels(ShaderProgram shader, GL_ControlModern control)
        {
            shader.EnableVertexAttributes();
            foreach (CMDLWrapper mdl in Models)
            {
                if (mdl.Checked)
                {
                    foreach (SOBJWrapper shp in mdl.Shapes)
                    {
                        DrawModel(shp, mdl, shader, mdl.IsSelected);
                    }
                }
            }
            shader.DisableVertexAttributes();
        }

        private void SetVertexAttributes(SOBJWrapper m, ShaderProgram shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 0); //+12
            GL.VertexAttribPointer(shader.GetAttribute("vNormal"), 3, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 12); //+12
            GL.VertexAttribPointer(shader.GetAttribute("vTangent"), 3, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 24); //+12
            GL.VertexAttribPointer(shader.GetAttribute("vUV0"), 2, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 36); //+8
            GL.VertexAttribPointer(shader.GetAttribute("vColor"), 4, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 44); //+16
            GL.VertexAttribIPointer(shader.GetAttribute("vBone"), 4, VertexAttribIntegerType.Int, SOBJWrapper.DisplayVertex.Size, new IntPtr(60)); //+16
            GL.VertexAttribPointer(shader.GetAttribute("vWeight"), 4, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 76);//+16
            GL.VertexAttribPointer(shader.GetAttribute("vUV1"), 2, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 92);//+8
            GL.VertexAttribPointer(shader.GetAttribute("vUV2"), 2, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 100);//+8
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
        }

        private void DrawModel(SOBJWrapper m, CMDLWrapper mdl, ShaderProgram shader, bool drawSelection)
        {
            if (m.lodMeshes[m.DisplayLODIndex].faces.Count <= 3)
                return;

            SetBoneUniforms(shader, mdl, m);
            SetVertexAttributes(m, shader);
            SetTextureUniforms(m.MaterialWrapper, m, shader);
            SetUniforms(m.MaterialWrapper, shader,m, m.DisplayId);

            if ((m.IsSelected))
            {
                DrawModelSelection(m, shader);
            }
            else
            {
                if (Runtime.RenderModels)
                {
                    GL.DrawElements(PrimitiveType.Triangles, m.lodMeshes[m.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, m.Offset);
                }
            }
        }

        private static void DrawModelSelection(STGenericObject p, ShaderProgram shader)
        {

        }
    }
}
