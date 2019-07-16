using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.Animations;
using Bfres.Structs;

namespace FirstPlugin.Forms
{
    public partial class SceneAnimEditor : UserControl
    {
        public SceneAnimEditor()
        {
            InitializeComponent();
            listViewCustom1.CanResizeList = false;
        }

        public void OnPropertyChanged()
        {

        }

        public void LoadCameraAnim(FSCN.BfresCameraAnim cameraAnim)
        {
            if (cameraAnim.CameraAnimU != null)
                stPropertyGrid1.LoadProperty(cameraAnim.CameraAnimU, OnPropertyChanged);
            else
                stPropertyGrid1.LoadProperty(cameraAnim.CameraAnim, OnPropertyChanged);

            listViewCustom1.Items.Clear();
            listViewCustom1.Columns.Clear();

            ColumnHeader frameColumn = new ColumnHeader() { Text = $"Frame" };
            listViewCustom1.Columns.Add(frameColumn);

            foreach (var value in cameraAnim.Values)
            {
                ColumnHeader valueColumn = new ColumnHeader()
                { Text = $"{(CameraAnimation.CameraOffsetType)value.AnimDataOffset}" };
                listViewCustom1.Columns.Add(valueColumn);
            }

            for (int Frame = 0; Frame < cameraAnim.FrameCount; Frame++)
            {
                var item1 = new ListViewItem($"{Frame}");
                listViewCustom1.Items.Add(item1);

                foreach (var track in cameraAnim.Values)
                {
                    float value = track.GetValue(Frame);

                    if (track.AnimDataOffset == (uint)CameraAnimation.CameraOffsetType.FieldOFView)
                        value = OpenTK.MathHelper.RadiansToDegrees(track.GetValue(Frame));

                    item1.SubItems.Add(value.ToString());
                }
            }
        }

        public void LoadLightAnim(LightAnimation lightAnim)
        {

        }

        public void LoadFogAnim(FogAnimation fogAnim)
        {

        }
    }
}
