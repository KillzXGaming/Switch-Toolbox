using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public class MinMaxButton : System.Windows.Forms.Button
    {
        Color clr1;
        private Color color = Color.Gray;
        private Color m_hovercolor = Color.FromArgb(180, 200, 240);
        private Color clickcolor = Color.FromArgb(160, 180, 200);
        private int textX = 6;
        private int textY = -20;
        private String text = "_";

        public enum CustomFormState
        {
            Normal,
            Maximize
        }

        CustomFormState _customFormState;

        public CustomFormState CFormState
        {
            get { return _customFormState; }
            set { _customFormState = value; Invalidate(); }
        }


        public String DisplayText
        {
            get { return text; }
            set { text = value; Invalidate(); }
        }
        public Color BZBackColor
        {
            get { return color; }
            set { color = value; Invalidate(); }
        }

        public Color MouseHoverColor
        {
            get { return m_hovercolor; }
            set { m_hovercolor = value; Invalidate(); }
        }

        public Color MouseClickColor1
        {
            get { return clickcolor; }
            set { clickcolor = value; Invalidate(); }
        }


        public int TextLocation_X
        {
            get { return textX; }
            set { textX = value; Invalidate(); }
        }
        public int TextLocation_Y
        {
            get { return textY; }
            set { textY = value; Invalidate(); }
        }

        public MinMaxButton()
        {
            this.Size = new System.Drawing.Size(31, 24);
            this.ForeColor = Color.White;
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Text = "_";
            text = this.Text;
        }

        //method mouse enter  
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            clr1 = color;
            color = m_hovercolor;
        }
        //method mouse leave  
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            color = clr1;
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            color = clickcolor;
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            color = clr1;
        }


        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            switch (_customFormState)
            {
                case CustomFormState.Normal:
                    pe.Graphics.FillRectangle(new SolidBrush(color), ClientRectangle);

                    //draw and fill thw rectangles of maximized window       
                    for (int i = 0; i < 2; i++)
                    {
                        pe.Graphics.DrawRectangle(new Pen(this.ForeColor), textX + i + 1, textY, 10, 10);
                        pe.Graphics.FillRectangle(new SolidBrush(this.ForeColor), textX + 1, textY - 1, 12, 4);
                    }
                    break;

                case CustomFormState.Maximize:
                    pe.Graphics.FillRectangle(new SolidBrush(color), ClientRectangle);

                    //draw and fill thw rectangles of maximized window       
                    for (int i = 0; i < 2; i++)
                    {
                        pe.Graphics.DrawRectangle(new Pen(this.ForeColor), textX + 5, textY, 8, 8);
                        pe.Graphics.FillRectangle(new SolidBrush(this.ForeColor), textX + 5, textY - 1, 9, 4);

                        pe.Graphics.DrawRectangle(new Pen(this.ForeColor), textX + 2, textY + 5, 8, 8);
                        pe.Graphics.FillRectangle(new SolidBrush(this.ForeColor), textX + 2, textY + 4, 9, 4);

                    }
                    break;
            }

        }


    }
}