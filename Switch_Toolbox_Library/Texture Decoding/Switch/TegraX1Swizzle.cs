using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Toolbox.Library
{
    public class TegraX1Swizzle
    {
        // Swizzle code and surface calculations are performed using an efficient Rust implementation.
        // C# code can call the Rust code using the library's C API.
        // Github: https://github.com/ScanMountGoat/tegra_swizzle
        // FFI Docs: https://docs.rs/tegra_swizzle/0.3.1/tegra_swizzle/ffi/index.html

        // 64 Bit.
        [StructLayout(LayoutKind.Sequential)]
        struct BlockDimX64
        {
            public ulong width;
            public ulong height;
            public ulong depth;
        }

        [DllImport("tegra_swizzle_x64", EntryPoint = "deswizzle_surface")]
        private static unsafe extern void DeswizzleSurfaceX64(ulong width, ulong height, ulong depth,
            byte* source, ulong sourceLength,
            byte* destination, ulong destinationLength,
            BlockDimX64 blockDim, ulong blockHeightMip0, ulong bytesPerPixel,
            ulong mipmapCount, ulong arrayCount);

        [DllImport("tegra_swizzle_x64", EntryPoint = "swizzle_surface")]
        private static unsafe extern void SwizzleSurfaceX64(ulong width, ulong height, ulong depth,
            byte* source, ulong sourceLength,
            byte* destination, ulong destinationLength,
            BlockDimX64 blockDim, ulong blockHeightMip0, ulong bytesPerPixel,
            ulong mipmapCount, ulong arrayCount);

        [DllImport("tegra_swizzle_x64", EntryPoint = "deswizzle_block_linear")]
        private static unsafe extern void DeswizzleBlockLinearX64(ulong width, ulong height, ulong depth, byte* source, ulong sourceLength,
            byte* destination, ulong destinationLength, ulong blockHeight, ulong bytesPerPixel);

        [DllImport("tegra_swizzle_x64", EntryPoint = "swizzle_block_linear")]
        private static unsafe extern void SwizzleBlockLinearX64(ulong width, ulong height, ulong depth, byte* source, ulong sourceLength,
            byte* destination, ulong destinationLength, ulong blockHeight, ulong bytesPerPixel);

        [DllImport("tegra_swizzle_x64", EntryPoint = "swizzled_surface_size")]
        private static extern ulong SwizzledSurfaceSizeX64(ulong width, ulong height, ulong depth, BlockDimX64 blockDim, ulong blockHeightMip0, ulong bytesPerPixel, ulong mipmapCount, ulong arrayCount);

        [DllImport("tegra_swizzle_x64", EntryPoint = "deswizzled_surface_size")]
        private static extern ulong DeswizzledSurfaceSizeX64(ulong width, ulong height, ulong depth, BlockDimX64 blockDim, ulong bytesPerPixel, ulong mipmapCount, ulong arrayCount);

        [DllImport("tegra_swizzle_x64", EntryPoint = "block_height_mip0")]
        private static extern ulong BlockHeightMip0X64(ulong height);

        [DllImport("tegra_swizzle_x64", EntryPoint = "mip_block_height")]
        private static extern ulong MipBlockHeightX64(ulong mipHeight, ulong blockHeightMip0);

        // 32 Bit.
        [StructLayout(LayoutKind.Sequential)]
        struct BlockDimX86
        {
            public uint width;
            public uint height;
            public uint depth;
        }

        [DllImport("tegra_swizzle_x86", EntryPoint = "deswizzle_surface")]
        private static unsafe extern void DeswizzleSurfaceX86(uint width, uint height, uint depth,
            byte* source, uint sourceLength,
            byte* destination, uint destinationLength,
            BlockDimX86 blockDim, uint blockHeightMip0, uint bytesPerPixel,
            uint mipmapCount, uint arrayCount);

        [DllImport("tegra_swizzle_x86", EntryPoint = "swizzle_surface")]
        private static unsafe extern void SwizzleSurfaceX86(uint width, uint height, uint depth,
            byte* source, uint sourceLength,
            byte* destination, uint destinationLength,
            BlockDimX86 blockDim, uint blockHeightMip0, uint bytesPerPixel,
            uint mipmapCount, uint arrayCount);

        [DllImport("tegra_swizzle_x86", EntryPoint = "deswizzle_block_linear")]
        private static unsafe extern void DeswizzleBlockLinearX86(uint width, uint height, uint depth, byte* source, uint sourceLength,
            byte* destination, uint destinationLength, uint blockHeight, uint bytesPerPixel);

        [DllImport("tegra_swizzle_x86", EntryPoint = "swizzle_block_linear")]
        private static unsafe extern void SwizzleBlockLinearX86(uint width, uint height, uint depth, byte* source, uint sourceLength,
            byte* destination, uint destinationLength, uint blockHeight, uint bytesPerPixel);

        [DllImport("tegra_swizzle_x86", EntryPoint = "swizzled_surface_size")]
        private static extern uint SwizzledSurfaceSizeX86(uint width, uint height, uint depth, BlockDimX86 blockDim, uint blockHeightMip0, uint bytesPerPixel, uint mipmapCount, uint arrayCount);

        [DllImport("tegra_swizzle_x86", EntryPoint = "deswizzled_surface_size")]
        private static extern uint DeswizzledSurfaceSizeX86(uint width, uint height, uint depth, BlockDimX86 blockDim, uint bytesPerPixel, uint mipmapCount, uint arrayCount);

        [DllImport("tegra_swizzle_x86", EntryPoint = "block_height_mip0")]
        private static extern uint BlockHeightMip0X86(uint height);

        [DllImport("tegra_swizzle_x86", EntryPoint = "mip_block_height")]
        private static extern uint MipBlockHeightX86(uint mipHeight, uint blockHeightMip0);

        public static List<uint[]> GenerateMipSizes(TEX_FORMAT Format, uint Width, uint Height, uint Depth, uint SurfaceCount, uint MipCount)
        {
            List<uint[]> MipMapSizes = new List<uint[]>();

            uint bpp = STGenericTexture.GetBytesPerPixel(Format);
            uint blkWidth = STGenericTexture.GetBlockWidth(Format);
            uint blkHeight = STGenericTexture.GetBlockHeight(Format);
            uint blkDepth = STGenericTexture.GetBlockDepth(Format);

            for (int arrayLevel = 0; arrayLevel < SurfaceCount; arrayLevel++)
            {
                uint[] MipOffsets = new uint[MipCount];

                for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                {
                    uint width = Math.Max(1, Width >> mipLevel);
                    uint height = Math.Max(1, Height >> mipLevel);
                    uint depth = Math.Max(1, Depth >> mipLevel);

                    uint size = DIV_ROUND_UP(width, blkWidth) * DIV_ROUND_UP(height, blkHeight) * DIV_ROUND_UP(depth, blkDepth) * bpp;
                    MipOffsets[mipLevel] = size;
                }

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

        public static byte[] GetDirectImageData(STGenericTexture texture, byte[] ImageData, int mipLevel, int target = 1, bool LinearTileMode = false)
        {
            uint blkWidth = STGenericTexture.GetBlockWidth(texture.Format);
            uint blkHeight = STGenericTexture.GetBlockHeight(texture.Format);
            uint blkDepth = STGenericTexture.GetBlockDepth(texture.Format);
            var blockHeightMip0 = GetBlockHeight(DIV_ROUND_UP(texture.Height, blkHeight));
            uint bpp = STGenericTexture.GetBytesPerPixel(texture.Format);
            uint TileMode = LinearTileMode ? 1u : 0u;

            uint width = Math.Max(1, texture.Width >> mipLevel);
            uint height = Math.Max(1, texture.Height >> mipLevel);
            uint depth = Math.Max(1, texture.Depth >> mipLevel);
            uint heightInBlocks = DIV_ROUND_UP(height, blkHeight);
            // tegra_swizzle only allows block heights supported by the TRM (1,2,4,8,16,32).
            var mipBlockHeightLog2 = (int)Math.Log(GetMipBlockHeight(heightInBlocks, blockHeightMip0), 2);

            try
            {
                byte[] result = deswizzle(width, height, depth, blkWidth, blkHeight, blkDepth, target, bpp, TileMode, mipBlockHeightLog2, ImageData);
                return result;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show($"Failed to swizzle texture {texture.Text}!");
                Console.WriteLine(e);

                return new byte[0];
            }
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

            uint width = texture.Width;
            uint height = texture.Height;
            uint depth = Math.Max(1, texture.Depth);

            var blockHeightMip0 = GetBlockHeight(DIV_ROUND_UP(height, blkHeight));

            var blockDim = new BlockDimX64 { width = blkWidth, height = blkHeight, depth = blkDepth };

            uint arrayOffset = 0;
            // TODO: Why is depth done like this?
            for (int depthLevel = 0; depthLevel < depth; depthLevel++)
            {
                for (int arrayLevel = 0; arrayLevel < texture.ArrayCount; arrayLevel++)
                {
                    var mipOffset = 0u;

                    for (int mipLevel = 0; mipLevel < texture.MipCount; mipLevel++)
                    {
                        uint mipWidth = Math.Max(1, width >> mipLevel);
                        uint mipHeight = Math.Max(1, height >> mipLevel);
                        uint mipDepth = Math.Max(1, depth >> mipLevel);

                        uint mipHeightInBlocks = DIV_ROUND_UP(mipHeight, blkHeight);

                        // tegra_swizzle only allows block heights supported by the TRM (1,2,4,8,16,32).
                        var mipBlockHeightLog2 = (int)Math.Log(GetMipBlockHeight(mipHeightInBlocks, blockHeightMip0), 2);

                        uint mipSize = (uint)SwizzledSurfaceSizeX64(mipWidth, mipHeight, mipDepth, blockDim, blockHeightMip0, bpp, 1, 1);
                        // TODO: Avoid this copy.
                        byte[] mipData = Utils.SubArray(ImageData, arrayOffset + mipOffset, mipSize);

                        try
                        {
                            if (ArrayLevel == arrayLevel && MipLevel == mipLevel && DepthLevel == depthLevel)
                            {
                                // The set of swizzled addresses is at least as big as the set of linear addresses.
                                // When defined appropriately, we only require a single memory allocation for the output.
                                byte[] result = deswizzle(mipWidth, mipHeight, mipDepth, blkWidth, blkHeight, blkDepth, target, bpp, TileMode, mipBlockHeightLog2, mipData);
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
            uint bpp, int blockHeightLog2, byte[] data, bool deswizzle, uint size, uint alignment = 512)
        {

            // tegra_swizzle only allows block heights supported by the TRM (1,2,4,8,16,32).
            var blockHeightMip0 = (ulong)(1 << Math.Max(Math.Min(blockHeightLog2, 5), 0));

            if (deswizzle)
            {
                // This function expects the surface dimensions in blocks rather than pixels for block compressed formats.
                // This ensures the bytes per pixel parameter is used correctly.
                width = DIV_ROUND_UP(width, blkWidth);
                height = DIV_ROUND_UP(height, blkHeight);
                depth = DIV_ROUND_UP(depth, blkDepth);

                var output = new byte[width * height * depth * bpp];

                DeswizzleBlockLinear(width, height, depth, bpp, data, (uint)blockHeightMip0, output);

                return output;
            }
            else
            {
                var blockDim = new BlockDimX86 { width = blkWidth, height = blkHeight, depth = blkDepth };

                ulong surfaceSize = SwizzledSurfaceSize(width, height, depth, blockDim, (uint)blockHeightMip0, bpp, 1, 1);
                var output = new byte[surfaceSize];

                // This function expects the surface dimensions in blocks rather than pixels for block compressed formats.
                // This ensures the bytes per pixel parameter is used correctly.
                width = DIV_ROUND_UP(width, blkWidth);
                height = DIV_ROUND_UP(height, blkHeight);
                depth = DIV_ROUND_UP(depth, blkDepth);

                SwizzleBlockLinear(width, height, depth, bpp, data, (uint)blockHeightMip0, output);

                return output;
            }
        }

        private static unsafe void SwizzleBlockLinear(uint width, uint height, uint depth, uint bpp, byte[] data, uint blockHeightMip0, byte[] output)
        {
            fixed (byte* dataPtr = data)
            {
                fixed (byte* outputPtr = output)
                {
                    if (Environment.Is64BitProcess)
                        SwizzleBlockLinearX64(width, height, depth, dataPtr, (ulong)data.Length, outputPtr, (ulong)output.Length, blockHeightMip0, bpp);
                    else
                        SwizzleBlockLinearX86(width, height, depth, dataPtr, (uint)data.Length, outputPtr, (uint)output.Length, blockHeightMip0, bpp);
                }
            }
        }

        private static unsafe void DeswizzleBlockLinear(uint width, uint height, uint depth, uint bpp, byte[] data, uint blockHeightMip0, byte[] output)
        {
            fixed (byte* dataPtr = data)
            {
                fixed (byte* outputPtr = output)
                {
                    if (Environment.Is64BitProcess)
                        DeswizzleBlockLinearX64(width, height, depth, dataPtr, (ulong)data.Length, outputPtr, (ulong)output.Length, blockHeightMip0, bpp);
                    else
                        DeswizzleBlockLinearX86(width, height, depth, dataPtr, (uint)data.Length, outputPtr, (uint)output.Length, blockHeightMip0, bpp);
                }
            }
        }

        private static ulong SwizzledSurfaceSize(uint width, uint height, uint depth, BlockDimX86 blockDim, uint blockHeightMip0, uint bytesPerPixel, uint mipmapCount, uint arrayCount)
        {
            if (Environment.Is64BitProcess)
            {
                var blockDimX64 = new BlockDimX64 { width = blockDim.width, height = blockDim.height, depth = blockDim.depth };
                return SwizzledSurfaceSizeX64(width, height, depth, blockDimX64, blockHeightMip0, bytesPerPixel, mipmapCount, arrayCount);
            }
            else
            {
                return SwizzledSurfaceSizeX86(width, height, depth, blockDim, blockHeightMip0, bytesPerPixel, mipmapCount, arrayCount);
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

        // TODO: These should not be public.
        public static byte[] deswizzle(uint width, uint height, uint depth, uint blkWidth, uint blkHeight, uint blkDepth, int roundPitch, uint bpp, uint tileMode, int blockHeightLog2, byte[] data)
        {
            if (tileMode == 1)
                return SwizzlePitchLinear(width, height, depth, blkWidth, blkHeight, blkDepth, roundPitch, bpp, blockHeightLog2, data, true, 0);
            else
                return SwizzleDeswizzleBlockLinear(width, height, depth, blkWidth, blkHeight, blkDepth, bpp, blockHeightLog2, data, true, 0);
        }

        public static byte[] swizzle(uint width, uint height, uint depth, uint blkWidth, uint blkHeight, uint blkDepth, int roundPitch, uint bpp, uint tileMode, int blockHeightLog2, byte[] data, uint size)
        {
            if (tileMode == 1)
                return SwizzlePitchLinear(width, height, depth, blkWidth, blkHeight, blkDepth, roundPitch, bpp, blockHeightLog2, data, false, size);
            else
                return SwizzleDeswizzleBlockLinear(width, height, depth, blkWidth, blkHeight, blkDepth, bpp, blockHeightLog2, data, false, size);
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

        private static byte[] SwizzlePitchLinear(uint width, uint height, uint depth, uint blkWidth, uint blkHeight, uint blkDepth, int roundPitch, uint bpp, int blockHeightLog2, byte[] data, bool deswizzle, uint size)
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

            for (uint z = 0; z < depth; z++)
            {
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
            }

            return result;
        }
    }
}