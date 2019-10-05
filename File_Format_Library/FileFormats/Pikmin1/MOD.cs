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

                    switch (opcode)
                    {
                        case 0x10: // VERTICES
                            int vertexCount = reader.ReadInt32();
                            Vertices = new Vertex[vertexCount];

                            SkipPadding(reader, 0x20);

                            for (int i = 0; i < vertexCount; i++)
                            {
                                float x = reader.ReadSingle();
                                float y = reader.ReadSingle();
                                float z = reader.ReadSingle();
                                Vertices[i] = new Vertex
                                {
                                    pos = new Vector3(x, y, z)
                                };
                            }

                            SkipPadding(reader, 0x20);
                            break;
                        case 0x11:
                            int vertexNormalCount = reader.ReadInt32();
                            VertexNormals = new Vertex[vertexNormalCount];
                            SkipPadding(reader, 0x20);

                            for (int i = 0; i < vertexNormalCount; i++)
                            {
                                float x = reader.ReadSingle();
                                float y = reader.ReadSingle();
                                float z = reader.ReadSingle();
                                VertexNormals[i] = new Vertex
                                {
                                    nrm = new Vector3(x, y, z)
                                };
                            }

                            SkipPadding(reader, 0x20);
                            break;
                        case 0x13: // COLOURS
                            int colorCount = reader.ReadInt32();
                            Colors = new Vertex[colorCount];
                            SkipPadding(reader, 0x20);

                            for (int i = 0; i < colorCount; i++)
                            {
                                byte x = reader.ReadByte();
                                byte y = reader.ReadByte();
                                byte z = reader.ReadByte();
                                byte w = reader.ReadByte();
                                Colors[i] = new Vertex
                                {
                                    col = new Vector4(x, y, z, w)
                                };
                            }

                            SkipPadding(reader, 0x20);
                            break;
                        case 0x50:
                            int meshCount = reader.ReadInt32();
                            SkipPadding(reader, 0x20);

                            for (int i = 0; i < meshCount; i++)
                            {
                                //Create a renderable object for our mesh
                                var renderedMesh = new GenericRenderedObject
                                {
                                    Checked = true,
                                    ImageKey = "mesh",
                                    SelectedImageKey = "mesh",
                                    Text = $"Mesh {i}"
                                };
                                Nodes.Add(renderedMesh);
                                Renderer.Meshes.Add(renderedMesh);

                                STGenericPolygonGroup polyGroup = new STGenericPolygonGroup();
                                renderedMesh.PolygonGroups.Add(polyGroup);

                                reader.ReadInt32();

                                int vtxDescriptor = reader.ReadInt32();
                                int mtxGroupCount = reader.ReadInt32();

                                Console.WriteLine("mtxGroupCount " + mtxGroupCount);

                                for (int a = 0; a < mtxGroupCount; a++)
                                {
                                    int unkCount = reader.ReadInt32();
                                    for (int unkIter = 0; unkIter < unkCount; unkIter++)
                                        reader.ReadInt16();

                                    int dispListCount = reader.ReadInt32();

                                    Console.WriteLine("dispListCount " + dispListCount);

                                    for (int b = 0; b < dispListCount; b++)
                                    {
                                        reader.ReadInt32();
                                        reader.ReadInt32();

                                        int displacementSize = reader.ReadInt32();
                                        SkipPadding(reader, 0x20);

                                        long end_displist = reader.Position + displacementSize;

                                        Console.WriteLine("end_displist " + end_displist);
                                        Console.WriteLine("displacementSize " + displacementSize);
                                        Console.WriteLine("reader.Position " + reader.Position);

                                        while (reader.Position < end_displist)
                                        {
                                            byte faceOpCode = reader.ReadByte();

                                            if (faceOpCode == 0x98 || faceOpCode == 0xA0)
                                            {
                                                short vCount = reader.ReadInt16();

                                                int[] polys = new int[vCount];
                                                for (int vc = 0; vc < vCount; vc++)
                                                {
                                                    if ((vtxDescriptor & 0x1) == 0x1)
                                                        reader.ReadByte(); // Position Matrix

                                                    if ((vtxDescriptor & 0x2) == 0x2)
                                                        reader.ReadByte(); // tex1 matrix

                                                    ushort vtxPosIndex = reader.ReadUInt16();

                                                    uint normalID = 0;
                                                    if (VertexNormals.Length > 0)
                                                        normalID = reader.ReadUInt16();

                                                    uint colorID = 0;
                                                    if ((vtxDescriptor & 0x4) == 0x4)
                                                        colorID = reader.ReadUInt16();

                                                    int tmpVar = vtxDescriptor >> 3;

                                                    uint texCoordID = 0;
                                                    for (int c = 0; c < 8; c++)
                                                    {
                                                        if ((tmpVar & 0x1) == 0x1)
                                                            if (c == 0) texCoordID = reader.ReadUInt16();
                                                        tmpVar >>= 1;
                                                    }

                                                    Vertex vert = new Vertex
                                                    {
                                                        pos = Vertices[vtxPosIndex].pos,
                                                        nrm = VertexNormals[normalID].nrm,
                                                        //col = Colors[colorID].col
                                                    };

                                                    polys[vc] = renderedMesh.vertices.Count;
                                                    renderedMesh.vertices.Add(vert);
                                                }

                                                List<Triangle> curPolys = ToTris(polys, faceOpCode);

                                                foreach (Triangle poly in curPolys)
                                                {
                                                    Console.WriteLine($"{poly.A} {poly.B} {poly.C}");

                                                    polyGroup.faces.Add(poly.A);
                                                    polyGroup.faces.Add(poly.B);
                                                    polyGroup.faces.Add(poly.C);
                                                }
                                            }
                                        }
                                    }
                                }

                                Console.WriteLine("vertices " + renderedMesh.vertices.Count);
                                Console.WriteLine("faces " + renderedMesh.PolygonGroups[0].faces.Count);
                                Console.WriteLine("Vertices " + Vertices.Length);
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
