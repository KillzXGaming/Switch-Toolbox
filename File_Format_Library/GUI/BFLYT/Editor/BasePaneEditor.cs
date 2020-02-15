using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;

namespace LayoutBXLYT
{
    public partial class BasePaneEditor : EditorPanelBase
    {
        private bool Loaded = false;
    
        public BasePaneEditor()
        {
            InitializeComponent();

            userNameTB.MaxLength = 8;

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();
            stDropDownPanel3.ResetColors();
            stDropDownPanel4.ResetColors();
            stDropDownPanel5.ResetColors();

            partPaneScalingCB.LoadEnum(typeof(PartPaneScaling));

            alphaSelectorHorizontalPanel1.AlphaChanged += OnAlphaSliderChanged;
            alphaUD.ValueChanged += OnAlphaChanged;

            tranXUD.ValueChanged += OnTransformChanged;
            tranYUD.ValueChanged += OnTransformChanged;
            tranZUD.ValueChanged += OnTransformChanged;
            rotXUD.ValueChanged += OnTransformChanged;
            rotYUD.ValueChanged += OnTransformChanged;
            rotZUD.ValueChanged += OnTransformChanged;
            scaleXUD.ValueChanged += OnTransformChanged;
            scaleYUD.ValueChanged += OnTransformChanged;
            sizeXUD.ValueChanged += OnTransformChanged;
            sizeYUD.ValueChanged += OnTransformChanged;

            radioBottomBtn.CheckedChanged += OnOrientationChanged;
            radioBottomLeftBtn.CheckedChanged += OnOrientationChanged;
            radioBottomRightBtn.CheckedChanged += OnOrientationChanged;
            radioRightBtn.CheckedChanged += OnOrientationChanged;
            radioLeftBtn.CheckedChanged += OnOrientationChanged;
            radioCenterBtn.CheckedChanged += OnOrientationChanged;
            radioTopBtn.CheckedChanged += OnOrientationChanged;
            radioTopLeftBtn.CheckedChanged += OnOrientationChanged;
            radioTopRightBtn.CheckedChanged += OnOrientationChanged;

            radioBottomBtnParent.CheckedChanged += OnParentOrientationChanged;
            radioBottomLeftBtnParent.CheckedChanged += OnParentOrientationChanged;
            radioBottomRightBtnParent.CheckedChanged += OnParentOrientationChanged;
            radioRightBtnParent.CheckedChanged += OnParentOrientationChanged;
            radioLeftBtnParent.CheckedChanged += OnParentOrientationChanged;
            radioCenterBtnParent.CheckedChanged += OnParentOrientationChanged;
            radioTopBtnParent.CheckedChanged += OnParentOrientationChanged;
            radioTopLeftBtnParent.CheckedChanged += OnParentOrientationChanged;
            radioTopRightBtnParent.CheckedChanged += OnParentOrientationChanged;

            alphaUD.Maximum = 255;
            alphaUD.Minimum = 0;
        }

        private BasePane ActivePane;
        private PaneEditor parentEditor;

        public void LoadPane(BasePane pane, PaneEditor paneEditor)
        {
            parentEditor = paneEditor;
            Loaded = false;

            ActivePane = pane;

            SetUIState();

            nameTB.Bind(pane, "Name");
            userNameTB.Bind(pane, "UserDataInfo");
            partPaneScalingCB.SelectedItem = (PartPaneScaling)pane.PaneMagFlags;

            SetTransform();

            alphaChildrenChk.Bind(pane, "InfluenceAlpha");
            paneVisibleChk.Bind(pane, "Visible");

            alphaUD.Value = pane.Alpha;
            alphaSelectorHorizontalPanel1.Alpha = pane.Alpha;
            SetOrientation();
            SetParentOrientation();

            Loaded = true;
        }

        public override void SetUIState()
        {
            if (Runtime.LayoutEditor.AnimationEditMode)
            {
                //Change any UI that can be keyed or is keyed
                tranXUD.SetUnkeyedTheme();
                tranYUD.SetUnkeyedTheme();
                tranZUD.SetUnkeyedTheme();
                rotXUD.SetUnkeyedTheme();
                rotYUD.SetUnkeyedTheme();
                rotZUD.SetUnkeyedTheme();
                scaleXUD.SetUnkeyedTheme();
                scaleYUD.SetUnkeyedTheme();
                sizeXUD.SetUnkeyedTheme();
                sizeYUD.SetUnkeyedTheme();
                alphaUD.SetUnkeyedTheme();

                if (ActivePane.animController.PaneSRT.ContainsKey(LPATarget.TranslateX))
                    tranXUD.SetKeyedTheme();
                if (ActivePane.animController.PaneSRT.ContainsKey(LPATarget.TranslateY))
                    tranYUD.SetKeyedTheme();
                if (ActivePane.animController.PaneSRT.ContainsKey(LPATarget.TranslateZ))
                    tranZUD.SetKeyedTheme();

                if (ActivePane.animController.PaneSRT.ContainsKey(LPATarget.RotateX))
                    rotXUD.SetKeyedTheme();
                if (ActivePane.animController.PaneSRT.ContainsKey(LPATarget.RotateY))
                    rotYUD.SetKeyedTheme();
                if (ActivePane.animController.PaneSRT.ContainsKey(LPATarget.RotateZ))
                    rotZUD.SetKeyedTheme();

                paneVisibleChk.ShowBorder = true;
                paneVisibleChk.BorderColor = Color.Red;
                paneVisibleChk.Refresh();
            }
            else
            {
                paneVisibleChk.ShowBorder = false;
                paneVisibleChk.Refresh();

                //Set UI to default theme colors
                tranXUD.ReloadTheme();
                tranYUD.ReloadTheme();
                tranZUD.ReloadTheme();
                rotXUD.ReloadTheme();
                rotYUD.ReloadTheme();
                rotZUD.ReloadTheme();
                scaleXUD.ReloadTheme();
                scaleYUD.ReloadTheme();
                sizeXUD.ReloadTheme();
                sizeYUD.ReloadTheme();
                alphaUD.ReloadTheme();
            }
        }

        public void RefreshEditor()
        {
            Loaded = false;
            SetTransform();
            Loaded = true;
        }

        private void SetTransform()
        {
            var translate = ActivePane.Translate;
            var rotate = ActivePane.Rotate;
            var scale = ActivePane.Scale;
            var sizeX = ActivePane.Width;
            var sizeY = ActivePane.Height;

            if (Runtime.LayoutEditor.AnimationEditMode)
            {
                translate = ActivePane.GetTranslation();
                rotate = ActivePane.GetRotation();
                scale = ActivePane.GetScale();
                sizeX = ActivePane.GetSize().X;
                sizeY = ActivePane.GetSize().Y;
            }

            tranXUD.Value = translate.X;
            tranYUD.Value = translate.Y;
            tranZUD.Value = translate.Z;
            rotXUD.Value = rotate.X;
            rotYUD.Value = rotate.Y;
            rotZUD.Value = rotate.Z;
            scaleXUD.Value = scale.X;
            scaleYUD.Value = scale.Y;
            sizeXUD.Value = sizeX;
            sizeYUD.Value = sizeY;
        }

        private void OnAlphaSliderChanged(object sender, EventArgs e) {
            if (!Loaded) return;
            alphaUD.Value = alphaSelectorHorizontalPanel1.Alpha;

            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void OnAlphaChanged(object sender, EventArgs e) {
            if (!Loaded) return;
            ActivePane.Alpha = (byte)alphaUD.Value;

            //Apply to all selected panes
            foreach (BasePane pane in parentEditor.SelectedPanes)
                pane.Alpha = (byte)alphaUD.Value;


            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private bool IsRadioOrientationEdited = false;

        private void OnOrientationChanged(object sender, EventArgs e) { 
            if (!Loaded || IsRadioOrientationEdited) return;

            IsRadioOrientationEdited = true;

            if (radioTopRightBtn.Checked) {
                ActivePane.originX = OriginX.Right;
                ActivePane.originY = OriginY.Top;
            }
            else if (radioTopLeftBtn.Checked) {
                ActivePane.originX = OriginX.Left;
                ActivePane.originY = OriginY.Top;
            }
            else if (radioTopBtn.Checked) {
                ActivePane.originX = OriginX.Center;
                ActivePane.originY = OriginY.Top;
            }
            else if (radioBottomBtn.Checked) {
                ActivePane.originX = OriginX.Center;
                ActivePane.originY = OriginY.Bottom;
            }
            else if (radioBottomRightBtn.Checked) {
                ActivePane.originX = OriginX.Right;
                ActivePane.originY = OriginY.Bottom;
            }
            else if (radioBottomLeftBtn.Checked)
            {
                ActivePane.originX = OriginX.Left;
                ActivePane.originY = OriginY.Bottom;
            }
            else if (radioCenterBtn.Checked) {
                ActivePane.originX = OriginX.Center;
                ActivePane.originY = OriginY.Center;
            }
            else if (radioRightBtn.Checked) {
                ActivePane.originX = OriginX.Right;
                ActivePane.originY = OriginY.Center;
            }
            else if (radioLeftBtn.Checked) {
                ActivePane.originX = OriginX.Left;
                ActivePane.originY = OriginY.Center;
            }

            //Apply to all selected panes
            foreach (BasePane pane in parentEditor.SelectedPanes) {
                pane.originX = ActivePane.originX;
                pane.originY = ActivePane.originY;
            }

            SetOrientation();

            IsRadioOrientationEdited = false;

            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void OnParentOrientationChanged(object sender, EventArgs e)
        {
            if (!Loaded || IsRadioOrientationEdited) return;

            IsRadioOrientationEdited = true;

            if (radioTopRightBtnParent.Checked)
            {
                ActivePane.ParentOriginX = OriginX.Right;
                ActivePane.ParentOriginY = OriginY.Top;
            }
            else if (radioTopLeftBtnParent.Checked)
            {
                ActivePane.ParentOriginX = OriginX.Left;
                ActivePane.ParentOriginY = OriginY.Top;
            }
            else if (radioTopBtnParent.Checked)
            {
                ActivePane.ParentOriginX = OriginX.Center;
                ActivePane.ParentOriginY = OriginY.Top;
            }
            else if (radioBottomBtnParent.Checked)
            {
                ActivePane.ParentOriginX = OriginX.Center;
                ActivePane.ParentOriginY = OriginY.Bottom;
            }
            else if (radioBottomRightBtnParent.Checked)
            {
                ActivePane.ParentOriginX = OriginX.Right;
                ActivePane.ParentOriginY = OriginY.Bottom;
            }
            else if (radioBottomLeftBtnParent.Checked)
            {
                ActivePane.ParentOriginX = OriginX.Left;
                ActivePane.ParentOriginY = OriginY.Bottom;
            }
            else if (radioCenterBtnParent.Checked)
            {
                ActivePane.ParentOriginX = OriginX.Center;
                ActivePane.ParentOriginY = OriginY.Center;
            }
            else if (radioRightBtnParent.Checked)
            {
                ActivePane.ParentOriginX = OriginX.Right;
                ActivePane.ParentOriginY = OriginY.Center;
            }
            else if (radioLeftBtnParent.Checked)
            {
                ActivePane.ParentOriginX = OriginX.Left;
                ActivePane.ParentOriginY = OriginY.Center;
            }

            //Apply to all selected panes
            foreach (BasePane pane in parentEditor.SelectedPanes) {
                if (!pane.IsRoot && !pane.ParentIsRoot)
                {
                    pane.ParentOriginX = ActivePane.ParentOriginX;
                    pane.ParentOriginY = ActivePane.ParentOriginY;
                }
            }

            SetParentOrientation();

            IsRadioOrientationEdited = false;

            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void OnTransformChanged(object sender, EventArgs e) {
            if (!Loaded) return;

            if (Runtime.LayoutEditor.AnimationEditMode)
            {

            }
            else
            {
                ActivePane.Translate = new Syroot.Maths.Vector3F(
                tranXUD.Value, tranYUD.Value, tranZUD.Value);

                ActivePane.Rotate = new Syroot.Maths.Vector3F(
                  rotXUD.Value, rotYUD.Value, rotZUD.Value);

                ActivePane.Scale = new Syroot.Maths.Vector2F(
                scaleXUD.Value, scaleYUD.Value);

                ActivePane.Width = sizeXUD.Value;
                ActivePane.Height = sizeYUD.Value;
            }

            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void SetParentOrientation()
        {
            radioCenterBtnParent.Checked = true;
            radioLeftBtnParent.Checked = false;
            radioRightBtnParent.Checked = false;
            radioTopLeftBtnParent.Checked = true;
            radioTopRightBtnParent.Checked = false;
            radioBottomLeftBtnParent.Checked = false;
            radioBottomRightBtnParent.Checked = false;
            radioTopBtnParent.Checked = false;
            radioBottomBtnParent.Checked = false;

            if (ActivePane.IsRoot || ActivePane.ParentIsRoot)
            {
                radioCenterBtnParent.Enabled = false;
                radioLeftBtnParent.Enabled = false;
                radioRightBtnParent.Enabled = false;
                radioTopLeftBtnParent.Enabled = true;
                radioTopRightBtnParent.Enabled = false;
                radioBottomLeftBtnParent.Enabled = false;
                radioBottomRightBtnParent.Enabled = false;
                radioTopBtnParent.Enabled = false;
                radioBottomBtnParent.Enabled = false;
                return;
            }
            else
            {
                radioCenterBtnParent.Enabled = true;
                radioLeftBtnParent.Enabled = true;
                radioRightBtnParent.Enabled = true;
                radioTopLeftBtnParent.Enabled = true;
                radioTopRightBtnParent.Enabled = true;
                radioBottomLeftBtnParent.Enabled = true;
                radioBottomRightBtnParent.Enabled = true;
                radioTopBtnParent.Enabled = true;
                radioBottomBtnParent.Enabled = true;
            }

            switch (ActivePane.ParentOriginX)
            {
                case OriginX.Center:
                    if (ActivePane.ParentOriginY == OriginY.Center)
                        radioCenterBtnParent.Checked = true;
                    else if (ActivePane.ParentOriginY == OriginY.Bottom)
                        radioBottomBtnParent.Checked = true;
                    else if (ActivePane.ParentOriginY == OriginY.Top)
                        radioTopBtnParent.Checked = true;
                    break;
                case OriginX.Left:
                    if (ActivePane.ParentOriginY == OriginY.Center)
                        radioLeftBtnParent.Checked = true;
                    else if (ActivePane.ParentOriginY == OriginY.Bottom)
                        radioBottomLeftBtnParent.Checked = true;
                    else if (ActivePane.ParentOriginY == OriginY.Top)
                        radioTopLeftBtnParent.Checked = true;
                    break;
                case OriginX.Right:
                    if (ActivePane.ParentOriginY == OriginY.Center)
                        radioRightBtnParent.Checked = true;
                    else if (ActivePane.ParentOriginY == OriginY.Bottom)
                        radioBottomRightBtnParent.Checked = true;
                    else if (ActivePane.ParentOriginY == OriginY.Top)
                        radioTopRightBtnParent.Checked = true;
                    break;
            }
        }

        private void SetOrientation()
        {
            radioCenterBtn.Checked = true;
            radioLeftBtn.Checked = false;
            radioRightBtn.Checked = false;
            radioTopLeftBtn.Checked = true;
            radioTopRightBtn.Checked = false;
            radioBottomLeftBtn.Checked = false;
            radioBottomRightBtn.Checked = false;
            radioTopBtn.Checked = false;
            radioBottomBtn.Checked = false;

            switch (ActivePane.originX)
            {
                case OriginX.Center:
                    if (ActivePane.originY == OriginY.Center)
                        radioCenterBtn.Checked = true;
                    else if (ActivePane.originY == OriginY.Bottom)
                        radioBottomBtn.Checked = true;
                    else if (ActivePane.originY == OriginY.Top)
                        radioTopBtn.Checked = true;
                    break;
                case OriginX.Left:
                    if (ActivePane.originY == OriginY.Center)
                        radioLeftBtn.Checked = true;
                    else if (ActivePane.originY == OriginY.Bottom)
                        radioBottomLeftBtn.Checked = true;
                    else if (ActivePane.originY == OriginY.Top)
                        radioTopLeftBtn.Checked = true;
                    break;
                case OriginX.Right:
                    if (ActivePane.originY == OriginY.Center)
                        radioRightBtn.Checked = true;
                    else if (ActivePane.originY == OriginY.Bottom)
                        radioBottomRightBtn.Checked = true;
                    else if (ActivePane.originY == OriginY.Top)
                        radioTopRightBtn.Checked = true;
                    break;
            }
        }

        private void nameTB_TextChanged(object sender, EventArgs e) {
            if (ActivePane.NodeWrapper != null)
                ActivePane.NodeWrapper.Text = nameTB.Text;
            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void paneVisibleChk_CheckedChanged(object sender, EventArgs e) {
            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void BasePaneEditor_Enter(object sender, EventArgs e) {
        }

        private void BasePaneEditor_MouseEnter(object sender, EventArgs e) {
            RefreshEditor();
        }

        private void partPaneScalingCB_SelectedIndexChanged(object sender, EventArgs e) {
            if (!Loaded) return;
            var scalingMode = (PartPaneScaling)partPaneScalingCB.SelectedItem;
            ActivePane.PaneMagFlags = (byte)scalingMode;
        }
    }
}
