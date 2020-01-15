using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SPICA.PICA.Commands;
using SPICA.Formats.CtrH3D.Model.Material;
using Toolbox.Library;

namespace FirstPlugin.CtrLibrary.Forms
{
    public partial class BCHMaterialBlendingEditor : UserControl, IMaterialLoader
    {
        private H3DMaterialWrapper ActiveWrapper;
        private bool IsLoaded = false;

        public BCHMaterialBlendingEditor()
        {
            InitializeComponent();

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();
            stDropDownPanel3.ResetColors();
            stDropDownPanel4.ResetColors();



            blendModeCB.LoadEnum(typeof(PICABlendMode));

            alphaCompareCB.LoadEnum(typeof(PICATestFunc));
            depthCompareCB.LoadEnum(typeof(PICATestFunc));

            colorEquatCB.LoadEnum(typeof(PICATestFunc));
            colorSourceCB.LoadEnum(typeof(PICATestFunc));
            colorDestCB.LoadEnum(typeof(PICATestFunc));

            alphaEquatCB.LoadEnum(typeof(PICATestFunc));
            alphaSourceCB.LoadEnum(typeof(PICATestFunc));
            alphaDestCB.LoadEnum(typeof(PICATestFunc));

            presetCB.Items.Add("Translucent");
            presetCB.Items.Add("Opaque");
            presetCB.Items.Add("Translucent (Blended)");
            presetCB.Items.Add("Translucent (Additive)");
            presetCB.Items.Add("Translucent (Subtractive)");
            presetCB.Items.Add("Translucent (Multiplicative)");

            renderLayerCB.Items.Add("Layer 0 (Opaque)");
            renderLayerCB.Items.Add("Layer 1 (Translucent)");
            renderLayerCB.Items.Add("Layer 2 (Subtractive Blending)");
            renderLayerCB.Items.Add("Layer 3 (Additive Blending)");
        }

        public void LoadMaterial(H3DMaterialWrapper wrapper)
        {
            ActiveWrapper = wrapper;
            IsLoaded = false;

            var matParams = wrapper.Material.MaterialParams;
            var blend = matParams.BlendFunction;

            foreach (var mesh in wrapper.FindMappedMeshes())
                if (mesh is H3DMeshWrapper)
                    renderLayerCB.SelectedIndex = ((H3DMeshWrapper)mesh).Layer;

            blendModeCB.SelectedItem = matParams.ColorOperation.BlendMode;

            //Load alpha test
            chkEnableAlpha.Checked = matParams.AlphaTest.Enabled;
            alphaValueUD.Value = matParams.AlphaTest.Reference / 255f;
            alphaCompareCB.SelectedItem = matParams.AlphaTest.Function;

            //Load depth test
            chkEnableDepth.Checked = matParams.DepthColorMask.Enabled;
            polyOffsetUD.Value = matParams.PolygonOffsetUnit;
            depthCompareCB.SelectedItem = matParams.DepthColorMask.DepthFunc;
            chkDepthWrite.Checked = matParams.DepthColorMask.DepthWrite;
            chkUsePolyOffset.Checked = matParams.Flags.HasFlag(H3DMaterialFlags.IsPolygonOffsetEnabled);

            //Load blend params
            colorEquatCB.SelectedItem = blend.ColorEquation;
            colorSourceCB.SelectedItem = blend.ColorSrcFunc;
            colorDestCB.SelectedItem = blend.ColorDstFunc;
            alphaEquatCB.SelectedItem = blend.AlphaEquation;
            alphaSourceCB.SelectedItem = blend.AlphaSrcFunc;
            alphaDestCB.SelectedItem = blend.AlphaDstFunc;

            IsLoaded = true;
        }

        private void EditBlendData(object sender, EventArgs e)
        {
            if (!IsLoaded) return;

            var matParams = ActiveWrapper.Material.MaterialParams;
            var blend = matParams.BlendFunction;

            //Edit alpha test
             matParams.AlphaTest.Enabled = chkEnableAlpha.Checked;
            matParams.AlphaTest.Reference = (byte)(alphaValueUD.Value / 255);
            matParams.AlphaTest.Function = (PICATestFunc)alphaCompareCB.SelectedItem;

            //Edit depth test
            matParams.DepthColorMask.Enabled = chkEnableDepth.Checked;
            matParams.PolygonOffsetUnit = polyOffsetUD.Value;
            matParams.DepthColorMask.DepthFunc = (PICATestFunc)depthCompareCB.SelectedItem;
            matParams.DepthColorMask.DepthWrite = chkDepthWrite.Checked;
            if (chkUsePolyOffset.Checked)
                matParams.Flags |= H3DMaterialFlags.IsPolygonOffsetEnabled;
            else
                matParams.Flags &= H3DMaterialFlags.IsPolygonOffsetEnabled;

            //Edit blend params
            blend.ColorEquation = (PICABlendEquation)colorEquatCB.SelectedItem;
            blend.ColorSrcFunc = (PICABlendFunc)colorSourceCB.SelectedItem;
            blend.ColorDstFunc = (PICABlendFunc)colorDestCB.SelectedItem;

            blend.AlphaEquation = (PICABlendEquation)alphaEquatCB.SelectedItem;
            blend.AlphaSrcFunc = (PICABlendFunc)alphaSourceCB.SelectedItem;
            blend.AlphaDstFunc = (PICABlendFunc)alphaDestCB.SelectedItem;

            ActiveWrapper.UpdateViewport();
        }

        private void EditLayer(int layer)
        {

        }
    }
}
