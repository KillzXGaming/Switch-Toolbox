using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.IO
{
    public static class ArrayExtension
    {
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            var dest = new List<T>(array);
            dest.RemoveAt(index);
            return dest.ToArray();
        }

        public static T[] AddToArray<T>(this T[] array, T item)
        {
            var dest = new List<T>(array);
            dest.Add(item);
            return dest.ToArray();
        }
        
    }
}
