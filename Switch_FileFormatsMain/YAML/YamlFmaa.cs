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
