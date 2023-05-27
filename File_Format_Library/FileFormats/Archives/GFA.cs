using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using System.IO;
using VGAudio.Utilities;

namespace FirstPlugin
{
    public class GFA : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Good Feel Archive" };
        public string[] Extension { get; set; } = new string[] { "*.gfa" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "GFAC");
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

        public List<FileEntry> files = new List<FileEntry>();

        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        private uint Unknown1;
        private uint Version;
        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                reader.ReadSignature(4, "GFAC");
                Unknown1 = reader.ReadUInt32();
                Version = reader.ReadUInt32();
                uint FileInfoOffset = reader.ReadUInt32();
                uint FileInfoSize = reader.ReadUInt32();
                uint DataOffset = reader.ReadUInt32();
                uint DataSize = reader.ReadUInt32();
                byte[] Padding = reader.ReadBytes(0x10); //Not sure

                reader.SeekBegin(FileInfoOffset);
                uint FileCount = reader.ReadUInt32();
                for (int i = 0; i < FileCount; i++)
                {
                    var file = new FileEntry();
                    file.Read(reader);
                    files.Add(file);
                }

                reader.SeekBegin(DataOffset);
                reader.ReadSignature(4, "GFCP");
                uint VersionGFCP = reader.ReadUInt32();
                uint CompressionType = reader.ReadUInt32();
                uint DecompressedSize = reader.ReadUInt32();
                uint CompressedSize = reader.ReadUInt32();

                byte[] input = reader.ReadBytes((int)CompressedSize);
                if (CompressionType == 1) input = BPE.Decompress(input, DecompressedSize, CompressedSize);
                if (CompressionType == 2) input = LZ77_WII.Decompress(input);
                if (CompressionType == 3) input = LZ77_WII.Decompress(input); //same as 2

                using (var r = new FileReader(input))
                {
                    for (int i = 0; i < FileCount; i++)
                    {
                        r.SeekBegin(files[i].Offset - DataOffset);
                        files[i].FileData = r.ReadBytes((int)files[i].Size);
                    }
                }
            }
        }


        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream, true))
            {
                writer.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                writer.WriteSignature("GFAC");
                writer.Write(Unknown1);
                writer.Write(Version);
                writer.Write(uint.MaxValue); //Info offset for later
                writer.Write(uint.MaxValue); //Info size for later
                writer.Write(uint.MaxValue); //Data offset for later
                writer.Write(uint.MaxValue); //Data size for later
                writer.Write(new byte[0x10]); //Padding

                writer.WriteUint32Offset(12); //Save info offset
                writer.Write(files.Count);

             /*   //Save info
                for (int i = 0; i < files.Count; i++)
                    files[i].Write(writer);

                //Save strings and offsets
                for (int i = 0; i < files.Count; i++)
                    files[i].Write(writer);

                writer.Write(new uint[files.Count]); //Save space for offsets
                for (int i = 0; i < files.Count; i++)
                    writer.Write(files[i].FileData.Length);
                */
            }
        }

        private void Align(FileWriter writer, int alignment)
        {
            var startPos = writer.Position;
            long position = writer.Seek((-writer.Position % alignment + alignment) % alignment, System.IO.SeekOrigin.Current);

            writer.Seek(startPos, System.IO.SeekOrigin.Begin);
            while (writer.Position != position)
            {
                writer.Write((byte)0x30);
            }
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public class FileEntry : ArchiveFileInfo
        {
            public uint Hash { get; set; }
            public uint Offset { get; set; }
            public uint Size { get; set; }

            public void Read(FileReader reader)
            {
                Hash = reader.ReadUInt32();
                FileName = GetName(reader);
                Size = reader.ReadUInt32();
                Offset = reader.ReadUInt32();
            }
        }

        private static string GetName(FileReader reader)
        {
            uint Offset = reader.ReadUInt32();
            using (reader.TemporarySeek(Offset & 0x00ffffff, System.IO.SeekOrigin.Begin))
            {
                return reader.ReadZeroTerminatedString();
            }
        }

        class BPE
        {
            //Decompiled from MasterF0X tool for BPE decompression
            static int index = 0;
            static bool end = false;

            public static byte[] Decompress(byte[] input, uint decompressedSize, uint cs)
            {
                char[] chArray1 = new char[16777215];
                char[] chArray2 = new char[16777215];
                char[] chArray3 = new char[16777215];

                var mem = new MemoryStream();
                BinaryWriter binaryWriter = new BinaryWriter(mem);
            label_26:
                int num1;
                int num2;
                while (index < input.Length)
                {
                    if (end)
                    {
                        binaryWriter.Close();
                        end = false;
                        index = 0;
                        num1 = 0;
                        num2 = 0;
                        return mem.ToArray();
                    }
                    for (int index = 0; index < 256; ++index)
                        chArray1[index] = (char)index;
                    int num3 = (int)getc(input, cs);
                    int index1 = 0;
                    while (true)
                    {
                        if (num3 > (int)sbyte.MaxValue)
                        {
                            index1 += num3 - (int)sbyte.MaxValue;
                            num3 = 0;
                        }
                        if (index1 != 256)
                        {
                            int num4 = 0;
                            while (num4 <= num3)
                            {
                                chArray1[index1] = getc(input, cs);
                                if (index1 != (int)chArray1[index1])
                                    chArray2[index1] = getc(input, cs);
                                ++num4;
                                ++index1;
                            }
                            if (index1 != 256)
                                num3 = (int)getc(input, cs);
                            else
                                break;
                        }
                        else
                            break;
                    }
                    int num5 = 256 * (int)getc(input, cs) + (int)getc(input, cs);
                    int num6 = 0;
                    while (true)
                    {
                        int index2;
                        if (num6 > 0)
                            index2 = (int)chArray3[--num6];
                        else if (num5-- != 0)
                            index2 = (int)getc(input, cs);
                        else
                            goto label_26;
                        if (index2 == (int)chArray1[index2])
                        {
                            binaryWriter.Write((byte)index2);
                        }
                        else
                        {
                            char[] chArray4 = chArray3;
                            int index3 = num6;
                            int num7 = index3 + 1;
                            int num8 = (int)chArray2[index2];
                            chArray4[index3] = (char)num8;
                            char[] chArray5 = chArray3;
                            int index4 = num7;
                            num6 = index4 + 1;
                            int num9 = (int)chArray1[index2];
                            chArray5[index4] = (char)num9;
                        }
                    }
                }
                binaryWriter.Close();
                end = false;
                index = 0;
                num1 = 0;
                num2 = 0;
                return mem.ToArray();
            }

            static char getc(byte[] input, uint uncompS)
            {
                if (index >= uncompS)
                {
                    end = true;
                    return '0';
                }
                char ch = (char)input[index];
                ++index;
                return ch;
            }
        }
    }
}
