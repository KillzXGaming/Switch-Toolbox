using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Toolbox.Library
{
    public class Imaging
    {
        public enum Channel
        {
            Red,
            Green,
            Blue,
            Alpha,
            Zero,
            One,
        }
        public static Bitmap GetLoadingImage()
        {
            return Properties.Resources.LoadingImage;
        }
    }
}
