using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutBXLYT.Revolution
{
    public class ShaderGenerator
    {


        private string GenerateTexSource(TexCoordGenSource src)
        {
            switch (src)
            {
                case TexCoordGenSource.GX_TG_POS: return "vec4(a_Position, 1.0)";

                default:
                    throw new Exception("Unsupported tex source " + src);
            }
        }
    }
}
