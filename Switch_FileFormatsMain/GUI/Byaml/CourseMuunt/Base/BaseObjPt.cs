using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.ComponentModel;
using ByamlExt.Byaml;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public class BaseObjPoint
    {
        [Browsable(false)]
        public ByamlPathPoint Point { get; set; } = new ByamlPathPoint();

        public BaseObjPoint(dynamic bymlNode)
        {
            if (bymlNode is ByamlPathPoint) Point = (ByamlPathPoint)bymlNode;
            else throw new Exception("Not a ByamlPathPoint");
        }
    }
}
