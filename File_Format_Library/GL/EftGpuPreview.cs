using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FirstPlugin
{
    /// <summary>
    /// GPU emitter preview: renders a BotW EFTB emitter with the game's OWN shaders, from file
    /// data plus the preview camera only. The SHDA vertex/fragment groups decompile through
    /// Gx2ShaderDecompiler, the uniform banks come from EftUniformBanks (EmitterStatic = the
    /// emitter payload; view = the camera; EmitterDynamic = the playback clock), per-particle
    /// streams come from the CPU sim (EftEmitterRender.BuildInstanceStreams), and the
    /// engine-bound inputs the file does not carry (env/scene banks 11/13/14, the HDR
    /// environment cube at PS slot 10, the VS random table and env palette, scene depth for the
    /// soft-particle fade) are shipped static assets under EftPreviewAssets (the depth is a
    /// synthetic far plane so the fade stays open). GL objects are created lazily in the
    /// caller's current context; Render draws into the caller's framebuffer. Billboards get the
    /// unit quad; PRIM mesh emitters get their mesh as the per-vertex streams (pos w=1 + uv +
    /// normals).
    /// </summary>
    public class EftGpuPreview : IDisposable
    {
        public class TextureInput
        {
            public byte[] Rgba;              // RGBA8 bytes (decode BC/format beforehand)
            public int W, H;
            public string Swizzle = "RGBA";  // GX2 compSel (BC4 = RRRR, BC5 = RRRG)
        }

        readonly byte[] vtxGroup, fragGroup, payload;
        readonly List<TextureInput> art;
        readonly List<Gx2ShaderDecompiler.SamplerVar> psSamplers;
        readonly EftEmitterRender sim;
        readonly float[] meshVerts, meshNormals;   // PRIM mesh (pos3+uv2 / nrm3); null = billboard quad
        readonly int[] meshIndices;

        int prog, vao, ibo;
        IGraphicsContext owner;   // context the GL objects were made in; VAOs and FBOs are private to it
        // HDR-accumulate + tonemap: the emitter draws into an RGBA16F buffer (so additive
        // accumulation is not clipped at 8 bits; orange fire otherwise overflows to a yellow/
        // white blob), then a fullscreen pass resolves it to the caller's framebuffer with BotW's
        // own tonemap operator (1 - exp(-x), from the game's tonemap pixel shader)
        int hdrFbo, hdrTex, hdrW, hdrH;
        int resolveProg, resolveVao;
        // sysFrameBufferTexture (distortion/refraction, PS unit 3, ~200 emitters of the _ind class):
        // the emitter's own shader samples the rendered scene at a normal-map offset to warp it. The
        // caller's framebuffer (grid + background from the opaque pass) is grabbed here each frame so
        // the shader has the real scene to distort instead of the synthetic 1x1 far-plane it would
        // otherwise fall to. sceneTexUnit < 0 = not a distortion shader (no scene sampler declared).
        int sceneTex, sceneTexUnit = -1, sceneW, sceneH;
        // preview visibility aid: a faithful refraction of the editor's dark, low-contrast scene is
        // nearly invisible. When set, the emitter refracts a high-contrast reference grid instead of
        // the grabbed viewport, so the warp reads clearly (the refraction math stays the game's own).
        bool sceneRefGrid;
        const int RefGridSize = 512;
        readonly List<int> vertVbos = new List<int>();
        readonly List<int> ubos = new List<int>();
        // attribute locations with an enabled vertex array; every OTHER declared attribute
        // reads the context-global generic slot, which Render must set deterministically
        // (a previous instance's leftover VertexAttribI4 is otherwise what it reads)
        readonly HashSet<int> fedAttribLocs = new HashSet<int>();
        int drawIdxCount = 6;
        DrawElementsType drawIdxType = DrawElementsType.UnsignedShort;

        // connection/history stripes (vertexTransformMode@struct+0x8F4 = 2/3, 2-attribute VS
        // variant): the CPU sends (pos.xyz, +-scale) and (dir.xyz, alpha) per ribbon-edge vertex
        // and the VS unfolds the width against the camera (pos - w x cross(dir, ...)) and reads
        // the stripe block as bank 10. Which semantic is dir vs pos is per-emitter shader truth,
        // detected from the VS's own normalize (self-dot) of the dir attribute.
        bool stripeMode;
        bool stripeTrail;                       // vertexTransformMode 2 = per-particle history trail; 3 = one connection chain
        int stripePosSem = -1, stripeDirSem = -1;
        int stripeOuterSem = -1, stripeTexSem = -1;   // 3/4-attribute variant: CPU-supplied width vector (+ uv set)
        int stripePosVbo, stripeDirVbo, stripeOuterVbo, stripeTexVbo;
        byte[] stripeBank10;
        const int TrailNodes = 6;               // reconstructed history depth (pos - vel*j; exact for linear motion)
        readonly List<int> instVbos = new List<int>();
        readonly List<int> textures = new List<int>();
        // our textures are (re)bound around every draw and the previous bindings restored:
        // the editor viewport's other drawables (orientation cube, background) share these
        // units, and one-time Init bindings leak both ways (effects sampling the axis-cube
        // texture; the axis cube showing emitter art)
        struct UnitTex { public int Unit; public TextureTarget Target; public int Tex; }
        readonly List<UnitTex> unitTex = new List<UnitTex>();
        Dictionary<string, int> attribSem;      // attr name -> semantic id
        string error;
        bool ready;

        // remapped-mode support (uniformMode=1, ~69% of emitters): these shaders read every
        // uniform through uf_remappedVS instead of the numbered banks. The decompiler's own
        // mapping lists each slot's true source (bank + vec4Index); Render fills the array from
        // the same generated banks the bank-mode path feeds. Unsourced slots default to 1.0
        // (GL's zeros collapse the view rows and multiply whole shader families to black).
        struct RemapEntry { public int Slot, Bank, Index; }
        readonly List<RemapEntry> vsRemap = new List<RemapEntry>();
        readonly List<RemapEntry> psRemap = new List<RemapEntry>();
        int vsRemapLoc = -1, vsRemapDecl;
        int psRemapLoc = -1, psRemapDecl;
        byte[] bank7Bytes;
        int bank7Ubo;
        int rainBank10Ubo;                      // vtxMode-4 emitter block: rows 5/9 track the camera per frame
        byte[] rainBank10Bytes;                 // same block for remapped twins (FieldSnow class), fed via UploadRemapped
        // the alpha1 track (struct+0x4F0 -> bank 0x540) is the emitter-lifecycle envelope:
        // shaders in this family read bank7[84].x as the CURRENT erosion scale (Sem1.w), so the
        // slot is runtime-maintained, not a static curve key: a fade-in curve's key 0 is 0 and
        // erodes the whole draw to nothing. Render patches it per frame from the sim's evaluated
        // envelope, only for shaders that actually read the slot.
        bool readsAlpha1Slot;
        // alpha0 twin: a shader that reads the alpha0 track's key-0 row (bank7[68]) and none of
        // the other key rows gets the runtime-maintained envelope, same law as readsAlpha1Slot.
        bool readsAlpha0Key0Only;
        readonly Dictionary<int, byte[]> staticBankBytes = new Dictionary<int, byte[]>();

        static void ParseRemapEntries(string mapping, List<RemapEntry> into)
        {
            if (mapping == null) return;
            foreach (Match m in Regex.Matches(mapping, @"uf_remapped\[(\d+)\] <- BUFFER bank=(\d+) vec4Index=(\d+)"))
                into.Add(new RemapEntry {
                    Slot = int.Parse(m.Groups[1].Value),
                    Bank = int.Parse(m.Groups[2].Value),
                    Index = int.Parse(m.Groups[3].Value),
                });
        }

        /// <summary>payload = the emitter's DATA-frame bytes (the 0x50 header preceding the
        /// struct frame included; slice file[DataPosition-0x50 ..]). Whether the pixel shader
        /// is lit (samples the HDR environment cube) and which unit every texture binds are
        /// read from the fragment group's own samplerVar table.</summary>
        public EftGpuPreview(byte[] vtxGroup, byte[] fragGroup, byte[] payload,
                             IList<TextureInput> artTextures,
                             float[] meshVerts = null, float[] meshNormals = null, int[] meshIndices = null)
        {
            this.vtxGroup = vtxGroup;
            this.fragGroup = fragGroup;
            this.payload = payload;
            this.art = artTextures != null ? new List<TextureInput>(artTextures) : new List<TextureInput>();
            if (meshVerts != null && meshVerts.Length >= 15 && meshIndices != null && meshIndices.Length >= 3)
            {
                this.meshVerts = meshVerts;
                this.meshNormals = meshNormals;
                this.meshIndices = meshIndices;
            }
            psSamplers = Gx2ShaderDecompiler.ParseSamplerVars(fragGroup);
            int e = 0x10; while (e < 0x40 && payload[e] != 0) e++;
            string name = Encoding.ASCII.GetString(payload, 0x10, e - 0x10);
            var structData = new byte[payload.Length - 0x50];
            Array.Copy(payload, 0x50, structData, 0, structData.Length);
            sim = new EftEmitterRender(new[] { new EftEmitterRender.EmitterInput {
                Name = name, EmtrName = name, Data = structData, MeshVerts = this.meshVerts, MeshIndices = this.meshIndices,
                StreamMode = true } });
        }

        public string Error { get { return error; } }

        /// <summary>Effect extent of the last rendered frame (max instance |pos| + scale), for
        /// camera auto-fit. 0 until a frame with live particles has rendered.</summary>
        public float BoundsRadius { get; private set; }

        /// <summary>GPU-preview inputs from a loaded emitter: the DATA-frame payload (the 0x50 head
        /// cached at file load + the LIVE EmitterData, so parameter/colour edits show), the art
        /// textures decoded to RGBA8 with each texture's own GX2 CompSel as the sampler swizzle, and
        /// the emitter's PRIM mesh (null for billboards; mesh emitters' vertex shaders consume the
        /// mesh verbatim as per-vertex streams, so the unit quad renders nothing for them).
        /// False when the file has no shader bundle (no decompilable groups cached).</summary>
        public static bool BuildInputs(PTCL.Emitter em, out byte[] payload, out List<TextureInput> art,
                                       out float[] meshVerts, out float[] meshNormals, out int[] meshIndices)
        {
            payload = null; art = null; meshVerts = null; meshNormals = null; meshIndices = null;
            if (em == null || em.VtxGroupBytes == null || em.FragGroupBytes == null ||
                em.HeadBytes == null || em.EmitterData == null)
                return false;
            payload = new byte[0x50 + em.EmitterData.Length];
            Array.Copy(em.HeadBytes, 0, payload, 0, 0x50);
            Array.Copy(em.EmitterData, 0, payload, 0x50, em.EmitterData.Length);
            art = new List<TextureInput>();
            for (int slot = 0; slot < 3; slot++)
                art.Add(DecodeArtTexture(em.GetSamplerTexture(slot)));
            var inp = EftEmitterRender.BuildInput(em, null);
            if (inp != null)
            {
                meshVerts = inp.MeshVerts;
                meshNormals = inp.MeshNormals;
                meshIndices = inp.MeshIndices;
            }
            return true;
        }

        static TextureInput DecodeArtTexture(Toolbox.Library.STGenericTexture tex)
        {
            System.Drawing.Bitmap decoded = null;
            if (tex != null)
                try { decoded = tex.GetBitmap(); } catch { }
            if (decoded == null)
                return new TextureInput { Rgba = new byte[] { 255, 255, 255, 255 }, W = 1, H = 1 };   // multiply-identity stand-in
            using (var bmp = decoded)
            {
                // locking as 32bppArgb makes the copy exactly W x H BGRA texels whatever the decoded
                // bitmap's own format is, with no row padding to skip
                var data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var bgra = new byte[bmp.Width * bmp.Height * 4];
                System.Runtime.InteropServices.Marshal.Copy(data.Scan0, bgra, 0, bgra.Length);
                bmp.UnlockBits(data);
                byte[] rgba = Toolbox.Library.STGenericTexture.ConvertBgraToRgba(bgra);
                // the texture's own GX2 CompSel (bytes = output R,G,B,A selectors; 0-3 = R,G,B,A, 4 = zero, 5 = one)
                uint compSel = tex is PTCL.TEXR ? ((PTCL.TEXR)tex).CompSel : 0x00010203u;
                var sw = new char[4];
                for (int c = 0; c < 4; c++)
                {
                    int sel = (int)((compSel >> (24 - c * 8)) & 0xFF);
                    sw[c] = sel >= 0 && sel <= 5 ? "RGBA01"[sel] : "RGBA"[c];
                }
                return new TextureInput { Rgba = rgba, W = bmp.Width, H = bmp.Height, Swizzle = new string(sw) };
            }
        }

        /// <summary>Null when the inputs the preview ships (the EftPreviewAssets folder and the bank7 mask) are
        /// deployed beside the plugin, otherwise what is missing. Without them the game's shaders would still draw,
        /// but with the engine-bound inputs absent, so the editor checks this and stays on the software preview
        /// rather than showing an effect that looks faithful and is not.</summary>
        public static string MissingAsset()
        {
            if (FindAssets() == null) return "EftPreviewAssets folder";
            if (EftUniformBanks.FindMask() == null) return "bank7_mask.txt";
            return null;
        }

        static string FindAssets()
        {
            var dirs = new List<string>();
            try { var loc = typeof(EftGpuPreview).Assembly.Location; if (!string.IsNullOrEmpty(loc)) dirs.Add(Path.GetDirectoryName(loc)); } catch { }
            try { dirs.Add(AppDomain.CurrentDomain.BaseDirectory); } catch { }
            foreach (var d in dirs)
            {
                if (string.IsNullOrEmpty(d)) continue;
                foreach (var c in new[] { Path.Combine(d, "EftPreviewAssets"), Path.Combine(d, "Lib", "Plugins", "EftPreviewAssets") })
                    if (Directory.Exists(c)) return c;
            }
            return null;
        }

        static float F11(uint v)
        {
            uint e = v >> 6, m = v & 63;
            if (e == 0) return (float)(m / 64.0 * Math.Pow(2, -14));
            if (e == 31) return m != 0 ? float.NaN : 65504f;
            return (float)((1.0 + m / 64.0) * Math.Pow(2, (int)e - 15));
        }
        static float F10(uint v)
        {
            uint e = v >> 5, m = v & 31;
            if (e == 0) return (float)(m / 32.0 * Math.Pow(2, -14));
            if (e == 31) return m != 0 ? float.NaN : 65504f;
            return (float)((1.0 + m / 32.0) * Math.Pow(2, (int)e - 15));
        }

        static float[] DecodeR11G11B10(byte[] raw, int px)
        {
            var f = new float[px * 3];
            for (int i = 0; i < px; i++)
            {
                uint u = BitConverter.ToUInt32(raw, i * 4);
                f[i * 3] = F11(u & 0x7FF);
                f[i * 3 + 1] = F11((u >> 11) & 0x7FF);
                f[i * 3 + 2] = F10(u >> 22);
            }
            return f;
        }

        /// <summary>Create a texture for the given unit: bind on that unit, run the upload,
        /// then put the unit's previous binding back (the framework's drawables own these
        /// units between our draws). Render rebinds the recorded list around every draw.</summary>
        int OnUnit(int unit, TextureTarget target, Action fill)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            int prev = GL.GetInteger(target == TextureTarget.TextureCubeMap ? GetPName.TextureBindingCubeMap : GetPName.TextureBinding2D);
            int tex = GL.GenTexture();
            GL.BindTexture(target, tex);
            fill();
            GL.BindTexture(target, prev);
            textures.Add(tex);
            unitTex.Add(new UnitTex { Unit = unit, Target = target, Tex = tex });
            return tex;
        }

        // GX2 wrap enum @ EMTR sampler+0x08(U)/+0x09(V), mapped to GL by EftEmitterRender.WrapMode.
        // GL's default REPEAT tiles any texture sampled outside [0,1], and PRIM meshes do sample
        // outside it (the snow-flake triangle circumscribes the unit square, corner UVs to 1.69).
        int SlotWrap(int slot, int axis)
        {
            int rec = 0x50 + 0x9A8 + slot * 0x20;
            if (payload == null || rec + 0x20 > payload.Length) return (int)TextureWrapMode.ClampToEdge;
            bool pop = false;
            for (int i = 0; i < 8; i++) if (payload[rec + i] != 0xFF) { pop = true; break; }
            return (int)EftEmitterRender.WrapMode(pop ? payload[rec + 8 + axis] : 2);
        }

        /// <summary>Create a uniform buffer holding the given bytes and bind it to a block's binding
        /// point. Returns the buffer name; the caller owns it (Init's banks live with the instance,
        /// Render's per-frame banks are deleted at the end of the frame).</summary>
        static int MakeUniformBuffer(int binding, byte[] data, BufferUsageHint usage)
        {
            int ubo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, ubo);
            GL.BufferData(BufferTarget.UniformBuffer, data.Length, data, usage);
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, binding, ubo);
            return ubo;
        }

        /// <summary>Compile a vertex/fragment GLSL pair into a linked program and drop the shaders.
        /// linkLog is the program info log when the link failed, null when it succeeded.</summary>
        static int BuildProgram(string vsGlsl, string fsGlsl, out string linkLog)
        {
            int v = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(v, vsGlsl); GL.CompileShader(v);
            int f = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(f, fsGlsl); GL.CompileShader(f);
            int p = GL.CreateProgram();
            GL.AttachShader(p, v); GL.AttachShader(p, f); GL.LinkProgram(p);
            int ok; GL.GetProgram(p, GetProgramParameterName.LinkStatus, out ok);
            GL.DeleteShader(v); GL.DeleteShader(f);
            linkLog = ok == 0 ? GL.GetProgramInfoLog(p) : null;
            return p;
        }

        /// <summary>Attribute location the named stream feeds, or -1 when the shader declares
        /// neither the semantic nor its attribute.</summary>
        int AttribLoc(string attrName)
        {
            int sem;
            if (attribSem == null || !attribSem.TryGetValue(attrName, out sem)) return -1;
            return GL.GetAttribLocation(prog, "attrDataSem" + sem);
        }

        /// <summary>Fill the vbo with a float4-per-element stream and point the attribute at it
        /// (float bits through the uvec4 attr, the Cemu in-shader decode ABI).</summary>
        void BindAttrStream(int aloc, int vbo, float[] data, BufferUsageHint usage)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * 4, data, usage);
            GL.EnableVertexAttribArray(aloc);
            GL.VertexAttribIPointer(aloc, 4, VertexAttribIntegerType.UnsignedInt, 0, IntPtr.Zero);
            fedAttribLocs.Add(aloc);
        }

        void Tex2D(TextureInput t, int unit, int slot)
        {
            OnUnit(unit, TextureTarget.Texture2D, () =>
            {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, t.W, t.H, 0,
                          PixelFormat.Rgba, PixelType.UnsignedByte, t.Rgba);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, SlotWrap(slot, 0));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, SlotWrap(slot, 1));
            if (t.Swizzle != null && t.Swizzle != "RGBA")
            {
                var mapc = new Dictionary<char, int> {
                    { 'R', (int)All.Red }, { 'G', (int)All.Green }, { 'B', (int)All.Blue },
                    { 'A', (int)All.Alpha }, { '0', (int)All.Zero }, { '1', (int)All.One } };
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleR, mapc[t.Swizzle[0]]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleG, mapc[t.Swizzle[1]]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleB, mapc[t.Swizzle[2]]);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureSwizzleA, mapc[t.Swizzle[3]]);
            }
            });
        }

        void Init()
        {
            ready = true;
            var cubeUnits = new List<int>();
            foreach (var sv in psSamplers)
                if (sv.Type == 4) cubeUnits.Add(sv.Location);
            var vs = Gx2ShaderDecompiler.DecompileVertex(vtxGroup, fragGroup);          // all-float4 stream ABI
            var ps = Gx2ShaderDecompiler.DecompileFragment(fragGroup, cubeUnits.Count > 0 ? cubeUnits.ToArray() : null);
            if (vs.Error != null || ps.Error != null) { error = vs.Error ?? ps.Error; return; }
            attribSem = vs.AttribVars;
            uint vtxMode = payload.Length > 0x947
                ? (uint)((payload[0x944] << 24) | (payload[0x945] << 16) | (payload[0x946] << 8) | payload[0x947]) : 0;
            if (vtxMode == 2 || vtxMode == 3)
            {
                // attribute ROLES are per-emitter shader truth (twins reorder semantics):
                // dir = the attr the VS self-dots (normalizes); in the 3/4-attr variant the
                // CPU also supplies outer (the width vector) and the VS unfolds the edge as
                // pos.w x outer.xyz: that product pins the (pos, outer) pair, and the leftover
                // 4th attr is the uv set. The 2-attr variant derives outer on the GPU.
                var semReg = new Dictionary<int, int>();
                foreach (Match m in Regex.Matches(vs.Glsl, @"attrDecoder = attrDataSem(\d+);\s*R(\d+)i = ivec4"))
                    semReg[int.Parse(m.Groups[1].Value)] = int.Parse(m.Groups[2].Value);
                Func<int, bool> selfDot = r => Regex.IsMatch(vs.Glsl,
                    @"dot\(vec4\(intBitsToFloat\(R" + r + @"i\.x\),intBitsToFloat\(R" + r + @"i\.y\),intBitsToFloat\(R" + r + @"i\.z\),.*?\),vec4\(intBitsToFloat\(R" + r + @"i\.x\)");
                // pos = the attr whose w drives the edge side-select ((w > 0.0) ? ...);
                // outer (when CPU-supplied) = the attr multiplied component-wise by pos.w;
                // dir = the self-dot-normalized attr among the rest; the leftover is the uv set
                foreach (var kv in semReg)
                    if (Regex.IsMatch(vs.Glsl, @"\(intBitsToFloat\(R" + kv.Value + @"i\.w\) > 0\.0\)"))
                    { stripePosSem = kv.Key; break; }
                if (stripePosSem >= 0)
                    foreach (var o in semReg)
                        if (o.Key != stripePosSem && Regex.IsMatch(vs.Glsl,
                            @"mul_nonIEEE\(intBitsToFloat\(R" + semReg[stripePosSem] + @"i\.w\), intBitsToFloat\(R" + o.Value + @"i\.[xyz]\)\)"))
                        { stripeOuterSem = o.Key; break; }
                foreach (var kv in semReg)
                {
                    if (kv.Key == stripePosSem || kv.Key == stripeOuterSem) continue;
                    if (stripeDirSem < 0 && selfDot(kv.Value)) stripeDirSem = kv.Key;
                    else if (stripeTexSem < 0) stripeTexSem = kv.Key;
                }
                if (stripeDirSem < 0 || stripePosSem < 0)
                {
                    error = "stripe VS variant with " + semReg.Count + " attributes: roles not recognized";
                    return;
                }
                stripeMode = true;
                stripeTrail = vtxMode == 2;
            }
            string psGlsl = ps.Glsl;
            if (psGlsl.Contains("samplerCubeArray"))
            {
                psGlsl = psGlsl.Replace("uniform samplerCubeArray", "uniform samplerCube");
                psGlsl = Regex.Replace(psGlsl, @"vec4\((redcCUBEReverse\([^;]+?\)),cubeMapArrayIndex\d+\)", "($1)");
            }

            string vsGlsl = vs.Glsl;
            string linkLog;
            prog = BuildProgram(vsGlsl, psGlsl, out linkLog);
            if (linkLog != null) { error = "link: " + linkLog; return; }
            GL.UseProgram(prog);
            int loc = GL.GetUniformLocation(prog, "uf_fragCoordScale");
            if (loc >= 0) GL.Uniform2(loc, 1f, 1f);
            // Remapped uniforms (uf_remappedVS/PS): the decompiler's mapping names every slot's
            // bank source; Render fills them per frame from the same generated banks the
            // bank-mode path binds. Unsourced slots stay 1.0; GL's default zeros collapse the
            // view rows and multiply whole shader families (color/alpha/soft-fade) to black.
            vsRemapLoc = GL.GetUniformLocation(prog, "uf_remappedVS");
            if (vsRemapLoc >= 0)
            {
                var mv = Regex.Match(vs.Glsl, @"uf_remappedVS\[(\d+)\]");
                vsRemapDecl = mv.Success ? int.Parse(mv.Groups[1].Value) : 0;
                ParseRemapEntries(vs.Mapping, vsRemap);
            }
            psRemapLoc = GL.GetUniformLocation(prog, "uf_remappedPS");
            if (psRemapLoc >= 0)
            {
                var mp = Regex.Match(psGlsl, @"uf_remappedPS\[(\d+)\]");
                psRemapDecl = mp.Success ? int.Parse(mp.Groups[1].Value) : 0;
                ParseRemapEntries(ps.Mapping, psRemap);
            }
            foreach (var e in vsRemap)
                if (e.Bank == 7 && e.Index == 84) readsAlpha1Slot = true;
            foreach (var e in psRemap)
                if (e.Bank == 7 && e.Index == 84) readsAlpha1Slot = true;
            if (vs.Glsl.Contains("uf_blockVS7[84]") || psGlsl.Contains("uf_blockPS7[84]"))
                readsAlpha1Slot = true;
            // alpha0 twin (bank7[68].x = bank 0x440): a shader that reads ONLY the track's
            // key-0 row gets the runtime-maintained envelope value; readers of rows 69-75
            // evaluate the curve per particle in-shader and keep the static keys (see
            // EftEmitterRender.EnvelopeAlpha0)
            bool reads68 = false, readsA0Tail = false;
            foreach (var e in vsRemap)
                if (e.Bank == 7) { if (e.Index == 68) reads68 = true; else if (e.Index >= 69 && e.Index <= 75) readsA0Tail = true; }
            foreach (var e in psRemap)
                if (e.Bank == 7) { if (e.Index == 68) reads68 = true; else if (e.Index >= 69 && e.Index <= 75) readsA0Tail = true; }
            for (int a0i = 68; a0i <= 75; a0i++)
                if (vs.Glsl.Contains("uf_blockVS7[" + a0i + "]") || psGlsl.Contains("uf_blockPS7[" + a0i + "]"))
                { if (a0i == 68) reads68 = true; else readsA0Tail = true; }
            readsAlpha0Key0Only = reads68 && !readsA0Tail && vtxMode != 4;

            string assets = FindAssets();
            if (assets == null) { error = "EftPreviewAssets folder not found beside the plugin"; return; }

            // texture units from the fragment group's samplerVar table (file truth): art slot N ->
            // sysTextureSamplerN's unit, scene depth (synthetic far plane) -> sysDepthBufferTexture's
            // unit, the shipped HDR environment cube -> the cube-typed unit. Groups without a
            // parseable table fall back to the GLSL declarations: art -> ascending 2D units,
            // leftover 2D units = depth.
            Action<int> synthDepth = unit => OnUnit(unit, TextureTarget.Texture2D, () =>
            {
                // the far plane in the (viewZ - near) / (far - near) encoding, so the soft-particle fade stays open
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32f, 1, 1, 0,
                              PixelFormat.Red, PixelType.Float, new[] { 1.0f });
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            });
            Action<int> envCube = unit =>
            {
                OnUnit(unit, TextureTarget.TextureCubeMap, () =>
                {
                    for (int face = 0; face < 6; face++)
                    {
                        byte[] raw = File.ReadAllBytes(Path.Combine(assets, "envcube_" + face + ".bin"));
                        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + face, 0, PixelInternalFormat.Rgb32f,
                                      16, 16, 0, PixelFormat.Rgb, PixelType.Float, DecodeR11G11B10(raw, 256));
                    }
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                });
            };
            if (psSamplers.Count > 0)
            {
                foreach (var sv in psSamplers)
                {
                    int slot;
                    if (sv.Type == 4)
                        envCube(sv.Location);
                    else if (sv.Name.StartsWith("sysTextureSampler") &&
                             int.TryParse(sv.Name.Substring("sysTextureSampler".Length), out slot) &&
                             slot >= 0 && slot < art.Count && art[slot] != null)
                        Tex2D(art[slot], sv.Location, slot);
                    else if (sv.Name == "sysFrameBufferTexture")
                    {
                        // Scene colour for refraction, matched by EXACT name: the depth buffer and
                        // sysCustomShaderTextureSampler* also fail the art/cube tests and must keep
                        // their far-plane 1x1. The emitter's own shader warps whatever is bound here.
                        // A faithful refraction of the editor's dark, low-contrast scene is nearly
                        // invisible, so the preview refracts a high-contrast reference grid instead
                        // (the refraction math stays the game's own; only the surface being warped is
                        // a preview stand-in). Set EFTPREV_REALSCENE to warp the live viewport
                        // instead (Render grabs it).
                        sceneRefGrid = Environment.GetEnvironmentVariable("EFTPREV_REALSCENE") == null;
                        sceneTex = OnUnit(sceneTexUnit = sv.Location, TextureTarget.Texture2D, () =>
                        {
                            if (sceneRefGrid)
                            {
                                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, RefGridSize, RefGridSize, 0,
                                              PixelFormat.Rgb, PixelType.UnsignedByte, BuildRefGrid());
                                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                            }
                            else
                                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 1, 1, 0,
                                              PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
                            int wrap = sceneRefGrid ? (int)TextureWrapMode.Repeat : (int)TextureWrapMode.ClampToEdge;
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, wrap);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, wrap);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                                            (int)(sceneRefGrid ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear));
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                        });
                    }
                    else
                        synthDepth(sv.Location);
                }
            }
            else
            {
                var ps2d = new List<int>();
                var psCube = new List<int>();
                foreach (Match m in Regex.Matches(psGlsl, @"TEXTURE_LAYOUT\((\d+), 1, \d+\) uniform sampler(\w+) textureUnitPS\d+"))
                {
                    if (m.Groups[2].Value.StartsWith("Cube")) psCube.Add(int.Parse(m.Groups[1].Value));
                    else ps2d.Add(int.Parse(m.Groups[1].Value));
                }
                ps2d.Sort();
                for (int i = 0; i < ps2d.Count; i++)
                {
                    if (i < art.Count && art[i] != null) Tex2D(art[i], ps2d[i], i);
                    else synthDepth(ps2d[i]);
                }
                if (psCube.Count > 0) envCube(psCube[0]);
            }
            // VS lookup tables. Shaders that declare textureUnitVS9 (the vtxMode-4 drop class)
            // get the trio by table number: textureUnitVS8 = the 256x256 RGBA16F random table,
            // textureUnitVS9 = the 192x192 R16 terrain heightmap, textureUnitVS13 = the 12x1
            // env palette. The heightmap is INVERTED (the VS kills drops below
            // bank13[32].w x (1 - height)), so the constant 1.0 puts the terrain at the
            // preview's y=0 grid plane. Every other shader keeps the positional assignment:
            // first declared unit = random table, second = palette.
            var vsDecls = new List<int[]>();
            foreach (Match m in Regex.Matches(vs.Glsl, @"TEXTURE_LAYOUT\((\d+), 0, \d+\) uniform sampler2D textureUnitVS(\d+)"))
                vsDecls.Add(new[] { int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value) });
            Action<int> vsRandomTable = unit => OnUnit(unit, TextureTarget.Texture2D, () =>
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, 256, 256, 0,
                              PixelFormat.Rgba, PixelType.HalfFloat, File.ReadAllBytes(Path.Combine(assets, "randomtable.bin")));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            });
            Action<int> vsEnvPalette = unit => OnUnit(unit, TextureTarget.Texture2D, () =>
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb32f, 12, 1, 0,
                              PixelFormat.Rgb, PixelType.Float,
                              DecodeR11G11B10(File.ReadAllBytes(Path.Combine(assets, "envpalette.bin")), 12));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            });
            if (vsDecls.Exists(t => t[1] == 9))
                foreach (var t in vsDecls)
                {
                    if (t[1] == 8) vsRandomTable(t[0]);
                    else if (t[1] == 13) vsEnvPalette(t[0]);
                    else OnUnit(t[0], TextureTarget.Texture2D, () =>
                    {
                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32f, 1, 1, 0,
                                      PixelFormat.Red, PixelType.Float, new[] { t[1] == 9 ? 1.0f : 0.0f });
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    });
                }
            else
            {
                vsDecls.Sort((a, b) => a[0].CompareTo(b[0]));
                if (vsDecls.Count > 0) vsRandomTable(vsDecls[0][0]);
                if (vsDecls.Count > 1) vsEnvPalette(vsDecls[1][0]);
            }
            GL.ActiveTexture(TextureUnit.Texture0);

            // static banks: env/scene 11/13/14 from assets; every other declared block without
            // data gets zeros (GL leaves unbound blocks undefined)
            var bound = new HashSet<int> { 6, 7, 8 };
            foreach (int b in new[] { 11, 13, 14 })
            {
                string p = Path.Combine(assets, "bank" + b + ".bin");
                if (!File.Exists(p)) continue;
                byte[] d = File.ReadAllBytes(p);
                staticBankBytes[b] = d;
                ubos.Add(MakeUniformBuffer(b, d, BufferUsageHint.StaticDraw));
                bound.Add(b);
            }
            // vtxMode-4 (velocity-aligned rain/snow/wind drops) reads bank 10 as its emitter
            // block: rows 2..5 = emitter world matrix, 6..9 = its inverse, 11 = the wrap box
            // every drop is folded into (the VS divides by the box, so a zero there NaNs the
            // whole class to nothing). The game PARENTS this class to the camera: the drops'
            // view position is built from the matrix translation relative to the camera, so a
            // translation pinned at the origin collapses every drop onto the camera axis.
            // Render refreshes rows 5/9 with the camera position each frame; the box is the
            // emission volume scale @struct+0x80C. The FieldSnow twins compile REMAPPED and
            // read the same block through uf_remappedVS slots (file VS map: bank10[0..11]);
            // UploadRemapped serves them these bytes.
            if (vtxMode == 4 && payload.Length > 0x868)
            {
                var b10 = new byte[192];
                Action<int, float> put10 = (o10, f10) => Array.Copy(BitConverter.GetBytes(f10), 0, b10, o10, 4);
                put10(0x0C, 1f);                                       // [0] second-pass offset, w=1
                for (int r = 0; r < 2; r++)                            // [2..5] matrix, [6..9] inverse
                    for (int c = 0; c < 4; c++)
                        put10((2 + r * 4 + c) * 16 + c * 4, 1f);
                // [11] wrap box = the volume scale, or the weather system's own box where
                // GetWeatherProfile supplies one.
                int nl = 0; while (0x10 + nl < 0x50 && payload[0x10 + nl] != 0) nl++;
                var wxp = EftEmitterRender.GetWeatherProfile(System.Text.Encoding.ASCII.GetString(payload, 0x10, nl), payload, 0x50);
                for (int c = 0; c < 3; c++)
                    put10(11 * 16 + c * 4, wxp != null && wxp.Box != null ? wxp.Box[c] : BitConverter.ToSingle(
                        new byte[] { payload[0x85F + c * 4], payload[0x85E + c * 4], payload[0x85D + c * 4], payload[0x85C + c * 4] }, 0));
                rainBank10Bytes = b10;
                rainBank10Ubo = MakeUniformBuffer(10, b10, BufferUsageHint.StaticDraw);
                ubos.Add(rainBank10Ubo);
                bound.Add(10);
            }
            foreach (Match m in Regex.Matches(vs.Glsl + psGlsl, @"UNIFORM_BUFFER_LAYOUT\((\d+), \d+, \d+\) uniform uniformBlock"))
            {
                int b = int.Parse(m.Groups[1].Value);
                if (bound.Contains(b)) continue;
                ubos.Add(MakeUniformBuffer(b, new byte[65536], BufferUsageHint.StaticDraw));
                bound.Add(b);
            }

            // EmitterStatic = the emitter's own payload (BuildBank7).
            byte[] b7 = EftUniformBanks.BuildBank7(payload);
            // the 4-attribute stripe VS gates its main path on flag word 0 bit 30
            // (bank7[5].x & 0x40000000); DeriveFlagWords only produces bits 0-27, so set it here.
            if (stripeMode)
                b7[0x53] |= 0x40;   // flag word 0 @ bank 0x50, LE: bit 30 lives in byte 3
            // Refraction strength (the _ind/Dist class): the PS multiplies its scene-warp offset by
            // uf_remappedPS[0].xy <- bank7[16] (bank 0x100). The disk usually carries the strength
            // pair there (0.005 to 0.15), but for the ambient InWater_*/SwimDash family it is ZERO on
            // disk and the runtime defaults the y/z/w words to 1.0; without that the warp multiplies
            // to 0 and the whole class refracts nothing. Gated to shaders that sample the scene,
            // since zero-disk non-refractive shaders keep the zeros.
            if (b7.Length >= 0x110 && BitConverter.ToUInt32(b7, 0x100) == 0 && BitConverter.ToUInt32(b7, 0x104) == 0)
                foreach (var sv in psSamplers)
                    if (sv.Name == "sysFrameBufferTexture")
                    {
                        var one = BitConverter.GetBytes(1.0f);
                        for (int o = 0x104; o <= 0x10C; o += 4) Array.Copy(one, 0, b7, o, 4);
                        break;
                    }
            // vtxMode-4: the runtime holds 0 in bank7[68].x for this class regardless of the disk
            // key. The rain VS scales a positional displacement by this word x bank8[0].w, so a 1
            // here throws every drop off-screen. The class is also excluded from the per-frame
            // EnvelopeAlpha0 patch, which would write the evaluated envelope back over this zero.
            if (vtxMode == 4 && b7.Length >= 0x444)
                Array.Clear(b7, 0x440, 4);
            // bank7[48] = (uvScaleInit.x, uvScaleInit.y, numU, numV): the VSes compute the uv cell
            // scale as x/z and y/w, so a 0 numerator collapses u to a constant and the sample sits
            // on one atlas column / the texture's transparent border. The disk holds x=0 on every
            // emitter and the runtime writes it at emitter init, so default it to 1. The build
            // already fills y.
            if (b7.Length >= 0x304 && BitConverter.ToSingle(b7, 0x300) == 0f)
                Array.Copy(BitConverter.GetBytes(1.0f), 0, b7, 0x300, 4);
            bank7Ubo = MakeUniformBuffer(7, b7, BufferUsageHint.StaticDraw);
            ubos.Add(bank7Ubo);
            bank7Bytes = b7;

            // geometry: PRIM mesh emitters get their mesh verbatim as per-vertex streams
            // (sysPosAttr = mesh position w=1, sysNormalAttr/sysTexCoordAttr = mesh data;
            // feeding the quad renders nothing for this class); billboards get the unit
            // quad (sysPosAttr = corner (+-0.5, +-0.5, 0, index))
            owner = GraphicsContext.CurrentContext;
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            if (stripeMode)
            {
                // ribbon vertices are rebuilt from the sim chain every frame (see BuildStripeGeometry)
                stripePosVbo = GL.GenBuffer(); vertVbos.Add(stripePosVbo);
                stripeDirVbo = GL.GenBuffer(); vertVbos.Add(stripeDirVbo);
                if (stripeOuterSem >= 0) { stripeOuterVbo = GL.GenBuffer(); vertVbos.Add(stripeOuterVbo); }
                if (stripeTexSem >= 0) { stripeTexVbo = GL.GenBuffer(); vertVbos.Add(stripeTexVbo); }
                ibo = GL.GenBuffer();
                drawIdxCount = 0;
            }
            else if (meshVerts != null)
            {
                int nv = meshVerts.Length / 5;
                var pos = new float[nv * 4];
                var uv = new float[nv * 4];
                var nrm = meshNormals != null && meshNormals.Length >= nv * 3 ? new float[nv * 4] : null;
                for (int i = 0; i < nv; i++)
                {
                    pos[i*4] = meshVerts[i*5]; pos[i*4+1] = meshVerts[i*5+1]; pos[i*4+2] = meshVerts[i*5+2]; pos[i*4+3] = 1f;
                    uv[i*4] = meshVerts[i*5+3]; uv[i*4+1] = meshVerts[i*5+4];
                    if (nrm != null) { nrm[i*4] = meshNormals[i*3]; nrm[i*4+1] = meshNormals[i*3+1]; nrm[i*4+2] = meshNormals[i*3+2]; }
                }
                UploadVertexStream("sysPosAttr", pos);
                UploadVertexStream("sysTexCoordAttr", uv);
                if (nrm != null) UploadVertexStream("sysNormalAttr", nrm);
                ibo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, meshIndices.Length * 4, meshIndices, BufferUsageHint.StaticDraw);
                drawIdxCount = meshIndices.Length;
                drawIdxType = DrawElementsType.UnsignedInt;
            }
            else
            {
                float[] quad = { -0.5f, 0.5f, 0, 0, -0.5f, -0.5f, 0, 1, 0.5f, -0.5f, 0, 2, 0.5f, 0.5f, 0, 3 };
                UploadVertexStream("sysPosAttr", quad);
                // per-vertex texcoords for VSes that read sysTexCoordAttr on billboards (v = 0 at
                // top, matching the corner order above)
                float[] quadUv = { 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 0, 0 };
                UploadVertexStream("sysTexCoordAttr", quadUv);
                ushort[] idx = { 0, 1, 2, 0, 2, 3 };
                ibo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, idx.Length * 2, idx, BufferUsageHint.StaticDraw);
                drawIdxCount = 6;
                drawIdxType = DrawElementsType.UnsignedShort;
            }
        }

        /// <summary>Upload a float4-per-vertex stream to the named attribute (float bits through
        /// the uvec4 attr, the Cemu in-shader decode ABI). Returns the vbo (0 if undeclared).</summary>
        int UploadVertexStream(string attrName, float[] data)
        {
            int aloc = AttribLoc(attrName);
            if (aloc < 0) return 0;
            int vbo = GL.GenBuffer();
            BindAttrStream(aloc, vbo, data, BufferUsageHint.StaticDraw);
            vertVbos.Add(vbo);
            return vbo;
        }

        /// <summary>Build the ribbon from the sim chain: particles ordered by birth are the
        /// stripe nodes; each node emits the two edge vertices (pos.w = +-scale selects the
        /// side, the VS unfolds the width against the camera; dir = chain tangent, normalized
        /// in-shader). Fills the stripe block (bank 10) with NW4F StripeUniformBlock-shaped
        /// defaults: stParam-class flags/edge-Us, 0..1 uv range, white vertex colors, and the
        /// stripe age in [4].x (it keys every bank7 curve slope). Returns the bank-10 UBO.</summary>
        int BuildStripeGeometry(EftEmitterRender.InstanceStreams s, int frame)
        {
            int n = s.Count;
            var order = new int[n];
            var birth = new float[n];
            for (int i = 0; i < n; i++) { order[i] = i; birth[i] = s.LocalVec[i * 4 + 3]; }
            Array.Sort(birth, order);

            float[] pos, dir;
            ushort[] idx;
            if (stripeTrail)
            {
                // one ribbon per particle along its own history, reconstructed first-order
                // from the current position and velocity (node j = pos - vel*j)
                n = Math.Min(n, ushort.MaxValue / (TrailNodes * 2));   // 16-bit index space
                pos = new float[n * TrailNodes * 2 * 4];
                dir = new float[n * TrailNodes * 2 * 4];
                idx = new ushort[n * (TrailNodes - 1) * 6];
                for (int i = 0; i < n; i++)
                {
                    float vx = s.LocalVec[i * 4], vy = s.LocalVec[i * 4 + 1], vz = s.LocalVec[i * 4 + 2];
                    float dx = vx, dy = vy, dz = vz;
                    if (dx == 0 && dy == 0 && dz == 0) dy = 1;
                    float w = Math.Max(s.Scale[i * 4], 0.0001f);
                    float span = Math.Min(frame - s.LocalVec[i * 4 + 3], TrailNodes - 1);   // no nodes before birth
                    for (int k = 0; k < TrailNodes; k++)
                    {
                        float back = span * k / (TrailNodes - 1);
                        float px = s.LocalPos[i * 4] - vx * back;
                        float py = s.LocalPos[i * 4 + 1] - vy * back;
                        float pz = s.LocalPos[i * 4 + 2] - vz * back;
                        for (int side = 0; side < 2; side++)
                        {
                            int o = ((i * TrailNodes + k) * 2 + side) * 4;
                            pos[o] = px; pos[o + 1] = py; pos[o + 2] = pz; pos[o + 3] = side == 0 ? w : -w;
                            dir[o] = dx; dir[o + 1] = dy; dir[o + 2] = dz; dir[o + 3] = 1f;
                        }
                    }
                    for (int k = 0; k < TrailNodes - 1; k++)
                    {
                        int o = (i * (TrailNodes - 1) + k) * 6, v = (i * TrailNodes + k) * 2;
                        idx[o] = (ushort)v; idx[o + 1] = (ushort)(v + 1); idx[o + 2] = (ushort)(v + 2);
                        idx[o + 3] = (ushort)(v + 1); idx[o + 4] = (ushort)(v + 3); idx[o + 5] = (ushort)(v + 2);
                    }
                }
            }
            else
            {
                // connection stripe: the birth-ordered live particles are the chain nodes
                pos = new float[n * 2 * 4];
                dir = new float[n * 2 * 4];
                for (int k = 0; k < n; k++)
                {
                    int i = order[k], prev = order[Math.Max(k - 1, 0)], next = order[Math.Min(k + 1, n - 1)];
                    float px = s.LocalPos[i * 4], py = s.LocalPos[i * 4 + 1], pz = s.LocalPos[i * 4 + 2];
                    float dx = s.LocalPos[next * 4] - s.LocalPos[prev * 4];
                    float dy = s.LocalPos[next * 4 + 1] - s.LocalPos[prev * 4 + 1];
                    float dz = s.LocalPos[next * 4 + 2] - s.LocalPos[prev * 4 + 2];
                    if (dx == 0 && dy == 0 && dz == 0) dy = 1;
                    float w = Math.Max(s.Scale[i * 4], 0.0001f);
                    for (int side = 0; side < 2; side++)
                    {
                        int o = (k * 2 + side) * 4;
                        pos[o] = px; pos[o + 1] = py; pos[o + 2] = pz; pos[o + 3] = side == 0 ? w : -w;
                        dir[o] = dx; dir[o + 1] = dy; dir[o + 2] = dz; dir[o + 3] = 1f;
                    }
                }
                idx = new ushort[(n - 1) * 6];
                for (int k = 0; k < n - 1; k++)
                {
                    int o = k * 6, v = k * 2;
                    idx[o] = (ushort)v; idx[o + 1] = (ushort)(v + 1); idx[o + 2] = (ushort)(v + 2);
                    idx[o + 3] = (ushort)(v + 1); idx[o + 4] = (ushort)(v + 3); idx[o + 5] = (ushort)(v + 2);
                }
            }
            UploadStripeAttr(stripePosSem, stripePosVbo, pos);
            UploadStripeAttr(stripeDirSem, stripeDirVbo, dir);
            if (stripeOuterSem >= 0 || stripeTexSem >= 0)
            {
                // 3/4-attribute variant: the CPU supplies the width vector too. NW4F derives it
                // from the emitter matrix's up axis (identity here) crossed with the slice
                // direction; texCoord = (edge U, V along the ribbon) for both uv channels.
                int verts = pos.Length / 4;
                int ribbonNodes = stripeTrail ? TrailNodes : Math.Max(n, 2);
                var outer = stripeOuterSem >= 0 ? new float[verts * 4] : null;
                var tex = stripeTexSem >= 0 ? new float[verts * 4] : null;
                for (int v = 0; v < verts; v++)
                {
                    int o = v * 4;
                    if (outer != null)
                    {
                        float ox = dir[o + 2], oz = -dir[o];           // cross(up, dir)
                        float len = (float)Math.Sqrt(ox * ox + oz * oz);
                        if (len < 1e-6f) { ox = 1; oz = 0; len = 1; }  // dir parallel to up
                        outer[o] = ox / len; outer[o + 1] = 0; outer[o + 2] = oz / len; outer[o + 3] = 1f;
                    }
                    if (tex != null)
                    {
                        int node = (v / 2) % ribbonNodes;
                        float va = ribbonNodes > 1 ? node / (float)(ribbonNodes - 1) : 0f;
                        float eu = (v & 1) == 0 ? 0f : 1f;
                        tex[o] = eu; tex[o + 1] = va; tex[o + 2] = eu; tex[o + 3] = va;
                    }
                }
                if (outer != null) UploadStripeAttr(stripeOuterSem, stripeOuterVbo, outer);
                if (tex != null) UploadStripeAttr(stripeTexSem, stripeTexVbo, tex);
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, idx.Length * 2, idx, BufferUsageHint.StreamDraw);
            drawIdxCount = idx.Length;
            drawIdxType = DrawElementsType.UnsignedShort;

            float age = Math.Max(frame - birth[0], 0f);
            // [1].xy = U range along the ribbon (the 2-attr VS lerps -x+y), [1].z = WIDTH
            // multiplier (the 4-attr VS computes its edge scale as ([1].z + f)/2, so a zero here
            // collapses every ribbon of that class to a line). [4] = (age, nodeCount, 1, 1): the
            // VS derives the along-ribbon parameter as (vertexID/2) / ([4].y - 1), so nodeCount
            // must be the real count and at least 2.
            int b10Nodes = Math.Max(pos.Length / 8, 2);
            var b10f = new float[] { 1, 0, 0, 1,  0, 1, 1, 1,  1, 1, 1, 1,  1, 1, 1, 1,  age, b10Nodes, 1, 1 };
            stripeBank10 = new byte[b10f.Length * 4];
            System.Buffer.BlockCopy(b10f, 0, stripeBank10, 0, stripeBank10.Length);
            return MakeUniformBuffer(10, stripeBank10, BufferUsageHint.StreamDraw);
        }

        void UploadStripeAttr(int sem, int vbo, float[] data)
        {
            int aloc = GL.GetAttribLocation(prog, "attrDataSem" + sem);
            if (aloc < 0) return;
            BindAttrStream(aloc, vbo, data, BufferUsageHint.StreamDraw);
        }

        void UploadRemapped(int loc, int decl, List<RemapEntry> entries, byte[] b6, byte[] b8)
        {
            if (loc < 0 || decl <= 0) return;
            var blob = new int[decl * 4];
            for (int i = 0; i < blob.Length; i++) blob[i] = 0x3F800000;   // 1.0f bits (the GLSL array is ivec4)
            foreach (var e in entries)
            {
                byte[] src = e.Bank == 6 ? b6 : e.Bank == 7 ? bank7Bytes : e.Bank == 8 ? b8
                           : e.Bank == 10 ? (rainBank10Bytes ?? stripeBank10) : null;
                if (src == null) staticBankBytes.TryGetValue(e.Bank, out src);
                int off = e.Index * 16;
                if (src == null || e.Slot < 0 || e.Slot >= decl || off + 16 > src.Length) continue;
                for (int c = 0; c < 4; c++)
                    blob[e.Slot * 4 + c] = BitConverter.ToInt32(src, off + c * 4);
            }
            GL.Uniform4(loc, decl, blob);
        }

        /// <summary>Write the sim's evaluated envelope into the bank7 word at byte offset off, in
        /// both the CPU copy the remapped path reads and the bound UBO. A NaN envelope (the emitter
        /// has no curve) leaves the file's static key in place.</summary>
        void PatchBank7Envelope(int off, Func<float> envelope)
        {
            if (bank7Bytes == null || bank7Bytes.Length < off + 4) return;
            float env = envelope();
            if (float.IsNaN(env)) return;
            var bits = BitConverter.GetBytes(env);
            Array.Copy(bits, 0, bank7Bytes, off, 4);
            GL.BindBuffer(BufferTarget.UniformBuffer, bank7Ubo);
            GL.BufferSubData(BufferTarget.UniformBuffer, (IntPtr)off, 4, bits);
        }

        static readonly string[] StreamNames = { "sysInPos", "sysInVec", "sysLocalPosAttr", "sysLocalVecAttr", "sysScaleAttr", "sysRandomAttr",
                                                 "sysEmtMat0Attr", "sysEmtMat1Attr", "sysEmtMat2Attr" };

        static readonly Dictionary<string, float[]> AttrNeutral = new Dictionary<string, float[]> {
            { "sysVertexColor0Attr", new float[] { 1, 1, 1, 1 } },
            { "sysNormalAttr",       new float[] { 0, 0, 1, 0 } },
            { "sysTangentAttr",      new float[] { 1, 0, 0, 0 } },
            { "sysInitRotateAttr",   new float[] { 0, 0, 0, 0 } },
            { "sysLocalDiffAttr",    new float[] { 0, 0, 0, 0 } },
        };
        static readonly float[] AttrGenericDefault = { 0, 0, 0, 1 };

        /// <summary>High-contrast reference surface the refraction preview warps when no live scene
        /// is grabbed: a neutral grid (light lines on mid-grey) so a distortion emitter's warp reads
        /// clearly against the editor's otherwise near-featureless background. Screen-space, tiled.</summary>
        static byte[] BuildRefGrid()
        {
            const int cell = 32;                                  // grid spacing in texels
            var px = new byte[RefGridSize * RefGridSize * 3];
            for (int y = 0; y < RefGridSize; y++)
                for (int x = 0; x < RefGridSize; x++)
                {
                    bool line = (x % cell) < 2 || (y % cell) < 2;
                    bool major = (x % (cell * 4)) < 2 || (y % (cell * 4)) < 2;
                    byte v = major ? (byte)210 : line ? (byte)150 : (byte)62;
                    int o = (y * RefGridSize + x) * 3;
                    px[o] = v; px[o + 1] = (byte)(v + 6 < 255 ? v + 6 : 255); px[o + 2] = (byte)(v + 16 < 255 ? v + 16 : 255);
                }
            return px;
        }

        static float[] RepeatRow(int count, float x, float y, float z, float w)
        {
            var rows = new float[count * 4];
            for (int i = 0; i < count; i++)
            {
                rows[i * 4] = x; rows[i * 4 + 1] = y; rows[i * 4 + 2] = z; rows[i * 4 + 3] = w;
            }
            return rows;
        }

        /// <summary>Create (or resize) the RGBA16F accumulation buffer to the viewport size.
        /// Restores the previously-bound texture and framebuffer.</summary>
        void EnsureHdrTarget(int w, int h)
        {
            if (w <= 0 || h <= 0 || (hdrFbo != 0 && w == hdrW && h == hdrH)) return;
            hdrW = w; hdrH = h;
            if (hdrTex == 0) hdrTex = GL.GenTexture();
            int prevBind = GL.GetInteger(GetPName.TextureBinding2D);
            GL.BindTexture(TextureTarget.Texture2D, hdrTex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, w, h, 0,
                          PixelFormat.Rgba, PixelType.HalfFloat, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.BindTexture(TextureTarget.Texture2D, prevBind);
            if (hdrFbo == 0) hdrFbo = GL.GenFramebuffer();
            int prevFbo = GL.GetInteger(GetPName.FramebufferBinding);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, hdrFbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2D, hdrTex, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, prevFbo);
        }

        // BotW display tonemap, read from the game's tonemap pixel shader: 1 - exp(-x) with the
        // exp2 coefficient == log2(e) (unit exposure), applied as a blend of a luminance-preserving
        // term (keeps mid-tone hue) and a per-channel term (drives highlights to white), the blend
        // weight being the tonemapped luminance squared. A fullscreen triangle from gl_VertexID.
        const string ResolveVs =
            "#version 430\nout vec2 vUv;\n" +
            "void main(){ vec2 p = vec2((gl_VertexID<<1)&2, gl_VertexID&2); vUv = p;" +
            " gl_Position = vec4(p*2.0-1.0, 0.0, 1.0); }\n";
        // passthrough=1 skips the tonemap: distortion/refraction emitters output scene colour
        // sampled from the already-tonemapped framebuffer, so tonemapping it again would darken the
        // refracted patch below the surrounding scene. Their coverage still composites normally.
        const string ResolveFs =
            "#version 430\nin vec2 vUv; out vec4 o;\nuniform sampler2D hdrTex; uniform int additive; uniform int passthrough;\n" +
            "vec3 tm(vec3 c){ float L = dot(c, vec3(0.2989,0.5866,0.1144)); float Ld = 1.0-exp(-L);" +
            " vec3 lp = L > 1e-6 ? c*(Ld/L) : c; vec3 pc = 1.0-exp(-c); float t = Ld*Ld;" +
            " return clamp(mix(lp, pc, t), 0.0, 1.0); }\n" +
            "vec3 map(vec3 c){ return passthrough==1 ? c : tm(c); }\n" +
            "void main(){ vec4 h = texture(hdrTex, vUv);" +
            " if(additive==1){ o = vec4(map(h.rgb), 0.0); }" +
            " else { vec3 col = h.a > 1e-4 ? h.rgb/h.a : h.rgb; o = vec4(map(col)*h.a, h.a); } }\n";

        void EnsureResolveProgram()
        {
            if (resolveProg != 0) return;
            string linkLog;
            resolveProg = BuildProgram(ResolveVs, ResolveFs, out linkLog);
            resolveVao = GL.GenVertexArray();
        }

        /// <summary>Draw the emitter at the given playback frame into the CURRENT framebuffer.
        /// view/proj are row-major 4x4 row-vector matrices (the view-bank convention).</summary>
        public void Render(double[] view, double[] proj, int frame)
        {
            if (!ready)
            {
                try { Init(); }
                catch (Exception ex) { error = ex.Message; }   // missing asset/sidecar/decompiler -> report, never throw
            }
            if (error != null) return;
            GL.UseProgram(prog);
            GL.BindVertexArray(vao);

            byte[] b6 = EftUniformBanks.BuildBank6(view, proj);
            int b6ubo = MakeUniformBuffer(6, b6, BufferUsageHint.StaticDraw);
            byte[] b8 = EftUniformBanks.BuildBank8(frame, 1.5f,
                new float[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0 });
            int b8ubo = MakeUniformBuffer(8, b8, BufferUsageHint.StaticDraw);

            // vtxMode-4: the game parents this class to the camera, so the emitter block's
            // matrix translation (row 5) and inverse translation (row 9) follow the camera
            // position, bank6's row 17 (see the Init bank10 note; a static translation
            // collapses every drop onto the camera axis)
            if (rainBank10Ubo != 0)
            {
                var camRow = new byte[16];
                Array.Copy(b6, 17 * 16, camRow, 0, 12);
                Array.Copy(BitConverter.GetBytes(1f), 0, camRow, 12, 4);
                var invRow = new byte[16];
                for (int c = 0; c < 3; c++)
                    Array.Copy(BitConverter.GetBytes(-BitConverter.ToSingle(camRow, c * 4)), 0, invRow, c * 4, 4);
                Array.Copy(BitConverter.GetBytes(1f), 0, invRow, 12, 4);
                GL.BindBuffer(BufferTarget.UniformBuffer, rainBank10Ubo);
                GL.BufferSubData(BufferTarget.UniformBuffer, (IntPtr)(5 * 16), 16, camRow);
                GL.BufferSubData(BufferTarget.UniformBuffer, (IntPtr)(9 * 16), 16, invRow);
                Array.Copy(camRow, 0, rainBank10Bytes, 5 * 16, 16);
                Array.Copy(invRow, 0, rainBank10Bytes, 9 * 16, 16);
            }

            var s = sim.BuildInstanceStreams(0, frame);
            if (s.Count == 0 || (stripeMode && !stripeTrail && s.Count < 2))
            {
                // leaving our VAO bound would let the framework's VAO-less drawables
                // scribble attribute state into it between frames
                GL.BindVertexArray(0);
                GL.DeleteBuffer(b6ubo); GL.DeleteBuffer(b8ubo);
                return;
            }
            int b10ubo = 0;
            if (stripeMode)
                b10ubo = BuildStripeGeometry(s, frame);

            // emitter-lifecycle envelope: shaders that read bank7[84].x (the alpha1 key-0
            // slot) expect the runtime's CURRENT envelope value there, not the static key
            if (readsAlpha1Slot)
                PatchBank7Envelope(0x540, () => sim.EnvelopeAlpha1(0, frame));
            // alpha0 twin: key0-only readers get the evaluated alpha0 envelope in bank 0x440
            if (readsAlpha0Key0Only)
                PatchBank7Envelope(0x440, () => sim.EnvelopeAlpha0(0, frame));

            // remapped uniforms: fill from the same bank contents, per each shader's own mapping
            // (bank 6/8/10 slots change every frame: camera, clock, stripe age)
            UploadRemapped(vsRemapLoc, vsRemapDecl, vsRemap, b6, b8);
            UploadRemapped(psRemapLoc, psRemapDecl, psRemap, b6, b8);

            float maxR = 0;
            for (int i = 0; i < s.Count; i++)
            {
                float px = Math.Abs(s.LocalPos[i * 4]), py = Math.Abs(s.LocalPos[i * 4 + 1]), pz = Math.Abs(s.LocalPos[i * 4 + 2]);
                float r = Math.Max(px, Math.Max(py, pz)) + Math.Max(s.Scale[i * 4], s.Scale[i * 4 + 1]);
                if (r > maxR) maxR = r;
            }
            if (maxR > BoundsRadius) BoundsRadius = maxR;
            foreach (int vb in instVbos) GL.DeleteBuffer(vb);
            instVbos.Clear();
            if (!stripeMode)
            {
                // emitter world matrix rows (== the bank8[4..6] class): identity, the preview
                // places the emitter at the origin
                var streams = new float[][] { s.InPos, s.InVec, s.LocalPos, s.LocalVec, s.Scale, s.Random,
                                              RepeatRow(s.Count, 1, 0, 0, 0), RepeatRow(s.Count, 0, 1, 0, 0), RepeatRow(s.Count, 0, 0, 1, 0) };
                for (int i = 0; i < StreamNames.Length; i++)
                {
                    int aloc = AttribLoc(StreamNames[i]);
                    if (aloc < 0) continue;
                    int vb = GL.GenBuffer();
                    BindAttrStream(aloc, vb, streams[i], BufferUsageHint.StreamDraw);
                    GL.VertexAttribDivisor(aloc, 1);
                    instVbos.Add(vb);
                }
            }

            // deterministic generic values for EVERY declared attribute nothing feeds. The
            // generic slot is CONTEXT state, not VAO state, so whatever the previous preview
            // instance left there is what an uncovered attribute reads. Known semantics get
            // their neutral (GL's zero default multiplies vertex-colored families to black);
            // the rest get GL's initial (0,0,0,1) explicitly.
            if (attribSem != null)
                foreach (var kv in attribSem)
                {
                    int aloc = GL.GetAttribLocation(prog, "attrDataSem" + kv.Value);
                    if (aloc < 0 || fedAttribLocs.Contains(aloc)) continue;
                    float[] nv;
                    if (!AttrNeutral.TryGetValue(kv.Key, out nv)) nv = AttrGenericDefault;
                    GL.VertexAttribI4(aloc,
                        BitConverter.ToInt32(BitConverter.GetBytes(nv[0]), 0),
                        BitConverter.ToInt32(BitConverter.GetBytes(nv[1]), 0),
                        BitConverter.ToInt32(BitConverter.GetBytes(nv[2]), 0),
                        BitConverter.ToInt32(BitConverter.GetBytes(nv[3]), 0));
                }

            // Save the raster/target state both passes touch and put it back afterwards; it is
            // context state shared with the framework's other drawables (same leak class as the
            // texture bindings)
            bool wasBlend = GL.IsEnabled(EnableCap.Blend);
            bool wasDepth = GL.IsEnabled(EnableCap.DepthTest);
            bool wasCull = GL.IsEnabled(EnableCap.CullFace);
            int prevDepthFunc = GL.GetInteger(GetPName.DepthFunc);
            int prevBlendSrcRgb = GL.GetInteger(GetPName.BlendSrcRgb), prevBlendDstRgb = GL.GetInteger(GetPName.BlendDstRgb);
            int prevBlendSrcA = GL.GetInteger(GetPName.BlendSrcAlpha), prevBlendDstA = GL.GetInteger(GetPName.BlendDstAlpha);
            int prevFbo = GL.GetInteger(GetPName.FramebufferBinding);
            var prevVp = new int[4]; GL.GetInteger(GetPName.Viewport, prevVp);
            var prevClear = new float[4]; GL.GetFloat(GetPName.ColorClearValue, prevClear);
            bool additive = payload.Length > 0x8DD && payload[0x8DD] == 1;
            EnsureHdrTarget(prevVp[2], prevVp[3]);
            EnsureResolveProgram();

            // distortion/refraction: copy the caller's rendered scene (grid + background, drawn this
            // frame in the still-bound framebuffer by the opaque pass; the effect is a TRANSPARENT-
            // pass drawable) into sceneTex, so pass 1's sysFrameBufferTexture sampler warps the real
            // scene. prevFbo is the read source here, before pass 1 switches to the offscreen target.
            // The screen UV is projective (VS-passed passParameterSem3.xy/w, same bank-6 projection as
            // gl_Position), so the grab needs no flip or fragCoord scale; it maps 1:1 to the viewport.
            if (sceneTexUnit >= 0 && sceneTex != 0 && !sceneRefGrid && prevVp[2] > 0 && prevVp[3] > 0)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + sceneTexUnit);
                int prevSceneBind = GL.GetInteger(GetPName.TextureBinding2D);
                GL.BindTexture(TextureTarget.Texture2D, sceneTex);
                if (prevVp[2] != sceneW || prevVp[3] != sceneH)
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, prevVp[2], prevVp[3], 0,
                                  PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
                    sceneW = prevVp[2]; sceneH = prevVp[3];
                }
                GL.CopyTexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, prevVp[0], prevVp[1], prevVp[2], prevVp[3]);
                GL.BindTexture(TextureTarget.Texture2D, prevSceneBind);
                GL.ActiveTexture(TextureUnit.Texture0);
            }

            // pass 1: accumulate the emitter in HDR (blendType@struct 0x88D = payload 0x8DD).
            // Additive sums src*srcAlpha; alpha composites over transparent black as premultiplied
            // colour with coverage in A (the resolve un-premultiplies it). No scene depth exists in
            // the offscreen buffer, so the floor does not occlude the effect; acceptable for a
            // standalone effect preview, and the soft-particle fade still works off its own texture.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, hdrFbo);
            GL.Viewport(0, 0, hdrW, hdrH);
            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Enable(EnableCap.Blend);
            GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha,
                additive ? BlendingFactorDest.One : BlendingFactorDest.OneMinusSrcAlpha,
                additive ? BlendingFactorSrc.Zero : BlendingFactorSrc.One,
                BlendingFactorDest.OneMinusSrcAlpha);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
            GL.Disable(EnableCap.CullFace);

            // bind our textures for this draw only; the viewport's other drawables share the
            // low units, so put every unit's previous binding back afterwards
            var prevTex = new int[unitTex.Count];
            for (int i = 0; i < unitTex.Count; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + unitTex[i].Unit);
                prevTex[i] = GL.GetInteger(unitTex[i].Target == TextureTarget.TextureCubeMap ? GetPName.TextureBindingCubeMap : GetPName.TextureBinding2D);
                GL.BindTexture(unitTex[i].Target, unitTex[i].Tex);
            }

            GL.DrawElementsInstanced(PrimitiveType.Triangles, drawIdxCount, drawIdxType, IntPtr.Zero, stripeMode ? 1 : s.Count);

            for (int i = 0; i < unitTex.Count; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + unitTex[i].Unit);
                GL.BindTexture(unitTex[i].Target, prevTex[i]);
            }

            // pass 2: tonemap-resolve the HDR buffer onto the caller's framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, prevFbo);
            GL.Viewport(prevVp[0], prevVp[1], prevVp[2], prevVp[3]);
            GL.UseProgram(resolveProg);
            const int hdrUnit = 15;
            GL.ActiveTexture(TextureUnit.Texture0 + hdrUnit);
            int prevHdrBind = GL.GetInteger(GetPName.TextureBinding2D);
            GL.BindTexture(TextureTarget.Texture2D, hdrTex);
            GL.Uniform1(GL.GetUniformLocation(resolveProg, "hdrTex"), hdrUnit);
            GL.Uniform1(GL.GetUniformLocation(resolveProg, "additive"), additive ? 1 : 0);
            GL.Uniform1(GL.GetUniformLocation(resolveProg, "passthrough"), sceneTexUnit >= 0 ? 1 : 0);
            GL.Enable(EnableCap.Blend);
            // additive emission adds over the scene; alpha composites the un-premultiplied colour
            GL.BlendFunc(BlendingFactor.One, additive ? BlendingFactor.One : BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(resolveVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            GL.BindTexture(TextureTarget.Texture2D, prevHdrBind);
            GL.ActiveTexture(TextureUnit.Texture0);

            // restore all touched context state
            GL.ClearColor(prevClear[0], prevClear[1], prevClear[2], prevClear[3]);
            GL.DepthMask(true);
            if (!wasBlend) GL.Disable(EnableCap.Blend); else GL.Enable(EnableCap.Blend);
            if (wasDepth) GL.Enable(EnableCap.DepthTest); else GL.Disable(EnableCap.DepthTest);
            if (wasCull) GL.Enable(EnableCap.CullFace);
            GL.DepthFunc((DepthFunction)prevDepthFunc);
            GL.BlendFuncSeparate((BlendingFactorSrc)prevBlendSrcRgb, (BlendingFactorDest)prevBlendDstRgb,
                                 (BlendingFactorSrc)prevBlendSrcA, (BlendingFactorDest)prevBlendDstA);
            GL.BindVertexArray(0);
            GL.DeleteBuffer(b6ubo);
            GL.DeleteBuffer(b8ubo);
            if (b10ubo != 0) GL.DeleteBuffer(b10ubo);
        }

        /// <summary>Frees the preview's GL objects. Buffers, textures and programs are shared across the
        /// viewports' contexts (GraphicsContext.ShareContexts), so any current context can delete them.
        /// VAOs and FBOs are containers, private to the context that made them and freed with it, and their
        /// names mean something else in any other context, so they are only deleted on the owning one.</summary>
        public void Dispose()
        {
            foreach (int t in textures) GL.DeleteTexture(t);
            foreach (int vb in instVbos) GL.DeleteBuffer(vb);
            foreach (int vb in vertVbos) GL.DeleteBuffer(vb);
            foreach (int u in ubos) GL.DeleteBuffer(u);   // deleting also detaches them from the global binding points
            if (ibo != 0) GL.DeleteBuffer(ibo);
            if (prog != 0) GL.DeleteProgram(prog);
            if (hdrTex != 0) GL.DeleteTexture(hdrTex);
            if (resolveProg != 0) GL.DeleteProgram(resolveProg);
            if (owner == null || !ReferenceEquals(owner, GraphicsContext.CurrentContext)) return;
            if (vao != 0) GL.DeleteVertexArray(vao);
            if (hdrFbo != 0) GL.DeleteFramebuffer(hdrFbo);
            if (resolveVao != 0) GL.DeleteVertexArray(resolveVao);
        }
    }
}
