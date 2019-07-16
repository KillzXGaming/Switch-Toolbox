namespace FirstPlugin.Forms
{
    partial class ShaderBinaryDisplay
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
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.stPanel2 = new Toolbox.Library.Forms.STPanel();
            this.textEditor1 = new Toolbox.Library.Forms.TextEditor();
            this.hexEditor1 = new Toolbox.Library.Forms.HexEditor();
            this.stPanel1.SuspendLayout();
            this.stPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.hexEditor1);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stPanel1.Location = new System.Drawing.Point(0, 315);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(414, 123);
            this.stPanel1.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 312);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(414, 3);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // stPanel2
            // 
            this.stPanel2.Controls.Add(this.textEditor1);
            this.stPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stPanel2.Location = new System.Drawing.Point(0, 0);
            this.stPanel2.Name = "stPanel2";
            this.stPanel2.Size = new System.Drawing.Size(414, 312);
            this.stPanel2.TabIndex = 2;
            // 
            // textEditor1
            // 
            this.textEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditor1.Location = new System.Drawing.Point(0, 0);
            this.textEditor1.Name = "textEditor1";
            this.textEditor1.Size = new System.Drawing.Size(414, 312);
            this.textEditor1.TabIndex = 0;
            // 
            // hexEditor1
            // 
            this.hexEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexEditor1.Location = new System.Drawing.Point(0, 0);
            this.hexEditor1.Name = "hexEditor1";
            this.hexEditor1.Size = new System.Drawing.Size(414, 123);
            this.hexEditor1.TabIndex = 0;
            // 
            // ShaderBinaryDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stPanel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.stPanel1);
            this.Name = "ShaderBinaryDisplay";
            this.Size = new System.Drawing.Size(414, 438);
            this.stPanel1.ResumeLayout(false);
            this.stPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STPanel stPanel1;
        private Toolbox.Library.Forms.HexEditor hexEditor1;
        private System.Windows.Forms.Splitter splitter1;
        private Toolbox.Library.Forms.STPanel stPanel2;
        private Toolbox.Library.Forms.TextEditor textEditor1;
    }
}
