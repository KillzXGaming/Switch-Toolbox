using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Bfres.Structs;

namespace FirstPlugin.Forms
{
    public partial class VisObjectAddDialog : STForm
    {
        public VisObjectAddDialog()
        {
            InitializeComponent();
        }

        public FVIS ActiveAnim { get; set; }

        public void LoadAnim(FVIS anim) { ActiveAnim = anim; }

        public string BoneName { get; set; }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (BoneName == string.Empty)
            {
                MessageBox.Show("Bone name must not be empty! Please fill in a valid name!", "Add Bone Dialog",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                DialogResult = DialogResult.None;
            }
            if (ActiveAnim.BoneNames.Contains(BoneName))
            {
                MessageBox.Show("A bone name already eixsts with that name!  Please fill in a valid name!", "Add Bone Dialog",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

                DialogResult = DialogResult.None;
            }

        }

        private void stTextBox1_TextChanged(object sender, EventArgs e)
        {
            BoneName = stTextBox1.Text;
        }
    }
}
