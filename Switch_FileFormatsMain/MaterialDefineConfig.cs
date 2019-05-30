using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin
{
    public class MaterialDefineConfig
    {
        public Dictionary<string, ConfigEntry> ParamConfigs = new Dictionary<string, ConfigEntry>();
        public Dictionary<string, ConfigEntry> OptionConfigs = new Dictionary<string, ConfigEntry>();
        public Dictionary<string, ConfigEntry> RenderInfoConfigs = new Dictionary<string, ConfigEntry>();
        public Dictionary<string, ConfigEntry> UserDataConfigs = new Dictionary<string, ConfigEntry>();

        public class ConfigEntry
        {
            //The original name of the parameter
            public string Name { get; set; }

            //Alternate name a user can give to potentially make it easier to read
            public string AlternateName { get; set; }

            //What effect the data gives.
            public string Description { get; set; }
        }
    }
}
