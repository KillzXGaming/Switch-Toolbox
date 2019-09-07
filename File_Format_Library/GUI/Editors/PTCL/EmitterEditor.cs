using System;
using Toolbox.Library.Forms;
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class EmitterEditor : STUserControl
    {
        private EFTB.EMTR activeEmitter;
        private ColorArrayEditor selectedColorArray;
        private ColorConstantPanel selectedColorConstant;

        public EmitterEditor()
        {
            InitializeComponent();
            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
            tabPageData.BackColor = FormThemes.BaseTheme.TabPageActive;
            colorEditor.Visible = false;

            color0ArrayEditor.ColorSelected += ColorArrayEditor_ColorSelected;
            color0ArrayEditor.TimingChanged += ColorArrayEditor_TimingChanged;
            alpha0ArrayEditor.ColorSelected += ColorArrayEditor_ColorSelected;
            alpha0ArrayEditor.TimingChanged += ColorArrayEditor_TimingChanged;
            color1ArrayEditor.ColorSelected += ColorArrayEditor_ColorSelected;
            color1ArrayEditor.TimingChanged += ColorArrayEditor_TimingChanged;
            alpha1ArrayEditor.ColorSelected += ColorArrayEditor_ColorSelected;
            alpha1ArrayEditor.TimingChanged += ColorArrayEditor_TimingChanged;
            scaleArrayEditor.ColorSelected += ColorArrayEditor_ColorSelected;
            scaleArrayEditor.TimingChanged += ColorArrayEditor_TimingChanged;
            constant0Panel.ColorSelected += ColorConstantPanel_ColorSelected;
            constant1Panel.ColorSelected += ColorConstantPanel_ColorSelected;
            colorEditor.ColorChanged += ColorEditor_ColorChanged;
        }

        public void LoadEmitter(EFTB.EMTR emitter)
        {
            colorEditor.Visible = false; // prevent editing unloaded color
            activeEmitter = emitter;

            color0ArrayEditor.LoadColorArray(emitter.Color0Array);
            alpha0ArrayEditor.LoadColorArray(emitter.Color0AlphaArray);
            color1ArrayEditor.LoadColorArray(emitter.Color1Array);
            alpha1ArrayEditor.LoadColorArray(emitter.Color1AlphaArray);
            scaleArrayEditor.LoadColorArray(emitter.ScaleArray);
            constant0Panel.LoadColor(emitter.ConstantColor0);
            constant1Panel.LoadColor(emitter.ConstantColor1);

            // TODO:
            // clamp edited time to surrounding keys
            // select first color on load (or null selectedPanel on load) - prevent from editing previously unloaded stuff
            // edit constant color
        }

        private void ColorArrayEditor_ColorSelected(object sender, EventArgs e)
        {
            selectedColorArray = (ColorArrayEditor)sender;

            // deselect the others
            if (color0ArrayEditor != selectedColorArray) color0ArrayEditor.Deselect();
            if (alpha0ArrayEditor != selectedColorArray) alpha0ArrayEditor.Deselect();
            if (color1ArrayEditor != selectedColorArray) color1ArrayEditor.Deselect();
            if (alpha1ArrayEditor != selectedColorArray) alpha1ArrayEditor.Deselect();
            if (scaleArrayEditor != selectedColorArray) scaleArrayEditor.Deselect();
            selectedColorConstant = null;
            constant0Panel.DeselectPanel();
            constant1Panel.DeselectPanel();

            // push selected color to color editor
            colorEditor.LoadColor(selectedColorArray.SelectedColor, selectedColorArray.ColorArray.IsAlpha, selectedColorArray.ColorArray.Timed);
            colorEditor.Visible = true;
        }

        private void ColorConstantPanel_ColorSelected(object sender, EventArgs e)
        {
            selectedColorConstant = (ColorConstantPanel)sender;

            // deselect the others
            selectedColorArray = null;
            color0ArrayEditor.Deselect();
            alpha0ArrayEditor.Deselect();
            color1ArrayEditor.Deselect();
            alpha1ArrayEditor.Deselect();
            scaleArrayEditor.Deselect();
            if (constant0Panel != selectedColorConstant) constant0Panel.DeselectPanel();
            if (constant1Panel != selectedColorConstant) constant1Panel.DeselectPanel();

            colorEditor.LoadColor(selectedColorConstant.SelectedColor, false, false);
            colorEditor.Visible = true;
        }

        private void ColorArrayEditor_ColorDeselected(object sender, EventArgs e)
        {
            colorEditor.Visible = false;
        }

        private void ColorEditor_ColorChanged(object sender, STColor e)
        {
            if (selectedColorArray != null)
            {
                selectedColorArray.UpdateSelectedColor();
            }
            else if (selectedColorConstant != null)
            {
                selectedColorConstant.UpdateSelectedColor();
            }
        }

        private void ColorArrayEditor_TimingChanged(object sender, EventArgs e)
        {
            if (sender == selectedColorArray)
            {
                colorEditor.ShowTimeInput(selectedColorArray.ColorArray.Timed);
            }
        }
    }
}
