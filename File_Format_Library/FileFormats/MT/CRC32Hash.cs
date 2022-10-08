using System.Text;

namespace CafeLibrary.M2
{
    class CRC32Hash
    {
        private static uint[] Table = new uint[256];

        private static bool IsInitialized = false;

        public static void Initialize()
        {
            uint Polynomial = 0xedb88320;

            for (uint Value, i = 0; i < Table.Length; i++)
            {
                Value = i;

                for (int j = 8; j > 0; --j)
                {
                    Value = (Value >> 1) ^ (Polynomial * (Value & 1));
                }

                Table[i] = Value;
            }

            IsInitialized = true;
        }

        public static uint Hash(byte[] Data)
        {
            if (!IsInitialized) Initialize();

            uint CRC = 0xffffffff;

            for (int i = 0; i < Data.Length; i++)
            {
                CRC = (CRC >> 8) ^ Table[(CRC & 0xff) ^ Data[i]];
            }

            return CRC;
        }

        public static uint Hash(string Text)
        {
            return Hash(Encoding.ASCII.GetBytes(Text));
        }

        public static uint HashNegated(byte[] Data)
        {
            return ~Hash(Data);
        }

        public static uint HashNegated(string Text)
        {
            return ~Hash(Text);
        }
    }
}