namespace Testing
{
	partial class TestingForm
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
			this.gL_ControlModern1 = new GL_Core.GL_ControlModern();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// gL_ControlModern1
			// 
			this.gL_ControlModern1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gL_ControlModern1.BackColor = System.Drawing.Color.Black;
			this.gL_ControlModern1.Location = new System.Drawing.Point(0, 35);
			this.gL_ControlModern1.MainDrawable = null;
			this.gL_ControlModern1.Name = "gL_ControlModern1";
			this.gL_ControlModern1.Size = new System.Drawing.Size(800, 467);
			this.gL_ControlModern1.Stereoscopy = false;
			this.gL_ControlModern1.TabIndex = 0;
			this.gL_ControlModern1.VSync = false;
			this.gL_ControlModern1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gL_ControlModern1_MouseClick);
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(12, 12);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(85, 17);
			this.checkBox1.TabIndex = 1;
			this.checkBox1.Text = "Stereoscopy";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// TestingForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 501);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.gL_ControlModern1);
			this.Name = "TestingForm";
			this.Text = "Test Window";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private GL_Core.GL_ControlModern gL_ControlModern1;
		private System.Windows.Forms.CheckBox checkBox1;
	}
}

