using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class ExportModelSettings : STForm
    {
        public DAE.ExportSettings Settings = new DAE.ExportSettings();

        public ExportModelSettings()
        {
            InitializeComponent();

            chkFlipUvsVertical.Checked = Settings.FlipTexCoordsVertical;
            exportTexturesChkBox.Checked = Settings.ExportTextures;
        }

        private void exportTexturesChkBox_CheckedChanged(object sender, EventArgs e) {
            Settings.ExportTextures = exportTexturesChkBox.Checked;
        }

        private void chkFlipUvsVertical_CheckedChanged(object sender, EventArgs e) {
            Settings.FlipTexCoordsVertical = chkFlipUvsVertical.Checked;
        }

        private void stCheckBox1_CheckedChanged(object sender, EventArgs e) {
            Settings.UseOldExporter = chkOldExporter.Checked;
        }

        private void chkVertexColors_CheckedChanged(object sender, EventArgs e) {
            Settings.UseVertexColors = chkVertexColors.Checked;
        }

        private void chkExportRiggedBonesOnly_CheckedChanged(object sender, EventArgs e) {
            Settings.OnlyExportRiggedBones = chkExportRiggedBonesOnly.Checked;
        }

        private void contentContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void chkApplyUVTransforms_CheckedChanged(object sender, EventArgs e) {
            Settings.TransformColorUVs = chkApplyUVTransforms.Checked;
        }

        private void chkTextureChannelComps_CheckedChanged(object sender, EventArgs e) {
            Settings.UseTextureChannelComponents = chkApplyUVTransforms.Checked;
        }
    }
}
