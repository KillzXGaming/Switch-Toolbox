using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library
{
    /// <summary>
    /// Represets a parent form MDI container base to add mdi childern to.
    /// This is currently used to interact with the MainForm
    /// </summary>
    public interface IMdiContainer
    {
        void AddChildContainer(Form form);
    }
}
