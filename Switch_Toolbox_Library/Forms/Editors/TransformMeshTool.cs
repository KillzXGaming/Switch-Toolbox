using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Rendering;
using OpenTK;

namespace Toolbox.Library.Forms
{
    public partial class TransformMeshTool : Form
    {
        public TransformMeshTool()
        {
            InitializeComponent();
        }
        public STGenericObject[] cachedBackups;
        List<STGenericObject> objects = new List<STGenericObject>();
        Action UpdateVertices;

        public void LoadGenericMesh(STGenericObject obj, Action updateVertices)
        {
            objects.Add(obj);

            UpdateVertices = updateVertices;
        }
        public void CacheList()
        {
            cachedBackups = new STGenericObject[objects.Count];
            Array.Copy(objects.ToArray(), 0, cachedBackups, 0, cachedBackups.Length);
        }
        private void UD_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var genericObject in objects)
            {
                Vector3 pos = new Vector3((float)posXUD.Value, (float)posYUD.Value, (float)posZUD.Value);
                Vector3 rot = new Vector3((float)rotXUD.Value, (float)rotYUD.Value, (float)rotZUD.Value);
                Vector3 sca = new Vector3((float)scaXUD.Value, (float)scaYUD.Value, (float)scaZUD.Value);

                genericObject.TransformPosition(pos, rot, sca);
            }
            UpdateVertices();
        }

        private void flipX90_Click(object sender, EventArgs e)
        {
            foreach (var genericObject in objects)
                genericObject.TransformPosition(Vector3.Zero, new Vector3(0,90,0),new Vector3(1));
            UpdateVertices();
        }

        private void flipY90_Click(object sender, EventArgs e)
        {
            foreach (var genericObject in objects)
                genericObject.TransformPosition(Vector3.Zero, new Vector3(90, 0, 0), new Vector3(1));
            UpdateVertices();
        }

        private void flipZ90_Click(object sender, EventArgs e)
        {
            foreach (var genericObject in objects)
                genericObject.TransformPosition(Vector3.Zero, new Vector3(0, 0, 90), new Vector3(1));
            UpdateVertices();
        }
    }
}
