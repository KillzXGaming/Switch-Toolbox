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

        private STSkeleton Skeleton;

        private Vector3[] Vertices;
        private Vector3[] VertexNormals;
        private Vector4[] Colors;
        private Envelope[] Envelopes;

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
            Skeleton = new STSkeleton();
            DrawableContainer.Name = FileName;
            DrawableContainer.Drawables.Add(Renderer);
            DrawableContainer.Drawables.Add(Skeleton);

            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(true);

                string[] JointNames = new string[0];
                Joint[] Joints = new Joint[0];
                Envelopes = new Envelope[0];
                while (reader.EndOfStream == false)
                {
                    long chunkStart = reader.Position;

                    int opcode = reader.ReadInt32();
                    int lengthOfStruct = reader.ReadInt32();

                    // basic error checking
                    if ((chunkStart & 0x1F) != 0)
                        throw new Exception($"Chunk start ({chunkStart}) not on boundary!");

                    switch ((ChunkNames)opcode)
                    {
                        case ChunkNames.VertexPosition:
                            Vertices = ReadVector3Array(reader);
                            break;
                        case ChunkNames.VertexNormal:
                            VertexNormals = ReadVector3Array(reader);
                            break;
                        case ChunkNames.VertexColor:
                            Colors = ReadVertexColors(reader);
                            break;
                        case ChunkNames.Mesh:
                            ReadMeshChunk(reader);
                            break;
                        case ChunkNames.Envelope:
                            Envelopes = ParseArray<Envelope>(reader);
                            break;
                        case ChunkNames.JointName:
                            JointNames = ReadStrings(reader);
                            break;
                        case ChunkNames.Joint:
                            Joints = ParseArray<Joint>(reader);
                            break;
                        default:
                            reader.Seek(lengthOfStruct, System.IO.SeekOrigin.Current);
                            break;
                    }
                }

                for (int i = 0; i < Joints.Length; i++)
                {
                    STBone bone = new STBone(Skeleton);
                    bone.parentIndex = Joints[i].ParentIndex;
                    bone.Position = Joints[i].Position;
                    bone.EulerRotation = Joints[i].Rotation;
                    bone.Scale = Joints[i].Scale;
                    Skeleton.bones.Add(bone);
                }
                Skeleton.reset();
                Skeleton.update();

                foreach (var mesh in Renderer.Meshes)
                {
                    for (int v = 0; v < mesh.vertices.Count; v++) {
                        var vertex = mesh.vertices[v];
                        if (vertex.boneIds.Count == 1)
                        {
                            var transform = Skeleton.bones[vertex.boneIds[0]].Transform;
                            vertex.pos = Vector3.TransformPosition(vertex.pos, transform);
                        }
                    }
                }
            }
        }

        private T[] ParseArray<T>(FileReader reader) 
            where T : IModChunk, new()
        {
            int count = reader.ReadInt32();

            SkipPadding(reader, 0x20);
            T[] values = new T[count];
            for (int i = 0; i < count; i++) {
                values[i] = new T();
                values[i].Read(reader);
            }

            SkipPadding(reader, 0x20);
            return values;
        }

        private Vector4[] ReadVertexColors(FileReader reader)
        {
            int count = reader.ReadInt32();
            Vector4[] vertexData = new Vector4[count];

            SkipPadding(reader, 0x20);
            for (int i = 0; i < count; i++)
                Colors[i] = new Vector4(
                    reader.ReadByte() / 255f,
                    reader.ReadByte() / 255f,
                    reader.ReadByte() / 255f,
                    reader.ReadByte() / 255f);

            SkipPadding(reader, 0x20);
            return vertexData;
        }

        private Vector3[] ReadVector3Array(FileReader reader)
        {
            int count = reader.ReadInt32();
            Vector3[] vertexData = new Vector3[count];

            SkipPadding(reader, 0x20);
            for (int i = 0; i < count; i++)
                vertexData[i] = reader.ReadVec3();

            SkipPadding(reader, 0x20);
            return vertexData;
        }

        private string[] ReadStrings(FileReader reader)
        {
            int count = reader.ReadInt32();
            string[] vertexData = new string[count];

            SkipPadding(reader, 0x20);
            for (int i = 0; i < count; i++) {
                uint length = reader.ReadUInt32();
                vertexData[i] = reader.ReadString((int)length, true);
            }

            SkipPadding(reader, 0x20);
            return vertexData;
        }

        private void ReadMeshChunk(FileReader reader)
        {
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
                                        pos = Vertices[vtxIdx]
                                    };

                                    int envIdx = 0;
                                    if (Envelopes.Length > envIdx)
                                    {
                                        for (int i = 0; i < Envelopes[envIdx].Indices?.Length; i++) {
                                            newVertex.boneIds.Add(Envelopes[envIdx].Indices[i]);
                                            newVertex.boneWeights.Add(Envelopes[envIdx].Weights[i]);
                                        }
                                    }

                                    if (VertexNormals != null)
                                        newVertex.nrm = VertexNormals[nrmIdx];
                                    if (Colors != null)
                                        newVertex.col = Colors[colIdx];

                                    polygons[fIdx] = renderedMesh.vertices.Count;
                                    renderedMesh.vertices.Add(newVertex);
                                }

                                List<Triangle> currentPolygons = ToTris(polygons, faceType);

                                Console.WriteLine($"faceType {faceType} polygons {polygons.Length} ");
                                foreach (Triangle triangle in currentPolygons)
                                {
                                    if (faceType == 0x98)
                                    {
                                        polyGroup.faces.Add(triangle.B);
                                        polyGroup.faces.Add(triangle.C);
                                        polyGroup.faces.Add(triangle.A);
                                    }
                                    else
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

        public interface IModChunk
        {
            void Read(FileReader reader);
            void Write(FileWriter writer);
        }

        public class Envelope : IModChunk
        {
            public float[] Weights;
            public ushort[] Indices;

            public void Read(FileReader reader)
            {
                ushort count = reader.ReadUInt16();

                Weights = new float[count];
                Indices = new ushort[count];
                for (int i = 0; i < count; i++) {
                    Indices[i] = reader.ReadUInt16();
                    Weights[i] = reader.ReadSingle();
                }
            }

            public void Write(FileWriter writer)
            {
                writer.Write((ushort)Indices.Length);
                for (int i = 0; i < Indices.Length; i++) {
                    writer.Write(Indices[i]);
                    writer.Write(Weights[i]);
                }
            }
        }

        public class Joint : IModChunk
        {
            public int ParentIndex;

            public bool UseVolume { get; set; }
            public bool FoundLightGroup { get; set; }

            public BoundingBox BoundingBox { get; set; }

            public float VolumeRadius { get; set; }

            public Vector3 Scale { get; set; }
            public Vector3 Rotation { get; set; }
            public Vector3 Position { get; set; }

            public List<MatPoly> MatPolys = new List<MatPoly>();

            private uint flags;

            public void Read(FileReader reader)
            {
                ParentIndex = reader.ReadInt32();
                flags = reader.ReadUInt32(); 
                ushort usingIdentifier = (ushort)flags;
                UseVolume = usingIdentifier > 0;
                FoundLightGroup = (usingIdentifier & 0x4000) != 0; 
                BoundingBox = new BoundingBox()
                {
                    Min = reader.ReadVec3(),
                    Max = reader.ReadVec3(),
                };
                VolumeRadius = reader.ReadSingle();
                Scale = reader.ReadVec3();
                Rotation = reader.ReadVec3();
                Position = reader.ReadVec3();

                uint numMatPolys = reader.ReadUInt32();
                for (int i = 0; i < numMatPolys; i++)
                {
                    MatPolys.Add(new MatPoly()
                    {
                        Index = reader.ReadUInt16(),
                        Unknown = reader.ReadUInt16(),
                    });
                }
            }

            public void Write(FileWriter writer)
            {
                writer.Write(ParentIndex);
                writer.Write(flags);
                writer.Write(BoundingBox.Min);
                writer.Write(BoundingBox.Max);
                writer.Write(VolumeRadius);
                writer.Write(Scale);
                writer.Write(Rotation);
                writer.Write(Position);
                foreach (var matPoly in MatPolys) {
                    writer.Write(matPoly.Index);
                    writer.Write(matPoly.Unknown);
                }
            }
        }

        public struct MatPoly
        {
            public ushort Index { get; set; }
            public ushort Unknown { get; set; }
        }

        public struct BoundingBox
        {
            public Vector3 Min { get; set; }
            public Vector3 Max { get; set; }
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
