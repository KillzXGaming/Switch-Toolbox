using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syroot.NintenTools.NSW.Bfres;
using Toolbox.Library;
using Toolbox.Library.IO;
using ZstdSharp;
using ZstdSharp.Unsafe;

namespace FirstPlugin
{
    internal class MeshCodec
    {
        static ResFile ExternalStringBinary;

        public TexToGoFolder TextureFolder;

        public List<TXTG> TextureList = new List<TXTG>();

        public bool IsAnyTextureEdited => TextureList.Any(x => x.IsEdited);

        public static ExternalFlags GetExternalFlags(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                using (reader.TemporarySeek(10, SeekOrigin.Begin))
                {
                    byte version = reader.ReadByte();
                    //Check if bfres supports external strings or not
                    if (version < 10)
                        return (ExternalFlags)0;

                    //Check external flags
                    reader.SeekBegin(0xee);
                    ExternalFlags flag = (ExternalFlags)reader.ReadByte();
                    byte flag2 = reader.ReadByte(); 
                    if (flag2 == 1) //flag custom set by bfres resave to detect an mc resave
                        return ExternalFlags.MeshCodecResave;
                    return flag;
                }
            }
        }

        //Flags thanks to watertoon
        public enum ExternalFlags : byte
        {
            IsExternalModelUninitalized = 1 << 0,
            HasExternalString = 1 << 1,
            HoldsExternalStrings = 1 << 2,
            HasExternalGPU = 1 << 3,

            MeshCodecResave = 1 << 7,
        }

        // Every BFRES flagged HasExternalString stores some names as 64-bit IDs into a string table shared by the
        // whole game: romfs/Shader/ExternalBinaryString.bfres. TOTK ships it mesh-codec compressed (.mc), later
        // ModuleSystem titles (e.g. Ocarina of Time for Switch 2) ship it zstd compressed (.zs). The IDs differ per
        // game, so the table must come from the same game as the model.
        static readonly string[] ExternalStringTableNames = new string[]
        {
            "ExternalBinaryString.bfres.mc",
            "ExternalBinaryString.bfres.zs",
        };

        // Loaded tables by file path, so files from different games can be opened in one session.
        static readonly Dictionary<string, Dictionary<long, string>> ExternalStringTables =
            new Dictionary<string, Dictionary<long, string>>(StringComparer.OrdinalIgnoreCase);

        // Folder picked by the user this session for files outside of a romfs dump.
        static string SelectedRomfsPath = "";

        /// <summary>
        /// The romfs root the last external string table was loaded from.
        /// </summary>
        public static string ExternalStringRomfsPath { get; private set; } = "";

        /// <summary>
        /// The path of the external string table used for the last loaded file.
        /// </summary>
        public static string ExternalStringTablePath { get; private set; } = "";

        /// <summary>
        /// Returns the external string table inside the given romfs folder, or null if there is none.
        /// </summary>
        public static string FindExternalStringTable(string romfsPath)
        {
            if (string.IsNullOrEmpty(romfsPath) || !Directory.Exists(romfsPath))
                return null;

            foreach (var name in ExternalStringTableNames)
            {
                string path = Path.Combine(romfsPath, "Shader", name);
                if (File.Exists(path))
                    return path;
            }
            return null;
        }

        /// <summary>
        /// Searches the folders containing the given file for the romfs root holding the external string table.
        /// </summary>
        static string FindExternalStringTableNear(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            string dir;
            try { dir = Path.GetDirectoryName(Path.GetFullPath(filePath)); }
            catch { return null; }

            for (int i = 0; i < 8 && !string.IsNullOrEmpty(dir); i++)
            {
                string path = FindExternalStringTable(dir);
                if (path != null)
                    return path;
                dir = Path.GetDirectoryName(dir);
            }
            return null;
        }

        /// <summary>
        /// Loads the external string table needed by a BFRES flagged HasExternalString.
        /// The table is searched next to the file first (its romfs root), then in a folder selected this session,
        /// then in the configured TOTK romfs path, and finally the user is asked for the game's romfs folder.
        /// </summary>
        /// <returns>True if a table was loaded.</returns>
        public static bool Prepare(string filePath)
        {
            string path = FindExternalStringTableNear(filePath)
                       ?? FindExternalStringTable(SelectedRomfsPath)
                       ?? FindExternalStringTable(Runtime.TotkGamePath);

            if (path == null)
            {
                MessageBox.Show("This BFRES stores its names in an external string table " +
                    "(Shader/ExternalBinaryString.bfres.zs or .bfres.mc), which was not found in the folders " +
                    "containing the file.\n\nPlease select the romfs folder of the game this file is from " +
                    "(e.g. Tears of the Kingdom or Ocarina of Time).");

                FolderSelectDialog dlg = new FolderSelectDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    path = FindExternalStringTable(dlg.SelectedPath);
                    if (path == null)
                    {
                        MessageBox.Show($"Given folder was not valid! Expecting file " +
                            $"{Path.Combine(dlg.SelectedPath, "Shader", ExternalStringTableNames[1])} or " +
                            $"{Path.Combine(dlg.SelectedPath, "Shader", ExternalStringTableNames[0])}");
                        return false;
                    }

                    SelectedRomfsPath = dlg.SelectedPath;
                    //Only remember the folder if there is no valid TOTK path already, so it is never overwritten
                    if (FindExternalStringTable(Runtime.TotkGamePath) == null)
                    {
                        Runtime.TotkGamePath = dlg.SelectedPath;
                        Toolbox.Library.Config.Save();
                    }
                }
            }

            if (path == null)
                return false;

            LoadExternalStrings(path);
            return true;
        }

        public static void LoadExternalStrings(string path)
        {
            path = Path.GetFullPath(path);
            if (!ExternalStringTables.ContainsKey(path))
            {
                byte[] data = path.EndsWith(".mc", StringComparison.OrdinalIgnoreCase) ?
                    DecompressMeshCodec(path) : DecompressZstd(path);

                //Loading the table fills StringCache.Strings with its id -> string pairs
                ExternalStringBinary = new ResFile(new MemoryStream(data));
                ExternalStringTables.Add(path, new Dictionary<long, string>(StringCache.Strings));
            }

            //Strings are stored in a static list which will be used for opened bfres
            StringCache.Strings = new Dictionary<long, string>(ExternalStringTables[path]);
            ExternalStringTablePath = path;
            ExternalStringRomfsPath = Path.GetDirectoryName(Path.GetDirectoryName(path));
        }

        static byte[] DecompressZstd(string path)
        {
            //Selects the zstd dictionary by file extension like any other opened .zs file
            new Zstb().Init(path);
            return Zstb.SDecompress(File.ReadAllBytes(path));
        }

        public static byte[] DecompressMeshCodec(string file)
        {
            using (var fs = File.OpenRead(file)) {
                return DecompressMeshCodec(fs);
            }
        }

        public static byte[] DecompressMeshCodec(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
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

        public static byte[] CompressMeshCodec(Stream stream)
        {
            var src = stream.ToArray();

            var mem = new MemoryStream();
            using (var writer = new BinaryWriter(mem))
            {
                writer.Write(1263551309); //MCPK
                //Version 1.1.0.0
                writer.Write((byte)1);
                writer.Write((byte)1);
                writer.Write((byte)0);
                writer.Write((byte)0);
                //Flags
                writer.Write(GetMeshCodecFlags((uint)src.Length));
                //ZSTD bfres with no magic
                writer.Write(CompressZSTD(src));
            }
            return mem.ToArray();
        }

        static uint GetMeshCodecFlags(uint decompSize)
        {
            var aligned = (uint)(-decompSize % 0x1000 + 0x1000) % 0x1000;
            decompSize = decompSize + aligned;
            return ((decompSize >> (int)0xc) << 5) + 0xc;
        }

        static byte[] CompressZSTD(byte[] src)
        {
            int level = 20;
            Compressor comp = new Compressor(level);
            comp.SetParameter(ZSTD_cParameter.ZSTD_c_contentSizeFlag, 0);
            comp.SetParameter(ZSTD_cParameter.ZSTD_c_checksumFlag, 0);
            comp.SetParameter(ZSTD_cParameter.ZSTD_c_dictIDFlag, 0);
            comp.SetParameter(ZSTD_cParameter.ZSTD_c_experimentalParam2, 1);

            var compressed = comp.Wrap(src);
            return compressed.ToArray();
        }

        public void Dispose()
        {
            foreach (var tex in this.TextureList)
            {
                tex.Dispose();
            }
            TextureList.Clear();
        }

        public void SaveTexToGo()
        {
            if (!IsAnyTextureEdited)
                return;

            var msg = MessageBox.Show("Textures have been edited. Select the save location");
            TextureFolder.SaveEdited();
        }

        public void PrepareTexToGo(ResFile resFile)
        {
            var materials = resFile.Models.SelectMany(x => x.Materials);

            //Use the romfs the external strings came from, so the textures match the game of the model
            string romfsPath = resFile.UsesExternalStrings && !string.IsNullOrEmpty(ExternalStringRomfsPath) ?
                ExternalStringRomfsPath : Runtime.TotkGamePath;
            if (string.IsNullOrEmpty(romfsPath) || !Directory.Exists(Path.Combine(romfsPath, "TexToGo")))
                return;

            List<string> textureList = materials.SelectMany(x => x.TextureRefs).Distinct().ToList();
            foreach (var tex in textureList)
            {
                string path = Path.Combine(romfsPath, "TexToGo", $"{tex}.txtg");
                if (File.Exists(path))
                {
                    try
                    {
                        //File will be loaded and cached from TXTG load method
                        TextureList.Add((TXTG)STFileLoader.OpenFileFormat(path));
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}
