using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Toolbox.Library.IO;

namespace DKCTF
{
    /// <summary>
    /// Represents the base of a file format.
    /// </summary>
    public class FileForm
    {
        /// <summary>
        /// The form header containing the type and version of the file.
        /// </summary>
        public CFormDescriptor FileHeader;

        public FileForm() { }

        public bool IsLittleEndian = false;
        public bool IsMPR = false;

        public FileForm(Stream stream, bool leaveOpen = false)
        {
            using (var reader = new FileReader(stream, leaveOpen))
            {
                //Small "hack" to detect endianness.
                using (reader.TemporarySeek(4, SeekOrigin.Begin)) {
                    //Size is a uint64. If the first 4 bytes are present, file is little endian
                    IsLittleEndian = reader.ReadUInt32() != 0;
                    //MPR is only game currently that is little endian
                    IsMPR = IsLittleEndian;
                }

                reader.SetByteOrder(!IsLittleEndian);
                FileHeader = reader.ReadStruct<CFormDescriptor>();
                Read(reader);
                AfterLoad();
            }
        }


        /// <summary>
        /// Reads the file by looping through chunks..
        /// </summary>
        public virtual void Read(FileReader reader)
        {
            var endPos = ReadMetaFooter(reader);

            while (reader.BaseStream.Position < endPos)
            {
                var chunk = reader.ReadStruct<CChunkDescriptor>();
                var pos = reader.Position;

                reader.SeekBegin(pos + chunk.DataOffset);
                ReadChunk(reader, chunk);

                reader.SeekBegin(pos + chunk.DataSize);
            }
        }

        /// <summary>
        /// Reads a specified chunk.
        /// </summary>
        public virtual void ReadChunk(FileReader reader, CChunkDescriptor chunk)
        {
        }

        /// <summary>
        /// Reads meta data information within the pak archive.
        /// </summary>
        public virtual void ReadMetaData(FileReader reader, CFormDescriptor pakVersion)
        {

        }

        /// <summary>
        /// Writes meta data information within the pak archive.
        /// </summary>
        public virtual void WriteMetaData(FileWriter writer, CFormDescriptor pakVersion)
        {

        }

        /// <summary>
        /// Executes after the file has been fully read.
        /// </summary>
        public virtual void AfterLoad()
        {

        }

        /// <summary>
        /// Reads the tool created footer containing meta data used for decompressing buffer data.
        /// </summary>
        public long ReadMetaFooter(FileReader reader)
        {
            using (reader.TemporarySeek(reader.BaseStream.Length - 20, SeekOrigin.Begin))
            {
                if (reader.ReadString(4, Encoding.ASCII) != "META")
                    return reader.BaseStream.Length;

            }

            using (reader.TemporarySeek(reader.BaseStream.Length - 20, SeekOrigin.Begin))
            {
                reader.ReadSignature(4, "META");
                reader.ReadString(4); //type of file
                uint versionA = reader.ReadUInt32(); //pak version A
                uint versionB = reader.ReadUInt32(); //pak version B
                uint size = reader.ReadUInt32(); //size of meta data
                //Seek back to meta data
                reader.SeekBegin(reader.Position - size);
                //Read meta data
                CFormDescriptor pakHeader = new CFormDescriptor();
                pakHeader.VersionA = versionA;
                pakHeader.VersionB = versionB;

                ReadMetaData(reader, pakHeader);

                return reader.BaseStream.Length - size;
            }
        }

        /// <summary>
        /// Writes a footer to a file for accessing meta data information outside a .pak archive.
        /// </summary>
        public static byte[] WriteMetaFooter(FileReader reader, uint metaOffset, string type, PACK pack)
        {
            //Magic + meta offset first
            var mem = new MemoryStream();
            using (var writer = new FileWriter(mem))
            {
                writer.SetByteOrder(!pack.IsLittleEndian);

                reader.SeekBegin(metaOffset);
                var file = GetFileForm(type);
                file.ReadMetaData(reader, pack.FileHeader);
                file.WriteMetaData(writer, pack.FileHeader);

                //Write footer header last
                writer.WriteSignature("META");
                writer.WriteSignature(type);
                writer.Write(pack.FileHeader.VersionA);
                writer.Write(pack.FileHeader.VersionB);
                writer.Write((uint)(writer.BaseStream.Length + 4)); //size
            }
            return mem.ToArray();
        }

        //Creates file instances for read/writing meta entries from pak archives
        static FileForm GetFileForm(string type)
        {
            switch (type)
            {
                case "CMDL": return new CMDL();
                case "SMDL": return new CMDL();
                case "WMDL": return new CMDL();
                case "TXTR": return new TXTR();
            }
            return new FileForm();
        }
    }

    //Documentation from https://github.com/Kinnay/Nintendo-File-Formats/wiki/DKCTF-Types#cformdescriptor

    /// <summary>
    /// Represents the header of a file format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CFormDescriptor
    {
        public Magic Magic = "RFRM";
        public ulong DataSize;
        public ulong Unknown;
        public Magic FormType; //File type identifier
        public uint VersionA;
        public uint VersionB;
    }

    /// <summary>
    /// Represents the header of a chunk of a file form.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CChunkDescriptor
    {
        public Magic ChunkType;
        public long DataSize;
        public uint Unknown;
        public long DataOffset;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CMayaSpline
    {
        public Magic ChunkType;
        public ulong DataSize;
        public uint Unknown;
        public ulong DataOffset;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CMayaSplineKnot
    {
        public float Time;
        public float Value;
        public ETangentType TangentType1;
        public ETangentType TangentType2;
        public float FieldC;
        public float Field10;
        public float Field14;
        public float Field18;
    }

    /// <summary>
    /// Tag data for an object providing the type and id.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CObjectTag
    {
        public Magic Type;
        public CObjectId Objectid;
    }

    /// <summary>
    /// A header for asset data providing a type and version number.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CAssetHeader
    {
        public ushort TypeID;
        public ushort Version;
    }


    /// <summary>
    /// Stores a unique ID for a given object to identify it.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CObjectId
    {
        public CGuid Guid;

        public override string ToString()
        {
            return Guid.ToString();
        }

        public bool IsZero()
        {
            return Guid.Part1 == 0 && 
                   Guid.Part2 == 0 && 
                   Guid.Part3 == 0 &&
                   Guid.Part4[0] == 0;
        }
    }

    /// <summary>
    /// An axis aligned bounding box with a min and max position value.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CAABox
    {
        public Vector3f Min;
        public Vector3f Max;
    }

    /// <summary>
    /// A vector with X/Y/Z axis values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Vector3f
    {
        public float X;
        public float Y;
        public float Z;
    }

    /// <summary>
    /// A color struct of RGBA values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Color4f
    {
        public float R;
        public float G;
        public float B;
        public float A;
    }

    /// <summary>
    /// A 128 bit guid for identifying objects.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CGuid
    {
        public uint Part1;
        public ushort Part2;
        public ushort Part3;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Part4;

        public Guid ToGUID()
        {
            return new Guid(Part1, Part2, Part3, Part4[0], Part4[1], Part4[2], Part4[3], Part4[4], Part4[5], Part4[6], Part4[7]);
        }

        public override string ToString() //Represented based on output guids in demo files
        {
            return ToGUID().ToString();
        }
    }

    public enum ETangentType
    {
        Linear,
        Flat,
        Smooth,
        Step,
        Clamped,
        Fixed,
    }

    public enum EInfinityType
    {
        Constant,
        Linear,
        Cycle,
        CycleRelative,
        Oscillate,
    }
}
