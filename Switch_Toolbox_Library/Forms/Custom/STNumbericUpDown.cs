using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public enum NumericDataType
    {
        Byte,
        Ubyte,
        Float,
        Uint32,
        Int32,
        Int64,
        Uint64,
    }

    public class STNumbericUpDown : NumericUpDown
    {
        public STNumbericUpDown()
        {
            InitializeComponent();

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
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

            DataBindings.Add("Value", Object, PropertyName);
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // STNumbericUpDown
            // 
            this.ValueChanged += new System.EventHandler(this.STNumbericUpDown_ValueChanged);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        private void STNumbericUpDown_ValueChanged(object sender, System.EventArgs e)
        {
            foreach (Binding data in DataBindings)
            {
                data.WriteValue();
            }
        }
    }

    public class STNumbericUpDown2 : UserControl
    {
        public NumericDataType DataType;

        private STTextBox stTextBox1;
        private PictureBox upArrow;
        private STPanel stPanel1;
        private PictureBox downArrow;

        public STNumbericUpDown2()
        {
            InitializeComponent();

            BackColor = FormThemes.BaseTheme.ConsoleEditorBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(STUserControl));
            this.stTextBox1 = new Toolbox.Library.Forms.STTextBox();
            this.downArrow = new System.Windows.Forms.PictureBox();
            this.upArrow = new System.Windows.Forms.PictureBox();
            this.stPanel1 = new Toolbox.Library.Forms.STPanel();
            ((System.ComponentModel.ISupportInitialize)(this.downArrow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upArrow)).BeginInit();
            this.stPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // stTextBox1
            // 
            this.stTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.stTextBox1.Location = new System.Drawing.Point(3, 2);
            this.stTextBox1.Name = "stTextBox1";
            this.stTextBox1.Size = new System.Drawing.Size(119, 20);
            this.stTextBox1.TabIndex = 0;
            this.stTextBox1.TextChanged += new System.EventHandler(this.stTextBox1_TextChanged);
            // 
            // downArrow
            // 
            this.downArrow.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.downArrow.Image = ((System.Drawing.Image)(resources.GetObject("downArrow.Image")));
            this.downArrow.Location = new System.Drawing.Point(1, 12);
            this.downArrow.Name = "downArrow";
            this.downArrow.Size = new System.Drawing.Size(19, 11);
            this.downArrow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.downArrow.TabIndex = 2;
            this.downArrow.TabStop = false;
            // 
            // upArrow
            // 
            this.upArrow.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.upArrow.Image = global::Toolbox.Library.Properties.Resources.NumbericUpDownArrow;
            this.upArrow.Location = new System.Drawing.Point(1, 3);
            this.upArrow.Name = "upArrow";
            this.upArrow.Size = new System.Drawing.Size(19, 11);
            this.upArrow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.upArrow.TabIndex = 1;
            this.upArrow.TabStop = false;
            // 
            // stPanel1
            // 
            this.stPanel1.Controls.Add(this.upArrow);
            this.stPanel1.Controls.Add(this.downArrow);
            this.stPanel1.Location = new System.Drawing.Point(125, 0);
            this.stPanel1.Name = "stPanel1";
            this.stPanel1.Size = new System.Drawing.Size(23, 25);
            this.stPanel1.TabIndex = 3;
            // 
            // STUserControl
            // 
            this.Controls.Add(this.stPanel1);
            this.Controls.Add(this.stTextBox1);
            this.Name = "STUserControl";
            this.Size = new System.Drawing.Size(148, 25);
            this.ValueChanged += new System.EventHandler(stTextBox1_TextChanged);
            ((System.ComponentModel.ISupportInitialize)(this.downArrow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upArrow)).EndInit();
            this.stPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private decimal maximum;

        public decimal Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                maximum = value;
            }
        }

        private decimal minimum;

        public decimal Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                minimum = value;
            }
        }

        private decimal _value;

        public decimal Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public int DecimalPlaces { get; set; }

        public decimal Increment { get; set; }

        public event System.EventHandler ValueChanged;

        private void stTextBox1_TextChanged(object sender, System.EventArgs e)
        {
            bool IsSuccess = false;

            byte dataByte;
            float dataFloat;
            int dataInt32;
            uint dataUInt32;
            long dataInt64;
            ulong dataUInt64;
            decimal dataDecimal;

            string text = stTextBox1.Text;
            switch (DataType)
            {
                case NumericDataType.Byte:
                    IsSuccess = byte.TryParse(text, out dataByte);
                    break;
                case NumericDataType.Ubyte:
                    IsSuccess = byte.TryParse(text, out dataByte);
                    break;
                case NumericDataType.Float:
                    IsSuccess = float.TryParse(text, out dataFloat);
                    break;
                case NumericDataType.Int32:
                    IsSuccess = int.TryParse(text, out dataInt32);
                    break;
                case NumericDataType.Uint32:
                    IsSuccess = uint.TryParse(text, out dataUInt32);
                    break;
                case NumericDataType.Int64:
                    IsSuccess = long.TryParse(text, out dataInt64);
                    break;
                case NumericDataType.Uint64:
                    IsSuccess = ulong.TryParse(text, out dataUInt64);
                    break;
                default:
                    IsSuccess = decimal.TryParse(text, out dataDecimal);
                    break;
            }
        }
    }
}
