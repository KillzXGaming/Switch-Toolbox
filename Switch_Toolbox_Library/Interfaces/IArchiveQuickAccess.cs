using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public interface IArchiveQuickAccess
    {
        /// <summary>
        /// Determines the category. The keys are the extension, value is category.
        /// </summary>
        Dictionary<string, string> CategoryLookup { get; }
    }
}
