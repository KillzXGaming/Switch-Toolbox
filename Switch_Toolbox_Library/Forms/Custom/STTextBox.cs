using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public class STTextBox : TextBox
    {
        public STTextBox()
        {
            BackColor = FormThemes.BaseTheme.TextEditorBackColor;
            ForeColor = FormThemes.BaseTheme.TextForeColor;
            BorderStyle = BorderStyle.FixedSingle;

            InitializeComponent();
        }

        /// <summary>
        /// Binds a property from the given object to the textbox
        /// </summary>
        /// <param name="Object"></param>
        /// <param name="PropertyName"></param>
        /// <param name="ResetBindings"></param>
        public void Bind(object Object, string PropertyName, bool ResetBindings = true)
        {
            if (ResetBindings)
                DataBindings.Clear();

            DataBindings.Add("Text", Object, PropertyName);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // STTextBox
            // 
            this.TextChanged += new System.EventHandler(this.STTextBox_TextChanged);
            this.ResumeLayout(false);

        }

        private void STTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.Text.Length == 0) return;

            foreach (Binding data in DataBindings)
            {
                data.WriteValue();
            }
        }
    }
}
