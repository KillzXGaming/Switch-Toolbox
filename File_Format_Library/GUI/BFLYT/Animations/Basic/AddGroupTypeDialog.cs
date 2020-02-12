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
    public partial class AddGroupTypeDialog : STForm
    {
        private BxlanPaiEntry ActiveGroup;

        public AddGroupTypeDialog(BxlanPaiEntry animGroup)
        {
            InitializeComponent();
            CanResize = false;
            ActiveGroup = animGroup;

            if (animGroup.Target == AnimationTarget.Pane)
            {
                stComboBox1.Items.Add("PaneSRT");
                stComboBox1.Items.Add("Visibility");
                stComboBox1.Items.Add("TextureSRT");
                stComboBox1.Items.Add("VertexColor");
            }
            else
            {
                stComboBox1.Items.Add("MaterialColor");
                stComboBox1.Items.Add("TexturePattern");
                stComboBox1.Items.Add("IndTextureSRT");
                stComboBox1.Items.Add("AlphaTest");
                stComboBox1.Items.Add("FontShadow");
                stComboBox1.Items.Add("PerCharacterTransformCurve");
            }

            stComboBox1.SelectedIndex = 0;
        }

        public BxlanPaiTag AddEntry()
        {
            string tagValue = "";

            if (ActiveGroup is BFLAN.PaiEntry)
                tagValue = BxlanPaiTag.CafeTypeDefine.FirstOrDefault(x => x.Value == (string)stComboBox1.SelectedItem).Key;
            if (ActiveGroup is BRLAN.PaiEntry)
                tagValue = BxlanPaiTag.RevTypeDefine.FirstOrDefault(x => x.Value == (string)stComboBox1.SelectedItem).Key;
            if (ActiveGroup is BCLAN.PaiEntry)
                tagValue = BxlanPaiTag.CtrTypeDefine.FirstOrDefault(x => x.Value == (string)stComboBox1.SelectedItem).Key;

            return ActiveGroup.AddEntry(tagValue);
        }
    }
}
