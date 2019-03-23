namespace Switch_Toolbox.Library.Forms
{
    partial class STPropertyGrid
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
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                | System.Windows.Forms.AnchorStyles.Left)
                                | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.CategoryForeColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormForeColor;
            this.propertyGrid1.CategorySplitterColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.MDIChildBorderColor;
            this.propertyGrid1.CommandsActiveLinkColor = System.Drawing.Color.Red;
            this.propertyGrid1.CommandsBorderColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.MDIParentBackColor;
            this.propertyGrid1.DisabledItemForeColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.DisabledItemColor;
            this.propertyGrid1.HelpBackColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
            this.propertyGrid1.HelpBorderColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
            this.propertyGrid1.HelpForeColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormForeColor;
            this.propertyGrid1.LineColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.SelectedItemWithFocusForeColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormContextMenuSelectColor;
            this.propertyGrid1.Size = new System.Drawing.Size(613, 532);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.ViewBackColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
            this.propertyGrid1.ViewBorderColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormBackColor;
            this.propertyGrid1.ViewForeColor = Switch_Toolbox.Library.Forms.FormThemes.BaseTheme.FormForeColor;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // STPropertyGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.propertyGrid1);
            this.Name = "STPropertyGrid";
            this.Size = new System.Drawing.Size(613, 532);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}
