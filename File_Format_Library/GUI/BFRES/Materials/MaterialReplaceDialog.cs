using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class MaterialReplaceDialog : STForm
    {
        bool IsLoaded = false;

        public MaterialReplaceDialog()
        {
            InitializeComponent();

            stButton2.Select();

            paramChkBox.Checked = PluginRuntime.MaterialReplace.SwapShaderParams;
            optionsChkBox.Checked = PluginRuntime.MaterialReplace.SwapShaderOptions;
            renderInfosChkBox.Checked = PluginRuntime.MaterialReplace.SwapRenderInfos;
            textureChkBox.Checked = PluginRuntime.MaterialReplace.SwapTextures;
            UserDataChkBox.Checked = PluginRuntime.MaterialReplace.SwapUserData;

            IsLoaded = true;
        }

        private void ChkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoaded)
                return;

            PluginRuntime.MaterialReplace.SwapShaderParams = paramChkBox.Checked;
            PluginRuntime.MaterialReplace.SwapShaderOptions = optionsChkBox.Checked;
            PluginRuntime.MaterialReplace.SwapRenderInfos = renderInfosChkBox.Checked;
            PluginRuntime.MaterialReplace.SwapTextures = textureChkBox.Checked;
            PluginRuntime.MaterialReplace.SwapUserData = UserDataChkBox.Checked;
        }
    }
}
