using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstPlugin
{
    public partial class TextureOpenEditor : Form
    {
        public TextureOpenEditor()
        {
            InitializeComponent();
        }
        public void LoadTexture(TextureData tex)
        {
            TextureData.BRTI_Texture renderedTex = tex.renderedGLTex;
            bntxEditor1.LoadProperty(tex);
        }
    }
}
