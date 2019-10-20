using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FirstPlugin.MuuntEditor
{
    public class PropertyObject
    {
        /// <summary>
        /// Child properties to be added to the tree.
        /// </summary>
        public virtual List<PropertyObject> SubObjects { get; } = new List<PropertyObject>();

        /// <summary>
        /// The name of the property
        /// </summary>
        public string Name { get; set; }

        [TypeConverter(typeof(PropertyGridTypes.DictionaryConverter))]
        public Dictionary<string, dynamic> Prop { get; set; }
    }
}
