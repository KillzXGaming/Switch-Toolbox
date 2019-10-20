namespace FirstPlugin.MuuntEditor
{
    partial class MuuntObjectList
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
            this.stComboBox1 = new Toolbox.Library.Forms.STComboBox();
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.SuspendLayout();
            // 
            // stComboBox1
            // 
            this.stComboBox1.BorderColor = System.Drawing.Color.Empty;
            this.stComboBox1.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.stComboBox1.ButtonColor = System.Drawing.Color.Empty;
            this.stComboBox1.FormattingEnabled = true;
            this.stComboBox1.IsReadOnly = false;
            this.stComboBox1.Location = new System.Drawing.Point(12, 12);
            this.stComboBox1.Name = "stComboBox1";
            this.stComboBox1.Size = new System.Drawing.Size(257, 21);
            this.stComboBox1.TabIndex = 0;
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.HideSelection = false;
            this.listViewCustom1.Location = new System.Drawing.Point(12, 39);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(257, 265);
            this.listViewCustom1.TabIndex = 1;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            // 
            // MuuntObjectList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 316);
            this.Controls.Add(this.listViewCustom1);
            this.Controls.Add(this.stComboBox1);
            this.Name = "MuuntObjectList";
            this.Text = "MuuntObjectList";
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STComboBox stComboBox1;
        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
    }
}