using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using PluginContracts;

namespace Switch_Toolbox.Library
{
    public class PluginLoader
    {
        public Dictionary<string, IPlugin> _Plugins;

        public PluginLoader()
        {

        }
        public static ICollection<IPlugin> LoadPlugins()
        {
            string path = Runtime.ExecutableDir + "/Lib/Plugins";

            string[] dllFileNames = null;
            if (Directory.Exists(path))
            {
                dllFileNames = Directory.GetFiles(path, "*.Plg.dll");
            }
            if (dllFileNames == null)
            {
                System.Windows.Forms.MessageBox.Show($"Could not find any plugins in {path}", "",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return new List<IPlugin>();
            }

            ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
            foreach (string dllFile in dllFileNames)
            {
                AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                Assembly assembly = Assembly.Load(an);
                assemblies.Add(assembly);
            }

            Type pluginType = typeof(IPlugin);
            ICollection<Type> pluginTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly != null)
                {
                    try
                    {
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.IsInterface || type.IsAbstract)
                            {
                                continue;
                            }
                            else
                            {
                                if (type.GetInterface(pluginType.FullName) != null)
                                {
                                    pluginTypes.Add(type);
                                }
                            }
                        }
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (Exception exSub in ex.LoaderExceptions)
                        {
                            sb.AppendLine(exSub.Message);
                            FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                            if (exFileNotFound != null)
                            {
                                if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                                {
                                    sb.AppendLine("Fusion Log:");
                                    sb.AppendLine(exFileNotFound.FusionLog);
                                }
                            }
                            sb.AppendLine();
                        }
                        string errorMessage = sb.ToString();
                        throw new Exception(errorMessage);
                    }
                }
            }

            ICollection<IPlugin> plugins = new List<IPlugin>(pluginTypes.Count);
            foreach (Type type in pluginTypes)
            {
                IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                plugins.Add(plugin);
            }
            assemblies.Clear();
            pluginTypes.Clear();

            return plugins;
        }
    }
}
