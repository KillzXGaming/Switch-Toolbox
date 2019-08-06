using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    /// <summary>
    /// Represets a directory from an <see cref="IArchiveFile"/>
    /// </summary>
    public interface IDirectoryContainer : INode
    {
        IEnumerable<INode> Nodes { get; }
    }
}
