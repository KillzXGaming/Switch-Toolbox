namespace FirstPlugin
{
    partial class AttributeEditor
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
            this.objectList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.formatCB = new Toolbox.Library.Forms.STComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.hintLabel = new System.Windows.Forms.Label();
            this.attributeCB = new Toolbox.Library.Forms.STComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // objectList
            // 
            this.objectList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.objectList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.objectList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.objectList.Dock = System.Windows.Forms.DockStyle.Left;
            this.objectList.ForeColor = System.Drawing.Color.White;
            this.objectList.Location = new System.Drawing.Point(0, 0);
            this.objectList.Name = "objectList";
            this.objectList.Size = new System.Drawing.Size(298, 353);
            this.objectList.TabIndex = 0;
            this.objectList.UseCompatibleStateImageBehavior = false;
            this.objectList.View = System.Windows.Forms.View.Details;
            this.objectList.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Shape";
            this.columnHeader1.Width = 129;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(12, 7);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(585, 56);
            this.panel1.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Batch Operations";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(142, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(201, 41);
            this.button1.TabIndex = 0;
            this.button1.Text = "Remove Selected Attribute";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(304, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Attribute";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(304, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Format";
            // 
            // formatCB
            // 
            this.formatCB.DropDownStyle = Toolbox.Library.Forms.STComboBox.STDropDownStyle;
            this.formatCB.FormattingEnabled = true;
            this.formatCB.Location = new System.Drawing.Point(374, 59);
            this.formatCB.Name = "formatCB";
            this.formatCB.Size = new System.Drawing.Size(194, 21);
            this.formatCB.TabIndex = 4;
            this.formatCB.SelectedIndexChanged += new System.EventHandler(this.formatCB_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.hintLabel);
            this.panel2.Controls.Add(this.attributeCB);
            this.panel2.Controls.Add(this.objectList);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.formatCB);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(12, 69);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(585, 353);
            this.panel2.TabIndex = 6;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // hintLabel
            // 
            this.hintLabel.AutoSize = true;
            this.hintLabel.Location = new System.Drawing.Point(307, 97);
            this.hintLabel.Name = "hintLabel";
            this.hintLabel.Size = new System.Drawing.Size(26, 13);
            this.hintLabel.TabIndex = 7;
            this.hintLabel.Text = "Hint";
            // 
            // attributeCB
            // 
            this.attributeCB.DropDownStyle = Toolbox.Library.Forms.STComboBox.STDropDownStyle;
            this.attributeCB.FormattingEnabled = true;
            this.attributeCB.Location = new System.Drawing.Point(374, 21);
            this.attributeCB.Name = "attributeCB";
            this.attributeCB.Size = new System.Drawing.Size(194, 21);
            this.attributeCB.TabIndex = 6;
            this.attributeCB.SelectedIndexChanged += new System.EventHandler(this.attributeCB_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(522, 430);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Ok";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(461, 128);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(107, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "Remove Attribute";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // AttributeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(609, 465);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AttributeEditor";
            this.Text = "AttributeEditor";
            this.Load += new System.EventHandler(this.AttributeEditor_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView objectList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Toolbox.Library.Forms.STComboBox formatCB;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label3;
        private Toolbox.Library.Forms.STComboBox attributeCB;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label hintLabel;
        private System.Windows.Forms.Button button3;
    }
}