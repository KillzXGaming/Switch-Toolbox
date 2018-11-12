using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;

namespace PluginContracts
{
    public interface IPlugin
    {
        string Name { get; }
        string Author { get; }
        string Description { get; }
        string Version { get; }
        //List of types
        //IFileFormat
        Type[] Types { get; } //Types hold File extensions
        Form MainForm { get; set; }
        DockContentST DockedEditor { get; set; }
        void Load();
        void Unload();
    }
}
