using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginContracts;

namespace Toolbox.Library
{
   public static class GenericPluginLoader
    {
        public static Dictionary<string, IPlugin> _Plugins;

        public static void LoadPlugin()
        {
            _Plugins = new Dictionary<string, IPlugin>();

            ICollection<IPlugin> plugins = PluginLoader.LoadPlugins();
            foreach (var item in plugins)
            {
                _Plugins.Add(item.Name, item);
            }
            plugins.Clear();
        }
    }
}
