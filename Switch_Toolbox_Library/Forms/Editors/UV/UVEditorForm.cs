using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class UVEditorForm : STForm
    {
        public UVEditorForm()
        {
            InitializeComponent();
        }

        public void LoadEditor(List<DrawableContainer> Drawables)
        {
            if (Drawables == null) return;

            uvEditor1.Materials.Clear();
            uvEditor1.Textures.Clear();
            uvEditor1.Objects.Clear();
            uvEditor1.Containers.Clear();

            uvEditor1.Containers.AddRange(Drawables);
            uvEditor1.ResetContainerList();
        }
    }
}
