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
using Toolbox.Library.Forms;
using Toolbox.Library.Animations;

namespace FirstPlugin.Forms
{
    public partial class AnimKeyViewer : STForm
    {
        public AnimKeyViewer()
        {
            InitializeComponent();

            CanResize = false;


            stListView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            stListView1.ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        Animation.KeyGroup acitveGroup;
        public void LoadKeyData(Animation.KeyGroup keyGroup)
        {
            acitveGroup = keyGroup;

            stListView1.View = View.Details;
            stListView1.HeaderStyle = ColumnHeaderStyle.None; 
            foreach (var key in keyGroup.Keys)
            {
                stListView1.Items.Add($"Frame: [{key.Frame}] Value: {key.Value}");
            }
        }

        private void stListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stListView1.SelectedItems.Count > 0)
            {
               var keyFrame = acitveGroup.Keys[stListView1.SelectedIndices[0]];

                stLabel1.Text = $"key Frame {keyFrame.Frame}";
                numericUpDownFloat1.Value = (decimal)keyFrame.Value1;
                numericUpDownFloat2.Value = (decimal)keyFrame.Slope1;
                numericUpDownFloat3.Value = (decimal)keyFrame.Slope2;
                numericUpDownFloat4.Value = (decimal)keyFrame.Delta;
            }
            else
            {
                stLabel1.Text = $"";
                numericUpDownFloat1.Value = 0;
                numericUpDownFloat2.Value = 0;
                numericUpDownFloat3.Value = 0;
                numericUpDownFloat4.Value = 0;
            }

        }

        private void stButton2_Click(object sender, EventArgs e)
        {

        }
    }
}
