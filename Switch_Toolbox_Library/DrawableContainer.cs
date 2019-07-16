using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.Interfaces;
using Toolbox.Library.Forms;

namespace Toolbox.Library
{
    //Represents a container that stores multiple drawables
    //These can be switched between the viewport
    public class DrawableContainer
    {
        private string _name;
        public string Name
        {
            set
            {
                List<string> Names = ObjectEditor.GetDrawableContainers().Select(o => o.Name).ToList();
                foreach (string str in Names)
                    Console.WriteLine("STR NAME " + str);
                _name = Utils.RenameDuplicateString(Names, value);
            }
            get
            {
                return _name;
            }
        }

        public ContainerState ContainerState { get; set; }

        public List<AbstractGlDrawable> Drawables = new List<AbstractGlDrawable>();
    }

    public enum ContainerState
    {
        Active,
        Inactive,
        Disposed,
    }
}
