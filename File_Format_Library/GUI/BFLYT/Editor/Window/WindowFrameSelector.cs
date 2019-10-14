using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.IO;

namespace LayoutBXLYT
{
    public enum FrameSelect
    {
        Content,
        TopLeft,
        Top,
        TopRight,
        Right,
        Left,
        Bottom,
        BottomLeft,
        BottomRight,
    }

    public partial class WindowFrameSelector : EditorPanelBase
    {
        public EventHandler OnFrameSelected;

        private bool canSelectContent = true;
        public bool CanSelectContent
        {
            get { return canSelectContent; }
            set
            {
                canSelectContent = value;
                SetValidSelection();
            }
        }

        private WindowKind windowKind = WindowKind.Around;
        public WindowKind WindowKind
        {
            get { return windowKind; }
            set
            {
                windowKind = value;
                SetValidSelection();
            }
        }

        private int frameCount = 1;
        public int FrameCount
        {
            get { return frameCount; }
            set
            {
                frameCount = value;
                SetValidSelection();
            }
        }

        public void SetValidSelection()
        {
            //Make sure to choose no content
            if (WindowKind == WindowKind.HorizontalNoContent)
            {
                if (SelectedFrame == FrameSelect.Content)
                    SelectedFrame = FrameSelect.Left;
            }

            if (WindowKind == WindowKind.HorizontalNoContent ||
                WindowKind == WindowKind.Horizontal)
            {
                if (!CanSelectContent && SelectedFrame == FrameSelect.Content)
                    SelectedFrame = FrameSelect.Left;

                if (SelectedFrame == FrameSelect.BottomLeft)
                    SelectedFrame = FrameSelect.Left;
                else if (SelectedFrame == FrameSelect.TopLeft)
                    SelectedFrame = FrameSelect.Left;
                else if (SelectedFrame == FrameSelect.Bottom)
                    SelectedFrame = FrameSelect.Left;
                else if (SelectedFrame == FrameSelect.Top)
                    SelectedFrame = FrameSelect.Left;
                else if (SelectedFrame == FrameSelect.BottomRight)
                    SelectedFrame = FrameSelect.Right;
                else if (SelectedFrame == FrameSelect.TopRight)
                    SelectedFrame = FrameSelect.Right;
            }
            else
            {
                if (frameCount == 4)
                {
                    if (SelectedFrame == FrameSelect.Right)
                        SelectedFrame = FrameSelect.TopRight;
                    else if (SelectedFrame == FrameSelect.Bottom)
                        SelectedFrame = FrameSelect.BottomLeft;
                    else if (SelectedFrame == FrameSelect.Top)
                        SelectedFrame = FrameSelect.TopRight;
                    else if (SelectedFrame == FrameSelect.Left)
                        SelectedFrame = FrameSelect.TopLeft;
                }

                if (!CanSelectContent && SelectedFrame == FrameSelect.Content)
                    SelectedFrame = FrameSelect.TopLeft;
            }
        }

        public FrameSelect SelectedFrame = FrameSelect.Content;

        public WindowFrameSelector()
        {
            InitializeComponent();
        }

        public void ResetSelection()
        {
            if (WindowKind == WindowKind.HorizontalNoContent)
                SelectLeft();
            else
                SelectContent();
        }

        private WindowRectangle TopLeft = new WindowRectangle();
        private WindowRectangle TopRight = new WindowRectangle();
        private WindowRectangle BottomLeft = new WindowRectangle();
        private WindowRectangle BottomRight = new WindowRectangle();
        private WindowRectangle Top = new WindowRectangle();
        private WindowRectangle Bottom = new WindowRectangle();
        private WindowRectangle Right = new WindowRectangle();
        private WindowRectangle Left = new WindowRectangle();
        private WindowRectangle Content = new WindowRectangle();

        private void RefreshRectangle()
        {
            bool IsHorizontal = WindowKind == WindowKind.Horizontal ||
                                WindowKind == WindowKind.HorizontalNoContent;

            int boundSizeX = (ClientRectangle.Width / 2) / 2;
            int boundSizeY = (ClientRectangle.Height / 2) / 2;

            int leftPosition = 0;
            int rightPosition = ClientRectangle.Width - boundSizeX;
            int topPosition = 0;
            int bottomPosition = ClientRectangle.Height - boundSizeY;

            int centerWidth = ClientRectangle.Width - (ClientRectangle.Width / 2);
            int centerHeight = ClientRectangle.Height - (ClientRectangle.Height / 2);

            TopLeft.rect = new Rectangle(leftPosition, topPosition, boundSizeX, boundSizeY);
            TopRight.rect = new Rectangle(rightPosition, topPosition, boundSizeX, boundSizeY);
            BottomLeft.rect = new Rectangle(leftPosition, bottomPosition, boundSizeX, boundSizeY);
            BottomRight.rect = new Rectangle(rightPosition, bottomPosition, boundSizeX, boundSizeY);
            Top.rect = new Rectangle(boundSizeX, topPosition, centerWidth, boundSizeY);
            Bottom.rect = new Rectangle(boundSizeX, bottomPosition, centerWidth, boundSizeY);
            Left.rect = new Rectangle(leftPosition, boundSizeY, boundSizeX, centerHeight);
            Right.rect = new Rectangle(rightPosition, boundSizeY, boundSizeX, centerHeight);
            Content.rect = new Rectangle(boundSizeX, boundSizeY, centerWidth, centerHeight);

            if (IsHorizontal)
            {
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var foreBrush = new SolidBrush(this.ForeColor))
            using (var brush = new SolidBrush(this.BackColor))
            {
                //Draw normal rectangle
                e.Graphics.DrawRectangle(new Pen(brush), ClientRectangle);

                var outlineColor = new Pen(foreBrush);

                //Draw border
                float penWidth = 1;
                float shrinkAmount = outlineColor.Width / 2;

                RefreshRectangle();

                if (WindowKind == WindowKind.Horizontal)
                {
                    Left.DrawRectangle(e.Graphics, this.ForeColor);
                    Right.DrawRectangle(e.Graphics, this.ForeColor);
                    Content.DrawRectangle(e.Graphics, this.ForeColor);
                }
                else if (WindowKind == WindowKind.HorizontalNoContent)
                {
                    Left.DrawRectangle(e.Graphics, this.ForeColor);
                    Right.DrawRectangle(e.Graphics, this.ForeColor);
                }
                else
                {
                    e.Graphics.DrawRectangle(
                    outlineColor,
                    ClientRectangle.X + shrinkAmount,
                    ClientRectangle.Y + shrinkAmount,
                    ClientRectangle.Width - penWidth,
                    ClientRectangle.Height - penWidth);

                    TopLeft.DrawRectangle(e.Graphics, this.ForeColor);
                    TopRight.DrawRectangle(e.Graphics, this.ForeColor);
                    BottomLeft.DrawRectangle(e.Graphics, this.ForeColor);
                    BottomRight.DrawRectangle(e.Graphics, this.ForeColor);
                    Top.DrawRectangle(e.Graphics, this.ForeColor);
                    Bottom.DrawRectangle(e.Graphics, this.ForeColor);
                    Left.DrawRectangle(e.Graphics, this.ForeColor);
                    Right.DrawRectangle(e.Graphics, this.ForeColor);
                    Content.DrawRectangle(e.Graphics, this.ForeColor);
                }
            }
        }

        public class WindowRectangle 
        {
            public Rectangle rect;

            public bool Selected = false;

            public void DrawRectangle(Graphics g, Color outlineColor)
            {
                if (Selected)
                    g.FillRectangle(new SolidBrush(Color.Red), rect);

                var outlinePen = new Pen(outlineColor);
                g.DrawRectangle(outlinePen, rect);
            }
        }

        private void WindowFrameSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (WindowKind == WindowKind.Horizontal)
                {
                    if (FrameCount == 1)
                    {
                        //One frame (left)
                        if (Right.rect.IsHit(e.Location))
                        {
                            SelectLeft();
                            Right.Selected = true;
                        }
                        if (Left.rect.IsHit(e.Location))
                        {
                            SelectLeft();
                            Right.Selected = true;
                        }
                    }
                    else
                    {
                        if (Right.rect.IsHit(e.Location))
                            SelectRight();
                        if (Left.rect.IsHit(e.Location))
                            SelectLeft();
                    }

                    if (Content.rect.IsHit(e.Location))
                        SelectContent();
                }
                else if (WindowKind == WindowKind.HorizontalNoContent)
                {
                    if (FrameCount == 1)
                    {
                        //One frame (left)
                        if (Right.rect.IsHit(e.Location))
                            SelectLeft();
                        if (Left.rect.IsHit(e.Location))
                            SelectLeft();
                    }
                    else
                    {
                        if (Right.rect.IsHit(e.Location))
                            SelectRight();
                        if (Left.rect.IsHit(e.Location))
                            SelectLeft();
                    }
                }
                else
                {
                    switch (FrameCount)
                    {
                        case 1:
                            if (TopLeft.rect.IsHit(e.Location))
                                SelectCorners();
                            if (TopRight.rect.IsHit(e.Location))
                                SelectCorners();
                            if (BottomLeft.rect.IsHit(e.Location))
                                SelectCorners();
                            if (BottomRight.rect.IsHit(e.Location))
                                SelectCorners();
                            if (Top.rect.IsHit(e.Location))
                                SelectCorners();
                            if (Bottom.rect.IsHit(e.Location))
                                SelectCorners();
                            if (Right.rect.IsHit(e.Location))
                                SelectCorners();
                            if (Left.rect.IsHit(e.Location))
                                SelectCorners();
                            if (Content.rect.IsHit(e.Location))
                                SelectContent();
                            break;
                        case 4:
                            if (TopLeft.rect.IsHit(e.Location))
                                SelectTopLeft();
                            if (TopRight.rect.IsHit(e.Location))
                                SelectTopRight();
                            if (BottomLeft.rect.IsHit(e.Location))
                                SelectBottomLeft();
                            if (BottomRight.rect.IsHit(e.Location))
                                SelectBottomRight();
                            if (Top.rect.IsHit(e.Location))
                                SelectTopLeft();
                            if (Bottom.rect.IsHit(e.Location))
                                SelectBottomRight();
                            if (Right.rect.IsHit(e.Location))
                                SelectTopRight();
                            if (Left.rect.IsHit(e.Location))
                                SelectBottomLeft();
                            if (Content.rect.IsHit(e.Location))
                                SelectContent();
                            break;
                        case 8:
                            if (TopLeft.rect.IsHit(e.Location))
                                SelectTopLeft();
                            if (TopRight.rect.IsHit(e.Location))
                                SelectTopRight();
                            if (BottomLeft.rect.IsHit(e.Location))
                                SelectBottomLeft();
                            if (BottomRight.rect.IsHit(e.Location))
                                SelectBottomRight();
                            if (Top.rect.IsHit(e.Location))
                                SelectTop();
                            if (Bottom.rect.IsHit(e.Location))
                                SelectBottom();
                            if (Right.rect.IsHit(e.Location))
                                SelectRight();
                            if (Left.rect.IsHit(e.Location))
                                SelectLeft();
                            if (Content.rect.IsHit(e.Location))
                                SelectContent();
                            break;
                    }
                }
            }

            this.Invalidate();
        }

        private void SelectContent()
        {
            UpdateSelection(FrameSelect.Content);
            Content.Selected = true;
        }

        private void SelectCorners()
        {
            UpdateSelection(FrameSelect.TopLeft);
            TopLeft.Selected = true;
            TopRight.Selected = true;
            BottomLeft.Selected = true;
            BottomRight.Selected = true;
        }

        private void SelectTopLeft()
        {
            UpdateSelection(FrameSelect.TopLeft);
            TopLeft.Selected = true;
        }

        private void SelectBottomLeft()
        {
            UpdateSelection(FrameSelect.BottomLeft);
            BottomLeft.Selected = true;
        }

        private void SelectLeft()
        {
            UpdateSelection(FrameSelect.Left);
            Left.Selected = true;
        }

        private void SelectBottom()
        {
            UpdateSelection(FrameSelect.Bottom);
            Bottom.Selected = true;
        }

        private void SelectTop()
        {
            UpdateSelection(FrameSelect.Top);
            Top.Selected = true;
        }

        private void SelectTopRight()
        {
            UpdateSelection(FrameSelect.TopRight);
            TopRight.Selected = true;
        }

        private void SelectRight()
        {
            UpdateSelection(FrameSelect.Right);
            Right.Selected = true;
        }

        private void SelectBottomRight()
        {
            UpdateSelection(FrameSelect.BottomRight);
            BottomRight.Selected = true;
        }

        private void UpdateSelection(FrameSelect frameSelect)
        {
            DeselectAll();

            bool Changed = SelectedFrame != frameSelect;
            SelectedFrame = frameSelect;

            //Frame has been changed, then update this
            if (Changed)
                OnFrameSelected?.Invoke(this, EventArgs.Empty);
        }

        private void DeselectAll()
        {
            Content.Selected = false;
            TopLeft.Selected = false;
            TopRight.Selected = false;
            BottomLeft.Selected = false;
            BottomRight.Selected = false;
            Top.Selected = false;
            Bottom.Selected = false;
            Right.Selected = false;
            Left.Selected = false;
        }
    }
}
