using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Security;
using System.Security.Permissions;
using System.Diagnostics;

namespace Toolbox.WindowsExplorer
{
    static class FileShellExtension
    {
        public static void Register(string keyText, string menuText, string regType = "*", string command = "")
        {
            string regPath = string.Format(@"{0}\shell\{1}", regType, keyText);
            string cmdPath = string.Format(@"{0}\shell\{1}\command", regType, keyText);
            string iconPath = string.Format(@"{0}\shell\{1}\command", regType, keyText);

            //Key for menu
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(regPath))
            {
                key.SetValue(null, menuText);
            }

            //Command key
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(cmdPath)) {
                key.SetValue(null, command);
            }

            //Add an icon key
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(regPath)) {
                key.SetValue("Icon", System.Reflection.Assembly.GetEntryAssembly().Location, Microsoft.Win32.RegistryValueKind.String);
            }
        }

        public static void Unregister(string fileType, string shellKeyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(fileType) &&
                !string.IsNullOrEmpty(shellKeyName));

            // path to the registry location
            string regPath = string.Format(@"{0}\shell\{1}",
                                           fileType, shellKeyName);

            // remove context menu from the registry
            Registry.ClassesRoot.DeleteSubKeyTree(regPath);
        }
    }

    public class ExplorerContextMenu
    {
        private const string FileMenuName = "File\\shell";
        private const string DirectoryMenuName = "Directory\\Background\\Shell";
        private const string FolderMenuName = "Folder\\shell";

        private static string ApplicationExecutable
        {
            get { return System.Reflection.Assembly.GetEntryAssembly().Location; }
        }

        public static void LoadMenus()
        {
            //     RemoveRegistry("Compress YAZ0");
            //   RemoveRegistry("Decompress YAZ0");

            string menuCommand = string.Format("\"{0}\" \"%L\" \"-YAZ0\" \"-d\" ", ApplicationExecutable);

          //  FileShellExtension.Register("Switch Toolbox", "Switch Toolbox/Decompress YAZ0","*", menuCommand);
         //   FileShellExtension.Register("jpegfile", "Switch Toolbox", "Decompress YAZ0", menuCommand);
         //   AddFileRegistry("Compress YAZ0", "-Yaz0 -c");
          //  AddDirectoryRegistry("Create Archive/New SARC", "-sarc -p");

           // AddDirectoryRegistry("Compress YAZ0", "-Yaz0 -c");
          //  AddDirectoryRegistry("Decompress YAZ0", "-Yaz0 -d");
        }

        private static void AddFileRegistry(string fileType, string shellKeyName, string menuText, string menuCommand)
        {
            string regPath = string.Format(@"{0}\shell\{1}", fileType, shellKeyName);

            // add context menu to the registry
            using (RegistryKey key =
                   Registry.ClassesRoot.CreateSubKey(regPath))
            {
                key.SetValue(null, menuText);
            }

            // add command that is invoked to the registry
            using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(
                string.Format(@"{0}\command", regPath)))
            {
                key.SetValue(null, menuCommand);
            }
        }

        private static void AddDirectoryRegistry(string MenuName, string Arguments)
        {
            RegistryKey _key = Registry.ClassesRoot.OpenSubKey(FolderMenuName, true);
            RegistryKey newkey = _key.CreateSubKey(MenuName);
            RegistryKey subNewkey = newkey.CreateSubKey("Command");
            subNewkey.SetValue("",string.Format("{0} {1}", ApplicationExecutable, Arguments));
            subNewkey.Close();
            newkey.Close();
            _key.Close();
        }

        private static void RemoveDirectoryRegistry(string MenuName)
        {
            RegistryKey _key = Registry.ClassesRoot.OpenSubKey("Directory\\Background\\Shell\\", true);
            _key.DeleteSubKey(MenuName);
            _key.Close();
        }
    }
}
