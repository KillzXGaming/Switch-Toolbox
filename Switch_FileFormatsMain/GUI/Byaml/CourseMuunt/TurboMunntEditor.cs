using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library;
using System.Windows.Forms;
using FirstPlugin.Turbo.CourseMuuntStructs;
using GL_EditorFramework.EditorDrawables;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FirstPlugin.Forms
{
    public partial class TurboMunntEditor : UserControl
    {
        Viewport viewport;

        public TurboMunntEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            viewport = new Viewport();
            viewport.Dock = DockStyle.Fill;
            viewport.scene.SelectionChanged += Scene_SelectionChanged;
            stPanel4.Controls.Add(viewport);
        }

        CourseMuuntScene scene;

        public void LoadCourseInfo(System.Collections.IEnumerable by, string FilePath)
        {
            string CourseFolder = System.IO.Path.GetDirectoryName(FilePath);
            scene = new CourseMuuntScene(by);

            if (File.Exists($"{CourseFolder}/course_kcl.szs"))
                scene.AddRenderableKcl($"{CourseFolder}/course_kcl.szs");
            if (File.Exists($"{CourseFolder}/course.kcl"))
                scene.AddRenderableKcl($"{CourseFolder}/course.kcl");

            if (File.Exists($"{CourseFolder}/course_model.szs"))
            {
          //      scene.AddRenderableBfres($"{CourseFolder}/course_model.szs");

               

            }

            foreach (var kcl in scene.KclObjects)
            {
             //   viewport.AddDrawable(kcl.Renderer);
            //    kcl.Renderer.UpdateVertexData();
            }

            foreach (var bfres in scene.BfresObjects)
            {
                viewport.AddDrawable(bfres.BFRESRender);

                bfres.BFRESRender.UpdateVertexData();
                bfres.BFRESRender.UpdateTextureMaps();
            }
            viewport.AddDrawable(new GL_EditorFramework.EditorDrawables.SingleObject(new OpenTK.Vector3(0)));

            viewport.LoadObjects();

            objectCB.Items.Add("Scene");
            objectCB.SelectedIndex = 0;

            if (scene.LapPaths.Count > 0) {
                objectCB.Items.Add("Lap Paths");

                foreach (var group in scene.LapPaths)
                {
                    foreach (var path in group.PathPoints)
                    {
                        path.OnPathMoved = OnPathMoved;
                        viewport.AddDrawable(path.RenderablePoint);
                    }
                }
            }

            if (scene.EnemyPaths.Count > 0)
            {
                objectCB.Items.Add("Enemy Paths");
                var renderablePath = new RenderableConnectedPaths();
                foreach (var group in scene.EnemyPaths)
                    renderablePath.AddGroup(group);

                viewport.AddDrawable(renderablePath);

                foreach (var group in scene.EnemyPaths)
                {
                    foreach (var path in group.PathPoints)
                    {
                        path.OnPathMoved = OnPathMoved;
                        viewport.AddDrawable(path.RenderablePoint);
                    }
                }
            }

            
        }

        private void OnPathMoved() {
            stPropertyGrid1.Refresh();
        }

        private void Scene_SelectionChanged(object sender, EventArgs e)
        {
            foreach (EditableObject o in viewport.scene.objects)
            {
                if (o.IsSelected() && o is RenderablePathPoint)
                {
                    stPropertyGrid1.LoadProperty(((RenderablePathPoint)o).NodeObject, OnPropertyChanged);
                }
            }
        }


        private void objectCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectCB.SelectedIndex >= 0)
            {
                string Text = objectCB.GetSelectedText();

                treeViewCustom1.Nodes.Clear();
                if (Text == "Scene")
                {
                    stPropertyGrid1.LoadProperty(scene, OnPropertyChanged);
                }
                else if (Text == "Lap Paths")
                {
                    stPropertyGrid1.LoadProperty(scene, OnPropertyChanged);

                    for (int i = 0; i < scene.LapPaths.Count; i++)
                    {
                        TreeNode group = new TreeNode("Lap Path Group " + i);
                        treeViewCustom1.Nodes.Add(group);
                        for (int p = 0; p < scene.LapPaths[i].PathPoints.Count; p++)
                        {
                            group.Nodes.Add("Lap Path Point " + p);
                        }
                    }
                }
                else
                {
                    stPropertyGrid1.LoadProperty(null, OnPropertyChanged);
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
