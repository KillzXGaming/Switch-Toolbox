using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Bfres.Structs;

namespace FirstPlugin
{
    public partial class BfresBoneEditor : UserControl
    {
        public BfresBoneEditor()
        {
            InitializeComponent();

            foreach (var type in Enum.GetValues(typeof(STBone.BoneRotationType)).Cast<STBone.BoneRotationType>())
                rotModeCB.Items.Add(type);

            rotMeasureCB.SelectedIndex = 1;
        }
        BfresBone activeBone;
        public void LoadBone(BfresBone bone)
        {
            activeBone = bone;

            if (bone.Parent == null)
                parentUD.Value = bone.parentIndex;
            else
                parentUD.Value = -1;

            billboardIDUD.Value = bone.BillboardIndex;

            rotModeCB.SelectedIndex = (int)bone.boneRotationType;
            chkboxVisible.Checked = bone.IsVisible;
            textBoxName.Text = bone.Text;
            transXUD.Value = (decimal)bone.position[0];
            transYUD.Value = (decimal)bone.position[1];
            transZUD.Value = (decimal)bone.position[2];
            rotUDX.Value = (decimal)bone.rotation[0];
            rotUDY.Value = (decimal)bone.rotation[1];
            rotUDZ.Value = (decimal)bone.rotation[2];
            scaleUDX.Value = (decimal)bone.scale[0];
            scaleUDY.Value = (decimal)bone.scale[1];
            scaleUDZ.Value = (decimal)bone.scale[2];
        }

        private void rotMeasureCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activeBone == null)
                return;

            if (rotMeasureCB.SelectedIndex == 0)
            {
                rotUDX.Value = (decimal)OpenTK.MathHelper.RadiansToDegrees(activeBone.rotation[0]);
                rotUDY.Value = (decimal)OpenTK.MathHelper.RadiansToDegrees(activeBone.rotation[1]);
                rotUDZ.Value = (decimal)OpenTK.MathHelper.RadiansToDegrees(activeBone.rotation[2]);
            }
            else if (rotMeasureCB.SelectedIndex == 1)
            {
                rotUDX.Value = (decimal)activeBone.rotation[0];
                rotUDY.Value = (decimal)activeBone.rotation[1];
                rotUDZ.Value = (decimal)activeBone.rotation[2];
            }
        }

        private void valueUD_ValueChanged(object sender, EventArgs e)
        {
            Viewport.Instance.UpdateViewport();
        }
    }
}
