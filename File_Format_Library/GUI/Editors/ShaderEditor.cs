using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class ShaderEditor : STUserControl
    {
        public ShaderEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
        }

        public void FillEditor(SHARC.ShaderProgram program, SHARC.Header header)
        {
            LoadProgram(program);
        }

        public void FillEditor(SHARCFBNX.ShaderProgram program, SHARCFBNX.Header header)
        {
            LoadProgram(program);
        }

        public void FillEditor(SHARCFBNX.ShaderVariation var)
        {
            string programXML = Sharc2XML.WriteProgram(var);
            textEditor1.FillEditor(programXML);
            textEditor1.IsXML = true;
        }

        public void FillEditor(SHARCFB.ShaderProgram program, SHARCFB.Header header)
        {
            LoadProgram(program);

            var binary = header.BinaryDatas[program.Index];
            if (binary.Type == SHARCFB.BinaryData.ShaderType.GX2VertexShader)
                hexVertexData.LoadData(header.BinaryDatas[program.Index].Data);
            if (binary.Type == SHARCFB.BinaryData.ShaderType.GX2PixelShader)
                hexPixelData.LoadData(header.BinaryDatas[program.Index].Data);
            if (binary.Type == SHARCFB.BinaryData.ShaderType.GX2GeometryShader)
                hexGeomData.LoadData(header.BinaryDatas[program.Index].Data);
        }

        private void LoadProgram(SHARC.ShaderProgram program)
        {
            string programXML = Sharc2XML.WriteProgram(program);
            textEditor1.FillEditor(programXML);
            textEditor1.IsXML = true;
        }

        private void LoadProgram(SHARCFBNX.ShaderProgram program)
        {
            string programXML = Sharc2XML.WriteProgram(program);
            textEditor1.FillEditor(programXML);
            textEditor1.IsXML = true;
        }

        private void LoadProgram(SHARCFB.ShaderProgram program)
        {
            string programXML = Sharc2XML.WriteProgram(program);
            textEditor1.FillEditor(programXML);
            textEditor1.IsXML = true;
        }

        private void samplerListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
