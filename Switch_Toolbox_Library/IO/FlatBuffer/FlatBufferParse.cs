using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Toolbox.Library.IO;
using OpenTK;

namespace Toolbox.Library.IO.FlatBuffer
{
    public class FlatBufferParse
    {
        public static T ParseFlatTable<T>(FileReader reader)
                         where T : FlatTable, new()
        {
            long origin = reader.Position;

            int vtableSOffset = reader.ReadInt32();
            var dataOffset = reader.ReadOffset(true, typeof(uint));

            T section = new T();
            using (reader.TemporarySeek(origin - vtableSOffset, System.IO.SeekOrigin.Begin))
            {
                ushort vTableSize = reader.ReadUInt16();
                ushort tableSize = reader.ReadUInt16();

                Console.WriteLine($"vTableSize {vTableSize}");
                Console.WriteLine($"tableSize {tableSize}");

                List<ushort> pointers = new List<ushort>();
                uint looper = 4;
                while (looper < vTableSize)
                {
                    pointers.Add(reader.ReadUInt16());
                    looper += 2;
                }

                section.LayoutPointers = pointers.ToArray();
            }

            using (reader.TemporarySeek(dataOffset, System.IO.SeekOrigin.Begin))
            {
                section.Read(reader);
            }

            return section;
        }

    }

    public class FlatTableParse : Attribute { }

    public class FlatTable
    {
        public ushort LayoutSize { get; set; }
        public ushort LayoutStride { get; set; }

        public ushort[] LayoutPointers { get; set; }

        public void Read(FileReader reader)
        {
            long origin = reader.Position;

            PropertyInfo[] types = new PropertyInfo[(int)LayoutPointers?.Length];

            var sectionType = this.GetType();

            int index = 0;
            foreach (var prop in sectionType.GetProperties())
            {
                if (!Attribute.IsDefined(prop, typeof(FlatTableParse)))
                    continue;

                types[index++] = prop;
            }

            for (int i = 0; i < LayoutPointers?.Length; i++)
            {
                reader.SeekBegin(origin + LayoutPointers[i]);
                if (types[i] != null)
                {
                    var prop = types[i];
                    var propertyType = prop.PropertyType;

                    if (propertyType == typeof(uint))
                        prop.SetValue(this, reader.ReadUInt32());
                    else if (propertyType == typeof(int))
                        prop.SetValue(this, reader.ReadInt32());
                    else if (propertyType == typeof(byte))
                        prop.SetValue(this, reader.ReadByte());
                    else if (propertyType == typeof(sbyte))
                        prop.SetValue(this, reader.ReadSByte());
                    else if (propertyType == typeof(ushort))
                        prop.SetValue(this, reader.ReadUInt16());
                    else if (propertyType == typeof(short))
                        prop.SetValue(this, reader.ReadInt16());
                    if (propertyType == typeof(string))
                    {
                        var offset = reader.ReadOffset(true, typeof(uint));
                        reader.SeekBegin(offset);
                        prop.SetValue(this, reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ByteLengthPrefix));
                    }
                    else if (propertyType == typeof(Vector2))
                        prop.SetValue(this, new Vector2(
                            reader.ReadSingle(),
                            reader.ReadSingle())
                         );
                    else if (propertyType == typeof(Vector3))
                        prop.SetValue(this, new Vector3(
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle())
                         );
                    else if (propertyType == typeof(Vector4))
                        prop.SetValue(this, new Vector4(
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle(),
                            reader.ReadSingle())
                         );
                    else if (propertyType == typeof(FlatTable))
                    {
                        var offset = reader.ReadOffset(true, typeof(uint));
                        reader.SeekBegin(offset);
                    }
                    else if (propertyType is IEnumerable<FlatTable>)
                    {

                    }
                }
            }
        }
    }

}
