// https://github.com/KFreon/CSharpImageLibrary

// ReSharper disable InconsistentNaming

using System.Diagnostics;

namespace CSharpImageLibrary.DDS
{
    /// <summary>
    /// Adapted almost wholesale from DirectXTex from Microsoft. https://github.com/Microsoft/DirectXTex
    /// </summary>
    internal static class BC7
    {
        struct Mode
        {
            public readonly int Partitions;
            public readonly int PartitionBits;
            public readonly int IndexPrecision;
            public readonly DX10_Helpers.LDRColour RawRGBPrecision;
            public readonly DX10_Helpers.LDRColour RGBPrecisionWithP;
            public readonly int APrecision;
            public readonly int PBits;
            public readonly int RotationBits;
            public readonly int IndexModeBits;

            public Mode(int partitions, int partitionBits, int IndexPrecision, DX10_Helpers.LDRColour rawrgbPrecision, DX10_Helpers.LDRColour RGBPrecisionWithP, int APrecision, int PBits, int RotationBits, int IndexModeBits)
            {
                Partitions = partitions;
                PartitionBits = partitionBits;
                this.IndexPrecision = IndexPrecision;
                RawRGBPrecision = rawrgbPrecision;
                this.RGBPrecisionWithP = RGBPrecisionWithP;
                this.APrecision = APrecision;
                this.PBits = PBits;
                this.RotationBits = RotationBits;
                this.IndexModeBits = IndexModeBits;
            }
        }

        // Mode: Partitions, partitionBits, indexPrecision, rgbPrecision, rgbPrecisionWithP, APrecision, PBits, Rotation, IndexMode
        static Mode[] Modes = {
            /* Mode 0: */ new Mode(2, 4, 3, new DX10_Helpers.LDRColour(4, 4, 4, 0), new DX10_Helpers.LDRColour(5, 5, 5, 0), 0, 6, 0, 0),
            /* Mode 1: */ new Mode(1, 6, 3, new DX10_Helpers.LDRColour(6, 6, 6, 0), new DX10_Helpers.LDRColour(7, 7, 7, 0), 0, 2, 0, 0),
            /* Mode 2: */ new Mode(2, 6, 2, new DX10_Helpers.LDRColour(5, 5, 5, 0), new DX10_Helpers.LDRColour(5, 5, 5, 0), 0, 0, 0, 0),
            /* Mode 3: */ new Mode(1, 6, 2, new DX10_Helpers.LDRColour(7, 7, 7, 0), new DX10_Helpers.LDRColour(8, 8, 8, 0), 0, 4, 0, 0),
            /* Mode 4: */ new Mode(0, 0, 2, new DX10_Helpers.LDRColour(5, 5, 5, 6), new DX10_Helpers.LDRColour(5, 5, 5, 6), 3, 0, 2, 1),
            /* Mode 5: */ new Mode(0, 0, 2, new DX10_Helpers.LDRColour(7, 7, 7, 8), new DX10_Helpers.LDRColour(7, 7, 7, 8), 2, 0, 2, 0),
            /* Mode 6: */ new Mode(0, 0, 4, new DX10_Helpers.LDRColour(7, 7, 7, 7), new DX10_Helpers.LDRColour(8, 8, 8, 8), 0, 2, 0, 0),
            /* Mode 7: */ new Mode(1, 6, 2, new DX10_Helpers.LDRColour(5, 5, 5, 5), new DX10_Helpers.LDRColour(6, 6, 6, 6), 0, 4, 0, 0)
        };

        private static DX10_Helpers.LDRColour Interpolate(DX10_Helpers.LDRColour lDRColour1, DX10_Helpers.LDRColour lDRColour2, int wc, int wa, int wcPrec, int waPrec)
        {
            DX10_Helpers.LDRColour temp = DX10_Helpers.InterpolateRGB(lDRColour1, lDRColour2, wc, wcPrec);
            temp.A = DX10_Helpers.InterpolateA(lDRColour1, lDRColour2, wa, waPrec);
            return temp;
        }

        internal static void SetColoursFromDX10(DX10_Helpers.LDRColour[] block, byte[] destination, int xPos, int yPos, int width)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    var offset = (xPos + j + (yPos+ i) * width) * 4;
                    var colour = block[(i * 4) + j];

                    destination[offset] = (byte) colour.B;
                    destination[offset + 1] = (byte) colour.G;
                    destination[offset + 2] = (byte) colour.R;
                    destination[offset + 3] = (byte) colour.A;
                }
            }
        }

        public static DX10_Helpers.LDRColour[] DecompressBC7(byte[] source, int sourceStart)
        {
            int start = 0;
            while (start < 128 && DX10_Helpers.GetBit(source, sourceStart, ref start) == 0) { }
            int modeVal = start - 1;
            Mode mode = Modes[modeVal];

            var outColours = new DX10_Helpers.LDRColour[DX10_Helpers.NUM_PIXELS_PER_BLOCK];

            if (modeVal < 8)
            {
                int partitions = mode.Partitions;
                int numEndPoints = (partitions + 1) << 1;
                int indexPrecision = mode.IndexPrecision;
                int APrecision = mode.APrecision;
                int i;
                int[] P = new int[mode.PBits];
                int shape = DX10_Helpers.GetBits(source, sourceStart, ref start, mode.PartitionBits);
                int rotation = DX10_Helpers.GetBits(source, sourceStart, ref start, mode.RotationBits);
                int indexMode = DX10_Helpers.GetBits(source, sourceStart, ref start, mode.IndexModeBits);

                DX10_Helpers.LDRColour[] c = new DX10_Helpers.LDRColour[6];
                DX10_Helpers.LDRColour RGBPrecision = mode.RawRGBPrecision;
                DX10_Helpers.LDRColour RGBPrecisionWithP = mode.RGBPrecisionWithP;

                // Red
                for(i = 0; i < numEndPoints; i++)
                {
                    if (start + RGBPrecision.R > 128)
                        Debugger.Break();  // Error

                    c[i].R = DX10_Helpers.GetBits(source, sourceStart, ref start, RGBPrecision.R);
                }

                // Green
                for (i = 0; i < numEndPoints; i++)
                {
                    if (start + RGBPrecision.G > 128)
                        Debugger.Break();  // Error

                    c[i].G = DX10_Helpers.GetBits(source, sourceStart, ref start, RGBPrecision.G);
                }

                // Blue
                for (i = 0; i < numEndPoints; i++)
                {
                    if (start + RGBPrecision.B > 128)
                        Debugger.Break();  // Error

                    c[i].B = DX10_Helpers.GetBits(source, sourceStart, ref start, RGBPrecision.B);
                }

                // Alpha
                for (i = 0; i < numEndPoints; i++)
                {
                    if (start + RGBPrecision.A > 128)
                        Debugger.Break();  // Error

                    c[i].A = RGBPrecision.A == 0 ? 255 : DX10_Helpers.GetBits(source, sourceStart, ref start, RGBPrecision.A);
                }

                // P Bits
                for (i = 0; i < mode.PBits; i++)
                {
                    if (start > 127)
                    {
                        Debugger.Break();
                        // Error
                    }

                    P[i] = DX10_Helpers.GetBit(source, sourceStart, ref start);
                }


                // Adjust for P bits
                bool rDiff = RGBPrecision.R != RGBPrecisionWithP.R;
                bool gDiff = RGBPrecision.G != RGBPrecisionWithP.B;
                bool bDiff = RGBPrecision.G != RGBPrecisionWithP.G;
                bool aDiff = RGBPrecision.A != RGBPrecisionWithP.A;
                if (mode.PBits != 0)
                {
                    for (i = 0; i < numEndPoints; i++)
                    {
                        int pi = i * mode.PBits / numEndPoints;
                        if (rDiff)
                            c[i].R = (c[i].R << 1) | P[pi];

                        if (gDiff)
                            c[i].G = (c[i].G << 1) | P[pi];

                        if (bDiff)
                            c[i].B = (c[i].B << 1) | P[pi];

                        if (aDiff)
                            c[i].A = (c[i].A << 1) | P[pi];
                    }
                }

                for (i = 0; i < numEndPoints; i++)
                    c[i] = DX10_Helpers.Unquantise(c[i], RGBPrecisionWithP);

                int[] w1 = new int[DX10_Helpers.NUM_PIXELS_PER_BLOCK];
                int[] w2 = new int[DX10_Helpers.NUM_PIXELS_PER_BLOCK];

                // Read colour indicies
                for (i = 0; i < DX10_Helpers.NUM_PIXELS_PER_BLOCK; i++)
                {
                    int numBits = DX10_Helpers.IsFixUpOffset(partitions, shape, i) ? indexPrecision - 1 : indexPrecision;
                    if (start + numBits > 128)
                    {
                        Debugger.Break();
                        // Error
                    }
                    w1[i] = DX10_Helpers.GetBits(source, sourceStart, ref start, numBits);
                }

                // Read Alpha
                if (APrecision != 0)
                {
                    for (i = 0; i < DX10_Helpers.NUM_PIXELS_PER_BLOCK; i++)
                    {
                        int numBits = i != 0 ? APrecision : APrecision - 1;
                        if (start + numBits > 128)
                        {
                        Debugger.Break();
                            // Error
                        }
                        w2[i] = DX10_Helpers.GetBits(source, sourceStart, ref start, numBits);
                    }
                }


                for (i = 0; i < DX10_Helpers.NUM_PIXELS_PER_BLOCK; i++)
                {
                    int region = DX10_Helpers.PartitionTable[partitions][shape][i];
                    DX10_Helpers.LDRColour outPixel;
                    if (APrecision == 0)
                        outPixel = Interpolate(c[region << 1], c[(region << 1) + 1], w1[i], w1[i], indexPrecision, indexPrecision);
                    else
                    {
                        if (indexMode == 0)
                            outPixel = Interpolate(c[region << 1], c[(region << 1) + 1], w1[i], w2[i], indexPrecision, APrecision);
                        else
                            outPixel = Interpolate(c[region << 1], c[(region << 1) + 1], w2[i], w1[i], APrecision, indexPrecision);
                    }

                    switch (rotation)
                    {
                        case 1:
                            int temp = outPixel.R;
                            outPixel.R = outPixel.A;
                            outPixel.A = temp;
                            break;
                        case 2:
                            temp = outPixel.G;
                            outPixel.G = outPixel.A;
                            outPixel.A = temp;
                            break;
                        case 3:
                            temp = outPixel.B;
                            outPixel.B = outPixel.A;
                            outPixel.A = temp;
                            break;
                    }

                    outColours[i] = outPixel;
                }
                return outColours;
            }

            return outColours;
        }
    }
}
