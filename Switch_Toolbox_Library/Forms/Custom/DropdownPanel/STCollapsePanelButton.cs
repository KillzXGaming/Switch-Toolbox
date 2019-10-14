using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class STCollapsePanelButton : UserControl
    {
        //Somewhat based on https://github.com/alexander-makarov/ExpandCollapsePanel

        public STCollapsePanelButton()
        {
            InitializeComponent();

            isExpanded = false;

            BackColor = FormThemes.BaseTheme.DropdownButtonBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
            SetIconColor = FormThemes.BaseTheme.DropdownButtonBackColor;

            ExpandedImage = pictureBox1.Image;

            CollapsedImage = new Bitmap(pictureBox1.Image);
            CollapsedImage.RotateFlip(RotateFlipType.Rotate90FlipX);
        }

        public string PanelName
        {
            get
            {
                return stLabel1.Text;
            }
            set
            {
                 stLabel1.Text = value;
            }
        }

        public string PanelValueName
        {
            get
            {
                return alternativeLabel.Text;
            }
            set
            {
                alternativeLabel.Text = value;
            }
        }
        

        public Image SetIcon
        {
            get
            {
                return pictureBox2.Image;
            }
            set
            {
                pictureBox2.Image = value;
            }
        }

        public Color SetIconColor
        {
            get
            {
                return pictureBox3.BackColor;
            }
            set
            {
                pictureBox3.BackColor = value;
            }
        }

        public Color SetIconAlphaColor
        {
            get
            {
                return pictureBox2.BackColor;
            }
            set
            {
                pictureBox2.BackColor = value;
            }
        }

        private Image ExpandedImage { get; set; }

        private Image CollapsedImage { get; set; }

        private bool isExpanded = false;

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnExpandCollapse();
            }
        }

        [Browsable(false)]
        public override bool AutoScroll
        {
            get
            {
                return base.AutoScroll;
            }
            set
            {
                base.AutoScroll = value;
            }
        }

        public event EventHandler<ExpandCollapseEventArgs> ExpandCollapse;

        protected virtual void OnExpandCollapse()
        {
            pictureBox1.Image = isExpanded ? ExpandedImage : CollapsedImage;

            EventHandler<ExpandCollapseEventArgs> handler = ExpandCollapse;
            if (handler != null)
                handler(this, new ExpandCollapseEventArgs(isExpanded));

        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            // just invert current state
            IsExpanded = !IsExpanded;
        }
    }
}
