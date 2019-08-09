using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using System.Runtime.InteropServices;

namespace CTR.NCSD
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Header
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] Rsa2048Signature;
        public Magic Magic = "NCSD";
        public uint ContentSize; //In media units (0x200 bytes = 1 unit)
        public ulong MediaID;
        public ulong PartFSType; // (0=None, 1=Normal, 3=FIRM, 4=AGB_FIRM save)
        public ulong PartCryptoType;
    
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public PartEntry[] Parts;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] SHA256Hash;

        uint ExtraHeaderSize;
        uint SectorZeroOffset;
        ulong PartitionFlags;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public ulong[] PartionIdTableEntries;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public byte[] Reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xE)]
        public byte[] Reserved2;

        byte Verification;
        byte SaveCrypto;
    }

    public struct PartEntry
    {
        public uint Offset; //In media units (0x200 bytes = 1 unit)
        public uint Size;   //In media units (0x200 bytes = 1 unit)
    }
}
