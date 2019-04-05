using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin.Turbo.CourseMuuntStructs
{
    public interface IObject
    {
        dynamic this[string name] { get; set; }
    }
}
