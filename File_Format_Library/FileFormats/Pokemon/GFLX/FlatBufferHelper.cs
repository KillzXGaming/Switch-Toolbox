using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatBuffers.Gfbmdl;
using FlatBuffers;

namespace FirstPlugin
{
    public class FlatBufferHelper
    {
        public static StringOffset[] CreateStringList(FlatBufferBuilder builder, List<string> strings)
        {
            StringOffset[] offsets = new StringOffset[strings.Count];
            for (int i = 0; i < strings.Count; i++)
                offsets[i] = builder.CreateString(strings[i]);
            return offsets;
        }

        public static Offset<Vector3> CreateVector(
            FlatBufferBuilder builder, Vector3? vector)
        {
            return Vector3.CreateVector3(builder,
                vector.Value.X,
                vector.Value.Y,
                vector.Value.Z);
        }

        public static Offset<Vector3> CreateVector(
            FlatBufferBuilder builder, Vector3 vector)
        {
            return Vector3.CreateVector3(builder,
                vector.X,
                vector.Y,
                vector.Z);
        }
    }
}
