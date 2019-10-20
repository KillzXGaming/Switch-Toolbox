using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin.MuuntEditor
{
    public class ObjectGroup
    {
        public ObjectGroup() { }

        public ObjectGroup(string name) {
            Name = name;
        }

        public ObjectGroup(string name, PropertyObject property) {
            Name = name;
            Objects.Add(property);
        }

        /// <summary>
        /// Name of the group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of properties in the group.
        /// </summary>
        public virtual List<PropertyObject> Objects { get; } = new List<PropertyObject>();
    }
}
