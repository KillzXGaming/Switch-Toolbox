using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Toolbox.Library.IO;

namespace DKCTF
{
    //Documentation from https://github.com/Kinnay/Nintendo-File-Formats/wiki/DKCTF-Types#cformdescriptor

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CFormDescriptor
    {
        public Magic Magic = "RFRM";
        public ulong DataSize;
        public ulong Unknown;
        public Magic FormType;
        public uint VersionA;
        public uint VersionB;
    }

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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CObjectTag
    {
        public Magic Type;
        public CGuid Objectid;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CObjectId
    {
        public CGuid Guid;

        public override string ToString()
        {
            return Guid.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class CGuid
    {
        public uint Part1;
        public ushort Part2;
        public ushort Part3;
        public ulong Part4;

        public override string ToString()
        {
            return $"{Part1}|{Part2}|{Part3}|{Part4}";
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
