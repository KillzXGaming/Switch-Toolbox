// https://github.com/KFreon/CSharpImageLibrary

// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CSharpImageLibrary.DDS
{
    public static class BC6
    {
        static readonly int[] ModeToInfo = { 0, 1, 2, 10, -1, -1, 3, 11, -1, -1, 4, 12, -1, -1, 5, 13, -1, -1, 6, -1, -1, -1, 7, -1, -1, -1, 8, -1, -1, -1, 9, -1 };

        const int BC6H_MAX_REGIONS = 2;
        const ushort HALF_FLOAT_MASK = 32768;

        enum EField
        {
            NA,
            M,
            D,
            RW,
            RX,
            RY,
            RZ,
            GW,
            GX,
            GY,
            GZ,
            BW,
            BX,
            BY,
            BZ
        }

        #region Structs
        struct ModeDescriptor
        {
            public int m_uBit;
            public EField eField;

            public ModeDescriptor(EField field, int bit)
            {
                eField = field;
                m_uBit = bit;
            }
        }

        struct ModeInfo
        {
            public int Partitions;
            public bool Transformed;
            public int IndexPrecision;
            public DX10_Helpers.LDRColour[][] RGBAPrec;  // [BC6 max regions][2]

            public ModeInfo(int partitions, bool transformed, int indexPrecision, DX10_Helpers.LDRColour[] first, DX10_Helpers.LDRColour[] second)
            {
                Partitions = partitions;
                Transformed = transformed;
                IndexPrecision = indexPrecision;
                RGBAPrec = new[] { first, second };
            }
        }

        static INTColour SignExtend(INTColour c, DX10_Helpers.LDRColour prec)
        {
            return new INTColour
            {
                R = SignExtend(c.R, prec.R),
                G = SignExtend(c.G, prec.G),
                B = SignExtend(c.B, prec.B)
            };
        }

        static int SignExtend(int colour, int precision)
        {
            return ((colour & (1 << (precision - 1))) != 0 ? (~0 << precision) : 0) | colour;
        }

        internal struct INTColour
        {
            public int R, G, B, Pad;

            public INTColour(int nr, int ng, int nb)
            {
                R = nr;
                G = ng;
                B = nb;
                Pad = 0;
            }

            public override string ToString()
            {
                return $"R: {R} G: {G} B: {B}, Pad: {Pad}";
            }

            public static INTColour operator +(INTColour first, INTColour second)
            {
                return new INTColour(first.R + second.R, first.G + second.G, first.B + second.B);
            }

            public static INTColour operator &(INTColour first, INTColour second)
            {
                return new INTColour(first.R & second.R, first.G & second.G, first.B & second.B);
            }

            internal DX10_Helpers.LDRColour ToLDRColour(bool isSigned)
            {
                var r = IntToFloatIsh(R, isSigned);
                var g = IntToFloatIsh(G, isSigned);
                var b = IntToFloatIsh(B, isSigned);

                DX10_Helpers.LDRColour colour = new DX10_Helpers.LDRColour
                {
                    R = ScRgbTosRgb(r),
                    G = ScRgbTosRgb(g),
                    B = ScRgbTosRgb(b)
                };

                return colour;
            }

            private static byte ScRgbTosRgb(float val)
            {
                if (!(val > 0.0))       // Handles NaN case too
                {
                    return (0);
                }

                if (val <= 0.0031308)
                {
                    return ((byte)((255.0f * val * 12.92f) + 0.5f));
                }

                if (val < 1.0)
                {
                    return ((byte)((255.0f * ((1.055f * (float)Math.Pow(val, (1.0 / 2.4))) - 0.055f)) + 0.5f));
                }

                return (255);
            }

            static float IntToFloatIsh(int input, bool isSigned)
            {
                ushort outVal;
                if (isSigned)
                {
                    ushort s = 0;
                    if (input < 0)
                    {
                        s = HALF_FLOAT_MASK;
                        input *= -1;
                    }

                    outVal = (ushort)(s | input);
                }
                else
                {
                    outVal = (ushort)input;
                }

                // outVal is a 'half float' now apparently

                return HalfFloatToFloat(outVal);
            }

            const uint FloatMantissasMask = 0x03FF;
            const uint FloatExponentMask = 0x7C00;
            const uint FloatSignMask = 0x8000;

            static float HalfFloatToFloat(ushort halfFloat)
            {
                uint mantissa = halfFloat & FloatMantissasMask;
                uint exponent = halfFloat & FloatExponentMask;
                if (exponent == FloatExponentMask)  // Inf/NAN
                    exponent = 0x8f;
                else if (exponent != 0)  // normalised
                    exponent = (uint)((halfFloat >> 10) & 0x1F);
                else if (mantissa != 0)  // denormalised
                {
                    // Normalise
                    exponent = 1;
                    do
                    {
                        exponent--;
                        mantissa <<= 1;
                    } while ((mantissa & 0x0400) == 0);

                    mantissa &= 0x03FF;
                }
                else  // == 0
                {
                    unchecked
                    {
                        exponent = (uint)(-112);
                    }
                }

                uint longResult = ((halfFloat & FloatSignMask) << 16) | // Sign
                                ((exponent + 112) << 23) |  // Exponent
                                (mantissa << 13);  // Mantissa


                // Reinterpret cast
                var bytes = BitConverter.GetBytes(longResult);
                return BitConverter.ToSingle(bytes, 0);
            }
        }

        internal struct INTColourPair
        {
            public INTColour A;
            public INTColour B;
        }
        #endregion Structs


        #region Tables
        static readonly List<List<ModeDescriptor>> ms_aDesc = new List<List<ModeDescriptor>>
        {
            // Mode 1 (0x00) - 10 5 5 5
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4)
            },

            // Mode 2 (0x01) - 7 6 6 6
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor(EField.GY, 5), new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.GZ, 5), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.BY, 5), new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor(EField.BZ, 5), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RX, 5), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GX, 5), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BX, 5), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.RY, 5), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.RZ, 5), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4)
            },
            
            // Mode 3 (0x02) - 11 5 4 4
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RW,10), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GW,10),
                new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BW,10),
                new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4)
            },

            // Mode 4 (0x06) - 11 4 5 4
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RW,10),
                new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GW,10), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BW,10),
                new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.BZ, 0),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4)
            },

            // Mode 5 (0x0a) - 11 4 4 5
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RW,10),
                new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GW,10),
                new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BW,10), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.BZ, 1),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4)
            },

            // Mode 6 (0x0e) - 9 5 5 5
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4)
            },

            // Mode 7 (0x12) - 8 6 5 5
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RX, 5), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.RY, 5), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.RZ, 5), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4)
            },

            // Mode 8 (0x16) - 8 5 6 5
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GY, 5), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.GZ, 5), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GX, 5), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4)
            },

            // Mode 9 (0x1a) - 8 5 5 6
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.BY, 5), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BZ, 5), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BX, 5), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4)
            },

            // Mode 10 (0x1e) - 6 6 6 6
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.GZ, 4), new ModeDescriptor(EField.BZ, 0), new ModeDescriptor(EField.BZ, 1), new ModeDescriptor(EField.BY, 4), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GY, 5), new ModeDescriptor(EField.BY, 5), new ModeDescriptor(EField.BZ, 2), new ModeDescriptor(EField.GY, 4), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.GZ, 5), new ModeDescriptor(EField.BZ, 3), new ModeDescriptor(EField.BZ, 5), new ModeDescriptor(EField.BZ, 4), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RX, 5), new ModeDescriptor(EField.GY, 0), new ModeDescriptor(EField.GY, 1), new ModeDescriptor(EField.GY, 2), new ModeDescriptor(EField.GY, 3), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GX, 5), new ModeDescriptor(EField.GZ, 0), new ModeDescriptor(EField.GZ, 1), new ModeDescriptor(EField.GZ, 2), new ModeDescriptor(EField.GZ, 3), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BX, 5), new ModeDescriptor(EField.BY, 0), new ModeDescriptor(EField.BY, 1), new ModeDescriptor(EField.BY, 2), new ModeDescriptor(EField.BY, 3), new ModeDescriptor(EField.RY, 0), new ModeDescriptor(EField.RY, 1), new ModeDescriptor(EField.RY, 2), new ModeDescriptor(EField.RY, 3), new ModeDescriptor(EField.RY, 4),
                new ModeDescriptor(EField.RY, 5), new ModeDescriptor(EField.RZ, 0), new ModeDescriptor(EField.RZ, 1), new ModeDescriptor(EField.RZ, 2), new ModeDescriptor(EField.RZ, 3), new ModeDescriptor(EField.RZ, 4), new ModeDescriptor(EField.RZ, 5), new ModeDescriptor( EField.D, 0), new ModeDescriptor( EField.D, 1), new ModeDescriptor( EField.D, 2),
                new ModeDescriptor( EField.D, 3), new ModeDescriptor( EField.D, 4)
            },

            // Mode 11 (0x03) - 10 10
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RX, 5), new ModeDescriptor(EField.RX, 6), new ModeDescriptor(EField.RX, 7), new ModeDescriptor(EField.RX, 8), new ModeDescriptor(EField.RX, 9), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GX, 5), new ModeDescriptor(EField.GX, 6), new ModeDescriptor(EField.GX, 7), new ModeDescriptor(EField.GX, 8), new ModeDescriptor(EField.GX, 9), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BX, 5), new ModeDescriptor(EField.BX, 6), new ModeDescriptor(EField.BX, 7), new ModeDescriptor(EField.BX, 8), new ModeDescriptor(EField.BX, 9), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0)
            },

            // Mode 12 (0x07) - 11 9
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RX, 5), new ModeDescriptor(EField.RX, 6), new ModeDescriptor(EField.RX, 7), new ModeDescriptor(EField.RX, 8), new ModeDescriptor(EField.RW,10), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GX, 5), new ModeDescriptor(EField.GX, 6), new ModeDescriptor(EField.GX, 7), new ModeDescriptor(EField.GX, 8), new ModeDescriptor(EField.GW,10), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BX, 5), new ModeDescriptor(EField.BX, 6), new ModeDescriptor(EField.BX, 7), new ModeDescriptor(EField.BX, 8), new ModeDescriptor(EField.BW,10), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0)
            },

            // Mode 13 (0x0b) - 12 8
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RX, 4),
                new ModeDescriptor(EField.RX, 5), new ModeDescriptor(EField.RX, 6), new ModeDescriptor(EField.RX, 7), new ModeDescriptor(EField.RW,11), new ModeDescriptor(EField.RW,10), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GX, 4),
                new ModeDescriptor(EField.GX, 5), new ModeDescriptor(EField.GX, 6), new ModeDescriptor(EField.GX, 7), new ModeDescriptor(EField.GW,11), new ModeDescriptor(EField.GW,10), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BX, 4),
                new ModeDescriptor(EField.BX, 5), new ModeDescriptor(EField.BX, 6), new ModeDescriptor(EField.BX, 7), new ModeDescriptor(EField.BW,11), new ModeDescriptor(EField.BW,10), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0)
            },

            // Mode 14 (0x0f) - 16 4
            new List<ModeDescriptor>
            {
                new ModeDescriptor( EField.M, 0), new ModeDescriptor( EField.M, 1), new ModeDescriptor( EField.M, 2), new ModeDescriptor( EField.M, 3), new ModeDescriptor( EField.M, 4), new ModeDescriptor(EField.RW, 0), new ModeDescriptor(EField.RW, 1), new ModeDescriptor(EField.RW, 2), new ModeDescriptor(EField.RW, 3), new ModeDescriptor(EField.RW, 4),
                new ModeDescriptor(EField.RW, 5), new ModeDescriptor(EField.RW, 6), new ModeDescriptor(EField.RW, 7), new ModeDescriptor(EField.RW, 8), new ModeDescriptor(EField.RW, 9), new ModeDescriptor(EField.GW, 0), new ModeDescriptor(EField.GW, 1), new ModeDescriptor(EField.GW, 2), new ModeDescriptor(EField.GW, 3), new ModeDescriptor(EField.GW, 4),
                new ModeDescriptor(EField.GW, 5), new ModeDescriptor(EField.GW, 6), new ModeDescriptor(EField.GW, 7), new ModeDescriptor(EField.GW, 8), new ModeDescriptor(EField.GW, 9), new ModeDescriptor(EField.BW, 0), new ModeDescriptor(EField.BW, 1), new ModeDescriptor(EField.BW, 2), new ModeDescriptor(EField.BW, 3), new ModeDescriptor(EField.BW, 4),
                new ModeDescriptor(EField.BW, 5), new ModeDescriptor(EField.BW, 6), new ModeDescriptor(EField.BW, 7), new ModeDescriptor(EField.BW, 8), new ModeDescriptor(EField.BW, 9), new ModeDescriptor(EField.RX, 0), new ModeDescriptor(EField.RX, 1), new ModeDescriptor(EField.RX, 2), new ModeDescriptor(EField.RX, 3), new ModeDescriptor(EField.RW,15),
                new ModeDescriptor(EField.RW,14), new ModeDescriptor(EField.RW,13), new ModeDescriptor(EField.RW,12), new ModeDescriptor(EField.RW,11), new ModeDescriptor(EField.RW,10), new ModeDescriptor(EField.GX, 0), new ModeDescriptor(EField.GX, 1), new ModeDescriptor(EField.GX, 2), new ModeDescriptor(EField.GX, 3), new ModeDescriptor(EField.GW,15),
                new ModeDescriptor(EField.GW,14), new ModeDescriptor(EField.GW,13), new ModeDescriptor(EField.GW,12), new ModeDescriptor(EField.GW,11), new ModeDescriptor(EField.GW,10), new ModeDescriptor(EField.BX, 0), new ModeDescriptor(EField.BX, 1), new ModeDescriptor(EField.BX, 2), new ModeDescriptor(EField.BX, 3), new ModeDescriptor(EField.BW,15),
                new ModeDescriptor(EField.BW,14), new ModeDescriptor(EField.BW,13), new ModeDescriptor(EField.BW,12), new ModeDescriptor(EField.BW,11), new ModeDescriptor(EField.BW,10), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0),
                new ModeDescriptor(EField.NA, 0), new ModeDescriptor(EField.NA, 0)
            }
        };

        static readonly ModeInfo[] ms_aInfo = {
            new ModeInfo(1, true,  3, new[] { new DX10_Helpers.LDRColour(10,10,10,0), new DX10_Helpers.LDRColour( 5, 5, 5,0) },    new[] { new DX10_Helpers.LDRColour( 5, 5, 5,0), new DX10_Helpers.LDRColour( 5, 5, 5,0) }),
            new ModeInfo(1, true,  3, new[] { new DX10_Helpers.LDRColour(7,7,7,0),    new DX10_Helpers.LDRColour( 6, 6, 6,0) },    new[] { new DX10_Helpers.LDRColour( 6, 6, 6,0), new DX10_Helpers.LDRColour( 6, 6, 6,0) }),
            new ModeInfo(1, true,  3, new[] { new DX10_Helpers.LDRColour(11,11,11,0), new DX10_Helpers.LDRColour( 5, 4, 4,0) },    new[] { new DX10_Helpers.LDRColour( 5, 4, 4,0), new DX10_Helpers.LDRColour( 5, 4, 4,0) }),
            new ModeInfo(1, true,  3, new[] { new DX10_Helpers.LDRColour(11,11,11,0), new DX10_Helpers.LDRColour( 4, 5, 4,0) },    new[] { new DX10_Helpers.LDRColour( 4, 5, 4,0), new DX10_Helpers.LDRColour( 4, 5, 4,0) }),
            new ModeInfo(1, true,  3, new[] { new DX10_Helpers.LDRColour(11,11,11,0), new DX10_Helpers.LDRColour( 4, 4, 5,0) },    new[] { new DX10_Helpers.LDRColour( 4, 4, 5,0), new DX10_Helpers.LDRColour( 4, 4, 5,0) }),
            new ModeInfo(1, true,  3, new[] { new DX10_Helpers.LDRColour(9,9,9,0),    new DX10_Helpers.LDRColour( 5, 5, 5,0) },    new[] { new DX10_Helpers.LDRColour( 5, 5, 5,0), new DX10_Helpers.LDRColour( 5, 5, 5,0) }),
            new ModeInfo(1, true,  3, new[] { new DX10_Helpers.LDRColour(8,8,8,0),    new DX10_Helpers.LDRColour( 6, 5, 5,0) },    new[] { new DX10_Helpers.LDRColour( 6, 5, 5,0), new DX10_Helpers.LDRColour( 6, 5, 5,0) }),
            new ModeInfo(1, true,  3, new[] { new DX10_Helpers.LDRColour(8,8,8,0),    new DX10_Helpers.LDRColour( 5, 6, 5,0) },    new[] { new DX10_Helpers.LDRColour( 5, 6, 5,0), new DX10_Helpers.LDRColour( 5, 6, 5,0) }),
            new ModeInfo(1, true,  3, new[] { new DX10_Helpers.LDRColour(8,8,8,0),    new DX10_Helpers.LDRColour( 5, 5, 6,0) },    new[] { new DX10_Helpers.LDRColour( 5, 5, 6,0), new DX10_Helpers.LDRColour( 5, 5, 6,0) }),
            new ModeInfo(1, false, 3, new[] { new DX10_Helpers.LDRColour(6,6,6,0),    new DX10_Helpers.LDRColour( 6, 6, 6,0) },    new[] { new DX10_Helpers.LDRColour( 6, 6, 6,0), new DX10_Helpers.LDRColour( 6, 6, 6,0) }),
            new ModeInfo(0, false, 4, new[] { new DX10_Helpers.LDRColour(10,10,10,0), new DX10_Helpers.LDRColour( 10, 10, 10,0) }, new[] { new DX10_Helpers.LDRColour( 0, 0, 0,0), new DX10_Helpers.LDRColour( 0, 0, 0,0) }),
            new ModeInfo(0, true,  4, new[] { new DX10_Helpers.LDRColour(11,11,11,0), new DX10_Helpers.LDRColour( 9, 9, 9,0) },    new[] { new DX10_Helpers.LDRColour( 0, 0, 0,0), new DX10_Helpers.LDRColour( 0, 0, 0,0) }),
            new ModeInfo(0, true,  4, new[] { new DX10_Helpers.LDRColour(12,12,12,0), new DX10_Helpers.LDRColour( 8, 8, 8,0) },    new[] { new DX10_Helpers.LDRColour( 0, 0, 0,0), new DX10_Helpers.LDRColour( 0, 0, 0,0) }),
            new ModeInfo(0, true,  4, new[] { new DX10_Helpers.LDRColour(16,16,16,0), new DX10_Helpers.LDRColour( 4,4, 4,0) },     new[] { new DX10_Helpers.LDRColour( 0,0, 0,0),  new DX10_Helpers.LDRColour( 0,0, 0,0) } )
        };
        #endregion Tables

        #region Decompression
        internal static DX10_Helpers.LDRColour[] DecompressBC6(byte[] source, int sourceStart, bool isSigned)
        {
            DX10_Helpers.LDRColour[] block = new DX10_Helpers.LDRColour[DX10_Helpers.NUM_PIXELS_PER_BLOCK];

            int startBit = 0;
            int mode = DX10_Helpers.GetBits(source, sourceStart, ref startBit, 2);
            if (mode != 0 && mode != 0x01)
                mode = (DX10_Helpers.GetBits(source, sourceStart, ref startBit, 3) << 2) | mode;


            if (ModeToInfo[mode] >= 0)
            {
                List<ModeDescriptor> desc = ms_aDesc[ModeToInfo[mode]];
                ModeInfo info = ms_aInfo[ModeToInfo[mode]];
                int shape = 0;
                INTColourPair[] endPoints = new INTColourPair[BC6H_MAX_REGIONS];

                // Header?
                int headerBits = info.Partitions > 0 ? 82 : 65;
                while (startBit < headerBits)
                {
                    int currBit = startBit;
                    if (DX10_Helpers.GetBit(source, sourceStart, ref startBit) != 0)
                    {
                        switch (desc[currBit].eField)
                        {
                            case EField.D: shape |= 1 << desc[currBit].m_uBit; break;
                            case EField.RW: endPoints[0].A.R |= 1 << desc[currBit].m_uBit; break;
                            case EField.RX: endPoints[0].B.R |= 1 << desc[currBit].m_uBit; break;
                            case EField.RY: endPoints[1].A.R |= 1 << desc[currBit].m_uBit; break;
                            case EField.RZ: endPoints[1].B.R |= 1 << desc[currBit].m_uBit; break;
                            case EField.GW: endPoints[0].A.G |= 1 << desc[currBit].m_uBit; break;
                            case EField.GX: endPoints[0].B.G |= 1 << desc[currBit].m_uBit; break;
                            case EField.GY: endPoints[1].A.G |= 1 << desc[currBit].m_uBit; break;
                            case EField.GZ: endPoints[1].B.G |= 1 << desc[currBit].m_uBit; break;
                            case EField.BW: endPoints[0].A.B |= 1 << desc[currBit].m_uBit; break;
                            case EField.BX: endPoints[0].B.B |= 1 << desc[currBit].m_uBit; break;
                            case EField.BY: endPoints[1].A.B |= 1 << desc[currBit].m_uBit; break;
                            case EField.BZ: endPoints[1].B.B |= 1 << desc[currBit].m_uBit; break;
                            default:
                                Debugger.Break();
                                break;
                        }
                    }
                }

                // Sign extend necessary end points
                if (isSigned)
                    endPoints[0].A = SignExtend(endPoints[0].A, info.RGBAPrec[0][0]);

                if (isSigned || info.Transformed)
                {
                    for (int p = 0; p <= info.Partitions; p++)
                    {
                        if (p != 0)
                            endPoints[p].A = SignExtend(endPoints[p].A, info.RGBAPrec[p][0]);

                        endPoints[p].B = SignExtend(endPoints[p].B, info.RGBAPrec[p][1]);
                    }
                }

                // Inverse transform end points
                if (info.Transformed)
                    TransformInverse(endPoints, info.RGBAPrec[0][0], isSigned);

                // Read indicies
                int prec = info.IndexPrecision;
                int partitions = info.Partitions;
                byte[] partTable = DX10_Helpers.PartitionTable[partitions][shape];

                for (int i = 0; i < DX10_Helpers.NUM_PIXELS_PER_BLOCK; i++)
                {
                    int numBits = DX10_Helpers.IsFixUpOffset(partitions, shape, i) ? prec - 1 : prec;
                    if (startBit + numBits > 128)
                        Debugger.Break();

                    int index = DX10_Helpers.GetBits(source, sourceStart, ref startBit, numBits);
                    if (index >= ((partitions > 0) ? 8 : 16))
                        Debugger.Break();

                    int region = partTable[i];

                    // Unquantise endpoints and interpolate
                    int r1 = Unquantise(endPoints[region].A.R, info.RGBAPrec[0][0].R, isSigned);
                    int g1 = Unquantise(endPoints[region].A.G, info.RGBAPrec[0][0].G, isSigned);
                    int b1 = Unquantise(endPoints[region].A.B, info.RGBAPrec[0][0].B, isSigned);
                    int r2 = Unquantise(endPoints[region].B.R, info.RGBAPrec[0][0].R, isSigned);
                    int g2 = Unquantise(endPoints[region].B.G, info.RGBAPrec[0][0].G, isSigned);
                    int b2 = Unquantise(endPoints[region].B.B, info.RGBAPrec[0][0].B, isSigned);

                    int[] aWeights = info.Partitions > 0 ? DX10_Helpers.AWeights3 : DX10_Helpers.AWeights4;
                    INTColour fc = new INTColour
                    {
                        R = FinishUnquantise((r1 * (DX10_Helpers.BC67_WEIGHT_MAX - aWeights[index]) + r2 * aWeights[index] + DX10_Helpers.BC67_WEIGHT_ROUND) >> DX10_Helpers.BC67_WEIGHT_SHIFT, isSigned),
                        G = FinishUnquantise((g1 * (DX10_Helpers.BC67_WEIGHT_MAX - aWeights[index]) + g2 * aWeights[index] + DX10_Helpers.BC67_WEIGHT_ROUND) >> DX10_Helpers.BC67_WEIGHT_SHIFT, isSigned),
                        B = FinishUnquantise((b1 * (DX10_Helpers.BC67_WEIGHT_MAX - aWeights[index]) + b2 * aWeights[index] + DX10_Helpers.BC67_WEIGHT_ROUND) >> DX10_Helpers.BC67_WEIGHT_SHIFT, isSigned)
                    };

                    DX10_Helpers.LDRColour colour = fc.ToLDRColour(isSigned);
                    colour.A = 255;
                    block[i] = colour;
                }
            }

            return block;
        }

        private static int FinishUnquantise(int comp, bool isSigned)
        {
            if (isSigned)
                return (comp < 0) ? -((-comp * 31) >> 5) : (comp * 31) >> 5;  // Scale magnitude by 31/32
            return (comp * 31) >> 6;  // Scale magnitude by 31/64
        }

        private static int Unquantise(int comp, int bitsPerComp, bool isSigned)
        {
            int unq, s = 0;
            if (isSigned)
            {
                if (bitsPerComp >= 16)
                    unq = comp;
                else
                {
                    if (comp < 0)
                    {
                        s = 1;
                        comp *= -1;
                    }

                    if (comp == 0)
                        unq = 0;
                    else if (comp >= ((1 << (bitsPerComp - 1)) - 1))
                        unq = 0x7FFF;
                    else
                        unq = ((comp << 15) + 0x4000) >> (bitsPerComp - 1);

                    if (s != 0)
                        unq *= -1;
                }
            }
            else
            {
                if (bitsPerComp >= 15)
                    unq = comp;
                else if (comp == 0)
                    unq = 0;
                else if (comp == ((1 << bitsPerComp) - 1))
                    unq = 0xFFFF;
                else
                    unq = ((comp << 16) + 0x8000) >> bitsPerComp;
            }

            return unq;
        }

        private static void TransformInverse(INTColourPair[] endPoints, DX10_Helpers.LDRColour prec, bool isSigned)
        {
            INTColour wrapMask = new INTColour((1 << prec.R) - 1, (1 << prec.G) - 1, (1 << prec.B) - 1);
            endPoints[0].B += endPoints[0].A;
            endPoints[0].B &= wrapMask;

            endPoints[1].A += endPoints[0].A;
            endPoints[1].A &= wrapMask;

            endPoints[1].B += endPoints[0].A;
            endPoints[1].B &= wrapMask;

            if (isSigned)
            {
                endPoints[0].B = SignExtend(endPoints[0].B, prec);
                endPoints[1].A = SignExtend(endPoints[1].A, prec);
                endPoints[1].B = SignExtend(endPoints[1].B, prec);
            }
        }
        #endregion Decompression
    }
}
