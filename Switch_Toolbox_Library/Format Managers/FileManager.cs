using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public class FileManager
    {
        public FileManager()
        {

        }

        public static string GetSourcePath(IFileFormat fileFormat)
        {
            var info = fileFormat.IFileInfo;
            if (info != null && info.ArchiveParent != null)
                return GetSourcePath((IFileFormat)info.ArchiveParent);

            return fileFormat.FilePath;
        }

        public static IFileMenuExtension[] GetMenuExtensions()
        {
            //Add plugin and main application menu extensions
            List<IFileMenuExtension> types = new List<IFileMenuExtension>();
            foreach (IFileFormat fileFormat in GetFileFormats())
            {
                foreach (Type type in fileFormat.Types)
                {
                    Type[] interfaces_array = type.GetInterfaces();
                    for (int i = 0; i < interfaces_array.Length; i++)
                    {
                        if (interfaces_array[i] == typeof(IFileMenuExtension))
                        {
                            types.Add((IFileMenuExtension)Activator.CreateInstance(type));
                        }
                    }
                }
            }

            return types.ToArray();
        }

        public static IFileMenuExtension GetMenuExtensions(IFileFormat fileFormat)
        {
            foreach (Type type in fileFormat.Types)
            {
                Type[] interfaces_array = type.GetInterfaces();
                for (int i = 0; i < interfaces_array.Length; i++)
                {
                    if (interfaces_array[i] == typeof(IFileMenuExtension))
                    {
                       return (IFileMenuExtension)Activator.CreateInstance(type);
                    }
                }
            }
            return null;
        }

        public static IEditor<System.Windows.Forms.UserControl>[] GetEditors()
        {
            var editors = new List<IEditor<System.Windows.Forms.UserControl>>();
            foreach (var plugin in GenericPluginLoader._Plugins)
            {
                foreach (Type type in plugin.Value.Types)
                {
                    Type[] interfaces_array = type.GetInterfaces();
                    for (int i = 0; i < interfaces_array.Length; i++)
                    {
                        if (interfaces_array[i] == typeof(IEditor<System.Windows.Forms.UserControl>))
                        {
                            editors.Add((IEditor<System.Windows.Forms.UserControl>)Activator.CreateInstance(type));
                        }
                    }
                }
            }
            return editors.ToArray();
        }

        public static VGAdudioFile[] GetVGAudioFileFormats()
        {
            List<VGAdudioFile> types = new List<VGAdudioFile>();

            foreach (var fileFormat in GetFileFormats())
            {
                if (fileFormat is VGAdudioFile)
                    types.Add((VGAdudioFile)fileFormat);
            }

            return types.ToArray();
        }

        public static ICompressionFormat[] GetCompressionFormats()
        {
            //Add plugin and main application file formats
            List<ICompressionFormat> types = new List<ICompressionFormat>();
            LoadCompressionFormats(FormatList.GetCompressionFormats(), types);

            if (GenericPluginLoader._Plugins == null)
                GenericPluginLoader.LoadPlugin();

            foreach (var plugin in GenericPluginLoader._Plugins) {
                LoadCompressionFormats(plugin.Value.Types, types);
            }

            return types.ToArray();
        }

        private static void LoadCompressionFormats(Type[] Types, List<ICompressionFormat> Formats)
        {
            foreach (Type type in Types)
            {
                Type[] interfaces_array = type.GetInterfaces();
                for (int i = 0; i < interfaces_array.Length; i++)
                {
                    if (interfaces_array[i] == typeof(ICompressionFormat))
                    {
                        Formats.Add((ICompressionFormat)Activator.CreateInstance(type));
                    }
                }
            }
        }

        public static IFileFormat[] GetFileFormats()
        {
            //Add plugin and main application file formats
            List<IFileFormat> types = new List<IFileFormat>();
            LoadFileFormats(FormatList.GetFileFormats(), types);

            if (GenericPluginLoader._Plugins == null)
                GenericPluginLoader.LoadPlugin();

            foreach (var plugin in GenericPluginLoader._Plugins)
            {
                LoadFileFormats(plugin.Value.Types, types);
            }

            return types.ToArray();
        }

        private static void LoadFileFormats(Type[] Types, List<IFileFormat> Formats)
        {
            foreach (Type type in Types)
            {
                Type[] interfaces_array = type.GetInterfaces();
                for (int i = 0; i < interfaces_array.Length; i++)
                {
                    if (interfaces_array[i] == typeof(IFileFormat))
                    {
                        Formats.Add((IFileFormat)Activator.CreateInstance(type));
                    }
                }
            }
        }
    }
}
