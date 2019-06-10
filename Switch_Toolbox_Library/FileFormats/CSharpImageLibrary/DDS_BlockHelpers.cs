using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpImageLibrary.DDS
{
    public class DDS_BlockHelpers
    {
        //From https://github.com/KFreon/CSharpImageLibrary/blob/master/CSharpImageLibrary/DDS/DDS_BlockHelpers.cs
        internal static void Decompress8BitBlock(byte[] source, int sourceStart, byte[] destination, int decompressedStart, int decompressedLineLength, bool isSigned)
        {
            byte min = source[sourceStart];
            byte max = source[sourceStart + 1];

            byte[] Colours = Build8BitPalette(min, max, isSigned);

            // KFreon: Decompress pixels
            ulong bitmask = (ulong)source[sourceStart + 2] << 0 | (ulong)source[sourceStart + 3] << 8 | (ulong)source[sourceStart + 4] << 16 |   // KFreon: Read all 6 compressed bytes into single.
                (ulong)source[sourceStart + 5] << 24 | (ulong)source[sourceStart + 6] << 32 | (ulong)source[sourceStart + 7] << 40;


            // KFreon: Bitshift and mask compressed data to get 3 bit indicies, and retrieve indexed colour of pixel.
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int index = i * 4 + j;
                    int destPos = decompressedStart + j * 4 + (i * decompressedLineLength);
                    destination[destPos] = Colours[bitmask >> (index * 3) & 0x7];
                }
            }
        }

        /// <summary>
        /// Builds palette for 8 bit channel.
        /// </summary>
        /// <param name="min">First main colour (often actually minimum)</param>
        /// <param name="max">Second main colour (often actually maximum)</param>
        /// <param name="isSigned">true = sets signed alpha range (-254 -- 255), false = 0 -- 255</param>
        /// <returns>8 byte colour palette.</returns>
        internal static byte[] Build8BitPalette(byte min, byte max, bool isSigned)
        {
            byte[] Colours = new byte[8];
            Colours[0] = min;
            Colours[1] = max;

            // KFreon: Choose which type of interpolation is required
            if (min > max)
            {
                // KFreon: Interpolate other colours
                Colours[2] = (byte)((6d * min + 1d * max) / 7d);  // NO idea what the +3 is...not in the Microsoft spec, but seems to be everywhere else.
                Colours[3] = (byte)((5d * min + 2d * max) / 7d);
                Colours[4] = (byte)((4d * min + 3d * max) / 7d);
                Colours[5] = (byte)((3d * min + 4d * max) / 7d);
                Colours[6] = (byte)((2d * min + 5d * max) / 7d);
                Colours[7] = (byte)((1d * min + 6d * max) / 7d);
            }
            else
            {
                // KFreon: Interpolate other colours and add Opacity or something...
                Colours[2] = (byte)((4d * min + 1d * max) / 5d);
                Colours[3] = (byte)((3d * min + 2d * max) / 5d);
                Colours[4] = (byte)((2d * min + 3d * max) / 5d);
                Colours[5] = (byte)((1d * min + 4d * max) / 5d);
                Colours[6] = (byte)(isSigned ? -254 : 0);  // KFreon: snorm and unorm have different alpha ranges
                Colours[7] = 255;
            }

            return Colours;
        }
    }
}
