using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class ALI1 : PAN1
    {
        public override string Signature { get; } = "ali1";

        public ALI1() : base()
        {

        }

        public ALI1(FileReader reader, Header header) : base(reader, header)
        {

        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
            base.Write(writer, header);
        }
    }
}
