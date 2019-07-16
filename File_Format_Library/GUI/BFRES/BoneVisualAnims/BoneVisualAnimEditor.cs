using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using ResU = Syroot.NintenTools.Bfres;
using Syroot.NintenTools.NSW.Bfres;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bfres.Structs;
using Toolbox.Library.Forms;
using Toolbox.Library.Animations;
using BrightIdeasSoftware;

namespace FirstPlugin.Forms
{
    public partial class BoneVisualAnimEditor : UserControl
    {
        public BoneVisualAnimEditor()
        {
            InitializeComponent();
        }

        public void UpdateDataGrid()
        {
            if (activeFVIS == null)
                return;

            if (activeFVIS.VisibilityAnimU != null)
            {
                activeFVIS.FrameCount = activeFVIS.VisibilityAnimU.FrameCount;

                if (activeFVIS.FrameCount != dataGridView1.RowCount)
                {
                    UpdateFrameCells(activeFVIS);
                }
            }
            else
            {
                activeFVIS.FrameCount = activeFVIS.VisibilityAnim.FrameCount;

                if (activeFVIS.FrameCount != dataGridView1.RowCount)
                {
                    UpdateFrameCells(activeFVIS);
                }
            }
        }

        FVIS activeFVIS;

        bool isLoaded = false;
        public void LoadVisualAnim(FVIS vis)
        {
            activeFVIS = vis;

            UpdateBoneColumns(vis);
            UpdateFrameCells(vis);

            isLoaded = true;
        }

        private void UpdateBoneColumns(FVIS vis)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();

            //Load frame column always first!
            var column = new DataGridViewTextBoxColumn()
            {
                HeaderText = "Frame",
                Name = "FrameColumn",
                ReadOnly = true,
            };
            dataGridView1.Columns.Add(column);

            //Set bone columns
            for (int curve = 0; curve < vis.Values.Count; curve++)
            {
                var col4 = new DataGridViewCheckBoxColumn()
                {
                    HeaderText = vis.Values[curve].Text,
                    Name = vis.Values[curve].Text,
                };

                dataGridView1.Columns.Add(col4);
            }

            dataGridView1.ApplyStyles();
        }

        private void UpdateFrameCells(FVIS vis)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();

            int rowIndex = 0;
            for (int frame = 0; frame < vis.FrameCount; frame++)
            {
                if (IsKeyed(vis, frame))
                {
                    rowIndex = this.dataGridView1.Rows.Add();
                    var row = this.dataGridView1.Rows[rowIndex];
                    Console.WriteLine("rowIndex " + rowIndex);

                    row.Cells["FrameColumn"].Value = frame.ToString();

                    for (int curve = 0; curve < vis.Values.Count; curve++)
                    {
                        bool IsVisible = vis.Values[curve].GetValue(frame);

                        string boneName = vis.Values[curve].Text;

                        row.Cells[boneName].Value = IsVisible;
                    }
                }
            }


            dataGridView1.ApplyStyles();
        }

        private bool IsKeyed(FVIS fvis, int Frame)
        {
            foreach (BooleanKeyGroup track in fvis.Values)
            {
                if (track.GetKeyFrame(Frame).IsKeyed == true)
                    return true;
            }

            return false;
        }

        public void AddBone(string BoneName)
        {
            var column = new DataGridViewColumn
            {
                HeaderText = BoneName,
                Name = BoneName,
                CellTemplate = new DataGridViewCheckBoxCell(),
                HeaderCell = new DataGridViewColumnHeaderCell() { },
            };
            dataGridView1.Columns.Add(column);

            if (activeFVIS.VisibilityAnimU != null) {
                activeFVIS.VisibilityAnimU.Names.Add(BoneName);
            }
            else {
                activeFVIS.VisibilityAnim.Names.Add(BoneName);
            }

        }

        public void RemoveBone(string BoneName)
        {
            dataGridView1.Columns.Remove(dataGridView1.Columns[BoneName]);

            if (activeFVIS.VisibilityAnimU != null) {
                activeFVIS.VisibilityAnimU.Names.Remove(BoneName);
            }
            else {
                activeFVIS.VisibilityAnim.Names.Remove(BoneName);
            }
        }

        public class BoneObject
        {
            public int Frame { get; set; }
            public bool IsVisible { get; set; }
            public string Name { get; set; }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            BoneVisListEditor editor = new BoneVisListEditor();
            editor.LoadBones(activeFVIS.BoneNames);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                activeFVIS.BoneNames = editor.GetNewBones();

                foreach (string bone in activeFVIS.BoneNames)
                {
                    bool HasBone = activeFVIS.Values.Any(x => x.Text == bone);
                    if (!HasBone)
                    {
                        BooleanKeyGroup group = new BooleanKeyGroup();
                        group.Text = bone;
                        group.Offset = 0;
                        group.Scale = 0;
                        group.Delta = 0;

                        for (int frame = 0; frame <= activeFVIS.FrameCount; frame++)
                        {
                            group.Keys.Add(new BooleanKeyFrame() { Frame = frame, InterType = InterpolationType.STEPBOOL, Visible = false });
                        }

                        activeFVIS.Values.Add(group);
                    }
                }

                //If a keygroup exists not in the bonename, we need to remove it as it was removed from the list
                activeFVIS.Values.RemoveAll(x => !activeFVIS.BoneNames.Contains(x.Text));

                UpdateBoneColumns(activeFVIS);
                UpdateFrameCells(activeFVIS);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
      
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!isLoaded)
                return;

            var row = dataGridView1.Rows[e.RowIndex];
            DataGridViewCheckBoxCell cell = row.Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;

            if (cell != null && cell.Value != null)
            {
                float Frame = e.RowIndex;

                string BoneName = dataGridView1.Columns[e.ColumnIndex].HeaderText;

                foreach (var value in activeFVIS.Values)
                {
                    if (value.Text == BoneName)
                    {
                        bool IsChecked = (bool)(Convert.ToBoolean(cell.Value) == true);
                        bool IsNotChecked = (bool)(Convert.ToBoolean(cell.Value) == false);

                        if (IsChecked)
                        {
                            value.SetBoolean(Frame, true);
                        }
                        if (IsNotChecked)
                        {
                            value.SetBoolean(Frame, false);
                        }

                        activeFVIS.IsEdited = true;
                    }
                }
            }
        }
    }
}
