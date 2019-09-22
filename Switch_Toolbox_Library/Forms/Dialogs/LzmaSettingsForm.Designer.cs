namespace Toolbox.Library.Forms
{
    partial class LzmaSettingsForm
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
            this.useMagicHeaderCB = new Toolbox.Library.Forms.STCheckBox();
            this.writePropertiesCB = new Toolbox.Library.Forms.STCheckBox();
            this.writeDecompSizeCB = new Toolbox.Library.Forms.STCheckBox();
            this.stButton1 = new Toolbox.Library.Forms.STButton();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.stButton1);
            this.contentContainer.Controls.Add(this.writeDecompSizeCB);
            this.contentContainer.Controls.Add(this.writePropertiesCB);
            this.contentContainer.Controls.Add(this.useMagicHeaderCB);
            this.contentContainer.Size = new System.Drawing.Size(283, 154);
            this.contentContainer.Controls.SetChildIndex(this.useMagicHeaderCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.writePropertiesCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.writeDecompSizeCB, 0);
            this.contentContainer.Controls.SetChildIndex(this.stButton1, 0);
            // 
            // useMagicHeaderCB
            // 
            this.useMagicHeaderCB.AutoSize = true;
            this.useMagicHeaderCB.Checked = true;
            this.useMagicHeaderCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useMagicHeaderCB.Location = new System.Drawing.Point(9, 43);
            this.useMagicHeaderCB.Name = "useMagicHeaderCB";
            this.useMagicHeaderCB.Size = new System.Drawing.Size(147, 17);
            this.useMagicHeaderCB.TabIndex = 11;
            this.useMagicHeaderCB.Text = "Use LZMA Magic Header";
            this.useMagicHeaderCB.UseVisualStyleBackColor = true;
            // 
            // writePropertiesCB
            // 
            this.writePropertiesCB.AutoSize = true;
            this.writePropertiesCB.Checked = true;
            this.writePropertiesCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.writePropertiesCB.Location = new System.Drawing.Point(9, 66);
            this.writePropertiesCB.Name = "writePropertiesCB";
            this.writePropertiesCB.Size = new System.Drawing.Size(101, 17);
            this.writePropertiesCB.TabIndex = 12;
            this.writePropertiesCB.Text = "Save Properties";
            this.writePropertiesCB.UseVisualStyleBackColor = true;
            // 
            // writeDecompSizeCB
            // 
            this.writeDecompSizeCB.AutoSize = true;
            this.writeDecompSizeCB.Checked = true;
            this.writeDecompSizeCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.writeDecompSizeCB.Location = new System.Drawing.Point(11, 89);
            this.writeDecompSizeCB.Name = "writeDecompSizeCB";
            this.writeDecompSizeCB.Size = new System.Drawing.Size(148, 17);
            this.writeDecompSizeCB.TabIndex = 13;
            this.writeDecompSizeCB.Text = "Save Decompressed Size";
            this.writeDecompSizeCB.UseVisualStyleBackColor = true;
            // 
            // stButton1
            // 
            this.stButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.stButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stButton1.Location = new System.Drawing.Point(199, 122);
            this.stButton1.Name = "stButton1";
            this.stButton1.Size = new System.Drawing.Size(75, 23);
            this.stButton1.TabIndex = 14;
            this.stButton1.Text = "Ok";
            this.stButton1.UseVisualStyleBackColor = false;
            // 
            // LzmaSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 159);
            this.Name = "LzmaSettingsForm";
            this.Text = "Lzma Settings";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private STCheckBox useMagicHeaderCB;
        private STCheckBox writeDecompSizeCB;
        private STCheckBox writePropertiesCB;
        private STButton stButton1;
    }
}