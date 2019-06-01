using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class NUSHDB : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Namco Shader Data Binary" };
        public string[] Extension { get; set; } = new string[] { "*.nushdb" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "HBSS") && reader.CheckSignature(4, "RDHS", 16) ;
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

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                //Skip the HBSS header
                reader.Seek(0x10, System.IO.SeekOrigin.Begin);
                reader.ReadSignature(4, "RDHS");
                ushort VersionMajor = reader.ReadUInt16();
                ushort VersionMinor = reader.ReadUInt16();
                ulong Unknown = reader.ReadUInt64(); //Usually 16
                ulong ProgramCount = reader.ReadUInt64();

                for (int i = 0; i < (int)ProgramCount; i++)
                {
                    ShaderProgram program = new ShaderProgram();
                    program.Text = reader.LoadString(true, typeof(ulong));
                    uint unk2 = reader.ReadUInt32();
                    uint Type = reader.ReadUInt32();
                    long BinaryOffset = reader.ReadOffset(true, typeof(ulong));
                    ulong BinarySize = reader.ReadUInt64();
                    ulong BinarySize2 = reader.ReadUInt64();
                    ulong padding = reader.ReadUInt64();
                    ulong padding2 = reader.ReadUInt64();

                    if (Type == 0)
                        program.ShaderType = NSWShaderDecompile.NswShaderType.Vertex;
                    if (Type == 2)
                        program.ShaderType = NSWShaderDecompile.NswShaderType.Fragment;

                    using (reader.TemporarySeek(BinaryOffset, System.IO.SeekOrigin.Begin))
                    {
                        program.Data = reader.ReadBytes((int)BinarySize);
                    }

                    Nodes.Add(program);
                }
            }
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }

        public class ShaderProgram : TreeNodeCustom
        {
            public ShaderProgram()
            {
                ContextMenuStrip = new STContextMenuStrip();
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export Data", null, Export, Keys.Control | Keys.E));
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export Raw Binary", null, ExportBinary, Keys.Control | Keys.B));
            }

            public NSWShaderDecompile.NswShaderType ShaderType { get; set; }

            public byte[] Data { get; set; }

            public byte[] RawBinaryData { get; set; }

            public string Code { get; set; }

            public void ParseBinary()
            {
                //Get the raw binary data
                using (var reader = new FileReader(Data))
                {
                    reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                    reader.Seek(68, System.IO.SeekOrigin.Begin);
                    uint BinarySize = reader.ReadUInt32();
                    uint BinaryOffset = reader.ReadUInt32();

                    Console.WriteLine("Binary Size " + BinarySize);
                    Console.WriteLine("Binary Offset " + BinaryOffset);

                    reader.Seek((int)BinaryOffset, System.IO.SeekOrigin.Begin);
                    RawBinaryData = reader.ReadBytes((int)BinarySize);
                }
            }

            public bool TryDecompileBinary()
            {
                ParseBinary();
                Code = NSWShaderDecompile.DecompileShader(ShaderType, Data);

                if (Code.Length > 0) return true;
                else return false;
            }

            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "bin";
                sfd.FileName = Text;

                sfd.Filter = "Supported Formats|*.bin;";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllBytes(sfd.FileName, Data);
                }
            }

            private void ExportBinary(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "bin";
                sfd.FileName = Text;

                sfd.Filter = "Supported Formats|*.bin;";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllBytes(sfd.FileName, RawBinaryData);
                }
            }

            public override void OnClick(TreeView treeview)
            {
                bool IsSuccess = TryDecompileBinary();

                TextEditor editor = (TextEditor)LibraryGUI.Instance.GetActiveContent(typeof(TextEditor));
                if (editor == null)
                {
                    editor = new TextEditor();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.Instance.LoadEditor(editor);
                }

                editor.Text = Text;
                editor.FillEditor(string.Join("", Code));
            }
        }
    }
}
