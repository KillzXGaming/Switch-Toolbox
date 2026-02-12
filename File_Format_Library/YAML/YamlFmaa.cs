using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Bfres.Structs;
using SharpYaml;
using SharpYaml.Events;
using SharpYaml.Serialization;
using SharpYaml.Serialization.Serializers;
using Syroot.NintenTools.NSW.Bfres;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FirstPlugin
{
    public class YamlFmaa
    {
        public class AnimConfig
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public bool Loop { get; set; }
            public int FrameCount { get; set; }
            public string AnimationType { get; set; }
            public List<string> TextureNames { get; set; }

            public uint BakedSize { get; set; }
            public List<ushort> BindIndices { get; set; }

            public List<MatAnimConfig> MaterialAnimConfigs { get; set; }
            public List<UserDataConfig> UserData { get; set; }

            public void ToYaml(MaterialAnim materialAnim, Toolbox.Library.Animations.MaterialAnimation.AnimationType animType)
            {
                MaterialAnimConfigs = new List<MatAnimConfig>();
                UserData = new List<UserDataConfig>();
                if (materialAnim.TextureNames != null)
                    TextureNames = materialAnim.TextureNames.ToList();

                Name = materialAnim.Name;
                Path = materialAnim.Path;
                FrameCount = materialAnim.FrameCount;
                Loop = materialAnim.Loop;
                AnimationType = animType.ToString();
                BakedSize = materialAnim.BakedSize;
                if (materialAnim.BindIndices != null)
                    BindIndices = materialAnim.BindIndices.ToList();

                if (materialAnim.UserData != null)
                {
                    foreach (var ud in materialAnim.UserData)
                    {
                        UserData.Add(new UserDataConfig()
                        {
                            Name = ud.Name,
                            Type = ud.Type.ToString(),
                            Values = ud.GetValueStringArray().ToList()
                        });
                    }
                }

                foreach (var mat in materialAnim.MaterialAnimDataList)
                {
                    MatAnimConfig matConfig = new MatAnimConfig();
                    matConfig.Name = mat.Name;
                    MaterialAnimConfigs.Add(matConfig);

                    foreach (var paramInfo in mat.ParamAnimInfos)
                    {
                        ParamInfo paramCfg = new ParamInfo();
                        paramCfg.Name = paramInfo.Name;
                        paramCfg.IsConstant = paramInfo.ConstantCount != 0;
                        paramCfg.IntCurveCount = paramInfo.IntCurveCount;
                        paramCfg.FloatCurveCount = paramInfo.FloatCurveCount;
                        matConfig.ParamInfos.Add(paramCfg);

                        // Guess type for naming
                        bool isTexSrt = paramInfo.Name.Contains("st0") || paramInfo.Name.Contains("st1") || paramInfo.Name.Contains("uvw_") || paramInfo.Name.Contains("Srt");
                        bool isSrt2D = !isTexSrt && (paramInfo.Name.Contains("srv_") || paramInfo.Name.Contains("st_") || paramInfo.Name.Contains("Layout") || paramInfo.Name.Contains("Pos"));
                        paramCfg.PropertyType = isTexSrt ? "TexSrt" : (isSrt2D ? "Srt2D" : "Generic");

                        if (paramInfo.BeginConstant != ushort.MaxValue && paramInfo.ConstantCount != 0)
                        {
                            paramCfg.Constants = new List<ConstantConfig>();
                            for (int i = 0; i < paramInfo.ConstantCount; i++)
                            {
                                AnimConstant constant = mat.Constants[paramInfo.BeginConstant + i];
                                ConstantConfig ConstantValue = new ConstantConfig();
                                ConstantValue.Offset = ConvertParamToName(constant.AnimDataOffset, paramCfg.PropertyType);
                                ConstantValue.Value = constant.Value;

                                paramCfg.Constants.Add(ConstantValue);
                            }
                        }

                        if (paramInfo.BeginCurve != ushort.MaxValue)
                        {
                            paramCfg.CurveData = new List<CurveConfig>();
                            for (int i = 0; i < paramInfo.IntCurveCount + paramInfo.FloatCurveCount; i++)
                            {
                                var curve = mat.Curves[(int)paramInfo.BeginCurve + i];
                                var CurveCfg = new CurveConfig();
                                CurveCfg.Offset = ConvertParamToName(curve.AnimDataOffset, paramCfg.PropertyType);
                                CurveCfg.CurveType = curve.CurveType;
                                CurveCfg.KeyType = curve.KeyType;
                                CurveCfg.FrameType = curve.FrameType;
                                CurveCfg.Scale = curve.Scale;
                                CurveCfg.ValueOffset = curve.Offset;
                                CurveCfg.Delta = curve.Delta;

                                if (curve.Frames != null)
                                {
                                    for (int f = 0; f < curve.Frames.Length; f++)
                                    {
                                        int frame = (int)curve.Frames[f];
                                        List<float> values = new List<float>();
                                        for (int k = 0; k < curve.Keys.GetLength(1); k++)
                                            values.Add(curve.Keys[f, k]); // Raw keys!

                                        CurveCfg.KeyFrames.Add(new KeyFrameConfig() { Frame = frame, Values = values });
                                    }
                                }

                                paramCfg.CurveData.Add(CurveCfg);
                            }
                        }
                    }

                    foreach (var patternInfo in mat.TexturePatternAnimInfos)
                    {
                        PatternInfo infoCfg = new PatternInfo();
                        infoCfg.Name = patternInfo.Name;
                        infoCfg.IsConstant = patternInfo.BeginConstant != ushort.MaxValue;
                        matConfig.TexturePatternInfos.Add(infoCfg);

                        if (infoCfg.IsConstant)
                        {
                            infoCfg.ConstantValue = new ConstantTPConfig();
                            int Index = (int)mat.Constants[(int)patternInfo.BeginConstant].Value;
                            if (materialAnim.TextureNames != null && Index >= 0 && Index < materialAnim.TextureNames.Count)
                                infoCfg.ConstantValue.Texture = materialAnim.TextureNames[Index];
                        }
                        if (patternInfo.CurveIndex != ushort.MaxValue)
                        {
                            var curve = mat.Curves[(int)patternInfo.CurveIndex];
                            if (curve != null)
                            {
                                infoCfg.CurveData = new CurveTPConfig();
                                if (curve.Frames != null)
                                {
                                    for (int f = 0; f < curve.Frames.Length; f++)
                                    {
                                        int frame = (int)curve.Frames[f];
                                        int Value = (int)(curve.Offset + curve.Keys[f, 0] * curve.Scale);
                                        if (materialAnim.TextureNames != null && Value >= 0 && Value < materialAnim.TextureNames.Count)
                                            infoCfg.CurveData.KeyFrames.Add(frame, materialAnim.TextureNames[Value]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            private string ConvertParamToName(uint offset, string propType)
            {
                string name = $"Offset_{offset}";
                if (propType == "TexSrt")
                {
                    switch (offset)
                    {
                        case 0: name = "Mode"; break;
                        case 4: name = "Scale X"; break;
                        case 8: name = "Scale Y"; break;
                        case 12: name = "Rotation"; break;
                        case 16: name = "Translate X"; break;
                        case 20: name = "Translate Y"; break;
                    }
                }
                else if (propType == "Srt2D")
                {
                    switch (offset)
                    {
                        case 0: name = "Scale X"; break;
                        case 4: name = "Scale Y"; break;
                        case 8: name = "Rotation"; break;
                        case 12: name = "Translate X"; break;
                        case 16: name = "Translate Y"; break;
                    }
                }
                else
                {
                    switch (offset)
                    {
                        case 0: name = "Scale X"; break;
                        case 4: name = "Scale Y"; break;
                        case 8: name = "Scale Z"; break;
                        case 12: name = "Rotate X"; break;
                        case 16: name = "Rotate Y"; break;
                        case 20: name = "Rotate Z"; break;
                        case 24: name = "Translate X"; break;
                        case 28: name = "Translate Y"; break;
                        case 32: name = "Translate Z"; break;
                    }
                }
                return $"{name} [{offset}]";
            }

            private uint ConvertNameToParam(string name)
            {
                if (name.Contains("[") && name.Contains("]"))
                {
                    int start = name.LastIndexOf('[') + 1;
                    int end = name.LastIndexOf(']');
                    if (uint.TryParse(name.Substring(start, end - start), out uint res))
                        return res;
                }
                if (name.StartsWith("Offset_") && uint.TryParse(name.Substring(7), out uint res2))
                    return res2;
                if (name.Contains("Translate X")) return 16;
                if (name.Contains("Translate Y")) return 20;
                return 0;
            }

            public MaterialAnim FromYaml()
            {
                MaterialAnim matAnim = new MaterialAnim();
                matAnim.Name = Name;
                matAnim.Path = Path;
                matAnim.Loop = Loop;
                matAnim.FrameCount = FrameCount;
                matAnim.BakedSize = BakedSize;
                matAnim.TextureNames = (TextureNames != null) ? TextureNames.OrderBy(s => s).ToList() : GenerateTextureList();
                if (BindIndices != null) matAnim.BindIndices = BindIndices.ToArray();
                else matAnim.BindIndices = new ushort[MaterialAnimConfigs?.Count ?? 0];

                if (UserData != null)
                {
                    matAnim.UserData = new List<UserData>();
                    foreach (var udCfg in UserData)
                    {
                        var ud = new UserData();
                        ud.Name = udCfg.Name;
                        if (Enum.TryParse(udCfg.Type, out UserDataType type))
                        {
                            switch (type)
                            {
                                case UserDataType.Byte: ud.SetValue(udCfg.Values.Select(v => byte.Parse(v)).ToArray()); break;
                                case UserDataType.Int32: ud.SetValue(udCfg.Values.Select(v => int.Parse(v)).ToArray()); break;
                                case UserDataType.Single: ud.SetValue(udCfg.Values.Select(v => float.Parse(v)).ToArray()); break;
                                case UserDataType.String:
                                case UserDataType.WString: ud.SetValue(udCfg.Values.ToArray()); break;
                            }
                        }
                        matAnim.UserData.Add(ud);
                    }
                }

                int globalConstantCounter = 0;
                int globalCurveCounter = 0;

                foreach (var matCfg in MaterialAnimConfigs ?? new List<MatAnimConfig>())
                {
                    var matAnimData = new MaterialAnimData();
                    matAnimData.Name = matCfg.Name;
                    matAnimData.Constants = new List<AnimConstant>();
                    matAnimData.Curves = new List<AnimCurve>();
                    matAnimData.TexturePatternAnimInfos = new List<TexturePatternAnimInfo>();
                    matAnimData.ParamAnimInfos = new List<ParamAnimInfo>();
                    matAnimData.VisualConstantIndex = globalConstantCounter;
                    matAnimData.TexturePatternCurveIndex = globalCurveCounter;
                    matAnim.MaterialAnimDataList.Add(matAnimData);

                    ushort localConstantCounter = 0;
                    ushort localCurveCounter = 0;

                    foreach (var patternCfg in matCfg.TexturePatternInfos ?? new List<PatternInfo>())
                    {
                        var info = new TexturePatternAnimInfo() { Name = patternCfg.Name };
                        matAnimData.TexturePatternAnimInfos.Add(info);
                        info.BeginConstant = 65535;
                        info.CurveIndex = 65535;

                        if (patternCfg.IsConstant && patternCfg.ConstantValue != null)
                        {
                            info.BeginConstant = localConstantCounter++;
                            matAnimData.Constants.Add(new AnimConstant() { Value = matAnim.TextureNames.IndexOf(patternCfg.ConstantValue.Texture) });
                        }
                        else if (patternCfg.CurveData != null)
                        {
                            info.CurveIndex = localCurveCounter++;
                            var curve = new AnimCurve() { CurveType = AnimCurveType.StepInt, KeyType = AnimCurveKeyType.Single, FrameType = AnimCurveFrameType.Single, Scale = 1 };
                            matAnimData.Curves.Add(curve);
                            curve.Frames = patternCfg.CurveData.KeyFrames.Keys.Select(k => (float)k).OrderBy(f => f).ToArray();
                            curve.Keys = new float[curve.Frames.Length, 1];
                            for (int i = 0; i < curve.Frames.Length; i++)
                                curve.Keys[i, 0] = matAnim.TextureNames.IndexOf(patternCfg.CurveData.KeyFrames[(int)curve.Frames[i]]);
                            curve.EndFrame = curve.Frames.Length > 0 ? curve.Frames.Max() : 0;
                        }
                    }

                    matAnimData.BeginVisalConstantIndex = globalConstantCounter + localConstantCounter;
                    matAnimData.ShaderParamCurveIndex = globalCurveCounter + localCurveCounter;

                    foreach (var paramCfg in matCfg.ParamInfos ?? new List<ParamInfo>())
                    {
                        var info = new ParamAnimInfo() { Name = paramCfg.Name };
                        info.BeginConstant = localConstantCounter;
                        info.ConstantCount = (ushort)(paramCfg.Constants?.Count ?? 0);
                        localConstantCounter += info.ConstantCount;

                        if (paramCfg.Constants != null)
                        {
                            foreach (var c in paramCfg.Constants)
                                matAnimData.Constants.Add(new AnimConstant() { AnimDataOffset = ConvertNameToParam(c.Offset), Value = c.Value });
                        }

                        info.BeginCurve = localCurveCounter;
                        info.IntCurveCount = paramCfg.IntCurveCount;
                        info.FloatCurveCount = paramCfg.FloatCurveCount;
                        if (info.IntCurveCount == 0 && info.FloatCurveCount == 0) info.FloatCurveCount = (ushort)(paramCfg.CurveData?.Count ?? 0);

                        localCurveCounter += (ushort)(info.IntCurveCount + info.FloatCurveCount);

                        if (paramCfg.CurveData != null)
                        {
                            foreach (var cCfg in paramCfg.CurveData)
                            {
                                int valCount = 1;
                                if (cCfg.CurveType == AnimCurveType.Linear) valCount = 2;
                                if (cCfg.CurveType == AnimCurveType.Cubic) valCount = 4;

                                var curve = new AnimCurve() { CurveType = cCfg.CurveType, AnimDataOffset = ConvertNameToParam(cCfg.Offset), KeyType = cCfg.KeyType, FrameType = cCfg.FrameType, Scale = cCfg.Scale, Offset = cCfg.ValueOffset, Delta = cCfg.Delta };
                                if (curve.KeyType == 0) curve.KeyType = AnimCurveKeyType.Single;
                                if (curve.FrameType == 0) curve.FrameType = AnimCurveFrameType.Single;
                                if (curve.Scale == 0 && curve.Offset == 0 && curve.Keys == null) curve.Scale = 1;

                                matAnimData.Curves.Add(curve);
                                curve.Frames = cCfg.KeyFrames.Select(k => (float)k.Frame).OrderBy(f => f).ToArray();

                                int yamlValCount = (cCfg.KeyFrames != null && cCfg.KeyFrames.Count > 0) ? cCfg.KeyFrames[0].Values.Count : 0;
                                curve.Keys = new float[curve.Frames.Length, Math.Max(valCount, yamlValCount)];

                                var sortedKeys = cCfg.KeyFrames.OrderBy(k => k.Frame).ToList();
                                for (int i = 0; i < curve.Frames.Length; i++)
                                {
                                    for (int v = 0; v < curve.Keys.GetLength(1) && v < sortedKeys[i].Values.Count; v++)
                                        curve.Keys[i, v] = sortedKeys[i].Values[v];
                                }
                                
                                // Recalculate Delta for Linear if missing
                                if (curve.CurveType == AnimCurveType.Linear && curve.Frames.Length > 1 && curve.Keys.GetLength(1) >= 2 && curve.Delta == 0)
                                {
                                    float frameDist = curve.Frames[1] - curve.Frames[0];
                                    if (frameDist != 0)
                                        curve.Delta = (curve.Keys[1, 0] - curve.Keys[0, 0]) / frameDist;
                                }
                                curve.EndFrame = curve.Frames.Length > 0 ? curve.Frames.Max() : 0;
                            }
                        }
                        matAnimData.ParamAnimInfos.Add(info);
                    }

                    globalConstantCounter += localConstantCounter;
                    globalCurveCounter += localCurveCounter;
                }
                return matAnim;
            }

            private List<string> GenerateTextureList()
            {
                var dict = new HashSet<string>();
                foreach (var m in MaterialAnimConfigs ?? new List<MatAnimConfig>())
                {
                    foreach (var p in m.TexturePatternInfos ?? new List<PatternInfo>())
                    {
                        if (p.ConstantValue != null) dict.Add(p.ConstantValue.Texture);
                        if (p.CurveData != null) foreach (var k in p.CurveData.KeyFrames.Values) dict.Add(k);
                    }
                }
                return dict.OrderBy(s => s).ToList();
            }
        }

        public class UserDataConfig { public string Name { get; set; } public string Type { get; set; } public List<string> Values { get; set; } }
        public class ConstantConfig { public string Offset { get; set; } public float Value { get; set; } }
        public class ConstantTPConfig { public string Texture { get; set; } }
        public class KeyFrameConfig { public int Frame { get; set; } public List<float> Values { get; set; } }
        public class CurveConfig { public string Offset { get; set; } public AnimCurveType CurveType { get; set; } public AnimCurveKeyType KeyType { get; set; } public AnimCurveFrameType FrameType { get; set; } public float Scale { get; set; } public float ValueOffset { get; set; } public float Delta { get; set; } public List<KeyFrameConfig> KeyFrames { get; set; } public CurveConfig() { KeyFrames = new List<KeyFrameConfig>(); } }
        public class CurveTPConfig { public Dictionary<int, string> KeyFrames { get; set; } public CurveTPConfig() { KeyFrames = new Dictionary<int, string>(); } }
        public class MatAnimConfig { public string Name { get; set; } public List<PatternInfo> TexturePatternInfos { get; set; } public List<ParamInfo> ParamInfos { get; set; } public MatAnimConfig() { TexturePatternInfos = new List<PatternInfo>(); ParamInfos = new List<ParamInfo>(); } }
        public class ParamInfo { public string Name { get; set; } public string PropertyType { get; set; } public bool IsConstant { get; set; } public ushort IntCurveCount { get; set; } public ushort FloatCurveCount { get; set; } public List<ConstantConfig> Constants { get; set; } public List<CurveConfig> CurveData { get; set; } }
        public class PatternInfo { public string Name { get; set; } public bool IsConstant { get; set; } public ConstantTPConfig ConstantValue { get; set; } public CurveTPConfig CurveData { get; set; } }

        public static MaterialAnim FromYaml(string Name)
        {
            var serializer = new Serializer(new SerializerSettings() { ComparerForKeySorting = null });
            serializer.Settings.RegisterTagMapping("AnimConfig", typeof(AnimConfig));
            return serializer.Deserialize<AnimConfig>(File.ReadAllText(Name)).FromYaml();
        }

        public static string ToYaml(string Name, MaterialAnim MatAnim, Toolbox.Library.Animations.MaterialAnimation.AnimationType animType)
        {
            var config = new AnimConfig();
            config.ToYaml(MatAnim, animType);
            var serializer = new Serializer(new SerializerSettings() { EmitTags = false, EmitAlias = false, ComparerForKeySorting = null });
            serializer.Settings.RegisterTagMapping("AnimConfig", typeof(AnimConfig));
            return serializer.Serialize(config, typeof(AnimConfig));
        }
    }
}
