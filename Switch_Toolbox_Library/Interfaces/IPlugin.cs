using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;

namespace PluginContracts
{
    /// <summary>
    /// Reprenets a plguin to be loaded externally.
    /// These will be loaded if a project uses this, is in the Lib/Plugins folder
    /// and that the .dll has .plg in the name.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The author of the plugin.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// The description of the plugin.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The version of the plugin.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// A type array to load custom data
        /// These can load <see cref="IFileFormat"/> to load new files
        /// These can also load <see cref="IMenuExtension"/> to load custom menu extensions
        /// </summary>
        Type[] Types { get; } //Types hold File extensions

        /// <summary>
        /// Method to execute when the file is loaded.
        /// </summary>
        void Load();

        /// <summary>
        /// Method to execute when the file is unloaded.
        /// </summary>
        void Unload();
    }
}
