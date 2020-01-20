using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Toolbox.Library;
using GL_EditorFramework.Interfaces;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Toolbox.Library.Rendering;
using System.Drawing;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using FirstPlugin.Forms;
using ByamlExt.Byaml;

namespace FirstPlugin
{
    public class KCL : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Collision;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "WiiU/Switch Collision" };
        public string[] Extension { get; set; } = new string[] { "*.kcl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
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

        private byte[] data;
        private STToolStripItem EndiannessToolstrip;

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

        private void OpenMaterialEditor(object sender, EventArgs args)
        {
            CollisionMaterialEditor editor = new CollisionMaterialEditor();
            editor.LoadCollisionValues(kcl, Renderer);

            if (editor.ShowDialog() == DialogResult.OK)
            {

            }
        }

        public DrawableContainer DrawableContainer = new DrawableContainer();

        public BymlFileData AttributeByml = null;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            Renderer = new KCLRendering();

            DrawableContainer = new DrawableContainer()
            {
                Name = FileName,
                Drawables = new List<AbstractGlDrawable>() { Renderer },
            };

            stream.Position = 0;
            data = stream.ToArray();
            Read(data);
        }

        class MenuExt : IFileMenuExtension
        {
            public STToolStripItem[] NewFileMenuExtensions => null;
            public STToolStripItem[] NewFromFileMenuExtensions => newFileExt;
            public STToolStripItem[] ToolsMenuExtensions => toolExt;
            public STToolStripItem[] TitleBarExtensions => null;
            public STToolStripItem[] CompressionMenuExtensions => null;
            public STToolStripItem[] ExperimentalMenuExtensions => null;
            public STToolStripItem[] EditMenuExtensions => null;
            public ToolStripButton[] IconButtonMenuExtensions => null;

            STToolStripItem[] newFileExt = new STToolStripItem[2];
            STToolStripItem[] toolExt = new STToolStripItem[1];

            public MenuExt()
            {
                newFileExt[0] = new STToolStripItem("KCL (Switch)", CreateNew);
                newFileExt[1] = new STToolStripItem("KCL (Wii U)", CreateNew);

                toolExt[0] = new STToolStripItem("KCL (Monoscript MKT) to OBJ", MontoscriptToOBJ);
            }

            public void MontoscriptToOBJ(object sender, EventArgs args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "All files(*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    KclMonoscript monscript = new KclMonoscript();
                    monscript.ReadKCL(ofd.FileName);

                    SaveFileDialog sfd = new SaveFileDialog();
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        var model = new STGenericModel();
                        var mesh = new STGenericObject();
                        mesh.faces = new List<int>();
                        var meshes = new List<STGenericObject>();
                        model.Objects = meshes;

                        List<uint> Attributes = new List<uint>();


                        int ft = 0;
                        foreach (var prisim in monscript.Prisims)
                        {
                            if (!Attributes.Contains(prisim.CollisionType))
                            {
                                Attributes.Add(prisim.CollisionType);
                                mesh = new STGenericObject();
                                mesh.Text = prisim.CollisionType.ToString("X");
                                meshes.Add(mesh);
                            }

                            var triangle = monscript.GetTriangle(prisim);
                            var normal = triangle.Normal;
                            var pointA = triangle.PointA;
                            var pointB = triangle.PointB;
                            var pointC = triangle.PointC;

                            Vertex vtx = new Vertex();
                            Vertex vtx2 = new Vertex();
                            Vertex vtx3 = new Vertex();

                            vtx.pos = pointA;
                            vtx2.pos = pointB;
                            vtx3.pos = pointC;
                            vtx.nrm = normal;
                            vtx2.nrm = normal;
                            vtx3.nrm = normal;

                            mesh.faces.Add(ft);
                            mesh.faces.Add(ft + 1);
                            mesh.faces.Add(ft + 2);
                            mesh.vertices.Add(vtx);
                            mesh.vertices.Add(vtx2);
                            mesh.vertices.Add(vtx3);


                            ft += 3;
                        }

                        OBJ.ExportModel(sfd.FileName, model, new List<STGenericTexture>());
                    }
                }
            }

            public void CreateNew(object sender, EventArgs args)
            {
                var ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                if (sender.ToString() == "KCL (Wii U)")
                    ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                OpenFileDialog opn = new OpenFileDialog();
                opn.Filter = "Supported Formats|*.obj";
                if (opn.ShowDialog() != DialogResult.OK) return;
                var mod = EditorCore.Common.OBJ.Read(new MemoryStream(File.ReadAllBytes(opn.FileName)), null);
                var f = MarioKart.MK7.KCL.FromOBJ(mod);

                string name = System.IO.Path.GetFileNameWithoutExtension(opn.FileName);

                KCL kcl = new KCL();
                kcl.Text = name;
                kcl.IFileInfo = new IFileInfo();
                kcl.FileName = name;
                kcl.Renderer = new KCLRendering();

                kcl.DrawableContainer = new DrawableContainer()
                {
                    Name = kcl.FileName,
                    Drawables = new List<AbstractGlDrawable>() { kcl.Renderer },
                };

                kcl.Read(f.Write(ByteOrder));

                ObjectEditor editor = new ObjectEditor(kcl);
                editor.Text = name;
                LibraryGUI.CreateMdiWindow(editor);
            }
        }

        public void Unload()
        {
            ObjectEditor.RemoveContainer(DrawableContainer);
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {
                writer.Write(data);
            }

            if (AttributeByml != null)
                SaveAttributeByml();
        }

        private void SaveAttributeByml(bool UpdateArchive = false)
        {
            string byml = $"{Path.GetFileNameWithoutExtension(Text)}Attribute.byml";
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files) {
                    if (file.FileName == byml) {
                        var mem = new MemoryStream();
                        ByamlFile.SaveN(mem, new BymlFileData
                        {
                            Version = AttributeByml.Version,
                            byteOrder = AttributeByml.byteOrder,
                            SupportPaths = AttributeByml.SupportPaths,
                            RootNode = AttributeByml.RootNode
                        });

                        file.FileData = mem.ToArray();
                        //Reload the file format
                        if (file.FileFormat != null) {
                            file.FileFormat = null;
                            file.FileFormat = file.OpenFile();
                        }
                    }
                }
            }
            else if (!UpdateArchive)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Supported Formats|*.byml";
                sfd.FileName = byml;
                sfd.DefaultExt = ".byml";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var fileStream = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write)) {
                        ByamlFile.SaveN(fileStream, new BymlFileData
                        {
                            Version = AttributeByml.Version,
                            byteOrder = AttributeByml.byteOrder,
                            SupportPaths = AttributeByml.SupportPaths,
                            RootNode = AttributeByml.RootNode
                        });
                    }
                }
            }
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

        private void Export(object sender, EventArgs args)
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

        private void Replace(object sender, EventArgs args)
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
                AttributeByml = kcl.AttributeByml;
                data = kcl.Write(Endianness);
                Read(data);
                Renderer.UpdateVertexData();
                SaveAttributeByml(true);
            }
        }

        private void SwapEndianess(object sender, EventArgs args)
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

        private Viewport viewport
        {
            get
            {
                var editor = LibraryGUI.GetObjectEditor();
                return editor.GetViewport();
            }
            set
            {
                var editor = LibraryGUI.GetObjectEditor();
                editor.LoadViewport(value);
            }
        }

        private KCLRendering Renderer;
        bool DrawablesLoaded = false;
        public override void OnClick(TreeView treeView)
        {
            if (Runtime.UseOpenGL)
            {
                if (viewport == null)
                {
                    viewport = new Viewport(ObjectEditor.GetDrawableContainers());
                    viewport.Dock = DockStyle.Fill;
                }

                if (!DrawablesLoaded)
                {
                    ObjectEditor.AddContainer(DrawableContainer);
                    DrawablesLoaded = true;
                }

                viewport.ReloadDrawables(DrawableContainer);
                LibraryGUI.LoadEditor(viewport);

                viewport.Text = Text;
            }
        }

        private MarioKart.MK7.KCL kcl;
        private void Read(byte[] file_data)
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

        private void LoadModelTree(TreeNode parent, MarioKart.ModelOctree[] modelOctrees)
        {
            if (modelOctrees == null)
                return;

            foreach (var model in modelOctrees)
            {
                OctreeNode modelNode = new OctreeNode(model, model.Key.ToString("X"));
                parent.Nodes.Add(modelNode);
                LoadModelTree(modelNode, model.Children);
            }
        }

        public class OctreeNode : TreeNodeCustom
        {
            public List<OctreeNode> Children
            {
                get
                {
                    List<OctreeNode> trees = new List<OctreeNode>();
                    foreach (var node in Nodes)
                        trees.Add((OctreeNode)node);
                    return trees;
                }
            }

            MarioKart.ModelOctree Octree;

            public OctreeNode(MarioKart.ModelOctree octree, string name)
            {
                Octree = octree;
                Text = name;
            }

            public override void OnClick(TreeView treeview)
            {

            }
        }

        public List<KCLModel> GetKclModels()
        {
            return Renderer.models;
        }

        private void Read(MarioKart.MK7.KCL kcl)
        {
            Vector3 min = new Vector3();
            Vector3 max = new Vector3();

            Nodes.Clear();
            Renderer.OctreeNodes.Clear();
            Renderer.models.Clear();
            Renderer.KclFile = kcl;

            TreeNode modelTree = new TreeNode("Model Octree");
            LoadModelTree(modelTree, kcl.GlobalHeader.ModelOctrees);
            foreach (var node in modelTree.Nodes)
                Renderer.OctreeNodes.Add((OctreeNode)node);
            Nodes.Add(modelTree);

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