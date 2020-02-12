using Toolbox.Library.IO;

namespace LayoutBXLYT.Revolution
{
    public class TevSwapModeTable
    {
        public SwapMode[] SwapModes;

        public TevSwapModeTable() {
            SwapModes = new SwapMode[4];
            for (int i = 0; i < 4; i++)
                SwapModes[i] = new SwapMode();
        }

        public TevSwapModeTable(FileReader reader)
        {
            SwapModes = new SwapMode[4];
            for (int i = 0; i < 4; i++)
                SwapModes[i] = new SwapMode(reader);
        }

        public void Write(FileWriter writer)
        {
            for (int i = 0; i < 4; i++)
                SwapModes[i].Write(writer);
        }
    }

    public class SwapMode
    {
        public SwapChannel R { get; set; } = SwapChannel.Red;
        public SwapChannel G { get; set; } = SwapChannel.Green;
        public SwapChannel B { get; set; } = SwapChannel.Blue;
        public SwapChannel A { get; set; } = SwapChannel.Alpha;

        public SwapMode() { }

        public SwapMode(FileReader reader) {
            byte value = reader.ReadByte();
            R = (SwapChannel)((value >> 0) & 0x3);
            G = (SwapChannel)((value >> 2) & 0x3);
            B = (SwapChannel)((value >> 4) & 0x3);
            A = (SwapChannel)((value >> 6) & 0x3);
        }

        public void Write(FileWriter writer) {
            writer.Write(GetFlags());
        }

        private byte GetFlags()
        {
            byte value = 0;
            value |= (byte)(((byte)R & 0x3) << 0);
            value |= (byte)(((byte)G & 0x3) << 2);
            value |= (byte)(((byte)B & 0x3) << 4);
            value |= (byte)(((byte)A & 0x3) << 6);
            return value;
        }
    }
}
