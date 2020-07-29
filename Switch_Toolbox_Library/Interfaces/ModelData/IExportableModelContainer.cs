using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public interface IExportableModelContainer
    {
        IEnumerable<STGenericModel> ExportableModels { get; }
        IEnumerable<STGenericTexture> ExportableTextures { get; }
    }
}
