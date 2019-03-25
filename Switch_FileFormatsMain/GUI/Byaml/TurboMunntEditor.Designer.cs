namespace FirstPlugin.Forms
{
    partial class TurboMunntEditor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stPanel2 = new Switch_Toolbox.Library.Forms.STPanel();
            this.stPropertyGrid1 = new Switch_Toolbox.Library.Forms.STPropertyGrid();
            this.stPanel3 = new Switch_Toolbox.Library.Forms.STPanel();
            this.listViewCustom1 = new Switch_Toolbox.Library.Forms.ListViewCustom();
            this.objectCB = new Switch_Toolbox.Library.Forms.STComboBox();
            this.stButton1 = new Switch_Toolbox.Library.Forms.STButton();
            this.stButton2 = new Switch_Toolbox.Library.Forms.STButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.stPanel2.SuspendLayout();
            this.stPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.stPanel2);
            this.splitContainer1.Panel1.Controls.Add(this.splitter1);
            this.splitContainer1.Panel1.Controls.Add(this.stPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(780, 492);
            this.splitContainer1.SplitterDistance = 170;
            this.splitContainer1.TabIndex = 1;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.stButton2);
            this.stPanel1.Controls.Add(this.stButton1);
            this.stPanel1.Controls.Add(this.listViewCustom1);
            this.stPanel1.Controls.Add(this.stPanel3);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel1.Location = new System.Drawing.Point(0, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(170, 317);
            this.stPanel1.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 317);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(170, 3);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.stPropertyGrid1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(0, 320);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(170, 172);
            this.stPanel2.TabIndex = 2;
            // 
            // stPropertyGrid1
            // 
            this.stPropertyGrid1.AutoScroll = true;
            this.stPropertyGrid1.DisableHintDisplay = true;
            this.stPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPropertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.stPropertyGrid1.Name = "stPropertyGrid1";
            this.stPropertyGrid1.Size = new System.Drawing.Size(170, 172);
            this.stPropertyGrid1.TabIndex = 0;
            // 
            // stPanel3
            // 
            this.stPanel3.Controls.Add(this.objectCB);
            this.stPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.stPanel3.Location = new System.Drawing.Point(0, 0);
            this.stPanel3.Name = "stPanel3";
            this.stPanel3.Size = new System.Drawing.Size(170, 32);
            this.stPanel3.TabIndex = 0;
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Location = new System.Drawing.Point(3, 30);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(167, 250);
            this.listViewCustom1.TabIndex = 1;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            // 
            // objectCB
            // 
            this.objectCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectCB.BorderColor = System.Drawing.Color.Empty;
            this.objectCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.objectCB.ButtonColor = System.Drawing.Color.Empty;
            this.objectCB.FormattingEnabled = true;
            this.objectCB.Location = new System.Drawing.Point(3, 3);
            this.objectCB.Name = "objectCB";
            this.objectCB.ReadOnly = true;
            this.objectCB.Size = new System.Drawing.Size(164, 21);
            this.objectCB.TabIndex = 0;
            this.objectCB.SelectedIndexChanged += new System.EventHandler(this.objectCB_SelectedIndexChanged);
            // 
            // stButton1
            // 
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(3, 288);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(43, 23);
            this.stButton1.TabIndex = 1;
            this.stButton1.Text = "Add";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stButton2
            // 
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(52, 288);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(57, 23);
            this.stButton2.TabIndex = 2;
            this.stButton2.Text = "Remove";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // TurboMunntEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "TurboMunntEditor";
            this.Size = new System.Drawing.Size(780, 492);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.stPanel2.ResumeLayout(false);
            this.stPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel2;
        private Switch_Toolbox.Library.Forms.STPropertyGrid stPropertyGrid1;
        private System.Windows.Forms.Splitter splitter1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel1;
        private Switch_Toolbox.Library.Forms.ListViewCustom listViewCustom1;
        private Switch_Toolbox.Library.Forms.STPanel stPanel3;
        private Switch_Toolbox.Library.Forms.STComboBox objectCB;
        private Switch_Toolbox.Library.Forms.STButton stButton2;
        private Switch_Toolbox.Library.Forms.STButton stButton1;
    }
}
