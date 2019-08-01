using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    //A texture list to display icons on a treeview
    //Note these will load and attach an icon when the parent treenode is expanded!
    public interface ITextureIconLoader
    {
        List<STGenericTexture> IconTextureList { get; set; }
    }
}
