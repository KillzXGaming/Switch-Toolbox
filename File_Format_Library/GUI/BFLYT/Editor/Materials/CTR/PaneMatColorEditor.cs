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

namespace LayoutBXLYT.CTR
{
    public partial class PaneMatCTRColorEditor : EditorPanelBase
    {
        private PaneEditor ParentEditor;
        private Material ActiveMaterial;

        public PaneMatCTRColorEditor()
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
            tevColor1PB.Color = material.TevConstantColors[0].Color;
            tevColor2PB.Color = material.TevConstantColors[1].Color;
            tevColor3PB.Color = material.TevConstantColors[2].Color;
            tevColor4PB.Color = material.TevConstantColors[3].Color;

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
                    var mat = pane.TryGetActiveMaterial() as CTR.Material;
                    if (mat != null)
                        ApplyColors(mat, sender);
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
            else if (sender == tevColor1PB)
                mat.TevConstantColors[0].Color = colorDlg.NewColor;
            else if (sender == tevColor2PB)
                mat.TevConstantColors[1].Color = colorDlg.NewColor;
            else if (sender == tevColor3PB)
                mat.TevConstantColors[2].Color = colorDlg.NewColor;
            else if (sender == tevColor4PB)
                mat.TevConstantColors[3].Color = colorDlg.NewColor;
        }
    }
}
