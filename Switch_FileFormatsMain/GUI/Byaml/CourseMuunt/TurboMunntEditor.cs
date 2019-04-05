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
          //      scene.AddRenderableBfres($"{CourseFolder}/course_model.szs");
             //   scene.AddRenderableKcl($"{CourseFolder}/course.kcl");

               
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

            if (scene.LapPaths.Count > 0)
            {
                objectCB.Items.Add("Lap Paths");

                foreach (var group in scene.LapPaths)
                {
                    foreach (var path in group.PathPoints)
                    {
                        Console.WriteLine(path.Translate);
                        Console.WriteLine(path.Rotate);
                        Console.WriteLine(path.Scale);

                        viewport.AddDrawable(path.RenderablePoint);
                    }
                }
            }
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
                if (Text == "Lap Paths")
                {
                    stPropertyGrid1.LoadProperty(scene, OnPropertyChanged);

                    listViewCustom1.Items.Clear();
                    for (int i = 0; i < scene.LapPaths.Count; i++)
                    {
                        listViewCustom1.Items.Add("Lap Path Group " + i);
                    }
                }
            }
        }

        private void OnPropertyChanged()
        {

        }

        private void viewIntroCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
