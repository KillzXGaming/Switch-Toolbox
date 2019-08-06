using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    /// <summary>
    /// A texture list to display icons on a treeview
    /// Note these will load and attach an icon when the parent treenode is expanded!
    /// </summary>
    public interface ITextureIconLoader
    {
        List<STGenericTexture> IconTextureList { get; set; }
    }

    /// <summary>
    /// Reprenets a single texture that loads an icon. 
    /// These will check both on expand, and on root
    /// </summary>
    public interface ISingleTextureIconLoader
    {
        STGenericTexture IconTexture { get;}
    }
}
