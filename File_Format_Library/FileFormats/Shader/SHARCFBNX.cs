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

namespace FirstPlugin
{
    public class SHARCFBNX : TreeNodeCustom
    {
        public Header header;

        public class Header
        {
            public List<ShaderProgram> ShaderPrograms = new List<ShaderProgram>();

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



              
                reader.Seek(Position + ShaderProgramArrayOffset, System.IO.SeekOrigin.Begin);
                uint ProgramArraySize = reader.ReadUInt32();
                uint ProgramCount = reader.ReadUInt32();

                for (int i = 0; i < ProgramCount; i++)
                {
                    ShaderProgram program = new ShaderProgram();
                    program.Read(reader);
                    ShaderPrograms.Add(program);
                }

                reader.Close();
                reader.Dispose();
            }
        }

        public class ShaderProgram : TreeNodeCustom
        {
            public VariationMacroData variationMacroData;
            public VariationSymbolData variationSymbolData;
            public ShaderSymbolData UniformVariables;

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
                uint ShaderType = reader.ReadUInt32();
                uint index = reader.ReadUInt32();

                Text = reader.ReadString((int)NameLength);

                variationMacroData = new VariationMacroData();
                variationSymbolData = new VariationSymbolData();
                UniformVariables = new ShaderSymbolData();

                variationMacroData.Read(reader);
                variationSymbolData.Read(reader);
                UniformVariables.Read(reader);

                reader.Seek(SectionSize + pos, System.IO.SeekOrigin.Begin);
            }
        }
    }
}
