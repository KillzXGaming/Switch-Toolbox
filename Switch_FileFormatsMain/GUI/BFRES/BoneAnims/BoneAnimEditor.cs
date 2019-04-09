using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bfres.Structs;
using Switch_Toolbox.Library.Forms;
using BrightIdeasSoftware;

namespace FirstPlugin.Forms
{
    public partial class BoneAnimEditor2 : UserControl
    {
        public BoneAnimEditor2()
        {
            InitializeComponent();

            objectListView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            objectListView1.ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        public void LoadBoneAnim(FSKA.BoneAnimNode boneAnim)
        {
            objectListView1.BeginUpdate();
            objectListView1.ClearObjects();
            objectListView1.ShowGroups = false;

            FSKA fska = (FSKA)boneAnim.Parent;

            if (boneAnim.BoneAnimU != null)
                stPropertyGrid1.LoadProperty(boneAnim.BoneAnimU, onPropertyChanged);
            else
                stPropertyGrid1.LoadProperty(boneAnim.BoneAnim, onPropertyChanged);

            frameCountLbl.Text = $" / {fska.FrameCount}";
            currentFrameUD.Maximum = fska.FrameCount;

            return;

            for (int frame = 0; frame < fska.FrameCount; frame++)
            {
                bool IsKeyed = (boneAnim.XPOS.HasAnimation() && boneAnim.XPOS.GetKeyFrame(frame).IsKeyed ||
                    boneAnim.YPOS.HasAnimation() && boneAnim.YPOS.GetKeyFrame(frame).IsKeyed ||
                    boneAnim.ZPOS.HasAnimation() && boneAnim.ZPOS.GetKeyFrame(frame).IsKeyed ||
                    boneAnim.XROT.HasAnimation() && boneAnim.XROT.GetKeyFrame(frame).IsKeyed ||
                    boneAnim.YROT.HasAnimation() && boneAnim.YROT.GetKeyFrame(frame).IsKeyed ||
                    boneAnim.ZROT.HasAnimation() && boneAnim.ZROT.GetKeyFrame(frame).IsKeyed ||
                    boneAnim.WROT.HasAnimation() && boneAnim.WROT.GetKeyFrame(frame).IsKeyed ||
                    boneAnim.XSCA.HasAnimation() && boneAnim.XSCA.GetKeyFrame(frame).IsKeyed ||
                    boneAnim.YSCA.HasAnimation() && boneAnim.YSCA.GetKeyFrame(frame).IsKeyed ||
                    boneAnim.ZSCA.HasAnimation() && boneAnim.ZSCA.GetKeyFrame(frame).IsKeyed);

                if (IsKeyed)
                {
                    KeyFrame key = new KeyFrame();
                    key.Frame = frame;

                    //Load position
                    if (boneAnim.XPOS.HasAnimation())
                        key.PosX = boneAnim.XPOS.GetValue(frame);
                    if (boneAnim.YPOS.HasAnimation())
                        key.PosY = boneAnim.YPOS.GetValue(frame);
                    if (boneAnim.ZPOS.HasAnimation())
                        key.PosZ = boneAnim.ZPOS.GetValue(frame);

                    //Load Rotation
                    if (boneAnim.XROT.HasAnimation())
                        key.RotX = boneAnim.XROT.GetValue(frame);
                    if (boneAnim.YROT.HasAnimation())
                        key.RotY = boneAnim.YROT.GetValue(frame);
                    if (boneAnim.ZROT.HasAnimation())
                        key.RotZ = boneAnim.ZROT.GetValue(frame);
                    if (boneAnim.WROT.HasAnimation())
                        key.RotW = boneAnim.WROT.GetValue(frame);

                    //Load Scale
                    if (boneAnim.XSCA.HasAnimation())
                        key.ScaX = boneAnim.XSCA.GetValue(frame);
                    if (boneAnim.YSCA.HasAnimation())
                        key.ScaY = boneAnim.YSCA.GetValue(frame);
                    if (boneAnim.ZSCA.HasAnimation())
                        key.ScaZ = boneAnim.ZSCA.GetValue(frame);

                    objectListView1.AddObject(key);
                }
            }
            objectListView1.EndUpdate();

        }

        public class KeyFrame
        {

            public int Frame { get; set; }

            public float PosX { get; set; }
            public float PosY { get; set; }
            public float PosZ { get; set; }

            public float RotX { get; set; }
            public float RotY { get; set; }
            public float RotZ { get; set; }
            public float RotW { get; set; } = 1;

            public float ScaX { get; set; } = 1;
            public float ScaY { get; set; } = 1;
            public float ScaZ { get; set; } = 1;

            const int size = 4;
            const int padding = 8;

            /*      public override string ToString()
                {
                      return String.Format("[{0}][{1}],[{2}],[{3}],[{4}],[{5}],[{6}],[{7}],[{8}],[{9}]", (Frame + 1).ToString().PadLeft(5),
                        PosX.ToString().ToFixedString(size, ' ').PadRight(padding),
                        PosY.ToString().ToFixedString(size, ' ').PadRight(padding),
                        PosZ.ToString().ToFixedString(size, ' ').PadRight(padding),
                        RotX.ToString().ToFixedString(size, ' ').PadRight(padding),
                        RotY.ToString().ToFixedString(size, ' ').PadRight(padding),
                        RotZ.ToString().ToFixedString(size, ' ').PadRight(padding),
                        RotW.ToString().ToFixedString(size, ' ').PadRight(padding),
                        ScaX.ToString().ToFixedString(size, ' ').PadRight(padding),
                        ScaY.ToString().ToFixedString(size, ' ').PadRight(padding),
                        ScaZ.ToString().ToFixedString(size, ' ').PadRight(padding));
                    }*/
        }


        public void onPropertyChanged() { }

        private void stPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
