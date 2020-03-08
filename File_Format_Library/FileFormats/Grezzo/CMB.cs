using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;
using Grezzo.CmbEnums;
using OpenTK.Graphics.OpenGL;
using FirstPlugin.Forms;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.PICA.Commands;

namespace FirstPlugin
{
    public class CMB : TreeNodeFile, IFileFormat, IContextMenuNode, IExportableModel
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "*CTR Model Binary" };
        public string[] Extension { get; set; } = new string[] { "*.cmb" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "cmb ");
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

        public IEnumerable<STGenericObject> ExportableMeshes => Renderer.Meshes;
        public IEnumerable<STGenericMaterial> ExportableMaterials => Materials;
        public IEnumerable<STGenericTexture> ExportableTextures => Renderer.TextureList;
        public STSkeleton ExportableSkeleton => Skeleton;

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Save", null, SaveAction, Keys.Control | Keys.S));
            return Items.ToArray();
        }

        private void SaveAction(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(this);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        bool DrawablesLoaded = false;
        public override void OnClick(TreeView treeView)
        {
            LoadEditor<STPropertyGrid>();
        }

        public T LoadEditor<T>() where T : UserControl, new()
        {
            ViewportEditor editor = (ViewportEditor)LibraryGUI.GetActiveContent(typeof(ViewportEditor));
            if (editor == null)
            {
                editor = new ViewportEditor(true);
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }
            if (!DrawablesLoaded)
            {
                ObjectEditor.AddContainer(DrawableContainer);
                DrawablesLoaded = true;
            }
            if (Runtime.UseOpenGL)
                editor.LoadViewport(DrawableContainer);

            foreach (var subControl in editor.GetEditorPanel().Controls)
                if (subControl.GetType() == typeof(T))
                    return subControl as T;

            T control = new T();
            control.Dock = DockStyle.Fill;
            editor.LoadEditor(control);
            return control;
        }

        public CMB_Renderer Renderer;
        public DrawableContainer DrawableContainer = new DrawableContainer();
        List<STGenericMaterial> Materials = new List<STGenericMaterial>();

        public Header header;
        STTextureFolder texFolder;
        public STSkeleton Skeleton;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            Renderer = new CMB_Renderer();
            DrawableContainer.Drawables.Add(Renderer);
        

            Skeleton = new STSkeleton();
            //These models/skeletons come out massive so scale them with an overridden scale
            Skeleton.PreviewScale = Renderer.PreviewScale;
            Skeleton.BonePointScale = 40;
            Renderer.Skeleton = Skeleton;

            DrawableContainer.Drawables.Add(Skeleton);

            header = new Header();
            header.Read(new FileReader(stream));

            Text = header.Name;

            DrawableContainer.Name = Text;

            //Load textures
            if (header.SectionData.TextureChunk != null)
            {
                texFolder = new TextureFolder("Texture");
                TreeNode meshFolder = new TreeNode("Meshes");
                TreeNode materialFolder = new TreeNode("Materials");
                TreeNode skeletonFolder = new TreeNode("Skeleton");

                bool HasTextures = header.SectionData.TextureChunk != null &&
                     header.SectionData.TextureChunk.Textures.Count != 0;

                bool HasMeshes = header.SectionData.SkeletalMeshChunk != null &&
                     header.SectionData.SkeletalMeshChunk.ShapeChunk.SeperateShapes.Count != 0;

                bool HasSkeleton = header.SectionData.SkeletonChunk != null &&
                    header.SectionData.SkeletonChunk.Bones.Count != 0;

                bool HasMaterials = header.SectionData.MaterialChunk != null &&
                   header.SectionData.MaterialChunk.Materials.Count != 0;

                if (HasSkeleton)
                {
                    var bonesOrdered = header.SectionData.SkeletonChunk.Bones.OrderBy(x => x.ID).ToList(); 
                    foreach (var bone in bonesOrdered)
                    {
                        STBone genericBone = new STBone(Skeleton);
                        genericBone.parentIndex = bone.ParentIndex;
                        genericBone.Checked = true;

                        genericBone.Text = $"Bone {bone.ID}";
                        genericBone.RotationType = STBone.BoneRotationType.Euler;

                        genericBone.Position = new OpenTK.Vector3(
                            bone.Translation.X,
                            bone.Translation.Y,
                            bone.Translation.Z
                        );
                        genericBone.EulerRotation = new OpenTK.Vector3(
                                      bone.Rotation.X,
                                      bone.Rotation.Y,
                                      bone.Rotation.Z
                        );
                        genericBone.Scale = new OpenTK.Vector3(
                                      bone.Scale.X,
                                      bone.Scale.Y,
                                      bone.Scale.Z
                        );
                        Skeleton.bones.Add(genericBone);
                    }

                    foreach (var bone in Skeleton.bones)
                    {
                        if (bone.Parent == null)
                            skeletonFolder.Nodes.Add(bone);
                    }

                    Skeleton.reset();
                    Skeleton.update();
                }

                if (HasTextures)
                {
                    int texIndex = 0;
                    foreach (var tex in header.SectionData.TextureChunk.Textures)
                    {
                        var texWrapper = new CTXB.TextureWrapper(tex);
                        texWrapper.Text = $"Texture {texIndex++}";
                        texWrapper.ImageKey = "texture";
                        texWrapper.SelectedImageKey = texWrapper.ImageKey;

                        if (tex.Name != string.Empty)
                            texWrapper.Text = tex.Name;

                        texWrapper.Width = tex.Width;
                        texWrapper.Height = tex.Height;
                        texWrapper.Format = CTR_3DS.ConvertPICAToGenericFormat(tex.PicaFormat);
                        texWrapper.ImageData = tex.ImageData;
                        texFolder.Nodes.Add(texWrapper);

                        Renderer.TextureList.Add(texWrapper);
                    }
                }

                if (HasMaterials)
                {
                    int materialIndex = 0;
                    foreach (var mat in header.SectionData.MaterialChunk.Materials)
                    {
                        CMBMaterialWrapper material = new CMBMaterialWrapper(mat, this);
                        material.Text = $"Material {materialIndex++}";
                        materialFolder.Nodes.Add(material);
                        Materials.Add(material);

                        bool HasDiffuse = false;
                        foreach (var tex in mat.TextureMaps)
                        {
                            if (tex.TextureIndex != -1)
                            {
                                CMBTextureMapWrapper matTexture = new CMBTextureMapWrapper(tex, this);
                                matTexture.TextureIndex = tex.TextureIndex;
                                material.TextureMaps.Add(matTexture);

                                if (tex.TextureIndex < Renderer.TextureList.Count && tex.TextureIndex >= 0)
                                {
                                    matTexture.Name = Renderer.TextureList[tex.TextureIndex].Text;
                                    material.Nodes.Add(matTexture.Name);
                                }

                                if (!HasDiffuse && matTexture.Name != "bg_syadowmap") //Quick hack till i do texture env stuff
                                {
                                    matTexture.Type = STGenericMatTexture.TextureType.Diffuse;
                                    HasDiffuse = true;
                                }
                            }
                        }
                    }
                }

                if (HasMeshes)
                {
                    int MeshIndex = 0;
                    foreach (var mesh in header.SectionData.SkeletalMeshChunk.MeshChunk.Meshes)
                    {
                        STGenericMaterial mat = new STGenericMaterial();
                        if (Materials.Count > mesh.MaterialIndex) //Incase materials for some reason are in a seperate file, check this
                            mat = Materials[mesh.MaterialIndex];

                        CmbMeshWrapper genericMesh = new CmbMeshWrapper(mat);
                        genericMesh.Text = $"Mesh_{MeshIndex++}";
                        genericMesh.MaterialIndex = mesh.MaterialIndex;

                        //Wow this is long
                        var shape = header.SectionData.SkeletalMeshChunk.ShapeChunk.SeperateShapes[(int)mesh.SepdIndex];
                        genericMesh.Shape = shape;

                        List<ushort> SkinnedBoneTable = new List<ushort>();
                        foreach (var prim in shape.Primatives)
                        {
                            if (prim.BoneIndexTable != null) {
                                SkinnedBoneTable.AddRange(prim.BoneIndexTable);
                                foreach (var bone in prim.BoneIndexTable)
                                    Console.WriteLine($"{genericMesh.Text} SkeletonB {Skeleton.bones[(int)bone].Text} index {bone}");
                            }
                        }

                        //Now load the vertex and face data
                        if (shape.Position.VertexData != null)
                        {
                            int VertexCount = shape.Position.VertexData.Length;
                            for (int v = 0; v < VertexCount; v++)
                            {
                                Vertex vert = new Vertex();
                                vert.pos = new OpenTK.Vector3(
                                    shape.Position.VertexData[v].X,
                                    shape.Position.VertexData[v].Y,
                                    shape.Position.VertexData[v].Z);

                                if (shape.Normal.VertexData != null && shape.Normal.VertexData.Length > v)
                                {
                                    vert.nrm = new OpenTK.Vector3(
                                    shape.Normal.VertexData[v].X,
                                    shape.Normal.VertexData[v].Y,
                                    shape.Normal.VertexData[v].Z).Normalized();
                                }

                                if (shape.Color.VertexData != null && shape.Color.VertexData.Length > v)
                                {
                                    vert.col = new OpenTK.Vector4(
                                    shape.Color.VertexData[v].X,
                                    shape.Color.VertexData[v].Y,
                                    shape.Color.VertexData[v].Z,
                                    shape.Color.VertexData[v].W).Normalized();
                                }

                                if (shape.TexCoord0.VertexData != null && shape.TexCoord0.VertexData.Length > v)
                                {
                                    vert.uv0 = new OpenTK.Vector2(
                                    shape.TexCoord0.VertexData[v].X,
                                    1 - shape.TexCoord0.VertexData[v].Y);
                                }

                                if (shape.TexCoord1.VertexData != null)
                                {
                               
                                }

                                if (shape.TexCoord2.VertexData != null)
                                {
                               
                                }

                                for (int i = 0; i < 16; i++)
                                {
                                    if (i < shape.Primatives[0].BoneIndexTable.Length)
                                    {
                                        int boneId = shape.Primatives[0].BoneIndexTable[i];

                                        if (shape.Primatives[0].SkinningMode == SkinningMode.RIGID_SKINNING)
                                        {
                                            vert.pos = OpenTK.Vector3.TransformPosition(vert.pos, Skeleton.bones[boneId].Transform);
                                            vert.nrm = OpenTK.Vector3.TransformNormal(vert.nrm, Skeleton.bones[boneId].Transform);
                                        }
                                    }
                                }

                                bool HasSkinning = shape.Primatives[0].SkinningMode != SkinningMode.SINGLE_BONE
                                && shape.BoneIndices.Type == CmbDataType.UByte; //Noclip checks the type for ubyte so do the same

                                bool HasWeights = shape.Primatives[0].SkinningMode == SkinningMode.SMOOTH_SKINNING;

                               if (shape.BoneIndices.VertexData != null && HasSkinning && shape.BoneIndices.VertexData.Length > v)
                                {
                                    var BoneIndices = shape.BoneIndices.VertexData[v];
                                    for (int j = 0; j < shape.boneDimension; j++)
                                    {
                                          if (BoneIndices[j] < SkinnedBoneTable.Count)
                                               vert.boneIds.Add((int)SkinnedBoneTable[(int)BoneIndices[j]]);
                                        //   Console.WriteLine("boneIds " + BoneIndices[j]);

                                        //    ushort index = shape.Primatives[0].BoneIndexTable[(uint)BoneIndices[j]];
                                    }
                                }
                                if (shape.BoneWeights.VertexData != null && HasWeights && shape.BoneWeights.VertexData.Length > v)
                                {
                                    var BoneWeights = shape.BoneWeights.VertexData[v];
                                    for (int j = 0; j < shape.boneDimension; j++)
                                    {
                                        vert.boneWeights.Add(BoneWeights[j]);
                                    }
                                }

                                genericMesh.vertices.Add(vert);
                            }
                        }

                        foreach (var prim in shape.Primatives)
                        {
                            STGenericPolygonGroup group = new STGenericPolygonGroup();
                            genericMesh.PolygonGroups.Add(group);

                            for (int i = 0; i < prim.Primatives[0].Indices.Length; i++)
                            {
                                group.faces.Add((int)prim.Primatives[0].Indices[i]);
                            }
                        }

                        Renderer.Meshes.Add(genericMesh);
                        meshFolder.Nodes.Add(genericMesh);
                    }
                }
            
                if (meshFolder.Nodes.Count > 0)
                    Nodes.Add(meshFolder);

                if (skeletonFolder.Nodes.Count > 0)
                    Nodes.Add(skeletonFolder);

                if (materialFolder.Nodes.Count > 0)
                    Nodes.Add(materialFolder);

                if (texFolder.Nodes.Count > 0)
                    Nodes.Add(texFolder);
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream)) {
                header.Write(writer);
            }
        }

        public enum CMBVersion
        {
            OOT3DS,
            MM3DS,
            LM3DS,
        }

        public class CMBMaterialWrapper : CtrLibrary.H3DMaterialWrapper
        {
            private CMB ParentCMB;

            public Material CMBMaterial { get; set; }

            public override List<STGenericObject> FindMappedMeshes()
            {
                List<STGenericObject> meshes = new List<STGenericObject>();
                foreach (var mesh in ParentCMB.Renderer.Meshes)
                    if (mesh.GetMaterial() == this)
                        meshes.Add(mesh);
                return meshes;
            }

            public CMBMaterialWrapper(Material material, CMB cmb) : base(material.CTRMaterial)
            {
                ParentCMB = cmb;
                CMBMaterial = material;
            }

            public override void OnClick(TreeView treeView)
            {
                var editor = ParentCMB.LoadEditor<CtrLibrary.Forms.BCHMaterialEditor>();
                editor.LoadMaterial(this);

             /*   STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));
                if (editor == null)
                {
                    editor = new STPropertyGrid();
                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadProperty(Material, null);*/
            }
        }

        public class CMBTextureMapWrapper : STGenericMatTexture
        {
            public CMB CMBParent;

            public int TextureIndex { get; set; }

            public TextureMap TextureMapData;

            public override STGenericTexture GetTexture()
            {
                foreach (var tex in CMBParent.Renderer.TextureList)
                    if (tex.Text == this.Name)
                        return tex;

                return null;
            }

            public CMBTextureMapWrapper(TextureMap texMap, CMB cmb)
            {
                CMBParent = cmb;
                TextureMapData = texMap;

                //Linear filtering looks better according to noclip
                if (TextureMapData.MinFiler == TextureFilter.LINEAR_MIPMAP_NEAREST)
                    TextureMapData.MinFiler = TextureFilter.LINEAR_MIPMAP_LINEAR;

                 this.WrapModeS = ConvertWrapMode(TextureMapData.WrapS);
                 this.WrapModeT = ConvertWrapMode(TextureMapData.WrapT);
                 this.MinFilter = ConvertMinFilterMode(TextureMapData.MinFiler);
                 this.MagFilter = ConvertMagFilterMode(TextureMapData.MagFiler);
            }

            private STTextureMinFilter ConvertMinFilterMode(TextureFilter PicaFilterMode)
            {
                switch (PicaFilterMode)
                {
                    case TextureFilter.LINEAR: return STTextureMinFilter.Linear;
                    case TextureFilter.LINEAR_MIPMAP_LINEAR: return  STTextureMinFilter.LinearMipMapNearest;
                    case TextureFilter.LINEAR_MIPMAP_NEAREST: return STTextureMinFilter.NearestMipmapLinear;
                    case TextureFilter.NEAREST: return STTextureMinFilter.Nearest;
                    case TextureFilter.NEAREST_MIPMAP_LINEAR: return STTextureMinFilter.NearestMipmapLinear;
                    case TextureFilter.NEAREST_MIPMAP_NEAREST: return STTextureMinFilter.NearestMipmapNearest;
                    default: return 0;
                }
            }

            private STTextureMagFilter ConvertMagFilterMode(TextureFilter PicaFilterMode)
            {
                switch (PicaFilterMode)
                {
                    case TextureFilter.LINEAR: return STTextureMagFilter.Linear;
                    case TextureFilter.NEAREST: return STTextureMagFilter.Nearest;
                    default: return 0;
                }
            }

            private STTextureWrapMode ConvertWrapMode(CMBTextureWrapMode PicaWrapMode)
            {
                switch (PicaWrapMode)
                {
                    case CMBTextureWrapMode.REPEAT: return STTextureWrapMode.Repeat;
                    case CMBTextureWrapMode.MIRRORED_REPEAT: return STTextureWrapMode.Mirror;
                    case CMBTextureWrapMode.CLAMP: return STTextureWrapMode.Clamp;
                    case CMBTextureWrapMode.CLAMP_TO_EDGE: return STTextureWrapMode.Clamp;
                    default: return STTextureWrapMode.Repeat;
                }
            }
        }

        public class CmbMeshWrapper : GenericRenderedObject
        {
            public SeperateShape Shape { get; set; }

            STGenericMaterial material;

            public CmbMeshWrapper(STGenericMaterial mat) {
                material = mat;
            }

            public override STGenericMaterial GetMaterial()
            {
                return material;
            }
        }

        private class TextureFolder : STTextureFolder, ITextureContainer
        {
            public bool DisplayIcons => true;

            public List<STGenericTexture> TextureList
            {
                get
                {
                    List<STGenericTexture> textures = new List<STGenericTexture>();
                    foreach (STGenericTexture node in Nodes)
                        textures.Add(node);

                    return textures;
                }
                set {}
            }

            public TextureFolder(string text) : base(text)
            {

            }
        }

        public class Header
        {
            public string Name { get; set; }

            public CMBVersion Version;

            public uint ChunkCount; //Fixed count per game

            public uint Unknown;

            public SectionData SectionData;

            public void Read(FileReader reader)
            {
                string magic = reader.ReadSignature(4, "cmb ");
                uint FileSize = reader.ReadUInt32();
                ChunkCount = reader.ReadUInt32();
                Unknown = reader.ReadUInt32();

                Name = reader.ReadString(0x10).TrimEnd('\0');

                //Check the chunk count used by the game
                if (ChunkCount == 0x0F)
                    Version = CMBVersion.LM3DS;
                else if (ChunkCount == 0x0A)
                    Version = CMBVersion.MM3DS;
                else if (ChunkCount == 0x06)
                    Version = CMBVersion.OOT3DS;
                else
                    throw new Exception("Unexpected chunk count! " + ChunkCount);

                SectionData = new SectionData();
                SectionData.Read(reader, this);
            }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature("cmb ");
                writer.Write(uint.MaxValue); //Reserve space for file size offset
                writer.Write(ChunkCount);
                writer.Write(Unknown);
                writer.WriteString(Name, 0x10);

                SectionData.Write(writer, this);

                //Write the total file size
                using (writer.TemporarySeek(4, System.IO.SeekOrigin.Begin))
                {
                    writer.Write((uint)writer.BaseStream.Length);
                }
            }
        }

        public class SectionData
        {
            public SkeletonChunk SkeletonChunk;
            public QuadTreeChunk QuadTreeChunk;
            public MaterialChunk MaterialChunk;

            public TextureChunk TextureChunk;
            public SkeletalMeshChunk SkeletalMeshChunk;
            public LUTSChunk LUTSChunk;
            public VertexAttributesChunk VertexAttributesChunk;

            public void Read(FileReader reader, Header header)
            {
                uint numIndices = reader.ReadUInt32();
                SkeletonChunk = ReadChunkSection<SkeletonChunk>(reader, header);
                if (header.Version >= CMBVersion.MM3DS)
                    QuadTreeChunk = ReadChunkSection<QuadTreeChunk>(reader, header);

                MaterialChunk = ReadChunkSection<MaterialChunk>(reader, header);
                TextureChunk = ReadChunkSection<TextureChunk>(reader, header);
                SkeletalMeshChunk = ReadChunkSection<SkeletalMeshChunk>(reader, header);
                LUTSChunk = ReadChunkSection<LUTSChunk>(reader, header);
                VertexAttributesChunk = ReadChunkSection<VertexAttributesChunk>(reader, header);

                uint indexBufferOffset = reader.ReadUInt32();
                uint textureDataOffset = reader.ReadUInt32();

                if (header.Version >= CMBVersion.MM3DS)
                    reader.ReadUInt32(); //Padding?

                if (VertexAttributesChunk != null)
                {
                    long bufferStart = VertexAttributesChunk.StartPosition;
                    foreach (var shape in SkeletalMeshChunk.ShapeChunk.SeperateShapes)
                    {
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.PositionSlice, shape.Position, 3, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.NormalSlice, shape.Normal, 3, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.TangentSlice, shape.Tangent, 3, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.ColorSlice, shape.Color, 4, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.Texcoord0Slice, shape.TexCoord0, 2, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.Texcoord1Slice, shape.TexCoord1, 2, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.Texcoord2Slice, shape.TexCoord2, 2, bufferStart);

                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.BoneIndicesSlice, shape.BoneIndices, shape.boneDimension, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.BoneWeightsSlice, shape.BoneWeights, shape.boneDimension, bufferStart);
                    }
                }

                if (indexBufferOffset != 0)
                {
                    foreach (var shape in SkeletalMeshChunk.ShapeChunk.SeperateShapes)
                    {
                        foreach (var prim in shape.Primatives)
                        {
                            foreach (var subprim in prim.Primatives) //Note 3DS usually only has one sub primative
                            {
                                subprim.Indices = new uint[subprim.IndexCount];

                                reader.SeekBegin(indexBufferOffset + subprim.Offset);

                                switch (subprim.IndexType)
                                {
                                    case CmbDataType.UByte:
                                        for (int i = 0; i < subprim.IndexCount; i++)
                                            subprim.Indices[i] = reader.ReadByte();
                                        break;
                                    case CmbDataType.UShort:
                                        for (int i = 0; i < subprim.IndexCount; i++)
                                            subprim.Indices[i] = reader.ReadUInt16();
                                        break;
                                    case CmbDataType.UInt:
                                        for (int i = 0; i < subprim.IndexCount; i++)
                                            subprim.Indices[i] = reader.ReadUInt32();
                                        break;
                                    default:
                                        throw new Exception("Unsupported index type! " + subprim.IndexType);
                                }
                            }
                        }
                    }
                }

                foreach (var tex in TextureChunk.Textures)
                {
                    reader.SeekBegin(textureDataOffset + tex.DataOffset);
                    tex.ImageData = reader.ReadBytes((int)tex.ImageSize);
                }
            }

            private static void ReadVertexDataFromSlice(FileReader reader, BufferSlice Slice, SepdVertexAttribute VertexAttribute, int elementCount, long bufferStart)
            {
                if (Slice == null || Slice.Size == 0)
                    return;

                reader.SeekBegin(bufferStart + VertexAttribute.StartPosition + Slice.Offset);

                int StrideSize = CalculateStrideSize(VertexAttribute.Type, elementCount);
                int VertexCount = (int)Slice.Size / StrideSize;

                VertexAttribute.VertexData = new Syroot.Maths.Vector4F[VertexCount];
                if (VertexAttribute.Mode == SepdVertexAttribMode.ARRAY)
                {
                    for (int v = 0; v < VertexCount; v++) {
                        VertexAttribute.VertexData[v] = ReadVertexBufferData(reader, VertexAttribute, elementCount);
                    }
                }
                else
                {
                    VertexAttribute.VertexData[0] = new Syroot.Maths.Vector4F(
                        VertexAttribute.Constants[0],
                        VertexAttribute.Constants[1],
                        VertexAttribute.Constants[2],
                        VertexAttribute.Constants[3]);
                }

            }

            private static Syroot.Maths.Vector4F ReadVertexBufferData(FileReader reader, SepdVertexAttribute VertexAttribute, int elementCount)
            {
                List<float> values = new List<float>();

                for (int i = 0; i < elementCount; i++)
                {
                    switch (VertexAttribute.Type)
                    {
                        case CmbDataType.Byte:
                            values.Add(reader.ReadSByte());
                            break;
                        case CmbDataType.Float:
                            values.Add(reader.ReadSingle());
                            break;
                        case CmbDataType.Int:
                            values.Add(reader.ReadInt32());
                            break;
                        case CmbDataType.Short:
                            values.Add(reader.ReadInt16());
                            break;
                        case CmbDataType.UByte:
                            values.Add(reader.ReadByte());
                            break;
                        case CmbDataType.UInt:
                            values.Add(reader.ReadUInt32());
                            break;
                        case CmbDataType.UShort:
                            values.Add(reader.ReadUInt16());
                            break;
                        default: throw new Exception("Unknown format! " + VertexAttribute.Type);
                    }
                }

                while (values.Count < 4) values.Add(0);

                return new Syroot.Maths.Vector4F(
                    values[0] * VertexAttribute.Scale,
                    values[1] * VertexAttribute.Scale, 
                    values[2] * VertexAttribute.Scale, 
                    values[3] * VertexAttribute.Scale);
            }

            private static void WriteVertexBufferData(FileWriter writer, long startPos, BufferSlice Slice, 
                SepdVertexAttribute VertexAttribute, int elementCount)
            {
                if (Slice == null || VertexAttribute == null)
                    return;

                writer.SeekBegin(startPos + VertexAttribute.StartPosition + Slice.Offset);
                for (int v = 0; v < VertexAttribute.VertexData?.Length; v++) {
                    WriteVertexBufferData(writer, VertexAttribute, VertexAttribute.VertexData[v], elementCount);
                }
            }

            private static void WriteVertexBufferData(FileWriter writer, SepdVertexAttribute VertexAttribute,
                Syroot.Maths.Vector4F value, int elementCount)
            {
                float[] values = new float[4] { value.X, value.Y, value.Z, value.W };

                for (int i = 0; i < elementCount; i++)
                {
                    switch (VertexAttribute.Type)
                    {
                        case CmbDataType.Byte:
                            writer.Write((sbyte)values[i]);
                            break;
                        case CmbDataType.Float:
                            writer.Write(values[i]);
                            break;
                        case CmbDataType.Int:
                            writer.Write((int)values[i]);
                            break;
                        case CmbDataType.Short:
                            writer.Write((short)values[i]);
                            break;
                        case CmbDataType.UByte:
                            writer.Write((byte)values[i]);
                            break;
                        case CmbDataType.UInt:
                            writer.Write((uint)values[i]);
                            break;
                        case CmbDataType.UShort:
                            writer.Write((ushort)values[i]);
                            break;
                        default: throw new Exception("Unknown format! " + VertexAttribute.Type);
                    }
                }
            }

            private static int CalculateStrideSize(CmbDataType type, int elementCount)
            {
                switch (type)
                {
                    case CmbDataType.Byte: return elementCount * sizeof(sbyte); 
                    case CmbDataType.Float: return elementCount * sizeof(float); 
                    case CmbDataType.Int: return elementCount * sizeof(int); 
                    case CmbDataType.Short: return elementCount * sizeof(short);
                    case CmbDataType.UByte: return elementCount * sizeof(byte);
                    case CmbDataType.UInt: return elementCount * sizeof(uint); 
                    case CmbDataType.UShort: return elementCount * sizeof(ushort);
                    default: throw new Exception("Unknwon format! " + type);
                }
            }

            private uint GetTotalIndexCount()
            {
                uint total = 0;
                foreach (var shape in SkeletalMeshChunk.ShapeChunk.SeperateShapes)
                {
                    foreach (var prim in shape.Primatives)
                    {
                        foreach (var subprim in prim.Primatives) //Note 3DS usually only has one sub primative
                            total += (uint)subprim.Indices.Length;
                    }
                }
                return total;
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.Write(GetTotalIndexCount());
                //Reserve space for all the offses
                writer.Write(0); //SkeletonChunk
                if (header.Version >= CMBVersion.MM3DS)
                    writer.Write(0); //QuadTreeChunk
                writer.Write(0); //MaterialChunk
                writer.Write(0); //TextureChunk
                writer.Write(0); //SkeletalMeshChunk
                writer.Write(0); //LUTSChunk
                writer.Write(0); //VertexAttributesChunk
                writer.Write(0); //indexBufferOffset
                writer.Write(0); //textureDataOffset

                if (header.Version >= CMBVersion.MM3DS)
                    writer.Write(0); //padding or unknown unused section

                //Write sections and offsets
                int _offsetPos = 4;
                if (SkeletonChunk != null)
                {
                    writer.WriteUint32Offset(pos + _offsetPos);
                    SkeletonChunk.Write(writer, header);
                }

                if (QuadTreeChunk != null && header.Version >= CMBVersion.MM3DS)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos += 4));
                    QuadTreeChunk.Write(writer, header);
                }

                _offsetPos += 4;
                if (MaterialChunk != null)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos));
                    MaterialChunk.Write(writer, header);
                }

                _offsetPos += 4;
                if (TextureChunk != null)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos));
                    TextureChunk.Write(writer, header);
                }

                _offsetPos += 4;
                if (SkeletalMeshChunk != null)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos));
                    SkeletalMeshChunk.Write(writer, header);
                }

                _offsetPos += 4;
                if (LUTSChunk != null)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos));
                    LUTSChunk.Write(writer, header);
                }

                _offsetPos += 4;
                if (VertexAttributesChunk != null)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos));
                    long vatrPos = writer.Position;
                    VertexAttributesChunk.Write(writer, header);
                    foreach (var shape in SkeletalMeshChunk.ShapeChunk.SeperateShapes)
                    {
                        WriteVertexBufferData(writer, vatrPos, VertexAttributesChunk.PositionSlice, shape.Position, 3);
                        WriteVertexBufferData(writer, vatrPos, VertexAttributesChunk.NormalSlice, shape.Normal, 3);
                        WriteVertexBufferData(writer, vatrPos, VertexAttributesChunk.TangentSlice, shape.Tangent, 3);
                        WriteVertexBufferData(writer, vatrPos, VertexAttributesChunk.ColorSlice, shape.Color, 4);
                        WriteVertexBufferData(writer, vatrPos, VertexAttributesChunk.Texcoord0Slice, shape.TexCoord0, 2);
                        WriteVertexBufferData(writer, vatrPos, VertexAttributesChunk.Texcoord1Slice, shape.TexCoord1, 2);
                        WriteVertexBufferData(writer, vatrPos, VertexAttributesChunk.Texcoord2Slice, shape.TexCoord2, 2);

                        WriteVertexBufferData(writer, vatrPos, VertexAttributesChunk.BoneIndicesSlice, shape.BoneIndices, shape.boneDimension);
                        WriteVertexBufferData(writer, vatrPos, VertexAttributesChunk.BoneWeightsSlice, shape.BoneWeights, shape.boneDimension);
                    }
                    writer.WriteSectionSizeU32(vatrPos + 4, vatrPos, writer.Position);
                }

                if (SkeletalMeshChunk != null && SkeletalMeshChunk.ShapeChunk.SeperateShapes.Count > 0)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos += 4));

                    long indexBufferPos = writer.Position;
                    foreach (var shape in SkeletalMeshChunk.ShapeChunk.SeperateShapes)
                    {
                        foreach (var prim in shape.Primatives)
                        {
                            foreach (var subprim in prim.Primatives) //Note 3DS usually only has one sub primative
                            {
                                writer.SeekBegin(indexBufferPos + subprim.Offset);

                                switch (subprim.IndexType)
                                {
                                    case CmbDataType.UByte:
                                        for (int i = 0; i < subprim.IndexCount; i++)
                                            writer.Write((byte)subprim.Indices[i]);
                                        break;
                                    case CmbDataType.UShort:
                                        for (int i = 0; i < subprim.IndexCount; i++)
                                            writer.Write((ushort)subprim.Indices[i]);
                                        break;
                                    case CmbDataType.UInt:
                                        for (int i = 0; i < subprim.IndexCount; i++)
                                            writer.Write((uint)subprim.Indices[i]);
                                        break;
                                    default:
                                        throw new Exception("Unsupported index type! " + subprim.IndexType);
                                }
                            }
                        }
                    }
                    writer.Align(64);
                }

                if (TextureChunk != null && TextureChunk.Textures.Count > 0)
                {
                    long dataStart = writer.Position;
                    writer.WriteUint32Offset(pos + (_offsetPos += 4));
                    //Save image data
                    foreach (var tex in TextureChunk.Textures)
                        writer.Write(tex.ImageData);
                }
            }
        }

        //Connects all the meshes, vertex attributes, and shape data together
        public class SkeletalMeshChunk : IChunkCommon
        {
            private const string Magic = "sklm";

            public MeshesChunk MeshChunk { get; set; }
            public ShapesChunk ShapeChunk { get; set; }

            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                MeshChunk = ReadChunkSection<MeshesChunk>(reader, header, pos);
                ShapeChunk = ReadChunkSection<ShapesChunk>(reader, header,pos);
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                long _offsetPos = writer.Position;
                writer.Write(uint.MaxValue);//MeshChunk
                writer.Write(uint.MaxValue);//ShapeChunk

                if (MeshChunk != null)
                {
                    writer.WriteUint32Offset(_offsetPos, pos);
                    MeshChunk.Write(writer, header);
                }

                if (ShapeChunk != null)
                {
                    writer.WriteUint32Offset(_offsetPos + 4, pos);
                    ShapeChunk.Write(writer, header);
                }


                long endPos = writer.Position;
                using (writer.TemporarySeek(pos + 4, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)(endPos - pos));
                }
            }
        }

        public class MeshesChunk : IChunkCommon
        {
            private const string Magic = "mshs";

            public List<Mesh> Meshes = new List<Mesh>();

            public uint Unknown;

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint meshCount = reader.ReadUInt32();
                Unknown = reader.ReadUInt32();

                long meshPos = reader.Position;
                for (int i = 0; i < meshCount; i++)
                {
                    Mesh mesh = new Mesh();

                    mesh.SepdIndex = reader.ReadUInt16();
                    mesh.MaterialIndex = reader.ReadByte();
                    mesh.ID = reader.ReadByte();
                    Meshes.Add(mesh);

                    if (header.Version == CMBVersion.MM3DS)
                        mesh.unks = reader.ReadBytes(8);
                    else if (header.Version >= CMBVersion.LM3DS)
                        mesh.unks = reader.ReadBytes(84);

                    Console.WriteLine($"SepdIndex {mesh.SepdIndex}");
                    Console.WriteLine($"MaterialIndex { mesh.MaterialIndex}");
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(Meshes.Count);
                writer.Write(Unknown);

                for (int i = 0; i < Meshes.Count; i++) {
                    writer.Write(Meshes[i].SepdIndex);
                    writer.Write(Meshes[i].MaterialIndex);
                    writer.Write(Meshes[i].ID);
                    if (Meshes[i].unks != null)
                        writer.Write(Meshes[i].unks);
                }

                long endPos = writer.Position;
                using (writer.TemporarySeek(pos + 4, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)(endPos - pos));
                }
            }

            public class Mesh
            {
                public byte[] unks;

                public ushort SepdIndex { get; set; }
                public byte MaterialIndex { get; set; }
                public byte ID { get; set; }
            }
        }

        public class ShapesChunk : IChunkCommon
        {
            private const string Magic = "shp ";

            public uint Unknown;

            public List<SeperateShape> SeperateShapes = new List<SeperateShape>();

            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint sepdCount = reader.ReadUInt32();
                Unknown = reader.ReadUInt32();
                ushort[] offsets = reader.ReadUInt16s((int)sepdCount);
                for (int i = 0; i < sepdCount; i++)
                {
                    reader.SeekBegin(pos + offsets[i]);
                    var sepd = new SeperateShape();
                    sepd.Read(reader, header);
                    SeperateShapes.Add(sepd);
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(SeperateShapes.Count);
                writer.Write(Unknown);
                long offsetPos = writer.Position;
                writer.Write(new ushort[SeperateShapes.Count]);
                for (int i = 0; i < SeperateShapes.Count; i++)
                {
                    writer.WriteUint16Offset(offsetPos + (i * 2), pos);
                    SeperateShapes[i].Write(writer, header);
                }

                long endPos = writer.Position;
                using (writer.TemporarySeek(pos + 4, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)(endPos - pos));
                }
            }
        }

        public class SeperateShape : IChunkCommon
        {
            private const string Magic = "sepd";

            public SepdVertexAttribute Position { get; set; }
            public SepdVertexAttribute Normal { get; set; }
            public SepdVertexAttribute Tangent { get; set; }
            public SepdVertexAttribute Color { get; set; }
            public SepdVertexAttribute TexCoord0 { get; set; }
            public SepdVertexAttribute TexCoord1 { get; set; }
            public SepdVertexAttribute TexCoord2 { get; set; }
            public SepdVertexAttribute BoneIndices { get; set; }
            public SepdVertexAttribute BoneWeights { get; set; }

            public List<PrimativesChunk> Primatives = new List<PrimativesChunk>();

            public ushort boneDimension;
            public ushort Unknown;

            private byte[] unks;

            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint count = reader.ReadUInt16();

                if (header.Version >= CMBVersion.LM3DS)
                    unks = reader.ReadBytes(50);
                else
                    unks = reader.ReadBytes(26);

                Position = ReadVertexAttrib(reader);
                Normal = ReadVertexAttrib(reader);
                if (header.Version >= CMBVersion.MM3DS)
                    Tangent = ReadVertexAttrib(reader);

                Color = ReadVertexAttrib(reader);
                TexCoord0 = ReadVertexAttrib(reader);
                TexCoord1 = ReadVertexAttrib(reader);
                TexCoord2 = ReadVertexAttrib(reader);
                BoneIndices = ReadVertexAttrib(reader);
                BoneWeights = ReadVertexAttrib(reader);

                boneDimension = reader.ReadUInt16();
                Unknown = reader.ReadUInt16();

                ushort[] Offsets = reader.ReadUInt16s((int)count);

                for (int i = 0; i < count; i++)
                {
                    reader.SeekBegin(pos + Offsets[i]);
                    PrimativesChunk prim = new PrimativesChunk();
                    prim.Read(reader, header);
                    Primatives.Add(prim);
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue); //section size
                writer.Write((ushort)Primatives.Count);
                writer.Write(unks);
                WriteVertexAttrib(writer, Position);
                WriteVertexAttrib(writer, Normal);
                if (header.Version >= CMBVersion.MM3DS)
                    WriteVertexAttrib(writer, Tangent);

                WriteVertexAttrib(writer, Color);
                WriteVertexAttrib(writer, TexCoord0);
                WriteVertexAttrib(writer, TexCoord1);
                WriteVertexAttrib(writer, TexCoord2);
                WriteVertexAttrib(writer, BoneIndices);
                WriteVertexAttrib(writer, BoneWeights);
                writer.Write((ushort)boneDimension);
                writer.Write((ushort)Unknown);

                long offsetPos = writer.Position;
                writer.Write(new ushort[Primatives.Count]);
                writer.Write((ushort)0);
                for (int i = 0; i < Primatives.Count; i++)
                {
                    writer.WriteUint16Offset(offsetPos + (i * 2), pos);
                    Primatives[i].Write(writer, header);
                }

                long endPos = writer.Position;
                using (writer.TemporarySeek(pos + 4, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)(endPos - pos));
                }
            }

            private SepdVertexAttribute ReadVertexAttrib(FileReader reader)
            {
                long pos = reader.Position;

                SepdVertexAttribute att = new SepdVertexAttribute();
                att.StartPosition = reader.ReadUInt32();
                att.Scale = reader.ReadSingle();
                att.Type = reader.ReadEnum<CmbDataType>(true);
                att.Mode = reader.ReadEnum<SepdVertexAttribMode>(true);
                att.Constants = new float[4];
                att.Constants[0] = reader.ReadSingle();
                att.Constants[1] = reader.ReadSingle();
                att.Constants[2] = reader.ReadSingle();
                att.Constants[3] = reader.ReadSingle();

                reader.SeekBegin(pos + 0x1C);

                return att;
            }

            private void WriteVertexAttrib(FileWriter writer, SepdVertexAttribute att)
            {
                long pos = writer.Position;

                writer.Write(att.StartPosition);
                writer.Write(att.Scale);
                writer.Write(att.Type, true);
                writer.Write(att.Mode, true);
                writer.Write(att.Constants[0]);
                writer.Write(att.Constants[1]);
                writer.Write(att.Constants[2]);
                writer.Write(att.Constants[3]);

                writer.SeekBegin(pos + 0x1C);
            }
        }

        public class SepdVertexAttribute
        {
            public uint StartPosition { get; set; }
            public float Scale { get; set; }
            public CmbDataType Type { get; set; }
            public SepdVertexAttribMode Mode { get; set; }

            public Syroot.Maths.Vector4F[] VertexData { get; set; }

            public float[] Constants { get; set; }
        }

        public class PrimativesChunk : IChunkCommon
        {
            private const string Magic = "prms";

            public SkinningMode SkinningMode;

            public List<SubPrimativeChunk> Primatives = new List<SubPrimativeChunk>();

            public ushort[] BoneIndexTable { get; set; }

            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint count = reader.ReadUInt32();
                SkinningMode = reader.ReadEnum<SkinningMode>(true);
                ushort boneTableCount = reader.ReadUInt16();
                uint boneIndexOffset = reader.ReadUInt32();
                uint primativeOffset = reader.ReadUInt32();

                reader.SeekBegin(pos + boneIndexOffset);
                BoneIndexTable = reader.ReadUInt16s(boneTableCount);

                reader.SeekBegin(pos + primativeOffset);
                for (int i = 0; i < count; i++)
                {
                    SubPrimativeChunk prim = new SubPrimativeChunk();
                    prim.Read(reader, header);
                    Primatives.Add(prim);
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(Primatives.Count);
                writer.Write(SkinningMode, true);
                writer.Write((ushort)BoneIndexTable.Length);

                long boneIndexOfsPos = writer.Position;
                writer.Write(uint.MaxValue); //bone index offset

                long primativeOfsPos = writer.Position;
                writer.Write(uint.MaxValue); //primative offset

                writer.WriteUint32Offset(boneIndexOfsPos, pos);
                writer.Write(BoneIndexTable);
                writer.Align(4);

                writer.WriteUint32Offset(primativeOfsPos, pos);
                for (int i = 0; i < Primatives.Count; i++)
                    Primatives[i].Write(writer, header);

                long endPos = writer.Position;
                using (writer.TemporarySeek(pos + 4, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)(endPos - pos));
                }
            }
        }

        public class SubPrimativeChunk : IChunkCommon
        {
            private const string Magic = "prm ";

            public SkinningMode SkinningMode { get; private set; }

            public CmbDataType IndexType { get; private set; }

            public ushort IndexCount { get; private set; }

            public uint Offset { get; private set; }

            private uint[] _indices;
            public uint[] Indices
            {
                get
                {
                    return _indices;
                }
                set
                {
                    _indices = value;
                }
            }

            public uint Unknown;
            public uint Unknown2;

            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                Unknown = reader.ReadUInt32();
                Unknown2 = reader.ReadUInt32();
                IndexType = reader.ReadEnum<CmbDataType>(true);
                reader.Seek(2); //padding

                IndexCount = reader.ReadUInt16();

                //This value is the index, so we'll use it as an offset
                //Despite the data type, this is always * 2
                Offset = (uint)reader.ReadUInt16() * sizeof(ushort); 
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(Unknown);
                writer.Write(Unknown2);
                writer.Write(IndexType, true);
                writer.Seek(2);
                writer.Write(IndexCount);
                writer.Write((ushort)(Offset / sizeof(ushort)));

                long endPos = writer.Position;
                using (writer.TemporarySeek(pos + 4, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)(endPos - pos));
                }
            }
        }

        public class LUTSChunk : IChunkCommon
        {
            private const string Magic = "luts";

            private byte[] data;

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();

                data = reader.getSection((uint)reader.Position, sectionSize - 8);
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(data);

                long endPos = writer.Position;
                using (writer.TemporarySeek(pos + 4, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)(endPos - pos));
                }
            }
        }

        public class VertexAttributesChunk : IChunkCommon
        {
            private const string Magic = "vatr";

            public BufferSlice PositionSlice;
            public BufferSlice NormalSlice;
            public BufferSlice TangentSlice; //Used in MM3DS and newer
            public BufferSlice ColorSlice;
            public BufferSlice Texcoord0Slice;
            public BufferSlice Texcoord1Slice;
            public BufferSlice Texcoord2Slice;
            public BufferSlice BoneIndicesSlice;
            public BufferSlice BoneWeightsSlice;

            public long StartPosition;

            public uint MaxIndex;

            public void Read(FileReader reader, Header header)
            {
                StartPosition = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                MaxIndex = reader.ReadUInt32();

                PositionSlice = ReadSlice(reader);
                NormalSlice = ReadSlice(reader);
                if (header.Version >= CMBVersion.MM3DS)
                    TangentSlice = ReadSlice(reader);

                ColorSlice = ReadSlice(reader);
                Texcoord0Slice = ReadSlice(reader);
                Texcoord1Slice = ReadSlice(reader);
                Texcoord2Slice = ReadSlice(reader);
                BoneIndicesSlice = ReadSlice(reader);
                BoneWeightsSlice = ReadSlice(reader);
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(MaxIndex);
                WriteSlice(writer, PositionSlice);
                WriteSlice(writer, NormalSlice);
                if (header.Version >= CMBVersion.MM3DS)
                    WriteSlice(writer, TangentSlice);

                WriteSlice(writer, ColorSlice);
                WriteSlice(writer, Texcoord0Slice);
                WriteSlice(writer, Texcoord1Slice);
                WriteSlice(writer, Texcoord2Slice);
                WriteSlice(writer, BoneIndicesSlice);
                WriteSlice(writer, BoneWeightsSlice);

                long endPos = writer.Position;
                using (writer.TemporarySeek(pos + 4, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)(endPos - pos));
                }
            }

            private void WriteSlice(FileWriter writer, BufferSlice slice) {
                writer.Write(slice.Size);
                writer.Write(slice.Offset);
            }

            private BufferSlice ReadSlice(FileReader reader)
            {
                BufferSlice slice = new BufferSlice();
                slice.Size = reader.ReadUInt32();
                slice.Offset = reader.ReadUInt32();
                return slice;
            }
        }

        public class BufferSlice
        {
            public uint Offset;
            public uint Size;
        }

        public class SkeletonChunk : IChunkCommon
        {
            private const string Magic = "skl ";

            public List<BoneChunk> Bones = new List<BoneChunk>();

            public uint Unknown;

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint boneCount = reader.ReadUInt32();
                Unknown = reader.ReadUInt32();

                for (int i = 0; i < boneCount; i++)
                {
                    BoneChunk bone = new BoneChunk();
                    bone.ID = reader.ReadInt16() & 0xFFFF;
                    bone.ParentIndex = reader.ReadInt16();
                    bone.Scale = reader.ReadVec3SY();
                    bone.Rotation = reader.ReadVec3SY();
                    bone.Translation = reader.ReadVec3SY();
                    if (header.Version >= CMBVersion.MM3DS)
                        bone.Unknown = reader.ReadInt32();

                    Bones.Add(bone);
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(Bones.Count);
                writer.Write(Unknown);
                for (int i = 0; i < Bones.Count; i++)
                {
                    writer.Write((ushort)Bones[i].ID);
                    writer.Write((short)Bones[i].ParentIndex);
                    writer.Write(Bones[i].Scale);
                    writer.Write(Bones[i].Rotation);
                    writer.Write(Bones[i].Translation);
                    if (header.Version >= CMBVersion.MM3DS)
                        writer.Write(Bones[i].Unknown);
                }

                long endPos = writer.Position;
                using (writer.TemporarySeek(pos + 4, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)(endPos - pos));
                }
            }
        }

        public class BoneChunk
        {
            public int ID { get; set; }
            public int ParentIndex { get; set; }

            public Syroot.Maths.Vector3F Scale { get; set; }
            public Syroot.Maths.Vector3F Rotation { get; set; }
            public Syroot.Maths.Vector3F Translation { get; set; }

            //An unknown value used in versions MM3DS and newer
            public int Unknown { get; set; }
        }

        public class QuadTreeChunk : IChunkCommon
        {
            private const string Magic = "qtrs";

            byte[] data;

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();

                data = reader.getSection((uint)reader.Position, sectionSize);
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(data);

                long endPos = writer.Position;
                using (writer.TemporarySeek(pos + 4, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)(endPos - pos));
                }
            }
        }

        public class MaterialChunk : IChunkCommon
        {
            private const string Magic = "mats";

            public List<Material> Materials = new List<Material>();

            internal int textureCombinerSettingsTableOffs;
            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint count = reader.ReadUInt32();

                int materialSize = 0x15C;
                if (header.Version >= CMBVersion.MM3DS)
                    materialSize = 0x16C;

                Console.WriteLine($"materialSize {materialSize.ToString("x")}");

                textureCombinerSettingsTableOffs = (int)(pos + 12 + (count * materialSize));

                for (int i = 0; i < count; i++)
                {
                    reader.SeekBegin(pos + 0xC + (i * materialSize));

                    Material mat = new Material();
                    mat.Read(reader, header, this);
                    Materials.Add(mat);
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(Materials.Count);

                for (int i = 0; i < Materials.Count; i++)
                    Materials[i].Write(writer, header);
                for (int i = 0; i < Materials.Count; i++) {
                    foreach (var combiner in Materials[i].TextureCombiners)
                    {
                        writer.Write(combiner.combineRGB, false);
                        writer.Write(combiner.combineAlpha, false);
                        writer.Write(combiner.scaleRGB, false);
                        writer.Write(combiner.scaleAlpha, false);
                        writer.Write(combiner.bufferInputRGB, false);
                        writer.Write(combiner.bufferInputAlpha, false);
                        writer.Write(combiner.source0RGB, false);
                        writer.Write(combiner.source1RGB, false);
                        writer.Write(combiner.source2RGB, false);
                        writer.Write(combiner.op0RGB, false);
                        writer.Write(combiner.op1RGB, false);
                        writer.Write(combiner.op2RGB, false);
                        writer.Write(combiner.source0Alpha, false);
                        writer.Write(combiner.source1Alpha, false);
                        writer.Write(combiner.source2Alpha, false);
                        writer.Write(combiner.op0Alpha, false);
                        writer.Write(combiner.op1Alpha, false);
                        writer.Write(combiner.op2Alpha, false);
                        writer.Write(combiner.constantIndex);
                    }
                }

                long endPos = writer.Position;
                using (writer.TemporarySeek(pos + 4, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)(endPos - pos));
                }
            }
        }

        //Thanks for noclip for material RE stuff
        //https://github.com/magcius/noclip.website/blob/9270b9e5022c691703689990f9c536cd9058e5cd/src/oot3d/cmb.ts#L232
        public class Material
        {
            private Header CMBHeader;

            private H3DMaterial ctrMaterial;
            public H3DMaterial CTRMaterial
            {
                get
                {
                    if (ctrMaterial == null)
                        ctrMaterial = ToH3DMaterial();
                    return ctrMaterial;
                }
            }

            public bool IsTransparent = false;

            public bool IsFragmentLightingEnabled;
            public bool IsVertexLightingEnabled;
            public bool IsHemiSphereLightingEnabled;
            public bool IsHemiSphereOcclusionEnabled;

            public CullMode CullMode { get; set; }

            public bool IsPolygonOffsetEnabled { get; set; }
            public ushort PolygonOffset { get; set; }

            public TextureMap[] TextureMaps { get; set; }
            public TextureMatrix[] TextureMaticies { get; set; }

            public ushort LightSetIndex { get; set; }
            public ushort FogIndex { get; set; }

            public List<TextureCombiner> TextureCombiners { get; set; }

            public STColor8[] ConstantColors { get; set; }

            public bool AlphaTestEnable { get; set; }
            public float AlphaTestReference { get; set; }

            public bool DepthTestEnable { get; set; }

            public bool DepthWriteEnable { get; set; }

            public AlphaFunction AlphaTestFunction { get; set; }

            public DepthFunction DepthTestFunction { get; set; }

            public bool BlendEnaled { get; set; }

            public BlendingFactor BlendingFactorSrcRGB { get; set; }
            public BlendingFactor BlendingFactorDestRGB { get; set; }
            public BlendEquationMode BlendingEquationRGB { get; set; }

            public BlendingFactor BlendingFactorSrcAlpha { get; set; }
            public BlendingFactor BlendingFactorDestAlpha { get; set; }
            public BlendEquationMode BlendingEquationAlpha { get; set; }


            public float BlendColorR { get; set; }
            public float BlendColorG { get; set; }
            public float BlendColorB { get; set; }
            public float BlendColorA { get; set; }

            public float BufferColorR { get; set; }
            public float BufferColorG { get; set; }
            public float BufferColorB { get; set; }
            public float BufferColorA { get; set; }

            public short BumpMapIndex { get; set; }
            public ushort BumpMapMode { get; set; }
            public short IsBumpRenormalize { get; set; }

            public LayerConfig LayerConfig { get; set; }

            public FresnelSelector FresnelSelector { get; set; }

            public bool IsClampHighLight { get; set; }
            public bool IsDistribution0Enabled { get; set; }
            public bool IsDistribution1Enabled { get; set; }
            public bool IsGeometricFactor0Enabled { get; set; }
            public bool IsGeometricFactor1Enabled { get; set; }
            public bool IsReflectionEnabled { get; set; }

            private byte[] data;

            public H3DMaterial ToH3DMaterial()
            {
                H3DMaterial h3dMaterial = new H3DMaterial();
                var matParams = h3dMaterial.MaterialParams;

                if (IsVertexLightingEnabled)
                    matParams.Flags |= H3DMaterialFlags.IsVertexLightingEnabled;
                if (IsFragmentLightingEnabled)
                    matParams.Flags |= H3DMaterialFlags.IsFragmentLightingEnabled;
                if (IsHemiSphereLightingEnabled)
                    matParams.Flags |= H3DMaterialFlags.IsHemiSphereLightingEnabled;
                if (IsHemiSphereOcclusionEnabled)
                    matParams.Flags |= H3DMaterialFlags.IsHemiSphereOcclusionEnabled;

                switch (LayerConfig)
                {
                    case LayerConfig.LayerConfig0: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig0; break;
                    case LayerConfig.LayerConfig1: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig1; break;
                    case LayerConfig.LayerConfig2: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig2; break;
                    case LayerConfig.LayerConfig3: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig3; break;
                    case LayerConfig.LayerConfig4: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig4; break;
                    case LayerConfig.LayerConfig5: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig5; break;
                    case LayerConfig.LayerConfig6: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig6; break;
                    case LayerConfig.LayerConfig7: matParams.TranslucencyKind = H3DTranslucencyKind.LayerConfig7; break;
                }

                for (int i = 0; i < TextureMaps?.Length; i++)
                {
                    string texture = GetTextureName(TextureMaps[i].TextureIndex);
                    if (texture == string.Empty)
                        continue;

                    if (i == 0) h3dMaterial.Texture0Name = texture;
                    if (i == 0) h3dMaterial.Texture1Name = texture;
                    if (i == 0) h3dMaterial.Texture2Name = texture;

                    h3dMaterial.TextureMappers[i].WrapU = ConvertWrapMode(TextureMaps[i].WrapS);
                    h3dMaterial.TextureMappers[i].WrapV = ConvertWrapMode(TextureMaps[i].WrapT);
                    h3dMaterial.TextureMappers[i].MagFilter = ConvertTexMagFilter(TextureMaps[i].MagFiler);
                    h3dMaterial.TextureMappers[i].MinFilter = ConvertTexMinFilter(TextureMaps[i].MinFiler);
                    h3dMaterial.TextureMappers[i].LODBias = TextureMaps[i].LodBias;
                    h3dMaterial.TextureMappers[i].MinLOD = (byte)(TextureMaps[i].MinLOD / 255);
                    h3dMaterial.TextureMappers[i].BorderColor = new SPICA.Math3D.RGBA(
                        TextureMaps[i].borderColorR,
                        TextureMaps[i].borderColorG,
                        TextureMaps[i].borderColorB,
                        TextureMaps[i].borderColorA);

                    matParams.TextureCoords[i].TransformType = H3DTextureTransformType.DccMaya;
                    matParams.TextureCoords[i].MappingType = H3DTextureMappingType.UvCoordinateMap;

                    matParams.TextureCoords[i].Scale = new System.Numerics.Vector2(
                        TextureMaticies[i].Scale.X, TextureMaticies[i].Scale.Y);
                    matParams.TextureCoords[i].Translation = new System.Numerics.Vector2(
                        TextureMaticies[i].Translate.X, TextureMaticies[i].Translate.Y);

                    matParams.TextureCoords[i].Rotation = TextureMaticies[i].Rotate;
                }

                matParams.DiffuseColor = new SPICA.Math3D.RGBA(255,255,255,255);
                matParams.Specular0Color = new SPICA.Math3D.RGBA(0, 0, 0, 255);
                matParams.Specular1Color = new SPICA.Math3D.RGBA(0, 0, 0, 255);
                matParams.EmissionColor = new SPICA.Math3D.RGBA(0, 0, 0, 255);
                matParams.Constant0Color = ConvertRGBA(ConstantColors[0]);
                matParams.Constant1Color = ConvertRGBA(ConstantColors[1]);
                matParams.Constant2Color = ConvertRGBA(ConstantColors[2]);
                matParams.Constant3Color = ConvertRGBA(ConstantColors[3]);
                matParams.Constant4Color = ConvertRGBA(ConstantColors[4]);
                matParams.Constant5Color = ConvertRGBA(ConstantColors[5]);
                matParams.BlendColor = ConvertRGBA(BlendColorR, BlendColorG, BlendColorB, BlendColorA);
                matParams.TexEnvBufferColor = ConvertRGBA(BufferColorR, BufferColorG, BufferColorB, BufferColorA);

                if (CullMode == CullMode.BACK)
                    matParams.FaceCulling = PICAFaceCulling.BackFace;
                else if (CullMode == CullMode.FRONT)
                    matParams.FaceCulling = PICAFaceCulling.FrontFace;
                else
                    matParams.FaceCulling = PICAFaceCulling.Never;

                matParams.AlphaTest.Enabled = AlphaTestEnable;
                matParams.AlphaTest.Function = ConvertAlphaFunction(AlphaTestFunction);
                matParams.AlphaTest.Reference = (byte)(AlphaTestReference / 0xff);
                matParams.BlendFunction.ColorSrcFunc = ConvertBlendFunc(BlendingFactorSrcRGB);
                matParams.BlendFunction.ColorDstFunc = ConvertBlendFunc(BlendingFactorDestRGB);
                matParams.BlendFunction.AlphaSrcFunc = ConvertBlendFunc(BlendingFactorSrcAlpha);
                matParams.BlendFunction.AlphaDstFunc = ConvertBlendFunc(BlendingFactorDestAlpha);

                for (int i = 0; i < TextureCombiners.Count; i++)
                {
                    var combiner = TextureCombiners[i];
                    var h3dStage = new PICATexEnvStage();
                /*    h3dStage.Source.Color[0] = ConvertCombinerSrc[combiner.source0RGB];
                    h3dStage.Source.Color[1] = ConvertCombinerSrc[combiner.source1RGB];
                    h3dStage.Source.Color[2] = ConvertCombinerSrc[combiner.source2RGB];
                    h3dStage.Source.Alpha[0] = ConvertCombinerSrc[combiner.source0Alpha];
                    h3dStage.Source.Alpha[1] = ConvertCombinerSrc[combiner.source1Alpha];
                    h3dStage.Source.Alpha[2] = ConvertCombinerSrc[combiner.source2Alpha];
                    h3dStage.Operand.Alpha[0] = ConvertConvertCombinerAlphaOp[combiner.op0Alpha];
                    h3dStage.Operand.Alpha[1] = ConvertConvertCombinerAlphaOp[combiner.op1Alpha];
                    h3dStage.Operand.Alpha[2] = ConvertConvertCombinerAlphaOp[combiner.op2Alpha];
                    h3dStage.Operand.Color[0] = ConvertConvertCombinerColorOp[combiner.op0RGB];
                    h3dStage.Operand.Color[1] = ConvertConvertCombinerColorOp[combiner.op1RGB];
                    h3dStage.Scale.Color = ConvertScale[combiner.scaleRGB];
                    h3dStage.Scale.Alpha = ConvertScale[combiner.scaleAlpha];
                    h3dStage.Combiner.Alpha = ConvertConvertCombiner[combiner.combineAlpha];
                    h3dStage.Combiner.Color = ConvertConvertCombiner[combiner.combineRGB];*/

                    matParams.TexEnvStages[i] = h3dStage;
                }

                matParams.LUTInputAbsolute.Dist0 = LUTTable.reflectance0SamplerIsAbs;
                matParams.LUTInputAbsolute.Dist1 = LUTTable.reflectance1SamplerIsAbs;
                matParams.LUTInputAbsolute.ReflecR = LUTTable.reflectanceRSamplerIsAbs;
                matParams.LUTInputAbsolute.ReflecG = LUTTable.reflectanceGSamplerIsAbs;
                matParams.LUTInputAbsolute.ReflecB = LUTTable.reflectanceBSamplerIsAbs;
                matParams.LUTInputAbsolute.Fresnel = LUTTable.fresnelSamplerIsAbs;

                return h3dMaterial;
            }

            private Dictionary<CombineResultOpDMP, PICATextureCombinerMode> ConvertConvertCombiner =
             new Dictionary<CombineResultOpDMP, PICATextureCombinerMode>()
             {
                    { CombineResultOpDMP.ADD, PICATextureCombinerMode.Add },
                    { CombineResultOpDMP.ADD_MULT, PICATextureCombinerMode.AddMult },
                    { CombineResultOpDMP.ADD_SIGNED, PICATextureCombinerMode.AddSigned },
                    { CombineResultOpDMP.DOT3_RGB, PICATextureCombinerMode.DotProduct3Rgb },
                    { CombineResultOpDMP.DOT3_RGBA, PICATextureCombinerMode.DotProduct3Rgba },
                    { CombineResultOpDMP.INTERPOLATE, PICATextureCombinerMode.Interpolate },
                    { CombineResultOpDMP.MODULATE, PICATextureCombinerMode.Modulate },
                    { CombineResultOpDMP.MULT_ADD, PICATextureCombinerMode.MultAdd },
                    { CombineResultOpDMP.REPLACE, PICATextureCombinerMode.Replace },
                    { CombineResultOpDMP.SUBTRACT, PICATextureCombinerMode.Subtract },
             };

            private Dictionary<CombineSourceDMP, PICATextureCombinerSource> ConvertCombinerSrc =
                new Dictionary<CombineSourceDMP, PICATextureCombinerSource>()
                {
                    { CombineSourceDMP.CONSTANT, PICATextureCombinerSource.Constant },
                    { CombineSourceDMP.FRAGMENT_PRIMARY_COLOR, PICATextureCombinerSource.FragmentPrimaryColor },
                    { CombineSourceDMP.FRAGMENT_SECONDARY_COLOR, PICATextureCombinerSource.FragmentSecondaryColor },
                    { CombineSourceDMP.PREVIOUS, PICATextureCombinerSource.Previous },
                    { CombineSourceDMP.PREVIOUS_BUFFER, PICATextureCombinerSource.PreviousBuffer },
                    { CombineSourceDMP.PRIMARY_COLOR, PICATextureCombinerSource.PrimaryColor },
                    { CombineSourceDMP.TEXTURE0, PICATextureCombinerSource.Texture0 },
                    { CombineSourceDMP.TEXTURE1, PICATextureCombinerSource.Texture1 },
                    { CombineSourceDMP.TEXTURE2, PICATextureCombinerSource.Texture2 },
                    { CombineSourceDMP.TEXTURE3, PICATextureCombinerSource.Texture3 },
                };

            private Dictionary<CombineOpDMP, PICATextureCombinerAlphaOp> ConvertConvertCombinerAlphaOp =
                new Dictionary<CombineOpDMP, PICATextureCombinerAlphaOp>()
                {
                    { CombineOpDMP.ONE_MINUS_SRC_COLOR, PICATextureCombinerAlphaOp.OneMinusAlpha },
                    { CombineOpDMP.ONE_MINUS_SRC_ALPHA, PICATextureCombinerAlphaOp.OneMinusAlpha },
                    { CombineOpDMP.ONE_MINUS_SRC_R, PICATextureCombinerAlphaOp.OneMinusRed },
                    { CombineOpDMP.ONE_MINUS_SRC_G, PICATextureCombinerAlphaOp.OneMinusGreen },
                    { CombineOpDMP.ONE_MINUS_SRC_B, PICATextureCombinerAlphaOp.OneMinusBlue },
                    { CombineOpDMP.SRC_ALPHA, PICATextureCombinerAlphaOp.Alpha },
                    { CombineOpDMP.SRC_COLOR, PICATextureCombinerAlphaOp.Alpha },
                    { CombineOpDMP.SRC_R, PICATextureCombinerAlphaOp.Red },
                    { CombineOpDMP.SRC_B, PICATextureCombinerAlphaOp.Blue },
                    { CombineOpDMP.SRC_G, PICATextureCombinerAlphaOp.Green },
                };

            private Dictionary<CombineOpDMP, PICATextureCombinerColorOp> ConvertConvertCombinerColorOp =
                new Dictionary<CombineOpDMP, PICATextureCombinerColorOp>()
                {
                    { CombineOpDMP.ONE_MINUS_SRC_COLOR, PICATextureCombinerColorOp.OneMinusColor },
                    { CombineOpDMP.ONE_MINUS_SRC_ALPHA, PICATextureCombinerColorOp.OneMinusAlpha },
                    { CombineOpDMP.ONE_MINUS_SRC_R, PICATextureCombinerColorOp.OneMinusRed },
                    { CombineOpDMP.ONE_MINUS_SRC_G, PICATextureCombinerColorOp.OneMinusGreen },
                    { CombineOpDMP.ONE_MINUS_SRC_B, PICATextureCombinerColorOp.OneMinusBlue },
                    { CombineOpDMP.SRC_ALPHA, PICATextureCombinerColorOp.Alpha },
                    { CombineOpDMP.SRC_COLOR, PICATextureCombinerColorOp.Color },
                    { CombineOpDMP.SRC_R, PICATextureCombinerColorOp.Red },
                    { CombineOpDMP.SRC_B, PICATextureCombinerColorOp.Blue },
                    { CombineOpDMP.SRC_G, PICATextureCombinerColorOp.Green },
                };

            private Dictionary<CombineScaleDMP, PICATextureCombinerScale> ConvertScale =
                new Dictionary<CombineScaleDMP, PICATextureCombinerScale>()
                {
                    { CombineScaleDMP._1, PICATextureCombinerScale.One },
                    { CombineScaleDMP._2, PICATextureCombinerScale.Two },
                    { CombineScaleDMP._4, PICATextureCombinerScale.Four },
                };


            private PICABlendEquation ConvertEquation()
            {
                return PICABlendEquation.FuncAdd;
            }

            private PICABlendFunc ConvertBlendFunc(BlendingFactor factor)
            {
                switch (factor)
                {
                    case BlendingFactor.ConstantAlpha: return PICABlendFunc.ConstantAlpha;
                    case BlendingFactor.ConstantColor: return PICABlendFunc.ConstantColor;
                    case BlendingFactor.DstAlpha: return PICABlendFunc.DestinationAlpha;
                    case BlendingFactor.DstColor: return PICABlendFunc.DestinationColor;
                    case BlendingFactor.One: return PICABlendFunc.One;
                    case BlendingFactor.OneMinusConstantAlpha: return PICABlendFunc.OneMinusConstantAlpha;
                    case BlendingFactor.OneMinusConstantColor: return PICABlendFunc.OneMinusConstantColor;
                    case BlendingFactor.OneMinusDstAlpha: return PICABlendFunc.OneMinusDestinationAlpha;
                    case BlendingFactor.OneMinusDstColor: return PICABlendFunc.OneMinusDestinationColor;
                    case BlendingFactor.OneMinusSrcAlpha: return PICABlendFunc.OneMinusSourceAlpha;
                    case BlendingFactor.OneMinusSrcColor: return PICABlendFunc.OneMinusSourceColor;
                    case BlendingFactor.Src1Alpha: return PICABlendFunc.SourceAlpha;
                    case BlendingFactor.Src1Color: return PICABlendFunc.SourceColor;
                    case BlendingFactor.SrcAlpha: return PICABlendFunc.SourceColor;
                    case BlendingFactor.SrcColor: return PICABlendFunc.SourceColor;
                    case BlendingFactor.SrcAlphaSaturate: return PICABlendFunc.SourceAlphaSaturate;
                    case BlendingFactor.Zero: return PICABlendFunc.Zero;
                    default: return PICABlendFunc.Zero;
                }
            }

            private PICATestFunc ConvertAlphaFunction(AlphaFunction func)
            {
                switch (func)
                {
                    case AlphaFunction.Always: return PICATestFunc.Always;
                    case AlphaFunction.Equal: return PICATestFunc.Equal;
                    case AlphaFunction.Gequal: return PICATestFunc.Gequal;
                    case AlphaFunction.Greater: return PICATestFunc.Greater;
                    case AlphaFunction.Lequal: return PICATestFunc.Lequal ;
                    case AlphaFunction.Less: return PICATestFunc.Less;
                    case AlphaFunction.Never: return PICATestFunc.Never;
                    case AlphaFunction.Notequal: return PICATestFunc.Notequal;
                    default: return PICATestFunc.Always;
                }
            }

            private SPICA.Math3D.RGBA ConvertRGBA(STColor8 color)
            {
                return new SPICA.Math3D.RGBA(color.R, color.G, color.B, color.A);
            }

            private SPICA.Math3D.RGBA ConvertRGBA(float R, float G, float B, float A = 255)
            {
                return new SPICA.Math3D.RGBA((byte)Utils.FloatToIntClamp(R),
                                             (byte)Utils.FloatToIntClamp(G),
                                             (byte)Utils.FloatToIntClamp(B),
                                             (byte)Utils.FloatToIntClamp(A));
            }

            private H3DTextureMagFilter ConvertTexMagFilter(TextureFilter filterMode)
            {
                switch (filterMode)
                {
                    case TextureFilter.LINEAR: return H3DTextureMagFilter.Linear;
                    case TextureFilter.NEAREST: return H3DTextureMagFilter.Nearest;
                    default: return H3DTextureMagFilter.Linear;

                }
            }

            private H3DTextureMinFilter ConvertTexMinFilter(TextureFilter filterMode)
            {
                switch (filterMode)
                {
                    case TextureFilter.LINEAR: return H3DTextureMinFilter.Linear;
                    case TextureFilter.LINEAR_MIPMAP_LINEAR: return H3DTextureMinFilter.LinearMipmapLinear;
                    case TextureFilter.LINEAR_MIPMAP_NEAREST: return H3DTextureMinFilter.LinearMipmapNearest;
                    case TextureFilter.NEAREST: return H3DTextureMinFilter.Nearest;
                    case TextureFilter.NEAREST_MIPMAP_LINEAR: return H3DTextureMinFilter.NearestMipmapLinear;
                    case TextureFilter.NEAREST_MIPMAP_NEAREST: return H3DTextureMinFilter.NearestMipmapNearest;
                    default: return H3DTextureMinFilter.Linear;

                }
            }

            private PICATextureWrap ConvertWrapMode(CMBTextureWrapMode wrapMode)
            {
                switch (wrapMode)
                {
                    case CMBTextureWrapMode.CLAMP: return PICATextureWrap.ClampToBorder;
                    case CMBTextureWrapMode.CLAMP_TO_EDGE: return PICATextureWrap.ClampToEdge;
                    case CMBTextureWrapMode.MIRRORED_REPEAT: return PICATextureWrap.Mirror;
                    case CMBTextureWrapMode.REPEAT: return PICATextureWrap.Repeat;
                    default: return PICATextureWrap.Repeat;
                }
            }

            private string GetTextureName(int index)
            {
                if (index != -1 && index < CMBHeader.SectionData.TextureChunk?.Textures?.Count)
                    return CMBHeader.SectionData.TextureChunk.Textures[index].Name;
                else
                    return "";
            }

            public LightTable LUTTable;

            public struct LightTable
            {
                public bool reflectanceRSamplerIsAbs;
                public LUTInput reflectanceRSamplerInput;
                public uint reflectanceRSamplerScale;

                public bool reflectanceGSamplerIsAbs;
                public LUTInput reflectanceGSamplerInput;
                public uint reflectanceGSamplerScale;

                public bool reflectanceBSamplerIsAbs;
                public LUTInput reflectanceBSamplerInput;
                public uint reflectanceBSamplerScale;

                public bool reflectance0SamplerIsAbs;
                public LUTInput reflectance0SamplerInput;
                public uint reflectance0SamplerScale;

                public bool reflectance1SamplerIsAbs;
                public LUTInput reflectance1SamplerInput;
                public uint reflectance1SamplerScale;

                public bool fresnelSamplerIsAbs;
                public LUTInput fresnelSamplerInput;
                public uint fresnelSamplerScale;
            }

            public uint TotalUsedTextures { get; set; }
            public uint TotalUsedTextureCoords { get; set; }

            public void Read(FileReader reader, Header header, MaterialChunk materialChunkParent)
            {
                int materialSize = 0x15C;
                if (header.Version >= CMBVersion.MM3DS)
                    materialSize = 0x16C;

                CMBHeader = header;

                 TextureMaps = new TextureMap[3];
                TextureMaticies = new TextureMatrix[3];
                TextureCombiners = new List<TextureCombiner>();

                long pos = reader.Position;

                IsFragmentLightingEnabled = reader.ReadBoolean();
                IsVertexLightingEnabled = reader.ReadBoolean();
                IsHemiSphereLightingEnabled = reader.ReadBoolean();
                IsHemiSphereOcclusionEnabled = reader.ReadBoolean();

                CullMode = reader.ReadEnum<CullMode>(true); //byte
                IsPolygonOffsetEnabled = reader.ReadBoolean(); //byte
                PolygonOffset = reader.ReadUInt16();
                PolygonOffset = IsPolygonOffsetEnabled ? (ushort)((int)PolygonOffset / 0x10000) : (ushort)0;
                TotalUsedTextures = reader.ReadUInt32();
                TotalUsedTextureCoords = reader.ReadUInt32();

                //Texture bind data
                for (int j = 0; j < 3; j++)
                {
                    TextureMaps[j] = new TextureMap();
                    TextureMaps[j].TextureIndex = reader.ReadInt16();
                    reader.ReadInt16(); //padding
                    TextureMaps[j].MinFiler = (TextureFilter)reader.ReadUInt16();
                    TextureMaps[j].MagFiler = (TextureFilter)reader.ReadUInt16();
                    TextureMaps[j].WrapS = (CMBTextureWrapMode)reader.ReadUInt16();
                    TextureMaps[j].WrapT = (CMBTextureWrapMode)reader.ReadUInt16();
                    TextureMaps[j].MinLOD = reader.ReadSingle();
                    TextureMaps[j].LodBias = reader.ReadSingle();
                    TextureMaps[j].borderColorR = reader.ReadByte();
                    TextureMaps[j].borderColorG = reader.ReadByte();
                    TextureMaps[j].borderColorB = reader.ReadByte();
                    TextureMaps[j].borderColorA = reader.ReadByte();
                }

                LightSetIndex = reader.ReadUInt16();
                FogIndex = reader.ReadUInt16();

                for (int j = 0; j < 3; j++)
                {
                    TextureMaticies[j] = new TextureMatrix();
                    TextureMaticies[j].Scale = reader.ReadVec2SY();
                    TextureMaticies[j].Rotate = reader.ReadSingle();
                    TextureMaticies[j].Translate = reader.ReadVec2SY();
                    TextureMaticies[j].MatrixMode = reader.ReadByte();
                    TextureMaticies[j].ReferenceCamera = reader.ReadByte();
                    TextureMaticies[j].MappingMethod = reader.ReadByte();
                    TextureMaticies[j].CoordinateIndex = reader.ReadByte();
                }

                long dataPos = reader.Position;
                data = reader.ReadBytes(materialSize - (int)(dataPos - pos));
                reader.SeekBegin(dataPos);

                uint unkColor0 = reader.ReadUInt32();

                ConstantColors = new STColor8[6];
                ConstantColors[0] = STColor8.FromBytes(reader.ReadBytes(4));
                ConstantColors[1] = STColor8.FromBytes(reader.ReadBytes(4));
                ConstantColors[2] = STColor8.FromBytes(reader.ReadBytes(4));
                ConstantColors[3] = STColor8.FromBytes(reader.ReadBytes(4));
                ConstantColors[4] = STColor8.FromBytes(reader.ReadBytes(4));
                ConstantColors[5] = STColor8.FromBytes(reader.ReadBytes(4));

                BufferColorR = reader.ReadSingle();
                BufferColorG = reader.ReadSingle();
                BufferColorB = reader.ReadSingle();
                BufferColorA = reader.ReadSingle();

                BumpMapIndex = reader.ReadInt16();
                BumpMapMode = reader.ReadUInt16();
                IsBumpRenormalize = reader.ReadInt16();
                reader.ReadInt16(); //padding
                LayerConfig = (LayerConfig)reader.ReadUInt16();
                reader.ReadInt16(); //padding
                FresnelSelector = (FresnelSelector)reader.ReadUInt16();
                IsClampHighLight = reader.ReadBoolean();
                IsDistribution0Enabled = reader.ReadBoolean();
                IsDistribution1Enabled = reader.ReadBoolean();
                IsGeometricFactor0Enabled = reader.ReadBoolean();
                IsGeometricFactor1Enabled = reader.ReadBoolean();
                IsReflectionEnabled = reader.ReadBoolean();

                // Fragment lighting table.
                LUTTable.reflectanceRSamplerIsAbs = reader.ReadBoolean();
                LUTTable.reflectanceRSamplerInput = (LUTInput)reader.ReadUInt16();
                LUTTable.reflectanceRSamplerScale = reader.ReadUInt32();

                LUTTable.reflectanceGSamplerIsAbs = reader.ReadBoolean();
                LUTTable.reflectanceGSamplerInput = (LUTInput)reader.ReadUInt16();
                LUTTable.reflectanceGSamplerScale = reader.ReadUInt32();

                LUTTable.reflectanceBSamplerIsAbs = reader.ReadBoolean();
                LUTTable.reflectanceBSamplerInput = (LUTInput)reader.ReadUInt16();
                LUTTable.reflectanceBSamplerScale = reader.ReadUInt32();

                LUTTable.reflectance0SamplerIsAbs = reader.ReadBoolean();
                LUTTable.reflectance0SamplerInput = (LUTInput)reader.ReadUInt16();
                LUTTable.reflectance0SamplerScale = reader.ReadUInt32();

                LUTTable.reflectance1SamplerIsAbs = reader.ReadBoolean();
                LUTTable.reflectance1SamplerInput = (LUTInput)reader.ReadUInt16();
                LUTTable.reflectance1SamplerScale = reader.ReadUInt32();

                LUTTable.fresnelSamplerIsAbs = reader.ReadBoolean();
                LUTTable.fresnelSamplerInput = (LUTInput)reader.ReadUInt16();
                LUTTable.fresnelSamplerScale = reader.ReadUInt32();

                reader.SeekBegin(pos + 0x120);
                uint textureCombinerTableCount = reader.ReadUInt32();
                int textureCombinerTableIdx = (int)pos + 0x124;
                for (int i = 0; i < textureCombinerTableCount; i++)
                {
                    reader.SeekBegin(textureCombinerTableIdx + 0x00);
                    ushort textureCombinerIndex = reader.ReadUInt16();

                    reader.SeekBegin(materialChunkParent.textureCombinerSettingsTableOffs + textureCombinerIndex * 0x28);
                    TextureCombiner combner = new TextureCombiner();
                    combner.combineRGB = reader.ReadEnum<CombineResultOpDMP>(false);
                    combner.combineAlpha = reader.ReadEnum<CombineResultOpDMP>(false);
                    combner.scaleRGB = reader.ReadEnum<CombineScaleDMP>(false);
                    combner.scaleAlpha = reader.ReadEnum<CombineScaleDMP>(false);
                    combner.bufferInputRGB = reader.ReadEnum<CombineBufferInputDMP>(false);
                    combner.bufferInputAlpha = reader.ReadEnum<CombineBufferInputDMP>(false);
                    combner.source0RGB = reader.ReadEnum<CombineSourceDMP>(false);
                    combner.source1RGB = reader.ReadEnum<CombineSourceDMP>(false);
                    combner.source2RGB = reader.ReadEnum<CombineSourceDMP>(false);
                    combner.op0RGB = reader.ReadEnum<CombineOpDMP>(false);
                    combner.op1RGB = reader.ReadEnum<CombineOpDMP>(false);
                    combner.op2RGB = reader.ReadEnum<CombineOpDMP>(false);
                    combner.source0Alpha = reader.ReadEnum<CombineSourceDMP>(false);
                    combner.source1Alpha = reader.ReadEnum<CombineSourceDMP>(false);
                    combner.source2Alpha = reader.ReadEnum<CombineSourceDMP>(false);
                    combner.op0Alpha = reader.ReadEnum<CombineOpDMP>(false);
                    combner.op1Alpha = reader.ReadEnum<CombineOpDMP>(false);
                    combner.op2Alpha = reader.ReadEnum<CombineOpDMP>(false);
                    combner.constantIndex = reader.ReadUInt32();
                    TextureCombiners.Add(combner);

                    textureCombinerTableIdx += 0x2;
                }

                reader.ReadUInt16(); //padding
                AlphaTestEnable = reader.ReadBoolean();
                AlphaTestReference = reader.ReadByte() / 0xFF;
                AlphaTestFunction = (AlphaFunction)reader.ReadUInt16();

                DepthTestEnable = reader.ReadBoolean();
                DepthWriteEnable = reader.ReadBoolean();
                DepthTestFunction = (DepthFunction)reader.ReadUInt16();

                if (!AlphaTestEnable)
                    AlphaTestFunction = AlphaFunction.Always;

                if (!DepthTestEnable)
                    DepthTestFunction = DepthFunction.Always;

                BlendEnaled = reader.ReadBoolean();

                //Unknown. 
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();

                BlendingFactorSrcAlpha = (BlendingFactor)reader.ReadUInt16();
                BlendingFactorDestAlpha = (BlendingFactor)reader.ReadUInt16();
                BlendingEquationAlpha = (BlendEquationMode)reader.ReadUInt16();

                BlendingFactorSrcRGB = (BlendingFactor)reader.ReadUInt16();
                BlendingFactorDestRGB = (BlendingFactor)reader.ReadUInt16();
                BlendingEquationRGB = (BlendEquationMode)reader.ReadUInt16();

                BlendColorR = reader.ReadSingle();
                BlendColorG = reader.ReadSingle();
                BlendColorB = reader.ReadSingle();
                BlendColorA = reader.ReadSingle();

                IsTransparent = BlendEnaled;

                if (header.Version > CMBVersion.OOT3DS)
                {
                    byte StencilEnabled = reader.ReadByte();
                    byte StencilReferenceValue = reader.ReadByte();
                    byte BufferMask = reader.ReadByte();
                    byte Buffer = reader.ReadByte();
                    ushort StencilFunc = reader.ReadUInt16();
                    ushort FailOP = reader.ReadUInt16();
                    ushort ZFailOP = reader.ReadUInt16();
                    ushort ZPassOP = reader.ReadUInt16();
                    float unk6 = reader.ReadSingle();
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.Write(IsFragmentLightingEnabled);
                writer.Write(IsVertexLightingEnabled);
                writer.Write(IsHemiSphereLightingEnabled);
                writer.Write(IsHemiSphereOcclusionEnabled);
                writer.Write(CullMode, true);
                writer.Write(IsPolygonOffsetEnabled);
                writer.Write(PolygonOffset);
                writer.Write(TotalUsedTextures);
                writer.Write(TotalUsedTextureCoords);

                for (int j = 0; j < 3; j++)
                {
                    writer.Write(TextureMaps[j].TextureIndex);
                    writer.Write((ushort)0);
                    writer.Write((ushort)TextureMaps[j].MinFiler);
                    writer.Write((ushort)TextureMaps[j].MagFiler);
                    writer.Write((ushort)TextureMaps[j].WrapS);
                    writer.Write((ushort)TextureMaps[j].WrapT);
                    writer.Write(TextureMaps[j].MinLOD);
                    writer.Write(TextureMaps[j].LodBias);
                    writer.Write(TextureMaps[j].borderColorR);
                    writer.Write(TextureMaps[j].borderColorG);
                    writer.Write(TextureMaps[j].borderColorB);
                    writer.Write(TextureMaps[j].borderColorA);
                }

                writer.Write(LightSetIndex);
                writer.Write(FogIndex);

                for (int j = 0; j < 3; j++)
                {
                    writer.Write(TextureMaticies[j].Scale);
                    writer.Write(TextureMaticies[j].Rotate);
                    writer.Write(TextureMaticies[j].Translate);
                    writer.Write(TextureMaticies[j].MatrixMode);
                    writer.Write(TextureMaticies[j].ReferenceCamera);
                    writer.Write(TextureMaticies[j].MappingMethod);
                    writer.Write(TextureMaticies[j].CoordinateIndex);
                }

                writer.Write(data);
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"----------------------------------------------------\n");
                sb.Append($"AlphaTest {AlphaTestEnable} {AlphaTestFunction} {AlphaTestReference}\n");
                sb.Append($"DepthTest {DepthTestEnable} {DepthTestFunction} DepthWrite {DepthTestFunction}\n");

                sb.Append($"BlendingFactorSrcRGB {BlendingFactorSrcRGB}\n");
                sb.Append($"BlendingFactorDestRGB {BlendingFactorDestRGB}\n");
                sb.Append($"BlendingFactorSrcAlpha {BlendingFactorSrcAlpha}\n");
                sb.Append($"BlendingFactorDestAlpha {BlendingFactorDestAlpha}\n");
                sb.Append($"BlendEnaled {BlendEnaled}\n");
                sb.Append($"----------------------------------------------------\n");

                sb.AppendLine();

                return sb.ToString();
            }
        }

        public class TextureCombiner
        {
            public CombineResultOpDMP combineRGB;
            public CombineResultOpDMP combineAlpha;
            public CombineScaleDMP scaleRGB;
            public CombineScaleDMP scaleAlpha;
            public CombineBufferInputDMP bufferInputRGB;
            public CombineBufferInputDMP bufferInputAlpha;
            public CombineSourceDMP source0RGB;
            public CombineSourceDMP source1RGB;
            public CombineSourceDMP source2RGB;
            public CombineOpDMP op0RGB;
            public CombineOpDMP op1RGB;
            public CombineOpDMP op2RGB;
            public CombineSourceDMP source0Alpha;
            public CombineSourceDMP source1Alpha;
            public CombineSourceDMP source2Alpha;

            public CombineOpDMP op0Alpha;
            public CombineOpDMP op1Alpha;
            public CombineOpDMP op2Alpha;
            public uint constantIndex;

        }

        public class TextureMatrix
        {
            public Syroot.Maths.Vector2F Scale { get; set; }
            public Syroot.Maths.Vector2F Translate { get; set; }
            public float Rotate { get; set; }

            public byte MatrixMode { get; set; }
            public byte ReferenceCamera { get; set; }
            public byte MappingMethod { get; set; }
            public byte CoordinateIndex { get; set; }
        }

        public class TextureMap
        {
            public short TextureIndex { get; set; }
            public TextureFilter MinFiler { get; set; }
            public TextureFilter MagFiler { get; set; }
            public CMBTextureWrapMode WrapS { get; set; }
            public CMBTextureWrapMode WrapT { get; set; }
            public float MinLOD { get; set; }
            public float LodBias { get; set; }
            public byte borderColorR { get; set; }
            public byte borderColorG { get; set; }
            public byte borderColorB { get; set; }
            public byte borderColorA { get; set; }
        }

        public class TextureChunk : IChunkCommon
        {
            private const string Magic = "tex ";

            public List<CTXB.Texture> Textures = new List<CTXB.Texture>();

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint TextureCount = reader.ReadUInt32();
                for (int i = 0; i < TextureCount; i++)
                    Textures.Add(new CTXB.Texture(reader));
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(Textures.Count);

                for (int i = 0; i < Textures.Count; i++)
                    Textures[i].Write(writer);

                //Write the total file size
                writer.WriteSectionSizeU32(pos + 4, pos, writer.Position);
            }
        }

        public static T ReadChunkSection<T>(FileReader reader, Header header, long startPos = 0)
             where T : IChunkCommon, new()
        {
            long pos = reader.Position;

            //Read offset and seek it
            uint offset = reader.ReadUInt32();
            reader.SeekBegin(startPos + offset);

            //Create chunk instance
            T chunk = new T();
            chunk.Read(reader, header);

            //Seek back and shift 4 from reading offset
            reader.SeekBegin(pos + 0x4);
            return chunk;
        }

        public interface IChunkCommon
        {
            void Read(FileReader reader, Header header);
            void Write(FileWriter writer, Header header);
        }
    }
}
