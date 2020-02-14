using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;

namespace LayoutBXLYT.GCBLO
{
    public class Material : BxlytMaterial
    {
        public Material()
        {
            WhiteColor = STColor8.White;
            BlackColor = STColor8.Black;
            TevStages = new BxlytTevStage[0];
            TextureMaps = new BxlytTextureRef[0];
        }
    }
}
