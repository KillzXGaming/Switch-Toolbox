using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.Animations;

namespace FirstPlugin
{
    internal class TRANM : GFBANM, IFileFormat, IAnimationContainer, IConvertableTextFormat
    {
        public new bool Identify(System.IO.Stream stream)
        {
            return Utils.GetExtension(FileName) == ".tranm";
        }
    }
}