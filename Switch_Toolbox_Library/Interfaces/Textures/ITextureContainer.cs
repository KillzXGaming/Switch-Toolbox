using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public interface ITextureContainer
    {
        List<STGenericTexture> TextureList { get; set; }

        bool DisplayIcons { get; }
    }
}
