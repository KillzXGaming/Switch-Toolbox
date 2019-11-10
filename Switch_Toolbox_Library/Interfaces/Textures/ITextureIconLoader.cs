using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    /// <summary>
    /// Reprenets a single texture that loads an icon. 
    /// These will check both on expand, and on root
    /// </summary>
    public interface ISingleTextureIconLoader
    {
        STGenericTexture IconTexture { get;}
    }
}
