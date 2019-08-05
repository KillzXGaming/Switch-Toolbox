using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    //Based from https://stackoverflow.com/questions/8653430/how-can-i-show-a-message-box-with-details-in-winforms/40469355#40469355

    /// <summary>
    /// A dialog-style form with optional colapsable details section
    /// </summary>
    public partial class STErrorDialog : Form
    {
        private const string DetailsFormat = "Details {0}";

        public STErrorDialog(string message, string title, string details = null)
        {
            InitializeComponent();

            lblMessage.Text = message;
            this.Text = title;

            if (details != null)
            {
                btnDetails.Enabled = true;
                btnDetails.Text = DownArrow;
                tbDetails.Text = details;
            }
            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;

            tbDetails.BackColor = FormThemes.BaseTheme.FormBackColor;
            tbDetails.ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        private string UpArrow
        {
            get
            {
                return string.Format(DetailsFormat, char.ConvertFromUtf32(0x25B4));
            }
        }

        private string DownArrow
        {
            get
            {
                return string.Format(DetailsFormat, char.ConvertFromUtf32(0x25BE));
            }
        }

        /// <summary>
        /// Meant to give the look and feel of a regular MessageBox
        /// </summary>
        public static void Show(string message, string title, string details = null)
        {
            new STErrorDialog(message, title, details).ShowDialog();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Change these properties now so the label is rendered so we get its real height
            var height = lblMessage.Height;
            SetMessageBoxHeight(height);
        }

        private void SetMessageBoxHeight(int heightChange)
        {
            this.Height = this.Height + heightChange;
            if (this.Height < 150)
            {
                this.Height = 150;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            // Re-anchoring the controls so they stay in their place while the form is resized
            btnCopy.Anchor = AnchorStyles.Top;
            btnClose.Anchor = AnchorStyles.Top;
            btnDetails.Anchor = AnchorStyles.Top;
            tbDetails.Anchor = AnchorStyles.Top;

            tbDetails.Visible = !tbDetails.Visible;
            btnCopy.Visible = !btnCopy.Visible;

            btnDetails.Text = tbDetails.Visible ? UpArrow : DownArrow;

            SetMessageBoxHeight(tbDetails.Visible ? tbDetails.Height + 10 : -tbDetails.Height - 10);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            string text = tbDetails.Text;

            Thread STAThread = new Thread(
           delegate ()
           {
                  // Use a fully qualified name for Clipboard otherwise it
                  // will end up calling itself.
                  System.Windows.Forms.Clipboard.SetText(text);
           });
            STAThread.SetApartmentState(ApartmentState.STA);
            STAThread.Start();
            STAThread.Join();
        }
    }
}
