using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LayoutBXLYT
{
    public partial class PaneTextBoxEditor : EditorPanelBase
    {
        private bool loaded = false;
        private PaneEditor parentEditor;
        private ITextPane activePane;

        public PaneTextBoxEditor()
        {
            InitializeComponent();

            sliderItalicTilt.Minimum = -100;
            sliderItalicTilt.Maximum = 100;
            sliderShadowItalicTilt.Minimum = -100;
            sliderShadowItalicTilt.Maximum = 100;

            vertexColorTopBottomBox1.OnColorChanged += OnFontColor_ValueChanged;
            shadowColorBox.OnColorChanged += OnShadowColor_ValueChanged;

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();
            stDropDownPanel4.ResetColors();
            stDropDownPanel4.Visible = true;
        }

        public void LoadPane(ITextPane pane, PaneEditor paneEditor)
        {
            loaded = false;
            parentEditor = paneEditor;
            activePane = pane;

            fontFileCB.Items.Clear();
            var fonts = pane.GetFonts;
            if (fonts != null)
            {
                foreach (var font in fonts)
                    fontFileCB.Items.Add(font);
            }

            if (pane.FontName != null)
            {
                if (fontFileCB.Items.Contains(pane.FontName))
                    fontFileCB.SelectedItem = pane.FontName;
            }


            textBoxTB.Bind(pane, "TextBoxName");
            scaleXUD.Value = pane.FontSize.X;
            scaleYUD.Value = pane.FontSize.Y;

            italicTiltUD.Value = pane.ItalicTilt;
            sliderItalicTilt.Value = (int)italicTiltUD.Value;
            italicTiltUD.Maximum = sliderItalicTilt.Maximum;

            vertexColorTopBottomBox1.TopColor = activePane.FontTopColor.Color;
            vertexColorTopBottomBox1.BottomColor = activePane.FontBottomColor.Color;

            stTextBox1.Text = pane.Text;

            spacingXTB.Value = pane.CharacterSpace;
            spacingYTB.Value = pane.LineSpace;

            alighmentHCB.Bind(typeof(OriginX), pane, "HorizontalAlignment");
            alighmentVCB.Bind(typeof(OriginY), pane, "VerticalAlignment");
            alighmentLineCB.Bind(typeof(LineAlign), pane, "LineAlignment");
            chkEnableShadows.Bind(pane, "ShadowEnabled");
            chkSizeRestrict.Bind(pane, "RestrictedTextLengthEnabled");

            if (pane.RestrictedTextLengthEnabled)
                sizeRestrictUD.Value = (int)pane.RestrictedLength;
            else
                sizeRestrictUD.Value = 0;

            //BRLYT has no shader parameters. Text cannot do those so hide them
            if (pane is Revolution.TXT1 || pane is CTR.TXT1)
            {
                stDropDownPanel4.Visible = false;
            }
            else
            {
                shadowColorBox.BottomColor = pane.ShadowBackColor.Color;
                shadowColorBox.TopColor = pane.ShadowForeColor.Color;
                shadowItalicTiltUD.Maximum = sliderShadowItalicTilt.Maximum;
                shadowItalicTiltUD.Value = pane.ShadowItalic;
                sliderShadowItalicTilt.Value = (int)shadowItalicTiltUD.Value;


                shadowOffseXUD.Value = pane.ShadowXY.X;
                shadowOffseYUD.Value = pane.ShadowXY.Y;
                shadowScaleXUD.Value = pane.ShadowXYSize.X;
                shadowScaleYUD.Value = pane.ShadowXYSize.Y;
            }

            loaded = true;
        }

        private void OnFontColor_ValueChanged(object sender, EventArgs e)
        {
            if (!loaded) return;

            activePane.FontTopColor.Color = vertexColorTopBottomBox1.TopColor;
            activePane.FontBottomColor.Color = vertexColorTopBottomBox1.BottomColor;
        }

        private void OnShadowColor_ValueChanged(object sender, EventArgs e)
        {
            if (!loaded) return;

            activePane.ShadowForeColor.Color = shadowColorBox.TopColor;
            activePane.ShadowBackColor.Color = shadowColorBox.BottomColor;
        }

        private bool changing = false;
        private void italicTiltUD_ValueChanged(object sender, EventArgs e)
        {
            if (changing || !loaded) return;

            if (italicTiltUD.Value > sliderItalicTilt.Maximum)
                return;

            sliderItalicTilt.Value = (int)italicTiltUD.Value;
            activePane.ItalicTilt = (float)italicTiltUD.Value;
        }

        private void sliderItalicTilt_ValueChanged(object sender, EventArgs e) {
            if (!loaded) return;

            changing = true;
            italicTiltUD.Value = sliderItalicTilt.Value;
            activePane.ItalicTilt = (float)italicTiltUD.Value;
            parentEditor.PropertyChanged?.Invoke(sender, e);
            changing = false;
        }

        private void stTextBox1_TextChanged(object sender, EventArgs e) {
            activePane.Text = stTextBox1.Text;
            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void scaleUD_ValueChanged(object sender, EventArgs e) {
            if (!loaded) return;

            activePane.FontSize = new Syroot.Maths.Vector2F(
                scaleXUD.Value, scaleYUD.Value);

            activePane.CharacterSpace = spacingXTB.Value;
            activePane.LineSpace = spacingYTB.Value;

            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void scaleXUD_ValueChanged(object sender, EventArgs e)
        {

        }

        private void scaleXUD_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void vertexColorTopBottomBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void shadowItalicTiltUD_ValueChanged(object sender, EventArgs e) {
            if (changing || !loaded) return;
            sliderShadowItalicTilt.Value = (int)shadowItalicTiltUD.Value;
        }

        private void sliderItalicTild_ValueChanged(object sender, EventArgs e) {
            if (!loaded) return;

            changing = true;
            shadowItalicTiltUD.Value = sliderShadowItalicTilt.Value;
            parentEditor.PropertyChanged?.Invoke(sender, e);
            changing = false;
        }

        private void shadowTransform_ValueChanged(object sender, EventArgs e) {
            if (!loaded) return;

            activePane.ShadowXY = new Syroot.Maths.Vector2F(
                shadowOffseXUD.Value, shadowOffseYUD.Value);

            activePane.ShadowXYSize = new Syroot.Maths.Vector2F(
              shadowScaleXUD.Value, shadowScaleYUD.Value);

            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void chkSizeRestrict_CheckedChanged(object sender, EventArgs e) {
            if (chkSizeRestrict.Checked)
                sizeRestrictUD.Enabled = true;
            else
                sizeRestrictUD.Enabled = false;
        }

        private void stLabel7_Click(object sender, EventArgs e)
        {

        }

        private void alighmentH_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loaded) return;

            activePane.HorizontalAlignment = (OriginX)alighmentHCB.SelectedItem;
            activePane.VerticalAlignment = (OriginY)alighmentVCB.SelectedItem;
            activePane.LineAlignment = (LineAlign)alighmentLineCB.SelectedItem;

            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void fontFileCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loaded && fontFileCB.SelectedIndex >= 0) return;

            activePane.FontName = (string)fontFileCB.SelectedItem;
            activePane.FontIndex = (ushort)fontFileCB.SelectedIndex;
            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void sizeRestrictUD_Scroll(object sender, ScrollEventArgs e) {

        }
    }
}
