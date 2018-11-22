using Switch_Toolbox.Library;
using System.Windows.Forms;
using FirstPlugin;

namespace Bfres.Structs
{
    public class FbnvFolder : TreeNodeCustom
    {
        public FbnvFolder()
        {
            Text = "Bone Visabilty Animations";
            Name = "FBNV";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }

}
