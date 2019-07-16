using System;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightIdeasSoftware;
using System.Runtime.InteropServices;

namespace Toolbox.Library.Forms
{
    public class STListView : ObjectListView
    {
        public bool CanResizeList = true;

        public STListView()
        {
            InitializeComponent();

            BackColor = FormThemes.BaseTheme.ListViewBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        public void SetDoubleBuffer() { this.DoubleBuffered = true; }

        public void SetTheme() => adjustMyObjectListViewHeader();

        public void FillLastColumnSpace(bool FillSpace)
        {
            if (Columns.Count > 0)
            {
                ((OLVColumn)Columns[Columns.Count - 1]).FillsFreeSpace = FillSpace;
            }
        }

        private void adjustMyObjectListViewHeader()
        {
            BorderStyle = BorderStyle.None;
            foreach (OLVColumn item in Columns)
            {
                var headerstyle = new HeaderFormatStyle();
                headerstyle.SetBackColor(FormThemes.BaseTheme.FormBackColor);
                headerstyle.SetForeColor(FormThemes.BaseTheme.FormForeColor);
                item.HeaderFormatStyle = headerstyle;
            }
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // STListView
            // 
            this.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.STListView_ColumnWidthChanged);
            this.Resize += new System.EventHandler(this.STListView_Resize);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        private void STListView_Resize(object sender, EventArgs e)
        {

        }

        private void STListView_DrawColumnHeader(object sender, System.Windows.Forms.DrawListViewColumnHeaderEventArgs e)
        {
        }

        private void STListView_ColumnWidthChanged(object sender, System.Windows.Forms.ColumnWidthChangedEventArgs e)
        {
    
        }
    }
}
