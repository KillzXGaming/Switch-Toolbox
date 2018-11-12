using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switch_Toolbox.Library
{
    public class FileManager
    {
        public FileManager()
        {

        }
        public static IFileMenuExtension[] GetMenuExtensions()
        {
            //Add plugin and main application menu extensions
            List<IFileMenuExtension> types = new List<IFileMenuExtension>();
            foreach (IFileFormat fileFormat in GetFileFormats())
            {
                foreach (Type T in fileFormat.Types)
                {
                    Type[] interfaces_array = T.GetInterfaces();
                    for (int i = 0; i < interfaces_array.Length; i++)
                    {
                        if (interfaces_array[i] == typeof(IFileMenuExtension))
                        {
                            types.Add((IFileMenuExtension)Activator.CreateInstance(T));
                        }
                    }
                }
            }

            return types.ToArray();
        }
        public static IFileFormat[] GetFileFormats()
        {
            //Add plugin and main application file formats
            List<IFileFormat> types = new List<IFileFormat>();
            foreach (var plugin in GenericPluginLoader._Plugins)
            {
                foreach (Type T in plugin.Value.Types)
                {
                    Type[] interfaces_array = T.GetInterfaces();
                    for (int i = 0; i < interfaces_array.Length; i++)
                    {
                        if (interfaces_array[i] == typeof(IFileFormat))
                        {
                            types.Add((IFileFormat)Activator.CreateInstance(T));
                        }
                    }
                }
            }

            return types.ToArray();
        }
    }
}
