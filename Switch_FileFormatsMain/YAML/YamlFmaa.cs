using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bfres.Structs;
using SharpYaml;
using SharpYaml.Serialization;
using Syroot.NintenTools.NSW.Bfres;

namespace FirstPlugin
{
    public class YamlFmaa
    {
        public class AnimConfig
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public int FrameCount { get; set; }

            public List<MatAnimConfig> MaterialAnimConfigs = new List<MatAnimConfig>();

            public AnimConfig(MaterialAnim materialAnim)
            {
                Name = materialAnim.Name;
                Path = materialAnim.Path;
                FrameCount = materialAnim.FrameCount;

                foreach (var mat in materialAnim.MaterialAnimDataList)
                {
                    MatAnimConfig matConfig = new MatAnimConfig();
                    matConfig.Name = mat.Name;
                    MaterialAnimConfigs.Add(matConfig);

                    foreach (var paramInfo in mat.ParamAnimInfos)
                    {
                        ParamInfo paramCfg = new ParamInfo();
                        paramCfg.Name = paramInfo.Name;
                        paramCfg.IsConstant = paramInfo.BeginConstant != ushort.MaxValue;
                        matConfig.ParamInfos.Add(paramCfg);

                        if (paramInfo.BeginCurve != ushort.MaxValue)
                        {
                            var curve = mat.Curves[(int)paramInfo.BeginCurve];
                            for (int i = 0; i < paramInfo.IntCurveCount + paramInfo.FloatCurveCount; i++)
                            {

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
                        if (patternInfo.CurveIndex != uint.MaxValue)
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

            public MaterialAnim FromYaml()
            {
                MaterialAnim matAnim = new MaterialAnim();
                matAnim.Name = Name;
                matAnim.Path = Path;
                matAnim.FrameCount = FrameCount;
                matAnim.TextureNames = GenerateTextureList();

                foreach (var matCfg in MaterialAnimConfigs)
                {
                    var matAnimData = new MaterialAnimData();
                    matAnimData.Constants = new List<AnimConstant>();
                    matAnimData.Curves = new List<AnimCurve>();
                    matAnimData.BeginVisalConstantIndex = 0;

                    foreach (var texturePatternCfg in matCfg.TexturePatternInfos)
                    {
                        if (texturePatternCfg.IsConstant && texturePatternCfg.ConstantValue != null)
                        {
                            AnimConstant constant = new AnimConstant();
                            constant.AnimDataOffset = 0;
                            constant.Value = matAnim.TextureNames.IndexOf(texturePatternCfg.ConstantValue.Texture);
                        }
                        else if (texturePatternCfg.CurveData != null)
                        {
                            AnimCurve curve = new AnimCurve();
                            matAnimData.Curves.Add(curve);

                            curve.Offset = 0;
                            curve.AnimDataOffset = 0;
                            curve.Scale = 1;
                            curve.CurveType = AnimCurveType.StepInt;
                            curve.StartFrame = 0;

                            int FrameCount = texturePatternCfg.CurveData.KeyFrames.Count;

                            curve.Frames = new float[FrameCount];
                            curve.Keys = new float[FrameCount, 0];

                            int MaxFrame = 0;
                            int MaxIndex = 0;

                            int i = 0;
                            foreach (var KeyFrame in texturePatternCfg.CurveData.KeyFrames)
                            {
                                int Index = matAnim.TextureNames.IndexOf(KeyFrame.Value);
                                curve.Frames[i] = KeyFrame.Key;
                                curve.Keys[i, 0] = Index;

                                MaxFrame = Math.Max(MaxIndex, KeyFrame.Key);
                                MaxIndex = Math.Max(MaxIndex, Index);

                                i++;
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

                    }

                    matAnim.MaterialAnimDataList.Add(matAnimData);
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

        public class ConstantTPConfig
        {
            public string Texture;
        }

        public class CurveTPConfig
        {
            public Dictionary<int, string> KeyFrames = new Dictionary<int, string>();
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

            public AnimCurve Curve { get; set; }
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
            var serializer = new Serializer();
            var config = serializer.Deserialize(Name);

            return ((AnimConfig)config).FromYaml();
        }

        public static string ToYaml(string Name, FMAA anim)
        {
            var serializerSettings = new SerializerSettings()
            {
                EmitTags = false
            };

            serializerSettings.DefaultStyle = YamlStyle.Any;
            serializerSettings.ComparerForKeySorting = null;

            var MatAnim = anim.MaterialAnim;
            var config = new AnimConfig(MatAnim);

            var serializer = new Serializer(serializerSettings);
            return serializer.Serialize(config);
        }

        private void SetConfig()
        {

        }
    }
}
