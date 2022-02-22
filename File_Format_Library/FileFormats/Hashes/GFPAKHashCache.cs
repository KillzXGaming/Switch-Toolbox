using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;

namespace FirstPlugin.FileFormats.Hashes
{
    public class GFPAKHashCache
    {
        private static Dictionary<ulong, string> HashCacheContent;

        private static ulong CurrentVersionHash;
        private static string HashBinaryPath;
        private static bool CacheUpdatedFlag;
        private static List<string> PokeHashTemplates = new List<string>();

        public static void InitializeHashCache()
        {
            HashBinaryPath = Path.Combine(Runtime.ExecutableDir, "Hashes", "GFPAKHashCache.bin");
            string HashExtraPath = Path.Combine(Runtime.ExecutableDir, "Hashes", "GFPAK.txt");

            bool NeedsBaseCacheRebuild = true;
            CurrentVersionHash = GetToolboxVersionHash();

            if (File.Exists(HashBinaryPath))
            {
                using (BinaryReader Reader = new BinaryReader(new FileStream(HashBinaryPath, FileMode.Open)))
                {
                    ulong CacheVersionHash = Reader.ReadUInt64();
                    NeedsBaseCacheRebuild = CacheVersionHash != CurrentVersionHash;
                    uint Count = Reader.ReadUInt32();
                    for (uint HashIndex = 0; HashIndex < Count; HashIndex++)
                    {
                        ulong HashCode = Reader.ReadUInt64();
                        string HashName = Reader.ReadString();
                        PutHash(HashCode, HashName);
                    }
                }
            }

            if (NeedsBaseCacheRebuild)
            {
                GenerateBaseHashList();
            }

            if (File.Exists(HashExtraPath))
            {
                string[] UserHashLines = File.ReadAllLines(HashExtraPath);
                foreach (string Line in UserHashLines){
                    PutHash(Line);
                }
            }

            WriteCache();
        }

        public static void WriteCache()
        {
            if (CacheUpdatedFlag)
            {
                using (BinaryWriter writer = new BinaryWriter(new FileStream(HashBinaryPath, FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    writer.Write(CurrentVersionHash);
                    writer.Write(HashCacheContent.Count);
                    foreach (KeyValuePair<ulong, string> Entry in HashCacheContent)
                    {
                        writer.Write(Entry.Key);
                        writer.Write(Entry.Value);
                    }
                }
                CacheUpdatedFlag = false;
            }
        }

        private static void GenerateBaseHashList()
        {
            foreach (string hashStr in Properties.Resources.Pkmn.Split('\n'))
            {
                string HashString = hashStr.TrimEnd();

                PutHash(HashString);

                //Mon nums
                if (HashString.Contains("XXXX"))
                {
                    GeneratePkmnString(HashString);
                    PokeHashTemplates.Add(HashString);
                }
            }
        }

        private static void GeneratePkmnString(string hashStr)
        {
            int[] alolanMons = {
                37, 38
            };

            int[] husuiMons = {
                58, 59, 100, 101, 157, 211, 215, 503, 549, 550,
                570, 571, 628, 751, 764, 765, 843, 1003, 1005, 1006
            };

            int[] frenzyForms = { 
                59, 101, 549, 751, 1002
            };

            string pokeStr = string.Empty;
            List<string> monNames;
            for (int i = 0; i < 1010; i++)
            {
                monNames = new List<string>();
                //Gen species num
                pokeStr = hashStr.Replace("XXXX", i.ToString("D4"));

                //..also sub out alt forms
                if (frenzyForms.Contains(i))
                    monNames.Add(pokeStr.Replace("YY", "71"));

                monNames.Add(pokeStr.Replace("YY", "00"));

                //..also sub out region forms
                foreach (var n in monNames) {
                    if (alolanMons.Contains(i)) 
                        TryAddHash(n.Replace("ZZ", "11"));
                    else if (husuiMons.Contains(i)) 
                        TryAddHash(n.Replace("ZZ", "41"));
                    else 
                        TryAddHash(n.Replace("ZZ", "00"));
                }
                monNames.Clear();
            }
        }

        private static void TryAddHash(string str) {
            ulong hash = FNV64A1.Calculate(str);
            if (!HashCacheContent.ContainsKey(hash))
                HashCacheContent.Add(hash, str);
        }

        public static void GeneratePokeStringsFromFile(string FileName)
        {
            foreach (string PokeNameBase in PokeHashTemplates)
            {
                string pokeStrFile = PokeNameBase.Replace("pm0000_00", FileName);
                PutHash(pokeStrFile);
            }
        }

        public static void EnsureHashCache()
        {
            if (HashCacheContent == null)
            {
                HashCacheContent = new Dictionary<ulong, string>();
                InitializeHashCache();
            }
        }

        public static string GetHashName(ulong Hash)
        {
            if (HashCacheContent.ContainsKey(Hash))
            {
                return HashCacheContent[Hash];
            }
            return null;
        }

        public static void PutHash(string Name)
        {
            PutHash(FNV64A1.Calculate(Name), Name);       
        }

        public static void PutHash(ulong Hash, string Name)
        {
            if (!HashCacheContent.ContainsKey(Hash))
            {
                CacheUpdatedFlag = true;
                HashCacheContent.Add(Hash, Name);

                if (Name.Contains('/'))
                {
                    string[] HashPaths = Name.Split('/');
                    for (int i = 0; i < HashPaths.Length; i++)
                    {
                        PutHash(HashPaths[i]);
                    }
                }
            }
        }

        private static ulong GetToolboxVersionHash()
        {
            string VersionFilePath = Path.Combine(Runtime.ExecutableDir, "Version.txt");
            ulong VersionHash = 0;
            if (File.Exists(VersionFilePath))
            {
                byte[] VersionBytes = File.ReadAllBytes(VersionFilePath);
                VersionHash = FNV64A1.Calculate(VersionBytes);
            }
            return VersionHash;
        }
    }
}
