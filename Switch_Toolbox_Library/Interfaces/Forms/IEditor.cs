using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using System.Windows.Forms;

namespace Toolbox.Library
{
    /// <summary>
    /// Represets a control to load from a <see cref="UserControl"/> for an <see cref="IFileFormat"/>.
    /// </summary>
    public interface IEditor<T> where T : UserControl
    {
        T OpenForm();
        void FillEditor(UserControl Editor);
    }
}
