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

namespace FirstPlugin.CtrLibrary.Forms
{
    public partial class BCHMaterialGeneralEditor : UserControl, IMaterialLoader
    {
        public BCHMaterialGeneralEditor()
        {
            InitializeComponent();

            faceCullingCB.LoadEnum(typeof(PICAFaceCulling));

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();
            stDropDownPanel3.ResetColors();
        }

        public void LoadMaterial(H3DMaterialWrapper wrapper)
        {
            var matParams = wrapper.Material.MaterialParams;
            faceCullingCB.SelectedItem = matParams.FaceCulling;

            chkEnableFragLighting.Checked = matParams.Flags.HasFlag(H3DMaterialFlags.IsFragmentLightingEnabled);
            chkEnableHemiLighting.Checked = matParams.Flags.HasFlag(H3DMaterialFlags.IsHemiSphereLightingEnabled);
            chkEnableOcclusion.Checked = matParams.Flags.HasFlag(H3DMaterialFlags.IsHemiSphereOcclusionEnabled);
            chkEnableVertLighting.Checked = matParams.Flags.HasFlag(H3DMaterialFlags.IsVertexLightingEnabled);
            chkEnableFog.Checked = matParams.Flags.HasFlag(H3DMaterialFlags.IsFogEnabled);
            fogIndexUD.Value = matParams.FogIndex;
        }
    }
}
