using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class MTXT : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Image;

        public bool CanSave { get; set; }
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

        public void Load(System.IO.Stream stream)
        {
            Text = this.FileName;

            using (var reader = new FileReader(stream))
            {
                reader.ReadSignature(4, "MTXT");
                reader.ReadBytes(4);
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
                reader.ReadUInt64();
                uint w = reader.ReadUInt32(); //width
                uint h = reader.ReadUInt32(); //height
                reader.ReadUInt32(); //num mips
                reader.ReadUInt32(); //unk (-1)
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
                XTX xtx = new XTX();
                xtx.FileName = textureName;
                xtx.Load(stream);
                this.Tag = xtx;
                foreach (STGenericTexture node in xtx.Nodes)
                    Nodes.Add(node);
            }
        }

        public void Save(System.IO.Stream stream)
        {

        }

        public void Unload()
        {

        }
    }
}
