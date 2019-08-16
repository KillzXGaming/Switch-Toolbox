using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DKCTF
{
    public class PAK
    {

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class PakHeader
    {
        CFormDescriptor PackForm;
    }
}
