using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;

namespace FirstPlugin
{
    public class BMDTextureMap : STGenericMatTexture
    {
        BMDMaterialWrapper material;
        public BMDTextureMap(BMDMaterialWrapper mat)
        {
            material = mat;
        }

        public int TextureIndex { get; set; }
    }
}
