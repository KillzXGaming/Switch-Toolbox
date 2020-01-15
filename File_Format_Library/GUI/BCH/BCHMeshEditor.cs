using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SPICA.Formats.CtrH3D.Model.Mesh;
using Toolbox.Library.Forms;

namespace FirstPlugin.CtrLibrary.Forms
{
    public partial class BCHMeshEditor : UserControl
    {
        public H3DMeshWrapper MeshWrapper;
        public H3DMesh ActiveMesh => MeshWrapper.Mesh;

        public BCHMeshEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
        }

        public void LoadMesh(H3DMeshWrapper mesh) {
            MeshWrapper = mesh;

            if (ActiveMesh.MetaData != null)
                bchUserDataEditor1.LoadUserData(ActiveMesh.MetaData);
        }
    }
}
