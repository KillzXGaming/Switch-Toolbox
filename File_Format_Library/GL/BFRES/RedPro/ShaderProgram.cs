using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Bfres.Structs;
using Toolbox.Library.IO;

namespace FirstPlugin.RedPro
{
    public class SHShaderProgram : GLShaderGeneric
    {
        public SHARC.Header ShaderArchive { get; set; }
        public SHARC.ShaderProgram Program { get; set; }

        public SHShaderProgram(SHARC.Header shader, SHARC.ShaderProgram program)
        {
            ShaderArchive = shader;
            Program = program;

            this.Compile();
        }

        public bool IsLinked(string shaderArchive, string shaderProgram)
        {
            return (ShaderArchive.Name == shaderArchive.Replace(" ", string.Empty) && Program.Text.Replace(" ", string.Empty) == shaderProgram);
        }

        public void LoadUniforms(FMDL fmdl, FSHP shp, STSkeleton skeleton, GL_EditorFramework.GL_Core.GL_ControlBase control)
        {

        }
    }
}
