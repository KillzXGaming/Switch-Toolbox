using System;
using System.Collections.Generic;
using System.IO;

namespace FirstPlugin
{
    /// <summary>
    /// Builds the uniform banks the game's own EFT vertex shaders read (GPU preview, bank-mode
    /// class), from file data + preview camera only; no captures at runtime.
    ///
    /// bank7 (EmitterStatic, VS bank 7): the on-disk EMTR data-frame payload (big-endian,
    /// includes the 0x50-byte head before the toolbox's EmitterData struct frame) copied with a
    /// per-word LE swap for numeric words and raw bytes for byte-array words (name, enums,
    /// sampler records); the raw/swap split is the mask sidecar (bank7_mask.txt), plus the
    /// deterministic load-time rules: constant fills, unconditional rotation-window zeroing,
    /// curve-key tail extension, and the runtime FLAG WORDS at bank 0x50 derived from emitter
    /// config bytes (the VS reads word0 bits 0-27 and word1 bit 0 as feature muxes; zeroed
    /// flags kill every vertex).
    ///
    /// bank6 (view, 304B): a pure function of the camera (view + projection); near/far are
    /// recovered from the projection z row. v18 = (near, far, far, far-near) is the source of
    /// the scene-depth encoding; v16.w/v17.w are uninitialized padding in the game.
    ///
    /// bank8 (EmitterDynamic): color multipliers, (emitterTime, 1, 1, K) and the fade envelope
    /// (1s = fully faded in), plus the optional 3x4 emitter world matrix (176B variant).
    /// </summary>
    public static class EftUniformBanks
    {
        static HashSet<int> maskRaw;
        static Dictionary<int, byte[]> maskFills;
        static List<int> maskZeroed;

        /// <summary>The deployed bank7_mask.txt, or null when it is not there. It sits next to the plugin
        /// assembly or under Lib/Plugins, the same deployment as gx2dec.exe.</summary>
        internal static string FindMask()
        {
            string dir = Path.GetDirectoryName(typeof(EftUniformBanks).Assembly.Location);
            foreach (string cand in new[] {
                Path.Combine(dir, "bank7_mask.txt"),
                Path.Combine(dir, "Lib", "Plugins", "bank7_mask.txt") })
            {
                if (File.Exists(cand)) return cand;
            }
            return null;
        }

        static void EnsureMask()
        {
            if (maskRaw != null)
                return;
            string path = FindMask();
            if (path == null)
                throw new FileNotFoundException("bank7_mask.txt sidecar not found (EftUniformBanks)");
            var raw = new HashSet<int>();
            var fills = new Dictionary<int, byte[]>();
            var zeroed = new List<int>();
            foreach (string line in File.ReadAllLines(path))
            {
                if (line.Length == 0 || line[0] == '#')
                    continue;
                string[] tok = line.Split(' ');
                int off = Convert.ToInt32(tok[1], 16);
                if (tok[0] == "raw") raw.Add(off);
                else if (tok[0] == "zero") zeroed.Add(off);
                else if (tok[0] == "fill")
                {
                    var b = new byte[4];
                    for (int i = 0; i < 4; i++)
                        b[i] = Convert.ToByte(tok[2].Substring(i * 2, 2), 16);
                    fills[off] = b;
                }
            }
            maskRaw = raw; maskFills = fills; maskZeroed = zeroed;
        }

        /// <summary>The EMTR data-frame payload (big-endian file bytes from the emitter's data
        /// position; toolbox struct-frame offsets are payload - 0x50) -> the 64KB-class
        /// EmitterStatic bank image (LE), sized max(payload, 0xB00) rounded to 16.</summary>
        public static byte[] BuildBank7(byte[] payload)
        {
            EnsureMask();
            int size = (Math.Max(payload.Length, 0xB00) + 15) & ~15;
            var outb = new byte[size];
            int L = Math.Min(payload.Length / 4 * 4, size);
            for (int o = 0; o < L; o += 4)
            {
                if (maskRaw.Contains(o) || (o >= 0x10 && o < 0x40))
                {
                    Array.Copy(payload, o, outb, o, 4);
                }
                else
                {
                    outb[o] = payload[o + 3]; outb[o + 1] = payload[o + 2];
                    outb[o + 2] = payload[o + 1]; outb[o + 3] = payload[o];
                }
            }
            foreach (var kv in maskFills)
            {
                int o = kv.Key;
                if (o + 4 <= size && outb[o] == 0 && outb[o + 1] == 0 && outb[o + 2] == 0 && outb[o + 3] == 0)
                    Array.Copy(kv.Value, 0, outb, o, 4);
            }
            // rotation-window words zeroed unconditionally at load
            var zero = new List<int>(maskZeroed) { 0x700, 0x704, 0x708, 0x70C, 0x710, 0x714, 0x720 };
            foreach (int o in zero)
                if (o + 4 <= size)
                    Array.Clear(outb, o, 4);
            // curve-key tail extension: repeat the last live key out to 8 (key counts at struct
            // 0x10-0x20; tracks at struct 0x370/0x3F0/0x470/0x4F0/0x5B0)
            int[,] tracks = { { 0x370, 0x10 }, { 0x3F0, 0x14 }, { 0x470, 0x18 }, { 0x4F0, 0x1C }, { 0x5B0, 0x20 } };
            for (int t = 0; t < 5; t++)
            {
                int baseOff = 0x50 + tracks[t, 0];
                uint n = ReadU32LE(outb, 0x50 + tracks[t, 1]);
                if (n > 0 && n <= 8 && baseOff + 128 <= size)
                    for (int k = (int)n; k < 8; k++)
                        Array.Copy(outb, baseOff + ((int)n - 1) * 16, outb, baseOff + k * 16, 16);
            }
            // disabled color tracks (count 0): the runtime writes the track's own const color
            // (struct+0x958 / struct+0x968) into key 0 (xyz, w = 0) and leaves the tail zero
            int[,] constTracks = { { 0x370, 0x10, 0x958 }, { 0x470, 0x18, 0x968 } };
            for (int t = 0; t < 2; t++)
            {
                int baseOff = 0x50 + constTracks[t, 0], constOff = 0x50 + constTracks[t, 2];
                if (ReadU32LE(outb, 0x50 + constTracks[t, 1]) != 0 ||
                    constOff + 12 > payload.Length || baseOff + 16 > size) continue;
                for (int c = 0; c < 3; c++)
                {
                    outb[baseOff + c * 4] = payload[constOff + c * 4 + 3];
                    outb[baseOff + c * 4 + 1] = payload[constOff + c * 4 + 2];
                    outb[baseOff + c * 4 + 2] = payload[constOff + c * 4 + 1];
                    outb[baseOff + c * 4 + 3] = payload[constOff + c * 4];
                }
                Array.Clear(outb, baseOff + 12, 4);
            }
            if (payload.Length > 0xA68)
            {
                uint w0, w1;
                DeriveFlagWords(payload, out w0, out w1);
                WriteU32LE(outb, 0x50, w0);
                WriteU32LE(outb, 0x54, w1);
            }
            return outb;
        }

        /// <summary>Runtime flag words (bank 0x50-0x5F; the disk holds zeros there). The VS reads
        /// word0 bits 0-27 and word1 bit 0 as feature muxes; word2/word3 are never VS-read.
        /// Offsets are payload-frame (= struct + 0x50). Bits 0..2 select the alpha-fluctuation
        /// waveform from the 0x9EF bit-field (8=sine, 16=random, 32=blink; the VS sums one dot
        /// term per set bit, so an unmapped value zeroes the alpha of every fluctuating
        /// emitter).</summary>
        public static void DeriveFlagWords(byte[] p, out uint w0, out uint w1)
        {
            w0 = 0;
            if (p[0x9EF] == 8) w0 |= 1u << 0;
            if (p[0x9EF] == 16) w0 |= 1u << 1;
            if (p[0x9EF] == 32) w0 |= 1u << 2;
            if (p[0x7F1] != 0) w0 |= 1u << 3;
            if (p[0xA58] == 3) w0 |= (1u << 6) | (1u << 25);
            if (p[0xA58] == 4) w0 |= 1u << 7;
            if (p[0xA68] != 0) w0 |= 1u << 11;
            if (p[0x8AD] != 0) w0 |= 1u << 16;
            if (p[0x8AE] != 0) w0 |= 1u << 17;
            if (p[0x8AF] != 0) w0 |= 1u << 18;
            if (p[0xA5D] != 0) w0 |= 1u << 19;
            if (p[0x8AC] == 0) w0 |= 1u << 29;
            w1 = p[0x8B4] != 0 ? 1u : 0u;
            w1 |= p[0x753] != 0 ? 1u << 11 : 1u << 9;
        }

        /// <summary>View bank (304B, 19 vec4s) from camera matrices (row-major 4x4, row vectors
        /// as double[16]). near/far derive from the projection z row. Math runs in double and
        /// packs to f32.</summary>
        public static byte[] BuildBank6(double[] view, double[] proj)
        {
            double A = proj[10], B = proj[11];
            double near = B / (A - 1.0), far = B / (A + 1.0);
            var rows = new List<double[]>();
            for (int r = 0; r < 4; r++)
                rows.Add(new[] { view[r * 4], view[r * 4 + 1], view[r * 4 + 2], view[r * 4 + 3] });
            for (int r = 0; r < 4; r++)
                rows.Add(new[] { proj[r * 4], proj[r * 4 + 1], proj[r * 4 + 2], proj[r * 4 + 3] });
            for (int r = 0; r < 3; r++)                                 // v8-10: (proj*view) rows
            {
                var row = new double[4];
                for (int c = 0; c < 4; c++)
                {
                    double s = 0.0;
                    for (int k = 0; k < 4; k++)
                        s += proj[r * 4 + k] * view[k * 4 + c];
                    row[c] = s;
                }
                rows.Add(row);
            }
            rows.Add(new[] { -view[8], -view[9], -view[10], -view[11] });   // v11: clip.w row (viewZ)
            for (int i = 0; i < 3; i++)                                     // v12-14: R^T (billboard)
                rows.Add(new[] { view[i], view[4 + i], view[8 + i], 0.0 });
            rows.Add(new[] { 0.0, 0.0, 0.0, 1.0 });                         // v15
            rows.Add(new[] { view[8], view[9], view[10], 0.0 });            // v16: forward (w = padding)
            var campos = new double[4];                                     // v17: camera pos = -R^T * t
            for (int i = 0; i < 3; i++)
            {
                double s = 0.0;
                for (int r = 0; r < 3; r++)
                    s += view[r * 4 + i] * view[r * 4 + 3];
                campos[i] = -s;
            }
            rows.Add(campos);
            rows.Add(new[] { near, far, far, far - near });                 // v18: depth-encode source
            var outb = new byte[304];
            for (int r = 0; r < 19; r++)
                for (int c = 0; c < 4; c++)
                    WriteF32LE(outb, r * 16 + c * 4, (float)rows[r][c]);
            return outb;
        }

        /// <summary>EmitterDynamic bank: 64B header, or the 176B variant when a 3x4 emitter
        /// world matrix (row-major, 12 floats) is supplied. K is per-instance sim state (1.5);
        /// the envelope defaults to fully faded in. Rows [8..10] repeat the emitter matrix
        /// without the world scale: the ripple/wave mesh VSes transform their ring PRIM through
        /// them.</summary>
        public static byte[] BuildBank8(float emitterTime, float k, float[] emitterMat3x4 = null)
        {
            var outb = new byte[emitterMat3x4 == null ? 64 : 176];
            for (int c = 0; c < 4; c++)
            {
                WriteF32LE(outb, c * 4, 1f);          // [0] color0 multiplier
                WriteF32LE(outb, 16 + c * 4, 1f);     // [1] color1 multiplier
                WriteF32LE(outb, 48 + c * 4, 1f);     // [3] fade/scale envelope
            }
            WriteF32LE(outb, 32, emitterTime);        // [2] (time, 1, 1, K)
            WriteF32LE(outb, 36, 1f);
            WriteF32LE(outb, 40, 1f);
            WriteF32LE(outb, 44, k);
            if (emitterMat3x4 != null)
                for (int i = 0; i < 12; i++)
                {
                    WriteF32LE(outb, 64 + i * 4, emitterMat3x4[i]);    // [4..6] emitter world matrix
                    WriteF32LE(outb, 128 + i * 4, emitterMat3x4[i]);   // [8..10] unscaled twin
                }
            return outb;
        }

        static uint ReadU32LE(byte[] b, int o)
        {
            return (uint)(b[o] | (b[o + 1] << 8) | (b[o + 2] << 16) | (b[o + 3] << 24));
        }

        static void WriteU32LE(byte[] b, int o, uint v)
        {
            b[o] = (byte)v; b[o + 1] = (byte)(v >> 8); b[o + 2] = (byte)(v >> 16); b[o + 3] = (byte)(v >> 24);
        }

        static void WriteF32LE(byte[] b, int o, float v)
        {
            byte[] fb = BitConverter.GetBytes(v);
            Array.Copy(fb, 0, b, o, 4);
        }
    }
}
