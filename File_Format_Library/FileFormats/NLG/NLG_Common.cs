using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin
{
    public class NLG_Common
    {
        public static uint StringToHash(string name, bool caseSensative = false)
        {
            //From (Works as tested comparing hashbin strings/hashes
            //https://gist.github.com/RoadrunnerWMC/f4253ef38c8f51869674a46ee73eaa9f
            byte[] data = Encoding.Default.GetBytes(name);

            int h = -1;
            for (int i = 0; i < data.Length; i++)
            {
                int c = (int)data[i];
                if (caseSensative && ((c - 65) & 0xFFFFFFFF) <= 0x19)
                    c |= 0x20;

                h = (int)((h * 33 + c) & 0xFFFFFFFF);
            }

            return (uint)h;
        }
    }
}
