using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Toolbox.Library.Compression.LZ77_wii_11_compresss.Formats
{
    /// <summary>
    /// Base class for all compression formats.
    /// </summary>
    public abstract class CompressionFormat
    {
        /// <summary>
        /// Checks if the decompressor for this format supports the data from the given stream.
        /// Returns false when it is certain that the given data is not supported.
        /// False positives may occur, as this method should not do any decompression, and may
        /// mis-interpret a similar data format as compressed.
        /// </summary>
        /// <param name="stream">The stream that may or may not contain compressed data. The
        /// position of this stream may change during this call, but will be returned to its
        /// original position when the method returns.</param>
        /// <param name="inLength">The length of the input stream.</param>
        /// <returns>False if the data can certainly not be decompressed using this decompressor.
        /// True if the data may potentially be decompressed using this decompressor.</returns>
        public abstract bool Supports(Stream stream, long inLength);



        /// <summary>
        /// Decompresses the given stream, writing the decompressed data to the given output stream.
        /// Assumes <code>Supports(instream)</code> returns <code>true</code>.
        /// After this call, the input stream will be positioned at the end of the compressed stream,
        /// or at the initial position + <code>inLength</code>, whichever comes first.
        /// </summary>
        /// <param name="instream">The stream to decompress. At the end of this method, the position
        /// of this stream is directly after the compressed data.</param>
        /// <param name="inLength">The length of the input data. Not necessarily all of the
        /// input data may be read (if there is padding, for example), however never more than
        /// this number of bytes is read from the input stream.</param>
        /// <param name="outstream">The stream to write the decompressed data to.</param>
        /// <returns>The length of the output data.</returns>
        /// <exception cref="NotEnoughDataException">When the given length of the input data
        /// is not enough to properly decompress the input.</exception>
        public abstract long Decompress(Stream instream, long inLength, Stream outstream);



        /// <summary>
        /// Compresses the next <code>inLength</code> bytes from the input stream,
        /// and writes the compressed data to the given output stream.
        /// </summary>
        /// <param name="instream">The stream to read plaintext data from.</param>
        /// <param name="inLength">The length of the plaintext data.</param>
        /// <param name="outstream">The stream to write the compressed data to.</param>
        /// <returns>The size of the compressed stream. If -1, the file could not be compressed.</returns>
        public abstract int Compress(Stream instream, long inLength, Stream outstream);

        /// <summary>
        /// Gets a short string identifying this compression format.
        /// </summary>
        public abstract string ShortFormatString { get; }
        /// <summary>
        /// Gets a short description of this compression format (used in the program usage).
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets if this format supports compressing a file.
        /// </summary>
        public abstract bool SupportsCompression { get; }
        /// <summary>
        /// Gets if this format supports decompressing a file.
        /// </summary>
        public virtual bool SupportsDecompression { get { return true; } }
        /// <summary>
        /// Gets the value that must be given on the command line in order to compress using this format.
        /// </summary>
        public abstract string CompressionFlag { get; }
        /// <summary>
        /// Parses any input specific for this format. Does nothing by default.
        /// </summary>
        /// <param name="args">Any arguments that may be used by the format.</param>
        /// <returns>The number of input arguments consumed by this format.</returns>
        public virtual int ParseCompressionOptions(string[] args) { return 0; }
    }
}
