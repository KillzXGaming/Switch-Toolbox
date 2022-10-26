using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace CafeLibrary.M2
{
    public class MaterialList
    {
        public uint Version { get; set; }

        public Dictionary<uint, Material> Materials = new Dictionary<uint, Material>();

        public List<Texture> Textures = new List<Texture>();

        public uint ShaderHash { get; set; }

        private bool _hasUint64Offsets;

        public MaterialList(Stream stream, bool hasUint64Offsets)
        {
            _hasUint64Offsets = hasUint64Offsets;

            using (var reader = new FileReader(stream))
            {
                Read(reader);
            }
        }

        public void Read(FileReader reader)
        {
            reader.ReadUInt32(); //version
            Version = reader.ReadUInt32();
            uint materialCount = reader.ReadUInt32();
            uint textureCount = reader.ReadUInt32();
            ShaderHash = ReadUint(reader);
            uint textureOffset = ReadUint(reader);
            uint materialOffset = ReadUint(reader);

            reader.SeekBegin(textureOffset);
            for (int i = 0; i < textureCount; i++)
            {
                Texture tex = new Texture();
                tex.Flags = ReadUint(reader);
                ReadUint(reader); //0
                ReadUint(reader); //0
                long pos = reader.Position;
                tex.Name = reader.ReadZeroTerminatedString();

                reader.SeekBegin(pos + 64);
                Textures.Add(tex);
            }

            reader.SeekBegin(materialOffset);
            for (int i = 0; i < materialCount; i++)
            {
                Material material = new Material();

                uint id = ReadUint(reader);
                material.NameHash = reader.ReadUInt32();
                uint paramSize = reader.ReadUInt32();
                material.ShaderHash = reader.ReadUInt32();
                uint skinningHash = reader.ReadUInt32();
                reader.ReadUInt16();
                reader.ReadByte();

                reader.ReadBytes(9);
                reader.ReadByte();
                reader.ReadBytes(19);
                uint paramOffset = ReadUint(reader);
                ReadUint(reader);

                Materials.Add(material.NameHash, material);

                using (reader.TemporarySeek(paramOffset, SeekOrigin.Begin))
                {
                    if (_hasUint64Offsets)
                    {
                        ulong[] parameters = reader.ReadUInt64s((int)paramSize / 8);
                        for (int j = 0; j < parameters.Length; j++)
                        {
                            if (parameters[j].ToString().StartsWith("34397848")) //tAlbedoMap
                                material.DiffuseMap = FindTextureParam(parameters, j);
                            if (parameters[j].ToString().StartsWith("577110")) //tNormalMap
                                material.NormalMap = FindTextureParam(parameters, j);
                            if (parameters[j].ToString().StartsWith("24862")) //tSpecularMap
                                material.SpecularMap = FindTextureParam(parameters, j);
                        }
                    }
                    else
                    {
                        uint[] parameters = reader.ReadUInt32s((int)paramSize / 4);
                        for (int j = 0; j < parameters.Length; j++)
                        {
                            if (parameters[j].ToString().StartsWith("34397848")) //tAlbedoMap
                                material.DiffuseMap = FindTextureParam(parameters, j);
                            if (parameters[j].ToString().StartsWith("5771109")) //tNormalMap
                                material.NormalMap = FindTextureParam(parameters, j);
                             if (parameters[j].ToString().StartsWith("2486240")) //tSpecularMap
                                material.SpecularMap = FindTextureParam(parameters, j);
                        }
                    }
                }
            }
        }

        private Texture FindTextureParam(ulong[] parameters, int id)
        {
            var index = (uint)parameters[id - 1] - 1;
            if (index < Textures.Count) //Index before texture hash. Sometimes not always the case?
                return Textures[(int)index];
            return null;
        }

        private Texture FindTextureParam(uint[] parameters, int id)
        {
            var index = parameters[id - 1] - 1;
            if (index < Textures.Count) //Index before texture hash. Sometimes not always the case?
                return Textures[(int)index];
            return null;
        }

        private uint ReadUint(FileReader reader)
        {
            if (_hasUint64Offsets)
            {
                var v = (uint)reader.ReadInt32();
                reader.ReadInt32();
                return v;
            }
            return reader.ReadUInt32();
        }
    }

    public class Material
    {
        public uint NameHash;
        public uint ShaderHash;

        public Texture DiffuseMap;
        public Texture NormalMap;
        public Texture SpecularMap;
    }

    public class Texture
    {
        public uint Flags;
        public string Name;
    }
}
