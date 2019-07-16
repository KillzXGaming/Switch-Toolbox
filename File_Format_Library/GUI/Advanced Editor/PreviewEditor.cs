using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.GL_Core;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public partial class PreviewEditor : STForm
    {
        GL_ControlModern gL_ControlModern;
        GL_ControlLegacy GL_ControlLegacy;
        TextureViewer textureViewer;

        public PreviewEditor()
        {
            InitializeComponent();

            textureViewer = new TextureViewer();
            textureViewer.Dock = DockStyle.Fill;
            stPanel1.Controls.Add(textureViewer);

            textureViewer.LoadTextures();

        }
        private void SetupViewport()
        {

        }

        private void PreviewEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            textureViewer.OnControlClosing();
        }
    }
}
