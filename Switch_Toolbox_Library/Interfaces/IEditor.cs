using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.Forms;
using System.Windows.Forms;

namespace Switch_Toolbox.Library
{
    public interface IEditor<T> where T : STForm
    {
        T OpenForm();
    }
}
