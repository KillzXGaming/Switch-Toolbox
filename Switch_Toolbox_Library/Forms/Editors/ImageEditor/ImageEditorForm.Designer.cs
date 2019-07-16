namespace Toolbox.Library.Forms
{
    partial class ImageEditorForm
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
            this.editorBase = new Toolbox.Library.Forms.ImageEditorBase();
            this.btnCancel = new Toolbox.Library.Forms.STButton();
            this.btnSave = new Toolbox.Library.Forms.STButton();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            this.contentContainer.SuspendLayout();
            this.stPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stPanel1);
            this.contentContainer.Controls.Add(this.btnSave);
            this.contentContainer.Controls.Add(this.btnCancel);
            this.contentContainer.Size = new System.Drawing.Size(543, 403);
            this.contentContainer.Controls.SetChildIndex(this.btnCancel, 0);
            this.contentContainer.Controls.SetChildIndex(this.btnSave, 0);
            this.contentContainer.Controls.SetChildIndex(this.stPanel1, 0);
            // 
            // editorBase
            // 
            this.editorBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorBase.HasBeenEdited = false;
            this.editorBase.Location = new System.Drawing.Point(0, 0);
            this.editorBase.Name = "editorBase";
            this.editorBase.Size = new System.Drawing.Size(543, 334);
            this.editorBase.TabIndex = 11;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(459, 371);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(378, 371);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // stPanel1
            // 
            this.stPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stPanel1.Controls.Add(this.editorBase);
            this.stPanel1.Location = new System.Drawing.Point(0, 31);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(543, 334);
            this.stPanel1.TabIndex = 14;
            // 
            // ImageEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 408);
            this.Name = "ImageEditorForm";
            this.Text = "ImageEditorForm";
            this.contentContainer.ResumeLayout(false);
            this.stPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public ImageEditorBase editorBase;
        private STButton btnSave;
        private STButton btnCancel;
        private STPanel stPanel1;
    }
}