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
    public partial class TurboMunntEditor : UserControl, IViewportContainer
    {
        Viewport viewport;
        bool IsLoaded = false;

        public TurboMunntEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;

            viewport = new Viewport();
            viewport.Dock = DockStyle.Fill;
            viewport.scene.SelectionChanged += Scene_SelectionChanged;
            stPanel4.Controls.Add(viewport);
        }


        public Viewport GetViewport() => viewport;

        public void UpdateViewport()
        {
            if (viewport != null)
                viewport.UpdateViewport();
        }

        public AnimationPanel GetAnimationPanel() => null;


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

            treeView1.Nodes.Add("Scene");

            if (scene.EnemyPaths.Count > 0) {
                AddPathDrawable("Enemy Path", scene.EnemyPaths, Color.Red);
            }
            if (scene.LapPaths.Count > 0) {
                AddPathDrawable("Lap Path", scene.LapPaths,Color.Blue, false);
            }
            if (scene.GlidePaths.Count > 0) {
                AddPathDrawable("Glide Path", scene.GlidePaths, Color.Orange);
            }
            if (scene.ItemPaths.Count > 0) {
                AddPathDrawable("Item Path", scene.ItemPaths, Color.Yellow);
            }
            if (scene.SteerAssistPaths.Count > 0) {
                AddPathDrawable("Steer Assist Path", scene.SteerAssistPaths, Color.Green);
            }
            

            IsLoaded = true;
        }

        private void AddPathDrawable(string Name, IEnumerable<PathGroup> Groups, Color color, bool CanConnect = true)
        {
            //Create a connectable object to connect each point
            var renderablePathConnected = new RenderableConnectedPaths(color);

            if (CanConnect) {
                viewport.AddDrawable(renderablePathConnected);
            }

            //Load a node wrapper to the tree
            var pathNode = new PathCollectionNode(Name);
            treeView1.Nodes.Add(pathNode);

            int groupIndex = 0;
            foreach (var group in Groups)
            {
                if (CanConnect)
                    renderablePathConnected.AddGroup(group);

                var groupNode = new PathGroupNode($"{Name} Group{groupIndex++}");
                pathNode.Nodes.Add(groupNode);

                int pointIndex = 0;
                foreach (var path in group.PathPoints)
                {
                    var pontNode = new PathPointNode($"{Name} Point{pointIndex++}");
                    pontNode.PathPoint = path;
                    groupNode.Nodes.Add(pontNode);

                    path.OnPathMoved = OnPathMoved;
                    viewport.AddDrawable(path.RenderablePoint);
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

        private void OnPropertyChanged()
        {

        }

        private void viewIntroCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            List<EditableObject> newSelection = new List<EditableObject>();

            TreeNode node = treeView1.SelectedNode;
            if (node == null)
                return;

            if (node is PathCollectionNode)
            {
                foreach (var group in ((PathCollectionNode)node).Nodes)
                {
                    foreach (var point in ((PathGroupNode)group).Nodes)
                    {
                        newSelection.Add(((PathPointNode)point).PathPoint.RenderablePoint);
                    }
                }
            }
            if (node is PathGroupNode)
            {
                foreach (var point in ((PathGroupNode)node).Nodes)
                {
                    newSelection.Add(((PathPointNode)point).PathPoint.RenderablePoint);
                }
            }
            if (node is PathPointNode)
            {
                newSelection.Add(((PathPointNode)node).PathPoint.RenderablePoint);
            }

            if (newSelection.Count > 0)
                viewport.scene.SelectedObjects = newSelection;
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e) {
            if (!IsLoaded)
                return;

            if (e.Node is PathPointNode)
                ((PathPointNode)e.Node).OnChecked(e.Node.Checked);

            CheckChildNodes(e.Node, e.Node.Checked);
        }

        private void CheckChildNodes(TreeNode node, bool IsChecked)
        {
            foreach (TreeNode n in node.Nodes)
            {
                n.Checked = IsChecked;
            }
        }
    }
}
