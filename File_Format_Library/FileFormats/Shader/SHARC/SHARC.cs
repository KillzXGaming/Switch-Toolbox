using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using FirstPlugin.Forms;

namespace FirstPlugin
{
    public class SHARC : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Shader;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Shader Archive" };
        public string[] Extension { get; set; } = new string[] { "*.sharc" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "AAHS");
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
            CanSave = true;

            header = new Header();
            header.Read(new FileReader(stream));

            this.Text = header.Name;

            Nodes.Add("SourceData");
            Nodes.Add("ShaderPrograms");

            foreach (var item in header.SourceDatas)
            {
                Nodes[0].Nodes.Add(item);
            }
            foreach (var item in header.ShaderPrograms)
            {
                Nodes[1].Nodes.Add(item);
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream) {
            header.Write(new FileWriter(stream));
        }

        //Docs https://github.com/Kinnay/Wii-U-File-Formats/wiki/SHARCFB-File-Format
        public class Header
        {
            public string Name;
            public uint Version;
            public List<SourceData> SourceDatas = new List<SourceData>();
            public List<ShaderProgram> ShaderPrograms = new List<ShaderProgram>();

            public void Read (FileReader reader)
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                reader.ReadSignature(4, "AAHS");
                Version = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt32();
                uint bom = reader.ReadUInt32();
                uint FileLength = reader.ReadUInt32();
                Name = reader.ReadString((int)FileLength, true);


                var pos = reader.Position;
                uint SourceArrayOffset = reader.ReadUInt32();
                uint ProgramCount = reader.ReadUInt32();

                for (int i = 0; i < ProgramCount; i++)
                {
                    ShaderProgram program = new ShaderProgram();
                    program.Read(reader, this);
                    ShaderPrograms.Add(program);
                }

                reader.Seek(SourceArrayOffset + pos, System.IO.SeekOrigin.Begin);

                uint SourceSecSize = reader.ReadUInt32(); //Seems to cover that section entirely
                uint SourceFileCount = reader.ReadUInt32();

                for (int i = 0; i < SourceFileCount; i++)
                {
                    SourceData source = new SourceData();
                    source.Read(reader);
                    SourceDatas.Add(source);
                }
            }

            public void Write(FileWriter writer)
            {
                writer.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                writer.WriteSignature("AAHS");
                writer.Write(Version);
                writer.Write(uint.MaxValue); //fileSize
                writer.Write(1);
                writer.Write(Name.Length);
                writer.WriteString(Name);
                Console.WriteLine("pos " + writer.Position);
                long sourceArrayPos = writer.Position;
                writer.Write(uint.MaxValue);
                writer.Write(ShaderPrograms.Count);

                for (int i = 0; i < ShaderPrograms.Count; i++) {
                    long pos = writer.Position;
                    ShaderPrograms[i].Write(writer, this);
                    SharcCommon.WriteSectionSize(writer, pos);
                }

                writer.WriteUint32Offset(sourceArrayPos);
                for (int i = 0; i < SourceDatas.Count; i++) {
                    long pos = writer.Position;
                    SourceDatas[i].Write(writer, this);
                    SharcCommon.WriteSectionSize(writer, pos);
                }

                using (writer.TemporarySeek(8, System.IO.SeekOrigin.Begin)) {
                    writer.Write((uint)writer.BaseStream.Length);
                };
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

            public VariationMacroData variationVertexMacroData;
            public VariationMacroData variationFragmenMacroData;
            public VariationMacroData variationGeometryMacroData;
            public VariationMacroData variationComputeMacroData;

            public VariationSymbolData variationSymbolData;
            public VariationSymbolData variationSymbolDataFull;

            public ShaderSymbolData UniformVariables;
            public ShaderSymbolData UniformBlocks;
            public ShaderSymbolData SamplerVariables;
            public ShaderSymbolData AttributeVariables;

            public int VertexShaderIndex;
            public int FragmentShaderIndex;
            public int GeoemetryShaderIndex;

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
                editor.FillEditor(this, ((SHARC)Parent.Parent).header);
            }

            public void Read(FileReader reader, SHARC.Header header)
            {
                var pos = reader.Position;

                uint SectionSize = reader.ReadUInt32();
                uint NameLength = reader.ReadUInt32();

                if (header.Version >= 13)
                {
                    VertexShaderIndex = reader.ReadInt32();
                    FragmentShaderIndex = reader.ReadInt32();
                    GeoemetryShaderIndex = reader.ReadInt32();
                    ushort computeUnk1 = reader.ReadUInt16();
                    ushort vertexUnk2 = reader.ReadUInt16();
                    ushort fragmentUnk2 = reader.ReadUInt16();
                    ushort geometryUnk2 = reader.ReadUInt16();
                    ushort computeUnk2 = reader.ReadUInt16();
                }
                else
                {
                    VertexShaderIndex = reader.ReadInt32();
                    FragmentShaderIndex = reader.ReadInt32();
                    GeoemetryShaderIndex = reader.ReadInt32();
                }
                Text = reader.ReadString((int)NameLength, true);

                variationVertexMacroData = new VariationMacroData();
                variationFragmenMacroData = new VariationMacroData();
                variationGeometryMacroData = new VariationMacroData();
                variationComputeMacroData = new VariationMacroData();

                variationSymbolData = new VariationSymbolData();
                variationSymbolDataFull = new VariationSymbolData();
                UniformVariables = new ShaderSymbolData();
                UniformBlocks = new ShaderSymbolData();
                SamplerVariables = new ShaderSymbolData();
                AttributeVariables = new ShaderSymbolData();

                variationVertexMacroData.Read(reader, header.Version);
                variationFragmenMacroData.Read(reader, header.Version);
                variationGeometryMacroData.Read(reader, header.Version);

                if (header.Version >= 13)
                {
                    variationComputeMacroData.Read(reader, header.Version);
                }

                variationSymbolData.Read(reader);
                variationSymbolDataFull.Read(reader);

                if (header.Version <= 12)
                {
                    UniformVariables.Read(reader);

                    if (header.Version >= 11)
                    {
                        UniformBlocks.Read(reader, header.Version, true);
                    }

                    SamplerVariables.Read(reader);
                    AttributeVariables.Read(reader);
                }


                reader.Seek(SectionSize + pos, System.IO.SeekOrigin.Begin);
            }

            public void Write(FileWriter writer, SHARC.Header header)
            {
                writer.Write(uint.MaxValue);
                writer.Write(Text.Length);
                writer.Write(VertexShaderIndex);
                writer.Write(FragmentShaderIndex);
                writer.Write(GeoemetryShaderIndex);

                variationVertexMacroData.Write(writer, header.Version);
                variationFragmenMacroData.Write(writer, header.Version);
                variationGeometryMacroData.Write(writer, header.Version);

                if (header.Version >= 13)
                {
                    variationComputeMacroData.Write(writer, header.Version);
                }

                variationSymbolData.Write(writer);
                variationSymbolDataFull.Write(writer);

                if (header.Version <= 12)
                {
                    UniformVariables.Write(writer);

                    if (header.Version >= 11) {
                        UniformBlocks.Write(writer, header.Version);
                    }

                    SamplerVariables.Write(writer);
                    AttributeVariables.Write(writer);
                }
            }
        }

        public class SourceData : TreeNodeCustom
        {
            public string Code;

            public override void OnClick(TreeView treeview)
            {
                TextEditor editor = (TextEditor)LibraryGUI.GetActiveContent(typeof(TextEditor));
                if (editor == null)
                {
                    editor = new TextEditor();
                    LibraryGUI.LoadEditor(editor);
                }

                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.FillEditor(Code);
            }

            public void Read(FileReader reader)
            {
                var pos = reader.Position;

                uint SectioSize = reader.ReadUInt32();
                uint FileNameLength = reader.ReadUInt32();
                uint CodeLength = reader.ReadUInt32();
                uint CodeLength2 = reader.ReadUInt32(); //?????
                Text = reader.ReadString((int)FileNameLength);
                byte[] data = reader.ReadBytes((int)CodeLength);
                Code = Encoding.GetEncoding("shift_jis").GetString(data);

                reader.Seek(SectioSize + pos, System.IO.SeekOrigin.Begin);
            }

            public void Write(FileWriter writer, SHARC.Header header)
            {
                var data = Encoding.GetEncoding("shift_jis").GetBytes(Code);
                writer.Write(uint.MaxValue);
                writer.Write(Name.Length);
                writer.Write(data.Length);
                writer.Write(data.Length);
                writer.WriteString(Name);
                writer.Write(data);
            }
        }
    }
}
