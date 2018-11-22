using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using WeifenLuo.WinFormsUI.Docking;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using ResU = Syroot.NintenTools.Bfres;
using ResUGX2 = Syroot.NintenTools.Bfres.GX2;
using ResGFX = Syroot.NintenTools.NSW.Bfres.GFX;
using FirstPlugin;

namespace Bfres.Structs
{
    public class FmmaFolder : TreeNodeCustom
    {
        public FmmaFolder()
        {
            Text = "Material Animations";
            Name = "FMAA";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }

    public class FMAA
    {

    }
}
