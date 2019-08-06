using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    /// <summary>
    /// A list of supported formatting types for how to syntax the text editor
    /// </summary>
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

    /// <summary>
    /// Represets a text format which can be converted to and from a string based file
    /// This can be used for formats to be converted as yaml, xml, etc.
    /// </summary>
    public interface IConvertableTextFormat
    {
        TextFileType TextFileType { get; }
        bool CanConvertBack { get; }
        string ConvertToString();
        void ConvertFromString(string text);
    }
}
