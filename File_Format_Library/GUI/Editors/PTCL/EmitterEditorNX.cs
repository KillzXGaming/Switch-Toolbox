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
using FirstPlugin.Forms;

namespace FirstPlugin
{
    public partial class EmitterEditorNX : STUserControl
    {
        public EmitterEditorNX()
        {
            InitializeComponent();
            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
            tabPageData.BackColor = FormThemes.BaseTheme.TabPageActive;

            foreach (PTCL.Emitter.ColorType format in (PTCL.Emitter.ColorType[])Enum.GetValues(typeof(PTCL.Emitter.ColorType)))
            {
                color0TypeCB.Items.Add(format);
                color1TypeCB.Items.Add(format);
                alpha0TypeCB.Items.Add(format);
                alpha1TypeCB.Items.Add(format);
            }

            color0TypeCB.SetAsReadOnly();
            color1TypeCB.SetAsReadOnly();
            alpha0TypeCB.SetAsReadOnly();
            alpha1TypeCB.SetAsReadOnly();
        }

        private Thread Thread;
        PTCL.Emitter ActiveEmitter;

        private IColorPanelCommon ActivePanel;

        private void LoadColors(STColor[] colors, PTCL.Emitter.ColorType colorType, int type)
        {
            STPanel panel = new STPanel();
            if (type == 0)
                panel = stPanel3;
            if (type == 1)
                panel = stPanel4;
            if (type == 2)
                panel = stPanel5;
            if (type == 3)
                panel = stPanel6;

            panel.Controls.Clear();

            if (colorType == PTCL.Emitter.ColorType.Animated8Key)
            {
                Color8KeySlider colorSlider = new Color8KeySlider();
                colorSlider.Dock = DockStyle.Fill;
                colorSlider.ColorSelected += ColorPanelSelected;
                panel.Controls.Add(colorSlider);
                colorSlider.IsAlpha = (type == 2 || type == 3);

                if (type == 0)
                    colorSlider.LoadColors(colors, (int)ActiveEmitter.Color0KeyCount);
                if (type == 1)
                    colorSlider.LoadColors(colors, (int)ActiveEmitter.Color1KeyCount);
                if (type == 2)
                    colorSlider.LoadColors(colors, (int)ActiveEmitter.Alpha0KeyCount);
                if (type == 3)
                    colorSlider.LoadColors(colors, (int)ActiveEmitter.Alpha1KeyCount);
            }
            else if (colorType == PTCL.Emitter.ColorType.Random)
            {
                ColorRandomPanel colorRandomPnl = new ColorRandomPanel();
                colorRandomPnl.ColorSelected += ColorPanelSelected;
                panel.Controls.Add(colorRandomPnl);
                colorRandomPnl.IsAlpha = (type == 2 || type == 3);

                colorRandomPnl.LoadColors(colors);
            }
            else
            {
                ColorConstantPanel colorConstantPnl = new ColorConstantPanel();
                colorConstantPnl.ColorSelected += ColorPanelSelected;
                panel.Controls.Add(colorConstantPnl);
                colorConstantPnl.IsAlpha = (type == 2 || type == 3);

                if (type == 0)
                    colorConstantPnl.LoadColor(ActiveEmitter.ConstantColor0);
                if (type == 1)
                    colorConstantPnl.LoadColor(ActiveEmitter.ConstantColor1);
                if (type == 2)
                    colorConstantPnl.LoadColor(ActiveEmitter.ConstantAlpha0);
                if (type == 3)
                    colorConstantPnl.LoadColor(ActiveEmitter.ConstantAlpha1);
            }
        }

        private void ColorPanelSelected(object sender, EventArgs e)
        {
            var panel = sender as IColorPanelCommon;
            if (panel != null)
            {
                hexTB.Text = "";

                ActivePanel = panel;
                if (ActivePanel.IsAlpha)
                {
                    int alpha = panel.GetColor().R;

                    colorSelector1.DisplayColor = false;
                    colorSelector1.DisplayAlpha = true;
                    colorSelector1.Alpha = alpha;
                    UpdateColorSelector(Color.FromArgb(alpha, alpha, alpha));
                }
                else
                {
                    colorSelector1.DisplayColor = true;
                    colorSelector1.DisplayAlpha = false;
                    UpdateColorSelector(panel.GetColor());
                }


                if (panel is Color8KeySlider)
                    UpdateTimeDisplay(((Color8KeySlider)panel).GetTime());
            }
        }

        private void UpdateTimeDisplay(float time)
        {
            timeTB.Text = time.ToString();
        }

        public void LoadEmitter(PTCL.Emitter Emitter)
        {
            IsColorsLoaded = false;

            ActiveEmitter = Emitter;

            color0TypeCB.SelectedItem = Emitter.Color0Type;
            color1TypeCB.SelectedItem = Emitter.Color1Type;
            alpha0TypeCB.SelectedItem = Emitter.Alpha0Type;
            alpha1TypeCB.SelectedItem = Emitter.Alpha1Type;

            LoadColors(Emitter.Color0Array, Emitter.Color0Type, 0);
            LoadColors(Emitter.Color1Array, Emitter.Color1Type, 1);
            LoadColors(Emitter.Color0AlphaArray, Emitter.Alpha0Type, 2);
            LoadColors(Emitter.Color1AlphaArray, Emitter.Alpha1Type, 3);

            stLabel1.Text = $"Color 0 ({Emitter.Color0KeyCount} Keys)";
            stLabel2.Text = $"Color 1 ({Emitter.Color1KeyCount} Keys)";
            stLabel3.Text = $"Alpha 0 ({Emitter.Alpha0KeyCount} Keys)";
            stLabel4.Text = $"Alpha 1 ({Emitter.Alpha1KeyCount} Keys)";

            UpdateColorSelector(Color.Black);

            IsColorsLoaded = true;

            Console.WriteLine("DrawableTex " + Emitter.DrawableTex.Count);

            if (Emitter.DrawableTex.Count <= 0)
                return;

            emitterTexturePanel1.LoadTextures(Emitter);
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
            if (sender is TextBox)
            {
                ((TextBox)sender).MaxLength = 8;

                if (((TextBox)sender).Text.Length != 8)
                    return;

                var color = Utils.HexToColor(((TextBox)sender).Text);
                UpdateColorSelector(color);
            }
        }

        private bool _UpdateSelector = true;
        private void colorSelector1_ColorChanged(object sender, EventArgs e)
        {
            if (!IsColorsLoaded)
                return;

            _UpdateSelector = false;

            hexTB.Text = Utils.ColorToHex(colorSelector1.Color);
            pictureBox4.BackColor = colorSelector1.Color;

            if (ActivePanel != null)
                ActivePanel.SetColor(colorSelector1.Color);

            _UpdateSelector = true;
        }

        private void UpdateColorSelector(Color color) {
            if (_UpdateSelector)
                colorSelector1.Color = color;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
    }
}
