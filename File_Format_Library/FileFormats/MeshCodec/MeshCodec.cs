using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AampLibraryCSharp.IO;
using Syroot.NintenTools.NSW.Bfres;
using Toolbox.Library;
using ZstdSharp.Unsafe;

namespace FirstPlugin
{
    internal class MeshCodec
    {
        static ResFile ExternalStringBinary;

        public static bool IsMeshCodec(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                reader.Seek(238, SeekOrigin.Begin);
                byte flag = reader.ReadByte();
                reader.Position = 0;
                //Check if flag is external buffer .mc binary
                return flag == 11;
            }
        }

        public static void Prepare()
        {
            //Check if a valid directory exists
            string path = Path.Combine(Runtime.TotkGamePath, "Shader", "ExternalBinaryString.bfres.mc");
            if (!File.Exists(path))
            {
                 MessageBox.Show("A game dump of TOTK is required to load this file. Please select the romfs folder path.");

                FolderSelectDialog dlg = new FolderSelectDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Runtime.TotkGamePath = dlg.SelectedPath;
                    path = Path.Combine(Runtime.TotkGamePath, "Shader", "ExternalBinaryString.bfres.mc");
                    Toolbox.Library.Config.Save();
                }
            }

            if (!File.Exists(path))
            {
                MessageBox.Show($"Given folder was not valid! Expecting file {path}");
                return;
            }

            LoadExternalStrings();
        }

        static void LoadExternalStrings()
        {
            if (ExternalStringBinary != null)
                return;

            string path = Path.Combine(Runtime.TotkGamePath, "Shader", "ExternalBinaryString.bfres.mc");
            byte[] data = DecompressMeshCodec(path);
            //Load string table into memory
            //Strings are stored in a static list which will be used for opened bfres
            ExternalStringBinary = new ResFile(new MemoryStream(data));
        }

        static byte[] DecompressMeshCodec(string file)
        {
            using (var fs = File.OpenRead(file))
            using (var reader = new BinaryReader(fs))
            {
                reader.ReadUInt32(); //Magic
                reader.ReadUInt32(); //Version 1.1.0.0
                var flags = reader.ReadInt32();
                var decompressed_size = (flags >> 5) << (flags & 0xf);

                reader.BaseStream.Seek(0xC, SeekOrigin.Begin);
                byte[] src = reader.ReadBytes((int)reader.BaseStream.Length - 0xC);
               return Decompress(src, (uint)decompressed_size);
            }
        }

        static unsafe byte[] Decompress(byte[] src, uint decompressed_size)
        {
            var dctx = Methods.ZSTD_createDCtx();
            Methods.ZSTD_DCtx_setFormat(dctx, ZSTD_format_e.ZSTD_f_zstd1_magicless);
            var uncompressed = new byte[decompressed_size];
            fixed (byte* srcPtr = src)
            fixed (byte* uncompressedPtr = uncompressed)
            {
                var decompressedLength = Methods.ZSTD_decompressDCtx(dctx, uncompressedPtr, (UIntPtr)uncompressed.Length, srcPtr, (UIntPtr)src.Length);

                byte[] arr = new byte[(uint)decompressed_size];
                Marshal.Copy((IntPtr)uncompressedPtr, arr, 0, arr.Length);
                return arr;
            }
        }
    }
}
