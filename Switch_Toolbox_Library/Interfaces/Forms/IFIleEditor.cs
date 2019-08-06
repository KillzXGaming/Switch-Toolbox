using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    /// <summary>
    /// Represets a control to SAVE from a <see cref="UserControl"/> for an <see cref="IFileFormat"/>.
    /// </summary>
    public interface IFIleEditor
    {
        List<IFileFormat> GetFileFormats();
    }
}
