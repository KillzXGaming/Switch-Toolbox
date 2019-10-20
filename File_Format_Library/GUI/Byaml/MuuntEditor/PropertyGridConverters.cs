using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections;
using System.Linq;

namespace FirstPlugin.MuuntEditor
{
    public class PropertyGridTypes
    {
        public static Dictionary<Type, Type> CustomClassConverter = new Dictionary<Type, Type>();

        public static class GetTypeConv
        {
            public static TypeConverter Get(string _key, dynamic _obj)
            {
                if (_obj == null) return new NullConverter();
                else if (CustomClassConverter.ContainsKey(_obj.GetType()))
                    return Activator.CreateInstance(CustomClassConverter[_obj.GetType()]);
                else if (_obj is IDictionary<string, dynamic>)
                {
                    if (_obj.Keys.Count == 3 && _obj.ContainsKey("X") && _obj.ContainsKey("Y") && _obj.ContainsKey("Z")) return new Vector3DDictionaryConverter();
                    return new DictionaryConverter();
                }
                else if (_obj is IList<dynamic>) return new ArrayNodeConverter();
                else return TypeDescriptor.GetConverter(_obj);
            }
        }

        public class NullConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
            {
                return false;
            }

            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                return (string)value;
            }

            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                return "<Null>";
            }
        }

        public class Vector3DDictionaryConverter : System.ComponentModel.TypeConverter
        {
            public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                ArrayList properties = new ArrayList();
                foreach (string e in ((Dictionary<string, dynamic>)value).Keys)
                {
                    properties.Add(new DictionaryConverter.DictionaryPropertyDescriptor((Dictionary<string, dynamic>)value, e));
                }

                PropertyDescriptor[] props =
                    (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

                return new PropertyDescriptorCollection(props);
            }

            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                string[] tokens = ((string)value).Split(';');
                IDictionary<string, dynamic> dict = new Dictionary<string, dynamic>();
                dict["X"] = Single.Parse(tokens[0]);
                dict["Y"] = Single.Parse(tokens[1]);
                dict["Z"] = Single.Parse(tokens[2]);
                return dict;
            }

            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                var v = value as IDictionary<string, dynamic>;
                if (v == null) //MultiMergeCollection workaround
                {
                    var values = new float[3];
                    int count = 0;
                    foreach (object o in (ICollection)value)
                        values[count++] = ((KeyValuePair<string, dynamic>)o).Value;
                    return $"{Math.Round(values[0], 2)}; {Math.Round(values[1], 2)}; {Math.Round(values[2], 2)}";
                }
                return $"{Math.Round(v["X"], 2)}; {Math.Round(v["Y"], 2)}; {Math.Round(v["Z"], 2)}";
            }
        }

        public class DictionaryConverter : System.ComponentModel.TypeConverter
        {
            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                return "<Dictionary node>";
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                ArrayList properties = new ArrayList();
                foreach (string e in ((Dictionary<string, dynamic>)value).Keys)
                {
                    properties.Add(new DictionaryPropertyDescriptor((Dictionary<string, dynamic>)value, e));
                }

                PropertyDescriptor[] props =
                    (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

                return new PropertyDescriptorCollection(props);
            }

            public class DictionaryPropertyDescriptor : PropertyDescriptor //TODO Fix JIS encoding
            {
                IDictionary<string, dynamic> _dictionary;
                string _key;

                public override TypeConverter Converter
                {
                    get
                    {
                        return GetTypeConv.Get(_key, _dictionary[_key]);
                    }
                }

                internal DictionaryPropertyDescriptor(IDictionary<string, dynamic> d, string key)
                    : base(key, null)
                {
                    _dictionary = d;
                    _key = key;
                }

                public override Type PropertyType
                {
                    get { return _dictionary[_key] == null ? typeof(string) : _dictionary[_key].GetType(); }
                }

                public override void SetValue(object component, object value)
                {
                    _dictionary[_key] = value;
                }

                public override object GetValue(object component)
                {
                    return _dictionary[_key];
                }

                public override bool IsReadOnly
                {
                    get { return false; }
                }

                public override Type ComponentType
                {
                    get { return null; }
                }

                public override bool CanResetValue(object component)
                {
                    return false;
                }

                public override void ResetValue(object component)
                {
                }

                public override bool ShouldSerializeValue(object component)
                {
                    return false;
                }
            }
        }

        public class ArrayNodeConverter : System.ComponentModel.TypeConverter
        {
            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                return "<Array node>";
            }

            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                ArrayList properties = new ArrayList();
                for (int i = 0; i < ((List<dynamic>)value).Count; i++)
                {
                    properties.Add(new ArrayPropertyDescriptor(((List<dynamic>)value)[i], "Item " + i.ToString() + " :"));
                }

                PropertyDescriptor[] props =
                    (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

                return new PropertyDescriptorCollection(props);
            }

            public class ArrayPropertyDescriptor : PropertyDescriptor //TODO Fix JIS encoding
            {
                dynamic _obj;
                string _key;

                public override TypeConverter Converter
                {
                    get
                    {
                        return GetTypeConv.Get(_key, _obj);
                    }
                }

                internal ArrayPropertyDescriptor(dynamic obj, string key)
                    : base(key, null)
                {
                    _obj = obj;
                    _key = key;
                }

                public override Type PropertyType
                {
                    get { return _obj == null ? typeof(string) : _obj.GetType(); }
                }

                public override void SetValue(object component, object value)
                {
                    _obj = value;
                }

                public override object GetValue(object component)
                {
                    return _obj;
                }

                public override bool IsReadOnly
                {
                    get { return false; }
                }

                public override Type ComponentType
                {
                    get { return null; }
                }

                public override bool CanResetValue(object component)
                {
                    return false;
                }

                public override void ResetValue(object component)
                {
                }

                public override bool ShouldSerializeValue(object component)
                {
                    return false;
                }
            }
        }
    }
}
