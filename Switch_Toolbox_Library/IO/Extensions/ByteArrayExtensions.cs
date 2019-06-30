using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace System
{
    public static class ByteArrayExtensions
    {
        public static T[] Slice<T>(this T[] arr, uint indexFrom, uint size)
        {
            T[] result = new T[size];
            Array.Copy(arr, indexFrom, result, 0, size);

            return result;
        }

        public static T[] SubArrayDeepClone<T>(this T[] data, int index, int length)
        {
            T[] arrCopy = new T[length];
            Array.Copy(data, index, arrCopy, 0, length);
            using (MemoryStream ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, arrCopy);
                ms.Position = 0;
                return (T[])bf.Deserialize(ms);
            }
        }
    }
}
