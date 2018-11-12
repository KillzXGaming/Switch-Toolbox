using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Switch_Toolbox.Library;
using GL_Core.Public_Interfaces;
using GL_Core;

namespace FirstPlugin
{
    public partial class PreviewEditor : Form
    {
        GL_ControlModern gL_ControlModern;
        GL_ControlLegacy GL_ControlLegacy;
        TextureViewer textureViewer;

        public PreviewEditor()
        {
            InitializeComponent();

            textureViewer = new TextureViewer();
            textureViewer.LoadTextures();
            textureViewer.Show(dockPanel1, DockState.DockLeft);

            DockContent dockedViewport = new DockContent();
            SetupViewport(dockedViewport);
            dockedViewport.Show(dockPanel1, DockState.Document);
        }
        private void SetupViewport(DockContent dockContent)
        {
            if (!Runtime.UseLegacyGL)
            {
                gL_ControlModern = new GL_Core.GL_ControlModern();
                gL_ControlModern.Dock = DockStyle.Fill;
                gL_ControlModern.Visible = true;
                dockContent.Controls.Add(gL_ControlModern);
            }
            else
            {
                GL_ControlLegacy = new GL_Core.GL_ControlLegacy();
                GL_ControlLegacy.Dock = DockStyle.Fill;
                GL_ControlLegacy.Visible = true;
                dockContent.Controls.Add(GL_ControlLegacy);
            }
        }

        private void PreviewEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            PluginRuntime.bntxContainers.Clear();
            Runtime.abstractGlDrawables.Clear();

            if (gL_ControlModern != null)
                gL_ControlModern.Dispose();
            if (GL_ControlLegacy != null)
                GL_ControlLegacy.Dispose();
            if (textureViewer != null)
            {
                textureViewer.Close();
            }
            textureViewer = null;
            gL_ControlModern = null;
            GL_ControlLegacy = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
