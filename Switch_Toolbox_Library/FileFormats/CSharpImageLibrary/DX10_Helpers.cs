using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://github.com/KFreon/CSharpImageLibrary

namespace CSharpImageLibrary.DDS
{
    internal static class DX10_Helpers
    {
        public struct LDRColourEndPointPair
        {
            public LDRColour A;
            public LDRColour B;

            public LDRColourEndPointPair(LDRColour a, LDRColour b)
            {
                A = a;
                B = b;
            }

            public override string ToString()
            {
                return "A" + Environment.NewLine + $"R: {A.R} G: {A.G} B: {A.B} A: {A.A}" + Environment.NewLine +
                    "B" + Environment.NewLine + $"R: {B.R} G: {B.G} B: {B.B} A: {B.A}";
            }
        }



        #region Structs
        public struct LDRColour
        {
            public int R;
            public int G;
            public int B;
            public int A;

            public LDRColour(byte R, byte G, byte B, byte A)
            {
                this.R = R;
                this.B = B;
                this.G = G;
                this.A = A;
            }

            public LDRColour(float r, float g, float b, float a)
            {
                R = (byte)(Clamp(r, 0f, 1f) * 255);
                G = (byte)(Clamp(g, 0f, 1f) * 255);
                B = (byte)(Clamp(b, 0f, 1f) * 255);
                A = (byte)(Clamp(a, 0f, 1f) * 255);
            }

            public override string ToString()
            {
                return $"R: {R}, G: {G}, B: {B}, A: {A}";
            }

            static float Clamp(float val, float lower, float upper)
            {
                if (val > upper)
                    return upper;

                if (val < lower)
                    return lower;

                return val;
            }
        }

        public struct HDRColour
        {
            public float R, G, B, A;

            public HDRColour(LDRColour colour)
            {
                R = colour.R * 1f / 255f;
                G = colour.G * 1f / 255f;
                B = colour.B * 1f / 255f;
                A = colour.A * 1f / 255f;
            }

            public override string ToString()
            {
                return $"R: {R}, G: {G}, B: {B}, A: {A}";
            }
        }
        #endregion Structs

        #region Constants
        internal const int BC67_WEIGHT_MAX = 64;
        internal const int BC67_WEIGHT_ROUND = 32;
        internal const int BC67_WEIGHT_SHIFT = 6;
        internal const int NUM_PIXELS_PER_BLOCK = 16;
        internal const int BC7_MAX_REGIONS = 3;
        internal const int BC7_MAX_SHAPES = 64;
        internal const int BC7_MAX_INDICIES = 16;
        internal const int BC7_NUM_CHANNELS = 4;
        #endregion Constants

        #region Tables
        // 3,64,16
        internal static byte[][][] PartitionTable = new byte[3][][]
        {   // 1 Region case has no subsets (all 0)
            new byte[64][]
            {
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            },

            new byte[64][]
            {   // BC6H/BC7 Partition Set for 2 Subsets
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1 }, // Shape 0
                new byte[16] { 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1 }, // Shape 1
                new byte[16] { 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1 }, // Shape 2
                new byte[16] { 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1 }, // Shape 3
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1 }, // Shape 4
                new byte[16] { 0, 0, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1 }, // Shape 5
                new byte[16] { 0, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1 }, // Shape 6
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1 }, // Shape 7
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1 }, // Shape 8
                new byte[16] { 0, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, // Shape 9
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1 }, // Shape 10
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 1 }, // Shape 11
                new byte[16] { 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, // Shape 12
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1 }, // Shape 13
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, // Shape 14
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 }, // Shape 15
                new byte[16] { 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1 }, // Shape 16
                new byte[16] { 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 }, // Shape 17
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0 }, // Shape 18
                new byte[16] { 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0 }, // Shape 19
                new byte[16] { 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 }, // Shape 20
                new byte[16] { 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0 }, // Shape 21
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0 }, // Shape 22
                new byte[16] { 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1 }, // Shape 23
                new byte[16] { 0, 0, 1, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0 }, // Shape 24
                new byte[16] { 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0 }, // Shape 25
                new byte[16] { 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0 }, // Shape 26
                new byte[16] { 0, 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 0, 0 }, // Shape 27
                new byte[16] { 0, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0 }, // Shape 28
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 }, // Shape 29
                new byte[16] { 0, 1, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0 }, // Shape 30
                new byte[16] { 0, 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0 }, // Shape 31

                                                                // BC7 Partition Set for 2 Subsets (second-half)
                new byte[16] { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 }, // Shape 32
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 }, // Shape 33
                new byte[16] { 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0 }, // Shape 34
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0 }, // Shape 35
                new byte[16] { 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0 }, // Shape 36
                new byte[16] { 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0 }, // Shape 37
                new byte[16] { 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1 }, // Shape 38
                new byte[16] { 0, 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1 }, // Shape 39
                new byte[16] { 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0 }, // Shape 40
                new byte[16] { 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 1, 0, 0, 0 }, // Shape 41
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 0, 0 }, // Shape 42
                new byte[16] { 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0 }, // Shape 43
                new byte[16] { 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0 }, // Shape 44
                new byte[16] { 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1 }, // Shape 45
                new byte[16] { 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1 }, // Shape 46
                new byte[16] { 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0 }, // Shape 47
                new byte[16] { 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0 }, // Shape 48
                new byte[16] { 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0 }, // Shape 49
                new byte[16] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0 }, // Shape 50
                new byte[16] { 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0 }, // Shape 51
                new byte[16] { 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1 }, // Shape 52
                new byte[16] { 0, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1 }, // Shape 53
                new byte[16] { 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0 }, // Shape 54
                new byte[16] { 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 0 }, // Shape 55
                new byte[16] { 0, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 0, 0, 1 }, // Shape 56
                new byte[16] { 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 0, 1 }, // Shape 57
                new byte[16] { 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1 }, // Shape 58
                new byte[16] { 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1 }, // Shape 59
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1 }, // Shape 60
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 }, // Shape 61
                new byte[16] { 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0 }, // Shape 62
                new byte[16] { 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1 }  // Shape 63
            },

            new byte[64][]
            {   // BC7 Partition Set for 3 Subsets
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 1, 0, 2, 2, 1, 2, 2, 2, 2 }, // Shape 0
                new byte[16] { 0, 0, 0, 1, 0, 0, 1, 1, 2, 2, 1, 1, 2, 2, 2, 1 }, // Shape 1
                new byte[16] { 0, 0, 0, 0, 2, 0, 0, 1, 2, 2, 1, 1, 2, 2, 1, 1 }, // Shape 2
                new byte[16] { 0, 2, 2, 2, 0, 0, 2, 2, 0, 0, 1, 1, 0, 1, 1, 1 }, // Shape 3
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 2, 2, 1, 1, 2, 2 }, // Shape 4
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 2, 2, 0, 0, 2, 2 }, // Shape 5
                new byte[16] { 0, 0, 2, 2, 0, 0, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1 }, // Shape 6
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 1, 2, 2, 1, 1, 2, 2, 1, 1 }, // Shape 7
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2 }, // Shape 8
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2 }, // Shape 9
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2 }, // Shape 10
                new byte[16] { 0, 0, 1, 2, 0, 0, 1, 2, 0, 0, 1, 2, 0, 0, 1, 2 }, // Shape 11
                new byte[16] { 0, 1, 1, 2, 0, 1, 1, 2, 0, 1, 1, 2, 0, 1, 1, 2 }, // Shape 12
                new byte[16] { 0, 1, 2, 2, 0, 1, 2, 2, 0, 1, 2, 2, 0, 1, 2, 2 }, // Shape 13
                new byte[16] { 0, 0, 1, 1, 0, 1, 1, 2, 1, 1, 2, 2, 1, 2, 2, 2 }, // Shape 14
                new byte[16] { 0, 0, 1, 1, 2, 0, 0, 1, 2, 2, 0, 0, 2, 2, 2, 0 }, // Shape 15
                new byte[16] { 0, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 2, 1, 1, 2, 2 }, // Shape 16
                new byte[16] { 0, 1, 1, 1, 0, 0, 1, 1, 2, 0, 0, 1, 2, 2, 0, 0 }, // Shape 17
                new byte[16] { 0, 0, 0, 0, 1, 1, 2, 2, 1, 1, 2, 2, 1, 1, 2, 2 }, // Shape 18
                new byte[16] { 0, 0, 2, 2, 0, 0, 2, 2, 0, 0, 2, 2, 1, 1, 1, 1 }, // Shape 19
                new byte[16] { 0, 1, 1, 1, 0, 1, 1, 1, 0, 2, 2, 2, 0, 2, 2, 2 }, // Shape 20
                new byte[16] { 0, 0, 0, 1, 0, 0, 0, 1, 2, 2, 2, 1, 2, 2, 2, 1 }, // Shape 21
                new byte[16] { 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 2, 2, 0, 1, 2, 2 }, // Shape 22
                new byte[16] { 0, 0, 0, 0, 1, 1, 0, 0, 2, 2, 1, 0, 2, 2, 1, 0 }, // Shape 23
                new byte[16] { 0, 1, 2, 2, 0, 1, 2, 2, 0, 0, 1, 1, 0, 0, 0, 0 }, // Shape 24
                new byte[16] { 0, 0, 1, 2, 0, 0, 1, 2, 1, 1, 2, 2, 2, 2, 2, 2 }, // Shape 25
                new byte[16] { 0, 1, 1, 0, 1, 2, 2, 1, 1, 2, 2, 1, 0, 1, 1, 0 }, // Shape 26
                new byte[16] { 0, 0, 0, 0, 0, 1, 1, 0, 1, 2, 2, 1, 1, 2, 2, 1 }, // Shape 27
                new byte[16] { 0, 0, 2, 2, 1, 1, 0, 2, 1, 1, 0, 2, 0, 0, 2, 2 }, // Shape 28
                new byte[16] { 0, 1, 1, 0, 0, 1, 1, 0, 2, 0, 0, 2, 2, 2, 2, 2 }, // Shape 29
                new byte[16] { 0, 0, 1, 1, 0, 1, 2, 2, 0, 1, 2, 2, 0, 0, 1, 1 }, // Shape 30
                new byte[16] { 0, 0, 0, 0, 2, 0, 0, 0, 2, 2, 1, 1, 2, 2, 2, 1 }, // Shape 31
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 2, 2, 1, 2, 2, 2 }, // Shape 32
                new byte[16] { 0, 2, 2, 2, 0, 0, 2, 2, 0, 0, 1, 2, 0, 0, 1, 1 }, // Shape 33
                new byte[16] { 0, 0, 1, 1, 0, 0, 1, 2, 0, 0, 2, 2, 0, 2, 2, 2 }, // Shape 34
                new byte[16] { 0, 1, 2, 0, 0, 1, 2, 0, 0, 1, 2, 0, 0, 1, 2, 0 }, // Shape 35
                new byte[16] { 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 0, 0, 0, 0 }, // Shape 36
                new byte[16] { 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0, 1, 2, 0 }, // Shape 37
                new byte[16] { 0, 1, 2, 0, 2, 0, 1, 2, 1, 2, 0, 1, 0, 1, 2, 0 }, // Shape 38
                new byte[16] { 0, 0, 1, 1, 2, 2, 0, 0, 1, 1, 2, 2, 0, 0, 1, 1 }, // Shape 39
                new byte[16] { 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 0, 0, 0, 0, 1, 1 }, // Shape 40
                new byte[16] { 0, 1, 0, 1, 0, 1, 0, 1, 2, 2, 2, 2, 2, 2, 2, 2 }, // Shape 41
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 2, 1, 2, 1, 2, 1 }, // Shape 42
                new byte[16] { 0, 0, 2, 2, 1, 1, 2, 2, 0, 0, 2, 2, 1, 1, 2, 2 }, // Shape 43
                new byte[16] { 0, 0, 2, 2, 0, 0, 1, 1, 0, 0, 2, 2, 0, 0, 1, 1 }, // Shape 44
                new byte[16] { 0, 2, 2, 0, 1, 2, 2, 1, 0, 2, 2, 0, 1, 2, 2, 1 }, // Shape 45
                new byte[16] { 0, 1, 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 0, 1, 0, 1 }, // Shape 46
                new byte[16] { 0, 0, 0, 0, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 }, // Shape 47
                new byte[16] { 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 2, 2, 2, 2 }, // Shape 48
                new byte[16] { 0, 2, 2, 2, 0, 1, 1, 1, 0, 2, 2, 2, 0, 1, 1, 1 }, // Shape 49
                new byte[16] { 0, 0, 0, 2, 1, 1, 1, 2, 0, 0, 0, 2, 1, 1, 1, 2 }, // Shape 50
                new byte[16] { 0, 0, 0, 0, 2, 1, 1, 2, 2, 1, 1, 2, 2, 1, 1, 2 }, // Shape 51
                new byte[16] { 0, 2, 2, 2, 0, 1, 1, 1, 0, 1, 1, 1, 0, 2, 2, 2 }, // Shape 52
                new byte[16] { 0, 0, 0, 2, 1, 1, 1, 2, 1, 1, 1, 2, 0, 0, 0, 2 }, // Shape 53
                new byte[16] { 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 2, 2, 2, 2 }, // Shape 54
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 2, 2, 1, 1, 2 }, // Shape 55
                new byte[16] { 0, 1, 1, 0, 0, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 2 }, // Shape 56
                new byte[16] { 0, 0, 2, 2, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 2, 2 }, // Shape 57
                new byte[16] { 0, 0, 2, 2, 1, 1, 2, 2, 1, 1, 2, 2, 0, 0, 2, 2 }, // Shape 58
                new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 2 }, // Shape 59
                new byte[16] { 0, 0, 0, 2, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 1 }, // Shape 60
                new byte[16] { 0, 2, 2, 2, 1, 2, 2, 2, 0, 2, 2, 2, 1, 2, 2, 2 }, // Shape 61
                new byte[16] { 0, 1, 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 }, // Shape 62
                new byte[16] { 0, 1, 1, 1, 2, 0, 1, 1, 2, 2, 0, 1, 2, 2, 2, 0 }  // Shape 63
            }
        };


        // 3,64,3
        internal static byte[][][] FixUpTable = new byte[3][][]
        {
            new byte[64][]
            {   // No fix-ups for 1st subset for BC6H or BC7
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 },
                new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }, new byte[3] { 0, 0, 0 }
            },

            new byte[64][]
            {   // BC6H/BC7 Partition Set Fixups for 2 Subsets
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0, 2, 0 },
                new byte[3] { 0, 2, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0, 2, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 2, 0 },
                new byte[3] { 0, 8, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 2, 0 },
                
                 // BC7 Partition Set Fixups for 2 Subsets (second-half)
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0, 6, 0 }, new byte[3] { 0, 8, 0 },
                new byte[3] { 0, 2, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0, 2, 0 }, new byte[3] { 0, 8, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 2, 0 },
                new byte[3] { 0, 2, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0, 6, 0 },
                new byte[3] { 0, 6, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 6, 0 }, new byte[3] { 0, 8, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 2, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 }, new byte[3] { 0,15, 0 },
                new byte[3] { 0,15, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0, 2, 0 }, new byte[3] { 0,15, 0 }
            },

            new byte[64][]
            {   // BC7 Partition Set Fixups for 3 Subsets
                new byte[3] { 0, 3,15 }, new byte[3] { 0, 3, 8 }, new byte[3] { 0,15, 8 }, new byte[3] { 0,15, 3 },
                new byte[3] { 0, 8,15 }, new byte[3] { 0, 3,15 }, new byte[3] { 0,15, 3 }, new byte[3] { 0,15, 8 },
                new byte[3] { 0, 8,15 }, new byte[3] { 0, 8,15 }, new byte[3] { 0, 6,15 }, new byte[3] { 0, 6,15 },
                new byte[3] { 0, 6,15 }, new byte[3] { 0, 5,15 }, new byte[3] { 0, 3,15 }, new byte[3] { 0, 3, 8 },
                new byte[3] { 0, 3,15 }, new byte[3] { 0, 3, 8 }, new byte[3] { 0, 8,15 }, new byte[3] { 0,15, 3 },
                new byte[3] { 0, 3,15 }, new byte[3] { 0, 3, 8 }, new byte[3] { 0, 6,15 }, new byte[3] { 0,10, 8 },
                new byte[3] { 0, 5, 3 }, new byte[3] { 0, 8,15 }, new byte[3] { 0, 8, 6 }, new byte[3] { 0, 6,10 },
                new byte[3] { 0, 8,15 }, new byte[3] { 0, 5,15 }, new byte[3] { 0,15,10 }, new byte[3] { 0,15, 8 },
                new byte[3] { 0, 8,15 }, new byte[3] { 0,15, 3 }, new byte[3] { 0, 3,15 }, new byte[3] { 0, 5,10 },
                new byte[3] { 0, 6,10 }, new byte[3] { 0,10, 8 }, new byte[3] { 0, 8, 9 }, new byte[3] { 0,15,10 },
                new byte[3] { 0,15, 6 }, new byte[3] { 0, 3,15 }, new byte[3] { 0,15, 8 }, new byte[3] { 0, 5,15 },
                new byte[3] { 0,15, 3 }, new byte[3] { 0,15, 6 }, new byte[3] { 0,15, 6 }, new byte[3] { 0,15, 8 },
                new byte[3] { 0, 3,15 }, new byte[3] { 0,15, 3 }, new byte[3] { 0, 5,15 }, new byte[3] { 0, 5,15 },
                new byte[3] { 0, 5,15 }, new byte[3] { 0, 8,15 }, new byte[3] { 0, 5,15 }, new byte[3] { 0,10,15 },
                new byte[3] { 0, 5,15 }, new byte[3] { 0,10,15 }, new byte[3] { 0, 8,15 }, new byte[3] { 0,13,15 },
                new byte[3] { 0,15, 3 }, new byte[3] { 0,12,15 }, new byte[3] { 0, 3,15 }, new byte[3] { 0, 3, 8 }
            }
        };

        internal static int[] AWeights2 = new int[] { 0, 21, 43, 64 };
        internal static int[] AWeights3 = new int[] { 0, 9, 18, 27, 37, 46, 55, 64 };
        internal static int[] AWeights4 = new int[] { 0, 4, 9, 13, 17, 21, 26, 30, 34, 38, 43, 47, 51, 55, 60, 64 };
        #endregion Tables


        #region Decompression Helpers
        internal static int InterpolateA(LDRColour lDRColour1, LDRColour lDRColour2, int wa, int waPrec)
        {
            int[] weights = null;
            switch (waPrec)
            {
                case 2:
                    weights = AWeights2;
                    break;
                case 3:
                    weights = AWeights3;
                    break;
                case 4:
                    weights = AWeights4;
                    break;
                default:
                    return 0;
            }
            return (lDRColour1.A * (BC67_WEIGHT_MAX - weights[wa]) + lDRColour2.A * weights[wa] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT;
        }

        internal static LDRColour InterpolateRGB(LDRColour lDRColour1, LDRColour lDRColour2, int wc, int wcPrec)
        {
            LDRColour temp = new LDRColour();
            int[] weights = null;
            switch (wcPrec)
            {
                case 2:
                    weights = AWeights2;
                    break;
                case 3:
                    weights = AWeights3;
                    break;
                case 4:
                    weights = AWeights4;
                    break;
                default:
                    return temp;
            }
            temp.R = (lDRColour1.R * (BC67_WEIGHT_MAX - weights[wc]) + lDRColour2.R * weights[wc] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT;
            temp.G = (lDRColour1.G * (BC67_WEIGHT_MAX - weights[wc]) + lDRColour2.G * weights[wc] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT;
            temp.B = (lDRColour1.B * (BC67_WEIGHT_MAX - weights[wc]) + lDRColour2.B * weights[wc] + BC67_WEIGHT_ROUND) >> BC67_WEIGHT_SHIFT;
            return temp;
        }

        internal static bool IsFixUpOffset(int partitions, int shape, int offset)
        {
            for (int i = 0; i <= partitions; i++)
            {
                if (offset == FixUpTable[partitions][shape][i])
                    return true;
            }

            return false;
        }

        internal static LDRColour Unquantise(LDRColour colour, LDRColour rGBPrecisionWithP)
        {
            LDRColour temp = new LDRColour()
            {
                R = Unquantise(colour.R, rGBPrecisionWithP.R),
                G = Unquantise(colour.G, rGBPrecisionWithP.G),
                B = Unquantise(colour.B, rGBPrecisionWithP.B),
                A = rGBPrecisionWithP.A > 0 ? Unquantise(colour.A, rGBPrecisionWithP.A) : 255
            };
            return temp;
        }

        internal static int Unquantise(int r1, int r2)
        {
            int temp = r1 << (8 - r2);
            return temp | (temp >> r2);
        }

        internal static int GetBit(byte[] source, int sourceStart, ref int start)
        {
            int uIndex = start >> 3;
            int ret = (source[sourceStart + uIndex] >> (start - (uIndex << 3))) & 0x01;
            start++;
            return ret;
        }

        internal static int GetBits(byte[] source, int sourceStart, ref int start, int length)
        {
            if (length == 0)
                return 0;

            int uIndex = start >> 3;
            int uBase = start - (uIndex << 3);
            int ret = 0;

            if (uBase + length > 8)
            {
                int firstIndexBits = 8 - uBase;
                int nextIndexBits = length - firstIndexBits;
                ret = (source[sourceStart + uIndex] >> uBase) | ((source[sourceStart + uIndex + 1] & ((1 << nextIndexBits) - 1)) << firstIndexBits);
            }
            else
                ret = (source[sourceStart + uIndex] >> uBase) & ((1 << length) - 1);


            start += length;
            return ret;
        }
        #endregion Decompression Helpers
    }
}