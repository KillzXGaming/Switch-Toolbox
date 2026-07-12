using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FirstPlugin
{
    /// <summary>
    /// Drives the standalone Cemu-derived GX2 (Wii U Latte) -> GLSL decompiler (gx2dec.exe) on an emitter's
    /// fragment shader. It extracts the PS microcode from the GFD group and reconstructs the Latte
    /// context registers from the embedded GX2PixelShader struct (the same scatter Cemu's GX2SetPixelShader
    /// does) + forces every PS texture unit DIM to 2D (eft textures are 2D; the GX2 struct carries no texture
    /// state). The decompiler runs as a separate process (arm's length; keeps the MPL-2.0 code at a process
    /// boundary), prints GLSL to stdout and the host binding/uniform contract to stderr.
    /// </summary>
    public static class Gx2ShaderDecompiler
    {
        public class Result
        {
            public string Glsl;       // decompiled GLSL (Cemu flavor; non-VULKAN path is the GL one)
            public string Mapping;    // the stderr "=== MAPPING ===" block (textures, uf_remapped sources, offsets)
            public string Error;      // non-null on failure
            public Dictionary<string, int> AttribVars; // eft attr name (sys*Attr) -> semantic id
        }

        // Parse the GX2 attribVar name table from a vertex-shader group: eft attribute name (sysInPosAttr,
        // sysPosAttr, sysScaleAttr, ...) -> semantic id (the attrDataSemN the decompiled GLSL reads). Entry =
        // {name_VA(u32), type(u32), count(u32), location(u32)} (16B BE); the run has format codes at +4 and
        // ascending location at +12; VA base auto-detected (file = VA - base + 0x20). Lets the editor feed each
        // attribute by its real ROLE (corner/pos/scale) instead of a fixed sem guess. Never throws.
        public static Dictionary<string, int> ParseAttribVars(byte[] d)
        {
            var map = new Dictionary<string, int>();
            try
            {
                Func<int, uint> b = o => U32BE(d, o);
                var nameRe = new System.Text.RegularExpressions.Regex(@"^sys[A-Za-z0-9]+$");
                Func<long, string> readstr = f =>
                {
                    if (f < 0 || f >= d.Length) return null;
                    int e = (int)f; while (e < d.Length && d[e] != 0) e++;
                    string s; try { s = System.Text.Encoding.ASCII.GetString(d, (int)f, e - (int)f); } catch { return null; }
                    return nameRe.IsMatch(s) ? s : null;
                };
                var fmt = new HashSet<uint> { 4, 10, 11, 12 };
                int bestO = -1, bestN = 0;                          // longest entry run (location == index)
                for (int o = 0x20; o < d.Length - 16; o += 4)
                {
                    int cnt = 0;
                    while (o + cnt * 16 + 16 <= d.Length)
                    {
                        int e = o + cnt * 16;
                        if (fmt.Contains(b(e + 4)) && b(e + 12) == (uint)cnt && b(e) >= 0x80000000u) cnt++;
                        else break;
                    }
                    if (cnt >= 2 && cnt > bestN) { bestN = cnt; bestO = o; }
                }
                if (bestO < 0) return map;
                var vas = new uint[bestN];
                for (int i = 0; i < bestN; i++) vas[i] = b(bestO + i * 16);
                // collect candidate name offsets (any "sys...Attr" string)
                var nameOffs = new List<int>();
                for (int o = 0; o < d.Length - 4; o++)
                    if (d[o] == (byte)'s' && d[o + 1] == (byte)'y' && d[o + 2] == (byte)'s')
                    { var s = readstr(o); if (s != null && s.EndsWith("Attr")) nameOffs.Add(o); }
                long bestBase = 0x20; int bestHits = -1;            // base maximizing resolved names
                var cands = new HashSet<long> { 0xca700000 };
                foreach (var va in vas) foreach (var off in nameOffs) cands.Add((long)va - (off - 0x20));
                foreach (var cand in cands)
                {
                    int hits = 0; foreach (var va in vas) if (readstr((long)va - cand + 0x20) != null) hits++;
                    if (hits > bestHits) { bestHits = hits; bestBase = cand; }
                }
                for (int i = 0; i < bestN; i++) { var nm = readstr((long)vas[i] - bestBase + 0x20); if (nm != null) map[nm] = i; }
            }
            catch { }
            return map;
        }

        public class SamplerVar
        {
            public string Name;    // NW4F source sampler name (sysTextureSampler0, sysDepthBufferTexture, sysCustomShaderCubeSampler0, ...)
            public int Type;       // GX2SamplerVarType: 1 = 2D, 4 = cube
            public int Location;   // GX2 texture slot = the textureUnitPS<N> binding in the decompiled GLSL
        }

        // Parse the GX2 samplerVar table from a pixel-shader group: count @ header+0xD0, table VA
        // @ header+0xD4, entries = {name_VA, type, location} (12B BE). The names identify each unit's
        // ROLE (art slot N / scene depth / environment cube), so lit-ness and unit assignment are file
        // truth instead of convention (cube type 4 at slot 10, depth 2D at slot 4, art at slots 0..2).
        // VAs map into the header payload by their low bits (payloads are small;
        // table VAs sit in the 0xD06xxxxx space, names in 0xCA70xxxx). Never throws; empty on any doubt.
        public static List<SamplerVar> ParseSamplerVars(byte[] group)
        {
            var vars = new List<SamplerVar>();
            try
            {
                if (group == null || group.Length < 0x20 + 0xD8) return vars;
                int count = (int)U32BE(group, 0x20 + 0xD0);
                if (count <= 0 || count > 32) return vars;
                int table = (int)(U32BE(group, 0x20 + 0xD4) & 0xFFFFF) + 0x20;
                if (table + count * 12 > group.Length) return vars;
                for (int i = 0; i < count; i++)
                {
                    int e = table + i * 12;
                    int nameOff = (int)(U32BE(group, e) & 0xFFFFF) + 0x20;
                    if (nameOff < 0 || nameOff >= group.Length) return new List<SamplerVar>();
                    int end = nameOff;
                    while (end < group.Length && group[end] != 0) end++;
                    string name = Encoding.ASCII.GetString(group, nameOff, end - nameOff);
                    if (name.Length == 0 || name.Length > 64 || !name.StartsWith("sys")) return new List<SamplerVar>();
                    vars.Add(new SamplerVar
                    {
                        Name = name,
                        Type = (int)U32BE(group, e + 4),
                        Location = (int)U32BE(group, e + 8),
                    });
                }
            }
            catch { vars.Clear(); }
            return vars;
        }

        private static string _exePath;
        private static bool _searched;

        public static string FindExe()
        {
            if (_searched) return _exePath;
            _searched = true;
            var cands = new List<string>();
            var env = Environment.GetEnvironmentVariable("GX2DEC_PATH");
            if (!string.IsNullOrEmpty(env)) cands.Add(env);

            //The build copies gx2dec.exe next to the plugin (Lib\Plugins), but plugins can be shadow-copied, so the
            //assembly's own Location may be empty or a temp path. Probe the assembly folder, the app base folder and
            //the assembly CodeBase, each with the Lib and Lib\Plugins subfolders.
            var dirs = new List<string>();
            try { var loc = typeof(Gx2ShaderDecompiler).Assembly.Location; if (!string.IsNullOrEmpty(loc)) dirs.Add(Path.GetDirectoryName(loc)); } catch { }
            try { dirs.Add(AppDomain.CurrentDomain.BaseDirectory); } catch { }
            try { dirs.Add(Path.GetDirectoryName(new Uri(typeof(Gx2ShaderDecompiler).Assembly.CodeBase).LocalPath)); } catch { }
            foreach (var d in dirs)
            {
                if (string.IsNullOrEmpty(d)) continue;
                cands.Add(Path.Combine(d, "gx2dec.exe"));
                cands.Add(Path.Combine(d, "Lib", "gx2dec.exe"));
                cands.Add(Path.Combine(d, "Lib", "Plugins", "gx2dec.exe"));
            }
            foreach (var c in cands)
                if (!string.IsNullOrEmpty(c) && File.Exists(c)) { _exePath = c; break; }
            return _exePath;
        }

        private static uint U32BE(byte[] d, int o)
        {
            return ((uint)d[o] << 24) | ((uint)d[o + 1] << 16) | ((uint)d[o + 2] << 8) | d[o + 3];
        }

        // Scatter the GX2PixelShader 'regs' (big-endian, at the start of the header payload) into a
        // contextRegisters[0x10009] array, exactly as Cemu's GX2SetPixelShader submits them, then return it as
        // native little-endian bytes (the decompiler memcpy's it into a uint32[] and reads bitfields natively).
        private static byte[] BuildPixelRegs(byte[] hdrPayload, int[] cubeUnits)
        {
            const int N = 0x10000 + 9;
            uint[] r = new uint[N];
            Func<int, uint> rb = i => U32BE(hdrPayload, i * 4);
            r[0xA214] = rb(0);                              // mmSQ_PGM_RESOURCES_PS
            r[0xA1B3] = rb(2); r[0xA1B4] = rb(3);           // mmSPI_PS_IN_CONTROL_0/1
            uint numInputs = Math.Min(rb(4), 0x20u);
            for (uint i = 0; i < numInputs; i++) r[0xA191 + i] = rb((int)(5 + i)); // mmSPI_PS_INPUT_CNTL_0+i
            r[0xA08F] = rb(37); r[0xA1E8] = rb(38); r[0xA203] = rb(39); r[0xA1B6] = rb(40); // CB_SHADER_MASK/CONTROL, DB_SHADER_CONTROL, SPI_INPUT_Z
            for (int u = 0; u < 18; u++) r[0xE000 + u * 7] = 1; // mmSQ_TEX_RESOURCE_WORD0_0 + u*7, DIM_2D
            // Env-cube units must decompile as DIM_CUBE(3); the blanket 2D silently drops the cube-sample
            // instruction and with it the env-lighting term of lit translucents. Convention: env cube at PS
            // slot 10, scene depth 2D at slot 4.
            if (cubeUnits != null)
                foreach (int u in cubeUnits)
                    if (u >= 0 && u < 18) r[0xE000 + u * 7] = 3;
            byte[] outb = new byte[N * 4];
            Buffer.BlockCopy(r, 0, outb, 0, outb.Length); // x86 native = little-endian
            return outb;
        }

        // Run the decompiler, draining stdout and stderr concurrently (a synchronous ReadToEnd on one stream
        // before the other deadlocks when the child fills the unread pipe), and bound the wait. On success
        // fills res.Glsl/res.Mapping; on timeout or empty output sets res.Error.
        private static void RunDecompiler(string exe, string args, Result res)
        {
            var psi = new ProcessStartInfo(exe, args)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };
            var sbOut = new StringBuilder();
            var sbErr = new StringBuilder();
            using (var p = Process.Start(psi))
            {
                p.OutputDataReceived += (s, e) => { if (e.Data != null) sbOut.AppendLine(e.Data); };
                p.ErrorDataReceived += (s, e) => { if (e.Data != null) sbErr.AppendLine(e.Data); };
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                if (!p.WaitForExit(15000)) { try { p.Kill(); } catch { } res.Error = "decompiler timed out"; return; }
                p.WaitForExit();   // let the async readers flush after the process exits
                res.Glsl = sbOut.ToString();
                res.Mapping = sbErr.ToString();
                if (string.IsNullOrWhiteSpace(res.Glsl))
                    res.Error = "decompiler produced no GLSL (exit " + p.ExitCode + "). " + res.Mapping;
            }
        }

        // Vertex-shader decompile. Scatters the GX2VertexShader regs into a Latte contextRegisters array as
        // Cemu's GX2SetVertexShader does, ALSO scatters the paired fragment shader's SPI_PS_INPUT_CNTL
        // registers (the VS only emits a passParameterSemN export when the paired PS consumes that
        // semantic), forces PA_CL_VTE_CNTL to the clip-space value (unset = Cemu's window-space path, i.e.
        // garbage positions) and every VS texture DIM to 2D (sampler1D output is an artifact of unset dims),
        // and builds the runtime fetch-shader microcode (what nw::eft builds via GX2InitFetchShaderEx) with
        // Cemu's fetch-shader bit-packing: ENDIAN NONE + one float4 slot per semantic, matching the preview's
        // pre-decoded little-endian streams. regs.bin / fetch.bin are native little-endian u32 (how gx2dec
        // reads both).

        // --- mm* register indices (RegDefines.h) ---
        private const int mmSQ_PGM_RESOURCES_VS = 0xA21A;
        private const int mmVGT_PRIMITIVEID_EN = 0xA2A1;
        private const int mmSPI_VS_OUT_CONFIG = 0xA1B1;
        private const int mmSPI_VS_OUT_ID_0 = 0xA185;
        private const int mmPA_CL_VS_OUT_CNTL = 0xA207;
        private const int mmPA_CL_VTE_CNTL = 0xA206;
        private const int mmSQ_VTX_SEMANTIC_0 = 0xA0E0;
        private const int mmSQ_TEX_RESOURCE_WORD0_0 = 0xE000; // 7 dwords per texture unit (PS range)
        private const int mmSQ_TEX_RESOURCE_WORD0_0_VS = 0xE460; // VS texture resources live in their own range
        private const int mmSPI_PS_IN_CONTROL_0 = 0xA1B3;
        private const int mmSPI_PS_INPUT_CNTL_0 = 0xA191;

        // --- Latte fetch-format / NFA constants (LatteConst.h) ---
        private const int FMT_32_32_32_32_FLOAT = 0x23;
        private const int NFA_SCALED = 2;          // NUM_FORMAT_SCALED (what float formats use)
        private const int ENDIAN_NONE = 0;         // streams are pre-decoded LE floats
        private const int VTX_INST_SEMANTIC = 1;
        private const int FETCH_TYPE_VERTEX = 0;   // per-vertex index

        // INST_VTX_TC fetch clause (LatteCFInstruction_DEFAULT).
        private static void MakeCfVtxTc(uint addrBytes, int count, out uint word0, out uint word1)
        {
            word0 = addrBytes >> 3;
            word1 = 0x03u << 23;                     // INST_VTX_TC opcode (bits 23..30)
            uint c = (uint)(count - 1);
            word1 |= (c & 0x7u) << 10;               // COUNT low 3 bits
            word1 |= (c << 16) & (1u << 19);         // COUNT R700 extra bit at 19
        }
        // INST_RETURN + BARRIER.
        private static void MakeCfReturn(out uint word0, out uint word1)
        {
            word0 = 0;
            word1 = 0x14u << 23;                      // INST_RETURN
            word1 |= (1u << 31);                      // BARRIER
        }
        // LatteClauseInstruction_VTX semantic fetch (word3 unused).
        private static void MakeVtxSemantic(int semanticId, int bufferId, int offset, int dataFormat,
            int nfa, int endianSwap, bool signed, int[] dstSel, out uint w0, out uint w1, out uint w2, out uint w3)
        {
            w0 = 0; w1 = 0; w2 = 0; w3 = 0;
            w0 |= ((uint)VTX_INST_SEMANTIC & 0x1F) << 0;     // VTX_INST
            w0 |= ((uint)FETCH_TYPE_VERTEX & 0x3) << 5;      // FETCH_TYPE
            w0 |= ((uint)bufferId & 0xFF) << 8;              // BUFFER_ID
            // SRC_SEL_X = 0 at bit 24
            w1 |= ((uint)semanticId & 0xFF) << 0;            // SEM_SEMANTIC_ID
            for (int i = 0; i < dstSel.Length; i++)
                w1 |= ((uint)dstSel[i] & 0x7) << (9 + i * 3);// DST_SEL[i]
            w1 |= ((uint)dataFormat & 0x3F) << 22;           // DATA_FORMAT
            w1 |= ((uint)nfa & 0x3) << 28;                   // NUM_FORMAT_ALL
            w1 |= (signed ? 1u : 0u) << 30;                  // FORMAT_COMP_ALL
            w2 |= ((uint)offset & 0xFFFF) << 0;              // OFFSET
            w2 |= ((uint)endianSwap & 0x3) << 16;            // ENDIAN_SWAP
        }
        // Build the runtime fetch shader (CF program + VTX clauses). One float4 slot per semantic, which is
        // the preview's stream ABI: every attribute is fed as a full float4.
        private static byte[] BuildFetchShader(uint[] semantics)
        {
            int n = semantics.Length;
            int numCf = ((n + 15) / 16) + 1;
            int cfSize = (numCf * 8 + 0xF) & ~0xF;   // pad to 16
            var words = new List<uint>();
            // CF program
            int ai = 0;
            while (ai < n)
            {
                int cnt = Math.Min(n - ai, 16);
                uint w0, w1; MakeCfVtxTc((uint)(cfSize + ai * 16), cnt, out w0, out w1);
                words.Add(w0); words.Add(w1);
                ai += 16;
            }
            uint rw0, rw1; MakeCfReturn(out rw0, out rw1);
            words.Add(rw0); words.Add(rw1);
            while (words.Count * 4 < cfSize) words.Add(0);
            // VTX clauses
            for (int i = 0; i < n; i++)
            {
                int sem = (int)(semantics[i] & 0xFF);
                var dstSel = new int[] { 0, 1, 2, 3 };
                int off = i * 16;                    // sequential, float4 stride
                uint vw0, vw1, vw2, vw3;
                MakeVtxSemantic(sem, 0 + 0xA0, off, FMT_32_32_32_32_FLOAT,
                    NFA_SCALED, ENDIAN_NONE, false, dstSel, out vw0, out vw1, out vw2, out vw3);
                words.Add(vw0); words.Add(vw1); words.Add(vw2); words.Add(vw3);
            }
            byte[] outb = new byte[words.Count * 4];
            Buffer.BlockCopy(words.ToArray(), 0, outb, 0, outb.Length); // x86 native = little-endian
            return outb;
        }

        private class VertexInputs
        {
            public byte[] Program;    // raw GX2 VS microcode
            public byte[] Regs;       // Latte contextRegisters image (LE u32)
            public byte[] Fetch;      // synthetic fetch-shader microcode (LE u32)
            public uint[] Semantics;  // SQ_VTX_SEMANTIC table (float4 slot order of the fetch)
            public string Error;      // non-null on failure
        }

        // Build the three gx2dec inputs (program / regs / fetch) for an emitter's vertex shader. vtxGroup =
        // its VS GFD group (header block ++ program block); fragGroup = the PAIRED fragment shader's group.
        private static VertexInputs BuildVertexInputs(byte[] vtxGroup, byte[] fragGroup)
        {
            var inp = new VertexInputs();
            if (vtxGroup == null || vtxGroup.Length < 0x40) { inp.Error = "no vertex shader for this emitter"; return inp; }
            // VS GFD group layout (same as the PS group): 0x20 header BLK header, then the GX2VertexShader
            // payload (big-endian), then the program BLK header + program payload.
            int hdrDsz = (int)U32BE(vtxGroup, 0x14);          // header BLK data size = GX2VertexShader payload length
            if (hdrDsz < 0 || hdrDsz > vtxGroup.Length - 0x20) { inp.Error = "vertex header block out of range"; return inp; }
            byte[] h = new byte[hdrDsz];
            Array.Copy(vtxGroup, 0x20, h, 0, hdrDsz);
            Func<int, uint> rb = off => U32BE(h, off);        // read a GX2VertexShader field (BE)

            int vsSize = (int)rb(0xD0);                       // shaderSize
            int progPayloadStart = 0x20 + hdrDsz + 0x20;      // after header block, skip program BLK header
            if (progPayloadStart > vtxGroup.Length) { inp.Error = "vertex program block out of range"; return inp; }
            int progAvail = vtxGroup.Length - progPayloadStart;
            if (vsSize <= 0 || vsSize > progAvail) vsSize = progAvail; // clamp shaderSize to the available program bytes
            inp.Program = new byte[vsSize];
            Array.Copy(vtxGroup, progPayloadStart, inp.Program, 0, vsSize);

            const int N = 0x10000 + 9;
            uint[] regs = new uint[N];
            regs[mmPA_CL_VTE_CNTL] = 0x43F;                   // clip-space transforms; unset = window-space garbage
            for (int u = 0; u < 18; u++)
                regs[mmSQ_TEX_RESOURCE_WORD0_0_VS + u * 7] = 1; // every VS texture DIM_2D (unset = sampler1D artifact)

            // scatter GX2VertexShader regs (GX2SetVertexShader)
            regs[mmSQ_PGM_RESOURCES_VS] = rb(0x00);
            regs[mmVGT_PRIMITIVEID_EN] = rb(0x04);
            regs[mmSPI_VS_OUT_CONFIG] = rb(0x08);
            uint vsOutIdTableSize = rb(0x0C);
            for (uint i = 0; i < Math.Min(vsOutIdTableSize, 10u); i++)
                regs[mmSPI_VS_OUT_ID_0 + i] = rb((int)(0x10 + i * 4));
            regs[mmPA_CL_VS_OUT_CNTL] = rb(0x38);
            uint semSize = rb(0x40);
            int semCount = (int)Math.Min(semSize, 32u);
            inp.Semantics = new uint[semCount];
            for (int i = 0; i < semCount; i++)
            {
                uint v = rb(0x44 + i * 4);
                regs[mmSQ_VTX_SEMANTIC_0 + i] = v;
                inp.Semantics[i] = v;
            }

            // scatter the PAIRED frag's PS-input registers (so the VS emits the params the PS consumes)
            if (fragGroup != null && fragGroup.Length >= 0x40)
            {
                int fHdrDsz = (int)U32BE(fragGroup, 0x14);
                byte[] fh = new byte[fHdrDsz];
                Array.Copy(fragGroup, 0x20, fh, 0, fHdrDsz);
                Func<int, uint> fb = i => U32BE(fh, i * 4);   // GX2PixelShader regs[i]
                regs[mmSPI_PS_IN_CONTROL_0] = fb(2);
                regs[mmSPI_PS_IN_CONTROL_0 + 1] = fb(3);      // SPI_PS_IN_CONTROL_1
                uint numPSInputs = Math.Min(fb(4), 0x20u);
                for (uint i = 0; i < numPSInputs; i++)
                    regs[mmSPI_PS_INPUT_CNTL_0 + i] = fb((int)(5 + i));
            }

            inp.Regs = new byte[N * 4];
            Buffer.BlockCopy(regs, 0, inp.Regs, 0, inp.Regs.Length); // x86 native = little-endian
            inp.Fetch = BuildFetchShader(inp.Semantics);
            return inp;
        }

        /// <summary>Decompile an emitter's vertex shader to GLSL. vtxGroup = its VS GFD group (header block
        /// ++ program block); fragGroup = the PAIRED fragment shader's group (its SPI_PS_INPUT_CNTL registers are
        /// scattered too so the VS emits the params the PS consumes). Returns the VS GLSL in Result.Glsl.</summary>
        public static Result DecompileVertex(byte[] vtxGroup, byte[] fragGroup)
        {
            var res = new Result();
            string exe = FindExe();
            if (exe == null) { res.Error = "gx2dec.exe not found (set GX2DEC_PATH, or place gx2dec.exe next to the plugin)"; return res; }

            string progPath = null, regsPath = null, fetchPath = null;
            try
            {
                var inp = BuildVertexInputs(vtxGroup, fragGroup);
                if (inp.Error != null) { res.Error = inp.Error; return res; }

                string tmp = Path.Combine(Path.GetTempPath(), "gx2dec_" + Guid.NewGuid().ToString("N"));
                progPath = tmp + "_vp.bin"; regsPath = tmp + "_vr.bin"; fetchPath = tmp + "_vf.bin";
                File.WriteAllBytes(progPath, inp.Program);
                File.WriteAllBytes(regsPath, inp.Regs);
                File.WriteAllBytes(fetchPath, inp.Fetch);

                RunDecompiler(exe, "vs \"" + progPath + "\" \"" + regsPath + "\" \"" + fetchPath + "\"", res);
            }
            catch (Exception ex) { res.Error = ex.Message; }
            finally
            {
                try { if (progPath != null) File.Delete(progPath); } catch { }
                try { if (regsPath != null) File.Delete(regsPath); } catch { }
                try { if (fetchPath != null) File.Delete(fetchPath); } catch { }
            }
            res.AttribVars = ParseAttribVars(vtxGroup);   // eft attr name -> semantic id, for name-based feeding
            return res;
        }

        /// <summary>Decompile an emitter's fragment shader group (header block ++ program block) to GLSL.
        /// cubeUnits lists PS texture slots holding environment CUBE maps (dim from the GX2 struct is
        /// unavailable; the eft convention binds the env cube at slot 10 for lit translucents).</summary>
        public static Result DecompileFragment(byte[] fragGroup)
        {
            return DecompileFragment(fragGroup, null);
        }

        public static Result DecompileFragment(byte[] fragGroup, int[] cubeUnits)
        {
            var res = new Result();
            if (fragGroup == null || fragGroup.Length < 0x40) { res.Error = "no fragment shader for this emitter"; return res; }
            string exe = FindExe();
            if (exe == null) { res.Error = "gx2dec.exe not found (set GX2DEC_PATH, or place gx2dec.exe next to the plugin)"; return res; }

            string progPath = null, regsPath = null;
            try
            {
                int hdrDsz = (int)U32BE(fragGroup, 0x14);          // header BLK data size = GX2PixelShader payload length
                if (hdrDsz < 0 || hdrDsz > fragGroup.Length - 0x20) { res.Error = "fragment header block out of range"; return res; }
                byte[] hdrPayload = new byte[hdrDsz];
                Array.Copy(fragGroup, 0x20, hdrPayload, 0, hdrDsz);
                int psSize = (int)U32BE(hdrPayload, 0xA4);          // program length
                int progPayloadStart = 0x20 + hdrDsz + 0x20;       // after header block, skip program BLK header
                if (progPayloadStart + psSize > fragGroup.Length) { res.Error = "program block out of range"; return res; }
                byte[] prog = new byte[psSize];
                Array.Copy(fragGroup, progPayloadStart, prog, 0, psSize);
                byte[] regs = BuildPixelRegs(hdrPayload, cubeUnits);

                string tmp = Path.Combine(Path.GetTempPath(), "gx2dec_" + Guid.NewGuid().ToString("N"));
                progPath = tmp + "_p.bin"; regsPath = tmp + "_r.bin";
                File.WriteAllBytes(progPath, prog);
                File.WriteAllBytes(regsPath, regs);

                RunDecompiler(exe, "ps \"" + progPath + "\" \"" + regsPath + "\"", res);
            }
            catch (Exception ex) { res.Error = ex.Message; }
            finally
            {
                try { if (progPath != null) File.Delete(progPath); } catch { }
                try { if (regsPath != null) File.Delete(regsPath); } catch { }
            }
            return res;
        }
    }
}
