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
    /// Represets a form to load from a <see cref="Form"/> for an <see cref="IFileFormat"/>.
    /// </summary>
    public interface IEditorForm<T> where T : Form
    {
        T OpenForm();
        void FillEditor(Form Editor);
    }
}
