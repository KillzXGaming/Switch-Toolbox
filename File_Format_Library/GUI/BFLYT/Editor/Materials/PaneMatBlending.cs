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
    public partial class PaneMatBlending : EditorPanelBase
    {
        private bool loaded = false;
        private PaneEditor ParentEditor;
        private BxlytMaterial ActiveMaterial;

        public PaneMatBlending()
        {
            InitializeComponent();

            presetsCB.Items.Add("Default (Translucent)");
            presetsCB.SelectedIndex = 0;

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();
            stDropDownPanel3.ResetColors();
        }

        public void LoadMaterial(BxlytMaterial material, PaneEditor paneEditor)
        {
            loaded = false;
            ActiveMaterial = material;
            ParentEditor = paneEditor;

            chkAlphaDefaults.Checked = !material.EnableAlphaCompare;
            chkColorBlendDefaults.Checked = !material.EnableBlend;

            SetupDefaults();
            ReloadBlendValues(material);

            loaded = true;
        }

        private void ReloadBlendValues(BxlytMaterial material)
        {
            ReloadAlphaCompareValues(material);
            ReloadColorValues(material);
        }

        private void ReloadAlphaCompareValues(BxlytMaterial material)
        {
            loaded = false;
            if (material.AlphaCompare != null && material.EnableAlphaCompare)
            {
                alphaSourcCB.Bind(typeof(GfxAlphaFunction), material.AlphaCompare, "CompareMode");
                alphaValueUD.Value = material.AlphaCompare.Value;

                alphaSourcCB.SelectedItem = material.AlphaCompare.CompareMode;
            }
            loaded = true;
        }

        private void ReloadColorValues(BxlytMaterial material)
        {
            loaded = false;
            if (material.BlendMode != null && material.EnableBlend)
            {
                colorBlendSrcCB.Bind(typeof(BxlytBlendMode.GX2BlendFactor), material.BlendMode, "SourceFactor");
                colorBlendDestCB.Bind(typeof(BxlytBlendMode.GX2BlendFactor), material.BlendMode, "DestFactor");
                colorBlendOpCB.Bind(typeof(BxlytBlendMode.GX2BlendOp), material.BlendMode, "BlendOp");
                colorBlendLogicCB.Bind(typeof(BxlytBlendMode.GX2LogicOp), material.BlendMode, "LogicOp");

                colorBlendSrcCB.SelectedItem = material.BlendMode.SourceFactor;
                colorBlendDestCB.SelectedItem = material.BlendMode.DestFactor;
                colorBlendOpCB.SelectedItem = material.BlendMode.BlendOp;
                colorBlendLogicCB.SelectedItem = material.BlendMode.LogicOp;
            }
            loaded = true;
        }

        private void SetupDefaults()
        {
            //Setup default values
            alphaSourcCB.ResetBind();
            alphaSourcCB.Items.Add("Always");
            alphaSourcCB.SelectedIndex = 0;
            alphaValueUD.Value = 0;

            colorBlendSrcCB.ResetBind();
            colorBlendSrcCB.Items.Add(BxlytBlendMode.GX2BlendFactor.SourceAlpha);
            colorBlendSrcCB.SelectedIndex = 0;

            colorBlendDestCB.ResetBind();
            colorBlendDestCB.Items.Add(BxlytBlendMode.GX2BlendFactor.SourceInvAlpha);
            colorBlendDestCB.SelectedIndex = 0;

            colorBlendOpCB.ResetBind();
            colorBlendOpCB.Items.Add(BxlytBlendMode.GX2BlendOp.Add);
            colorBlendOpCB.SelectedIndex = 0;

            colorBlendLogicCB.ResetBind();
            colorBlendLogicCB.Items.Add(BxlytBlendMode.GX2LogicOp.Disable);
            colorBlendLogicCB.SelectedIndex = 0;

            alphaSourcCB.SelectedIndex = 0;
        }

        private void chkAlphaDefaults_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAlphaDefaults.Checked)
            {
                alphaComparePanel.Hide();
                stDropDownPanel2.Height -= alphaComparePanel.Height;
            }
            else
            {
                alphaComparePanel.Show();
                stDropDownPanel2.Height += alphaComparePanel.Height;

                if (ActiveMaterial.AlphaCompare == null && loaded)
                    ActiveMaterial.AlphaCompare = new BxlytAlphaCompare();
            }

            ActiveMaterial.EnableAlphaCompare = !chkAlphaDefaults.Checked;

            if (loaded)
            {
                ReloadAlphaCompareValues(ActiveMaterial);
                ParentEditor.PropertyChanged?.Invoke(sender, e);
            }
        }

        private void chkColorBlendDefaults_CheckedChanged(object sender, EventArgs e)
        {
            if (chkColorBlendDefaults.Checked)
            {
                colorBlendPanel.Hide();
                stDropDownPanel3.Height -= colorBlendPanel.Height;
            }
            else
            {
                colorBlendPanel.Show();
                stDropDownPanel3.Height += colorBlendPanel.Height;

                if (ActiveMaterial.BlendMode == null && loaded)
                    ActiveMaterial.BlendMode = new BxlytBlendMode();
            }

            ActiveMaterial.EnableBlend = !chkColorBlendDefaults.Checked;

            if (loaded)
            {
                ReloadColorValues(ActiveMaterial);
                ParentEditor.PropertyChanged?.Invoke(sender, e);
            }
        }

        private void alphaSourcCB_SelectedIndexChanged(object sender, EventArgs e) {
            if (!loaded || ActiveMaterial.AlphaCompare == null || !ActiveMaterial.EnableAlphaCompare)
                return;

            ActiveMaterial.AlphaCompare.CompareMode = (GfxAlphaFunction)alphaSourcCB.SelectedItem;
            ParentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void alphaValueUD_ValueChanged(object sender, EventArgs e) {
            if (!loaded || ActiveMaterial.AlphaCompare == null || !ActiveMaterial.EnableAlphaCompare)
                return;

            ActiveMaterial.AlphaCompare.Value = alphaValueUD.Value;
            ParentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void colorBlend_ValueChanged(object sender, EventArgs e) {
            if (!loaded || ActiveMaterial.BlendMode == null || !ActiveMaterial.EnableBlend)
                return;

            ActiveMaterial.BlendMode.SourceFactor = (BxlytBlendMode.GX2BlendFactor)colorBlendSrcCB.SelectedItem;
            ActiveMaterial.BlendMode.DestFactor = (BxlytBlendMode.GX2BlendFactor)colorBlendDestCB.SelectedItem;
            ActiveMaterial.BlendMode.BlendOp = (BxlytBlendMode.GX2BlendOp)colorBlendOpCB.SelectedItem;
            ActiveMaterial.BlendMode.LogicOp = (BxlytBlendMode.GX2LogicOp)colorBlendLogicCB.SelectedItem;

            ParentEditor.PropertyChanged?.Invoke(sender, e);
        }
    }
}
