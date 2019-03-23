using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    public partial class STExceptionDialog : Form
    {
        #region designer

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(STExceptionDialog));
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnDetails = new System.Windows.Forms.Button();
            this.btnQuit = new System.Windows.Forms.Button();
            this.detailsBox = new System.Windows.Forms.RichTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.errorMessageLabel = new System.Windows.Forms.RichTextBox();
            this.stPanel1 = new Switch_Toolbox.Library.Forms.STPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.stPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnContinue
            // 
            this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnContinue.Location = new System.Drawing.Point(203, 102);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(87, 23);
            this.btnContinue.TabIndex = 0;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = true;
            // 
            // btnDetails
            // 
            this.btnDetails.Location = new System.Drawing.Point(12, 102);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(93, 23);
            this.btnDetails.TabIndex = 1;
            this.btnDetails.Text = "Details";
            this.btnDetails.UseVisualStyleBackColor = true;
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // btnQuit
            // 
            this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnQuit.Location = new System.Drawing.Point(291, 102);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(87, 23);
            this.btnQuit.TabIndex = 2;
            this.btnQuit.Text = "Quit";
            this.btnQuit.UseVisualStyleBackColor = true;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // detailsBox
            // 
            this.detailsBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.detailsBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.detailsBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsBox.ForeColor = System.Drawing.Color.White;
            this.detailsBox.Location = new System.Drawing.Point(0, 0);
            this.detailsBox.Name = "detailsBox";
            this.detailsBox.Size = new System.Drawing.Size(386, 219);
            this.detailsBox.TabIndex = 3;
            this.detailsBox.Text = "";
            this.detailsBox.TextChanged += new System.EventHandler(this.detailsBox_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // errorMessageLabel
            // 
            this.errorMessageLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.errorMessageLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.errorMessageLabel.ForeColor = System.Drawing.Color.White;
            this.errorMessageLabel.Location = new System.Drawing.Point(67, 12);
            this.errorMessageLabel.Name = "errorMessageLabel";
            this.errorMessageLabel.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.errorMessageLabel.Size = new System.Drawing.Size(289, 73);
            this.errorMessageLabel.TabIndex = 5;
            this.errorMessageLabel.Text = "";
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.detailsBox);
            this.stPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stPanel1.Location = new System.Drawing.Point(0, 131);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(386, 219);
            this.stPanel1.TabIndex = 6;
            // 
            // STErrorDialog
            // 
            this.AcceptButton = this.btnContinue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CancelButton = this.btnQuit;
            this.ClientSize = new System.Drawing.Size(386, 350);
            this.Controls.Add(this.stPanel1);
            this.Controls.Add(this.errorMessageLabel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnDetails);
            this.Controls.Add(this.btnContinue);
            this.Name = "STErrorDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Error";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.stPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Button btnDetails;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.RichTextBox detailsBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private STPanel stPanel1;
        private System.Windows.Forms.RichTextBox errorMessageLabel;

        #endregion

        public STExceptionDialog()
        {
            InitializeComponent();
            detailsBox.Multiline = true;

            // no smaller than design time size
            MinimumSize = new System.Drawing.Size(this.Width, this.Height);

            // no larger than screen size
            MaximumSize = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }
        public static void Show(string ErrorMessage, string Details)
        {
            STExceptionDialog dlg = new STExceptionDialog();
            dlg.errorMessageLabel.Text = ErrorMessage;
            dlg.detailsBox.Text = Details;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                dlg.Close();
            }
        }
        private void AutoSizeTextBox(TextBox txt)
        {
            const int x_margin = 0;
            const int y_margin = 2;
            Size size = TextRenderer.MeasureText(txt.Text, txt.Font);
            txt.ClientSize =
                new Size(size.Width + x_margin, size.Height + y_margin);
        }

        private void detailsBox_TextChanged(object sender, EventArgs e)
        {
            AutoSizeTextBox(sender as TextBox);
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {

        }
    }
}
