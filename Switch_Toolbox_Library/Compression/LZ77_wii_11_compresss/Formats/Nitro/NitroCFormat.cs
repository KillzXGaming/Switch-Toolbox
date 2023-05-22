using System;
using System.Collections.Generic;
using System.Text;
using Toolbox.Library.Compression.LZ77_wii_11_compresss.Exceptions;
using Toolbox.Library.Compression.LZ77_wii_11_compresss.Utils;

namespace Toolbox.Library.Compression.LZ77_wii_11_compresss.Formats.Nitro
{
    /// <summary>
    /// Base class for Nitro-based decompressors. Uses the 1-byte magic and 3-byte decompression
    /// size format.
    /// </summary>
    public abstract class NitroCFormat : CompressionFormat
    {
        /// <summary>
        /// If true, Nitro Decompressors will not decompress files that have a decompressed
        /// size (plaintext size) larger than MaxPlaintextSize.
        /// </summary>
        public static bool SkipLargePlaintexts = true;
        /// <summary>
        /// The maximum allowed size of the decompressed file (plaintext size) allowed for Nitro
        /// Decompressors. Only used when SkipLargePlaintexts = true.
        /// If the expected plaintext size is larger that this, the 'Supports' method will partially
        /// decompress the data to check if the file is OK.
        /// </summary>
        public static int MaxPlaintextSize = 0x180000;

        /// <summary>
        /// The first byte of every file compressed with the format for this particular
        /// Nitro Dcompressor instance.
        /// </summary>
        protected byte magicByte;

        /// <summary>
        /// Creates a new instance of the Nitro Compression Format base class.
        /// </summary>
        /// <param name="magicByte">The expected first byte of the file for this format.</param>
        protected NitroCFormat(byte magicByte)
        {
            this.magicByte = magicByte;
        }

        /// <summary>
        /// Checks if the first four (or eight) bytes match the format used in nitro compression formats.
        /// </summary>
        public override bool Supports(System.IO.Stream stream, long inLength)
        {
            long startPosition = stream.Position;
            try
            {
                int firstByte = stream.ReadByte();
                if (firstByte != this.magicByte)
                    return false;
                // no need to read the size info as well if it's used anyway.
                if (!SkipLargePlaintexts)
                    return true;
                byte[] sizeBytes = new byte[3];
                stream.Read(sizeBytes, 0, 3);
                int outSize = IOUtils.ToNDSu24(sizeBytes, 0);
                if (outSize == 0)
                {
                    sizeBytes = new byte[4];
                    stream.Read(sizeBytes, 0, 4);
                    outSize = (int)IOUtils.ToNDSu32(sizeBytes, 0);
                }
                if (outSize <= MaxPlaintextSize)
                    return true;

                try
                {
                    stream.Position = startPosition;
                    this.Decompress(stream, Math.Min(Math.Min(inLength, 0x80000), MaxPlaintextSize), new System.IO.MemoryStream());
                    // we expect a NotEnoughDataException, since we're giving the decompressor only part of the file.
                    return false;
                }
                catch (NotEnoughDataException)
                {
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            finally
            {
                stream.Position = startPosition;
            }
        }
    }
}
