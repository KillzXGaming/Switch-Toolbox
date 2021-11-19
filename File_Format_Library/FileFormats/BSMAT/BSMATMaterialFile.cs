using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Toolbox.Library.IO;
using Toolbox.Library;

namespace MetroidDreadLibrary
{
    public class BSMATMaterialFile
    {
        const int TYPE_ID = 1114114;

        public string Name { get; set; }
        public string ShaderPath { get; set; }

        public TranslucencyType TranslucencyState { get; set; }
        public uint RenderLayer { get; set; }

        public BlendState Blending { get; set; }
        public PolygonState PolygonInfo { get; set; }
        public StencilState Stencil { get; set; }
        public AlphaState AlphaTest { get; set; }
        public DepthState DepthTest { get; set; }
        public FillMode PolyFillMode { get; set; }

        public ShaderStage[] ShaderStages { get; set; }

        public BSMATMaterialFile() { }

        public BSMATMaterialFile(Stream stream)
        {
            using (var reader = new FileReader(stream)) {
                Read(reader);
            }
        }

        public void Save(Stream stream)
        {
            using (var writer = new FileWriter(stream)) {
                Write(writer);
            }
        }

        public void Read(FileReader reader)
        {
            reader.ReadSignature(4, "MSUR");
            reader.ReadInt32(); //type
            Name = reader.ReadZeroTerminatedString();
            TranslucencyState = (TranslucencyType)reader.ReadUInt32();
            RenderLayer = reader.ReadUInt32();
            ShaderPath = reader.ReadZeroTerminatedString();

            Blending = reader.ReadStruct<BlendState>();
            PolygonInfo = reader.ReadStruct<PolygonState>();
            Stencil = reader.ReadStruct<StencilState>();
            AlphaTest = reader.ReadStruct<AlphaState>();
            PolyFillMode = reader.ReadEnum<FillMode>(false);
            DepthTest = reader.ReadStruct<DepthState>();
            reader.ReadBytes(4);

            uint numStages = reader.ReadUInt32();

            ShaderStages = new ShaderStage[numStages];
            for (int i = 0; i < numStages; i++)
                ShaderStages[i] = new ShaderStage(reader);
        }

        public void Export(string filePath) {
            File.WriteAllText(filePath, ToJson());
        }

        public string ToJson() {
            var conv = new Newtonsoft.Json.Converters.StringEnumConverter();
            return JsonConvert.SerializeObject(this, Formatting.Indented, conv);
        }

        public static BSMATMaterialFile ImportFromFile(string filePath) {
            return Import(File.ReadAllText(filePath));
        }

        public static BSMATMaterialFile Import(string text)
        {
            var conv = new Newtonsoft.Json.Converters.StringEnumConverter();
            var bsmat = JsonConvert.DeserializeObject<BSMATMaterialFile>(text, conv);
            foreach (var stage in bsmat.ShaderStages)
            {
                List< UniformParam > uniformList = new List<UniformParam>();
                foreach (var uniform in stage.UniformData)
                {
                    var uniformParam = new UniformParam();
                    uniformList.Add(uniformParam);

                    uniformParam.Name = uniform.Key;

                    //Basic type define via the start of uniform name.
                    //All params have starting characters (f float, i int, v vec, etc)
                    uniformParam.DataType = ParamType.Float;
                    if (uniform.Key.StartsWith("i"))
                        uniformParam.DataType = ParamType.Int;

                    //Turn the data into normal array objects
                    if (uniformParam.DataType == ParamType.Float)
                        uniformParam.Data = ((Newtonsoft.Json.Linq.JArray)uniform.Value).ToObject<float[]>();
                    if (uniformParam.DataType == ParamType.Int)
                        uniformParam.Data = ((Newtonsoft.Json.Linq.JArray)uniform.Value).ToObject<int[]>();

                }
                stage.Uniforms = uniformList.ToArray();
            }
            return bsmat;
        }

        public void Write(FileWriter writer)
        {
            writer.WriteSignature("MSUR");
            writer.Write(TYPE_ID);
            writer.WriteString(Name);
            writer.Write((uint)TranslucencyState);
            writer.Write(RenderLayer);
            writer.WriteString(ShaderPath);
            writer.WriteStruct(Blending);
            writer.WriteStruct(PolygonInfo);
            writer.WriteStruct(Stencil);
            writer.WriteStruct(AlphaTest);
            writer.Write((uint)PolyFillMode);
            writer.WriteStruct(DepthTest);
            writer.Write((uint)0);
            writer.Write(ShaderStages.Length);
            foreach (var stage in ShaderStages)
                stage.Write(writer);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class BlendState
        {
            public byte Enabled;
            public BlendOp BlendOperation;
            public BlendMode SrcBlend;
            public BlendMode DstBlend;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PolygonState
        {
            public CullMode CullMode;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class StencilState
        {
            public byte Enabled;
            public uint Mask;
            public uint Ref;
            public StencilOp StencilFail;
            public StencilOp StencilPass;
            public StencilOp DepthFail;
            public StencilOp DepthPass;
            public CompareMode CompareMode;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class AlphaState
        {
            public byte EnableAlphaTest;
            public CompareMode CompareMode;
            public float Ref;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class DepthState
        {
            public byte EnableDepthTest;
            public byte EnableDepthWrite;
            public CompareMode CompareMode;
            public byte zPrePass;
        }

        public class ShaderStage
        {
            //Better json display for params
            [JsonProperty(ItemConverterType = typeof(NoFormattingConverter))]
            public Dictionary<string, object> UniformData = new Dictionary<string, object>();

            [JsonIgnore]
            public UniformParam[] Uniforms;

            public SamplerParam[] Samplers;

            public ShaderType Type;

            public ShaderStage() { }

            public ShaderStage(FileReader reader)
            {
                Type = (ShaderType)reader.ReadUInt32();

                uint numParams = reader.ReadUInt32();
                Uniforms = new UniformParam[numParams];
                for (int i = 0; i < numParams; i++)
                    Uniforms[i] = new UniformParam(reader);

                uint numSamplers = reader.ReadUInt32();
                Samplers = new SamplerParam[numSamplers];
                for (int i = 0; i < numSamplers; i++)
                    Samplers[i] = new SamplerParam(reader);

                foreach (var uniform in Uniforms)
                    UniformData.Add(uniform.Name, uniform.Data);
            }

            public void Write(FileWriter writer)
            {
                writer.Write((uint)Type);
                writer.Write(Uniforms.Length);
                foreach (var uniform in Uniforms)
                    uniform.Write(writer);

                writer.Write(Samplers.Length);
                foreach (var sampler in Samplers)
                    sampler.Write(writer);
            }

            public enum ShaderType
            {
                Vertex,
                Fragment,
            }
        }

        public class UniformParam
        {
            public object Data { get; set; }

            [JsonIgnore]
            public string Name { get; set; }

            [JsonIgnore]
            public ParamType DataType { get; set; }

            Dictionary<char, ParamType> FormatList = new Dictionary<char, ParamType>()
            {
                { 'f', ParamType.Float },
                { 'i', ParamType.Int },
                { 'u', ParamType.Uint },
            };

            public UniformParam() { }

            public UniformParam(FileReader reader)
            {
                Name = reader.ReadZeroTerminatedString();
                byte format = reader.ReadByte();
                DataType = FormatList[(char)format];

                int count = reader.ReadInt32();
                switch (DataType)
                {
                    case ParamType.Float: Data = reader.ReadSingles(count); break;
                    case ParamType.Uint: Data = reader.ReadUInt32s(count); break;
                    case ParamType.Int: Data = reader.ReadInt32s(count); break;
                }
            }

            public void Write(FileWriter writer)
            {
                var format = FormatList.FirstOrDefault(x => x.Value == this.DataType).Key;

                writer.WriteString(Name);
                writer.Write((byte)format);
                writer.Write(((Array)Data).Length);
                switch (DataType)
                {
                    case ParamType.Float: writer.Write((float[])Data); break;
                    case ParamType.Uint: writer.Write((uint[])Data); break;
                    case ParamType.Int: writer.Write((int[])Data); break;
                }
            }
        }

        public class SamplerParam
        {
            public string Name { get; set; }
            public string Sampler { get; set; }
            public string Type { get; set; }
            public uint SlotID { get; set; }
            public string FilePath { get; set; }
            public uint[] Parameters { get; set; }

            public FilterMode MinFilter;
            public FilterMode MagFilter;
            public FilterMode MipFilter;
            public CompareMode CompMode;
            public TileWrapMode WrapModeU;
            public TileWrapMode WrapModeV;
            public uint uBorderColor = 0xFFFFFF0F;
            public float fMinLod;
            public float fLodBias;
            public float fAnisotropic;
            public float fMaxMipLevel;
            public float fMaxAnisotropy;

            public SamplerParam() { }

            public SamplerParam(FileReader reader)
            {
                Name = reader.ReadZeroTerminatedString();
                Sampler = reader.ReadZeroTerminatedString();
                Type = reader.ReadZeroTerminatedString();
                SlotID = reader.ReadUInt32();
                FilePath = reader.ReadZeroTerminatedString();
                MinFilter = (FilterMode)reader.ReadUInt32();
                MagFilter = (FilterMode)reader.ReadUInt32();
                MipFilter = (FilterMode)reader.ReadUInt32();
                CompMode = (CompareMode)reader.ReadUInt32();
                WrapModeU = (TileWrapMode)reader.ReadUInt32();
                WrapModeV = (TileWrapMode)reader.ReadUInt32();
                uBorderColor = reader.ReadUInt32();
                fMinLod = reader.ReadSingle();
                fLodBias = reader.ReadSingle();
                fAnisotropic = reader.ReadSingle();
                fMaxMipLevel = reader.ReadSingle();
                fMaxAnisotropy = reader.ReadSingle();
            }

            public void Write(FileWriter writer)
            {
                writer.WriteString(Name);
                writer.WriteString(Sampler);
                writer.WriteString(Type);
                writer.Write(SlotID);
                writer.WriteString(FilePath);
                writer.Write((uint)MinFilter);
                writer.Write((uint)MagFilter);
                writer.Write((uint)MipFilter);
                writer.Write((uint)CompMode);
                writer.Write((uint)WrapModeU);
                writer.Write((uint)WrapModeV);
                writer.Write(uBorderColor);
                writer.Write(fMinLod);
                writer.Write(fLodBias);
                writer.Write(fAnisotropic);
                writer.Write(fMaxMipLevel);
                writer.Write(fMaxAnisotropy);
            }
        }

        public enum ParamType
        {
            Float,
            Uint,
            Int,
        }

        public enum FillMode
        {
            SOLID = 0x0,
            WIRE = 0x1,
        }

        public enum ShaderType
        {
            VERTEX = 0x0,
            PIXEL = 0x1,
            GEOMETRY = 0x2,
        }

        public enum TileWrapMode
        {
            CLAMP = 0x0,
            CLAMPCOLOR = 0x1,
            REPEAT = 0x2,
            MIRROR = 0x3,
        }

        public enum FilterMode
        {
            NEAREST = 0x0,
            LINEAR = 0x1,
            NEARESTMIPNEAREST = 0x2,
            NEARESTMIPLINEAR = 0x3,
            LINEARMIPNEAREST = 0x4,
            LINEARMIPLINEAR = 0x5,
        }

        public enum CompareMode
        {
            ALWAYS = 0x0,
            NEVER = 0x1,
            EQUAL = 0x2,
            NOTEQUAL = 0x3,
            LESS = 0x4,
            LESSEQUAL = 0x5,
            GREATER = 0x6,
            GREATEREQUAL = 0x7,
        }

        public enum StencilOp
        {
            KEEP = 0x0,
            ZERO = 0x1,
            REPLACE = 0x2,
            INCRSAT = 0x3,
            DECRSAT = 0x4,
            INVERT = 0x5,
            INCR = 0x6,
            DECR = 0x7,
        }

        public enum BlendMode
        {
            ZERO = 0x0,
            ONE = 0x1,
            SRC_COLOR = 0x2,
            INV_SRC_COLOR = 0x3,
            DST_COLOR = 0x4,
            INV_DST_COLOR = 0x5,
            SRC_ALPHA = 0x6,
            INV_SRC_ALPHA = 0x7,
            DST_ALPHA = 0x8,
            INV_DST_ALPHA = 0x9,
        }

        public enum BlendOp
        {
            ADD = 0x0,
            SUBDST = 0x1,
            SUBSRC = 0x2,
            MIN = 0x3,
            MAX = 0x4,
        }

        public enum CullMode
        {
            FRONT = 0x2,
            BACK = 0x3,
            NONE = 0x4,
        }

        public enum TranslucencyType
        {
            NONE = 0x0,
            OPAQUE = 0x1,
            TRANSLUCENT = 0x2,
            SUBSTRACTIVE = 0x4,
            ADDITIVE = 0x8,
            OPAQUE_FWD = 0x10,
            ALL = 0x1f,
            NOT_OPAQUE = 0xe,
        }
    }
}
