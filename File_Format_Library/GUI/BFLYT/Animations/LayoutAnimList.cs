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
using Toolbox.Library.Forms;
using LayoutBXLYT.Cafe;

namespace LayoutBXLYT
{
    public partial class LayoutAnimList : LayoutDocked
    {
        private EventHandler OnProperySelected;
        private bool isLoaded = false;
        private LayoutEditor ParentEditor;
        private ImageList imgList = new ImageList();

        public LayoutAnimList(LayoutEditor parentEditor, EventHandler onPropertySelected)
        {
            OnProperySelected = onPropertySelected;
            ParentEditor = parentEditor;
            InitializeComponent();

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;

            listView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            listView1.ForeColor = FormThemes.BaseTheme.FormForeColor;
            listView1.FullRowSelect = true;

            imgList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(24, 24),
            };

            imgList.Images.Add("LayoutAnimation", FirstPlugin.Properties.Resources.LayoutAnimation);

            listView1.SmallImageList = imgList;
            listView1.LargeImageList = imgList;

            listView1.Sorting = SortOrder.Ascending;
        }

        public void SearchAnimations(BxlytHeader bxlyt)
        {
            isLoaded = false;

            var layoutFile = bxlyt.FileInfo;
            var parentArchive = layoutFile.IFileInfo.ArchiveParent;
            if (parentArchive == null) return;

            listView1.BeginUpdate();
            foreach (var file in parentArchive.Files)
            {
                if (Utils.GetExtension(file.FileName) == ".brlan" ||
                    Utils.GetExtension(file.FileName) == ".bclan" ||
                    Utils.GetExtension(file.FileName) == ".bflan")
                {
                    LoadAnimation(file);
                }
            }

            listView1.Sort();
            listView1.EndUpdate();

            isLoaded = true;
        }

        public void LoadAnimation(ArchiveFileInfo archiveEntry)
        {
            listView1.Items.Add(new ListViewItem(System.IO.Path.GetFileName(archiveEntry.FileName))
            {
                Tag = archiveEntry,
                ImageKey = "LayoutAnimation",
            });
        }

        public void LoadAnimation(BxlanHeader bxlan)
        {
            isLoaded = false;
            listView1.BeginUpdate();
            listView1.Items.Add(new ListViewItem(bxlan.FileName)
            {
                Tag = bxlan,
                ImageKey = "LayoutAnimation",
            });
            listView1.Sort();
            listView1.EndUpdate();

            isLoaded = true;
        }

        public ListView.SelectedListViewItemCollection GetSelectedAnimations => listView1.SelectedItems;

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (isLoaded)
                OnProperySelected.Invoke("Select", e);

            if (listView1.SelectedItems.Count > 0)
            {
                var bxlan = listView1.SelectedItems[0].Tag as BxlanHeader;
              //  if (bxlan != null)
               //     ParentEditor.ShowBxlanEditor(bxlan);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var bxlan = listView1.SelectedItems[0].Tag as BxlanHeader;
                if (bxlan != null)
                    ParentEditor.ShowBxlanEditor(bxlan);
            }
        }
    }
}
