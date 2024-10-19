using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class ColorArrayEditor : UserControl
    {
        public EFTB.EMTR.STColorArray ColorArray { get; private set; }
        public STColor SelectedColor
        {
            get
            {
                return colorArrayPanel.SelectedColor;
            }
        }

        public event EventHandler ColorSelected;
        public event EventHandler ColorDeselected; // this is only called when a color that was previously selected is explicitly deselected
        public event EventHandler TimingChanged;
        private IColorPanelCommon colorArrayPanel;

        [Category("Appearance"), Description("Name of the color array.")]
        public string Label
        {
            get
            {
                return colorArrayLabel.Text;
            }
            set
            {
                colorArrayLabel.Text = value;
            }
        }

        public ColorArrayEditor()
        {
            InitializeComponent();
        }

        public void LoadColorArray(EFTB.EMTR.STColorArray colorArray)
        {
            this.ColorArray = colorArray;
            InitializeColorArrayPanel();
            keyCountUpDown.Value = colorArray.KeyCount;
            animatedCheckBox.Checked = colorArray.Timed;
        }

        public void UpdateSelectedColor()
        {
            colorArrayPanel.UpdateSelectedColor();
        }

        public void Deselect()
        {
            colorArrayPanel.DeselectPanel();
        }

        private void InitializeColorArrayPanel()
        {
            panel.Controls.Clear();

            if (ColorArray.Timed)
            {
                Color8KeySlider colorSlider = new Color8KeySlider();
                colorSlider.Dock = DockStyle.Fill;
                colorSlider.ColorSelected += ColorPanelSelected;
                colorSlider.IsAlpha = ColorArray.IsAlpha;
                colorSlider.LoadColors(ColorArray.ColorKeys, (int)ColorArray.KeyCount);
                panel.Controls.Add(colorSlider);
                colorArrayPanel = colorSlider;
            }
            else
            {
                ColorRandomPanel colorRandomPnl = new ColorRandomPanel();
                colorRandomPnl.ColorSelected += ColorPanelSelected;
                colorRandomPnl.IsAlpha = ColorArray.IsAlpha;
                colorRandomPnl.LoadColors(ColorArray.ColorKeys, (int)ColorArray.KeyCount);
                panel.Controls.Add(colorRandomPnl);
                colorArrayPanel = colorRandomPnl;
            }
        }

        private void ColorPanelSelected(object sender, EventArgs e)
        {
            ColorSelected?.Invoke(this, e);
        }

        private void AnimatedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ColorArray.Timed = animatedCheckBox.Checked;
            InitializeColorArrayPanel();
            TimingChanged?.Invoke(this, null);
        }

        private void KeyCountUpDown_ValueChanged(object sender, EventArgs e)
        {
            uint value = (uint)keyCountUpDown.Value;
            while (ColorArray.KeyCount < value) ColorArray.AddKey();
            while (ColorArray.KeyCount > value) ColorArray.RemoveKey();
            // TODO raise ColorDeselected event if we lost the selected color
            colorArrayPanel.LoadColors(ColorArray.ColorKeys, (int)ColorArray.KeyCount); // TODO add LoadColors(STColorArray) to the interface
        }
    }
}
