using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace HyruleWarriors.G1M
{
    public interface IChunk
    {
        void Read(FileReader reader);
        void Write(FileWriter reader);
    }

    public class G1MCommon
    {
        public static T[] ParseStructArray<T>(FileReader reader) {
            int count = reader.ReadInt32();
            return reader.ReadMultipleStructs<T>(count).ToArray();
        }

        public static T[] ParseArray<T>(FileReader reader)
             where T : IChunk, new()
        {
            int count = reader.ReadInt32();
            T[] values = new T[count];
            for (int i = 0; i < count; i++)
            {
                values[i] = new T();
                values[i].Read(reader);
            }
            return values;
        }
    }
}
