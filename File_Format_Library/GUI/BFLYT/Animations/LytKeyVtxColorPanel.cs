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
using Toolbox.Library;

namespace LayoutBXLYT
{
    public partial class LytKeyVtxColorPanel : LytAnimEditorKeyGUI
    {
        public LytKeyVtxColorPanel()
        {
            InitializeComponent();
        }

        private LytAnimGroup activeGroup;
        public void SetColors(Color[] colors, float frame, float startFrame, LytAnimGroup group)
        {
            activeGroup = group;

            SetValue(TLRUD, colors[0].R, false);
            SetValue(TLGUD, colors[0].G, false);
            SetValue(TLBUD, colors[0].B, false);
            SetValue(TLAUD, colors[0].A, false);

            SetValue(TRRUD, colors[1].R, false);
            SetValue(TRGUD, colors[1].G, false);
            SetValue(TRBUD, colors[1].B, false);
            SetValue(TRAUD, colors[1].A, false);

            SetValue(BLRUD, colors[2].R, false);
            SetValue(BLGUD, colors[2].G, false);
            SetValue(BLBUD, colors[2].B, false);
            SetValue(BLAUD, colors[2].A, false);

            SetValue(BRRUD, colors[3].R, false);
            SetValue(BRGUD, colors[3].G, false);
            SetValue(BRBUD, colors[3].B, false);
            SetValue(BRAUD, colors[3].A, false);

            foreach (var grp in group.SubAnimGroups)
            {
                if (grp is LytVertexColorGroup)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        var track = ((LytVertexColorGroup)grp).GetTrack(i);
                        if (track.HasKeys)
                        {
                            float value = track.GetFrameValue(frame, startFrame);
                            bool keyed = track.IsKeyed(frame - startFrame);
                            switch ((LVCTarget)i)
                            {
                                case LVCTarget.LeftTopRed: SetValue(TLRUD, value, keyed); break;
                                case LVCTarget.LeftTopGreen: SetValue(TLGUD, value, keyed); break;
                                case LVCTarget.LeftTopBlue: SetValue(TLBUD, value, keyed); break;
                                case LVCTarget.LeftTopAlpha: SetValue(TLAUD, value, keyed); break;
                                case LVCTarget.RightTopRed: SetValue(TRRUD, value, keyed); break;
                                case LVCTarget.RightTopGreen: SetValue(TRGUD, value, keyed); break;
                                case LVCTarget.RightTopBlue: SetValue(TRBUD, value, keyed); break;
                                case LVCTarget.RightTopAlpha: SetValue(TRAUD, value, keyed); break;
                                case LVCTarget.LeftBottomRed: SetValue(BLRUD, value, keyed); break;
                                case LVCTarget.LeftBottomGreen: SetValue(BLGUD, value, keyed); break;
                                case LVCTarget.LeftBottomBlue: SetValue(BLBUD, value, keyed); break;
                                case LVCTarget.LeftBottomAlpha: SetValue(BLAUD, value, keyed); break;
                                case LVCTarget.RightBottomRed: SetValue(BRRUD, value, keyed); break;
                                case LVCTarget.RightBottomGreen: SetValue(BRGUD, value, keyed); break;
                                case LVCTarget.RightBottomBlue: SetValue(BRBUD, value, keyed); break;
                                case LVCTarget.RightBottomAlpha: SetValue(BRAUD, value, keyed); break;
                            }
                        }
                    }
                }
            }

            UpdateColors();
        }

        private void UpdateColors()
        {
            vertexColorBox1.TopLeftColor = Color.FromArgb((byte)TLAUD.Value, (byte)TLRUD.Value, (byte)TLGUD.Value, (byte)TLBUD.Value);
            vertexColorBox1.TopRightColor = Color.FromArgb((byte)TRAUD.Value, (byte)TRRUD.Value, (byte)TRGUD.Value, (byte)TRBUD.Value);
            vertexColorBox1.BottomLeftColor = Color.FromArgb((byte)BLAUD.Value, (byte)BLRUD.Value, (byte)BLGUD.Value, (byte)BLBUD.Value);
            vertexColorBox1.BottomRightColor = Color.FromArgb((byte)BRAUD.Value, (byte)BRRUD.Value, (byte)BRGUD.Value, (byte)BRBUD.Value);
            vertexColorBox1.OnColorChanged += OnColorChanged;
        }


        private void OnColorChanged(object sender, EventArgs e)
        {
            SetKeyedValue(TLRUD, vertexColorBox1.TopLeftColor.R);
            SetKeyedValue(TLGUD, vertexColorBox1.TopLeftColor.G);
            SetKeyedValue(TLBUD, vertexColorBox1.TopLeftColor.B);
            SetKeyedValue(TLAUD, vertexColorBox1.TopLeftColor.A);

            SetKeyedValue(TRRUD, vertexColorBox1.TopRightColor.R);
            SetKeyedValue(TRGUD, vertexColorBox1.TopRightColor.G);
            SetKeyedValue(TRBUD, vertexColorBox1.TopRightColor.B);
            SetKeyedValue(TRAUD, vertexColorBox1.TopRightColor.A);

            SetKeyedValue(BLRUD, vertexColorBox1.BottomLeftColor.R);
            SetKeyedValue(BLGUD, vertexColorBox1.BottomLeftColor.G);
            SetKeyedValue(BLBUD, vertexColorBox1.BottomLeftColor.B);
            SetKeyedValue(BLAUD, vertexColorBox1.BottomLeftColor.A);

            SetKeyedValue(BRRUD, vertexColorBox1.BottomRightColor.R);
            SetKeyedValue(BRGUD, vertexColorBox1.BottomRightColor.G);
            SetKeyedValue(BRBUD, vertexColorBox1.BottomRightColor.B);
            SetKeyedValue(BRAUD, vertexColorBox1.BottomRightColor.A);
        }

        private void SetKeyedValue(BarSlider.BarSlider barSlider, float value)
        {
            bool changed = barSlider.Value != value;
            if (!changed)
                return;

            SetValue(barSlider, value, true);
        }

        private void stLabel2_Click(object sender, EventArgs e)
        {

        }
    }
}
