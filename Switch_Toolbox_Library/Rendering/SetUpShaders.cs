using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Shaders;
using SFGraphics.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.Rendering
{
    public static class ShaderTools
    {
        public static string shaderSourceDirectory;
        public static string executableDir;

        public static void SetUpShaders(bool forceBinaryUpdate = false)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            shaderSourceDirectory = Path.Combine(executableDir, "Shader");

            // Reset the shaders first so that shaders can be replaced.
            OpenTKSharedResources.shaders.Clear();
            SetUpAllShaders();

            System.Diagnostics.Debug.WriteLine("Shader Setup: {0} ms", stopwatch.ElapsedMilliseconds);
        }

        private static void SetUpAllShaders()
        {
            if (Switch_Toolbox.Library.Runtime.UseLegacyGL)
                SetUpLegacyBfresShaders();
            else
            {
                SetUpBfresShaders();
                SetUBoneShaders();
            }
        }
        private static void SetUpLegacyBfresShaders()
        {
            List<string> bfresSharedShaders = new List<string>
            {
                "Bfres\\Legacy\\BFRES.vert",
                "Bfres\\Legacy\\BFRES.frag",
            };

            CreateAndAddShader("BFRES", bfresSharedShaders.ToArray());
            CreateAndAddShader("BFRES_Debug", bfresSharedShaders.ToArray());

        }

        private static void SetUBoneShaders()
        {
            List<string> boneSharedShaders = new List<string>
            {
                "Bone.vert",
                "Bone.frag",
            };

            CreateAndAddShader("BONE", boneSharedShaders.ToArray());
        }

        private static void SetUpBfresShaders()
        {
            List<string> bfresSharedShaders = new List<string>
            {
                "Bfres\\BFRES.vert",
                "Bfres\\BFRES_utility.frag",
                "Utility\\Utility.frag"
            };
            List<string> NormalsSharedShaders = new List<string>
            {
                "Bfres\\Normals.frag",
                "Bfres\\Normals.vert",
                "Bfres\\Normals.geom",
            };
            List<string> HDRShaders = new List<string>
            {
                "HDRSkyBox\\HDRSkyBox.vert",
                "HDRSkyBox\\HDRSkyBox.frag",
            };
            

            List<string> bfresDebugShaders = new List<string>(bfresSharedShaders);
            bfresDebugShaders.Add("Bfres\\BFRES_Debug.frag");
            bfresDebugShaders.Add("Bfres\\BFRESTurboShadow.frag");

            List<string> bfresShaders = new List<string>(bfresSharedShaders);
            bfresShaders.Add("Bfres\\BFRES.frag");
            bfresShaders.Add("Bfres\\BFRESTurboShadow.frag");

            List<string> bfresPBRShaders = new List<string>(bfresSharedShaders);
            bfresPBRShaders.Add("Bfres\\BFRES_PBR.frag");
            bfresPBRShaders.Add("Bfres\\BFRESTurboShadow.frag");


            List<string> bfresBotwShaders = new List<string>(bfresSharedShaders);
            bfresBotwShaders.Add("Bfres\\BFRES_Botw.frag");


            CreateAndAddShader("BFRES", bfresShaders.ToArray());
            CreateAndAddShader("BFRES_PBR", bfresPBRShaders.ToArray());
            CreateAndAddShader("BFRES_Debug", bfresDebugShaders.ToArray());
            CreateAndAddShader("BFRES_Botw", bfresBotwShaders.ToArray());
            CreateAndAddShader("BFRES_Normals", NormalsSharedShaders.ToArray());
            CreateAndAddShader("KCL", "KCL.frag", "KCL.vert");
            CreateAndAddShader("HDRSkyBox", HDRShaders.ToArray());
        }

        public static void CreateAndAddShader(string shaderProgramName, params string[] shaderRelativePaths)
        {
            if (!OpenTKSharedResources.shaders.ContainsKey(shaderProgramName))
            {
                Shader shader = CreateShader(shaderProgramName, shaderRelativePaths);
                OpenTKSharedResources.shaders.Add(shaderProgramName, shader);
            }
        }

        private static Shader CreateShader(string shaderProgramName, string[] shaderRelativePaths)
        {
            Shader shader = new Shader();
            LoadShaderFiles(shader, shaderRelativePaths);
            return shader;
        }

        private static void LoadShaderFiles(Shader shader, string[] shaderRelativePaths)
        {
            var shaders = new List<Tuple<string, ShaderType, string>>();
            foreach (string file in shaderRelativePaths)
            {
                // The input paths are relative to the main shader directory.
                string shaderPath = shaderSourceDirectory + "\\" + file;
                if (!File.Exists(shaderPath))
                    continue;

                // Read the shader file.
                string shaderName = Path.GetFileNameWithoutExtension(shaderPath);
                string shaderSource = File.ReadAllText(shaderPath);

                // Determine the shader type based on the file extension.
                ShaderType shaderType = ShaderType.FragmentShader;
                if (file.EndsWith(".vert"))
                    shaderType = ShaderType.VertexShader;
                else if (file.EndsWith(".frag"))
                    shaderType = ShaderType.FragmentShader;
                else if (file.EndsWith(".geom"))
                    shaderType = ShaderType.GeometryShader;

                shaders.Add(new Tuple<string, ShaderType, string>(shaderSource, shaderType, shaderName));
            }
            shader.LoadShaders(shaders);
        }

        public static void SystemColorVector3Uniform(Shader shader, System.Drawing.Color color, string name)
        {
            shader.SetVector3(name, ColorUtils.GetVector4(color).Xyz);
        }

        public static void SaveErrorLogs()
        {
            // Export error logs for all the shaders.
            List<String> compileErrorList = new List<String>();
            int successfulCompilations = OpenTKSharedResources.shaders.Count;
            foreach (string shaderName in OpenTKSharedResources.shaders.Keys)
            {
                if (!OpenTKSharedResources.shaders[shaderName].LinkStatusIsOk)
                {
                    compileErrorList.Add(shaderName);
                    successfulCompilations -= 1;
                }

                // Create the error logs directory if not found.
                string errorLogDirectory = executableDir + "\\Shader Error Logs\\";
                if (!Directory.Exists(errorLogDirectory))
                    Directory.CreateDirectory(errorLogDirectory);

                // Export the error log.
                string logExport = OpenTKSharedResources.shaders[shaderName].GetErrorLog();
                File.WriteAllText(errorLogDirectory + shaderName + " Error Log.txt", logExport.Replace("\n", Environment.NewLine));
            }

            // Display how many shaders correctly compiled.
            string message = String.Format("{0} of {1} shaders compiled successfully. Error logs have been saved to the Shader Error Logs directory.\n",
                successfulCompilations, OpenTKSharedResources.shaders.Count);

            // Display the shaders that didn't compile.
            if (compileErrorList.Count > 0)
            {
                message += "The following shaders failed to compile:\n";
                foreach (String shader in compileErrorList)
                    message += shader + "\n";
            }

            MessageBox.Show(message, "GLSL Shader Error Logs Exported");
        }
    }
}
