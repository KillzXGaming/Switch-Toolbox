using Toolbox.Library.IO;
using System.Runtime.InteropServices;

namespace CTR.NCCH
{
    //Documentation from https://www.3dbrew.org/wiki/NCCH
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] Rsa2048Signature;
        public Magic Magic = "NCCH";
        public uint ContentSize; //In media units (0x200 bytes = 1 unit)
        public ulong PartitionID;
        public ushort MakerCode;
        public ushort Version;
        public uint SeedHashCheck;
        public ulong ProgramID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] LogoRegionSHA256Hash;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] ProductCode;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] ExtendedHeaderSHA256Hash; //2x Alignment Size

        public uint ExtendedHeaderSize;
        public uint Reserved2;
        public ulong Flags;
        public uint PlainRegionOffset;
        public uint PlainRegionSize;
        public uint LogoRegionOffset;
        public uint LogoRegionSize;
        public uint ExeFSOffset;
        public uint ExeFSSize;
        public uint ExeFSHashRegionSize;
        public uint Reserved3;
        public uint RomfsOffset;
        public uint RomfsSize;
        public uint RomfsHashRegionSize;
        public uint Reserved4;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] ExeFSSuperBlockSHA256Hash; //2x Alignment Size

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] RomFSSuperBlockSHA256Hash; //2x Alignment Size
    }
}
