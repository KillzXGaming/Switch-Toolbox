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
using Toolbox.Library;
using ByamlExt.Byaml;
using ByamlExt;

namespace FirstPlugin.Turbo
{
    public partial class MK8MapCameraEditor : UserControl, IFIleEditor
    {
        public List<IFileFormat> GetFileFormats()
        {
            return new List<IFileFormat>() { activeCameraFile };
        }

        public MK8MapCameraEditor()
        {
            InitializeComponent();
        }

        Course_MapCamera_bin activeCameraFile;
        public void LoadFile(Course_MapCamera_bin mapCamera)
        {
            activeCameraFile = mapCamera;

            var cam = mapCamera.cameraData;

            if (activeCameraFile.BigEndian)
                beBtnRadio.Checked = true;
            else
                leBtnRadio.Checked = true;

            stPropertyGrid1.LoadProperty(activeCameraFile.cameraData, OnPropertyChanged);

            glControl2D1.AddCircle(new OpenTK.Vector2(cam.PositionX, cam.PositionZ));
            glControl2D1.AddCircle(new OpenTK.Vector2(cam.TargetX, cam.TargetZ));

            glControl2D1.AddRectangle(cam.BoundingWidth, cam.BoundingHeight, new OpenTK.Vector2(cam.PositionX, cam.PositionZ));
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
