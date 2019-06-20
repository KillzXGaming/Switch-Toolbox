using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class GFBMDL : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Graphic Model" };
        public string[] Extension { get; set; } = new string[] { "*.gfbmdl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                bool IsMatch = reader.ReadUInt32() == 0x20000000;
                reader.Position = 0;

                return IsMatch;
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
                var editor = LibraryGUI.Instance.GetObjectEditor();
                return editor.GetViewport();
            }
            set
            {
                var editor = LibraryGUI.Instance.GetObjectEditor();
                editor.LoadViewport(value);
            }
        }

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
                LibraryGUI.Instance.LoadEditor(viewport);

                viewport.Text = Text;
            }
        }

        public Header header;
        public DrawableContainer DrawableContainer = new DrawableContainer();

        public void Load(System.IO.Stream stream)
        {
            DrawableContainer.Name = FileName;

            header = new Header();
            header.Read(new FileReader(stream));
            DrawableContainer.Drawables.Add(header.Skeleton);

        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            header.Write(new FileWriter(mem));
            return mem.ToArray();
        }

        public class Header
        {
            public STSkeleton Skeleton { get; set; }

            public uint Version { get; set; }
            public float[] Boundings { get; set; }
            public List<string> TextureMaps = new List<string>();
            public List<string> Materials = new List<string>();

            public void Read(FileReader reader)
            {
                Skeleton = new STSkeleton();

                reader.SetByteOrder(false);

                Version = reader.ReadUInt32();
                Boundings = reader.ReadSingles(9);
                long TextureOffset = reader.ReadOffset(true, typeof(uint));
                long MaterialOffset = reader.ReadOffset(true, typeof(uint));
                long UnknownOffset = reader.ReadOffset(true, typeof(uint));
                long Unknown2Offset = reader.ReadOffset(true, typeof(uint));
                long ShaderOffset = reader.ReadOffset(true, typeof(uint));
                long VisGroupOffset = reader.ReadOffset(true, typeof(uint));
                long VertexDataOffset = reader.ReadOffset(true, typeof(uint));
                long BoneDataOffset = reader.ReadOffset(true, typeof(uint));

                if (TextureOffset != 0)
                {
                    reader.Seek(TextureOffset, SeekOrigin.Begin);
                    uint Count = reader.ReadUInt32();
                    TextureMaps = reader.ReadNameOffsets(Count, true, typeof(uint), true);
                }

                if (MaterialOffset != 0)
                {
                    reader.Seek(MaterialOffset, SeekOrigin.Begin);
                    uint Count = reader.ReadUInt32();
                    Materials = reader.ReadNameOffsets(Count, true, typeof(uint));
                }

                if (BoneDataOffset != 0)
                {
                    reader.Seek(BoneDataOffset, SeekOrigin.Begin);
                    uint Count = reader.ReadUInt32();
                    Console.WriteLine($"BoneCount {Count}");

                    for (int i = 0; i < Count; i++)
                    {
                        var bone = new Bone(Skeleton);
                        bone.Read(reader);
                        Skeleton.bones.Add(bone);
                    }
                }
            }

            public void Write(FileWriter writer)
            {
                writer.Write(Version);
            }
        }

        public class Bone : STBone
        {
            internal BoneInfo BoneInfo { get; set; }

            public Bone(STSkeleton skeleton) : base(skeleton) { }

            public void Read(FileReader reader)
            {
                long DataPosition = reader.Position;
                var BoneDataOffset = reader.ReadOffset(true, typeof(uint));

                reader.SeekBegin(BoneDataOffset);
                long InfoPosition = reader.Position;

                uint BoneInfoOffset = reader.ReadUInt32();

                //Read the info section for position data
                reader.SeekBegin(InfoPosition - BoneInfoOffset);

                BoneInfo = new BoneInfo();
                BoneInfo.Read(reader);

                if (BoneInfo.NamePosition != 0)
                {
                    reader.SeekBegin(DataPosition + BoneInfo.NamePosition);
                    uint NameLength = reader.ReadUInt32();
                    Text = reader.ReadString((int)NameLength);
                }

                if (BoneInfo.RotationPosition != 0)
                {
                    reader.SeekBegin(DataPosition + BoneInfo.RotationPosition);
                    float RotationX = reader.ReadSingle();
                    float RotationY = reader.ReadSingle();
                    float RotationZ = reader.ReadSingle();
                    rotation = new float[] { RotationX,RotationY, RotationZ };
                }

                if (BoneInfo.TranslationPosition != 0)
                {
                    reader.SeekBegin(DataPosition + BoneInfo.RotationPosition);
                    float TranslateX = reader.ReadSingle();
                    float TranslateY = reader.ReadSingle();
                    float TranslateZ = reader.ReadSingle();
                    position = new float[] { TranslateX, TranslateY, TranslateZ };
                }

                if (BoneInfo.ScalePosition != 0)
                {
                    reader.SeekBegin(DataPosition + BoneInfo.ScalePosition);
                    float ScaleX = reader.ReadSingle();
                    float ScaleY = reader.ReadSingle();
                    float ScaleZ = reader.ReadSingle();
                    scale = new float[] { ScaleX, ScaleY, ScaleZ };
                }

                if (BoneInfo.ParentPosition != 0)
                {
                    reader.SeekBegin(DataPosition + BoneInfo.ParentPosition);
                    parentIndex = reader.ReadInt32();
                }

                Console.WriteLine("BONE " + Text);

                //Seek back to next bone in array
                reader.SeekBegin(DataPosition + sizeof(uint));
            }
        }

        //A section that stores position info for bone data
        public class BoneInfo
        {
            internal ushort SectionSize { get; set; }
            internal ushort NamePosition { get; set; }
            internal ushort UnknownPosition { get; set; }
            internal ushort Unknown2Position { get; set; }
            internal ushort ParentPosition { get; set; }
            internal ushort Unknown3Position { get; set; }
            internal ushort IsVisablePosition { get; set; }
            internal ushort ScalePosition { get; set; }
            internal ushort RotationPosition { get; set; }
            internal ushort TranslationPosition { get; set; }
            internal ushort Unknown4Position { get; set; }
            internal ushort Unknown5Position { get; set; }

            public void Read(FileReader reader)
            {
                SectionSize = reader.ReadUInt16();
                NamePosition = reader.ReadUInt16();
                UnknownPosition = reader.ReadUInt16();
                Unknown2Position = reader.ReadUInt16(); 
                ParentPosition = reader.ReadUInt16();
                Unknown3Position = reader.ReadUInt16(); //Padding
                IsVisablePosition = reader.ReadUInt16(); //Points to byte. 0 or 1 for visibilty
                ScalePosition = reader.ReadUInt16();
                RotationPosition = reader.ReadUInt16();
                TranslationPosition = reader.ReadUInt16();
                Unknown4Position = reader.ReadUInt16(); //Padding
                Unknown5Position = reader.ReadUInt16();  //Padding
            }
        }

        public class Material
        {

        }
    }
}
