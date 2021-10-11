using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class MTXT : TreeNodeFile, IFileFormat, ITextureContainer, IContextMenuNode
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; } = true;
        public string[] Description { get; set; } = new string[] { "MTXT" };
        public string[] Extension { get; set; } = new string[] { "*.bctex" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream) {
            using (var reader = new FileReader(stream, true)) {
                return reader.CheckSignature(4, "MTXT");
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

        public override void OnAfterAdded()
        {
            if (Nodes.Count > 0 && this.TreeView != null)
                this.TreeView.SelectedNode = Nodes[0];
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            return new ToolStripItem[]
            {
                new ToolStripMenuItem("Save", null, Save, Keys.Control | Keys.S),
            };
        }

        private void Save(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "bctex";
            sfd.Filter = "Supported Formats|*.bctex;";
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK) {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        public List<STGenericTexture> TextureList { get; set; }

        public bool DisplayIcons => false;

        XTX TextureFile;

        uint HeaderFlags;
        ulong UnkData;
        int TextureFlag;

        public void Load(System.IO.Stream stream)
        {
            Text = this.FileName;
            Tag = this;

            using (var reader = new FileReader(stream))
            {
                reader.ReadSignature(4, "MTXT");
                HeaderFlags = reader.ReadUInt32();
                 var comp = reader.ReadBytes((int)reader.BaseStream.Length - 8);
                var decomp = STLibraryCompression.GZIP.Decompress(comp);
                ReadTextureBinary(decomp);
            }
        }


        private void ReadTextureBinary(byte[] data)
        {
            using (var reader = new FileReader(data))
            {
                //Note offsets are -8 relative due to decomp data and header being 8 bytes long
                UnkData = reader.ReadUInt64();
                uint w = reader.ReadUInt32(); //width
                uint h = reader.ReadUInt32(); //height
                reader.ReadUInt32(); //num mips
                TextureFlag = reader.ReadInt32(); //unk (-1)
                uint nameOffset = reader.ReadUInt32(); //name offset
                reader.ReadUInt32();
                var textureOffset = reader.ReadUInt32(); //texture xtx offset
                reader.ReadUInt32();
                var textureSize = reader.ReadUInt32(); //texture xtx size

                string textureName = "";
                using (reader.TemporarySeek(nameOffset - 8, System.IO.SeekOrigin.Begin)) {
                    textureName = reader.ReadZeroTerminatedString();
                }

                var stream = new SubStream(reader.BaseStream, (long)textureOffset - 8, (long)textureSize);
                TextureFile = new XTX();
                TextureFile.FileName = textureName;
                TextureFile.Load(stream);
                foreach (STGenericTexture node in TextureFile.Nodes)
                    Nodes.Add(node);

                TextureList = TextureFile.TextureList;
            }
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {
                writer.WriteSignature("MTXT");
                writer.Write(HeaderFlags);
                writer.Write(STLibraryCompression.GZIP.Compress(WriteTextureBinary()));
            }
        }

        private byte[] WriteTextureBinary()
        {
            //Save XTX file binary
            var binaryMem = new System.IO.MemoryStream();
            TextureFile.Save(binaryMem);
            var rawTextureFile = binaryMem.ToArray();

            var mem = new System.IO.MemoryStream();
            using (var writer = new FileWriter(mem))
            {
                //Write file header
                writer.Write(UnkData);
                writer.Write(TextureFile.TextureList[0].Width);
                writer.Write(TextureFile.TextureList[0].Height);
                writer.Write(TextureFile.TextureList[0].MipCount);
                writer.Write(TextureFlag);
                writer.Write(uint.MaxValue); //reserve name offset
                writer.Write(0);
                writer.Write(uint.MaxValue); //data offset
                writer.Write(0);
                writer.Write(rawTextureFile.Length); //data size
                for (int i = 0; i < 19; i++)
                    writer.Write(0xFFFFFFFF);

                //Write xtx file data
                writer.WriteUint32Offset(32, -8);
                writer.Write(rawTextureFile);

                writer.WriteUint32Offset(24, -8);
                writer.WriteString(Text);
            }
            return mem.ToArray();
        }

        public void Unload()
        {

        }
    }
}
