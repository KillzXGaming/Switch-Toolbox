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
    public partial class ImageProgramSettings : STForm
    {
        public ImageProgramSettings()
        {
            InitializeComponent();

            CanResize = false;
        }

        public void LoadImage(STGenericTexture texture)
        {
            foreach (var format in texture.SupportedFormats)
            {
                textureImageFormatCB.Items.Add(format);
            }

            textureImageFormatCB.SelectedItem = texture.Format;
        }
    }
}
