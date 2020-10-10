using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;
using static Toolbox.Library.IO.Bit;

namespace LayoutBXLYT.Revolution
{
    public class MAT1 : SectionCommon
    {
        public List<BxlytMaterial> Materials { get; set; }

        public MAT1()
        {
            Materials = new List<BxlytMaterial>();
        }

        public MAT1(FileReader reader, BxlytHeader header) : base()
        {
            Materials = new List<BxlytMaterial>();

            long pos = reader.Position;

            ushort numMats = reader.ReadUInt16();
            reader.Seek(2); //padding

            uint[] offsets = reader.ReadUInt32s(numMats);
            for (int i = 0; i < numMats; i++)
            {
                reader.SeekBegin(pos + offsets[i] - 8);
                Materials.Add(new Material(reader, header));
            }
        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
            long pos = writer.Position - 8;

            writer.Write((ushort)Materials.Count);
            writer.Seek(2);

            long _ofsPos = writer.Position;
            //Fill empty spaces for offsets later
            writer.Write(new uint[Materials.Count]);

            //Save offsets and strings
            for (int i = 0; i < Materials.Count; i++)
            {
                writer.WriteUint32Offset(_ofsPos + (i * 4), pos);
                ((Material)Materials[i]).Write(writer, header);
                writer.Align(4);
            }
        }
    }

    public class Material : BxlytMaterial
    {
        public STColor8 ColorRegister3 { get; set; }
        public STColor8 MatColor { get; set; } = STColor8.White;
        public STColor8 TevColor1 { get; set; }
        public STColor8 TevColor2 { get; set; }
        public STColor8 TevColor3 { get; set; }
        public STColor8 TevColor4 { get; set; }

        public List<TexCoordGenEntry> TexCoordGens { get; set; }
        public ChanCtrl ChanControl { get; set; } = new ChanCtrl();
        public TevSwapModeTable TevSwapModeTable { get; set; } = new TevSwapModeTable();
        public List<BxlytTextureTransform> IndirectTexTransforms { get; set; }

        public List<IndirectStage> IndirectStages { get; set; }

        private uint flags;

        public string GetTexture(int index)
        {
            if (TextureMaps[index].ID != -1)
                return ParentLayout.Textures[TextureMaps[index].ID];
            else
                return "";
        }

        public override BxlytMaterial Clone()
        {
            Material mat = new Material();
            return mat;
        }

        public override void RemoveTexture(int index)
        {
            base.RemoveTexture(index);
            if (TexCoordGens.Count > index)
                TexCoordGens.RemoveAt(index);
            if (IndirectTexTransforms.Count > index)
                IndirectTexTransforms.RemoveAt(index);
            if (TextureTransforms.Length > index)
                TextureTransforms = TextureTransforms.RemoveAt(index);

            RemoveTexCoordSources(index);

            Console.WriteLine($"TexCoordGens {TexCoordGens.Count}");
            Console.WriteLine($"TextureMaps {TextureMaps.Length}");
            Console.WriteLine($"TextureTransforms {TextureTransforms.Length}");
            Console.WriteLine($"IndirectTexTransforms {IndirectTexTransforms.Count}");
        }

        public override bool RemoveTexCoordSources(int index)
        {
            foreach (var texGen in TexCoordGens)
            {
                //Shift all tex coord types down when an index is removed
                if (texGen.Source >= TexCoordGenSource.GX_TG_TEX1 &&
                    texGen.Source < TexCoordGenSource.GX_TG_TEX7)
                {
                    texGen.Source = texGen.Source - 1;
                }
            }
            return true;
        }

        public bool HasMaterialColor { get; set; }
        public bool HasChannelControl { get; set; }
        public bool HasBlendMode { get; set; }
        public bool HasAlphaCompare { get; set; }
        public bool HasTevSwapTable { get; set; }

        public Shader Shader { get; set; }

        public Material() { }

        public Material(string name, BxlytHeader header)
        {
            ParentLayout = header;
            Name = name;
            TextureMaps = new TextureRef[0];
            TextureTransforms = new BxlytTextureTransform[0];
            TexCoordGens = new List<TexCoordGenEntry>();
            IndirectTexTransforms = new List<BxlytTextureTransform>();
            IndirectStages = new List<IndirectStage>();
            TevSwapModeTable = new TevSwapModeTable();
            ChanControl = new ChanCtrl();
            BlackColor = new STColor8(0, 0, 0, 0);
            WhiteColor = STColor8.White;
            ColorRegister3 = STColor8.White;
            TevColor1 = STColor8.White;
            TevColor2 = STColor8.White;
            TevColor3 = STColor8.White;
            TevColor4 = STColor8.White;
            TevStages = new TevStage[0];
            BlendMode = new BxlytBlendMode();
            AlphaCompare = new AlphaCompare();
        }

        public override void AddTexture(string texture)
        {
            int index = ParentLayout.AddTexture(texture);
            TextureRef textureRef = new TextureRef();
            textureRef.ID = (short)index;
            textureRef.Name = texture;
            TextureMaps = TextureMaps.AddToArray(textureRef);
            TexCoordGens.Add(new TexCoordGenEntry()
            {
                Type = TexCoordGenTypes.GX_TG_MTX2x4,
                MatrixSource = TexCoordGenMatrixSource.GX_DTTMTX9 + (TexCoordGens.Count * 4),
                Source = TexCoordGenSource.GX_TG_TEX0 
            });
            TextureTransforms = TextureTransforms.AddToArray(new BxlytTextureTransform());
        }

        public Material(FileReader reader, BxlytHeader header) : base()
        {
            ParentLayout = header;

            BlendMode = new BxlytBlendMode();
            AlphaCompare = new AlphaCompare();
            TexCoordGens = new List<TexCoordGenEntry>();
            IndirectTexTransforms = new List<BxlytTextureTransform>();
            IndirectStages = new List<IndirectStage>();

            Name = reader.ReadString(0x14, true);

            BlackColor = reader.ReadColor16RGBA().ToColor8();
            WhiteColor = reader.ReadColor16RGBA().ToColor8();
            ColorRegister3 = reader.ReadColor16RGBA().ToColor8();
            TevColor1 = reader.ReadColor8RGBA();
            TevColor2 = reader.ReadColor8RGBA();
            TevColor3 = reader.ReadColor8RGBA();
            TevColor4 = reader.ReadColor8RGBA();
            flags = reader.ReadUInt32();

            HasMaterialColor = Convert.ToBoolean(ExtractBits(flags, 1, 4));
            HasChannelControl = Convert.ToBoolean(ExtractBits(flags, 1, 6));
            HasBlendMode = Convert.ToBoolean(ExtractBits(flags, 1, 7));
            HasAlphaCompare = Convert.ToBoolean(ExtractBits(flags, 1, 8));
            uint tevStagesCount = ExtractBits(flags, 5, 9);
            uint indTexOrderCount = ExtractBits(flags, 2, 16);
            uint indSrtCount = ExtractBits(flags, 2, 17);
            HasTevSwapTable = Convert.ToBoolean(ExtractBits(flags, 1, 19));
            uint texCoordGenCount = ExtractBits(flags, 4, 20);
            uint mtxCount = ExtractBits(flags, 4, 24);
            uint texCount = ExtractBits(flags, 4, 28);

            Console.WriteLine($"HasMaterialColor {HasMaterialColor}");
            Console.WriteLine($"HasChannelControl {HasChannelControl}");
            Console.WriteLine($"HasBlendMode {HasBlendMode}");
            Console.WriteLine($"HasAlphaCompare {HasAlphaCompare}");
            Console.WriteLine($"tevStagesCount {tevStagesCount}");
            Console.WriteLine($"indTexOrderCount {indTexOrderCount}");
            Console.WriteLine($"indSrtCount {indSrtCount}");
            Console.WriteLine($"HasTevSwapTable {HasTevSwapTable}");
            Console.WriteLine($"texCoordGenCount {texCoordGenCount}");
            Console.WriteLine($"mtxCount {mtxCount}");
            Console.WriteLine($"texCount {texCount}");

            TextureMaps = new TextureRef[texCount];
            TevStages = new TevStage[tevStagesCount];
            TextureTransforms = new BxlytTextureTransform[mtxCount];

            for (int i = 0; i < texCount; i++)
                TextureMaps[i] = new TextureRef(reader, header);

            for (int i = 0; i < mtxCount; i++)
                TextureTransforms[i] = new BxlytTextureTransform(reader);

            for (int i = 0; i < texCoordGenCount; i++)
            {
                TexCoordGens.Add(new TexCoordGenEntry(reader));
            }

            if (HasChannelControl)
                ChanControl = new ChanCtrl(reader);

            if (HasMaterialColor)
                MatColor = reader.ReadColor8RGBA();

            if (HasTevSwapTable)
                TevSwapModeTable = new TevSwapModeTable(reader);

            for (int i = 0; i < indSrtCount; i++)
                IndirectTexTransforms.Add(new BxlytTextureTransform(reader));

            for (int i = 0; i < indTexOrderCount; i++)
                IndirectStages.Add(new IndirectStage(reader));

            for (int i = 0; i < tevStagesCount; i++)
                TevStages[i] = new TevStage(reader, header);

            if (HasAlphaCompare)
                AlphaCompare = new AlphaCompare(reader);

            if (HasBlendMode)
                BlendMode = new BxlytBlendMode(reader, header);
        }

        public void Write(FileWriter writer, LayoutHeader header)
        {
            if (MatColor != STColor8.White)
                HasMaterialColor = true;
            if (!ChanControl.HasDefaults())
                HasChannelControl = true;
            if (!BlendMode.HasDefaults())
                HasBlendMode = true;
            if (!((AlphaCompare)AlphaCompare).HasDefaults())
                HasAlphaCompare = true;

            if (TextureMaps?.Length > 0 && TexCoordGens?.Count == 0)
            {
                TexCoordGens = new List<TexCoordGenEntry>(TextureMaps.Length);
                for (int i = 0; i < TextureMaps?.Length; i++)
                    TexCoordGens[i] = new TexCoordGenEntry();
            }

            flags = 0;
            if (HasMaterialColor)
                flags |= (1 << 27);

            if (HasChannelControl)
                flags |= (1 << 25);

            if (HasBlendMode)
                flags |= (1 << 24);

            if (HasAlphaCompare)
                flags |= (1 << 23);

            flags |= (uint)((TevStages.Length & 31) << 18);
            flags |= (uint)((IndirectStages.Count & 0x7) << 15);
            flags |= (uint)((IndirectTexTransforms.Count & 0x3) << 13);
            if (HasTevSwapTable)
                flags |= (1 << 12);

            flags |= (uint)((TexCoordGens.Count & 0xF) << 8);
            flags |= (uint)((TextureTransforms.Length & 0xF) << 4);
            flags |= (uint)((TextureMaps.Length & 0xF) << 0);


            writer.WriteString(Name, 0x14);
            writer.Write(BlackColor.ToColor16());
            writer.Write(WhiteColor.ToColor16());
            writer.Write(ColorRegister3.ToColor16());
            writer.Write(TevColor1);
            writer.Write(TevColor2);
            writer.Write(TevColor3);
            writer.Write(TevColor4);
            writer.Write(flags);

            for (int i = 0; i < TextureMaps.Length; i++)
                ((TextureRef)TextureMaps[i]).Write(writer);

            for (int i = 0; i < TextureTransforms.Length; i++)
                TextureTransforms[i].Write(writer);

            for (int i = 0; i < TexCoordGens.Count; i++)
                TexCoordGens[i].Write(writer);

            if (HasChannelControl)
                ChanControl.Write(writer);
            if (HasMaterialColor)
                writer.Write(MatColor);
            if (HasTevSwapTable)
                TevSwapModeTable.Write(writer);

            for (int i = 0; i < IndirectTexTransforms.Count; i++)
                IndirectTexTransforms[i].Write(writer);

            for (int i = 0; i < IndirectStages.Count; i++)
                IndirectStages[i].Write(writer);

            for (int i = 0; i < TevStages.Length; i++)
                ((TevStage)TevStages[i]).Write(writer);

            if (HasAlphaCompare)
                AlphaCompare.Write(writer);
            if (HasBlendMode)
                BlendMode.Write(writer);
        }
    }
}
