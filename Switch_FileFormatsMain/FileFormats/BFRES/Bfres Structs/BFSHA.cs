using System;
using System.Windows.Forms;

namespace Bfres.Structs
{
    public class BfshaFileData : TreeNode
    {
        public byte[] Data;
        public BfshaFileData(byte[] data, string Name)
        {
            Text = Name;
            ImageKey = "bfsha";
            SelectedImageKey = "bfsha";
            Data = data;

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Import;
        }


        private void Import(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Data = System.IO.File.ReadAllBytes(ofd.FileName);
            }
        }

        private void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All files(*.*)|*.*";
            sfd.DefaultExt = System.IO.Path.GetExtension(Text);
            sfd.FileName = Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllBytes(sfd.FileName, Data);

            }
        }
    }
}
