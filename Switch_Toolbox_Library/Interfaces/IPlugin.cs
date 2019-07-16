using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;

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
        void Load();
        void Unload();
    }
}
