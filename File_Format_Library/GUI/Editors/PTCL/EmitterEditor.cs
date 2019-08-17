using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;

namespace FirstPlugin
{
    public partial class EmitterEditor : STUserControl
    {
        public EmitterEditor()
        {
            InitializeComponent();
            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
            tabPageColors.BackColor = FormThemes.BaseTheme.TabPageActive;
        }

        private void Reset()
        {
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            TBTexture0.Text = "";
            TBTexture1.Text = "";
            TBTexture2.Text = "";
        }

        private Thread Thread;
        PTCL.Emitter ActiveEmitter;

        public void LoadEmitter(PTCL.Emitter Emitter)
        {
            ActiveEmitter = Emitter;

            Reset();

            LoadColors(Emitter);

            if (Emitter.DrawableTex.Count <= 0)
                return;

            if (Emitter.DrawableTex[0].MipCount > 0)
            {
                pictureBox1.Image = Imaging.GetLoadingImage();
                pictureBox1.Image = Emitter.DrawableTex[0].GetBitmap();

                TBTexture0.Text = Emitter.DrawableTex[0].Text;
            }
            if (Emitter.DrawableTex.Count < 2)
                return;

            if (Emitter.DrawableTex[1].MipCount > 0)
            {
                pictureBox2.Image = Imaging.GetLoadingImage();
                pictureBox2.Image = Emitter.DrawableTex[1].GetBitmap();
                TBTexture1.Text = Emitter.DrawableTex[1].Text;
            }
            if (Emitter.DrawableTex.Count < 3)
                return;

            if (Emitter.DrawableTex[2].MipCount > 0)
            {
                pictureBox3.Image = Imaging.GetLoadingImage();
                pictureBox3.Image = Emitter.DrawableTex[2].GetBitmap();
                TBTexture2.Text = Emitter.DrawableTex[2].Text;
            }

            Thread = new Thread((ThreadStart)(() =>
            {
          
            }));

            try
            {

           /*     Thread Thread2 = new Thread((ThreadStart)(() =>
                {
                    pictureBox2.Image = Imaging.GetLoadingImage();
                    pictureBox2.Image = Emitter.DrawableTex[1].GetBitmap();
                }));
                Thread Thread3 = new Thread((ThreadStart)(() =>
                {
                    pictureBox3.Image = Imaging.GetLoadingImage();
                    pictureBox3.Image = Emitter.DrawableTex[2].GetBitmap();
                }));*/

            }
            catch
            {

            }
        }

        bool IsColorsLoaded = false;

        public ColorAlphaBox GetColor(int colorType, int index)
        {
            foreach (Control control in stPanel2.Controls)
            {
                if (control.Name == $"color{colorType}Index{index}")
                    return (ColorAlphaBox)control;
            }
            return null;
        }

        public void RefreshColorBoxes()
        {
            foreach (Control control in stPanel2.Controls)
            {
                if (control is ColorAlphaBox)
                    control.Refresh();
            }
        }

        private void LoadColors(PTCL.Emitter Emitter)
        {
            IsColorsLoaded = false;

            for (int i = 0; i < 8; i++)
            {
                var colrBtn = GetColor(0, i);
                colrBtn.Color = Emitter.Color0Array[i].Color;
            }

            for (int i = 0; i < 8; i++)
            {
                var colrBtn = GetColor(1, i);
                colrBtn.Color = Emitter.Color1Array[i].Color;
            }

            RefreshColorBoxes();

            color0TB.Text = Utils.ColorToHex(Emitter.Color0Array[0].Color);
            color0TB2.Text = Utils.ColorToHex(Emitter.Color0Array[1].Color);
            color0TB3.Text = Utils.ColorToHex(Emitter.Color0Array[2].Color);
            color0TB4.Text = Utils.ColorToHex(Emitter.Color0Array[3].Color);
            color0TB5.Text = Utils.ColorToHex(Emitter.Color0Array[4].Color);
            color0TB6.Text = Utils.ColorToHex(Emitter.Color0Array[5].Color);
            color0TB7.Text = Utils.ColorToHex(Emitter.Color0Array[6].Color);
            color0TB8.Text = Utils.ColorToHex(Emitter.Color0Array[7].Color);
            color1TB.Text = Utils.ColorToHex(Emitter.Color1Array[0].Color);
            color1TB2.Text = Utils.ColorToHex(Emitter.Color1Array[1].Color);
            color1TB3.Text = Utils.ColorToHex(Emitter.Color1Array[2].Color);
            color1TB4.Text = Utils.ColorToHex(Emitter.Color1Array[3].Color);
            color1TB5.Text = Utils.ColorToHex(Emitter.Color1Array[4].Color);
            color1TB6.Text = Utils.ColorToHex(Emitter.Color1Array[5].Color);
            color1TB7.Text = Utils.ColorToHex(Emitter.Color1Array[6].Color);
            color1TB8.Text = Utils.ColorToHex(Emitter.Color1Array[7].Color);

            IsColorsLoaded = true;
        }

        private void ExportImage0(object sender, EventArgs e)
        {
            ActiveEmitter.DrawableTex[0].ExportImage();
        }
        private void ReplaceImage0(object sender, EventArgs e)
        {
            if (ActiveEmitter is PTCL_WiiU.EmitterU)
            {
                var emitter = (PTCL_WiiU.EmitterU)ActiveEmitter;

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Supported Formats|*.dds; *.png;*.tga;*.jpg;*.tiff|" +
                             "Microsoft DDS |*.dds|" +
                             "Portable Network Graphics |*.png|" +
                             "Joint Photographic Experts Group |*.jpg|" +
                             "Bitmap Image |*.bmp|" +
                             "Tagged Image File Format |*.tiff|" +
                             "All files(*.*)|*.*";

                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    ((PTCL_WiiU.TextureInfo)emitter.DrawableTex[0]).Replace(ofd.FileName);
                }
            }
        }

        private void color_Click(object sender, EventArgs e)
        {
            var button = sender as ColorAlphaBox;
            if (button != null)
            {
                char LastChar = button.Name[button.Name.Length - 1];
                int index = int.Parse(LastChar.ToString());

                ColorDialog dialog = new ColorDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Color output;

                    if (button.Name.Contains("color0"))
                        output = SetColor(ActiveEmitter.Color0Array[index].Color, dialog.Color);
                    else
                        output = SetColor(ActiveEmitter.Color1Array[index].Color, dialog.Color);

                    SetEmitterColor(output, index, button.Name.Contains("color0"));

                    button.Color = output;
                    RefreshColorBoxes();
                }
            }
        }

        private void SetEmitterColor(Color color, int index, bool IsColor0)
        {
            if (IsColor0)
                ActiveEmitter.Color0Array[index].Color = color;
            else
                ActiveEmitter.Color1Array[index].Color = color;
        }

        public Color SetColor(Color input, Color output)
        {
            return Color.FromArgb(input.A, output.R, output.G, output.B);
        }

        private void hexTB_TextChanged(object sender, EventArgs e)
        {
            if (!IsColorsLoaded)
                return;

            if (sender is TextBox)
            {
                ((TextBox)sender).MaxLength = 8;

                if (((TextBox)sender).Text.Length != 8)
                    return;


                Color[] colors0 = new Color[8];
                Color[] colors1 = new Color[8];

                ActiveEmitter.Color0Array[0].Color = Utils.HexToColor(color0TB.Text);
                ActiveEmitter.Color0Array[1].Color = Utils.HexToColor(color0TB2.Text);
                ActiveEmitter.Color0Array[2].Color = Utils.HexToColor(color0TB3.Text);
                ActiveEmitter.Color0Array[3].Color = Utils.HexToColor(color0TB4.Text);
                ActiveEmitter.Color0Array[4].Color = Utils.HexToColor(color0TB5.Text);
                ActiveEmitter.Color0Array[5].Color = Utils.HexToColor(color0TB6.Text);
                ActiveEmitter.Color0Array[6].Color = Utils.HexToColor(color0TB7.Text);
                ActiveEmitter.Color0Array[7].Color = Utils.HexToColor(color0TB8.Text);

                ActiveEmitter.Color1Array[0].Color = Utils.HexToColor(color1TB.Text);
                ActiveEmitter.Color1Array[1].Color = Utils.HexToColor(color1TB2.Text);
                ActiveEmitter.Color1Array[2].Color = Utils.HexToColor(color1TB3.Text);
                ActiveEmitter.Color1Array[3].Color = Utils.HexToColor(color1TB4.Text);
                ActiveEmitter.Color1Array[4].Color = Utils.HexToColor(color1TB5.Text);
                ActiveEmitter.Color1Array[5].Color = Utils.HexToColor(color1TB6.Text);
                ActiveEmitter.Color1Array[6].Color = Utils.HexToColor(color1TB7.Text);
                ActiveEmitter.Color1Array[7].Color = Utils.HexToColor(color1TB8.Text);

                LoadColors(ActiveEmitter);
            }
        }
    }
}
