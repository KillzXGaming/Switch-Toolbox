using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Rendering;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;

namespace FirstPlugin
{
    //Rendering methods based on noclip
    //https://github.com/magcius/noclip.website/blob/master/src/oot3d/render.ts
    public class CMB_Renderer : GenericModelRenderer
    {
        public override float PreviewScale { get; set; } = 0.01f;

        public List<CTXB.TextureWrapper> TextureList = new List<CTXB.TextureWrapper>();

        private string FragmentShader()
        {
            string frag = "";
            return frag;
        }

        private string GenerateAlphaTestCompare(AlphaFunction compare, int reference)
        {
            float refSingle = (float)reference;
            switch (compare)
            {
                case AlphaFunction.Never: return "false";
                case AlphaFunction.Less: return $"t_CmbOut.a <  {refSingle}";
                case AlphaFunction.Lequal: return $"t_CmbOut.a <= {refSingle}";
                case AlphaFunction.Equal: return $"t_CmbOut.a == {refSingle}";
                case AlphaFunction.Notequal: return $"t_CmbOut.a != {refSingle}";
                case AlphaFunction.Greater: return $"t_CmbOut.a >  ${refSingle}";
                case AlphaFunction.Gequal: return $"t_CmbOut.a >= ${refSingle}";
                case AlphaFunction.Always: return "true";
                default: throw new Exception("Unsupported alpha function");
            }
        }

        private string GenerateTextureEnvironment()
        {
            string vale = $"";
            return vale;
        }

        public override void OnRender(GLControl control)
        {
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public override void DrawModels(ShaderProgram shader, GL_ControlModern control)
        {
            shader.EnableVertexAttributes();

            List<STGenericObject> opaque = new List<STGenericObject>();
            List<STGenericObject> transparent = new List<STGenericObject>();

            for (int m = 0; m < Meshes.Count; m++)
            {
                if (((CMB.CMBMaterialWrapper)Meshes[m].GetMaterial()).CMBMaterial.AlphaTest.Enabled)
                    transparent.Add(Meshes[m]);
                else
                    opaque.Add(Meshes[m]);
            }

            for (int m = 0; m < transparent.Count; m++)
            {
                DrawModel(control, Skeleton, transparent[m].GetMaterial(), transparent[m], shader);
            }

            for (int m = 0; m < opaque.Count; m++)
            {
                DrawModel(control, Skeleton, opaque[m].GetMaterial(), opaque[m], shader);
            }

            shader.DisableVertexAttributes();
        }

        public override void SetRenderData(STGenericMaterial mat, ShaderProgram shader, STGenericObject m)
        {
            var cmbMaterial = ((CMB.CMBMaterialWrapper)mat).CMBMaterial;
            var cmbMesh = ((CMB.CmbMeshWrapper)m);

            bool HasNoNormals = cmbMesh.Mesh.HasNormal == false;

            shader.SetBoolToInt("HasNoNormals", HasNoNormals);
            shader.SetBoolToInt("isTransparent", cmbMaterial.BlendEnabled);

            SetGLCullMode(cmbMaterial.CullMode);

            if (cmbMaterial.BlendEnabled)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendColor(cmbMaterial.BlendColor.R / 255, cmbMaterial.BlendColor.G / 255, cmbMaterial.BlendColor.B / 255, cmbMaterial.BlendColor.A / 255);
                GL.BlendFunc(ConvertBlendFunc(cmbMaterial.BlendFunction.AlphaSrcFunc), ConvertBlendFunc(cmbMaterial.BlendFunction.AlphaDstFunc));
            }
            else
                GL.Disable(EnableCap.Blend);

            if (cmbMaterial.AlphaTest.Enabled)
            {
                GL.Enable(EnableCap.AlphaTest);
                GL.AlphaFunc(ConvertTestFunction(cmbMaterial.AlphaTest.Function), cmbMaterial.AlphaTest.Reference / 255f);
            }
            else
                GL.Disable(EnableCap.AlphaTest);
        }

        private AlphaFunction ConvertTestFunction(ZeldaLib.CtrModelBinary.Types.TestFunc func)
        {
            switch (func)
            {
                case ZeldaLib.CtrModelBinary.Types.TestFunc.Always: return AlphaFunction.Always;
                case ZeldaLib.CtrModelBinary.Types.TestFunc.Equal: return AlphaFunction.Equal;
                case ZeldaLib.CtrModelBinary.Types.TestFunc.Gequal: return AlphaFunction.Gequal;
                case ZeldaLib.CtrModelBinary.Types.TestFunc.Greater: return AlphaFunction.Greater;
                case ZeldaLib.CtrModelBinary.Types.TestFunc.Lequal: return AlphaFunction.Lequal;
                case ZeldaLib.CtrModelBinary.Types.TestFunc.Less: return AlphaFunction.Less;
                case ZeldaLib.CtrModelBinary.Types.TestFunc.Never: return AlphaFunction.Never;
                case ZeldaLib.CtrModelBinary.Types.TestFunc.Notequal: return AlphaFunction.Notequal;
                default: return AlphaFunction.Always;
            }
        }

        private BlendingFactor ConvertBlendFunc(ZeldaLib.CtrModelBinary.Types.BlendFunc factor)
        {
            switch (factor)
            {
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.ConstantAlpha: return BlendingFactor.ConstantAlpha;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.ConstantColor: return BlendingFactor.ConstantColor;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.DestinationAlpha: return BlendingFactor.DstAlpha;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.DestinationColor: return BlendingFactor.DstColor;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.One: return BlendingFactor.One;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.OneMinusConstantAlpha: return BlendingFactor.OneMinusConstantAlpha;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.OneMinusConstantColor: return BlendingFactor.OneMinusConstantColor;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.OneMinusDestinationAlpha: return BlendingFactor.OneMinusDstAlpha;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.OneMinusDestinationColor: return BlendingFactor.OneMinusDstColor;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.OneMinusSourceAlpha: return BlendingFactor.OneMinusSrcAlpha;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.OneMinusSourceColor: return BlendingFactor.OneMinusSrcColor;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.SourceAlpha: return BlendingFactor.SrcAlpha;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.SourceColor: return BlendingFactor.SrcColor;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.SourceAlphaSaturate: return BlendingFactor.SrcAlphaSaturate;
                case ZeldaLib.CtrModelBinary.Types.BlendFunc.Zero: return BlendingFactor.Zero;
                default: return BlendingFactor.Zero;
            }
        }

        private void SetGLCullMode(ZeldaLib.CtrModelBinary.Types.CullMode CullMode)
        {
            GL.Enable(EnableCap.CullFace);

            if (CullMode == ZeldaLib.CtrModelBinary.Types.CullMode.Back)
                GL.CullFace(CullFaceMode.Back);
            if (CullMode == ZeldaLib.CtrModelBinary.Types.CullMode.Front)
                GL.CullFace(CullFaceMode.Front);
            if (CullMode == ZeldaLib.CtrModelBinary.Types.CullMode.Both)
                GL.CullFace(CullFaceMode.FrontAndBack);
            if (CullMode == ZeldaLib.CtrModelBinary.Types.CullMode.None)
                GL.Disable(EnableCap.CullFace);

        }

        public override int BindTexture(STGenericMatTexture tex, ShaderProgram shader)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + tex.textureUnit + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.RenderableTex.TexID);

            string activeTex = tex.Name;

            foreach (var texture in TextureList)
            {
                if (TextureList.IndexOf(texture) == ((CMB.CMBTextureMapWrapper)tex).TextureIndex)
                {
                    BindGLTexture(tex, shader, TextureList[((CMB.CMBTextureMapWrapper)tex).TextureIndex]);
                    return tex.textureUnit + 1;
                }
            }

            return tex.textureUnit + 1;
        }
    }
}
