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

                if (HashString.Contains("pm0000") ||
                    HashString.Contains("poke_XXXX") ||
                    HashString.Contains("poke_ball_0000") ||
                    HashString.Contains("poke_face_0000") ||
                    HashString.Contains("poke_motion_0000"))
                {
                    GenerateGenericPokeStrings(HashString);
                    PokeHashTemplates.Add(HashString);
                }
            }
        }

        private static void GenerateGenericPokeStrings(string hashStr)
        {
            for (int i = 0; i < 1000; i++)
            {
                string pokeStr = string.Empty;
                if (hashStr.Contains("pm0000")) pokeStr = hashStr.Replace("pm0000", $"pm{i.ToString("D4")}");
                else if (hashStr.Contains("poke_XXXX")) pokeStr = hashStr.Replace("poke_XXXX", $"poke_{i.ToString("D4")}");
                else if (hashStr.Contains("poke_ball_0000")) pokeStr = hashStr.Replace("poke_ball_0000", $"poke_ball_{i.ToString("D4")}");
                else if (hashStr.Contains("poke_face_0000")) pokeStr = hashStr.Replace("poke_face_0000", $"poke_face_{i.ToString("D4")}");
                else if (hashStr.Contains("poke_motion_0000")) pokeStr = hashStr.Replace("poke_motion_0000", $"poke_motion_{i.ToString("D4")}");
                ulong hash = FNV64A1.Calculate(pokeStr);
                if (!HashCacheContent.ContainsKey(hash))
                    HashCacheContent.Add(hash, pokeStr);
            }

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
