using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin.MuuntEditor
{
    public interface IMuuntLoader
    {
        List<ObjectGroup> Groups { get; set; }

        bool Identify(dynamic byml, string fileName);
        void Load(dynamic byml);
    }
}
