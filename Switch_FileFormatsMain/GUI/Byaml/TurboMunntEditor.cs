using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library;
using System.Windows.Forms;
using FirstPlugin.Turbo.CourseMuuntStructs;

namespace FirstPlugin.Forms
{
    public partial class TurboMunntEditor : UserControl
    {
        Viewport viewport;

        public TurboMunntEditor()
        {
            InitializeComponent();

            viewport = new Viewport();
            viewport.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(viewport);
        }

        CourseMuuntScene scene;

        public void LoadCourseInfo(System.Collections.IEnumerable by, string FilePath)
        {
            string CourseFolder = System.IO.Path.GetDirectoryName(FilePath);
            scene = new CourseMuuntScene(by);
            if (File.Exists($"{CourseFolder}/course_model.szs"))
            {
                scene.AddRenderableBfres($"{CourseFolder}/course_model.szs");
                scene.AddRenderableKcl($"{CourseFolder}/course.kcl");

               
                foreach (var kcl in scene.KclObjects)
                {
                    viewport.AddDrawable(kcl.Renderer);

                    kcl.Renderer.UpdateVertexData();
                }

                foreach (var bfres in scene.BfresObjects)
                {
                    viewport.AddDrawable(bfres.BFRESRender);

                    bfres.BFRESRender.UpdateVertexData();
                    bfres.BFRESRender.UpdateTextureMaps();
                }
                viewport.LoadObjects();
            }

            objectCB.Items.Add("Scene");
            objectCB.SelectedIndex = 0;
        }

        private void objectCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectCB.SelectedIndex <= 0)
            {
                string Text = objectCB.GetSelectedText();

                if (Text == "Scene")
                {
                    stPropertyGrid1.LoadProperty(scene, OnPropertyChanged);
                }
            }
        }

        private void OnPropertyChanged()
        {

        }
    }
}
