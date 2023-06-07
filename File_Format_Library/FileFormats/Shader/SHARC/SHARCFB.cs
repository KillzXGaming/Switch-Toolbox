using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstPlugin.Forms;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class SHARCFB : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Shader;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Binary Shader Archive" };
        public string[] Extension { get; set; } = new string[] { "*.sharcfb" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "BAHS");
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public Header header;

        public void Load(System.IO.Stream stream)
        {
            header = new Header();
            header.Read(new FileReader(stream));

            this.Text = header.Name;

            Nodes.Add("BinaryData");
            Nodes.Add("ShaderPrograms");
            Nodes[0].Nodes.Add("Vertex Shaders");
            Nodes[0].Nodes.Add("Fragment Shaders");
            Nodes[0].Nodes.Add("Geometry Shaders");

            int v = 0;
            int p = 0;
            int g = 0;
            foreach (BinaryData item in header.BinaryDatas)
            {
                if (item.Type == BinaryData.ShaderType.GX2VertexShader)
                {
                    item.Text += $"Binary [{v++}]";
                    Nodes[0].Nodes[0].Nodes.Add(item);
                }
                if (item.Type == BinaryData.ShaderType.GX2PixelShader)
                {
                    item.Text += $"Binary [{p++}]";
                    Nodes[0].Nodes[1].Nodes.Add(item);
                }
                if (item.Type == BinaryData.ShaderType.GX2GeometryShader)
                {
                    item.Text += $"Binary [{g++}]";
                    Nodes[0].Nodes[2].Nodes.Add(item);
                }
            }

            if (header.sharcNX != null)
            {
                foreach (var item in header.sharcNX.header.Variations)
                {
                    if (item.Type == SHARCFBNX.ShaderVariation.ShaderType.Vertex)
                        Nodes[0].Nodes[0].Nodes.Add(item);
                    else
                        Nodes[0].Nodes[1].Nodes.Add(item);
                }
                foreach (var item in header.sharcNX.header.ShaderPrograms)
                {
                    Nodes[1].Nodes.Add(item);
                }
            }
            else
            {
                foreach (ShaderProgram item in header.ShaderPrograms)
                {
                    Nodes[1].Nodes.Add(item);
                }
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        //Docs https://github.com/Kinnay/Wii-U-File-Formats/wiki/SHARCFB-File-Format
        public class Header
        {
            public string Name;
            public uint Version;
            public List<BinaryData> BinaryDatas = new List<BinaryData>();
            public List<ShaderProgram> ShaderPrograms = new List<ShaderProgram>();

            public bool IsNX = false;
            public SHARCFBNX sharcNX;

            public void Read(FileReader reader)
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.ReadSignature(4, "BAHS");
                uint Version = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt32();
                uint ByteOrderMark = reader.ReadUInt32();

                if (ByteOrderMark == 1)
                    reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                else
                    reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                uint unk = reader.ReadUInt32();
                uint NameLength = reader.ReadUInt32();

                if (NameLength == 4096 || NameLength == 8192)
                {
                    IsNX = true;
                    sharcNX = new SHARCFBNX();
                    sharcNX.header = new SHARCFBNX.Header();
                    sharcNX.header.Read(reader);
                    return;
                }

                Name = reader.ReadString((int)NameLength);

                var pos = reader.Position;
                uint BinarySectionSize = reader.ReadUInt32();
                uint BinaryFileCount = reader.ReadUInt32();

                for (int i = 0; i < BinaryFileCount; i++)
                {
                    BinaryData binary = new BinaryData();
                    binary.Read(reader);
                    BinaryDatas.Add(binary);
                }
                reader.Seek(BinarySectionSize + pos, System.IO.SeekOrigin.Begin);
                pos = reader.Position;
                uint ProgramSectionSize = reader.ReadUInt32();
                uint ProgramCount = reader.ReadUInt32();

                for (int i = 0; i < ProgramCount; i++)
                {
                    ShaderProgram program = new ShaderProgram();
                    program.Read(reader);
                    ShaderPrograms.Add(program);
                }
            }
            public void Write(FileWriter reader)
            {

            }
        }

        public class ShaderProgram : TreeNodeCustom
        {
            public enum ShaderType
            {
                GX2VertexShader = 0x1,
                GX2PixelShader = 0x2,
                GX2ComputeShader = 0x3,
                GX2GeometryShader = 0x4,
            }
            public ShaderType Type;

            public VariationSymbolData variationMacroData;
            public VariationSymbolData variationSymbolData;
            public ShaderSymbolData UniformVariables;
            public ShaderSymbolData UniformBlocks;
            public ShaderSymbolData SamplerVariables;
            public ShaderSymbolData AttributeVariables;

            public override void OnClick(TreeView treeview)
            {
                ShaderEditor editor = (ShaderEditor)LibraryGUI.GetActiveContent(typeof(ShaderEditor));
                if (editor == null)
                {
                    editor = new ShaderEditor();
                    LibraryGUI.LoadEditor(editor);
                }

                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.FillEditor(this, ((SHARCFB)Parent.Parent).header);
            }

            public void Read(FileReader reader)
            {
                var pos = reader.Position;

                uint SectionSize = reader.ReadUInt32();
                uint NameLength = reader.ReadUInt32();

                Type = reader.ReadEnum<ShaderType>(false);
                uint index = reader.ReadUInt32();

                Text = reader.ReadString((int)NameLength);

                variationMacroData = new VariationSymbolData();
                variationSymbolData = new VariationSymbolData();
                UniformVariables = new ShaderSymbolData();
                UniformBlocks = new ShaderSymbolData();
                SamplerVariables = new ShaderSymbolData();
                AttributeVariables = new ShaderSymbolData();

                variationMacroData.Read(reader);
                variationSymbolData.Read(reader);
                UniformVariables.Read(reader);
                UniformBlocks.Read(reader);
                SamplerVariables.Read(reader);
                AttributeVariables.Read(reader);

                reader.Seek(SectionSize + pos, System.IO.SeekOrigin.Begin);
            }
        }

        public class BinaryData : TreeNodeCustom
        {
            public enum ShaderType
            {
                GX2VertexShader,
                GX2PixelShader,
                GX2GeometryShader,
            }

            public ShaderType Type;
            public byte[] Data;

            public override void OnClick(TreeView treeview)
            {
                HexEditor editor = (HexEditor)LibraryGUI.GetActiveContent(typeof(HexEditor));
                if (editor == null)
                {
                    editor = new HexEditor();
                    LibraryGUI.LoadEditor(editor);
                }

                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadData(Data);
            }

            private void ExportShader(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "bin";
                sfd.FileName = "shader";
                sfd.Filter = "Supported Formats|*.bin;";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllBytes(sfd.FileName, Data);
                }
            }

            public void Read(FileReader reader)
            {
                var pos = reader.Position;

                uint SectionSize = reader.ReadUInt32();
                Type = reader.ReadEnum<ShaderType>(true);
                uint Offset = reader.ReadUInt32();
                uint BinarySize = reader.ReadUInt32();
                Data = reader.ReadBytes((int)BinarySize);

                reader.Seek(SectionSize + pos, System.IO.SeekOrigin.Begin);

                ContextMenu = new ContextMenu();
                ContextMenu.MenuItems.Add(new MenuItem("Export Shader", ExportShader));
            }

            public void Write(FileWriter reader)
            {

            }
        }

    }

    public class VariationMacroData
    {
        public List<VariationMacro> macros = new List<VariationMacro>();
        public void Read(FileReader reader, uint Version  = 12)
        {
            var SectionPos = reader.Position;
            uint SectionSize = reader.ReadUInt32();
            uint SectionCount = reader.ReadUInt32();

            for (int i = 0; i < SectionCount; i++)
            {
                VariationMacro variation = new VariationMacro();
                variation.Read(reader, Version);
                macros.Add(variation);
            }
            reader.Seek(SectionPos + SectionSize, System.IO.SeekOrigin.Begin);
        }

        public void Write(FileWriter writer, uint Version = 12)
        {
            long pos = writer.Position;
            writer.Write(uint.MaxValue);
            writer.Write(macros.Count);

            for (int i = 0; i < macros.Count; i++)
                macros[i].Write(writer, Version);

            SharcCommon.WriteSectionSize(writer, pos);
        }
    }

    public class VariationMacro
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public void Read(FileReader reader, uint Version)
        {
            var pos = reader.Position;
            uint SectionSize = reader.ReadUInt32();

            uint NameLength = reader.ReadUInt32();
            uint ValueLength = reader.ReadUInt32();
            Name = reader.ReadString((int)NameLength);
            Value = reader.ReadString((int)ValueLength);


            Console.WriteLine("VariationMacro ------------------");
            Console.WriteLine(Name);
            Console.WriteLine(Value);
            Console.WriteLine("------------------");


            reader.Seek(pos + SectionSize, System.IO.SeekOrigin.Begin);
        }

        public void Write(FileWriter writer, uint Version)
        {
            var pos = writer.Position;
            writer.Write(uint.MaxValue);
            writer.Write(Name.Length);
            writer.Write(Value.Length);
            writer.WriteString(Name);
            writer.WriteString(Value);
            SharcCommon.WriteSectionSize(writer, pos);
        }
    }

    public class VariationSymbolData
    {
        public List<VariationSymbol> symbols = new List<VariationSymbol>();
        public void Read(FileReader reader)
        {
            var SectionPos = reader.Position;
            uint SectionSize = reader.ReadUInt32();
            uint SectionCount = reader.ReadUInt32();

            for (int i = 0; i < SectionCount; i++)
            {
                VariationSymbol symbol = new VariationSymbol();
                symbol.Read(reader);
                symbols.Add(symbol);
            }
            reader.Seek(SectionPos + SectionSize, System.IO.SeekOrigin.Begin);
        }

        public void Write(FileWriter writer)
        {
            var pos = writer.Position;
            writer.Write(uint.MaxValue);
            writer.Write(symbols.Count);
            for (int i = 0; i < symbols.Count; i++)
                symbols[i].Write(writer);

            SharcCommon.WriteSectionSize(writer, pos);
        }
    }
    public class VariationSymbol
    {
        public string Name { get; set; }
        public List<string> Values { get; set; }
        public string SymbolName { get; set; }

        public void Read(FileReader reader)
        {
            var pos = reader.Position;
            uint SectionSize = reader.ReadUInt32();
            uint macroNameLength = reader.ReadUInt32();
            uint valueLength = reader.ReadUInt32();
            uint symbolNameLength = reader.ReadUInt32();

            Name = reader.ReadString((int)macroNameLength, true);
            Values = reader.ReadStrings((int)valueLength, Syroot.BinaryData.BinaryStringFormat.ZeroTerminated, Encoding.UTF8).ToList();
            SymbolName = reader.ReadString((int)symbolNameLength, true);
            reader.Seek(pos + SectionSize, System.IO.SeekOrigin.Begin);

            reader.Seek(pos + SectionSize, System.IO.SeekOrigin.Begin);
        }

        public void Write(FileWriter writer)
        {
            var pos = writer.Position;
            writer.Write(Name.Length + 1);
            writer.Write(Values.Count + 1);
            writer.Write(SymbolName.Length + 1);
            writer.WriteString(Name);
            for (int i = 0; i < Values.Count; i++)
                writer.WriteString(Values[i]);
            writer.WriteString(SymbolName);
            SharcCommon.WriteSectionSize(writer, pos);
        }
    }

    public class ShaderSymbolData
    {
        public List<ShaderSymbol> symbols = new List<ShaderSymbol>();
        public void Read(FileReader reader, uint Version = 12, bool SkipReading = false)
        {
            var SectionPos = reader.Position;
            uint SectionSize = reader.ReadUInt32();
            uint SectionCount = reader.ReadUInt32();

            if (!SkipReading)
            {
                for (int i = 0; i < SectionCount; i++)
                {
                    ShaderSymbol symbol = new ShaderSymbol();
                    symbol.Read(reader, Version);
                    symbols.Add(symbol);
                }
            }

            reader.Seek(SectionPos + SectionSize, System.IO.SeekOrigin.Begin);
        }

        public void Write(FileWriter writer, uint Version = 12)
        {
            var pos = writer.Position;
            writer.Write(uint.MaxValue);
            writer.Write(symbols.Count);
            for (int i = 0; i < symbols.Count; i++)
                symbols[i].Write(writer);

            SharcCommon.WriteSectionSize(writer, pos);
        }
    }

    public class ShaderSymbol
    {
        public string Name { get; set; }
        public byte[] DefaultValue { get; set; }
        public string SymbolName { get; set; }
        public byte[] flags;
        public string flagData { get; set; }

        public List<SharcNXValue> sharcNXValues = new List<SharcNXValue>();

        public string DefaultValueString
        {
            get
            {
                return DefaultValueToString();
            }
        }

        private string DefaultValueToString()
        {
            if (DefaultValue != null)
            {
                using (var reader = new FileReader(DefaultValue))
                {
                    if (DefaultValue.Length == 32)
                    {
                        float[] values = reader.ReadSingles(4);
                        return $"{values[0]},{values[1]},{values[2]},{values[3]}" +
                               $"{values[4]},{values[5]},{values[6]},{values[7]}";
                    }
                    if (DefaultValue.Length == 16)
                    {
                        float[] values = reader.ReadSingles(4);
                        return $"{values[0]},{values[1]},{values[2]},{values[3]}";
                    }
                    if (DefaultValue.Length == 12)
                    {
                        float[] values = reader.ReadSingles(3);
                        return $"{values[0]},{values[1]},{values[2]}";
                    }
                    if (DefaultValue.Length == 8)
                    {
                        float[] values = reader.ReadSingles(3);
                        return $"{values[0]},{values[1]}";
                    }
                    if (DefaultValue.Length == 4)
                    {
                        float[] values = reader.ReadSingles(3);
                        return $"{values[0]}}}";
                    }

                    return reader.ReadString(DefaultValue.Length);
                }
            }
            else
            {
                return "";
            }
        }

        public void Read(FileReader reader, uint Version)
        {
            var pos = reader.Position;
            uint SectionSize = reader.ReadUInt32();

            if (Version >= 13)
            {
                uint shaderVariableSize = reader.ReadUInt32();
                uint variationNameLength = reader.ReadUInt32();
                Name = reader.ReadString((int)variationNameLength);
                uint ValueSectionSize = reader.ReadUInt32();
                uint ValueCount = reader.ReadUInt32();

                for (int i = 0; i < ValueCount; i++)
                {
                    SharcNXValue value = new SharcNXValue();
                    value.Read(reader);
                    sharcNXValues.Add(value);
                }
            }
            else
            {
                uint shaderVariableSize = reader.ReadUInt32();
                uint variationNameLength = reader.ReadUInt32();
                uint symbolNameLength = reader.ReadUInt32();
                uint defaultValueLength = reader.ReadUInt32();
                uint variationCount = reader.ReadUInt32();

                Name = reader.ReadString((int)variationNameLength);
                SymbolName = reader.ReadString((int)symbolNameLength);
                DefaultValue = reader.ReadBytes((int)defaultValueLength);

                Console.WriteLine("----------------SHADER SYMBOL -----------");
                Console.WriteLine(SectionSize);
                Console.WriteLine(Name);
                Console.WriteLine(SymbolName);
                Console.WriteLine(DefaultValue);
                Console.WriteLine("-----------------------------------------");

                flags = new byte[variationCount];
                for (int i = 0; i < variationCount; i++)
                {
                    //  flags[i] = reader.ReadByte();

                    // flagData += " " + flags[i];
                }
            }

        
            reader.Seek(pos + SectionSize, System.IO.SeekOrigin.Begin);
        }

        public void Write(FileWriter writer)
        {
            var pos = writer.Position;
            writer.Write(0);
            writer.Write(Name.Length);
            writer.Write(SymbolName.Length);
            writer.Write(DefaultValue.Length);
            writer.Write(0);
            writer.WriteString(Name);
            writer.Write(DefaultValue);
            writer.WriteString(SymbolName);
            SharcCommon.WriteSectionSize(writer, pos);
        }

        public class SharcNXValue
        {
            public string Name { get; set; }

            public void Read(FileReader reader)
            {
                var pos = reader.Position;

                uint dataSize = reader.ReadUInt32();
                uint unk = reader.ReadUInt32();
                uint stringLength = reader.ReadUInt32();
                Name = reader.ReadString((int)stringLength);

                reader.Seek(pos + dataSize, System.IO.SeekOrigin.Begin);
            }
        }
    }
}