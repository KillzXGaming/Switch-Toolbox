using System;
using System.Runtime.InteropServices;

namespace Toolbox.Library.IO
{
    public class MarioTennisCmp32
    {
        [DllImport("Lib/LibTennis32.dll", CallingConvention = CallingConvention.Cdecl)]
       public static extern void DecompressBuffer(IntPtr output, IntPtr input, uint len);
    }
}
