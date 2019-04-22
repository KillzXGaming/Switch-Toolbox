using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class MSBTEditor : STForm, IFIleEditor
    {
        public List<IFileFormat> GetFileFormats()
        {
            return new List<IFileFormat>() { activeMessageFile };
        }

        public MSBTEditor()
        {
            InitializeComponent();

            listViewCustom1.HeaderStyle = ColumnHeaderStyle.None;
        }

        MSBT activeMessageFile;

        public void LoadMSBT(MSBT msbt)
        {
            activeMessageFile = msbt;
        }
    }
}
