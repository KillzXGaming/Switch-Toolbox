using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Toolbox.Library.IO;
using Syroot.BinaryData;

namespace AudioLibrary
{
    public static class AudioHelperExtension
    {
        public static T ReadReference<T>(this FileReader reader, long startOffset) where T : IAudioData, new()
        {
            var reference = reader.ReadStruct<SectionReference>();
            reader.SeekBegin(startOffset + reference.Offset);

            return CreateInstance<T>(reader);
        }

        private static T CreateInstance<T>(FileReader reader)
            where T : IAudioData, new()
        {
            long pos = reader.Position;

            T instance = new T();
            instance.Read(reader);

            reader.SeekBegin(pos);
            return instance;
        }
    }

    public interface IAudioData
    {
        void Read(FileReader reader);
        void Write(FileWriter writer);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class AudioHeader
    {
        public Magic Magic;
        public ushort ByteOrder;
        public ushort HeaderSize;
        public uint Version;
        public uint FileSize;
        public ushort BlockCount;
        public ushort padding;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class BlockReference
    {
        public AudioBlockType Type;
        public ushort padding;
        public uint Offset;
        public uint Size;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SectionReference
    {
        public AudioBlockType Type;
        public ushort padding;
        public uint Offset;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ItemID
    {
        public AudioFileType TypeID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] FileIndex;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class WaveFileID
    {
        public ItemID ArchiveID;
        public uint WavFileIndex;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ADSHRCurve
    {
        public byte Attack;
        public byte Decay;
        public byte Sustain;
        public byte Hold;
        public byte Release;
    }

    //Enum list from vgmstream
    public enum AudioBlockType : ushort
    {
        ByteTableType = 0x0100,
        ReferenceTableType = 0x0101,
        DspAdpcmInfoType = 0x0300,
        ImaAdpcmInfoType = 0x0301,
        SampleDataType = 0x1f00,
        InfoBlockType = 0x4000,
        SeekBlockType = 0x4001,
        DataBlockType = 0x4002,
        RegionBlockType = 0x4003,
        PrefetchDataBlockType = 0x4004,
        StreamInfoType = 0x4100,
        TrackInfoType = 0x4101,
        ChannelInfoType = 0x4102,
        WavInfoBlockType = 0x7000,
        WavDataBlockType = 0x7001,
        WavChannelInfoType = 0x7100,
        InfoBlockType2 = 0x7800,
    }

    public enum AudioFileType : byte
    {
        Sound,
        SoundGroup,
        Bank,
        WaveArchive,
        Group,
    }
}
