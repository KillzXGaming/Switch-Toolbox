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
    public class FshuFolder : TreeNodeCustom
    {
        public FshuFolder()
        {
            Text = "Shader Parameter Animations";
            Name = "FSHA";
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }
    public class FshuColorFolder : TreeNodeCustom
    {
        public FshuColorFolder()
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
