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
    public class YamlFska
    {
        public static string ToYaml(string Name, FSKA skeletalAnim)
        {
            var serializerSettings = new SerializerSettings()
            {
                EmitTags = false
            };

            var serializer = new Serializer(serializerSettings);
            return serializer.Serialize(skeletalAnim);
        }
    }
}
