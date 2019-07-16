using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class SearchHex : STForm
    {
        public HexBox HexBox { get; set; }

        private bool _finding;

        private FindOptions _findOptions;

        public FindOptions FindOptions
        {
            get
            {
                return _findOptions;
            }
            set
            {
                _findOptions = value;
                Reinitialize();
            }
        }

        private void Reinitialize()
        {
            if (hexFind.ByteProvider != null)
                hexFind.ByteProvider.Changed -= new EventHandler(ByteProvider_Changed);

            var hex = this._findOptions.Hex != null ? _findOptions.Hex : new byte[0];
            hexFind.ByteProvider = new DynamicByteProvider(hex);
            hexFind.ByteProvider.Changed += new EventHandler(ByteProvider_Changed);
        }

        public FindType findType;
        public string findString;
        public bool matchCase;
        public byte[] findHex;

        public SearchHex()
        {
            InitializeComponent();
            hexFind.BackColor = FormThemes.BaseTheme.FormBackColor;
            hexFind.ForeColor = FormThemes.BaseTheme.FormBackColor;


        }
        public void SearchItem()
        {

        }

        private void stButton1_Click(object sender, EventArgs e)
        {
            var provider = this.hexFind.ByteProvider as DynamicByteProvider;
            _findOptions.Hex = provider.Bytes.ToArray();
            _findOptions.Text = txtFind.Text;
            _findOptions.Type = radioBtnText.Checked ? FindType.Hex : FindType.Text;
            _findOptions.MatchCase = chkMatchCase.Checked;
            _findOptions.IsValid = true;

            FindNext();
        }

        public void FindNext()
        {
            if (!_findOptions.IsValid)
                return;

            UpdateUIToFindingState();

            // start find process
            long res = HexBox.Find(_findOptions);

            UpdateUIToNormalState();

            Application.DoEvents();

            if (res == -1) // -1 = no match
            {
             //   MessageBox.Show(strings.FindOperationEndOfFile, Application.ProductName,
             //       MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (res == -2) // -2 = find was aborted
            {
                return;
            }
            else // something was found
            {
                this.Close();

                Application.DoEvents();

                if (!HexBox.Focused)
                    HexBox.Focus();
            }
        }

        private void UpdateUIToNormalState()
        {
            timer.Stop();
            timerPercent.Stop();
            _finding = false;
            txtFind.Enabled = chkMatchCase.Enabled = raditnHex.Enabled = radioBtnText.Enabled
                = hexFind.Enabled = btnOK.Enabled = true;
        }

        private void UpdateUIToFindingState()
        {
            _finding = true;
            timer.Start();
            timerPercent.Start();
            txtFind.Enabled = chkMatchCase.Enabled = raditnHex.Enabled = radioBtnText.Enabled
                = hexFind.Enabled = btnOK.Enabled = false;
        }

        void ByteProvider_Changed(object sender, EventArgs e)
        {
            ValidateFind();
        }

        private void ValidateFind()
        {
            var isValid = false;
            if (radioBtnText.Checked && txtFind.Text.Length > 0)
                isValid = true;
            if (raditnHex.Checked && hexFind.ByteProvider.Length > 0)
                isValid = true;
            this.btnOK.Enabled = isValid;
        }

        private void radioBtn_CheckedChanged(object sender, EventArgs e)
        {
            txtFind.Enabled = radioBtnText.Checked;
            hexFind.Enabled = !txtFind.Enabled;

            if (txtFind.Enabled)
                txtFind.Focus();
            else
                hexFind.Focus();
        }

        private void radioBtnText_Enter(object sender, EventArgs e) {
            txtFind.Focus();
        }

        private void raditnHex_Enter(object sender, EventArgs e) {
            hexFind.Focus();
        }

        private void SearchHex_Activated(object sender, EventArgs e)
        {
            if (radioBtnText.Checked)
                txtFind.Focus();
            else
                hexFind.Focus();
        }

        private void timerPercent_Tick(object sender, EventArgs e)
        {

        }
    }
}
