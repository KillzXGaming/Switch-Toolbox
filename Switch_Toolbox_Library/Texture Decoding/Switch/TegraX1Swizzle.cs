using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public class TegraX1Swizzle
    {
        // TODO: Support 32 bit.
        [DllImport("tegra_swizzle", EntryPoint = "deswizzle_block_linear")]
        private static unsafe extern void DeswizzleBlockLinear(ulong width, ulong height, ulong depth, byte* source, ulong sourceLength, 
            byte[] destination, ulong destinationLength, ulong blockHeight, ulong bytesPerPixel);

        [DllImport("tegra_swizzle", EntryPoint = "swizzle_block_linear")]
        private static unsafe extern void SwizzleBlockLinear(ulong width, ulong height, ulong depth, byte* source, ulong sourceLength,
            byte[] destination, ulong destinationLength, ulong blockHeight, ulong bytesPerPixel);

        [DllImport("tegra_swizzle", EntryPoint = "swizzled_surface_size")]
        private static extern ulong GetSurfaceSize(ulong width, ulong height, ulong depth, ulong blockHeight, ulong bytesPerPixel);

        public static List<uint[]> GenerateMipSizes(TEX_FORMAT Format, uint Width, uint Height, uint Depth, uint SurfaceCount, uint MipCount, uint ImageSize)
        {
            List<uint[]> MipMapSizes = new List<uint[]>();

            uint bpp = STGenericTexture.GetBytesPerPixel(Format);
            uint blkWidth = STGenericTexture.GetBlockWidth(Format);
            uint blkHeight = STGenericTexture.GetBlockHeight(Format);
            uint blkDepth = STGenericTexture.GetBlockDepth(Format);

            uint blockHeight = GetBlockHeight(DIV_ROUND_UP(Height, blkHeight));
            uint BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;

            uint Pitch = 0;
            uint DataAlignment = 512;

            int linesPerBlockHeight = (1 << (int)BlockHeightLog2) * 8;

            uint ArrayCount = SurfaceCount;

            uint ArrayOffset = 0;
            for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
            {
                uint SurfaceSize = 0;
                int blockHeightShift = 0;

                uint[] MipOffsets = new uint[MipCount];

                for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                {
                    uint width = (uint)Math.Max(1, Width >> mipLevel);
                    uint height = (uint)Math.Max(1, Height >> mipLevel);
                    uint depth = (uint)Math.Max(1, Depth >> mipLevel);

                    uint size = DIV_ROUND_UP(width, blkWidth) * DIV_ROUND_UP(height, blkHeight) * bpp;
                    MipOffsets[mipLevel] = size;
                }
                ArrayOffset += (uint)(ImageSize / ArrayCount);

                MipMapSizes.Add(MipOffsets);
            }

            return MipMapSizes;
        }

        public static byte[] GetImageData(STGenericTexture texture, byte[] ImageData, int ArrayLevel, int MipLevel, int DepthLevel, int target = 1, bool LinearTileMode = false)
        {
            uint blkHeight = STGenericTexture.GetBlockHeight(texture.Format);
            uint blkDepth = STGenericTexture.GetBlockDepth(texture.Format);
            uint blockHeight = GetBlockHeight(DIV_ROUND_UP(texture.Height, blkHeight));
            uint BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;
            return GetImageData(texture, ImageData, ArrayLevel, MipLevel, DepthLevel, BlockHeightLog2, target, LinearTileMode);
        }

        public static byte[] GetImageData(STGenericTexture texture, byte[] ImageData, int ArrayLevel, int MipLevel, int DepthLevel, uint BlockHeightLog2, int target = 1, bool LinearTileMode = false)
        {
            uint bpp = STGenericTexture.GetBytesPerPixel(texture.Format);
            uint blkWidth = STGenericTexture.GetBlockWidth(texture.Format);
            uint blkHeight = STGenericTexture.GetBlockHeight(texture.Format);
            uint blkDepth = STGenericTexture.GetBlockDepth(texture.Format);

            uint TileMode = 0;
            if (LinearTileMode)
                TileMode = 1;
            uint numDepth = 1;
            if (texture.Depth > 1)
                numDepth = texture.Depth;

            int linesPerBlockHeight = (1 << (int)BlockHeightLog2) * 8;

            uint ArrayOffset = 0;
            for (int depthLevel = 0; depthLevel < numDepth; depthLevel++)
            {
                for (int arrayLevel = 0; arrayLevel < texture.ArrayCount; arrayLevel++)
                {
                    uint surfaceSize = 0;
                    int blockHeightShift = 0;

                    List<uint> mipOffsets = new List<uint>();

                    for (int mipLevel = 0; mipLevel < texture.MipCount; mipLevel++)
                    {
                        uint width = Math.Max(1, texture.Width >> mipLevel);
                        uint height = Math.Max(1, texture.Height >> mipLevel);
                        uint depth = Math.Max(1, texture.Depth >> mipLevel);

                        uint size = DIV_ROUND_UP(width, blkWidth) * DIV_ROUND_UP(height, blkHeight) * bpp;

                        Console.WriteLine($"size " + size);

                        if (pow2_round_up(DIV_ROUND_UP(height, blkWidth)) < linesPerBlockHeight)
                            blockHeightShift += 1;

                        uint widthInBlocks = DIV_ROUND_UP(width, blkWidth);
                        uint heightInBlocks = DIV_ROUND_UP(height, blkHeight);
                        uint depthInBlocks = DIV_ROUND_UP(depth, blkDepth);

                        //Calculate the mip size instead
                        var mipBlockHeightLog2 = (int)Math.Max(0, BlockHeightLog2 - blockHeightShift);
                        var mipBlockHeight = 1 << mipBlockHeightLog2;

                        mipOffsets.Add(surfaceSize);
                        surfaceSize += (uint)GetSurfaceSize(widthInBlocks, heightInBlocks, depthInBlocks, (ulong)mipBlockHeight, bpp);

                        //Get the first mip offset and current one and the total image size
                        int msize = (int)((mipOffsets[0] + ImageData.Length - mipOffsets[mipLevel]) / texture.ArrayCount);

                        // TODO: Avoid this copy.
                        byte[] mipData = Utils.SubArray(ImageData, ArrayOffset + mipOffsets[mipLevel], (uint)msize);

                        try
                        {
                            if (ArrayLevel == arrayLevel && MipLevel == mipLevel && DepthLevel == depthLevel)
                            {
                                // The set of swizzled addresses is at least as big as the set of linear addresses.
                                // When defined appropriately, we only require a single memory allocation for the output.
                                byte[] result = deswizzle(width, height, depth, blkWidth, blkHeight, blkDepth, target, bpp, TileMode, mipBlockHeightLog2, mipData);
                                return result;
                            }
                        }
                        catch (Exception e)
                        {
                            System.Windows.Forms.MessageBox.Show($"Failed to swizzle texture {texture.Text}!");
                            Console.WriteLine(e);

                            return new byte[0];
                        }
                    }
                    ArrayOffset += (uint)(ImageData.Length / texture.ArrayCount);
                }
            }
            return new byte[0];
        }

        private static unsafe byte[] SwizzleDeswizzleBlockLinear(uint width, uint height, uint depth, uint blkWidth, uint blkHeight, uint blkDepth,
            uint bpp, int blockHeightLog2, byte[] data, bool deswizzle)
        {
            // This function expects the surface dimensions in blocks rather than pixels for block compressed formats.
            // This ensures the bytes per pixel parameter is used correctly.
            width /= blkWidth;
            height /= blkHeight;
            depth /= blkDepth;

            // TODO: tegra_swizzle only allows valid block heights (1,2,..,32), so this will require some validity checks.
            var blockHeight = (ulong)(1 << blockHeightLog2);

            if (deswizzle)
            {
                var output = new byte[width * height * bpp];

                fixed (byte* dataPtr = data)
                {
                    DeswizzleBlockLinear(width, height, depth, dataPtr, (ulong)data.Length, output, (ulong)output.Length, blockHeight, bpp);
                }

                return output;
            }
            else
            {
                var surfaceSize = GetSurfaceSize(width, height, depth, blockHeight, bpp);
                var output = new byte[surfaceSize];

                fixed (byte* dataPtr = data)
                {
                    SwizzleBlockLinear(width, height, depth, dataPtr, (ulong)data.Length, output, (ulong)output.Length, blockHeight, bpp);
                }

                return output;
            }
        }


        /*---------------------------------------
         * 
         * Code ported from AboodXD's BNTX Extractor https://github.com/aboood40091/BNTX-Extractor/blob/master/swizzle.py
         * 
         *---------------------------------------*/

        // TODO: This doesn't seem to be entirely accurate for some Nutexb textures.
        public static uint GetBlockHeight(uint height)
        {
            uint blockHeight = pow2_round_up(height / 8);
            if (blockHeight > 16)
                blockHeight = 16;

            return blockHeight;
        }

        public static uint DIV_ROUND_UP(uint n, uint d)
        {
            return (n + d - 1) / d;
        }
        public static uint round_up(uint x, uint y)
        {
            return ((x - 1) | (y - 1)) + 1;
        }
        public static uint pow2_round_up(uint x)
        {
            x -= 1;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }

        private static byte[] SwizzlePitchLinear(uint width, uint height, uint depth, uint blkWidth, uint blkHeight, uint blkDepth, int roundPitch, uint bpp, int blockHeightLog2, byte[] data, bool deswizzle)
        {
            // TODO: Investigate doing this more efficiently in Rust.
            width = DIV_ROUND_UP(width, blkWidth);
            height = DIV_ROUND_UP(height, blkHeight);
            depth = DIV_ROUND_UP(depth, blkDepth);

            uint pitch;
            uint surfSize;

            pitch = width * bpp;

            if (roundPitch == 1)
                pitch = round_up(pitch, 32);

            surfSize = pitch * height;


            byte[] result = new byte[surfSize];

            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x++)
                {
                    uint pos;
                    uint pos_;

                    pos = y * pitch + x * bpp;

                    pos_ = (y * width + x) * bpp;

                    if (pos + bpp <= surfSize)
                    {
                        if (!deswizzle)
                            Array.Copy(data, pos, result, pos_, bpp);
                        else
                            Array.Copy(data, pos_, result, pos, bpp);
                    }
                }
            }
            return result;
        }

        public static byte[] deswizzle(uint width, uint height, uint depth, uint blkWidth, uint blkHeight, uint blkDepth, int roundPitch, uint bpp, uint tileMode, int blockHeightLog2, byte[] data)
        {
            if (tileMode == 1)
                return SwizzlePitchLinear(width, height, depth, blkWidth, blkHeight, blkDepth, roundPitch, bpp, blockHeightLog2, data, true);
            else
                return SwizzleDeswizzleBlockLinear(width, height, depth, blkWidth, blkHeight, blkDepth, bpp, blockHeightLog2, data, true);
        }

        public static byte[] swizzle(uint width, uint height, uint depth, uint blkWidth, uint blkHeight, uint blkDepth, int roundPitch, uint bpp, uint tileMode, int blockHeightLog2, byte[] data)
        {
            if (tileMode == 1)
                return SwizzlePitchLinear(width, height, depth, blkWidth, blkHeight, blkDepth, roundPitch, bpp, blockHeightLog2, data, false);
            else
                return SwizzleDeswizzleBlockLinear(width, height, depth, blkWidth, blkHeight, blkDepth, bpp, blockHeightLog2, data, false);
        }
    }
}