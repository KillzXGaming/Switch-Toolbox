using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public interface IExportableModel
    {
        IEnumerable<STGenericObject> ExportableMeshes { get; }
        IEnumerable<STGenericMaterial> ExportableMaterials { get; }
        IEnumerable<STGenericTexture> ExportableTextures { get; }
        STSkeleton ExportableSkeleton { get; }
    }
}
