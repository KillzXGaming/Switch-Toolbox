using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
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
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
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
        public byte[] Save()
        {
            return null;
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
                Name = reader.ReadString((int)FileLength);


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

                reader.Close();
                reader.Dispose();
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

            public override void OnClick(TreeView treeview)
            {
                ShaderEditor editor = (ShaderEditor)LibraryGUI.Instance.GetActiveContent(typeof(ShaderEditor));
                if (editor == null)
                {
                    editor = new ShaderEditor();
                    LibraryGUI.Instance.LoadEditor(editor);
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
                    ushort vertexUnk1 = reader.ReadUInt16();
                    ushort fragmentUnk1 = reader.ReadUInt16();
                    ushort geometryUnk1 = reader.ReadUInt16();
                    ushort computeUnk1 = reader.ReadUInt16();
                    ushort vertexUnk2 = reader.ReadUInt16();
                    ushort fragmentUnk2 = reader.ReadUInt16();
                    ushort geometryUnk2 = reader.ReadUInt16();
                    ushort computeUnk2 = reader.ReadUInt16();
                }
                else
                {
                    uint vertexUnk1 = reader.ReadUInt32();
                    uint fragmentUnk1 = reader.ReadUInt32();
                    uint geometryUnk1 = reader.ReadUInt32();
                }

                Text = reader.ReadString((int)NameLength);

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
        }

        public class SourceData : TreeNodeCustom
        {
            public string Code;

            public override void OnClick(TreeView treeview)
            {
                TextEditor editor = (TextEditor)LibraryGUI.Instance.GetActiveContent(typeof(TextEditor));
                if (editor == null)
                {
                    editor = new TextEditor();
                    LibraryGUI.Instance.LoadEditor(editor);
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
             //   Code = reader.ReadString((int)CodeLength, Encoding.UTF32);
                byte[] CodeData = reader.ReadBytes((int)CodeLength);
               Code = Encoding.Unicode.GetString(Encoding.Convert(Encoding.Default, Encoding.Unicode, CodeData));

                reader.Seek(SectioSize + pos, System.IO.SeekOrigin.Begin);
            }
            public void Write(FileWriter reader)
            {

            }
        }
    }
}
