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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestingForm));
			this.button1 = new System.Windows.Forms.Button();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.gL_ControlLegacy1 = new GL_EditorFramework.GL_Core.GL_ControlLegacy();
			this.gL_ControlModern1 = new GL_EditorFramework.GL_Core.GL_ControlModern();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(12, 208);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 3;
			this.button1.Text = "Add Object";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// listBox1
			// 
			this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new System.Drawing.Point(13, 238);
			this.listBox1.Name = "listBox1";
			this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBox1.Size = new System.Drawing.Size(278, 498);
			this.listBox1.TabIndex = 4;
			// 
			// gL_ControlLegacy1
			// 
			this.gL_ControlLegacy1.ActiveCamera = null;
			this.gL_ControlLegacy1.BackColor = System.Drawing.Color.Black;
			this.gL_ControlLegacy1.CameraDistance = -10F;
			this.gL_ControlLegacy1.CameraTarget = ((OpenTK.Vector3)(resources.GetObject("gL_ControlLegacy1.CameraTarget")));
			this.gL_ControlLegacy1.CamRotX = 0F;
			this.gL_ControlLegacy1.CamRotY = 0F;
			this.gL_ControlLegacy1.DragStartPos = new System.Drawing.Point(0, 0);
			this.gL_ControlLegacy1.Fov = 0.7853982F;
			this.gL_ControlLegacy1.Location = new System.Drawing.Point(13, 20);
			this.gL_ControlLegacy1.MainDrawable = null;
			this.gL_ControlLegacy1.Name = "gL_ControlLegacy1";
			this.gL_ControlLegacy1.ShowOrientationCube = false;
			this.gL_ControlLegacy1.Size = new System.Drawing.Size(278, 182);
			this.gL_ControlLegacy1.Stereoscopy = true;
			this.gL_ControlLegacy1.TabIndex = 2;
			this.gL_ControlLegacy1.VSync = false;
			this.gL_ControlLegacy1.ZFar = 1000F;
			this.gL_ControlLegacy1.ZNear = 0.01F;
			// 
			// gL_ControlModern1
			// 
			this.gL_ControlModern1.ActiveCamera = null;
			this.gL_ControlModern1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gL_ControlModern1.BackColor = System.Drawing.Color.Black;
			this.gL_ControlModern1.CameraDistance = -10F;
			this.gL_ControlModern1.CameraTarget = ((OpenTK.Vector3)(resources.GetObject("gL_ControlModern1.CameraTarget")));
			this.gL_ControlModern1.CamRotX = 0F;
			this.gL_ControlModern1.CamRotY = 0F;
			this.gL_ControlModern1.CurrentShader = null;
			this.gL_ControlModern1.DragStartPos = new System.Drawing.Point(0, 0);
			this.gL_ControlModern1.Fov = 0.7853982F;
			this.gL_ControlModern1.GradientBackground = true;
			this.gL_ControlModern1.Location = new System.Drawing.Point(297, 20);
			this.gL_ControlModern1.MainDrawable = null;
			this.gL_ControlModern1.Name = "gL_ControlModern1";
			this.gL_ControlModern1.ShowOrientationCube = true;
			this.gL_ControlModern1.Size = new System.Drawing.Size(689, 715);
			this.gL_ControlModern1.Stereoscopy = false;
			this.gL_ControlModern1.TabIndex = 1;
			this.gL_ControlModern1.VSync = false;
			this.gL_ControlModern1.ZFar = 1000F;
			this.gL_ControlModern1.ZNear = 0.01F;
			// 
			// TestingForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(998, 747);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.gL_ControlLegacy1);
			this.Controls.Add(this.gL_ControlModern1);
			this.Name = "TestingForm";
			this.Text = "Testing";
			this.ResumeLayout(false);

		}

		#endregion
		private GL_EditorFramework.GL_Core.GL_ControlModern gL_ControlModern1;
		private GL_EditorFramework.GL_Core.GL_ControlLegacy gL_ControlLegacy1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ListBox listBox1;
	}
}

