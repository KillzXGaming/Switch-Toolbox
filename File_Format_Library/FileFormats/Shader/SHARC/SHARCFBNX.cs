using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;
using Toolbox.Library.Forms;
using System.Windows.Forms;
using FirstPlugin.Forms;
using static FirstPlugin.GMX.Header;
using static FirstPlugin.RARC_Parser;

namespace FirstPlugin
{
    public class SHARCFBNX : TreeNodeCustom
    {
        public Header header;

        public class Header
        {
            public List<ShaderProgram> ShaderPrograms = new List<ShaderProgram>();
            public List<ShaderVariation> Variations = new List<ShaderVariation>();

            public void Read(FileReader reader)
            {
                reader.Position = 0;

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
                uint Alignment = reader.ReadUInt32();
                uint BinaryArraySize = reader.ReadUInt32();
                uint BinaryArrayOffset = reader.ReadUInt32();



                reader.Seek(336, System.IO.SeekOrigin.Begin); //String table always at 336

                //String Table here

                reader.Seek(336 + Alignment, System.IO.SeekOrigin.Begin);

                var Position = reader.Position; //All offsets will be relative to this

                uint ShaderProgramArrayOffset = reader.ReadUInt32();
                uint VariationCount = reader.ReadUInt32();
                for (int i = 0; i < VariationCount; i++)
                {
                    ShaderVariation var = new ShaderVariation();
                    var.Read(reader);
                    Variations.Add(var);
                }

                reader.Seek(Position + ShaderProgramArrayOffset, System.IO.SeekOrigin.Begin);
                uint ProgramArraySize = reader.ReadUInt32();
                uint ProgramCount = reader.ReadUInt32();

                for (int i = 0; i < ProgramCount; i++)
                {
                    ShaderProgram program = new ShaderProgram(this);
                    program.Read(reader);
                    ShaderPrograms.Add(program);
                }
            }
        }

        public static string ReadString(FileReader reader)
        {
            var offset = reader.ReadUInt64();
            using (reader.TemporarySeek(336 + (uint)offset, System.IO.SeekOrigin.Begin))
            {
                return reader.ReadZeroTerminatedString();
            }
        }

        public class ShaderVariation : TreeNodeCustom
        {
            public ulong BinaryDataOffset;

            public byte[] ShaderA;

            public ShaderType Type;

            public List<Symbol> Attributes = new List<Symbol>();
            public List<Symbol> Samplers = new List<Symbol>();

            public List<Symbol> Buffers = new List<Symbol>();
            public List<SymbolUniform> Uniforms = new List<SymbolUniform>();
            public List<SymbolUniformBlock> UniformBlocks = new List<SymbolUniformBlock>();

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
                editor.FillEditor(this);
            }

            public void Read(FileReader reader)
            {
                var pos = reader.Position;

                uint SectionSize = reader.ReadUInt32();
                Type = (ShaderType)reader.ReadUInt32();
                reader.ReadUInt32(); //0

                uint SectionSize2 = reader.ReadUInt32(); //next section size

                uint p = (uint)reader.Position;

                BinaryDataOffset = reader.ReadUInt64();
                uint ShaderASize = reader.ReadUInt32();
                uint ShaderAOffset = reader.ReadUInt32(); 
                reader.ReadUInt32(); //0
                uint numUniformBlocks = reader.ReadUInt32();
                var uniformBlockOffset = reader.ReadUInt64();

                uint numBuffers = 0;
                ulong bufferOffset = 0;

                //A dumb hack as version number isn't changed
                bool isNewVersion = ShaderAOffset == 96 || uniformBlockOffset == 92;

                if (isNewVersion)
                {
                    numBuffers = reader.ReadUInt32();
                    bufferOffset = reader.ReadUInt64();
                }

                uint numAttributes = reader.ReadUInt32();
                var attributeOffset = reader.ReadUInt64();
                uint numUniforms = reader.ReadUInt32();
                var uniformOffset = reader.ReadUInt64();
                uint numSamplers = reader.ReadUInt32();
                var samplerOffset = reader.ReadUInt64();

                reader.SeekBegin(p + uniformBlockOffset);
                for (int i = 0; i < numUniformBlocks; i++)
                {
                    UniformBlocks.Add(new SymbolUniformBlock()
                    {
                        Name = ReadString(reader),
                        Location = reader.ReadInt32(),
                        Size = reader.ReadUInt32(),
                    });
                }

                reader.SeekBegin(p + attributeOffset);
                for (int i = 0; i < numAttributes; i++)
                {
                    Attributes.Add(new Symbol()
                    {
                        Name = ReadString(reader),
                        Location = reader.ReadInt32(),
                    });
                }

                reader.SeekBegin(p + bufferOffset);
                for (int i = 0; i < numBuffers; i++)
                {
                    Buffers.Add(new Symbol()
                    {
                        Name = ReadString(reader),
                        Location = reader.ReadInt32(),
                    });
                }

                reader.SeekBegin(p + uniformOffset);
                for (int i = 0; i < numUniforms; i++)
                {
                    Uniforms.Add(new SymbolUniform()
                    {
                        Name = ReadString(reader),
                        Offset = reader.ReadUInt32(),
                    });
                }

                reader.SeekBegin(p + samplerOffset);
                for (int i = 0; i < numSamplers; i++)
                {
                    Samplers.Add(new Symbol()
                    {
                        Name = ReadString(reader),
                        Location = reader.ReadInt32(),
                    });
                }

                if (ShaderAOffset < SectionSize)
                {
                    reader.SeekBegin(p + ShaderAOffset);
                    ShaderA = reader.ReadBytes((int)ShaderASize);
                }

                reader.SeekBegin(pos + SectionSize);
            }

            public enum ShaderType
            {
                Vertex,
                Pixel,
            }
        }

        public class Symbol
        {
            public string Name;
            public int Location;
        }

        public class SymbolUniform
        {
            public string Name;
            public uint Offset;
        }

        public class SymbolUniformBlock
        {
            public string Name;
            public int Location;
            public uint Size;
        }

        public class ShaderProgram : TreeNodeCustom
        {
            public VariationMacroData variationMacroData;
            public VariationSymbolData variationSymbolData;
            public ShaderSymbolData UniformVariables;

            public int BaseIndex;

            private Header Header;

            public ShaderProgram(Header header)
            {
                Header = header;
            }

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
                editor.FillEditor(this, ((SHARCFB)Parent.Parent).header.sharcNX.header);
            }

            public void Read(FileReader reader)
            {
                var pos = reader.Position;

                uint SectionSize = reader.ReadUInt32();
                uint NameLength = reader.ReadUInt32();
                uint sectionCount = reader.ReadUInt32(); //3
                BaseIndex = reader.ReadInt32();

                Text = reader.ReadString((int)NameLength);

                variationMacroData = new VariationMacroData();
                variationSymbolData = new VariationSymbolData();
                UniformVariables = new ShaderSymbolData();

                variationSymbolData.Read(reader);
                variationMacroData.Read(reader);
                UniformVariables.Read(reader);

                reader.Seek(SectionSize + pos, System.IO.SeekOrigin.Begin);
            }

            public ShaderVariation GetDefaultVertexVariation()
            {
                Dictionary<string, string> settings = new Dictionary<string, string>();
                foreach (var macro in variationSymbolData.symbols)
                {
                    settings.Add(macro.Name, macro.Values.FirstOrDefault());
                }
                return GetVariation(0, settings);
            }

            public ShaderVariation GetDefaultPixelVariation()
            {
                Dictionary<string, string> settings = new Dictionary<string, string>();
                foreach (var macro in variationSymbolData.symbols)
                {
                    settings.Add(macro.Name, macro.Values.FirstOrDefault());
                }
                return GetVariation(1, settings);
            }

            public ShaderVariation GetVariation(int type, Dictionary<string, string> settings)
            {
                int index = GetVariationIndex(type, settings);
                return Header.Variations[index];
            }

            public int GetVariationIndex(int type, Dictionary<string, string> settings)
            {
                int index = 0;
                foreach (var macro in variationSymbolData.symbols)
                {
                    index *= macro.Values.Count;
                    index += macro.Values.IndexOf(settings[macro.Name]);
                }
                return BaseIndex + type + index * 2;
            }
        }
    }
}
