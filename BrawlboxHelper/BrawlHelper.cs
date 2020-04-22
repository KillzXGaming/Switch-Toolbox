using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using ResU = Syroot.NintenTools.Bfres;
using System.Windows;
using Syroot.Maths;
using BrawlLib;
using BrawlLib.SSBB.ResourceNodes;
using BrawlLib.Wii.Animations;
using System.IO;

namespace BrawlboxHelper
{
    public class KeyGroupData
    {
        public float Value;
    }

    public class BoneAnimKeyData
    {
        public Dictionary<int, KeyGroupData> XPOS = new Dictionary<int, KeyGroupData>();
        public Dictionary<int, KeyGroupData> YPOS = new Dictionary<int, KeyGroupData>();
        public Dictionary<int, KeyGroupData> ZPOS = new Dictionary<int, KeyGroupData>();

        public Dictionary<int, KeyGroupData> XROT = new Dictionary<int, KeyGroupData>();
        public Dictionary<int, KeyGroupData> YROT = new Dictionary<int, KeyGroupData>();
        public Dictionary<int, KeyGroupData> ZROT = new Dictionary<int, KeyGroupData>();

        public Dictionary<int, KeyGroupData> XSCA = new Dictionary<int, KeyGroupData>();
        public Dictionary<int, KeyGroupData> YSCA = new Dictionary<int, KeyGroupData>();
        public Dictionary<int, KeyGroupData> ZSCA = new Dictionary<int, KeyGroupData>();
    }

    public class FMATConverter
    {
        public static void ExportMaterial(MaterialPrototype Material, String OutputPath)
        {
            MDL0MaterialNode NewMat = new MDL0MaterialNode();

            if (Material.Bake0 != null)
            {
                MDL0MaterialRefNode ShadowBakeReference = MDL0MatRefFromGeneric(Material.Bake0);
                ShadowBakeReference.Coordinates = BrawlLib.Wii.Graphics.TexSourceRow.TexCoord1;
                ShadowBakeReference.Scale = new System.Vector2(Material.st0[0], Material.st0[1]);
                ShadowBakeReference.Translation = new System.Vector2(-(Material.st0[2] / Material.st0[0]), (Material.st0[3] - 1) / Material.st0[1] + 1);
                NewMat.AddChild(ShadowBakeReference);
            }
            if (Material.Albedo != null)
            {
                MDL0MaterialRefNode AlbedoReference = MDL0MatRefFromGeneric(Material.Albedo);
                AlbedoReference.Coordinates = BrawlLib.Wii.Graphics.TexSourceRow.TexCoord0;
                NewMat.AddChild(AlbedoReference);
            }
            if (Material.Emission != null)
            {
                MDL0MaterialRefNode EmmReference = MDL0MatRefFromGeneric(Material.Emission);
                EmmReference.Coordinates = BrawlLib.Wii.Graphics.TexSourceRow.TexCoord0;
                NewMat.AddChild(EmmReference);
            }
            if (Material.Bake1 != null)
            {
                MDL0MaterialRefNode LightBakeReference = MDL0MatRefFromGeneric(Material.Bake1);
                LightBakeReference.Coordinates = BrawlLib.Wii.Graphics.TexSourceRow.TexCoord1;
                LightBakeReference.Scale = new System.Vector2(Material.st1[0], Material.st1[1]);
                LightBakeReference.Translation = new System.Vector2(-(Material.st1[2] / Material.st1[0]), (Material.st1[3] - 1) / Material.st1[1] + 1);
                NewMat.AddChild(LightBakeReference);
            }

            if (Material.IsAlphaTest)
            {
                NewMat.Comp0 = (BrawlLib.Wii.Graphics.AlphaCompare)Material.AlphaTestFunction;
                NewMat.Ref0 = (byte)Material.AlphaTestReference;
            }
            if (Material.IsXLU)
            {
                NewMat.EnableBlend = true;
                NewMat.XLUMaterial = true;
                NewMat.SrcFactor = ((int)Material.SrcFactor <= 7) ? (BrawlLib.Wii.Graphics.BlendFactor)Material.SrcFactor : BrawlLib.Wii.Graphics.BlendFactor.SourceAlpha;
                NewMat.DstFactor = ((int)Material.DestFactor <= 7) ? (BrawlLib.Wii.Graphics.BlendFactor)Material.DestFactor : BrawlLib.Wii.Graphics.BlendFactor.InverseSourceAlpha;
            }

            NewMat.CompareBeforeTexture = !( Material.IsAlphaTest | Material.IsXLU);

            NewMat._lightSetIndex = -1;
            NewMat._fogIndex = 0;
            NewMat.CullMode = BrawlLib.SSBBTypes.CullMode.Cull_Inside;
            NewMat.Name = Material.Name;

            MDL0Node dummyModel = new MDL0Node();
            dummyModel.Version = 11;
            MDL0GroupNode matGroup = new MDL0GroupNode(BrawlLib.Wii.Models.MDLResourceType.Materials);
            dummyModel.AddChild(matGroup, true);
            dummyModel._matGroup = matGroup;
            dummyModel._matGroup.AddChild(NewMat);

            NewMat.Rebuild();
            Export(NewMat, OutputPath);
        }

        private static int[] WrapModeLUT = { 1, 2, 0 };
        private static int[] FilteringLUT = { 3, 0, 1, 4, 2, 5 };

        public static MDL0MaterialRefNode MDL0MatRefFromGeneric(MaterialPrototype.TextureReference Tex)
        {
            MDL0MaterialRefNode Mat = new MDL0MaterialRefNode();
            Mat.Name = Tex.Name;
            Mat.Texture = Tex.Name;
            Mat.UWrapMode = (MatWrapMode)WrapModeLUT[(int)Tex.UWrapMode];
            Mat.VWrapMode = (MatWrapMode)WrapModeLUT[(int)Tex.VWrapMode];

            Mat.MagFilter = (MatTextureMagFilter)(Tex.MagnFilter - 1);
            Mat.MinFilter = (MatTextureMinFilter)(FilteringLUT[(int)Tex.MinFilter]);

            return Mat;
        }

        protected static internal unsafe void PostProcess(MDL0MaterialNode mat, BrawlLib.IO.FileMap map, int dataLen, VoidPtr mdlAddress, VoidPtr dataAddress, BrawlLib.StringTable stringTable)
        {
            BrawlLib.SSBBTypes.MDL0Material* header = (BrawlLib.SSBBTypes.MDL0Material*)dataAddress;
            header->_mdl0Offset = 0;
            header->_stringOffset = (int)((long)stringTable[mat.Name] + 4 - (long)dataAddress);
            header->_index = 0;

            BrawlLib.SSBBTypes.MDL0TextureRef* first = header->First;
            foreach (MDL0MaterialRefNode n in mat.Children)
                PostProcessRef(n, mdlAddress, first++, stringTable, dataLen);
        }

        protected static internal unsafe void PostProcessRef(MDL0MaterialRefNode r, VoidPtr mdlAddress, VoidPtr dataAddress, BrawlLib.StringTable stringTable, int stOffset)
        {
            BrawlLib.SSBBTypes.MDL0TextureRef* header = (BrawlLib.SSBBTypes.MDL0TextureRef*)dataAddress;

            header->_texOffset = r.Name != null ? (int)((long)stringTable[r.Name] + 4 - (long)dataAddress) : 0;
            header->_pltOffset = 0;
            header->_texPtr = 0;
            header->_pltPtr = 0;
            header->_index1 = r.Index;
            header->_index2 = r.Index;
            header->_uWrap = (int)r.UWrapMode;
            header->_vWrap = (int)r.VWrapMode;
            header->_minFltr = (int)r.MinFilter;
            header->_magFltr = (int)r.MagFilter;
            header->_lodBias = (int)r.LODBias;
            header->_maxAniso = (int)r.MaxAnisotropy;
            header->_clampBias = (byte)(r.ClampBias ? 1 : 0);
            header->_texelInterp = (byte)(r.TexelInterpolate ? 1 : 0);
            header->_pad = (short)0;
        }

        public static unsafe void Export(MDL0MaterialNode mat, string outPath)
        {
            BrawlLib.StringTable table = new BrawlLib.StringTable();
            table.Add(mat.Name);
            foreach (MDL0MaterialRefNode r in mat._children)
            {
                table.Add(r.Name);
            }
            int dataLen = mat.CalculateSize(false);
            int totalLen = dataLen + table.GetTotalSize();

            using (FileStream stream = new FileStream(outPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 8, FileOptions.RandomAccess))
            {
                stream.SetLength(totalLen);
                using (BrawlLib.IO.FileMap map = BrawlLib.IO.FileMap.FromStream(stream))
                {
                    mat.Rebuild(map.Address, dataLen, false);
                    table.WriteTable(map.Address + dataLen);
                    PostProcess(mat, map, dataLen, map.Address, map.Address, table);
                    ((BrawlLib.SSBBTypes.MDL0Material*)map.Address)->_pad = (byte)11;
                }
            }
        }
    }

    public class MaterialPrototype
    {
        public String Name;

        public TextureReference Albedo;
        public TextureReference Emission;
        public TextureReference Bake0;
        public TextureReference Bake1;

        public Boolean IsAlphaTest;
        public ResU.GX2.GX2CompareFunction AlphaTestFunction;
        public int AlphaTestReference;

        public Boolean IsXLU;
        public ResU.GX2.GX2BlendFunction SrcFactor;
        public ResU.GX2.GX2BlendFunction DestFactor;

        public CullMode FaceCulling;

        public float[] st0;
        public float[] st1;

        public enum CullMode
        {
            None,
            Back,
            Front,
            All,
        }

        public class TextureReference
        {
            public String Name;
            public WrapMode UWrapMode;
            public WrapMode VWrapMode;
            public FilteringMode MagnFilter;
            public FilteringMode MinFilter;

            public TextureReference(String Name)
            {
                this.Name = Name;
            }

            public enum WrapMode
            {
                Repeat,
                Mirror,
                Clamp
            }

            public enum FilteringMode
            {
                Linear_Mipmap_Nearest,
                Nearest,
                Linear,
                Nearest_Mipmap_Linear,
                Nearest_Mipmap_Nearest,
                Linear_Mipmap_Linear
            }
        }
    }

    public class FSHUConverter
    {
        public static ResU.ShaderParamAnim Clr02Fshu(string FileName)
        {
            CLR0Node clr0 = NodeFactory.FromFile(null, FileName) as CLR0Node;


            ResU.ShaderParamAnim fshu = new ResU.ShaderParamAnim();
            fshu.FrameCount = clr0.FrameCount;
            fshu.Name = clr0.Name;
            fshu.Path = clr0.OriginalPath;
            fshu.UserData = new ResU.ResDict<Syroot.NintenTools.Bfres.UserData>();

            //Set flags
            if (clr0.Loop)
                fshu.Flags |= ResU.ShaderParamAnimFlags.Looping;

            //Set mat anims and then calculate data after
            foreach (var entry in clr0.Children)
            {
                if (entry is CLR0MaterialNode)
                    fshu.ShaderParamMatAnims.Add(Clr0Entry2ShaderMatAnim(clr0, (CLR0MaterialNode)entry));
            }

            fshu.BakedSize = CalculateBakeSize(fshu);
            fshu.BindIndices = SetIndices(fshu);

            return fshu;
        }

        public static ResU.ShaderParamMatAnim Clr0Entry2ShaderMatAnim(CLR0Node clr0, CLR0MaterialNode clrMaterial)
        {
            ResU.ShaderParamMatAnim matAnim = new ResU.ShaderParamMatAnim();
            matAnim.Name = clrMaterial.Name;
            matAnim.Constants = new List<ResU.AnimConstant>();
            matAnim.Curves = new List<ResU.AnimCurve>();
            matAnim.ParamAnimInfos = new List<ResU.ParamAnimInfo>();

            foreach (var entry in clrMaterial.Children)
            {
                ushort curveIndex = 0;
                ushort constantIndex = 0;

                CLR0MaterialEntryNode ParamEntry = (CLR0MaterialEntryNode)entry;

                //Add constants for RGBA if constant
                if (ParamEntry.Constant)
                {
                    //Red
                    matAnim.Constants.Add(new ResU.AnimConstant()
                    {
                        AnimDataOffset = 0,
                        Value = (float)ParamEntry.Colors[0].R / 255f,
                    });

                    //Green
                    matAnim.Constants.Add(new ResU.AnimConstant()
                    {
                        AnimDataOffset = 4,
                        Value = (float)ParamEntry.Colors[0].G / 255f,
                    });

                    //Blue
                    matAnim.Constants.Add(new ResU.AnimConstant()
                    {
                        AnimDataOffset = 8,
                        Value = (float)ParamEntry.Colors[0].B / 255f,
                    });

                    //Alpha
                    matAnim.Constants.Add(new ResU.AnimConstant()
                    {
                        AnimDataOffset = 12,
                        Value = (float)ParamEntry.Colors[0].A / 255f,
                    });
                }

                var RedCurve = GenerateCurve(0, clr0, ParamEntry);
                var GreenCurve = GenerateCurve(4, clr0, ParamEntry);
                var BlueCurve = GenerateCurve(8, clr0, ParamEntry);
                var AlphaCurve = GenerateCurve(12, clr0, ParamEntry);

                if (RedCurve != null)
                    matAnim.Curves.Add(RedCurve);
                if (GreenCurve != null)
                    matAnim.Curves.Add(GreenCurve);
                if (BlueCurve != null)
                    matAnim.Curves.Add(BlueCurve);
                if (AlphaCurve != null)
                    matAnim.Curves.Add(AlphaCurve);

                matAnim.ParamAnimInfos.Add(new ResU.ParamAnimInfo()
                {
                    Name = entry.Name,
                    BeginCurve = matAnim.Curves.Count > 0 ? curveIndex : ushort.MaxValue,
                    FloatCurveCount = (ushort)matAnim.Curves.Count,
                    SubBindIndex = ushort.MaxValue,
                    ConstantCount = (ushort)matAnim.Constants.Count,
                    BeginConstant = matAnim.Constants.Count > 0 ? constantIndex : ushort.MaxValue,
                });

                constantIndex += (ushort)matAnim.Constants.Count;
                curveIndex += (ushort)matAnim.Curves.Count;
            }

            return matAnim;
        }


        private static ResU.AnimCurve GenerateCurve(uint AnimOffset, CLR0Node anim, CLR0MaterialEntryNode entry)
        {
            ResU.AnimCurve curve = new ResU.AnimCurve();
            curve.AnimDataOffset = AnimOffset;
            curve.StartFrame = 0;
            curve.Offset = 0;
            curve.Scale = 1;
            curve.FrameType = ResU.AnimCurveFrameType.Single;
            curve.KeyType = ResU.AnimCurveKeyType.Single;
            curve.CurveType = ResU.AnimCurveType.Linear;

            List<float> Frames = new List<float>();
            List<float> Keys = new List<float>();

            for (int c = 0; c < entry.Colors.Count; c++)
            {
                Frames.Add(c);
                //Max of 4 values.  Cubic using 4, linear using 2, and step using 1
                float[] KeyValues = new float[4];

                switch (AnimOffset)
                {
                    case 0: //Red
                        Keys.Add((float)entry.Colors[c].R / 255f);
                        break;
                    case 4: //Green
                        Keys.Add((float)entry.Colors[c].G / 255f);
                        break;
                    case 8: //Blue
                        Keys.Add((float)entry.Colors[c].B / 255f);
                        break;
                    case 12: //Alpha
                        Keys.Add((float)entry.Colors[c].A / 255f);
                        break;
                    default:
                        throw new Exception("Invalid animation offset set!");
                }
            }

            //Max value in frames is our end frame
            curve.EndFrame = Frames.Max();
            curve.Frames = Frames.ToArray();

            //If a curve only has one frame we don't need to interpolate or add keys to a curve as it's constant
            if (curve.Frames.Length <= 1)
                return null;

            switch (curve.CurveType)
            {
                case ResU.AnimCurveType.Cubic:
                    curve.Keys = new float[Keys.Count, 4];
                    for (int frame = 0; frame < Keys.Count; frame++)
                    {
                        float Delta = 0;

                        if (frame < Keys.Count - 1)
                            Delta = Keys[frame + 1] - Keys[frame];

                        float value = Keys[frame];
                        float Slope = 0;
                        float Slope2 = 0;

                        curve.Keys[frame, 0] = value;
                        curve.Keys[frame, 1] = Slope;
                        curve.Keys[frame, 2] = Slope2;
                        curve.Keys[frame, 3] = Delta;
                    }
                    break;
                case ResU.AnimCurveType.StepInt:
                    //Step requires no interpolation
                    curve.Keys = new float[Keys.Count, 1];
                    for (int frame = 0; frame < Keys.Count; frame++)
                    {
                        curve.Keys[frame, 0] = 0;
                    }
                    break;
                case ResU.AnimCurveType.Linear:
                    curve.Keys = new float[Keys.Count, 2];
                    for (int frame = 0; frame < Keys.Count; frame++)
                    {
                        //Delta for second value used in linear curves
                        float time = curve.Frames[frame];
                        float Delta = 0;

                        if (frame < Keys.Count - 1)
                            Delta = Keys[frame + 1] - Keys[frame];

                        curve.Keys[frame, 0] = Keys[frame];
                        curve.Keys[frame, 1] = Delta;
                    }
                    break;
            }

            return curve;
        }

        private static ushort[] SetIndices(ResU.ShaderParamAnim fshu)
        {
            List<ushort> indces = new List<ushort>();
            foreach (var matAnim in fshu.ShaderParamMatAnims)
                indces.Add(65535);

            return indces.ToArray();
        }
        private static uint CalculateBakeSize(ResU.ShaderParamAnim fshu)
        {
            return 0;
        }
    }

    public class FTXPConverter
    {
        public static void Pat02Ftxp(MaterialAnim matAnim, string FileName)
        {
            PAT0Node pat0 = new PAT0Node();
            matAnim.FrameCount = pat0.FrameCount;
            matAnim.Name = pat0.Name;
            matAnim.Path = pat0.OriginalPath;
            matAnim.Loop = pat0.Loop;

            foreach (var entry in pat0.Children)
            {
                var Material = (PAT0EntryNode)entry;
             
            }
        }
    }

    public class FSKAConverter
    {
        public static List<BoneAnimKeyData> BoneAnimKeys = new List<BoneAnimKeyData>();

        static float Deg2Rad = (float)(Math.PI / 180f);
        static float Rad2Deg = (float)(180f / Math.PI);

        public static SkeletalAnim Anim2Fska(string FileName)
        {
            CHR0Node chr0 = AnimFormat.Read(FileName);
            return Chr02Fska(chr0);
        }

        public static void Fska2Chr0(SkeletalAnim fska, string FileName)
        {
            Console.WriteLine("Making CHR0Node");
            CHR0Node chr0 = new CHR0Node();
            Console.WriteLine("Created CHR0Node");

            chr0.FrameCount = fska.FrameCount;
            chr0.Name = fska.Name;
            chr0.OriginalPath = fska.Path;
            chr0.UserEntries = new UserDataCollection();
            chr0.Loop = fska.Loop;

            foreach (var entry in fska.BoneAnims)
                BoneAnim2Chr0Entry(entry, chr0);

            Console.WriteLine("Exporting CHR0Node");

            chr0.Export(FileName);
        }

        public class FSKAKeyNode
        {
            public float Value = 0;
            public float Slope = 0;
            public float Slope2 = 0;
            public float Delta = 0;
            public float Frame = 0;
        }

        public static void BoneAnim2Chr0Entry(BoneAnim boneAnim, CHR0Node chr0)
        {
            CHR0EntryNode chr0Entry = chr0.CreateEntry(boneAnim.Name);
            chr0Entry.UseModelRotate = false;
            chr0Entry.UseModelScale = false;
            chr0Entry.UseModelTranslate = false;
            chr0Entry.ScaleCompensateApply = boneAnim.ApplySegmentScaleCompensate;

            //Float for time/frame
            Dictionary<float, FSKAKeyNode> TranslateX = new Dictionary<float, FSKAKeyNode>();
            Dictionary<float, FSKAKeyNode> TranslateY = new Dictionary<float, FSKAKeyNode>();
            Dictionary<float, FSKAKeyNode> TranslateZ = new Dictionary<float, FSKAKeyNode>();
            Dictionary<float, FSKAKeyNode> RotateX = new Dictionary<float, FSKAKeyNode>();
            Dictionary<float, FSKAKeyNode> RotateY = new Dictionary<float, FSKAKeyNode>();
            Dictionary<float, FSKAKeyNode> RotateZ = new Dictionary<float, FSKAKeyNode>();
            Dictionary<float, FSKAKeyNode> ScaleX = new Dictionary<float, FSKAKeyNode>();
            Dictionary<float, FSKAKeyNode> ScaleY = new Dictionary<float, FSKAKeyNode>();
            Dictionary<float, FSKAKeyNode> ScaleZ = new Dictionary<float, FSKAKeyNode>();

            if (boneAnim.FlagsBase.HasFlag(BoneAnimFlagsBase.Translate))
            {
                TranslateX.Add(0, new FSKAKeyNode() { Value = boneAnim.BaseData.Translate.X, });
                TranslateY.Add(0, new FSKAKeyNode() { Value = boneAnim.BaseData.Translate.Y, });
                TranslateZ.Add(0, new FSKAKeyNode() { Value = boneAnim.BaseData.Translate.Z, });
            }
            if (boneAnim.FlagsBase.HasFlag(BoneAnimFlagsBase.Rotate))
            {
                RotateX.Add(0, new FSKAKeyNode() { Value = boneAnim.BaseData.Rotate.X, });
                RotateY.Add(0, new FSKAKeyNode() { Value = boneAnim.BaseData.Rotate.Y, });
                RotateZ.Add(0, new FSKAKeyNode() { Value = boneAnim.BaseData.Rotate.Z, });
            }

            if (boneAnim.FlagsBase.HasFlag(BoneAnimFlagsBase.Scale))
            {
                ScaleX.Add(0, new FSKAKeyNode() { Value = boneAnim.BaseData.Scale.X, });
                ScaleY.Add(0, new FSKAKeyNode() { Value = boneAnim.BaseData.Scale.Y, });
                ScaleZ.Add(0, new FSKAKeyNode() { Value = boneAnim.BaseData.Scale.Z, });
            }
            else
            {
                ScaleX.Add(0, new FSKAKeyNode() { Value = 1 });
                ScaleY.Add(0, new FSKAKeyNode() { Value = 1 });
                ScaleZ.Add(0, new FSKAKeyNode() { Value = 1 });
            }

            foreach (var curve in boneAnim.Curves)
            {
                for (int frame = 0; frame < curve.Frames.Length; frame++)
                {
                    float time = curve.Frames[frame];
                    float value = 0;
                    float slope = 0;
                    float slope2 = 0;
                    float delta = 0;

                    float scale = curve.Scale;
                    if (scale <= 0)
                        scale = 1;

                    if (curve.CurveType == AnimCurveType.Cubic)
                    {
                        value = curve.Offset + curve.Keys[frame, 0] * scale;
                        slope = curve.Offset + curve.Keys[frame, 1] * scale;
                        slope2 = curve.Offset + curve.Keys[frame, 2] * scale;
                        delta = curve.Offset + curve.Keys[frame, 3] * scale;
                    }
                    if (curve.CurveType == AnimCurveType.Linear)
                    {
                        value = curve.Offset + curve.Keys[frame, 0] * scale;
                        delta = curve.Offset + curve.Keys[frame, 1] * scale;
                    }
                    if (curve.CurveType == AnimCurveType.StepInt)
                    {
                        value = curve.Offset + curve.Keys[frame, 0] * scale;
                    }


                    switch (curve.AnimDataOffset)
                    {
                        case 0x10:
                            if (TranslateX.Count > 0 && frame == 0)
                                TranslateX.Remove(0);

                            TranslateX.Add(time, new FSKAKeyNode()
                            {
                                Value = value,
                                Slope = slope,
                                Slope2 = slope2,
                                Delta = delta,
                                Frame = time,
                            });
                            break;
                        case 0x14:
                            if (TranslateY.Count > 0 && frame == 0)
                                TranslateY.Remove(0);

                            TranslateY.Add(time, new FSKAKeyNode()
                            {
                                Value = value,
                                Slope = slope,
                                Slope2 = slope2,
                                Delta = delta,
                                Frame = time,
                            });
                            break;
                        case 0x18:
                            if (TranslateZ.Count > 0 && frame == 0)
                                TranslateZ.Remove(0);

                            TranslateZ.Add(time, new FSKAKeyNode()
                            {
                                Value = value,
                                Slope = slope,
                                Slope2 = slope2,
                                Delta = delta,
                                Frame = time,
                            });
                            break;
                        case 0x20:
                            if (RotateX.Count > 0 && frame == 0)
                                RotateX.Remove(0);

                            RotateX.Add(time, new FSKAKeyNode()
                            {
                                Value = value,
                                Slope = slope,
                                Slope2 = slope2,
                                Delta = delta,
                                Frame = time,
                            });
                            break;
                        case 0x24:
                            if (RotateY.Count > 0 && frame == 0)
                                RotateY.Remove(0);

                            RotateY.Add(time, new FSKAKeyNode()
                            {
                                Value = value,
                                Slope = slope,
                                Slope2 = slope2,
                                Delta = delta,
                                Frame = time,
                            });
                            break;
                        case 0x28:
                            if (RotateZ.Count > 0 && frame == 0)
                                RotateZ.Remove(0);

                            RotateZ.Add(time, new FSKAKeyNode()
                            {
                                Value = value,
                                Slope = slope,
                                Slope2 = slope2,
                                Delta = delta,
                                Frame = time,
                            });
                            break;
                        case 0x04:
                            if (ScaleX.Count > 0 && frame == 0)
                                ScaleX.Remove(0);

                            ScaleX.Add(time, new FSKAKeyNode()
                            {
                                Value = value,
                                Slope = slope,
                                Slope2 = slope2,
                                Delta = delta,
                                Frame = time,
                            });
                            break;
                        case 0x08:
                            if (ScaleY.Count > 0 && frame == 0)
                                ScaleY.Remove(0);

                            ScaleY.Add(time, new FSKAKeyNode()
                            {
                                Value = value,
                                Slope = slope,
                                Slope2 = slope2,
                                Delta = delta,
                                Frame = time,
                            });
                            break;
                        case 0x0C:
                            if (ScaleZ.Count > 0 && frame == 0)
                                ScaleZ.Remove(0);

                            ScaleZ.Add(time, new FSKAKeyNode()
                            {
                                Value = value,
                                Slope = slope,
                                Slope2 = slope2,
                                Delta = delta,
                                Frame = time,
                            });
                            break;
                    }
                }
            }

            for (int frame = 0; frame < chr0.FrameCount; frame++)
            {
                if (TranslateX.ContainsKey(frame))
                    chr0Entry.SetKeyframe(6, frame, TranslateX[frame].Value);
                if (TranslateY.ContainsKey(frame))
                    chr0Entry.SetKeyframe(7, frame, TranslateY[frame].Value);
                if (TranslateZ.ContainsKey(frame))
                    chr0Entry.SetKeyframe(8, frame, TranslateZ[frame].Value);
                if (RotateX.ContainsKey(frame))
                    chr0Entry.SetKeyframe(3, frame, RotateX[frame].Value * Rad2Deg);
                if (RotateY.ContainsKey(frame))
                    chr0Entry.SetKeyframe(4, frame, RotateY[frame].Value * Rad2Deg);
                if (RotateZ.ContainsKey(frame))
                    chr0Entry.SetKeyframe(5, frame, RotateZ[frame].Value * Rad2Deg);
                if (ScaleX.ContainsKey(frame))
                    chr0Entry.SetKeyframe(0, frame, ScaleX[frame].Value);
                if (ScaleY.ContainsKey(frame))
                    chr0Entry.SetKeyframe(1, frame, ScaleY[frame].Value);
                if (ScaleZ.ContainsKey(frame))
                    chr0Entry.SetKeyframe(2, frame, ScaleZ[frame].Value);
            }
        }

        public static SkeletalAnim Chr02Fska(string FileName)
        {
            CHR0Node chr0 = CHR0Node.FromFile(FileName);
            return Chr02Fska(chr0);
        }

        private static SkeletalAnim Chr02Fska(CHR0Node chr0)
        {
            SkeletalAnim fska = new SkeletalAnim();
            fska.FrameCount = chr0.FrameCount;
            fska.Name = chr0.Name;
            fska.Path = chr0.OriginalPath;
            fska.UserDatas = new List<Syroot.NintenTools.NSW.Bfres.UserData>();
            fska.UserDataDict = new ResDict();

            //Set flags
            if (chr0.Loop)
                fska.FlagsAnimSettings |= SkeletalAnimFlags.Looping;
            fska.FlagsRotate = SkeletalAnimFlagsRotate.EulerXYZ;
            fska.FlagsScale = SkeletalAnimFlagsScale.Maya;

            //Set bone anims and then calculate data after
            foreach (var entry in chr0.Children)
                fska.BoneAnims.Add(Chr0Entry2BoneAnim((CHR0EntryNode)entry));

            fska.BakedSize = CalculateBakeSize(fska);
            fska.BindIndices = SetIndices(fska);

            return fska;
        }

        private static string NameConverterMkWii2Mk8(string name)
        {
            switch (name)
            {
                case "skl_root": return "Skl_Root";
                case "leg_l1": return "LegL";
                case "leg_l2": return "KneeL";
                case "ankle_l1": return "FootL";
                case "leg_r1": return "LegR";
                case "leg_r2": return "KneeR";
                case "ankle_r1": return "FootR";
                case "spin": return "Spine1";
                case "arm_l1": return "ShoulderL";
                case "arm_l2": return "ArmL";
                case "wrist_l1": return "HandL";
                case "arm_r1": return "ShoulderR";
                case "arm_r2": return "ArmR";
                case "wrist_r1": return "HandR";
                case "face_1": return "Head";
            }

            return name;
        }

        private static BoneAnim Chr0Entry2BoneAnim(CHR0EntryNode entry)
        {
            BoneAnim boneAnim = new BoneAnim();
            boneAnim.Name = entry.Name;
         //   boneAnim.Name = NameConverterMkWii2Mk8(boneAnim.Name);

            if (entry.UseModelTranslate)
                boneAnim.FlagsBase |= BoneAnimFlagsBase.Translate;
            if (entry.UseModelRotate)
                boneAnim.FlagsBase |= BoneAnimFlagsBase.Rotate;
            if (entry.UseModelScale)
                boneAnim.FlagsBase |= BoneAnimFlagsBase.Scale;

            var baseData = new BoneAnimData();

            var FirstFrame = entry.GetAnimFrame(0);
            if (FirstFrame.HasKeys)
            {
                float xRadian = FirstFrame.Rotation._x * Deg2Rad;
                float yRadian = FirstFrame.Rotation._y * Deg2Rad;
                float zRadian = FirstFrame.Rotation._z * Deg2Rad;

                baseData.Translate = new Vector3F(FirstFrame.Translation._x, FirstFrame.Translation._y, FirstFrame.Translation._z);
                baseData.Rotate = new Vector4F(xRadian, yRadian, zRadian, 1);
                baseData.Scale = new Vector3F(FirstFrame.Scale._x, FirstFrame.Scale._y, FirstFrame.Scale._z);
                baseData.Flags = 0;
                boneAnim.BaseData = baseData;
            }
            else
            {
                baseData.Translate = new Vector3F(0,0,0);
                baseData.Rotate = new Vector4F(0, 0, 0, 1);
                baseData.Scale = new Vector3F(1, 1, 1);
                baseData.Flags = 0;
                boneAnim.BaseData = baseData;
            }


            boneAnim.FlagsBase |= BoneAnimFlagsBase.Translate;
            boneAnim.FlagsBase |= BoneAnimFlagsBase.Scale;
            boneAnim.FlagsBase |= BoneAnimFlagsBase.Rotate;

            if (baseData.Rotate == new Vector4F(0, 0, 0, 1))
                boneAnim.FlagsTransform |= BoneAnimFlagsTransform.RotateZero;
            if (baseData.Translate == new Vector3F(0, 0, 0))
                boneAnim.FlagsTransform |= BoneAnimFlagsTransform.TranslateZero;
            if (baseData.Scale == new Vector3F(1, 1, 1))
                boneAnim.FlagsTransform |= BoneAnimFlagsTransform.ScaleOne;
            if (IsUniform(baseData.Scale))
                boneAnim.FlagsTransform |= BoneAnimFlagsTransform.ScaleUniform;
            if (!IsRoot(boneAnim))
                boneAnim.FlagsTransform |= BoneAnimFlagsTransform.SegmentScaleCompensate;

            boneAnim.BeginTranslate = 6;
            boneAnim.BeginRotate = 3;

            if (FirstFrame.HasKeys)
            {
                var AnimFrame = entry.GetAnimFrame(0);
                if (AnimFrame.hasTx)
                {
                    boneAnim.FlagsCurve |= BoneAnimFlagsCurve.TranslateX;
                    var curve = GenerateCurve(0x10, entry);
                    if (curve != null)
                        boneAnim.Curves.Add(curve);
                }
                if (AnimFrame.hasTy)
                {
                    boneAnim.FlagsCurve |= BoneAnimFlagsCurve.TranslateY;
                    var curve = GenerateCurve(0x14, entry);
                    if (curve != null)
                        boneAnim.Curves.Add(curve);
                }
                if (AnimFrame.hasTz)
                {
                    boneAnim.FlagsCurve |= BoneAnimFlagsCurve.TranslateZ;
                    var curve = GenerateCurve(0x18, entry);
                    if (curve != null)
                        boneAnim.Curves.Add(curve);
                }
                if (AnimFrame.hasRx)
                {
                    boneAnim.FlagsCurve |= BoneAnimFlagsCurve.RotateX;
                    var curve = GenerateCurve(0x20, entry);
                    if (curve != null)
                        boneAnim.Curves.Add(curve);
                }
                if (AnimFrame.hasRy)
                {
                    boneAnim.FlagsCurve |= BoneAnimFlagsCurve.RotateY;
                    var curve = GenerateCurve(0x24, entry);
                    if (curve != null)
                        boneAnim.Curves.Add(curve);
                }
                if (AnimFrame.hasRz)
                {
                    boneAnim.FlagsCurve |= BoneAnimFlagsCurve.RotateZ;
                    var curve = GenerateCurve(0x28, entry);
                    if (curve != null)
                        boneAnim.Curves.Add(curve);
                }
                if (AnimFrame.hasSx)
                {
                    boneAnim.FlagsCurve |= BoneAnimFlagsCurve.ScaleX;
                    var curve = GenerateCurve(0x4, entry);
                    if (curve != null)
                        boneAnim.Curves.Add(curve);
                }
                if (AnimFrame.hasSy)
                {
                    boneAnim.FlagsCurve |= BoneAnimFlagsCurve.ScaleY;
                    var curve = GenerateCurve(0x8, entry);
                    if (curve != null)
                        boneAnim.Curves.Add(curve);
                }
                if (AnimFrame.hasSz)
                {
                    boneAnim.FlagsCurve |= BoneAnimFlagsCurve.ScaleZ;
                    var curve = GenerateCurve(0xC, entry);
                    if (curve != null)
                        boneAnim.Curves.Add(curve);
                }
            }

            return boneAnim;
        }

        private static bool IsRoot(BoneAnim boneAnim)
        {
            if (boneAnim.Name.Contains("root") || boneAnim.Name.Contains("center"))
                return true;

            return false;
        }

        private static bool IsUniform(Vector3F value)
        {
            return value.X == value.Y && value.X == value.Z;
        }

        private static void QuantizeCurveData(AnimCurve curve)
        {
            float MaxFrame = 0;
            float MaxValues = 0;

            List<bool> IntegerValues = new List<bool>();
            for (int frame = 0; frame < curve.Frames.Length; frame++)
            {
                MaxFrame = Math.Max(MaxFrame, curve.Frames[frame]);

                if (curve.CurveType == AnimCurveType.Linear)
                {
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 0]);
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 1]);

                    IntegerValues.Add(IsInt(curve.Keys[frame, 0]));
                    IntegerValues.Add(IsInt(curve.Keys[frame, 1]));
                }
                else if (curve.CurveType == AnimCurveType.Cubic)
                {
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 0]);
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 1]);
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 2]);
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 3]);

                    IntegerValues.Add(IsInt(curve.Keys[frame, 0]));
                    IntegerValues.Add(IsInt(curve.Keys[frame, 1]));
                    IntegerValues.Add(IsInt(curve.Keys[frame, 2]));
                    IntegerValues.Add(IsInt(curve.Keys[frame, 3]));
                }
                else
                {
                    MaxValues = Math.Max(MaxValues, curve.Keys[frame, 0]);

                    IntegerValues.Add(IsInt(curve.Keys[frame, 0]));
                }

                int ConvertedInt = Convert.ToInt32(MaxValues);

            }

            if (MaxFrame < Byte.MaxValue)
                curve.FrameType = AnimCurveFrameType.Byte;
            else if (MaxFrame < Int16.MaxValue)
                curve.FrameType = AnimCurveFrameType.Decimal10x5;
            else
                curve.FrameType = AnimCurveFrameType.Single;


            if (IntegerValues.Any(x => x == false))
            {
                curve.KeyType = AnimCurveKeyType.Single;
            }
            else
            {
                if (MaxValues < Byte.MaxValue)
                    curve.KeyType = AnimCurveKeyType.SByte;
                else if (MaxFrame < Int16.MaxValue)
                    curve.KeyType = AnimCurveKeyType.Int16;
                else
                    curve.KeyType = AnimCurveKeyType.Single;
            }
        }

        private static bool IsInt(float value) => value == Math.Truncate(value);

        private static AnimCurve GenerateCurve(uint AnimOffset, CHR0EntryNode entry)
        {
            AnimCurve curve = new AnimCurve();
            curve.AnimDataOffset = AnimOffset;
            curve.StartFrame = 0;
            curve.Offset = 0;
            curve.Scale = 1;
            curve.FrameType = AnimCurveFrameType.Single;
            curve.KeyType = AnimCurveKeyType.Single;
            curve.CurveType = AnimCurveType.Linear;

            List<float> Frames = new List<float>();
            List<float> Keys = new List<float>();

            CHRAnimationFrame ain, aout;
            for (int frame = 0; frame < entry.FrameCount; frame++)
            {
                //Max of 4 values.  Cubic using 4, linear using 2, and step using 1
                float[] KeyValues = new float[4];

                //Set the main values to the curve based on offset for encoding later
                if ((ain = entry.GetAnimFrame(frame)).HasKeys)
                {
                    aout = entry.GetAnimFrame(frame, true);
                    FillKeyList(aout, frame, curve.AnimDataOffset, Frames, Keys);

                    if (!ain.Equals(aout))
                    {
                        FillKeyList(aout, frame, curve.AnimDataOffset, Frames, Keys);
                    }
                }
            }

            if (Frames.Count <= 0)
                return null;

         //   Console.WriteLine($"AnimOffset {AnimOffset} Keys {Keys.Count}");

            //Max value in frames is our end frame
            curve.EndFrame = Frames.Max();
            curve.Frames = Frames.ToArray();

            //If a curve only has one frame we don't need to interpolate or add keys to a curve as it's constant
            if (curve.Frames.Length <= 1)
                return null;

            switch (curve.CurveType)
            {
                case AnimCurveType.Cubic:
                    curve.Keys = new float[Keys.Count, 4];
                    for (int frame = 0; frame < Keys.Count; frame++)
                    {
                        float Delta = 0;

                        if (frame < Keys.Count - 1)
                            Delta = Keys[frame + 1] - Keys[frame];

                        float value = Keys[frame];
                        float Slope = 0;
                        float Slope2 = 0;

                        curve.Keys[frame, 0] = value;
                        curve.Keys[frame, 1] = Slope;
                        curve.Keys[frame, 2] = Slope2;
                        curve.Keys[frame, 3] = Delta;
                    }
                    break;
                case AnimCurveType.StepInt:
                    //Step requires no interpolation
                    curve.Keys = new float[Keys.Count, 1];
                    for (int frame = 0; frame < Keys.Count; frame++)
                    {
                        curve.Keys[frame, 0] = Keys[frame];
                    }
                    break;
                case AnimCurveType.Linear:
                    curve.Keys = new float[Keys.Count, 2];
                    for (int frame = 0; frame < Keys.Count; frame++)
                    {
                        //Delta for second value used in linear curves
                        float time = curve.Frames[frame];
                        float Delta = 0;

                        if (frame < Keys.Count - 1)
                            Delta = Keys[frame + 1] - Keys[frame];

                        curve.Keys[frame, 0] = Keys[frame];
                        curve.Keys[frame, 1] = Delta;
                    }
                    break;
            }

            QuantizeCurveData(curve);

            return curve;
        }

        private static void FillKeyList(CHRAnimationFrame AnimFrame, int frame, uint AnimOffset, List<float> Frames, List<float> Keys)
        {
            if (AnimFrame.hasTx && AnimOffset == 0x10)
            {
                Frames.Add(frame);
                Keys.Add(AnimFrame.Translation._x);
            }
            if (AnimFrame.hasTy && AnimOffset == 0x14)
            {
                Frames.Add(frame);
                Keys.Add(AnimFrame.Translation._y);
            }
            if (AnimFrame.hasTz && AnimOffset == 0x18)
            {
                Frames.Add(frame);
                Keys.Add(AnimFrame.Translation._z);
            }
            if (AnimFrame.hasRx && AnimOffset == 0x20)
            {
                Frames.Add(frame);
                Keys.Add(AnimFrame.Rotation._x * Deg2Rad);
            }
            if (AnimFrame.hasRy && AnimOffset == 0x24)
            {
                Frames.Add(frame);
                Keys.Add(AnimFrame.Rotation._y * Deg2Rad);
            }
            if (AnimFrame.hasRz && AnimOffset == 0x28)
            {
                Frames.Add(frame);
                Keys.Add(AnimFrame.Rotation._z * Deg2Rad);
            }
            if (AnimFrame.hasSx && AnimOffset == 0x04)
            {
                Frames.Add(frame);
                Keys.Add(AnimFrame.Scale._x);
            }
            if (AnimFrame.hasSy && AnimOffset == 0x08)
            {
                Frames.Add(frame);
                Keys.Add(AnimFrame.Scale._y);
            }
            if (AnimFrame.hasSz && AnimOffset == 0x0C)
            {
                Frames.Add(frame);
                Keys.Add(AnimFrame.Scale._z);
            }
        }

        private static ushort[] SetIndices(SkeletalAnim fska)
        {
            List<ushort> indces = new List<ushort>();
            foreach (var boneAnim in fska.BoneAnims)
                indces.Add(65535);

            return indces.ToArray();
        }
        private static uint CalculateBakeSize(SkeletalAnim fska)
        {
            return 0;
        }
    }
}
