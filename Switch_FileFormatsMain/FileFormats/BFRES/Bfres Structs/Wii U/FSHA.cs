using Switch_Toolbox.Library;
using System.Windows.Forms;
using FirstPlugin;

namespace Bfres.Structs
{
    public class TexSrtFolder : TreeNodeCustom
    {
        public TexSrtFolder()
        {
            Text = "Texture SRT Animations";
            Name = "TEXSRT";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
    public class FshaFolder : TreeNodeCustom
    {
        public FshaFolder()
        {
            Text = "Shader Parameter Animations";
            Name = "FSHA";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
    public class FshaColorFolder : TreeNodeCustom
    {
        public FshaColorFolder()
        {
            Text = "Color Animations";
            Name = "FSHAColor";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
}
