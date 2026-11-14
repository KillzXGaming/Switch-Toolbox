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
        private NumericUpDown keyFrameUD;

        public BoneVisualAnimEditor()
        {
            InitializeComponent();

            var bar = new FlowLayoutPanel() { Dock = DockStyle.Top, Height = 29, WrapContents = false };
            keyFrameUD = new NumericUpDown()
            {
                Width = 70,
                Maximum = 1000000,
                Margin = new Padding(3, 4, 3, 0),
                BackColor = FormThemes.BaseTheme.FormBackColor,
                ForeColor = FormThemes.BaseTheme.FormForeColor
            };
            var btnAddKey = new STButton() { Text = "Add key", Size = new Size(75, 23) };
            var btnRemoveKey = new STButton() { Text = "Remove key", Size = new Size(85, 23) };
            btnAddKey.Click += btnAddKey_Click;
            btnRemoveKey.Click += btnRemoveKey_Click;
            bar.Controls.Add(new Label()
            {
                Text = "Frame",
                AutoSize = true,
                Margin = new Padding(3, 7, 0, 0),
                ForeColor = FormThemes.BaseTheme.FormForeColor
            });
            bar.Controls.AddRange(new Control[] { keyFrameUD, btnAddKey, btnRemoveKey });

            Controls.Add(bar);
            dataGridView1.Dock = DockStyle.Fill;
            Controls.SetChildIndex(dataGridView1, 0);
            Controls.SetChildIndex(bar, 1);

            dataGridView1.CurrentCellDirtyStateChanged += (s, e) => {
                if (dataGridView1.IsCurrentCellDirty)
                    dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            dataGridView1.SelectionChanged += (s, e) => {
                int frame;
                if (dataGridView1.CurrentRow != null && int.TryParse(dataGridView1.CurrentRow.Cells["FrameColumn"].Value?.ToString(), out frame))
                    keyFrameUD.Value = Math.Min(frame, keyFrameUD.Maximum);
            };
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
            keyFrameUD.Maximum = Math.Max(vis.FrameCount, 1);
            keyFrameUD.Value = Math.Min(Math.Max((decimal)vis.Frame, 0), keyFrameUD.Maximum);

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
            bool wasLoaded = isLoaded;
            isLoaded = false;

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
            isLoaded = wasLoaded;
        }

        private bool IsKeyed(FVIS fvis, int Frame)
        {
            foreach (BooleanKeyGroup track in fvis.Values)
            {
                if (track.Keys.Any(k => k.Frame == Frame && k.IsKeyed))
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

            if (activeFVIS.VisibilityAnimU != null)
            {
                activeFVIS.VisibilityAnimU.Names.Add(BoneName);
            }
            else
            {
                activeFVIS.VisibilityAnim.Names.Add(BoneName);
            }

        }

        public void RemoveBone(string BoneName)
        {
            dataGridView1.Columns.Remove(dataGridView1.Columns[BoneName]);

            if (activeFVIS.VisibilityAnimU != null)
            {
                activeFVIS.VisibilityAnimU.Names.Remove(BoneName);
            }
            else
            {
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

                        //One key at frame 0 with the bone's current visibility
                        var names = activeFVIS.VisibilityAnimU != null ? activeFVIS.VisibilityAnimU.Names : activeFVIS.VisibilityAnim.Names;
                        int baseIndex = names.IndexOf(bone);
                        bool visible = baseIndex < 0 || activeFVIS.BaseValues == null || baseIndex >= activeFVIS.BaseValues.Length || activeFVIS.BaseValues[baseIndex];
                        group.Keys.Add(new BooleanKeyFrame() { Frame = 0, InterType = InterpolationType.STEPBOOL, Visible = visible, IsKeyed = true });

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
                float Frame;
                if (!float.TryParse(row.Cells["FrameColumn"].Value?.ToString(), out Frame))
                    return;

                string BoneName = dataGridView1.Columns[e.ColumnIndex].HeaderText;

                foreach (var value in activeFVIS.Values)
                {
                    if (value.Text == BoneName)
                    {
                        bool IsChecked = (bool)(Convert.ToBoolean(cell.Value) == true);
                        bool IsNotChecked = (bool)(Convert.ToBoolean(cell.Value) == false);

                        var key = value.GetKeyFrame(Frame);
                        key.IsKeyed = true;
                        key.InterType = InterpolationType.STEPBOOL;

                        if (IsChecked)
                        {
                            key.Visible = true;
                        }
                        if (IsNotChecked)
                        {
                            key.Visible = false;
                        }

                        activeFVIS.IsEdited = true;
                        RefreshColumn(e.ColumnIndex, value);
                    }
                }
            }
        }

        private void RefreshColumn(int columnIndex, BooleanKeyGroup track)
        {
            bool wasLoaded = isLoaded;
            isLoaded = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                float frame;
                if (float.TryParse(row.Cells["FrameColumn"].Value?.ToString(), out frame))
                    row.Cells[columnIndex].Value = track.GetValue(frame);
            }

            isLoaded = wasLoaded;
        }

        private void btnAddKey_Click(object sender, EventArgs e)
        {
            if (activeFVIS == null || activeFVIS.Values.Count == 0)
                return;

            float frame = (float)keyFrameUD.Value;

            //Key every bone at this frame with the value it already has there
            foreach (var track in activeFVIS.Values)
            {
                bool current = track.Keys.Count == 0 || track.GetValue(frame);
                var key = track.GetKeyFrame(frame);
                if (!key.IsKeyed)
                {
                    key.IsKeyed = true;
                    key.InterType = InterpolationType.STEPBOOL;
                    key.Visible = current;
                }
            }

            activeFVIS.IsEdited = true;
            UpdateFrameCells(activeFVIS);

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["FrameColumn"].Value?.ToString() == ((int)frame).ToString())
                {
                    dataGridView1.CurrentCell = row.Cells["FrameColumn"];
                    break;
                }
            }
        }

        private void btnRemoveKey_Click(object sender, EventArgs e)
        {
            if (activeFVIS == null)
                return;

            float frame = (float)keyFrameUD.Value;
            if (frame == 0)
            {
                MessageBox.Show("Frame 0 holds each bone's starting visibility. Change its boxes instead of removing it.");
                return;
            }

            int removed = 0;
            foreach (var track in activeFVIS.Values)
                removed += track.Keys.RemoveAll(k => k.Frame == frame);

            if (removed > 0)
            {
                activeFVIS.IsEdited = true;
                UpdateFrameCells(activeFVIS);
            }
        }
    }
}
