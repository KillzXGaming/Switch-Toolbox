using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin
{
    public class MaterialPresetConfig
    {
        public List<string> MaterialConfigs = new List<string>();

        public class MaterialEntry
        {
            //The name of the preset
            public string Name { get; set; }

            //The effects of the material
            public string Description { get; set; }

            //The game material belongs to
            public string Game { get; set; }

            //The shader used by the material (optional)
            //If a game does not use multiple shaders per bfres this is not necessary
            public string ShaderPath { get; set; }
        }
    }
}
