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

namespace FirstPlugin.Forms
{
    public partial class KeyShapeList : UserControl
    {
        public KeyShapeList()
        {
            InitializeComponent();

            keyShapeListView.HeaderStyle = ColumnHeaderStyle.None;
        }

        public void LoadKeyShapes(ResU.ResDict<ResU.KeyShape> KeyShapes)
        {
            foreach (var keyShape in KeyShapes)
            {
                keyShapeListView.Items.Add($"{keyShape.Key}");
            }
        }

        public void LoadKeyShapes(IList<KeyShape> KeyShapes, ResDict keys)
        {
            keyShapeListView.Items.Clear();

            foreach (var keyShape in keys)
            {
                keyShapeListView.Items.Add($"{keyShape}");
            }
        }
    }
}
