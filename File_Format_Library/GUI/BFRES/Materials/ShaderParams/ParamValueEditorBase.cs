using System;
using System.Windows.Forms;
using Bfres.Structs;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class ParamValueEditorBase : STUserControl
    {
        public Func<BfresShaderParam, UserControl, bool> OnPanelChanged;

        public BfresShaderParam activeParam;

        public ParamValueEditorBase()
        {
        }

        public void LoadAction(Func<BfresShaderParam, UserControl, bool> panelChanged)
        {
            OnPanelChanged = panelChanged;
        }
    }
}
