using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;

namespace Switch_Toolbox.Library
{
    public class ASTC
    {
        const uint Magic = 0x5CA1AB13;

        public byte BlockDimX;
        public byte BlockDimY;
        public byte BlockDimZ;
        public int X;
        public int Y;
        public int Z;

        public void WriteASTCHeader(FileWriter writer)
        {
            writer.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

            writer.Write(Magic);
            writer.Write(BlockDimX);
            writer.Write(BlockDimY);
            writer.Write(BlockDimZ);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }
    }
}
