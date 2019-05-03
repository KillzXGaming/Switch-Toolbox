using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aampv1 = AampV1Library;
using Aampv2 = AampV2Library;
using System.IO;
using Syroot.Maths;

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

    public class YamlConverter
    {
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
                writer.Write($"{WriteParamData(entry)}\n".Indent(IndentAmount + 2));
            }
        }

        private static string WriteParamData(Aampv1.ParamEntry entry)
        {
            switch (entry.ParamType)
            {
                case Aampv1.ParamType.Boolean: return $"{entry.HashString}: {(bool)entry.Value}";
                case Aampv1.ParamType.BufferBinary: return $"{entry.HashString}: !BufferBinary [ {WriteBytes((byte[])entry.Value)} ]";
                case Aampv1.ParamType.BufferFloat: return $"{entry.HashString}: !BufferFloat [ {WriteFloats((float[])entry.Value)} ]";
                case Aampv1.ParamType.BufferInt: return $"{entry.HashString}: !BufferInt [ {WriteInts((int[])entry.Value)} ]";
                case Aampv1.ParamType.BufferUint: return $"{entry.HashString}: !BufferUint [ {WriteUints((uint[])entry.Value)} ]";
                case Aampv1.ParamType.Color4F: return $"{entry.HashString}: {WriteColor4F((Vector4F)entry.Value)}";
                case Aampv1.ParamType.Vector2F: return $"{entry.HashString}: {WriteVec2F((Vector2F)entry.Value)}";
                case Aampv1.ParamType.Vector3F: return $"{entry.HashString}: {WriteVec3F((Vector3F)entry.Value)}";
                case Aampv1.ParamType.Vector4F: return $"{entry.HashString}: {WriteVec4F((Vector4F)entry.Value)}";
                case Aampv1.ParamType.Uint: return $"{entry.HashString}: {(uint)entry.Value}";
                case Aampv1.ParamType.Int: return $"{entry.HashString}: {(int)entry.Value}";
                case Aampv1.ParamType.Float: return $"{entry.HashString}: {(float)entry.Value}";
                case Aampv1.ParamType.String256: return $"{entry.HashString}: !str256 {(string)entry.Value}";
                case Aampv1.ParamType.String32: return $"{entry.HashString}: !str32 {(string)entry.Value}";
                case Aampv1.ParamType.String64: return $"{entry.HashString}: !str64 {(string)entry.Value}";
                case Aampv1.ParamType.StringRef: return $"{entry.HashString}: !strRef {(string)entry.Value}";
                case Aampv1.ParamType.Curve1: return $"{entry.HashString}: {WriteCurve((Aampv2.Curve[])entry.Value, 1)}";
                case Aampv1.ParamType.Curve2: return $"{entry.HashString}: {WriteCurve((Aampv2.Curve[])entry.Value, 2)}";
                case Aampv1.ParamType.Curve3: return $"{entry.HashString}: {WriteCurve((Aampv2.Curve[])entry.Value, 3)}";
                case Aampv1.ParamType.Curve4: return $"{entry.HashString}: {WriteCurve((Aampv2.Curve[])entry.Value, 4)}";
                default:
                    throw new Exception("Unsupported type! " + entry.ParamType);
            }
        }

        private static string WriteCurve(Aampv1.Curve[] curves, int Num)
        {
            string curveStr = "";
            foreach (var curve in curves)
                curveStr += $"!curve{Num}[{WriteUints(curve.valueUints)}] [{WriteFloats(curve.valueFloats)}] \n";

            return curveStr;
        }

        public static void ToAamp(Aampv1.AampFile aampFile)
        {

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
                case Aampv2.ParamType.Color4F: return $"{entry.HashString}: {WriteColor4F((Vector4F)entry.Value)}";
                case Aampv2.ParamType.Vector2F: return $"{entry.HashString}: {WriteVec2F((Vector2F)entry.Value)}";
                case Aampv2.ParamType.Vector3F: return $"{entry.HashString}: {WriteVec3F((Vector3F)entry.Value)}";
                case Aampv2.ParamType.Vector4F: return $"{entry.HashString}: {WriteVec4F((Vector4F)entry.Value)}";
                case Aampv2.ParamType.Uint: return $"{entry.HashString}: {(uint)entry.Value}";
                case Aampv2.ParamType.Int: return $"{entry.HashString}: {(int)entry.Value}";
                case Aampv2.ParamType.Float: return $"{entry.HashString}: {(float)entry.Value}";
                case Aampv2.ParamType.String256: return $"{entry.HashString}: !str256 {(string)entry.Value}";
                case Aampv2.ParamType.String32: return $"{entry.HashString}: !str32 {(string)entry.Value}";
                case Aampv2.ParamType.String64: return $"{entry.HashString}: !str64 {(string)entry.Value}";
                case Aampv2.ParamType.StringRef: return $"{entry.HashString}: !strRef {(string)entry.Value}";
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
