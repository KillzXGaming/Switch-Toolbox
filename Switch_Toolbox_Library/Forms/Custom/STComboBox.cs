using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection;

namespace Toolbox.Library.Forms
{
    public class STComboBox : ComboBox
    {
        public static ComboBoxStyle STDropDownStyle = ComboBoxStyle.DropDown;

        private Brush BorderBrush = new SolidBrush(FormThemes.BaseTheme.ComboBoxBackColor);
        private Brush ArrowBrush = new SolidBrush(SystemColors.ControlText);
        private Brush DropButtonBrush = new SolidBrush(SystemColors.Control);

        private Color _borderColor
        {
            get
            {
                if (Enabled)
                    return  FormThemes.BaseTheme.ComboBoxBorderColor; 
                else
                    return  FormThemes.BaseTheme.DisabledBorderColor;
            }
        }
        private ButtonBorderStyle _borderStyle = ButtonBorderStyle.Solid;
        private static int WM_PAINT = 0x000F;

        private Color _ButtonColor = SystemColors.Control;

        public Color ButtonColor
        {
            get { return _ButtonColor; }
            set
            {
                _ButtonColor = value;
                DropButtonBrush = new SolidBrush(this.ButtonColor);
                this.Invalidate();
            }
        }

        public STComboBox()
        {
            ButtonColor = FormThemes.BaseTheme.ComboBoxBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
            BackColor = FormThemes.BaseTheme.ComboBoxBackColor;
            DropDownStyle = ComboBoxStyle.DropDown;

            if (FormThemes.ActivePreset == FormThemes.Preset.White)
                DropDownStyle = ComboBoxStyle.DropDownList;

            InitializeComponent();
        }

        public void LoadEnum(Type type)
        {
            DataBindings.Clear();
            DataSource = Enum.GetValues(type);
        }

        private bool IsTextReadOnly = true;

        public void SetAsReadOnly()
        {
            IsReadOnly = true;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (IsReadOnly)
                ((HandledMouseEventArgs)e).Handled = true;
        }

        private dynamic value;

        public string GetSelectedText()
        {
            return GetItemText(SelectedItem);
        }

        public void SelectItemByText(string text)
        {
            SelectedIndex = FindStringExact(text);
        }

        public void ResetBind()
        {
            DataBindings.Clear();
            DataSource = null;

            Items.Clear();
        }

        public void Bind(Type EnumType, object Object, string PropertyName, bool IsReset = true)
        {
            if (IsReset)
                DataBindings.Clear();

            DataSource = Enum.GetValues(EnumType);
            return;

            if (IsReset)
                DataBindings.Clear();

            DataSource = Enum.GetValues(EnumType);

            DataBindings.Add(new Binding("SelectedValue", Object, PropertyName, true, DataSourceUpdateMode.OnPropertyChanged));
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case 0xf:
                    if (FormThemes.ActivePreset != FormThemes.Preset.White)
                    {
                        Graphics g = this.CreateGraphics();
                        Pen p = new Pen(Color.Black);
                        g.FillRectangle(BorderBrush, this.ClientRectangle);

                        //Draw the background of the dropdown button
                        Rectangle rect = new Rectangle(this.Width - 17, 0, 17, this.Height);
                        g.FillRectangle(DropButtonBrush, rect);

                        //Create the path for the arrow
                        System.Drawing.Drawing2D.GraphicsPath pth = new System.Drawing.Drawing2D.GraphicsPath();
                        PointF TopLeft = new PointF(this.Width - 13, (this.Height - 5) / 2);
                        PointF TopRight = new PointF(this.Width - 6, (this.Height - 5) / 2);
                        PointF Bottom = new PointF(this.Width - 9, (this.Height + 2) / 2);
                        pth.AddLine(TopLeft, TopRight);
                        pth.AddLine(TopRight, Bottom);

                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                        //Determine the arrow's color.
                        ArrowBrush = new SolidBrush(FormThemes.BaseTheme.ComboBoxArrowColor);

                        //Draw the arrow
                        g.FillPath(ArrowBrush, pth);
                    }


                    break;
                default:
                    break;
            }
        }

        [Category("Appearance")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                Invalidate(); // causes control to be redrawn
            }
        }

        [Category("Appearance")]
        public ButtonBorderStyle BorderStyle
        {
            get { return _borderStyle; }
            set
            {
                _borderStyle = value;
                Invalidate();
            }
        }

        public Dictionary<string, Brush> ItemColorMapper = new Dictionary<string, Brush>();
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var backBrush = new SolidBrush(BorderColor);
            var foreBrush = new SolidBrush(ForeColor);

            e.Graphics.FillRectangle(backBrush, this.ClientRectangle);

            if (ItemColorMapper.ContainsKey(this.Text))
                e.Graphics.DrawString(this.Text, this.Font, ItemColorMapper[this.Text], this.Location);
            else
                e.Graphics.DrawString(this.Text, this.Font, foreBrush, this.Location);
        }

        protected override void OnLostFocus(System.EventArgs e)
        {
            base.OnLostFocus(e);
            this.Invalidate();
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            this.Invalidate();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // STComboBox
            // 
            this.SelectedIndexChanged += new System.EventHandler(this.STComboBox_SelectedIndexChanged);
            this.DropDownStyleChanged += new System.EventHandler(this.STComboBox_DropDownStyleChanged);
            this.DropDownClosed += new System.EventHandler(this.STComboBox_DropDownClosed);
            this.TextChanged += new System.EventHandler(this.STComboBox_TextChanged);
            this.EnabledChanged += new EventHandler(EnableDisplayCombo_EnabledChanged);
            this.ResumeLayout(false);

        }

        void EnableDisplayCombo_EnabledChanged(object sender, EventArgs e)
        {
            if (this.Enabled)
                this.DropDownStyle = ComboBoxStyle.DropDown;
            else
                this.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void STComboBox_TextChanged(object sender, EventArgs e)
        {

        }

        private bool _readOnly = false;

        public bool IsReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (_readOnly || IsTextReadOnly)
            {
                switch (e.KeyCode)
                {
                    case Keys.Back:
                    case Keys.Delete:
                        e.SuppressKeyPress = true;
                        return;
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (_readOnly ||  IsTextReadOnly)
            {
                e.Handled = true;
                return;
            }
            base.OnKeyPress(e);
        }

        private void STComboBox_DropDownClosed(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() => { Select(0, 0); }));
        }

        private void STComboBox_DropDownStyleChanged(object sender, EventArgs e)
        {
            if (FormThemes.ActivePreset != FormThemes.Preset.White)
                DropDownStyle = ComboBoxStyle.DropDown;
        }

        private void STComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndex >= 0)
            {
                value = SelectedItem;
            }
        }

        protected override void OnDropDown(EventArgs e)
        {
            if (_readOnly)
            {
                DropDownHeight = 1;
                var t = new Thread(CloseDropDown);
                t.Start();
                return;
            }
            base.OnDropDown(e);
        }

        private delegate void CloseDropDownDelegate();
        private void WaitForDropDown()
        {
            if (InvokeRequired)
            {
                var d = new CloseDropDownDelegate(WaitForDropDown);
                Invoke(d);
            }
            else
            {
                DroppedDown = false;
            }
        }
        private void CloseDropDown()
        {
            WaitForDropDown();
        }
    }
}
