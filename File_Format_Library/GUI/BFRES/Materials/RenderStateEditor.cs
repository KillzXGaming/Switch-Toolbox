using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syroot.NintenTools.Bfres;
using Syroot.NintenTools.Bfres.GX2;

namespace FirstPlugin.Forms
{
    public partial class RenderStateEditor : UserControl
    {
        public RenderStateEditor()
        {
            InitializeComponent();
        }

        RenderState activeRenderState;

        public void LoadRenderState(RenderState renderState)
        {
            activeRenderState = renderState;

            stPropertyGrid1.LoadProperty(renderState, OnPropertyChanged);
        }

        public void OnPropertyChanged()
        {
            
        }
    }
}
