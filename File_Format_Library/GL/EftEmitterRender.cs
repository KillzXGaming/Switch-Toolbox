using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.EditorDrawables;
using GL_EditorFramework;
using Toolbox.Library;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    // Software BotW EFTB particle renderer: billboards, mesh (PRIM) geometry and stripe/ribbon types, driven from the
    // file's own EMTR bytes. Confirmed-offset fields render exactly: color/alpha/scale 8-key curves, constant
    // colors, radius, blend and flipbook UV. GX2 textures decode natively (BC4/BC5 luminance/normal handling). The per-slot
    // nw::eft FragmentComposite ops drive the combiner. The motion model (absolute velocity / gravity) is a best-effort
    // preview tuned for watchability, not a frame-accurate simulation.
    public class EftEmitterRender : AbstractGlDrawable
    {
        public class EmitterInput {
            public string Name; public byte[] Data; public STGenericTexture Tex; public STGenericTexture Tex1; public STGenericTexture Tex2;
            public string EmtrName;     // the emitter's OWN file name (head +0x10), the weather-profile key; Name stays the RNG-seed string
            public float[] MeshVerts;   // pos3+uv2 interleaved (mesh emitters only)
            public float[] MeshNormals; // nrm3 per vertex (GPU preview feeds these to the game VS; the software path is unlit)
            public int[] MeshIndices;
            public bool StreamMode;     // built for the GPU preview's instance streams: game units and the runtime's
                                        // weather steady state, rather than the software preview's watchable approximation
        }

        /// <summary>Runtime overrides for the ambient weather and volume-mask emitters, which the
        /// game's env/weather system drives itself rather than from the file: a constant per-particle
        /// life, a sustained live count, a per-frame fall speed along the file's dir, and for some of
        /// them a replaced wrap box (Box stays null where the emitter keeps its file volume). None of
        /// it is derivable from the file, whose emission words stay at their authored values.
        /// The key is the emitter's own name, unique to the weather families corpus-wide, and the
        /// profile only applies while the emitter's FILE emission words (lifespan/emitRate/volScale)
        /// still match FileSig, so demo variants and user-edited emitters keep their file
        /// behavior.</summary>
        public class WeatherProfile { public int Life, Alive; public float FallMin, FallMax; public float[] Box; internal float[] FileSig; }
        public static WeatherProfile GetWeatherProfile(string emtrName, byte[] d, int structOff)
        {
            WeatherProfile wp;
            switch (emtrName ?? "")
            {                                                                                                  // FileSig = file lifespan, emitRate, volScale
                case "rain_near":      wp = new WeatherProfile { Life = 25,  Alive = 270, Box = new float[]{13f,5f,13f}, FallMin = 1.08f, FallMax = 1.20f,
                                                                 FileSig = new float[]{6f,1000f,15f,7.5f,15f} }; break;
                case "rain_middle":    wp = new WeatherProfile { Life = 20,  Alive = 44,  FallMin = 0.27f, FallMax = 0.30f,
                                                                 FileSig = new float[]{4f,1000f,15f,3f,15f} }; break;
                case "rain_far":       wp = new WeatherProfile { Life = 21,  Alive = 230, Box = new float[]{80f,64f,80f}, FallMin = 2.81f, FallMax = 4.00f,
                                                                 FileSig = new float[]{10f,100f,200f,200f,200f} }; break;
                case "Flake_Near":     wp = new WeatherProfile { Life = 50,  Alive = 156, FallMin = 0.04f, FallMax = 0.08f,
                                                                 FileSig = new float[]{2f,5f,200f,200f,200f} }; break;
                case "Flake_Far":      wp = new WeatherProfile { Life = 50,  Alive = 208, FallMin = 0.10f, FallMax = 0.20f,
                                                                 FileSig = new float[]{5f,60f,200f,200f,200f} }; break;
                case "particle1":      wp = new WeatherProfile { Life = 100, Alive = 510, Box = new float[]{10f,10f,10f},
                                                                 FileSig = new float[]{20f,1000f,50f,5f,50f} }; break;    // dust hovers (fall 0)
                case "particle2":      wp = new WeatherProfile { Life = 100, Alive = 510, Box = new float[]{10f,10f,10f},
                                                                 FileSig = new float[]{20f,1000f,50f,5f,50f} }; break;
                case "particle_Copy1": wp = new WeatherProfile { Life = 100, Alive = 204,
                                                                 FileSig = new float[]{20f,1000f,50f,5f,50f} }; break;
                case "bokeh":          wp = new WeatherProfile { Life = 120, Alive = 122,                                 // file ls=1 sentinel; runtime streams 120-frame particles
                                                                 FileSig = new float[]{1f,1000f,50f,5f,50f} }; break;
                case "Moya":           wp = new WeatherProfile { Life = 100, Alive = 52,  Box = new float[]{24f,24f,24f},
                                                                 FileSig = new float[]{10f,10000f,300f,300f,300f} }; break;
                default: return null;
            }
            if (d == null || structOff + 0x818 > d.Length) return null;
            int[] offs = { 0x6F0, 0x6F4, 0x80C, 0x810, 0x814 };
            for (int i = 0; i < 5; i++)
            {
                int o = structOff + offs[i];
                byte[] t = { d[o + 3], d[o + 2], d[o + 1], d[o] };
                if (Math.Abs(BitConverter.ToSingle(t, 0) - wp.FileSig[i]) > 1e-4f) return null;
            }
            return wp;
        }

        class DrawEmitter
        {
            public string Name; public string EmtrName = ""; public bool Additive; public float Radius;
            public int Color0N, Alpha0N, ScaleN;
            public float[,] Color0 = new float[8,4], Alpha0 = new float[8,4], Scale = new float[8,4];
            // the RAW alpha1 track (@0x4F0, count @0x1C): the emitter-lifecycle envelope some
            // shader families read as a runtime-maintained bank7 slot (see EnvelopeAlpha1);
            // kept unswapped even when the channel-1 fallback above adopts it as Alpha0
            public int Alpha1N;
            public float[,] Alpha1 = new float[8,4];
            public bool Alpha0Adopted;   // channel-1 fallback replaced Alpha0[] (true track is blank)
            public Vector3 ConstColor0; public bool Drawable;
            public STGenericTexture Tex; public bool HasAlpha = true;
            public STGenericTexture Tex1; public bool HasAlpha1 = false; public int Tex1Mode = 0;  // (legacy fallback) 2nd-sampler format heuristic
            public STGenericTexture Tex2; public bool HasAlpha2 = false;   // 3rd sampler (377 emitters use it; usually a grayscale mask added into alpha)
            // FragmentComposite combine ops, decoded per-emitter (0=Mul 1=Add 2=Sub 3=Max): textureColorBlend@0x8AD (slot0xslot1 RGB),
            // primitiveColorBlend@0x8AE (slot2 RGB), textureAlphaBlend@0x8B1 (slot0xslot1 alpha), primitiveAlphaBlend@0x8B2 (slot2 alpha).
            public int TexColorOp = 0, Slot2ColorOp = 0, TexAlphaOp = 0, Slot2AlphaOp = 0;
            // nw::eft fragmentColorMode@0x8A8 / fragmentAlphaMode@0x8A9. AlphaMode==3 = SUBTRACT/erosion bias
            // (a = clamp(texAlpha - particleAlpha), a "dissolve" fade-in/out keyed by the alpha-over-life curve);
            // else plain MULTIPLY (a = texAlpha * particleAlpha = current behavior). DECODED vs captured PS:
            // mode3 sub = Spark/Wind_sub/Emitter1/Furikake/eid12662 + Splash/Water/Ring/Spike archetypes;
            // mode1 mul = StripeTop/Steam. (Lit GBuffer smoke is also mode3 but uses a curve-shifted variant the
            // forward preview can't read; mode0 = the default/light-volume population, left as multiply.)
            public int ColorMode = 0, AlphaMode = 0;
            public bool IsDistortion = false;   // refraction emitter: resamples the scene buffer at an offset (0x8B8==1 || 0x701==3)
            public bool TexIsNorm=false, Tex1IsNorm=false, Tex2IsNorm=false;   // slot is a BC5 normal/flow map -> excluded from the alpha coverage mask
            public bool IsStripe = false, StripeConnection = false;   // legacy Plate_XZ-as-trail path (vertexTransformMode@0x8F4 == 2)
            public int VtxMode = 0; public bool IsDirectional = false;   // 0x8F4: 3=Directional_Y / 4=Directional_Polygon = velocity-stretched billboard
            public int StripeType = 0;   // StripeData.type: 0=camera-facing ribbon, 1=planar, 2=fixed-axis
            public int TrailLen = 16;    // trail-stripe history node count (StripeData numHistory; default until decoded)
            public float UvScaleX = 1f, UvScaleY = 1f; public int Cols = 1, Rows = 1, Frames = 1;
            // PER-SLOT flipbook grid: slot1 @0x308/0x30C, slot2 @0x358/0x35C (stride 0x50 from slot0 @0x2B8/0x2BC). VERIFIED
            // library-wide (clean grid dims) + matches ArrowHit_Fire/Fire slot1 C2F93B90 2x2 atlas. Each sampler has its own atlas.
            public float UvScaleX1 = 1f, UvScaleY1 = 1f, UvScaleX2 = 1f, UvScaleY2 = 1f; public int Cols1 = 1, Rows1 = 1, Cols2 = 1, Rows2 = 1;
            public bool MirrorUV = false;                // slot0 is a 1/2-scale quadrant + mirror wrap -> double the billboard UV so the stored quarter tiles into the full sprite (e.g. Gdn_Target reticle ring)
            public int[] SlotWrapU = new int[]{2,2,2};   // per-sampler GX2 wrapU @ struct sampler+0x08: 0=Mirror 1=Wrap(repeat) 2=Clamp (PINNED by capture cross-ref)
            public int[] SlotWrapV = new int[]{2,2,2};   // per-sampler GX2 wrapV @ struct sampler+0x09
            public int DispSide = 0, ZBuf = 0, StaticIdx = 0;   // 0x84F displaySide, 0x88E zBufATest, 0x00D0 flipbook static index
            // motion (decoded via NW4F field-order overlay + library validation; VERIFIED unless noted)
            public int Lifespan = 60, EmitInterval = 1;
            public int WeatherAlive = 0;   // weather steady state: sustain this many live particles (0 = no profile)
            public float WeatherFallMin, WeatherFallMax;   // weather per-frame fall speed range along Dir (profiled emitters)
            public bool OneShot = false; public bool SingleBurst = false; public int EndFrame = -1;  // 0x6F0 ls<=1 sentinel; SingleBurst = infinite-endFrame (emit once); 0x780 endFrame (-1=inf)
            const int DEFAULT_BURST_LIFE = 60;                       // mesh ring (ripple/ShockWave) expands over the full life via its scale curve
            const int SPRAY_BURST_LIFE = 7;                          // billboard ls=0 burst = a SHORT puff. CAPTURE-CALIBRATED (WaterSplash eid15182 ptcl maxR~5.5u -> life~6; Splash 0.5u -> life~8): the fabricated 60 over-spread the spray ~10x; allDirVel carries the spread, so one short life reproduces both real spreads.
            public float EmitRate = 8f, AllDirVel = 1f, AirResist = 1f, Dispersion = 3.14159f, ArcLength = 6.28318f;
            public float RotInit = 0f, AngVel = 0f, MomRand = 0f; public bool RotEnabled = false;  // 0x6C8 init Z-rot (2pi=random), 0x6D8 angularVelocity, 0x7C4 momentumRandom
            public Vector3 VolScale = Vector3.One;
            public Vector3 BaseScale = Vector3.One;   // ptclScaleStart @0x978 (GPU-preview stream scale; see parse)
            public Vector3 ScaleRand = Vector3.Zero;  // ptclScaleRandom @0x984 (PERCENT; see parse)
            // nw::eft TWO-STAGE emission velocity (decompiled from open-ead/NW4F-Eft eft_EmitterVolume.cpp + eft_Emitter.cpp:288):
            //   v0 = vShape + vDir.  STAGE 1 vShape = shapeNormal(emitFunction@0x714) * allDirVel@0x7B0 (the OMNIDIRECTIONAL
            //   burst -> explosions). STAGE 2 vDir = cone(half-angle=dispersion@0x7F4, AXIS = dir@0x7C8) * dirVel@0x7D4 (the
            //   DIRECTIONAL push -> rain falls because dir=(0,-1,0)). The 4-10x allDirVel gap (rain ~2 vs explosion ~8-16)
            //   auto-selects which term wins, so rain falls AND explosions burst with the SAME downward dir. Gravity is
            //   flag-off across BotW (no isolable accel VEC3; verified 9572 emitters) -> NO synthetic g; fall = initial dir velocity.
            public int EmitFunc = 0;            // 0x714 volumeType: 0=Point 1=Circle 3=FillCircle 4=Sphere 6=Sphere64 7=FillSphere 8=Cylinder 10/11=Box 12=Line 14=Rect (BotW 17/18/20/21/26 ~ burst family)
            public float DirVel = 0f;           // 0x7D4 directional speed along Dir
            public Vector3 Dir = Vector3.Zero; public float DirLen = 0f;   // 0x7C8 emission dir unit VEC3 (cone axis); rain/splash = (0,-1,0)
            // mesh
            public float[] MeshVerts; public int[] MeshIndices; public bool IsMesh;
            public int mvbo, mibo, idxCount; public bool meshReady;

            static uint  U(byte[] d,int o){ return (uint)((d[o]<<24)|(d[o+1]<<16)|(d[o+2]<<8)|d[o+3]); }
            static float F(byte[] d,int o){ byte[] t={d[o+3],d[o+2],d[o+1],d[o]}; return BitConverter.ToSingle(t,0); }
            static float San(float v){ return (float.IsNaN(v) || v <= 0f || v > 1e5f) ? 1f : v; }
            static float SanPct(float v){ return (float.IsNaN(v) || v < 0f || v > 400f) ? 0f : v; }

            public DrawEmitter(EmitterInput inp)
            {
                Name = inp.Name ?? ""; EmtrName = inp.EmtrName ?? ""; Tex = inp.Tex; Tex1 = inp.Tex1; Tex2 = inp.Tex2; Radius = 1;
                MeshVerts = inp.MeshVerts; MeshIndices = inp.MeshIndices;
                IsMesh = (MeshVerts != null && MeshVerts.Length >= 15 && MeshIndices != null && MeshIndices.Length >= 3);
                byte[] e = inp.Data;
                if (Tex != null) HasAlpha = FormatHasAlpha(Tex.Format);
                // 2nd color sampler (nw::eft s_secondTexture). The combine op is NOT a readable EMTR field (it is baked into the
                // GX2 shader with no EMTR->shader index) -> select faithfully by texture FORMAT: single-channel / BC5_UNORM mask ->
                // Mul into alpha (engine default composite; VERIFIED vs captured PS eid08685: a=clamp(a0*a1)); BC5_SNORM = normal/flow map -> ignore.
                if (Tex1 != null) {
                    HasAlpha1 = FormatHasAlpha(Tex1.Format);
                    Tex1Mode = (Tex1.Format == TEX_FORMAT.BC5_SNORM) ? 0 : 1;
                }
                if (Tex2 != null) HasAlpha2 = FormatHasAlpha(Tex2.Format);
                // Flag BC5 normal/flow-map slots so they are excluded from the alpha mask (see IsNormalMap).
                TexIsNorm = IsNormalMap(Tex); Tex1IsNorm = IsNormalMap(Tex1); Tex2IsNorm = IsNormalMap(Tex2);
                if (e == null || e.Length < 0xA00) return;
                // FragmentComposite combine ops (single bytes, endian-agnostic; clamp junk to 0=Mul=passthrough).
                // 0x8AD textureColorBlend (slot0xslot1 RGB), 0x8AE primitiveColorBlend (slot2 RGB),
                // 0x8B1 textureAlphaBlend (slot0xslot1 alpha), 0x8B2 primitiveAlphaBlend (slot2 alpha). Decoded vs captured PS.
                TexColorOp=e[0x8AD]; Slot2ColorOp=e[0x8AE]; TexAlphaOp=e[0x8B1]; Slot2AlphaOp=e[0x8B2];
                if ((uint)TexColorOp>3) TexColorOp=0; if ((uint)Slot2ColorOp>3) Slot2ColorOp=0;
                if ((uint)TexAlphaOp>3) TexAlphaOp=0; if ((uint)Slot2AlphaOp>3) Slot2AlphaOp=0;
                // fragmentColorMode/fragmentAlphaMode (single bytes). AlphaMode==3 selects the subtract/erosion alpha
                // (a = clamp(texAlpha - particleAlpha)); all other values keep the plain multiply. VERIFIED vs captured PS.
                ColorMode=e[0x8A8]; AlphaMode=e[0x8A9];
                if ((uint)ColorMode>4) ColorMode=0; if ((uint)AlphaMode>4) AlphaMode=0;
                // DISTORTION/refraction emitters resample the scene at an offset (Ripple_ind/Haze/Dist/Wall_Ind...). Discriminator
                // multi-example-verified vs the _ind/Distortion name class: refraction-mode byte 0x8B8==1 OR ripple-flag 0x701==3
                // (~95% coverage, ~0 normal false positives). slot0 (em.Tex) is the offset/indirection (normal) map.
                IsDistortion = (e[0x8B8]==1) || (e[0x701]==3);
                Color0N=(int)U(e,0x10); Alpha0N=(int)U(e,0x14); ScaleN=(int)U(e,0x20);
                Radius = F(e,0x360);
                // ptclScaleStart @0x978 (vec3): the base particle scale, fed to the GPU preview's sysScaleAttr
                // stream. Distinct from Radius@0x360 (emission-volume extent; the Fire family has 20-100 there
                // against a true scale of 0.2-8). Used by the GPU-preview stream only.
                BaseScale=new Vector3(San(F(e,0x978)),San(F(e,0x97C)),San(F(e,0x980)));
                // ptclScaleRandom @0x984 (vec3, PERCENT): stream scale = ptclScaleStart * (1 - u*percent/100),
                // with ONE uniform u per particle shared by all axes (the NW4F 1 - ptclScaleRandom*rand law).
                // Percent > 100 lawfully flips the sign; the GPU draw runs cull-off, so the mirrored winding
                // still renders.
                ScaleRand=new Vector3(SanPct(F(e,0x984)),SanPct(F(e,0x988)),SanPct(F(e,0x98C)));
                // blendType @0x88D (u8): 1=Add/additive, 0=Normal/alpha. VERIFIED vs captured GPU blend (0x8DC was REFUTED/inverted).
                Additive = (e[0x88D] == 1);
                DispSide = e[0x84F];   // 0=Both, 1=Front(cull back), 2=Back(cull front)
                ZBuf     = e[0x88E];   // 0=Normal(depth test, no write), 1=Ignore_Z(no depth test)
                ConstColor0 = new Vector3(F(e,0x958),F(e,0x95C),F(e,0x960));
                for (int k=0;k<8;k++) for(int c=0;c<4;c++){
                    Color0[k,c]=F(e,0x370+k*16+c*4); Alpha0[k,c]=F(e,0x3F0+k*16+c*4); Scale[k,c]=F(e,0x5B0+k*16+c*4);
                }
                // CHANNEL-1 FALLBACK: ~25 emitters disable channel 0 (Alpha0 keys all 0) and carry the real colour/alpha in
                // the 2nd channel (Color1@0x470 / Alpha1@0x4F0) -> they render INVISIBLE because the renderer reads only channel 0.
                // When Alpha0 is blank but Alpha1 is meaningful, adopt channel 1 as the particle colour/alpha (the colorMode that
                // would combine the two channels is undecoded, but with channel 0 blank the visible result IS channel 1). Grounded.
                int color1N=(int)U(e,0x18), alpha1N=(int)U(e,0x1C);
                Alpha1N=alpha1N;
                for (int k=0;k<8;k++) for(int c=0;c<4;c++) Alpha1[k,c]=F(e,0x4F0+k*16+c*4);
                float a0max=0f; if(Alpha0N>0) for(int k=0;k<Alpha0N && k<8;k++) a0max=Math.Max(a0max,Alpha0[k,0]);
                if (Alpha0N>0 && a0max<0.01f && alpha1N>0){
                    float a1max=0f; for(int k=0;k<alpha1N && k<8;k++) a1max=Math.Max(a1max,F(e,0x4F0+k*16));
                    if (a1max>0.05f){
                        for(int k=0;k<8;k++) for(int c=0;c<4;c++){ Color0[k,c]=F(e,0x470+k*16+c*4); Alpha0[k,c]=F(e,0x4F0+k*16+c*4); }
                        Alpha0N=alpha1N; if(color1N>0) Color0N=color1N;
                        Alpha0Adopted=true;   // Alpha0[] no longer holds the channel-0 track (see EnvelopeAlpha0)
                        ConstColor0=new Vector3(F(e,0x968),F(e,0x96C),F(e,0x970));   // channel-1 const colour too
                    }
                }
                // GATE the alphaMode==3 sub-bias to the EROSION signature: the captured subtract `clamp(texAlpha-particleAlpha)`
                // is only correct when the alpha-over-life curve STARTS LOW (full sprite at birth -> erodes as alpha rises,
                // e.g. Spark 0->0.71). When birth alpha is high/constant (rain=1.0, lit smoke=1.0) the subtract erodes the whole
                // sprite to nothing (or inverts it) -> 'all Rain emitters invisible'. So demote those back to plain multiply.
                // General + curve-driven (NOT per-emitter): keyed on Alpha0[0] only.
                if (AlphaMode==3 && AlphaAt(0f) > 0.25f) AlphaMode=0;
                // flipbook grid @0x2B0 (.z=numU/cols, .w=numV/rows). VERIFIED (0x800/0x804 was always 0 -> never tiled).
                float gz=F(e,0x2B8), gw=F(e,0x2BC);
                Cols=Math.Max(1,Math.Min(64,(int)Math.Round(gz<1f?1f:gz)));
                Rows=Math.Max(1,Math.Min(64,(int)Math.Round(gw<1f?1f:gw)));
                UvScaleX=1f/Cols; UvScaleY=1f/Rows; Frames=Cols*Rows;
                // per-slot atlas grids (slot1/slot2 have their OWN numU/numV; the renderer was applying slot0's grid to every slot,
                // so a 2x2 atlas on slot1 (e.g. ArrowHit_Fire/Fire's C2F93B90 fire sheet) drew all 4 cells at once).
                int gc1=(int)Math.Round(F(e,0x308)), gr1=(int)Math.Round(F(e,0x30C)), gc2=(int)Math.Round(F(e,0x358)), gr2=(int)Math.Round(F(e,0x35C));
                Cols1=Math.Max(1,Math.Min(64,gc1<1?1:gc1)); Rows1=Math.Max(1,Math.Min(64,gr1<1?1:gr1));
                Cols2=Math.Max(1,Math.Min(64,gc2<1?1:gc2)); Rows2=Math.Max(1,Math.Min(64,gr2<1?1:gr2));
                UvScaleX1=1f/Cols1; UvScaleY1=1f/Rows1; UvScaleX2=1f/Cols2; UvScaleY2=1f/Rows2;
                StaticIdx=(int)U(e,0xD0);   // per-emitter STATIC flipbook cell index (0xD0). Verified: 99% =0; variant-pickers
                if(StaticIdx<0 || StaticIdx>=Frames) StaticIdx=0;   // hold StaticIdx of the slot0 grid (Flower 0..11, Vector 1, Emitter1 3); clamp garbage to cell 0
                // --- motion sim fields (VERIFIED offsets; absolute velocity scale is tunable/approximate) ---
                // 0x6F0 ptclMaxLifespan: ls<=1 = one-shot/single-emission sentinel (NW4F-Eft; ~23% of emitters), NOT a 0-frame life.
                int ls=(int)F(e,0x6F0);
                float efr=F(e,0x780); EndFrame=(efr>=2f && efr<=100000f)?(int)Math.Min(efr,180f):-1;  // 0x780 endFrame: emit cutoff; -1=inf
                OneShot=(ls<=1);
                SingleBurst=(OneShot && EndFrame<0);   // infinite-endFrame one-shot = ONE persistent instance (e.g. ShockWave ring)
                if      (SingleBurst) Lifespan = IsMesh ? DEFAULT_BURST_LIFE : SPRAY_BURST_LIFE; // mesh ring expands in place over full life; billboard spray = short puff (capture-calibrated, not 60)
                else if (OneShot)     Lifespan=Math.Min(EndFrame,60);                           // finite-endFrame one-shot: bounded life so the spray fades
                else                  Lifespan=(ls>=2 && ls<=100000)?Math.Min(ls,180):60;       // continuous: decoded particle lifespan (frames)
                EmitRate=F(e,0x6F4);                                                            // 0x6F4 emit count/rate (interval-vestigial; see OneShot)
                int ei=(int)U(e,0x710); EmitInterval=(ei>=1 && ei<=600)?ei:1;                   // 0x710 emit interval
                AllDirVel=F(e,0x7B0);                                                           // 0x7B0 allDirVel (Stage-1 omnidirectional shape-burst speed)
                EmitFunc=(int)U(e,0x714);                                                       // 0x714 emitFunction/volumeType (shape of the Stage-1 burst)
                float dv=F(e,0x7D4); DirVel=(dv>=0f && dv<=100000f)?dv:0f;                       // 0x7D4 dirVel (Stage-2 directional speed along Dir)
                Dir=new Vector3(F(e,0x7C8),F(e,0x7CC),F(e,0x7D0)); DirLen=Dir.Length;            // 0x7C8 dir unit VEC3 (Stage-2 cone axis); rain/splash = (0,-1,0)
                float ar=F(e,0x6DC); AirResist=(ar>0f && ar<=1.5f)?ar:1f;                        // 0x6DC airResist (NOT rotEnable)
                float dsp=F(e,0x7F4); Dispersion=(dsp>=0f && dsp<=6.5f)?dsp:3.14159f;           // 0x7F4 dispersion cone half-angle (rad)
                float arc=F(e,0x7F0); ArcLength=(arc>0f && arc<=6.5f)?arc:6.28318f;             // 0x7F0 arc/azimuth span (rad)
                VolScale=new Vector3(F(e,0x80C),F(e,0x810),F(e,0x814));                         // 0x80C volumeScale (WEAK)
                RotInit=F(e,0x6C8); if(RotInit<0f || RotInit>6.5f) RotInit=0f;                   // 0x6C8 init Z-rotation (6.283=2pi sentinel = random full turn)
                AngVel=F(e,0x6D8); if(Math.Abs(AngVel)>3f) AngVel=0f;                            // 0x6D8 angularVelocity (rad/frame)
                MomRand=F(e,0x7C4); if(MomRand<0f || MomRand>1f) MomRand=0f;                     // 0x7C4 momentumRandom (per-particle speed spread)
                RotEnabled=(RotInit>1e-4f || Math.Abs(AngVel)>1e-4f);
                for (int i=0;i<3;i++){ uint hi=U(e,0x9A8+i*0x20), lo=U(e,0x9A8+i*0x20+4);
                    bool pop=!(hi==0xFFFFFFFF && lo==0xFFFFFFFF); if(pop) Drawable=true;
                    // GX2 address mode per axis: wrapU @ sampler+0x08, wrapV @ sampler+0x09 (enum 0=Mirror 1=Wrap/repeat 2=Clamp).
                    // PINNED by capture cross-ref (name+Color0): Smoke_Botttom 1,2=Wrap/Clamp ; Wind_sub 2,2=Clamp/Clamp ; reticle ring 0,0=Mirror.
                    SlotWrapU[i] = pop ? e[0x9A8+i*0x20+0x08] : 2;
                    SlotWrapV[i] = pop ? e[0x9A8+i*0x20+0x09] : 2; }
                // uv-expand flag @ slot0 sampler+0x17 (1 = the texture is a 1/2-scale quadrant): sample [0,2] so mirror wrap tiles it into the full image.
                // GATED to slot0 Mirror/Mirror so the x2 fires ONLY on genuine radial quadrants (Gdn_Target ring), never on wrap/clamp sprites (Wind_sub has 0x17=1 but is Clamp/Clamp).
                MirrorUV = (SlotWrapU[0]==0 && SlotWrapV[0]==0 && e[0x9A8+0x17]==1);
                if (U(e,0x87C)!=0 && U(e,0x87C)!=0xFFFFFFFF) Drawable=true;
                // vertexTransformMode@0x8F4. The NSMBU eft_Enum.h maps 3=Directional_Y / 4=Directional_Polygon (velocity-stretched
                // billboards), BUT that enum is UNVERIFIED for BotW and conflicts with the game-dump-grounded mapping: captured
                // "stripe" emitters draw as a single NON-instanced connected strip (a real connection stripe) while value-0
                // billboards draw INSTANCED, and every capture tie-able to a 0x8F4 value via bank7 is value 0, so value 3/4
                // cannot be confirmed as velocity-stretched. (Also a Simple-vs-Complex struct-offset confound: vertexTransformMode
                // lives at two offsets in the decomp.) Until a BotW capture of a rain/stripe emitter WITH bank7 settles it, keep the
                // verified behavior 2=trail / 3=connection. BuildDirectional() is implemented + ready to re-enable for 3/4 if confirmed.
                int vtx=(int)U(e,0x8F4); VtxMode=vtx; IsDirectional=false;
                IsStripe=(vtx==2 || vtx==3); StripeConnection=(vtx==3);
                if (IsStripe){
                    Drawable=true;
                    // StripeData lives in a trailing sub-record: 'EP02' = trail (history Stripe), 'EP03' = connection (Complex_Stripe).
                    // Confirmed vs NW4F-Eft (PtclStripe.queue history vs MakeConnectionStripe). StripeData.type @ record+0x20
                    // (rest of the struct is BotW-reordered, not yet mapped, so numSliceHistory/TrailLen stays defaulted).
                    for (int idx=0x900; idx+0x38<=e.Length; idx++)
                        if (e[idx]==0x45 && e[idx+1]==0x50 && e[idx+2]==0x30 && (e[idx+3]==0x32 || e[idx+3]==0x33)){
                            int st=(int)U(e,idx+0x20); if(st>=0 && st<=2) StripeType=st;
                            int nsh=(int)F(e,idx+0x34); if(nsh>=2 && nsh<=256) TrailLen=nsh;   // StripeData.numSliceHistory (stored as f32) = trail length
                            break;
                        }
                }
                // Ambient weather/volume classes: the game's env system drives life, sustained count, and
                // emission cadence itself (see GetWeatherProfile), so the stream build adopts that steady
                // state. Ambient is continuous by definition, so the file's one-shot sentinel (bokeh ls=1)
                // and any emission window give way to sustained streaming. The software preview keeps the
                // file's own values.
                var wp = inp.StreamMode ? GetWeatherProfile(EmtrName, e, 0) : null;
                if (wp != null){
                    Lifespan = wp.Life; WeatherAlive = wp.Alive;
                    WeatherFallMin = wp.FallMin; WeatherFallMax = wp.FallMax;
                    OneShot = false; SingleBurst = false; EndFrame = -1;
                    EmitInterval = (wp.Alive >= wp.Life) ? 1 : Math.Max(1, (int)Math.Round(wp.Life/(float)wp.Alive));
                }
            }
            static float Clamp01(float v){ if(float.IsNaN(v)||v<=0f||v>1f) return 1f; return v; }
            internal static bool FormatHasAlpha(TEX_FORMAT f){
                switch(f){
                    case TEX_FORMAT.BC4_UNORM: case TEX_FORMAT.BC4_SNORM:
                    case TEX_FORMAT.BC5_UNORM: case TEX_FORMAT.BC5_SNORM:
                    case TEX_FORMAT.R8_UNORM:  case TEX_FORMAT.R8G8_UNORM: return false;
                    default: return true;
                }
            }
            // A BC5 texture is a 2-channel NORMAL/FLOW map, NOT an alpha coverage mask. Using its (near-uniform) luminance as
            // alpha draws an opaque RECTANGLE (Water_Splash01 Splash/ptcl). CAPTURE-CONFIRMED (WaterSplash eid15224): BC5 = the
            // normal map for lighting, the BC4 slot is the real alpha mask. BUT BC5_UNORM is ALSO used as a genuine silhouette
            // mask (the Spike needle 25F944AE, VERIFIED eid08685) -> format alone can't tell them apart, so check CONTENT: a
            // normal/flow map is near-uniform (almost no transparent texels); a silhouette mask has a dark background. BC5_SNORM
            // is always a (signed) normal map. Verified vs 93 captured BC5 textures: normals 0% near-zero, silhouettes 25-95%.
            static readonly System.Collections.Generic.Dictionary<STGenericTexture,bool> _normCache = new System.Collections.Generic.Dictionary<STGenericTexture,bool>();
            internal static bool IsNormalMap(STGenericTexture tex){
                if(tex==null) return false;
                if(tex.Format==TEX_FORMAT.BC5_SNORM) return true;
                if(tex.Format!=TEX_FORMAT.BC5_UNORM) return false;
                lock(_normCache){ bool cv; if(_normCache.TryGetValue(tex,out cv)) return cv; }
                bool isNorm=false;
                try {
                    var bmp=tex.GetBitmap();
                    if(bmp!=null){
                        int n=0,zero=0, sw=Math.Max(1,bmp.Width/48), sh=Math.Max(1,bmp.Height/48);
                        for(int y=0;y<bmp.Height;y+=sh) for(int x=0;x<bmp.Width;x+=sw){
                            var px=bmp.GetPixel(x,y); int lum=Math.Max((int)px.R,Math.Max((int)px.G,(int)px.B));
                            n++; if(lum<26) zero++;
                        }
                        isNorm = (n>0) && ((float)zero/n < 0.12f);   // <12% near-zero texels -> uniform -> normal/flow map (not a mask)
                    }
                } catch { isNorm=false; }   // can't decode -> safest is the current behavior (treat as a mask)
                lock(_normCache){ _normCache[tex]=isNorm; }
                return isNorm;
            }
            static void Sample(float[,] keys,int n,float t,int comps,float[] outv){
                int m=Math.Max(1,n);
                if(m==1){ for(int c=0;c<comps;c++) outv[c]=keys[0,c]; return; }
                float maxT=0; for(int k=0;k<m;k++) maxT=Math.Max(maxT,keys[k,3]);
                if(maxT<=0f){ for(int c=0;c<comps;c++) outv[c]=keys[0,c]; return; }
                if(t<=keys[0,3]){ for(int c=0;c<comps;c++) outv[c]=keys[0,c]; return; }
                if(t>=keys[m-1,3]){ for(int c=0;c<comps;c++) outv[c]=keys[m-1,c]; return; }
                for(int i=0;i<m-1;i++){ float t0=keys[i,3],t1=keys[i+1,3];
                    if(t0<=t&&t<=t1&&t1>t0){ float a=(t-t0)/(t1-t0);
                        for(int c=0;c<comps;c++) outv[c]=keys[i,c]*(1-a)+keys[i+1,c]*a; return; } }
                for(int c=0;c<comps;c++) outv[c]=keys[m-1,c];
            }
            float[] _c=new float[4];
            // ELink override multipliers (identity by default), applied in the read path so billboard/stripe/mesh all get them.
            public Vector3 ColorMul = Vector3.One; public float AlphaMul = 1f;
            public Vector3 ColorAt(float t){
                Vector3 c;
                if(Color0N==0) c = ConstColor0; else { Sample(Color0,Color0N,t,3,_c); c = new Vector3(_c[0],_c[1],_c[2]); }
                return new Vector3(c.X*ColorMul.X, c.Y*ColorMul.Y, c.Z*ColorMul.Z);
            }
            public float AlphaAt(float t){ if(Alpha0N==0) return AlphaMul; Sample(Alpha0,Alpha0N,t,1,_c); return _c[0]*AlphaMul; }
            // Apply ELink asset overrides. Every param is a MULTIPLIER (ParamDefine defaults are all 1.0): motion/emission
            // scale the decoded fields; colour/alpha go through ColorMul/AlphaMul above. Position/Rotation/Duration are not
            // modelled by this renderer (it draws at origin and loops), so they are intentionally ignored.
            public void ApplyOverride(EftOverride o){
                if(o==null || !o.HasAny) return;
                Radius *= o.ScaleMul;
                BaseScale = new Vector3(BaseScale.X*o.ScaleMul, BaseScale.Y*o.ScaleMul, BaseScale.Z*o.ScaleMul);
                if(o.LifeMul!=1f) Lifespan = Math.Max(1,(int)Math.Round(Lifespan*o.LifeMul));
                DirVel *= o.DirVelMul;                                   // DirectionalVel = the Stage-2 directional speed only
                EmitRate *= o.EmitRateMul;
                if(o.EmitIntervalMul!=1f) EmitInterval = Math.Max(1,(int)Math.Round(EmitInterval*o.EmitIntervalMul));
                if(o.EmitVolMul!=1f) VolScale = new Vector3(VolScale.X*o.EmitVolMul, VolScale.Y*o.EmitVolMul, VolScale.Z*o.EmitVolMul); // EmissionScale -> emission-region size
                ColorMul = o.RgbMul; AlphaMul = o.AlphaMul;
            }
            public float Alpha1At(float t){ if(Alpha1N==0) return Alpha1[0,0]; Sample(Alpha1,Alpha1N,t,1,_c); return _c[0]; }
            public float Alpha0EnvAt(float t){ if(Alpha0N==0) return Alpha0[0,0]; Sample(Alpha0,Alpha0N,t,1,_c); return _c[0]; }
            public float ScaleAt(float t){ if(ScaleN==0) return 1f; Sample(Scale,ScaleN,t,1,_c); return _c[0]; }
            // Scale has separate X,Y keys; the billboard used X only (a square), collapsing thin-tall sprites (Line_* :
            // scaleX 0.01 / scaleY 0.55) to an invisible dot. Expose both so the quad can be non-uniform (a visible line).
            public void ScaleXYAt(float t, out float sx, out float sy){ if(ScaleN==0){ sx=1f; sy=1f; return; } Sample(Scale,ScaleN,t,2,_c); sx=_c[0]; sy=_c[1]; }
        }

        static readonly float[,] CORNERS = { {-1,-1},{1,-1},{1,1}, {-1,-1},{1,1},{-1,1} };
        const int STRIDE = 64;            // billboard interleave: pos3,color4,size1,corner2,uvoff2,uvoff1_2,uvoff2_2 = 16 floats (B2: per-slot atlas UVs)
        const int VFLOATS = 16;           // floats per billboard/stripe vertex (= STRIDE/4)
        const int LIFE = 60, RATE = 8, MRATE = 3, ONESHOT_GAP = 30;
        const int ONESHOT_PAUSE = 90;   // clear gap after a one-shot effect finishes before it replays (fire -> fade -> PAUSE -> replay)
        const int CONT_BURST = 30;      // bounded emit window for a continuous emitter that sits inside a one-shot effect (it puffs, then stops)
        const int ALIVE_CAP = 120;   // preview bound on simultaneously-live particles per emitter (decoded rates reach 1000s)
        // velocity-stretched billboard (vertexTransformMode 3/4): the quad is stretched along the particle's screen-projected
        // velocity into a motion streak. Length = size + uncompressedSpeed*K (faster -> longer), capped at size*MAXSTRETCH;
        // width = size*WIDTHFRAC (a streak is narrow). Tunable; the exact GPU factor lives in the precompiled VS.
        const float DIR_STREAK_K = 0.5f, DIR_MAXSTRETCH = 8f, DIR_WIDTHFRAC = 0.35f;

        readonly List<DrawEmitter> emitters = new List<DrawEmitter>();
        // GL programs/buffers are SHARED across every preview instance and compiled exactly once. GraphicsContext.ShareContexts
        // is enabled (OpenTKSharedResources), so one set is valid in every viewport/context, and a single shared set can never
        // leak per-preview. Only per-emitter mesh buffers (DrawEmitter.mvbo/mibo) are per-instance; those are freed via
        // QueueDispose -> FlushDeadBuffers when a render is swapped out of the editor preview.
        // The framework's ShaderProgram is keyed internally by GL control, so ONE shared instance throws
        // KeyNotFoundException in ShaderProgram.Setup when a NEW preview viewport (e.g. after reopening a file) draws
        // with it. Cache one shader pair PER control instead: bounded (one pair per open editor, not per preview, so it
        // doesn't grow as you browse emitters) and valid for every control it is drawn on.
        static readonly Dictionary<GL_ControlModern, ShaderProgram> sBillboardShaders = new Dictionary<GL_ControlModern, ShaderProgram>();
        static readonly Dictionary<GL_ControlModern, ShaderProgram> sMeshShaders = new Dictionary<GL_ControlModern, ShaderProgram>();
        ShaderProgram shader, meshShader; static int vbo = 0;
        int frame = 0, cycle = 120;
        readonly PlaybackClock clock = new PlaybackClock();
        bool faithfulUnits = false;   // stream mode: GAME units (see Emit); preview draws keep the compressed units
        static int sceneTex = 0, sceneW = 0, sceneH = 0;   // grabbed-scene colour texture for the distortion/refraction pass (shared scratch)
        static readonly List<int> deadBuffers = new List<int>();   // mesh VBO/IBOs from swapped-out renders; deleted on the next Draw (GL context current)
        const float DISTORT_STRENGTH = 0.05f;        // screen-UV offset scale for refraction (absolute field undecoded; tuned for visibility)
        bool oneShotEffect = false; int effectActive = 30;   // set-level: is this an event-triggered (fire-once-pause-replay) effect, and its emit-active window
        float viewScale = 6f, motionScale = 1f;
        // PREVIEW velocity-compression scale. The faithful absolute is allDirVel units/frame (VEL=1.0, capture-confirmed by the
        // Haze disc), but in an isolated, auto-framed preview that sends every effect flying off-screen: a ripple (allDirVel 1)
        // travels 60x its own size, and one fast spray (Debris 64, Furikake 240) blows the camera frame up so everything else
        // shrinks to invisible. So compress motion for watchability; relative speeds stay faithful. Raise toward 1.0 for absolute scale.
        const float VEL = 0.03f;
        // GLOBAL gravity (per-frame Y accel, VEL-compressed space), integrated by the closed-form S2 term. Default 0:
        // BotW carries no per-emitter gravity field (falling comes from the Stage-2 downward dir velocity). Kept as a
        // tunable knob for long-lived arcs (leaves/snow/debris).
        const float GRAVITY_Y = 0f;
        // A single mesh RING (ripple/ShockWave) expands via its scale curve and should stay ~centered; cap its
        // translation (dir.Y sink/drift) to this fraction of its peak size so expansion stays the visible motion.
        const float RING_TRANSLATE_FRAC = 0.15f;
        static bool shaderChecked = false;   // validate GLSL link status once (shaders are shared) and report failures to the console
        bool framed = false; float frameRadius = 30f;   // one-time camera framing to the effect's world extent
        public bool AutoFrame = true;   // false = skip the one-time FrameSelect, so switching emitters keeps the camera put
        // cursor-follow emitter path: per-frame emitter world position (ring buffer). STRIPE emitters only consume it,
        // so ribbons sweep along the cursor while billboards/mesh stay at the origin. Supplies the emitter motion the
        // .sesetlist lacks (the game gets it from animation); the particle math itself stays faithful.
        readonly Vector3[] emPath = new Vector3[256]; bool emPathInit = false;
        Vector3 EmitterAt(int fr){ if(!emPathInit || fr<0) return Vector3.Zero; return emPath[((fr%emPath.Length)+emPath.Length)%emPath.Length]; }

        // ELink asset override multipliers carried into the preview; all default to identity (no change).
        public class EftOverride {
            public float ScaleMul=1f, LifeMul=1f, DirVelMul=1f, EmitRateMul=1f, EmitIntervalMul=1f, AlphaMul=1f, EmitVolMul=1f;
            public Vector3 RgbMul = Vector3.One;
            public int DurationFrames=0;   // 0 = no override; >0 bounds the set's emission window (see ctor)
            public bool HasAny;
        }

        public EftEmitterRender(IEnumerable<EmitterInput> inputs, EftOverride ovr = null)
        {
            foreach (var inp in inputs){ var de=new DrawEmitter(inp); de.ApplyOverride(ovr); if(de.Drawable) emitters.Add(de); }
            float maxr=0.1f, maxs=1f;
            foreach(var e in emitters){ maxr=Math.Max(maxr,e.Radius); for(int k=0;k<8;k++) maxs=Math.Max(maxs,e.Scale[k,0]); }
            viewScale = 70f/(maxr*maxs+4f); motionScale = Math.Max(0.5f, maxr*maxs);
            // Replay cycle. A one-shot effect (event-triggered: explosion/hit/break) should fire, play out, pause, then
            // replay rather than emit non-stop. Detect it at the SET level: any emitter is a burst (lifespan<=1) or has a
            // finite emit window (endFrame). An ambient set (all continuous infinite streams: torch fire / rain / smoke)
            // keeps streaming with no pause; inside a one-shot set, even its continuous emitters stop at effectActive.
            int maxLife=1, maxEnd=0; oneShotEffect=false;
            foreach(var e in emitters){
                if (e.OneShot || e.EndFrame>=2) oneShotEffect=true;
                maxLife=Math.Max(maxLife, Math.Max(1,Math.Min(180,e.Lifespan)));
                if (e.EndFrame>=2) maxEnd=Math.Max(maxEnd, e.EndFrame);
            }
            effectActive=Math.Max(CONT_BURST, maxEnd);                                  // emission-active window of a one-shot effect
            if (oneShotEffect)
                cycle=Math.Min(600, effectActive + maxLife + ONESHOT_PAUSE);            // fire(effectActive) -> fade(maxLife) -> PAUSE(ONESHOT_PAUSE) -> replay
            else {
                int cyc=90; foreach(var e in emitters) cyc=Math.Max(cyc, Math.Max(e.Lifespan, (e.EndFrame>=2)?e.EndFrame:0));
                cycle=Math.Min(600, cyc+ONESHOT_GAP);                                   // ambient stream: small gap only
            }
            // ELink Duration override (frames): the asset bounds the effect's emission window, so a timed effect (e.g. a
            // 30-60f breath) emits for Duration frames, lets its particles live out, pauses, then replays -> one puff in
            // the preview rather than an endless stream. Without it the looping preview over-emits short timed effects.
            if (ovr != null && ovr.DurationFrames > 0){
                oneShotEffect = true;
                effectActive = Math.Max(1, ovr.DurationFrames);
                cycle = Math.Min(600, effectActive + maxLife + ONESHOT_PAUSE);
            }
            // camera frame radius: frame to the effect's SIZE (sprite size + emission-region), NOT the farthest a fast
            // particle travels. Chasing max-travel let one fast spray (Debris allDirVel 64 -> thousands of units) blow the
            // frame up and shrink every slow/stationary effect (ripples, splashes) to invisible specks. Now the travel term
            // is CAPPED to a few times the sprite size: the bulk stays framed and visible; genuinely fast sprays exit the
            // frame, which is faithful (they are fast). Orbit/zoom to follow them.
            float fr=1f;
            foreach(var e in emitters){
                float ms=0.01f; for(int k=0;k<8;k++) ms=Math.Max(ms,Math.Max(e.Scale[k,0],e.Scale[k,1]));   // max of scaleX/scaleY (thin-tall sprites frame by height)
                float life=Math.Max(4,Math.Min(180,e.Lifespan));
                float maxDist=(e.AirResist>0.999f)? life : (1f-(float)Math.Pow(e.AirResist,life))/(1f-e.AirResist);
                float size = Math.Max(e.Radius*ms, 0.05f);                                                   // world sprite size
                float vol  = 0.05f*motionScale*Math.Max(e.VolScale.X, Math.Max(e.VolScale.Y, e.VolScale.Z));   // emission region extent (matches volK)
                float travel = Math.Min(Math.Max(e.AllDirVel,e.DirVel)*VEL*maxDist, size*6f);                 // bounded travel (Stage 1 allDirVel or Stage 2 dirVel, whichever is faster)
                fr=Math.Max(fr, size*2f + vol + travel);
            }
            frameRadius=Math.Min(20000f, fr + 4f);
        }

        public override void Prepare(GL_ControlModern control)
        {
            if (vbo == 0) GL.GenBuffers(1, out vbo);                                                              // one shared streaming VBO (raw GL name, valid across the shared contexts)
            if (sBillboardShaders.TryGetValue(control, out shader)) { meshShader = sMeshShaders[control]; return; }  // already compiled for THIS GL control
            shader = new ShaderProgram(new FragmentShader(@"#version 330
uniform sampler2D tex; uniform sampler2D tex1; uniform sampler2D tex2;
uniform int uHasTex; uniform int uHasTex1; uniform int uHasTex2;
uniform int uAlphaFromLum; uniform int uAlphaFromLum1; uniform int uAlphaFromLum2;
uniform int uTexIsNorm; uniform int uTex1IsNorm; uniform int uTex2IsNorm;   // BC5 normal/flow slots -> excluded from the alpha mask
uniform int uTexColorOp; uniform int uSlot2ColorOp; uniform int uTexAlphaOp; uniform int uSlot2AlphaOp;
uniform int uAlphaMode;   // EMTR fragmentAlphaMode@0x8A9: 3 = subtract/erosion bias, else plain multiply
uniform int uAdditive; uniform float uf_alphaTestRef;
uniform sampler2D uSceneTex; uniform vec3 uViewport; uniform int uDistort; uniform float uDistortStrength;   // refraction: resample the grabbed scene at an offset
in vec4 vColor; in vec2 vUV; in vec2 vUV1; in vec2 vUV2; out vec4 FragColor;   // vUV1/vUV2 = slot1/slot2 own atlas cell (B2)
// nw::eft FragmentComposite op, decoded per-emitter from EMTR 0x8AD/0x8AE/0x8B1/0x8B2: 0=Mul 1=Add 2=Sub 3=Max
float fc(int op,float a,float b){ if(op==1) return a+b; if(op==2) return a-b; if(op==3) return max(a,b); return a*b; }
vec3 fc3(int op,vec3 a,vec3 b){ if(op==1) return a+b; if(op==2) return a; if(op==3) return max(a,b); return a*b; }   // COLOUR SUB(2)=passthrough: the captured PS never subtracts a texture from the particle albedo, so SUB keeps the particle colour (a literal subtract turned Soil dust green).
void main(){
    if(uDistort==1){   // DISTORTION/refraction (capture eid14551): warp the rendered scene by the slot0 offset/normal map
        vec2 suv=gl_FragCoord.xy/uViewport.xy;
        vec2 off=(texture(tex,vUV).rg*2.0-1.0)*uDistortStrength*vColor.a;
        FragColor=vec4(texture(uSceneTex, clamp(suv+off,0.0,1.0)).rgb, vColor.a);
        return;
    }
    // nw::eft: textures are alpha/intensity MASKS; RGB comes from the particle color. A BC5 NORMAL/FLOW map (uTexIsNorm)
    // carries NO coverage -> treat slot0 like a no-shape texture and use the procedural soft falloff, so alpha comes from a
    // real mask (BC4 slot below) or a soft sprite (capture eid15224: BC5=normal for lighting, BC4=the mask). Normal-map
    // slot1/slot2 are skipped entirely (they would draw an opaque rectangle).
    bool noShape0=(uHasTex==0)||(uTexIsNorm==1);
    vec4 t=(uHasTex==1)?texture(tex,vUV):vec4(1.0);
    vec2 pc=vUV*2.0-1.0; float dc=dot(pc,pc); float circ=exp(-dc*3.0)*(1.0-clamp(dc,0.0,1.0));
    float tA0=noShape0?circ:((uAlphaFromLum==1)?max(t.r,max(t.g,t.b)):t.a);
    vec3 rgb=vColor.rgb; if(uHasTex==1 && uTexIsNorm==0 && uAlphaFromLum==0) rgb=fc3(uTexColorOp, rgb, t.rgb);
    float a=tA0;
    if(uHasTex1==1 && uTex1IsNorm==0){                               // slot1: alpha by textureAlphaBlend@0x8B1 (skip BC5 normal maps)
        vec4 t1=texture(tex1,vUV1); float tA1=(uAlphaFromLum1==1)?max(t1.r,max(t1.g,t1.b)):t1.a;
        a=fc(uTexAlphaOp, a, tA1);
        if(uAlphaFromLum1==0) rgb=fc3(uTexColorOp, rgb, t1.rgb);
    }
    if(uHasTex2==1 && uTex2IsNorm==0){                               // slot2: a COVERAGE MASK folded INTO the shape alpha (skip BC5 normal)
        vec4 t2=texture(tex2,vUV2); float tA2=(uAlphaFromLum2==1)?max(t2.r,max(t2.g,t2.b)):t2.a;
        // CAPTURE-GROUNDED (PlayerJumpSoil/MaxAdd, eid14967 PS): the game folds slot2 MULTIPLICATIVELY inside the
        // slot0/slot1 shape (alpha = shape*(slot2.w + softEdge)); it never ADDS slot2 on top. Adding it (old
        // a=fc(Add,a,tA2)) let a full-coverage mask (cloud floor 0.15) lift alpha everywhere -> opaque square.
        // Multiply keeps slot2 a detail/coverage mask: it can only carve INSIDE the sprite, never flood outside it.
        a = a * tA2;
        if(uAlphaFromLum2==0) rgb=fc3(uSlot2ColorOp, rgb, t2.rgb);
    }
    // nw::eft fragmentAlphaMode@0x8A9: 3 = SUBTRACT/erosion (a = clamp(texAlpha - particleAlpha); dissolve fade
    // keyed by the alpha-over-life curve, VERIFIED vs captured Spark/Wind_sub/eid12662 PS); else plain MULTIPLY.
    if(uAlphaMode==3) a=clamp(a - vColor.a, 0.0, 1.0);
    else              a=clamp(a,0.0,1.0)*vColor.a;
    if(!(a>uf_alphaTestRef)) discard;   // universal nw::eft alpha test (VERIFIED in every captured PS)
    FragColor=(uAdditive==1)?vec4(rgb*a,a):vec4(rgb,a);
}"), new VertexShader(@"#version 330
in vec3 aPos; in vec4 aColor; in float aSize; in vec2 aCorner; in vec2 aUVOff; in vec2 aUVOff1; in vec2 aUVOff2;
uniform mat4 mtxMdl; uniform mat4 mtxCam; uniform vec3 uCamRight; uniform vec3 uCamUp; uniform vec3 uUVScale; uniform int uStripe;
out vec4 vColor; out vec2 vUV; out vec2 vUV1; out vec2 vUV2;
void main(){
    vec3 world; vec2 uv;
    if(uStripe==1){ world=aPos; uv=aCorner*uUVScale.xy+aUVOff; vUV1=uv; vUV2=uv; }               // stripe ribbon: aPos already world, aCorner = raw (u,v); tex1/tex2 sample at the stripe uv
    else { world=aPos + (uCamRight*aCorner.x + uCamUp*aCorner.y)*aSize; uv=aUVOff; vUV1=aUVOff1; vUV2=aUVOff2; }   // billboard: aCorner = spin-rotated quad corner; aUVOff/1/2 = full baked (unrotated) per-slot UV
    vec4 clip=mtxMdl*mtxCam*vec4(world,1.0);
    gl_Position=clip; vColor=aColor; vUV=uv;
}"), control);

            meshShader = new ShaderProgram(new FragmentShader(@"#version 330
uniform sampler2D tex; uniform sampler2D tex1; uniform sampler2D tex2;
uniform int uHasTex; uniform int uHasTex1; uniform int uHasTex2;
uniform int uAlphaFromLum; uniform int uAlphaFromLum1; uniform int uAlphaFromLum2;
uniform int uTexIsNorm; uniform int uTex1IsNorm; uniform int uTex2IsNorm;   // BC5 normal/flow slots -> excluded from the alpha mask
uniform int uTexColorOp; uniform int uSlot2ColorOp; uniform int uTexAlphaOp; uniform int uSlot2AlphaOp;
uniform int uAlphaMode;   // EMTR fragmentAlphaMode@0x8A9: 3 = subtract/erosion bias, else plain multiply
uniform int uAdditive;
uniform vec3 uColor; uniform float uAlpha; uniform float uf_alphaTestRef;
uniform sampler2D uSceneTex; uniform vec3 uViewport; uniform int uDistort; uniform float uDistortStrength;
in vec2 vUV; in vec2 vUV1; in vec2 vUV2; out vec4 FragColor;
float fc(int op,float a,float b){ if(op==1) return a+b; if(op==2) return a-b; if(op==3) return max(a,b); return a*b; }
vec3 fc3(int op,vec3 a,vec3 b){ if(op==1) return a+b; if(op==2) return a; if(op==3) return max(a,b); return a*b; }   // COLOUR SUB(2)=passthrough: the captured PS never subtracts a texture from the particle albedo, so SUB keeps the particle colour (a literal subtract turned Soil dust green).
void main(){
    if(uDistort==1){   // DISTORTION/refraction mesh (e.g. Ripple_ind ring): warp the scene by the slot0 offset map
        vec2 suv=gl_FragCoord.xy/uViewport.xy;
        vec2 off=(texture(tex,vUV).rg*2.0-1.0)*uDistortStrength*uAlpha;
        FragColor=vec4(texture(uSceneTex, clamp(suv+off,0.0,1.0)).rgb, uAlpha);
        return;
    }
    vec4 t=(uHasTex==1)?texture(tex,vUV):vec4(1.0);
    float tA0=(uHasTex==1 && uTexIsNorm==0)?((uAlphaFromLum==1)?max(t.r,max(t.g,t.b)):t.a):1.0;   // BC5 normal slot0 -> the mesh geometry is the shape
    vec3 rgb=uColor; if(uHasTex==1 && uTexIsNorm==0 && uAlphaFromLum==0) rgb=fc3(uTexColorOp, rgb, t.rgb);
    float a=tA0;
    if(uHasTex1==1 && uTex1IsNorm==0){
        vec4 t1=texture(tex1,vUV1); float tA1=(uAlphaFromLum1==1)?max(t1.r,max(t1.g,t1.b)):t1.a;   // vUV1 = slot1's own atlas cell
        a=fc(uTexAlphaOp, a, tA1);
        if(uAlphaFromLum1==0) rgb=fc3(uTexColorOp, rgb, t1.rgb);
    }
    if(uHasTex2==1 && uTex2IsNorm==0){
        vec4 t2=texture(tex2,vUV2); float tA2=(uAlphaFromLum2==1)?max(t2.r,max(t2.g,t2.b)):t2.a;   // vUV2 = slot2's own atlas cell
        a = a * tA2;   // slot2 = coverage mask, multiplied within the shape (never added on top); see billboard shader note
        if(uAlphaFromLum2==0) rgb=fc3(uSlot2ColorOp, rgb, t2.rgb);
    }
    if(uAlphaMode==3) a=clamp(a - uAlpha, 0.0, 1.0);   // fragmentAlphaMode 3 = subtract/erosion (see billboard shader)
    else              a=clamp(a,0.0,1.0)*uAlpha;
    if(!(a>uf_alphaTestRef)) discard;
    FragColor=(uAdditive==1)?vec4(rgb*a,a):vec4(rgb,a);
}"), new VertexShader(@"#version 330
in vec3 mPos; in vec2 mUV;
uniform mat4 mtxMdl; uniform mat4 mtxCam; uniform vec3 uPos; uniform float uSize; uniform vec3 uMeshUVScale; uniform vec3 uMeshUVOff; uniform vec3 uMeshUVScale1; uniform vec3 uMeshUVScale2;
out vec2 vUV; out vec2 vUV1; out vec2 vUV2;
void main(){ vec3 w=mPos*uSize+uPos; gl_Position=mtxMdl*mtxCam*vec4(w,1.0); vUV=mUV*uMeshUVScale.xy+uMeshUVOff.xy; vUV1=mUV*uMeshUVScale1.xy; vUV2=mUV*uMeshUVScale2.xy; }"), control);

            sBillboardShaders[control] = shader;   // cache per control (framework ShaderProgram is per-control-keyed)
            sMeshShaders[control] = meshShader;
        }

        public override void Prepare(GL_ControlLegacy control) { }
        public override void Draw(GL_ControlLegacy control, Pass pass) { }

        internal static TextureWrapMode WrapMode(int m){ return m==0?TextureWrapMode.MirroredRepeat : m==1?TextureWrapMode.Repeat : TextureWrapMode.ClampToEdge; }   // eft wrap enum 0=Mirror 1=Wrap 2=Clamp
        static bool BindUnit(STGenericTexture tex, TextureUnit unit, int wrapU=2, int wrapV=2)
        {
            if (tex==null) return false;
            try {
                if (tex.RenderableTex==null || !tex.RenderableTex.GLInitialized) tex.LoadOpenGLTexture();
                if (tex.RenderableTex!=null && tex.RenderableTex.GLInitialized){
                    GL.ActiveTexture(unit);
                    GL.BindTexture(TextureTarget.Texture2D, tex.RenderableTex.TexID);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)WrapMode(wrapU));
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)WrapMode(wrapV));
                    return true;
                }
            } catch { }
            return false;
        }
        // Bind slot2 + push the decoded FragmentComposite ops (shared by billboard / mesh / stripe paths).
        static void SetCombine(ShaderProgram sh, DrawEmitter em, bool hasTex2)
        {
            bool fl2 = hasTex2 && em.Tex2!=null && !DrawEmitter.FormatHasAlpha(em.Tex2.Format);
            sh.SetInt("uHasTex2", hasTex2?1:0); sh.SetInt("uAlphaFromLum2", fl2?1:0);
            sh.SetInt("uTexIsNorm", em.TexIsNorm?1:0); sh.SetInt("uTex1IsNorm", em.Tex1IsNorm?1:0); sh.SetInt("uTex2IsNorm", em.Tex2IsNorm?1:0);   // BC5 normal/flow slots excluded from alpha
            sh.SetInt("uTexColorOp", em.TexColorOp);   sh.SetInt("uSlot2ColorOp", em.Slot2ColorOp);
            sh.SetInt("uTexAlphaOp", em.TexAlphaOp);   sh.SetInt("uSlot2AlphaOp", em.Slot2AlphaOp);
            sh.SetInt("uAlphaMode", em.AlphaMode);     // fragmentAlphaMode@0x8A9 (3 = subtract/erosion alpha)
        }
        static void BindTex(DrawEmitter em, ShaderProgram sh, out bool hasTex, out bool fromLum)
        {
            hasTex = BindUnit(em.Tex, TextureUnit.Texture0); fromLum = hasTex && !em.HasAlpha;
        }

        public override void Draw(GL_ControlModern control, Pass pass)
        {
            FlushDeadBuffers();   // delete mesh buffers from swapped-out renders here, where the GL context is guaranteed current
            Prepare(control);     // ensure THIS control's shaders/vbo exist and shader/meshShader point at them (a reopened file uses a new viewport)
            if (pass != Pass.TRANSPARENT || shader == null || emitters.Count == 0) return;
            int prevFrame = frame;
            frame = clock.Advance();
            if (AutoFrame && !framed) { framed = true; try { control.FrameSelect(new List<Vector4>{ new Vector4(0f,0f,0f, frameRadius) }); } catch {} }   // frame camera once (suppressed on emitter-switch so the view persists)
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest); GL.DepthFunc(DepthFunction.Lequal); GL.DepthMask(false); // translucent: depth test on, write off (per-emitter zBufATest@0x88E below)
            GL.Disable(EnableCap.CullFace); GL.FrontFace(FrontFaceDirection.Ccw);                     // Both=NoCull default; eft winding CCW

            // ---- billboard pass ----
            control.CurrentShader = shader; control.UpdateModelMatrix(Matrix4.Identity);
            var cam=control.CameraMatrix;   // view matrix; rotation columns = camera world-space right/up axes (for camera-facing billboards)
            Vector3 cr=new Vector3(cam.M11,cam.M21,cam.M31), cu=new Vector3(cam.M12,cam.M22,cam.M32);
            if(cr.LengthSquared>1e-8f) cr.Normalize(); else cr=Vector3.UnitX;
            if(cu.LengthSquared>1e-8f) cu.Normalize(); else cu=Vector3.UnitY;
            Vector3 camFwd=Vector3.Cross(cu,cr);   // camera forward (for camera-facing stripe ribbons)
            // ---- cursor-follow emitter path (consumed by stripe ribbons only) ----
            // Project the mouse onto a camera-facing plane through the origin and record it each frame. Drag = orbit
            // camera (no follow); cursor off the viewport = hold the last position. Billboards/mesh ignore emPath.
            Vector3 emPos = emPathInit ? EmitterAt(frame-1) : Vector3.Zero;
            try {
                var cp = control.PointToClient(System.Windows.Forms.Cursor.Position);
                bool over = cp.X>=0 && cp.Y>=0 && cp.X<control.Width && cp.Y<control.Height;
                bool drag = System.Windows.Forms.Control.MouseButtons != System.Windows.Forms.MouseButtons.None;
                if (over && !drag && control.Width>0 && control.Height>0){
                    float nx=cp.X/(float)control.Width*2f-1f, ny=1f-cp.Y/(float)control.Height*2f;
                    // ZOOM-AWARE cursor->world: map onto the camera-facing plane through the origin using the LIVE camera
                    // distance + projection, so the ribbon sits under the cursor at ANY zoom. (Was scaled by the fixed
                    // frameRadius, so zooming desynced the cursor from the ribbon.) Visible half-extent at the origin plane
                    // = camera-distance * tan(halfFov); for an OpenTK perspective matrix tan(halfFovY)=1/Proj.M22,
                    // tan(halfFovX)=1/Proj.M11. Camera distance = |view-space position of the world origin| = |row4 of the view matrix|.
                    float dist=new Vector3(cam.M41,cam.M42,cam.M43).Length;
                    var proj=control.ProjectionMatrix;
                    float hw=(Math.Abs(proj.M11)>1e-6f)? dist/Math.Abs(proj.M11) : frameRadius;
                    float hh=(Math.Abs(proj.M22)>1e-6f)? dist/Math.Abs(proj.M22) : frameRadius;
                    emPos = cr*(nx*hw) + cu*(ny*hh);
                }
            } catch {}
            if (!emPathInit){ for(int k=0;k<emPath.Length;k++) emPath[k]=emPos; emPathInit=true; }
            // the wall clock can hold a frame across paints (fast refresh) or step several (slow
            // paint): refresh the current slot and fill any skipped ones so the trail has no stale gaps
            for (int fr=Math.Max(Math.Min(prevFrame+1, frame), frame-emPath.Length+1); fr<=frame; fr++)
                emPath[((fr%emPath.Length)+emPath.Length)%emPath.Length]=emPos;
            shader.SetVector3("uCamRight", cr); shader.SetVector3("uCamUp", cu);
            shader.SetInt("tex",0); shader.SetInt("tex1",1); shader.SetInt("tex2",2); shader.SetInt("uStripe",0); shader.SetInt("uDistort",0);
            shader.SetFloat("uf_alphaTestRef",0f);   // per-emitter alphaTestRef offset TBD; 0 = discard only fully-transparent
            int aPos=shader.GetAttribute("aPos"),aCol=shader.GetAttribute("aColor"),aSz=shader.GetAttribute("aSize"),
                aCor=shader.GetAttribute("aCorner"),aUV=shader.GetAttribute("aUVOff"),
                aUV1=shader.GetAttribute("aUVOff1"),aUV2=shader.GetAttribute("aUVOff2");   // B2 per-slot atlas UVs
            if (!shaderChecked) ReportShaderLink("particle-billboard", aPos);
            foreach (var em in emitters)
            {
                if (em.IsDistortion) continue;   // refraction emitters draw in the dedicated distortion pass below
                if (em.IsStripe || em.IsDirectional) {
                    // Directional_Y/Polygon (3/4) = per-particle velocity-stretched billboard (the rain/spark streak fix);
                    // legacy Plate_XZ (2) = trail. Both feed the camera-facing uStripe vertex path (aPos world, aCorner UV).
                    var sv = em.IsDirectional ? BuildDirectional(em, camFwd, cr, cu)
                           : em.StripeConnection ? BuildStripe(em, camFwd) : BuildTrail(em, camFwd); if(sv.Length==0) continue;
                    bool sht=BindUnit(em.Tex, TextureUnit.Texture0, em.SlotWrapU[0], em.SlotWrapV[0]), sht1=BindUnit(em.Tex1, TextureUnit.Texture1, em.SlotWrapU[1], em.SlotWrapV[1]), sht2=BindUnit(em.Tex2, TextureUnit.Texture2, em.SlotWrapU[2], em.SlotWrapV[2]);
                    bool sfl =sht  && em.Tex !=null && !DrawEmitter.FormatHasAlpha(em.Tex.Format);
                    bool sfl1=sht1 && em.Tex1!=null && !DrawEmitter.FormatHasAlpha(em.Tex1.Format);
                    shader.SetInt("uHasTex",sht?1:0); shader.SetInt("uAlphaFromLum",sfl?1:0); shader.SetInt("uAdditive",em.Additive?1:0);
                    shader.SetInt("uHasTex1",sht1?1:0); shader.SetInt("uAlphaFromLum1",sfl1?1:0); SetCombine(shader, em, sht2);
                    shader.SetVector3("uUVScale", new Vector3(em.UvScaleX,em.UvScaleY,0));
                    Blend(em); GL.Disable(EnableCap.CullFace); DepthState(em);   // ribbons are double-sided
                    shader.SetInt("uStripe",1);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                    GL.BufferData<float>(BufferTarget.ArrayBuffer, new IntPtr(sv.Length*4), sv, BufferUsageHint.StreamDraw);
                    Ptr(aPos,3,STRIDE,0); Ptr(aCol,4,STRIDE,12); Ptr(aSz,1,STRIDE,28); Ptr(aCor,2,STRIDE,32); Ptr(aUV,2,STRIDE,40); Ptr(aUV1,2,STRIDE,48); Ptr(aUV2,2,STRIDE,56);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, sv.Length/VFLOATS);
                    Off(aPos); Off(aCol); Off(aSz); Off(aCor); Off(aUV); Off(aUV1); Off(aUV2);
                    shader.SetInt("uStripe",0);
                    continue;
                }
                if (em.IsMesh) continue;
                var verts=BuildBillboards(em); if(verts.Length==0) continue;
                bool ht=BindUnit(em.Tex, TextureUnit.Texture0, em.SlotWrapU[0], em.SlotWrapV[0]);
                bool ht1=BindUnit(em.Tex1, TextureUnit.Texture1, em.SlotWrapU[1], em.SlotWrapV[1]);
                bool ht2=BindUnit(em.Tex2, TextureUnit.Texture2, em.SlotWrapU[2], em.SlotWrapV[2]);
                // Read format LIVE (BindUnit->LoadOpenGLTexture populates it; it is NOT reliably set at ctor time).
                bool fl =ht  && em.Tex !=null && !DrawEmitter.FormatHasAlpha(em.Tex.Format);
                bool fl1=ht1 && em.Tex1!=null && !DrawEmitter.FormatHasAlpha(em.Tex1.Format);
                shader.SetInt("uHasTex",ht?1:0); shader.SetInt("uAlphaFromLum",fl?1:0); shader.SetInt("uAdditive",em.Additive?1:0);
                shader.SetInt("uHasTex1",ht1?1:0); shader.SetInt("uAlphaFromLum1",fl1?1:0); SetCombine(shader, em, ht2);
                shader.SetVector3("uUVScale", new Vector3(em.UvScaleX,em.UvScaleY,0));
                Blend(em); Cull(em); DepthState(em);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData<float>(BufferTarget.ArrayBuffer, new IntPtr(verts.Length*4), verts, BufferUsageHint.StreamDraw);
                Ptr(aPos,3,STRIDE,0); Ptr(aCol,4,STRIDE,12); Ptr(aSz,1,STRIDE,28); Ptr(aCor,2,STRIDE,32); Ptr(aUV,2,STRIDE,40); Ptr(aUV1,2,STRIDE,48); Ptr(aUV2,2,STRIDE,56);
                GL.DrawArrays(PrimitiveType.Triangles, 0, verts.Length/VFLOATS);
                Off(aPos); Off(aCol); Off(aSz); Off(aCor); Off(aUV); Off(aUV1); Off(aUV2);
            }

            // ---- mesh pass ----
            control.CurrentShader = meshShader; control.UpdateModelMatrix(Matrix4.Identity);
            meshShader.SetInt("tex",0); meshShader.SetInt("tex1",1); meshShader.SetInt("tex2",2); meshShader.SetInt("uDistort",0);
            meshShader.SetFloat("uf_alphaTestRef",0f);
            int mP=meshShader.GetAttribute("mPos"), mU=meshShader.GetAttribute("mUV");
            if (!shaderChecked) { ReportShaderLink("particle-mesh", mP); shaderChecked = true; }
            foreach (var em in emitters)
            {
                if (!em.IsMesh || em.IsDistortion) continue;
                if (!em.meshReady){
                    GL.GenBuffers(1, out em.mvbo); GL.GenBuffers(1, out em.mibo);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, em.mvbo);
                    GL.BufferData<float>(BufferTarget.ArrayBuffer, new IntPtr(em.MeshVerts.Length*4), em.MeshVerts, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, em.mibo);
                    GL.BufferData<int>(BufferTarget.ElementArrayBuffer, new IntPtr(em.MeshIndices.Length*4), em.MeshIndices, BufferUsageHint.StaticDraw);
                    em.idxCount=em.MeshIndices.Length; em.meshReady=true;
                }
                bool ht=BindUnit(em.Tex, TextureUnit.Texture0, em.SlotWrapU[0], em.SlotWrapV[0]);
                bool ht1=BindUnit(em.Tex1, TextureUnit.Texture1, em.SlotWrapU[1], em.SlotWrapV[1]);
                bool ht2=BindUnit(em.Tex2, TextureUnit.Texture2, em.SlotWrapU[2], em.SlotWrapV[2]);
                bool fl =ht  && em.Tex !=null && !DrawEmitter.FormatHasAlpha(em.Tex.Format);
                bool fl1=ht1 && em.Tex1!=null && !DrawEmitter.FormatHasAlpha(em.Tex1.Format);
                meshShader.SetInt("uHasTex",ht?1:0); meshShader.SetInt("uAlphaFromLum",fl?1:0); meshShader.SetInt("uAdditive",em.Additive?1:0);
                meshShader.SetInt("uHasTex1",ht1?1:0); meshShader.SetInt("uAlphaFromLum1",fl1?1:0); SetCombine(meshShader, em, ht2);
                meshShader.SetVector3("uMeshUVScale", new Vector3(em.UvScaleX,em.UvScaleY,0));
                meshShader.SetVector3("uMeshUVOff", new Vector3((em.StaticIdx%em.Cols)*em.UvScaleX,(em.StaticIdx/em.Cols)*em.UvScaleY,0));
                meshShader.SetVector3("uMeshUVScale1", new Vector3(em.UvScaleX1,em.UvScaleY1,0));   // per-slot atlas: slot1/slot2 sample their OWN grid (cell 0); 1x1 slots -> (1,1) = whole texture (unchanged)
                meshShader.SetVector3("uMeshUVScale2", new Vector3(em.UvScaleX2,em.UvScaleY2,0));
                Blend(em); Cull(em); DepthState(em);
                GL.BindBuffer(BufferTarget.ArrayBuffer, em.mvbo);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, em.mibo);
                Ptr(mP,3,20,0); Ptr(mU,2,20,12);   // mesh stride 20
                foreach (var pt in Sim(em, MRATE)){
                    meshShader.SetVector3("uPos", pt.Item1);
                    meshShader.SetFloat("uSize", pt.Item4);
                    meshShader.SetVector3("uColor", pt.Item2);
                    meshShader.SetFloat("uAlpha", pt.Item3);
                    GL.DrawElements(PrimitiveType.Triangles, em.idxCount, DrawElementsType.UnsignedInt, 0);
                }
                Off(mP); Off(mU);
            }

            // ---- DISTORTION (refraction) pass ----
            // CAPTURE-DERIVED (eid14551): a distortion emitter resamples the rendered scene at an offset taken from its slot0
            // normal/indirection map -> out.rgb = scene(screenUV + (n.rg*2-1)*strength*alpha). The forward passes above skipped
            // IsDistortion emitters; here we GRAB what they drew (scene + particles) into a texture and refract it. One pass for
            // the whole _ind/Haze/Dist class: water ripples warp the floor grid, heat haze shimmers, barrier seals distort, etc.
            bool anyDistort=false; foreach(var de in emitters) if(de.IsDistortion){ anyDistort=true; break; }
            if (anyDistort)
            {
                int vw=Math.Max(1,control.Width), vh=Math.Max(1,control.Height);
                if (sceneTex==0) sceneTex=GL.GenTexture();
                GL.ActiveTexture(TextureUnit.Texture5);
                GL.BindTexture(TextureTarget.Texture2D, sceneTex);
                if (vw!=sceneW || vh!=sceneH){
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, vw, vh, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    sceneW=vw; sceneH=vh;
                }
                GL.CopyTexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, 0, 0, vw, vh);   // copy the current framebuffer colour into sceneTex
                Vector3 vp=new Vector3(vw, vh, 0f);

                // distortion BILLBOARDS
                control.CurrentShader = shader; control.UpdateModelMatrix(Matrix4.Identity);
                shader.SetInt("tex",0); shader.SetInt("uSceneTex",5); shader.SetVector3("uViewport", vp);
                shader.SetInt("uDistort",1); shader.SetFloat("uDistortStrength", DISTORT_STRENGTH); shader.SetInt("uStripe",0);
                foreach (var em in emitters)
                {
                    if (!em.IsDistortion || em.IsMesh) continue;
                    var verts=BuildBillboards(em); if(verts.Length==0) continue;
                    BindUnit(em.Tex, TextureUnit.Texture0, em.SlotWrapU[0], em.SlotWrapV[0]);
                    Blend(em); Cull(em); DepthState(em);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                    GL.BufferData<float>(BufferTarget.ArrayBuffer, new IntPtr(verts.Length*4), verts, BufferUsageHint.StreamDraw);
                    Ptr(aPos,3,STRIDE,0); Ptr(aCol,4,STRIDE,12); Ptr(aSz,1,STRIDE,28); Ptr(aCor,2,STRIDE,32); Ptr(aUV,2,STRIDE,40); Ptr(aUV1,2,STRIDE,48); Ptr(aUV2,2,STRIDE,56);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, verts.Length/VFLOATS);
                    Off(aPos); Off(aCol); Off(aSz); Off(aCor); Off(aUV); Off(aUV1); Off(aUV2);
                }
                shader.SetInt("uDistort",0);

                // distortion MESHES (e.g. the ripple ring)
                control.CurrentShader = meshShader; control.UpdateModelMatrix(Matrix4.Identity);
                meshShader.SetInt("tex",0); meshShader.SetInt("uSceneTex",5); meshShader.SetVector3("uViewport", vp);
                meshShader.SetInt("uDistort",1); meshShader.SetFloat("uDistortStrength", DISTORT_STRENGTH);
                foreach (var em in emitters)
                {
                    if (!em.IsDistortion || !em.IsMesh) continue;
                    if (!em.meshReady){
                        GL.GenBuffers(1, out em.mvbo); GL.GenBuffers(1, out em.mibo);
                        GL.BindBuffer(BufferTarget.ArrayBuffer, em.mvbo);
                        GL.BufferData<float>(BufferTarget.ArrayBuffer, new IntPtr(em.MeshVerts.Length*4), em.MeshVerts, BufferUsageHint.StaticDraw);
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, em.mibo);
                        GL.BufferData<int>(BufferTarget.ElementArrayBuffer, new IntPtr(em.MeshIndices.Length*4), em.MeshIndices, BufferUsageHint.StaticDraw);
                        em.idxCount=em.MeshIndices.Length; em.meshReady=true;
                    }
                    BindUnit(em.Tex, TextureUnit.Texture0, em.SlotWrapU[0], em.SlotWrapV[0]);
                    Blend(em); Cull(em); DepthState(em);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, em.mvbo);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, em.mibo);
                    Ptr(mP,3,20,0); Ptr(mU,2,20,12);
                    foreach (var pt in Sim(em, MRATE)){
                        meshShader.SetVector3("uPos", pt.Item1); meshShader.SetFloat("uSize", pt.Item4); meshShader.SetFloat("uAlpha", pt.Item3);
                        GL.DrawElements(PrimitiveType.Triangles, em.idxCount, DrawElementsType.UnsignedInt, 0);
                    }
                    Off(mP); Off(mU);
                }
                meshShader.SetInt("uDistort",0);
                GL.ActiveTexture(TextureUnit.Texture5); GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.ActiveTexture(TextureUnit.Texture0);
            }
            GL.DepthMask(true); GL.UseProgram(0);
            // Restore texture-unit state: unbind every unit and leave unit0 active, so the framework's gizmo/axis-box
            // (which assumes unit0) does not sample a leftover particle texture.
            GL.ActiveTexture(TextureUnit.Texture2); GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture1); GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0); GL.BindTexture(TextureTarget.Texture2D, 0);
            // Particle sim advances one tick per Draw; the viewport only repaints on demand (mouse/camera), so request the
            // next repaint to drive continuous animation. VSync (Viewport sets it) caps this at the refresh rate.
            try { RequestNextFrame(control); } catch { }
        }

        // Per-instance GL cleanup. Called from the UI thread when this render is swapped out of the editor preview; the GL
        // context may not be current there, so we only QUEUE the per-emitter mesh buffers and delete them on the next Draw
        // (FlushDeadBuffers). The shared shader/meshShader/vbo/sceneTex are app-lifetime and intentionally NOT freed here.
        public void QueueDispose()
        {
            lock (deadBuffers) {
                foreach (var em in emitters) {
                    if (em.mvbo != 0) { deadBuffers.Add(em.mvbo); em.mvbo = 0; }
                    if (em.mibo != 0) { deadBuffers.Add(em.mibo); em.mibo = 0; }
                    em.meshReady = false;
                }
            }
        }
        static void FlushDeadBuffers()
        {
            lock (deadBuffers) {
                if (deadBuffers.Count == 0) return;
                foreach (int b in deadBuffers) if (b != 0) GL.DeleteBuffer(b);
                deadBuffers.Clear();
            }
        }
        static void Ptr(int loc,int n,int stride,int off){ if(loc>=0){ GL.EnableVertexAttribArray(loc);
            GL.VertexAttribPointer(loc,n,VertexAttribPointerType.Float,false,stride,off);} }
        static void Off(int loc){ if(loc>=0) GL.DisableVertexAttribArray(loc); }
        [System.Runtime.InteropServices.DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        // True only when our main window is the foreground window. When a modal dialog (the save / Yaz0-compress prompt)
        // or another app is in front, this is false, so the preview pauses its repaint loop -> the dialog is not starved
        // or buried, and we stop burning CPU in the background. Resumes when the window repaints on regaining focus.
        // Defaults to true on any error (keep animating).
        internal static bool AppIsForeground()
        {
            try { var mf = Toolbox.Library.Runtime.MainForm; return mf == null || !mf.IsHandleCreated || GetForegroundWindow() == mf.Handle; }
            catch { return true; }
        }
        // The repaint that drives the animation: requested once per Draw, served from the idle pump. Windows raises
        // WM_PAINT only for an otherwise empty queue, so a viewport that invalidates itself from inside its own paint
        // keeps one permanently pending and the FIRST paint of every sibling control (the GPU toggle, the viewport's
        // own menu and model row) is never dispatched; they stay blank until a hover forces a synchronous repaint.
        // Idle is raised only once the queue, pending paints included, has drained, so those paints land first and the
        // animation still runs at the paint rate. Self-terminating: a request is consumed when it is served, so a
        // viewport that is no longer drawing a preview stops being repainted.
        static readonly List<System.Windows.Forms.Control> repaintTargets = new List<System.Windows.Forms.Control>();
        static bool idleHooked;
        internal static void RequestNextFrame(System.Windows.Forms.Control control)
        {
            if (control != null && !repaintTargets.Contains(control)) repaintTargets.Add(control);   // one entry per viewport
            if (idleHooked) return;
            idleHooked = true;
            System.Windows.Forms.Application.Idle += OnIdleRepaint;
        }
        static void OnIdleRepaint(object sender, EventArgs e)
        {
            if (repaintTargets.Count == 0) return;
            var pending = repaintTargets.ToArray();
            repaintTargets.Clear();
            if (!AppIsForeground()) return;
            foreach (var c in pending)
                if (!c.IsDisposed && c.IsHandleCreated) c.Invalidate();
        }
        // Wall-clock playback frame for the previews (software + GPU drawable). Sim frames are 60fps
        // ticks (velocities = field/60, lifespans = frame counts), but paints arrive at the monitor's
        // refresh rate or as fast as the repaint loop spins, so advance by elapsed wall time. A long gap
        // between paints (window hidden, modal dialog, stall) counts as pause, preserving the
        // foreground-pause behaviour.
        internal sealed class PlaybackClock
        {
            readonly System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            double seconds;
            public int Advance()
            {
                double dt = sw.Elapsed.TotalSeconds;
                sw.Restart();
                if (dt < 0.25) seconds += dt;
                return (int)(seconds * 60.0);
            }
        }
        // Surface GLSL compile/link failures to the toolbox console instead of failing silently. A non-ASCII char in the
        // inline GLSL (even in a comment), or any syntax error, makes the program never link -> every GetAttribute returns
        // -1 and the draw renders NOTHING with no error shown.
        static void ReportShaderLink(string name, int probeAttrLoc)
        {
            if (probeAttrLoc >= 0) return;   // a real vertex attribute resolved -> the program linked fine
            Console.WriteLine("[EftEmitterRender] GLSL SHADER '" + name + "' FAILED TO LINK (a vertex attribute resolves to -1; the " +
                "draw renders NOTHING). Likely cause: a non-ASCII char in the inline GLSL (even in a comment) or a syntax error.");
        }
        void Blend(DrawEmitter em){ if(em.Additive) GL.BlendFunc(BlendingFactor.One,BlendingFactor.One);
            else GL.BlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.OneMinusSrcAlpha); }
        void Cull(DrawEmitter em){
            if(em.DispSide==1){ GL.Enable(EnableCap.CullFace); GL.CullFace(CullFaceMode.Back); }
            else if(em.DispSide==2){ GL.Enable(EnableCap.CullFace); GL.CullFace(CullFaceMode.Front); }
            else GL.Disable(EnableCap.CullFace);
        }
        void DepthState(DrawEmitter em){
            if(em.ZBuf==1) GL.Disable(EnableCap.DepthTest);                                  // Ignore_Z
            else { GL.Enable(EnableCap.DepthTest); GL.DepthFunc(DepthFunction.Lequal); }      // Normal (depth test; write off globally)
        }

        // per-particle sim -> (worldPos, colorRGB, alpha, sizeWorld)
        // CPU sim driven by decoded EMTR motion fields, matching open-ead/NW4F-Eft. Initial velocity = Stage1 (shape-normal *
        // allDirVel, omnidirectional) + Stage2 (cone about the dir VEC3 * dirVel, directional); damped per frame by AirResist;
        // lifetime = Lifespan frames. GRAVITY is flag-off across BotW (verified: no isolable accel VEC3 over 9572 emitters) ->
        // falling = downward dir initial velocity, NOT a parabola. VEL = preview velocity compression (faithful absolute = 1.0).
        // Per-particle constants drawn once at birth; position is CLOSED-FORM in age, so PosAt(a) reconstructs any past
        // position with no stored state -> lets the trail-stripe builder walk a particle's history analytically.
        struct Ptcl {
            public Vector3 Ep, V0; public float Rf, AirResist, Ang, Grav; public int Age, Life;   // V0 = initial velocity (vShape+vDir); Grav = per-frame Y accel
            public int Seq;   // index within the particle's emission burst (stream randoms seed per (birth, Seq))
            public Vector3 PosAt(int n){
                if (n<0) n=0;
                float ar=AirResist;
                float s1=(ar>0.999f)? n : (1f-(float)Math.Pow(ar,n))/(1f-ar);             // velocity contribution (S1)
                float s2=(ar>0.999f)? n*(n-1)*0.5f : (n - s1)/(1f-ar);                     // gravity double-sum (S2): constant accel -> closed form
                return Ep + V0*(s1*Rf) + new Vector3(0f,Grav,0f)*(s2*Rf);
            }
        }

        // nw::eft Stage-1 shape normal: the outward unit direction the emission VOLUME imparts (emitFunction@0x714).
        // Sphere/Point family (+ BotW burst family) = random unit sphere -> this is what gives EXPLOSIONS their omni blast.
        static Vector3 ShapeDir(int fn, Vector3 ep, Random r){
            if (fn==1||fn==2||fn==3||fn==8||fn==9){                                   // Circle/Cylinder: radial in XZ (y=0)
                double th=r.NextDouble()*6.2831853; return new Vector3((float)Math.Cos(th),0f,(float)Math.Sin(th));
            }
            if (fn==12||fn==13) return Vector3.UnitZ;                                  // Line: along +Z
            // Sphere/Point/Box/burst family: outward = normalize(scaled spawn pos) (decomp FillSphere/Box v=normalize(pos)).
            // A FLAT emission volume (rain (15,7.5,15)) thus scatters HORIZONTALLY, not in a chaotic full sphere.
            if (ep.LengthSquared>1e-8f){ var d=ep; d.Normalize(); return d; }
            return RandSphere(r);                                                     // degenerate point / zero-volume: random sphere
        }
        static Vector3 RandSphere(Random r){
            float y=(float)(r.NextDouble()*2.0-1.0); double ph=r.NextDouble()*6.2831853; float a=(float)Math.Sqrt(Math.Max(0f,1f-y*y));
            return new Vector3(a*(float)Math.Cos(ph), y, a*(float)Math.Sin(ph));
        }
        // nw::eft Stage-2 cone: a +Y-centred cone of half-angle `dispRad`, rotated so its AXIS becomes `dir` (eft_Emitter.cpp:310-330).
        // NW4F's dispersionAngle is a 0..90 DEGREE half-angle (t=1-angle/90); the field is radians -> convert (the #1 wiring risk).
        static Vector3 ConeAboutDir(Vector3 dir, float dispRad, Random r){
            float halfDeg=Math.Min(90f, Math.Max(0f, dispRad*57.29578f));
            float t=1f - halfDeg/90f;                                                 // t=0 at 90deg (hemisphere), t=1 at 0deg (beam)
            double phi=r.NextDouble()*6.2831853;
            float y=(float)(r.NextDouble()*(1.0-t)+t);                                // y in [t,1], +Y-centred
            float a=(float)Math.Sqrt(Math.Max(0f,1f-y*y));
            return RotateFromTo(Vector3.UnitY, dir, new Vector3(a*(float)Math.Cos(phi), y, a*(float)Math.Sin(phi)));
        }
        // Rotate v by the rotation mapping unit `from` -> unit `to`. Handles dir==-Y (the common rain case) without NaN.
        static Vector3 RotateFromTo(Vector3 from, Vector3 to, Vector3 v){
            if (to.LengthSquared<1e-8f) return v; to.Normalize();
            float d=Vector3.Dot(from,to);
            if (d>0.9999f) return v;                                                  // parallel (dir up): identity
            if (d<-0.9999f){                                                          // antiparallel (dir=(0,-1,0)): 180deg flip
                Vector3 ax=Vector3.Cross(from,Vector3.UnitX);
                if (ax.LengthSquared<1e-6f) ax=Vector3.Cross(from,Vector3.UnitZ);
                ax.Normalize(); return RotateAxis(v,ax,(float)Math.PI);
            }
            Vector3 axis=Vector3.Cross(from,to); axis.Normalize();
            return RotateAxis(v,axis,(float)Math.Acos(d));
        }
        static Vector3 RotateAxis(Vector3 v, Vector3 k, float ang){                    // Rodrigues rotation of v about unit k
            float c=(float)Math.Cos(ang), s=(float)Math.Sin(ang);
            return v*c + Vector3.Cross(k,v)*s + k*(Vector3.Dot(k,v)*(1f-c));
        }

        static int NameSeed(string name){                                               // FNV-1a over the emitter name
            uint h=2166136261u;
            foreach (char c in name) h=(h^c)*16777619u;
            return unchecked((int)h);
        }

        // Per-particle RNG seed. The stream build needs one that is stable across processes, so a given
        // emitter places identical particles every run (string.GetHashCode is randomized per process); the
        // software preview keeps the hash it has always seeded with.
        int EmitSeed(string name){ return faithfulUnits ? NameSeed(name) : name.GetHashCode(); }

        // Emission core: yields one Ptcl per live particle (gating + the exact per-particle random draw order). Sim() and the
        // stripe/trail builders all consume this so the random sequence is defined in ONE place (existing renders unchanged).
        IEnumerable<Ptcl> Emit(DrawEmitter em, int rate)
        {
            int life=Math.Max(4,Math.Min(180,em.Lifespan));
            int interval=Math.Max(1,em.EmitInterval);
            // perEmit: single-burst (infinite-endFrame, e.g. ShockWave) emits once = mesh ring(1)/billboard spray(rate);
            // finite-endFrame one-shot (e.g. Debris) emits 1 per interval over its window (faithful rate when emitRate==0, so the
            // origin stays replenished and the spray is visible); continuous (ls>1) uses the decoded rate.
            int perEmit;
            if      (em.SingleBurst) perEmit = em.IsMesh ? 1 : rate;
            else if (em.OneShot)    perEmit = 1;
            // weather steady state: sustain WeatherAlive live particles
            // (alive = perEmit * life / interval, so perEmit = alive * interval / life)
            else if (em.WeatherAlive > 0) perEmit = Math.Max(1, (int)Math.Round(em.WeatherAlive*interval/(float)life));
            // Stream mode emits the file's emission rate (0x6F4) as particles-per-second at 60fps, which is
            // load-bearing for the dense emitters that only read as a cloud in bulk (low rates round to 1 and
            // the ALIVE_CAP clamp below bounds the rest). Rates above 10000 are "fill as fast as possible"
            // sentinels rather than literal counts, so they keep perEmit = 1.
            else if (faithfulUnits) perEmit = (em.EmitRate > 0f && em.EmitRate <= 10000f)
                                                ? Math.Max(1, (int)Math.Round(em.EmitRate / 60f)) : 1;
            else                    perEmit = Math.Max(1,Math.Min(rate,(int)Math.Round(em.EmitRate<=0f?rate:em.EmitRate)));
            // Alive-cap for continuous emitters: they stream perEmit every `interval` for `life` frames, so a high decoded
            // rate -> hundreds of live particles ("several hundred spawned at once"). Bound the on-screen count so dense
            // effects still read as dense without flooding the view. General (not per-effect); one-shots already emit 1/burst.
            if (!em.SingleBurst && !em.OneShot && em.WeatherAlive == 0){   // the weather profile's own count is already the bound
                int emitFrames = Math.Max(1, life/interval);
                perEmit = Math.Max(1, Math.Min(perEmit, ALIVE_CAP/emitFrames));
            }
            // Emission window end. Finite endFrame -> that. Else: inside a ONE-SHOT effect a continuous emitter stops at
            // effectActive (so the effect finishes and PAUSES); in an AMBIENT effect it streams the whole cycle (no pause).
            int emitEnd = (em.EndFrame>=2) ? em.EndFrame : (oneShotEffect ? effectActive : cycle);
            int cyc = Math.Max(life+1, cycle);                      // guarantee at most one instance alive for a single-burst one-shot
            // GAME units (stream mode): emission spans +-volumeScale (captured Haze_30 births span
            // +-27 of volScale 30) and velocity fields are units/second at 60fps (captured birth
            // vel -1/60 of dirVel 1.0). Preview units: compressed for watchability (see VEL).
            float volK = faithfulUnits ? 2f : 0.05f*motionScale;    // (rand-0.5)*volScale*volK -> +-volScale when faithful
            float vel = faithfulUnits ? (1f/60f) : VEL;
            for (int birth=Math.Max(0,frame-life); birth<=frame; birth++){
                int local=((birth%cyc)+cyc)%cyc;
                bool emit = em.SingleBurst ? (local==0) : (local<emitEnd && local%interval==0);
                if (!emit) continue;
                var r=new Random(unchecked(birth*7919 + EmitSeed(em.Name)));
                for(int p=0;p<perEmit;p++){
                    int age=frame-birth; if(age<0||age>=life) continue;
                    var ep=new Vector3((float)(r.NextDouble()-0.5)*em.VolScale.X,
                                       (float)(r.NextDouble()-0.5)*em.VolScale.Y,
                                       (float)(r.NextDouble()-0.5)*em.VolScale.Z)*volK;
                    // nw::eft two-stage velocity. STAGE 1: omnidirectional shape-burst = shapeNormal*allDirVel (explosions).
                    // STAGE 2: directional cone about Dir = cone(dispersion,axis=Dir)*dirVel (rain falls; Dir=(0,-1,0)).
                    // allDirVel >> dirVel for blasts -> Stage 1 wins -> spherical; allDirVel << for rain -> Stage 2 wins -> down.
                    // SINGLE-particle burst (one mesh ring: ripple/ShockWave): Stage-1's outward-from-shape collapses to one
                    // random heading and just SLIDES the ring (the population-spread is meaningless for N=1) -> suppress it;
                    // the ring's real growth is its SCALE curve (stays centered). For N=1 Stage-2 follows the dir AXIS directly
                    // (no population dispersion cone), so any motion is the clean directional sink (ripple sinks per dir.Y).
                    bool singleParticle = (em.SingleBurst && perEmit==1);
                    Vector3 v0;
                    // vtxMode-4 drops (rain/snow/dust/fog) move by an UNCOMPRESSED per-frame fall
                    // vector straight along dir, with no Stage-1 shape term and no dispersion cone.
                    // The magnitude is the weather system's own (profile FallMin..FallMax: rain
                    // 1.08-1.20/frame vs snow 0.04-0.20 vs hovering dust, all from the SAME file
                    // dir x dirVel 1); unprofiled class members keep the rain-anchored
                    // dirVel x [1..1.15]. The /60 velocity law applies to units/second fields
                    // (Haze), not to this class.
                    if (faithfulUnits && em.VtxMode==4 && em.DirLen>0.5f){
                        float fall = em.WeatherAlive > 0
                            ? em.WeatherFallMin + (em.WeatherFallMax - em.WeatherFallMin)*(float)r.NextDouble()
                            : em.DirVel*(1f + 0.15f*(float)r.NextDouble());
                        v0 = em.Dir*(fall/em.DirLen);
                    } else {
                        v0 = singleParticle ? Vector3.Zero : ShapeDir(em.EmitFunc, ep, r)*(em.AllDirVel*vel);
                        if (em.DirLen>0.5f && em.DirVel>1e-4f){
                            Vector3 dirV;
                            if (singleParticle){ dirV=em.Dir; if(dirV.LengthSquared>1e-8f) dirV.Normalize(); }
                            else                dirV=ConeAboutDir(em.Dir, em.Dispersion, r);
                            v0 += dirV*(em.DirVel*vel);
                        }
                    }
                    // A lone ring's TRANSLATION must stay below its own size, else the dir.Y sink/drift dwarfs the
                    // scale-curve EXPANSION (the defining motion of a ring). ShockWave (ring grows to ~18u) keeps its
                    // small sink unchanged; the tiny Ripple ring (grows to ~0.70u) had a 1.8u sink that buried the
                    // growth -> cap it so the expansion reads. (Approximates the per-emitter scale that scales velocity
                    // with effect size; absolute dirVel over-translates a small ring.)
                    if (singleParticle && em.IsMesh && v0.LengthSquared>1e-12f){
                        float ms=0.01f; for(int k=0;k<8;k++) ms=Math.Max(ms, em.Scale[k,0]);     // peak scaleX over life
                        float peak=Math.Max(em.Radius*ms, 0.05f);
                        float s1L=(em.AirResist>0.999f)? life : (1f-(float)Math.Pow(em.AirResist,life))/(1f-em.AirResist);
                        float travel=v0.Length*s1L, cap=RING_TRANSLATE_FRAC*peak;
                        if (travel>cap) v0 *= cap/travel;
                    }
                    float rf=1f - em.MomRand*(float)(r.NextDouble()*2.0-1.0);                     // momentumRandom@0x7C4: per-particle speed spread
                    float ang=0f;                                                                  // particle Z-spin: init@0x6C8 (2pi=random) + angularVelocity@0x6D8*age
                    if (em.RotEnabled){ float ini=(em.RotInit>=6.0f)?(float)(r.NextDouble()*6.2831853):em.RotInit; ang=ini + em.AngVel*age; }
                    yield return new Ptcl{ Ep=ep, V0=v0, Rf=rf, AirResist=em.AirResist, Ang=ang, Grav=GRAVITY_Y, Age=age, Life=life, Seq=p };
                }
            }
        }

        // per-particle sim -> (worldPos, colorRGB, alpha, sizeWorld, t, spinAngle)
        IEnumerable<Tuple<Vector3,Vector3,float,float,float,float>> Sim(DrawEmitter em, int rate)
        {
            foreach (var pt in Emit(em, rate)){
                float t=pt.Age/(float)pt.Life;
                var pos=pt.PosAt(pt.Age);
                var col=em.ColorAt(t); float al=em.AlphaAt(t); float sc=em.ScaleAt(t);
                float sz=Math.Max(em.Radius,0.01f)*Math.Max(sc,0f);
                yield return Tuple.Create(pos, col, al, sz, t, pt.Ang);
            }
        }

        /// <summary>Per-instance vertex streams for the GPU preview: one float4 row per live particle,
        /// in the layout the game's own EFT vertex shaders consume. InPos = the particle's CURRENT
        /// position, InVec = its current per-frame velocity (the streak direction and length); the
        /// uniformMode-3 billboard VS renders the particle AT InPos, stretches it along InVec, and
        /// ignores LocalPos/LocalVec xyz entirely. w-channel bookkeeping: InPos w = 0, InVec w = age,
        /// LocalPos w = per-particle lifespan, LocalVec w = BIRTH emitterTime; the VS computes
        /// age = bank8 emitterTime - LocalVec.w, gates 0 &lt;= age &lt; int(lifespan) and keys every
        /// color/alpha/scale curve on it. Random = 4 uniform [0,1) values stable for the particle's
        /// life, seeded independently of the motion RNG so existing renders are unchanged.
        /// Scale = the BIRTH scale ptclScaleStart @0x978 (the VS applies the scale-over-life curve
        /// from bank7). The shader does the rest (color/alpha curves, flipbook, rotation,
        /// billboarding) from the EmitterStatic bank; the sim only integrates motion and keeps this
        /// bookkeeping.</summary>
        public class InstanceStreams
        {
            public int Count;                                          // live particles
            public float[] InPos, InVec, LocalPos, LocalVec, Scale, Random;   // Count*4 floats each
        }

        /// <summary>The emitter-lifecycle alpha1 envelope (raw track @0x4F0) evaluated at the
        /// playback frame, normalized over the effect's active window (one-shot: emit + fade,
        /// then the pause holds the tail value; ambient: the whole cycle). Splash-family
        /// shaders read this as a runtime-maintained bank7 slot, not a static curve key:
        /// their erosion scale is bank7[0x540].x, and a fade-in curve's static key 0 is 0,
        /// which erodes every pixel of the draw.</summary>
        public float EnvelopeAlpha1(int emitterIndex, int atFrame)
        {
            if (emitterIndex < 0 || emitterIndex >= emitters.Count) return float.NaN;
            var em = emitters[emitterIndex];
            if (em.Alpha1N == 0) return float.NaN;   // disabled track: the bank's const-fill law already applies
            int cyc = Math.Max(1, cycle);
            int span = oneShotEffect ? Math.Min(cyc, effectActive + em.Lifespan) : cyc;
            float t = (((atFrame % cyc) + cyc) % cyc) / (float)Math.Max(1, span);
            return em.Alpha1At(Math.Min(t, 1f));
        }

        /// <summary>The alpha0 twin of EnvelopeAlpha1, for shaders that read ONLY the track's
        /// key-0 slot (bank7[68].x = bank 0x440): the runtime maintains the evaluated envelope
        /// there. NaN skips the patch: disabled track, or the channel-1 fallback adopted alpha1
        /// as Alpha0 (the true channel-0 track is blank, so the game's slot stays at the static
        /// key).</summary>
        public float EnvelopeAlpha0(int emitterIndex, int atFrame)
        {
            if (emitterIndex < 0 || emitterIndex >= emitters.Count) return float.NaN;
            var em = emitters[emitterIndex];
            if (em.Alpha0N == 0 || em.Alpha0Adopted) return float.NaN;
            int cyc = Math.Max(1, cycle);
            int span = oneShotEffect ? Math.Min(cyc, effectActive + em.Lifespan) : cyc;
            float t = (((atFrame % cyc) + cyc) % cyc) / (float)Math.Max(1, span);
            return em.Alpha0EnvAt(Math.Min(t, 1f));
        }

        public InstanceStreams BuildInstanceStreams(int emitterIndex, int atFrame)
        {
            if (emitterIndex < 0 || emitterIndex >= emitters.Count)   // input was rejected at parse (not drawable)
                return new InstanceStreams {
                    Count = 0,
                    InPos = new float[0], InVec = new float[0], LocalPos = new float[0],
                    LocalVec = new float[0], Scale = new float[0], Random = new float[0],
                };
            var em = emitters[emitterIndex];
            int saved = frame;
            frame = atFrame;
            faithfulUnits = true;
            var pts = new List<Ptcl>();
            try { foreach (var pt in Emit(em, RATE)) pts.Add(pt); }
            finally { frame = saved; faithfulUnits = false; }
            var s = new InstanceStreams {
                Count = pts.Count,
                InPos = new float[pts.Count*4], InVec = new float[pts.Count*4],
                LocalPos = new float[pts.Count*4], LocalVec = new float[pts.Count*4],
                Scale = new float[pts.Count*4], Random = new float[pts.Count*4],
            };
            for (int i = 0; i < pts.Count; i++){
                var pt = pts[i];
                int birth = atFrame - pt.Age;
                var pos = pt.PosAt(pt.Age);
                var vel = pos - pt.PosAt(pt.Age - 1);                  // frame-difference = current velocity
                var rr = new Random(unchecked(birth*7919 + NameSeed(em.Name)*31 + pt.Seq*104729));
                int o = i*4;
                s.InPos[o]=pos.X;      s.InPos[o+1]=pos.Y;      s.InPos[o+2]=pos.Z;      s.InPos[o+3]=0f;
                s.InVec[o]=vel.X;      s.InVec[o+1]=vel.Y;      s.InVec[o+2]=vel.Z;      s.InVec[o+3]=pt.Age;
                s.LocalPos[o]=pos.X;   s.LocalPos[o+1]=pos.Y;   s.LocalPos[o+2]=pos.Z;   s.LocalPos[o+3]=pt.Life;
                // vtxMode-4 drops need no special-casing here: Emit() gives the class the
                // uncompressed per-frame fall as its velocity, so LocalVec (streak direction)
                // and the motion agree by construction.
                s.LocalVec[o]=vel.X;   s.LocalVec[o+1]=vel.Y;   s.LocalVec[o+2]=vel.Z;   s.LocalVec[o+3]=birth;
                s.Random[o]=(float)rr.NextDouble(); s.Random[o+1]=(float)rr.NextDouble();
                s.Random[o+2]=(float)rr.NextDouble(); s.Random[o+3]=(float)rr.NextDouble();
                // The stream carries the BIRTH scale (ptclScaleStart @0x978) x the ptclScaleRandom law
                // (@0x984, see the parse comment); the VS applies the scale-over-life curve from bank7.
                // Draw the random u AFTER the 4 Random values so their sequence (and every
                // random-dependent render) is unchanged.
                float su=(float)rr.NextDouble();
                s.Scale[o]  =em.BaseScale.X*(1f-su*em.ScaleRand.X*0.01f);
                s.Scale[o+1]=em.BaseScale.Y*(1f-su*em.ScaleRand.Y*0.01f);
                s.Scale[o+2]=em.BaseScale.Z*(1f-su*em.ScaleRand.Z*0.01f);
                s.Scale[o+3]=1f;
            }
            return s;
        }

        float[] BuildBillboards(DrawEmitter em)
        {
            var v=new List<float>();
            foreach (var pt in Sim(em, RATE)){
                var pos=pt.Item1; var col=pt.Item2; float al=pt.Item3; float szw=pt.Item4; float t=pt.Item5; float ang=pt.Item6;
                float half=Math.Max(0.0001f, szw*0.5f);   // WORLD half-size (= radius*scale*0.5); the camera is framed to the effect extent so relative sizes are faithful
                // flipbook cell = per-emitter STATIC index (0xD0). The cell is HELD for life, NOT animated over t.
                // GROUNDED (B1): the 12 GameResident "Flower" emitters each hold StaticIdx 0..11 of one 4x4 atlas
                // (one emitter per flower variant) -> static per-emitter pick, never a cycle; MaxAdd capture eid14862
                // shows all 7 particles at cell 0 (no 0.5 U-offset across varied ages). nw::eft confirms: Emit() sets
                // the cell once (cell 0 when texPtnAnimNum<=1) and only CalcBehavior::TexPtnAnim overwrites it per frame
                // WHEN the hasTexPtnAnim flag is set; that enable flag's BotW offset is not yet located, and the
                // evidence (Flower/MaxAdd/Spike all static) shows the common case is NOT animated. So default to the
                // faithful static StaticIdx (this also makes the billboard path consistent with the mesh path/v22).
                int cell=em.StaticIdx % em.Frames; if(cell<0) cell+=em.Frames;
                float uox=(cell%em.Cols)*em.UvScaleX, uoy=(cell/em.Cols)*em.UvScaleY;
                float ca=(float)Math.Cos(ang), sa=(float)Math.Sin(ang);   // particle spin: rotate the quad CORNER (position) but bake the UNROTATED UV so the sprite image spins
                float sxk,syk; em.ScaleXYAt(t, out sxk, out syk); float aspect=(sxk>1e-5f)?syk/sxk:1f;   // non-uniform: scale the quad height by scaleY/scaleX (half = radius*scaleX)
                for(int k=0;k<6;k++){
                    float cx=CORNERS[k,0], cyo=CORNERS[k,1]; float cy=cyo*aspect;            // cyo = original corner (UV); cy = aspect-scaled (position)
                    float rx=cx*ca-cy*sa, ry=cx*sa+cy*ca;                                  // rotated position-corner -> aCorner
                    float uvm=em.MirrorUV?2f:1f;   // mirror sprites sample a 2x span ([0,2]); with MirroredRepeat the stored quadrant tiles into the full image (ring centered on the texture corner @ quad centre)
                    float ru=cx*0.5f+0.5f, rv=cyo*0.5f+0.5f;                                   // raw unrotated corner UV (0..1)
                    float uvx=ru*uvm*em.UvScaleX+uox, uvy=rv*uvm*em.UvScaleY+uoy;               // slot0 baked UV (StaticIdx cell + mirror) -> aUVOff
                    // B2 per-slot atlas: slot1/slot2 sample their OWN grid at cell 0 (static, like the mesh path); 1x1 slot -> ru*1 = whole texture (unchanged)
                    float uv1x=ru*em.UvScaleX1, uv1y=rv*em.UvScaleY1, uv2x=ru*em.UvScaleX2, uv2y=rv*em.UvScaleY2;
                    v.Add(pos.X);v.Add(pos.Y);v.Add(pos.Z);
                    v.Add(col.X);v.Add(col.Y);v.Add(col.Z);v.Add(al);
                    v.Add(half); v.Add(rx);v.Add(ry); v.Add(uvx);v.Add(uvy); v.Add(uv1x);v.Add(uv1y); v.Add(uv2x);v.Add(uv2y);
                }
            }
            return v.ToArray();
        }

        // nw::eft CONNECTION stripe: link the emitter's birth-ordered live particles (a single chain via Sim rate=1) into one
        // camera-facing triangle-strip ribbon. Width = particle size, U across / V along. Verified vs the captured Guardian beam.
        // (Same vertex interleave as billboards; the shader's uStripe path uses aPos as world pos and aCorner as raw (u,v).)
        float[] BuildStripe(DrawEmitter em, Vector3 camFwd)
        {
            var nodes = new List<Tuple<Vector3,Vector3,float,float>>();   // pos, color, alpha, half-width
            // connection threads through the live particles; each spawns where the emitter was at its birth (cursor path),
            // so a moving emitter spreads them along the swept path (stationary -> EmitterAt=origin -> original behavior).
            foreach (var pt in Emit(em, 1)){
                float t=pt.Age/(float)pt.Life;
                Vector3 pos = EmitterAt(frame - pt.Age) + pt.PosAt(pt.Age);
                float half = Math.Max(Math.Max(em.Radius,0.01f)*Math.Max(em.ScaleAt(t),0f)*0.5f, 0.0001f);
                nodes.Add(Tuple.Create(pos, em.ColorAt(t), em.AlphaAt(t), half));
            }
            int N = nodes.Count; if (N < 2) return new float[0];
            var L = new Vector3[N]; var R = new Vector3[N];
            for (int i=0;i<N;i++){
                Vector3 dir = (i==0) ? nodes[1].Item1-nodes[0].Item1 : (i==N-1) ? nodes[N-1].Item1-nodes[N-2].Item1 : nodes[i+1].Item1-nodes[i-1].Item1;
                if (dir.LengthSquared < 1e-10f) dir = Vector3.UnitX; dir.Normalize();
                Vector3 outer = (em.StripeType==1) ? Vector3.Cross(Vector3.UnitY, dir) : (em.StripeType==2) ? Vector3.UnitY : Vector3.Cross(camFwd, dir);
                if (outer.LengthSquared < 1e-10f) outer = Vector3.UnitY; outer.Normalize();
                L[i] = nodes[i].Item1 + outer*nodes[i].Item4;
                R[i] = nodes[i].Item1 - outer*nodes[i].Item4;
            }
            var v = new List<float>((N-1)*96);   // 6 verts * 16 floats per segment
            for (int i=0;i<N-1;i++){
                float v0=i/(float)(N-1), v1=(i+1)/(float)(N-1);
                var c0=nodes[i].Item2; float a0=nodes[i].Item3; var c1=nodes[i+1].Item2; float a1=nodes[i+1].Item3;
                AddStripeVert(v,L[i],c0,a0,0f,v0);   AddStripeVert(v,R[i],c0,a0,1f,v0);   AddStripeVert(v,L[i+1],c1,a1,0f,v1);
                AddStripeVert(v,L[i+1],c1,a1,0f,v1); AddStripeVert(v,R[i],c0,a0,1f,v0);   AddStripeVert(v,R[i+1],c1,a1,1f,v1);
            }
            return v.ToArray();
        }
        // nw::eft TRAIL stripe (vertexTransformMode 2): a single emitter-attached particle whose worldPos is pushed
        // into a history queue each frame (NW4F CalcStripe: queue[]=ptcl->worldPos). With the cursor-follow emitter,
        // that worldPos history IS the swept cursor path -> the ribbon traces the swing. TrailLen = numSliceHistory.
        // (Cursor stationary -> emPath degenerate -> no sweep, nothing to draw, which is faithful: no motion, no trail.)
        float[] BuildTrail(DrawEmitter em, Vector3 camFwd)
        {
            int H = Math.Min(Math.Max(2, em.TrailLen), emPath.Length - 2);
            var P = new System.Collections.Generic.List<Vector3>(H+1);
            for (int h=0; h<=H; h++){ int fr=frame-h; if(fr<0) break; P.Add(EmitterAt(fr)); }   // head (now) back to now-H
            int n = P.Count; if (n < 2) return new float[0];
            if ((P[0]-P[n-1]).LengthSquared < 1e-8f) return new float[0];                         // emitter hasn't moved -> no trail
            var v = new List<float>();
            float baseW = Math.Max(0.0001f, Math.Max(em.Radius,0.01f)*Math.Max(em.ScaleAt(0f),0f)*0.5f);
            var col = em.ColorAt(0f); float al0 = em.AlphaAt(0f);
            for (int i=0;i<n-1;i++){
                Vector3 dir=P[i]-P[i+1]; if(dir.LengthSquared<1e-10f) continue; dir.Normalize();
                Vector3 outer=Vector3.Cross(camFwd,dir); if(outer.LengthSquared<1e-10f) outer=Vector3.UnitY; outer.Normalize();
                float w0=baseW*(1f-i/(float)n),     w1=baseW*(1f-(i+1)/(float)n);                 // taper width head -> tail
                float a0=al0 *(1f-i/(float)n),      a1=al0 *(1f-(i+1)/(float)n);                  // fade alpha head -> tail
                Vector3 l0=P[i]+outer*w0,   r0=P[i]-outer*w0;
                Vector3 l1=P[i+1]+outer*w1, r1=P[i+1]-outer*w1;
                float u0=i/(float)(n-1), u1=(i+1)/(float)(n-1);
                AddStripeVert(v,l0,col,a0,0f,u0);     AddStripeVert(v,r0,col,a0,1f,u0);     AddStripeVert(v,l1,col,a1,0f,u1);
                AddStripeVert(v,l1,col,a1,0f,u1);     AddStripeVert(v,r0,col,a0,1f,u0);     AddStripeVert(v,r1,col,a1,1f,u1);
            }
            return v.ToArray();
        }
        static void AddStripeVert(List<float> v, Vector3 pos, Vector3 col, float a, float u, float vv){
            // stripe vertex: aPos=world, aCorner=raw (u,v), aUVOff/1/2 unused (the VS sets vUV1=vUV2=vUV for uStripe==1) -> 4 trailing zeros pad to 16 floats
            v.Add(pos.X);v.Add(pos.Y);v.Add(pos.Z); v.Add(col.X);v.Add(col.Y);v.Add(col.Z);v.Add(a); v.Add(0f); v.Add(u);v.Add(vv); v.Add(0f);v.Add(0f); v.Add(0f);v.Add(0f); v.Add(0f);v.Add(0f);
        }

        // nw::eft VELOCITY-STRETCHED billboard (vertexTransformMode 3 Directional_Y / 4 Directional_Polygon): each particle is an
        // independent quad stretched along its OWN velocity into a motion streak (the VS stretches by wldPosDf = the particle
        // velocity). This is what makes rain/sparks render as thin streaks instead of big squares. Built per-particle as a
        // camera-facing quad whose long axis = the screen-projected velocity, length grows with speed, width = a fraction of size.
        // (Reuses the uStripe vertex path: aPos = world corner, aCorner = raw UV.)
        float[] BuildDirectional(DrawEmitter em, Vector3 camFwd, Vector3 cr, Vector3 cu)
        {
            var v=new List<float>();
            foreach (var pt in Emit(em, RATE)){
                float t=pt.Age/(float)pt.Life;
                Vector3 pos=pt.PosAt(pt.Age);
                var col=em.ColorAt(t); float al=em.AlphaAt(t);
                float sz=Math.Max(em.Radius,0.01f)*Math.Max(em.ScaleAt(t),0f);
                Vector3 vel=pt.V0; float speed=vel.Length;
                Vector3 sdir, sperp; float len;
                if (speed>1e-6f){
                    Vector3 vdir=vel*(1f/speed);
                    sdir=vdir - camFwd*Vector3.Dot(vdir,camFwd);                              // project velocity onto the screen plane
                    if (sdir.LengthSquared>1e-8f){
                        sdir.Normalize(); sperp=Vector3.Cross(camFwd,sdir);
                        if (sperp.LengthSquared>1e-8f) sperp.Normalize(); else sperp=cr;
                        len=Math.Min(sz*DIR_MAXSTRETCH, sz + (speed/VEL)*DIR_STREAK_K);        // length grows with uncompressed speed
                    } else { sdir=cu; sperp=cr; len=sz; }                                      // velocity ~ toward camera: no usable streak
                } else { sdir=cu; sperp=cr; len=sz; }                                          // motionless: plain square
                float halfW=Math.Max(0.0001f, sz*DIR_WIDTHFRAC*0.5f);
                Vector3 head=pos, tail=pos - sdir*len;                                         // streak trails BEHIND the particle
                Vector3 hl=head+sperp*halfW, hr=head-sperp*halfW, tl=tail+sperp*halfW, tr=tail-sperp*halfW;
                AddStripeVert(v,hl,col,al,0f,0f); AddStripeVert(v,hr,col,al,1f,0f); AddStripeVert(v,tl,col,al,0f,1f);
                AddStripeVert(v,tl,col,al,0f,1f); AddStripeVert(v,hr,col,al,1f,0f); AddStripeVert(v,tr,col,al,1f,1f);
            }
            return v.ToArray();
        }

        // Build an EmitterInput straight from a PTCL.Emitter (resolved textures via the cross-file pool + primitive mesh).
        // Used by the live in-editor preview so a render can be rebuilt on every parameter edit.
        public static EmitterInput BuildInput(PTCL.Emitter em, string name)
        {
            if (em == null || em.EmitterData == null) return null;
            var inp = new EmitterInput {
                Name = name ?? "",
                Data = em.EmitterData,
                Tex  = em.GetSamplerTexture(0),
                Tex1 = em.GetSamplerTexture(1),
                Tex2 = em.GetSamplerTexture(2),
            };
            if (em.HeadBytes != null && em.HeadBytes.Length >= 0x50){   // emitter file name @ head+0x10 (weather-profile key)
                int n = 0; while (0x10 + n < 0x50 && em.HeadBytes[0x10 + n] != 0) n++;
                inp.EmtrName = System.Text.Encoding.ASCII.GetString(em.HeadBytes, 0x10, n);
            }
            uint h = em.PrimitiveHash;
            if (h != 0 && h != 0xFFFFFFFF)
            {
                PTCL.Primitive prim = null;
                if (em.AvailablePrimitives != null)
                    foreach (var pr in em.AvailablePrimitives) { if (pr.Hash == h && pr.Objects != null) { prim = pr; break; } }
                if (prim == null) prim = PTCL.FindGlobalPrimitive(h);
                if (prim != null && prim.Objects != null)
                {
                    STGenericObject obj = null;
                    foreach (var o in prim.Objects) { obj = o; break; }
                    if (obj != null && obj.vertices != null && obj.vertices.Count > 0 && obj.faces != null && obj.faces.Count >= 3)
                    {
                        var vs = new System.Collections.Generic.List<float>(obj.vertices.Count * 5);
                        var ns = new System.Collections.Generic.List<float>(obj.vertices.Count * 3);
                        foreach (var v in obj.vertices) {
                            vs.Add(v.pos.X); vs.Add(v.pos.Y); vs.Add(v.pos.Z); vs.Add(v.uv0.X); vs.Add(v.uv0.Y);
                            ns.Add(v.nrm.X); ns.Add(v.nrm.Y); ns.Add(v.nrm.Z);
                        }
                        inp.MeshVerts = vs.ToArray();
                        inp.MeshNormals = ns.ToArray();
                        inp.MeshIndices = obj.faces.ToArray();
                    }
                }
            }
            return inp;
        }
    }
}
