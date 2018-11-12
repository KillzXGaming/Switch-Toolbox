using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syroot.NintenTools.NSW.Bfres;
using Switch_Toolbox.Library;

namespace FirstPlugin
{
    public partial class SRTValuesPanel : UserControl
    {
        public SRTValuesPanel()
        {
            InitializeComponent();
            HideControls();
        }
        public void LoadValues(Srt2D srt2D)
        {
            scaleUDX.Value = (decimal)srt2D.Scaling.X;
            scaleUDY.Value = (decimal)srt2D.Scaling.Y;
            rotUDX.Value = (decimal)srt2D.Rotation;
            transUDX.Value = (decimal)srt2D.Translation.X;
            transUDY.Value = (decimal)srt2D.Translation.Y;
        }
        public void LoadValues(Srt3D srt3D)
        {
            scaleUDX.Value = (decimal)srt3D.Scaling.X;
            scaleUDY.Value = (decimal)srt3D.Scaling.Y;
            scaleUDZ.Value = (decimal)srt3D.Scaling.Z;
            rotUDX.Value = (decimal)srt3D.Rotation.X;
            rotUDY.Value = (decimal)srt3D.Rotation.Y;
            rotUDZ.Value = (decimal)srt3D.Rotation.Z;
            transUDX.Value = (decimal)srt3D.Translation.X;
            transUDY.Value = (decimal)srt3D.Translation.Y;
            transUDZ.Value = (decimal)srt3D.Translation.Y;

            scaleUDZ.Visible = true;
            transUDZ.Visible = true;
            rotUDZ.Visible = true;
            rotUDY.Visible = true;
        }

        public void LoadValues(TexSrt texSrt)
        {
            scaleUDX.Value = (decimal)texSrt.Scaling.X;
            scaleUDY.Value = (decimal)texSrt.Scaling.Y;
            rotUDX.Value = (decimal)texSrt.Rotation;
            transUDX.Value = (decimal)texSrt.Translation.X;
            transUDY.Value = (decimal)texSrt.Translation.Y;

            modeComboBox.Items.Add(TexSrtMode.Mode3dsMax);
            modeComboBox.Items.Add(TexSrtMode.ModeMaya);
            modeComboBox.Items.Add(TexSrtMode.ModeSoftimage);
            modeComboBox.SelectedItem = texSrt.Mode;

            modeComboBox.Visible = true;
            modeLabel.Visible = true;
        }
        public void LoadValues(TexSrtEx texSrt)
        {
            scaleUDX.Value = (decimal)texSrt.Scaling.X;
            scaleUDY.Value = (decimal)texSrt.Scaling.Y;
            rotUDX.Value = (decimal)texSrt.Rotation;
            transUDX.Value = (decimal)texSrt.Translation.X;
            transUDY.Value = (decimal)texSrt.Translation.Y;

            modeComboBox.Items.Add(TexSrtMode.Mode3dsMax);
            modeComboBox.Items.Add(TexSrtMode.ModeMaya);
            modeComboBox.Items.Add(TexSrtMode.ModeSoftimage);
            modeComboBox.SelectedItem = texSrt.Mode;

            matrixPtrNumUD.Value = texSrt.MatrixPointer;

            modeComboBox.Visible = true;
            modeLabel.Visible = true;
            matrixPtrLabel.Visible = true;
            matrixPtrNumUD.Visible = true;
        }
        private void HideControls() //Hide controls that may be unused for other SRT types
        {
            modeComboBox.Visible = false;
            modeLabel.Visible = false;
            matrixPtrLabel.Visible = false;
            matrixPtrNumUD.Visible = false;
            scaleUDZ.Visible = false;
            transUDZ.Visible = false;
            rotUDZ.Visible = false;
            rotUDY.Visible = false;
        }

        private void UD_ValueChanged(object sender, EventArgs e)
        {
            Viewport.Instance.UpdateViewport();
        }

        private void modeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Viewport.Instance.UpdateViewport();
        }
    }
}
