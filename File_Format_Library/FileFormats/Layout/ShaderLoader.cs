using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutBXLYT
{
    public class ShaderLoader
    {
        private static BflytShader cafeShader;
        private static BrlytShader revShader;
        private static BclytShader ctrShader;
        private static BloShader bloShader;

        public static BflytShader CafeShader
        {
            get
            {
                if (cafeShader == null)
                    cafeShader = new BflytShader();
                return cafeShader;
            }
        }
        public static BrlytShader RevShader
        {
            get
            {
                if (revShader == null)
                    revShader = new BrlytShader();
                return revShader;
            }
        }
        public static BclytShader CtrShader
        {
            get
            {
                if (ctrShader == null)
                    ctrShader = new BclytShader();
                return ctrShader;
            }
        }
        public static BloShader BLOShader
        {
            get
            {
                if (bloShader == null)
                    bloShader = new BloShader();
                return bloShader;
            }
        }
    }
}
