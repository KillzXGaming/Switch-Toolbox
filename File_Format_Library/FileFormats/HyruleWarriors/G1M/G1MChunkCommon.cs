using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyruleWarriors.G1M
{
    public class G1MChunkCommon
    {
        public string Magic { get; set; }
        public long ChunkPosition { get; set; }
        public uint ChunkSize { get; set; }
        public uint ChunkVersion { get; set; }
    }
}
