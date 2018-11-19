using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.BinaryData;
using System.IO;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;

namespace Switch_Toolbox.Library
{
    public class DDS
    {
        public Header header;
        public DX10Header DX10header;
        public class Header
        {
            public uint size = 0x7C;
            public uint flags = 0x00000000;
            public uint height = 0;
            public uint width = 0;
            public uint pitchOrLinearSize = 0;
            public uint depth = 0;
            public uint mipmapCount = 0;
            public uint[] reserved1 = new uint[11];
            public DDS_PixelFormat ddspf = new DDS_PixelFormat();
            public class DDS_PixelFormat
            {
                public uint size = 0x20;
                public uint flags = 0x00000000;
                public string fourCC;
                public uint RGBBitCount = 0;
                public uint RBitMask = 0x00000000;
                public uint GBitMask = 0x00000000;
                public uint BBitMask = 0x00000000;
                public uint ABitMask = 0x00000000;
            }
            public uint caps = 0;
            public uint caps2 = 0;
            public uint caps3 = 0;
            public uint caps4 = 0;
            public uint reserved2 = 0;
        }
        public class DX10Header
        {
            public DXGI_FORMAT DXGI_Format;
            public uint ResourceDim;
            public uint miscFlag;
            public uint arrayFlag;
            public uint miscFlags2;

        }
        public byte[] bdata;
        public List<byte[]> mipmaps = new List<byte[]>();

        public enum DXGI_FORMAT : uint
        {
            DXGI_FORMAT_UNKNOWN = 0,
            DXGI_FORMAT_R32G32B32A32_TYPELESS = 1,
            DXGI_FORMAT_R32G32B32A32_FLOAT = 2,
            DXGI_FORMAT_R32G32B32A32_UINT = 3,
            DXGI_FORMAT_R32G32B32A32_SINT = 4,
            DXGI_FORMAT_R32G32B32_TYPELESS = 5,
            DXGI_FORMAT_R32G32B32_FLOAT = 6,
            DXGI_FORMAT_R32G32B32_UINT = 7,
            DXGI_FORMAT_R32G32B32_SINT = 8,
            DXGI_FORMAT_R16G16B16A16_TYPELESS = 9,
            DXGI_FORMAT_R16G16B16A16_FLOAT = 10,
            DXGI_FORMAT_R16G16B16A16_UNORM = 11,
            DXGI_FORMAT_R16G16B16A16_UINT = 12,
            DXGI_FORMAT_R16G16B16A16_SNORM = 13,
            DXGI_FORMAT_R16G16B16A16_SINT = 14,
            DXGI_FORMAT_R32G32_TYPELESS = 15,
            DXGI_FORMAT_R32G32_FLOAT = 16,
            DXGI_FORMAT_R32G32_UINT = 17,
            DXGI_FORMAT_R32G32_SINT = 18,
            DXGI_FORMAT_R32G8X24_TYPELESS = 19,
            DXGI_FORMAT_D32_FLOAT_S8X24_UINT = 20,
            DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS = 21,
            DXGI_FORMAT_X32_TYPELESS_G8X24_UINT = 22,
            DXGI_FORMAT_R10G10B10A2_TYPELESS = 23,
            DXGI_FORMAT_R10G10B10A2_UNORM = 24,
            DXGI_FORMAT_R10G10B10A2_UINT = 25,
            DXGI_FORMAT_R11G11B10_FLOAT = 26,
            DXGI_FORMAT_R8G8B8A8_TYPELESS = 27,
            DXGI_FORMAT_R8G8B8A8_UNORM = 28,
            DXGI_FORMAT_R8G8B8A8_UNORM_SRGB = 29,
            DXGI_FORMAT_R8G8B8A8_UINT = 30,
            DXGI_FORMAT_R8G8B8A8_SNORM = 31,
            DXGI_FORMAT_R8G8B8A8_SINT = 32,
            DXGI_FORMAT_R16G16_TYPELESS = 33,
            DXGI_FORMAT_R16G16_FLOAT = 34,
            DXGI_FORMAT_R16G16_UNORM = 35,
            DXGI_FORMAT_R16G16_UINT = 36,
            DXGI_FORMAT_R16G16_SNORM = 37,
            DXGI_FORMAT_R16G16_SINT = 38,
            DXGI_FORMAT_R32_TYPELESS = 39,
            DXGI_FORMAT_D32_FLOAT = 40,
            DXGI_FORMAT_R32_FLOAT = 41,
            DXGI_FORMAT_R32_UINT = 42,
            DXGI_FORMAT_R32_SINT = 43,
            DXGI_FORMAT_R24G8_TYPELESS = 44,
            DXGI_FORMAT_D24_UNORM_S8_UINT = 45,
            DXGI_FORMAT_R24_UNORM_X8_TYPELESS = 46,
            DXGI_FORMAT_X24_TYPELESS_G8_UINT = 47,
            DXGI_FORMAT_R8G8_TYPELESS = 48,
            DXGI_FORMAT_R8G8_UNORM = 49,
            DXGI_FORMAT_R8G8_UINT = 50,
            DXGI_FORMAT_R8G8_SNORM = 51,
            DXGI_FORMAT_R8G8_SINT = 52,
            DXGI_FORMAT_R16_TYPELESS = 53,
            DXGI_FORMAT_R16_FLOAT = 54,
            DXGI_FORMAT_D16_UNORM = 55,
            DXGI_FORMAT_R16_UNORM = 56,
            DXGI_FORMAT_R16_UINT = 57,
            DXGI_FORMAT_R16_SNORM = 58,
            DXGI_FORMAT_R16_SINT = 59,
            DXGI_FORMAT_R8_TYPELESS = 60,
            DXGI_FORMAT_R8_UNORM = 61,
            DXGI_FORMAT_R8_UINT = 62,
            DXGI_FORMAT_R8_SNORM = 63,
            DXGI_FORMAT_R8_SINT = 64,
            DXGI_FORMAT_A8_UNORM = 65,
            DXGI_FORMAT_R1_UNORM = 66,
            DXGI_FORMAT_R9G9B9E5_SHAREDEXP = 67,
            DXGI_FORMAT_R8G8_B8G8_UNORM = 68,
            DXGI_FORMAT_G8R8_G8B8_UNORM = 69,
            DXGI_FORMAT_BC1_TYPELESS = 70,
            DXGI_FORMAT_BC1_UNORM = 71,
            DXGI_FORMAT_BC1_UNORM_SRGB = 72,
            DXGI_FORMAT_BC2_TYPELESS = 73,
            DXGI_FORMAT_BC2_UNORM = 74,
            DXGI_FORMAT_BC2_UNORM_SRGB = 75,
            DXGI_FORMAT_BC3_TYPELESS = 76,
            DXGI_FORMAT_BC3_UNORM = 77,
            DXGI_FORMAT_BC3_UNORM_SRGB = 78,
            DXGI_FORMAT_BC4_TYPELESS = 79,
            DXGI_FORMAT_BC4_UNORM = 80,
            DXGI_FORMAT_BC4_SNORM = 81,
            DXGI_FORMAT_BC5_TYPELESS = 82,
            DXGI_FORMAT_BC5_UNORM = 83,
            DXGI_FORMAT_BC5_SNORM = 84,
            DXGI_FORMAT_B5G6R5_UNORM = 85,
            DXGI_FORMAT_B5G5R5A1_UNORM = 86,
            DXGI_FORMAT_B8G8R8A8_UNORM = 87,
            DXGI_FORMAT_B8G8R8X8_UNORM = 88,
            DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM = 89,
            DXGI_FORMAT_B8G8R8A8_TYPELESS = 90,
            DXGI_FORMAT_B8G8R8A8_UNORM_SRGB = 91,
            DXGI_FORMAT_B8G8R8X8_TYPELESS = 92,
            DXGI_FORMAT_B8G8R8X8_UNORM_SRGB = 93,
            DXGI_FORMAT_BC6H_TYPELESS = 94,
            DXGI_FORMAT_BC6H_UF16 = 95,
            DXGI_FORMAT_BC6H_SF16 = 96,
            DXGI_FORMAT_BC7_TYPELESS = 97,
            DXGI_FORMAT_BC7_UNORM = 98,
            DXGI_FORMAT_BC7_UNORM_SRGB = 99,
            DXGI_FORMAT_AYUV = 100,
            DXGI_FORMAT_Y410 = 101,
            DXGI_FORMAT_Y416 = 102,
            DXGI_FORMAT_NV12 = 103,
            DXGI_FORMAT_P010 = 104,
            DXGI_FORMAT_P016 = 105,
            DXGI_FORMAT_420_OPAQUE = 106,
            DXGI_FORMAT_YUY2 = 107,
            DXGI_FORMAT_Y210 = 108,
            DXGI_FORMAT_Y216 = 109,
            DXGI_FORMAT_NV11 = 110,
            DXGI_FORMAT_AI44 = 111,
            DXGI_FORMAT_IA44 = 112,
            DXGI_FORMAT_P8 = 113,
            DXGI_FORMAT_A8P8 = 114,
            DXGI_FORMAT_B4G4R4A4_UNORM = 115,
            DXGI_FORMAT_P208 = 130,
            DXGI_FORMAT_V208 = 131,
            DXGI_FORMAT_V408 = 132,
            DXGI_FORMAT_FORCE_UINT = 0xFFFFFFFF
        }
        public DDS()
        {

        }
        public DDS(byte[] data)
        {
            FileReader reader = new FileReader(new MemoryStream(data));
            Load(reader);
        }
        public DDS(string FileName)
        {
            FileReader reader = new FileReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read));

            Load(reader);
        }
        public void Load(BinaryDataReader reader)
        {
            reader.Seek(0);
            string Magic = reader.ReadString(4);
            Console.WriteLine(Magic);
            if (Magic != "DDS ")
            {
                MessageBox.Show("The file does not appear to be a valid DDS file.");
            }

            header = new Header();
            header.size = reader.ReadUInt32();
            header.flags = reader.ReadUInt32();
            header.height = reader.ReadUInt32();
            header.width = reader.ReadUInt32();


            header.pitchOrLinearSize = reader.ReadUInt32();
            header.depth = reader.ReadUInt32();
            header.mipmapCount = reader.ReadUInt32();
            header.reserved1 = new uint[11];
            for (int i = 0; i < 11; ++i)
                header.reserved1[i] = reader.ReadUInt32();

            header.ddspf.size = reader.ReadUInt32();
            header.ddspf.flags = reader.ReadUInt32();
            header.ddspf.fourCC = reader.ReadString(4);
            header.ddspf.RGBBitCount = reader.ReadUInt32();
            header.ddspf.RBitMask = reader.ReadUInt32();
            header.ddspf.GBitMask = reader.ReadUInt32();
            header.ddspf.BBitMask = reader.ReadUInt32();
            header.ddspf.ABitMask = reader.ReadUInt32();

            header.caps = reader.ReadUInt32();
            header.caps2 = reader.ReadUInt32();
            header.caps3 = reader.ReadUInt32();
            header.caps4 = reader.ReadUInt32();
            header.reserved2 = reader.ReadUInt32();

            int DX10HeaderSize = 0;
            if (header.ddspf.fourCC == "DX10")
            {
                DX10HeaderSize = 20;
                ReadDX10Header(reader);
            }

            reader.TemporarySeek((int)(4 + header.size + DX10HeaderSize), SeekOrigin.Begin);
            bdata = reader.ReadBytes((int)(reader.BaseStream.Length - reader.Position));




            reader.Dispose();
            reader.Close();
        }
        private void ReadDX10Header(BinaryDataReader reader)
        {
            DX10header = new DX10Header();
            DX10header.DXGI_Format = reader.ReadEnum<DXGI_FORMAT>(true);
            DX10header.ResourceDim = reader.ReadUInt32();
            DX10header.miscFlag = reader.ReadUInt32();
            DX10header.arrayFlag = reader.ReadUInt32();
            DX10header.miscFlags2 = reader.ReadUInt32();
        }
        public void Save(DDS dds, string FileName, bool IsDX10 = false, List<List<byte[]>> data = null)
        {
            FileWriter writer = new FileWriter(new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Write));
            writer.Write(Encoding.ASCII.GetBytes("DDS "));
            writer.Write(header.size);
            writer.Write(header.flags);
            writer.Write(header.height);
            writer.Write(header.width);



            writer.Write(header.pitchOrLinearSize);
            writer.Write(header.depth);
            writer.Write(header.mipmapCount);
            for (int i = 0; i < 11; ++i)
                writer.Write(header.reserved1[i]);

            writer.Write(header.ddspf.size);
            writer.Write(header.ddspf.flags);
            writer.WriteSignature(header.ddspf.fourCC);
            writer.Write(header.ddspf.RGBBitCount);
            writer.Write(header.ddspf.RBitMask);
            writer.Write(header.ddspf.GBitMask);
            writer.Write(header.ddspf.BBitMask);
            writer.Write(header.ddspf.ABitMask);
            writer.Write(header.caps);
            writer.Write(header.caps2);
            writer.Write(header.caps3);
            writer.Write(header.caps4);
            writer.Write(header.reserved2);

            if (IsDX10)
            {
                WriteDX10Header(writer);
            }

            if (data != null)
            {
                foreach (byte[] mip in data[0])
                {
                    writer.Write(mip);
                }
            }
            else
            {
                writer.Write(bdata);
            }

            writer.Flush();
        }
        private void WriteDX10Header(BinaryDataWriter writer)
        {
            if (DX10header == null)
                DX10header = new DX10Header();

            writer.Write((uint)DX10header.DXGI_Format);
            writer.Write(DX10header.ResourceDim);
            writer.Write(DX10header.miscFlag);
            writer.Write(DX10header.arrayFlag);
            writer.Write(DX10header.miscFlags2);
        }
        public static byte[] CompressBC1Block(byte[] data, int Width, int Height)
        {
            byte[] image = new byte[0];

            return image;
        }
        public static void ToRGBA(byte[] data, int Width, int Height, int bpp, int compSel)
        {
            int Size = Width * Height * 4;

            byte[] result = new byte[Size];

            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {
                    int pos = (Y * Width + X) * bpp;
                    int pos_ = (Y * Width + X) * 4;

                    int pixel = 0;


                }
            }
        }
    }
}
