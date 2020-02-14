using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.GCBLO
{
    public static class MAT1
    {
        public static List<Material> ReadMaterials(
            FileReader reader, BLOHeader header)
        {
            List<Material> materials = new List<Material>();

            ushort count = reader.ReadUInt16();
            reader.ReadUInt16(); //0xFF
            uint materialDataOffset = reader.ReadUInt32();

            for (int i = 0; i < count; i++)
                materials.Add(new Material());

            return materials;
        }
    }
}
