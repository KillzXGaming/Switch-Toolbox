using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace LayoutBXLYT
{
    public partial class WindowFrameEditorSettings : EditorPanelBase
    {
        public PaneEditor ParentEditor;
        public EventHandler FrameSelectChanged;

        private bool loaded = false;
        private IWindowPane ActivePane;

        public WindowFrameEditorSettings()
        {
            InitializeComponent();

            windowFrameSelector1.OnFrameSelected += OnFrameSelected;
        }

        public void LoadPane(IWindowPane pane, PaneEditor paneEditor, bool reset = false)
        {
            loaded = false;
            ActivePane = pane;
            ParentEditor = paneEditor;

            windowFrameSelector1.WindowKind = pane.WindowKind;
            windowFrameSelector1.FrameCount = pane.FrameCount;

            ReloadFrameCounter();

            SetupEnums(pane);

            if (reset)
                windowFrameSelector1.ResetSelection();
            windowFrameSelector1.Invalidate();

            chkRenderContent.Bind(pane, "NotDrawnContent");

            loaded = true;
        }

        public BxlytMaterial GetActiveMaterial(IWindowPane pane)
        {
            if (windowFrameSelector1.SelectedFrame == FrameSelect.Content)
                return pane.Content.Material;
            else
                return GetActiveFrame(pane).Material;
        }

        public BxlytWindowFrame GetActiveFrame(IWindowPane pane)
        {
            if (pane.WindowKind == WindowKind.Horizontal || pane.WindowKind == WindowKind.HorizontalNoContent)
            {
                switch (windowFrameSelector1.SelectedFrame)
                {
                    case FrameSelect.Left: return pane.WindowFrames[0];
                    case FrameSelect.Right: return pane.WindowFrames[1];
                    default: return null;
                }
            }
            else
            {
                if (windowFrameSelector1.FrameCount == 4)
                {
                    switch (windowFrameSelector1.SelectedFrame)
                    {
                        case FrameSelect.TopLeft: return pane.WindowFrames[0];
                        case FrameSelect.TopRight: return pane.WindowFrames[1];
                        case FrameSelect.BottomLeft: return pane.WindowFrames[2];
                        case FrameSelect.BottomRight: return pane.WindowFrames[3];
                        default: return null;
                    }
                }
                else
                {
                    switch (windowFrameSelector1.SelectedFrame)
                    {
                        case FrameSelect.TopLeft: return pane.WindowFrames[0];
                        case FrameSelect.TopRight: return pane.WindowFrames[1];
                        case FrameSelect.BottomLeft: return pane.WindowFrames[2];
                        case FrameSelect.BottomRight: return pane.WindowFrames[3];
                        case FrameSelect.Left: return pane.WindowFrames[4];
                        case FrameSelect.Right: return pane.WindowFrames[5];
                        case FrameSelect.Top: return pane.WindowFrames[6];
                        case FrameSelect.Bottom: return pane.WindowFrames[7];
                        default: return null;
                    }
                }
            }
        }

        private void ReloadFrameCounter()
        {
            frameNumCB.Items.Clear();
            switch (ActivePane.WindowKind)
            {
                case WindowKind.Around:
                    this.frameNumCB.Items.AddRange(new object[] {
                    "1 (Top Left)",
                    "4 (Corners)",
                    "8 (Corners + Sides)"});
                    break;
                case WindowKind.Horizontal:
                case WindowKind.HorizontalNoContent:
                    this.frameNumCB.Items.AddRange(new object[] {
                    "1 (Left)",
                    "2 (Left and Right)" });
                    break;
            }
        }

        private void SetupEnums(IWindowPane pane)
        {
            bool isHorizintal = false;
            switch (pane.WindowKind)
            {
                case WindowKind.Around:
                    typeCB.SelectedIndex = 0;
                    break;
                case WindowKind.Horizontal:
                    typeCB.SelectedIndex = 1;
                    isHorizintal = true;
                    break;
                case WindowKind.HorizontalNoContent:
                    typeCB.SelectedIndex = 2;
                    isHorizintal = true;
                    break;
            }


            if (isHorizintal)
            {
                switch (pane.FrameCount)
                {
                    case 1:
                        frameNumCB.SelectedIndex = 0;
                        break;
                    case 2:
                        frameNumCB.SelectedIndex = 1;
                        break;
                }
            }
            else
            {
                switch (pane.FrameCount)
                {
                    case 1:
                        frameNumCB.SelectedIndex = 0;
                        break;
                    case 4:
                        frameNumCB.SelectedIndex = 1;
                        break;
                    case 8:
                        frameNumCB.SelectedIndex = 2;
                        break;
                }
            }
        }

        private void OnFrameSelected(object sender, EventArgs e)
        {
            frameLbl.Text = $"Selected Frame: [{windowFrameSelector1.SelectedFrame}]";

            FrameSelectChanged?.Invoke(sender, e);
        }

        private void chkRenderContent_CheckedChanged(object sender, EventArgs e) {
            ParentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void typeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loaded) return;

            switch (typeCB.SelectedIndex)
            {
                case 0:
                    ActivePane.WindowKind = WindowKind.Around;
                    windowFrameSelector1.WindowKind = WindowKind.Around;
                    break;
                case 1:
                    ActivePane.WindowKind = WindowKind.Horizontal;
                    windowFrameSelector1.WindowKind = WindowKind.Horizontal;
                    break;
                case 2:
                    ActivePane.WindowKind = WindowKind.HorizontalNoContent;
                    windowFrameSelector1.WindowKind = WindowKind.HorizontalNoContent;
                    break;
            }
            ReloadFrameCounter();

            ActivePane.ReloadFrames();

            windowFrameSelector1.Invalidate();
            ParentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void frameNumCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loaded) return;

            if (ActivePane.WindowKind != WindowKind.Around)
            {
                switch (frameNumCB.SelectedIndex)
                {
                    case 0:
                        ActivePane.FrameCount = 1;
                        windowFrameSelector1.FrameCount = 1;
                        break;
                    case 1:
                        ActivePane.FrameCount = 2;
                        windowFrameSelector1.FrameCount = 2;
                        break;
                }
            }
            else
            {
                switch (frameNumCB.SelectedIndex)
                {
                    case 0:
                        ActivePane.FrameCount = 1;
                        windowFrameSelector1.FrameCount = 1;
                        break;
                    case 1:
                        ActivePane.FrameCount = 4;
                        windowFrameSelector1.FrameCount = 4;
                        break;
                    case 2:
                        ActivePane.FrameCount = 8;
                        windowFrameSelector1.FrameCount = 8;
                        break;
                }
            }

            ActivePane.ReloadFrames();

            windowFrameSelector1.Invalidate();
            ParentEditor.PropertyChanged?.Invoke(sender, e);
        }
    }
}
