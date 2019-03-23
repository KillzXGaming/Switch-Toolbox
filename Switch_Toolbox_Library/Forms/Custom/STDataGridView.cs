using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System;

namespace Switch_Toolbox.Library.Forms
{
    public class STDataGridView : DataGridView
    {
        DataGridViewCellStyle GridViewCellStyle;

        private Color BoxColor { get; set; }

        private Image TickTock => Properties.Resources.CheckMark;

        public STDataGridView()
        {
            InitializeComponent();

            this.BoxColor = FormThemes.BaseTheme.CheckBoxBackColor;

            if (FormThemes.BaseTheme.FormBackColor != Color.Empty)
            {
                BackgroundColor = FormThemes.BaseTheme.FormBackColor;
                ForeColor = FormThemes.BaseTheme.FormForeColor;
                ColumnHeadersDefaultCellStyle.BackColor = FormThemes.BaseTheme.FormBackColor;
                ColumnHeadersDefaultCellStyle.ForeColor = FormThemes.BaseTheme.FormForeColor;
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                RowHeadersDefaultCellStyle.BackColor = FormThemes.BaseTheme.FormBackColor;
                RowHeadersDefaultCellStyle.ForeColor = FormThemes.BaseTheme.FormBackColor;
            }
            if (FormThemes.BaseTheme.ConsoleEditorBackColor != Color.Empty)
            {
                GridColor = FormThemes.BaseTheme.ConsoleEditorBackColor;
            }


            EnableHeadersVisualStyles = false;


            GridViewCellStyle = new DataGridViewCellStyle()
            {
                BackColor = FormThemes.BaseTheme.FormBackColor,
                ForeColor = FormThemes.BaseTheme.FormForeColor
            };

            Refresh();
        }

        public void ApplyStyles()
        {
            foreach (DataGridViewColumn column in Columns)
                column.DefaultCellStyle = GridViewCellStyle;

            foreach (DataGridViewRow row in Rows)
                row.DefaultCellStyle = GridViewCellStyle;
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // STDataGridView
            // 
            this.BackgroundColor = Color.Gray;
            this.GridColor = Color.Black;

            this.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.STDataGridView_CellClick);
            this.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.STDataGridView_CellContentClick);
            this.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.STDataGridView_CellPainting);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        private void STDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                bool IsSelected = (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected;

                e.Handled = true;

                Rectangle rect = new Rectangle(e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - 1, e.CellBounds.Height - 1);

                if (IsSelected)
                    e.Graphics.FillRectangle(new SolidBrush(e.CellStyle.SelectionBackColor), rect);
                else
                    e.Graphics.FillRectangle(new SolidBrush(e.CellStyle.BackColor), rect);

                using (Pen pen = new Pen(GridColor))
                {
                    e.Graphics.DrawRectangle(pen, e.CellBounds.X - 1, e.CellBounds.Y - 1, e.CellBounds.Width, e.CellBounds.Height);
                }

                RadioButtonState state = RadioButtonState.CheckedDisabled;

                if (e.Value != null)
                {
                    var TextCell = Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewTextBoxCell;

                    if (TextCell != null)
                    {
                        // Render the cell text.

                        string cellValue = e.FormattedValue.ToString();

                        // Set the alignment settings. Unfortunately, there's no // straightforward way to get the cell style settings and // convert them to the text alignment values you need here.

                        StringFormat format = new StringFormat(); format.LineAlignment = StringAlignment.Center; format.Alignment = StringAlignment.Near;

                        using (Brush valueBrush = new SolidBrush(e.CellStyle.ForeColor))
                        {
                            e.Graphics.DrawString(cellValue, e.CellStyle.Font, valueBrush, rect, format);
                        }
                    }
                }


                try
                {
                    if (e.Value != null)
                    {
                        var Cell = Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;

                        if (Cell == null)
                            return;

                        bool IsChecked = (bool)(Convert.ToBoolean(Cell.Value) == true);
                        bool IsNotChecked = (bool)(Convert.ToBoolean(Cell.Value) == false);

                        if (IsChecked)
                            state = RadioButtonState.CheckedNormal;

                        var size = RadioButtonRenderer.GetGlyphSize(e.Graphics, state);

                        var location = new Point((e.CellBounds.Width - size.Width) / 2,
                                ((e.CellBounds.Height - size.Height) / 2) - 1);
                        location.Offset(e.CellBounds.Location);



                        if (IsChecked)
                        {
                            Color checkedColor = FormThemes.BaseTheme.CheckBoxEnabledBackColor;
                            e.Graphics.FillRectangle(new SolidBrush(checkedColor), new Rectangle(location.X, location.Y, 15, 15));
                            e.Graphics.DrawImage(this.TickTock, location.X, location.Y, 12, 12);
                        }
                        else if (IsNotChecked)
                        {
                            //   e.Graphics.FillRectangle(new SolidBrush(this.BoxColor), new Rectangle(location.X, location.Y, 15, 15));
                            e.Graphics.FillRectangle(new SolidBrush(this.BoxColor), new Rectangle(location.X, location.Y, 15, 15));
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    STErrorDialog.Show("Failed to paint STDataGridView!", "STDataGridView", ex.ToString());
                }

         
            }

            return;

          /*      using (Pen gridLinePen = new Pen(gridBrush))
            {
                // Erase the cell.
                e.Graphics.FillRectangle(backColorBrush, e.CellBounds);

                // Draw the grid lines (only the right and bottom lines;
                // DataGridView takes care of the others).
                e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                    e.CellBounds.Bottom - 1, e.CellBounds.Right - 1,
                    e.CellBounds.Bottom - 1);
                e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1,
                    e.CellBounds.Top, e.CellBounds.Right - 1,
                    e.CellBounds.Bottom);


                // Draw the text content of the cell, ignoring alignment.
                if (e.Value != null)
                {
                    e.Graphics.DrawString((String)e.Value, e.CellStyle.Font,
                        Brushes.Crimson, e.CellBounds.X + 2,
                        e.CellBounds.Y + 2, StringFormat.GenericDefault);
                }
                e.Handled = true;
                */

                /*     e.Paint(e.CellBounds, DataGridViewPaintParts.All & DataGridViewPaintParts.ContentForeground);

                     RadioButtonState state = RadioButtonState.CheckedDisabled;

                     var Cell = Rows[e.RowIndex].Cells[e.ColumnIndex];

                     DataGridViewCheckBoxCell chkchecking = Cell as DataGridViewCheckBoxCell;

                     bool IsChecked = (bool)(Convert.ToBoolean(chkchecking.Value) == true);

                     if (IsChecked)
                         state = RadioButtonState.CheckedNormal;

                     var size = RadioButtonRenderer.GetGlyphSize(e.Graphics, state);
                     var location = new Point((e.CellBounds.Width - size.Width) / 2, (e.CellBounds.Height - size.Height) / 2);

                     //Draw cell
                     e.Graphics.FillRectangle(new SolidBrush(BackgroundColor), e.CellBounds);

                     if (IsChecked)
                     {
                         e.Graphics.DrawImage(this.TickTock, location.X, location.Y, 12, 12);
                     }
                     else
                     {
                         //   e.Graphics.FillRectangle(new SolidBrush(this.BoxColor), new Rectangle(location.X, location.Y, 15, 15));
                         e.Graphics.FillRectangle(new SolidBrush(this.BoxColor), new Rectangle(location.X, location.Y, 15, 15));
                     }*/

        }

        /// <summary>
        /// Sets the size of the layout to 16x;16x
        /// </summary>
        /// <param name="pb"></param>
        private void Set16(PictureBox pb)
        {
            pb.Size = new Size(16, 16);
        }
        /// <summary>
        /// Sets the size of the layout to 18x;18x
        /// </summary>
        /// <param name="pb"></param>
        private void Set18(PictureBox pb)
        {
            pb.Size = new Size(18, 18);
        }

        public bool IsBooleanCell(DataGridViewCell cell)
        {
            var Cell = CurrentCell as DataGridViewCheckBoxCell;

            return (Cell != null);
        }

        private void STDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (CurrentCell.Value != null)
            {
                if (!IsBooleanCell(CurrentCell))
                    return;

                var Cell = CurrentCell as DataGridViewCheckBoxCell;

                bool IsChecked = (bool)(Convert.ToBoolean(Cell.Value) == true);
                bool IsNotChecked = (bool)(Convert.ToBoolean(Cell.Value) == false);

                if (IsChecked)
                {
                    CurrentCell.Value = false;
                }
                else if (IsNotChecked)
                {
                    CurrentCell.Value = true;
                }

                foreach (DataGridViewCell cell in SelectedCells)
                {
                    if (IsBooleanCell(cell) && !cell.ReadOnly)
                        cell.Value = CurrentCell.Value;
                }

                this.Invalidate();

                /*   foreach (Binding data in DataBindings)
                   {
                       //   data.WriteValue();
                   }*/
            }
        }

        private void STDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}
