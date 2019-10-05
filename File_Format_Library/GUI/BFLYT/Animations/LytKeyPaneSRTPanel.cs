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
using Toolbox.Library.Animations;

namespace LayoutBXLYT
{
    public partial class LytKeyPanePanel : LytAnimEditorKeyGUI
    {
        public LytKeyPanePanel()
        {
            InitializeComponent();
        }

        public void SetSRT(float frame, float startFrame, BasePane pane, List<STAnimGroup> groups)
        {
            foreach (var group in groups)
            {
                if (group is LytPaneSRTGroup)
                {
                    SetValue(rotXUD, pane.Rotate.X, false);
                    SetValue(rotYUD, pane.Rotate.Y, false);
                    SetValue(rotZUD, pane.Rotate.Z, false);
                    SetValue(tranXUD, pane.Translate.X, false);
                    SetValue(tranYUD, pane.Translate.Y, false);
                    SetValue(tranZUD, pane.Translate.Z, false);
                    SetValue(scaleXUD, pane.Scale.X, false);
                    SetValue(scaleYUD, pane.Scale.Y, false);
                    SetValue(sizeXUD, pane.Width, false);
                    SetValue(sizeYUD, pane.Height, false);
               
                    for (int i = 0; i < 10; i++)
                    {
                        var track = ((LytPaneSRTGroup)group).GetTrack(i);
                        if (track.HasKeys)
                        {
                            float value = track.GetFrameValue(frame, startFrame);
                            bool keyed = track.IsKeyed(frame - startFrame);

                            Console.WriteLine($"SetSRT track {track.Name} {value}" + frame);
                            switch ((LPATarget)i)
                            {
                                case LPATarget.RotateX:
                                    SetValue(rotXUD, value, keyed); break;
                                case LPATarget.RotateY:
                                    SetValue(rotYUD, value, keyed); break;
                                case LPATarget.RotateZ:
                                    SetValue(rotZUD, value, keyed); break;
                                case LPATarget.TranslateX:
                                    SetValue(tranXUD, value, keyed); break;
                                case LPATarget.TranslateY:
                                    SetValue(tranYUD, value, keyed); break;
                                case LPATarget.TranslateZ:
                                    SetValue(tranZUD, value, keyed); break;
                                case LPATarget.ScaleX:
                                    SetValue(scaleXUD, value, keyed); break;
                                case LPATarget.ScaleY:
                                    SetValue(scaleYUD, value, keyed); break;
                                case LPATarget.SizeX:
                                    SetValue(sizeXUD, value, keyed); break;
                                case LPATarget.SizeY:
                                    SetValue(sizeYUD, value, keyed); break;
                            }
                        }
                    }
                }
            }
        }

        private void LytKeyPaneSRTPanel_Load(object sender, EventArgs e)
        {

        }
    }
}
