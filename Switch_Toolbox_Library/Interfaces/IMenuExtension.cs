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
        STToolStripItem[] FileMenuExtensions { get; }
        STToolStripItem[] ToolsMenuExtensions { get; }
        STToolStripItem[] TitleBarExtensions { get; }
    }
    public interface IFileMenuExtension
    {
        STToolStripItem[] NewFileMenuExtensions { get; }
        STToolStripItem[] NewFromFileMenuExtensions { get; }
        STToolStripItem[] CompressionMenuExtensions { get; }
        STToolStripItem[] ToolsMenuExtensions { get; }
        STToolStripItem[] TitleBarExtensions { get; }
        STToolStripItem[] ExperimentalMenuExtensions { get; }
    }
}
