using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library
{
    /// <summary>
    /// Represents a folder system from the computer
    /// </summary>
    public class ExplorerFolder : TreeNodeCustom
    {
        private string _path;
        private readonly bool CanExpand = true;

        public ExplorerFolder(string path)
        {
            _path = path;
            Text = Path.GetFileName(_path);

            if (!DirectoryHelper.HasFolderPermission(_path))
            {
                ForeColor = System.Drawing.Color.DarkRed;
                CanExpand = false;
            }
            else if (!DirectoryHelper.IsDirectoryEmpty(_path))
                OnAfterCollapse();

            ImageKey = "folder";
            SelectedImageKey = "folder";
        }

        public void OnBeforeExpand()
        {
            if (IsExpanded || !CanExpand) return;
            Nodes.Clear();
            foreach (string str in Directory.GetDirectories($"{_path}\\"))
            {
                if (File.GetAttributes(str).HasFlag(FileAttributes.Directory))
                    Nodes.Add(new ExplorerFolder(str));
            }
            foreach (string str in Directory.GetFiles($"{_path}\\"))
            {
                if (!File.GetAttributes(str).HasFlag(FileAttributes.Directory))
                    Nodes.Add(new ExplorerFile(str));
            }
        }

        public void OnAfterCollapse()
        {
            Nodes.Clear();
            Nodes.Add(new TreeNode("0"));
        }
    }
}
