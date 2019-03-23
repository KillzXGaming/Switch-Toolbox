using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using ByamlExt.Byaml;
using ByamlExt;

namespace FirstPlugin.Turbo
{
    public partial class MK8MapCameraEditor : STForm
    {
        public MK8MapCameraEditor()
        {
            InitializeComponent();
        }

        Course_MapCamera_bin activeCameraFile;
        public void LoadFile(Course_MapCamera_bin mapCamera)
        {
            activeCameraFile = mapCamera;

            if (activeCameraFile.BigEndian)
                beBtnRadio.Checked = true;
            else
                leBtnRadio.Checked = true;

            stPropertyGrid1.LoadProperty(activeCameraFile.cameraData, OnPropertyChanged);
        }

        public void OnPropertyChanged() { }

        private void stButton1_Click(object sender, EventArgs e)
        {

        }

        private void beBtnRadio_CheckedChanged(object sender, EventArgs e)
        {
            activeCameraFile.BigEndian = beBtnRadio.Checked;
        }
    }
}
