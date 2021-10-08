using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using OpenTK;
using System.Windows.Threading;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
   public class Utils
    {
        public static void DelayAction(int millisecond, Action action)
        {
            var timer = new DispatcherTimer();
            timer.Tick += delegate

            {
                action.Invoke();
                timer.Stop();
            };

            timer.Interval = TimeSpan.FromMilliseconds(millisecond);
            timer.Start();
        }

        public static bool CompareNoExtension(string a, string b)
        {
            return (Path.GetFileNameWithoutExtension(a) == Path.GetFileNameWithoutExtension(b));
        }

        public static byte[] CreateMD5Hash(string filename)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }

        public static int GetSizeInBytes(List<byte[]> bytes)
        {
            int length = 0;
            for (int i = 0; i < bytes.Count; i++)
                length += bytes[i].Length;
            return length;
        }

        public static bool HasInterface(Type objectType, Type interfaceType)
        {
            foreach (var inter in objectType.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == interfaceType)
                    return true;
            }
            return false;
        }

        public static bool IsInDesignMode()
        {
            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv")
                return true;

            return false;
        }

        public static string ColorToHex(Color color)
        {
           return color.R.ToString("X2") +
                  color.G.ToString("X2") +
                  color.B.ToString("X2") +
                  color.A.ToString("X2");
        }

        public static void DeleteIfExists(string FilePath)
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }

        public static Color HexToColor(string HexText)
        {
            try
            {
                return Color.FromArgb(
                int.Parse(HexText.Substring(6, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(HexText.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(HexText.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(HexText.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
            }
            catch
            {
                throw new Exception("Invalid Hex Format!");
            }
        }

        public static int FloatToIntClamp(float r)
        {
            return Clamp((int)(r * 255), 0, 255);
        }

        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static string AddQuotesIfRequired(string path)
        {
            return !string.IsNullOrWhiteSpace(path) ?
                path.Contains(" ") && (!path.StartsWith("\"") && !path.EndsWith("\"")) ?
                    "\"" + path + "\"" : path :
                    string.Empty;
        }

        public static string GetExtension(string FileName)
        {
            return Path.GetExtension(FileName.RemoveIllegaleFileNameCharacters()).ToLower();
        }
           
        public static bool HasExtension(string FileName, string Extension)
        {
            return FileName.EndsWith(Extension, StringComparison.Ordinal);
        }

        public static Vector2 ToVec2(float[] v)
        {
            return new Vector2(v[0], v[1]);
        }
        public static Vector3 ToVec3(float[] v)
        {
            return new Vector3(v[0], v[1], v[2]);
        }
        public static Vector4 ToVec4(float[] v)
        {
            return new Vector4(v[0], v[1], v[2], v[3]);
        }
        public static Vector2 ToVec2(Syroot.Maths.Vector2F v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Vector3 ToVec3(Syroot.Maths.Vector3F v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        public static Vector4 ToVec4(Syroot.Maths.Vector4F v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }
        public static byte[] CombineByteArray(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        public static byte[] SubArray(Stream data, uint offset, uint length)
        {
            using (var reader = new FileReader(data))
            {
                reader.Position = offset;
                return reader.ReadBytes((int)length);
            }
        }

        public static byte[] SubArray(byte[] data, uint offset, uint length)
        {
            //Slice only when the dst size is big enough for the src
            if (data.Length - offset >= length)
                return data.Slice(offset, length);
            else
                return data.Skip((int)offset).Take((int)length).ToArray();
        }

        public static string RenameDuplicateString(List<string> strings, string oldString, int index = 0, int numDigits = 1)
        {
            if (strings.Contains(oldString))
            {
                string key = $"{index++}";
                if (numDigits == 2)
                    key = string.Format("{0:00}", key);

                string NewString = $"{oldString}_{key}";
                if (strings.Contains(NewString))
                    return RenameDuplicateString(strings, oldString, index, numDigits);
                else
                    return NewString;
            }

            return oldString;
        }

        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }
        public static Matrix4 TransformValues(Vector3 translation, Vector3 rotation, float scale)
        {
            return TransformValues(translation, rotation, new Vector3(scale));
        }
        public static Matrix4 TransformValues(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            Matrix4 positionMat = Matrix4.CreateTranslation(translation);
            Matrix4 rotXMat = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X));
            Matrix4 rotYMat = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y));
            Matrix4 rotZMat = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z));
            Matrix4 scaleMat = Matrix4.CreateScale(scale);
            return scaleMat * (rotXMat * rotYMat * rotZMat) * positionMat;
        }

        public static string GenerateUniqueHashID()
        {
            return Guid.NewGuid().ToString();
        }

        public static string GetAllFilters(Type[] Types)
        {
            string Filter = "All Supported Files|";
            List<string> FilterEach = new List<string>();
            foreach (var type in Types)
            {
                Object instance = Activator.CreateInstance(type);

                IFileFormat f = (IFileFormat)instance;
                for (int i = 0; i < f.Extension.Length; i++)
                {
                    Filter += $"{f.Extension[i]};";
                    FilterEach.Add($"{f.Description[0]} ({f.Extension[i]}) |{f.Extension[i]}|");
                }
            }

            //Get compression formats
            foreach (ICompressionFormat f in FileManager.GetCompressionFormats())
            {
                for (int i = 0; i < f.Extension.Length; i++)
                {
                    Filter += $"{f.Extension[i]};";
                    FilterEach.Add($"{f.Description[0]} ({f.Extension[i]}) |{f.Extension[i]}|");
                }
            }

            Filter += $"{"*.z"};";
            Filter += $"{"*.cmp"};";
            Filter += $"{"*.yaz0"};";
            Filter += $"{"*.zstb"};";
            Filter += $"{"*.lz4"};";
            Filter += $"{"*.gz"};";
            Filter += $"{"*.szs"};";
            Filter += $"{"*.zs"};";
            Filter += $"{"*.yaz0"};";


            FilterEach.Add($"{"Compressed File"} ({"*.cmp"}) |{"*.cmp"}|");
            FilterEach.Add($"{"Zlib Compressed"} ({"*.z"}) |{"*.z"}|");
            FilterEach.Add($"{"Yaz0 Compressed"} ({"*.yaz0"}) |{"*.yaz0"}|");
            FilterEach.Add($"{"Zstb Compressed"} ({"*.zstb"}) |{"*.zstb"}|");
            FilterEach.Add($"{"Lz4 Compressed"} ({"*.lz4"}) |{"*.lz4"}|");
            FilterEach.Add($"{"GZIP Compressed"} ({"*.gz"}) |{"*.gz"}|");
            FilterEach.Add($"{"SZS ZSTD Compressed File"} ({"*.zs"}) |{"*.zs"}|");
            FilterEach.Add($"{"SZS Yaz0 Compressed"} ({"*.szs"}) |{"*.szs"}|");
            FilterEach.Add($"{"Yaz0 Compressed"} ({"*.yaz0"}) |{"*.yaz0"}|");

            Filter += "|";
            Filter += string.Join("", FilterEach.ToArray());
            Filter += "All files(*.*)|*.*";
            return Filter;
        }

        public static string GetAllFilters(Type type)
        {
            Object instance = Activator.CreateInstance(type);
            return GetAllFilters((IFileFormat)instance);
        }

        public static string GetAllFilters(IFileFormat format)
        {
            List<IFileFormat> f = new List<IFileFormat>();
            f.Add(format);
            return GetAllFilters(f, format);
        }

        public static string GetAllFilters(IEnumerable<IFileFormat> format, IFileFormat targetFormat = null)
        {
            var alltypes = format;

            string Filter = "All Supported Files|";
            List<string> FilterEach = new List<string>();

            //Set the current extension used by the target's file information if used
            if (targetFormat != null && targetFormat.FilePath != null) {
                string extension = Path.GetExtension(targetFormat.FilePath);
                if (extension != "" && !format.Any(x => x.Extension.Contains($"*{extension}")))
                {
                    Filter += $"*{extension};";
                    FilterEach.Add($"({extension}) |*{extension}|");
                }
            }

            foreach (IFileFormat f in format)
            {
                for (int i = 0; i < f.Extension.Length; i++)
                {
                    Filter += $"{f.Extension[i]};";
                    FilterEach.Add($"{f.Description[0]} ({f.Extension[i]}) |{f.Extension[i]}|");
                }
            }

            //Get compression formats
            foreach (ICompressionFormat f in FileManager.GetCompressionFormats())
            {
                for (int i = 0; i < f.Extension.Length; i++)
                {
                    Filter += $"{f.Extension[i]};";
                    FilterEach.Add($"{f.Description[0]} ({f.Extension[i]}) |{f.Extension[i]}|");
                }
            }

            Filter += $"{"*.z"};";
            Filter += $"{"*.cmp"};";
            Filter += $"{"*.yaz0"};";
            Filter += $"{"*.zstb"};";
            Filter += $"{"*.lz4"};";
            Filter += $"{"*.gz"};";
            Filter += $"{"*.szs"};";
            Filter += $"{"*.zs"};";
            Filter += $"{"*.yaz0"};";


            FilterEach.Add($"{"Compressed File"} ({"*.cmp"}) |{"*.cmp"}|");
            FilterEach.Add($"{"Zlib Compressed"} ({"*.z"}) |{"*.z"}|");
            FilterEach.Add($"{"Yaz0 Compressed"} ({"*.yaz0"}) |{"*.yaz0"}|");
            FilterEach.Add($"{"Zstb Compressed"} ({"*.zstb"}) |{"*.zstb"}|");
            FilterEach.Add($"{"Lz4 Compressed"} ({"*.lz4"}) |{"*.lz4"}|");
            FilterEach.Add($"{"GZIP Compressed"} ({"*.gz"}) |{"*.gz"}|");
            FilterEach.Add($"{"SZS ZSTD Compressed File"} ({"*.zs"}) |{"*.zs"}|");
            FilterEach.Add($"{"SZS Yaz0 Compressed"} ({"*.szs"}) |{"*.szs"}|");
            FilterEach.Add($"{"Yaz0 Compressed"} ({"*.yaz0"}) |{"*.yaz0"}|");

            Filter += "|";
            Filter += string.Join("", FilterEach.ToArray());
            Filter += "All files(*.*)|*.*";
            return Filter;
        }

        private static void AddCompressionExtensions()
        {

        }
    }
}
