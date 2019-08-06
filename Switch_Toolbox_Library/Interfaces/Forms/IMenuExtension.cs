using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using System.Windows.Forms;

namespace Toolbox.Library
{
    /// <summary>
    /// Represets a set of menu extensions to load within the main menu toolbar.
    /// </summary>
    public interface IMenuExtension
    {
        STToolStripItem[] FileMenuExtensions { get; }
        STToolStripItem[] ToolsMenuExtensions { get; }
        STToolStripItem[] TitleBarExtensions { get; }
    }

    /// <summary>
    /// Represets a set of menu extensions to load within the main file menu toolbar.
    /// </summary>
    public interface IFileMenuExtension
    {
        STToolStripItem[] NewFileMenuExtensions { get; }
        STToolStripItem[] NewFromFileMenuExtensions { get; }
        STToolStripItem[] CompressionMenuExtensions { get; }
        STToolStripItem[] ToolsMenuExtensions { get; }
        STToolStripItem[] TitleBarExtensions { get; }
        STToolStripItem[] ExperimentalMenuExtensions { get; }
        STToolStripItem[] EditMenuExtensions { get; }
        ToolStripButton[] IconButtonMenuExtensions { get; }
    }
}
