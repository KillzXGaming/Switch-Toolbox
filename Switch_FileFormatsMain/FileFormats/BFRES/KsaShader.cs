using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syroot.NintenTools.NSW.Bfres;
using Bfres.Structs;

namespace FirstPlugin
{
    public class KsaShader
    {
        public static void LoadRenderInfo(FMAT mat, List<BfresRenderInfo> renderInfos)
        {
            foreach (var renderInfo in renderInfos)
            {
                switch (renderInfo.Name)
                {
                    case "renderPass":
                        mat.isTransparent = renderInfo.ValueString[0] == "xlu";
                        break;
                }
            }
        }
    }
}
