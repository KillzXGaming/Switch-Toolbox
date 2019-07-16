namespace Toolbox.Library
{
    internal class CRC
    {
        private static uint[] crcTable;

        public const uint INITIAL_CRC = 0xFFFFFFFF;

        private static void MakeCRCTable()
        {
            crcTable = new uint[256];
            uint c, n, k;

            for (n = 0; n < crcTable.Length; n++)
            {
                c = n;
                for (k = 0; k < 8; k++)
                {
                    if ((c & 1) != 0)
                    {
                        c = 0xedb88320 ^ (c >> 1);
                    }
                    else
                    {
                        c = c >> 1;
                    }
                }
                crcTable[n] = c;
            }
        }

        public static uint UpdateCRC(uint crc, byte[] bytes)
        {
            uint c = crc;
            uint n;
            if (crcTable == null)
                MakeCRCTable();
            for (n = 0; n < bytes.Length; n++)
            {
                c = crcTable[(c ^ bytes[n]) & 0xff] ^ (c >> 8);
            }
            return c;


        }

        public static uint Calculate(byte[] bytes)
        {
            return UpdateCRC(INITIAL_CRC, bytes);
        }
    }
}
