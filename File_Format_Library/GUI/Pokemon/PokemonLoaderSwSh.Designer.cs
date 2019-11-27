namespace FirstPlugin.Forms
{
    partial class PokemonLoaderSwShForm
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
            this.SuspendLayout();
            // 
            // listViewCustom1
            // 
            this.listViewCustom1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewCustom1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewCustom1.HideSelection = false;
            this.listViewCustom1.Location = new System.Drawing.Point(0, 0);
            this.listViewCustom1.Name = "listViewCustom1";
            this.listViewCustom1.OwnerDraw = true;
            this.listViewCustom1.Size = new System.Drawing.Size(800, 450);
            this.listViewCustom1.TabIndex = 0;
            this.listViewCustom1.UseCompatibleStateImageBehavior = false;
            this.listViewCustom1.DoubleClick += new System.EventHandler(this.listViewCustom1_DoubleClick);
            // 
            // PokemonLoaderSwShForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.contentContainer.Controls.Add(this.listViewCustom1);
            this.Name = "PokemonLoaderSwShForm";
            this.Text = "PokemonLoaderSwSh";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PokemonLoaderSwShForm_FormClosing);
            this.Load += new System.EventHandler(this.PokemonLoaderSwShForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Toolbox.Library.Forms.ListViewCustom listViewCustom1;
    }
}