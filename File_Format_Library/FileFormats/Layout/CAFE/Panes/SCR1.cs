using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class SCR1 : PAN1
    {
        public override string Signature { get; } = "scr1";

        public SCR1() : base()
        {

        }

        public SCR1(FileReader reader, Header header) : base(reader, header)
        {

        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
            base.Write(writer, header);
        }
    }

}
