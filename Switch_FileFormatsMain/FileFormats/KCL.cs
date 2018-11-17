using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using SFGraphics.GLObjects.Shaders;
using Smash_Forge.Rendering;
using GL_Core.Interfaces;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Switch_Toolbox.Library.Rendering;
using WeifenLuo.WinFormsUI.Docking;
using GL_Core;
using System.Drawing;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class KCL : IFileFormat
    {
        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "KCL" };
        public string[] Extension { get; set; } = new string[] { "*.kcl" };
        public string Magic { get; set; } = "";
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public TreeNodeFile EditorRoot { get; set; }
        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public void Load()
        {
            IsActive = true;
            EditorRoot = new KCLRoot(FileName, this);
            IFileInfo = new IFileInfo();
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            KCLRoot root = (KCLRoot)EditorRoot;
            return root.kcl.Write(Syroot.BinaryData.ByteOrder.LittleEndian);
        }

        private static void SaveCompressFile(byte[] data, string FileName, CompressionType CompressionType, int Alignment = 0, bool EnableDialog = true)
        {
            if (EnableDialog && CompressionType != CompressionType.None)
            {
                DialogResult save = MessageBox.Show($"Compress file as {CompressionType}?", "File Save", MessageBoxButtons.YesNo);

                if (save == DialogResult.Yes)
                {
                    switch (CompressionType)
                    {
                        case CompressionType.Yaz0:
                            data = EveryFileExplorer.YAZ0.Compress(data, Runtime.Yaz0CompressionLevel, (uint)Alignment);
                            break;
                        case CompressionType.Lz4f:
                            data = STLibraryCompression.Type_LZ4F.Compress(data);
                            break;
                        case CompressionType.Lz4:
                            break;
                    }
                }
            }
            File.WriteAllBytes(FileName, data);
            MessageBox.Show($"File has been saved to {FileName}");
            Cursor.Current = Cursors.Default;
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

        public class KCLRoot : TreeNodeFile
        {
            public KCLRoot(string Name, IFileFormat handler)
            {
                Text = Name;
                FileHandler = handler;
                Renderer = new KCLRendering();
                Read(handler.Data);

                ContextMenu = new ContextMenu();
                MenuItem save = new MenuItem("Save");
                ContextMenu.MenuItems.Add(save);
                save.Click += Save;
                MenuItem export = new MenuItem("Export");
                ContextMenu.MenuItems.Add(export);
                export.Click += Export;
                MenuItem replace = new MenuItem("Replace");
                ContextMenu.MenuItems.Add(replace);
                replace.Click += Replace;
            }
            public void Save(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Supported Formats|*.kcl";
                sfd.FileName = Text;
                sfd.DefaultExt = ".kcl";

                if (sfd.ShowDialog() == DialogResult.OK)
                {

                    int Alignment = FileHandler.IFileInfo.Alignment;
                    SaveCompressFile(FileHandler.Save(), sfd.FileName, FileHandler.CompressionType, Alignment);
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
                    Read(kcl.Write(Syroot.BinaryData.ByteOrder.LittleEndian));
                }
            }

            KCLRendering Renderer;
            public override void OnClick(TreeView treeView)
            {
                Renderer.LoadViewport();
                Renderer.UpdateVertexData();
            }

            public MarioKart.MK7.KCL kcl = null;
            public void Read(byte[] file_data)
            {
                try
                {
                    kcl = new MarioKart.MK7.KCL(file_data, Syroot.BinaryData.ByteOrder.LittleEndian);
                }
                catch
                {
                    kcl = new MarioKart.MK7.KCL(file_data, Syroot.BinaryData.ByteOrder.BigEndian);
                }
                Read(kcl);
                Renderer.UpdateVertexData();
            }
            public void Read(MarioKart.MK7.KCL kcl)
            {
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
                    }

                    Renderer.models.Add(kclmodel);
                    Nodes.Add(kclmodel);

                    CurModelIndx++;
                }
            }
        }

        public class KCLRendering : AbstractGlDrawable
        {
            // gl buffer objects
            int vbo_position;
            int ibo_elements;

            //Set the game's material list
            public GameSet GameMaterialSet = GameSet.MarioKart8D;
            public List<KCLModel> models = new List<KCLModel>();
            public Shader shader = null;

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
                if (OpenTKSharedResources.SetupStatus == OpenTKSharedResources.SharedResourceStatus.Unitialized)
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

                Viewport.Instance.UpdateViewport();
            }

            string FileName;
            public void LoadViewport()
            {
                if (!EditorIsViewportActive(FirstPlugin.DockedViewport))
                {
                    Viewport.Instance.gL_ControlModern1.MainDrawable = this;
                    FirstPlugin.DockedViewport.Text = FileName;
                    FirstPlugin.DockedViewport = Viewport.Instance;
                }
            }
            public bool EditorIsViewportActive(DockContent dock)
            {
                if (dock is Viewport)
                {
                    dock.Text = FileName;
                    ((Viewport)dock).gL_ControlModern1.MainDrawable = this;
                    return true;
                }
                return false;
            }

            public override void Prepare(GL_ControlModern control)
            {
            }

            public override void Prepare(GL_ControlLegacy control)
            {

            }
            public override void Draw(GL_ControlLegacy control)
            {
                    
            }
            public override void Draw(GL_ControlModern control)
            {
                bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
                if (!buffersWereInitialized)
                    GenerateBuffers();

                if (OpenTKSharedResources.SetupStatus == OpenTKSharedResources.SharedResourceStatus.Unitialized)
                    return;

                shader = OpenTKSharedResources.shaders["KCL"];
                shader.UseProgram();

                shader.EnableVertexAttributes();
                SetRenderSettings(shader);

                Matrix4 previewScale = Utils.TransformValues(Vector3.Zero, Vector3.Zero, Runtime.previewScale);

                Matrix4 camMat = previewScale * control.mtxCam * control.mtxProj;

                shader.SetVector3("difLightDirection", Vector3.TransformNormal(new Vector3(0f, 0f, -1f), camMat.Inverted()).Normalized());
                shader.SetVector3("difLightColor", new Vector3(1));
                shader.SetVector3("ambLightColor", new Vector3(1));

                shader.EnableVertexAttributes();
                SetRenderSettings(shader);

                shader.SetMatrix4x4("mvpMatrix", ref camMat);

                foreach (KCLModel mdl in models)
                {
                    DrawModel(mdl, shader);
                }

                shader.DisableVertexAttributes();
            }
            private void SetRenderSettings(Shader shader)
            {
                shader.SetBoolToInt("renderVertColor", Runtime.renderVertColor);
                shader.SetInt("renderType", (int)Runtime.viewportShading);
            }
            private void DrawModel(KCLModel m, Shader shader, bool drawSelection = false)
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
            private static void DrawModelSelection(KCLModel p, Shader shader)
            {
                //This part needs to be reworked for proper outline. Currently would make model disappear

                GL.DrawElements(PrimitiveType.Triangles, p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);

                GL.Enable(EnableCap.StencilTest);
                // use vertex color for wireframe color
                shader.SetInt("colorOverride", 1);
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
                GL.Enable(EnableCap.LineSmooth);
                GL.LineWidth(1.5f);
                GL.DrawElements(PrimitiveType.Triangles, p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                shader.SetInt("colorOverride", 0);

                GL.Enable(EnableCap.DepthTest);
            }
            private void SetVertexAttributes(KCLModel m, Shader shader)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
                GL.VertexAttribPointer(shader.GetAttribLocation("vPosition"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 0);
                GL.VertexAttribPointer(shader.GetAttribLocation("vNormal"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 12);
                GL.VertexAttribPointer(shader.GetAttribLocation("vColor"), 3, VertexAttribPointerType.Float, false, DisplayVertex.Size, 24);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            }
            private static void DrawModelWireframe(KCLModel p, Shader shader)
            {
                // use vertex color for wireframe color
                shader.SetInt("colorOverride", 1);
                GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
                GL.Enable(EnableCap.LineSmooth);
                GL.LineWidth(1.5f);
                GL.DrawElements(PrimitiveType.Triangles, p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                shader.SetInt("colorOverride", 0);
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
