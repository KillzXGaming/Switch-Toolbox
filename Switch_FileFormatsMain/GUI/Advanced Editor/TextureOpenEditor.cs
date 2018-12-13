using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;

namespace FirstPlugin
{
    public partial class TextureOpenEditor : Form
    {
        public TextureOpenEditor()
        {
            InitializeComponent();
        }
        public void LoadTexture(STGenericTexture tex)
        {
            RenderableTex renderedTex = tex.RenderableTex;
        }
    }
}
