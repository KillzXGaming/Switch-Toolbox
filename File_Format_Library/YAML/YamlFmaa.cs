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

            public List<MatAnimConfig> MaterialAnimConfigs { get; set; }

            public void ToYaml(MaterialAnim materialAnim, bool isColorParam)
            {
                MaterialAnimConfigs = new List<MatAnimConfig>();

                Name = materialAnim.Name;
                Path = materialAnim.Path;
                FrameCount = materialAnim.FrameCount;
                Loop = materialAnim.Loop;

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
                        matConfig.ParamInfos.Add(paramCfg);

                        if (paramInfo.ConstantCount != 0)
                        {
                            paramCfg.Constants = new List<ConstantConfig>();
                            for (int i = 0; i < paramInfo.ConstantCount; i++)
                            {
                                AnimConstant constant = mat.Constants[paramInfo.BeginConstant + i];
                                ConstantConfig ConstantValue = new ConstantConfig();
                                ConstantValue.Offset = ConvertParamOffset(constant.AnimDataOffset, isColorParam);
                                ConstantValue.Value = constant.AnimDataOffset;

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
                                CurveCfg.Offset = ConvertParamOffset(curve.AnimDataOffset, isColorParam);

                                if (curve.Scale == 0)
                                    curve.Scale = 1;

                                for (int f = 0; f < curve.Frames.Length; f++)
                                {
                                    int frame = (int)curve.Frames[f];
                                    float Value = curve.Offset + curve.Keys[f, 0] * curve.Scale;
                                    CurveCfg.KeyFrames.Add(frame, Value);
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
                            infoCfg.ConstantValue.Texture = materialAnim.TextureNames[Index];
                        }
                        if (patternInfo.CurveIndex != ushort.MaxValue)
                        {
                            var curve = mat.Curves[(int)patternInfo.CurveIndex];
                            infoCfg.CurveData = new CurveTPConfig();

                            if (curve.Scale == 0)
                                curve.Scale = 1;

                            for (int f = 0; f < curve.Frames.Length; f++)
                            {
                                int frame = (int)curve.Frames[f];
                                int Value = (int)curve.Offset + (int)curve.Keys[f, 0] * (int)curve.Scale;

                                infoCfg.CurveData.KeyFrames.Add(frame, materialAnim.TextureNames[Value]);
                            }
                        }
                    }
                }
            }

            private uint ConvertParamOffset(string offset)
            {
                uint val = 0;
                switch (offset)
                {
                    case "R": return 0;
                    case "G": return 4;
                    case "B": return 8;
                    case "A": return 12;
                    default:
                        uint.TryParse(offset, out val);
                        break;
                }

                return val;
            }


            private string ConvertParamOffset(uint offset, bool isColorParam)
            {
                if (isColorParam)
                {
                    switch (offset)
                    {
                        case 0: return "R";
                        case 4: return "G";
                        case 8: return "B";
                        case 12: return "A";
                        default:
                            return offset.ToString();
                    }
                }

                return offset.ToString();
            }

            public MaterialAnim FromYaml()
            {
                MaterialAnim matAnim = new MaterialAnim();
                matAnim.Name = Name;
                matAnim.Path = Path;
                matAnim.Loop = Loop;
                matAnim.FrameCount = FrameCount;
                matAnim.TextureNames = GenerateTextureList();
                matAnim.BindIndices = new ushort[MaterialAnimConfigs.Count];
                for (int i = 0; i < matAnim.BindIndices.Length; i++)
                    matAnim.BindIndices[i] = ushort.MaxValue;

                int ShaderParamCurveIndex = 0;
                int TexturePatternCurveIndex = 0;

                Console.WriteLine("MaterialAnimConfigs " + MaterialAnimConfigs.Count);
                foreach (var matCfg in MaterialAnimConfigs)
                {
                    var matAnimData = new MaterialAnimData();
                    matAnimData.Name = matCfg.Name;
                    matAnimData.Constants = new List<AnimConstant>();
                    matAnimData.Curves = new List<AnimCurve>();
                    matAnimData.TexturePatternAnimInfos = new List<TexturePatternAnimInfo>();
                    matAnimData.ParamAnimInfos = new List<ParamAnimInfo>();
                    matAnimData.BeginVisalConstantIndex = -1;
                    matAnimData.ShaderParamCurveIndex = -1;
                    matAnimData.VisualConstantIndex = -1;
                    matAnimData.TexturePatternCurveIndex = -1;
                    matAnimData.VisalCurveIndex = -1;
                    matAnim.MaterialAnimDataList.Add(matAnimData);

                    ushort CurveIndex = 0;
                    ushort BeginConstantIndex = 0;

                    foreach (var texturePatternCfg in matCfg.TexturePatternInfos)
                    {
                        TexturePatternAnimInfo patternInfo = new TexturePatternAnimInfo();
                        patternInfo.Name = texturePatternCfg.Name;
                        matAnimData.TexturePatternAnimInfos.Add(patternInfo);

                        if (texturePatternCfg.IsConstant && texturePatternCfg.ConstantValue != null)
                        {
                            patternInfo.BeginConstant = BeginConstantIndex++;

                            AnimConstant constant = new AnimConstant();
                            constant.AnimDataOffset = 0;
                            constant.Value = matAnim.TextureNames.IndexOf(texturePatternCfg.ConstantValue.Texture);
                            matAnimData.Constants.Add(constant);

                            matAnimData.VisualConstantIndex = 0;
                            matAnimData.BeginVisalConstantIndex = 0;
                        }
                        else if (texturePatternCfg.CurveData != null)
                        {
                            patternInfo.CurveIndex = CurveIndex++;

                            matAnimData.TexturePatternCurveIndex = TexturePatternCurveIndex;
                            matAnimData.BeginVisalConstantIndex = 0;

                            AnimCurve curve = new AnimCurve();
                            matAnimData.Curves.Add(curve);

                            curve.Offset = 0;
                            curve.AnimDataOffset = 0;
                            curve.Scale = 1;
                            curve.CurveType = AnimCurveType.StepInt;
                            curve.StartFrame = 0;

                            int FrameCount = texturePatternCfg.CurveData.KeyFrames.Count;

                            curve.Frames = new float[FrameCount];
                            curve.Keys = new float[FrameCount, 1];

                            int MaxFrame = 0;
                            int MaxIndex = 0;

                            int i = 0;
                            foreach (var KeyFrame in texturePatternCfg.CurveData.KeyFrames)
                            {
                                int Index = matAnim.TextureNames.IndexOf(KeyFrame.Value);
                                Console.WriteLine($"{Index} {KeyFrame.Value}");

                                curve.Frames[i] = KeyFrame.Key;
                                curve.Keys[i, 0] = Index;

                                MaxFrame = Math.Max(MaxIndex, KeyFrame.Key);
                                MaxIndex = Math.Max(MaxIndex, Index);

                                i++;
                            }

                            curve.EndFrame = curve.Frames.Max();

                            if (curve.Keys.Length > 1)
                            {
                                curve.Delta = curve.Keys[curve.Keys.Length - 1,0] - curve.Keys[0, 0];
                            }

                            if (MaxFrame < byte.MaxValue)
                                curve.FrameType = AnimCurveFrameType.Byte;
                            else if (MaxFrame < ushort.MaxValue)
                                curve.FrameType = AnimCurveFrameType.Decimal10x5;
                            else
                                curve.FrameType = AnimCurveFrameType.Single;

                            if (MaxIndex < byte.MaxValue)
                                curve.KeyType = AnimCurveKeyType.SByte;
                            else if (MaxIndex < ushort.MaxValue)
                                curve.KeyType = AnimCurveKeyType.Int16;
                            else
                                curve.KeyType = AnimCurveKeyType.Single;
                        }
                    }

                    foreach (var paramCfg in matCfg.ParamInfos)
                    {
                        ParamAnimInfo paramInfo = new ParamAnimInfo();
                        paramInfo.Name = paramCfg.Name;
                        matAnimData.ParamAnimInfos.Add(paramInfo);

                        if (paramCfg.Constants != null && paramCfg.Constants.Count > 0)
                        {
                            paramInfo.BeginConstant = BeginConstantIndex;
                            paramInfo.ConstantCount = (ushort)paramCfg.Constants.Count;

                            BeginConstantIndex += (ushort)paramCfg.Constants.Count;
                            foreach (var constantCfg in paramCfg.Constants)
                            {
                                AnimConstant constant = new AnimConstant();
                                constant.AnimDataOffset = ConvertParamOffset(constantCfg.Offset);
                                constant.Value = constantCfg.Value;
                                matAnimData.Constants.Add(constant);
                            }

                            matAnimData.VisualConstantIndex = 0;
                            matAnimData.BeginVisalConstantIndex = 0;
                        }
                        if (paramCfg.CurveData != null && paramCfg.CurveData.Count > 0)
                        {
                            paramInfo.BeginCurve = CurveIndex;
                            paramInfo.FloatCurveCount = (ushort)paramCfg.CurveData.Count;

                            CurveIndex += (ushort)paramCfg.CurveData.Count;
                            foreach (var curveCfg in paramCfg.CurveData)
                            {
                                AnimCurve curve = new AnimCurve();
                                matAnimData.Curves.Add(curve);
                                curve.Offset = 0;
                                curve.AnimDataOffset = ConvertParamOffset(curveCfg.Offset);
                                curve.Scale = 1;
                                curve.CurveType = AnimCurveType.Linear;
                                curve.StartFrame = 0;

                                int MaxFrame = 0;
                                float MaxValue = 0;

                                int FrameCount = curveCfg.KeyFrames.Count;
                                curve.Frames = new float[FrameCount];
                                curve.Keys = new float[FrameCount, 2];

                                int i = 0;
                                var values = curveCfg.KeyFrames.Values.ToList();
                                foreach (var KeyFrame in curveCfg.KeyFrames)
                                {
                                    curve.Frames[i] = KeyFrame.Key;
                                    curve.Keys[i, 0] = KeyFrame.Value;

                                    //Calculate delta
                                    float Delta = 0;
                                    if (i < values.Count - 1)
                                        Delta = values[i + 1] - values[i];

                                    curve.Keys[i, 1] = Delta;

                                    MaxFrame = Math.Max(MaxFrame, KeyFrame.Key);
                                    MaxValue = Math.Max(MaxValue, KeyFrame.Value);
                                    i++;
                                }

                                curve.EndFrame = curve.Frames.Max();

                                if (curve.Keys.Length > 1)
                                {
                                    curve.Delta = values[values.Count - 1] - values[0];
                                }


                                if (MaxFrame < byte.MaxValue)
                                    curve.FrameType = AnimCurveFrameType.Byte;
                                else if (MaxFrame < ushort.MaxValue)
                                    curve.FrameType = AnimCurveFrameType.Decimal10x5;
                                else
                                    curve.FrameType = AnimCurveFrameType.Single;

                                if (MaxValue < byte.MaxValue)
                                    curve.KeyType = AnimCurveKeyType.SByte;
                                else if (MaxValue < ushort.MaxValue)
                                    curve.KeyType = AnimCurveKeyType.Int16;
                                else
                                    curve.KeyType = AnimCurveKeyType.Single;
                            }
                        }
                    }

                    TexturePatternCurveIndex += matAnimData.TexturePatternAnimInfos.Where(item => item.CurveIndex != uint.MaxValue).ToList().Count;
                    ShaderParamCurveIndex += matAnimData.ParamAnimInfos.Where(item => item.BeginCurve != ushort.MaxValue).ToList().Count;
                }

                return matAnim;
            }

            private List<string> GenerateTextureList()
            {
                List<string> Textures = new List<string>();
                foreach (var matCfg in MaterialAnimConfigs)
                {
                    foreach (var texturePatternCfg in matCfg.TexturePatternInfos)
                    {
                        if (texturePatternCfg.ConstantValue != null)
                        {
                            if (!Textures.Contains(texturePatternCfg.ConstantValue.Texture))
                                Textures.Add(texturePatternCfg.ConstantValue.Texture);
                        }

                        if (texturePatternCfg.CurveData == null)
                            continue;

                        foreach (var KeyFrame in texturePatternCfg.CurveData.KeyFrames)
                            if (!Textures.Contains(KeyFrame.Value))
                                Textures.Add(KeyFrame.Value);
                    }
                }

                return Textures;
            }
        }

        public class ConstantConfig
        {
            public string Offset { get; set; }
            public float Value { get; set; }
        }

        public class ConstantTPConfig
        {
            public string Texture { get; set; }
        }

        public class CurveConfig
        {
            public Dictionary<int, float> KeyFrames { get; set; }

            public string Offset;

            public CurveConfig()
            {
                KeyFrames = new Dictionary<int, float>();
            }
        }

        public class CurveTPConfig
        {
            public Dictionary<int, string> KeyFrames { get; set; }

            public CurveTPConfig()
            {
                KeyFrames = new Dictionary<int, string>(); 
            }
        }

        public class MatAnimConfig
        {
            public string Name { get; set; }

            public List<PatternInfo> TexturePatternInfos { get; set; }
            public List<ParamInfo> ParamInfos { get; set; }

            public MatAnimConfig()
            {
                TexturePatternInfos = new List<PatternInfo>();
                ParamInfos = new List<ParamInfo>();
            }
        }

        public class ParamInfo
        {
            public string Name { get; set; }

            public bool IsConstant { get; set; }

            public List<ConstantConfig> Constants { get; set; }

            public List<CurveConfig> CurveData { get; set; }
        }

        public class PatternInfo
        {
            public string Name { get; set; }

            public bool IsConstant { get; set; }

            public ConstantTPConfig ConstantValue { get; set; }

            public CurveTPConfig CurveData { get; set; }
        }

        public static MaterialAnim FromYaml(string Name)
        {
            var serializerSettings = new SerializerSettings()
            {
               // EmitTags = false
            };

            serializerSettings.DefaultStyle = YamlStyle.Any;
            serializerSettings.ComparerForKeySorting = null;
            serializerSettings.RegisterTagMapping("AnimConfig", typeof(AnimConfig));

            var serializer = new Serializer( serializerSettings);
            AnimConfig config = serializer.Deserialize<AnimConfig>(File.ReadAllText(Name));

            return config.FromYaml();
        }

        public static string ToYaml(string Name, MaterialAnim MatAnim, bool isColorParam)
        {
            var serializerSettings = new SerializerSettings()
            {
              //  EmitTags = false
            };

            serializerSettings.DefaultStyle = YamlStyle.Any;
            serializerSettings.ComparerForKeySorting = null;
            serializerSettings.RegisterTagMapping("AnimConfig", typeof(AnimConfig));

            var config = new AnimConfig();
            config.ToYaml(MatAnim, isColorParam);

            var serializer = new Serializer(serializerSettings);
            string yaml = serializer.Serialize(config, typeof(AnimConfig));

            return yaml;
        }

        private void SetConfig()
        {

        }
    }
}
