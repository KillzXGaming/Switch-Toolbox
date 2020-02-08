using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public interface IExportableModel
    {
        List<STGenericObject> Meshes { get; }
        List<STGenericMaterial> Materials { get; }
        STSkeleton Skeleton { get; }
        List<STGenericTexture> TextureList { get; }
    }
}
