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

                    foreach (var paramInfo in mat.ParamAnimInfos)
                    {

                    }

                    foreach (var patternInfo in mat.TexturePatternAnimInfos)
                    {
                        PatternInfo infoCfg = new PatternInfo();
                        infoCfg.Name = patternInfo.Name;
                        infoCfg.IsConstant = patternInfo.BeginConstant != ushort.MaxValue;
                        if (patternInfo.CurveIndex != uint.MaxValue)
                        {
                            var curve = mat.Curves[(int)patternInfo.CurveIndex];
                            infoCfg.CurveData = new CurveTPConfig();
                        }
                    }
                }
            }
        }

        public class CurveTPConfig
        {

        }

        public class MatAnimConfig
        {
            public string Name { get; set; }

            public List<PatternInfo> TexturePatternInfos { get; set; }
            public List<ParamInfo> ParamInfos { get; set; }
            public List<PatternInfo> TexturePatternInfo { get; set; }
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

            public CurveTPConfig CurveData { get; set; }
        }

        public static string ToYaml(string Name, FMAA anim)
        {
            var serializerSettings = new SerializerSettings()
            {
                EmitTags = false
            };

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
