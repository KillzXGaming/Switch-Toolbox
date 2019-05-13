using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switch_Toolbox.Library
{
    public class CTR_3DS
    {
        //From https://github.com/gdkchan/SPICA/blob/42c4181e198b0fd34f0a567345ee7e75b54cb58b/SPICA/PICA/Converters/TextureConverter.cs

        public enum PICASurfaceFormat
        {
            RGBA8,
            RGB8,
            RGBA5551,
            RGB565,
            RGBA4,
            LA8,
            HiLo8,
            L8,
            A8,
            LA4,
            L4,
            A4,
            ETC1,
            ETC1A4
        }

        public static TEX_FORMAT ConvertPICAToGenericFormat(PICASurfaceFormat format)
        {
             switch (format)
            {
                case PICASurfaceFormat.RGB565: return TEX_FORMAT.B5G6R5_UNORM;
                case PICASurfaceFormat.RGB8: return TEX_FORMAT.R8G8_UNORM;
                case PICASurfaceFormat.RGBA5551: return TEX_FORMAT.B5G5R5A1_UNORM;
                case PICASurfaceFormat.RGBA4: return TEX_FORMAT.B4G4R4A4_UNORM;
                case PICASurfaceFormat.LA8: return TEX_FORMAT.LA8;
                case PICASurfaceFormat.HiLo8: return TEX_FORMAT.HIL08;
                case PICASurfaceFormat.L8: return TEX_FORMAT.L8;
                case PICASurfaceFormat.A8: return TEX_FORMAT.A8_UNORM;
                case PICASurfaceFormat.LA4: return TEX_FORMAT.LA4;
                case PICASurfaceFormat.L4: return TEX_FORMAT.L4;
                case PICASurfaceFormat.A4: return TEX_FORMAT.A4;
                case PICASurfaceFormat.ETC1: return TEX_FORMAT.ETC1;
                case PICASurfaceFormat.ETC1A4: return TEX_FORMAT.ETC1_A4;
                default:
                    throw new NotImplementedException("Unsupported format! " + format);
            }
        }

        public static PICASurfaceFormat ConvertToPICAFormat(TEX_FORMAT GenericFormat)
        {
            switch (GenericFormat)
            {
                case TEX_FORMAT.B5G6R5_UNORM: return PICASurfaceFormat.RGB565;
                case TEX_FORMAT.R8G8_UNORM: return PICASurfaceFormat.RGB8;
                case TEX_FORMAT.B5G5R5A1_UNORM: return PICASurfaceFormat.RGBA5551;
                case TEX_FORMAT.B4G4R4A4_UNORM: return PICASurfaceFormat.RGBA4;
                case TEX_FORMAT.LA8: return PICASurfaceFormat.LA8;
                case TEX_FORMAT.HIL08: return PICASurfaceFormat.HiLo8;
                case TEX_FORMAT.L8: return PICASurfaceFormat.L8;
                case TEX_FORMAT.A8_UNORM: return PICASurfaceFormat.A8;
                case TEX_FORMAT.LA4: return PICASurfaceFormat.LA4;
                case TEX_FORMAT.A4: return PICASurfaceFormat.A4;
                case TEX_FORMAT.ETC1: return PICASurfaceFormat.ETC1;
                case TEX_FORMAT.ETC1_A4: return PICASurfaceFormat.ETC1A4;
                default:
                    throw new NotImplementedException("Unsupported format! " + GenericFormat);
            }
        }

        private static int[] FmtBPP = new int[] { 32, 24, 16, 16, 16, 16, 16, 8, 8, 8, 4, 4, 4, 8 };

        public static int[] SwizzleLUT =
{
             0,  1,  8,  9,  2,  3, 10, 11,
            16, 17, 24, 25, 18, 19, 26, 27,
             4,  5, 12, 13,  6,  7, 14, 15,
            20, 21, 28, 29, 22, 23, 30, 31,
            32, 33, 40, 41, 34, 35, 42, 43,
            48, 49, 56, 57, 50, 51, 58, 59,
            36, 37, 44, 45, 38, 39, 46, 47,
            52, 53, 60, 61, 54, 55, 62, 63
        };

        public static byte[] DecodeBlock(byte[] Input, int Width, int Height, TEX_FORMAT Format)
        {
            if (Format == TEX_FORMAT.ETC1 || Format == TEX_FORMAT.ETC1_A4)
                return ETC1.ETC1Decompress(Input, Width, Height, Format == TEX_FORMAT.ETC1_A4);

            byte[] Output = new byte[Width * Height * 4];

            int Increment = FmtBPP[(int)ConvertToPICAFormat(Format)] / 8;
            if (Increment == 0) Increment = 1;

            int IOffset = 0;

            for (int TY = 0; TY < Height; TY += 8)
            {
                for (int TX = 0; TX < Width; TX += 8)
                {
                    for (int Px = 0; Px < 64; Px++)
                    {
                        int X = SwizzleLUT[Px] & 7;
                        int Y = (SwizzleLUT[Px] - X) >> 3;

                        int OOffet = (TX + X + ((Height - 1 - (TY + Y)) * Width)) * 4;

                        switch (Format)
                        {
                            case TEX_FORMAT.R8G8_UNORM:
                                Output[OOffet + 0] = Input[IOffset + 3];
                                Output[OOffet + 1] = Input[IOffset + 2];
                                Output[OOffet + 2] = Input[IOffset + 1];
                                Output[OOffet + 3] = Input[IOffset + 0];
                                break;
                            case TEX_FORMAT.R8G8B8A8_UNORM:
                                Output[OOffet + 0] = Input[IOffset + 2];
                                Output[OOffet + 1] = Input[IOffset + 1];
                                Output[OOffet + 2] = Input[IOffset + 0];
                                Output[OOffet + 3] = 0xff;
                                break;
                            case TEX_FORMAT.B5G5R5A1_UNORM:
                                DecodeRGBA5551(Output, OOffet, GetUShort(Input, IOffset));
                                break;
                            case TEX_FORMAT.B5G6R5_UNORM:
                                DecodeRGB565(Output, OOffet, GetUShort(Input, IOffset));
                                break;
                            case TEX_FORMAT.B4G4R4A4_UNORM:
                                DecodeRGBA4(Output, OOffet, GetUShort(Input, IOffset));
                                break;
                            case TEX_FORMAT.LA8:
                                Output[OOffet + 0] = Input[IOffset + 1];
                                Output[OOffet + 1] = Input[IOffset + 1];
                                Output[OOffet + 2] = Input[IOffset + 1];
                                Output[OOffet + 3] = Input[IOffset + 0];
                                break;
                            case TEX_FORMAT.HIL08:
                                Output[OOffet + 0] = Input[IOffset + 1];
                                Output[OOffet + 1] = Input[IOffset + 0];
                                Output[OOffet + 2] = 0;
                                Output[OOffet + 3] = 0xff;
                                break;
                            case TEX_FORMAT.L8:
                                Output[OOffet + 0] = Input[IOffset];
                                Output[OOffet + 1] = Input[IOffset];
                                Output[OOffet + 2] = Input[IOffset];
                                Output[OOffet + 3] = 0xff;
                                break;

                            case TEX_FORMAT.A8_UNORM:
                                Output[OOffet + 0] = 0xff;
                                Output[OOffet + 1] = 0xff;
                                Output[OOffet + 2] = 0xff;
                                Output[OOffet + 3] = Input[IOffset];
                                break;
                            case TEX_FORMAT.LA4:
                                Output[OOffet + 0] = (byte)((Input[IOffset] >> 4) | (Input[IOffset] & 0xf0));
                                Output[OOffet + 1] = (byte)((Input[IOffset] >> 4) | (Input[IOffset] & 0xf0));
                                Output[OOffet + 2] = (byte)((Input[IOffset] >> 4) | (Input[IOffset] & 0xf0));
                                Output[OOffet + 3] = (byte)((Input[IOffset] << 4) | (Input[IOffset] & 0x0f));
                                break;
                            case TEX_FORMAT.L4:
                                int L = (Input[IOffset >> 1] >> ((IOffset & 1) << 2)) & 0xf;
                                Output[OOffet + 0] = (byte)((L << 4) | L);
                                Output[OOffet + 1] = (byte)((L << 4) | L);
                                Output[OOffet + 2] = (byte)((L << 4) | L);
                                Output[OOffet + 3] = 0xff;
                                break;
                            case TEX_FORMAT.A4:
                                int A = (Input[IOffset >> 1] >> ((IOffset & 1) << 2)) & 0xf;

                                Output[OOffet + 0] = 0xff;
                                Output[OOffet + 1] = 0xff;
                                Output[OOffet + 2] = 0xff;
                                Output[OOffet + 3] = (byte)((A << 4) | A);

                                break;
                        }

                        Output[OOffet + 0] = 0xff;
                        Output[OOffet + 1] = 0xff;
                        Output[OOffet + 2] = 0xff;
                        Output[OOffet + 3] = Input[IOffset];

                        IOffset += Increment;
                    }
                }
            }

            return Output;
        }

        private static void DecodeRGB565(byte[] Buffer, int Address, ushort Value)
        {
            int R = ((Value >> 0) & 0x1f) << 3;
            int G = ((Value >> 5) & 0x3f) << 2;
            int B = ((Value >> 11) & 0x1f) << 3;

            SetColor(Buffer, Address, 0xff,
                B | (B >> 5),
                G | (G >> 6),
                R | (R >> 5));
        }

        private static void DecodeRGBA4(byte[] Buffer, int Address, ushort Value)
        {
            int R = (Value >> 4) & 0xf;
            int G = (Value >> 8) & 0xf;
            int B = (Value >> 12) & 0xf;

            SetColor(Buffer, Address, (Value & 0xf) | (Value << 4),
                B | (B << 4),
                G | (G << 4),
                R | (R << 4));
        }

        private static void DecodeRGBA5551(byte[] Buffer, int Address, ushort Value)
        {
            int R = ((Value >> 1) & 0x1f) << 3;
            int G = ((Value >> 6) & 0x1f) << 3;
            int B = ((Value >> 11) & 0x1f) << 3;

            SetColor(Buffer, Address, (Value & 1) * 0xff,
                B | (B >> 5),
                G | (G >> 5),
                R | (R >> 5));
        }

        private static void SetColor(byte[] Buffer, int Address, int A, int B, int G, int R)
        {
            Buffer[Address + 0] = (byte)B;
            Buffer[Address + 1] = (byte)G;
            Buffer[Address + 2] = (byte)R;
            Buffer[Address + 3] = (byte)A;
        }

        private static ushort GetUShort(byte[] Buffer, int Address)
        {
            return (ushort)(
                Buffer[Address + 0] << 0 |
                Buffer[Address + 1] << 8);
        }
    }
}
