using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.Revolution
{
    public class BND1 : PAN1, IBoundryPane
    {
        public override string Signature { get; } = "bnd1";

        public BND1() : base()
        {

        }

        public BND1(BxlytHeader header, string name) : base()
        {
            LoadDefaults();
            Name = name;
        }

        public BND1(FileReader reader, BxlytHeader header) : base(reader, header)
        {

        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
            base.Write(writer, header);
        }
    }
}
