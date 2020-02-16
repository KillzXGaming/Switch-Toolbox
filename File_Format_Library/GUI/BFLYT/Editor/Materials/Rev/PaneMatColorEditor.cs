using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.Forms;
using LayoutBXLYT.Revolution;

namespace LayoutBXLYT
{
    public partial class PaneMatRevColorEditor : EditorPanelBase
    {
        private PaneEditor ParentEditor;
        private Material ActiveMaterial;

        public PaneMatRevColorEditor()
        {
            InitializeComponent();

            whiteColorPB.DisplayAlphaSolid = true;
            blackColorBP.DisplayAlphaSolid = true;
        }

        public void LoadMaterial(Material material, PaneEditor paneEditor)
        {
            ActiveMaterial = material;
            ParentEditor = paneEditor;

            whiteColorPB.Color = material.WhiteColor.Color;
            blackColorBP.Color = material.BlackColor.Color;
            materialColorPB.Color = material.MatColor.Color;
            colorReg3PB.Color = material.ColorRegister3.Color;
            tevColor1PB.Color = material.TevColor1.Color;
            tevColor2PB.Color = material.TevColor2.Color;
            tevColor3PB.Color = material.TevColor3.Color;
            tevColor4PB.Color = material.TevColor4.Color;

            chkAlphaInterpolation.Bind(material, "AlphaInterpolation");
        }

        private STColorDialog colorDlg;
        private bool dialogActive = false;
        private void ColorPB_Click(object sender, EventArgs e)
        {
            if (dialogActive)
            {
                colorDlg.Focus();
                return;
            }
            dialogActive = true;
            if (sender is ColorAlphaBox)
                colorDlg = new STColorDialog(((ColorAlphaBox)sender).Color);

            colorDlg.FormClosed += delegate
            {
                dialogActive = false;
            };
            colorDlg.ColorChanged += delegate
            {
                ((ColorAlphaBox)sender).Color = colorDlg.NewColor;

                ApplyColors(ActiveMaterial, sender);

                //Apply to all selected panes
                foreach (BasePane pane in ParentEditor.SelectedPanes)
                {
                    var mat = pane.TryGetActiveMaterial() as Revolution.Material;
                    if (mat != null) {
                        ApplyColors(mat, sender);
                    }
                }

                ParentEditor.PropertyChanged?.Invoke(sender, e);
            };
            colorDlg.Show();
        }

        private void ApplyColors(Material mat, object sender)
        {
            if (sender == whiteColorPB)
                mat.WhiteColor.Color = colorDlg.NewColor;
            else if (sender == blackColorBP)
                mat.BlackColor.Color = colorDlg.NewColor;
            else if (sender == materialColorPB)
                mat.MatColor.Color = colorDlg.NewColor;
            else if (sender == colorReg3PB)
                mat.ColorRegister3.Color = colorDlg.NewColor;
            else if (sender == tevColor1PB)
                mat.TevColor1.Color = colorDlg.NewColor;
            else if (sender == tevColor2PB)
                mat.TevColor2.Color = colorDlg.NewColor;
            else if (sender == tevColor3PB)
                mat.TevColor3.Color = colorDlg.NewColor;
            else if (sender == tevColor4PB)
                mat.TevColor4.Color = colorDlg.NewColor;

            if (!mat.HasMaterialColor && mat.MatColor != STColor8.White) {
                mat.HasMaterialColor = true;
            }

        }
    }
}
