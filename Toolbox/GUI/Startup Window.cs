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

namespace Toolbox
{
    public partial class Startup_Window : Form
    {
        public Startup_Window()
        {
            InitializeComponent();

            CreateTipList();
            richTextBox1.Text = GetRandomTip();
        }
        List<string> Tips = new List<string>();
        private void CreateTipList()
        {
            Tips.Add("You can view every model and texture in a .bea file by right clicking it in the tree and clicking ''Preview''.");
            Tips.Add("Bfres materials have an option to be copied when right clicked on. Use this to easily transfer new materials!");
            Tips.Add("Most sections in a bfres can be exported and replaced!");
            Tips.Add("For MK8D and Splatoon 2, in the material editor, if the gsys_pass in render info is set to seal on an object ontop of another, you can prevent z fighting!");
        }
        private string GetRandomTip()
        {
            var shuffledTips = Tips.OrderBy(a => Guid.NewGuid()).ToList();
            return shuffledTips[0];
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Runtime.OpenStartupWindow = !checkBox1.Checked;
            Config.Save();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string fileName = listView1.SelectedItems[0].Text;
                Close();
            }
        }
    }
}
