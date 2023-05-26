using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Toolbox.Library.Compression.LZ77_wii_11_compresss.Exceptions;
using Toolbox.Library.Compression.LZ77_wii_11_compresss.Utils;

namespace Toolbox.Library.Compression.LZ77_wii_11_compresss.Formats.Nitro
{
    /// <summary>
    /// Compressor and decompressor for the LZ-0x11 format used in many of the games for the
    /// newer Nintendo consoles and handhelds.
    /// </summary>
    public sealed class LZ11 : NitroCFormat
    {
        /// <summary>
        /// Gets a short string identifying this compression format.
        /// </summary>
        public override string ShortFormatString
        {
            get { return "LZ-11"; }
        }

        /// <summary>
        /// Gets a short description of this compression format (used in the program usage).
        /// </summary>
        public override string Description
        {
            get { return "Variant of the LZ-0x10 format to support longer repetitions."; }
        }

        /// <summary>
        /// Gets the value that must be given on the command line in order to compress using this format.
        /// </summary>
        public override string CompressionFlag
        {
            get { return "lz11"; }
        }

        /// <summary>
        /// Gets if this format supports compressing a file.
        /// </summary>
        public override bool SupportsCompression
        {
            get { return true; }
        }

        private static bool lookAhead = false;
        /// <summary>
        /// Sets the flag that determines if 'look-ahead'/DP should be used when compressing
        /// with the LZ-11 format. The default is false, which is what is used in the original
        /// implementation.
        /// </summary>
        public static bool LookAhead
        {
            set { lookAhead = value; }
        }

        /// <summary>
        /// Creates a new instance of the LZ-11 compression format.
        /// </summary>
        public LZ11() : base(0x11) { }

        /// <summary>
        /// Checks if the given aguments have the '-opt' option, which makes this format
        /// compress using (near-)optimal compression instead of the original compression algorithm.
        /// </summary>
        public override int ParseCompressionOptions(string[] args)
        {
            LookAhead = false;
            if (args.Length > 0)
                if (args[0] == "-opt")
                {
                    LookAhead = true;
                    return 1;
                }
            return 0;
        }

        #region Decompression method
        /// <summary>
        /// Decompresses the input using the LZ-11 compression scheme.
        /// </summary>
        public override long Decompress(Stream instream, long inLength, Stream outstream)
        {
            #region Format definition in NDSTEK style
            /*  Data header (32bit)
                  Bit 0-3   Reserved
                  Bit 4-7   Compressed type (must be 1 for LZ77)
                  Bit 8-31  Size of decompressed data. if 0, the next 4 bytes are decompressed length
                Repeat below. Each Flag Byte followed by eight Blocks.
                Flag data (8bit)
                  Bit 0-7   Type Flags for next 8 Blocks, MSB first
                Block Type 0 - Uncompressed - Copy 1 Byte from Source to Dest
                  Bit 0-7   One data byte to be copied to dest
                Block Type 1 - Compressed - Copy LEN Bytes from Dest-Disp-1 to Dest
                    If Reserved is 0: - Default
                      Bit 0-3   Disp MSBs
                      Bit 4-7   LEN - 3
                      Bit 8-15  Disp LSBs
                    If Reserved is 1: - Higher compression rates for files with (lots of) long repetitions
                      Bit 4-7   Indicator
                        If Indicator > 1:
                            Bit 0-3    Disp MSBs
                            Bit 4-7    LEN - 1 (same bits as Indicator)
                            Bit 8-15   Disp LSBs
                        If Indicator is 1: A(B CD E)(F GH)
                            Bit 0-3     (LEN - 0x111) MSBs
                            Bit 4-7     Indicator; unused
                            Bit 8-15    (LEN- 0x111) 'middle'-SBs
                            Bit 16-19   Disp MSBs
                            Bit 20-23   (LEN - 0x111) LSBs
                            Bit 24-31   Disp LSBs
                        If Indicator is 0:
                            Bit 0-3     (LEN - 0x11) MSBs
                            Bit 4-7     Indicator; unused
                            Bit 8-11    Disp MSBs
                            Bit 12-15   (LEN - 0x11) LSBs
                            Bit 16-23   Disp LSBs
             */
            #endregion

            long readBytes = 0;

            byte type = (byte)instream.ReadByte();
            if (type != base.magicByte)
                throw new InvalidDataException("The provided stream is not a valid LZ-0x11 "
                            + "compressed stream (invalid type 0x" + type.ToString("X") + ")");
            byte[] sizeBytes = new byte[3];
            instream.Read(sizeBytes, 0, 3);
            int decompressedSize = IOUtils.ToNDSu24(sizeBytes, 0);
            readBytes += 4;
            if (decompressedSize == 0)
            {
                sizeBytes = new byte[4];
                instream.Read(sizeBytes, 0, 4);
                decompressedSize = IOUtils.ToNDSs32(sizeBytes, 0);
                readBytes += 4;
            }

            // the maximum 'DISP-1' is still 0xFFF.
            int bufferLength = 0x1000;
            byte[] buffer = new byte[bufferLength];
            int bufferOffset = 0;

            int currentOutSize = 0;
            int flags = 0, mask = 1;
            while (currentOutSize < decompressedSize)
            {
                // (throws when requested new flags byte is not available)
                #region Update the mask. If all flag bits have been read, get a new set.
                // the current mask is the mask used in the previous run. So if it masks the
                // last flag bit, get a new flags byte.
                if (mask == 1)
                {
                    if (readBytes >= inLength)
                        throw new NotEnoughDataException(currentOutSize, decompressedSize);
                    flags = instream.ReadByte(); readBytes++;
                    if (flags < 0)
                        throw new StreamTooShortException();
                    mask = 0x80;
                }
                else
                {
                    mask >>= 1;
                }
                #endregion

                // bit = 1 <=> compressed.
                if ((flags & mask) > 0)
                {
                    // (throws when not enough bytes are available)
                    #region Get length and displacement('disp') values from next 2, 3 or 4 bytes

                    // read the first byte first, which also signals the size of the compressed block
                    if (readBytes >= inLength)
                        throw new NotEnoughDataException(currentOutSize, decompressedSize);
                    int byte1 = instream.ReadByte(); readBytes++;
                    if (byte1 < 0)
                        throw new StreamTooShortException();

                    int length = byte1 >> 4;
                    int disp = -1;
                    if (length == 0)
                    {
                        #region case 0; 0(B C)(D EF) + (0x11)(0x1) = (LEN)(DISP)

                        // case 0:
                        // data = AB CD EF (with A=0)
                        // LEN = ABC + 0x11 == BC + 0x11
                        // DISP = DEF + 1

                        // we need two more bytes available
                        if (readBytes + 1 >= inLength)
                            throw new NotEnoughDataException(currentOutSize, decompressedSize);
                        int byte2 = instream.ReadByte(); readBytes++;
                        int byte3 = instream.ReadByte(); readBytes++;
                        if (byte3 < 0)
                            throw new StreamTooShortException();

                        length = (((byte1 & 0x0F) << 4) | (byte2 >> 4)) + 0x11;
                        disp = (((byte2 & 0x0F) << 8) | byte3) + 0x1;

                        #endregion
                    }
                    else if (length == 1)
                    {
                        #region case 1: 1(B CD E)(F GH) + (0x111)(0x1) = (LEN)(DISP)

                        // case 1:
                        // data = AB CD EF GH (with A=1)
                        // LEN = BCDE + 0x111
                        // DISP = FGH + 1

                        // we need three more bytes available
                        if (readBytes + 2 >= inLength)
                            throw new NotEnoughDataException(currentOutSize, decompressedSize);
                        int byte2 = instream.ReadByte(); readBytes++;
                        int byte3 = instream.ReadByte(); readBytes++;
                        int byte4 = instream.ReadByte(); readBytes++;
                        if (byte4 < 0)
                            throw new StreamTooShortException();

                        length = (((byte1 & 0x0F) << 12) | (byte2 << 4) | (byte3 >> 4)) + 0x111;
                        disp = (((byte3 & 0x0F) << 8) | byte4) + 0x1;

                        #endregion
                    }
                    else
                    {
                        #region case > 1: (A)(B CD) + (0x1)(0x1) = (LEN)(DISP)

                        // case other:
                        // data = AB CD
                        // LEN = A + 1
                        // DISP = BCD + 1

                        // we need only one more byte available
                        if (readBytes >= inLength)
                            throw new NotEnoughDataException(currentOutSize, decompressedSize);
                        int byte2 = instream.ReadByte(); readBytes++;
                        if (byte2 < 0)
                            throw new StreamTooShortException();

                        length = ((byte1 & 0xF0) >> 4) + 0x1;
                        disp = (((byte1 & 0x0F) << 8) | byte2) + 0x1;

                        #endregion
                    }

                    if (disp > currentOutSize)
                        throw new InvalidDataException("Cannot go back more than already written. "
                                + "DISP = " + disp + ", #written bytes = 0x" + currentOutSize.ToString("X")
                                + " before 0x" + instream.Position.ToString("X") + " with indicator 0x"
                                + (byte1 >> 4).ToString("X"));
                    #endregion

                    int bufIdx = bufferOffset + bufferLength - disp;
                    for (int i = 0; i < length; i++)
                    {
                        byte next = buffer[bufIdx % bufferLength];
                        bufIdx++;
                        outstream.WriteByte(next);
                        buffer[bufferOffset] = next;
                        bufferOffset = (bufferOffset + 1) % bufferLength;
                    }
                    currentOutSize += length;
                }
                else
                {
                    if (readBytes >= inLength)
                        throw new NotEnoughDataException(currentOutSize, decompressedSize);
                    int next = instream.ReadByte(); readBytes++;
                    if (next < 0)
                        throw new StreamTooShortException();

                    outstream.WriteByte((byte)next); currentOutSize++;
                    buffer[bufferOffset] = (byte)next;
                    bufferOffset = (bufferOffset + 1) % bufferLength;
                }
            }

            if (readBytes < inLength)
            {
                // the input may be 4-byte aligned.
                if ((readBytes ^ (readBytes & 3)) + 4 < inLength)
                    throw new TooMuchInputException(readBytes, inLength);
            }

            return decompressedSize;
        }
        #endregion

        #region Original compression method
        /// <summary>
        /// Compresses the input using the 'original', unoptimized compression algorithm.
        /// This algorithm should yield files that are the same as those found in the games.
        /// (delegates to the optimized method if LookAhead is set)
        /// </summary>
        public unsafe override int Compress(Stream instream, long inLength, Stream outstream)
        {
            // make sure the decompressed size fits in 3 bytes.
            // There should be room for four bytes, however I'm not 100% sure if that can be used
            // in every game, as it may not be a built-in function.
            if (inLength > 0xFFFFFF)
                throw new InputTooLargeException();

            // use the other method if lookahead is enabled
            if (lookAhead)
            {
                return CompressWithLA(instream, inLength, outstream);
            }

            // save the input data in an array to prevent having to go back and forth in a file
            byte[] indata = new byte[inLength];
            int numReadBytes = instream.Read(indata, 0, (int)inLength);
            if (numReadBytes != inLength)
                throw new StreamTooShortException();



            // write the compression header first
            byte magicByte = this.magicByte;
            byte thirteen = 0x13;
            byte lowByte = (byte)(inLength & 0xFF);
            byte highByte = (byte)((inLength >> 8) & 0xFF);
            byte middleByte = (byte)((inLength >> 16) & 0xFF);
            byte zero = (byte)((inLength >> 32) & 0xFF);    // always zero??

            byte[] compressionHeader = new byte[] { lowByte, highByte, middleByte, zero, lowByte, highByte, middleByte, zero, thirteen, lowByte, highByte, middleByte, magicByte, lowByte, highByte, middleByte };
            outstream.Write(compressionHeader, 0, compressionHeader.Length);

            int compressedLength = 4;

            fixed (byte* instart = &indata[0])
            {
                // we do need to buffer the output, as the first byte indicates which blocks are compressed.
                // this version does not use a look-ahead, so we do not need to buffer more than 8 blocks at a time.
                // (a block is at most 4 bytes long)
                byte[] outbuffer = new byte[8 * 4 + 1];
                outbuffer[0] = 0;
                int bufferlength = 1, bufferedBlocks = 0;
                int readBytes = 0;
                while (readBytes < inLength)
                {
                    #region If 8 blocks are bufferd, write them and reset the buffer
                    // we can only buffer 8 blocks at a time.
                    if (bufferedBlocks == 8)
                    {
                        outstream.Write(outbuffer, 0, bufferlength);
                        compressedLength += bufferlength;
                        // reset the buffer
                        outbuffer[0] = 0;
                        bufferlength = 1;
                        bufferedBlocks = 0;
                    }
                    #endregion

                    // determine if we're dealing with a compressed or raw block.
                    // it is a compressed block when the next 3 or more bytes can be copied from
                    // somewhere in the set of already compressed bytes.
                    int disp;
                    int oldLength = Math.Min(readBytes, 0x1000);
                    int length = LZUtil.GetOccurrenceLength(instart + readBytes, (int)Math.Min(inLength - readBytes, 0x10110),
                                                          instart + readBytes - oldLength, oldLength, out disp);

                    // length not 3 or more? next byte is raw data
                    if (length < 3)
                    {
                        outbuffer[bufferlength++] = *(instart + (readBytes++));
                    }
                    else
                    {
                        // 3 or more bytes can be copied? next (length) bytes will be compressed into 2 bytes
                        readBytes += length;

                        // mark the next block as compressed
                        outbuffer[0] |= (byte)(1 << (7 - bufferedBlocks));

                        if (length > 0x110)
                        {
                            // case 1: 1(B CD E)(F GH) + (0x111)(0x1) = (LEN)(DISP)
                            outbuffer[bufferlength] = 0x10;
                            outbuffer[bufferlength] |= (byte)(((length - 0x111) >> 12) & 0x0F);
                            bufferlength++;
                            outbuffer[bufferlength] = (byte)(((length - 0x111) >> 4) & 0xFF);
                            bufferlength++;
                            outbuffer[bufferlength] = (byte)(((length - 0x111) << 4) & 0xF0);
                        }
                        else if (length > 0x10)
                        {
                            // case 0; 0(B C)(D EF) + (0x11)(0x1) = (LEN)(DISP)
                            outbuffer[bufferlength] = 0x00;
                            outbuffer[bufferlength] |= (byte)(((length - 0x111) >> 4) & 0x0F);
                            bufferlength++;
                            outbuffer[bufferlength] = (byte)(((length - 0x111) << 4) & 0xF0);
                        }
                        else
                        {
                            // case > 1: (A)(B CD) + (0x1)(0x1) = (LEN)(DISP)
                            outbuffer[bufferlength] = (byte)(((length - 1) << 4) & 0xF0);
                        }
                        // the last 1.5 bytes are always the disp
                        outbuffer[bufferlength] |= (byte)(((disp - 1) >> 8) & 0x0F);
                        bufferlength++;
                        outbuffer[bufferlength] = (byte)((disp - 1) & 0xFF);
                        bufferlength++;
                    }
                    bufferedBlocks++;
                }

                // copy the remaining blocks to the output
                if (bufferedBlocks > 0)
                {
                    outstream.Write(outbuffer, 0, bufferlength);
                    compressedLength += bufferlength;
                    /*/ make the compressed file 4-byte aligned.
                    while ((compressedLength % 4) != 0)
                    {
                        outstream.WriteByte(0);
                        compressedLength++;
                    }/**/
                }
            }

            return compressedLength;
        }
        #endregion

        #region Dynamic Programming compression method
        /// <summary>
        /// Variation of the original compression method, making use of Dynamic Programming to 'look ahead'
        /// and determine the optimal 'length' values for the compressed blocks. Is not 100% optimal,
        /// as the flag-bytes are not taken into account.
        /// </summary>
        private unsafe int CompressWithLA(Stream instream, long inLength, Stream outstream)
        {
            // save the input data in an array to prevent having to go back and forth in a file
            byte[] indata = new byte[inLength];
            int numReadBytes = instream.Read(indata, 0, (int)inLength);
            if (numReadBytes != inLength)
                throw new StreamTooShortException();

            // write the compression header first
            outstream.WriteByte(this.magicByte);
            outstream.WriteByte((byte)(inLength & 0xFF));
            outstream.WriteByte((byte)((inLength >> 8) & 0xFF));
            outstream.WriteByte((byte)((inLength >> 16) & 0xFF));

            int compressedLength = 4;

            fixed (byte* instart = &indata[0])
            {
                // we do need to buffer the output, as the first byte indicates which blocks are compressed.
                // this version does not use a look-ahead, so we do not need to buffer more than 8 blocks at a time.
                // blocks are at most 4 bytes long.
                byte[] outbuffer = new byte[8 * 4 + 1];
                outbuffer[0] = 0;
                int bufferlength = 1, bufferedBlocks = 0;
                int readBytes = 0;

                // get the optimal choices for len and disp
                int[] lengths, disps;
                this.GetOptimalCompressionLengths(instart, indata.Length, out lengths, out disps);
                while (readBytes < inLength)
                {
                    // we can only buffer 8 blocks at a time.
                    if (bufferedBlocks == 8)
                    {
                        outstream.Write(outbuffer, 0, bufferlength);
                        compressedLength += bufferlength;
                        // reset the buffer
                        outbuffer[0] = 0;
                        bufferlength = 1;
                        bufferedBlocks = 0;
                    }


                    if (lengths[readBytes] == 1)
                    {
                        outbuffer[bufferlength++] = *(instart + (readBytes++));
                    }
                    else
                    {
                        // mark the next block as compressed
                        outbuffer[0] |= (byte)(1 << (7 - bufferedBlocks));

                        if (lengths[readBytes] > 0x110)
                        {
                            // case 1: 1(B CD E)(F GH) + (0x111)(0x1) = (LEN)(DISP)
                            outbuffer[bufferlength] = 0x10;
                            outbuffer[bufferlength] |= (byte)(((lengths[readBytes] - 0x111) >> 12) & 0x0F);
                            bufferlength++;
                            outbuffer[bufferlength] = (byte)(((lengths[readBytes] - 0x111) >> 4) & 0xFF);
                            bufferlength++;
                            outbuffer[bufferlength] = (byte)(((lengths[readBytes] - 0x111) << 4) & 0xF0);
                        }
                        else if (lengths[readBytes] > 0x10)
                        {
                            // case 0; 0(B C)(D EF) + (0x11)(0x1) = (LEN)(DISP)
                            outbuffer[bufferlength] = 0x00;
                            outbuffer[bufferlength] |= (byte)(((lengths[readBytes] - 0x111) >> 4) & 0x0F);
                            bufferlength++;
                            outbuffer[bufferlength] = (byte)(((lengths[readBytes] - 0x111) << 4) & 0xF0);
                        }
                        else
                        {
                            // case > 1: (A)(B CD) + (0x1)(0x1) = (LEN)(DISP)
                            outbuffer[bufferlength] = (byte)(((lengths[readBytes] - 1) << 4) & 0xF0);
                        }
                        // the last 1.5 bytes are always the disp
                        outbuffer[bufferlength] |= (byte)(((disps[readBytes] - 1) >> 8) & 0x0F);
                        bufferlength++;
                        outbuffer[bufferlength] = (byte)((disps[readBytes] - 1) & 0xFF);
                        bufferlength++;

                        readBytes += lengths[readBytes];
                    }


                    bufferedBlocks++;
                }

                // copy the remaining blocks to the output
                if (bufferedBlocks > 0)
                {
                    outstream.Write(outbuffer, 0, bufferlength);
                    compressedLength += bufferlength;
                    /*/ make the compressed file 4-byte aligned.
                    while ((compressedLength % 4) != 0)
                    {
                        outstream.WriteByte(0);
                        compressedLength++;
                    }/**/
                }
            }

            return compressedLength;
        }
        #endregion

        #region DP compression helper method; GetOptimalCompressionLengths
        /// <summary>
        /// Gets the optimal compression lengths for each start of a compressed block using Dynamic Programming.
        /// This takes O(n^2) time, although in practice it will often be O(n^3) since one of the constants is 0x10110
        /// (the maximum length of a compressed block)
        /// </summary>
        /// <param name="indata">The data to compress.</param>
        /// <param name="inLength">The length of the data to compress.</param>
        /// <param name="lengths">The optimal 'length' of the compressed blocks. For each byte in the input data,
        /// this value is the optimal 'length' value. If it is 1, the block should not be compressed.</param>
        /// <param name="disps">The 'disp' values of the compressed blocks. May be 0, in which case the
        /// corresponding length will never be anything other than 1.</param>
        private unsafe void GetOptimalCompressionLengths(byte* indata, int inLength, out int[] lengths, out int[] disps)
        {
            lengths = new int[inLength];
            disps = new int[inLength];
            int[] minLengths = new int[inLength];

            for (int i = inLength - 1; i >= 0; i--)
            {
                // first get the compression length when the next byte is not compressed
                minLengths[i] = int.MaxValue;
                lengths[i] = 1;
                if (i + 1 >= inLength)
                    minLengths[i] = 1;
                else
                    minLengths[i] = 1 + minLengths[i + 1];
                // then the optimal compressed length
                int oldLength = Math.Min(0x1000, i);
                // get the appropriate disp while at it. Takes at most O(n) time if oldLength is considered O(n) and 0x10110 constant.
                // however since a lot of files will not be larger than 0x10110, this will often take ~O(n^2) time.
                // be sure to bound the input length with 0x10110, as that's the maximum length for LZ-11 compressed blocks.
                int maxLen = LZUtil.GetOccurrenceLength(indata + i, Math.Min(inLength - i, 0x10110),
                                                 indata + i - oldLength, oldLength, out disps[i]);
                if (disps[i] > i)
                    throw new Exception("disp is too large");
                for (int j = 3; j <= maxLen; j++)
                {
                    int blocklen;
                    if (j > 0x110)
                        blocklen = 4;
                    else if (j > 0x10)
                        blocklen = 3;
                    else
                        blocklen = 2;
                    int newCompLen;
                    if (i + j >= inLength)
                        newCompLen = blocklen;
                    else
                        newCompLen = blocklen + minLengths[i + j];
                    if (newCompLen < minLengths[i])
                    {
                        lengths[i] = j;
                        minLengths[i] = newCompLen;
                    }
                }
            }

            // we could optimize this further to also optimize it with regard to the flag-bytes, but that would require 8 times
            // more space and time (one for each position in the block) for only a potentially tiny increase in compression ratio.
        }
        #endregion
    }
}
