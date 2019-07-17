using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public class STUserControl : UserControl
    {
        public string Text { get; set; } = "";

        public STUserControl()
        {
            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        public virtual void OnControlClosing()
        {

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // STUserControl
            // 
            this.Name = "STUserControl";
            this.ResumeLayout(false);

        }
    }
}
