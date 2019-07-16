namespace FirstPlugin.Forms
{
    partial class TextureLoader
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.stComboBox1 = new Toolbox.Library.Forms.STComboBox();
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.barSlider1 = new BarSlider.BarSlider();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.btnAdd = new Toolbox.Library.Forms.STButton();
            this.btnRemove = new Toolbox.Library.Forms.STButton();
            this.textureContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textureContextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // stComboBox1
            // 
            this.stComboBox1.BorderColor = System.Drawing.Color.Empty;
            this.stComboBox1.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.stComboBox1.ButtonColor = System.Drawing.Color.Empty;
            this.stComboBox1.FormattingEnabled = true;
            this.stComboBox1.Location = new System.Drawing.Point(3, 3);
            this.stComboBox1.Name = "stComboBox1";
            this.stComboBox1.ReadOnly = true;
            this.stComboBox1.Size = new System.Drawing.Size(158, 21);
            this.stComboBox1.TabIndex = 12;
            this.stComboBox1.SelectedIndexChanged += new System.EventHandler(this.stComboBox1_SelectedIndexChanged);
            this.stComboBox1.Click += new System.EventHandler(this.stComboBox1_Click);
            this.stComboBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.stComboBox1_KeyDown);
            this.stComboBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.stComboBox1_MouseDown);
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Location = new System.Drawing.Point(9, 30);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(872, 246);
            this.listViewCustom1.TabIndex = 13;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.SmallIcon;
            this.listViewCustom1.SelectedIndexChanged += new System.EventHandler(this.listViewCustom1_SelectedIndexChanged);
            this.listViewCustom1.DoubleClick += new System.EventHandler(this.textureListView_DoubleClick);
            this.listViewCustom1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.textureListView_MouseClick);
            // 
            // barSlider1
            // 
            this.barSlider1.ActiveEditColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.barSlider1.BarInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.barSlider1.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.barSlider1.BarPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.barSlider1.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.barSlider1.BorderRoundRectSize = new System.Drawing.Size(32, 32);
            this.barSlider1.DataType = null;
            this.barSlider1.DrawSemitransparentThumb = false;
            this.barSlider1.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider1.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider1.ElapsedPenColorMiddle = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            this.barSlider1.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.barSlider1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.barSlider1.ForeColor = System.Drawing.Color.White;
            this.barSlider1.IncrementAmount = 0.01F;
            this.barSlider1.InputName = null;
            this.barSlider1.LargeChange = 10F;
            this.barSlider1.Location = new System.Drawing.Point(240, 3);
            this.barSlider1.Maximum = 100F;
            this.barSlider1.Minimum = 0F;
            this.barSlider1.Name = "barSlider1";
            this.barSlider1.Precision = 0.01F;
            this.barSlider1.ScaleDivisions = 1;
            this.barSlider1.ScaleSubDivisions = 2;
            this.barSlider1.ShowDivisionsText = false;
            this.barSlider1.ShowSmallScale = false;
            this.barSlider1.Size = new System.Drawing.Size(174, 21);
            this.barSlider1.SmallChange = 1F;
            this.barSlider1.TabIndex = 1;
            this.barSlider1.Text = "barSlider1";
            this.barSlider1.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(121)))), ((int)(((byte)(180)))));
            this.barSlider1.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(121)))), ((int)(((byte)(180)))));
            this.barSlider1.ThumbRoundRectSize = new System.Drawing.Size(1, 1);
            this.barSlider1.ThumbSize = new System.Drawing.Size(1, 1);
            this.barSlider1.TickAdd = 0F;
            this.barSlider1.TickColor = System.Drawing.Color.White;
            this.barSlider1.TickDivide = 0F;
            this.barSlider1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.barSlider1.UseInterlapsedBar = true;
            this.barSlider1.Value = 70F;
            this.barSlider1.ValueChanged += new System.EventHandler(this.barSlider1_ValueChanged);
            this.barSlider1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.barSlider1_Scroll);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(171, 6);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(63, 13);
            this.stLabel1.TabIndex = 14;
            this.stLabel1.Text = "Thumb Size";
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(420, 1);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 15;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(501, 1);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 16;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // textureContextMenuStrip1
            // 
            this.textureContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.replaceToolStripMenuItem});
            this.textureContextMenuStrip1.Name = "textureContextMenuStrip1";
            this.textureContextMenuStrip1.Size = new System.Drawing.Size(116, 48);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.replaceToolStripMenuItem.Text = "Replace";
            this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
            // 
            // TextureLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.stLabel1);
            this.Controls.Add(this.barSlider1);
            this.Controls.Add(this.listViewCustom1);
            this.Controls.Add(this.stComboBox1);
            this.Name = "TextureLoader";
            this.Size = new System.Drawing.Size(884, 284);
            this.textureContextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Toolbox.Library.Forms.STComboBox stComboBox1;
        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private BarSlider.BarSlider barSlider1;
        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STButton btnAdd;
        private Toolbox.Library.Forms.STButton btnRemove;
        private System.Windows.Forms.ContextMenuStrip textureContextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
    }
}