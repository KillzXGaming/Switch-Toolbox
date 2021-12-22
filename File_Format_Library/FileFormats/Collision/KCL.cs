using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Toolbox.Library;
using GL_EditorFramework.Interfaces;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Toolbox.Library.Rendering;
using System.Threading;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using FirstPlugin.Forms;
using ByamlExt.Byaml;
using KclLibrary;
using KclLibraryGUI;
using Syroot.NintenTools.MarioKart8.Collisions;

namespace FirstPlugin
{
    public class KCL : TreeNodeFile, IContextMenuNode, IFileFormat
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
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true)) {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                return reader.ReadUInt32() == 0x02020000 || Utils.GetExtension(FileName) == ".kcl";
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

        public KCLFile KclFile { get; set; }

        public ToolStripItem[] GetContextMenuItems()
        {
            return new ToolStripItem[]
            {
                new ToolStripMenuItem("Save", null, Save, Keys.Control | Keys.S),
                new ToolStripMenuItem("Export", null, Export, Keys.Control | Keys.E),
                new ToolStripMenuItem("Replace", null, Replace, Keys.Control | Keys.R),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Big Endian Mode", null, SwapEndianess, Keys.Control | Keys.B)
                { Checked = (KclFile.ByteOrder == Syroot.BinaryData.ByteOrder.BigEndian), CheckOnClick = true },
            };
        }

        public KCL()
        {
            CanSave = true;
            IFileInfo = new IFileInfo();
        }

        public bool UseOverlay
        {
            get { return Renderer.UseOverlay; }
            set { Renderer.UseOverlay = value; }
        }

        public bool Visible
        {
            get { return Renderer.Visible; }
            set { Renderer.Visible = value; }
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
            KclFile = new KCLFile(stream);
            ReloadData();

            string path = Path.Combine(Runtime.ExecutableDir, "KclMaterialPresets");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            CollisionPresetData.LoadPresets(Directory.GetFiles(path));
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
                bool isBigEndian = false;
                if (sender.ToString() == "KCL (Wii U)")
                    isBigEndian = true;

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Supported Formats|*.obj";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                string path = Path.Combine(Runtime.ExecutableDir, "KclMaterialPresets");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                CollisionPresetData.LoadPresets(Directory.GetFiles("KclMaterialPresets"));

                var form = Runtime.MainForm;

                var thread = new Thread(() =>
                {
                    //Load runtime values to gui handler
                    MaterialWindowSettings.GamePreset = Runtime.CollisionSettings.KCLGamePreset;
                    MaterialWindowSettings.Platform = Runtime.CollisionSettings.KCLGamePreset;
                    MaterialWindowSettings.UsePresetEditor = Runtime.CollisionSettings.KCLUsePresetEditor;

                    var result = CollisionLoader.CreateCollisionFromObject(form, ofd.FileName);
                    CollisionLoader.CloseConsole(form);

                    if (result.KclFie == null) return;

                    SaveMaterialWindowSettings();

                    form.Invoke((MethodInvoker)delegate
                    {
                        string name = Path.GetFileNameWithoutExtension(ofd.FileName);

                        KCL kcl = new KCL();
                        kcl.KclFile = result.KclFie;
                        if (result.AttributeFile is MaterialAttributeBymlFile)
                            kcl.AttributeByml = ((MaterialAttributeBymlFile)result.AttributeFile).BymlFile;
                        kcl.Text = name;
                        kcl.IFileInfo = new IFileInfo();
                        kcl.FileName = name;
                        kcl.Renderer = new KCLRendering();
                        kcl.ReloadData();

                        kcl.DrawableContainer = new DrawableContainer()
                        {
                            Name = kcl.FileName,
                            Drawables = new List<AbstractGlDrawable>() { kcl.Renderer },
                        };

                        ObjectEditor editor = new ObjectEditor(kcl);
                        editor.Text = name;
                        LibraryGUI.CreateMdiWindow(editor);
                    });
                });
                thread.Start();
            }
        }

        public void Unload()
        {
            ObjectEditor.RemoveContainer(DrawableContainer);
        }

        public void Save(System.IO.Stream stream)
        {
            KclFile.Save(stream);
            SaveAttributeByml();
        }

        private void SaveAttributeByml(bool UpdateArchive = false)
        {
            if (AttributeByml == null || AttributeByml.RootNode == null)
                return;

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

        private void Export(object sender, EventArgs args)
        {
            if (KclFile == null)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.obj";
            sfd.FileName = Path.GetFileNameWithoutExtension(Text) + ".obj";
            sfd.DefaultExt = ".obj";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var obj = KclFile.CreateGenericModel();
                obj.Save(sfd.FileName);
            }
        }

        private void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.obj";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var form = Runtime.MainForm;

                var thread = new Thread(() =>
                {
                    //Load runtime values to gui handler
                    MaterialWindowSettings.GamePreset = Runtime.CollisionSettings.KCLGamePreset;
                    MaterialWindowSettings.Platform = GetPlatform();
                    MaterialWindowSettings.UsePresetEditor = Runtime.CollisionSettings.KCLUsePresetEditor;

                    var result = CollisionLoader.CreateCollisionFromObject(form, ofd.FileName);
                    CollisionLoader.CloseConsole(form);

                    if (result.KclFie == null) return;

                    SaveMaterialWindowSettings();

                    form.Invoke((MethodInvoker)delegate
                    {
                        KclFile = result.KclFie;
                        if (result.AttributeFile is MaterialAttributeBymlFile)
                            AttributeByml = ((MaterialAttributeBymlFile)result.AttributeFile).BymlFile;
                        ReloadData();
                        Renderer.UpdateVertexData();
                        SaveAttributeByml(true);
                    });
                });
                thread.Start();
            }
        }

        private string GetPlatform()
        {
            switch (KclFile.Version)
            {
                case FileVersion.VersionDS: return "NDS";
                case FileVersion.VersionGC: return "GCN";
                case FileVersion.VersionWII: return "WII";
                case FileVersion.Version2:
                    if (KclFile.ByteOrder == Syroot.BinaryData.ByteOrder.BigEndian)
                        return "WII U";
                    else
                        return "SWITCH";
                default:
                    return "SWITCH";
            }
        }

        private void SwapEndianess(object sender, EventArgs args)
        {
            if (!((ToolStripMenuItem)sender).Checked)
                KclFile.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
            else
                KclFile.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
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

        public List<KCLModel> GetKclModels() {
            return Renderer.models;
        }

        private static void SaveMaterialWindowSettings()
        {
            //Apply runtime values for later use (and to save as config)
            Runtime.CollisionSettings.KCLGamePreset = MaterialWindowSettings.GamePreset;
            Runtime.CollisionSettings.KCLPlatform = MaterialWindowSettings.Platform;
            Runtime.CollisionSettings.KCLUsePresetEditor = MaterialWindowSettings.UsePresetEditor;
            Toolbox.Library.Config.Save();
        }

        private void ReloadData()
        {
            //Split collision triangles by materials between all the models
            Dictionary<int, List<Triangle>> triangleList = new Dictionary<int, List<Triangle>>();

            foreach (var model in KclFile.Models) {
                foreach (var prism in model.Prisms)
                {
                    var triangle = model.GetTriangle(prism);
                    if (!triangleList.ContainsKey(prism.CollisionFlags))
                        triangleList.Add(prism.CollisionFlags, new List<Triangle>());

                    triangleList[prism.CollisionFlags].Add(triangle);
                }
                //Triangle indexed 
                //It's not pratical to split materials up with these.
                //Materials are instead handled seperately and need to be handled in another way.
                if (triangleList.Count == model.Prisms.Length)
                {
                    triangleList.Clear();
                    triangleList.Add(0, new List<Triangle>());
                    foreach (var prism in model.Prisms)
                    {
                        var triangle = model.GetTriangle(prism);
                        triangleList[0].Add(triangle);
                    }
                }
            }

            Renderer.models.Clear();
            Nodes.Clear();

            //Load the vertex data for rendering
            foreach (var triGroup in triangleList)
            {
                KCLModel kclmodel = new KCLModel();
                kclmodel.Text = $"COL_{triGroup.Key.ToString("X")}";

                int faceIndex = 0;

                var triangles = triGroup.Value;
                for (int i = 0; i < triangles.Count; i++)
                {
                    for (int v = 0; v < triangles[i].Vertices.Length; v++) {
                        var positon = triangles[i].Vertices[v];
                        var normal = triangles[i].Normal;
                        var attribute = triGroup.Key;

                        kclmodel.vertices.Add(new Vertex()
                        {
                            pos = new Vector3(positon.X, positon.Y, positon.Z),
                            nrm = new Vector3(normal.X, normal.Y, normal.Z),
                            col = new Vector4(1,1,1,1),
                        });
                        kclmodel.faces.Add(faceIndex + v);
                    }
                    faceIndex += 3;
                }

                Renderer.models.Add(kclmodel);
                Nodes.Add(kclmodel);
            }
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
