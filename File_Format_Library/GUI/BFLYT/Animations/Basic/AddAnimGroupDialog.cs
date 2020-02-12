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

namespace LayoutBXLYT
{
    public partial class AddAnimGroupDialog : STForm
    {
        private BxlanPAI1 AnimInfo;
        private BxlytHeader ParentLayout;

        public AddAnimGroupDialog(BxlanPAI1 bxlanPai, BxlytHeader parentLayout)
        {
            InitializeComponent();
            CanResize = false;

            AnimInfo = bxlanPai;
            ParentLayout = parentLayout;

            typeCB.LoadEnum(typeof(AnimationTarget));
            typeCB.SelectedIndex = 0;
        }

        public BxlanPaiEntry AddEntry()
        {
            return AnimInfo.AddEntry(stTextBox1.Text, (byte)typeCB.SelectedItem);
        }

        private void typeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ParentLayout != null && typeCB.SelectedItem is AnimationTarget)
            {
                objectTargetsCB.Items.Clear();
                if ((AnimationTarget)typeCB.SelectedItem == AnimationTarget.Pane)
                {
                    foreach (var pane in ParentLayout.PaneLookup.Keys)
                        if (!AnimInfo.ContainsEntry(pane))
                            objectTargetsCB.Items.Add(pane);
                }
                else if ((AnimationTarget)typeCB.SelectedItem == AnimationTarget.Material)
                {
                    foreach (var mat in ParentLayout.Materials)
                        if (!AnimInfo.ContainsEntry(mat.Name))
                            objectTargetsCB.Items.Add(mat.Name);
                }

                if (objectTargetsCB.Items.Count > 0)
                    objectTargetsCB.SelectedIndex = 0;
            }
        }

        private void objectTargetsCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            stTextBox1.Text = (string)objectTargetsCB.SelectedItem;
        }
    }
}
