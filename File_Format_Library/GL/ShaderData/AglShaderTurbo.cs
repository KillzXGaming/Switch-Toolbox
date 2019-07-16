using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfres.Structs;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL_EditorFramework.GL_Core;

namespace FirstPlugin
{
    public class AglShaderTurbo
    {
        public DisplayFace DisplayCullFace = DisplayFace.Front;
        public RenderStateMode RenderMode = RenderStateMode.Opaque;

        public enum DisplayFace
        {
            Front,
            Back,
            Both,
            None,
        }

        public enum RenderStateMode
        {
            Opaque,
            Mask,
            Translucent,
            Custom,
        }

        AlphaGLControl alphaControl;
        DepthGLControl depthControl;
        ColorGLControl colorControl;


        public void LoadRenderInfo(BfresRenderInfo renderInfo)
        {
            alphaControl = new AlphaGLControl();
            depthControl = new DepthGLControl();
            colorControl = new ColorGLControl();

            switch (renderInfo.Name)
            {
                case "gsys_alpha_test_enable":
                    alphaControl.EnableAlphaTest = ParseString(renderInfo.ValueString);
                    break;
                case "gsys_alpha_test_func":
                    alphaControl.AlphaFunction = ParseAlphaFunction(renderInfo.ValueString);
                    break;
                case "gsys_alpha_test_value":
                    alphaControl.AlphaTestRef = renderInfo.ValueFloat[0];
                    break;
                case "gsys_color_blend_alpha_dst_func":
                    break; 
                case "gsys_color_blend_alpha_op":
                    break;
                case "gsys_color_blend_alpha_src_func":
                    break;
                case "gsys_color_blend_const_color":
                    colorControl.BlendColorConst = ParseFloat4(renderInfo.ValueFloat);
                    break;
                case "gsys_color_blend_rgb_dst_func":
                    break;
                case "gsys_color_blend_rgb_op":
                    break;
                case "gsys_color_blend_rgb_src_func":
                    break;
                case "gsys_depth_test_enable":
                    depthControl.EnableTest = ParseString(renderInfo.ValueString);
                    break;
                case "gsys_depth_test_func":
                    depthControl.DepthFunction = ParseDepthFunction(renderInfo.ValueString);
                    break;
                case "gsys_depth_test_write":
                    depthControl.EnableWrite = ParseString(renderInfo.ValueString);
                    break;
                case "gsys_render_state_display_face":
                    DisplayCullFace = ParseDisplayFace(renderInfo.ValueString);
                    break;
                case "gsys_render_state_mode":
                    RenderMode = ParseRenderStateMode(renderInfo.ValueString);
                    break;
            }
        }

        public void LoadRenderPass(FMAT mat, ShaderProgram shader)
        {
            alphaControl.LoadRenderPass();
        //    depthControl.LoadRenderPass();
      //      colorControl.LoadRenderPass();

            GL.Enable(EnableCap.CullFace);
            switch (DisplayCullFace)
            {
                case DisplayFace.Front:
                    GL.CullFace(CullFaceMode.Back);
                    break;
                case DisplayFace.Back:
                    GL.CullFace(CullFaceMode.Front);
                    break;
                case DisplayFace.None:
                    GL.CullFace(CullFaceMode.FrontAndBack);
                    break;
                case DisplayFace.Both:
                    GL.Disable(EnableCap.CullFace);
                    break;
            }
            switch (RenderMode)
            {
                case RenderStateMode.Opaque:
                    mat.isTransparent = false;
                    break;
            }
        }
        
        private static RenderStateMode ParseRenderStateMode(string[] Value)
        {
            if (Value == null || Value.Length <= 0) return RenderStateMode.Opaque;

            switch (Value[0])
            {
                case "opaque": return RenderStateMode.Opaque;
                case "translucent": return RenderStateMode.Translucent;
                case "mask": return RenderStateMode.Mask;
                case "custom": return RenderStateMode.Custom;
                default:
                    throw new Exception("Unknown Render State Mode " + Value[0]);
            }
        }

        private static DisplayFace ParseDisplayFace(string[] Value)
        {
            if (Value == null || Value.Length <= 0) return DisplayFace.Front;

            switch (Value[0])
            {
                case "front": return DisplayFace.Front;
                case "back": return DisplayFace.Back;
                case "both": return DisplayFace.Both;
                case "none": return DisplayFace.None;
                default:
                    throw new Exception("Unknown Display Face " + Value[0]);
            }
        }

        private static Vector4 ParseFloat4(float[] Values)
        {
            if (Values == null || Values.Length != 4) return new Vector4(1,1,1,1);

            return new Vector4(Values[0], Values[1], Values[2], Values[3]);
        }

        private static DepthFunction ParseDepthFunction(string[] Value)
        {
            if (Value == null || Value.Length <= 0) return DepthFunction.Lequal;

            switch (Value[0])
            {
                case "gequal": return DepthFunction.Gequal;
                case "lequal": return DepthFunction.Lequal;
                case "always": return DepthFunction.Always;
                case "equal": return DepthFunction.Equal;
                case "less": return DepthFunction.Less;
                case "greater": return DepthFunction.Greater;
                case "never": return DepthFunction.Never;
                case "notequal": return DepthFunction.Notequal;
                default:
                    throw new Exception("Unknown Depth Function " + Value[0]);
            }
        }

        private static AlphaFunction ParseAlphaFunction(string[] Value)
        {
            if (Value == null || Value.Length <= 0) return AlphaFunction.Never;

            switch (Value[0])
            {
                case "gequal":   return AlphaFunction.Gequal;
                case "lequal":   return AlphaFunction.Lequal;
                case "always":   return AlphaFunction.Always;
                case "equal":    return AlphaFunction.Equal;
                case "less":     return AlphaFunction.Less;
                case "greater":  return AlphaFunction.Greater;
                case "never":    return AlphaFunction.Never;
                case "notequal": return AlphaFunction.Notequal;
                default:
                    throw new Exception("Unnown Alpha Function " + Value[0]);
            }
        }

        private static bool ParseString(string[] Value)
        {
            if (Value == null || Value.Length <= 0) return false;

            if (Value[0] == "true")
                return true;
            else
                return false;
        }
    }
}
