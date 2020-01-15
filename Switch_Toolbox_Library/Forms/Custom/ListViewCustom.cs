using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Toolbox.Library.Forms
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ListView))]
    public class ListViewCustom : ListView
    {
        public ListViewCustom()
        {
            this.DoubleBuffered = true;

            BackColor = FormThemes.BaseTheme.ListViewBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;

            InitializeComponent();
        }
        public bool CanResizeList = true;

        public void SetDoubleBuffer() { this.DoubleBuffered = true; }

        public bool TrySelectItem(int index)
        {
            if (Items.Count > 0 && Items.Count < index && index != -1)
            {
                Items[index].Selected = true;
                Select();

                return true;
            }
            else
                return false;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ListViewCustom
            // 
            this.OwnerDraw = true;
            this.BorderStyle = BorderStyle.None;
            this.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.ListViewCustom_DrawColumnHeader);
            this.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListViewCustom_DrawItem);
            this.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.ListViewCustom_DrawSubItem);
            this.Resize += new System.EventHandler(this.ListViewCustom_Resize);
            this.ResumeLayout(false);
        }

        private void ListViewCustom_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(FormThemes.BaseTheme.FormBackColor))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            using (SolidBrush foreBrush = new SolidBrush(FormThemes.BaseTheme.TextForeColor))
            {
                e.Graphics.DrawString(e.Header.Text, e.Font, foreBrush,
                    new RectangleF()
                    {
                        X = e.Bounds.X,
                        Y = e.Bounds.Y + 5,
                        Height = e.Bounds.Height,
                        Width = e.Bounds.Width,
                    });
            }
        }

        private void ListViewCustom_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void ListViewCustom_Resize(object sender, EventArgs e)
        {
            if (View == View.Details && HeaderStyle != ColumnHeaderStyle.None && CanResizeList)
            {
                ((ListView)sender).BeginUpdate();
                SizeLastColumn((ListView)sender);
                ((ListView)sender).EndUpdate();
            }
        }
        private void SizeLastColumn(ListView lv)
        {
            if (lv.Columns.Count > 0)
                lv.Columns[lv.Columns.Count - 1].Width = -2;
        }

        private void ListViewCustom_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }

    public class ListSorter : System.Collections.IComparer
    {
        public int Column = 0;
        public System.Windows.Forms.SortOrder Order = SortOrder.Ascending;
        public int Compare(object x, object y) // IComparer Member
        {
            if (!(x is ListViewItem))
                return (0);
            if (!(y is ListViewItem))
                return (0);

            ListViewItem l1 = (ListViewItem)x;
            ListViewItem l2 = (ListViewItem)y;

            if (l1.ListView.Columns[Column].Tag == null)
            {
                l1.ListView.Columns[Column].Tag = "Text";
            }

            if (l1.ListView.Columns[Column].Tag.ToString() == "Numeric")
            {
                float fl1 = float.Parse(l1.SubItems[Column].Text);
                float fl2 = float.Parse(l2.SubItems[Column].Text);

                if (Order == SortOrder.Ascending)
                {
                    return fl1.CompareTo(fl2);
                }
                else
                {
                    return fl2.CompareTo(fl1);
                }
            }
            else
            {
                string str1 = l1.SubItems[Column].Text;
                string str2 = l2.SubItems[Column].Text;

                if (Order == SortOrder.Ascending)
                {
                    return str1.CompareTo(str2);
                }
                else
                {
                    return str2.CompareTo(str1);
                }
            }
        }
    }
}
