using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public interface ITexture
    {
        uint MipCount { get; set; }
        uint ArrayCount { get; set; }
        uint Width { get; set; }
        uint Height { get; set; }
    }
}
