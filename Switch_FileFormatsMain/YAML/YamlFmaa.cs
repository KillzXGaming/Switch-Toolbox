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
        }

        public static string ToYaml(string Name, FMAA anim)
        {
            var serializerSettings = new SerializerSettings()
            {
                EmitTags = false
            };

            var MatAnim = anim.MaterialAnim;
            var config = new AnimConfig();

            var serializer = new Serializer(serializerSettings);
            return serializer.Serialize(anim);
        }
    }
}
