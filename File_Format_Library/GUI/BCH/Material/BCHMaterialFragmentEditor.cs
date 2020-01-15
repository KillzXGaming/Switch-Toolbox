using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.PICA.Commands;
using Toolbox.Library;

namespace FirstPlugin.CtrLibrary.Forms
{
    public partial class BCHMaterialFragmentEditor : UserControl, IMaterialLoader
    { 
        private H3DMaterial ActiveMaterial;
        private H3DMaterialWrapper ActiveWrapper;

        public string ActiveLUT => lookupTablesCB.SelectedItem != null ?
            lookupTablesCB.SelectedItem.ToString() : "";

        public BCHMaterialFragmentEditor()
        {
            InitializeComponent();

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();

            layerCB.LoadEnum(typeof(H3DTranslucencyKind));

            dist0InputCB.LoadEnum(typeof(SPICA.PICA.Commands.PICALUTInput));
            dist1InputCB.LoadEnum(typeof(SPICA.PICA.Commands.PICALUTInput));
            reflectionRInputCB.LoadEnum(typeof(SPICA.PICA.Commands.PICALUTInput));
            reflectionGInputCB.LoadEnum(typeof(SPICA.PICA.Commands.PICALUTInput));
            reflectionBInputCB.LoadEnum(typeof(SPICA.PICA.Commands.PICALUTInput));
            fresneInputCB.LoadEnum(typeof(SPICA.PICA.Commands.PICALUTInput));


            colorCombinerCB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerMode));
            colorSource0CB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerSource));
            colorSource1CB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerSource));
            colorSource2CB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerSource));
            colorScaleCB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerScale));
            colorOp0CB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerColorOp));
            colorOp1CB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerColorOp));
            colorOp2CB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerColorOp));

            alphaCombinerCB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerMode));
            alphaSource0CB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerSource));
            alphaSource1CB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerSource));
            alphaSource2CB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerSource));
            alphaScaleCB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerScale));
            alphaOp0CB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerAlphaOp));
            alphaOp1CB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerAlphaOp));
            alphaOp2CB.LoadEnum(typeof(SPICA.PICA.Commands.PICATextureCombinerAlphaOp));
        }

        public void LoadMaterial(H3DMaterialWrapper wrapper) {
            ActiveMaterial = wrapper.Material;
            ActiveWrapper = wrapper;

            layerCB.SelectedItem = ActiveMaterial.MaterialParams.TranslucencyKind;

          //  LoadCombinerImages();
            UpdateTev();
            UpdateTerms();
            UpdateLUT();
        }

        Dictionary<string, Bitmap> CombinerImages = new Dictionary<string, Bitmap>();

        private void LoadCombinerImages()
        {
            foreach (var image in CombinerImages.Values)
                image?.Dispose();

            CombinerImages.Clear();

            Thread Thread = new Thread((ThreadStart)(() =>
            {
                Bitmap image = Properties.Resources.TextureError;
                for (int i = 0; i < ActiveWrapper.TextureMaps?.Count; i++)
                {
                    var texMap = ActiveWrapper.TextureMaps[i];
                    foreach (var bch in PluginRuntime.bchTexContainers)
                    {
                        if (bch.ResourceNodes.ContainsKey(texMap.Name))
                        {
                            var texture = (STGenericTexture)bch.ResourceNodes[texMap.Name];

                            try {
                                image = texture.GetBitmap();
                            }
                            catch {
                                image = Properties.Resources.TextureError;
                            }

                            CombinerImages.Add($"Texture{i}", image);
                        }
                    }
                }
            }));
            Thread.Start();
        }

        private void UpdateTev()
        {
            var matParams = ActiveMaterial.MaterialParams;

            tevStagesCB.Items.Clear();
            for (int i = 0; i < matParams.TexEnvStages?.Length; i++) {
                tevStagesCB.Items.Add($"Tev Stage {i}");
            }

            if (tevStagesCB.Items.Count > 0)
                tevStagesCB.SelectedIndex = 0;

            //Reached max tev stages
            if (tevStagesCB.Items.Count == 6)
                btnAddTevStage.Enabled = false;
        }

        private void UpdateLUT()
        {
            var matParams = ActiveMaterial.MaterialParams;
            var bch = ActiveWrapper.ParentBCH;
            if (bch == null) return;

            lookupTablesCB.Items.Clear();

            string table = FindLUTSet();
            if (table != string.Empty)
                lookupTablesCB.Items.Add(table);

            for (int i = 0; i < bch.H3DFile.LUTs.Count; i++) {
                if (!lookupTablesCB.Items.Contains(bch.H3DFile.LUTs[i].Name))
                    lookupTablesCB.Items.Add(bch.H3DFile.LUTs[i].Name);
            }

            if (table != string.Empty)
                lookupTablesCB.SelectedItem = table;

            LoadLUT(dist0SamplerCB, matParams.LUTDist0TableName, matParams.LUTDist0SamplerName);
            LoadLUT(dist1SamplerCB, matParams.LUTDist1TableName, matParams.LUTDist1SamplerName);
            LoadLUT(reflectionRSamplerCB, matParams.LUTReflecRTableName, matParams.LUTReflecRSamplerName);
            LoadLUT(reflectionGSamplerCB, matParams.LUTReflecGTableName, matParams.LUTReflecGSamplerName);
            LoadLUT(reflectionBSamplerCB, matParams.LUTReflecBTableName, matParams.LUTReflecBSamplerName);
            LoadLUT(fresneSamplerCB, matParams.LUTFresnelTableName, matParams.LUTFresnelSamplerName);

            dist0InputCB.SelectedItem = matParams.LUTInputSelection.Dist0;
            dist1InputCB.SelectedItem = matParams.LUTInputSelection.Dist1;
            reflectionRInputCB.SelectedItem = matParams.LUTInputSelection.ReflecR;
            reflectionGInputCB.SelectedItem = matParams.LUTInputSelection.ReflecG;
            reflectionBInputCB.SelectedItem = matParams.LUTInputSelection.ReflecB;
            fresneInputCB.SelectedItem = matParams.LUTInputSelection.Fresnel;

            chkIsAbsoluteReflectR.Checked = matParams.LUTInputAbsolute.ReflecR;
            chkIsAbsoluteReflectG.Checked = matParams.LUTInputAbsolute.ReflecG;
            chkIsAbsoluteReflectB.Checked = matParams.LUTInputAbsolute.ReflecB;
            chkIsAbsoluteDist0.Checked = matParams.LUTInputAbsolute.Dist0;
            chkIsAbsoluteDist1.Checked = matParams.LUTInputAbsolute.Dist1;
            chkIsAbsoluteFresnel.Checked = matParams.LUTInputAbsolute.Fresnel;
        }

        //Go through all terms and find an active table
        //All should use the same set
        private string FindLUTSet()
        {
            var matParams = ActiveMaterial.MaterialParams;
            if (matParams.LUTDist0TableName != null) return matParams.LUTDist0TableName;
            else if (matParams.LUTDist1TableName != null) return matParams.LUTDist1TableName;
            else if (matParams.LUTReflecRTableName != null) return matParams.LUTReflecRTableName;
            else if (matParams.LUTReflecGSamplerName != null) return matParams.LUTReflecGSamplerName;
            else if (matParams.LUTReflecBTableName != null) return matParams.LUTReflecBTableName;
            else if (matParams.LUTFresnelTableName != null) return matParams.LUTFresnelTableName;
            return "";
        }

        private void LoadLUT(ComboBox combobox, string tableName, string samplerName)
        {
            var bch = ActiveWrapper.ParentBCH;

            combobox.Items.Clear();
            combobox.Items.Add("None Set");

            if (samplerName == null)
                combobox.SelectedIndex = 0;
            else {
                combobox.Items.Add(samplerName);
                combobox.SelectedIndex = 1;
            }

            //Loop through all lookup tables
            for (int i = 0; i < bch.H3DFile.LUTs.Count; i++) {
                var table = bch.H3DFile.LUTs[i];
                for (int j = 0; j < table.Samplers?.Count; j++)
                {
                    var sampler = table.Samplers[j];
                    //Add all samplers if table is active
                    if (table.Name == ActiveLUT && !combobox.Items.Contains(sampler.Name))
                        combobox.Items.Add(sampler.Name);

                    //Find matching table set and sampler
                    if (table.Name == tableName && sampler.Name == samplerName)
                    {
                        //Select the sampler active to term
                        combobox.SelectedItem = sampler.Name;
                    }
                }
            }
        }

        private void UpdateTerms()
        {

            activatedTermList.BeginUpdate();
            activatedTermList.Items.Clear();
            switch (ActiveMaterial.MaterialParams.TranslucencyKind)
            {
                case H3DTranslucencyKind.LayerConfig0:
                    activatedTermList.Items.Add("Spot");
                    activatedTermList.Items.Add("Dist_0");
                    activatedTermList.Items.Add("Reflection_Red");
                    break;
                case H3DTranslucencyKind.LayerConfig1:
                    activatedTermList.Items.Add("Spot");
                    activatedTermList.Items.Add("Reflection_Red");
                    activatedTermList.Items.Add("Fresnel");
                    break;
                case H3DTranslucencyKind.LayerConfig2:
                    activatedTermList.Items.Add("Dist_0");
                    activatedTermList.Items.Add("Dist_1");
                    activatedTermList.Items.Add("Reflection_Red");
                    break;
                case H3DTranslucencyKind.LayerConfig3:
                    activatedTermList.Items.Add("Dist_0");
                    activatedTermList.Items.Add("Dist_1");
                    activatedTermList.Items.Add("Fresnel");
                    break;
                case H3DTranslucencyKind.LayerConfig4:
                    activatedTermList.Items.Add("Spot");
                    activatedTermList.Items.Add("Dist_0");
                    activatedTermList.Items.Add("Dist_1");
                    activatedTermList.Items.Add("Reflection_Red");
                    activatedTermList.Items.Add("Reflection_Green");
                    activatedTermList.Items.Add("Reflection_Blue");
                    break;
                case H3DTranslucencyKind.LayerConfig5:
                    activatedTermList.Items.Add("Spot");
                    activatedTermList.Items.Add("Dist_0");
                    activatedTermList.Items.Add("Reflection_Red");
                    activatedTermList.Items.Add("Reflection_Green");
                    activatedTermList.Items.Add("Reflection_Blue");
                    activatedTermList.Items.Add("Fresnel");
                    break;
                case H3DTranslucencyKind.LayerConfig6:
                    activatedTermList.Items.Add("Spot");
                    activatedTermList.Items.Add("Dist_0");
                    activatedTermList.Items.Add("Dist_1");
                    activatedTermList.Items.Add("Reflection_Red");
                    activatedTermList.Items.Add("Fresnel");
                    break;
                case H3DTranslucencyKind.LayerConfig7:
                    activatedTermList.Items.Add("Spot");
                    activatedTermList.Items.Add("Dist_0");
                    activatedTermList.Items.Add("Dist_1");
                    activatedTermList.Items.Add("Reflection_Red");
                    activatedTermList.Items.Add("Reflection_Green");
                    activatedTermList.Items.Add("Reflection_Blue");
                    activatedTermList.Items.Add("Fresnel");
                    break;
            }
            activatedTermList.EndUpdate();
        }

        private void LoadTevStage()
        {
            int index = tevStagesCB.SelectedIndex;
            if (index != -1)
            {
                var matParams = ActiveMaterial.MaterialParams;
                var stage = matParams.TexEnvStages[index];

                colorCombinerCB.SelectedItem = stage.Combiner.Color;
                alphaCombinerCB.SelectedItem = stage.Combiner.Alpha;
                colorScaleCB.SelectedItem = stage.Scale.Color;
                alphaScaleCB.SelectedItem = stage.Scale.Alpha;


                for (int i = 0; i < stage.Source.Color?.Length; i++) {
                    switch (i)
                    {
                        case 0:
                            colorSource0CB.SelectedItem = stage.Source.Color[i];
                            break;
                        case 1:
                            colorSource1CB.SelectedItem = stage.Source.Color[i];
                            break;
                        case 2:
                            colorSource2CB.SelectedItem = stage.Source.Color[i];
                            break;
                    }
                }

                for (int i = 0; i < stage.Operand.Color?.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            colorOp0CB.SelectedItem = stage.Operand.Color[i];
                            break;
                        case 1:
                            colorOp1CB.SelectedItem = stage.Operand.Color[i];
                            break;
                        case 2:
                            colorOp2CB.SelectedItem = stage.Operand.Color[i];
                            break;
                    }
                }


                for (int i = 0; i < stage.Source.Alpha?.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            alphaSource0CB.SelectedItem = stage.Source.Alpha[i];
                            break;
                        case 1:
                            alphaSource1CB.SelectedItem = stage.Source.Alpha[i];
                            break;
                        case 2:
                            alphaSource2CB.SelectedItem = stage.Source.Alpha[i];
                            break;
                    }
                }

                for (int i = 0; i < stage.Operand.Alpha?.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            alphaOp0CB.SelectedItem = stage.Operand.Alpha[i];
                            break;
                        case 1:
                            alphaOp1CB.SelectedItem = stage.Operand.Alpha[i];
                            break;
                        case 2:
                            alphaOp2CB.SelectedItem = stage.Operand.Alpha[i];
                            break;
                    }
                }

               /* SetSourceImage(colorSource0CB);
                SetSourceImage(colorSource1CB);
                SetSourceImage(colorSource2CB);
                SetSourceImage(alphaSource0CB);
                SetSourceImage(alphaSource1CB);
                SetSourceImage(alphaSource2CB);*/
            }
        }

        private void SetSourceImage(Toolbox.Library.Forms.ImagedComboBox comboBox)
        {
            int index = comboBox.SelectedIndex;
            if (index == -1 || !(comboBox.SelectedItem is PICATextureCombinerSource))
                return;

     /*       switch ((PICATextureCombinerSource)comboBox.SelectedItem)
            {
                case PICATextureCombinerSource.Texture0:
                    if (CombinerImages.ContainsKey("Texture0"))
                        comboBox.Items[index].Image = CombinerImages["Texture0"];
                    break;
                case PICATextureCombinerSource.Texture1:
                    if (CombinerImages.ContainsKey("Texture1"))
                        comboBox.Items[index].Image = CombinerImages["Texture1"];
                    break;
                case PICATextureCombinerSource.Texture2:
                    if (CombinerImages.ContainsKey("Texture2"))
                        comboBox.Items[index].Image = CombinerImages["Texture2"];
                    break;
            }*/
        }

        private void layerCB_SelectedIndexChanged(object sender, EventArgs e) {
            if (ActiveMaterial == null) return;

            ActiveMaterial.MaterialParams.TranslucencyKind = (H3DTranslucencyKind)layerCB.SelectedItem;
            UpdateTerms();
        }

        private void stPanel3_Paint(object sender, PaintEventArgs e)
        {
        }

        private void activatedTermList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void stDropDownPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void stComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tevStagesCB_SelectedIndexChanged(object sender, EventArgs e) {
            LoadTevStage();
        }

        private void colorCombinerCB_SelectedIndexChanged(object sender, EventArgs e) {
        }

        private void colorSource0CB_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void stCheckBox5_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
