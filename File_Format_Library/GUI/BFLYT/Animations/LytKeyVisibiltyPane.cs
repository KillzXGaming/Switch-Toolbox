using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Animations;
using Toolbox.Library.Forms;

namespace LayoutBXLYT
{
    public partial class LytKeyVisibiltyPane : LytAnimEditorKeyGUI
    {
        public LytKeyVisibiltyPane()
        {
            InitializeComponent();
        }

        public void SetPane(float frame, float startFrame, BasePane pane, List<STAnimGroup> groups)
        {
            paneVisibleChkBox.Checked = pane.Visible;
            alphaSelectorHorizontalPanel1.Alpha = pane.Alpha;
            alphaUD.Value = pane.Alpha;

            foreach (var group in groups)
            {
                if (group is LytVisibiltyGroup)
                {
                    var track = ((LytVisibiltyGroup)group).GetTrack(0);
                    if (track.HasKeys)
                    {
                        bool visbile = track.GetFrameValue(frame, startFrame) == 1;
                        paneVisibleChkBox.Checked = visbile;
                        bool keyed = track.IsKeyed(frame - startFrame);

                        if (keyed)
                            paneVisibleChkBox.BackColor = Color.FromArgb(255, 150, 106, 18); 
                        else
                            paneVisibleChkBox.BackColor = FormThemes.BaseTheme.CheckBoxBackColor;
                    }
                }
                else if (group is LytVertexColorGroup)
                {
                    var track = ((LytVertexColorGroup)group).GetTrack(16);
                    if (track.HasKeys)
                    {
                        byte alpha = (byte)track.GetFrameValue(frame);
                        alphaUD.Value = alpha;
                        alphaSelectorHorizontalPanel1.Alpha = alpha;
                        bool keyed = track.IsKeyed(frame);

                        SetValue(alphaUD, alpha, keyed);
                    }
                }
            }
        }
    }
}
