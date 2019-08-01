using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    //A texture list to display icons on a treeview
    //Attach this to an IFileFormat
    public interface ITextureIconLoader
    {
        List<STGenericTexture> IconTextureList { get; set; }
    }
}
