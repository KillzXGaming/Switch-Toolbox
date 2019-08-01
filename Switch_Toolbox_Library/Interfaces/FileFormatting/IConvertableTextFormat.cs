using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public enum TextFileType
    {
        Normal,
        Yaml,
        Xml,
        Json,
        CSharp,
        CPP,
        Python,
        Glsl,
    }

    //Represets a text format which can be converted to and from a string based file
    //This can be used for formats to be used as yaml, xml, etc
    public interface IConvertableTextFormat
    {
        TextFileType TextFileType { get; }
        bool CanConvertBack { get; }
        string ConvertToString();
        void ConvertFromString(string text);
    }
}
