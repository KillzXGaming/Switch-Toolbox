using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Toolbox.Library;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class OdysseyCostumeSelector : STForm
    {
        public string SelectedCostumeName = "";

        public List<string> ExcludeFileList = new List<string>(new string[] {
       "Eye","Face", "Head", "HeadTexture","Under","HandL","HandR","HandTexture", "BodyTexture","Hair","2D","Cap","Tail","Ruck",
       "aHakama","Skirt","Shell","PonchoPoncho","PonchoGuitar"
        });

        public OdysseyCostumeSelector(string GamePath)
        {
            InitializeComponent();

            foreach (string dir in Directory.GetFiles($"{GamePath}\\ObjectData", "Mario*"))
            {
                string filename = Path.GetFileNameWithoutExtension(dir);

                listViewCustom1.BeginUpdate();
                bool Exluded = ExcludeFileList.Any(filename.Contains);
                if (Exluded == false)
                {
                    listViewCustom1.Items.Add(new ListViewItem(filename) { Tag = dir });
                }
                listViewCustom1.EndUpdate();
            }
        }

        private void listViewCustom1_DoubleClick(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count <= 0)
                return;

            SelectedCostumeName = (string)listViewCustom1.SelectedItems[0].Tag;
            DialogResult = DialogResult.OK;
        }
    }
}