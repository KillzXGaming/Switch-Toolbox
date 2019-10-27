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
using Toolbox.Library.IO;
using ByamlExt.Byaml;
using ByamlExt;
using FirstPlugin.MuuntEditor;

namespace FirstPlugin.Turbo
{
    public partial class MK8MapCameraEditor : UserControl, IFIleEditor
    {
        public List<MuuntCollisionObject> CollisionObjects = new List<MuuntCollisionObject>();

        public List<IFileFormat> GetFileFormats()
        {
            return new List<IFileFormat>() { activeCameraFile };
        }

        public MK8MapCameraEditor()
        {
            InitializeComponent();
        }

        private Course_MapCamera_bin activeCameraFile;
        public void LoadFile(Course_MapCamera_bin mapCamera)
        {
            activeCameraFile = mapCamera;

            var cam = mapCamera.cameraData;

            if (activeCameraFile.BigEndian)
                beBtnRadio.Checked = true;
            else
                leBtnRadio.Checked = true;

            stPropertyGrid1.LoadProperty(activeCameraFile.cameraData, OnPropertyChanged);

            mapCameraViewer1.LoadCameraFile(mapCamera, this);
        }

        public void OnPropertyChanged() { }

        private void stButton1_Click(object sender, EventArgs e)
        {

        }

        private void beBtnRadio_CheckedChanged(object sender, EventArgs e)
        {
            activeCameraFile.BigEndian = beBtnRadio.Checked;
        }

        private void mapCameraViewer1_MouseDown(object sender, MouseEventArgs e)
        {
          
        }

        private void MK8MapCameraEditor_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
            }
        }

        private void btnCollisionPreview_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Utils.GetAllFilters(typeof(KCL));
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var fileFormat = STFileLoader.OpenFileFormat(ofd.FileName);
                if (fileFormat != null && fileFormat is KCL)
                    LoadCollision((KCL)fileFormat);
            }
        }

        private void LoadCollision(KCL kcl)
        {
            MuuntCollisionObject col = new MuuntCollisionObject();
            col.CollisionFile = kcl;
            col.Renderer = new KCLRendering2D(kcl);
            CollisionObjects.Add(col);
        }
    }
}
