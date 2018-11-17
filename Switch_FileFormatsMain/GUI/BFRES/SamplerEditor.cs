using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Bfres.Structs;

namespace FirstPlugin
{
    public partial class SamplerEditor : Form
    {
        public SamplerEditor()
        {
            InitializeComponent();

            foreach (var type in Enum.GetValues(typeof(TextureWrapMode)).Cast<TextureWrapMode>())
            {
                wrapXCB.Items.Add(type);
                wrapYCB.Items.Add(type);
                wrapWCB.Items.Add(type);
            }
        }
        public void LoadSampler(MatTexture texture)
        {
            wrapXCB.SelectedItem = MatTexture.wrapmode[texture.wrapModeS];
            wrapYCB.SelectedItem = MatTexture.wrapmode[texture.wrapModeT];
            wrapWCB.SelectedItem = MatTexture.wrapmode[texture.wrapModeW];
        }
    }
}
