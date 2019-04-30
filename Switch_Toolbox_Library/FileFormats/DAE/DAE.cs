using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collada141;

namespace Switch_Toolbox.Library
{
    public class DAE
    {
        public bool UseTransformMatrix = true;

        public void LoadFile(string FileName)
        {
            COLLADA collada = COLLADA.Load(FileName);

            foreach (var item in collada.Items)
            {
                if (item is library_geometries)
                {

                }
            }
        }

        private void LoadGeometry()
        {

        }
    }
}
