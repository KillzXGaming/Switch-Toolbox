using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LayoutBXLYT;
using OpenTK.Graphics.OpenGL;

//Code from Wii Layout Editor
//https://github.com/Gericom/WiiLayoutEditor
//This is so materials/tev display correctly for brlyt
namespace WiiLayoutEditor.IO
{
/*    public class Shader
    {
        public BRLYT.Material.TevStage[] TevStages;
        public int[] Textures;
        private float[][] g_color_registers;
        private float[][] g_color_consts;
        private float[] MatColor;
        private byte color_matsrc;
        private byte alpha_matsrc;
        public Shader(BRLYT.Material Material, int[] textures)
        {
            this.color_matsrc = Material.ChanControl.ColorMaterialSource;
            this.alpha_matsrc = Material.ChanControl.AlphaMaterialSource;
            this.MatColor = new float[]
                {
                    Material.MatColor.R/255f,
                    Material.MatColor.G/255f,
                    Material.MatColor.B/255f,
                    Material.MatColor.A/255f
                };
            TevStages = Material.TevStageEntries;
            Textures = textures;
            g_color_registers = new float[3][];
            g_color_registers[0] = new float[]
                {
                    Material.ForeColor.R/255f,
                    Material.ForeColor.G/255f,
                    Material.ForeColor.B/255f,
                    Material.ForeColor.A/255f
                };
            g_color_registers[1] = new float[]
                {
                    Material.BackColor.R/255f,
                    Material.BackColor.G/255f,
                    Material.BackColor.B/255f,
                    Material.BackColor.A/255f
                };
            g_color_registers[2] = new float[]
                {
                    Material.ColorReg3.R/255f,
                    Material.ColorReg3.G/255f,
                    Material.ColorReg3.B/255f,
                    Material.ColorReg3.A/255f
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
        public void RefreshColors(BRLYT.Material Material)
        {
            this.MatColor = new float[]
                {
                    Material.MatColor.R/255f,
                    Material.MatColor.G/255f,
                    Material.MatColor.B/255f,
                    Material.MatColor.A/255f
                };
            g_color_registers = new float[3][];
            g_color_registers[0] = new float[]
                {
                    Material.ForeColor.R/255f,
                    Material.ForeColor.G/255f,
                    Material.ForeColor.B/255f,
                    Material.ForeColor.A/255f
                };
            g_color_registers[1] = new float[]
                {
                    Material.BackColor.R/255f,
                    Material.BackColor.G/255f,
                    Material.BackColor.B/255f,
                    Material.BackColor.A/255f
                };
            g_color_registers[2] = new float[]
                {
                    Material.ColorReg3.R/255f,
                    Material.ColorReg3.G/255f,
                    Material.ColorReg3.B/255f,
                    Material.ColorReg3.A/255f
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
     
        public void Enable()
        {
            GL.UseProgram(program);
            //for (int i = 0; i < Textures.Length; ++i)
            //{
            //	String ss = "textures" + i;
            //	Gl.glUniform1i(Gl.glGetUniformLocation(program, ss), (int)i);
            //}
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
            //Gl.glDeleteProgram(program);
            //Gl.glDeleteShader(vertex_shader);
            //Gl.glDeleteShader(fragment_shader);
            // TODO: cache value of GetUniformLocation
            //Gl.glUniform4fv(Gl.glGetUniformLocation(program, "registers"), 3, g_color_registers[0]);
        }
        public void Compile()
        {
            // w.e good for now
            uint sampler_count = (uint)Textures.Length;
            //if (sampler_count == 0)
            //{
            //	sampler_count = 1;
            //}
            // generate vertex/fragment shader code
            //{
            StringBuilder vert_ss = new StringBuilder();
            //String vert_ss = "";

            vert_ss.AppendLine("void main()");
            vert_ss.AppendLine("{");
            {
                vert_ss.AppendLine("gl_FrontColor = gl_Color;");
                vert_ss.AppendLine("gl_BackColor = gl_Color;");

                for (uint i = 0; i != sampler_count; ++i)
                    vert_ss.AppendFormat("gl_TexCoord[{0}] = gl_TextureMatrix[{0}] * gl_MultiTexCoord{0};\n", i);

                vert_ss.AppendLine("gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;");
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
            for (uint i = 0; i != sampler_count; ++i)
                frag_ss.AppendFormat("uniform sampler2D textures{0};\n", i);

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
                            if (stage.TexCoord < sampler_count)
                            {
                                frag_ss.AppendFormat("color_texture = texture2D(textures{0}, gl_TexCoord[{1}].st);\n", (int)stage.TexCoord, (int)stage.TexCoord);
                            }
                            string color = "";
                            if (stage.ColorConstantSel <= 7)
                            {
                                switch (stage.ColorConstantSel)
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
                            else if (stage.ColorConstantSel < 0xc)
                            {
                                //warn("getColorOp(): unknown konst %x", konst);
                                //return "ERROR";
                                color = "vec3(1.0)";
                            }
                            else
                            {
                                string[] v1 = { "color_consts0", "color_consts1", "color_consts2", "color_consts3" };
                                string[] v2 = { ".rgb", ".rrr", ".ggg", ".bbb", ".aaa" };

                                color = v1[(stage.ColorConstantSel - 0xc) % 4] + v2[(stage.ColorConstantSel - 0xc) / 4];
                            }
                            string alpha = "";
                            if (stage.AlphaConstantSel <= 7)
                            {
                                switch (stage.AlphaConstantSel)
                                {
                                    case 0: alpha = "vec3(1.0)"; break;
                                    case 1: alpha = "vec3(0.875)"; break;
                                    case 2: alpha = "vec3(0.75)"; break;
                                    case 3: alpha = "vec3(0.625)"; break;
                                    case 4: alpha = "vec3(0.5)"; break;
                                    case 5: alpha = "vec3(0.375)"; break;
                                    case 6: alpha = "vec3(0.25)"; break;
                                    case 7: alpha = "vec3(0.125)"; break;
                                }

                            }
                            else if (stage.AlphaConstantSel < 0x10)
                            {
                                //warn("getColorOp(): unknown konst %x", konst);
                                //return "ERROR";
                                color = "1.0";
                            }
                            else
                            {
                                string[] v1 = { "color_consts0", "color_consts1", "color_consts2", "color_consts3" };
                                string[] v2 = { ".r", ".g", ".b", ".a" };

                                alpha = v1[(stage.AlphaConstantSel - 0x10) % 4] + v2[(stage.AlphaConstantSel - 0x10) / 4];
                            }
                            frag_ss.AppendFormat("color_constant = vec4({0}, {1});\n", color, alpha);


                            frag_ss.AppendLine("{");
                            {

                                // all 4 inputs
                                frag_ss.AppendFormat("vec4 a = vec4({0}, {1});\n", color_inputs[stage.ColorA], alpha_inputs[stage.AlphaA]);
                                frag_ss.AppendFormat("vec4 b = vec4({0}, {1});\n", color_inputs[stage.ColorB], alpha_inputs[stage.AlphaB]);
                                frag_ss.AppendFormat("vec4 c = vec4({0}, {1});\n", color_inputs[stage.ColorC], alpha_inputs[stage.AlphaC]);
                                frag_ss.AppendFormat("vec4 d = vec4({0}, {1});\n", color_inputs[stage.ColorD], alpha_inputs[stage.AlphaD]);


                                // TODO: could eliminate this result variable
                                frag_ss.AppendLine("vec4 result;");

                                if (stage.ColorOp != stage.AlphaOp)
                                {
                                    write_tevop(stage.ColorOp, ".rgb", ref frag_ss);
                                    write_tevop(stage.AlphaOp, ".a", ref frag_ss);
                                }
                                else
                                    write_tevop(stage.ColorOp, "", ref frag_ss);

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

                                if (stage.ColorOp < 2)
                                {
                                    frag_ss.AppendFormat("{0}.rgb = (result.rgb{1}){2};\n", output_registers[stage.ColorRegID], bias[stage.ColorBias], scale[stage.ColorScale]);
                                }
                                else
                                {
                                    frag_ss.AppendFormat("{0}.rgb = result.rgb;\n", output_registers[stage.ColorRegID]);
                                }

                                if (stage.AlphaOp < 2)
                                {
                                    frag_ss.AppendFormat("{0}.a = (result.a{1}){2};\n", output_registers[stage.AlphaRegID], bias[stage.AlphaBias], scale[stage.AlphaScale]);
                                }
                                else
                                {
                                    frag_ss.AppendFormat("{0}.a = result.a;\n", output_registers[stage.AlphaRegID]);
                                }

                                if (stage.ColorClamp && stage.ColorOp < 2)
                                {
                                    frag_ss.AppendFormat("{0}.rgb = clamp({0}.rgb,vec3(0.0, 0.0, 0.0),vec3(1.0, 1.0, 1.0));\n", output_registers[stage.ColorRegID]);
                                }
                                if (stage.AlphaClamp && stage.AlphaOp < 2)
                                {
                                    frag_ss.AppendFormat("{0}.a = clamp({0}.a, 0.0, 1.0);\n", output_registers[stage.AlphaRegID]);
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
                                frag_ss.AppendFormat("color_texture = texture2D(textures{0}, gl_TexCoord[{0}].st);\n", i);
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
                                frag_ss.AppendFormat("color_texture = texture2D(textures{0}, gl_TexCoord[{0}].st);\n", i);
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

            if (vert_compiled == 0)
            {
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
    }*/
}