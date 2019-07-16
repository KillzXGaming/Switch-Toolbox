using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bfres.Structs;
using Syroot.NintenTools.Bfres;

namespace FirstPlugin
{
    public partial class DebugInfoBox : Form
    {
        public DebugInfoBox()
        {
            InitializeComponent();
        }
        public void PrintDebugInfo(string FileName)
        {
            ResFile resFile = new ResFile(FileName);

            string txt = "";

            txt += "BFRES Debug Information \n";
            txt += "ResFile ======================================== \n";
            txt += $"Alignment                 {resFile.Alignment} \n";
            txt += $"Version VersionMajor      {resFile.VersionMajor} \n";
            txt += $"Version VersionMajor2      {resFile.VersionMajor2} \n";
            txt += $"Version VersionMinor      {resFile.VersionMinor} \n";
            txt += $"Version VersionMinor2      {resFile.VersionMinor2} \n";
            txt += $"Model Count               {resFile.Models.Count} \n";
            txt += $"Skeletal Anim Count       {resFile.SkeletalAnims.Count} \n";
            txt += $"Shader Param Anim Count   {resFile.ShaderParamAnims.Count} \n";
            txt += $"Color Anim Count          {resFile.ColorAnims.Count} \n";
            txt += $"Tex Srt Anim Count        {resFile.TexSrtAnims.Count} \n";
            txt += $"Tex Pattern Anim Count    {resFile.TexPatternAnims.Count} \n";
            txt += $"Mat Visibility Anim Count {resFile.MatVisibilityAnims.Count} \n";
            txt += $"Shape Anim Count          {resFile.ShapeAnims.Count} \n";
            txt += $"Scene Anim Count          {resFile.SceneAnims.Count} \n";
            txt += $"External File Count       {resFile.ExternalFiles.Count} \n";
            txt += "End of ResFile ======================================== \n";

            foreach (var mdl in resFile.Models.Values)
            {
                txt += $"    FMDL {mdl.Name} \n";

                txt += "         ======================================== \n";
                int index = 0;
                foreach (var shp in mdl.Shapes.Values)
                {
                    txt += $"        FSHP {shp.Name} Index {index++} \n" +
                           $"        Material Index {shp.MaterialIndex} \n" +
                           $"        Bone Index {shp.BoneIndex} \n" +
                           $"        Bone Index {shp.MaterialIndex} \n" +
                           $"        Vertex Buffer Index {shp.VertexBufferIndex} \n" +
                           $"        Vertex Skin Count {shp.VertexSkinCount} \n" +
                           $"        Bone Index {shp.BoneIndex} \n" +
                           $"        Flags {shp.Flags} \n" +
                           $"        TargetAttribCount {shp.TargetAttribCount} \n";
                    txt += "         ======================================== \n";
                }
                index = 0;
                foreach (var mat in mdl.Materials.Values)
                {
                    txt += "         ======================================== \n";
                    txt += $"        FMAT {mat.Name} Index {index++} \n";
                    txt += $"        TextureMaps \n";
                    foreach (var texture in mat.TextureRefs)
                    {
                        txt += $"            Texture {texture.Name} \n";
                    }
                }
            }

            richTextBox1.Text = txt;
        }
    }
}
