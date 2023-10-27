using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class Huffman_WII
    {
        //Ported from 
        //https://github.com/Barubary/dsdecmp/blob/4ddd87206bacf4ce7d803b40ff3bd2663327b083/CSharp/DSDecmp/Program.cs#L417
        //(Modified to use byte arrays instead of file paths)
        //Copyright (c) 2010 Nick Kraayenbrink
        //
        //Permission is hereby granted, free of charge, to any person obtaining a copy
        //of this software and associated documentation files (the "Software"), to deal
        //in the Software without restriction, including without limitation the rights
        //to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        //copies of the Software, and to permit persons to whom the Software is
        //furnished to do so, subject to the following conditions:
        //
        //The above copyright notice and this permission notice shall be included in
        //all copies or substantial portions of the Software.
        //
        //THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        //IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        //FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        //AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        //LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        //OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
        //THE SOFTWARE.
        public static byte[] DecompressHuffman(byte[] input, int decompSize)
        {

            FileReader br = new FileReader(new MemoryStream(input), true);

            byte firstByte = br.ReadByte();

            int dataSize = firstByte & 0x0F;

            if ((firstByte & 0xF0) != 0x20)
                throw new InvalidDataException(String.Format("Invalid huffman comressed file; invalid tag {0:x}", firstByte));

            //Console.WriteLine("Data size: {0:x}", dataSize);
            if (dataSize != 8 && dataSize != 4)
                throw new InvalidDataException(String.Format("Unhandled dataSize {0:x}", dataSize));

            int decomp_size = 0;
            for (int i = 0; i < 3; i++)
            {
                decomp_size |= br.ReadByte() << (i * 8);
            }

            byte treeSize = br.ReadByte();
            HuffTreeNode.maxInpos = 4 + (treeSize + 1) * 2;

            Console.WriteLine("Tree Size: {0:x}", treeSize);
            Console.WriteLine("Tee end: 0x{0:X}", HuffTreeNode.maxInpos);

            HuffTreeNode rootNode = new HuffTreeNode();
            rootNode.parseData(br);

            br.BaseStream.Position = 4 + (treeSize + 1) * 2; // go to start of coded bitstream.
            // read all data
            uint[] indata = new uint[(br.BaseStream.Length - br.BaseStream.Position) / 4];
            for (int i = 0; i < indata.Length; i++)
                indata[i] = br.ReadUInt32();

            long curr_size = 0;
            decomp_size *= dataSize == 8 ? 1 : 2;
            byte[] outdata = new byte[decomp_size];

            int idx = -1;
            string codestr = "";
            LinkedList<byte> code = new LinkedList<byte>();
            int value;

            // Overwriting the likely nonsense read with the proper intended texture size.
            decomp_size = decompSize;

            while (curr_size < decomp_size)
            {
                try
                {
                    string newstr = uint_to_bits(indata[++idx]);
                    codestr += newstr;
                    Console.WriteLine("next uint: " + newstr);
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new IndexOutOfRangeException("not enough data.", e);
                }
                while (codestr.Length > 0 && curr_size < decompSize) // Need to force stop at the max decompSize, otherwise we get IndexOutOfRange Exceptions.
                {
                    code.AddFirst(byte.Parse(codestr[0] + ""));
                    //Console.Write(code.First.Value);
                    codestr = codestr.Remove(0, 1);
                    if (rootNode.getValue(code.Last, out value))
                    {
                        //Console.WriteLine(" -> "+value.ToString("X"));
                        try
                        {
                            outdata[curr_size++] = (byte)value;
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            if (code.First.Value != 0)
                                throw ex;
                        }
                        code.Clear();
                    }
                }
            }

            br.Close();

            byte[] realout;
            if (dataSize == 4)
            {
                realout = new byte[decomp_size / 2];
                for (int i = 0; i < decomp_size / 2; i++)
                {
                    if ((outdata[i * 2] & 0xF0) > 0
                        || (outdata[i * 2 + 1] & 0xF0) > 0)
                        throw new Exception("first 4 bits of data should be 0 if dataSize = 4");
                    realout[i] = (byte)((outdata[i * 2] << 4) | outdata[i * 2 + 1]);
                }
            }
            else
            {
                realout = outdata;
            }
            Console.WriteLine("Huffman decompressed.");
            return realout;
        }

        private static string byte_to_bits(byte b)
        {
            string o = "";
            for (int i = 0; i < 8; i++)
                o = (((b & (1 << i)) >> i) & 1) + o;
            return o;
        }
        private static string uint_to_bits(uint u)
        {
            string o = "";
            for (int i = 3; i > -1; i--)
                o += byte_to_bits((byte)((u & (0xFF << (i * 8))) >> (i * 8)));
            return o;
        }
    }

    //Ported from 
    //https://github.com/Barubary/dsdecmp/blob/4ddd87206bacf4ce7d803b40ff3bd2663327b083/CSharp/DSDecmp/Program.cs#L1367
    class HuffTreeNode
    {
        internal static int maxInpos = 0;
        internal HuffTreeNode node0, node1;
        internal int data = -1; // [-1,0xFF]
        /// <summary>
        /// To get a value, provide the last node of a list of bytes &lt; 2. 
        /// the list will be read from back to front.
        /// </summary>
        internal bool getValue(LinkedListNode<byte> code, out int value)
        {
            value = data;
            if (code == null)
                return node0 == null && node1 == null && data >= 0;

            if (code.Value > 1)
                throw new Exception(String.Format("the list should be a list of bytes < 2. got:{0:g}", code.Value));

            byte c = code.Value;
            bool retVal;
            HuffTreeNode n = c == 0 ? node0 : node1;
            retVal = n != null && n.getValue(code.Previous, out value);
            return retVal;
        }

        internal int this[string code]
        {
            get
            {
                LinkedList<byte> c = new LinkedList<byte>();
                foreach (char ch in code)
                    c.AddFirst((byte)ch);
                int val = 1;
                return this.getValue(c.Last, out val) ? val : -1;
            }
        }

        internal void parseData(BinaryReader br)
        {
            /*
             * Tree Table (list of 8bit nodes, starting with the root node)
                     Root Node and Non-Data-Child Nodes are:
                       Bit0-5   Offset to next child node,
                                Next child node0 is at (CurrentAddr AND NOT 1)+Offset*2+2
                                Next child node1 is at (CurrentAddr AND NOT 1)+Offset*2+2+1
                       Bit6     Node1 End Flag (1=Next child node is data)
                       Bit7     Node0 End Flag (1=Next child node is data)
                     Data nodes are (when End Flag was set in parent node):
                       Bit0-7   Data (upper bits should be zero if Data Size is less than 8)
             */
            this.node0 = new HuffTreeNode();
            this.node1 = new HuffTreeNode();
            long currPos = br.BaseStream.Position;
            byte b = br.ReadByte();
            long offset = b & 0x3F;
            bool end0 = (b & 0x80) > 0, end1 = (b & 0x40) > 0;

            // parse data for node0
            br.BaseStream.Position = (currPos - (currPos & 1)) + offset * 2 + 2;
            if (br.BaseStream.Position < maxInpos)
            {
                if (end0)
                    node0.data = br.ReadByte();
                else
                    node0.parseData(br);
            }

            // parse data for node1
            br.BaseStream.Position = (currPos - (currPos & 1)) + offset * 2 + 2 + 1;
            if (br.BaseStream.Position < maxInpos)
            {
                if (end1)
                    node1.data = br.ReadByte();
                else
                    node1.parseData(br);
            }

            // reset position
            br.BaseStream.Position = currPos;
        }

        public override string ToString()
        {
            if (data < 0 && node0 != null && node1 != null)
                return "<" + node0.ToString() + ", " + node1.ToString() + ">";
            else
                return String.Format("[{0:x}]", data);
        }

        internal int Depth
        {
            get
            {
                if (data < 0)
                    return 0;
                else
                    return 1 + Math.Max(node0.Depth, node1.Depth);
            }
        }
    }
}
