namespace FirstPlugin.Forms
{
    partial class ShaderParamEditor
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
            this.stFlowLayoutPanel1 = new Switch_Toolbox.Library.Forms.STFlowLayoutPanel();
            this.stButton1 = new Switch_Toolbox.Library.Forms.STButton();
            this.stButton2 = new Switch_Toolbox.Library.Forms.STButton();
            this.SuspendLayout();
            // 
            // stFlowLayoutPanel1
            // 
            this.stFlowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stFlowLayoutPanel1.AutoScroll = true;
            this.stFlowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.stFlowLayoutPanel1.Location = new System.Drawing.Point(3, 29);
            this.stFlowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.stFlowLayoutPanel1.Name = "stFlowLayoutPanel1";
            this.stFlowLayoutPanel1.Size = new System.Drawing.Size(413, 578);
            this.stFlowLayoutPanel1.TabIndex = 0;
            this.stFlowLayoutPanel1.WrapContents = false;
            // 
            // stButton1
            // 
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(3, 3);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 1;
            this.stButton1.Text = "Add";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // stButton2
            // 
            this.stButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton2.Location = new System.Drawing.Point(84, 3);
            this.stButton2.Name = "stButton2";
            this.stButton2.Size = new System.Drawing.Size(75, 23);
            this.stButton2.TabIndex = 2;
            this.stButton2.Text = "Remove";
            this.stButton2.UseVisualStyleBackColor = false;
            // 
            // ShaderParamEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stButton2);
            this.Controls.Add(this.stButton1);
            this.Controls.Add(this.stFlowLayoutPanel1);
            this.Name = "ShaderParamEditor";
            this.Size = new System.Drawing.Size(420, 620);
            this.ResumeLayout(false);

        }

        #endregion

        private Switch_Toolbox.Library.Forms.STFlowLayoutPanel stFlowLayoutPanel1;
        private Switch_Toolbox.Library.Forms.STButton stButton1;
        private Switch_Toolbox.Library.Forms.STButton stButton2;
    }
}
