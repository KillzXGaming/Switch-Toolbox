using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin.CtrLibrary.Forms
{
    public interface IMaterialLoader
    {
        void LoadMaterial(H3DMaterialWrapper wrapper);
    }
}
