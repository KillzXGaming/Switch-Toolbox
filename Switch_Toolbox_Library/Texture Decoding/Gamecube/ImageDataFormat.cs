// CTools library - Library functions for CTools
// Copyright (C) 2010 Chadderz

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.ComponentModel;

namespace Chadsoft.CTools.Image
{
    public sealed class ImageDataFormat
    {
        private static ImageDataFormat _i4, _i8, _ia4, _ia8, _rgb565, _rgb5a3, _rgba32, _c4, _c8, _c14x2, _cmpr;

        public static ImageDataFormat I4 { get { if (_i4 == null) _i4 = new ImageDataFormat("I4", "I4", 4, 0, 8, 8, 32, false, false, false, false, 0, 0, ConvertBlockToI4, ConvertBlockFromI4); return _i4; } }
        public static ImageDataFormat I8 { get { if (_i8 == null) _i8 = new ImageDataFormat("I8", "I8", 8, 0, 8, 4, 32, false, false, false, false, 0, 0, ConvertBlockToI8, ConvertBlockFromI8); return _i8; } }
        public static ImageDataFormat IA4 { get { if (_ia4 == null) _ia4 = new ImageDataFormat("IA4", "IA4", 8, 4, 8, 4, 32, false, false, false, false, 0, 0, ConvertBlockToIa4, ConvertBlockFromIa4); return _ia4; } }
        public static ImageDataFormat IA8 { get { if (_ia8 == null) _ia8 = new ImageDataFormat("IA8", "IA8", 16, 8, 4, 4, 32, false, false, false, false, 0, 0, ConvertBlockToIa8, ConvertBlockFromIa8); return _ia8; } }
        public static ImageDataFormat RGB565 { get { if (_rgb565 == null) _rgb565 = new ImageDataFormat("RGB565", "RGB565", 16, 0, 4, 4, 32, true, false, false, false, 0, 0, ConvertBlockToRgb565, ConvertBlockFromRgb565); return _rgb565; } }
        public static ImageDataFormat RGB5A3 { get { if (_rgb5a3 == null) _rgb5a3 = new ImageDataFormat("RGB5A3", "RGB5A3", 16, 3, 4, 4, 32, true, false, false, false, 0, 0, ConvertBlockToRgb5a3, ConvertBlockFromRgb5a3); return _rgb5a3; } }
        public static ImageDataFormat Rgba32 { get { if (_rgba32 == null) _rgba32 = new ImageDataFormat("RGBA32", "RGBA32", 32, 8, 4, 4, 64, true, false, false, false, 0, 0, ConvertBlockToRgba32, ConvertBlockFromRgba32); return _rgba32; } }
        public static ImageDataFormat Cmpr { get { if (_cmpr == null) _cmpr = new ImageDataFormat("CMPR", "CMPR", 4, 1, 8, 8, 32, true, true, true, false, 0, 0, ConvertBlockToCmpr, ConvertBlockFromCmpr); return _cmpr; } }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public int BitsPerPixel { get; private set; }
        public int AlphaDepth { get; private set; }
        public int BlockWidth { get; private set; }
        public int BlockHeight { get; private set; }
        public int BlockStride { get; private set; }
        public bool HasColour { get; private set; }
        public bool IsCompressed { get; private set; }
        public bool LossyCompression { get; private set; }
        public bool Palette { get; private set; }
        public int PaletteSize { get; private set; }
        public int PaletteBitsPerEntry { get; private set; }

        private ConvertBlockDelegate _convertTo, _convertFrom;

        public ImageDataFormat(string name, string description, int bitsPerPixel, int alphaDepth, int blockWidth, int blockHeight, int blockStride, bool hasColour, bool isCompressed, bool lossyCompression, bool palette, int paletteSize, int paletteBitsPerEntry, ConvertBlockDelegate convertTo, ConvertBlockDelegate convertFrom)
        {
            Name = name;
            Description = description;
            BitsPerPixel = bitsPerPixel;
            AlphaDepth = alphaDepth;
            BlockWidth = blockWidth;
            BlockHeight = blockHeight;
            BlockStride = blockStride;
            HasColour = hasColour;
            IsCompressed = isCompressed;
            Palette = palette;
            PaletteSize = paletteSize;
            PaletteBitsPerEntry = paletteBitsPerEntry;
            _convertTo = convertTo;
            _convertFrom = convertFrom;
        }

        public int RoundWidth(int width)
        {
            return width + ((BlockWidth - (width % BlockWidth)) % BlockWidth);
        }
        public int RoundHeight(int height)
        {
            return height + ((BlockHeight - (height % BlockHeight)) % BlockHeight);
        }

        public byte[] ConvertFrom(byte[] data, int width, int height, ProgressChangedEventHandler progress = null)
        {
            byte[] result, blockResult, block;
            int step;

            step = Math.Max(width * height / BlockHeight / BlockWidth / 100, 1024);
            result = new byte[width * height << 2];
            block = new byte[BlockStride];

            for (int y = 0, i = 0; y < height; y += BlockHeight)
            {
                for (int x = 0; x < width; x += BlockWidth, i++)
                {
                    Array.Copy(data, i * block.Length, block, 0, block.Length);
                    blockResult = _convertFrom(block);

                    for (int dy = 0; dy < Math.Min(BlockHeight, height - y); dy++)
                    {
                        Array.Copy(blockResult, dy * BlockWidth << 2, result, ((dy + y) * width + x) << 2, Math.Min(BlockWidth, width - x) << 2);
                    }

                    if (i % step == 0 && progress != null)
                        progress(this, new ProgressChangedEventArgs((x + y * width * 100) / (result.Length / 4), null));
                }
            }

            return result;
        }

        public byte[] ConvertTo(byte[] data, int width, int height, ProgressChangedEventHandler progress)
        {
            byte[] result, block, blockResult;
            int step;

            step = Math.Max(width * height / BlockHeight / BlockWidth / 100, 1024);
            result = new byte[RoundWidth(width) / BlockWidth * RoundHeight(height) / BlockHeight * BlockStride];
            block = new byte[BlockWidth * BlockHeight << 2];

            for (int y = 0, i = 0; y < height; y += BlockHeight)
            {
                for (int x = 0; x < width; x += BlockWidth, i++)
                {
                    Array.Clear(block, 0, block.Length);

                    for (int dy = 0; dy < Math.Min(BlockHeight, height - y); dy++)
                    {
                        Array.Copy(data, ((y + dy) * width + x) << 2, block, dy * BlockWidth << 2, Math.Min(BlockWidth, width - x) << 2);
                    }

                    blockResult = _convertTo(block);
                    blockResult.CopyTo(result, i * BlockStride);

                    if (i % step == 0 && progress != null)
                        progress(this, new ProgressChangedEventArgs((x + y * width * 100) / (width * height), null));
                }
            }

            return result;
        }

        private static byte[] ConvertBlockFromI4(byte[] block)
        {
            byte[] result;

            result = new byte[256];

            for (int i = 0; i < block.Length; i++)
            {
                result[i * 8 + 0] = result[i * 8 + 1] = result[i * 8 + 2] = result[i * 8 + 3] = (byte)((block[i] >> 4) * 0x11);
                result[i * 8 + 4] = result[i * 8 + 5] = result[i * 8 + 6] = result[i * 8 + 7] = (byte)((block[i] & 0xF) * 0x11);
            }

            return result;
        }
        private static byte[] ConvertBlockFromI8(byte[] block)
        {
            byte[] result;

            result = new byte[128];

            for (int i = 0; i < block.Length; i++)
            {
                result[i * 4 + 0] = result[i * 4 + 1] = result[i * 4 + 2] = result[i * 4 + 3] = block[i];
            }

            return result;
        }
        private static byte[] ConvertBlockFromIa4(byte[] block)
        {
            byte[] result;

            result = new byte[128];

            for (int i = 0; i < block.Length; i++)
            {
                result[i * 4 + 0] = result[i * 4 + 1] = result[i * 4 + 2] = (byte)((block[i] & 0xF) * 0x11);
                result[i * 4 + 3] = (byte)((block[i] >> 4) * 0x11);
            }

            return result;
        }
        private static byte[] ConvertBlockFromIa8(byte[] block)
        {
            byte[] result;

            result = new byte[64];

            for (int i = 0; i < block.Length / 2; i++)
            {
                result[i * 4 + 0] = result[i * 4 + 1] = result[i * 4 + 2] = block[i * 2 + 1];
                result[i * 4 + 3] = block[i * 2 + 0];
            }

            return result;
        }
        private static byte[] ConvertBlockFromRgb565(byte[] block)
        {
            byte[] result;

            result = new byte[64];

            for (int i = 0; i < block.Length / 2; i++)
            {
                result[i * 4 + 0] = (byte)(block[i * 2 + 1] << 3 & 0xf8 | block[i * 2 + 1] >> 2 & 0x07);
                result[i * 4 + 1] = (byte)(block[i * 2 + 0] << 5 & 0xe0 | block[i * 2 + 1] >> 3 & 0x1c | block[i * 2 + 0] >> 1 & 0x03);
                result[i * 4 + 2] = (byte)(block[i * 2 + 0] & 0xf8 | block[i * 2 + 0] >> 5);
                result[i * 4 + 3] = 255;
            }

            return result;
        }
        private static byte[] ConvertBlockFromRgb5a3(byte[] block)
        {
            byte[] result;

            result = new byte[64];

            for (int i = 0; i < block.Length / 2; i++)
            {
                if ((block[i * 2 + 0] & 0x80) == 0)
                {
                    result[i * 4 + 0] = (byte)(block[i * 2 + 1] << 4 & 0xf0 | block[i * 2 + 1] & 0xf);
                    result[i * 4 + 1] = (byte)(block[i * 2 + 1] & 0xf0 | block[i * 2 + 1] >> 4 & 0xf);
                    result[i * 4 + 2] = (byte)(block[i * 2 + 0] << 4 & 0xf0 | block[i * 2 + 0] & 0xf);
                    result[i * 4 + 3] = (byte)(block[i * 2 + 0] << 1 & 0xe0 | block[i * 2 + 0] >> 2 & 0x1C | block[i * 2 + 0] >> 5 & 0x03);
                }
                else
                {
                    result[i * 4 + 0] = (byte)(block[i * 2 + 1] << 3 & 0xf8 | block[i * 2 + 1] >> 2 & 0x07);
                    result[i * 4 + 1] = (byte)(block[i * 2 + 0] << 6 & 0xc0 | block[i * 2 + 1] >> 2 & 0x38 | block[i * 2 + 0] & 0x06 | block[i * 2 + 1] >> 7 & 0x01);
                    result[i * 4 + 2] = (byte)(block[i * 2 + 0] << 1 & 0xf8 | block[i * 2 + 0] >> 4 & 0x07);
                    result[i * 4 + 3] = 0xff;
                }
            }

            return result;
        }
        private static byte[] ConvertBlockFromRgba32(byte[] block)
        {
            byte[] result;

            result = new byte[64];

            for (int i = 0; i < block.Length / 4; i++)
            {
                result[i * 4 + 0] = block[i * 2 + 33];
                result[i * 4 + 1] = block[i * 2 + 32];
                result[i * 4 + 2] = block[i * 2 + 1];
                result[i * 4 + 3] = block[i * 2 + 0];
            }

            return result;
        }
        private static byte[] ConvertBlockFromCmpr(byte[] block)
        {
            byte[] result;
            byte[][] results;

            result = new byte[256];
            results = new byte[4][];

            for (int i = 0, x = 0, y = 0; i < block.Length / 8; i++)
            {
                results[i] = ConvertBlockFromQuaterCmpr(block, i << 3);

                Array.Copy(results[i], 0, result, x + y + 0, 16);
                Array.Copy(results[i], 16, result, x + y + 32, 16);
                Array.Copy(results[i], 32, result, x + y + 64, 16);
                Array.Copy(results[i], 48, result, x + y + 96, 16);

                x = 16 - x;
                if (x == 0)
                    y = 128;
            }

            return result;
        }

        private static byte[] ConvertBlockFromQuaterCmpr(byte[] block, int offset)
        {
            byte[][] palette;
            byte[] result;

            result = new byte[64];
            palette = new byte[4][];

            palette[0] = new byte[] { (byte)(block[offset + 1] << 3 & 0xf8), (byte)(block[offset + 0] << 5 & 0xe0 | block[offset + 1] >> 3 & 0x1c), (byte)(block[offset + 0] & 0xf8), 0xff };
            palette[1] = new byte[] { (byte)(block[offset + 3] << 3 & 0xf8), (byte)(block[offset + 2] << 5 & 0xe0 | block[offset + 3] >> 3 & 0x1c), (byte)(block[offset + 2] & 0xf8), 0xff };

            if (block[offset + 0] > block[offset + 2] || (block[offset + 0] == block[offset + 2] && block[offset + 1] > block[offset + 3]))
            {
                palette[2] = new byte[] { (byte)(((palette[0][0] << 1) + palette[1][0]) / 3), (byte)(((palette[0][1] << 1) + palette[1][1]) / 3), (byte)(((palette[0][2] << 1) + palette[1][2]) / 3), 0xff };
                palette[3] = new byte[] { (byte)((palette[0][0] + (palette[1][0] << 1)) / 3), (byte)((palette[0][1] + (palette[1][1] << 1)) / 3), (byte)((palette[0][2] + (palette[1][2] << 1)) / 3), 0xff };
            }
            else
            {
                palette[2] = new byte[] { (byte)((palette[0][0] + palette[1][0]) >> 1), (byte)((palette[0][1] + palette[1][1]) >> 1), (byte)((palette[0][2] + palette[1][2]) >> 1), 0xff };
                palette[3] = new byte[] { 0, 0, 0, 0 };
            }

            for (int i = 0; i < 4; i++)
            {
                palette[block[offset + i + 4] >> 6].CopyTo(result, i * 16 + 0);
                palette[block[offset + i + 4] >> 4 & 0x3].CopyTo(result, i * 16 + 4);
                palette[block[offset + i + 4] >> 2 & 0x3].CopyTo(result, i * 16 + 8);
                palette[block[offset + i + 4] & 0x3].CopyTo(result, i * 16 + 12);
            }

            return result;
        }

        private static byte[] ConvertBlockToI4(byte[] block)
        {
            byte[] result;

            result = new byte[32];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)(((block[i * 8 + 0] * 11 + block[i * 8 + 1] * 59 + block[i * 8 + 2] * 30) / 1700) << 4 | (block[i * 8 + 4] * 11 + block[i * 8 + 5] * 59 + block[i * 8 + 6] * 30) / 1700);
            }

            return result;
        }
        private static byte[] ConvertBlockToI8(byte[] block)
        {
            byte[] result;

            result = new byte[32];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)((block[i * 4 + 0] * 11 + block[i * 4 + 1] * 59 + block[i * 4 + 2] * 30) / 100);
            }

            return result;
        }
        private static byte[] ConvertBlockToIa4(byte[] block)
        {
            byte[] result;

            result = new byte[32];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)((block[i * 4 + 0] * 11 + block[i * 4 + 1] * 59 + block[i * 4 + 2] * 30) / 1700 | block[i * 4 + 3] / 0x11 << 4);
            }

            return result;
        }
        private static byte[] ConvertBlockToIa8(byte[] block)
        {
            byte[] result;

            result = new byte[32];

            for (int i = 0; i < result.Length / 2; i++)
            {
                result[i * 2 + 1] = (byte)((block[i * 4 + 0] * 11 + block[i * 4 + 1] * 59 + block[i * 4 + 2] * 30) / 100);
                result[i * 2 + 0] = block[i * 4 + 3];
            }

            return result;
        }
        private static byte[] ConvertBlockToRgb565(byte[] block)
        {
            byte[] result;

            result = new byte[32];

            for (int i = 0; i < result.Length / 2; i++)
            {
                result[i * 2 + 0] = (byte)(block[i * 4 + 2] & 0xf8 | block[i * 4 + 1] >> 5);
                result[i * 2 + 1] = (byte)(block[i * 4 + 0] >> 3 | block[i * 4 + 1] << 3 & 0xe0);
            }

            return result;
        }
        private static byte[] ConvertBlockToRgb5a3(byte[] block)
        {
            byte[] result;

            result = new byte[32];

            for (int i = 0; i < result.Length / 2; i++)
            {
                if (block[i * 4 + 3] < 0xe0)
                {
                    result[i * 2 + 0] = (byte)(block[i * 4 + 3] >> 1 & 0x70 | block[i * 4 + 2] >> 4);
                    result[i * 2 + 1] = (byte)(block[i * 4 + 1] & 0xf0 | block[i * 4 + 0] >> 4);
                }
                else
                {
                    result[i * 2 + 0] = (byte)(0x80 | block[i * 4 + 2] >> 1 & 0x7c | block[i * 4 + 1] >> 6);
                    result[i * 2 + 1] = (byte)(block[i * 4 + 0] >> 3 | block[i * 4 + 1] << 2 & 0xe0);
                }
            }

            return result;
        }
        private static byte[] ConvertBlockToRgba32(byte[] block)
        {
            byte[] result;

            result = new byte[64];

            for (int i = 0; i < result.Length / 4; i++)
            {
                result[i * 2 + 33] = block[i * 4 + 0];
                result[i * 2 + 32] = block[i * 4 + 1];
                result[i * 2 + 1] = block[i * 4 + 2];
                result[i * 2 + 0] = block[i * 4 + 3];
            }

            return result;
        }
        private static byte[] ConvertBlockToCmpr(byte[] block)
        {
            byte[] subBlock;
            byte[] result;

            result = new byte[32];
            subBlock = new byte[64];

            for (int i = 0, x = 0, y = 0; i < block.Length / 64; i++)
            {
                Array.Copy(block, x + y + 0, subBlock, 0, 16);
                Array.Copy(block, x + y + 32, subBlock, 16, 16);
                Array.Copy(block, x + y + 64, subBlock, 32, 16);
                Array.Copy(block, x + y + 96, subBlock, 48, 16);

                x = 16 - x;
                if (x == 0)
                    y = 128;

                ConvertBlockToQuaterCmpr(subBlock).CopyTo(result, i << 3);
            }

            return result;
        }

        private static byte[] ConvertBlockToQuaterCmpr(byte[] block)
        {
            int col1, col2, dist, temp;
            bool alpha;
            byte[][] palette;
            byte[] result;

            dist = col1 = col2 = -1;
            alpha = false;
            result = new byte[8];

            for (int i = 0; i < 15; i++)
            {
                if (block[i * 4 + 3] < 16)
                    alpha = true;
                else
                {
                    for (int j = i + 1; j < 16; j++)
                    {
                        temp = Distance(block, i * 4, block, j * 4);

                        if (temp > dist)
                        {
                            dist = temp;
                            col1 = i;
                            col2 = j;
                        }
                    }
                }
            }

            if (dist == -1)
            {
                palette = new byte[][] { new byte[] { 0, 0, 0, 0xff }, new byte[] { 0xff, 0xff, 0xff, 0xff }, null, null };
            }
            else
            {
                palette = new byte[4][];
                palette[0] = new byte[4];
                palette[1] = new byte[4];

                Array.Copy(block, col1 * 4, palette[0], 0, 3);
                palette[0][3] = 0xff;
                Array.Copy(block, col2 * 4, palette[1], 0, 3);
                palette[1][3] = 0xff;

                if (palette[0][0] >> 3 == palette[1][0] >> 3 && palette[0][1] >> 2 == palette[1][1] >> 2 && palette[0][2] >> 3 == palette[1][2] >> 3)
                    if (palette[0][0] >> 3 == 0 && palette[0][1] >> 2 == 0 && palette[0][2] >> 3 == 0)
                        palette[1][0] = palette[1][1] = palette[1][2] = 0xff;
                    else
                        palette[1][0] = palette[1][1] = palette[1][2] = 0x0;
            }

            result[0] = (byte)(palette[0][2] & 0xf8 | palette[0][1] >> 5);
            result[1] = (byte)(palette[0][1] << 3 & 0xe0 | palette[0][0] >> 3);
            result[2] = (byte)(palette[1][2] & 0xf8 | palette[1][1] >> 5);
            result[3] = (byte)(palette[1][1] << 3 & 0xe0 | palette[1][0] >> 3);

            if ((result[0] > result[2] || (result[0] == result[2] && result[1] >= result[3])) == alpha)
            {
                Array.Copy(result, 0, result, 4, 2);
                Array.Copy(result, 2, result, 0, 2);
                Array.Copy(result, 4, result, 2, 2);

                palette[2] = palette[0];
                palette[0] = palette[1];
                palette[1] = palette[2];
            }

            if (!alpha)
            {
                palette[2] = new byte[] { (byte)(((palette[0][0] << 1) + palette[1][0]) / 3), (byte)(((palette[0][1] << 1) + palette[1][1]) / 3), (byte)(((palette[0][2] << 1) + palette[1][2]) / 3), 0xff };
                palette[3] = new byte[] { (byte)((palette[0][0] + (palette[1][0] << 1)) / 3), (byte)((palette[0][1] + (palette[1][1] << 1)) / 3), (byte)((palette[0][2] + (palette[1][2] << 1)) / 3), 0xff };
            }
            else
            {
                palette[2] = new byte[] { (byte)((palette[0][0] + palette[1][0]) >> 1), (byte)((palette[0][1] + palette[1][1]) >> 1), (byte)((palette[0][2] + palette[1][2]) >> 1), 0xff };
                palette[3] = new byte[] { 0, 0, 0, 0 };
            }

            for (int i = 0; i < block.Length >> 4; i++)
            {
                result[4 + i] = (byte)(LeastDistance(palette, block, i * 16 + 0) << 6 | LeastDistance(palette, block, i * 16 + 4) << 4 | LeastDistance(palette, block, i * 16 + 8) << 2 | LeastDistance(palette, block, i * 16 + 12));
            }

            return result;
        }
        private static int LeastDistance(byte[][] palette, byte[] colour, int offset)
        {
            int dist, best, temp;

            if (colour[offset + 3] < 8)
                return 3;

            dist = int.MaxValue;
            best = 0;

            for (int i = 0; i < palette.Length; i++)
            {
                if (palette[i][3] != 0xff)
                    break;

                temp = Distance(palette[i], 0, colour, offset);

                if (temp < dist)
                {
                    if (temp == 0)
                        return i;

                    dist = temp;
                    best = i;
                }
            }

            return best;
        }
        private static int Distance(byte[] colour1, int offset1, byte[] colour2, int offset2)
        {
            int temp, val;

            temp = 0;

            for (int i = 0; i < 3; i++)
            {
                val = colour1[offset1 + i] - colour2[offset2 + i];
                temp += val * val;
            }

            return temp;
        }

    }

    public delegate byte[] ConvertBlockDelegate(byte[] block);
}
