using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;
using System.ComponentModel;

namespace LayoutBXLYT.CTR
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
        public STColor8[] TevConstantColors { get; set; }

        [DisplayName("Texture Coordinate Params"), CategoryAttribute("Texture")]
        public TexCoordGen[] TexCoordGens { get; set; }

        [DisplayName("Indirect Parameter"), CategoryAttribute("Texture")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IndirectParameter IndParameter { get; set; }

        [DisplayName("Projection Texture Coord Parameters"), CategoryAttribute("Texture")]
        public ProjectionTexGenParam[] ProjTexGenParams { get; set; }

        [DisplayName("Font Shadow Parameters"), CategoryAttribute("Font")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public FontShadowParameter FontShadowParameter { get; set; }

        private uint flags;

        public string GetTexture(int index)
        {
            if (TextureMaps[index].ID != -1)
                return ParentLayout.Textures[TextureMaps[index].ID];
            else
                return "";
        }

        public Material()
        {
            TextureMaps = new TextureRef[0];
            TextureTransforms = new BxlytTextureTransform[0];
            TextureMaps = new TextureRef[0];
            TextureTransforms = new BxlytTextureTransform[0];
            ProjTexGenParams = new ProjectionTexGenParam[0];
            TevStages = new TevStage[0];
            TexCoordGens = new TexCoordGen[0];
         
            TevConstantColors = new STColor8[5];
            for (int i = 0; i < 5; i++)
                TevConstantColors[i] = STColor8.White;
            BlackColor = new STColor8(0, 0, 0, 0);
            WhiteColor = STColor8.White;
        }


        public Material(string name, BxlytHeader header)
        {
            ParentLayout = header;
            Name = name;
            TextureMaps = new TextureRef[0];
            TextureTransforms = new BxlytTextureTransform[0];
            ProjTexGenParams = new ProjectionTexGenParam[0];
            TevStages = new TevStage[0];
            TexCoordGens = new TexCoordGen[0];

            TevConstantColors = new STColor8[5];
            for (int i = 0; i < 5; i++)
                TevConstantColors[i] = STColor8.White;
            BlackColor = new STColor8(0, 0, 0, 0);
            WhiteColor = STColor8.White;
        }

        public Material(FileReader reader, BxlytHeader header) : base()
        {
            ParentLayout = header;

            Name = reader.ReadString(20, true);
            BlackColor = reader.ReadColor8RGBA();
            WhiteColor = reader.ReadColor8RGBA();
            TevConstantColors = reader.ReadColor8sRGBA(5);
            flags = reader.ReadUInt32();

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
            writer.WriteString(Name, 20);
            writer.Write(BlackColor);
            writer.Write(WhiteColor);
            writer.Write(TevConstantColors);
            long flagPos = writer.Position;
            writer.Write(flags);

            flags = 0;
            for (int i = 0; i < TextureMaps?.Length; i++)
            {
                flags += Bit.BitInsert(1, 1, 2, 30);
                ((TextureRef)TextureMaps[i]).Write(writer);
            }

            for (int i = 0; i < TextureTransforms?.Length; i++)
            {
                flags += Bit.BitInsert(1, 1, 2, 28);
                ((BxlytTextureTransform)TextureTransforms[i]).Write(writer);
            }

            for (int i = 0; i < TexCoordGens?.Length; i++)
            {
                flags += Bit.BitInsert(1, 1, 2, 26);
                TexCoordGens[i].Write(writer);
            }

            for (int i = 0; i < TevStages?.Length; i++)
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

            using (writer.TemporarySeek(flagPos, System.IO.SeekOrigin.Begin)) {
                writer.Write(flags);
            }
        }
    }
}
