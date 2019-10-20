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

namespace LayoutBXLYT
{
    public partial class AddGroupTargetDialog : STForm
    {
        private BxlanPaiTag activeTag;

        public AddGroupTargetDialog()
        {
            InitializeComponent();
            CanResize = false;
        }

        public bool LoadTag(BxlanPaiTag group)
        {
            activeTag = group;

            curveTypeCB.LoadEnum(typeof(CurveType));
            curveTypeCB.SelectedItem = CurveType.Hermite;

            //Go through each group type
            //If either all targets are used, or is not supported we will return false
            if (group.Type == "PaneSRT")
                return LoadEnum(typeof(LPATarget));
            else if (group.Type == "Visibility")
                return LoadEnum(typeof(LVITarget));
            else if (group.Type == "TextureSRT")
                return LoadEnum(typeof(LTSTarget));
            else if (group.Type == "VertexColor")
                return LoadEnum(typeof(LVCTarget));
            else if (group.Type == "TexturePattern")
                return LoadEnum(typeof(LTPTarget));
            else if (group.Type == "IndTextureSRT")
                return LoadEnum(typeof(LIMTarget));
            else if (group.Type == "AlphaTest")
            {

            }
            else if (group.Type == "FontShadow")
                return LoadEnum(typeof(LCTTarget));
            else if (group.Type == "PerCharacterTransformCurve")
            {

            }

            return false;
        }

        private bool LoadEnum(Type type)
        {
            targetCB.Items.Clear();
            var enums = Enum.GetValues(type);
            foreach (var val in Enum.GetValues(type))
            {
                if (!activeTag.Entries.Any(x => byte.Equals((byte)val, x.AnimationTarget)))
                    targetCB.Items.Add(val);
            }

            if (targetCB.Items.Count > 0)
                targetCB.SelectedIndex = 0;

            //Check if it loaded enums properly
            return targetCB.Items.Count > 0;
        }

        public BxlanPaiTagEntry GetGroupTarget()
        {
            return activeTag.CreateTarget(targetCB.SelectedItem, (byte)curveTypeCB.SelectedItem);
        }
    }
}
