namespace Toolbox.Library.Forms
{
    partial class SearchNodeForm
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
            this.listViewCustom1 = new Toolbox.Library.Forms.ListViewCustom();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.searchTB = new Toolbox.Library.Forms.STTextBox();
            this.chkMatchCase = new Toolbox.Library.Forms.STCheckBox();
            this.chkSearchSubNodes = new Toolbox.Library.Forms.STCheckBox();
            this.searchBtn = new Toolbox.Library.Forms.STButton();
            this.stLabel1 = new Toolbox.Library.Forms.STLabel();
            this.chkAutoSearch = new Toolbox.Library.Forms.STCheckBox();
            this.listViewModeCB = new Toolbox.Library.Forms.STComboBox();
            this.contentContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Controls.Add(this.listViewModeCB);
            this.contentContainer.Controls.Add(this.chkAutoSearch);
            this.contentContainer.Controls.Add(this.stLabel1);
            this.contentContainer.Controls.Add(this.searchBtn);
            this.contentContainer.Controls.Add(this.chkSearchSubNodes);
            this.contentContainer.Controls.Add(this.chkMatchCase);
            this.contentContainer.Controls.Add(this.searchTB);
            this.contentContainer.Controls.Add(this.listViewCustom1);
            this.contentContainer.Size = new System.Drawing.Size(402, 393);
            this.contentContainer.Controls.SetChildIndex(this.listViewCustom1, 0);
            this.contentContainer.Controls.SetChildIndex(this.searchTB, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkMatchCase, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkSearchSubNodes, 0);
            this.contentContainer.Controls.SetChildIndex(this.searchBtn, 0);
            this.contentContainer.Controls.SetChildIndex(this.stLabel1, 0);
            this.contentContainer.Controls.SetChildIndex(this.chkAutoSearch, 0);
            this.contentContainer.Controls.SetChildIndex(this.listViewModeCB, 0);
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewCustom1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewCustom1.Location = new System.Drawing.Point(5, 84);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(397, 279);
            this.listViewCustom1.TabIndex = 11;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.View = System.Windows.Forms.View.Details;
            this.listViewCustom1.DoubleClick += new System.EventHandler(this.listViewCustom1_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Node";
            this.columnHeader1.Width = 304;
            // 
            // searchTB
            // 
            this.searchTB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchTB.Location = new System.Drawing.Point(49, 31);
            this.searchTB.Name = "searchTB";
            this.searchTB.Size = new System.Drawing.Size(248, 20);
            this.searchTB.TabIndex = 12;
            this.searchTB.TextChanged += new System.EventHandler(this.searchTB_TextChanged);
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.AutoSize = true;
            this.chkMatchCase.Location = new System.Drawing.Point(9, 59);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(86, 17);
            this.chkMatchCase.TabIndex = 13;
            this.chkMatchCase.Text = "Match Case:";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            this.chkMatchCase.CheckedChanged += new System.EventHandler(this.chkMatchCase_CheckedChanged);
            // 
            // chkSearchSubNodes
            // 
            this.chkSearchSubNodes.AutoSize = true;
            this.chkSearchSubNodes.Location = new System.Drawing.Point(196, 59);
            this.chkSearchSubNodes.Name = "chkSearchSubNodes";
            this.chkSearchSubNodes.Size = new System.Drawing.Size(111, 17);
            this.chkSearchSubNodes.TabIndex = 14;
            this.chkSearchSubNodes.Text = "Search Subnodes";
            this.chkSearchSubNodes.UseVisualStyleBackColor = true;
            // 
            // searchBtn
            // 
            this.searchBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchBtn.Location = new System.Drawing.Point(318, 55);
            this.searchBtn.Name = "searchBtn";
            this.searchBtn.Size = new System.Drawing.Size(75, 23);
            this.searchBtn.TabIndex = 15;
            this.searchBtn.Text = "Search";
            this.searchBtn.UseVisualStyleBackColor = false;
            this.searchBtn.Click += new System.EventHandler(this.searchBtn_Click);
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(9, 33);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(38, 13);
            this.stLabel1.TabIndex = 16;
            this.stLabel1.Text = "Name:";
            // 
            // chkAutoSearch
            // 
            this.chkAutoSearch.AutoSize = true;
            this.chkAutoSearch.Location = new System.Drawing.Point(101, 59);
            this.chkAutoSearch.Name = "chkAutoSearch";
            this.chkAutoSearch.Size = new System.Drawing.Size(85, 17);
            this.chkAutoSearch.TabIndex = 17;
            this.chkAutoSearch.Text = "Auto Search";
            this.chkAutoSearch.UseVisualStyleBackColor = true;
            // 
            // listViewModeCB
            // 
            this.listViewModeCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewModeCB.BorderColor = System.Drawing.Color.Empty;
            this.listViewModeCB.BorderStyle = System.Windows.Forms.ButtonBorderStyle.Solid;
            this.listViewModeCB.ButtonColor = System.Drawing.Color.Empty;
            this.listViewModeCB.FormattingEnabled = true;
            this.listViewModeCB.Location = new System.Drawing.Point(5, 369);
            this.listViewModeCB.Name = "listViewModeCB";
            this.listViewModeCB.ReadOnly = true;
            this.listViewModeCB.Size = new System.Drawing.Size(143, 21);
            this.listViewModeCB.TabIndex = 18;
            this.listViewModeCB.SelectedIndexChanged += new System.EventHandler(this.listViewModeCB_SelectedIndexChanged);
            // 
            // SearchNodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 398);
            this.Name = "SearchNodeForm";
            this.Text = "Search Nodes";
            this.contentContainer.ResumeLayout(false);
            this.contentContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ListViewCustom listViewCustom1;
        private STTextBox searchTB;
        private STLabel stLabel1;
        private STButton searchBtn;
        private STCheckBox chkSearchSubNodes;
        private STCheckBox chkMatchCase;
        private STCheckBox chkAutoSearch;
        private STComboBox listViewModeCB;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}