using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    //Represents a node from an IArchive file
    public interface INode
    {
        string Name { get; set; }
    }
}
