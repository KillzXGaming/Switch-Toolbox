using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Toolbox.Library.IO;
using Toolbox.Library;

namespace LayoutBXLYT.Cafe
{
    public class MAT1 : SectionCommon
    {
        public List<BxlytMaterial> Materials { get; set; }

        public MAT1()
        {
            Materials = new List<BxlytMaterial>();
        }

        public MAT1(FileReader reader, Header header) : base()
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

    //Thanks to shibbs for the material info
    //https://github.com/shibbo/flyte/blob/master/flyte/lyt/common/MAT1.cs
    public class Material : BxlytMaterial
    {
        private string name;
        [DisplayName("Name"), CategoryAttribute("General")]
        public override string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (NodeWrapper != null)
                    NodeWrapper.Text = name;
            }
        }

        public override void AddTexture(string texture)
        {
            Console.WriteLine("TextureMaps AddTexture");

            int index = ParentLayout.AddTexture(texture);
            TextureRef textureRef = new TextureRef();
            textureRef.ID = (short)index;
            textureRef.Name = texture;
            TextureMaps = TextureMaps.AddToArray(textureRef);
            TextureTransforms = TextureTransforms.AddToArray(new BxlytTextureTransform());
        }

        [DisplayName("Black Color"), CategoryAttribute("Color")]
        public override STColor8 BlackColor { get; set; }

        [DisplayName("White Color"), CategoryAttribute("Color")]
        public override STColor8 WhiteColor { get; set; }

        [DisplayName("Indirect Parameter"), CategoryAttribute("Texture")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IndirectParameter IndParameter { get; set; }

        [DisplayName("Font Shadow Parameters"), CategoryAttribute("Font")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public FontShadowParameter FontShadowParameter { get; set; }

        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        private uint flags;
        private int unknown = 134480384;

        public string GetTexture(int index)
        {
            if (TextureMaps[index].ID != -1)
                return ((Header)ParentLayout).TextureList.Textures[TextureMaps[index].ID];
            else
                return "";
        }

        public override bool RemoveTexCoordSources(int index)
        {
            foreach (var texGen in TexCoordGens)
            {
                //Shift all tex coord types down when an index is removed
                if (texGen.Source > TexGenType.TextureCoord0 &&
                    texGen.Source <= TexGenType.TextureCoord2)
                {
                    texGen.Source = texGen.Source - 1;
                }
            }
            return true;
        }

        public override BxlytMaterial Clone()
        {
            Material mat = new Material();
            return mat;
        }

        public Material()
        {
            TextureMaps = new TextureRef[0];
            TextureTransforms = new BxlytTextureTransform[0];
            TexCoordGens = new TexCoordGen[0];
            TevStages = new TevStage[0];
            ProjTexGenParams = new ProjectionTexGenParam[0];

            BlackColor = new STColor8(0, 0, 0, 0);
            WhiteColor = STColor8.White;
        }

        public Material(string name, BxlytHeader header)
        {
            ParentLayout = header;
            Name = name;
            TextureMaps = new TextureRef[0];
            TextureTransforms = new BxlytTextureTransform[0];
            TexCoordGens = new TexCoordGen[0];
            TevStages = new TevStage[0];
            ProjTexGenParams = new ProjectionTexGenParam[0];

            BlackColor = new STColor8(0, 0, 0, 0);
            WhiteColor = STColor8.White;
        }

        public Material(FileReader reader, Header header) : base()
        {
            ParentLayout = header;

            Name = reader.ReadString(0x1C, true);
            Name = Name.Replace("\x01", string.Empty);
            Name = Name.Replace("\x04", string.Empty);

            if (header.VersionMajor >= 8)
            {
                flags = reader.ReadUInt32();
                unknown = reader.ReadInt32();
                BlackColor = STColor8.FromBytes(reader.ReadBytes(4));
                WhiteColor = STColor8.FromBytes(reader.ReadBytes(4));
            }
            else
            {
                BlackColor = STColor8.FromBytes(reader.ReadBytes(4));
                WhiteColor = STColor8.FromBytes(reader.ReadBytes(4));
                flags = reader.ReadUInt32();
            }

            uint texCount = Convert.ToUInt32(flags & 3);
            uint mtxCount = Convert.ToUInt32(flags >> 2) & 3;
            uint texCoordGenCount = Convert.ToUInt32(flags >> 4) & 3;
            uint tevStageCount = Convert.ToUInt32(flags >> 6) & 0x7;
            EnableAlphaCompare = Convert.ToBoolean((flags >> 9) & 0x1);
            EnableBlend = Convert.ToBoolean((flags >> 10) & 0x1);
            var useTextureOnly = Convert.ToBoolean((flags >> 11) & 0x1);
            EnableBlendLogic = Convert.ToBoolean((flags >> 12) & 0x1);
            EnableIndParams = Convert.ToBoolean((flags >> 14) & 0x1);
            var projTexGenParamCount = Convert.ToUInt32((flags >> 15) & 0x3);
            EnableFontShadowParams = Convert.ToBoolean((flags >> 17) & 0x1);
            AlphaInterpolation = Convert.ToBoolean((flags >> 18) & 0x1);


            Console.WriteLine($"MAT1 {Name}");
            Console.WriteLine($"texCount {texCount}");
            Console.WriteLine($"mtxCount {mtxCount}");
            Console.WriteLine($"texCoordGenCount {texCoordGenCount}");
            Console.WriteLine($"tevStageCount {tevStageCount}");
            Console.WriteLine($"hasAlphaCompare {EnableAlphaCompare}");
            Console.WriteLine($"hasBlendMode {EnableBlend}");
            Console.WriteLine($"useTextureOnly {useTextureOnly}");
            Console.WriteLine($"seperateBlendMode {EnableBlendLogic}");
            Console.WriteLine($"hasIndParam {EnableIndParams}");
            Console.WriteLine($"projTexGenParamCount {projTexGenParamCount}");
            Console.WriteLine($"hasFontShadowParam {EnableFontShadowParams}");
            Console.WriteLine($"AlphaInterpolation {AlphaInterpolation}");


            TextureMaps = new TextureRef[texCount];
            TextureTransforms = new BxlytTextureTransform[mtxCount];
            TexCoordGens = new TexCoordGen[texCoordGenCount];
            TevStages = new TevStage[tevStageCount];
            ProjTexGenParams = new ProjectionTexGenParam[projTexGenParamCount];

            for (int i = 0; i < texCount; i++)
                TextureMaps[i] = new TextureRef(reader, header);

            for (int i = 0; i < mtxCount; i++)
                TextureTransforms[i] = new BxlytTextureTransform(reader);

            for (int i = 0; i < texCoordGenCount; i++)
                TexCoordGens[i] = new TexCoordGen(reader, header);

            for (int i = 0; i < tevStageCount; i++)
                TevStages[i] = new TevStage(reader, header);

            if (EnableAlphaCompare)
                AlphaCompare = new BxlytAlphaCompare(reader, header);
            if (EnableBlend)
                BlendMode = new BxlytBlendMode(reader, header);
            if (EnableBlendLogic)
                BlendModeLogic = new BxlytBlendMode(reader, header);
            if (EnableIndParams)
                IndParameter = new IndirectParameter(reader, header);

            for (int i = 0; i < projTexGenParamCount; i++)
                ProjTexGenParams[i] = new ProjectionTexGenParam(reader, header);

            if (EnableFontShadowParams)
                FontShadowParameter = new FontShadowParameter(reader, header);
        }

        public void Write(FileWriter writer, LayoutHeader header)
        {
            Console.WriteLine(ToJson());

            long flagPos = 0;
            writer.WriteString(Name, 0x1C);
            if (header.VersionMajor >= 8)
            {
                flagPos = writer.Position;
                writer.Write(flags);
                writer.Write(unknown);
                writer.Write(BlackColor);
                writer.Write(WhiteColor);
            }
            else
            {
                writer.Write(BlackColor);
                writer.Write(WhiteColor);

                flagPos = writer.Position;
                writer.Write(flags);
            }

            //TODO. I don't know why but many cases there needs to be 2 tex coords to match up
            if (TextureMaps?.Length > 0 && TexCoordGens?.Length == 0)
            {
                TexCoordGens = new BxlytTexCoordGen[TextureMaps.Length];
                for (int i = 0; i < TextureMaps?.Length; i++)
                    TexCoordGens[i] = new TexCoordGen();
            }

            flags = 0;
            for (int i = 0; i < TextureMaps.Length; i++)
            {
                flags += Bit.BitInsert(1, 1, 2, 30);
                ((TextureRef)TextureMaps[i]).Write(writer);
            }

            if (TextureTransforms.Length == 1)
            {
                TextureTransforms = new BxlytTextureTransform[2]
                {
                    TextureTransforms[0],
                    new BxlytTextureTransform(),
                };
            }

            for (int i = 0; i < TextureTransforms.Length; i++)
            {
                flags += Bit.BitInsert(1, 1, 2, 28);
                ((BxlytTextureTransform)TextureTransforms[i]).Write(writer);
            }

            for (int i = 0; i < TexCoordGens.Length; i++)
            {
                flags += Bit.BitInsert(1, 1, 2, 26);
                ((TexCoordGen)TexCoordGens[i]).Write(writer, (BxlytHeader)header);
            }

            for (int i = 0; i < TevStages.Length; i++)
            {
                flags += Bit.BitInsert(1, 1, 2, 24);
                ((TevStage)TevStages[i]).Write(writer);
            }
            if (AlphaCompare != null && EnableAlphaCompare)
            {
                flags += Bit.BitInsert(1, 1, 1, 22);
                AlphaCompare.Write(writer);
            }
            if (BlendMode != null && EnableBlend)
            {
                flags += Bit.BitInsert(1, 1, 2, 20);
                BlendMode.Write(writer);
            }
            if (BlendModeLogic != null && EnableBlendLogic)
            {
                flags += Bit.BitInsert(1, 1, 2, 18);
                BlendModeLogic.Write(writer);
            }
            if (IndParameter != null && EnableIndParams)
            {
                flags += Bit.BitInsert(1, 1, 1, 17);
                IndParameter.Write(writer);
            }

            for (int i = 0; i < ProjTexGenParams.Length; i++)
            {
                flags += Bit.BitInsert(1, 1, 2, 15);
                ProjTexGenParams[i].Write(writer);
            }

            if (FontShadowParameter != null && EnableFontShadowParams)
            {
                flags += Bit.BitInsert(1, 1, 1, 14);
                FontShadowParameter.Write(writer);
            }

            if (AlphaInterpolation)
                flags += Bit.BitInsert(1, 1, 1, 13);

            using (writer.TemporarySeek(flagPos, SeekOrigin.Begin))
            {
                writer.Write(flags);
            }
        }
    }
}
