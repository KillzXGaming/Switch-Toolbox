    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    namespace Toolbox.Library.Forms
    {
        public class STButton : Button
        {
            public STButton()
            {
                BackColor = FormThemes.BaseTheme.FormBackColor;
                ForeColor = FormThemes.BaseTheme.FormForeColor;

                EnabledChanged += Button1_EnabledChanged;
                Paint += btn_Paint;

                if (FormThemes.ActivePreset == FormThemes.Preset.Dark)
                {
                    FlatStyle = FlatStyle.Flat;
                }
                if (FormThemes.ActivePreset == FormThemes.Preset.White)
                    FlatStyle = FlatStyle.Standard;
            }

            public override void NotifyDefault(bool value)
            {
                base.NotifyDefault(false);
            }

            private void Button1_EnabledChanged(object sender, System.EventArgs e)
            {
                var btn = (Button)sender;
                ForeColor = btn.Enabled == false ? FormThemes.BaseTheme.DisabledItemColor : FormThemes.BaseTheme.FormForeColor;
                BackColor = btn.Enabled == false ? FormThemes.BaseTheme.FormBackColor : FormThemes.BaseTheme.FormBackColor;
            }
            private void btn_Paint(object sender, PaintEventArgs e)
            {

            }
        }
    }
