namespace FirstPlugin.Forms
{
    partial class TexPatternInfoEditor
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
            this.listViewCustom1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAdd = new Switch_Toolbox.Library.Forms.STButton();
            this.btnRemove = new Switch_Toolbox.Library.Forms.STButton();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            this.stLabel2 = new Switch_Toolbox.Library.Forms.STLabel();
            this.nameTB = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stButton1 = new Switch_Toolbox.Library.Forms.STButton();
            this.stButton2 = new Switch_Toolbox.Library.Forms.STButton();
            this.stCheckBox1 = new Switch_Toolbox.Library.Forms.STCheckBox();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stCheckBox1);
            this.contentContainer.Controls.Add(this.stButton2);
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.nameTB);
            this.contentContainer.Controls.Add(this.stLabel2);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.btnRemove);
            this.contentContainer.Controls.Add(this.btnAdd);
            this.contentContainer.Controls.Add(this.listViewCustom1);
            this.contentContainer.Size = new System.Drawing.Size(427, 229);
            this.contentContainer.Controls.SetChildIndex(this.listViewCustom1, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnAdd, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnRemove, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel2, 0);
            this.contentContainer.Controls.SetChildIndex(this.nameTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton2, 0);
            this.contentContainer.Controls.SetChildIndex(this.stCheckBox1, 0);
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewCustom1.FullRowSelect = true;
            this.listViewCustom1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewCustom1.Location = new System.Drawing.Point(9, 77);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(183, 143);
            this.listViewCustom1.TabIndex = 11;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            this.listViewCustom1.SelectedIndexChanged += new System.EventHandler(this.listViewCustom1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 173;
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(9, 48);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(57, 23);
            this.btnAdd.TabIndex = 12;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(72, 48);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(64, 23);
            this.btnRemove.TabIndex = 13;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(9, 32);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(53, 13);
            this.stLabel1.TabIndex = 14;
            this.stLabel1.Text = "Samplers:";
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(198, 77);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(38, 13);
            this.stLabel2.TabIndex = 15;
            this.stLabel2.Text = "Name:";
            // 
            // nameTB
            // 
            this.nameTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameTB.Location = new System.Drawing.Point(257, 75);
            this.nameTB.Name = "nameTB";
            this.nameTB.Size = new System.Drawing.Size(165, 20);
            this.nameTB.TabIndex = 16;
            this.nameTB.TextChanged += new System.EventHandler(this.nameTB_TextChanged);
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(343, 197);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 19;
            this.stButton1.Text = "Cancel";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stButton2
            // 
            this.stButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(262, 197);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(75, 23);
            this.stButton2.TabIndex = 20;
            this.stButton2.Text = "Ok";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // stCheckBox1
            // 
            this.stCheckBox1.AutoSize = true;
            this.stCheckBox1.Location = new System.Drawing.Point(257, 112);
            this.stCheckBox1.Name = "stCheckBox1";
            this.stCheckBox1.Size = new System.Drawing.Size(79, 17);
            this.stCheckBox1.TabIndex = 21;
            this.stCheckBox1.Text = "Is Constant";
            this.stCheckBox1.UseVisualStyleBackColor = true;
            this.stCheckBox1.CheckedChanged += new System.EventHandler(this.stCheckBox1_CheckedChanged);
            // 
            // TexPatternInfoEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 234);
            this.Name = "TexPatternInfoEditor";
            this.Text = "TexPatternMaterialEditor";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Switch_Toolbox.Library.Forms.STTextBox nameTB;
        private Switch_Toolbox.Library.Forms.STLabel stLabel2;
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
        private Switch_Toolbox.Library.Forms.STButton btnRemove;
        private Switch_Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private Switch_Toolbox.Library.Forms.STButton btnAdd;
        private Switch_Toolbox.Library.Forms.STButton stButton2;
        private Switch_Toolbox.Library.Forms.STButton stButton1;
        private Switch_Toolbox.Library.Forms.STCheckBox stCheckBox1;
    }
}