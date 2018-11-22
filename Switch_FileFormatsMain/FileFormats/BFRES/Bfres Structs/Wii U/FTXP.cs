using Switch_Toolbox.Library;
using System.Windows.Forms;
using FirstPlugin;

namespace Bfres.Structs
{
    public class TexPatFolder : TreeNodeCustom
    {
        public TexPatFolder()
        {
            Text = "Texture Pattern Animations";
            Name = "TEXPAT";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }

}
