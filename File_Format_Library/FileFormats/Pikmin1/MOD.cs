using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;
using Toolbox.Library.Forms;
using OpenTK;
using GL_EditorFramework.GL_Core;
using OpenTK.Graphics.OpenGL;

namespace FirstPlugin
{
    class MOD : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Pikmin 1 Model Format" };
        public string[] Extension { get; set; } = new string[] { "*.mod" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.HasExtension(FileName, ".mod");
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        //Check for the viewport in the object editor
        //This is attached to it to load multiple file formats within the object editor to the viewer
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

        bool DrawablesLoaded = false;
        public override void OnClick(TreeView treeView)
        {
            //Make sure opengl is enabled
            if (Runtime.UseOpenGL)
            {
                //Open the viewport
                if (viewport == null)
                {
                    viewport = new Viewport(ObjectEditor.GetDrawableContainers());
                    viewport.Dock = DockStyle.Fill;
                }

                //Make sure to load the drawables only once so set it to true!
                if (!DrawablesLoaded)
                {
                    ObjectEditor.AddContainer(DrawableContainer);
                    DrawablesLoaded = true;
                }

                //Reload which drawable to display
                viewport.ReloadDrawables(DrawableContainer);
                LibraryGUI.LoadEditor(viewport);

                viewport.Text = Text;
            }
        }

        public MDL_Renderer Renderer;

        public DrawableContainer DrawableContainer = new DrawableContainer();

        private Vertex[] Vertices;
        private Vertex[] VertexNormals;
        private Vertex[] Colors;
        private enum ChunkNames
        {
            Header,

            VertexPosition = 0x0010,
            VertexNormal = 0x0011,
            VertexNBT = 0x0012,
            VertexColor = 0x0013,

            VertexUV0 = 0x0018,
            VertexUV1 = 0x0019,
            VertexUV2 = 0x001A,
            VertexUV3 = 0x001B,
            VertexUV4 = 0x001C,
            VertexUV5 = 0x001D,
            VertexUV6 = 0x001E,
            VertexUV7 = 0x001F,

            Texture = 0x0020,
            TextureAttribute = 0x0022,
            Material = 0x0030,

            VertexMatrix = 0x0040,

            Envelope = 0x0041,

            Mesh = 0x0050,

            Joint = 0x0060,
            JointName = 0x0061,

            CollisionPrism = 0x0100,
            CollisionGrid = 0x0110,

            EoF = 0xFFFF
        }

        private void SkipPadding(FileReader stream, int offset)
        {
            stream.Seek((~(offset - 1) & (stream.Position + offset - 1)) - stream.Position);
        }

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            Text = FileName;

            //Set renderer
            //Load it to a drawables list
            Renderer = new MDL_Renderer();
            DrawableContainer.Name = FileName;
            DrawableContainer.Drawables.Add(Renderer);

            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(true);

                while (reader.EndOfStream == false)
                {
                    long chunkStart = reader.Position;

                    int opcode = reader.ReadInt32();
                    int lengthOfStruct = reader.ReadInt32();

                    // basic error checking
                    if ((chunkStart & 0x1F) != 0)
                        throw new Exception($"Chunk start ({chunkStart}) not on boundary!");

                    Vector3Holder holder = new Vector3Holder();

                    switch ((ChunkNames)opcode)
                    {
                        case ChunkNames.VertexPosition:
                            int vertexCount = reader.ReadInt32();
                            Vertices = new Vertex[vertexCount];

                            SkipPadding(reader, 0x20);
                            for (int i = 0; i < vertexCount; i++)
                            {
                                holder.Read(reader);
                                Vertices[i] = new Vertex { pos = holder.value };
                            }
                            SkipPadding(reader, 0x20);

                            break;
                        case ChunkNames.VertexNormal:
                            int normalCount = reader.ReadInt32();
                            VertexNormals = new Vertex[normalCount];

                            SkipPadding(reader, 0x20);
                            for (int i = 0; i < normalCount; i++)
                            {
                                holder.Read(reader);
                                VertexNormals[i] = new Vertex { nrm = holder.value };
                            }
                            SkipPadding(reader, 0x20);

                            break;
                        case ChunkNames.VertexColor:
                            int colorCount = reader.ReadInt32();
                            Colors = new Vertex[colorCount];

                            SkipPadding(reader, 0x20);
                            for (int i = 0; i < colorCount; i++)
                            {
                                Colors[i] = new Vertex
                                {
                                    col = new Vector4(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte())
                                };
                            }
                            SkipPadding(reader, 0x20);

                            break;
                        case ChunkNames.Mesh:
                            int meshCount = reader.ReadInt32();
                            SkipPadding(reader, 0x20);

                            for (int mIdx = 0; mIdx < meshCount; mIdx++)
                            {
                                //Create a renderable object for our mesh
                                var renderedMesh = new GenericRenderedObject
                                {
                                    Checked = true,
                                    ImageKey = "mesh",
                                    SelectedImageKey = "mesh",
                                    Text = $"Mesh {mIdx}"
                                };
                                Nodes.Add(renderedMesh);
                                Renderer.Meshes.Add(renderedMesh);

                                STGenericPolygonGroup polyGroup = new STGenericPolygonGroup();
                                renderedMesh.PolygonGroups.Add(polyGroup);

                                renderedMesh.BoneIndex = reader.ReadInt32();

                                int vtxDescriptor = reader.ReadInt32();
                                int mtxGroupCount = reader.ReadInt32();
                                for (int mgIdx = 0; mgIdx < mtxGroupCount; mgIdx++)
                                {
                                    int dependencyCount = reader.ReadInt32();
                                    for (int ll = 0; ll < dependencyCount; ll++)
                                        reader.ReadInt16();

                                    int dListCount = reader.ReadInt32();
                                    for (int dlIdx = 0; dlIdx < dListCount; dlIdx++)
                                    {
                                        int flags = reader.ReadInt32();
                                        int unk1 = reader.ReadInt32();
                                        int dataSize = reader.ReadInt32();
                                        SkipPadding(reader, 0x20);
                                        long endPosition = reader.Position + dataSize;
                                        while (reader.Position < endPosition)
                                        {
                                            byte faceType = reader.ReadByte();
                                            if (faceType == 0x98 || faceType == 0xA0)
                                            {

                                                short faceCount = reader.ReadInt16();
                                                int[] polygons = new int[faceCount];

                                                for (int fIdx = 0; fIdx < faceCount; fIdx++)
                                                {
                                                    if ((vtxDescriptor & 1) == 1)
                                                        reader.ReadByte(); // posmat index
                                                    if ((vtxDescriptor & 2) == 2)
                                                        reader.ReadByte(); // tex1 index

                                                    ushort vtxIdx = reader.ReadUInt16();

                                                    ushort nrmIdx = 0;
                                                    if (VertexNormals.Length > 0)
                                                        nrmIdx = reader.ReadUInt16();

                                                    ushort colIdx = 0;
                                                    if ((vtxDescriptor & 4) == 4)
                                                        colIdx = reader.ReadUInt16();

                                                    int txCoordIdx = 0;
                                                    int txCoordDescriptor = vtxDescriptor >> 3;
                                                    for (int tcoordIdx = 0; tcoordIdx < 8; tcoordIdx++)
                                                    {
                                                        if ((txCoordDescriptor & 1) == 0x1)
                                                        {
                                                            // Only read for the first texcoord
                                                            txCoordIdx = reader.ReadInt16();
                                                            txCoordDescriptor >>= 1;
                                                        }
                                                    }

                                                    Vertex newVertex = new Vertex
                                                    {
                                                        pos = Vertices[vtxIdx].pos
                                                    };

                                                    if (VertexNormals != null)
                                                        newVertex.nrm = VertexNormals[nrmIdx].nrm;
                                                    if (Colors != null)
                                                        newVertex.col = Colors[colIdx].col;

                                                    polygons[fIdx] = renderedMesh.vertices.Count;
                                                    renderedMesh.vertices.Add(newVertex);
                                                }

                                                List<Triangle> currentPolygons = ToTris(polygons, faceType);

                                                foreach (Triangle triangle in currentPolygons)
                                                {
                                                    polyGroup.faces.Add(triangle.C);
                                                    polyGroup.faces.Add(triangle.B);
                                                    polyGroup.faces.Add(triangle.A);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        default:
                            reader.Seek(lengthOfStruct, System.IO.SeekOrigin.Current);
                            break;
                    }
                }
            }
        }

        private static List<Triangle> ToTris(int[] polys, byte opcode)
        {
            if (polys.Length == 3)
                return new List<Triangle>()
                {
                    new Triangle()
                    { A = polys[0], B = polys[1], C = polys[2] }
                };

            var tris = new List<Triangle>();
            if (opcode == 0x98)
            {
                int n = 2;
                for (int x = 0; x < polys.Length - 2; x++)
                {
                    int[] tri = new int[3];
                    bool isEven = (n % 2) == 0;
                    tri[0] = polys[n - 2];
                    tri[1] = isEven ? polys[n] : polys[n - 1];
                    tri[2] = isEven ? polys[n - 1] : polys[n];

                    if (tri[0] != tri[1] && tri[1] != tri[2] && tri[2] != tri[0])
                        tris.Add(new Triangle()
                        {
                            A = tri[0],
                            B = tri[1],
                            C = tri[2],
                        });

                    n++;
                }
            }
            if (opcode == 0xA0)
            {
                for (int n = 1; n < polys.Length - 1; n++)
                {
                    int[] tri = new int[3];
                    tri[0] = polys[n];
                    tri[1] = polys[n + 1];
                    tri[2] = polys[0];

                    if (tri[0] != tri[1] && tri[1] != tri[2] && tri[2] != tri[0])
                        tris.Add(new Triangle()
                        {
                            A = tri[0],
                            B = tri[1],
                            C = tri[2],
                        });
                }
            }
            return tris;
        }

        public class Triangle
        {
            public int A;
            public int B;
            public int C;
        }

        public class Vector3Holder
        {
            public Vector3 value;

            public void Read(FileReader reader)
            {
                value.X = reader.ReadSingle();
                value.Y = reader.ReadSingle();
                value.Z = reader.ReadSingle();
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public class MaterialTextureMap : STGenericMatTexture
        {
            //The index of a texture
            //Some formats will map them by index, some by name, some by a hash, it's up to how the user handles it
            public int TextureIndex { get; set; }
        }

        public class MDL_Renderer : GenericModelRenderer
        {
            //A list of textures to display on the model
            public List<STGenericTexture> TextureList = new List<STGenericTexture>();

            public override void OnRender(GLControl control)
            {
                //Here we can add things on each frame rendered
            }

            //Render data to display by per material and per mesh
            public override void SetRenderData(STGenericMaterial mat, ShaderProgram shader, STGenericObject m)
            {
            }

            //Custom bind texture method
            public override int BindTexture(STGenericMatTexture tex, ShaderProgram shader)
            {
                //By default we bind to the default texture to use
                //This will be used if no texture is found
                GL.ActiveTexture(TextureUnit.Texture0 + tex.textureUnit + 1);
                GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.RenderableTex.TexID);

                string activeTex = tex.Name;

                //We want to cast our custom texture map class to get any custom properties we may need
                //If you don't need any custom way of mapping, you can just stick with the generic one
                var matTexture = (MaterialTextureMap)tex;

                //Go through our texture maps in the material and see if the index matches
                foreach (var texture in TextureList)
                {
                    if (TextureList.IndexOf(texture) == matTexture.TextureIndex)
                    {
                        BindGLTexture(tex, shader, TextureList[matTexture.TextureIndex]);
                        return tex.textureUnit + 1;
                    }

                    //You can also check if the names match
                    if (texture.Text == tex.Name)
                    {
                        BindGLTexture(tex, shader, TextureList[matTexture.TextureIndex]);
                        return tex.textureUnit + 1;
                    }
                }

                //Return our texture uint id. 
                return tex.textureUnit + 1;
            }
        }
    }
}
