using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class EffectTableEditor : UserControl, IFIleEditor
    {
        public List<IFileFormat> GetFileFormats()
        {
            return new List<IFileFormat>() { EffectTableFile };
        }

        public EffectTableEditor()
        {
            InitializeComponent();
        }

        PTCL ptcl;

        EFCF EffectTableFile;
        public void LoadEffectFile(EFCF efcf)
        {
            EffectTableFile = efcf;

            stDataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "ActiveTime",
                Name = "ActiveTime",
                ReadOnly = true,
            });
            stDataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "PtclEmitter",
                Name = "PtclEmitter",
            });
            stDataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "PtclSlotData",
                Name = "PtclSlotData",
            });
            stDataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
            {
                HeaderText = "StringBankSlot",
                Name = "StringBankSlot",
            });

            ReloadDataGrid();
        }

        private void ReloadDataGrid()
        {
            stDataGridView1.Rows.Clear();

            int rowIndex = 0;
            foreach (var effect in EffectTableFile.EfcHeader.Entries)
            {
                rowIndex = this.stDataGridView1.Rows.Add();

                var row = stDataGridView1.Rows[rowIndex];

                row.Cells["ActiveTime"].Value = effect.ActiveTime;

                if (ptcl != null)
                {
                    if (ptcl.headerU.emitterSets.Count > effect.PtclStringSlot && effect.PtclStringSlot != -1)
                    {
                        row.Cells["PtclEmitter"].Value = ptcl.headerU.emitterSets[(int)effect.PtclStringSlot].Text;
                        row.Cells["PtclSlotData"].Value = effect.SlotSpecificPtclData;
                    }
                    else
                    {
                        row.Cells["PtclEmitter"].Value = effect.PtclStringSlot;
                        row.Cells["PtclSlotData"].Value = effect.SlotSpecificPtclData;
                    }
                }
                else
                {
                    row.Cells["PtclEmitter"].Value = effect.PtclStringSlot;
                    row.Cells["PtclSlotData"].Value = effect.SlotSpecificPtclData;
                }

                if (EffectTableFile.EfcHeader.StringEntries.Count > effect.StringBankSlot && effect.StringBankSlot != - 1)
                    row.Cells["StringBankSlot"].Value = EffectTableFile.EfcHeader.StringEntries[(int)effect.StringBankSlot];
                else
                    row.Cells["StringBankSlot"].Value = effect.StringBankSlot;
            }

            stDataGridView1.ApplyStyles();
        }

        private void addPTCLReferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ptcl =  (PTCL)Toolbox.Library.IO.STFileLoader.OpenFileFormat(ofd.FileName);

                ReloadDataGrid();
            }
        }
    }
}
