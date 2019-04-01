using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.NodeWrappers;
using Switch_Toolbox.Library;
using System.Windows.Forms;
using ResU = Syroot.NintenTools.Bfres;
using ResNX = Syroot.NintenTools.NSW.Bfres;
using Bfres.Structs;

namespace FirstPlugin.NodeWrappers
{
    public class BFRESWrapper : STGenericWrapper
    {
        public override void OnClick(TreeView treeview)
        {
            base.OnClick(treeview);
        }

        public bool IsWiiU { get; set; }
      

        public void LoadMenus(bool isWiiUBfres)
        {
        }

        public override void Delete()
        {
            var editor = LibraryGUI.Instance.GetObjectEditor();
            if (editor != null)
            {
                editor.RemoveFile(this);
                editor.ResetControls();
            }
        }

        protected void ImportModelAction(object sender, EventArgs e) { ImportModel(); }
        protected void ImportEmbeddedFileAction(object sender, EventArgs e) { ImportEmbeddedFile(); }

        public void ImportModel()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            BFRESGroupNode group = new BFRESGroupNode();
            group.Import(ofd.FileNames);
        }

        public void ImportEmbeddedFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            BFRESGroupNode group = new BFRESGroupNode();
            group.Import(ofd.FileNames);
        }

        public void NewModel()
        {
            BFRESGroupNode group = new BFRESGroupNode();
            FMDL anim = new FMDL();

            if (IsWiiU)
                BfresWiiU.ReadModel(anim, new ResU.Model());
            else
                BfresSwitch.ReadModel(anim, new ResNX.Model());
            
            group.AddNode(anim, "NewModel");
        }

        private void SetupAddedNode(BFRESGroupNode group, STGenericWrapper node)
        {
            Nodes.Add(group);
            TreeView.SelectedNode = node;
        }

        public void NewEmbeddedFile()
        {
            BFRESGroupNode group = new BFRESGroupNode();
            ExternalFileData fshu = new ExternalFileData("NewFile", new byte[0]);
            group.AddNode(fshu, "NewFile");
            SetupAddedNode(group, fshu);
        }
    }
}
