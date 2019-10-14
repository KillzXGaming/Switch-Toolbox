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
    public partial class WindowContentEditor : EditorPanelBase
    {
        private IWindowPane ActivePane;
        private bool Loaded = false;
        private PaneEditor parentEditor;
        private BxlytWindowFrame activeFrame;

        public WindowContentEditor()
        {
            InitializeComponent();

            vertexColorBox1.OnColorChanged += OnColorChanged;

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();
            stDropDownPanel3.ResetColors();
        }

        public void LoadPane(IWindowPane pane, BxlytWindowFrame frame, PaneEditor paneEditor)
        {
            Loaded = false;
            activeFrame = frame;
            ActivePane = pane;
            parentEditor = paneEditor;

            frameUpUD.Value = pane.FrameElementTop;
            frameDownUD.Value = pane.FrameElementBottm;
            frameLeftUD.Value = pane.FrameElementLeft;
            frameRightUD.Value = pane.FrameElementRight;

            vertexColorBox1.TopLeftColor = pane.Content.ColorTopLeft.Color;
            vertexColorBox1.TopRightColor = pane.Content.ColorTopRight.Color;
            vertexColorBox1.BottomLeftColor = pane.Content.ColorBottomLeft.Color;
            vertexColorBox1.BottomRightColor = pane.Content.ColorBottomRight.Color;
            vertexColorBox1.Refresh();

            if (frame == null)
                texRotateCB.ResetBind();
            else
            {
                texRotateCB.Bind(typeof(WindowFrameTexFlip), frame, "TextureFlip");
                texRotateCB.SelectedItem = frame.TextureFlip;
            }

            chkMaterialForAll.Bind(pane, "UseOneMaterialForAll");
            chkUseVtxColorsOnFrames.Bind(pane, "UseVertexColorForAll");
            Loaded = true;
        }

        private void OnColorChanged(object sender, EventArgs e)
        {
            if (!Loaded) return;

            ActivePane.Content.ColorTopLeft.Color = vertexColorBox1.TopLeftColor;
            ActivePane.Content.ColorTopRight.Color = vertexColorBox1.TopRightColor;
            ActivePane.Content.ColorBottomLeft.Color = vertexColorBox1.BottomLeftColor;
            ActivePane.Content.ColorBottomRight.Color = vertexColorBox1.BottomRightColor;

            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void btnResetColors_Click(object sender, EventArgs e)
        {
            vertexColorBox1.TopLeftColor = Color.White;
            vertexColorBox1.TopRightColor = Color.White;
            vertexColorBox1.BottomLeftColor = Color.White;
            vertexColorBox1.BottomRightColor = Color.White;
            vertexColorBox1.Refresh();
        }

        public override void OnControlClosing() {
            vertexColorBox1.DisposeControl();
        }

        private void frameUD_ValueChanged(object sender, EventArgs e)
        {
            if (!Loaded) return;

            ActivePane.FrameElementTop = (ushort)frameUpUD.Value;
            ActivePane.FrameElementRight = (ushort)frameRightUD.Value;
            ActivePane.FrameElementLeft = (ushort)frameLeftUD.Value;
            ActivePane.FrameElementBottm = (ushort)frameDownUD.Value;

            parentEditor.PropertyChanged?.Invoke(sender, e);
        }

        private void texRotateCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded || activeFrame == null) return;

            activeFrame.TextureFlip = (WindowFrameTexFlip)texRotateCB.SelectedItem;
            parentEditor.PropertyChanged?.Invoke(sender, e);
        }
    }
}
