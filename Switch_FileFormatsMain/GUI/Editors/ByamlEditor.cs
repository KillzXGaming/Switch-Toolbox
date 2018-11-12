using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ByamlExt;
using ByamlExt.Byaml;
using System.Xml;

namespace FirstPlugin
{
    public partial class ByamlEditor : DockContent
    {
        public ByamlEditor()
        {
            InitializeComponent();
        }
        public void LoadByaml(BymlFileData byamlFile)
        {

        }
    }
}
