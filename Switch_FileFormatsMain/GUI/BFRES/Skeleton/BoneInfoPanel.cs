using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bfres.Structs;
using Syroot.NintenTools.NSW.Bfres;
using ResU = Syroot.NintenTools.Bfres;

namespace FirstPlugin
{
    public partial class BoneInfoPanel : UserControl
    {
        public BoneInfoPanel()
        {
            InitializeComponent();
        }

        BfresBone activeBone;

        bool Loaded = false;
        public void LoadBone(BfresBone bn)
        {
            Loaded = false;

            activeBone = bn;

            parentTB.Text = "";

            if (bn.parentIndex != -1)
                parentTB.Text = bn.Parent.Text;

            nameIndexUD.Value = bn.GetIndex();

            nameTB.Bind(bn, "BoneName");
            parentIndexUD.Bind(bn, "parentIndex");
            visibleChk.Bind(bn, "Visible");

            Loaded = true;
        }

        private void nameTB_TextChanged(object sender, EventArgs e)
        {

        }

        private void visibleChk_CheckedChanged(object sender, EventArgs e) {
        }
    }
}
