using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aampv1 = AampV1Library;
using Aampv2 = AampV2Library;
using System.IO;
using Syroot.Maths;
using SharpYaml;
using SharpYaml.Events;
using SharpYaml.Serialization;
using SharpYaml.Serialization.Serializers;

namespace FirstPlugin
{
    public static class StringExtensions
    {
        public static string Indent(this string value, int size)
        {
            var strArray = value.Split('\n');
            var sb = new StringBuilder();
            foreach (var s in strArray)
                sb.Append(new string(' ', size)).Append(s);
            return sb.ToString();
        }
    }

    public class AampYamlConverter
    {
        public class YamlAamp
        {
            public const string Identifier = "!aamp";

            public string version { get; set; }
        }

        #region V1 AAMP

        public static string ToYaml(Aampv1.AampFile aampFile)
        {
            StringBuilder sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                writer.WriteLine("!aamp");
                writer.WriteLine($"version: {aampFile.Version}");
                writer.WriteLine("!io");
                writer.WriteLine($"version: 0");
                writer.WriteLine($"type: {aampFile.EffectType}");
                WriteParamList(writer, aampFile.RootNode, 0);
            }

            return sb.ToString();
        }

        private static void WriteParamList(TextWriter writer, Aampv1.ParamList paramList, int IndentAmount)
        {
            writer.WriteLine($"{paramList.HashString}: !list".Indent(IndentAmount));

            if (paramList.paramObjects.Length <= 0)
                writer.WriteLine("objects: {}".Indent(IndentAmount + 2));
            else
                writer.WriteLine("objects: ".Indent(IndentAmount + 2));

            foreach (var paramObj in paramList.paramObjects)
            {
                WriteParamObject(writer, paramObj, IndentAmount + 4);
            }

            if (paramList.childParams.Length <= 0)
                writer.WriteLine("lists: {}".Indent(IndentAmount + 2));
            else
                writer.WriteLine("lists: ".Indent(IndentAmount + 2));

            foreach (var child in paramList.childParams)
            {
                WriteParamList(writer, child, IndentAmount + 4);
            }
        }

        private static void WriteParamObject(TextWriter writer, Aampv1.ParamObject paramObj, int IndentAmount)
        {
            writer.WriteLine($"{paramObj.HashString} : !obj".Indent(IndentAmount));
            foreach (var entry in paramObj.paramEntries)
            {
                writer.WriteLine($"{entry.HashString}: {WriteParamData(entry)}".Indent(IndentAmount + 2));
            }
        }

        private static string WriteParamData(Aampv1.ParamEntry entry)
        {
            switch (entry.ParamType)
            {
                case Aampv1.ParamType.Boolean: return $"{(bool)entry.Value}";
                case Aampv1.ParamType.BufferBinary: return $"!BufferBinary [ {WriteBytes((byte[])entry.Value)} ]";
                case Aampv1.ParamType.BufferFloat: return $"!BufferFloat [ {WriteFloats((float[])entry.Value)} ]";
                case Aampv1.ParamType.BufferInt: return $"BufferInt [ {WriteInts((int[])entry.Value)} ]";
                case Aampv1.ParamType.BufferUint: return $"!BufferUint [ {WriteUints((uint[])entry.Value)} ]";
                case Aampv1.ParamType.Color4F: return $"{WriteColor4F((Vector4F)entry.Value)}";
                case Aampv1.ParamType.Vector2F: return $"{WriteVec2F((Vector2F)entry.Value)}";
                case Aampv1.ParamType.Vector3F: return $"{WriteVec3F((Vector3F)entry.Value)}";
                case Aampv1.ParamType.Vector4F: return $"{WriteVec4F((Vector4F)entry.Value)}";
                case Aampv1.ParamType.Uint: return $"{(uint)entry.Value}";
                case Aampv1.ParamType.Int: return $"{(int)entry.Value}";
                case Aampv1.ParamType.Float: return $"{(float)entry.Value}";
                case Aampv1.ParamType.String256: return $"!str256 {WriteStringEntry((AampCommon.StringEntry)entry.Value)}";
                case Aampv1.ParamType.String32: return $"!str32 {WriteStringEntry((AampCommon.StringEntry)entry.Value)}";
                case Aampv1.ParamType.String64: return $"!str64 {WriteStringEntry((AampCommon.StringEntry)entry.Value)}";
                case Aampv1.ParamType.StringRef: return $"!strRef {WriteStringEntry((AampCommon.StringEntry)entry.Value)}";
                case Aampv1.ParamType.Curve1: return $"{WriteCurve((Aampv1.Curve[])entry.Value, 1)}";
                case Aampv1.ParamType.Curve2: return $"{WriteCurve((Aampv1.Curve[])entry.Value, 2)}";
                case Aampv1.ParamType.Curve3: return $"{WriteCurve((Aampv1.Curve[])entry.Value, 3)}";
                case Aampv1.ParamType.Curve4: return $"{WriteCurve((Aampv1.Curve[])entry.Value, 4)}";
                default:
                    throw new Exception("Unsupported type! " + entry.ParamType);
            }
        }

        private static string WriteStringEntry(AampCommon.StringEntry value)
        {
            return BytesToStringConverted(value.Data).Replace(" ", string.Empty);
         //   return Encoding.Default.GetString(value.Data).Replace(" ", string.Empty);
        }

        static string BytesToStringConverted(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new Toolbox.Library.IO.FileReader(stream))
                {
                    return reader.ReadZeroTerminatedString();
                }
            }
        }


        private static string WriteCurve(Aampv1.Curve[] curves, int Num)
        {
            string curveStr = "";
            foreach (var curve in curves)
                curveStr += $"!curve{Num}[{WriteUints(curve.valueUints)}] [{WriteFloats(curve.valueFloats)}] \n";

            return curveStr;
        }

        //I could've used a yaml parser, but incase i need to change it up to look nicer and support leo's aamp layout, do it manually
        public static void ToAamp(Aampv1.AampFile aampFile, string text)
        {
            byte[] TextData = Encoding.Unicode.GetBytes(text);
            StreamReader t = new StreamReader(new MemoryStream(TextData), Encoding.GetEncoding(932));

            var yaml = new YamlStream();
           yaml.Load(new StringReader(text));

      /*     var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;
           foreach (var item in mapping.AllNodes)
           {
               Console.WriteLine("n " + item);
           }*/

            /*    byte[] TextData = Encoding.Unicode.GetBytes(text);
                StreamReader t = new StreamReader(new MemoryStream(TextData), Encoding.GetEncoding(932));
                using (var reader = new StringReader(t.ReadToEnd()))
                {
                    string AampCheck = reader.ReadLine();
                    if (AampCheck != "!aamp")
                        throw new Exception($"Expected !aamp got {AampCheck} at line 0");
                    string VersionCheck = reader.ReadLine();
                    string num = GetProperty(VersionCheck);
                    if (num == "1")
                    {

                    }
                }*/
        }

        public static void ParseList(StringReader reader)
        {
        }

        public static string GetProperty(string line)
        {
            if (line.Contains(":"))
                return line.Split(':')[1].Replace(string.Empty, "");
            return line;
        }

        #endregion

        #region V2 AAMP

        public static string ToYaml(Aampv2.AampFile aampFile)
        {
            StringBuilder sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                writer.WriteLine("!aamp");
                writer.WriteLine($"version: {aampFile.Version}");
                writer.WriteLine("!io");
                writer.WriteLine($"version: {aampFile.ParameterIOVersion}");
                writer.WriteLine($"type: {aampFile.ParameterIOType}");
                WriteParamList(writer, aampFile.RootNode, 0);
            }

            return sb.ToString();
        }

        private static void WriteParamList(TextWriter writer, Aampv2.ParamList paramList, int IndentAmount)
        {
            writer.WriteLine($"{paramList.HashString}: !list".Indent(IndentAmount));

            if (paramList.paramObjects.Length <= 0)
                writer.WriteLine("objects: {}".Indent(IndentAmount + 2));
            else
                writer.WriteLine("objects: ".Indent(IndentAmount + 2));

            foreach (var paramObj in paramList.paramObjects)
            {
                WriteParamObject(writer, paramObj, IndentAmount + 4);
            }

            if (paramList.childParams.Length <= 0)
                writer.WriteLine("lists: {}".Indent(IndentAmount + 2));
            else
                writer.WriteLine("lists: ".Indent(IndentAmount + 2));

            foreach (var child in paramList.childParams)
            {
                WriteParamList(writer, child, IndentAmount + 4);
            }
        }

        private static void WriteParamObject(TextWriter writer, Aampv2.ParamObject paramObj, int IndentAmount)
        {
            writer.WriteLine($"{paramObj.HashString} : !obj".Indent(IndentAmount));
            foreach (var entry in paramObj.paramEntries)
            {
                writer.WriteLine($"{WriteParamData(entry)}".Indent(IndentAmount + 2));
            }
        }

        private static string WriteParamData(Aampv2.ParamEntry entry)
        {
            switch (entry.ParamType)
            {
                case Aampv2.ParamType.Boolean: return $"{entry.HashString}: {(bool)entry.Value}";
                case Aampv2.ParamType.BufferBinary: return $"{entry.HashString}: !BufferBinary [ {WriteBytes((byte[])entry.Value)} ]";
                case Aampv2.ParamType.BufferFloat: return $"{entry.HashString}: !BufferFloat [ {WriteFloats((float[])entry.Value)} ]";
                case Aampv2.ParamType.BufferInt: return $"{entry.HashString}: !BufferInt [ {WriteInts((int[])entry.Value)} ]";
                case Aampv2.ParamType.BufferUint: return $"{entry.HashString}: !BufferUint [ {WriteUints((uint[])entry.Value)} ]";
                case Aampv2.ParamType.Quat: return $"{entry.HashString}: !BufferUint [ {WriteFloats((float[])entry.Value)} ]";
                case Aampv2.ParamType.Color4F: return $"{entry.HashString}: {WriteColor4F((Vector4F)entry.Value)}";
                case Aampv2.ParamType.Vector2F: return $"{entry.HashString}: {WriteVec2F((Vector2F)entry.Value)}";
                case Aampv2.ParamType.Vector3F: return $"{entry.HashString}: {WriteVec3F((Vector3F)entry.Value)}";
                case Aampv2.ParamType.Vector4F: return $"{entry.HashString}: {WriteVec4F((Vector4F)entry.Value)}";
                case Aampv2.ParamType.Uint: return $"{entry.HashString}: {(uint)entry.Value}";
                case Aampv2.ParamType.Int: return $"{entry.HashString}: {(int)entry.Value}";
                case Aampv2.ParamType.Float: return $"{entry.HashString}: {(float)entry.Value}";
                case Aampv2.ParamType.String256: return $"{entry.HashString}: !str256 {((AampCommon.StringEntry)entry.Value).ToString()}";
                case Aampv2.ParamType.String32: return $"{entry.HashString}: !str32 {((AampCommon.StringEntry)entry.Value).ToString()}";
                case Aampv2.ParamType.String64: return $"{entry.HashString}: !str64 {((AampCommon.StringEntry)entry.Value).ToString()}";
                case Aampv2.ParamType.StringRef: return $"{entry.HashString}: !strRef {((AampCommon.StringEntry)entry.Value).ToString()}";
                case Aampv2.ParamType.Curve1: return $"{entry.HashString}: {WriteCurve((Aampv2.Curve[])entry.Value, 1)}";
                case Aampv2.ParamType.Curve2: return $"{entry.HashString}: {WriteCurve((Aampv2.Curve[])entry.Value, 2)}";
                case Aampv2.ParamType.Curve3: return $"{entry.HashString}: {WriteCurve((Aampv2.Curve[])entry.Value, 3)}";
                case Aampv2.ParamType.Curve4: return $"{entry.HashString}: {WriteCurve((Aampv2.Curve[])entry.Value, 4)}";
                default:
                    throw new Exception("Unsupported type! " + entry.ParamType);
            }
        }

        private static string WriteCurve(Aampv2.Curve[] curves, int Num)
        {
            string curveStr = "";
            foreach (var curve in curves)
                curveStr += $"!curve{Num}[{WriteUints(curve.valueUints)}] [{WriteFloats(curve.valueFloats)}] \n";

            return curveStr;
        }

        public static Aampv2.AampFile ToAamp(string FileName)
        {
            var aampFile = new Aampv2.AampFile();

            return aampFile;
        }

        #endregion

        private static string WriteUints(uint[] arr) {
            return String.Join(",", arr.Select(p => p.ToString()).ToArray());
        }

        private static string WriteFloats(float[] arr) {
            return String.Join(",", arr.Select(p => p.ToString()).ToArray());
        }

        private static string WriteInts(int[] arr) {
            return String.Join(",", arr.Select(p => p.ToString()).ToArray());
        }

        private static string WriteBytes(byte[] arr) {
            return String.Join(",", arr.Select(p => p.ToString()).ToArray());
        }

        private static string WriteVec2F(Vector2F vec2) { return $"!vec2[{vec2.X}, {vec2.Y}]"; }
        private static string WriteVec3F(Vector3F vec3) { return $"!vec3[{vec3.X}, {vec3.Y}, {vec3.Z}]"; }
        private static string WriteVec4F(Vector4F vec4) { return $"!vec4[{vec4.X}, {vec4.Y}, {vec4.Z}, {vec4.W}]"; }
        private static string WriteColor4F(Vector4F vec4) { return $"!color[{vec4.X}, {vec4.Y}, {vec4.Z}, {vec4.W}]"; }

    }
}
