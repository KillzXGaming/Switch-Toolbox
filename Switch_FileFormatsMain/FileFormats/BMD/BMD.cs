using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.Rendering;
using SuperBMDLib;
using System.Drawing;
using SuperBMDLib.Rigging;
using SuperBMDLib.Geometry.Enums;
using SuperBMDLib.Util;

namespace FirstPlugin
{
    public class BMD : TreeNodeFile, IFileFormat, IContextMenuNode, ITextureContainer
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Gamecube/Wii Binary Model (BMD/BDL)" };
        public string[] Extension { get; set; } = new string[] { "*.bmd", "*.bdl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.SetByteOrder(true);
                bool IsBMD = reader.ReadUInt32() == 0x4A334432;
                reader.Position = 0;

                return IsBMD;
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        Viewport viewport
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

        public Dictionary<string, STGenericTexture> Textures { get; set; }

        bool DrawablesLoaded = false;
        public override void OnClick(TreeView treeView)
        {
            if (Runtime.UseOpenGL && !Runtime.UseLegacyGL)
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

        public BMD_Renderer Renderer;

        public DrawableContainer DrawableContainer = new DrawableContainer();

        public Model BMDFile;
        private TreeNode TextureFolder;
        private TreeNode ShapeFolder;
        private TreeNode MaterialFolder;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            CanSave = true;

            //Set renderer
            Renderer = new BMD_Renderer();
            DrawableContainer.Name = FileName;
            DrawableContainer.Drawables.Add(Renderer);
            Textures = new Dictionary<string, STGenericTexture>();

            BMD_Renderer.TextureContainers.Add(this);

            ShapeFolder = new TreeNode("Shapes");
            MaterialFolder = new TreeNode("Materials");
            TextureFolder = new TreeNode("Textures");
            Nodes.Add(ShapeFolder);
            Nodes.Add(MaterialFolder);
            Nodes.Add(TextureFolder);

            BMDFile = Model.Load(stream);

            for (int i = 0; i < BMDFile.Shapes.Shapes.Count; i++)
            {
                var mat = new BMDMaterialWrapper(BMDFile.Materials.GetMaterial(i), BMDFile);
                mat.Text = BMDFile.Materials.GetMaterialName(i);
                MaterialFolder.Nodes.Add(mat);

                var shpWrapper = new BMDShapeWrapper(BMDFile.Shapes.Shapes[i], BMDFile, mat);
                shpWrapper.Text = $"Shape {i}";
                ShapeFolder.Nodes.Add(shpWrapper);
                Renderer.Meshes.Add(shpWrapper);

                var polyGroup = new STGenericPolygonGroup();
                shpWrapper.PolygonGroups.Add(polyGroup);

                var curShape = BMDFile.Shapes.Shapes[i];
                var VertexAttributes = BMDFile.VertexData.Attributes;

                int vertexID = 0;
                foreach (SuperBMDLib.Geometry.Packet pack in curShape.Packets)
                {
                    foreach (SuperBMDLib.Geometry.Primitive prim in pack.Primitives)
                    {
                        List<SuperBMDLib.Geometry.Vertex> triVertices = J3DUtility.PrimitiveToTriangles(prim);
                        for (int triIndex = 0; triIndex < triVertices.Count; triIndex += 3)
                        {
                            polyGroup.faces.AddRange(new int[] { vertexID + 2, vertexID + 1, vertexID });

                            for (int triVertIndex = 0; triVertIndex < 3; triVertIndex++)
                            {
                                SuperBMDLib.Geometry.Vertex vert = triVertices[triIndex + triVertIndex];

                                Vertex vertex = new Vertex();
                                vertex.pos = VertexAttributes.Positions[(int)vert.GetAttributeIndex(GXVertexAttribute.Position)];
                                shpWrapper.vertices.Add(vertex);

                                if (curShape.Descriptor.CheckAttribute(GXVertexAttribute.Normal))
                                    vertex.nrm = VertexAttributes.Normals[(int)vert.NormalIndex];
                                if (curShape.Descriptor.CheckAttribute(GXVertexAttribute.Color0))
                                {
                                    var color0 = VertexAttributes.Color_0[(int)vert.Color0Index];
                                    vertex.col = new OpenTK.Vector4(color0.R, color0.G, color0.B, color0.A);
                                }

                                for (int texCoordNum = 0; texCoordNum < 8; texCoordNum++)
                                {
                                    if (curShape.Descriptor.CheckAttribute(GXVertexAttribute.Tex0 + texCoordNum))
                                    {
                                        switch (texCoordNum)
                                        {
                                            case 0:
                                                vertex.uv0 = VertexAttributes.TexCoord_0[(int)vert.TexCoord0Index];
                                                break;
                                            case 1:
                                                vertex.uv1 = VertexAttributes.TexCoord_0[(int)vert.TexCoord0Index];
                                                break;
                                            case 2:
                                                vertex.uv2 = VertexAttributes.TexCoord_0[(int)vert.TexCoord0Index];
                                                break;
                                        }
                                    }
                                }

                                vertexID++;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < BMDFile.Textures.Textures.Count; i++)
            {
                var texWrapper = new BMDTextureWrapper(BMDFile.Textures.Textures[i]);
                TextureFolder.Nodes.Add(texWrapper);
                Textures.Add(texWrapper.Text, texWrapper);
            }
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new STToolStipMenuItem("Save", null, SaveAction, Keys.Control | Keys.S));
            return Items.ToArray();
        }

        private void SaveAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(this);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            BMDFile.ExportBMD(mem);
            return mem.ToArray();
        }
    }
}
