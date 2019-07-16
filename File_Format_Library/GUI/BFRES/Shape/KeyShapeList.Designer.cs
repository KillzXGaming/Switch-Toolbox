namespace FirstPlugin.Forms
{
    partial class KeyShapeList
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
            this.btnRemoveKeyShape = new Toolbox.Library.Forms.STButton();
            this.btnAddKeyShape = new Toolbox.Library.Forms.STButton();
            this.keyShapeListView = new Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnMoveKeyShapeDown = new Toolbox.Library.Forms.STButton();
            this.btnMoveKeyShapeUp = new Toolbox.Library.Forms.STButton();
            this.SuspendLayout();
            // 
            // btnRemoveKeyShape
            // 
            this.btnRemoveKeyShape.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveKeyShape.Location = new System.Drawing.Point(84, 3);
            this.btnRemoveKeyShape.Name = "btnRemoveKeyShape";
            this.btnRemoveKeyShape.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveKeyShape.TabIndex = 31;
            this.btnRemoveKeyShape.Text = "Remove";
            this.btnRemoveKeyShape.UseVisualStyleBackColor = false;
            // 
            // btnAddKeyShape
            // 
            this.btnAddKeyShape.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddKeyShape.Location = new System.Drawing.Point(3, 3);
            this.btnAddKeyShape.Name = "btnAddKeyShape";
            this.btnAddKeyShape.Size = new System.Drawing.Size(75, 23);
            this.btnAddKeyShape.TabIndex = 30;
            this.btnAddKeyShape.Text = "Add";
            this.btnAddKeyShape.UseVisualStyleBackColor = false;
            // 
            // keyShapeListView
            // 
            this.keyShapeListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.keyShapeListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.keyShapeListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.keyShapeListView.Location = new System.Drawing.Point(3, 32);
            this.keyShapeListView.Name = "keyShapeListView";
            this.keyShapeListView.OwnerDraw = true;
            this.keyShapeListView.Size = new System.Drawing.Size(351, 144);
            this.keyShapeListView.TabIndex = 27;
            this.keyShapeListView.UseCompatibleStateImageBehavior = false;
            this.keyShapeListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 290;
            // 
            // btnMoveKeyShapeDown
            // 
            this.btnMoveKeyShapeDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveKeyShapeDown.Enabled = false;
            this.btnMoveKeyShapeDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveKeyShapeDown.Location = new System.Drawing.Point(360, 62);
            this.btnMoveKeyShapeDown.Name = "btnMoveKeyShapeDown";
            this.btnMoveKeyShapeDown.Size = new System.Drawing.Size(32, 24);
            this.btnMoveKeyShapeDown.TabIndex = 29;
            this.btnMoveKeyShapeDown.Text = "▼";
            this.btnMoveKeyShapeDown.UseVisualStyleBackColor = true;
            // 
            // btnMoveKeyShapeUp
            // 
            this.btnMoveKeyShapeUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveKeyShapeUp.Enabled = false;
            this.btnMoveKeyShapeUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveKeyShapeUp.Location = new System.Drawing.Point(360, 32);
            this.btnMoveKeyShapeUp.Name = "btnMoveKeyShapeUp";
            this.btnMoveKeyShapeUp.Size = new System.Drawing.Size(32, 24);
            this.btnMoveKeyShapeUp.TabIndex = 28;
            this.btnMoveKeyShapeUp.Text = "▲";
            this.btnMoveKeyShapeUp.UseVisualStyleBackColor = true;
            // 
            // KeyShapeList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnRemoveKeyShape);
            this.Controls.Add(this.btnAddKeyShape);
            this.Controls.Add(this.keyShapeListView);
            this.Controls.Add(this.btnMoveKeyShapeDown);
            this.Controls.Add(this.btnMoveKeyShapeUp);
            this.Name = "KeyShapeList";
            this.Size = new System.Drawing.Size(400, 189);
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.STButton btnRemoveKeyShape;
        private Toolbox.Library.Forms.STButton btnAddKeyShape;
        private Toolbox.Library.Forms.ListViewCustom keyShapeListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private Toolbox.Library.Forms.STButton btnMoveKeyShapeDown;
        private Toolbox.Library.Forms.STButton btnMoveKeyShapeUp;
    }
}
