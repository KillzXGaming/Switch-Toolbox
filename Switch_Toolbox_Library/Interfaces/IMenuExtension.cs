using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.Forms;

namespace Switch_Toolbox.Library
{
    //Based on Exelix's menu ext
    public interface IMenuExtension
    {
        ToolStripItemDark[] FileMenuExtensions { get; }
        ToolStripItemDark[] ToolsMenuExtensions { get; }
        ToolStripItemDark[] TitleBarExtensions { get; }
    }
    public interface IFileMenuExtension
    {
        ToolStripItemDark[] NewFileMenuExtensions { get; }
        ToolStripItemDark[] CompressionMenuExtensions { get; }
        ToolStripItemDark[] ToolsMenuExtensions { get; }
        ToolStripItemDark[] TitleBarExtensions { get; }
        ToolStripItemDark[] ExperimentalMenuExtensions { get; }
    }
}
