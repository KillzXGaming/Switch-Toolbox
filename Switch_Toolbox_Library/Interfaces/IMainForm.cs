using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    /// <summary>
    /// An interface specifically used for MainForm.cs for plugins to communicate to
    /// </summary>
    public interface IMainForm
    {
        void OpenFile(string FileName, bool InActiveEditor = false);
    }
}
