using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    public partial class HintHelpDialog : STForm
    {
        public HintHelpDialog()
        {
            InitializeComponent();

            FillTableContents();
        }

        public class HelpSection : TreeNodeCustom { }

        public HelpSection[] Sections = new HelpSection[]
        {
            new HImageEditor(),
        };

        private void FillTableContents()
        {
            treeViewCustom1.Nodes.AddRange(Sections);
        }

        public class HImageEditor : HelpSection
        {
            public HImageEditor()
            {
                Text = "Image Editor";
            }
        }
    }
}
