using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switch_Toolbox.Library
{
    public interface ITextureContainer
    {
        Dictionary<string, STGenericTexture> Textures { get; set; }
    }
}
