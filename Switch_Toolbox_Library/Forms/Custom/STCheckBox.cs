using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System;

namespace Toolbox.Library.Forms
{
    /// <summary>
    /// CheckBox created with code & design skills.
    /// </summary>
    public class STCheckBox : CheckBox
    {
        /// <summary>
        /// The color of the CheckBox background rectangle
        /// </summary>
        [Category("Options"), Description("The color of the background."), Browsable(true)]
        private Color BoxColor { get; set; }

        /// <summary>
        /// Ticked icon
        /// </summary>
        private Image TickTock => Properties.Resources.CheckMark;

        public Color BorderColor = Color.Black;

        public int BorderSize = 1;

        public bool ShowBorder = false;

        public STCheckBox()
        {
            SetColor();

            this.Paint += OnPaint;
            this.CheckedChanged += new EventHandler(CheckedChangedEvent);
        }

        bool value { get; set; }

        /// <summary>
        /// Binds a property from the given object to the checkbox
        /// </summary>
        /// <param name="Object"></param>
        /// <param name="PropertyName"></param>
        /// <param name="ResetBindings"></param>
        public void Bind(object Object, string PropertyName, bool ResetBindings = true)
        {
            if (ResetBindings)
                DataBindings.Clear();

            DataBindings.Add("Checked", Object, PropertyName);
        }

        /// <summary>
        /// Draws the ticked icon if the PsCheckBox is checked and its background color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaint(object sender, PaintEventArgs e)
        {
            SetColor();

            e.Graphics.FillRectangle(new SolidBrush(this.BoxColor), new Rectangle(0, 0, 15, 15));
            if (this.Checked)
            {
                e.Graphics.DrawImage(this.TickTock, 2, 2, 12, 12);
            }

            if (ShowBorder)
            {
                e.Graphics.DrawRectangle(new Pen(BorderColor), new Rectangle(
                    ClientRectangle.Location.X,
                    ClientRectangle.Location.Y,
                    ClientRectangle.Width - BorderSize,
                    ClientRectangle.Height - BorderSize));
            }
        }
        /// <summary>
        /// Sets the size of the layout to 16x;16x
        /// </summary>
        /// <param name="pb"></param>
        private void Set16(PictureBox pb)
        {
            pb.Size = new Size(16, 16);
        }
        /// <summary>
        /// Sets the size of the layout to 18x;18x
        /// </summary>
        /// <param name="pb"></param>
        private void Set18(PictureBox pb)
        {
            pb.Size = new Size(18, 18);
        }
        /// <summary>
        /// Changes thee way our layout behaves
        /// </summary>
        private void CheckedChangedEvent(object sender, EventArgs args)
        {
            value = Checked;

            SetColor();
            this.Invalidate();

            foreach (Binding data in DataBindings)
            {
                    data.WriteValue();
            }
        }

        private void SetColor()
        {
            if (!Enabled)
                this.BoxColor = FormThemes.BaseTheme.DisabledItemColor;
            else if (Checked)
                BoxColor = FormThemes.BaseTheme.CheckBoxEnabledBackColor;
            else
                BoxColor = FormThemes.BaseTheme.CheckBoxBackColor;
        }
    }
}
