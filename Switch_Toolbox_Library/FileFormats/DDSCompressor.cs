using System;
using System.Diagnostics;
using System.Drawing;
using Toolbox.Library;
using System.Runtime.InteropServices;
using DirectXTexNet;

namespace Toolbox.Library
{
    public class DDSCompressor
    {
        //Huge thanks to gdkchan and AbooodXD for the method of decomp BC5/BC4.

        //Todo. Add these to DDS code and add in methods to compress and decode more formats
        //BC7 also needs to be decompressed properly since OpenTK can't decompress those

        //BC4 actually breaks a bit with artifacts so i'll need to go back and fix

        private static byte[] BCnDecodeTile(byte[] Input, int Offset, bool IsBC1)
        {
            Color[] CLUT = new Color[4];

            int c0 = Get16(Input, Offset + 0);
            int c1 = Get16(Input, Offset + 2);

            CLUT[0] = DecodeRGB565(c0);
            CLUT[1] = DecodeRGB565(c1);
            CLUT[2] = CalculateCLUT2(CLUT[0], CLUT[1], c0, c1, IsBC1);
            CLUT[3] = CalculateCLUT3(CLUT[0], CLUT[1], c0, c1, IsBC1);

            int Indices = Get32(Input, Offset + 4);

            int IdxShift = 0;

            byte[] Output = new byte[4 * 4 * 4];

            int OOffset = 0;

            for (int TY = 0; TY < 4; TY++)
            {
                for (int TX = 0; TX < 4; TX++)
                {
                    int Idx = (Indices >> IdxShift) & 3;

                    IdxShift += 2;

                    Color Pixel = CLUT[Idx];

                    Output[OOffset + 0] = Pixel.B;
                    Output[OOffset + 1] = Pixel.G;
                    Output[OOffset + 2] = Pixel.R;
                    Output[OOffset + 3] = Pixel.A;

                    OOffset += 4;
                }
            }
            return Output;
        }
        private static Color DecodeRGB565(int Value)
        {
            int B = ((Value >> 0) & 0x1f) << 3;
            int G = ((Value >> 5) & 0x3f) << 2;
            int R = ((Value >> 11) & 0x1f) << 3;

            return Color.FromArgb(
                R | (R >> 5),
                G | (G >> 6),
                B | (B >> 5));
        }
        private static Color CalculateCLUT2(Color C0, Color C1, int c0, int c1, bool IsBC1)
        {
            if (c0 > c1 || !IsBC1)
            {
                return Color.FromArgb(
                    (2 * C0.R + C1.R) / 3,
                    (2 * C0.G + C1.G) / 3,
                    (2 * C0.B + C1.B) / 3);
            }
            else
            {
                return Color.FromArgb(
                    (C0.R + C1.R) / 2,
                    (C0.G + C1.G) / 2,
                    (C0.B + C1.B) / 2);
            }
        }
        private static Color CalculateCLUT3(Color C0, Color C1, int c0, int c1, bool IsBC1)
        {
            if (c0 > c1 || !IsBC1)
            {
                return
                    Color.FromArgb(
                        (2 * C1.R + C0.R) / 3,
                        (2 * C1.G + C0.G) / 3,
                        (2 * C1.B + C0.B) / 3);
            }

            return Color.Transparent;
        }
        public static Bitmap DecompressBC1(Byte[] data, int width, int height, bool IsSRGB)
        {
            int W = (width + 3) / 4;
            int H = (height + 3) / 4;

            byte[] Output = new byte[W * H * 64];

            for (int Y = 0; Y < H; Y++)
            {
                for (int X = 0; X < W; X++)
                {
                    int IOffs = (Y * W + X) * 8;

                    byte[] Tile = BCnDecodeTile(data, IOffs, true);

                    int TOffset = 0;

                    for (int TY = 0; TY < 4; TY++)
                    {
                        for (int TX = 0; TX < 4; TX++)
                        {
                            int OOffset = (X * 4 + TX + (Y * 4 + TY) * W * 4) * 4;

                            Output[OOffset + 0] = Tile[TOffset + 0];
                            Output[OOffset + 1] = Tile[TOffset + 1];
                            Output[OOffset + 2] = Tile[TOffset + 2];
                            Output[OOffset + 3] = Tile[TOffset + 3];

                            TOffset += 4;
                        }
                    }
                }
            }
            return BitmapExtension.GetBitmap(Output, W * 4, H * 4);
        }
        public static Bitmap DecompressBC3(Byte[] data, int width, int height, bool IsSRGB)
        {
            int W = (width + 3) / 4;
            int H = (height + 3) / 4;

            byte[] Output = new byte[W * H * 64];

            for (int Y = 0; Y < H; Y++)
            {
                for (int X = 0; X < W; X++)
                {
                    int IOffs = (Y * W + X) * 16;

                    byte[] Tile = BCnDecodeTile(data, IOffs + 8, false);

                    byte[] Alpha = new byte[8];

                    Alpha[0] = data[IOffs + 0];
                    Alpha[1] = data[IOffs + 1];

                    CalculateBC3Alpha(Alpha);

                    int AlphaLow = Get32(data, IOffs + 2);
                    int AlphaHigh = Get16(data, IOffs + 6);

                    ulong AlphaCh = (uint)AlphaLow | (ulong)AlphaHigh << 32;

                    int TOffset = 0;

                    for (int TY = 0; TY < 4; TY++)
                    {
                        for (int TX = 0; TX < 4; TX++)
                        {
                            int OOffset = (X * 4 + TX + (Y * 4 + TY) * W * 4) * 4;

                            byte AlphaPx = Alpha[(AlphaCh >> (TY * 12 + TX * 3)) & 7];

                            Output[OOffset + 0] = Tile[TOffset + 0];
                            Output[OOffset + 1] = Tile[TOffset + 1];
                            Output[OOffset + 2] = Tile[TOffset + 2];
                            Output[OOffset + 3] = AlphaPx;

                            TOffset += 4;
                        }
                    }
                }
            }

            return BitmapExtension.GetBitmap(Output, W * 4, H * 4);
        }
        public static Bitmap DecompressBC4(Byte[] data, int width, int height, bool IsSNORM)
          {
              int W = (width + 3) / 4;
              int H = (height + 3) / 4;

              byte[] Output = new byte[W * H * 64];

              for (int Y = 0; Y < H; Y++)
              {
                  for (int X = 0; X < W; X++)
                  {
                      int IOffs = (Y * W + X) * 8;

                      byte[] Red = new byte[8];

                      Red[0] = data[IOffs + 0];
                      Red[1] = data[IOffs + 1];

                    if (IsSNORM)
                        CalculateBC3AlphaS(Red);
                    else
                        CalculateBC3Alpha(Red);

                    int RedLow = Get32(data, IOffs + 2);
                      int RedHigh = Get16(data, IOffs + 6);

                      ulong RedCh = (uint)RedLow | (ulong)RedHigh << 32;

                      int TOffset = 0;
                      int TW = Math.Min(width - X * 4, 4);
                      int TH = Math.Min(height - Y * 4, 4);

                      for (int TY = 0; TY < 4; TY++)
                      {
                          for (int TX = 0; TX < 4; TX++)
                          {
                              int OOffset = (X * 4 + TX + (Y * 4 + TY) * W * 4) * 4;

                              byte RedPx = Red[(RedCh >> (TY * 12 + TX * 3)) & 7];

                              Output[OOffset + 0] = RedPx;
                              Output[OOffset + 1] = RedPx;
                              Output[OOffset + 2] = RedPx;
                              Output[OOffset + 3] = 255;

                              TOffset += 4;
                          }
                      }
                  }
              }

              return BitmapExtension.GetBitmap(Output, W * 4, H * 4);
          }

        public static byte[] DecompressBC5(Byte[] data, int width, int height, bool IsSNORM, bool IsByteArray)
        {
            int W = (width + 3) / 4;
            int H = (height + 3) / 4;

            byte[] Output = new byte[width * height * 4];

            for (int Y = 0; Y < H; Y++)
            {
                for (int X = 0; X < W; X++)
                {
                    int IOffs = (Y * W + X) * 16;
                    byte[] Red = new byte[8];
                    byte[] Green = new byte[8];

                    Red[0] = data[IOffs + 0];
                    Red[1] = data[IOffs + 1];

                    Green[0] = data[IOffs + 8];
                    Green[1] = data[IOffs + 9];

                    if (IsSNORM == true)
                    {
                        CalculateBC3AlphaS(Red);
                        CalculateBC3AlphaS(Green);
                    }
                    else
                    {
                        CalculateBC3Alpha(Red);
                        CalculateBC3Alpha(Green);
                    }

                    int RedLow = Get32(data, IOffs + 2);
                    int RedHigh = Get16(data, IOffs + 6);

                    int GreenLow = Get32(data, IOffs + 10);
                    int GreenHigh = Get16(data, IOffs + 14);

                    ulong RedCh = (uint)RedLow | (ulong)RedHigh << 32;
                    ulong GreenCh = (uint)GreenLow | (ulong)GreenHigh << 32;

                    int TW = Math.Min(width - X * 4, 4);
                    int TH = Math.Min(height - Y * 4, 4);

                    if (IsSNORM == true)
                    {
                        for (int TY = 0; TY < TH; TY++)
                        {
                            for (int TX = 0; TX < TW; TX++)
                            {

                                int Shift = TY * 12 + TX * 3;
                                int OOffset = ((Y * 4 + TY) * width + (X * 4 + TX)) * 4;

                                byte RedPx = Red[(RedCh >> Shift) & 7];
                                byte GreenPx = Green[(GreenCh >> Shift) & 7];

                                if (IsSNORM == true)
                                {
                                    RedPx += 0x80;
                                    GreenPx += 0x80;
                                }

                                float NX = (RedPx / 255f) * 2 - 1;
                                float NY = (GreenPx / 255f) * 2 - 1;
                                float NZ = (float)Math.Sqrt(1 - (NX * NX + NY * NY));

                                Output[OOffset + 0] = Clamp((NX + 1) * 0.5f);
                                Output[OOffset + 1] = Clamp((NY + 1) * 0.5f);
                                Output[OOffset + 2] = Clamp((NZ + 1) * 0.5f);
                                Output[OOffset + 3] = 0xff;
                            }
                        }
                    }
                    else
                    {
                        for (int TY = 0; TY < TH; TY++)
                        {
                            for (int TX = 0; TX < TW; TX++)
                            {

                                int Shift = TY * 12 + TX * 3;
                                int OOffset = ((Y * 4 + TY) * width + (X * 4 + TX)) * 4;

                                byte RedPx = Red[(RedCh >> Shift) & 7];
                                byte GreenPx = Green[(GreenCh >> Shift) & 7];

                                Output[OOffset + 0] = RedPx;
                                Output[OOffset + 1] = GreenPx;
                                Output[OOffset + 2] = 255;
                                Output[OOffset + 3] = 255;

                            }
                        }
                    }
                }
            }
            return Output;
        }
        public static Bitmap DecompressBC5(Byte[] data, int width, int height, bool IsSNORM)
        {
            int W = (width + 3) / 4;
            int H = (height + 3) / 4;

            byte[] Output = new byte[W * H * 64];

            for (int Y = 0; Y < H; Y++)
            {
                for (int X = 0; X < W; X++)

                {
                    int IOffs = (Y * W + X) * 16;
                    byte[] Red = new byte[8];
                    byte[] Green = new byte[8];

                    Red[0] = data[IOffs + 0];
                    Red[1] = data[IOffs + 1];

                    Green[0] = data[IOffs + 8];
                    Green[1] = data[IOffs + 9];

                    if (IsSNORM == true)
                    {
                        CalculateBC3AlphaS(Red);
                        CalculateBC3AlphaS(Green);
                    }
                    else
                    {
                        CalculateBC3Alpha(Red);
                        CalculateBC3Alpha(Green);
                    }

                    int RedLow = Get32(data, IOffs + 2);
                    int RedHigh = Get16(data, IOffs + 6);

                    int GreenLow = Get32(data, IOffs + 10);
                    int GreenHigh = Get16(data, IOffs + 14);

                    ulong RedCh = (uint)RedLow | (ulong)RedHigh << 32;
                    ulong GreenCh = (uint)GreenLow | (ulong)GreenHigh << 32;

                    int TW = Math.Min(width - X * 4, 4);
                    int TH = Math.Min(height - Y * 4, 4);


                    if (IsSNORM == true)
                    {
                        for (int TY = 0; TY < TH; TY++)
                        {
                            for (int TX = 0; TX < TW; TX++)
                            {

                                int Shift = TY * 12 + TX * 3;
                                int OOffset = ((Y * 4 + TY) * width + (X * 4 + TX)) * 4;

                                byte RedPx = Red[(RedCh >> Shift) & 7];
                                byte GreenPx = Green[(GreenCh >> Shift) & 7];

                                if (IsSNORM == true)
                                {
                                    RedPx += 0x80;
                                    GreenPx += 0x80;
                                }

                                float NX = (RedPx / 255f) * 2 - 1;
                                float NY = (GreenPx / 255f) * 2 - 1;
                                float NZ = (float)Math.Sqrt(1 - (NX * NX + NY * NY));

                                Output[OOffset + 0] = Clamp((NZ + 1) * 0.5f);
                                Output[OOffset + 1] = Clamp((NY + 1) * 0.5f);
                                Output[OOffset + 2] = Clamp((NX + 1) * 0.5f);
                                Output[OOffset + 3] = 0xff;
                            }
                        }
                    }
                    else
                    {
                        for (int TY = 0; TY < TH; TY++)
                        {
                            for (int TX = 0; TX < TW; TX++)
                            {

                                int Shift = TY * 12 + TX * 3;
                                int OOffset = ((Y * 4 + TY) * width + (X * 4 + TX)) * 4;

                                byte RedPx = Red[(RedCh >> Shift) & 7];
                                byte GreenPx = Green[(GreenCh >> Shift) & 7];

                                Output[OOffset + 0] = 255;
                                Output[OOffset + 1] = GreenPx;
                                Output[OOffset + 2] = RedPx;
                                Output[OOffset + 3] = 255;

                            }
                        }
                    }
                }
            }

            return BitmapExtension.GetBitmap(Output, W * 4, H * 4);
        }

        public static unsafe byte[] CompressBlock(Byte[] data, int width, int height, DDS.DXGI_FORMAT format, bool multiThread, float AlphaRef = 0.5f, STCompressionMode CompressionMode = STCompressionMode.Normal)
        {
            long inputRowPitch = width * 4;
            long inputSlicePitch = width * height * 4;

            if (data.Length == inputSlicePitch)
            {
                byte* buf;
                buf = (byte*)Marshal.AllocHGlobal((int)inputSlicePitch);
                Marshal.Copy(data, 0, (IntPtr)buf, (int)inputSlicePitch);

                DirectXTexNet.Image inputImage = new DirectXTexNet.Image(
                    width, height, DXGI_FORMAT.R8G8B8A8_UNORM, inputRowPitch,
                    inputSlicePitch, (IntPtr)buf, null);

                TexMetadata texMetadata = new TexMetadata(width, height, 1, 1, 1, 0, 0,
                    DXGI_FORMAT.R8G8B8A8_UNORM, TEX_DIMENSION.TEXTURE2D);

                ScratchImage scratchImage = TexHelper.Instance.InitializeTemporary(
                    new DirectXTexNet.Image[] { inputImage }, texMetadata, null);

                var compFlags = TEX_COMPRESS_FLAGS.DEFAULT;

                if (multiThread)
                    compFlags |= TEX_COMPRESS_FLAGS.PARALLEL;

                if (format == DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM ||
                    format == DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB ||
                    format == DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS)
                {
                    if (CompressionMode == STCompressionMode.Fast)
                        compFlags |= TEX_COMPRESS_FLAGS.BC7_QUICK;
                }

                if (format == DDS.DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB ||
                format == DDS.DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB ||
                format == DDS.DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB ||
                format == DDS.DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB ||
                format == DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB)
                {
                    compFlags |= TEX_COMPRESS_FLAGS.SRGB;
                }

                using (var comp = scratchImage.Compress((DXGI_FORMAT)format, compFlags, 0.5f))
                {
                    long outRowPitch;
                    long outSlicePitch;
                    TexHelper.Instance.ComputePitch((DXGI_FORMAT)format, width, height, out outRowPitch, out outSlicePitch, CP_FLAGS.NONE);

                    byte[] result = new byte[outSlicePitch];
                    Marshal.Copy(comp.GetImage(0).Pixels, result, 0, result.Length);

                    inputImage = null;
                    scratchImage.Dispose();


                    return result;
                }
            }
            return null;
        }
        public static unsafe byte[] DecompressBlock(Byte[] data, int width, int height, DDS.DXGI_FORMAT format)
        {
            Console.WriteLine(format);
            Console.WriteLine(width);
            Console.WriteLine(height);

            long inputRowPitch;
            long inputSlicePitch;
            TexHelper.Instance.ComputePitch((DXGI_FORMAT)format, width, height, out inputRowPitch, out inputSlicePitch, CP_FLAGS.NONE);

            DXGI_FORMAT FormatDecompressed;

            if (format.ToString().Contains("SRGB"))
                FormatDecompressed = DXGI_FORMAT.R8G8B8A8_UNORM_SRGB;
            else
                FormatDecompressed = DXGI_FORMAT.R8G8B8A8_UNORM;

            byte* buf;
            buf = (byte*)Marshal.AllocHGlobal((int)inputSlicePitch);
            Marshal.Copy(data, 0, (IntPtr)buf, (int)inputSlicePitch);

            DirectXTexNet.Image inputImage = new DirectXTexNet.Image(
                width, height, (DXGI_FORMAT)format, inputRowPitch,
                inputSlicePitch, (IntPtr)buf, null);

            TexMetadata texMetadata = new TexMetadata(width, height, 1, 1, 1, 0, 0,
                (DXGI_FORMAT)format, TEX_DIMENSION.TEXTURE2D);

            ScratchImage scratchImage = TexHelper.Instance.InitializeTemporary(
                new DirectXTexNet.Image[] { inputImage }, texMetadata, null);

            using (var decomp = scratchImage.Decompress(0, FormatDecompressed))
            {
                byte[] result = new byte[4 * width * height];
                Marshal.Copy(decomp.GetImage(0).Pixels, result, 0, result.Length);

                inputImage = null;
                scratchImage.Dispose();

                return result;
            }
        }
        public static unsafe byte[] DecodePixelBlock(Byte[] data, int width, int height, DDS.DXGI_FORMAT format, float AlphaRef = 0.5f)
        {
            if (format == DDS.DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM)
            {
                byte[] result = new byte[data.Length];
                Array.Copy(data, result, data.Length);
                return result;
            }

            return Convert(data, width, height, (DXGI_FORMAT)format, DXGI_FORMAT.R8G8B8A8_UNORM);
        }
        public static unsafe byte[] EncodePixelBlock(Byte[] data, int width, int height, DDS.DXGI_FORMAT format, float AlphaRef = 0.5f)
        {
            if (format == DDS.DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM || format == DDS.DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB)
                return data;

            return Convert(data, width, height, DXGI_FORMAT.R8G8B8A8_UNORM, (DXGI_FORMAT)format);
        }

        public static unsafe byte[] Convert(Byte[] data, int width, int height, DXGI_FORMAT inputFormat, DXGI_FORMAT outputFormat)
        {
            long inputRowPitch;
            long inputSlicePitch;
            TexHelper.Instance.ComputePitch(inputFormat, width, height, out inputRowPitch, out inputSlicePitch, CP_FLAGS.NONE);

            if (data.Length == inputSlicePitch)
            {
                byte* buf;
                buf = (byte*)Marshal.AllocHGlobal((int)inputSlicePitch);
                Marshal.Copy(data, 0, (IntPtr)buf, (int)inputSlicePitch);

                DirectXTexNet.Image inputImage = new DirectXTexNet.Image(
                    width, height, inputFormat, inputRowPitch,
                    inputSlicePitch, (IntPtr)buf, null);

                TexMetadata texMetadata = new TexMetadata(width, height, 1, 1, 1, 0, 0,
                    inputFormat, TEX_DIMENSION.TEXTURE2D);

                ScratchImage scratchImage = TexHelper.Instance.InitializeTemporary(
                    new DirectXTexNet.Image[] { inputImage }, texMetadata, null);

                var convFlags = TEX_FILTER_FLAGS.DEFAULT;

                if (inputFormat == DXGI_FORMAT.B8G8R8A8_UNORM_SRGB ||
                 inputFormat == DXGI_FORMAT.B8G8R8X8_UNORM_SRGB ||
                 inputFormat == DXGI_FORMAT.R8G8B8A8_UNORM_SRGB)
                {
                    convFlags |= TEX_FILTER_FLAGS.SRGB;
                }

                using (var decomp = scratchImage.Convert(0, outputFormat, convFlags, 0.5f))
                {
                    long outRowPitch;
                    long outSlicePitch;
                    TexHelper.Instance.ComputePitch(outputFormat, width, height, out outRowPitch, out outSlicePitch, CP_FLAGS.NONE);

                    byte[] result = new byte[outSlicePitch];
                    Marshal.Copy(decomp.GetImage(0).Pixels, result, 0, result.Length);

                    inputImage = null;
                    scratchImage.Dispose();


                    return result;
                }
            }
            return null;
        }
        /*    public static byte[] CompressBlock(Byte[] data, int width, int height, DDS.DXGI_FORMAT format, float AlphaRef)
            {
                return DirectXTex.ImageCompressor.Compress(data, width, height, (int)format, AlphaRef);
            }*/

        public static byte[] DecompressCompLibBlock(Byte[] data, int width, int height, DDS.DXGI_FORMAT format)
        {
            byte[] output = null;

            switch (format)
            {
                case DDS.DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
                case DDS.DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
                    output = CSharpImageLibrary.DDS.Dxt.DecompressDxt1(data, (int)width, (int)height);
                    break;
                case DDS.DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
                case DDS.DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
                    output = CSharpImageLibrary.DDS.Dxt.DecompressDxt5(data, (int)width, (int)height);
                    break;
                case DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
                case DDS.DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
                    output = CSharpImageLibrary.DDS.Dxt.DecompressDxt4(data, (int)width, (int)height);
                    break;
                case DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
                case DDS.DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
                    output = CSharpImageLibrary.DDS.Dxt.DecompressDxt4(data, (int)width, (int)height);
                    break;
                case DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
                case DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                    output = CSharpImageLibrary.DDS.Dxt.DecompressBc7(data, (int)width, (int)height);
                    break;
                default:
                    output = DecompressBlock(data, width, height, format);
                        break;

            }

            return output;
        }

        public static Bitmap DecompressCompLibBlock(Byte[] data, int width, int height, DDS.DXGI_FORMAT format, bool GetBitmap)
        {
            return BitmapExtension.GetBitmap(DecompressBlock(data, width, height, format), (int)width, (int)height);
        }

        public static int Get16(byte[] Data, int Address)
        {
            return
                Data[Address + 0] << 0 |
                Data[Address + 1] << 8;
        }

        public static int Get32(byte[] Data, int Address)
        {
            return
                Data[Address + 0] << 0 |
                Data[Address + 1] << 8 |
                Data[Address + 2] << 16 |
                Data[Address + 3] << 24;
        }

        private static byte Clamp(float Value)
        {
            if (Value > 1)
            {
                return 0xff;
            }
            else if (Value < 0)
            {
                return 0;
            }
            else
            {
                return (byte)(Value * 0xff);
            }
        }

        private static void CalculateBC3Alpha(byte[] Alpha)
        {
            if (Alpha[0] > Alpha[1])
            {
                for (int i = 2; i < 8; i++)
                    Alpha[i] = (byte)(Alpha[0] + ((Alpha[1] - Alpha[0]) * (i - 1)) / 7);
            }
            else
            {
                for (int i = 2; i < 6; i++)
                    Alpha[i] = (byte)(Alpha[0] + ((Alpha[1] - Alpha[0]) * (i - 1)) / 5);
                Alpha[6] = 0;
                Alpha[7] = 255;
            }
        }
        private static void CalculateBC3AlphaS(byte[] Alpha)
        {
            if ((sbyte)Alpha[0] > (sbyte)Alpha[1])
            {
                for (int i = 2; i < 8; i++)
                    Alpha[i] = (byte)(Alpha[0] + (((sbyte)Alpha[1] - (sbyte)Alpha[0]) * (i - 1)) / 7);
            }
            else
            {
                for (int i = 2; i < 6; i++)
                    Alpha[i] = (byte)(Alpha[0] + (((sbyte)Alpha[1] - (sbyte)Alpha[0]) * (i - 1)) / 5);
                Alpha[6] = 0x80;
                Alpha[7] = 0x7f;
            }
        }
    }
}
