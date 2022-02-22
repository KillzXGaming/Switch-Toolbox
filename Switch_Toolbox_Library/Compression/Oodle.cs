using System;
using System.Runtime.InteropServices;

namespace Toolbox.Library.Compression
{
    // Code from https://github.com/JKAnderson/SoulsFormats/blob/master/SoulsFormats/Util/Oodle26.cs
    public static class Oodle
    {
        public static byte[] Compress(byte[] source, OodleLZ_Compressor compressor, OodleLZ_CompressionLevel level)
        {
            IntPtr pOptions = OodleLZ_CompressOptions_GetDefault(compressor, level);
            OodleLZ_CompressOptions options = Marshal.PtrToStructure<OodleLZ_CompressOptions>(pOptions);
            // Required for the game to not crash
            options.seekChunkReset = true;
            // This is already the default but I am including it for authenticity to game code
            options.seekChunkLen = 0x40000;
            pOptions = Marshal.AllocHGlobal(Marshal.SizeOf<OodleLZ_CompressOptions>());

            try
            {
                Marshal.StructureToPtr(options, pOptions, false);
                long compressedBufferSizeNeeded = OodleLZ_GetCompressedBufferSizeNeeded(source.LongLength);
                byte[] compBuf = new byte[compressedBufferSizeNeeded];
                long compLen = OodleLZ_Compress(compressor, source, source.LongLength, compBuf, level, pOptions, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0);
                Array.Resize(ref compBuf, (int)compLen);
                return compBuf;
            }
            finally
            {
                Marshal.FreeHGlobal(pOptions);
            }
        }

        public static byte[] Decompress(byte[] source, long uncompressedSize)
        {
            long decodeBufferSize = OodleLZ_GetDecodeBufferSize(uncompressedSize, true);
            byte[] rawBuf = new byte[decodeBufferSize];
            long rawLen = OodleLZ_Decompress(source, source.LongLength, rawBuf, uncompressedSize);
            Array.Resize(ref rawBuf, (int)rawLen);
            return rawBuf;
        }


        /// <param name="compressor"></param>
        /// <param name="rawBuf"></param>
        /// <param name="rawLen"></param>
        /// <param name="compBuf"></param>
        /// <param name="level"></param>
        /// <param name="pOptions">= NULL</param>
        /// <param name="dictionaryBase">= NULL</param>
        /// <param name="lrm">= NULL</param>
        /// <param name="scratchMem">= NULL</param>
        /// <param name="scratchSize">= 0</param>
        [DllImport("oo2core_6_win64.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern long OodleLZ_Compress(
            OodleLZ_Compressor compressor,
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] rawBuf,
            long rawLen,
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] compBuf,
            OodleLZ_CompressionLevel level,
            IntPtr pOptions,
            IntPtr dictionaryBase,
            IntPtr lrm,
            IntPtr scratchMem,
            long scratchSize);

        private static long OodleLZ_Compress(OodleLZ_Compressor compressor, byte[] rawBuf, long rawLen, byte[] compBuf, OodleLZ_CompressionLevel level)
            => OodleLZ_Compress(compressor, rawBuf, rawLen, compBuf, level,
                IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0);


        /// <param name="compressor">= OodleLZ_Compressor_Invalid</param>
        /// <param name="lzLevel">= OodleLZ_CompressionLevel_Normal</param>
        [DllImport("oo2core_6_win64.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr OodleLZ_CompressOptions_GetDefault(
            OodleLZ_Compressor compressor,
            OodleLZ_CompressionLevel lzLevel);

        private static IntPtr OodleLZ_CompressOptions_GetDefault()
            => OodleLZ_CompressOptions_GetDefault(OodleLZ_Compressor.OodleLZ_Compressor_Invalid, OodleLZ_CompressionLevel.OodleLZ_CompressionLevel_Normal);


        /// <param name="compBuf"></param>
        /// <param name="compBufSize"></param>
        /// <param name="rawBuf"></param>
        /// <param name="rawLen"></param>
        /// <param name="fuzzSafe">= OodleLZ_FuzzSafe_Yes</param>
        /// <param name="checkCRC">= OodleLZ_CheckCRC_No</param>
        /// <param name="verbosity">= OodleLZ_Verbosity_None</param>
        /// <param name="decBufBase">= NULL</param>
        /// <param name="decBufSize">= 0</param>
        /// <param name="fpCallback">= NULL</param>
        /// <param name="callbackUserData">= NULL</param>
        /// <param name="decoderMemory">= NULL</param>
        /// <param name="decoderMemorySize">= 0</param>
        /// <param name="threadPhase">= OodleLZ_Decode_Unthreaded</param>
        [DllImport("oo2core_6_win64.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern long OodleLZ_Decompress(
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] compBuf,
            long compBufSize,
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] rawBuf,
            long rawLen,
            OodleLZ_FuzzSafe fuzzSafe,
            OodleLZ_CheckCRC checkCRC,
            OodleLZ_Verbosity verbosity,
            IntPtr decBufBase,
            long decBufSize,
            IntPtr fpCallback,
            IntPtr callbackUserData,
            IntPtr decoderMemory,
            long decoderMemorySize,
            OodleLZ_Decode_ThreadPhase threadPhase);

        private static long OodleLZ_Decompress(byte[] compBuf, long compBufSize, byte[] rawBuf, long rawLen)
            => OodleLZ_Decompress(compBuf, compBufSize, rawBuf, rawLen,
                OodleLZ_FuzzSafe.OodleLZ_FuzzSafe_Yes, OodleLZ_CheckCRC.OodleLZ_CheckCRC_No, OodleLZ_Verbosity.OodleLZ_Verbosity_None,
                IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0, OodleLZ_Decode_ThreadPhase.OodleLZ_Decode_Unthreaded);


        [DllImport("oo2core_6_win64.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern long OodleLZ_GetCompressedBufferSizeNeeded(
               long rawSize);


        [DllImport("oo2core_6_win64.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern long OodleLZ_GetDecodeBufferSize(
            long rawSize,
            [MarshalAs(UnmanagedType.Bool)]
            bool corruptionPossible);


        [StructLayout(LayoutKind.Sequential)]
        private struct OodleLZ_CompressOptions
        {
            public uint verbosity;
            public int minMatchLen;
            [MarshalAs(UnmanagedType.Bool)]
            public bool seekChunkReset;
            public int seekChunkLen;
            public OodleLZ_Profile profile;
            public int dictionarySize;
            public int spaceSpeedTradeoffBytes;
            public int maxHuffmansPerChunk;
            [MarshalAs(UnmanagedType.Bool)]
            public bool sendQuantumCRCs;
            public int maxLocalDictionarySize;
            public int makeLongRangeMatcher;
            public int matchTableSizeLog2;
        }


        private enum OodleLZ_CheckCRC : int
        {
            OodleLZ_CheckCRC_No = 0,
            OodleLZ_CheckCRC_Yes = 1,
            OodleLZ_CheckCRC_Force32 = 0x40000000
        }

        public enum OodleLZ_CompressionLevel : int
        {
            OodleLZ_CompressionLevel_None = 0,
            OodleLZ_CompressionLevel_SuperFast = 1,
            OodleLZ_CompressionLevel_VeryFast = 2,
            OodleLZ_CompressionLevel_Fast = 3,
            OodleLZ_CompressionLevel_Normal = 4,

            OodleLZ_CompressionLevel_Optimal1 = 5,
            OodleLZ_CompressionLevel_Optimal2 = 6,
            OodleLZ_CompressionLevel_Optimal3 = 7,
            OodleLZ_CompressionLevel_Optimal4 = 8,
            OodleLZ_CompressionLevel_Optimal5 = 9,

            OodleLZ_CompressionLevel_HyperFast1 = -1,
            OodleLZ_CompressionLevel_HyperFast2 = -2,
            OodleLZ_CompressionLevel_HyperFast3 = -3,
            OodleLZ_CompressionLevel_HyperFast4 = -4,

            OodleLZ_CompressionLevel_HyperFast = OodleLZ_CompressionLevel_HyperFast1,
            OodleLZ_CompressionLevel_Optimal = OodleLZ_CompressionLevel_Optimal2,
            OodleLZ_CompressionLevel_Max = OodleLZ_CompressionLevel_Optimal5,
            OodleLZ_CompressionLevel_Min = OodleLZ_CompressionLevel_HyperFast4,

            OodleLZ_CompressionLevel_Force32 = 0x40000000,
            OodleLZ_CompressionLevel_Invalid = OodleLZ_CompressionLevel_Force32
        }

        public enum OodleLZ_Compressor : int
        {
            OodleLZ_Compressor_Invalid = -1,
            OodleLZ_Compressor_None = 3,

            OodleLZ_Compressor_Kraken = 8,
            OodleLZ_Compressor_Leviathan = 13,
            OodleLZ_Compressor_Mermaid = 9,
            OodleLZ_Compressor_Selkie = 11,
            OodleLZ_Compressor_Hydra = 12,

            OodleLZ_Compressor_BitKnit = 10,
            OodleLZ_Compressor_LZB16 = 4,
            OodleLZ_Compressor_LZNA = 7,
            OodleLZ_Compressor_LZH = 0,
            OodleLZ_Compressor_LZHLW = 1,
            OodleLZ_Compressor_LZNIB = 2,
            OodleLZ_Compressor_LZBLW = 5,
            OodleLZ_Compressor_LZA = 6,

            OodleLZ_Compressor_Count = 14,
            OodleLZ_Compressor_Force32 = 0x40000000
        }

        private enum OodleLZ_Decode_ThreadPhase : int
        {
            OodleLZ_Decode_ThreadPhase1 = 1,
            OodleLZ_Decode_ThreadPhase2 = 2,
            OodleLZ_Decode_ThreadPhaseAll = 3,
            OodleLZ_Decode_Unthreaded = OodleLZ_Decode_ThreadPhaseAll
        }

        private enum OodleLZ_FuzzSafe : int
        {
            OodleLZ_FuzzSafe_No = 0,
            OodleLZ_FuzzSafe_Yes = 1
        }

        private enum OodleLZ_Profile : int
        {
            OodleLZ_Profile_Main = 0,
            OodleLZ_Profile_Reduced = 1,
            OodleLZ_Profile_Force32 = 0x40000000
        }

        private enum OodleLZ_Verbosity : int
        {
            OodleLZ_Verbosity_None = 0,
            OodleLZ_Verbosity_Minimal = 1,
            OodleLZ_Verbosity_Some = 2,
            OodleLZ_Verbosity_Lots = 3,
            OodleLZ_Verbosity_Force32 = 0x40000000
        }
    }
}
