using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using GL_EditorFramework.Interfaces;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Switch_Toolbox.Library.Rendering;
using GL_EditorFramework.GL_Core;
using System.Drawing;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using GL_EditorFramework.EditorDrawables;
using FirstPlugin.Forms;
using static GL_EditorFramework.EditorDrawables.EditorSceneBase;

namespace FirstPlugin
{
    public class KCL : TreeNodeFile, IFileFormat
    {
        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "KCL" };
        public string[] Extension { get; set; } = new string[] { "*.kcl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                return reader.ReadUInt32() == 0x02020000;
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(MenuExt));
                return types.ToArray();
            }
        }

        byte[] data;

        STToolStripItem EndiannessToolstrip;

        STToolStripItem OdysseyPresetToolstrip;
        STToolStripItem Mk8PresetToolstrip;
        STToolStripItem Splatoon2PresetToolstrip;
        STToolStripItem SplatoonPresetToolstrip;
        STToolStripItem OtherPresetToolstrip;

        public KCL()
        {
            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new STToolStripItem("Save", Save));
            ContextMenuStrip.Items.Add(new STToolStripItem("Export", Export));
            ContextMenuStrip.Items.Add(new STToolStripItem("Replace", Replace));
            ContextMenuStrip.Items.Add(new STToolStripItem("Open Material Editor", OpenMaterialEditor));
            EndiannessToolstrip = new STToolStripItem("Big Endian Mode", SwapEndianess) { Checked = true };
            ContextMenuStrip.Items.Add(EndiannessToolstrip);
            CanSave = true;
            IFileInfo = new IFileInfo();
        }

        public void OpenMaterialEditor(object sender, EventArgs args)
        {
            CollisionMaterialEditor editor = new CollisionMaterialEditor();
            editor.LoadCollisionValues(kcl, Renderer);

            if (editor.ShowDialog() == DialogResult.OK)
            {

            }
        }

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            Renderer = new KCLRendering();

            stream.Position = 0;
            data = stream.ToArray();
            Read(data);
        }

        class MenuExt : IFileMenuExtension
        {
            public STToolStripItem[] NewFileMenuExtensions => null;
            public STToolStripItem[] NewFromFileMenuExtensions => newFileExt;
            public STToolStripItem[] ToolsMenuExtensions => null;
            public STToolStripItem[] TitleBarExtensions => null;
            public STToolStripItem[] CompressionMenuExtensions => null;
            public STToolStripItem[] ExperimentalMenuExtensions => null;
            public STToolStripItem[] EditMenuExtensions => null;
            public ToolStripButton[] IconButtonMenuExtensions => null;

            STToolStripItem[] newFileExt = new STToolStripItem[2];

            public MenuExt()
            {
                newFileExt[0] = new STToolStripItem("KCL (Switch)", CreateNew);
                newFileExt[1] = new STToolStripItem("KCL (Wii U)", CreateNew);
            }

            public void CreateNew(object sender, EventArgs args)
            {
                var ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                if (sender.ToString() == "KCL (Wii U)")
                    ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                OpenFileDialog opn = new OpenFileDialog();
                if (opn.ShowDialog() != DialogResult.OK) return;
                var mod = EditorCore.Common.OBJ.Read(new MemoryStream(File.ReadAllBytes(opn.FileName)), null);
                var f = MarioKart.MK7.KCL.FromOBJ(mod);

                string name = System.IO.Path.GetFileNameWithoutExtension(opn.FileName);

                KCL kcl = new KCL();
                kcl.Text = name;
                kcl.IFileInfo = new IFileInfo();
                kcl.FileName = name;
                kcl.Renderer = new KCLRendering();
                kcl.Read(f.Write(ByteOrder));

                ObjectEditor editor = new ObjectEditor();
                editor.Text = name;
                editor.treeViewCustom1.Nodes.Add(kcl);
                LibraryGUI.Instance.CreateMdiWindow(editor);
            }
        }

        public void Unload()
        {

        }
        public byte[] Save()
        {
            return data;
        }
        public enum GameSet : ushort
        {
            MarioOdyssey = 0x0,
            MarioKart8D = 0x1,
            Splatoon2 = 0x2,
        }

        public enum CollisionType_MarioOdssey : ushort
        {

        }
        public enum CollisionType_MK8D : ushort
        {
            Road_Default = 0,
            Road_Bumpy = 2,
            Road_Sand = 4,
            Offroad_Sand = 6,
            Road_HeavySand = 8,
            Road_IcyRoad = 9,
            OrangeBooster = 10,
            AntiGravityPanel = 11,
            Latiku = 16,
            Wall5 = 17,
            Wall4 = 19,
            Wall = 23,
            Latiku2 = 28,
            Glider = 31,
            SidewalkSlope = 32,
            Road_Dirt = 33,
            Unsolid = 56,
            Water = 60,
            Road_Stone = 64,
            Wall1 = 81,
            Wall2 = 84,
            FinishLine = 93,
            RedFlowerEffect = 95,
            Wall3 = 113,
            WhiteFlowerEffect = 127,
            Road_Metal = 128,
            Road_3DS_MP_Piano = 129,
            Road_RoyalR_Grass = 134,
            TopPillar = 135,
            YoshiCuiruit_Grass = 144,
            YellowFlowerEffect = 159,

            Road_MetalGating = 160,
            Road_3DS_MP_Xylophone = 161,
            Road_3DS_MP_Vibraphone = 193,
            SNES_RR_road = 227,
            Offroad_Mud = 230,
            Trick = 4096,
            BoosterStunt = 4106,
            TrickEndOfRamp = 4108,
            Trick3 = 4130,
            Trick6 = 4160,
            Trick4 = 4224,
            Trick5 = 8192,
            BoostTrick = 8202,
        }

        public void Save(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.kcl";
            sfd.FileName = Text;
            sfd.DefaultExt = ".kcl";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        private Syroot.BinaryData.ByteOrder endianness;
        public Syroot.BinaryData.ByteOrder Endianness
        {
            get
            {
                return endianness;
            }
            set
            {
                endianness = value;
                if (value == Syroot.BinaryData.ByteOrder.BigEndian)
                    EndiannessToolstrip.Checked = true;
                else
                    EndiannessToolstrip.Checked = false;
            }
        }

        public void Export(object sender, EventArgs args)
        {
            if (kcl == null)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.obj";
            sfd.FileName = Text;
            sfd.DefaultExt = ".obj";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                kcl.ToOBJ().toWritableObj().WriteObj(sfd.FileName + ".obj");
            }
        }

        public void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.obj";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var mod = EditorCore.Common.OBJ.Read(new MemoryStream(File.ReadAllBytes(ofd.FileName)), null);
                if (mod.Faces.Count > 65535)
                {
                    MessageBox.Show("this model has too many faces, only models with less than 65535 triangles can be converted");
                    return;
                }
                kcl = MarioKart.MK7.KCL.FromOBJ(mod);
                data = kcl.Write(Endianness);
                Read(data);
                Renderer.UpdateVertexData();
            }
        }

        public void SwapEndianess(object sender, EventArgs args)
        {
            if (EndiannessToolstrip.Checked)
            {
                EndiannessToolstrip.Checked = false;
                Endianness = Syroot.BinaryData.ByteOrder.LittleEndian;
            }
            else
            {
                EndiannessToolstrip.Checked = true;
                Endianness = Syroot.BinaryData.ByteOrder.BigEndian;
            }
        }

        Viewport viewport
        {
            get
            {
                var editor = LibraryGUI.Instance.GetObjectEditor();
                return editor.GetViewport();
            }
            set
            {
                var editor = LibraryGUI.Instance.GetObjectEditor();
                editor.LoadViewport(value);
            }
        }

        public KCLRendering Renderer;
        bool IsLoaded = false;
        public override void OnClick(TreeView treeView)
        {
            if (Runtime.UseOpenGL)
            {
                if (viewport == null)
                {
                    viewport = new Viewport();
                    viewport.Dock = DockStyle.Fill;
                }
                LibraryGUI.Instance.LoadEditor(viewport);

                viewport.Text = Text;

                if (!IsLoaded)
                {
                    viewport.AddDrawable(Renderer);
                    viewport.LoadObjects();
                }

                IsLoaded = true;
            }
        }

        public MarioKart.MK7.KCL kcl = null;
        public void Read(byte[] file_data)
        {
            data = file_data;

            try
            {
                Endianness = Syroot.BinaryData.ByteOrder.LittleEndian;
                kcl = new MarioKart.MK7.KCL(file_data, Syroot.BinaryData.ByteOrder.LittleEndian);
            }
            catch
            {
                Endianness = Syroot.BinaryData.ByteOrder.BigEndian;
                kcl = new MarioKart.MK7.KCL(file_data, Syroot.BinaryData.ByteOrder.BigEndian);
            }
            Read(kcl);
        }
        public void Read(MarioKart.MK7.KCL kcl)
        {
            Vector3 min = new Vector3();
            Vector3 max = new Vector3();

            Nodes.Clear();
            Renderer.models.Clear();

            int CurModelIndx = 0;
            foreach (MarioKart.MK7.KCL.KCLModel mdl in kcl.Models)
            {
                KCLModel kclmodel = new KCLModel();

                kclmodel.Text = "Model " + CurModelIndx;

                int ft = 0;
                foreach (var plane in mdl.Planes)
                {
                    var triangle = mdl.GetTriangle(plane);
                    var normal = triangle.Normal;
                    var pointA = triangle.PointA;
                    var pointB = triangle.PointB;
                    var pointC = triangle.PointC;

                    Vertex vtx = new Vertex();
                    Vertex vtx2 = new Vertex();
                    Vertex vtx3 = new Vertex();

                    vtx.pos = new Vector3(Vec3D_To_Vec3(pointA));
                    vtx2.pos = new Vector3(Vec3D_To_Vec3(pointB));
                    vtx3.pos = new Vector3(Vec3D_To_Vec3(pointC));
                    vtx.nrm = new Vector3(Vec3D_To_Vec3(normal));
                    vtx2.nrm = new Vector3(Vec3D_To_Vec3(normal));
                    vtx3.nrm = new Vector3(Vec3D_To_Vec3(normal));

                    KCLModel.Face face = new KCLModel.Face();
                    face.Text = triangle.Collision.ToString();
                    face.MaterialFlag = triangle.Collision;

                    var col = MarioKart.MK7.KCLColors.GetMaterialColor(plane.CollisionType);
                    Vector3 ColorSet = new Vector3(col.R, col.G, col.B);

                    vtx.col = new Vector4(ColorSet, 1);
                    vtx2.col = new Vector4(ColorSet, 1);
                    vtx3.col = new Vector4(ColorSet, 1);

                    kclmodel.faces.Add(ft);
                    kclmodel.faces.Add(ft + 1);
                    kclmodel.faces.Add(ft + 2);

                    ft += 3;

                    kclmodel.vertices.Add(vtx);
                    kclmodel.vertices.Add(vtx2);
                    kclmodel.vertices.Add(vtx3);

                    #region FindMaxMin
                    if (triangle.PointA.X < min.X) min.X = (float)triangle.PointA.X;
                    if (triangle.PointA.Y < min.Y) min.Y = (float)triangle.PointA.Y;
                    if (triangle.PointA.Z < min.Z) min.Z = (float)triangle.PointA.Z;
                    if (triangle.PointA.X > max.X) max.X = (float)triangle.PointA.X;
                    if (triangle.PointA.Y > max.Y) max.Y = (float)triangle.PointA.Y;
                    if (triangle.PointA.Z > max.Z) max.Z = (float)triangle.PointA.Z;

                    if (triangle.PointB.X < min.X) min.X = (float)triangle.PointB.X;
                    if (triangle.PointB.Y < min.Y) min.Y = (float)triangle.PointB.Y;
                    if (triangle.PointB.Z < min.Z) min.Z = (float)triangle.PointB.Z;
                    if (triangle.PointB.X > max.X) max.X = (float)triangle.PointB.X;
                    if (triangle.PointB.Y > max.Y) max.Y = (float)triangle.PointB.Y;
                    if (triangle.PointB.Z > max.Z) max.Z = (float)triangle.PointB.Z;

                    if (triangle.PointC.X < min.X) min.X = (float)triangle.PointC.X;
                    if (triangle.PointC.Y < min.Y) min.Y = (float)triangle.PointC.Y;
                    if (triangle.PointC.Z < min.Z) min.Z = (float)triangle.PointC.Z;
                    if (triangle.PointC.X > max.X) max.X = (float)triangle.PointC.X;
                    if (triangle.PointC.Y > max.Y) max.Y = (float)triangle.PointC.Y;
                    if (triangle.PointC.Z > max.Z) max.Z = (float)triangle.PointC.Z;
                    #endregion
                }

                Renderer.Max = max;
                Renderer.Min = min;
                Renderer.models.Add(kclmodel);
                Nodes.Add(kclmodel);

                CurModelIndx++;
            }
        }

        public class KCLRendering : EditableObject
        {
            public Vector3 Max = new Vector3(0);
            public Vector3 Min = new Vector3(0);

            public List<ushort> SelectedTypes = new List<ushort>();

            public Vector3 position = new Vector3(0, 0, 0);

            protected bool Selected = false;
            protected bool Hovered = false;

            public override bool IsSelected() => Selected;
            public override bool IsSelected(int partIndex) => Selected;

            public bool IsHovered() => Selected;

            // gl buffer objects
            int vbo_position;
            int ibo_elements;

            //Set the game's material list
            public GameSet GameMaterialSet = GameSet.MarioKart8D;
            public List<KCLModel> models = new List<KCLModel>();

            private void GenerateBuffers()
            {
                GL.GenBuffers(1, out vbo_position);
                GL.GenBuffers(1, out ibo_elements);
            }

            public void Destroy()
            {
                GL.DeleteBuffer(vbo_position);
                GL.DeleteBuffer(ibo_elements);
            }

            public void UpdateVertexData()
            {
                if (!Runtime.OpenTKInitialized)
                    return;

                DisplayVertex[] Vertices;
                int[] Faces;

                int poffset = 0;
                int voffset = 0;
                List<DisplayVertex> Vs = new List<DisplayVertex>();
                List<int> Ds = new List<int>();
                foreach (KCLModel m in models)
                {
                    m.Offset = poffset * 4;
                    List<DisplayVertex> pv = m.CreateDisplayVertices();
                    Vs.AddRange(pv);

                    for (int i = 0; i < m.displayFaceSize; i++)
                    {
                        Ds.Add(m.display[i] + voffset);
                    }
                    poffset += m.displayFaceSize;
                    voffset += pv.Count;
                }

                // Binds
                Vertices = Vs.ToArray();
                Faces = Ds.ToArray();

                // Bind only once!
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
                GL.BufferData<DisplayVertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * DisplayVertex.Size), Vertices, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
                GL.BufferData<int>(BufferTarget.ElementArrayBuffer, (IntPtr)(Faces.Length * sizeof(int)), Faces, BufferUsageHint.StaticDraw);

                LibraryGUI.Instance.UpdateViewport();
            }

            public void DrawGlobalOctree()
            {
                var size = Max - Min;
                var BoxSize = size / 2f;
                for (int k = 0; k < 2; k++)
                {
                    for (int l = 0; l < 2; l++)
                    {
                        for (int m = 0; m < 2; m++)
                        {
                            var Boxmin = Min + new Vector3(BoxSize.X * m, BoxSize.Y * l, BoxSize.Z * k);
                            var pos = new Vector3(BoxSize.X * m, BoxSize.Y * l, BoxSize.Z * k);
                        }
                    }
                }
            }

            public ShaderProgram defaultShaderProgram;
            public ShaderProgram solidColorShaderProgram;

            public override void Prepare(GL_ControlModern control)
            {
                string pathFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader") + "\\KCL.frag";
                string pathVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader") + "\\KCL.vert";

                var defaultFrag = new FragmentShader(File.ReadAllText(pathFrag));
                var defaultVert = new VertexShader(File.ReadAllText(pathVert));

                var solidColorFrag = new FragmentShader(
               @"#version 330
				uniform vec4 color;
				out vec4 FragColor;

				void main(){
					FragColor = color;
				}");

                var solidColorVert = new VertexShader(
              @"#version 330
                in vec3 vPosition;
                in vec3 vNormal;
                in vec3 vColor;

                out vec3 normal;
                out vec3 color;
                out vec3 position;

	            uniform mat4 mtxMdl;
				uniform mat4 mtxCam;

				void main(){
                    normal = vNormal;
                    color = vColor;
	                position = vPosition;

                    gl_Position = mtxMdl * mtxCam  * vec4(vPosition.xyz, 1.0);
				}");

                defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);
                solidColorShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert);

            }

            public override void Prepare(GL_ControlLegacy control)
            {
                string pathFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Legacy") + "\\KCL.frag";
                string pathVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Legacy") + "\\KCL.vert";

                var defaultFrag = new FragmentShader(File.ReadAllText(pathFrag));
                var defaultVert = new VertexShader(File.ReadAllText(pathVert));

                var solidColorFrag = new FragmentShader(
          @"#version 330
				uniform vec4 color;
                out vec4 FragColor;

				void main(){
					FragColor = color;
				}");

                var solidColorVert = new VertexShader(
              @"#version 330
                in vec3 vPosition;
                in vec3 vNormal;
                in vec3 vColor;

                out vec3 normal;
                out vec3 color;
                out vec3 position;

				void main(){
                    normal = vNormal;
                    color = vColor;
	                position = vPosition;

                    gl_Position = mvpMatrix * vec4(vPosition.xyz, 1.0);
				}");

                defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);
                solidColorShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert);
            }

            private void CheckBuffers()
            {
                if (!Runtime.OpenTKInitialized)
                    return;

                bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
                if (!buffersWereInitialized)
                {
                    GenerateBuffers();
                    UpdateVertexData();
                }
            }
            public override void Draw(GL_ControlLegacy control, Pass pass)
            {
                CheckBuffers();

                if (!Runtime.OpenTKInitialized)
                    return;
            }

            public override void Draw(GL_ControlModern control, Pass pass) { }

            public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
            {
                CheckBuffers();

                if (!Runtime.OpenTKInitialized)
                    return;

                control.CurrentShader = defaultShaderProgram;

                control.UpdateModelMatrix(
                Matrix4.CreateScale(Runtime.previewScale) *
                Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(position) : position));

                SetRenderSettings(defaultShaderProgram);

                Matrix4 camMat = control.ModelMatrix * control.CameraMatrix * control.ProjectionMatrix;

                GL.Disable(EnableCap.CullFace);

                GL.Uniform3(defaultShaderProgram["difLightDirection"], Vector3.TransformNormal(new Vector3(0f, 0f, -1f), camMat.Inverted()).Normalized());
                GL.Uniform3(defaultShaderProgram["difLightColor"], new Vector3(1));
                GL.Uniform3(defaultShaderProgram["ambLightColor"], new Vector3(1));

                defaultShaderProgram.EnableVertexAttributes();

                foreach (KCLModel mdl in models)
                {
                    DrawModel(mdl, defaultShaderProgram);
                }

                defaultShaderProgram.DisableVertexAttributes();

                GL.UseProgram(0);
                GL.Disable(EnableCap.DepthTest);
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.CullFace);

                DrawGlobalOctree();
            }
            private void SetRenderSettings(ShaderProgram shader)
            {
                shader.SetBoolToInt("renderVertColor", Runtime.renderVertColor);
                GL.Uniform1(defaultShaderProgram["renderType"], (int)Runtime.viewportShading);

            }
            private void DrawModel(KCLModel m, ShaderProgram shader, bool drawSelection = false)
            {
                if (m.faces.Count <= 3)
                    return;

                SetVertexAttributes(m, shader);

                if (m.Checked)
                {
                    if ((m.IsSelected))
                    {
                        DrawModelSelection(m, shader);
                    }
                    else
                    {
                        if (Runtime.RenderModelWireframe)
                        {
                            DrawModelWireframe(m, shader);
                        }

                        if (Runtime.RenderModels)
                        {
                            GL.DrawElements(PrimitiveType.Triangles, m.displayFaceSize, DrawElementsType.UnsignedInt, m.Offset);
                        }
                    }
                }
            }
            private static void DrawModelSelection(KCLModel p, ShaderProgram shader)
            {
                //This part needs to be reworked for proper outline. Currently would make model disappear

                GL.DrawElements(PrimitiveType.Triangles, p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);

                GL.Enable(EnableCap.StencilTest);
                // use vertex color for wireframe color
                GL.Uniform1(shader["colorOverride"], 1);
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
                GL.Enable(EnableCap.LineSmooth);
                GL.LineWidth(1.5f);
                GL.DrawElements(PrimitiveType.Triangles, p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Uniform1(shader["colorOverride"], 0);

                GL.Enable(EnableCap.DepthTest);
            }
            private void SetVertexAttributes(KCLModel m, ShaderProgram shader)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
                GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 0);
                GL.VertexAttribPointer(shader.GetAttribute("vNormal"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 12);
                GL.VertexAttribPointer(shader.GetAttribute("vColor"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 24);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            }
            private static void DrawModelWireframe(KCLModel p, ShaderProgram shader)
            {
                // use vertex color for wireframe color
                GL.Uniform1(shader["colorOverride"], 1);
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
                GL.Enable(EnableCap.LineSmooth);
                GL.LineWidth(1.5f);
                GL.DrawElements(PrimitiveType.Triangles, p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Uniform1(shader["colorOverride"], 0);
            }

            public override BoundingBox GetSelectionBox()
            {
                Vector3 Min = new Vector3(0);
                Vector3 Max = new Vector3(0);

                foreach (var model in models)
                {
                    foreach (var vertex in model.vertices)
                    {
                        Min.X = Math.Min(Min.X, vertex.pos.X);
                        Min.Y = Math.Min(Min.Y, vertex.pos.Y);
                        Min.Z = Math.Min(Min.Z, vertex.pos.Z);
                        Max.X = Math.Max(Max.X, vertex.pos.X);
                        Max.Y = Math.Max(Max.Y, vertex.pos.Y);
                        Max.Z = Math.Max(Max.Z, vertex.pos.Z);
                    }
                }

                return new BoundingBox()
                {
                    minX = Min.X,
                    minY = Min.Y,
                    minZ = Min.Z,
                    maxX = Max.X,
                    maxY = Max.Y,
                    maxZ = Max.Z,
                };
            }

            public override LocalOrientation GetLocalOrientation(int partIndex)
            {
                return new LocalOrientation(position);
            }

            public override bool TryStartDragging(DragActionType actionType, int hoveredPart, out LocalOrientation localOrientation, out bool dragExclusively)
            {
                localOrientation = new LocalOrientation(position);
                dragExclusively = false;
                return Selected;
            }

            public override bool IsInRange(float range, float rangeSquared, Vector3 pos)
            {
                range = 20000; //Make the range large for now. Todo go back to this

                BoundingBox box = GetSelectionBox();

                if (pos.X < box.maxX + range && pos.X > box.minX - range &&
                  pos.Y < box.maxY + range && pos.Y > box.minY - range &&
                  pos.Z < box.maxZ + range && pos.Z > box.minZ - range)
                    return true;

                return false;
            }


            public override uint SelectAll(GL_ControlBase control)
            {
                Selected = true;
                return REDRAW;
            }

            public override uint SelectDefault(GL_ControlBase control)
            {
                Selected = true;
                return REDRAW;
            }

            public override uint Select(int partIndex, GL_ControlBase control)
            {
                Selected = true;
                return REDRAW;
            }

            public override uint Deselect(int partIndex, GL_ControlBase control)
            {
                Selected = false;
                return REDRAW;
            }

            public override uint DeselectAll(GL_ControlBase control)
            {
                Selected = false;
                return REDRAW;
            }

            public override Vector3 Position
            {
                get
                {
                    return position;
                }
                set
                {
                    position = value;
                }
            }
        }

        //Convert KCL lib vec3 to opentk one so i can use the cross and dot methods
        public static Vector3 Vec3D_To_Vec3(System.Windows.Media.Media3D.Vector3D v)
        {
            return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
        }
        public struct DisplayVertex
        {
            // Used for rendering.
            public Vector3 pos;
            public Vector3 nrm;
            public Vector3 col;

            public static int Size = 4 * (3 + 3 + 3);
        }
        public class KCLModel : STGenericObject
        {
            public KCLModel()
            {
                ImageKey = "mesh";
                SelectedImageKey = "mesh";

                Checked = true;
            }

            public int[] display;
            public int Offset; // For Rendering

            public int strip = 0x40;
            public int displayFaceSize = 0;

            public class Face : TreeNode
            {
                public int MaterialFlag = 0;

            }

            public List<DisplayVertex> CreateDisplayVertices()
            {
                // rearrange faces
                display = getDisplayFace().ToArray();

                List<DisplayVertex> displayVertList = new List<DisplayVertex>();

                if (faces.Count <= 3)
                    return displayVertList;

                foreach (Vertex v in vertices)
                {
                    DisplayVertex displayVert = new DisplayVertex()
                    {
                        pos = v.pos,
                        nrm = v.nrm,
                        col = v.col.Xyz,
                    };

                    displayVertList.Add(displayVert);
                }

                return displayVertList;
            }

            public List<int> getDisplayFace()
            {
                if ((strip >> 4) == 4)
                {
                    displayFaceSize = faces.Count;
                    return faces;
                }
                else
                {
                    List<int> f = new List<int>();

                    int startDirection = 1;
                    int p = 0;
                    int f1 = faces[p++];
                    int f2 = faces[p++];
                    int faceDirection = startDirection;
                    int f3;
                    do
                    {
                        f3 = faces[p++];
                        if (f3 == 0xFFFF)
                        {
                            f1 = faces[p++];
                            f2 = faces[p++];
                            faceDirection = startDirection;
                        }
                        else
                        {
                            faceDirection *= -1;
                            if ((f1 != f2) && (f2 != f3) && (f3 != f1))
                            {
                                if (faceDirection > 0)
                                {
                                    f.Add(f3);
                                    f.Add(f2);
                                    f.Add(f1);
                                }
                                else
                                {
                                    f.Add(f2);
                                    f.Add(f3);
                                    f.Add(f1);
                                }
                            }
                            f1 = f2;
                            f2 = f3;
                        }
                    } while (p < faces.Count);

                    displayFaceSize = f.Count;
                    return f;
                }
            }
        }
    }
}
