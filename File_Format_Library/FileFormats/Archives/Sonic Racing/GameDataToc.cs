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

namespace FirstPlugin
{
    public class GameDataToc : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "GameDataToc" };
        public string[] Extension { get; set; } = new string[] { "*.toc" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return FileName == "GameData.toc";
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

        public string EncryptionKey = "b0110b704a259cd8f0da9e4480bd0b9c8c7c94c609f322288273cf85930dd64f7baf13255b075df912e81c2d8aae98fe034dee5f61c232c4b39280fe4c68c46151be7f49b7cbd4ee0922acb02aaa76bc9585dfb89c5fb90da7094289ea18c81312839b06fd267273c81babea38405276a1d35835916e7cec88f26473abb79578762e3abe0ce719309114aa40dcf8d7466e4edc1b8a887d1ed7fad0c7d418fe81660ea5b9445d2db47174cc1bfaa2c96eb84cca8e447c95ef12a76e745c7d19497d4729beaba244eb992f2e46934920caa5ad09d6950e0f9fefdac6104b5609379eff0ad4de56a941e63baf25924dcdcb5632d27b58108d5af19ea2b3946986514940bd4953396391450286ac8f47e07b95a8ab59a770ac71acbf51ba9c8f4c18fb88865f1d49577fd14da819223d4c78a49f888d78141bfa140669eda3d890e05f6d85b8058042b127e50feb1a6fdf28a309309646ff3081933d93293239de9c74c55f0589b3658bb2764833a1001ec8c6dcba7c610d15df27fd7938da58ea7c877c0fd38c267ca295b50e37560c702b1a8f0fa9227eccbbc2b0d6e704b8a1bd65a3fb42250557c8675b1f9ebd6ce056fbf0098ee03e3ba75fc29dbc9217de62b67a04f82abb8b8b2eca5ee0ee89219805281b56c0beb6db5269c45cf330cbd7e1a7579456e380e7027ef77b4fb6fd2d9df00248357e72b206ed8d6db3577a7cf81c04dbf054ffbe69b2cede6ca266172a138754b588b48ff686c728838c4c9c4342045fd74e68f28b7d0f86a5bfbee86d5122b838aeab59cddf8acc7d71416f2239d2a22db3ee83cd0396089dbae3bf9f7bf7081ec8591c159507d0a1d2b8d34f477c45dfa2341716be64ab445bef8494fb8f2aa9b5dd60d1e61a22e5b44f73dfcabf407ba0e35f1a95d69cd73e42a3e9d966bf439c3b69fb82df4929a8d65fa734dcf39c738053bb17f3296b270b3a859970083b9eb0b9732f87f96502c443327902b14827056b3711c5518d8b2ef250f4a958d61fa89ade5c2af1759a57a2403dad07c1611bfd99736ac5c86b9079b676401ea10e6dd5fbf3078e1ae6723b63f4d3908c0b9b7f679b3b25ae7e7b98ba9c127936ba9a0e86466edb580681e4f92f1b8b0c91d2ca57a65c98f03f69c8946fd93c5051cf5210aa5e7d9a13e58c464170cc6d770e3078dfde6602c7c6526f216cbb6b28db3c8a442546092342531b875c887d66f57722e7b850161483c18ac2ab6cea7fb67f05f5636a2776c0dee80913a8cec042f9d48e4db79365cf0c36f6c6446328311134b1677218549f65bcd2e7ace3a303250a66cb942dfd4d234d20ef14315ad5c460a460fa26f56efe98448b72ab374e7fc6de475b801fa3ba1f0562370894e0815f545a6c671a5b0d89fc9ec983e26aea00e1b1407e4d278de74b31d5aaae78a5edbc439011ceb2db";

        public byte[] DecryptedTable { get; set; }

        public static byte[] EncryptOrDecryptXor(byte[] text, byte[] key)
        {
            byte[] xor = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                xor[i] = (byte)(text[i] ^ key[i % key.Length]);
            }
            return xor;
        }

        private byte[] FromHex(string hex)
        {
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            using (var reader = new FileReader(stream))
            {
                byte[] table = reader.ReadBytes((int)reader.BaseStream.Length);
                DecryptedTable = FromHex(EncryptionKey);
            }
        }

        public static byte[] StringToByteArray(string hex)
        {


            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

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
            editor.LoadData(DecryptedTable);
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
