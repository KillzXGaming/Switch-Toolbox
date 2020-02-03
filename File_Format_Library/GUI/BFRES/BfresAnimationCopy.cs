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
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class BfresAnimationCopy : STForm
    {
        public bool CopySettings => chkAnimSettings.Checked;
        public bool CopyBoneAnims => chkBoneAnims.Checked;
        public bool CopyUserData => chkUserData.Checked;

        public BfresAnimationCopy()
        {
            InitializeComponent();
        }

        public void LoadAnimationSections(FSKA target, List<FSKA> skeletalAnims)
        {
            this.Text = $"Animation Copy [{target.Text}]";

            listViewCustom1.Items.Clear();
            for (int i = 0; i < skeletalAnims.Count; i++) {
                if (target != skeletalAnims[i])
                    listViewCustom1.Items.Add(new ListViewItem(skeletalAnims[i].Text) 
                    {
                        Tag = skeletalAnims[i] 
                    });
            }
        }

        public List<FSKA> GetAnimationCopies()
        {
            List<FSKA> anims = new List<FSKA>();
            foreach (ListViewItem item in listViewCustom1.Items)
                if (item.Checked)
                    anims.Add(item.Tag as FSKA);
            return anims;
        }
    }
}
