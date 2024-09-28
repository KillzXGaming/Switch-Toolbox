using System;
using Toolbox.Library.Forms;
using Toolbox.Library;
using System.Windows.Forms;
using System.Linq;

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
            while (stTabControl1.TabPages.Count > 1) stTabControl1.TabPages.RemoveAt(1);
            activeEmitter = emitter;

            color0ArrayEditor.LoadColorArray(emitter.Color0Array);
            alpha0ArrayEditor.LoadColorArray(emitter.Color0AlphaArray);
            color1ArrayEditor.LoadColorArray(emitter.Color1Array);
            alpha1ArrayEditor.LoadColorArray(emitter.Color1AlphaArray);
            scaleArrayEditor.LoadColorArray(emitter.ScaleArray);
            constant0Panel.LoadColor(emitter.ConstantColor0);
            constant1Panel.LoadColor(emitter.ConstantColor1);
            sizeUpDown.Value = (decimal)emitter.Radius;
            blinkIntensity0UpDown.Value = (decimal)emitter.BlinkIntensity0;
            blinkDuration0UpDown.Value = (decimal)emitter.BlinkDuration0;
            blinkIntensity1UpDown.Value = (decimal)emitter.BlinkIntensity1;
            blinkDuration1UpDown.Value = (decimal)emitter.BlinkDuration1;

            // look for textures
            var parent = emitter.Parent;
            while (parent != null && !(parent is EFTB)) // emitters can be nested
            {
                parent = parent.Parent;
            }
            var texr = parent?.Nodes?.OfType<EFTB.TEXA>()?.FirstOrDefault()?.Nodes?.OfType<EFTB.TEXR>() ?? Enumerable.Empty<EFTB.TEXR>();
            texture0Editor.LoadTexture(emitter.Samplers[0], texr);
            texture1Editor.LoadTexture(emitter.Samplers[1], texr);
            texture2Editor.LoadTexture(emitter.Samplers[2], texr);

            foreach (var attribute in emitter.Attributes)
            {
                AddTab(attribute);
            }

            var unknownPanel = new FlowLayoutPanel();
            unknownPanel.Dock = DockStyle.Fill;
            unknownPanel.FlowDirection = FlowDirection.TopDown;
            unknownPanel.WrapContents = false;
            unknownPanel.AutoScroll = true;
            unknownPanel.Controls.Add(CreateHexEditor(emitter.Unknown0, (object sender, byte[] data) => emitter.Unknown0 = data));
            unknownPanel.Controls.Add(CreateHexEditor(emitter.Unknown1, (object sender, byte[] data) => emitter.Unknown1 = data));
            unknownPanel.Controls.Add(CreateHexEditor(emitter.Unknown2, (object sender, byte[] data) => emitter.Unknown2 = data));
            unknownPanel.Controls.Add(CreateHexEditor(emitter.Unknown3, (object sender, byte[] data) => emitter.Unknown3 = data));
            unknownPanel.Controls.Add(CreateHexEditor(emitter.Unknown4, (object sender, byte[] data) => emitter.Unknown4 = data));
            unknownPanel.Controls.Add(CreateHexEditor(emitter.Unknown5, (object sender, byte[] data) => emitter.Unknown5 = data));
            var unknownTab = new TabPage("Unknown");
            unknownTab.Controls.Add(unknownPanel);
            stTabControl1.TabPages.Add(unknownTab);

            // TODO:
            // clamp edited time to surrounding keys
        }

        private HexEditor CreateHexEditor(byte[] data, EventHandler<byte[]> saveHandler)
        {
            var editor = new HexEditor();
            editor.Width = 700;
            editor.LoadData(data);
            editor.SaveData += saveHandler;

            return editor;
        }

        private void AddTab(EFTB.NodeBase node)
        {
            var tab = new TabPage(node.Signature);
            if (node is EFTB.CADP)
            {
                tab.Controls.Add(CreateCADPEditor((EFTB.CADP)node));
            }
            else if (node.UnknownData != null && node.UnknownData.Length > 0)
            {
                var hexEditor = new HexEditor();
                hexEditor.Dock = DockStyle.Fill;
                hexEditor.LoadData(node.UnknownData);
                tab.Controls.Add(hexEditor);
            }
            stTabControl1.TabPages.Add(tab);
        }

        private Control CreateCADPEditor(EFTB.CADP cadp)
        {
            var panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.FlowDirection = FlowDirection.TopDown;
            var label = new Label();
            label.Text = "Shape";
            panel.Controls.Add(label);
            foreach (EFTB.CADP.Shape flag in Enum.GetValues(typeof(EFTB.CADP.Shape)))
            {
                var checkbox = new CheckBox();
                checkbox.Text = Enum.GetName(typeof(EFTB.CADP.Shape), flag);
                checkbox.Checked = cadp.ShapeFlag.HasFlag(flag);
                checkbox.CheckedChanged += (object sender, EventArgs e) =>
                {
                    if (checkbox.Checked)
                    {
                        cadp.ShapeFlag |= flag;
                    }
                    else
                    {
                        cadp.ShapeFlag &= ~flag;
                    }
                };
                panel.Controls.Add(checkbox);
            }
            var hexEditor = new HexEditor();
            hexEditor.Width = 700;
            // hexEditor.Dock = DockStyle.Fill;
            hexEditor.LoadData(cadp.UnknownData);
            hexEditor.SaveData += (object sender, byte[] data) => cadp.UnknownData = data;
            panel.Controls.Add(hexEditor);

            return panel;
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

        private void SizeUpDown_ValueChanged(object sender, EventArgs e)
        {
            float value = (float)sizeUpDown.Value;
            if (activeEmitter.Radius != value)
            {
                activeEmitter.Radius = value;
            }
        }

        private void BlinkIntensity0UpDown_ValueChanged(object sender, EventArgs e)
        {
            float value = (float)blinkIntensity0UpDown.Value;
            if (activeEmitter.BlinkIntensity0 != value)
            {
                activeEmitter.BlinkIntensity0 = value;
            }
        }

        private void BlinkDuration0UpDown_ValueChanged(object sender, EventArgs e)
        {
            float value = (float)blinkDuration0UpDown.Value;
            if (activeEmitter.BlinkDuration0 != value)
            {
                activeEmitter.BlinkDuration0 = value;
            }
        }

        private void BlinkIntensity1UpDown_ValueChanged(object sender, EventArgs e)
        {
            float value = (float)blinkIntensity1UpDown.Value;
            if (activeEmitter.BlinkIntensity1 != value)
            {
                activeEmitter.BlinkIntensity1 = value;
            }
        }

        private void BlinkDuration1UpDown_ValueChanged(object sender, EventArgs e)
        {
            float value = (float)blinkDuration1UpDown.Value;
            if (activeEmitter.BlinkDuration1 != value)
            {
                activeEmitter.BlinkDuration1 = value;
            }
        }
    }
}
