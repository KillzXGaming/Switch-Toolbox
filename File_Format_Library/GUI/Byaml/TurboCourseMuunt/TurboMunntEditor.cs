using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Toolbox.Library.Forms;
using Toolbox.Library;
using System.Windows.Forms;
using FirstPlugin.Turbo.CourseMuuntStructs;
using GL_EditorFramework.EditorDrawables;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library.Rendering;
using Toolbox.Library.IO;
using FirstPlugin.Turbo;
using AampLibraryCSharp;

namespace FirstPlugin.Forms
{
    public partial class TurboMunntEditor : UserControl, IViewportContainer
    {
        Viewport viewport;
        GLControl2D viewport2D;

        bool IsLoaded = false;

        public TurboMunntEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;

            viewport = new Viewport(ObjectEditor.GetDrawableContainers());
            viewport.Dock = DockStyle.Fill;
            viewport.scene.SelectionChanged += Scene_SelectionChanged;
            stPanel4.Controls.Add(viewport);

            viewport2D = new GLControl2D();
            viewport2D.Dock = DockStyle.Fill;
            stPanel3.Controls.Add(viewport2D);
        }


        public Viewport GetViewport() => viewport;

        public void UpdateViewport()
        {
            if (viewport != null)
                viewport.UpdateViewport();
        }

        public AnimationPanel GetAnimationPanel() => null;


        CourseMuuntScene scene;

        string CourseFolder;
        public void LoadCourseInfo(System.Collections.IEnumerable by, string FilePath)
        {
            CourseFolder = System.IO.Path.GetDirectoryName(FilePath);
            scene = new CourseMuuntScene(by);

            //Add collsion (switch)
            if (File.Exists($"{CourseFolder}/course_kcl.szs"))
                scene.AddRenderableKcl($"{CourseFolder}/course_kcl.szs");

            //Add collsion (wii u)
            if (File.Exists($"{CourseFolder}/course.kcl"))
                scene.AddRenderableKcl($"{CourseFolder}/course.kcl");

            //Add probe lighting config  (wii u)
            if (File.Exists($"{CourseFolder}/course.bglpbd"))
                scene.AddParameterArchive($"{CourseFolder}/course.bglpbd");

            //Add probe lighting config  (switch)
            if (File.Exists($"{CourseFolder}/course_bglpbd.szs"))
                scene.AddParameterArchive($"{CourseFolder}/course_bglpbd.szs");


            //Add course model
            if (File.Exists($"{CourseFolder}/course_model.szs"))
                scene.AddRenderableBfres($"{CourseFolder}/course_model.szs");

            //Add camera mini map parameters
            if (File.Exists($"{CourseFolder}/course_mapcamera.bin"))
                scene.MapCamera = STFileLoader.OpenFileFormat($"{CourseFolder}/course_mapcamera.bin");

            if (scene.MapCamera != null)
            {
                var cam = (Course_MapCamera_bin)scene.MapCamera;

                var wrapper = new MapCameraWrapper();
                wrapper.Text = "course_mapcamera.bin";
                wrapper.CameraFile = cam;

                var pointConnection = new RenderableConnectedMapPoints(Color.Blue);

                Vector4 PositionColor = new Vector4(1, 0, 0, 1);
                Vector4 TargetColor = new Vector4(0, 1, 0, 1);

                Vector3 BoundingScale = new Vector3(cam.cameraData.BoundingWidth, 1, cam.cameraData.BoundingHeight);

                Vector3 CamTranslate = new Vector3(cam.cameraData.PositionX, cam.cameraData.PositionY, cam.cameraData.PositionZ);
                Vector3 CamTargetTranslate = new Vector3(cam.cameraData.TargetX, cam.cameraData.TargetY, cam.cameraData.TargetZ);

                wrapper.MapCameraPosition = new RenderablePathPoint(PositionColor, CamTranslate, new Vector3(0), new Vector3(100), cam);
                wrapper.MapCameraTarget = new RenderablePathPoint(TargetColor, CamTargetTranslate, new Vector3(0), BoundingScale, cam);

                pointConnection.AddRenderable(wrapper.MapCameraPosition);
                pointConnection.AddRenderable(wrapper.MapCameraTarget);

                viewport.AddDrawable(pointConnection);
                viewport.AddDrawable(wrapper.MapCameraPosition);
                viewport.AddDrawable(wrapper.MapCameraTarget);

                treeView1.Nodes.Add(wrapper);
            }

            foreach (AAMP aamp in scene.ParameterArchives)
                LoadParameters(aamp.aampFile);

            viewport.AddDrawable(new GL_EditorFramework.EditorDrawables.SingleObject(new OpenTK.Vector3(0)));

            viewport.LoadObjects();

            treeView1.Nodes.Add("Scene");


            foreach (var bfres in scene.BfresObjects)
            {
                viewport.AddDrawable(bfres.BFRESRender);
                treeView1.Nodes.Add(bfres);
                bfres.Checked = true;
            }

            if (scene.LapPaths.Count > 0) {
                AddPathDrawable("Lap Path", scene.LapPaths, Color.Blue);
            }
            if (scene.GravityPaths.Count > 0) {
                AddPathDrawable("Gravity Path", scene.GravityPaths, Color.Purple);
            }
            if (scene.EnemyPaths.Count > 0) {
                AddPathDrawable("Enemy Path", scene.EnemyPaths, Color.Red);
            }
            if (scene.GlidePaths.Count > 0) {
                AddPathDrawable("Glide Path", scene.GlidePaths, Color.Orange);
            }
            if (scene.ItemPaths.Count > 0) {
                AddPathDrawable("Item Path", scene.ItemPaths, Color.Yellow);
            }
            if (scene.PullPaths.Count > 0) {
                AddPathDrawable("Pull Path", scene.PullPaths, Color.GreenYellow);
            }
            if (scene.SteerAssistPaths.Count > 0) {
                AddPathDrawable("Steer Assist Path", scene.SteerAssistPaths, Color.Green);
            }
            if (scene.Paths.Count > 0) {
                AddPathDrawable("Path", scene.Paths, Color.Black);
            }
            if (scene.ObjPaths.Count > 0) {
                //  AddPathDrawable("Object Path", scene.ObjPaths, Color.DarkSeaGreen);
            }
            if (scene.JugemPaths.Count > 0) {
                AddPathDrawable("Jugem Path", scene.JugemPaths, Color.DarkSeaGreen);
            }
            if (scene.IntroCameras.Count > 0) {
                AddPathDrawable("IntroCamera", scene.IntroCameras, Color.Pink);
            }

            foreach (var kcl in scene.KclObjects)
            {
                //    viewport.AddDrawable(kcl.Renderer);
                //    treeView1.Nodes.Add(kcl);
                //   kcl.Checked = true;
            }


            IsLoaded = true;
        }

        public class MapCameraWrapper : TreeNodeCustom
        {
            public RenderablePathPoint MapCameraPosition;
            public RenderablePathPoint MapCameraTarget;
            public Course_MapCamera_bin CameraFile;

            public MapCameraWrapper()
            {
                ContextMenuStrip = new STContextMenuStrip();
                ContextMenuStrip.Items.Add(new STToolStipMenuItem("Save", null, Save, Keys.Control | Keys.S));
            }

            private void Save(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    STFileSaver.SaveFileFormat(CameraFile, sfd.FileName);
                }
            }
        }

        private void UpdateCameraMapCoordinates(MapCameraWrapper wrapper)
        {
            if (scene.MapCamera != null)
            {
                var cam = wrapper.CameraFile;

                Vector3 BoundingScale = new Vector3(cam.cameraData.BoundingWidth, 1, cam.cameraData.BoundingHeight);

                Vector3 CamTranslate = new Vector3(cam.cameraData.PositionX, cam.cameraData.PositionY, cam.cameraData.PositionZ);
                Vector3 CamTargetTranslate = new Vector3(cam.cameraData.TargetX, cam.cameraData.TargetY, cam.cameraData.TargetZ);

                wrapper.MapCameraPosition.Position = CamTranslate;
                wrapper.MapCameraPosition.Scale = BoundingScale;
                wrapper.MapCameraTarget.Position = CamTargetTranslate;
            }
        }

        ProbeLighting probeLightingConfig;

        private void LoadParameters(AampFile aamp)
        {
            if (aamp.EffectType == "Probe Data")
            {
                probeLightingConfig = new ProbeLighting();
                viewport.AddDrawable(probeLightingConfig);
                var probeRoot = new ProbeLightingWrapper(probeLightingConfig);
                treeView1.Nodes.Add(probeRoot);

                uint index = 0;
                foreach (var val in aamp.RootNode.childParams)
                {
                    var entry = new ProbeLighting.Entry();
                    entry.Index = index++;
                    probeLightingConfig.Entries.Add(entry);

                    probeRoot.Nodes.Add(new ProbeLightingEntryWrapper(entry));

                    foreach (var param in val.paramObjects)
                    {
                        switch (param.HashString)
                        {
                            case "param_obj":
                                foreach (var data in param.paramEntries) {
                                    if (data.HashString == "index") entry.Index = (uint)data.Value;
                                    if (data.HashString == "type") entry.Type = (uint)data.Value;
                                }
                                break;
                            case "grid":
                                entry.Grid = LoadGridData(param.paramEntries);
                                break;
                            case "sh_index_buffer":
                                LoadIndexBuffer(param.paramEntries, entry);
                                break;
                            case "sh_data_buffer":
                                LoadDataBuffer(param.paramEntries, entry);
                                break;
                        }
                    }
                }

                aamp.Save($"{CourseFolder}/DEBUG_PROBE.aamp");

                foreach (var entry in probeLightingConfig.Entries)
                {
                    Console.WriteLine(entry.Name);
                    Console.WriteLine($"IndexType {entry.IndexType}");
                    Console.WriteLine($"DataType {entry.DataType}");
                    Console.WriteLine($"MaxIndexNum {entry.MaxIndexNum}");
                    Console.WriteLine($"UsedIndexNum {entry.UsedIndexNum}");
                    Console.WriteLine($"MaxShDataNum {entry.MaxShDataNum}");
                    Console.WriteLine($"UsedShDataNum {entry.UsedShDataNum}");

                    Console.WriteLine($"AABB_Max_Position {entry.Grid.AABB_Max_Position}");
                    Console.WriteLine($"AABB_Min_Position {entry.Grid.AABB_Min_Position}");
                    Console.WriteLine($"Voxel_Step_Position {entry.Grid.Voxel_Step_Position}");

                    Console.WriteLine($"DataBuffer {entry.DataBuffer.Length}");
                    Console.WriteLine($"IndexBuffer {entry.IndexBuffer.Length}");

                }

            }
        }

        private void LoadDataBuffer(ParamEntry[] paramEntries, ProbeLighting.Entry probeEntry)
        {
            foreach (var entry in paramEntries)
            {
                if (entry.HashString == "type")
                    probeEntry.DataType = (uint)entry.Value;
                if (entry.HashString == "used_data_num")
                    probeEntry.UsedShDataNum = (uint)entry.Value;
                if (entry.HashString == "max_sh_data_num")
                    probeEntry.MaxShDataNum = (uint)entry.Value;
                if (entry.HashString == "data_buffer")
                {
                    if (entry.ParamType == ParamType.BufferFloat)
                        probeEntry.DataBuffer = (float[])entry.Value;
                }
            }
        }

        private void LoadIndexBuffer(ParamEntry[] paramEntries, ProbeLighting.Entry probeEntry)
        {
            foreach (var entry in paramEntries)
            {
                if (entry.HashString == "type")
                    probeEntry.IndexType = (uint)entry.Value;
                if (entry.HashString == "used_index_num")
                    probeEntry.UsedIndexNum = (uint)entry.Value;
                if (entry.HashString == "max_index_num")
                    probeEntry.MaxIndexNum = (uint)entry.Value;
                if (entry.HashString == "index_buffer")
                {
                    if (entry.ParamType == ParamType.BufferUint)
                        probeEntry.IndexBuffer = (uint[])entry.Value;

                    //Experimental, just fill in indices
                    uint[] values = (uint[])entry.Value;
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = 0;
                    }
                    entry.Value = values;
                }
            }
        }

        private ProbeLighting.Grid LoadGridData(ParamEntry[] paramEntries)
        {
            ProbeLighting.Grid grid = new ProbeLighting.Grid();

            var mainBfres = scene.BfresObjects[0];
        /*    var boundings = mainBfres.BFRESRender.GetSelectionBox();

            foreach (var entry in paramEntries)
            {
                if (entry.HashString == "aabb_min_pos") {
                    grid.AABB_Max_Position = Utils.ToVec3((Syroot.Maths.Vector3F)entry.Value);

                    entry.Value = new Syroot.Maths.Vector3F(boundings.minX, boundings.minY, boundings.minZ);
                }
                if (entry.HashString == "aabb_max_pos") {
                    grid.AABB_Min_Position = Utils.ToVec3((Syroot.Maths.Vector3F)entry.Value);

                    entry.Value = new Syroot.Maths.Vector3F(boundings.maxX, boundings.maxY, boundings.maxZ);
                }
                if (entry.HashString == "voxel_step_pos")
                    grid.Voxel_Step_Position = Utils.ToVec3((Syroot.Maths.Vector3F)entry.Value);
            }*/

            return grid;
        }

        private void AddPathDrawable(string Name, IEnumerable<BasePathPoint> Groups, Color color, bool CanConnect = true)
        {

        }

        private void AddPathDrawable(string Name, IEnumerable<BasePathGroup> Groups, Color color, bool CanConnect = true)
        {
            //Create a connectable object to connect each point
            var renderablePathConnected = new RenderableConnectedPaths(color);

            if (Name == "Lap Path" || Name == "Gravity Path")
                renderablePathConnected.Use4PointConnection = true;

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
            var node = treeView1.SelectedNode;

            if (node is MapCameraWrapper)
            {
                UpdateCameraMapCoordinates((MapCameraWrapper)node);
            }
        }

        private void viewIntroCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Sort list by camera number/id
            scene.IntroCameras.Sort((x, y) => x.CameraNum.CompareTo(y.CameraNum));

            foreach (var camera in scene.IntroCameras)
            {
                var pathMove = scene.Paths[camera.Camera_Path];
                var pathLookAt = scene.Paths[camera.Camera_AtPath];

                //The time elapsed for each point
                int PathTime = camera.CameraTime / pathMove.PathPoints.Count;

                //Go through each point
                for (int p = 0; p < pathMove.PathPoints.Count; p++)
                {
                    //If lookat path is higher than the move path, break
                    if (pathLookAt.PathPoints.Count >= p)
                        break;

                    //Set our points
                    var pathMovePoint = pathMove.PathPoints[p];
                    var pathLookAtPoint = pathLookAt.PathPoints[p];

                    for (int frame = 0; frame < PathTime; frame++)
                    {
                        if (viewport.GL_Control != null)
                        {
                          //  viewport.GL_ControlModern.CameraEye = pathLookAtPoint.Translate;
                            viewport.GL_Control.CameraTarget = pathMovePoint.Translate;

                            viewport.UpdateViewport();
                        }
                    }
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            List<EditableObject> newSelection = new List<EditableObject>();

            TreeNode node = treeView1.SelectedNode; 
            if (node == null)
                return;


            if (node.Text == "Scene")
            {
                stPropertyGrid1.LoadProperty(scene, OnPropertyChanged);
            }
            else if (node is MapCameraWrapper)
            {
                stPropertyGrid1.LoadProperty( ((MapCameraWrapper)node).CameraFile.cameraData, OnPropertyChanged);
            }
            else if (node is ProbeLightingWrapper)
            {
            }
            else if (node is ProbeLightingEntryWrapper)
            {
                var parent = (ProbeLightingWrapper)node.Parent;

                foreach (var child in parent.Nodes)
                {
                   ( (ProbeLightingEntryWrapper)child).entry.Grid.GridColor = new Vector3(9, 0, 0);
                }

                var probeEntry = (ProbeLightingEntryWrapper)node;
                probeEntry.entry.Grid.GridColor = new Vector3(1,0,0);

                stPropertyGrid1.LoadProperty(probeEntry.entry, OnPropertyChanged);
            }
            else if (node is PathCollectionNode)
            {
                foreach (var group in ((PathCollectionNode)node).Nodes)
                {
                    foreach (var point in ((PathGroupNode)group).Nodes)
                    {
                        newSelection.Add(((PathPointNode)point).PathPoint.RenderablePoint);
                    }
                }
            }
            else if (node is PathGroupNode)
            {
                foreach (var point in ((PathGroupNode)node).Nodes)
                {
                    newSelection.Add(((PathPointNode)point).PathPoint.RenderablePoint);
                }
            }
            else if (node is PathPointNode)
            {
                newSelection.Add(((PathPointNode)node).PathPoint.RenderablePoint);
            }

            if (newSelection.Count > 0)
            {
                foreach (var ob in newSelection)
                    viewport.scene.ToogleSelected(ob, true);

                viewport.UpdateViewport();
            }
        }

        bool IsParentChecked = false;
        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e) {
            if (!IsLoaded || IsParentChecked)
                return;

            IsParentChecked = true;
            CheckChildNodes(e.Node, e.Node.Checked);
            IsParentChecked = false; //Update viewport on the last node checked

            viewport.UpdateViewport();
        }

        private void CheckChildNodes(TreeNode node, bool IsChecked)
        {
            OnNodeChecked(node, IsChecked);
            foreach (TreeNode n in node.Nodes)
            {
                n.Checked = IsChecked;
                OnNodeChecked(n, IsChecked);
                if (n.Nodes.Count > 0)
                {
                    CheckChildNodes(n, IsChecked);
                }
            }
        }

        private void OnNodeChecked(TreeNode node, bool IsChecked)
        {
            if (node is PathPointNode)
                ((PathPointNode)node).OnChecked(IsChecked);
            if (node is ProbeLightingEntryWrapper)
                ((ProbeLightingEntryWrapper)node).OnChecked(IsChecked);
        }
    }
}
