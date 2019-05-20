using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;
using System.IO;
using Syroot.BinaryData;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using Bfres.Structs;

namespace FirstPlugin
{
    public class PTCL : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Effect;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Cafe Particle" };
        public string[] Extension { get; set; } = new string[] { "*.ptcl", "*.sesetlist" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                if (reader.CheckSignature(4, "VFXB") ||
                    reader.CheckSignature(4, "SPBD") ||
                    reader.CheckSignature(4, "EFTF") ||
                    reader.CheckSignature(4, "EFTB")) 
                    return true;
                else
                    return false;
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

        public Header header;
        public PTCL_WiiU.Header headerU;
        public PTCL_3DS.Header header3DS;
        
        public byte[] data;

        bool IsWiiU = false;
        bool Is3DS = false;

        public void Load(Stream stream)
        {
            data = stream.ToArray();

            Text = FileName;
            CanSave = true;

            FileReader reader = new FileReader(new MemoryStream(data));

            reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            string Signature = reader.ReadString(4, Encoding.ASCII);

            byte VersionNum = reader.ReadByte();
            if (VersionNum != 0 && Signature == "SPBD")
                Is3DS = true;

            reader.Position = 0;
            if (Is3DS)
            {
                reader.ByteOrder = ByteOrder.LittleEndian;
                header3DS = new PTCL_3DS.Header();
                header3DS.Read(reader, this);
            }
            else if (Signature == "EFTF" || Signature == "SPBD")
            {
                IsWiiU = true;
                headerU = new PTCL_WiiU.Header();
                headerU.Read(reader, this);
            }
            else
            {
                header = new Header();
                header.Read(reader, this);
            }

            reader.Close();
            reader.Dispose();

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Save", null, Save, Keys.Control | Keys.S));
        }

        public void Unload()
        {

        }
        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();
            if (Is3DS)
                header3DS.Write(new FileWriter(mem), this);
            else if (IsWiiU)
                headerU.Write(new FileWriter(mem), this);
            else
                header.Write(new FileWriter(mem));

            return mem.ToArray();
        }
        private void Save(object sender, EventArgs args)
        {
            List<IFileFormat> formats = new List<IFileFormat>();
            formats.Add(this);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(formats);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
                STFileSaver.SaveFileFormat(this, sfd.FileName);
        }

        public class WiiU
        {

        }


        public static readonly uint NullOffset = 0xFFFFFFFF;

        public class Header
        {
            public string Signature;

            public ushort GraphicsAPIVersion;
            public ushort VFXVersion;
            public ushort ByteOrderMark;
            public byte Alignment;
            public byte TargetOffset;

            public ushort Flag;
            public ushort BlockOffset;

            public uint DataAlignment;

            //For saving
            public List<SectionBase> Sections = new List<SectionBase>();


            public void Read(FileReader reader, PTCL ptcl)
            {
                uint Position = (uint)reader.Position; //Offsets are relative to this

                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                Signature = reader.ReadString(4, Encoding.ASCII);

                if (Signature == "EFTB")
                {
                    reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                    reader.Seek(48, SeekOrigin.Begin);
                }
                else if (Signature == "VFXB")
                {
                    uint padding = reader.ReadUInt32();
                    GraphicsAPIVersion = reader.ReadUInt16();
                    VFXVersion = reader.ReadUInt16();
                    ByteOrderMark = reader.ReadUInt16();
                    Alignment = reader.ReadByte();
                    TargetOffset = reader.ReadByte();
                    uint HeaderSize = reader.ReadUInt32();
                    Flag = reader.ReadUInt16();
                    BlockOffset = reader.ReadUInt16();
                    uint padding2 = reader.ReadUInt32();
                    uint FileSize = reader.ReadUInt32();
                    reader.Seek(Position + BlockOffset, SeekOrigin.Begin);
                }
                else
                {
                    throw new Exception("Unknown ptcl format. Signature not valid " + Signature);
                }


                while (reader.Position < reader.BaseStream.Length)
                {
                    SectionBase sectionBase = new SectionBase();
                    sectionBase.Read(reader, this);
                    Sections.Add(sectionBase);
                    ptcl.Nodes.Add(sectionBase);

                    if (sectionBase.NextSectionOffset == NullOffset)
                        break;
                }

                MapTextureIDs(ptcl);

                reader.Dispose();
                reader.Close();
            }
            private void MapTextureIDs(PTCL ptcl)
            {
                List<TextureDescriptor> texDescp = new List<TextureDescriptor>();
                List<Emitter> emitters = new List<Emitter>();
                BNTX bntx = null;
                List<TEXR> botwTex = new List<TEXR>(); //Used for BOTW

                foreach (var node in TreeViewExtensions.Collect(ptcl.Nodes))
                {
                    if (node is TextureDescriptor)
                        texDescp.Add((TextureDescriptor)node);
                    if (node is SectionBase && ((SectionBase)node).BinaryData is Emitter)
                        emitters.Add((Emitter)((SectionBase)node).BinaryData);
                    if (node is BNTX)
                        bntx = (BNTX)node;
                    if (node is SectionBase && ((SectionBase)node).BinaryData is TEXR)
                        botwTex.Add((TEXR)((SectionBase)node).BinaryData);
                }

                int index = 0;
                if (botwTex.Count > 0)
                {
                    TreeNode textureFolder = new TreeNode("Textures");
                    ptcl.Nodes.Add(textureFolder);

                    List<TEXR> TextureList = new List<TEXR>();

                    foreach (var emitter in emitters)
                    {
                        foreach (TEXR tex in botwTex)
                        {
                            bool HasImage = TextureList.Any(item => item.data == tex.data);
                            if (!HasImage)
                            {
                                tex.Text = "Texture " + index++;
                                textureFolder.Nodes.Add(tex);
                            }
                            TextureList.Add(tex);

                            foreach (var sampler in emitter.Samplers)
                            {
                                if (sampler.TextureID == tex.TextureID)
                                {
                                    emitter.DrawableTex.Add(tex);
                                }
                            }
                        }
                    }
                    TextureList.Clear();
                }


                if (bntx == null)
                    return;

                foreach (var emitter in emitters)
                {
                    foreach (var tex in texDescp)
                    {
                        foreach (var sampler in emitter.Samplers)
                        {
                            if (sampler.TextureID == tex.TextureID)
                            {
                                if (bntx.Textures.ContainsKey(tex.TexName))
                                {
                                    emitter.DrawableTex.Add(bntx.Textures[tex.TexName]);
                                }
                            }
                        }
                    }
                }
            }

            private TreeNodeFile GetMagic(SectionBase section)
            {
                TreeNodeFile node = new TreeNodeFile();
                node.Text = section.Signature;

                foreach (var child in section.ChildSections)
                {
                    node.Nodes.Add(GetMagic(child));
                }

                return node;
            }
            public void Write(FileWriter writer)
            {
                writer.WriteSignature("VFXB");
                writer.Write(0x20202020);
                writer.Write(GraphicsAPIVersion);
                writer.Write(VFXVersion);
                writer.Write(ByteOrderMark);
                writer.Write(Alignment);
                writer.Write(TargetOffset);
                writer.Write(32);
                writer.Write(Flag);
                writer.Write(BlockOffset);
                writer.Write(0);
                long _ofsFileSize = writer.Position;
                writer.Write(0);
                writer.Seek(BlockOffset, SeekOrigin.Begin);

                foreach (var section in Sections)
                {
                    writer.Align(8);
                    section.Write(writer, this);
                }

                using (writer.TemporarySeek(_ofsFileSize, SeekOrigin.Begin))
                {
                    writer.Write(writer.BaseStream.Length);
                }

                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }

        static bool ChildHasBinary = false;

        //  public static readonly uint NullOffset = 0xFFFFFFFF;
        public class SectionBase : TreeNodeCustom
        {
            public long Position;  //Offsets are relative to this
            public string Signature;
            public uint SectionSize;
            public uint SubSectionSize;
            public uint SubSectionOffset;
            public uint NextSectionOffset;
            public uint Unkown; //0xFFFFFFFF
            public uint BinaryDataOffset; //32
            public uint Unkown3; //0
            public uint SubSectionCount;

            public object BinaryData;
            public byte[] BinaryDataBytes;

            public List<SectionBase> ChildSections = new List<SectionBase>();
            public byte[] data;

            public override void OnClick(TreeView treeview)
            {
                if (BinaryData is Emitter || Signature == "EMTR")
                {
                    EmitterEditor editor = (EmitterEditor)LibraryGUI.Instance.GetActiveContent(typeof(EmitterEditor));
                    if (editor == null)
                    {
                        editor = new EmitterEditor();
                        LibraryGUI.Instance.LoadEditor(editor);
                    }
                    editor.Text = Text;
                    editor.Dock = DockStyle.Fill;
                    editor.LoadEmitter((Emitter)BinaryData);
                }
            }

            public void Read(FileReader reader, Header ptclHeader, string MagicCheck = "")
            {
                Position = (uint)reader.Position;

                if (MagicCheck != "")
                    Signature = reader.ReadSignature(4, MagicCheck);
                else
                    Signature = reader.ReadString(4, Encoding.ASCII);

                SectionSize = reader.ReadUInt32();
                SubSectionOffset = reader.ReadUInt32();
                NextSectionOffset = reader.ReadUInt32();
                Unkown = reader.ReadUInt32();
                BinaryDataOffset = reader.ReadUInt32();
                Unkown3 = reader.ReadUInt32();

                if (ptclHeader.Signature == "EFTB")
                {
                    SubSectionCount = reader.ReadUInt16();
                    ushort unk = reader.ReadUInt16();
                }
                else
                {
                    SubSectionCount = reader.ReadUInt32();
                }

                Text = Signature;

                ReadSectionData(this, ptclHeader, reader);

                if (SubSectionOffset != NullOffset)
                {
                    uint tempCount = 0;

                    //Some sections will point to sub sections but have no count? (GRSN to GRSC)
                    //This will work decently for now
                    if (SubSectionCount == 0)
                    {
                        tempCount = 1;
                    }

                    reader.Seek(Position + SubSectionOffset, SeekOrigin.Begin);
                    for (int i = 0; i < SubSectionCount + tempCount; i++)
                    {
                        var ChildSection = new SectionBase();
                        Nodes.Add(ChildSection);

                        ChildSection.Read(reader, ptclHeader);
                        ChildSections.Add(ChildSection);

                        if (ChildSection.NextSectionOffset == NullOffset)
                            break;
                    }
                }

                reader.Seek(Position, SeekOrigin.Begin);

                if (ChildSections.Count != 0)
                    data = reader.ReadBytes((int)SubSectionOffset);
                else if (NextSectionOffset != NullOffset)
                    data = reader.ReadBytes((int)NextSectionOffset);
                else
                    data = reader.ReadBytes((int)SectionSize);

                if (NextSectionOffset != NullOffset)
                    reader.Seek(Position + NextSectionOffset, SeekOrigin.Begin);
            }

            private void ReadSectionData(SectionBase section, Header ptclHeader, FileReader reader)
            {
                if (section.BinaryDataOffset != NullOffset)
                {
                    using (reader.TemporarySeek(section.BinaryDataOffset + section.Position, SeekOrigin.Begin))
                    {
                        BinaryDataBytes = reader.ReadBytes((int)section.SectionSize);
                    }
                }

                switch (section.Signature)
                {
                    case "TEXR":
                        section.Text = "Texture Info";
                        BinaryData = new TEXR();

                        if (SubSectionCount > 0)
                        {
                            //Set the data block first!
                            reader.Seek(SubSectionOffset + section.Position, SeekOrigin.Begin);
                            var dataBlockSection = new SectionBase();
                            dataBlockSection.Read(reader, ptclHeader, "GX2B");

                            if (dataBlockSection.BinaryDataOffset != NullOffset)
                            {
                                reader.Seek(dataBlockSection.BinaryDataOffset + dataBlockSection.Position, SeekOrigin.Begin);
                                ((TEXR)BinaryData).data = reader.ReadBytes((int)dataBlockSection.SectionSize);
                            }

                        }

                        reader.Seek(BinaryDataOffset + section.Position, SeekOrigin.Begin);
                        ((TEXR)BinaryData).Read(reader, ptclHeader);

                        break;
                    case "SHDB":
                        reader.Seek(BinaryDataOffset + section.Position, SeekOrigin.Begin);
                        section.Text = "GTX Shader";
                        reader.ReadBytes((int)section.SectionSize);
                        break;
                    case "EMTR":
                        reader.Seek(BinaryDataOffset + 16 + section.Position, SeekOrigin.Begin);
                        Text = reader.ReadString(BinaryStringFormat.ZeroTerminated);

                        reader.Seek(BinaryDataOffset + 16 + 64 + section.Position, SeekOrigin.Begin);
                        BinaryData = new Emitter();
                        ((Emitter)BinaryData).Read(reader, ptclHeader);
                        break;
                    case "ESTA":
                        section.Text = "Emitter Sets";
                        break;
                    case "ESET":
                        byte[] Padding = reader.ReadBytes(16);
                        section.Text = reader.ReadString(BinaryStringFormat.ZeroTerminated);
                        break;
                    case "GRTF":
                        if (section.BinaryDataOffset != NullOffset)
                        {
                            reader.Seek(section.BinaryDataOffset + section.Position, SeekOrigin.Begin);
                            BinaryData = new BNTX();
                            ((BNTX)BinaryData).FileName = "textures.bntx";
                            ((BNTX)BinaryData).Load(new MemoryStream(reader.ReadBytes((int)section.SectionSize)));
                            ((BNTX)BinaryData).IFileInfo.InArchive = true;
                            Nodes.Add(((BNTX)BinaryData));
                        }
                        break;
                    case "PRMA":
                        break;
                    case "ESFT":
                        reader.Seek(28, SeekOrigin.Current);
                        int StringSize = reader.ReadInt32();
                        section.Text = reader.ReadString(StringSize, Encoding.ASCII);
                        break;
                    case "GRSN":
                        section.Text = "Shaders";
                        
                        if (section.BinaryDataOffset != NullOffset)
                        {
                            reader.Seek(section.BinaryDataOffset + section.Position, SeekOrigin.Begin);
                            BinaryData = reader.ReadBytes((int)section.SectionSize);
                        }
                        break;
                    case "GRSC":
                        section.Text = "Shaders 2";
                        if (section.BinaryDataOffset != NullOffset)
                        {
                            reader.Seek(section.BinaryDataOffset + section.Position, SeekOrigin.Begin);
                            BinaryData = reader.ReadBytes((int)section.SectionSize);
                        }
                        break;  
                    case "G3PR":
                        if (section.BinaryDataOffset != NullOffset)
                        {
                            reader.Seek(section.BinaryDataOffset + section.Position, SeekOrigin.Begin);
                            BinaryData = new BFRES();
                            ((BFRES)BinaryData).FileName = "model.bfres";
                            ((BFRES)BinaryData).Load(new MemoryStream(reader.ReadBytes((int)section.SectionSize)));
                            ((BFRES)BinaryData).IFileInfo = new IFileInfo();
                            ((BFRES)BinaryData).IFileInfo.InArchive = true;
                            Nodes.Add(((BFRES)BinaryData));
                        }
                        break;
                    case "GTNT":
                        if (section.BinaryDataOffset != NullOffset)
                        {
                            foreach (var node in Parent.Nodes)
                            {
                                if (node is BNTX)
                                {
                                    BNTX bntx = (BNTX)node;

                                    reader.Seek(section.BinaryDataOffset + section.Position, SeekOrigin.Begin);
                                    for (int i = 0; i < bntx.Textures.Count; i++)
                                    {
                                        var texDescriptor = new TextureDescriptor();
                                        Nodes.Add(texDescriptor);
                                        texDescriptor.Read(reader, bntx);
                                    }
                                }
                            }
                        }
                        break;
                }
            }

            public void Write(FileWriter writer, PTCL.Header header)
            {
                switch (Signature)
                {
                    case "GRSN":
                        SaveHeader(writer, header, BinaryDataBytes, 4096);
                        break;
                    case "GRSC":
                        SaveHeader(writer, header, BinaryDataBytes, 4096);
                        break;
                    case "G3PR":
                         SaveHeader(writer, header, ((BFRES)BinaryData).Save(), 4096);
                        //  SaveHeader(writer, header, BinaryDataBytes, 4096);
                        break;
                    case "GRTF":
                        SaveHeader(writer, header, ((BNTX)BinaryData).Save(), 4096);
                        // SaveHeader(writer, header, BinaryDataBytes, 4096);
                        break;
                    case "EMTR":
                        //Write all the data first
                        long _emitterPos = writer.Position;
                        writer.Write(data);
                        foreach (var child in ChildSections)
                        {
                            child.Write(writer, header);
                        }

                        using (writer.TemporarySeek(_emitterPos + BinaryDataOffset +16 + 64, SeekOrigin.Begin))
                        {
                            ((Emitter)BinaryData).Write(writer, header);
                        }
                        break;
                    default:
                        writer.Write(data);
                        foreach (var child in ChildSections)
                        {
                            child.Write(writer, header);
                        }
                        break;
                }


                /*      writer.Write(Signature);
                      writer.Write(SectionSize);
                      writer.Write(SubSectionOffset);
                      writer.Write(NextSectionOffset);
                      writer.Write(Unkown);
                      writer.Write(Unkown3);
                      writer.Write(SubSectionCount);*/
            }

            public class BinarySavedEntry
            {
                public long Position;
                public long _ofsData;
                public byte[] Data;
            }

            public List<BinarySavedEntry> BinariesSaved = new List<BinarySavedEntry>();

            private void SaveHeader(FileWriter writer,Header header, byte[] BinaryFile, int BinaryAlignment)
            {
                if (BinaryFile != null && BinaryFile.Length > 0)
                    SectionSize = (uint)BinaryFile.Length;

                long BasePosition = writer.Position;

                writer.WriteSignature(Signature);
                writer.Write(SectionSize);
                long _ofsChildPos = writer.Position;
                writer.Write(NullOffset); //Childern Offset for later
                long _ofsNextPos = writer.Position;
                writer.Write(NullOffset); //Next Offet for later
                writer.Write(Unkown);
                long _ofsBinaryPos = writer.Position;
                writer.Write(NullOffset); //Binary Offset for later
                writer.Write(Unkown3);
                writer.Write(SubSectionCount);

                if (ChildSections.Count > 0)
                    writer.WriteUint32Offset(_ofsChildPos, BasePosition);

                foreach (var child in ChildSections)
                {
                    if (child.BinaryData != null)
                    {
                        //Skip binaries for childern first
                        ChildHasBinary = true;
                        BinariesSaved.Add(new BinarySavedEntry()
                        {
                            Position = writer.Position,
                            _ofsData = writer.Position + 20,
                            Data = child.BinaryDataBytes,
                        });
                        child.Write(writer, header); //Save childern
                        ChildHasBinary = false; //Now all children headers have been written
                    }
                    else
                        child.Write(writer, header); //Save childern
                }

                if (!ChildHasBinary)
                {
                    if (BinaryFile != null && BinaryFile.Length > 0)
                    {
                        writer.Align(BinaryAlignment); //Align the file
                        Console.WriteLine($"{Signature} DATA BLOCK " + writer.Position + " " + BinaryFile.Length);

                        writer.WriteUint32Offset(_ofsBinaryPos, BasePosition); //Save binary offset
                        writer.Write(BinaryFile); //Save binary data
                    }

                    foreach (var binary in BinariesSaved)
                    {
                        writer.Align(4096); //Align the file
                        Console.WriteLine($"{Signature} DATA BLOCK " + writer.Position + " " + BinaryFile.Length);

                        writer.WriteUint32Offset(binary._ofsData, binary.Position); //Save binary offset
                        writer.Write(binary.Data); //Save binary data
                    }
                    BinariesSaved.Clear();
                }

                if (NextSectionOffset != NullOffset)
                    writer.WriteUint32Offset(_ofsNextPos, BasePosition);
            }
        }

        public class TEXR : STGenericTexture
        {
            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[]
                    {
                        TEX_FORMAT.BC1_UNORM,
                        TEX_FORMAT.BC1_UNORM_SRGB,
                        TEX_FORMAT.BC2_UNORM,
                        TEX_FORMAT.BC2_UNORM_SRGB,
                        TEX_FORMAT.BC3_UNORM,
                        TEX_FORMAT.BC3_UNORM_SRGB,
                        TEX_FORMAT.BC4_UNORM,
                        TEX_FORMAT.BC4_SNORM,
                        TEX_FORMAT.BC5_UNORM,
                        TEX_FORMAT.BC5_SNORM,
                        TEX_FORMAT.B5G6R5_UNORM,
                        TEX_FORMAT.B8G8R8A8_UNORM_SRGB,
                        TEX_FORMAT.B8G8R8A8_UNORM,
                        TEX_FORMAT.B5G5R5A1_UNORM,
                        TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                        TEX_FORMAT.R8G8B8A8_UNORM,
                        TEX_FORMAT.R8_UNORM,
                        TEX_FORMAT.R8G8_UNORM,
                    };
                }
            }

            public TEXR()
            {
                ImageKey = "Texture";
                SelectedImageKey = "Texture";
            }

            public override void OnClick(TreeView treeView)
            {
                UpdateEditor();
            }

            public void UpdateEditor()
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.Instance.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.Instance.LoadEditor(editor);
                }

                editor.Text = Text;
                editor.LoadProperties(GenericProperties);
                editor.LoadImage(this);
            }

            public override bool CanEdit { get; set; } = false;

            public enum SurfaceFormat : byte
            {
                INVALID = 0x0,
                TCS_R8_G8_B8_A8 = 2,
                T_BC1_UNORM = 3,
                T_BC1_SRGB = 4,
                T_BC2_UNORM = 5,
                T_BC2_SRGB = 6,
                T_BC3_UNORM = 7,
                T_BC3_SRGB = 8,
                T_BC4_UNORM = 9,
                T_BC4_SNORM = 10,
                T_BC5_UNORM = 11,
                T_BC5_SNORM = 12,
                TC_R8_UNORM = 13,
                TC_R8_G8_UNORM = 14,
                TCS_R8_G8_B8_A8_UNORM = 15,
                TCS_R5_G6_B5_UNORM = 25,
            };

            public uint TileMode;
            public uint Swizzle = 0;
            public byte WrapMode = 11;
            public byte Depth = 1;
            public uint MipCount;
            public uint CompSel;
            public uint ImageSize;
            public SurfaceFormat SurfFormat;
            public byte[] data;
            public uint TextureID;

            public void Replace(string FileName)
            {
             
            }
            public static GTXImporterSettings SetImporterSettings(string name)
            {
                var importer = new GTXImporterSettings();
                string ext = System.IO.Path.GetExtension(name);
                ext = ext.ToLower();

                switch (ext)
                {
                    case ".dds":
                        importer.LoadDDS(name);
                        break;
                    default:
                        importer.LoadBitMap(name);
                        break;
                }

                return importer;
            }

            public void Read(FileReader reader, Header header)
            {
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                uint unk = reader.ReadUInt32();
                CompSel = reader.ReadUInt32();
                MipCount = reader.ReadUInt32();
                uint unk2 = reader.ReadUInt32();
                TileMode = reader.ReadUInt32();
                uint unk3 = reader.ReadUInt32();
                ImageSize = reader.ReadUInt32();
                uint unk4 = reader.ReadUInt32();
                TextureID = reader.ReadUInt32();
                SurfFormat = reader.ReadEnum<SurfaceFormat>(false);
                byte unk5 = reader.ReadByte();
                short unk6 = reader.ReadInt16();
                uint unk7 = reader.ReadUInt32();
              
            }

            public override void SetImageData(Bitmap bitmap, int ArrayLevel)
            {
                throw new NotImplementedException("Cannot set image data! Operation not implemented!");
            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                uint GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC5_UNORM;

                switch (SurfFormat)
                {
                    case SurfaceFormat.T_BC1_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC1_UNORM;
                        Format = TEX_FORMAT.BC1_UNORM;
                        break;
                    case SurfaceFormat.T_BC1_SRGB:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC1_SRGB;
                        Format = TEX_FORMAT.BC1_UNORM_SRGB;
                        break;
                    case SurfaceFormat.T_BC2_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC2_UNORM;
                        Format = TEX_FORMAT.BC2_UNORM;
                        break;
                    case SurfaceFormat.T_BC2_SRGB:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC2_SRGB;
                        Format = TEX_FORMAT.BC2_UNORM_SRGB;
                        break;
                    case SurfaceFormat.T_BC3_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC3_UNORM;
                        Format = TEX_FORMAT.BC3_UNORM;
                        break;
                    case SurfaceFormat.T_BC3_SRGB:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC3_SRGB;
                        Format = TEX_FORMAT.BC3_UNORM_SRGB;
                        break;
                    case SurfaceFormat.T_BC4_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC4_UNORM;
                        Format = TEX_FORMAT.BC4_UNORM;
                        break;
                    case SurfaceFormat.T_BC4_SNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC4_SNORM;
                        Format = TEX_FORMAT.BC4_SNORM;
                        break;
                    case SurfaceFormat.T_BC5_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC5_UNORM;
                        Format = TEX_FORMAT.BC5_UNORM;
                        break;
                    case SurfaceFormat.T_BC5_SNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC5_SNORM;
                        Format = TEX_FORMAT.BC5_SNORM;
                        break;
                    case SurfaceFormat.TC_R8_G8_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TC_R8_G8_UNORM;
                        Format = TEX_FORMAT.R8G8_UNORM;
                        break;
                    case SurfaceFormat.TCS_R8_G8_B8_A8_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNORM;
                        Format = TEX_FORMAT.R8G8B8A8_UNORM;
                        break;
                    case SurfaceFormat.TCS_R8_G8_B8_A8:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNORM;
                        Format = TEX_FORMAT.R8G8B8A8_UNORM;
                        break;
                    case SurfaceFormat.TC_R8_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TC_R8_UNORM;
                        Format = TEX_FORMAT.R8_UNORM;
                        break;
                    case SurfaceFormat.TCS_R5_G6_B5_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TCS_R5_G6_B5_UNORM;
                        Format = TEX_FORMAT.B5G6R5_UNORM;
                        break;
                    default:
                        throw new Exception("Format unsupported! " + SurfFormat);
                }


                int swizzle = (int)Swizzle;
                int pitch = (int)0;
                uint bpp = GX2.surfaceGetBitsPerPixel(GX2Format) >> 3;

                GX2.GX2Surface surf = new GX2.GX2Surface();
                surf.bpp = bpp;
                surf.height = Height;
                surf.width = Width;
                surf.aa = (uint)0;
                surf.alignment = 0;
                surf.depth = Depth;
                surf.dim = 0x1;
                surf.format = GX2Format;
                surf.use = 0x1;
                surf.pitch = 0;
                surf.data = data;
                surf.numMips = 1;
                surf.mipOffset = new uint[0];
                surf.mipData = null;
                surf.tileMode = TileMode;
                surf.swizzle = Swizzle;
                surf.imageSize = ImageSize;

                var surfaces = GX2.Decode(surf);
                return surfaces[ArrayLevel][MipLevel];
            }

            public void Write(FileWriter writer)
            {

            }
        }

        public class Emitter : TreeNodeCustom
        {
            public Color[] Color0s = new Color[8];
            public Color[] Color1s = new Color[8];
            public List<STGenericTexture> DrawableTex = new List<STGenericTexture>();
            public List<SamplerInfo> Samplers = new List<SamplerInfo>();
            public ColorData[] Color0Array = new ColorData[8];
            public ColorData[] Color1Array = new ColorData[8];

            public class ColorData
            {
                public float R;
                public float G;
                public float B;
                public float A;
            }

            public void Read(FileReader reader, Header ptclHeader)
            {
                uint Position = (uint)reader.Position; 

                Color0Array = new ColorData[8];
                Color1Array = new ColorData[8];

                reader.Seek(Position + 880, SeekOrigin.Begin);
                for (int i = 0; i < 8; i++)
                {
                    Color0Array[i] = new ColorData();
                    Color0Array[i].R = reader.ReadSingle();
                    Color0Array[i].G = reader.ReadSingle();
                    Color0Array[i].B = reader.ReadSingle();
                    float time       = reader.ReadSingle();

                    int red = Utils.FloatToIntClamp(Color0Array[i].R);
                    int green = Utils.FloatToIntClamp(Color0Array[i].G);
                    int blue = Utils.FloatToIntClamp(Color0Array[i].B);

                    Color0s[i] = Color.FromArgb(255, red, green, blue);
                }
                for (int i = 0; i < 8; i++)
                {
                    Color0Array[i].A = reader.ReadSingle();
                    float padding = reader.ReadSingle();
                    float padding2 = reader.ReadSingle();
                    float time    = reader.ReadSingle();

                    int alpha = Utils.FloatToIntClamp(Color0Array[i].A);
                }
                for (int i = 0; i < 8; i++)
                {
                    Color1Array[i] = new ColorData();
                    Color1Array[i].R = reader.ReadSingle();
                    Color1Array[i].G = reader.ReadSingle();
                    Color1Array[i].B = reader.ReadSingle();
                    float time = reader.ReadSingle();

                    int red = Utils.FloatToIntClamp(Color1Array[i].R);
                    int green = Utils.FloatToIntClamp(Color1Array[i].G);
                    int blue = Utils.FloatToIntClamp(Color1Array[i].B);

                    Color1s[i] = Color.FromArgb(255, red, green, blue);
                }
                for (int i = 0; i < 8; i++)
                {
                    Color1Array[i].A = reader.ReadSingle();
                    float padding = reader.ReadSingle();
                    float padding2 = reader.ReadSingle();
                    float time = reader.ReadSingle();

                    int alpha = Utils.FloatToIntClamp(Color1Array[i].A);
                }

                if (ptclHeader.VFXVersion >= 22)
                    reader.Seek(Position + 2464, SeekOrigin.Begin);
                else
                    reader.Seek(Position + 2472, SeekOrigin.Begin);

                for (int i = 0; i < 3; i++)
                {
                    SamplerInfo samplerInfo = new SamplerInfo();
                    samplerInfo.Read(reader);
                    Samplers.Add(samplerInfo);
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                uint Position = (uint)writer.Position;

                writer.Seek(Position + 880, SeekOrigin.Begin);
                for (int i = 0; i < 8; i++)
                {
                    writer.Write(Color0Array[i].R);
                    writer.Write(Color0Array[i].G);
                    writer.Write(Color0Array[i].B);
                    writer.Seek(4, SeekOrigin.Current);
                }
                for (int i = 0; i < 8; i++)
                {
                    writer.Write(Color0Array[i].A);
                    writer.Seek(12, SeekOrigin.Current);
                }
                for (int i = 0; i < 8; i++)
                {
                    writer.Write(Color1Array[i].R);
                    writer.Write(Color1Array[i].G);
                    writer.Write(Color1Array[i].B);
                    writer.Seek(4, SeekOrigin.Current);
                }
                for (int i = 0; i < 8; i++)
                {
                    writer.Write(Color1Array[i].A);
                    writer.Seek(12, SeekOrigin.Current);
                }
            }

            public class SamplerInfo
            {
                public ulong TextureID;

                public void Read(FileReader reader)
                {
                    TextureID = reader.ReadUInt64();
                    byte wrapModeU = reader.ReadByte();
                    byte wrapMode = reader.ReadByte();
                    reader.Seek(22, SeekOrigin.Current);
                }
            }
            private Color ReadColorRgba(FileReader reader, int amount = 1)
            {
                Color[] colors = new Color[amount];
                for (int i = 0; i < 8; i++)
                {

                }
                    float R = reader.ReadSingle();
                float G = reader.ReadSingle();
                float B = reader.ReadSingle();
                float A = reader.ReadSingle();

                int red = Utils.FloatToIntClamp(R);
                int green = Utils.FloatToIntClamp(G);
                int blue = Utils.FloatToIntClamp(B);
                int alpha = Utils.FloatToIntClamp(B);

                return Color.FromArgb(255, red, green, blue);
            }
            private Color ReadColorAnim(FileReader reader, int amount = 1)
            {
                float R = reader.ReadSingle();
                float G = reader.ReadSingle();
                float B = reader.ReadSingle();
                float unk = reader.ReadSingle();

                int red = Utils.FloatToIntClamp(R);
                int green = Utils.FloatToIntClamp(G);
                int blue = Utils.FloatToIntClamp(B);

                return Color.FromArgb(255, red, green, blue);
            }
            private Color ReadColorA(FileReader reader, int amount = 1)
            {
                float A = reader.ReadSingle();
                float unk = reader.ReadSingle();
                float unk2 = reader.ReadSingle();
                float unk3 = reader.ReadSingle();

                int alpha = Utils.FloatToIntClamp(A);

                return Color.FromArgb(alpha, 0, 0, 0);
            }
        }

        public class TextureDescriptor : TreeNodeCustom
        {
            public ulong TextureID;
            public string TexName;

            public void Read(FileReader reader, BNTX bntx)
            {
                uint Position = (uint)reader.Position; //Offsets are relative to this

                TextureID                = reader.ReadUInt64();
                uint NextDesriptorOffset = reader.ReadUInt32();
                uint StringLength        = reader.ReadUInt32();
                TexName = reader.ReadString(BinaryStringFormat.ZeroTerminated);

                Text = TexName + " " + TextureID.ToString("x");

                if (NextDesriptorOffset != 0)
                    reader.Seek(NextDesriptorOffset + Position, SeekOrigin.Begin);
            }
        }
    }
}
