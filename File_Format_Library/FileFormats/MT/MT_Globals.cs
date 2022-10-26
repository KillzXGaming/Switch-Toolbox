using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Toolbox.Library.Security.Cryptography;
using Toolbox.Library;

namespace CafeLibrary.M2
{
    public class MT_Globals
    {
        private static Dictionary<uint, Shader.AttributeGroup> attGroups;

        public static Dictionary<uint, Shader.AttributeGroup> AttributeGroups
        {
            get
            {
                if (attGroups == null)
                    Load();

                return attGroups;
            }
        }
        public static void Load()
        {
            attGroups = new Dictionary<uint, Shader.AttributeGroup>();

            foreach (var file in Directory.GetFiles(Path.Combine(Runtime.ExecutableDir, "Lib", "MTVertexFormats")))
            {
                LoadPresets(file);
            }
        }

        static void LoadPresets(string filePath)
        {
            var shader = JsonConvert.DeserializeObject<Shader>(File.ReadAllText(filePath));
            foreach (var item in shader.AttributeGroups)
                if (!attGroups.ContainsKey(item.Key))
                    attGroups.Add(item.Key, item.Value);
        }

        public static uint Hash(string Text) {
            return CRC32Hash.Hash(Text);
        }

        public static uint jamcrc(string name)
        {
            var crc = Crc32.Compute(name);
            return (crc ^ 0xffffffff) & 0x7fffffff;
        }

        public static uint mfxcrc(string name, uint index)
        {
            var crc = Crc32.Compute(name);
            return (((crc ^ 0xffffffff) & 0x7fffffff) << 12) + index;
        }

        public static uint crc32(string name)
        {
            var crc = Crc32.Compute(name);
            return crc;
        }

    }
}
