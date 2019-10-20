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

            foreach (var tagDefine in BxlanPaiTag.TypeDefine)
            {
                if (!animGroup.Tags.Any(x => x.Tag == tagDefine.Key ))
                    stComboBox1.Items.Add(tagDefine.Value);
            }

            stComboBox1.SelectedIndex = 0;
        }

        public BxlanPaiTag AddEntry()
        {
            var tagValue = BxlanPaiTag.TypeDefine.FirstOrDefault(x => x.Value == (string)stComboBox1.SelectedItem).Key;
            return ActiveGroup.AddEntry(tagValue);
        }
    }
}
