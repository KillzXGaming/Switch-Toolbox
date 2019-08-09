using Toolbox.Library.IO;
using System.Runtime.InteropServices;

namespace CTR.NCCH
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class RomfsHeader
    {
        public Magic Magic = "IVFC";
        public uint MagicNumer = 0x10000;
        public uint MasterHashSize;
        public ulong Level1LogicalOffset;
        public ulong Level1HashDataOffset;
        public uint Level1BlockSize; //In Log2
        public uint Reserved;
        public ulong Level2LogicalOffset;
        public ulong Level2HashDataOffset;
        public uint Level2BlockSize; //In Log2
        public uint Reserved2;
        public ulong Level3LogicalOffset;
        public ulong Level3HashDataOffset;
        public uint Level3BlockSize; //In Log2
        public uint Reserved3;
        public uint Reserved4;
        public uint InfoSize; //Optional
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Level3Partition
    {

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Level3Header
    {
        public uint HeaderLength;
        public uint DirectoryHashTableOffset;
        public uint DirectoryHashTableSize;
        public uint DirectoryMetaDataTableOffset;
        public uint DirectoryMetaDataTableSize;
        public uint FileHashTableOffset;
        public uint FileHashTableSize;
        public uint FileMetaDataTableOffset;
        public uint FileMetaDataTableSize;
        public uint FileDataOffset;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DirectoryMetaData
    {
        public uint ParentDirectoryOffset;
        public uint NextSiblingOffset;
        public uint FirstChildOffset;
        public uint FirstFileOffset; //In meta data table
        public uint NextDirectoryOffset; //In same bucket
        public uint NameLength;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class FileMetaData
    {
        public uint DirectoryOffset;
        public uint NextSiblingOffset;
        public ulong FileDataOffset;
        public ulong FileDataSize;
        public uint NextFileOffset; //In same hash table bucket
        public uint NameLength;
    }
}
