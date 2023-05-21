using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Toolbox.Library.Compression.LZ77_wii_11_compresss.Exceptions;

namespace Toolbox.Library.Compression.LZ77_wii_11_compresss.Formats
{
    /// <summary>
    /// A format that is composed of multiple formats.
    /// When compressing, the input is compressed using the best contained format.
    /// When decompressing, all contained formats will try to decompress the file, until one succeeds.
    /// </summary>
    public abstract class CompositeFormat : CompressionFormat
    {
        /// <summary>
        /// The actual list of formats this format is somposed of.
        /// </summary>
        private List<CompressionFormat> formats;


        #region Constructors

        /// <summary>
        /// Creates a new composite format based on the given sequence of formats.
        /// </summary>
        protected CompositeFormat(IEnumerable<CompressionFormat> formats)
        {
            this.formats = new List<CompressionFormat>(formats);
        }
        /// <summary>
        /// Creates a new composite format based on the given formats.
        /// </summary>
        protected CompositeFormat(params CompressionFormat[] formats)
        {
            this.formats = new List<CompressionFormat>(formats);
        }

        #endregion


        #region Method: Supports
        /// <summary>
        /// Checks if any of the contained formats supports the given input.
        /// </summary>
        public override bool Supports(System.IO.Stream stream, long inLength)
        {
            foreach (CompositeFormat fmt in this.formats)
            {
                if (fmt.Supports(stream, inLength))
                    return true;
            }
            return false;
        }
        #endregion

        #region Method: Decompress
        /// <summary>
        /// Attempts to decompress the given input by letting all contained formats
        /// try to decompress the input.
        /// </summary>
        public override long Decompress(System.IO.Stream instream, long inLength, System.IO.Stream outstream)
        {
            byte[] inputData = new byte[instream.Length];
            instream.Read(inputData, 0, inputData.Length);

            foreach (CompressionFormat format in this.formats)
            {
                if (!format.SupportsDecompression)
                    continue;
                using (MemoryStream input = new MemoryStream(inputData))
                {
                    if (!format.Supports(input, inputData.Length))
                        continue;
                    MemoryStream output = new MemoryStream();
                    try
                    {
                        long decLength = format.Decompress(input, inputData.Length, output);
                        if (decLength > 0)
                        {
                            output.WriteTo(outstream);
                            return decLength;
                        }
                    }
                    catch (Exception) { continue; }
                }
            }

            throw new InvalidDataException("Input cannot be decompressed using the " + this.ShortFormatString + " formats.");
        }
        #endregion

        #region Method: Compress & Field: LastUsedCompressFormatString
        /// <summary>
        /// Gets the ShortFormatString of the last CompressionFormat that was used to compress input.
        /// </summary>
        public string LastUsedCompressFormatString { get; private set; }
        /// <summary>
        /// Compresses the given input using the contained format that yields the best results in terms of
        /// size reduction.
        /// </summary>
        public override int Compress(System.IO.Stream instream, long inLength, System.IO.Stream outstream)
        {
            // only read the input data once from the file.
            byte[] inputData = new byte[instream.Length];
            instream.Read(inputData, 0, inputData.Length);

            MemoryStream bestOutput = null;
            string bestFormatString = "";
            int minCompSize = int.MaxValue;
            foreach (CompressionFormat format in formats)
            {
                if (!format.SupportsCompression)
                    continue;

                #region compress the file in each format, and save the best one

                MemoryStream currentOutput = new MemoryStream();
                int currentOutSize;
                try
                {
                    using (MemoryStream input = new MemoryStream(inputData))
                    {
                        currentOutSize = format.Compress(input, input.Length, currentOutput);
                    }
                }
                catch (InputTooLargeException i)
                {
                    Console.WriteLine(i.Message);
                    bestFormatString = format.ShortFormatString;
                    return -1;
                }
                catch (Exception)
                {
                    continue;
                }
                if (currentOutSize < minCompSize)
                {
                    bestOutput = currentOutput;
                    minCompSize = currentOutSize;
                    bestFormatString = format.ShortFormatString;
                }

                #endregion
            }

            if (bestOutput == null)
                return -1;
            bestOutput.WriteTo(outstream);
            this.LastUsedCompressFormatString = bestFormatString;
            return minCompSize;
        }
        #endregion

        #region Method: ParseCompressionOptions(args)
        /// <summary>
        /// Handles the compression options for each of the contained compression formats.
        /// </summary>
        public override int ParseCompressionOptions(string[] args)
        {
            // try each option on each of the formats.
            // each pass over the formats lets them try to consume the options.
            // if one or more formats consume at least one option, the maximum number
            // of consumed options is treated as 'handled'; they are ignored in the
            // next pass. This continues until none of the formats consume the next
            // value in the options.

            int totalOptionCount = 0;
            bool usedOption = true;
            while (usedOption)
            {
                usedOption = false;
                if (args.Length <= totalOptionCount)
                    break;
                int maxOptionCount = 0;
                string[] subArray = new string[args.Length - totalOptionCount];
                Array.Copy(args, totalOptionCount, subArray, 0, subArray.Length);
                foreach (CompressionFormat format in this.formats)
                {
                    int optCount = format.ParseCompressionOptions(subArray);
                    maxOptionCount = Math.Max(optCount, maxOptionCount);
                }

                if (maxOptionCount > 0)
                {
                    totalOptionCount += maxOptionCount;
                    usedOption = true;
                }
            }
            return totalOptionCount;
        }
        #endregion

    }
}
