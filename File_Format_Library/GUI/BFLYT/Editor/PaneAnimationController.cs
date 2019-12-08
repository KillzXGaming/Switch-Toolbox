using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;

namespace LayoutBXLYT
{
    public partial class PaneAnimationController : UserControl
    {
        private PaneEditor ParentEditor;
        private LayoutAnimEditorBasic AnimEditor;

        public PaneAnimationController()
        {
            InitializeComponent();

            AnimEditor = new LayoutAnimEditorBasic();
            AnimEditor.Dock = DockStyle.Fill;
            stPanel1.Controls.Add(AnimEditor);

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();
        }

        public void LoadPane(BasePane pane, PaneEditor paneEditor)
        {
            ParentEditor = paneEditor;

            var animations = ParentEditor.GetAnimations();
            var material = pane.TryGetActiveMaterial();
            string matName = material != null ? material.Name : "";

            listViewCustom1.Items.Clear();

            var archive = pane.LayoutFile.FileInfo.IFileInfo.ArchiveParent;
            if (archive != null)
            {
                foreach (var file in archive.Files) {
                    if (Utils.GetExtension(file.FileName) == ".bflan" &&
                        !animations.Any(x => x.FileName == file.FileName))
                    {
                        if (BxlanHeader.ContainsEntry(file.FileData, new string[2] { pane.Name, matName }))
                            listViewCustom1.Items.Add(new ListViewItem(file.FileName) { Tag = file });
                    }
                }
            }

            for (int i = 0; i < animations?.Count; i++) {
                if (animations[i].ContainsEntry(pane.Name) || animations[i].ContainsEntry(matName))
                {
                    listViewCustom1.Items.Add(new ListViewItem(animations[i].FileName) { Tag = animations[i] });
                }
            }
            LayoutAnimEditorBasic editor = new LayoutAnimEditorBasic();
            stPanel1.Controls.Add(editor);
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                var item = listViewCustom1.SelectedItems[0];
                BxlanHeader bxlan = null;

                if (item.Tag is ArchiveFileInfo) {
                    var fileFormat = ((ArchiveFileInfo)item.Tag).OpenFile();
                    bxlan = ((BXLAN)fileFormat).BxlanHeader;
                }
                else {
                    bxlan = (BxlanHeader)item.Tag;
                }

                if (bxlan == null) return;

                for (int i = 0; i < bxlan.AnimationInfo.Entries.Count; i++)
                    if (bxlan.AnimationInfo.Entries[i].Name == item.Text)
                        AnimEditor.LoadAnimationEntry(bxlan, bxlan.AnimationInfo.Entries[i]);
            }
        }
    }
}
