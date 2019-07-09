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
    public class YamlFmat
    {
        public class RenderInfoConfig
        {
            public string Name { get; set; }
            public RenderInfoType Type { get; set; }
            public object Data { get; set; }
        }

        public static string ToYaml(string Name, FMAT material)
        {
            var serializerSettings = new SerializerSettings()
            {
                EmitTags = false
            };

            var serializer = new Serializer(serializerSettings);
            return serializer.Serialize(material);
        }
    }
}
