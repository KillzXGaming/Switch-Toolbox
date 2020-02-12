using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LayoutBXLYT;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;

//Code from Wii Layout Editor
//https://github.com/Gericom/WiiLayoutEditor
//This is so materials/tev display correctly for brlyt
namespace LayoutBXLYT.Revolution
{
    public class Shader
    {
        public TevStage[] TevStages;
        public uint TextureCount;
        private float[][] g_color_registers;
        private float[][] g_color_consts;
        private float[] MatColor;
        private byte color_matsrc;
        private byte alpha_matsrc;
        public Shader(Material Material, uint textureCount)
        {
            TextureCount = textureCount;
            this.color_matsrc = Material.ChanControl.ColorMatSource;
            this.alpha_matsrc = Material.ChanControl.AlphaMatSource;
            this.MatColor = new float[]
                {
                    Material.MatColor.R/255f,
                    Material.MatColor.G/255f,
                    Material.MatColor.B/255f,
                    Material.MatColor.A/255f
                };

            TevStages = new TevStage[Material.TevStages.Length];
            for (int i = 0; i < Material.TevStages?.Length; i++)
                TevStages[i] = (TevStage)Material.TevStages[i];

            g_color_registers = new float[3][];
            g_color_registers[0] = new float[]
                {
                    Material.BlackColor.R/255f,
                    Material.BlackColor.G/255f,
                    Material.BlackColor.B/255f,
                    Material.BlackColor.A/255f
                };
            g_color_registers[1] = new float[]
                {
                    Material.WhiteColor.R/255f,
                    Material.WhiteColor.G/255f,
                    Material.WhiteColor.B/255f,
                    Material.WhiteColor.A/255f
                };
            g_color_registers[2] = new float[]
                {
                    Material.ColorRegister3.R/255f,
                    Material.ColorRegister3.G/255f,
                    Material.ColorRegister3.B/255f,
                    Material.ColorRegister3.A/255f
                };
            g_color_consts = new float[4][];
            g_color_consts[0] = new float[]
                {
                    Material.TevColor1.R/255f,
                    Material.TevColor1.G/255f,
                    Material.TevColor1.B/255f,
                    Material.TevColor1.A/255f
                };
            g_color_consts[1] = new float[]
                {
                    Material.TevColor2.R/255f,
                    Material.TevColor2.G/255f,
                    Material.TevColor2.B/255f,
                    Material.TevColor2.A/255f
                };
            g_color_consts[2] = new float[]
                {
                    Material.TevColor3.R/255f,
                    Material.TevColor3.G/255f,
                    Material.TevColor3.B/255f,
                    Material.TevColor3.A/255f
                };
            g_color_consts[3] = new float[]
                {
                    Material.TevColor4.R/255f,
                    Material.TevColor4.G/255f,
                    Material.TevColor4.B/255f,
                    Material.TevColor4.A/255f
                };
        }
        public void RefreshColors(Material Material)
        {
            STColor8 WhiteColor = Material.WhiteColor;
            STColor8 BlackColor = Material.BlackColor;
            STColor8 MatColor = Material.MatColor;
            STColor8 TevColor1 = Material.TevColor1;
            STColor8 TevColor2 = Material.TevColor2;
            STColor8 TevColor3 = Material.TevColor3;
            STColor8 TevColor4 = Material.TevColor4;
            STColor8 ColorRegister3 = Material.ColorRegister3;

            foreach (var animItem in Material.animController.MaterialColors)
            {
                switch ((RevLMCTarget)animItem.Key)
                {
                    case RevLMCTarget.WhiteColorRed:
                        WhiteColor.R = (byte)animItem.Value; break;
                    case RevLMCTarget.WhiteColorGreen:
                        WhiteColor.G = (byte)animItem.Value; break;
                    case RevLMCTarget.WhiteColorBlue:
                        WhiteColor.B = (byte)animItem.Value; break;
                    case RevLMCTarget.WhiteColorAlpha:
                        WhiteColor.A = (byte)animItem.Value; break;
                    case RevLMCTarget.BlackColorRed:
                        BlackColor.R = (byte)animItem.Value; break;
                    case RevLMCTarget.BlackColorGreen:
                        BlackColor.G = (byte)animItem.Value; break;
                    case RevLMCTarget.BlackColorBlue:
                        BlackColor.B = (byte)animItem.Value; break;
                    case RevLMCTarget.BlackColorAlpha:
                        BlackColor.A = (byte)animItem.Value; break;
                    case RevLMCTarget.MatColorRed:
                        MatColor.R = (byte)animItem.Value; break;
                    case RevLMCTarget.MatColorGreen:
                        MatColor.G = (byte)animItem.Value; break;
                    case RevLMCTarget.MatColorBlue:
                        MatColor.B = (byte)animItem.Value; break;
                    case RevLMCTarget.MatColorAlpha:
                        MatColor.A = (byte)animItem.Value; break;
                    case RevLMCTarget.ColorReg3Red:
                        ColorRegister3.R = (byte)animItem.Value; break;
                    case RevLMCTarget.ColorReg3Green:
                        ColorRegister3.G = (byte)animItem.Value; break;
                    case RevLMCTarget.ColorReg3Blue:
                        ColorRegister3.B = (byte)animItem.Value; break;
                    case RevLMCTarget.ColorReg3Alpha:
                        ColorRegister3.A = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor1Red:
                        TevColor1.R = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor1Green:
                        TevColor1.G = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor1Blue:
                        TevColor1.B = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor1Alpha:
                        TevColor1.A = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor2Red:
                        TevColor2.R = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor2Green:
                        TevColor2.G = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor2Blue:
                        TevColor2.B = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor2Alpha:
                        TevColor2.A = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor3Red:
                        TevColor3.R = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor3Green:
                        TevColor3.G = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor3Blue:
                        TevColor3.B = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor3Alpha:
                        TevColor3.A = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor4Red:
                        TevColor4.R = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor4Green:
                        TevColor4.G = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor4Blue:
                        TevColor4.B = (byte)animItem.Value; break;
                    case RevLMCTarget.TevColor4Alpha:
                        TevColor4.A = (byte)animItem.Value; break;
                }
            }

            this.MatColor = new float[]
                {
                    MatColor.R/255f,
                    MatColor.G/255f,
                    MatColor.B/255f,
                    MatColor.A/255f
                };

            g_color_registers = new float[3][];
            g_color_registers[0] = new float[]
                {
                    BlackColor.R/255f,
                    BlackColor.G/255f,
                    BlackColor.B/255f,
                    BlackColor.A/255f
                };
            g_color_registers[1] = new float[]
                {
                    WhiteColor.R/255f,
                    WhiteColor.G/255f,
                    WhiteColor.B/255f,
                    WhiteColor.A/255f
                };
            g_color_registers[2] = new float[]
                {
                    ColorRegister3.R/255f,
                    ColorRegister3.G/255f,
                    ColorRegister3.B/255f,
                    ColorRegister3.A/255f
                };
            g_color_consts = new float[4][];
            g_color_consts[0] = new float[]
                {
                    TevColor1.R/255f,
                    TevColor1.G/255f,
                    TevColor1.B/255f,
                    TevColor1.A/255f
                };
            g_color_consts[1] = new float[]
                {
                    TevColor2.R/255f,
                    TevColor2.G/255f,
                    TevColor2.B/255f,
                    TevColor2.A/255f
                };
            g_color_consts[2] = new float[]
                {
                    TevColor3.R/255f,
                    TevColor3.G/255f,
                    TevColor3.B/255f,
                    TevColor3.A/255f
                };
            g_color_consts[3] = new float[]
                {
                    TevColor4.R/255f,
                    TevColor4.G/255f,
                    TevColor4.B/255f,
                    TevColor4.A/255f
                };
        }

        public void SetInt(string name, int value) {
            GL.Uniform1(GL.GetUniformLocation(program, name), value);
        }

        public void SetMatrix4(string name, ref OpenTK.Matrix4 matrix) {
            GL.UniformMatrix4(GL.GetUniformLocation(program, name), false, ref matrix);
        }

        public void SetVec3(string name, ref OpenTK.Vector3 value) {
            GL.Uniform3(GL.GetUniformLocation(program, name), value);
        }

        public void SetVec2(string name, ref OpenTK.Vector2 value) {
            GL.Uniform2(GL.GetUniformLocation(program, name), value);
        }

        public void Enable()
        {
            GL.UseProgram(program);
            for (int i = 0; i < 3; i++)
            {
                String ss = "color_register" + i;
                GL.Uniform4(GL.GetUniformLocation(program, ss), g_color_registers[i][0], g_color_registers[i][1], g_color_registers[i][2], g_color_registers[i][3]);
            }
            for (int i = 0; i < 1; i++)
            {
                String ss = "matColor";
                GL.Uniform4(GL.GetUniformLocation(program, ss), MatColor[0], MatColor[1], MatColor[2], MatColor[3]);
            }
            for (int i = 0; i < 4; i++)
            {
                String ss = "color_const" + i;
                GL.Uniform4(GL.GetUniformLocation(program, ss), g_color_consts[i][0], g_color_consts[i][1], g_color_consts[i][2], g_color_consts[i][3]);
            }
            // TODO: cache value of GetUniformLocation
            //Gl.glUniform4fv(Gl.glGetUniformLocation(program, "registers"), 3, new float[] { g_color_registers[0][0], g_color_registers[0][1], g_color_registers[0][2], g_color_registers[0][3], g_color_registers[1][0], g_color_registers[1][1], g_color_registers[1][2], g_color_registers[1][3], g_color_registers[2][0], g_color_registers[2][1], g_color_registers[2][2], g_color_registers[2][3] });
        }
        public void Disable()
        {
            GL.UseProgram(0);

            //Gl.glDeleteProgram(program);
            //Gl.glDeleteShader(vertex_shader);
            //Gl.glDeleteShader(fragment_shader);
            // TODO: cache value of GetUniformLocation
            //Gl.glUniform4fv(Gl.glGetUniformLocation(program, "registers"), 3, g_color_registers[0]);
        }
        public void Compile()
        {
            // w.e good for now
            uint sampler_count = (uint)TextureCount;
            if (sampler_count == 0)
                sampler_count = 1;
            //if (sampler_count == 
            //{
            //	sampler_count = 1;
            //}
            // generate vertex/fragment shader code
            //{
            StringBuilder vert_ss = new StringBuilder();
            //String vert_ss = "";

            string FlipTextureFunction = @"
            vec2 rotateUV(vec2 uv, float rotation)
            {
                float mid = 0.5;
                return vec2(
                    cos(rotation) * (uv.x - mid) + sin(rotation) * (uv.y - mid) + mid,
                    cos(rotation) * (uv.y - mid) - sin(rotation) * (uv.x - mid) + mid
                );
            }

            vec2 SetFlip(vec2 tex)
            {
                vec2 outTexCoord = tex;

	            if (flipTexture == 1) //FlipH
	                    return vec2(-1.0, 1.0) * tex + vec2(1.0, 0.0);
	            else if (flipTexture == 2) //FlipV
	                    return vec2(1.0, -1.0) * tex + vec2(0.0, 1.0);
	            else if (flipTexture == 3) //Rotate90
	            {
	                    float degreesR = 90.0;
	                    return rotateUV(tex, radians(degreesR));
                }
	            else if (flipTexture == 4) //Rotate180
	            {
		                float degreesR = 180.0;
	                    return rotateUV(tex, radians(degreesR));
	            }
	            else if (flipTexture == 5) //Rotate270
	            {
		                float degreesR = 270.0;
	                    return rotateUV(tex, radians(degreesR));
	            }
	            return outTexCoord;
            }";

            vert_ss.AppendLine("uniform int flipTexture;");
            vert_ss.AppendLine("uniform mat4 rotationMatrix;");
            vert_ss.AppendLine("uniform mat4 textureTransforms[3];");
            vert_ss.Append($"{FlipTextureFunction}");
            vert_ss.AppendLine("void main()");
            vert_ss.AppendLine("{");
            {
                vert_ss.AppendLine("gl_FrontColor = gl_Color;");
                vert_ss.AppendLine("gl_BackColor = gl_Color;");

                for (uint i = 0; i != sampler_count; ++i) {
                    vert_ss.AppendFormat("gl_TexCoord[{0}] = textureTransforms[{0}] * gl_MultiTexCoord{0};\n", i);
                    vert_ss.AppendFormat("gl_TexCoord[{0}].st = SetFlip(vec2(0.5, 0.5) + gl_TexCoord[{0}].st);\n", i);
                }

                vert_ss.AppendLine("gl_Position = gl_ModelViewProjectionMatrix * rotationMatrix * gl_Vertex;");
            }
            vert_ss.AppendLine("}");

            // create/compile vertex shader
            vertex_shader = GL.CreateShader(ShaderType.VertexShader);

            {
                var vert_src_str = vert_ss.ToString();
                //const GLchar* vert_src = vert_src_str.c_str();
                GL.ShaderSource(vertex_shader, 1, new string[] { vert_src_str }, new int[] { vert_src_str.Length });
            }

            //}	// done generating vertex shader

            GL.CompileShader(vertex_shader);

            // generate fragment shader code
            //{
            StringBuilder frag_ss = new StringBuilder();
            //frag_ss += "uniform sampler2D tex;";
            // uniforms
            for (uint i = 0; i != sampler_count; ++i) {
                frag_ss.AppendFormat("uniform sampler2D textures{0};\n", i);
                frag_ss.AppendFormat("uniform int hasTexture{0};\n", i);
            }

            frag_ss.AppendLine("uniform sampler2D uvTestPattern;");
            frag_ss.AppendLine("uniform int debugShading;");

            //frag_ss += "uniform vec4 registers[3]" + ";";

            for (uint i = 0; i < 3; ++i)
                frag_ss.AppendFormat("uniform vec4 color_register{0};\n", i);

            frag_ss.AppendFormat("uniform vec4 matColor;\n");

            for (uint i = 0; i < 4; ++i)
                frag_ss.AppendFormat("uniform vec4 color_const{0};\n", i);
            //frag_ss << "uniform vec4 color_constant" ";";
            frag_ss.AppendLine("vec4 color_constant;");

            frag_ss.AppendLine("vec4 rasColor;");

            frag_ss.AppendLine("void main()");
            frag_ss.AppendLine("{");
            {
                string[] rasColor =
                    {
                        "matColor",
                        "gl_Color"
                    };

                frag_ss.AppendFormat("rasColor.rgb = {0}.rgb;\n", rasColor[color_matsrc]);
                frag_ss.AppendFormat("rasColor.a = {0}.a;\n", rasColor[alpha_matsrc]);

                frag_ss.AppendLine("vec4 color_previous;");
                frag_ss.AppendLine("vec4 color_texture;");

                for (uint i = 0; i < 3; ++i)
                    frag_ss.AppendFormat("vec4 color_registers{0} = color_register{0};\n", i);

                for (uint i = 0; i < 4; ++i)
                    frag_ss.AppendFormat("vec4 color_consts{0} = color_const{0};\n", i);

                string[] color_inputs =
    {
        "color_previous"+".rgb",
        "color_previous"+".aaa",
        "color_registers"+"0"+".rgb",
        "color_registers"+"0"+".aaa",
        "color_registers"+"1"+".rgb",
        "color_registers"+"1"+".aaa",
        "color_registers"+"2"+".rgb",
        "color_registers"+"2"+".aaa",
        "color_texture"+".rgb",
        "color_texture"+".aaa",
        "rasColor"+".rgb",
        "rasColor"+".aaa",
        "vec3(1.0)",
        "vec3(0.5)",
        "color_constant"+".rgb",
        "vec3(0.0)"
    };

                string[] alpha_inputs = new string[]
    {
        "color_previous"+".a",
        "color_registers"+"0"+".a",
        "color_registers"+"1"+".a",
        "color_registers"+"2"+".a",
        "color_texture"+".a",
        "rasColor"+".a",
        "color_constant"+".a",
        "0.0"
    };

                string[] output_registers = new string[]
    {
        "color_previous",
        "color_registers"+"0",
        "color_registers"+"1",
        "color_registers"+"2"
    };

                frag_ss.AppendLine("const vec3 comp16 = vec3(1.0, 255.0, 0.0), comp24 = vec3(1.0, 255.0, 255.0 * 255.0);");
                //if (Textures.Length == 0)
                //{
                //	frag_ss += "gl_FragColor = color_registers1;";
                //}
                //else
                {
                    if (TevStages.Length != 0 && TevStages[0] != null)
                    {
                        foreach (var stage in TevStages)
                        {
                            // current texture color
                            // 0xff is a common value for a disabled texture
                            if ((byte)stage.TexCoord < sampler_count)
                            {
                                frag_ss.AppendFormat("if (hasTexture{0} == 1)\n", (int)stage.TexCoord);
                                frag_ss.AppendFormat("  color_texture = texture2D(textures{0}, gl_TexCoord[{1}].st);\n", (int)stage.TexCoord, (int)stage.TexCoord);
                                frag_ss.AppendLine("else");
                                frag_ss.AppendLine("    color_texture = vec4(1);");
                            }
                            string color = "";
                            if ((byte)stage.ColorConstantSel <= 7)
                            {
                                switch ((byte)stage.ColorConstantSel)
                                {
                                    case 0: color = "vec3(1.0)"; break;
                                    case 1: color = "vec3(0.875)"; break;
                                    case 2: color = "vec3(0.75)"; break;
                                    case 3: color = "vec3(0.625)"; break;
                                    case 4: color = "vec3(0.5)"; break;
                                    case 5: color = "vec3(0.375)"; break;
                                    case 6: color = "vec3(0.25)"; break;
                                    case 7: color = "vec3(0.125)"; break;
                                }

                            }
                            else if ((byte)stage.ColorConstantSel < 0xc)
                            {
                                //warn("getColorOp(): unknown konst %x", konst);
                                //return "ERROR";
                                color = "vec3(1.0)";
                            }
                            else
                            {
                                string[] v1 = { "color_consts0", "color_consts1", "color_consts2", "color_consts3" };
                                string[] v2 = { ".rgb", ".rrr", ".ggg", ".bbb", ".aaa" };

                                color = v1[((byte)stage.ColorConstantSel - 0xc) % 4] + v2[((byte)stage.ColorConstantSel - 0xc) / 4];
                            }
                            string alpha = "";
                            if ((byte)stage.AlphaConstantSel <= 7)
                            {
                                switch (stage.AlphaConstantSel)
                                {
                                    case TevKAlphaSel.Constant1_1: alpha = "vec3(1.0)"; break;
                                    case TevKAlphaSel.Constant7_8: alpha = "vec3(0.875)"; break;
                                    case TevKAlphaSel.Constant3_4: alpha = "vec3(0.75)"; break;
                                    case TevKAlphaSel.Constant5_8: alpha = "vec3(0.625)"; break;
                                    case TevKAlphaSel.Constant1_2: alpha = "vec3(0.5)"; break;
                                    case TevKAlphaSel.Constant3_8: alpha = "vec3(0.375)"; break;
                                    case TevKAlphaSel.Constant1_4: alpha = "vec3(0.25)"; break;
                                    case TevKAlphaSel.Constant1_8: alpha = "vec3(0.125)"; break;
                                }

                            }
                            else if ((byte)stage.AlphaConstantSel < 0x10)
                            {
                                //warn("getColorOp(): unknown konst %x", konst);
                                //return "ERROR";
                                color = "1.0";
                            }
                            else
                            {
                                string[] v1 = { "color_consts0", "color_consts1", "color_consts2", "color_consts3" };
                                string[] v2 = { ".r", ".g", ".b", ".a" };

                                alpha = v1[((byte)stage.AlphaConstantSel - 0x10) % 4] + v2[((byte)stage.AlphaConstantSel - 0x10) / 4];
                            }
                            frag_ss.AppendFormat("color_constant = vec4({0}, {1});\n", color, alpha);


                            frag_ss.AppendLine("{");
                            {

                                // all 4 inputs
                                frag_ss.AppendFormat("vec4 a = vec4({0}, {1});\n", color_inputs[(byte)stage.ColorA], alpha_inputs[(byte)stage.AlphaA]);
                                frag_ss.AppendFormat("vec4 b = vec4({0}, {1});\n", color_inputs[(byte)stage.ColorB], alpha_inputs[(byte)stage.AlphaB]);
                                frag_ss.AppendFormat("vec4 c = vec4({0}, {1});\n", color_inputs[(byte)stage.ColorC], alpha_inputs[(byte)stage.AlphaC]);
                                frag_ss.AppendFormat("vec4 d = vec4({0}, {1});\n", color_inputs[(byte)stage.ColorD], alpha_inputs[(byte)stage.AlphaD]);


                                // TODO: could eliminate this result variable
                                frag_ss.AppendLine("vec4 result;");

                                if ((byte)stage.ColorOp != (byte)stage.AlphaOp)
                                {
                                    write_tevop((byte)stage.ColorOp, ".rgb", ref frag_ss);
                                    write_tevop((byte)stage.AlphaOp, ".a", ref frag_ss);
                                }
                                else
                                    write_tevop((byte)stage.ColorOp, "", ref frag_ss);

                                string[] bias =
                            {
                                "+0",
                                "+0.5",
                                "-0.5"
                            };

                                string[] scale =
                            {
                                "*1",
                                "*2",
                                "*4",
                                "*0.5"
                            };

                                if ((byte)stage.ColorOp < 2)
                                {
                                    frag_ss.AppendFormat("{0}.rgb = (result.rgb{1}){2};\n", output_registers[(byte)stage.ColorRegID], bias[(byte)stage.ColorBias], scale[(byte)stage.ColorScale]);
                                }
                                else
                                {
                                    frag_ss.AppendFormat("{0}.rgb = result.rgb;\n", output_registers[(byte)stage.ColorRegID]);
                                }

                                if ((byte)stage.AlphaOp < 2)
                                {
                                    frag_ss.AppendFormat("{0}.a = (result.a{1}){2};\n", output_registers[(byte)stage.AlphaRegID], bias[(byte)stage.AlphaBias], scale[(byte)stage.AlphaScale]);
                                }
                                else
                                {
                                    frag_ss.AppendFormat("{0}.a = result.a;\n", output_registers[(byte)stage.AlphaRegID]);
                                }

                                if (stage.ColorClamp && (byte)stage.ColorOp < 2)
                                {
                                    frag_ss.AppendFormat("{0}.rgb = clamp({0}.rgb,vec3(0.0, 0.0, 0.0),vec3(1.0, 1.0, 1.0));\n", output_registers[(byte)stage.ColorRegID]);
                                }
                                if (stage.AlphaClamp && (byte)stage.AlphaOp < 2)
                                {
                                    frag_ss.AppendFormat("{0}.a = clamp({0}.a, 0.0, 1.0);\n", output_registers[(byte)stage.AlphaRegID]);
                                }
                            }
                            frag_ss.AppendLine("}");
                        }

                    }
                    else
                    {
                        //frag_ss += "vec4 color = texture2D(textures0,gl_TexCoord[0].st);";
                        //frag_ss += "gl_FragColor = mix(color,color_registers1,color_registers0);";
                        //frag_ss += "gl_FragColor = color + color_registers0;";
                        for (int i = 0; i < 1; i++)
                        {
                            // current texture color
                            // 0xff is a common value for a disabled texture
                            if (i < sampler_count)
                            {
                                frag_ss.AppendFormat("if (hasTexture{0} == 1)\n", i);
                                frag_ss.AppendFormat("  color_texture = texture2D(textures{0}, gl_TexCoord[{0}].st);\n", i);
                                frag_ss.AppendLine("else");
                                frag_ss.AppendLine("    color_texture = vec4(1);");
                            }

                            frag_ss.AppendLine("{");
                            {
                                // all 4 inputs
                                frag_ss.AppendFormat("vec4 a = vec4({0}, {1});\n", color_inputs[2], alpha_inputs[1]);
                                frag_ss.AppendFormat("vec4 b = vec4({0}, {1});\n", color_inputs[4], alpha_inputs[2]);
                                frag_ss.AppendFormat("vec4 c = vec4({0}, {1});\n", color_inputs[8], alpha_inputs[4]);
                                frag_ss.AppendFormat("vec4 d = vec4({0}, {1});\n", color_inputs[0xf], alpha_inputs[0x7]);


                                // TODO: could eliminate this result variable
                                frag_ss.AppendLine("vec4 result;");

                                write_tevop(0, "", ref frag_ss);

                                // output register
                                frag_ss.AppendFormat("{0}.rgb = result.rgb;\n", output_registers[0]);
                                frag_ss.AppendFormat("{0}.a = result.a;\n", output_registers[0]);
                            }
                            frag_ss.AppendLine("}");

                            // current texture color
                            // 0xff is a common value for a disabled texture
                            if (i < sampler_count)
                            {
                                frag_ss.AppendFormat("if (hasTexture{0} == 1)\n", i);
                                frag_ss.AppendFormat("  color_texture = texture2D(textures{0}, gl_TexCoord[{0}].st);\n", i);
                                frag_ss.AppendLine("else");
                                frag_ss.AppendLine("    color_texture = vec4(1);");
                            }

                            frag_ss.AppendLine("{");
                            {
                                // all 4 inputs
                                frag_ss.AppendFormat("vec4 a = vec4({0}, {1});\n", color_inputs[0xf], alpha_inputs[0x7]);
                                frag_ss.AppendFormat("vec4 b = vec4({0}, {1});\n", color_inputs[0], alpha_inputs[0]);
                                frag_ss.AppendFormat("vec4 c = vec4({0}, {1});\n", color_inputs[10], alpha_inputs[5]);
                                frag_ss.AppendFormat("vec4 d = vec4({0}, {1});\n", color_inputs[0xf], alpha_inputs[0x7]);



                                // TODO: could eliminate this result variable
                                frag_ss.AppendLine("vec4 result;");

                                write_tevop(0, "", ref frag_ss);

                                // output register
                                frag_ss.AppendFormat("{0}.rgb = result.rgb;\n", output_registers[0]);
                                frag_ss.AppendFormat("{0}.a = result.a;\n", output_registers[0]);
                            }
                            frag_ss.AppendLine("}");
                        }
                    }
                    frag_ss.AppendLine("gl_FragColor = color_previous;");
                }
            }

            frag_ss.AppendLine("if (debugShading == 4)");
            frag_ss.AppendLine("   gl_FragColor = texture2D(uvTestPattern, gl_TexCoord[0].st);");               
            frag_ss.AppendLine("}");

            //std::cout << frag_ss.str() << '\n';

            // create/compile fragment shader
            fragment_shader = GL.CreateShader(ShaderType.FragmentShader);

            {
                var frag_src_str = frag_ss.ToString();
                GL.ShaderSource(fragment_shader, 1, new String[] { frag_src_str }, new int[] { frag_src_str.Length });
            }

            //}	// done generating fragment shader

            GL.CompileShader(fragment_shader);

            // check compile status of both shaders
            //{
            int vert_compiled = 0;
            int frag_compiled = 0;

            GL.GetShader(vertex_shader, ShaderParameter.CompileStatus, out vert_compiled);
            GL.GetShader(fragment_shader, ShaderParameter.CompileStatus, out frag_compiled);

            string vertlog = GL.GetShaderInfoLog(vertex_shader);
            string fraglog = GL.GetShaderInfoLog(fragment_shader);
            Console.WriteLine(vertlog);
            Console.WriteLine(fraglog);

            if (vert_compiled == 0)
            {
                Console.WriteLine($"");
                //std::cout << "Failed to compile vertex shader\n";
            }

            if (frag_compiled == 0)
            {
                //std::cout << "Failed to compile fragment shader\n";
            }

            // create program, attach shaders
            program = GL.CreateProgram();
            GL.AttachShader(program, vertex_shader);
            GL.AttachShader(program, fragment_shader);

            // link program, check link status
            GL.LinkProgram(program);
            int link_status;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out link_status);

            if (link_status == 0)
            {
                //std::cout << "Failed to link program!\n";
            }

            GL.UseProgram(program);

            // set uniforms
            for (uint i = 0; i != sampler_count; ++i)
            {
                String ss = "textures" + i;
                GL.Uniform1(GL.GetUniformLocation(program, ss), (int)i);
            }
            for (int i = 0; i < 3; i++)
            {
                String ss = "color_register" + i;
                GL.Uniform4(GL.GetUniformLocation(program, ss), g_color_registers[i][0], g_color_registers[i][1], g_color_registers[i][2], g_color_registers[i][3]);
            }
            for (int i = 0; i < 1; i++)
            {
                String ss = "matColor";
                GL.Uniform4(GL.GetUniformLocation(program, ss), MatColor[0], MatColor[1], MatColor[2], MatColor[3]);
            }
            for (int i = 0; i < 4; i++)
            {
                String ss = "color_const" + i;
                GL.Uniform4(GL.GetUniformLocation(program, ss), g_color_consts[i][0], g_color_consts[i][1], g_color_consts[i][2], g_color_consts[i][3]);
            }
            // print log
            //{
            int length;
            string infolog;
            GL.GetProgramInfoLog(program, 10240, out length, out infolog);
            //std::cout << infolog;
            //}

            // pause
            //std::cin.get();
            //}
        }
        private void write_tevop(byte tevop, String swiz, ref StringBuilder frag_ss)
        {
            String condition_end = (" ? c : vec4(0.0))");
            condition_end += swiz;

            // d is added with every op except SUB
            if (tevop < 14)
                frag_ss.AppendFormat("result{0} = d{0} {1} ", swiz, (tevop == 1 ? '-' : '+'));

            String compare_op = ((tevop & 1) != 0) ? "==" : ">";

            switch (tevop)
            {
                case 0: // ADD
                case 1: // SUB
                    frag_ss.AppendFormat("mix(a{0}, b{0}, c{0})", swiz);
                    break;

                case 8: // COMP_R8_GT
                case 9: // COMP_R8_EQ
                    frag_ss.AppendFormat("((a.r {0} b.r){1}", compare_op, condition_end);
                    System.Windows.Forms.MessageBox.Show(tevop.ToString());
                    break;

                case 10: // COMP_GR16_GT
                case 11: // COMP_GR16_EQ
                    frag_ss.AppendFormat("((dot(a.gr, comp16) {0} dot(b.gr, comp16)){1}", compare_op, condition_end);
                    System.Windows.Forms.MessageBox.Show(tevop.ToString());
                    break;

                case 12: // COMP_BGR24_GT
                case 13: // COMP_BGR24_EQ
                    frag_ss.AppendFormat("((dot(a.bgr, comp24) {0} dot(b.bgr, comp24)){1}", compare_op, condition_end);
                    System.Windows.Forms.MessageBox.Show(tevop.ToString());
                    break;

                // TODO:
                case 14: // COMP_RGB8_GT
                case 15: // COMP_RGB8_EQ
                         //frag_ss += "  if(a" + swiz+ compare_op + "b" + swiz + ")\n    " + "result" + swiz + " = " + "c" + swiz + ";\n"
                         //	+ "  else\n    " + "result" + swiz + " = " + "vec4(0.0)" + swiz;
                    if (swiz == ".rgb")
                    {
                        frag_ss.AppendFormat("result.r = d.r + ");
                        frag_ss.AppendFormat("((a.r {0} b.r)" + " ? c.r : vec4(0.0).r);\n", compare_op);
                        frag_ss.AppendFormat("result.g = d.g + ");
                        frag_ss.AppendFormat("((a.g {0} b.g)" + " ? c.g : vec4(0.0).g);\n", compare_op);
                        frag_ss.AppendFormat("result.b = d.b + ");
                        frag_ss.AppendFormat("((a.b {0} b.b)" + " ? c.b : vec4(0.0).b);\n", compare_op);
                    }
                    else if (swiz == ".a")
                    {
                        frag_ss.AppendFormat("result.a = d.a + ");
                        frag_ss.AppendFormat("((a.a {0} b.a)" + " ? c.a : vec4(0.0).a);", compare_op);
                    }
                    else
                    {
                        frag_ss.AppendFormat("result.r = d.r + ");
                        frag_ss.AppendFormat("((a.r {0} b.r)" + " ? c.r : vec4(0.0).r);\n", compare_op);
                        frag_ss.AppendFormat("result.g = d.g + ");
                        frag_ss.AppendFormat("((a.g {0} b.g)" + " ? c.g : vec4(0.0).g);\n", compare_op);
                        frag_ss.AppendFormat("result.b = d.b + ");
                        frag_ss.AppendFormat("((a.b {0} b.b)" + " ? c.b : vec4(0.0).b);\n", compare_op);
                        frag_ss.AppendFormat("result.a = d.a + ");
                        frag_ss.AppendFormat("((a.a {0} b.a)" + " ? c.a : vec4(0.0).a);", compare_op);
                    }
                    //frag_ss += "(a.rgb " + compare_op + " b.rgb)" + condition_end;
                    //frag_ss += "(a.rgb " + compare_op + " b.rgb)" + condition_end;
                    //frag_ss += "(a.rgb " + compare_op + " b.rgb)" + condition_end;
                    break;

                default:
                    frag_ss.AppendFormat("mix(a{0}, b{0}, c{0})", swiz);
                    System.Windows.Forms.MessageBox.Show(tevop.ToString());
                    //std::cout << "Unsupported tevop!! " << (int)tevop << '\n';
                    break;
            }
            frag_ss.AppendLine(";");
        }
        public int program = 0, fragment_shader = 0, vertex_shader = 0;
    }
}