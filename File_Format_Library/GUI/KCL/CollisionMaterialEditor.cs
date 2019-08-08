using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirstPlugin;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class CollisionMaterialEditor : STForm
    {
        public CollisionMaterialEditor()
        {
            InitializeComponent();
        }

        public KCL.GameSet GameMaterialSet = KCL.GameSet.MarioKart8D;

        public class CollisionMaterial
        {
            public ushort ID { get; set; }
            public string Type { get; set; }
        }

        KCLRendering Render;

        public void LoadCollisionValues(MarioKart.MK7.KCL kcl, KCLRendering renderer)
        {
            Render = renderer;

            List<CollisionMaterial> Materials = new List<CollisionMaterial>();

            foreach (var model in kcl.Models)
            {
                foreach (var plane in model.Planes)
                {
                    string type = "Unknown";

                    switch (GameMaterialSet)
                    {
                        case KCL.GameSet.MarioKart8D:
                            type = ((KCL.CollisionType_MK8D)plane.CollisionType).ToString();
                            break;
                        case KCL.GameSet.MarioOdyssey:
                            type = ((KCL.CollisionType_MarioOdssey)plane.CollisionType).ToString();
                            break;
                    }

                    Materials.Add(new CollisionMaterial()
                    {
                        ID = plane.CollisionType,
                        Type = type,
                    });
                }

                stListView1.SetObjects(Materials);
            }

            stListView1.SetTheme();
        }

        private void stListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Render.SelectedTypes.Clear();
            foreach (var collision in stListView1.SelectedObjects)
            {
                var coll = (CollisionMaterial)collision;
                Render.SelectedTypes.Add(coll.ID);
            }
        }
    }
}
