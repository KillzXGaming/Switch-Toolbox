using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
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
            this.Text = "Image Program Settings";
            stButton1.Select();
        }

        public void LoadImage(STGenericTexture texture)
        {
            textureFileFormatCB.Items.Add("Direct Draw Surface (.dds)");
            textureFileFormatCB.Items.Add("Portable Network Graphics (.png)");
            textureFileFormatCB.Items.Add("Joint Photographic Experts Group (.jpg)");
            textureFileFormatCB.Items.Add("TGA (.tga)");
            textureFileFormatCB.Items.Add("Tagged Image File Format (.tiff)");
            textureFileFormatCB.Items.Add("Bitmap Image (.bmp)");
            textureFileFormatCB.SelectedIndex = 0;

            foreach (var format in texture.SupportedFormats)
            {
                textureImageFormatCB.Items.Add(format);
            }

            textureImageFormatCB.SelectedItem = texture.Format;
        }

        public TEX_FORMAT GetSelectedImageFormat()
        {
            return (TEX_FORMAT)textureImageFormatCB.SelectedItem;
        }

        public string GetSelectedExtension()
        {
            string SelectedExt = textureFileFormatCB.GetSelectedText();

            string output = GetSubstringByString("(", ")", SelectedExt);
            output = output.Replace('*', ' ');

            if (output == ".")
                output = ".raw";

            return output;
        }

        public string GetSubstringByString(string a, string b, string c)
        {
            return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
        }

        private void stButton2_Click(object sender, EventArgs e)
        {
            string UseExtension = GetSelectedExtension();

            string TemporaryName = Path.GetTempFileName();
            TemporaryName = Path.ChangeExtension(TemporaryName, UseExtension);

            ShowOpenWithDialog(TemporaryName);
        }

        public static Process ShowOpenWithDialog(string path)
        {
            var args = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
            args += ",OpenAs_RunDLL " + path;
            return Process.Start("rundll32.exe", args);
        }
    }
}
