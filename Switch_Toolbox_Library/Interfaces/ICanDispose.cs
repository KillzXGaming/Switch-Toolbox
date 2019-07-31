using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    //Check if the opened file stream should be disposed after loading 
    public interface IFileDisposeAfterLoad
    {
        bool CanDispose { get; }
    }
}
