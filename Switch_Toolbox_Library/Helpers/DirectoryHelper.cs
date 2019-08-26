using System.Linq;
using System.IO;
using System.Security.AccessControl;

namespace Toolbox.Library
{
    public class DirectoryHelper
    {
        public static bool IsDirectoryEmpty(string path)
        {
            if (!HasFolderPermission(path))
                return false;

            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        public static bool HasFolderPermission(string folderPath)
        {
            return true;

            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            try
            {
                DirectorySecurity dirAC = dirInfo.GetAccessControl(AccessControlSections.All);
                return true;
            }
            catch (PrivilegeNotHeldException)
            {
                return false;
            }
        }
    }
}
