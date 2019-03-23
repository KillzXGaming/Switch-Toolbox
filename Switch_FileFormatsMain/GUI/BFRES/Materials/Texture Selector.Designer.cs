namespace FirstPlugin
{
    partial class Texture_Selector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Texture_Selector));
            this.pictureBoxCustom1 = new Switch_Toolbox.Library.Forms.PictureBoxCustom();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.addTextureBtn = new System.Windows.Forms.Button();
            this.RemoveTextureBtn = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.stTextBox1 = new Switch_Toolbox.Library.Forms.STTextBox();
            this.stLabel1 = new Switch_Toolbox.Library.Forms.STLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxCustom1
            // 
            this.pictureBoxCustom1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxCustom1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxCustom1.BackgroundImage")));
            this.pictureBoxCustom1.Location = new System.Drawing.Point(252, 25);
            this.pictureBoxCustom1.Name = "pictureBoxCustom1";
            this.pictureBoxCustom1.Size = new System.Drawing.Size(408, 350);
            this.pictureBoxCustom1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCustom1.TabIndex = 0;
            this.pictureBoxCustom1.TabStop = false;
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.ForeColor = System.Drawing.Color.White;
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(5, 79);
            this.listView1.Name = "listView1";
            this.listView1.OwnerDraw = true;
            this.listView1.Size = new System.Drawing.Size(242, 296);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.listView1_DrawColumnHeader);
            this.listView1.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listView1_DrawItem);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Texture";
            this.columnHeader1.Width = 235;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(249, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Preview";
            // 
            // addTextureBtn
            // 
            this.addTextureBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addTextureBtn.ForeColor = System.Drawing.Color.White;
            this.addTextureBtn.Location = new System.Drawing.Point(4, 381);
            this.addTextureBtn.Name = "addTextureBtn";
            this.addTextureBtn.Size = new System.Drawing.Size(75, 23);
            this.addTextureBtn.TabIndex = 3;
            this.addTextureBtn.Text = "Add";
            this.addTextureBtn.UseVisualStyleBackColor = true;
            this.addTextureBtn.Click += new System.EventHandler(this.addTextureBtn_Click);
            // 
            // RemoveTextureBtn
            // 
            this.RemoveTextureBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RemoveTextureBtn.ForeColor = System.Drawing.Color.White;
            this.RemoveTextureBtn.Location = new System.Drawing.Point(171, 381);
            this.RemoveTextureBtn.Name = "RemoveTextureBtn";
            this.RemoveTextureBtn.Size = new System.Drawing.Size(75, 23);
            this.RemoveTextureBtn.TabIndex = 4;
            this.RemoveTextureBtn.Text = "Remove";
            this.RemoveTextureBtn.UseVisualStyleBackColor = true;
            this.RemoveTextureBtn.Click += new System.EventHandler(this.RemoveTextureBtn_Click);
            // 
            // button3
            // 
            this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(563, 381);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Save";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // stTextBox1
            // 
            this.stTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox1.Location = new System.Drawing.Point(5, 25);
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.Size = new System.Drawing.Size(242, 20);
            this.stTextBox1.TabIndex = 6;
            // 
            // stLabel1
            // 
            this.stLabel1.AutoSize = true;
            this.stLabel1.Location = new System.Drawing.Point(2, 9);
            this.stLabel1.Name = "stLabel1";
            this.stLabel1.Size = new System.Drawing.Size(77, 13);
            this.stLabel1.TabIndex = 7;
            this.stLabel1.Text = "Texture Name:";
            // 
            // Texture_Selector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(672, 408);
            this.Controls.Add(this.stLabel1);
            this.Controls.Add(this.stTextBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.RemoveTextureBtn);
            this.Controls.Add(this.addTextureBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.pictureBoxCustom1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Texture_Selector";
            this.Text = "Texture Selector";
            this.Load += new System.EventHandler(this.Texture_Selector_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCustom1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Switch_Toolbox.Library.Forms.PictureBoxCustom pictureBoxCustom1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button addTextureBtn;
        private System.Windows.Forms.Button RemoveTextureBtn;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private Switch_Toolbox.Library.Forms.STTextBox stTextBox1;
        private Switch_Toolbox.Library.Forms.STLabel stLabel1;
    }
}