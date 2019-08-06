using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    /// <summary>
    /// Represets a node from an <see cref="IArchiveFile"/>
    /// </summary>
    public interface INode
    {
        string Name { get; set; }
    }
}
