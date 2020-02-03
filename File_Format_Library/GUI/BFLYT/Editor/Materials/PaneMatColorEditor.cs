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

namespace LayoutBXLYT
{
    public partial class PaneMatColorEditor : EditorPanelBase
    {
        private PaneEditor ParentEditor;
        private BxlytMaterial ActiveMaterial;

        public PaneMatColorEditor()
        {
            InitializeComponent();

            whiteColorPB.DisplayAlphaSolid = true;
            blackColorBP.DisplayAlphaSolid = true;
        }

        public void LoadMaterial(BxlytMaterial material, PaneEditor paneEditor)
        {
            ActiveMaterial = material;
            ParentEditor = paneEditor;

            whiteColorPB.Color = material.WhiteColor.Color;
            blackColorBP.Color = material.BlackColor.Color;

            chkAlphaInterpolation.Bind(material, "AlphaInterpolation");
        }

        private STColorDialog colorDlg;
        private bool dialogActive = false;
        private void whiteColorPB_Click(object sender, EventArgs e)
        {
            if (dialogActive)
            {
                colorDlg.Focus();
                return;
            }

            dialogActive = true;
            colorDlg = new STColorDialog(whiteColorPB.Color);
            colorDlg.FormClosed += delegate
            {
                dialogActive = false;
            };
            colorDlg.ColorChanged += delegate
            {
                whiteColorPB.Color = colorDlg.NewColor;
                ActiveMaterial.WhiteColor.Color = colorDlg.NewColor;

                //Apply to all selected panes
                foreach (BasePane pane in ParentEditor.SelectedPanes)
                {
                    var mat = pane.TryGetActiveMaterial();
                    if (mat != null)
                        mat.WhiteColor.Color = colorDlg.NewColor;
                }

                ParentEditor.PropertyChanged?.Invoke(sender, e);
            };
            colorDlg.Show();
        }

        private void blackColorBP_Click(object sender, EventArgs e)
        {
            if (dialogActive)
            {
                colorDlg.Focus();
                return;
            }

            dialogActive = true;
            colorDlg = new STColorDialog(blackColorBP.Color);
            colorDlg.FormClosed += delegate
            {
                dialogActive = false;
            };
            colorDlg.ColorChanged += delegate
            {
                blackColorBP.Color = colorDlg.NewColor;
                ActiveMaterial.BlackColor.Color = colorDlg.NewColor;

                //Apply to all selected panes
                foreach (BasePane pane in ParentEditor.SelectedPanes)
                {
                    var mat = pane.TryGetActiveMaterial();
                    if (mat != null)
                        mat.BlackColor.Color = colorDlg.NewColor;
                }

                ParentEditor.PropertyChanged?.Invoke(sender, e);
            };
            colorDlg.Show();
        }
    }
}
