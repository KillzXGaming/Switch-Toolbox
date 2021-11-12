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
        // Swizzle code and surface calculations are performed using an efficient Rust implementation.
        // C# code can call the Rust code using the library's C API.
        // Documentation, code, and tests for the tegra_swizzle Rust library can be found here:
        // https://github.com/ScanMountGoat/nutexb_swizzle

        // TODO: Find a cleaner way to support both 32 and 64 bit binaries.
        // 64 Bit.
        [DllImport("tegra_swizzle_x64", EntryPoint = "deswizzle_block_linear")]
        private static unsafe extern void DeswizzleBlockLinearX64(ulong width, ulong height, ulong depth, byte* source, ulong sourceLength, 
            byte[] destination, ulong destinationLength, ulong blockHeight, ulong bytesPerPixel);

        [DllImport("tegra_swizzle_x64", EntryPoint = "swizzle_block_linear")]
        private static unsafe extern void SwizzleBlockLinearX64(ulong width, ulong height, ulong depth, byte* source, ulong sourceLength,
            byte[] destination, ulong destinationLength, ulong blockHeight, ulong bytesPerPixel);

        [DllImport("tegra_swizzle_x64", EntryPoint = "swizzled_surface_size")]
        private static extern ulong GetSurfaceSizeX64(ulong width, ulong height, ulong depth, ulong blockHeight, ulong bytesPerPixel);
        
        [DllImport("tegra_swizzle_x64", EntryPoint = "block_height_mip0")]
        private static extern ulong BlockHeightMip0X64(ulong height);

        [DllImport("tegra_swizzle_x64", EntryPoint = "mip_block_height")]
        private static extern ulong MipBlockHeightX64(ulong mipHeight, ulong blockHeightMip0);

        // 32 Bit.
        [DllImport("tegra_swizzle_x86", EntryPoint = "deswizzle_block_linear")]
        private static unsafe extern void DeswizzleBlockLinearX86(uint width, uint height, uint depth, byte* source, uint sourceLength,
            byte[] destination, uint destinationLength, uint blockHeight, uint bytesPerPixel);

        [DllImport("tegra_swizzle_x86", EntryPoint = "swizzle_block_linear")]
        private static unsafe extern void SwizzleBlockLinearX86(uint width, uint height, uint depth, byte* source, uint sourceLength,
            byte[] destination, uint destinationLength, uint blockHeight, uint bytesPerPixel);

        [DllImport("tegra_swizzle_x86", EntryPoint = "swizzled_surface_size")]
        private static extern uint GetSurfaceSizeX86(uint width, uint height, uint depth, uint blockHeight, uint bytesPerPixel);

        [DllImport("tegra_swizzle_x86", EntryPoint = "block_height_mip0")]
        private static extern uint BlockHeightMip0X86(uint height);

        [DllImport("tegra_swizzle_x86", EntryPoint = "mip_block_height")]
        private static extern uint MipBlockHeightX86(uint mipHeight, uint blockHeightMip0);

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

            var blockHeightMip0 = GetBlockHeight(DIV_ROUND_UP(texture.Height, blkHeight));

            uint arrayOffset = 0;
            // TODO: Why is depth done like this?
            for (int depthLevel = 0; depthLevel < numDepth; depthLevel++)
            {
                for (int arrayLevel = 0; arrayLevel < texture.ArrayCount; arrayLevel++)
                {
                    var mipOffset = 0u;

                    for (int mipLevel = 0; mipLevel < texture.MipCount; mipLevel++)
                    {
                        uint width = Math.Max(1, texture.Width >> mipLevel);
                        uint height = Math.Max(1, texture.Height >> mipLevel);
                        uint depth = Math.Max(1, texture.Depth >> mipLevel);

                        uint widthInBlocks = DIV_ROUND_UP(width, blkWidth);
                        uint heightInBlocks = DIV_ROUND_UP(height, blkHeight);
                        uint depthInBlocks = DIV_ROUND_UP(depth, blkDepth);

                        // tegra_swizzle only allows block heights supported by the TRM (1,2,4,8,16,32).
                        var mipBlockHeightLog2 = (int)Math.Log(GetMipBlockHeight(heightInBlocks, blockHeightMip0), 2);
                        var mipBlockHeight = 1 << Math.Max(Math.Min(mipBlockHeightLog2, 5), 0);

                        uint mipSize;
                        if (Environment.Is64BitProcess)
                            mipSize = (uint)GetSurfaceSizeX64(widthInBlocks, heightInBlocks, depthInBlocks, (ulong)mipBlockHeight, bpp);
                        else
                            mipSize = (uint)GetSurfaceSizeX86(widthInBlocks, heightInBlocks, depthInBlocks, (uint)mipBlockHeight, bpp);

                        // TODO: Avoid this copy.
                        byte[] mipData = Utils.SubArray(ImageData, arrayOffset + mipOffset, mipSize);

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

                        mipOffset += mipSize;
                    }

                    arrayOffset += (uint)(ImageData.Length / texture.ArrayCount);
                }
            }
            return new byte[0];
        }

        private static unsafe byte[] SwizzleDeswizzleBlockLinear(uint width, uint height, uint depth, uint blkWidth, uint blkHeight, uint blkDepth,
            uint bpp, int blockHeightLog2, byte[] data, bool deswizzle)
        {
            // This function expects the surface dimensions in blocks rather than pixels for block compressed formats.
            // This ensures the bytes per pixel parameter is used correctly.
            width = DIV_ROUND_UP(width, blkWidth);
            height = DIV_ROUND_UP(height, blkHeight);
            depth = DIV_ROUND_UP(depth, blkDepth);

            // tegra_swizzle only allows block heights supported by the TRM (1,2,4,8,16,32).
            var blockHeight = (ulong)(1 << Math.Max(Math.Min(blockHeightLog2, 5), 0));

            if (deswizzle)
            {
                var output = new byte[width * height * bpp];

                fixed (byte* dataPtr = data)
                {
                    if (Environment.Is64BitProcess)
                        DeswizzleBlockLinearX64(width, height, depth, dataPtr, (ulong)data.Length, output, (ulong)output.Length, blockHeight, bpp);
                    else
                        DeswizzleBlockLinearX86(width, height, depth, dataPtr, (uint)data.Length, output, (uint)output.Length, (uint)blockHeight, bpp);
                }

                return output;
            }
            else
            {
                ulong surfaceSize;
                if (Environment.Is64BitProcess)
                    surfaceSize = GetSurfaceSizeX64(width, height, depth, blockHeight, bpp);
                else
                    surfaceSize = GetSurfaceSizeX86(width, height, depth, (uint)blockHeight, bpp);

                var output = new byte[surfaceSize];

                fixed (byte* dataPtr = data)
                {
                    if (Environment.Is64BitProcess)
                        SwizzleBlockLinearX64(width, height, depth, dataPtr, (ulong)data.Length, output, (ulong)output.Length, blockHeight, bpp);
                    else
                        SwizzleBlockLinearX86(width, height, depth, dataPtr, (uint)data.Length, output, (uint)output.Length, (uint)blockHeight, bpp);
                }

                return output;
            }
        }

        public static uint GetBlockHeight(uint heightInBytes)
        {
            if (Environment.Is64BitProcess)
                return (uint)BlockHeightMip0X64(heightInBytes);
            else
                return BlockHeightMip0X86(heightInBytes);
        }

        public static uint GetMipBlockHeight(uint mipHeightInBytes, uint blockHeightMip0)
        {
            if (Environment.Is64BitProcess)
                return (uint)MipBlockHeightX64(mipHeightInBytes, blockHeightMip0);
            else
                return MipBlockHeightX86(mipHeightInBytes, blockHeightMip0);
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


        /*---------------------------------------
         * 
         * Code ported from AboodXD's BNTX Extractor https://github.com/aboood40091/BNTX-Extractor/blob/master/swizzle.py
         * 
         *---------------------------------------*/
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
    }
}