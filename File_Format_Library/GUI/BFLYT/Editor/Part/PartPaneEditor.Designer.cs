namespace LayoutBXLYT
{
    partial class PartPaneEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.stTextBox1 = new Toolbox.Library.Forms.STTextBox();
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.stLabel2 = new Toolbox.Library.Forms.STLabel();
            this.SuspendLayout();
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(14, 24);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(48, 13);
            this.stLabel1.TabIndex = 0;
            this.stLabel1.Text = "Part File:";
            // 
            // stTextBox1
            // 
            this.stTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox1.Location = new System.Drawing.Point(78, 22);
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.Size = new System.Drawing.Size(186, 20);
            this.stTextBox1.TabIndex = 1;
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Location = new System.Drawing.Point(3, 85);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(312, 231);
            this.listViewCustom1.TabIndex = 2;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            // 
            // stLabel2
            // 
            this.stLabel2.AutoSize = true;
            this.stLabel2.Location = new System.Drawing.Point(14, 69);
            this.stLabel2.Name = "stLabel2";
            this.stLabel2.Size = new System.Drawing.Size(57, 13);
            this.stLabel2.TabIndex = 3;
            this.stLabel2.Text = "Properties:";
            // 
            // PartPaneEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stLabel2);
            this.Controls.Add(this.listViewCustom1);
            this.Controls.Add(this.stTextBox1);
            this.Controls.Add(this.stLabel1);
            this.Name = "PartPaneEditor";
            this.Size = new System.Drawing.Size(318, 319);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Toolbox.Library.Forms.STLabel stLabel1;
        private Toolbox.Library.Forms.STTextBox stTextBox1;
        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private Toolbox.Library.Forms.STLabel stLabel2;
    }
}
