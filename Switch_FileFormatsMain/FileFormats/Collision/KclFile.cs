using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Toolbox.Library.IO;
using OpenTK;

namespace FirstPlugin
{
    public class KclFile
    {
        public class Header
        {
            public uint Signature { get; set; }
            public uint OctreeOffset { get; set; }
            public uint ModelOffsetArray { get; set; }
            public uint ModelCount { get; set; }
            public Vector3 MinCoordinates { get; set; }
            public Vector3 MaxCoordinates { get; set; }
            public Vector3 CoordinateShift { get; set; }
            public uint Unknown { get; set; }

            public void Read(FileReader reader)
            {

            }

            public void Write(FileWriter writer)
            {

            }
        }

        public class OctreeNode
        {
        /*    public List<ushort> TriangleIndices { get; set; }

            protected const uint _flagMask = 0b11000000_00000000_00000000_00000000;

            public const int ChildCount = 8;

            public OctreeNode[] Children { get; internal set; }

            public OctreeNode(FileReader reader, long ParentOffset)
            {
                uint Key = reader.ReadUInt32();
                long offset = ParentOffset + Key & ~_flagMask;
                if ((Flags)(Key & _flagMask) == Flags.Values)
                {
                    // Node is a leaf and key points to triangle list starting 2 bytes later.
                    using (reader.TemporarySeek(offset + sizeof(ushort), SeekOrigin.Begin))
                    {
                        TriangleIndices = new List<ushort>();
                        ushort index;
                        while ((index = reader.ReadUInt16()) != 0xFFFF)
                        {
                            TriangleIndices.Add(index);
                        }
                    }
                }
                else
                {
                    // Node is a branch and points to 8 child nodes.
                    using (reader.TemporarySeek(offset, SeekOrigin.Begin))
                    {
                        OctreeNode[] children = new OctreeNode[ChildCount];
                        for (int i = 0; i < ChildCount; i++)
                        {
                            children[i] = new OctreeNode(reader, offset);
                        }
                        Children = children;
                    }
                }
            }

            internal enum Flags : uint
            {
                Divide = 0b00000000_00000000_00000000_00000000,
                Values = 0b10000000_00000000_00000000_00000000,
                NoData = 0b11000000_00000000_00000000_00000000
            }

            public void Write(FileWriter writer)
            {

            }*/
        }

        public class Model
        {
            public void Read(FileReader reader)
            {

            }

            public void Write(FileWriter writer)
            {

            }
        }
    }
}
